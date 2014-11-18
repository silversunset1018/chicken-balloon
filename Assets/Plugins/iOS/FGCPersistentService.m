//
//  FGCKeychainService.m
//  fresvii-sdk-ios
//
//  Created by katsuhito.matsushima on 2/16/14.
//  Copyright (c) 2014 Fresvii Inc. All rights reserved.
//

#import "FGCPersistentService.h"
#import "FGCConstraint.h"
#import "NSError+FGCFresvii.h"

static NSString *const _kUserCertifications     = @"FGCUserCertificationsPersistentKey";
static NSString *const _kUserIdentifier         = @"FGCUserIdentifierPersistentKey";
static NSString *const _kUserToken              = @"FGCUserTokenPersistentKey";
static NSString *const _kSessionToken           = @"FGCSessionTOkenPersistentKey";
static NSString *const _kSNSAccountIdentifier   = @"FGCSNSAccountIdentifierPersistentKey";
static NSString *const _kExpireDate             = @"FGCExpireDatePersistentKey";
static NSString *const _kDataLastsync           = @"FGCDataLastsyncPersistentKey";
static NSString *const _kThreadLastsync         = @"FGCThreadLastsyncPersistentKey";
static NSString *const _kCommentLastsync        = @"FGCCommentLastsyncPersistentKey";

@interface FGCPersistentService ()

- (NSMutableDictionary *)_queryDictionaryForKey:(NSString *)key;
- (BOOL)_insertData:(NSData *)data
             forKey:(NSString *)key
              error:(NSError **)error;
- (BOOL)_updateData:(NSData *)data
             forKey:(NSString *)key
              error:(NSError **)error;
- (BOOL)_deleteItemWithKey:(NSString *)key
                     error:(NSError **)error;

@end

@implementation FGCPersistentService
{
    NSUserDefaults *_userDefaults;
}

#pragma mark - Initialize

+ (instancetype)sharedService
{
    static FGCPersistentService *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[FGCPersistentService alloc] init];
    });
    
    return sharedInstance;
}

- (id)init
{
    self = [super init];
    
    if (self)
    {
        _userDefaults = [NSUserDefaults standardUserDefaults];
    }
    
    return self;
}

#pragma mark - Private methods.

- (NSMutableDictionary *)_queryDictionaryForKey:(NSString *)key
{
    NSMutableDictionary *query = [NSMutableDictionary dictionary];
    query[(__bridge id)kSecClass] = (__bridge id)kSecClassGenericPassword;
    query[(__bridge id)kSecAttrAccessible] = (__bridge id)kSecAttrAccessibleAfterFirstUnlock;
    
    NSData *encodedIdentifier = [key dataUsingEncoding:NSUTF8StringEncoding];
    query[(__bridge id)kSecAttrAccount] = encodedIdentifier;
    
    return query;
}

- (id)_getDataWithKey:(NSString *)key
{
    NSMutableDictionary *query = [self _queryDictionaryForKey:key];
    query[(__bridge id)kSecMatchLimit] = (__bridge id)kSecMatchLimitOne;
    query[(__bridge id)kSecReturnData] = (__bridge id)kCFBooleanTrue;
    
    CFTypeRef cfResult;
    OSStatus status = SecItemCopyMatching((__bridge CFDictionaryRef)query, &cfResult);
    if (status != noErr)
    {
        return nil;
    }
    
    id data = CFBridgingRelease(cfResult);
    if (![data isKindOfClass:[NSData class]])
    {
        return nil;
    }
    
    return data;
}

- (BOOL)_insertData:(NSData *)data
             forKey:(NSString *)key
              error:(NSError **)error
{
    NSMutableDictionary *query = [self _queryDictionaryForKey:key];
    query[(__bridge id)kSecValueData] = data;
    
    OSStatus status = SecItemAdd((__bridge CFDictionaryRef)query, NULL);
    if (status != noErr)
    {
        if (error)
        {
            *error = [NSError fgc_errorForKeychain];
        }
        return NO;
    }
    
    return YES;
}

