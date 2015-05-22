using UnityEngine;
using System;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIThreadPopUpMenu : MonoBehaviour
    {
        public float sideMargin;
        public float topMargin;
        public float margin;
        public float hMargin;

        public static readonly string PopUpButtonName = "button_black";

        public Vector2 buttonSize;
        public GUIStyle guiStyleButton;

        private Texture2D popUpButtonTexture;
        private Texture2D popUpBaloonTriangleTexture;

        public int guiDepth = -10;

        private Vector2 position;
        public Vector2 offset;

        bool show;

        bool isMine;

        bool isSubscribed;

        private FresviiGUIPopUpShield shield;

        Action deleteCallback;
        Action subscribeCallback;
        Action unsubscribeCallback;
        Action cancelCallback;

        bool initialized;

        int buttonCount = 1;

        public void Show(float scaleFactor, bool isMine, bool isSubscribed, Vector2 position, Action deleteCallback, Action subscribeCallback, Action unsubscribeCallback, Action cancelCallback)
        {
            this.deleteCallback = deleteCallback;
            this.subscribeCallback = subscribeCallback;
            this.unsubscribeCallback = unsubscribeCallback;
            this.cancelCallback = cancelCallback;
            this.isMine = isMine;
            this.isSubscribed = isSubscribed;
            this.position = position;

            if (!initialized)
            {
                sideMargin *= scaleFactor;
                topMargin *= scaleFactor;
                margin *= scaleFactor;
                buttonSize *= scaleFactor;
                hMargin *= scaleFactor;
                offset *= scaleFactor;
                guiStyleButton.fontSize = (int)(guiStyleButton.fontSize * scaleFactor);
                guiStyleButton.padding.left = (int)(guiStyleButton.padding.left * scaleFactor);
                initialized = true;
            }

            buttonCount = (isMine) ? 3 : 2;

            show = true;

            shield = this.gameObject.AddComponent<FresviiGUIPopUpShield>();
            shield.guiDepth = guiDepth + 1;
            shield.Enable(OnCanceled);
        }

        void OnCanceled()
        {
            Hide();
        }

        void Hide()
        {
            show = false;
            cancelCallback();
            if(shield != null)
                Destroy(shield);
        }

        void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
				if(show)
	                Hide();
            }
        }

        void OnGUI()
        {
            GUI.depth = FASGui.GuiDepthBase + guiDepth;

            if (!show) return;

			Rect rect = new Rect(position.x - buttonSize.x + offset.x, position.y + offset.y, buttonSize.x, buttonSize.y * buttonCount + hMargin * (buttonCount - 1));

            GUILayout.BeginArea(rect);

            float hPos = 0;

            GUILayout.Space(hPos);

            GUILayout.BeginHorizontal();

            GUILayout.Space(rect.width - buttonSize.x - sideMargin);

            if (isSubscribed)
            {
                if (GUILayout.Button(FresviiGUIText.Get("Unsubscribe"), guiStyleButton, GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
                {
                    unsubscribeCallback();

                    Hide();
                }
            }
            else
            {
                if (GUILayout.Button(FresviiGUIText.Get("Subscribe"), guiStyleButton, GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
                {
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
    					Debug.Log("Subscribe Button");

					Hide();

                    FASNotification.RegisterRemoteNotification(delegate(Fresvii.AppSteroid.Models.Error _error){

						if(_error != null)
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("RegisterError"), delegate(bool del) {});
							
                            return;
						}

						subscribeCallback();

					});
                }
            }

            GUILayout.Space(sideMargin);

			GUILayout.EndHorizontal();

            GUILayout.Space(hMargin);

            if (isMine)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Space(rect.width - buttonSize.x - sideMargin);

                if (GUILayout.Button(FresviiGUIText.Get("Delete"), guiStyleButton, GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
                {
                    deleteCallback();

                    Hide();
                }

                GUILayout.Space(sideMargin);

                GUILayout.EndHorizontal();

                GUILayout.Space(hMargin);
            }
			
			GUILayout.BeginHorizontal();
			
				GUILayout.Space(rect.width - buttonSize.x - sideMargin);
				
				if (GUILayout.Button(FresviiGUIText.Get("Cancel"), guiStyleButton, GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
				{
					cancelCallback();
					
					Hide();                    
				}
				
				GUILayout.Space(sideMargin);

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            
        }
    }
}