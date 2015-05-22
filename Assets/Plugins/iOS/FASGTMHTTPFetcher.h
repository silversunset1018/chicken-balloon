#pragma once

#import <Foundation/Foundation.h>

#if defined(GTL_TARGET_NAMESPACE)
  // we're using target namespace macros
  #import "GTLDefines.h"
#elif defined(GDATA_TARGET_NAMESPACE)
  #import "GDataDefines.h"
#else
  #if TARGET_OS_IPHONE
    #ifndef FASGTM_FOUNDATION_ONLY
      #define FASGTM_FOUNDATION_ONLY 1
    #endif
    #ifndef FASGTM_IPHONE
      #define FASGTM_IPHONE 1
    #endif
  #endif
#endif

#if TARGET_OS_IPHONE && (__IPHONE_OS_VERSION_MAX_ALLOWED >= 40000)
  #define FASGTM_BACKGROUND_FETCHING 1
#endif

#ifndef FASGTM_ALLOW_INSECURE_REQUESTS
  // For builds prior to the iOS 8/10.10 SDKs, default to ignoring insecure requests for backwards
  // compatibility unless the project has smartly set FASGTM_ALLOW_INSECURE_REQUESTS explicitly.
  #if (!TARGET_OS_IPHONE && defined(MAC_OS_X_VERSION_10_10) && MAC_OS_X_VERSION_MAX_ALLOWED >= MAC_OS_X_VERSION_10_10) \
      || (TARGET_OS_IPHONE && defined(__IPHONE_8_0) && __IPHONE_OS_VERSION_MAX_ALLOWED >= __IPHONE_8_0)
    #define FASGTM_ALLOW_INSECURE_REQUESTS 0
  #else
    #define FASGTM_ALLOW_INSECURE_REQUESTS 1
  #endif
#endif

#if !defined(FASGTMBridgeFetcher)
  // These bridge macros should be identical in FASGTMHTTPFetcher.h and FASGTMSessionFetcher.h
  #if FASGTM_USE_SESSION_FETCHER
    // Macros to new fetcher class.
    #define FASGTMBridgeFetcher FASGTMSessionFetcher
    #define FASGTMBridgeFetcherService FASGTMSessionFetcherService
    #define FASGTMBridgeFetcherServiceProtocol FASGTMSessionFetcherServiceProtocol
    #define FASGTMBridgeAssertValidSelector FASGTMSessionFetcherAssertValidSelector
    #define FASGTMBridgeCookieStorage FASGTMSessionCookieStorage
    #define FASGTMBridgeCleanedUserAgentString FASGTMFetcherCleanedUserAgentString
    #define FASGTMBridgeSystemVersionString FASGTMFetcherSystemVersionString
    #define FASGTMBridgeApplicationIdentifier FASGTMFetcherApplicationIdentifier
    #define kFASGTMBridgeFetcherStatusDomain kFASGTMSessionFetcherStatusDomain
    #define kFASGTMBridgeFetcherStatusBadRequest kFASGTMSessionFetcherStatusBadRequest
  #else
    // Macros to old fetcher class.
    #define FASGTMBridgeFetcher FASGTMHTTPFetcher
    #define FASGTMBridgeFetcherService FASGTMHTTPFetcherService
    #define FASGTMBridgeFetcherServiceProtocol FASGTMHTTPFetcherServiceProtocol
    #define FASGTMBridgeAssertValidSelector FASGTMAssertSelectorNilOrImplementedWithArgs
    #define FASGTMBridgeCookieStorage FASGTMCookieStorage
    #define FASGTMBridgeCleanedUserAgentString FASGTMCleanedUserAgentString
    #define FASGTMBridgeSystemVersionString FASGTMSystemVersionString
    #define FASGTMBridgeApplicationIdentifier FASGTMApplicationIdentifier
    #define kFASGTMBridgeFetcherStatusDomain kFASGTMHTTPFetcherStatusDomain
    #define kFASGTMBridgeFetcherStatusBadRequest kFASGTMHTTPFetcherStatusBadRequest
  #endif  // FASGTM_USE_SESSION_FETCHER
#endif  // !defined(FASGTMBridgeFetcher)

