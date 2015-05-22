using UnityEngine;
using UnityEditor;
using System.Collections;

#if !FAS_DEVELOPMENT
[InitializeOnLoad]
#endif
public class FresviiSwitchPlatformProc
{
    private static readonly string dataDir = Application.dataPath + System.IO.Path.DirectorySeparatorChar + "Fresvii/Editor";

    private static readonly string pluginDir = System.IO.Path.Combine(Application.dataPath, "Plugins");

    private static readonly string iOSDllWithVcName = "AppSteroidWithVoiceChatIOS.dll";

    private static readonly string androidWithVcDllName = "AppSteroidWithVoiceChatAndroid.dll";

    private static readonly string iOSDllName = "AppSteroidIOS.dll";

    private static readonly string androidDllName = "AppSteroidAndroid.dll";

    static FresviiSwitchPlatformProc()
    {
        EditorUserBuildSettings.activeBuildTargetChanged += OnChangePlatform;

        OnChangePlatform();
    }

    public static void OnChangePlatform()
    {
#if UNITY_5
        if (!(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS))
#else
        if (!(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone))
#endif
        {
            EditorUtility.DisplayDialog("Platform is invalid", "AppSteroid requires iOS or Android platform.", "OK");

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);

            return;
        }


#if !UNITY_5
        bool isIOS = (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone);

        string vcLibPath = System.IO.Path.Combine(pluginDir, (isIOS) ? "iOS" + System.IO.Path.DirectorySeparatorChar + "libvoicechat.a" : "Android" + System.IO.Path.DirectorySeparatorChar + "libvoicechat.so");

        bool isGroupConference = System.IO.File.Exists(vcLibPath);

        string dllName = "AppSteroid" + (isGroupConference ? "WithVoiceChat" : "") + (isIOS ? "IOS" : "Android") ;

        string dataPath = System.IO.Path.Combine(dataDir, dllName + ".bytes");

        string tgtPluginDir = System.IO.Path.Combine(pluginDir, (isIOS ? "iOS" : "Android"));

        string dllPath = System.IO.Path.Combine(tgtPluginDir, dllName + ".dll");

        // Delete old DLL
        string iOSDllWithVcDest = System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "Plugins" + System.IO.Path.DirectorySeparatorChar + "iOS"), iOSDllWithVcName);

        if(System.IO.File.Exists(iOSDllWithVcDest))
            System.IO.File.Delete(iOSDllWithVcDest);

        string androidDllWithVcDest = System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "Plugins" + System.IO.Path.DirectorySeparatorChar + "Android"), androidWithVcDllName);

        if(System.IO.File.Exists(androidDllWithVcDest))
            System.IO.File.Delete(androidDllWithVcDest);

        string iOSDllDest = System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "Plugins" + System.IO.Path.DirectorySeparatorChar + "iOS"), iOSDllName);

        if(System.IO.File.Exists(iOSDllDest))
            System.IO.File.Delete(iOSDllDest);

        string androidDllDest = System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "Plugins" + System.IO.Path.DirectorySeparatorChar + "Android"), androidDllName);

        if(System.IO.File.Exists(androidDllDest))
            System.IO.File.Delete(androidDllDest);

        // Generate DLL

        byte[] fileData = System.IO.File.ReadAllBytes(dataPath);

        byte[] bytes = new byte[fileData.Length - 128];

        System.Array.Copy(fileData, 128, bytes, 0, bytes.Length);

        System.IO.File.WriteAllBytes(dllPath, bytes);

        AssetDatabase.Refresh();
#endif

    }
}
