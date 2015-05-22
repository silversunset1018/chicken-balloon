using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
	public class FresviiGUIDirectMessage : FresviiGUIModalFrame
    {
        private Texture2D palette;

        private FresviiGUIDirectMessageTop directMessageListTopMenu;

        private Rect baseRect;
        
        private Rect scrollViewRect;
        
        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;
        
        public Vector2 loadingSpinnerSize;
                
        private FresviiGUITabBar tabBar;

        public Fresvii.AppSteroid.Models.DirectMessage DirectMessage { get; set; }

        public GUIStyle guiStyleSubject;

        public GUIStyle guiStyleText;

        private GUIContent contentSubject;

        private GUIContent contentText;

        private Rect positionSubject;

        private Rect positionText;
                
        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = guiDepth;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleSubject.font = null;

                guiStyleSubject.fontStyle = FontStyle.Bold;

                guiStyleText.font = null;
            }

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.DirectMessageBackground);

            directMessageListTopMenu = GetComponent<FresviiGUIDirectMessageTop>();

			directMessageListTopMenu.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

			tabBar = GetComponent<FresviiGUITabBar>();

            tabBar.Init(postFix, scaleFactor, this.GuiDepth - 1);

            tabBar.GuiDepth = GuiDepth - 1;

            loadingSpinnerSize *= scaleFactor;

            guiStyleSubject.padding = FresviiGUIUtility.RectOffsetScale(guiStyleSubject.padding, scaleFactor);

            guiStyleText.padding = FresviiGUIUtility.RectOffsetScale(guiStyleText.padding, scaleFactor);

            guiStyleSubject.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.DirectMessageSubject);

            guiStyleText.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.DirectMessageText);

            guiStyleSubject.fontSize = (int)(guiStyleSubject.fontSize * scaleFactor);

            guiStyleText.fontSize = (int)(guiStyleText.fontSize * scaleFactor);

            loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GuiDepth - 10);

            SetScrollSlider(scaleFactor * 2.0f);
			
            FAS.Instance.Client.DirectMessageService.GetDirectMessage(DirectMessage.Id, OnGetDirectMessage);

            contentSubject = new GUIContent(DirectMessage.Subject);
		}

        public void OnUpdateScrollViewPosition(float value)
        {
            scrollViewRect.y = value;
        }

        public void OnScrollDelta(float delta)
        {
            scrollViewRect.y += delta;

            scrollViewRect.y = Mathf.Min(0f, scrollViewRect.y);
        }

        void OnGetDirectMessage(Fresvii.AppSteroid.Models.DirectMessage directMessage, Fresvii.AppSteroid.Models.Error error)
        {
            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }
           
            if (error == null)
            {
                contentText = new GUIContent(directMessage.Text);
            }
        }

		void OnEnable()
		{
            ControlLock = false;
		}

        float CalcScrollViewHeight()
        {
            if (contentText != null)
            { 
                return Mathf.Max( Screen.height - tabBar.height - directMessageListTopMenu.height, positionText.height);
            }
            else
            {
                return Screen.height - tabBar.height - directMessageListTopMenu.height;
            }
        }

        private Rect directMessageUploadedLabel;

        private Vector2 scrollPosition = Vector2.zero;

        void Update(){

            this.baseRect = new Rect(Position.x, Position.y + directMessageListTopMenu.height, Screen.width, Screen.height - directMessageListTopMenu.height - FresviiGUIFrame.OffsetPosition.y);

            backgroundRect = new Rect(0f, 0f, Screen.width, Screen.height - tabBar.height);

            float scrollViewHeight = CalcScrollViewHeight();

            scrollViewRect.x = Position.x;

            scrollViewRect.width = Screen.width;

            scrollViewRect.height = scrollViewHeight;

            if (loadingSpinner != null)
            {
				loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }

            float subjectHeight = guiStyleSubject.CalcHeight(contentSubject, Screen.width);

            positionSubject = new Rect(0f, 0f, Screen.width, subjectHeight);

            if (contentText != null)
            {
                float textHeight = guiStyleText.CalcHeight(contentText, Screen.width);

                positionText = new Rect(0f, positionSubject.height, Screen.width, textHeight);
            }
            else
            {
                positionText = new Rect(0f, positionSubject.height, Screen.width, guiStyleText.fontSize);
            }

            InertiaScrollView(ref scrollPosition, ref scrollViewRect, CalcScrollViewHeight(), baseRect, directMessageListTopMenu.height, tabBar.height);			

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && PostFrame != null && !ControlLock)
            {
                BackToPostFrame();
            }
#endif

			FASGesture.Use();
		}

        public void BackToPostFrame()
        {
            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }

            if(PostFrame != null)
                PostFrame.SetDraw(true);

            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
            {
                Destroy(this.gameObject);
            });

            if(PostFrame != null)
                PostFrame.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
        }


        public override void SetDraw(bool on)
        {
			if(FresviiGUIManager.Instance != null)
			{
	            if (on)
    	            FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;
			}

            this.enabled = on;
            
            directMessageListTopMenu.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                    loadingSpinner.Hide();
            }
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.BeginGroup(baseRect);

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.BeginGroup(scrollViewRect);

            GUI.Label(positionSubject, contentSubject, guiStyleSubject);

            if (contentText != null)
            {
                GUI.Label(positionText, contentText, guiStyleText);
            }

            GUI.EndGroup();

            GUI.EndGroup();

			if (Event.current.isMouse && Event.current.button >= 0)
			{
				Event.current.Use();
			}
        }
    }
}
	