#ifdef __cplusplus
extern "C" {
#endif

// notifications
//
// fetch started and stopped, and fetch retry delay started and stopped
extern NSString *const kFASGTMHTTPFetcherStartedNotification;
extern NSString *const kFASGTMHTTPFetcherStoppedNotification;
extern NSString *const kFASGTMHTTPFetcherRetryDelayStartedNotification;
extern NSString *const kFASGTMHTTPFetcherRetryDelayStoppedNotification;

// callback constants

extern NSString *const kFASGTMHTTPFetcherErrorDomain;
extern NSString *const kFASGTMHTTPFetcherStatusDomain;
extern NSString *const kFASGTMHTTPFetcherErrorChallengeKey;
extern NSString *const kFASGTMHTTPFetcherStatusDataKey;  // data returned with a kFASGTMHTTPFetcherStatusDomain error

#ifdef __cplusplus
}
#endif

enum {
  kFASGTMHTTPFetcherErrorDownloadFailed = -1,
  kFASGTMHTTPFetcherErrorAuthenticationChallengeFailed = -2,
  kFASGTMHTTPFetcherErrorChunkUploadFailed = -3,
  kFASGTMHTTPFetcherErrorFileHandleException = -4,
  kFASGTMHTTPFetcherErrorBackgroundExpiration = -6,

  // The code kFASGTMHTTPFetcherErrorAuthorizationFailed (-5) has been removed;
  // look for status 401 instead.

  kFASGTMHTTPFetcherStatusNotModified = 304,
  kFASGTMHTTPFetcherStatusBadRequest = 400,
  kFASGTMHTTPFetcherStatusUnauthorized = 401,
  kFASGTMHTTPFetcherStatusForbidden = 403,
  kFASGTMHTTPFetcherStatusPreconditionFailed = 412
};

// cookie storage methods
enum {
  kFASGTMHTTPFetcherCookieStorageMethodStatic = 0,
  kFASGTMHTTPFetcherCookieStorageMethodFetchHistory = 1,
  kFASGTMHTTPFetcherCookieStorageMethodSystemDefault = 2,
  kFASGTMHTTPFetcherCookieStorageMethodNone = 3
};

#ifdef __cplusplus
extern "C" {
#endif

void FASGTMAssertSelectorNilOrImplementedWithArgs(id obj, SEL sel, ...);

// Utility functions for applications self-identifying to servers via a
// user-agent header

// Make a proper app name without whitespace from the given string, removing
// whitespace and other characters that may be special parsed marks of
// the full user-agent string.
NSString *FASGTMCleanedUserAgentString(NSString *str);

// Make an identifier like "MacOSX/10.7.1" or "iPod_Touch/4.1 hw/iPod1_1"
NSString *FASGTMSystemVersionString(void);

// Make a generic name and version for the current application, like
// com.example.MyApp/1.2.3 relying on the bundle identifier and the
// CFBundleShortVersionString or CFBundleVersion.
//
// The bundle ID may be overridden as the base identifier string by
// adding to the bundle's Info.plist a "FASGTMUserAgentID" key.
//
// If no bundle ID or override is available, the process name preceded
// by "proc_" is used.
NSString *FASGTMApplicationIdentifier(NSBundle *bundle);

#ifdef __cplusplus
}  // extern "C"
#endif

@class FASGTMHTTPFetcher;

@protocol FASGTMCookieStorageProtocol <NSObject>
// This protocol allows us to call into the service without requiring
// FASGTMCookieStorage sources in this project
//
// The public interface for cookie handling is the FASGTMCookieStorage class,
// accessible from a fetcher service object's fetchHistory or from the fetcher's
// +staticCookieStorage method.
- (NSArray *)cookiesForURL:(NSURL *)theURL;
- (void)setCookies:(NSArray *)newCookies;
@end

@protocol FASGTMHTTPFetchHistoryProtocol <NSObject>
// This protocol allows us to call the fetch history object without requiring
// FASGTMHTTPFetchHistory sources in this project
- (void)updateRequest:(NSMutableURLRequest *)request isHTTPGet:(BOOL)isHTTPGet;
- (BOOL)shouldCacheETaggedData;
- (NSData *)cachedDataForRequest:(NSURLRequest *)request;
- (id <FASGTMCookieStorageProtocol>)cookieStorage;
- (void)updateFetchHistoryWithRequest:(NSURLRequest *)request
                             response:(NSURLResponse *)response
                       downloadedData:(NSData *)downloadedData;
- (void)removeCachedDataForRequest:(NSURLRequest *)request;
@end

#if FASGTM_USE_SESSION_FETCHER
@protocol FASGTMSessionFetcherServiceProtocol;
#endif

