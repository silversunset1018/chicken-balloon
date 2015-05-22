using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class FresviiPluginImporter : MonoBehaviour
{
#if UNITY_5
    private static readonly string pluginDir = System.IO.Path.Combine(Application.dataPath, "Plugins");

    private static readonly string iOSDllWithVcName = "AppSteroidWithVoiceChatIOS.dll";

    private static readonly string androidWithVcDllName = "AppSteroidWithVoiceChatAndroid.dll";

    private static readonly string iOSDllName = "AppSteroidIOS.dll";

    private static readonly string androidDllName = "AppSteroidAndroid.dll";

    private static PluginImporter pluginImporter;

    static FresviiPluginImporter()
    {
        SetPlugins();
    }

    public static void SetPlugins()
    {
        PluginImporter[] importers = PluginImporter.GetAllImporters();

        foreach (PluginImporter importer in importers)
        {
            // Voice chat
            if (System.IO.Path.GetFileName(importer.assetPath) == androidWithVcDllName)
            {
                importer.SetCompatibleWithEditor(true);

                importer.SetCompatibleWithPlatform(BuildTarget.Android, true);
            }
            else if (System.IO.Path.GetFileName(importer.assetPath) == iOSDllWithVcName)
            {
                importer.SetCompatibleWithPlatform(BuildTarget.iOS, true);
            }
            else if (System.IO.Path.GetFileName(importer.assetPath) == androidDllName)
            {
                importer.SetCompatibleWithEditor(true);

                importer.SetCompatibleWithPlatform(BuildTarget.Android, true);
            }
            else if (System.IO.Path.GetFileName(importer.assetPath) == iOSDllName)
            {
                importer.SetCompatibleWithPlatform(BuildTarget.iOS, true);
            }
        }
    }
#endif
}
