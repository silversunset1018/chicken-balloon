//
//  FASMovieMaker.m
//  fresvii-sdk-ios
//
//  Created by Ian Sabine on 11/14/14.
//  Copyright (c) 2014 Fresvii Inc. All rights reserved.
//

#include "FASMovieMaker.h"
#import <AVFoundation/AVFoundation.h>
#import <Foundation/NSDateFormatter.h>
#import "UnityAppController.h"
#include "Unity/GlesHelper.h"
#include <arm_neon.h>
#include <OpenGLES/ES3/gl.h>

@interface FASMovieMaker() <AVCaptureAudioDataOutputSampleBufferDelegate>

@property (nonatomic) bool						initialized;
@property (nonatomic) bool						audio;
@property (nonatomic) bool						recording;
@property (nonatomic) bool						stopRequested;
@property (nonatomic) bool						discardFirstFrame;

@property (nonatomic) void						*videoLatchBuffer;
@property (nonatomic) CMTime					firstAudioTimeStamp;
@property (nonatomic) NSDictionary				*audioSettings;
@property (nonatomic) CGColorSpaceRef			colorSpace;
@property (nonatomic) NSDate					*startedAt;
@property (nonatomic) UIView					*captureView;
@property (nonatomic) NSDateFormatter			*dateFormatter;
@property (nonatomic) CADisplayLink				*displayLink;
@property (nonatomic) int						frameWidth;
@property (nonatomic) int						frameHeight;
@property (nonatomic) int						screenHeight;

@property (nonatomic) AVAssetWriter				*movieWriter;
@property (nonatomic) AVAssetWriterInput		*audioInput;
@property (nonatomic) AVAssetWriterInput		*videoInput;
@property (nonatomic) AVAssetWriterInputPixelBufferAdaptor *videoAdaptor;

@property (nonatomic) AVCaptureDeviceInput		*audioCaptureInput;
@property (nonatomic) AVCaptureAudioDataOutput	*audioCaptureOutput;
@property (nonatomic) AVCaptureSession			*captureSession;

@property (nonatomic) BOOL allowCopy;

@end


@implementation FASMovieMaker

+ (FASMovieMaker *) movieMakerWithCaptureView:(UIView*)view withAudio:(bool)withAudio
{
    FASMovieMaker *movieMaker = [[FASMovieMaker alloc] initWithCaptureView:view withAudio:withAudio];
    return movieMaker;
}

- (FASMovieMaker *) initWithCaptureView:(UIView*)view withAudio:(bool)withAudio
{
    // Initialization
    
    _captureView = view;
    [_captureView retain];
    
    _frameInterval = 2;	// default to 30Hz
    float scale = [[UIScreen mainScreen] scale];
    CGSize size = _captureView.frame.size;
    _frameWidth = ((int)(size.width * scale) + 15) & -16;
    _frameHeight = ((int)(size.height * scale) + 15) & -16;
    CGRect rect = [UIScreen mainScreen].bounds;
    _screenHeight = (int)rect.size.height;
    NSLog(@"W=%d, H=%d, S=%d", _frameWidth, _frameHeight, _screenHeight);
    
    _movieWriter = nil;
    _audioInput = nil;
    _videoInput = nil;
    _videoAdaptor = nil;
    
    _recording = false;
    _startedAt = nil;
    
    _audioSettings = nil;
    
    _colorSpace = CGColorSpaceCreateDeviceRGB();
    
    _audio = withAudio;
    if (_audio)
        [self setUpAudioCapture];
    
    NSLog(@"ScreenCapture initialized");
    
    _initialized = true;
    return self;
}

- (void) setFrameInterval:(int)value
{
    if (value > 0)
        _frameInterval = value;
}

- (NSURL *) tempFileURL
{
    if (!_dateFormatter)
    {
        _dateFormatter = [[[NSDateFormatter alloc] init] retain];
        [_dateFormatter setDateFormat:@"dd-MM-yyyy HH:mm:ss"];
    }
    NSString *fname = [_dateFormatter stringFromDate:[NSDate date]];
    
    //NSString* outputPath = [[NSString alloc] initWithFormat:@"%@/%@.mp4", [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) objectAtIndex:0], fname];
    NSString* outputPath = [[NSString alloc] initWithFormat:@"%@/%@.mp4", NSTemporaryDirectory(), fname];

    NSLog(@"%@", outputPath);
    
    NSURL* outputURL = [[NSURL alloc] initFileURLWithPath:outputPath];
    NSFileManager* fileManager = [NSFileManager defaultManager];
    if ([fileManager fileExistsAtPath:outputPath])
    {
        NSError* error;
        if ([fileManager removeItemAtPath:outputPath error:&error] == NO) {
            NSLog(@"Could not delete old recording file at path:  %@", outputPath);
        }
    }
    
    return outputURL;
}

