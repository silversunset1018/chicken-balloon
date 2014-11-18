#import "UIKitUtil.h"

extern "C"{
    void _pasteboardSetText(const char *text) {
        
        NSString* str = [NSString stringWithCString: text encoding:NSUTF8StringEncoding];
        
        [UIPasteboard generalPasteboard].string = str;
        
    }
}


