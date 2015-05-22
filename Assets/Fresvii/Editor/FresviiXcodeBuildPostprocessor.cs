#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

public class FresviiXcodeBuildPostprocessor : MonoBehaviour {

	internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
	{
		if (Directory.Exists(dstPath))
			Directory.Delete(dstPath);
		if (File.Exists(dstPath))
			File.Delete(dstPath);
		
		Directory.CreateDirectory(dstPath);
		
		foreach (var file in Directory.GetFiles(srcPath))
			File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
		
		foreach (var dir in Directory.GetDirectories(srcPath))
			CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
	}
	
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path) {
		
#if UNITY_5
        if (buildTarget == BuildTarget.iOS)
#else
		if (buildTarget == BuildTarget.iPhone)
#endif
        {
			string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
			
			PBXProject proj = new PBXProject();

			proj.ReadFromString(File.ReadAllText(projPath));
			
			string target = proj.TargetGuidByName("Unity-iPhone");

			// Add custom system frameworks. Duplicate frameworks are ignored.
			// needed by our native plugin in Assets/Plugins/iOS
			proj.AddFrameworkToProject(target, "Security.framework", false /*not weak*/);
			proj.AddFrameworkToProject(target, "Social.framework", false /*not weak*/);
			proj.AddFrameworkToProject(target, "Accounts.framework", false /*not weak*/);
			proj.AddFrameworkToProject(target, "MediaPlayer.framework", false /*not weak*/);
			proj.AddFrameworkToProject(target, "MessageUI.framework", false /*not weak*/);
			proj.AddFrameworkToProject(target, "MobileCoreServices.framework", false /*not weak*/);

			proj.AddFrameworkToProject(target, "libc++.dylib", false /*not weak*/);

			// Add our framework directory to the framework include path
			//proj.SetBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
			//proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");
			
			// Set a custom link flag
			proj.AddBuildProperty(target, "GCC_PREPROCESSOR_DEFINITIONS", "$(CONFIGURATION) $(inherited)");
			proj.AddBuildProperty(target, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");

			File.WriteAllText(projPath, proj.WriteToString());
		}

		// edit Info.plist
		/*var plistPath = Path.Combine(path, "Info.plist");
		
		var plist = new PlistDocument();

		plist.ReadFromFile(plistPath);
		
		plist.root.SetString("UIBackgroundModes", "voip");
		
		plist.WriteToFile(plistPath);*/

	}
}
#endif
