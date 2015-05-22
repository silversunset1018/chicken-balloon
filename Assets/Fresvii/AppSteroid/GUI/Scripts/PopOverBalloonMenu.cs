using UnityEngine;
using System;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{
    public class PopOverBalloonMenu : MonoBehaviour
    {
        public string Name { get; set; }

        private int guiDepth = -100;

        private float scaleFactor;

        private Action<string> callback;

        private string[] buttons;
        
        public GUIStyle guiStyleButton;

        public iTween.EaseType easetype;

        public float tweenTime;

        private bool[] buttonSelected;

        public float hideDelayTime = 0.5f;

        private Texture2D textureButton;

        private Texture2D textureTriangle;

        public float buttonMargin = 1.0f;

        //private Rect texCoordsLine;

        private Rect textureCoordsBackground;

        private Vector2 position;

        private Rect[] buttonPositions;

        private GUIContent[] guiContents;

        private Rect trianglePosition;

        private Color normalColor;

        private Color activeColor;

        public static Fresvii.AppSteroid.Gui.PopOverBalloonMenu Show(float scaleFactor, string postFix, int guiDepth, Vector2 position, string[] buttons, Action<string> callback)
        {
            Fresvii.AppSteroid.Gui.PopOverBalloonMenu instance = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/PopOverBalloonMenu"))).GetComponent<Fresvii.AppSteroid.Gui.PopOverBalloonMenu>();

            instance.scaleFactor = scaleFactor;

            instance.buttonMargin *= scaleFactor;

            instance.guiStyleButton.fontSize = (int)(instance.guiStyleButton.fontSize * scaleFactor);

            instance.textureButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.PopOverBalloonButton + postFix, false);

            instance.textureTriangle = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.PopOverBalloonButtonTriangle + postFix, false);

            //instance.texCoordsLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.PopOverMenuLine);

            instance.guiStyleButton.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.PopOverMenuText);
            
            instance.callback = callback;

            instance.buttons = buttons;

            instance.guiDepth = guiDepth;

            instance.position = position;

            instance.normalColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.PopOverMenuNormal);

            instance.activeColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.PopOverMenuActive);

            instance.guiStyleButton.padding = FresviiGUIUtility.RectOffsetScale(instance.guiStyleButton.padding, scaleFactor);

            FASGesture.Pause();

            instance.buttonSelected = new bool[buttons.Length];

            instance.buttonPositions = new Rect[buttons.Length];

            instance.guiContents = new GUIContent[buttons.Length];

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                instance.guiStyleButton.font = null;
            }

            instance.CalcLayout();

            instance.Show();

            return instance;
        }

        private void Show()
        {
            iTween.ValueTo(this.gameObject, iTween.Hash("name", "alpha", "from", 0.0f, "to", 1.0f, "easetype", easetype, "time", tweenTime, "onupdate", "OnUpdatealpha"));
        }

        public void Hide()
        {
            iTween.StopByName("PopupMenuShow");

            iTween.StopByName("alpha");

            hiding = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("name", "alpha", "from", alpha, "to", 0.0f, "easetype", easetype, "time", tweenTime, "onupdate", "OnUpdatealpha", "delay", hideDelayTime, "oncomplete", "OnCompleteHide"));

			callback(null);            
        }

        void OnCompleteHide()
        {
            hiding = false;
            
            FASGesture.Resume();

            Destroy(this.gameObject);
        }

        void OnUpdatealpha(float value)
        {
            alpha = value;
        }

        private bool hiding;

        private float alpha;

        public void SetPosition(Vector2 position)
        {
            this.position = position;

            CalcLayout();
        }

        void CalcLayout()
        {
            trianglePosition = new Rect(position.x - 0.5f * textureTriangle.width, position.y - textureTriangle.height, textureTriangle.width, textureTriangle.height);

            float width = 0f;

            float height = 0f;

            for(int i = 0; i < buttons.Length; i++)
            {
                guiContents[i] = new GUIContent(buttons[i]);

                Vector2 buttonSize = guiStyleButton.CalcSize(guiContents[i]);

                buttonPositions[i] = new Rect(0f, 0f, buttonSize.x, buttonSize.y);

                width += buttonSize.x;

                height = Mathf.Max(height, buttonSize.y);
            }

            float xPos = position.x - 0.5f * width;

            float yPos = trianglePosition.y - height;

            for (int i = 0; i < buttons.Length; i++)
            {
                buttonPositions[i] = new Rect(xPos, yPos, buttonPositions[i].width, height);

                xPos += buttonPositions[i].width;
            }
        }

        public void OnGUI()
        {
			GUI.depth = guiDepth;

            Color tmp = GUI.color;

            GUI.color = new Color(tmp.r, tmp.g, tmp.b, alpha);

            Rect backgroundRect = new Rect(0f, 0f, Screen.width, Screen.height);

            for (int i = 0; i < buttons.Length; i++)
            {
                Color color = (buttonSelected[i]) ? new Color(activeColor.r, activeColor.g, activeColor.b, alpha) : new Color(normalColor.r, normalColor.g, normalColor.b, alpha);

                GUI.color = color;

                Rect buttonRect = buttonPositions[i];

                if (buttons.Length == 1)
                {
                    FresviiGUIUtility.DrawButtonFrame(buttonRect, textureButton, scaleFactor);
                }
                else if (i == 0)
                {
                    FresviiGUIUtility.DrawButtonFrame(buttonRect, textureButton, scaleFactor);
                }
                else if (i == buttons.Length - 1)
                {
                    FresviiGUIUtility.DrawButtonFrame(buttonRect, textureButton, scaleFactor);
                }
                else
                {
                    FresviiGUIUtility.DrawButtonFrame(buttonRect, textureButton, scaleFactor);
                }

                GUI.color = new Color(tmp.r, tmp.g, tmp.b, alpha);

                if(GUI.Button(buttonRect, guiContents[i], guiStyleButton) && !hiding)
                {
                    buttonSelected[i] = true;

                    callback(buttons[i]);
                    
                    Hide();
                }
            }

            GUI.color = new Color(normalColor.r, normalColor.g, normalColor.b, alpha);

            GUI.DrawTexture(trianglePosition, textureTriangle);

            if (GUI.Button(backgroundRect, "", GUIStyle.none) && !hiding)
            {
                Hide();
            }

            GUI.color = tmp;
        }
    }
}
