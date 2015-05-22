//
//  FASShareYoutube.m
//  appsteroid-ios
//
//  Created by katsuhito.matsushima on 1/22/15.
//  Copyright (c) 2015 Fresvii Inc. All rights reserved.
//

#import "FASShareYoutube.h"
#import "FASGTMOAuth2ViewControllerTouch.h"

static NSString *kKeychainItemName;
static NSString *scope;
static NSString *clientId;
static NSString *clientSecret;
static NSString *hasLoggedIn; // NSUserDefaultに保存するための文字列

extern "C" void UnitySendMessage(const char* obj, const char* method, const char* msg);

@interface FASShareYoutube ()

@end

@implementation FASShareYoutube

static FASShareYoutube *pInstance = nil;

+ (FASShareYoutube*) instance {
    @synchronized(self) {
        if(pInstance == nil) {
            pInstance = [[self alloc]init];
        }
    }
    return pInstance;
}

int videoDataLength = 0;
int dataTotal = 0;

- (id)init
{
    self = [super init];
    
    if (self)
    {
    }
    
    return self;
}

- (void)startLogin
{
    NSLog(@"startLogin");
    
    NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
    BOOL hasLoggedin = [defaults boolForKey:hasLoggedIn];
    
    if(hasLoggedin == YES)
    {
        self.auth = [FASGTMOAuth2ViewControllerTouch authForGoogleFromKeychainForName:kKeychainItemName
                                                                             clientID:clientId
                                                                         clientSecret:clientSecret];
        [self authorizeRequestWithCompletion:^(NSError *error)
         {
             if (error)
             {
                 [defaults setBool:NO forKey:hasLoggedIn];
                 
                 [defaults synchronize];
                 
                 UnitySendMessage("FASYoutubeObserver", "OnLoginGTMCompleted", "failure");
             }
             else
             {
                 const char* str = [self.auth.accessToken UTF8String];
                 
                 char* res = (char*)malloc(strlen(str) + 1);
                 
                 strcpy(res, str);
                 
                 UnitySendMessage("FASYoutubeObserver", "OnLoginGTMCompleted", res);
             }
         }];
    }
    else
    {
        FASGTMOAuth2ViewControllerTouch *auth2ViewController = [[FASGTMOAuth2ViewControllerTouch alloc] initWithScope:scope
                                                                                                             clientID:clientId
                                                                                                         clientSecret:clientSecret
                                                                                                     keychainItemName:kKeychainItemName
                                                                                                             delegate:self
                                                                                                     finishedSelector:@selector(viewController:finishedWithAuth:error:)];
        auth2ViewController.title = @"Google Login";
        
        UINavigationController *navigationController = [[UINavigationController alloc] initWithRootViewController:auth2ViewController];
        
        navigationController.navigationBar.backgroundColor = [UIColor blackColor];
        
        navigationController.navigationBar.barStyle = UIBarStyleBlack;
        
        UIBarButtonItem *barButton = [[UIBarButtonItem alloc] initWithTitle:@"Close"
                                                                      style:UIBarButtonItemStylePlain
                                                                     target:self
                                                                     action:@selector(pushedBackButton:)];
        
        auth2ViewController.navigationItem.leftBarButtonItem = barButton;
        
        [UnityGetGLViewController() presentViewController:navigationController animated:YES completion:nil];
    }
}

- (void)authorizeRequestWithCompletion:(void (^)(NSError *error))completion
{
    NSMutableURLRequest *req = [[NSMutableURLRequest alloc] initWithURL:self.auth.tokenURL];
    [self.auth authorizeRequest:req completionHandler:^(NSError *error)
     {
         NSLog(@"%@", self.auth);
         NSLog(@"%@", self.auth.accessToken);
         completion(error);
     }];
}

