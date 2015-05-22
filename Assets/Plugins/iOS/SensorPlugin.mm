#import "SensorPlugin.h"

extern "C" void UnitySendMessage(const char* obj, const char* method, const char* msg);

static char strGameObjectName[256];

static char strMethodName[256];

@implementation SensorPlugin

static SensorPlugin *pInstance = nil;

+ (SensorPlugin*) instance {
    @synchronized(self) {
        if(pInstance == nil) {
            pInstance = [[self alloc]init];
        }
    }
    return pInstance;
}

- (void) registerListnerProximitySensor
{
    
    UIDevice *device = [UIDevice currentDevice];
    
    device.proximityMonitoringEnabled = YES;
    
    if (device.proximityMonitoringEnabled)
    {
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(proximitySensorStateDidChange:)
                                                     name:UIDeviceProximityStateDidChangeNotification
                                                   object:nil];
    }
}

- (void) unregisterListnerProximitySensor
{
    [UIDevice currentDevice].proximityMonitoringEnabled = NO;
    
    [[NSNotificationCenter defaultCenter] removeObserver:self
                                                    name:UIDeviceProximityStateDidChangeNotification
                                                  object:nil];
}

- (void)proximitySensorStateDidChange:(NSNotification *)notification {
    if ([UIDevice currentDevice].proximityState == NO)
    {
        //  At far
        UnitySendMessage(strGameObjectName, strMethodName, "5.0");
    }
    else
    {
    	// At near
        UnitySendMessage(strGameObjectName, strMethodName, "0.0");
    }
}

@end

extern "C" void _RegisterListnerProximitySensor(const char *callbackGameObjectName, const char *callbackMethodName)
{
    strncpy(strGameObjectName, callbackGameObjectName, sizeof(strGameObjectName));
    
	strncpy(strMethodName, callbackMethodName, sizeof(strMethodName));
    
    [[SensorPlugin instance] registerListnerProximitySensor];
}

extern "C" void _UnregisterListnerProximitySensor() {
    
    [[SensorPlugin instance] unregisterListnerProximitySensor];
}

