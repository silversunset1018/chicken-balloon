//
//  FASShareFacebook.m
//  appsteroid-ios
//
//  Created by katsuhito.matsushima on 1/16/15.
//  Copyright (c) 2015 Fresvii Inc. All rights reserved.
//

#import "FASShareFacebook.h"
#import <Social/Social.h>
#import <Accounts/Accounts.h>

extern UIViewController *UnityGetGLViewController();

extern UIView *UnityGetGLView();

extern "C"
{
    void _FasShareFacebook(const char *text, const char *videoURL)
    {
        NSString* strText = [NSString stringWithCString: text encoding:NSUTF8StringEncoding];
        
        NSString* strULR = [NSString stringWithCString: videoURL encoding:NSUTF8StringEncoding];
        
        if ([SLComposeViewController isAvailableForServiceType:SLServiceTypeFacebook])
        {
            SLComposeViewController *composeViewController = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeFacebook];
            [composeViewController setInitialText:strText];
            [composeViewController addURL:[NSURL URLWithString:strULR]];
            [composeViewController setCompletionHandler:^(SLComposeViewControllerResult result)
             {
                 dispatch_async(dispatch_get_main_queue(), ^
                                {
                                    switch(result)
                                    {
                                        case SLComposeViewControllerResultCancelled:
                                        default:
                                            UnitySendMessage("FASFacebookObserver", "OnFacebookSharingCompleted", "Cancel");
                                            break;
                                            
                                        case SLComposeViewControllerResultDone:
                                            UnitySendMessage("FASFacebookObserver", "OnFacebookSharingCompleted", "Done");
                                            break;
                                    }
                                });
             }];
            
            [UnityGetGLViewController() presentViewController:composeViewController animated:YES completion:nil];
        }
        else
        {
            UnitySendMessage("FASFacebookObserver", "OnFacebookSharingCompleted", "Error");	
        }
    }

	bool _FasShareFacebookEnable(){
	
		return [SLComposeViewController isAvailableForServiceType:SLServiceTypeFacebook];

	}
}