- (bool) setUpWriter
{
    // get the first frame of video latched in
    if (_videoLatchBuffer == nil)
    {
        _videoLatchBuffer = malloc(_frameWidth * _frameHeight * 4);
    }
    dispatch_async(dispatch_get_main_queue(), ^
                   {
                       [self latchNextVideoFrame];
                   });
    
    NSError *error = nil;
    _movieWriter = [[[AVAssetWriter alloc] initWithURL:[self tempFileURL] fileType:AVFileTypeMPEG4 error:&error] retain];
    //_movieWriter = [[[AVAssetWriter alloc] initWithURL:[self tempFileURL] fileType:AVFileTypeQuickTimeMovie error:&error] retain];
    if (!_movieWriter)
    {
        NSParameterAssert(_movieWriter);
        return false;
    }
    _movieWriter.shouldOptimizeForNetworkUse = YES;
    
    NSDictionary* videoCompressionProps = [NSDictionary dictionaryWithObjectsAndKeys:
                                           [NSNumber numberWithFloat:1024.0f*1024.0f], AVVideoAverageBitRateKey,
                                           [NSNumber numberWithInt:60/_frameInterval], AVVideoExpectedSourceFrameRateKey,
                                           nil ];
    
    NSDictionary* videoSettings = [NSDictionary dictionaryWithObjectsAndKeys:
                                   AVVideoCodecH264, AVVideoCodecKey,
                                   [NSNumber numberWithInt:_frameWidth], AVVideoWidthKey,
                                   [NSNumber numberWithInt:_frameHeight], AVVideoHeightKey,
                                   videoCompressionProps, AVVideoCompressionPropertiesKey,
                                   nil];
    NSLog(@"canApplyOutputSettings");
    NSParameterAssert([_movieWriter canApplyOutputSettings:videoSettings forMediaType:AVMediaTypeVideo]);
    NSLog(@"assetWriterInputWithMediaType video");
    _videoInput = [[AVAssetWriterInput assetWriterInputWithMediaType:AVMediaTypeVideo outputSettings:videoSettings] retain];
    if (!_videoInput)
    {
        _movieWriter = nil;
        NSParameterAssert(_videoInput);
        return false;
    }
    _videoInput.expectsMediaDataInRealTime = YES;
    
    NSLog(@"canAddInput video");
    NSParameterAssert([_movieWriter canAddInput:_videoInput]);
    [_movieWriter addInput:_videoInput];
    
    NSDictionary* videoBufferAttributes = [NSDictionary dictionaryWithObjectsAndKeys:
                                           [NSNumber numberWithInt:kCVPixelFormatType_32ARGB], kCVPixelBufferPixelFormatTypeKey, nil];
    _videoAdaptor = [[AVAssetWriterInputPixelBufferAdaptor assetWriterInputPixelBufferAdaptorWithAssetWriterInput:_videoInput sourcePixelBufferAttributes:videoBufferAttributes] retain];
    
    if (_audio)
    {
        NSLog(@"assetWriterInputWithMediaType audio");
        _audioInput = [[AVAssetWriterInput assetWriterInputWithMediaType:AVMediaTypeAudio outputSettings:_audioSettings] retain];
        if (!_audioInput)
        {
            _movieWriter = nil;
            _videoInput = nil;
            _videoAdaptor = nil;
            NSParameterAssert(_audioInput);
            return false;
        }
        _audioInput.expectsMediaDataInRealTime = YES;
        
        NSLog(@"canAddInput audio");
        NSParameterAssert([_movieWriter canAddInput:_audioInput]);
        [_movieWriter addInput:_audioInput];
    }
    
    // the first frame looks jittery, so just discard it
    _discardFirstFrame = true;
    
    return true;
}

- (bool) isRecording
{
    return _recording;
}

- (bool) startRecordingWithAllowCopy:(bool)allowCopy
{
    self.allowCopy = allowCopy;
    
    return [self startRecording];
}

