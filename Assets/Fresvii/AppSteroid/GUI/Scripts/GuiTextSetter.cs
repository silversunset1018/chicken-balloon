using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{
    public class GuiTextSetter : MonoBehaviour
    {
		public Font defaultFont;

        public FontStyle fontStyle;

        // Use this for initialization
        void Awake()
        {	
            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Text text = this.gameObject.GetComponent<Text>();

				text.font = defaultFont;

                text.fontStyle = fontStyle;

				string str = text.text;

				text.text = str;
            }
        }
    }
}