- (void)shareWithVideoData:(NSData *)videoData
                dataLength:(int)dataLength
                     title:(NSString *)title
               description:(NSString *)description
                     token:(NSString *)token
{
    videoDataLength = dataLength;
    
    dataTotal = 0;
    
    NSString *urlString = @"https://www.googleapis.com/upload/youtube/v3/videos?part=status%2Csnippet&uploadType=multipart";
    NSURL *url = [NSURL URLWithString:urlString];
    
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:url];
    request.HTTPMethod = @"POST";
    NSMutableDictionary *headers = [@{} mutableCopy];
    NSString *boundary = @"---------------------------168072824752491622650073";
    headers[@"Authorization"] = [NSString stringWithFormat:@"Bearer %@", token];
    headers[@"Content-Type"] = [NSString stringWithFormat:@"multipart/related; boundary=%@", boundary];
    headers[@"Accept-Encoding"] = @"gzip";
    request.allHTTPHeaderFields = headers;
    
    NSMutableData *body = [NSMutableData data];
    
    // Set params
    NSDictionary *jsonDict = @{
                               @"snippet" : @{@"title" : title, @"description" : description},
                               @"status" : @{@"privacyStatus" : @"public"}
                               };
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:jsonDict
                                                       options:0
                                                         error:&error];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    [body appendData:[[NSString stringWithFormat:@"--%@\r\n", boundary] dataUsingEncoding:NSUTF8StringEncoding]];
    [body appendData:[@"Content-Type: application/json; charset=UTF-8\r\n" dataUsingEncoding:NSUTF8StringEncoding]];
    [body appendData:[[NSString stringWithFormat:@"\r\n%@\r\n", jsonString] dataUsingEncoding:NSUTF8StringEncoding]];
    
    // Set data
    [body appendData:[[NSString stringWithFormat:@"--%@\r\n", boundary] dataUsingEncoding:NSUTF8StringEncoding]];
    [body appendData:[@"Content-Type: video/mp4\r\n" dataUsingEncoding:NSUTF8StringEncoding]];
    [body appendData:[@"Content-Transfer-Encoding: binary\r\n\r\n" dataUsingEncoding:NSUTF8StringEncoding]];
    [body appendData:videoData];
    [body appendData:[[NSString stringWithFormat:@"\r\n--%@--\r\n", boundary] dataUsingEncoding:NSUTF8StringEncoding]];
    
    request.HTTPBody = body;
    
    // Send the url-request.
    /*[NSURLConnection sendAsynchronousRequest:request
     queue:[NSOperationQueue mainQueue]
     completionHandler:^(NSURLResponse *response, NSData *data, NSError *error) {
     
     if (data) {
     
     NSString *result = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
     
     NSLog(@"###### result:\n %@", result);
     
     const char* str = [result UTF8String];
     
     char* res = (char*)malloc(strlen(str) + 1);
     
     strcpy(res, str);
     
     UnitySendMessage("FASYoutubeObserver", "OnYoutubeSharingCompleted", res);
     
     } else {
     
     NSLog(@"###### error:\n %@", error);
     
     const char* str = [error UTF8String];
     
     char* res = (char*)malloc(strlen(str) + 1);
     
     strcpy(res, str);
     
     UnitySendMessage("FASYoutubeObserver", "OnYoutubeSharingCompleted", res);
     }
     }];*/
    
    [NSURLConnection connectionWithRequest:request delegate:self];
}


#pragma mark - Callbacks

#pragma mark - Delegate
-(NSURLRequest *)connection:(NSURLConnection *)connection willSendRequest:(NSURLRequest *)request redirectResponse:(NSURLResponse *)response {
    
    UnitySendMessage("FASYoutubeObserver", "OnYoutubeSharingProgress", "0");
    
    // Redirect?
    // You can add additional things for request
    return request;
}