@protocol FASGTMHTTPFetcherServiceProtocol <NSObject>
// This protocol allows us to call into the service without requiring
// FASGTMHTTPFetcherService sources in this project

@property (retain) NSOperationQueue *delegateQueue;

- (BOOL)fetcherShouldBeginFetching:(FASGTMHTTPFetcher *)fetcher;
- (void)fetcherDidStop:(FASGTMHTTPFetcher *)fetcher;

- (FASGTMHTTPFetcher *)fetcherWithRequest:(NSURLRequest *)request;
- (BOOL)isDelayingFetcher:(FASGTMHTTPFetcher *)fetcher;
@end

#if !defined(FASGTM_FETCHER_AUTHORIZATION_PROTOCOL)
#define FASGTM_FETCHER_AUTHORIZATION_PROTOCOL 1
@protocol FASGTMFetcherAuthorizationProtocol <NSObject>
@required
// This protocol allows us to call the authorizer without requiring its sources
// in this project.
- (void)authorizeRequest:(NSMutableURLRequest *)request
                delegate:(id)delegate
       didFinishSelector:(SEL)sel;

- (void)stopAuthorization;

- (void)stopAuthorizationForRequest:(NSURLRequest *)request;

- (BOOL)isAuthorizingRequest:(NSURLRequest *)request;

- (BOOL)isAuthorizedRequest:(NSURLRequest *)request;

@property (retain, readonly) NSString *userEmail;

@optional

// Indicate if authorization may be attempted. Even if this succeeds,
// authorization may fail if the user's permissions have been revoked.
@property (readonly) BOOL canAuthorize;

// For development only, allow authorization of non-SSL requests, allowing
// transmission of the bearer token unencrypted.
@property (assign) BOOL shouldAuthorizeAllRequests;

#if NS_BLOCKS_AVAILABLE
- (void)authorizeRequest:(NSMutableURLRequest *)request
       completionHandler:(void (^)(NSError *error))handler;
#endif

#if FASGTM_USE_SESSION_FETCHER
@property (assign) id<FASGTMSessionFetcherServiceProtocol> fetcherService; // WEAK
#else
@property (assign) id<FASGTMHTTPFetcherServiceProtocol> fetcherService; // WEAK
#endif

- (BOOL)primeForRefresh;

@end
#endif  // !defined(FASGTM_FETCHER_AUTHORIZATION_PROTOCOL)


// FASGTMHTTPFetcher objects are used for async retrieval of an http get or post
//
// See additional comments at the beginning of this file
@interface FASGTMHTTPFetcher : NSObject {
 @protected
  NSMutableURLRequest *request_;
  NSURLConnection *connection_;
  NSMutableData *downloadedData_;
  NSString *downloadPath_;
  NSString *temporaryDownloadPath_;
  NSFileHandle *downloadFileHandle_;
  unsigned long long downloadedLength_;
  NSArray *allowedInsecureSchemes_;
  BOOL allowLocalhostRequest_;
  NSURLCredential *credential_;     // username & password
  NSURLCredential *proxyCredential_; // credential supplied to proxy servers
  NSData *postData_;
  NSInputStream *postStream_;
  NSMutableData *loggedStreamData_;
  NSURLResponse *response_;         // set in connection:didReceiveResponse:
  id delegate_;
  SEL finishedSel_;                 // should by implemented by delegate
  SEL sentDataSel_;                 // optional, set with setSentDataSelector
  SEL receivedDataSel_;             // optional, set with setReceivedDataSelector
#if NS_BLOCKS_AVAILABLE
  void (^completionBlock_)(NSData *, NSError *);
  void (^receivedDataBlock_)(NSData *);
  void (^sentDataBlock_)(NSInteger, NSInteger, NSInteger);
  BOOL (^retryBlock_)(BOOL, NSError *);
#elif !__LP64__
  // placeholders: for 32-bit builds, keep the size of the object's ivar section
  // the same with and without blocks
  id completionPlaceholder_;
  id receivedDataPlaceholder_;
  id sentDataPlaceholder_;
  id retryPlaceholder_;
#endif
  BOOL hasConnectionEnded_;         // set if the connection need not be cancelled
  BOOL isCancellingChallenge_;      // set only when cancelling an auth challenge
  BOOL isStopNotificationNeeded_;   // set when start notification has been sent
  BOOL shouldFetchInBackground_;
#if FASGTM_BACKGROUND_FETCHING
  NSUInteger backgroundTaskIdentifer_; // UIBackgroundTaskIdentifier
#endif
  id userData_;                     // retained, if set by caller
  NSMutableDictionary *properties_; // more data retained for caller
  NSArray *runLoopModes_;           // optional
  NSOperationQueue *delegateQueue_; // optional; available iOS 6/10.7 and later
  id <FASGTMHTTPFetchHistoryProtocol> fetchHistory_; // if supplied by the caller, used for Last-Modified-Since checks and cookies
  NSInteger cookieStorageMethod_;   // constant from above
  id <FASGTMCookieStorageProtocol> cookieStorage_;

