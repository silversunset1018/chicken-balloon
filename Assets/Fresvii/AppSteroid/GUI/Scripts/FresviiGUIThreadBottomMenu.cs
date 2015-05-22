using UnityEngine;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIThreadBottomMenu : MonoBehaviour
    {
        private Texture palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        private Rect menuRect;

        private Texture2D addButton;
        private Texture2D addButtonH;
        private Texture2D textFiled;
        private Texture2D sendButton;
        private Texture2D sendButtonD;
        private Texture2D sendButtonH;

        public float height;

        public float miniMargin = 6;
        public float hMargin = 8;
        public float margin = 12;

        private float scaleFactor;
        
        public GUIStyle guiStyleAddButton;
        private Rect addButtonPosition;
        private Rect addButtonHitPosition;

        public GUIStyle guiStyleTextFiled;
		
		public GUIStyle guiStyleTextFiledLabel;
        
        public GUIStyle guiStyleSendButton;

        private string textInputField;

        private FresviiGUIThread frameThread;

		private Texture2D clipImage;

		public Vector2 clipImageMaxSize;

        private TouchScreenKeyboard keyboard;

        public Vector2 sendButtonSize;
        private Rect sendButtonPosition;
        private Rect textFieldPosition;

        private Color buttonTextEnableColor;
        private Color buttonTextUnableColor;
        private Color buttonTextHitColor;

		private Rect loadingSpinnerPosition = new Rect();

        private string postFix = "";

        private Fresvii.AppSteroid.Models.Video video = null;

        public void Init(string postFix, float scaleFactor, FresviiGUIThread frameThread)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleAddButton.font = null;
                guiStyleSendButton.font = null;
                guiStyleTextFiled.font = null;
                guiStyleTextFiledLabel.font = null;
            }

            this.frameThread = frameThread;

            this.scaleFactor = scaleFactor;

            this.postFix = postFix;

            palette = FresviiGUIColorPalette.Palette;

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.TextfieldBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.TextfieldTopLine);

            this.addButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconAddButtonTextureName + postFix, false);

            this.addButtonH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconAddButtonHTextureName + postFix, false);

            this.sendButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button04TextureName + postFix, false);

            this.sendButtonD = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button04DTextureName + postFix, false);

            this.sendButtonH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button04HTextureName + postFix, false);

            this.textFiled = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.TextFiledTextureName + postFix, false);

            height *= scaleFactor;
            
            margin *= scaleFactor;
            miniMargin *= scaleFactor;
            hMargin *= scaleFactor;
            sendButtonSize *= scaleFactor;

            guiStyleTextFiled.fontSize = (int)(guiStyleTextFiled.fontSize * scaleFactor);
            
            guiStyleSendButton.fontSize = (int)(guiStyleSendButton.fontSize * scaleFactor);
            
            guiStyleTextFiled.contentOffset *= scaleFactor;

			guiStyleAddButton.normal.background = addButton;

            guiStyleAddButton.active.background = guiStyleAddButton.hover.background = guiStyleAddButton.focused.background = addButtonH;

            textInputField = "";

            buttonTextUnableColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TextfieldSendTextUnable);

            buttonTextEnableColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TextfieldSendTextEnable);
            
            buttonTextHitColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TextfieldSendTextTapped);

        }

        public int GuiDepth { get; set; }

        private bool imageLoading;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpiner;

        private Fresvii.AppSteroid.Gui.ActionSheet actionSheet;

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(menuRect, palette, texCoordsMenu);

            GUI.DrawTextureWithTexCoords(new Rect(menuRect.x, menuRect.y - 1, Screen.width, 1), palette, texCoordsBorderLine);

            GUI.BeginGroup(menuRect);

            Event e = Event.current;

            if (!imageLoading)
            {
                if (clipImage == null)
                {
                    GUI.DrawTexture(addButtonPosition, addButton, ScaleMode.ScaleToFit);
                }
                else
                {
                    GUI.DrawTexture(addButtonPosition, clipImage, ScaleMode.ScaleAndCrop);
                }
            }

            if (!imageLoading)
            {
                if (GUI.Button(addButtonHitPosition, "", GUIStyle.none))
                {
                    List<string> buttons = new List<string>();

                    buttons.Add(FresviiGUIText.Get("TakePhoto"));

                    buttons.Add(FresviiGUIText.Get("ChoosePhoto"));

					buttons.Add(FresviiGUIText.Get("ChooseMovie"));

                    if (clipImage != null)
                        buttons.Add(FresviiGUIText.Get("CancelPhoto"));

					if (video != null)
						buttons.Add(FresviiGUIText.Get("CancelVideo"));

                    actionSheet = Fresvii.AppSteroid.Gui.ActionSheet.Show(scaleFactor, postFix, this.GuiDepth - 10, buttons.ToArray(), (selectedButton) =>
                    {                        
                        if (selectedButton == FresviiGUIText.Get("TakePhoto")) // TakePhoto
                        {
                            imageLoading = true;

							loadingSpinnerPosition = new Rect(menuRect.x + addButtonPosition.x, menuRect.y + addButtonPosition.y, addButtonPosition.width, addButtonPosition.height);

                            loadingSpiner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GUI.depth);

                            Fresvii.AppSteroid.Util.ImagePicker.Show(this, Fresvii.AppSteroid.Util.ImagePicker.Type.Camera, delegate(Texture2D loadedTexture)
                            {
                                imageLoading = false;

                                loadingSpiner.Hide();

                                if (loadedTexture != null)
                                {
                                    if (clipImage != null)
                                    {
                                        Destroy(clipImage);
                                        clipImage = null;
                                    }

                                    clipImage = loadedTexture;

                                    video = null;
                                }
                            });
                        }
                        else if (selectedButton == FresviiGUIText.Get("ChoosePhoto")) // Choose Photo from library
                        {
                            imageLoading = true;

                            loadingSpiner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(new Rect(menuRect.x + addButtonPosition.x, menuRect.y + addButtonPosition.y, addButtonPosition.width, addButtonPosition.height), GUI.depth);                           

                            Fresvii.AppSteroid.Util.ImagePicker.Show(this, Fresvii.AppSteroid.Util.ImagePicker.Type.Gallery, delegate(Texture2D loadedTexture)
                            {
                                imageLoading = false;

                                loadingSpiner.Hide();

                                if (loadedTexture != null)
                                {
                                    if (clipImage != null)
                                    {
                                        Destroy(clipImage);
                                        clipImage = null;
                                    }

                                    clipImage = loadedTexture;

                                    video = null;
                                }
                            });
                        }
						else if (selectedButton == FresviiGUIText.Get("ChooseMovie")) // Choose Video from library
                        {
                            imageLoading = true;

                            loadingSpiner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(new Rect(menuRect.x + addButtonPosition.x, menuRect.y + addButtonPosition.y, addButtonPosition.width, addButtonPosition.height), GUI.depth);

                            FresviiGUIVideoList.Show(frameThread, frameThread.GuiDepth - 500, delegate(Fresvii.AppSteroid.Models.Video _video, Texture2D loadedTexture)
                            //ImagePicker.ShowMoviePicker(this, delegate(string path, Texture2D loadedTexture)
                            {
                                imageLoading = false;

                                loadingSpiner.Hide();

                                if (loadedTexture != null)
                                {
                                    if (clipImage != null)
                                    {
                                        Destroy(clipImage);

                                        clipImage = null;
                                    }

                                    clipImage = loadedTexture;

                                    this.video = _video;
                                }
                            });
                        }
                        else if (selectedButton == FresviiGUIText.Get("CancelPhoto")) // Cancel photo
                        {
                            if (clipImage != null)
                            {
                                Destroy(clipImage);
                                
                                clipImage = null;
                                
                                video = null;
                            }
                        }
						else if (selectedButton == FresviiGUIText.Get("CancelVideo")) // Cancel video
						{
							if (clipImage != null)
							{
								Destroy(clipImage);
								
								clipImage = null;
								
								video = null;
							}
						}
                    });
                }
            }

            FresviiGUIUtility.DrawButtonFrame(textFieldPosition, textFiled, scaleFactor);

