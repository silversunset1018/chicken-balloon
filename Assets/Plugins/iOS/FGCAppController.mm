#import "UnityAppController.h"

#include "AppDelegateListener.h"

@interface FGCAppController : UnityAppController
@end

char* FgcMakeStringCopy (const char* string) {
    if (string == NULL) return NULL;
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

@implementation FGCAppController

const char* pushedIdData;

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    [super application:application didFinishLaunchingWithOptions:launchOptions];
    
    NSDictionary *userInfo = [launchOptions objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey];
    
    if (userInfo != nil) {
        
        NSDictionary *id = [userInfo objectForKey:@"id"];
        
        if(id != NULL)
            pushedIdData = FgcMakeStringCopy([[id description]UTF8String]);
        
    }
    
    return YES;
}

- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo
{
    AppController_SendNotificationWithArg(kUnityDidReceiveRemoteNotification, userInfo);
    UnitySendRemoteNotification(userInfo);
    
    if (application.applicationState == UIApplicationStateActive)
    {

    }
    
    if (application.applicationState == UIApplicationStateInactive)
    {
        if (userInfo != nil) {
            
            NSDictionary *id = [userInfo objectForKey:@"id"];
            
            if(id != NULL)
                pushedIdData = FgcMakeStringCopy([[id description]UTF8String]);
        }
    }
}

@end


extern "C" const char* _GetPushedNotificationId()
{
    if(pushedIdData != NULL)
        return pushedIdData;
    else
        return NULL;
}

extern "C" const void _ClearPushedNotificationId()
{
    pushedIdData = NULL;
}


IMPL_APP_CONTROLLER_SUBCLASS(FGCAppController)