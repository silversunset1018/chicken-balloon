//
//  FGCConstraint.h
//  fresvii-sdk-ios
//
//  Created by katsuhito.matsushima on 2/11/14.
//  Copyright (c) 2014 Fresvii Inc. All rights reserved.
//

// Header Parameters.
static NSString *const kFGCAPIPlatform = @"iOS";
static NSString *const kFGCAPIVersion  = @"v1";
static NSString *const kFGCAPIFormat   = @"json";

// API Host Name.
#ifdef DEBUG
static NSString *const kFGCAPIHost = @"staging.api.fresvii.com";
#elif ADHOC
static NSString *const kFGCAPIHost = @"staging.api.fresvii.com";
#else
static NSString *const kFGCAPIHost = @"api.fresvii.com";
#endif

// API Protocol.
static NSString *const kFGCAPIProtocolHTTPS = @"https";
static NSString *const kFGCAPIProtocolWWS   = @"wws";

// API Resources.ログインアカウントに対するものはFGCAccountにまとめたほうがいいですかね？
static NSString *const kFGCAPIUsersPath                         = @"users";
static NSString *const kFGCAPIUsersIDPath                       = @"users/%@";
static NSString *const kFGCAPIAccountAuthenticatePath           = @"account/authenticate";
static NSString *const kFGCAPIAccountSNSAccountsPath            = @"account/sns_accounts";
static NSString *const kFGCAPIAccountSNSAccountsIDPath          = @"account/sns_accounts/%@";
static NSString *const kFGCAPIAccountUserPath                   = @"account/user";
static NSString *const kFGCAPINotificationTokensRegisterPath    = @"notification/token/register";
static NSString *const kFGCAPINotificationTokensUnregisterPath  = @"notification/token/unregister";
static NSString *const kFGCAPIForumThreadsPath                  = @"forum/threads";
static NSString *const kFGCAPIForumThreadsIDPath                = @"forum/threads/%@";
static NSString *const kFGCAPIForumThreadsSubscribePath         = @"forum/threads/%@/subscribe";
static NSString *const kFGCAPIForumThreadsUnsubscribePath       = @"forum/threads/%@/unsubscribe";
static NSString *const kFGCAPIForumThreadsCommentsPath          = @"forum/threads/%@/comments";
static NSString *const kFGCAPIForumCommentsPath                 = @"forum/comments/%@";
static NSString *const kFGCAPIForumCommentsLikePath             = @"forum/comments/%@/like";
static NSString *const kFGCAPIForumCommentsUnlikePath           = @"forum/comments/%@/unlike";
static NSString *const kFGCAPIDataPath                          = @"data";
static NSString *const kFGCAPIDataKeyNamePath                   = @"data/key/%@";
static NSString *const kFGCAPIDataKyesPath                      = @"data/keys";

// API Request Headers.
static NSString *const kFGCAPIHeaderAcceptKey           = @"Accept";
static NSString *const kFGCAPIHeaderAcceptValue         = @"application/vnd.fresvii-%@+%@";
static NSString *const kFGCAPIHeaderContentTypeKey      = @"Content-Type";
static NSString *const kFGCAPIHeaderTypeJsonValue       = @"application/json";
static NSString *const kFGCAPIHeaderApplicationIdKey    = @"X-Fresvii-Application-Id";
static NSString *const kFGCAPIHeaderSessionTokenKey     = @"X-Fresvii-Session-Token";
static NSString *const kFGCAPIHeaderDeviceIdKey         = @"X-Fresvii-Device-Id";
static NSString *const kFGCAPIHeaderDevicePlatformKey   = @"X-Fresvii-Device-Platform";

// API Methods.
static NSString *const kFGCAPIMethodGet    = @"GET";
static NSString *const kFGCAPIMethodPost   = @"POST";
static NSString *const kFGCAPIMethodDelete = @"DELETE";
static NSString *const kFGCAPIMethodPut    = @"PUT";
static NSString *const kFGCAPIMethodPatch  = @"PATCH";

// Bundle name
static NSString *const kFGCBundleName = @"Fresvii.bundle";

// Storyboard name
static NSString *const kFGCStoryboardForum          = @"FGCForum_iPhone";
static NSString *const kFGCStoryboardMyProfile      = @"FGCMyProfile_iPhone";
static NSString *const kFGCStoryboardLeaderboard    = @"FGCLeaderboard_iPhone";

// Thread name
static char *const kFGCQueueCoreData = "com.fresvii.fresvii-sdk-ios.coredata";
static char *const kFGCQueueImageCache = "com.fresvii.fresvii-sdk-ios.imagecache";