using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUISegmentedControl : MonoBehaviour
    {
        private Action<int> OnTapped;

        public GUIStyle guiStyleLabel;

        private Color activeColor;

        private Color negativeColor;

        private List<string> labels;

        private int selectedIndex = 0;

        public int SelectedIndex {get {return selectedIndex;} protected set {selectedIndex = value;}}

        private Texture2D buttonActive, buttonNegative;

        private float scaleFactor;

        private bool touching;

        private bool isActive;

        public Color buttonLabelNegative, buttonLabelActive;

        public void Init(float scaleFactor, string postFix, List<string> labels, Action<int> OnTapped)
        {
            this.labels = labels;

            this.OnTapped = OnTapped;

            this.scaleFactor = scaleFactor;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleLabel.font = null;

                guiStyleLabel.fontStyle = FontStyle.Bold;
            }

            guiStyleLabel.fontSize = (int)(guiStyleLabel.fontSize * scaleFactor);

            this.buttonActive = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button04HTextureName + postFix, false);

            this.buttonNegative = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button04TextureName + postFix, false);
        }

        public void EventProc(Rect position, Event e)
        {
            if (labels.Count == 0) return;

            for (int i = 0; i < labels.Count; i++)
            {
                Rect buttonRect = new Rect(position.x + i * position.width / labels.Count, position.y, position.width / labels.Count, position.height);

                bool hitContain = (e.button == 0) && buttonRect.Contains(e.mousePosition);

                if (e.type == EventType.MouseDown && hitContain)
                {
                    touching = true;
                }

                if (FASGesture.IsDragging)
                {
                    touching = false;
                }

                if (e.type == EventType.MouseUp && hitContain && touching)
                {
                    e.Use();

                    OnTapped(i);

                    selectedIndex = i;
                }
            }
        }

        public void Draw(Rect position, Event e)
        {
            if (labels.Count == 0) return;

            for (int i = 0; i < labels.Count; i++)
            {
                Rect buttonRect = new Rect(position.x + i * position.width / labels.Count, position.y, position.width / labels.Count, position.height);

                FresviiGUIUtility.DrawPosition pos = FresviiGUIUtility.DrawPosition.Center;

                if (i == 0) pos = FresviiGUIUtility.DrawPosition.Left;
                else if (i == labels.Count- 1) pos = FresviiGUIUtility.DrawPosition.Right;

                FresviiGUIUtility.DrawSplitTexture(buttonRect, (i == selectedIndex) ? buttonActive : buttonNegative, scaleFactor * 4.0f, scaleFactor * 4.0f, scaleFactor * 4.0f, scaleFactor * 4.0f, pos);

                guiStyleLabel.normal.textColor = ((i == selectedIndex) ? buttonLabelActive : buttonLabelNegative);

                GUI.Label(buttonRect, labels[i], guiStyleLabel);

                bool hitContain = (e.button == 0) && buttonRect.Contains(e.mousePosition);

                if (e.type == EventType.MouseDown && hitContain)
                {
                    touching = true;
                }

                if (FASGesture.IsDragging)
                {
                    touching = false;
                }

                if (e.type == EventType.MouseUp && hitContain && touching)
                {
                    e.Use();

                    OnTapped(i);

                    selectedIndex = i;
                }
            }
        }
    }
}
