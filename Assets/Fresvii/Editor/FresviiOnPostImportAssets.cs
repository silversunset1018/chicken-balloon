#pragma warning disable 0414
using UnityEngine;
using UnityEditor;
using System.Collections;

public class FresviiOnPostImportAssets : AssetPostprocessor
{
    private static readonly string pluginDir = System.IO.Path.Combine(Application.dataPath, "Plugins");

    private static readonly string iOSDllWithVcName = "AppSteroidWithVoiceChatIOS.dll";

    private static readonly string androidWithVcDllName = "AppSteroidWithVoiceChatAndroid.dll";

    private static readonly string iOSDllName = "AppSteroidIOS.dll";

    private static readonly string androidDllName = "AppSteroidAndroid.dll";

    private static readonly string[] deleteFiles = { "Fresvii/AppSteroid/GUI/Scripts/Gesture.cs", "Fresvii/AppSteroid/GUI/Scripts/FresviiGUIText.cs"};

    private static readonly string[] deleteDirs = { "Fresvii/Editor/Xcode-for-Unity"};

    private static readonly string CompileFlags = "GROUP_CONFERENCE";

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
    {
        bool isAppSteroidPackage = false;

        foreach (string asset in importedAssets)
        {
            if (asset.IndexOf("FresviiAppSteroid") >= 0)
            {
                isAppSteroidPackage = true;

                break;
            }
        }

        if (!isAppSteroidPackage) return;

#if UNITY_5
        FresviiPluginImporter.SetPlugins();
#else

        // Delete old dirs
        foreach (string deleteDir in deleteDirs)
        {
            string pathDeleteDir = System.IO.Path.Combine(Application.dataPath, deleteDir);

            if (System.IO.Directory.Exists(pathDeleteDir))
            {
                System.IO.Directory.Delete(pathDeleteDir);
            }
        }

        //  Delete old files
        foreach (string deleteFile in deleteFiles)
        {
            string pathDeleteFile = System.IO.Path.Combine(Application.dataPath, deleteFile);

            if (System.IO.File.Exists(pathDeleteFile))
            {
                System.IO.File.Delete(pathDeleteFile);
            }
        }

        int dllCount = 0;

        if (System.IO.File.Exists(System.IO.Path.Combine(pluginDir, iOSDllWithVcName))) dllCount++;
        
        if (System.IO.File.Exists(System.IO.Path.Combine(pluginDir, androidWithVcDllName))) dllCount++;
        
        if (System.IO.File.Exists(System.IO.Path.Combine(pluginDir, iOSDllName))) dllCount++;
        
        if (System.IO.File.Exists(System.IO.Path.Combine(pluginDir, androidDllName))) dllCount++;

        if (dllCount > 1)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(pluginDir, iOSDllWithVcName)))
            {
                System.IO.File.Delete(System.IO.Path.Combine(pluginDir, iOSDllWithVcName));

                dllCount--;
            }

            if (dllCount > 1)
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(pluginDir, androidWithVcDllName)))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(pluginDir, androidWithVcDllName));

                    dllCount--;
                }
            }

            if (dllCount > 1)
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(pluginDir, iOSDllName)))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(pluginDir, iOSDllName));

                    dllCount--;
                }
            }
        }

        FresviiSwitchPlatformProc.OnChangePlatform();
#endif
    }
}
