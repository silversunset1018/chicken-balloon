using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fresvii.AppSteroid
{
    public class FASSettings : FASConfig
    {
        private static FASSettings instance;

        public static FASSettings Settings
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load(fasSettingAssetName) as FASSettings;

                    if (instance == null)
                    {
                        // If not found, autocreate the asset object.
                        instance = CreateInstance<FASSettings>();
#if UNITY_EDITOR
                        string properPath = Path.Combine(Application.dataPath, fasSettingPath);

                        if (!Directory.Exists(properPath))
                        {
                            AssetDatabase.CreateFolder("Assets/Fresvii/AppSteroid", "Resources");
                        }

                        string fullPath = Path.Combine(Path.Combine("Assets", fasSettingPath), fasSettingAssetName + fasSettingAssetExtension);

                        AssetDatabase.CreateAsset(instance, fullPath);
#endif
                    }

#if UNITY_PRO_LICENSE
                    instance.isProLicence = true;
#endif

#if UNITY_5
                    instance.unityVersion = 5.0f;
#else
                    instance.unityVersion = 4.6f;
#endif
                }

                return instance;
            }
        }
    }
}