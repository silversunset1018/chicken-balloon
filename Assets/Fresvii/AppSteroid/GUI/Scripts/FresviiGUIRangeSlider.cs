using UnityEngine;
using System;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIRangeSlider : MonoBehaviour
    {
        public event Action OnValueChanged;

        public float Value = 0.5f;

        public int GuiDepth = 0;

        public GUISkin guiSkin;

        public GUIStyle guiStyleLabel;

        public string Title = "";

        void Awake()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiSkin.label.font = null;
            }

            guiStyleLabel.fontSize = (int)(guiStyleLabel.fontSize * FresviiGUIManager.Instance.ScaleFactor);
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            Event e = Event.current;

            float length = Screen.width / 1.618f;

            Rect safeArea = new Rect(0.5f * (Screen.width - length), Screen.height * 0.5f - length / 1.618f * 0.25f, length, length / 1.618f * 0.5f);

            if(e.type == EventType.MouseDown && !safeArea.Contains(e.mousePosition))
            {
                e.Use();

                this.enabled = false;
            }

            GUI.Box(safeArea, "", guiSkin.box);

            GUI.BeginGroup(safeArea);

            Rect sliderPosition = new Rect(safeArea.width * 0.1f, safeArea.height / 1.618f, safeArea.width * 0.8f, 15f * FresviiGUIManager.Instance.ScaleFactor);

            Value = GUI.HorizontalSlider(sliderPosition, Value, 0f, 1f, guiSkin.verticalSlider, guiSkin.verticalSliderThumb);

            Rect labelPosition = new Rect(safeArea.width * 0.1f, 0f, safeArea.width * 0.8f, safeArea.height - sliderPosition.height);

            GUI.Label(labelPosition, Title, guiStyleLabel);

            GUI.EndGroup();

            if (GUI.changed)
            {
                if (OnValueChanged != null)
                {
                    OnValueChanged();
                }
            }
        }
    }
}
