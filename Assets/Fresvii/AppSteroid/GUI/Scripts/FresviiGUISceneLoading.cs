using UnityEngine;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUISceneLoading : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            Application.LoadLevelAsync(FASGui.FresviiGUISceneName);
        }       
    }
}
