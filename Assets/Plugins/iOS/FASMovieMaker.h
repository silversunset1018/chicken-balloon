//
//  FASMovieMaker.h
//  fresvii-sdk-ios
//
//  Created by Ian Sabine on 11/14/14.
//  Copyright (c) 2014 Fresvii Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface FASMovieMaker : NSObject

@property (nonatomic, setter = setFrameInterval:) int frameInterval;

+ (FASMovieMaker *) movieMakerWithCaptureView:(UIView*)view withAudio:(bool)audio;

- (FASMovieMaker*) initWithCaptureView:(UIView*)view withAudio:(bool)audio;

- (bool) isRecording;
- (bool) startRecording;
- (bool) startRecordingWithAllowCopy:(bool)copyToCameraRoll;
- (void) stopRecording;

@end
