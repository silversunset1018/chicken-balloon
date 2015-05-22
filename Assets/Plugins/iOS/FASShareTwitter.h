//
//  FASShareTwitter.h
//  appsteroid-ios
//
//  Created by katsuhito.matsushima on 1/16/15.
//  Copyright (c) 2015 Fresvii Inc. All rights reserved.
//

@interface FASShareTwitter : NSObject

+ (void)requestAccessWithCompletion:(void (^)(BOOL granted, NSError *error))completion;
+ (void)shareWithText:(NSString *)text
           completion:(void (^)(NSError *error))completion;

@end