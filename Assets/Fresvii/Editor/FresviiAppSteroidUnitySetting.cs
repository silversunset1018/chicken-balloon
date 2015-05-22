using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[InitializeOnLoad]
public class FresviiAppSteroidUnitySetting
{
    private static readonly string pathFresviiGUI = "Assets/Fresvii/AppSteroid/GUI/Scenes/FresviiGUI.unity";

    private static readonly string pathFresviiGUILoading = "Assets/Fresvii/AppSteroid/GUI/Scenes/FresviiGUILoading.unity";

    private static readonly string pluginDir = System.IO.Path.Combine(Application.dataPath, "Plugins");

    private static readonly string GroupConferenceFlag = "GROUP_CONFERENCE";

    static FresviiAppSteroidUnitySetting()
    {
        // Set GUI Scenes
        var original = EditorBuildSettings.scenes;

        List<EditorBuildSettingsScene> sceneList = new List<EditorBuildSettingsScene>();

        foreach (EditorBuildSettingsScene scene in original)
        {
            sceneList.Add(scene);
        }

        var guiScene = sceneList.Find(item => item.path == pathFresviiGUI);

        if (guiScene == null)
        {
            sceneList.Add(new EditorBuildSettingsScene(pathFresviiGUI, true));
        }

        var guiLoadingScene = sceneList.Find(item => item.path == pathFresviiGUILoading);

        if (guiLoadingScene == null)
        {
            sceneList.Add(new EditorBuildSettingsScene(pathFresviiGUILoading, true));
        }

        EditorBuildSettings.scenes = sceneList.ToArray();

        // Set Complie Flags
        {
            string vcLibPath = System.IO.Path.Combine(pluginDir, "Android" + System.IO.Path.DirectorySeparatorChar + "libvoicechat.so");

            bool isGroupConference = System.IO.File.Exists(vcLibPath);

            string flags = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

            if (isGroupConference)
            {
                if (!flags.Contains(GroupConferenceFlag))
                {
                    flags += ";" + GroupConferenceFlag;

                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, flags);
                }
            }
            else
            {
                if (flags.Contains(GroupConferenceFlag))
                {
                    flags = flags.Replace(GroupConferenceFlag, "");

                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, flags);
                }
            }
        }

        {
            string vcLibPath = System.IO.Path.Combine(pluginDir, "iOS" + System.IO.Path.DirectorySeparatorChar + "libvoicechat.a");

            bool isGroupConference = System.IO.File.Exists(vcLibPath);

#if UNITY_5
            string flags = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
#else
            string flags = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone);
#endif

            if (isGroupConference)
            {
                if (!flags.Contains(GroupConferenceFlag))
                {
                    flags += ";" + GroupConferenceFlag;
#if UNITY_5
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, flags);
#else
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone, flags);
#endif
                }
            }
            else
            {
                if (flags.Contains(GroupConferenceFlag))
                {
                    flags = flags.Replace(GroupConferenceFlag, "");
#if UNITY_5
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, flags);
#else
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iPhone, flags);
#endif
                }
            }
        }
    }
}
