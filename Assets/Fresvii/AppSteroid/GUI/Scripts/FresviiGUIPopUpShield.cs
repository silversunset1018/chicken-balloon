using UnityEngine;
using System;
using System.Collections;


using Fresvii.AppSteroid.Gui;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIPopUpShield : MonoBehaviour
    {
        private Action cancelCallback;

        public int guiDepth = -50;

        private bool on = false;

        public Texture2D palette;
        public Rect coodsBg;

        EventType cancelEventType;

        public void Enable(Action cancelCallback, EventType eventType, int guiDepth)
        {
            this.guiDepth = guiDepth;

            this.cancelCallback = cancelCallback;

            this.cancelEventType = eventType;

            on = true;

            FASGesture.Pause();
        }

        public void Enable(Action cancelCallback, int guiDepth)
        {
            this.guiDepth = guiDepth;

            this.cancelCallback = cancelCallback;

            on = true;

            FASGesture.Pause();
        }

        public void Enable(Action cancelCallback)
        {
            cancelEventType = EventType.MouseDown;

            this.cancelCallback = cancelCallback;

            on = true;

            FASGesture.Pause();
        }

        public void Done()
        {
            cancelCallback = null;

            on = false;

            FASGesture.Resume();
        }

        void OnDestroy()
        {
            FASGesture.Resume();
        }

        void OnGUI()
        {
            if (!on) return;

            GUI.depth = guiDepth;

            Rect shildRect = new Rect(0, 0, Screen.width, Screen.height);

            Event e = Event.current;

            if (shildRect.Contains(e.mousePosition) && e.type == cancelEventType)
            {
                e.Use();

                if (cancelCallback != null)
                    cancelCallback();

                on = false;

                FASGesture.Resume();
            }
        }

    }
}