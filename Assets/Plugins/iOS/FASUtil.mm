#import "FASUtil.h"

extern "C"{
    
    int isDevelopment() {
        
#ifdef DEBUG
        return 1;
#elif Debug
        return 1;
#else
        return 0;
#endif
    }
    
    void attemptRotationToDeviceOrientation() {
        
        [UIViewController attemptRotationToDeviceOrientation];
    }
    
    const char* _FasGetVideoThumbnailImagePath(const char* videofilepath){
        
        NSString *filepath = [NSString stringWithCString: videofilepath encoding:NSUTF8StringEncoding];
        
        NSURL *url = [NSURL fileURLWithPath:filepath];
        
        AVURLAsset *asset = [[AVURLAsset alloc] initWithURL:url options:nil];
        
        AVAssetImageGenerator *generate = [[AVAssetImageGenerator alloc] initWithAsset:asset];
        
        NSError *err = NULL;
        
        CMTime time = CMTimeMake(1, 60);
        
        CGImageRef imgRef = [generate copyCGImageAtTime:time actualTime:NULL error:&err];
        
        NSLog(@"err==%@, imageRef==%@", err, imgRef);
        
        UIImage *thumbnail =  [[UIImage alloc] initWithCGImage:imgRef];
        
        NSData *imageData = UIImagePNGRepresentation(thumbnail);
        
        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
        
        NSString *documentsDirectory = [paths objectAtIndex:0];
        
        NSString *filePath = [documentsDirectory stringByAppendingPathComponent:@"TmpThumbnail.png"];
        
        //file name
        NSLog(@"filePath %@",filePath);
        [imageData writeToFile:filePath atomically:YES];
        
        const char *str = [filePath UTF8String];
        assert(str);
        char *copyStr = (char *)malloc(strlen(str)+1);
        assert(copyStr);
        strcpy(copyStr, str);
        
        return copyStr;
    }
    
    UIActivityIndicatorView *fasAi ;
    
    void _FasStartAcitvityIndicator(){
        
        if(fasAi != NULL)
            [fasAi stopAnimating];
        
        fasAi = NULL;
        
        UIViewController* parent = UnityGetGLViewController();
        
        fasAi = [[UIActivityIndicatorView alloc] init];
        fasAi.frame = CGRectMake(0, 0, 50, 50);
        fasAi.center = parent.view.center;
        fasAi.activityIndicatorViewStyle = UIActivityIndicatorViewStyleWhite;
        
        [parent.view addSubview:fasAi];
        
        [fasAi startAnimating];
    }
    
    void _FasStopAcitvityIndicator(){
        
        if(fasAi != NULL)
            [fasAi stopAnimating];
        
        fasAi = NULL;
        
    }

	void _FasSaveVideoAtPathToSavedPhotosAlbum(const char *videoPath)
	{
		NSString *outputPath = [[NSString alloc] initWithUTF8String:videoPath];

		UISaveVideoAtPathToSavedPhotosAlbum(outputPath, nil, nil, nil);
	}
}


