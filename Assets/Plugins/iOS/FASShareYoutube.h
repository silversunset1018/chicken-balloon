//
//  FASShareYoutube.h
//  appsteroid-ios
//
//  Created by katsuhito.matsushima on 1/22/15.
//  Copyright (c) 2015 Fresvii Inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "FASGTMOAuth2Authentication.h"

@interface FASShareYoutube : NSObject

@property (nonatomic, retain) FASGTMOAuth2Authentication *auth;

+ (FASShareYoutube*) instance;
- (void)startLogin;
- (void)shareWithVideoData:(NSData *)videoData
                dataLength:(int)dataLength
                     title:(NSString *)title
               description:(NSString *)description
                     token:(NSString *)token;

@end