#if UNITY_EDITOR

            textInputField = GUI.TextField(textFieldPosition, textInputField, guiStyleTextFiled);

#elif UNITY_ANDROID || UNITY_IOS

            if (e.type == EventType.MouseUp && textFieldPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            //if (GUI.Button(textFieldPosition, textInputField, guiStyleTextFiled))
            {
                e.Use();
    
                FresviiGUIPopUpShield shield = gameObject.GetComponent<FresviiGUIPopUpShield>();

                if(shield == null)
                {
                    shield = gameObject.AddComponent<FresviiGUIPopUpShield>();
                }

				shield.Enable(delegate ()
                { 
					if(keyboard != null)
					{
						keyboard.active = false;

						keyboard.text = "";
						
                        keyboard = null;
					}			
				});

                keyboard = TouchScreenKeyboard.Open(textInputField, TouchScreenKeyboardType.Default, false, true, false, false);
            }

            if (keyboard != null)
            {
                textInputField = keyboard.text;

				if(keyboard.done || keyboard.wasCanceled || !keyboard.active)
				{
					gameObject.GetComponent<FresviiGUIPopUpShield>().Done();

					keyboard = null;
				}
            }

            GUI.Label(textFieldPosition, textInputField, guiStyleTextFiled);


#endif

            bool sendEnable = (clipImage != null) || !string.IsNullOrEmpty(textInputField);

            guiStyleSendButton.normal.textColor = (sendEnable) ? buttonTextEnableColor : buttonTextUnableColor;

            Texture2D sendButtonTexture = (sendEnable) ? sendButton : sendButtonD;
            
            if (sending)
            {
                sendButtonTexture = sendButtonH;

                guiStyleSendButton.normal.textColor = buttonTextHitColor;
            }

            FresviiGUIUtility.DrawButtonFrame(sendButtonPosition, sendButtonTexture, scaleFactor);

            GUI.Label(sendButtonPosition, FresviiGUIText.Get("send"), guiStyleSendButton);

            if(e.type == EventType.MouseUp && sendButtonPosition.Contains(e.mousePosition) && sendEnable && !FASGesture.IsDragging)
            {
                e.Use();

                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });

                    return;
                }

                frameThread.AddComment(frameThread.Thread.Id, textInputField, clipImage, video);

                StartCoroutine(SendingAnimation());

                textInputField = "";

                if(keyboard != null)
					keyboard.text = "";
				
                keyboard = null;

                clipImage = null;

				video = null;
            }

            GUI.EndGroup();
        }

        private bool sending = false;

        IEnumerator SendingAnimation()
        {
            sending = true;

            yield return new WaitForSeconds(0.5f);

            sending = false;
        }

        public void ClearTextInputField()
        {
            textInputField = "";
        }

        void OnDisable()
        {
            if (clipImage != null)
            {
                Destroy(clipImage);
            }

            guiStyleAddButton.normal.background = guiStyleAddButton.active.background = guiStyleAddButton.hover.background = guiStyleAddButton.focused.background = addButton;

            if (loadingSpiner != null)
            {
                loadingSpiner.Hide();
            }

            if (actionSheet != null)
            {
                actionSheet.Hide();
            }
        }

		void Update()
		{
			if(loadingSpiner != null)
			{
         		loadingSpiner.Position = new Rect(menuRect.x + addButtonPosition.x, menuRect.y + addButtonPosition.y, addButtonPosition.width, addButtonPosition.height);
			}

            menuRect = new Rect(frameThread.Position.x, frameThread.Position.y + Screen.height - height - FresviiGUIFrame.OffsetPosition.y, Screen.width, height);

            if (clipImage == null)
            {
                addButtonPosition = new Rect(miniMargin, hMargin, addButton.width, height - 2f * hMargin);
            }
            else
            {
                addButtonPosition = new Rect(miniMargin, 0.5f * (height - addButton.width), addButton.width, addButton.width);
            } 
            
            addButtonHitPosition = new Rect(0, 0, addButton.width + 2f * miniMargin, height);
            sendButtonPosition = new Rect(menuRect.width - miniMargin - sendButtonSize.x, hMargin, sendButtonSize.x, sendButtonSize.y);
            textFieldPosition = new Rect(addButtonPosition.x + addButtonPosition.width + miniMargin, hMargin, Screen.width - 4f * miniMargin - addButtonPosition.width - sendButtonPosition.width, sendButtonSize.y);

		}

        void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                imageLoading = false;

                if(loadingSpiner != null)
                    loadingSpiner.Hide();
            }
        }

		void OnDestroy(){
			if(clipImage != null)
				Destroy(clipImage);
		}
    }
}