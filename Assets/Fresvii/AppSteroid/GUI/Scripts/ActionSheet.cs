using UnityEngine;
using System;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{
    public class ActionSheet : MonoBehaviour
    {
        private int guiDepth = -100;

        private bool show = false;

        public Vector2 buttonSize = new Vector2(272f, 44f);

        private float scaleFactor;

        private Rect baseRect;

        private Rect drawRect;

        public float margin = 8;

        private Action<string> callback;

        public string[] buttons;
        
        public Font fontNormal, fontBold;

        public GUIStyle guiStyleButton;

        float tweenY;

        public iTween.EaseType easetype;

        public float tweenTime;

        private bool[] buttonSelected;

        public float hideDelayTime = 0.5f;

        private Texture2D textureButtonBase;

        private Texture2D textureButtonBaseL;

        public float buttonMargin = 1.0f;

        private bool tweening;

        private Texture2D palette;

        private Rect texCoordsLine;

        private Rect textureCoordsBackground;

        public event Action OnHidden;

        public static Fresvii.AppSteroid.Gui.ActionSheet Show(float scaleFactor, string postFix, int guiDepth, string[] buttons, Action<string> callback)
        {
            Fresvii.AppSteroid.Gui.ActionSheet instance = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/ActionSheet"))).GetComponent<Fresvii.AppSteroid.Gui.ActionSheet>();

            instance.Init(scaleFactor, postFix);

            instance.Show(buttons, guiDepth, callback);

            return instance;
        }

        void Init(float scaleFactor, string postFix)
        {
            this.scaleFactor = scaleFactor;

            palette = FresviiGUIColorPalette.Palette;
                                
            buttonSize *= scaleFactor;
                
            margin *= scaleFactor;
                
            buttonMargin *= scaleFactor;
                
            guiStyleButton.fontSize = (int)(guiStyleButton.fontSize * scaleFactor);

            this.textureButtonBase = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ActionSheetButtonName + postFix, false);

            this.textureButtonBaseL = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button04HTextureName + postFix, false);

            texCoordsLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.ActionSheetLine);

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.ModalBackground);
        }

        bool twoColumn = false;

        void Show(string[] buttons, int guiDepth, Action<string> callback)
        {            
            this.callback = callback;

            this.buttons = buttons;
			
            this.guiDepth = guiDepth;

            CalcLayout();

            postScreenWidth = Screen.width;
            
            show = true;
            
            FASGesture.Pause();

            buttonSelected = new bool[buttons.Length + 1];

            tweening = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("name", "PopupMenuShow", "from", drawRect.y, "to", baseRect.y, "easetype", easetype, "time", tweenTime, "onupdate", "OnUpdateTweenHeight", "oncomplete", "OnCompleteTweenHeight"));

            iTween.ValueTo(this.gameObject, iTween.Hash("name", "BgAlpha", "from", 0.0f, "to", 1.0f, "easetype", easetype, "time", tweenTime, "onupdate", "OnUpdateBgAlpha"));
        }

        void CalcLayout()
        {
            baseRect = new Rect();

            baseRect.x = margin;

            baseRect.height = (buttonSize.y + 1) * (buttons.Length) + margin + buttonSize.y + margin;

            if (baseRect.height > Screen.height)
            {
                twoColumn = true;

                baseRect.height = (buttonSize.y + 1) * Mathf.CeilToInt(buttons.Length / 2f) + margin + buttonSize.y + margin;
            }
            else
            {
                twoColumn = false;
            }

            baseRect.y = Screen.height - baseRect.height;

            baseRect.width = Screen.width - 2f * margin;           

            drawRect = new Rect(baseRect);

            drawRect.y = Screen.height + baseRect.height;
        }

        void OnUpdateTweenHeight(float value)
        {
            drawRect.y = value;           
        }

        void OnCompleteTweenHeight()
        {
            tweening = false;
        }

        public void Hide()
        {
            iTween.StopByName("PopupMenuShow");

            iTween.StopByName("BgAlpha");

            hiding = true;

            tweening = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("name", "PopupMenuHide", "from", drawRect.y, "to", Screen.height + baseRect.height, "delay", hideDelayTime, "easetype", easetype, "time", tweenTime, "onupdate", "OnUpdateTweenHeight", "oncomplete", "OnCompleteHide"));

            iTween.ValueTo(this.gameObject, iTween.Hash("name", "BgAlpha", "from", bgAlpha, "to", 0.0f, "easetype", easetype, "time", tweenTime, "onupdate", "OnUpdateBgAlpha", "delay", hideDelayTime));
        }

        void OnCompleteHide()
        {
            hiding = false;
            
            show = false;

            tweening = false;
            
            FASGesture.Resume();

            if (OnHidden != null)
                OnHidden();

            Destroy(this.gameObject);
        }

        void OnUpdateBgAlpha(float value)
        {
            bgAlpha = value;
        }

        private bool hiding;

        private float bgAlpha;

        private float postScreenWidth = 0f;

        public void OnGUI()
        {
			GUI.depth = guiDepth;

            if (!show) return;

            if (postScreenWidth != Screen.width)
            {
                CalcLayout();

                postScreenWidth = Screen.width;
            }

            Rect backgroundRect = new Rect(0f, 0f, Screen.width, Screen.height);

            Color tmp = GUI.color;

            GUI.color = new Color(tmp.r, tmp.g, tmp.b, bgAlpha);

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.color = tmp;

            baseRect.width = Screen.width - 2f * margin;

            buttonSize.x = baseRect.width;
            
            baseRect.width = Screen.width - 2f * margin;
            
            drawRect.width = Screen.width - 2f * margin;

            if (!tweening)
            {
                baseRect.y = Screen.height - baseRect.height;

                drawRect.y = baseRect.y;
            }

            GUI.BeginGroup(drawRect);

            float vpos = 0;

            if (Application.platform == RuntimePlatform.Android)
            {
                guiStyleButton.font = fontNormal;
            }
            else
            {
                guiStyleButton.font = null;
                
                guiStyleButton.fontStyle = FontStyle.Normal;
            }

            bool twoSide = false;

            for (int i = 0; i < buttons.Length; i++)
            {
                Rect buttonRect;

                if (twoColumn)
                {
                    buttonRect = new Rect((twoSide) ? (buttonSize.x - margin) * 0.5f + margin : 0, vpos, (buttonSize.x - margin) * 0.5f, buttonSize.y);

                    if (buttons.Length == 1)
                    {
                        FresviiGUIUtility.DrawButtonFrame(buttonRect, buttonSelected[i] ? textureButtonBaseL : textureButtonBase, scaleFactor);

                        vpos += buttonRect.height;
                    }
                    else if (i == 0 || i == Mathf.CeilToInt((buttons.Length - 1) / 2 + 1))
                    {
                        FresviiGUIUtility.DrawMenuTopButtonFrame(buttonRect, buttonSelected[i] ? textureButtonBaseL : textureButtonBase);

                        vpos += buttonRect.height;

                        GUI.DrawTextureWithTexCoords(new Rect(buttonRect.x, vpos, buttonSize.x, 1), palette, texCoordsLine);

                        vpos += 1;
                    }
                    else if (i == buttons.Length - 1 || i == Mathf.CeilToInt((buttons.Length -1) / 2 ) )
                    {
                        FresviiGUIUtility.DrawMenuBottomButtonFrame(buttonRect, buttonSelected[i] ? textureButtonBaseL : textureButtonBase);

                        if(i == Mathf.CeilToInt((buttons.Length -1) / 2 ))
                        {
                            vpos = 0;

                            twoSide = true;
                        }
                        else
                        {
                            vpos += buttonRect.height;

                            vpos += 1;

                            if (buttons.Length % 2 == 1)
                            {
                                vpos += buttonRect.height;

                                vpos += 1;
                            }
                        }
                    }
                    else
                    {
                        FresviiGUIUtility.DrawMenuCeneterButtonFrame(buttonRect, buttonSelected[i] ? textureButtonBaseL : textureButtonBase);

                        vpos += buttonRect.height;

                        GUI.DrawTextureWithTexCoords(new Rect(buttonRect.x, vpos, buttonSize.x, 1), palette, texCoordsLine);

                        vpos += 1;
                    }
                }
                else
                {
                    buttonRect = new Rect(0, vpos, buttonSize.x, buttonSize.y);

                    if (buttons.Length == 1)
                    {
                        FresviiGUIUtility.DrawButtonFrame(buttonRect, buttonSelected[i] ? textureButtonBaseL : textureButtonBase, scaleFactor);

                        vpos += buttonRect.height;
                    }
                    else if (i == 0)
                    {
                        FresviiGUIUtility.DrawMenuTopButtonFrame(buttonRect, buttonSelected[i] ? textureButtonBaseL : textureButtonBase);

                        vpos += buttonRect.height;

                        GUI.DrawTextureWithTexCoords(new Rect(buttonRect.x, vpos, buttonSize.x, 1), palette, texCoordsLine);

                        vpos += 1;
                    }
                    else if (i == buttons.Length - 1)
                    {
                        FresviiGUIUtility.DrawMenuBottomButtonFrame(buttonRect, buttonSelected[i] ? textureButtonBaseL : textureButtonBase);

                        vpos += buttonRect.height;

                        vpos += 1;
                    }
                    else
                    {
                        FresviiGUIUtility.DrawMenuCeneterButtonFrame(buttonRect, buttonSelected[i] ? textureButtonBaseL : textureButtonBase);

                        vpos += buttonRect.height;

                        GUI.DrawTextureWithTexCoords(new Rect(buttonRect.x, vpos, buttonSize.x, 1), palette, texCoordsLine);

                        vpos += 1;
                    }
                }

                if(GUI.Button(buttonRect, buttons[i], guiStyleButton) && !hiding)
                {
                    buttonSelected[i] = true;

                    callback(buttons[i]);
                    
                    Hide();
                }

            }

            vpos += margin;

            if (Application.platform == RuntimePlatform.Android)
            {
                guiStyleButton.font = fontBold;
            }
            else
            {
                guiStyleButton.font = null;
            
                guiStyleButton.fontStyle = FontStyle.Bold;
            }

            {
                Rect buttonRect = new Rect(0, vpos, buttonSize.x, buttonSize.y);

                FresviiGUIUtility.DrawButtonFrame(buttonRect, buttonSelected[buttons.Length] ? textureButtonBaseL : textureButtonBase, scaleFactor);

                if (GUI.Button(buttonRect, FresviiGUIText.Get("Cancel"), guiStyleButton) && !hiding)
                {
                    buttonSelected[buttons.Length] = true;

                    callback(FresviiGUIText.Get("Cancel"));

                    Hide();
                }
            }

            GUI.EndGroup();

            if (GUI.Button(backgroundRect, "", GUIStyle.none) && !hiding)
            {
                Hide();
            }
        }
    }
}
