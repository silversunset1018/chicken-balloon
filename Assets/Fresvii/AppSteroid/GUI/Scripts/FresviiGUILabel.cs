using UnityEngine;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{

    public class FresviiGUILabel : MonoBehaviour
    {

        private Rect position;
        private bool on;
        private string text;
        private GUIStyle guiStyle;
        private int guiDepth;

        public void SetLabel(int guiDepth, Rect position, string text, GUIStyle guiStyle)
        {
            this.position = position;
            this.text = text;
            this.guiStyle = (guiStyle == null) ? GUIStyle.none : guiStyle;
            this.guiDepth = guiDepth;
        }

        public void SetEnable(bool on)
        {
            this.on = on;
        }

        // Update is called once per frame
        void OnGUI()
        {

            if (!on) return;

            GUI.depth = guiDepth;

            GUI.Label(position, text, guiStyle);
        }
    }
}