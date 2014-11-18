#import "FGCPerisitentServicePlugin.h"
#import "FGCPersistentService.h"
#import "FGCConstraint.h"
#import "NSError+FGCFresvii.h"

char* FasMakeStringCopy (const char* string)
{
	if (string == NULL)
		return NULL;
	
	char* res = (char*)malloc(strlen(string) + 1);
	strcpy(res, string);
	return res;
}

void AddUserIdentifier_(const char* userIdentifier, const char *userToken ){
    
    NSError *error;
    
    [[FGCPersistentService sharedService] addUserIdentifier:[NSString stringWithUTF8String:userIdentifier]
                                                  userToken:[NSString stringWithUTF8String:userToken]
                                                      error:&error];
}

void DeleteUserCertificationWithUserIdentifier_(const char* userIdentifier){
    
    NSError *error;
    
    [[FGCPersistentService sharedService] deleteUserCertificationWithUserIdentifer:[NSString stringWithUTF8String:userIdentifier]
                                                                             error:&error];
}

char *UserCertifications_(){
    
    NSArray *userCertifications = [[FGCPersistentService sharedService] userCertifications];
    
    NSString *str = @"";
    int i;
    for(i = 0; i < [userCertifications count]; i++){
        
        str = [str stringByAppendingString:userCertifications[i][@"userIdentifier"]];
        str = [str stringByAppendingString:@","];
        str = [str stringByAppendingString:userCertifications[i][@"userToken"]];
        str = [str stringByAppendingString:@","];
    }
    
    return FasMakeStringCopy([str UTF8String]);
}