  id <FASGTMFetcherAuthorizationProtocol> authorizer_;

  // the service object that created and monitors this fetcher, if any
  id <FASGTMHTTPFetcherServiceProtocol> service_;
  NSString *serviceHost_;
  NSInteger servicePriority_;
  NSThread *thread_;

  BOOL isRetryEnabled_;             // user wants auto-retry
  SEL retrySel_;                    // optional; set with setRetrySelector
  NSTimer *retryTimer_;
  NSUInteger retryCount_;
  NSTimeInterval maxRetryInterval_; // default 600 seconds
  NSTimeInterval minRetryInterval_; // random between 1 and 2 seconds
  NSTimeInterval retryFactor_;      // default interval multiplier is 2
  NSTimeInterval lastRetryInterval_;
  NSDate *initialRequestDate_;
  BOOL hasAttemptedAuthRefresh_;

  NSString *comment_;               // comment for log
  NSString *log_;
#if !STRIP_FASGTM_FETCH_LOGGING
  NSURL *redirectedFromURL_;
  NSString *logRequestBody_;
  NSString *logResponseBody_;
  BOOL hasLoggedError_;
  BOOL shouldDeferResponseBodyLogging_;
#endif
}

// Create a fetcher
//
// fetcherWithRequest will return an autoreleased fetcher, but if
// the connection is successfully created, the connection should retain the
// fetcher for the life of the connection as well. So the caller doesn't have
// to retain the fetcher explicitly unless they want to be able to cancel it.
+ (FASGTMHTTPFetcher *)fetcherWithRequest:(NSURLRequest *)request;

// Convenience methods that make a request, like +fetcherWithRequest
+ (FASGTMHTTPFetcher *)fetcherWithURL:(NSURL *)requestURL;
+ (FASGTMHTTPFetcher *)fetcherWithURLString:(NSString *)requestURLString;

// Designated initializer
- (id)initWithRequest:(NSURLRequest *)request;

// Fetcher request
//
// The underlying request is mutable and may be modified by the caller
@property (retain) NSMutableURLRequest *mutableRequest;

// By default, the fetcher allows only secure (https) schemes unless this
// property is set, or the FASGTM_ALLOW_INSECURE_REQUESTS build flag is set.
//
// For example, during debugging when fetching from a development server that lacks SSL support,
// this may be set to @[ @"http" ], or when the fetcher is used to retrieve local files,
// this may be set to @[ @"file" ].
//
// This should be left as nil for release builds to avoid creating the opportunity for
// leaking private user behavior and data.  If a server is providing insecure URLs
// for fetching by the client app, report the problem as server security & privacy bug.
@property(copy) NSArray *allowedInsecureSchemes;

// By default, the fetcher prohibits localhost requests unless this property is set,
// or the FASGTM_ALLOW_INSECURE_REQUESTS build flag is set.
//
// For localhost requests, the URL scheme is not checked  when this property is set.
@property(assign) BOOL allowLocalhostRequest;

// Setting the credential is optional; it is used if the connection receives
// an authentication challenge
@property (retain) NSURLCredential *credential;

// Setting the proxy credential is optional; it is used if the connection
// receives an authentication challenge from a proxy
@property (retain) NSURLCredential *proxyCredential;

// If post data or stream is not set, then a GET retrieval method is assumed
@property (retain) NSData *postData;
@property (retain) NSInputStream *postStream;

// The default cookie storage method is kFASGTMHTTPFetcherCookieStorageMethodStatic
// without a fetch history set, and kFASGTMHTTPFetcherCookieStorageMethodFetchHistory
// with a fetch history set
//
// Applications needing control of cookies across a sequence of fetches should
// create fetchers from a FASGTMHTTPFetcherService object (which encapsulates
// fetch history) for a well-defined cookie store
@property (assign) NSInteger cookieStorageMethod;

