//
//  FASShareTwitter.m
//  appsteroid-ios
//
//  Created by katsuhito.matsushima on 1/16/15.
//  Copyright (c) 2015 Fresvii Inc. All rights reserved.
//

#import "FASShareTwitter.h"
#import <Social/Social.h>
#import <Accounts/Accounts.h>

@interface FASShareTwitter ()

@end

extern "C"
{
    void _FasShareTwitter(const char *text)
    {
        NSString* strText = [NSString stringWithCString:text encoding:NSUTF8StringEncoding];
        
        if ([SLComposeViewController isAvailableForServiceType:SLServiceTypeTwitter])
        {
            ACAccountStore *accountStore = [[ACAccountStore alloc] init];
        
            ACAccountType *accountType = [accountStore accountTypeWithAccountTypeIdentifier:ACAccountTypeIdentifierTwitter];
        
            [accountStore requestAccessToAccountsWithType:accountType
                                                  options:nil
                                               completion:^(BOOL granted, NSError *error)
             {
                 if (error)
                 {
                     dispatch_async(dispatch_get_main_queue(), ^
                                {
									UnitySendMessage("FASTwitterObserver", "OnTwitterSharingCompleted", "Error");
                                });
                     return;
                 }
             
                 if (granted)
                 {
                     NSArray *accountArray = [accountStore accountsWithAccountType:accountType];
                 
                     if (accountArray.count > 0)
                     {
                         NSURL *url = [NSURL URLWithString:@"https://api.twitter.com/1/statuses/update.json"];
                         
                         NSDictionary *params = @{@"status" : strText};
                     
                         SLRequest *request = [SLRequest requestForServiceType:SLServiceTypeTwitter
                                                                 requestMethod:SLRequestMethodPOST
                                                                           URL:url
                                                                    parameters:params];

                         [request setAccount:accountArray[0]];
                     
                         [request performRequestWithHandler:^(NSData *responseData, NSHTTPURLResponse *urlResponse, NSError *error)
                          {
							  if ([urlResponse statusCode] == 200) {
									UnitySendMessage("FASTwitterObserver", "OnTwitterSharingCompleted", "Done");
							  }
							  else{
									UnitySendMessage("FASTwitterObserver", "OnTwitterSharingCompleted", "Error");
							  }
                          }];
                     }
                 }
             }];
        }
        else
        {
			UnitySendMessage("FASTwitterObserver", "OnTwitterSharingCompleted", "Error");
        }
    }

	bool _FasShareTwitterEnable(){
	
		return [SLComposeViewController isAvailableForServiceType:SLServiceTypeTwitter];

	}
}
