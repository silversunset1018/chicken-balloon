using System;
using UnityEngine;

namespace Fresvii.AppSteroid.Gui
{
    public class PopUpBalloonMenu : MonoBehaviour
    {
        private Fresvii.AppSteroid.Gui.PopUpBalloonMenu instance;

        public int guiDepth = -10;

        public Vector2 buttonSize;

        public float tweenDuration;

        public Vector2 trianglePosition;

        public GUIStyle guiStyleButton;

        private static readonly float LineWidth = 1.0f;

        private bool show = false;

        private string[] buttons;

        private Vector2 position;

        private Texture2D textureBg;
        
        private Texture2D textureBaloonTriangle;

        private Action<string> callback;
        
        private Action<string> cancelCallback;

        private FresviiGUIPopUpShield shield;

        private float alpha = 0.0f;

        private Rect rect;

        private bool[] buttonSelected;

        private bool hiding;

        private Color bgNormal = Color.black;

        private Color bgActive = Color.white;

        private Color textColor = Color.white;

        public void Show(string[] buttons, Vector2 position, float scaleFactor, string postFix, int guiDepth, Color bgNormal, Color bgActive, Color textColor, Action<string> callback)
        {            
            this.buttons = buttons;

            this.position = position;
            
            this.buttonSize *= scaleFactor;

            this.trianglePosition *= scaleFactor;
            
            this.callback = callback;

            this.bgNormal = bgNormal;

            this.bgActive = bgActive;

            this.textColor = textColor;

            guiStyleButton.fontSize = (int)(guiStyleButton.fontSize * scaleFactor);

            guiStyleButton.padding.left = (int)(guiStyleButton.padding.left * scaleFactor);

            guiStyleButton.normal.textColor = this.textColor;

            this.textureBg = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.PopUpBalloonButtonBackgroundName + postFix, false);

            this.textureBaloonTriangle = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.PopUpBalloonButtonTriangle + postFix, false);

            shield = this.gameObject.AddComponent<FresviiGUIPopUpShield>();

            this.guiDepth = guiDepth;
            
            shield.Enable(OnCanceled, EventType.MouseUp, this.guiDepth + 1);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleButton.font = null;
            }

            buttonSelected = new bool[buttons.Length + 1];

            show = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", tweenDuration, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateAlpha"));
        }

        void OnUpdateAlpha(float value)
        {
            alpha = value;
        }

        void OnCanceled()
        {
            callback(FresviiGUIText.Get("Cancel"));

            Hide();
        }

        public void Hide()
        {
            hiding = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", tweenDuration, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateAlpha", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteHide"));
        }

        void OnCompleteHide()
        {
            show = false;

            Destroy(this.gameObject);

            if (shield != null)
                Destroy(shield);
        }

        void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (show)
                {
                    OnCompleteHide();
                }
            }
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        void OnGUI()
        {
            GUI.depth = guiDepth;

            if (!show) return;

            rect = new Rect(position.x - buttonSize.x, position.y, buttonSize.x, buttonSize.y * (buttons.Length + 1) + LineWidth * buttons.Length);

            Color tmpColor = GUI.color;

            GUI.color = new Color(bgNormal.r, bgNormal.g, bgNormal.b, alpha);

            FresviiGUIUtility.DrawButtonFrame(rect, textureBg, FresviiGUIManager.Instance.ScaleFactor);

            //  Trialgne
            GUI.DrawTexture(new Rect(rect.x + trianglePosition.x, rect.y + trianglePosition.y, textureBaloonTriangle.width, textureBaloonTriangle.height), textureBaloonTriangle);

            GUI.BeginGroup(rect);

            float vpos = 0.0f;

            for (int i = 0; i < buttons.Length; i++)
            {
                Rect buttonRect = new Rect(0, vpos, buttonSize.x, buttonSize.y);

                if (i == 0)
                {
                    GUI.color = (buttonSelected[i] ? new Color(bgActive.r, bgActive.g, bgActive.b, alpha) : new Color(bgNormal.r, bgNormal.g, bgNormal.b, alpha));

                    FresviiGUIUtility.DrawMenuTopButtonFrame(buttonRect, textureBg);
                }
                else
                {
                    GUI.color = (buttonSelected[i] ? new Color(bgActive.r, bgActive.g, bgActive.b, alpha) : new Color(bgNormal.r, bgNormal.g, bgNormal.b, alpha));
                    
                    FresviiGUIUtility.DrawMenuCeneterButtonFrame(buttonRect, textureBg);
                }

                vpos += buttonRect.height;

                GUI.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, alpha);

                GUI.DrawTexture(new Rect(buttonRect.x + guiStyleButton.padding.left, vpos, buttonSize.x - guiStyleButton.padding.left, LineWidth), textureBg);

                vpos += LineWidth;

                if (GUI.Button(buttonRect, buttons[i], guiStyleButton) && !hiding)
                {
                    buttonSelected[i] = true;

                    callback(buttons[i]);

                    Hide();
                }
            }

            {
                Rect buttonRect = new Rect(0, vpos, buttonSize.x, buttonSize.y);

                GUI.color = (buttonSelected[buttons.Length] ? new Color(bgActive.r, bgActive.g, bgActive.b, alpha) : new Color(bgNormal.r, bgNormal.g, bgNormal.b, alpha));

                FresviiGUIUtility.DrawMenuBottomButtonFrame(buttonRect, textureBg);

                GUI.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, alpha);

                if (GUI.Button(buttonRect, FresviiGUIText.Get("Cancel"), guiStyleButton) && !hiding)
                {
                    buttonSelected[buttons.Length] = true;

                    callback(FresviiGUIText.Get("Cancel"));

                    Hide();
                }
            }

            GUI.EndGroup();

            GUI.color = tmpColor;
        }
    }
}