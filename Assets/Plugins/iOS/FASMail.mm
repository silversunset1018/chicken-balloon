#import "FASMail.h"

@implementation FASMail

static FASMail *pInstance = nil;

static char callbackGameObjectName[256];

+ (FASMail*) instance {
    @synchronized(self) {
        if(pInstance == nil) {
            pInstance = [[self alloc]init];
        }
    }
    return pInstance;
}

- (IBAction)showEmail :(NSString*) title
                      :(NSString*) message
                      :(NSString*) to
{
    NSArray *toRecipents = [NSArray arrayWithObject:to];
    
    MFMailComposeViewController *mc = [[MFMailComposeViewController alloc] init];
    mc.mailComposeDelegate = self;
    [mc setSubject:title];
    [mc setMessageBody:message isHTML:NO];
    [mc setToRecipients:toRecipents];
    
    // Present mail view controller on screen
    [UnityGetGLViewController() presentViewController:mc animated:YES completion:NULL];
    
}

- (void) mailComposeController:(MFMailComposeViewController *)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError *)error
{
	char *copyStr = (char *)malloc(strlen(callbackGameObjectName)+1);
    assert(copyStr);
    strcpy(copyStr, callbackGameObjectName);

    switch (result) {
        case MFMailComposeResultCancelled:
            UnitySendMessage(copyStr, "OnEmailDone", "Cancelled");
            break;
        case MFMailComposeResultSaved:
            UnitySendMessage(copyStr, "OnEmailDone", "Saved");
            break;
        case MFMailComposeResultSent:
            UnitySendMessage(copyStr, "OnEmailDone", "Sent");
            break;
        default:
            UnitySendMessage(copyStr, "OnEmailDone", "Error");
            break;
    }

    // Close the Mail Interface
    [UnityGetGLViewController() dismissViewControllerAnimated:YES completion:NULL];
}

@end

extern "C" {
    
    UIViewController *UnityGetGLViewController();
    
    UIView *UnityGetGLView();
    
    void _FasSendMail(const char *title, const char *body, const char *to, const char *gameObjectName)
    {
        NSString *strTitle = [[NSString alloc] initWithUTF8String:title];
        NSString *strBody = [[NSString alloc] initWithUTF8String:body];
        NSString *strTo = [[NSString alloc] initWithUTF8String:to];

		strncpy(callbackGameObjectName, gameObjectName, sizeof(callbackGameObjectName));
        
        [[FASMail instance] showEmail:strTitle :strBody :strTo ];
    }
}