- (BOOL)_updateData:(NSData *)data
             forKey:(NSString *)key
              error:(NSError **)error
{
    NSMutableDictionary *query = [self _queryDictionaryForKey:key];
    query[(__bridge id)kSecValueData] = data;
    
    NSMutableDictionary *updateQuery = [NSMutableDictionary dictionary];
    updateQuery[(__bridge id)kSecValueData] = data;
    
    OSStatus status = SecItemUpdate((__bridge CFDictionaryRef)query, (__bridge CFDictionaryRef)updateQuery);
    if (status != noErr)
    {
        if (error)
        {
            *error = [NSError fgc_errorForKeychain];
        }
        return NO;
    }
    
    return YES;
}

- (BOOL)_deleteItemWithKey:(NSString *)key
                     error:(NSError **)error
{
    NSMutableDictionary *query = [self _queryDictionaryForKey:key];
    
    OSStatus status = SecItemDelete((__bridge CFDictionaryRef)query);
    if (status != noErr)
    {
        if (error)
        {
            *error = [NSError fgc_errorForKeychain];
        }
        return NO;
    }
    
    return YES;
}

#pragma mark - Public methods.

- (BOOL)deleteAllItemsWithError:(NSError **)error
{
    NSMutableDictionary *query = [NSMutableDictionary dictionary];
    query[(__bridge id)kSecClass] = (__bridge id)kSecClassGenericPassword;
    
    OSStatus status = SecItemDelete((__bridge CFDictionaryRef)query);
    if (status != noErr)
    {
        if (error)
        {
            *error = [NSError fgc_errorForKeychain];
        }
        return NO;
    }
    
    return YES;
}

#pragma mark -- User Certifications.

- (BOOL)addUserIdentifier:(NSString *)useIdentifier
                userToken:(NSString *)userToken
                    error:(NSError **)error
{
    // Always override at this time.
    // Change additional operation from override when we cope with multi-user.
    NSArray *certifications = @[
                                @{
                                  @"userIdentifier" : useIdentifier,
                                  @"userToken" : userToken
                                  }
                                ];
    NSData *data = [NSKeyedArchiver archivedDataWithRootObject:certifications];
    if ([self userCertifications])
    {
        // Update
        return [self _updateData:data
                          forKey:_kUserCertifications
                           error:error];
    }
    else
    {
        // Insert
        return [self _insertData:data
                          forKey:_kUserCertifications
                           error:error];
    }
}

- (BOOL)deleteUserCertificationWithUserIdentifer:(NSString *)useIdentifier
                                           error:(NSError **)error
{
    return [self _deleteItemWithKey:_kUserCertifications
                              error:error];
}

- (NSArray *)userCertifications
{
    id data = [self _getDataWithKey:_kUserCertifications];
    if (!data)
    {
        return nil;
    }
    
    return [NSKeyedUnarchiver unarchiveObjectWithData:data];
}

#pragma mark -- Current User Identifier.

- (BOOL)addCurrentUserIdentifier:(NSString *)userIdentifier
                           error:(NSError **)error
{
    NSData *data = [userIdentifier dataUsingEncoding:NSUTF8StringEncoding];
    if ([self currentUserIdentifier])
    {
        // Update
        return [self _updateData:data
                          forKey:_kUserIdentifier
                           error:error];
    }
    else
    {
        // Insert
        return [self _insertData:data
                          forKey:_kUserIdentifier
                           error:error];
    }
}

- (BOOL)deleteCurrentUserIdentiferWithError:(NSError **)error
{
    return [self _deleteItemWithKey:_kUserIdentifier
                              error:error];
}

- (NSString *)currentUserIdentifier;
{
    id data = [self _getDataWithKey:_kUserIdentifier];
    if (!data)
    {
        return nil;
    }
    
    return [[NSString alloc] initWithData:data
                                 encoding:NSUTF8StringEncoding];
}

#pragma mark -- Current User Token.

