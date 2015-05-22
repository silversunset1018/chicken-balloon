#import "FASMovieMaker.h"
#import "UnityAppController.h"

// Interface
static FASMovieMaker *movieMakerInstance = nil;

extern "C"
{
    bool _Init()
    {
        UIView *view = (UIView*)[GetAppController() unityView];
        movieMakerInstance = [[FASMovieMaker movieMakerWithCaptureView:view withAudio:true] retain];
        return movieMakerInstance != nil;
    }
    
    bool _StartRecording()
    {
        //NSLog(@"_StartRecording");
        if (!movieMakerInstance)
            return false;
        return [movieMakerInstance startRecording];
    }
    
    bool _StartRecordingWithAllowCopy(BOOL copyToCameraRoll)
    {
        //NSLog(@"_StartRecording");
        if (!movieMakerInstance)
            return false;
        
        return [movieMakerInstance startRecordingWithAllowCopy:copyToCameraRoll];
    }
    
    void _StopRecording()
    {
        //NSLog(@"_StopRecording");
        [movieMakerInstance stopRecording];
    }
}

