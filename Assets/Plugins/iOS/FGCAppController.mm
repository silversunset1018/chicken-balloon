#import "UnityAppController.h"

#include "AppDelegateListener.h"

@interface FGCAppController : UnityAppController
@end

char* MakeStringCopy (const char* string) {
    if (string == NULL) return NULL;
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

@implementation FGCAppController

const char* pushedIdData;

- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo
{
    AppController_SendNotificationWithArg(kUnityDidReceiveRemoteNotification, userInfo);
	UnitySendRemoteNotification(userInfo);
    
    if (application.applicationState == UIApplicationStateActive)
    {
        // アプリが起動している時に、push通知が届きpush通知から起動
    }
    
    if (application.applicationState == UIApplicationStateInactive)
    {
        // アプリがバックグラウンドで起動している時に、push通知が届きpush通知から起動
        if (userInfo != nil) {
            
            NSDictionary *id = [userInfo objectForKey:@"id"];
            
            if(id != NULL)
                pushedIdData = MakeStringCopy([[id description]UTF8String]);            
        }
    }
}

@end


extern "C" const char* GetPushedNotificationId_()
{
    if(pushedIdData != NULL)
        return pushedIdData;
    else
        return NULL;
}

extern "C" const void ClearPushedNotificationId_()
{
    pushedIdData = NULL;
}


IMPL_APP_CONTROLLER_SUBCLASS(FGCAppController)