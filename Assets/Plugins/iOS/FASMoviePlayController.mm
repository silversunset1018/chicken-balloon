#import "FASMoviePlayController.h"

extern UIViewController *UnityGetGLViewController();

extern UIView *UnityGetGLView();

char strUrl[512];

@implementation FASMoviePlayController

static FASMoviePlayController *pInstance = nil;

+ (FASMoviePlayController*) instance {
    @synchronized(self) {
        if(pInstance == nil) {
            pInstance = [[self alloc]init];
        }
    }
    return pInstance;
}

- (void)playMovie {
    
    NSString *str = [[NSString alloc] initWithUTF8String:strUrl];
    
    NSURL *url = [NSURL URLWithString:str];
    
    MPMoviePlayerViewController *vc = [[MPMoviePlayerViewController alloc] initWithContentURL:url];
    
    [vc.moviePlayer play];
    
    [UnityGetGLViewController() presentMoviePlayerViewControllerAnimated:vc];
    
    [vc release];
}

@end

extern "C"
{
    
    void _FasMoviePlay(const char *url)
    {
        strncpy(strUrl, url, sizeof(strUrl));
        
        [[FASMoviePlayController instance] playMovie];
    }
}