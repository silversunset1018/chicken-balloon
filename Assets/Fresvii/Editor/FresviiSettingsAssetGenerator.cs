using UnityEngine;
using UnityEditor;
using System.Collections;
using Fresvii.AppSteroid;

[InitializeOnLoad]
public class FresviiSettingsAssetGenerator : MonoBehaviour
{
    static FresviiSettingsAssetGenerator()
    {    
        FASConfig fasSettings = Resources.Load(FASConfig.fasSettingAssetName) as FASConfig;

        if (fasSettings == null)
        {
            // If not found, autocreate the asset object.
            fasSettings = ScriptableObject.CreateInstance<FASConfig>();

            fasSettings.appName = PlayerSettings.productName;

            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
            {
                string properPath = System.IO.Path.Combine(Application.dataPath, FASConfig.fasSettingPath);

                if (!System.IO.Directory.Exists(properPath))
                {
                    AssetDatabase.CreateFolder("Assets/Fresvii/AppSteroid", "Resources");
                }

                string fullPath = System.IO.Path.Combine(System.IO.Path.Combine("Assets", FASConfig.fasSettingPath), FASConfig.fasSettingAssetName + FASConfig.fasSettingAssetExtension);

                AssetDatabase.CreateAsset(fasSettings, fullPath);
            }
        }
    }
}