- (BOOL)addCurrentUserToken:(NSString *)userToken
                      error:(NSError **)error
{
    NSData *data = [userToken dataUsingEncoding:NSUTF8StringEncoding];
    if ([self currentUserToken])
    {
        // Update
        return [self _updateData:data
                          forKey:_kUserToken
                           error:error];
    }
    else
    {
        // Insert
        return [self _insertData:data
                          forKey:_kUserToken
                           error:error];
    }
}

- (BOOL)deleteCurrentUserTokenWithError:(NSError **)error
{
    return [self _deleteItemWithKey:_kUserToken
                              error:error];
}

- (NSString *)currentUserToken
{
    id data = [self _getDataWithKey:_kUserToken];
    if (!data)
    {
        return nil;
    }
    
    return [[NSString alloc] initWithData:data
                                 encoding:NSUTF8StringEncoding];
}

#pragma mark -- Session Token.

- (void)addSessionToken:(NSString *)sessionToken
{
    [_userDefaults setObject:sessionToken
                      forKey:_kSessionToken];
    [_userDefaults synchronize];
}

- (void)deleteSessionToken
{
    [_userDefaults removeObjectForKey:_kSessionToken];
}

- (NSString *)sessionToken
{
    return [_userDefaults objectForKey:_kSessionToken];
}

#pragma mark -- Expire date.

- (void)addExpireDate:(NSDate *)expireDate
{
    [_userDefaults setObject:expireDate
                      forKey:_kExpireDate];
    [_userDefaults synchronize];
}

- (void)deleteExpireDate
{
    [_userDefaults removeObjectForKey:_kExpireDate];
}

- (NSDate *)expireDate
{
    return [_userDefaults objectForKey:_kExpireDate];
}

#pragma mark -- Account identifier.

- (void)addSNSAccountIdentifier:(NSString *)identifier
{
    [_userDefaults setObject:identifier
                      forKey:_kSNSAccountIdentifier];
    [_userDefaults synchronize];
}

- (void)deleteSNSAccountIdentifier
{
    [_userDefaults removeObjectForKey:_kSNSAccountIdentifier];
}

- (NSString *)SNSAccountIdentifier
{
    return [_userDefaults objectForKey:_kSNSAccountIdentifier];
}

#pragma mark -- Data Lastsync.

- (void)addDataLastsync:(NSDate *)lastsync
{
    [_userDefaults setObject:lastsync
                      forKey:_kDataLastsync];
    [_userDefaults synchronize];
}

- (void)deleteDataLastsync
{
    [_userDefaults removeObjectForKey:_kDataLastsync];
}

- (NSDate *)dataLastsync
{
    NSDate *date = [_userDefaults objectForKey:_kDataLastsync];
    return date;
}

#pragma mark -- Thread Lastsync.

- (void)addThreadLastsync:(NSDate *)lastsync
{
    [_userDefaults setObject:lastsync
                      forKey:_kThreadLastsync];
    [_userDefaults synchronize];
}

- (void)deleteThreadLastsync
{
    [_userDefaults removeObjectForKey:_kThreadLastsync];
}

- (NSDate *)threadLastsync
{
    NSDate *date = [_userDefaults objectForKey:_kThreadLastsync];
    if (!date)
    {
        date = [NSDate dateWithTimeIntervalSince1970:0];
    }
    return date;
}

#pragma mark -- Comment Lastsync.

- (void)addCommentLastsync:(NSDate *)lastsync
{
    [_userDefaults setObject:lastsync
                      forKey:_kCommentLastsync];
    [_userDefaults synchronize];
}

- (void)deleteCommentLastsync
{
    [_userDefaults removeObjectForKey:_kCommentLastsync];
}

- (NSDate *)commentLastsync
{
    NSDate *date = [_userDefaults objectForKey:_kCommentLastsync];
    if (!date)
    {
        date = [NSDate dateWithTimeIntervalSince1970:0];
    }
    return date;
}

@end
