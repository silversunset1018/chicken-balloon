//
//  NSError+FGCFresvii.h
//  fresvii-sdk-ios
//
//  Created by katsuhito.matsushima on 2/11/14.
//  Copyright (c) 2014 Fresvii Inc. All rights reserved.
//

@interface NSError (FGCFresvii)

// Parameter error.
+ (instancetype)fgc_errorForNoAppIdentifier;
+ (instancetype)fgc_errorForIncorrectAppIdentifier;
+ (instancetype)fgc_errorForNoUserIdentifier;
+ (instancetype)fgc_errorForIncorrectUserIdentifier;
+ (instancetype)fgc_errorForNoUserToken;
+ (instancetype)fgc_errorForIncorrectUserToken;
+ (instancetype)fgc_errorForNoSessionToken;
+ (instancetype)fgc_errorForIncorrectSessionToken;
+ (instancetype)fgc_errorForExpiredSessionToken;
+ (instancetype)fgc_errorForNoRequiredParameter;
+ (instancetype)fgc_errorForInvalidParameter:(NSString *)errorDetail;
+ (instancetype)fgc_errorForInvalidJSONFormat;

// Network error.
+ (instancetype)fgc_errorForNetworkUnknown;
+ (instancetype)fgc_errorForNetworkStatus:(NSInteger)httpStatusCode;

// Storage error.
+ (instancetype)fgc_errorForKeychain;

// Parser error.
+ (instancetype)fgc_errorForJSON;

@end
