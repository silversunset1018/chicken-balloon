#if FASGTM_INCLUDE_OAUTH2 || !GDATA_REQUIRE_SERVICE_INCLUDES

#import <Foundation/Foundation.h>
#import <SystemConfiguration/SystemConfiguration.h>

// FASGTMHTTPFetcher brings in GTLDefines/GDataDefines
#if FASGTM_USE_SESSION_FETCHER
#import "FASGTMSessionFetcher.h"
#else
#import "FASGTMHTTPFetcher.h"
#endif

#import "FASGTMOAuth2Authentication.h"

@interface FASGTMOAuth2SignIn : NSObject {
 @private
  FASGTMOAuth2Authentication *auth_;

  // the endpoint for displaying the sign-in page
  NSURL *authorizationURL_;
  NSDictionary *additionalAuthorizationParameters_;

  id delegate_;
  SEL webRequestSelector_;
  SEL finishedSelector_;

  BOOL hasHandledCallback_;

  FASGTMOAuth2Fetcher *pendingFetcher_;

#if !FASGTM_OAUTH2_SKIP_GOOGLE_SUPPORT
  BOOL shouldFetchGoogleUserEmail_;
  BOOL shouldFetchGoogleUserProfile_;
  NSDictionary *userProfile_;
#endif

  SCNetworkReachabilityRef reachabilityRef_;
  NSTimer *networkLossTimer_;
  NSTimeInterval networkLossTimeoutInterval_;
  BOOL hasNotifiedNetworkLoss_;

  id userData_;
}

@property (nonatomic, retain) FASGTMOAuth2Authentication *authentication;

@property (nonatomic, retain) NSURL *authorizationURL;
@property (nonatomic, retain) NSDictionary *additionalAuthorizationParameters;

// The delegate is released when signing in finishes or is cancelled
@property (nonatomic, retain) id delegate;
@property (nonatomic, assign) SEL webRequestSelector;
@property (nonatomic, assign) SEL finishedSelector;

@property (nonatomic, retain) id userData;

// By default, signing in to Google will fetch the user's email, but will not
// fetch the user's profile.
//
// The email is saved in the auth object.
// The profile is available immediately after sign-in.
#if !FASGTM_OAUTH2_SKIP_GOOGLE_SUPPORT
@property (nonatomic, assign) BOOL shouldFetchGoogleUserEmail;
@property (nonatomic, assign) BOOL shouldFetchGoogleUserProfile;
@property (nonatomic, retain, readonly) NSDictionary *userProfile;
#endif

// The default timeout for an unreachable network during display of the
// sign-in page is 30 seconds; set this to 0 to have no timeout
@property (nonatomic, assign) NSTimeInterval networkLossTimeoutInterval;

// The delegate is retained until sign-in has completed or been canceled
//
// designated initializer
- (id)initWithAuthentication:(FASGTMOAuth2Authentication *)auth
            authorizationURL:(NSURL *)authorizationURL
                    delegate:(id)delegate
          webRequestSelector:(SEL)webRequestSelector
            finishedSelector:(SEL)finishedSelector;

// A default authentication object for signing in to Google services
#if !FASGTM_OAUTH2_SKIP_GOOGLE_SUPPORT
+ (FASGTMOAuth2Authentication *)standardGoogleAuthenticationForScope:(NSString *)scope
                                                         clientID:(NSString *)clientID
                                                     clientSecret:(NSString *)clientSecret;
#endif

#pragma mark Methods used by the Window Controller

// Start the sequence of fetches and sign-in window display for sign-in
- (BOOL)startSigningIn;

// Stop any pending fetches, and close the window (but don't call the
// delegate's finishedSelector)
- (void)cancelSigningIn;

// Window controllers must tell the sign-in object about any redirect
// requested by the web view, and any changes in the webview window title
//
// If these return YES then the event was handled by the
// sign-in object (typically by closing the window) and should be ignored by
// the window controller's web view

- (BOOL)requestRedirectedToRequest:(NSURLRequest *)redirectedRequest;
- (BOOL)titleChanged:(NSString *)title;
- (BOOL)cookiesChanged:(NSHTTPCookieStorage *)cookieStorage;
- (BOOL)loadFailedWithError:(NSError *)error;

// Window controllers must tell the sign-in object if the window was closed
// prematurely by the user (but not by the sign-in object); this calls the
// delegate's finishedSelector
- (void)windowWasClosed;

// Start the sequences for signing in with an authorization code. The
// authentication must contain an authorization code, otherwise the process
// will fail.
- (void)authCodeObtained;

#pragma mark -

#if !FASGTM_OAUTH2_SKIP_GOOGLE_SUPPORT
// Revocation of an authorized token from Google
+ (void)revokeTokenForGoogleAuthentication:(FASGTMOAuth2Authentication *)auth;

// Create a fetcher for obtaining the user's Google email address or profile,
// according to the current auth scopes.
//
// The auth object must have been created with appropriate scopes.
//
// The fetcher's response data can be parsed with NSJSONSerialization.
+ (FASGTMOAuth2Fetcher *)userInfoFetcherWithAuth:(FASGTMOAuth2Authentication *)auth;
#endif

#pragma mark -

// Standard authentication values
+ (NSString *)nativeClientRedirectURI;
#if !FASGTM_OAUTH2_SKIP_GOOGLE_SUPPORT
+ (NSURL *)googleAuthorizationURL;
+ (NSURL *)googleTokenURL;
+ (NSURL *)googleUserInfoURL;
#endif

@end

#endif // #if FASGTM_INCLUDE_OAUTH2 || !GDATA_REQUIRE_SERVICE_INCLUDES
