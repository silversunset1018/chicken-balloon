using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fresvii.AppSteroid
{
    public class FresviiAppSteroid : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            FASConfig.Instance = FASSettings.Settings;

#if UNITY_5
            FASConfig.Instance.unityVersion = 5.0f;
#else
            FASConfig.Instance.unityVersion = 4.6f;
#endif

#if UNITY_PRO_LICENSE
            FASConfig.Instance.isProLicence = true;
#else
            FASConfig.Instance.isProLicence = false;
#endif
            if (FASConfig.Instance == null || string.IsNullOrEmpty(FASConfig.Instance.appId) || string.IsNullOrEmpty(FASConfig.Instance.secretKey))
            {
                Debug.LogError("FASSetting App id or App secretKey is null or empty.  Please input FresviiAppSteroid parameters. Menu -> Fresvii -> FAS Settings");

#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Init error", "FASSetting App id or App secretKey is null or empty.  \nPlease input FresviiAppSteroid parameters. \nMenu -> Fresvii -> FAS Settings", "OK");
#endif
                return;
            }

            if (FASConfig.Instance.appIcon == null)
            {
                FASConfig.Instance.appIcon = (Texture2D)Resources.Load(Fresvii.AppSteroid.Gui.FresviiGUIConstants.ResouceTextureFolderName + "/" + Fresvii.AppSteroid.Gui.FresviiGUIConstants.BackIconTextureName + "@3x", typeof(Texture2D));
            }

            FAS.Init();
        }
    }
}