- (bool) startRecording
{
    if (!_initialized)
    {
        NSLog(@"Must call initWithView first");
        return false;
    }
    
    bool result = false;
    
    @synchronized(self)
    {
        if (!_recording)
        {
            result = [self setUpWriter];
            
            _recording = true;
            _stopRequested = false;
            
            dispatch_async( dispatch_get_global_queue( DISPATCH_QUEUE_PRIORITY_LOW, 0 ), ^
                           {
                               [_movieWriter startWriting];
                               
                               // need to create the display link on the main thread apparently
                               dispatch_async(dispatch_get_main_queue(), ^
                                              {
                                                  if (_audio)
                                                  {
                                                      float millisElapsed = [[NSDate date] timeIntervalSinceDate:_startedAt] * 1000.0;
                                                      [_movieWriter startSessionAtSourceTime:CMTimeAdd(_firstAudioTimeStamp, CMTimeMake(millisElapsed, 1000.0f))];
                                                  }
                                                  else
                                                  {
                                                      // if there's no audio, we still need to create these values, and start the video at time 0
                                                      _startedAt = [[NSDate date] retain];
                                                      _firstAudioTimeStamp = CMTimeMake(0, 1000);
                                                      [_movieWriter startSessionAtSourceTime:_firstAudioTimeStamp];
                                                  }
                                                  
                                                  _displayLink = [CADisplayLink displayLinkWithTarget:self
                                                                                             selector:@selector(addTimedVideoSample:)];
                                                  _displayLink.frameInterval = _frameInterval;
                                                  [_displayLink addToRunLoop:[NSRunLoop currentRunLoop] forMode:NSRunLoopCommonModes];
                                                  
                                                  NSLog(@"Started recording");
                                              });
                           });
        }
    }
    
    return result;
}

- (void) stopRecording
{
    @synchronized(self)
    {
        _stopRequested = true;
    }
}


- (void) completeMovie
{
    NSLog(@"completeMovie");
    
    [_displayLink invalidate];
    _displayLink = nil;
    
    [_videoInput markAsFinished];
    
    [_movieWriter finishWritingWithCompletionHandler:^(void)
     {
         dispatch_async(dispatch_get_main_queue(), ^
                        {
                            NSLog(@"Completed recording");
                            
                            //NSString* outputPath = [[NSString alloc] initWithFormat:@"%@/%@", [NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) objectAtIndex:0], _movieWriter.outputURL.lastPathComponent];
                            NSString* outputPath = [[NSString alloc] initWithFormat:@"%@/%@", NSTemporaryDirectory(), _movieWriter.outputURL.lastPathComponent];
                            
                            const char *str = [outputPath UTF8String];
                            assert(str);
                            char *copyStr = (char *)malloc(strlen(str)+1);
                            assert(copyStr);
                            strcpy(copyStr, str);
                            
                            UnitySendMessage("FASVideoMaker", "OnCompleted", copyStr);
                            
                            if(self.allowCopy)
                                UISaveVideoAtPathToSavedPhotosAlbum(outputPath, nil, @selector(videoCopied:didFinishSavingWithError:contextInfo:), nil);
                            
                            //cleanup
                            _movieWriter = nil;
                            _audioInput = nil;
                            _videoInput = nil;
                            _videoAdaptor = nil;
                            
                            _recording = false;
                        });
     }];
}

- (void) videoCopied:(NSString *)videoPath didFinishSavingWithError:(NSError *)error contextInfo:(void *)contextInfo
{
    // remove movie from documents
    /*NSFileManager* fileManager = [NSFileManager defaultManager];
     if ([fileManager fileExistsAtPath:videoPath])
     {
     NSError* error;
     if ([fileManager removeItemAtPath:videoPath error:&error] == NO) {
     NSLog(@"Could not delete old recording file at path:  %@", videoPath);
     }
     }*/
}

- (void) latchNextVideoFrame
{
    // use glReadPixels to read the current buffer (MUST be called by the main thread or you just get black pixels)
    glReadPixels(0, 0, _frameWidth, _frameHeight, GL_RGBA, GL_UNSIGNED_BYTE, _videoLatchBuffer);
}

- (void)addTimedVideoSample:(CADisplayLink *)displayLink
{
    if (_recording)
    {
        if (_stopRequested)
            [self completeMovie];
        else if ([_videoInput isReadyForMoreMediaData])
        {
            if (_discardFirstFrame)
                _discardFirstFrame = false;
            else
                [self addVideoSample];
        }
    }
}