+ (id <FASGTMCookieStorageProtocol>)staticCookieStorage;

// Object to add authorization to the request, if needed
@property (retain) id <FASGTMFetcherAuthorizationProtocol> authorizer;

// The service object that created and monitors this fetcher, if any
@property (retain) id <FASGTMHTTPFetcherServiceProtocol> service;

// The host, if any, used to classify this fetcher in the fetcher service
@property (copy) NSString *serviceHost;

// The priority, if any, used for starting fetchers in the fetcher service
//
// Lower values are higher priority; the default is 0, and values may
// be negative or positive. This priority affects only the start order of
// fetchers that are being delayed by a fetcher service.
@property (assign) NSInteger servicePriority;

// The thread used to run this fetcher in the fetcher service when no operation
// queue is provided.
@property (retain) NSThread *thread;

// The delegate is retained during the connection
@property (retain) id delegate;

// On iOS 4 and later, the fetch may optionally continue while the app is in the
// background until finished or stopped by OS expiration
//
// The default value is NO
//
// For Mac OS X, background fetches are always supported, and this property
// is ignored
@property (assign) BOOL shouldFetchInBackground;

// The delegate's optional sentData selector may be used to monitor upload
// progress. It should have a signature like:
//  - (void)myFetcher:(FASGTMHTTPFetcher *)fetcher
//              didSendBytes:(NSInteger)bytesSent
//            totalBytesSent:(NSInteger)totalBytesSent
//  totalBytesExpectedToSend:(NSInteger)totalBytesExpectedToSend;
//
// +doesSupportSentDataCallback indicates if this delegate method is supported
+ (BOOL)doesSupportSentDataCallback;

@property (assign) SEL sentDataSelector;

// The delegate's optional receivedData selector may be used to monitor download
// progress. It should have a signature like:
//  - (void)myFetcher:(FASGTMHTTPFetcher *)fetcher
//       receivedData:(NSData *)dataReceivedSoFar;
//
// The dataReceived argument will be nil when downloading to a path or to a
// file handle.
//
// Applications should not use this method to accumulate the received data;
// the callback method or block supplied to the beginFetch call will have
// the complete NSData received.
@property (assign) SEL receivedDataSelector;

#if NS_BLOCKS_AVAILABLE
// The full interface to the block is provided rather than just a typedef for
// its parameter list in order to get more useful code completion in the Xcode
// editor
@property (copy) void (^sentDataBlock)(NSInteger bytesSent, NSInteger totalBytesSent, NSInteger bytesExpectedToSend);

// The dataReceived argument will be nil when downloading to a path or to
// a file handle
@property (copy) void (^receivedDataBlock)(NSData *dataReceivedSoFar);
#endif

// retrying; see comments at the top of the file.  Calling
// setRetryEnabled(YES) resets the min and max retry intervals.
@property (assign, getter=isRetryEnabled) BOOL retryEnabled;

// Retry selector or block is optional for retries.
//
// If present, it should have the signature:
//   -(BOOL)fetcher:(FASGTMHTTPFetcher *)fetcher willRetry:(BOOL)suggestedWillRetry forError:(NSError *)error
// and return YES to cause a retry.  See comments at the top of this file.
@property (assign) SEL retrySelector;

#if NS_BLOCKS_AVAILABLE
@property (copy) BOOL (^retryBlock)(BOOL suggestedWillRetry, NSError *error);
#endif

// Retry intervals must be strictly less than maxRetryInterval, else
// they will be limited to maxRetryInterval and no further retries will
// be attempted.  Setting maxRetryInterval to 0.0 will reset it to the
// default value, 600 seconds.

@property (assign) NSTimeInterval maxRetryInterval;

// Starting retry interval.  Setting minRetryInterval to 0.0 will reset it
// to a random value between 1.0 and 2.0 seconds.  Clients should normally not
// call this except for unit testing.
@property (assign) NSTimeInterval minRetryInterval;

// Multiplier used to increase the interval between retries, typically 2.0.
// Clients should not need to call this.
@property (assign) double retryFactor;

// Number of retries attempted
@property (readonly) NSUInteger retryCount;

// interval delay to precede next retry
@property (readonly) NSTimeInterval nextRetryInterval;