#pragma mark - Complete Response
-(void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response {
    // Get complete response
    
    NSHTTPURLResponse *httpResponse = (NSHTTPURLResponse *)response;
    
    if(httpResponse.statusCode == 200)
        UnitySendMessage("FASYoutubeObserver", "OnYoutubeSharingCompleted", "Done");
    else
        UnitySendMessage("FASYoutubeObserver", "OnYoutubeSharingCompleted", "Error");
    
}

#pragma mark - Receive Data
-(void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data {
    // How many bytes in this chunk
    
    dataTotal += data.length;
    
    float progress = dataTotal / dataTotal;
    
    NSString *str1 = [NSString stringWithFormat:@"%.2f", progress]; // 1.23
    
    const char* str = [str1 UTF8String];
    
    char* res = (char*)malloc(strlen(str) + 1);
    
    strcpy(res, str);
    
    UnitySendMessage("FASYoutubeObserver", "OnYoutubeSharingProgress", res);
}

#pragma mark - Failed
-(void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error {
    NSLog(@"Upload failed with error %@", [error localizedDescription]);
    // Error handling
    // Close file delete tmp file or something
    
    UnitySendMessage("FASYoutubeObserver", "OnYoutubeSharingCompleted", "Error");
}

#pragma mark - Entire request has benn loaded, data was finished
-(void) connectionDidFinishLoading:(NSURLConnection *)connection {
    // File operation or complete operations
    UnitySendMessage("FASYoutubeObserver", "OnYoutubeSharingCompleted", "Done");
}

- (void)viewController:(FASGTMOAuth2ViewControllerTouch *)viewController
      finishedWithAuth:(FASGTMOAuth2Authentication *)auth
                 error:(NSError *)error
{
    if(error)
    {
        UnitySendMessage("FASYoutubeObserver", "OnLoginGTMCompleted", "failure");
    }
    else
    {
        self.auth = auth;
        
        NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
        
        [defaults setBool:YES forKey:hasLoggedIn];
        
        [defaults synchronize];
        
        [self authorizeRequestWithCompletion:^(NSError *error)
         {
             const char* str = [self.auth.accessToken UTF8String];
             
             char* res = (char*)malloc(strlen(str) + 1);
             
             strcpy(res, str);
             
             UnitySendMessage("FASYoutubeObserver", "OnLoginGTMCompleted", res);
             
             [viewController dismissViewControllerAnimated:YES completion:nil];
         }];
    }
}

- (void)pushedBackButton:(id)sender
{
    UnitySendMessage("FASYoutubeObserver", "OnLoginGTMCompleted", "failure");
    
    [UnityGetGLViewController() dismissViewControllerAnimated:YES completion:nil];
}

@end

extern "C" {
    
    UIViewController *UnityGetGLViewController();
    
    UIView *UnityGetGLView();
    
    void _FasShareYoutube(const char** videoData, const int videoDataLength, const char *title, const char *descrpition, const char *token)
    {
        NSString *strTitle = [[NSString alloc] initWithUTF8String:title];
        
        NSString *strDescrpition = [[NSString alloc] initWithUTF8String:descrpition];
        
        NSString *strToken = [[NSString alloc] initWithUTF8String:token];
        
        NSData *src = [NSData dataWithBytes:(const void *)videoData length:(sizeof(unsigned char) * videoDataLength)];
        
        [[FASShareYoutube instance] shareWithVideoData :src dataLength:videoDataLength title:strTitle description:strDescrpition token:strToken];
    }
    
    void _FasLoginGTM()
    {
        [[FASShareYoutube instance] startLogin];
    }
    
    void FasFreeVideoDataMemory(char** ptrDest)
    {
        free(*ptrDest);
    }
    
    void _FasSetYoutubeParameters(const char* _keyChainItemName, const char* _scope, const char* _clientId, const char* _clientSecret, const char* _hasLoggedIn)
    {
        kKeychainItemName = [[NSString alloc] initWithUTF8String:_keyChainItemName];
        
        scope = [[NSString alloc] initWithUTF8String:_scope];
        
        clientId = [[NSString alloc] initWithUTF8String:_clientId];
        
        clientSecret = [[NSString alloc] initWithUTF8String:_clientSecret];
        
        hasLoggedIn = [[NSString alloc] initWithUTF8String:_hasLoggedIn];
    }
}