- (void) addVideoSample
{
    CVPixelBufferRef sampleBuffer = NULL;
    
    CVReturn status = CVPixelBufferPoolCreatePixelBuffer(NULL, _videoAdaptor.pixelBufferPool, &sampleBuffer);
    NSParameterAssert(status == kCVReturnSuccess && sampleBuffer != NULL);
    
    CVPixelBufferLockBaseAddress(sampleBuffer, 0);
    void *pxdata = CVPixelBufferGetBaseAddress(sampleBuffer);
    NSParameterAssert(pxdata != NULL);
    
    // copy latched buffer and byte swap for correct colors
    Byte *p = _videoLatchBuffer + (_frameHeight * _frameWidth * 4);
    Byte *p1 = pxdata;
    for (int y = _frameHeight; y; --y)
    {
        p -= _frameWidth * 4;
        Byte* p0 = p;
#if 1
        // NEON version
        uint8x8_t rgba[5];
        rgba[0] = vdup_n_u8(0);
        for (int x = _frameWidth >> 3; x; --x)
        {
            // load into elements 1,2,3,4
            *(uint8x8x4_t*)&rgba[1]  = vld4_u8(p0);	// load and deinterleave rgba
            // save elements 0,1,2,3
            vst4_u8(p1, *(uint8x8x4_t*)&rgba[0]);	// store interleaved
            p0 += 8*4;
            p1 += 8*4;
        }
#else
        for (int x = _frameWidth; x; --x)
        {
            p1[0] = 0xFF;
            p1[1] = p0[0];
            p1[2] = p0[1];
            p1[3] = p0[2];
            p0 += 4;
            p1 += 4;
        }
#endif
    }
    
    dispatch_async(dispatch_get_main_queue(), ^
                   {
                       [self latchNextVideoFrame];
                   });
    
    CVPixelBufferUnlockBaseAddress(sampleBuffer, 0);
    
    float millisElapsed = [[NSDate date] timeIntervalSinceDate:_startedAt] * 1000.0;
    CMTime time = CMTimeAdd(_firstAudioTimeStamp, CMTimeMake((int)millisElapsed, 1000));
    [_videoAdaptor appendPixelBuffer:sampleBuffer withPresentationTime:time];
    
    CVPixelBufferRelease(sampleBuffer);
}

- (bool) setUpAudioCapture
{
    NSError *error;
    
    AVCaptureDevice *device = [[AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeAudio] retain];
    if (device && device.connected)
        NSLog(@"Connected Device: %@", device.localizedName);
    else
    {
        NSLog(@"AVCaptureDevice Failed");
        return FALSE;
    }
    
    // add device inputs
    _audioCaptureInput = [[AVCaptureDeviceInput deviceInputWithDevice:device error:&error] retain];
    if (!_audioCaptureInput)
    {
        NSLog(@"AVCaptureDeviceInput Failed");
        return FALSE;
    }
    if (error)
    {
        NSLog(@"%@", error);
        return FALSE;
    }
    
    // add output for audio
    _audioCaptureOutput = [[[AVCaptureAudioDataOutput alloc] init] retain];
    if (!_audioCaptureOutput)
    {
        NSLog(@"AVCaptureMovieFileOutput Failed");
        return FALSE;
    }
    dispatch_queue_t audioCaptureQueue = dispatch_queue_create("AudioCaptureQueue", NULL);
    [_audioCaptureOutput setSampleBufferDelegate:self queue:audioCaptureQueue];
    
    _captureSession = [[[AVCaptureSession alloc] init] retain];
    if (!_captureSession)
    {
        NSLog(@"AVCaptureSession Failed");
        return FALSE;
    }
    _captureSession.sessionPreset = AVCaptureSessionPresetMedium;
    if ([_captureSession canAddInput:_audioCaptureInput])
        [_captureSession addInput:_audioCaptureInput];
    else
    {
        NSLog(@"Failed to add input device to capture session");
        return FALSE;
    }
    if ([_captureSession canAddOutput:_audioCaptureOutput])
        [_captureSession addOutput:_audioCaptureOutput];
    else
    {
        NSLog(@"Failed to add output device to capture session");
        return FALSE;
    }
    
    _audioSettings = [[_audioCaptureOutput recommendedAudioSettingsForAssetWriterWithOutputFileType:AVFileTypeQuickTimeMovie] retain];
    
    [_captureSession startRunning];
    
    NSLog(@"Audio capture session running");
    
    return TRUE;
}

- (void)captureOutput:(AVCaptureOutput *)captureOutput didOutputSampleBuffer:(CMSampleBufferRef)sampleBuffer fromConnection:(AVCaptureConnection *)connection
{
    if (captureOutput == _audioCaptureOutput)
    {
        if (_startedAt == nil)
        {
            _startedAt = [[NSDate date] retain];
            _firstAudioTimeStamp = CMSampleBufferGetPresentationTimeStamp(sampleBuffer);
        }
        
        if (_recording && !_stopRequested && [_audioInput isReadyForMoreMediaData])
        {
            [_audioInput appendSampleBuffer:sampleBuffer];
        }
    }
}

@end