// Begin fetching the request
//
// The delegate can optionally implement the finished selectors or pass NULL
// for it.
//
// Returns YES if the fetch is initiated.  The delegate is retained between
// the beginFetch call until after the finish callback.
//
// An error is passed to the callback for server statuses 300 or
// higher, with the status stored as the error object's code.
//
// finishedSEL has a signature like:
//   - (void)fetcher:(FASGTMHTTPFetcher *)fetcher finishedWithData:(NSData *)data error:(NSError *)error;
//
// If the application has specified a downloadPath or downloadFileHandle
// for the fetcher, the data parameter passed to the callback will be nil.

- (BOOL)beginFetchWithDelegate:(id)delegate
             didFinishSelector:(SEL)finishedSEL;

#if NS_BLOCKS_AVAILABLE
- (BOOL)beginFetchWithCompletionHandler:(void (^)(NSData *data, NSError *error))handler;
#endif


// Returns YES if this is in the process of fetching a URL
- (BOOL)isFetching;

// Cancel the fetch of the request that's currently in progress
- (void)stopFetching;

// Return the status code from the server response
@property (readonly) NSInteger statusCode;

// Return the http headers from the response
@property (retain, readonly) NSDictionary *responseHeaders;

// The response, once it's been received
@property (retain) NSURLResponse *response;

// Bytes downloaded so far
@property (readonly) unsigned long long downloadedLength;

// Buffer of currently-downloaded data
@property (readonly, retain) NSData *downloadedData;

// Path in which to non-atomically create a file for storing the downloaded data
//
// The path must be set before fetching begins.  The download file handle
// will be created for the path, and can be used to monitor progress. If a file
// already exists at the path, it will be overwritten.
@property (copy) NSString *downloadPath;

// If downloadFileHandle is set, data received is immediately appended to
// the file handle rather than being accumulated in the downloadedData property
//
// The file handle supplied must allow writing and support seekToFileOffset:,
// and must be set before fetching begins.  Setting a download path will
// override the file handle property.
@property (retain) NSFileHandle *downloadFileHandle;

// The optional fetchHistory object is used for a sequence of fetchers to
// remember ETags, cache ETagged data, and store cookies.  Typically, this
// is set by a FASGTMFetcherService object when it creates a fetcher.
//
// Side effect: setting fetch history implicitly calls setCookieStorageMethod:
@property (retain) id <FASGTMHTTPFetchHistoryProtocol> fetchHistory;

// userData is retained for the convenience of the caller
@property (retain) id userData;

// Stored property values are retained for the convenience of the caller
@property (copy) NSMutableDictionary *properties;

- (void)setProperty:(id)obj forKey:(NSString *)key; // pass nil obj to remove property
- (id)propertyForKey:(NSString *)key;

- (void)addPropertiesFromDictionary:(NSDictionary *)dict;

// Comments are useful for logging
@property (copy) NSString *comment;

- (void)setCommentWithFormat:(NSString *)format, ... NS_FORMAT_FUNCTION(1, 2);

// Log of request and response, if logging is enabled
@property (copy) NSString *log;

// Callbacks can be invoked on an operation queue rather than via the run loop,
// starting on 10.7 and iOS 6.  If a delegate queue is supplied. the run loop
// modes are ignored. If no delegateQueue is supplied, and run loop modes are
// not supplied, and the fetcher is started off of the main thread, then a
// delegateQueue of [NSOperationQueue mainQueue] is assumed.
@property (retain) NSOperationQueue *delegateQueue;

// Using the fetcher while a modal dialog is displayed requires setting the
// run-loop modes to include NSModalPanelRunLoopMode
@property (retain) NSArray *runLoopModes;

// Users who wish to replace FASGTMHTTPFetcher's use of NSURLConnection
// can do so globally here.  The replacement should be a subclass of
// NSURLConnection.
+ (Class)connectionClass;
+ (void)setConnectionClass:(Class)theClass;

//
// Method for compatibility with FASGTMSessionFetcher
//
@property (retain) NSData *bodyData;

// Spin the run loop, discarding events, until the fetch has completed
//
// This is only for use in testing or in tools without a user interface.
//
// Synchronous fetches should never be done by shipping apps; they are
// sufficient reason for rejection from the app store.
- (void)waitForCompletionWithTimeout:(NSTimeInterval)timeoutInSeconds;

#if STRIP_FASGTM_FETCH_LOGGING
// if logging is stripped, provide a stub for the main method
// for controlling logging
+ (void)setLoggingEnabled:(BOOL)flag;
#endif // STRIP_FASGTM_FETCH_LOGGING

@end
