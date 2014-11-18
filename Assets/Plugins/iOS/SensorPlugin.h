#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>

@interface SensorPlugin : NSObject

+ (SensorPlugin*) instance;
- (void) registerListnerProximitySensor;
- (void) unregisterListnerProximitySensor;
- (void) proximitySensorStateDidChange:(NSNotification *)notification;

@end
