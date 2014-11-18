#import "FGCLogPlugin.h"

static int stdoutSave;

static int stderrSave;

void LogStart_(const char* path){

    // Save stderr so it can be restored.
    stderrSave = dup(fileno(stderr));
   
    stdoutSave = dup(fileno(stdout));
    
    // Send stderr to our file
    freopen(path, "a", stderr);

    freopen(path, "a", stdout);
    
}

void LogSave_(){
    
    // Flush before restoring stderr
    fflush(stderr);
    
    fflush(stdout);
    
    // Now restore stderr, so new output goes to console.
    dup2(stderrSave, fileno(stderr));
    
    close(stderrSave);
    
    dup2(stdoutSave, fileno(stdout));
              
    close(stdoutSave);

    // This NSLog will go to the console.
    NSLog(@"This goes to the console");
}

