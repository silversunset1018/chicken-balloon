using UnityEngine;
using System;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIPopUpTextInput : MonoBehaviour
    {
        private TouchScreenKeyboard keyboard;
        private Action<string> callback;
        private string initString;

        private FresviiGUIPopUpShield shild;

        public void Show(string text, bool multiline, Action<string> callback )
        {
            this.callback = callback;
            this.initString = text;

            shild = this.gameObject.AddComponent<FresviiGUIPopUpShield>();
            shild.Enable(Hide);

            keyboard = TouchScreenKeyboard.Open(text, TouchScreenKeyboardType.Default, false, multiline, false, false);
        }

        void Hide()
        {
            if (keyboard != null)
            {
                keyboard.active = false;

                keyboard = null;
            }

            if (shild != null)
            {
                Destroy(shild);
            }

        }

        void OnGUI()
        {
            if (keyboard == null) return;

            if (keyboard.wasCanceled)
            {
                callback(initString);
                Hide();
            }
            else if (keyboard.done)
            {
                callback(keyboard.text);
                Hide();
            }            
        }
    }
}
