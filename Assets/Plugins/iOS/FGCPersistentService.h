//
//  FGCKeychainService.h
//  fresvii-sdk-ios
//
//  Created by katsuhito.matsushima on 2/16/14.
//  Copyright (c) 2014 Fresvii Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface FGCPersistentService : NSObject

+ (instancetype)sharedService;

// Flash all data.
- (BOOL)deleteAllItemsWithError:(NSError **)error;

// Store user certification infomation in keychain as array.
- (BOOL)addUserIdentifier:(NSString *)useIdentifier
                userToken:(NSString *)userToken
                    error:(NSError **)error;
- (BOOL)deleteUserCertificationWithUserIdentifer:(NSString *)useIdentifier
                                           error:(NSError **)error;
- (NSArray *)userCertifications;

// Current User Identifier.
- (BOOL)addCurrentUserIdentifier:(NSString *)useIdentifier
                           error:(NSError **)error;
- (BOOL)deleteCurrentUserIdentiferWithError:(NSError **)error;
- (NSString *)currentUserIdentifier;

// Current User Token.
- (BOOL)addCurrentUserToken:(NSString *)userToken
                      error:(NSError **)error;
- (BOOL)deleteCurrentUserTokenWithError:(NSError **)error;
- (NSString *)currentUserToken;

// Session Token.
- (void)addSessionToken:(NSString *)sessionToken;
- (void)deleteSessionToken;
- (NSString *)sessionToken;

// Session Expire Date.
- (void)addExpireDate:(NSDate *)expireDate;
- (void)deleteExpireDate;
- (NSDate *)expireDate;

// SNS Account Identifier.
- (void)addSNSAccountIdentifier:(NSString *)identifier;
- (void)deleteSNSAccountIdentifier;
- (NSString *)SNSAccountIdentifier;

// Data Lastsync.
- (void)addDataLastsync:(NSDate *)lastsync;
- (void)deleteDataLastsync;
- (NSDate *)dataLastsync;

// Thread Lastsync.
- (void)addThreadLastsync:(NSDate *)lastsync;
- (void)deleteThreadLastsync;
- (NSDate *)threadLastsync;

// Comment Lastsync.
- (void)addCommentLastsync:(NSDate *)lastsync;
- (void)deleteCommentLastsync;
- (NSDate *)commentLastsync;

@end
