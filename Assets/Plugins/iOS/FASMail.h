#import <UIKit/UIKit.h>
#import <MessageUI/MessageUI.h>

@interface FASMail : UIViewController <MFMailComposeViewControllerDelegate>

+ (FASMail*) instance;
- (IBAction)showEmail:(NSString*) title :(NSString*) message :(NSString*) to;

@end
