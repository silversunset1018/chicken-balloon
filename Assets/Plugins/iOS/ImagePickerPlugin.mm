#import "ImagePickerPlugin.h"

extern UIViewController *UnityGetGLViewController();

extern UIView *UnityGetGLView();

extern "C" void UnitySendMessage(const char* obj, const char* method, const char* msg);

typedef struct {
	unsigned char r, g, b, a;
} Color32;

static const char *strSourceType_PhotoLibrary = "PhotoLibrary";
static const char *strSourceType_Camera = "Camera";
static const char *strSourceType_SavedPhotosAlbum = "SavedPhotosAlbum";

static const char *strCallbackResultMessage_Loaded = "Loaded";
static const char *strCallbackResultMessage_Canceled = "Canceled";
static const char *strCallbackResultMessage_Hidden = "Hidden";
static const char *strCallbackResultMessage_SourceTypeUnavailable = "SourceTypeUnavailable";

static UIImagePickerControllerSourceType sourceType = UIImagePickerControllerSourceTypePhotoLibrary;

static struct {
	char strGameObjectName[256];
	char strMethodName[256];
} callbackLoadedInfo = {0};

static void ImagePicker_callback(const char *msg);

@interface ImagePickerPlugin (Private)
+ (ImagePickerPlugin *) instance;

- (bool) showImagePicker:(const char *)sourceTypeText;
- (bool) getLoadedTexrure:(Color32 *)pixelBuffer width:(int)width height:(int)height;

- (void) releaseImage;
- (void) releasePicker;
- (void) hide;

@end

@implementation ImagePickerPlugin

static UIImagePickerController* imagePicker = nil;
static UIImage *loadedImage = nil;

static ImagePickerPlugin *pInstance = nil;

+ (ImagePickerPlugin *) instance {
	if (pInstance == nil) {
		pInstance = [[ImagePickerPlugin alloc] init];
    }
	return pInstance;
}

- (void) releaseImage {
	if (loadedImage) {
		[loadedImage release];
		loadedImage = nil;
	}
}
- (void) releasePicker {
	if (imagePicker) {
		imagePicker.delegate = nil;
		[imagePicker release];
		imagePicker = nil;
	}
    
}

- (void)dealloc {
    
	[self releaseImage];
	[self releasePicker];
    
	[super dealloc];
    
	pInstance = nil;
}

- (bool) getLoadedTexrure:(Color32 *)pixelBuffer width:(int)width height:(int)height {
    
	assert(pixelBuffer);
	if (pixelBuffer) {
		if (loadedImage) {

			// Resize
			UIGraphicsBeginImageContext(CGSizeMake(width, height));
			CGRect drawRect = CGRectMake(0, 0, width, height);
			[loadedImage drawInRect:drawRect];
			UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
			int newImageHeight = (int)newImage.size.height;
			UIGraphicsEndImageContext();
            
			// Pixel Data
			CGDataProviderRef dataProvider = CGImageGetDataProvider(newImage.CGImage);
			NSData *data = (NSData *) CFBridgingRelease(CGDataProviderCopyData(dataProvider));
			if (data) {
				assert([data length] >= (width*height*4));
				int newImagePitch = [data length] / newImageHeight / 4;
				const Color32 *pSrcBase = (const Color32 *)[data bytes];
				for (int y=0; y<height; ++y) {
					const Color32 *pSrc = &pSrcBase[newImagePitch*y];
					Color32 *pDst = &pixelBuffer[width*((height-1)-y)];
					for (int x=0; x<width; ++x) {
						pDst->r = pSrc->b;
						pDst->g = pSrc->g;
						pDst->b = pSrc->r;
						pDst->a = pSrc->a;
						++pSrc;
						++pDst;
					}
				}
			}
            
			return true;
		}
	}
	return false;
}

