using UnityEngine;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIBlocker : MonoBehaviour
    {


        public int guiDepth;

        void OnGUI()
        {
            GUI.depth = guiDepth;

            Event.current.Use();
        }
    }
}
