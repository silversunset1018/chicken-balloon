#import "FASImagePickerController.h"

@implementation FASImagePickerController

//------------------------------------------------------------------------------
#pragma mark - View life cycle
//------------------------------------------------------------------------------
- (BOOL)shouldAutorotate {
    return YES;
}

- (NSUInteger)supportedInterfaceOrientations {
    return UIInterfaceOrientationMaskAll;
}

@end