- (bool) showImagePicker:(const char *)sourceTypeText
{
	[self hide];
    
	[self releaseImage];
	
    [self releasePicker];
    
	sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
	
    if (strcmp(sourceTypeText, strSourceType_PhotoLibrary) == 0)
    {
		sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
	}
    else if (strcmp(sourceTypeText, strSourceType_Camera) == 0)
    {
		sourceType = UIImagePickerControllerSourceTypeCamera;
	}
    else if (strcmp(sourceTypeText, strSourceType_SavedPhotosAlbum) == 0)
    {
		sourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
	}
    
	if ([UIImagePickerController isSourceTypeAvailable:sourceType] == false)
    {
		ImagePicker_callback(strCallbackResultMessage_SourceTypeUnavailable);
	
        return false;
	}
    
	imagePicker = [[UIImagePickerController alloc] init];
	imagePicker.sourceType = sourceType;
	imagePicker.videoQuality = UIImagePickerControllerQualityType640x480;
	imagePicker.allowsEditing = NO;
	imagePicker.delegate = self;
    
	[UnityGetGLViewController() presentViewController:imagePicker animated:YES completion:^ {}];
    
	return true;
}

- (void) hide
{
	[UnityGetGLViewController() dismissViewControllerAnimated:YES completion:^ {}];
}

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info
{
	[self releaseImage];
	loadedImage = [info objectForKey:UIImagePickerControllerEditedImage];
	if (loadedImage == nil) {
		loadedImage = [info objectForKey:UIImagePickerControllerOriginalImage];
	}
	if (loadedImage) {
		[loadedImage retain];
	}
	ImagePicker_callback(strCallbackResultMessage_Loaded);
	
	[self hide];
    
	ImagePicker_callback(strCallbackResultMessage_Hidden);
	
}

- (void) imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
	[self hide];
    
    ImagePicker_callback(strCallbackResultMessage_Canceled);
}

@end

// Interface
extern "C" bool ImagePicker_showPicker(const char *sourceType, const char *callbackGameObjectName, const char *callbackMethodName) {
    
	strncpy(callbackLoadedInfo.strGameObjectName, callbackGameObjectName, sizeof(callbackLoadedInfo.strGameObjectName));
    
	strncpy(callbackLoadedInfo.strMethodName, callbackMethodName, sizeof(callbackLoadedInfo.strMethodName));
	    
    return [[ImagePickerPlugin instance] showImagePicker:sourceType];
}

extern "C" bool ImagePicker_isLoaded() {
	return (loadedImage);
}

extern "C" int ImagePicker_getLoadedTexrureWidth() {
	if (ImagePicker_isLoaded()) {
		return (int)[loadedImage size].width;
	}
	return 0;
}

extern "C" int ImagePicker_getLoadedTexrureHeight() {
	if (ImagePicker_isLoaded()) {
		return (int)[loadedImage size].height;
	}
	return 0;
}

extern "C" bool ImagePicker_getLoadedTexrure(Color32 *pixelBuffer, int width, int height) {
	return [[ImagePickerPlugin instance] getLoadedTexrure:pixelBuffer width:width height:height];
}

extern "C" void ImagePicker_release() {
	[[ImagePickerPlugin instance] release];
}

extern "C" void ImagePicker_releaseLoadedImage() {
	[[ImagePickerPlugin instance] releaseImage];
}

extern "C" void ImagePicker_savePngDataToPhotoLibrary(const char** ptrImageData, const int imageDataLength){
    
    NSData *imgData = [NSData dataWithBytes:(const void *)ptrImageData length:(sizeof(unsigned char) * imageDataLength)];
    UIImage *image = [UIImage imageWithData:imgData];
    UIImageWriteToSavedPhotosAlbum(image, nil, nil, nil);
    
}

static char *fasStrCopy(const char *str) {
	assert(str);
	char *copyStr = (char *)malloc(strlen(str)+1);
	assert(copyStr);
	strcpy(copyStr, str);
	return copyStr;
}

static void ImagePicker_callback(const char *msg) {
	UnitySendMessage(fasStrCopy(callbackLoadedInfo.strGameObjectName), fasStrCopy(callbackLoadedInfo.strMethodName), fasStrCopy(msg));
}

