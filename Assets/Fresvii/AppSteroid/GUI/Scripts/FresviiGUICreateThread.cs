#pragma warning disable 0414
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUICreateThread : FresviiGUIFrame
    {
        public float topMargin = 7.5f;
        public float sideMargin = 7.5f;
        public float margin = 18;
        public float miniMargin = 9;
        public float verticalMargin = 10;
        public float inputAreaHeight = 75f;
        public float inputAreaBottomHeight = 37f;
        public float clipImageSideMargin = 5f;

        public bool Initialized { get; protected set; }

        private FresviiGUICreateThreadTopMenu createThreadTopMenu;

        private Rect baseRect;
        private Rect scrollViewRect;
        private float scaleFactor;
        private string postFix;

        public Vector2 profileImageMaxSize;

        public float hMagin = 16;

        private Texture2D textFiled;

        private string inputString = "";
        public GUIStyle guiStyleTextButtonImage;
        public GUIStyle guiStyleTextButtonMovie;
        public GUIStyle guiStyleTextArea;
        public float labelHeight = 24f;

        private TouchScreenKeyboard keyboard;

        private FresviiGUIPopUpShield popUpShield;

        private Texture2D clipImage;

        public Rect clipImagePosition;

        private Texture2D chooseImageTexture;
        private Rect chooseImagePosition;

        private Texture2D chooseVideoTexture;
        private Rect chooseVideoPosition;

        private Rect textArea;

        public float buttonHeight = 100f;

        private Rect inputArea;
        private Rect inputAreaBottom;

        private Rect chooseImageButton;
		
        private Rect chooseVideoButton;

        public Color inputAreaColor;
        public Color inputAreaBottomColor;

        public Color placeHolderTextColor;
        public Color normalTextColor;

        private Texture2D closeButton;
        private Rect closeButtonPositon;
        private Rect closeButtonHitArea;

        private Fresvii.AppSteroid.Models.Video video;

        private bool showModal = false;

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            this.GuiDepth = guiDepth;

            this.scaleFactor = scaleFactor;

            createThreadTopMenu = GetComponent<FresviiGUICreateThreadTopMenu>();

            popUpShield = GetComponent<FresviiGUIPopUpShield>();

            createThreadTopMenu.Init(appIcon, postFix, scaleFactor, this);

            video = null;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTextArea.font = null;

                guiStyleTextButtonImage.font = null;

                guiStyleTextButtonMovie.font = null;
            }

            this.scaleFactor = scaleFactor;
            verticalMargin *= scaleFactor;
            margin *= scaleFactor;
            miniMargin *= scaleFactor;
            hMagin *= scaleFactor;
            labelHeight *= scaleFactor;
            topMargin = Mathf.CeilToInt(topMargin * scaleFactor);
            sideMargin = Mathf.CeilToInt(sideMargin * scaleFactor);
            inputAreaHeight = Mathf.CeilToInt(inputAreaHeight * scaleFactor);
            buttonHeight *= scaleFactor;
            inputAreaBottomHeight *= scaleFactor;
            clipImageSideMargin *= this.scaleFactor;
            
            this.textFiled = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.TextFiledWTextureName + postFix, false);

            chooseImageTexture = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconChooseImageTextureName + postFix, false);

            chooseVideoTexture = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconChooseVideoTextureName + postFix, false);
            
            closeButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconClose + postFix, false);

            guiStyleTextArea.fontSize = (int)(guiStyleTextArea.fontSize * scaleFactor);

            guiStyleTextArea.padding = FresviiGUIUtility.RectOffsetScale(guiStyleTextArea.padding, scaleFactor);

            guiStyleTextButtonImage.fontSize = guiStyleTextButtonMovie.fontSize = (int)(guiStyleTextButtonImage.fontSize * scaleFactor);

            guiStyleTextButtonImage.contentOffset *= scaleFactor;

            guiStyleTextButtonImage.padding.left = (int)(guiStyleTextButtonImage.padding.left * scaleFactor + chooseImageTexture.width);

            guiStyleTextButtonMovie.contentOffset *= scaleFactor;

            guiStyleTextButtonMovie.padding.left = (int)(guiStyleTextButtonMovie.padding.left * scaleFactor + chooseImageTexture.width);

            clipImagePosition = FresviiGUIUtility.RectScale(clipImagePosition, scaleFactor);

            inputString = "";

            Initialized = true;
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            createThreadTopMenu.enabled = on;


            if (!on && !showModal)
            {
                inputString = "";

                if (clipImage != null)
                {
                    Destroy(clipImage);
                }
            }
        }

        public bool WasCreated()
        {
            return !string.IsNullOrEmpty(inputString) || clipImage != null;
        }

        float CalcScrollViewHeight()
        {            
            // Change this method after implementing friends list

            return baseRect.height;
        }

        public void Create()
        {
            if (!string.IsNullOrEmpty(inputString) || clipImage != null)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });

                    return;
                }

                FresviiGUIForum.Instance.CreateThread(inputString, clipImage, video);

                BackToForum();

                clipImage = null;

                video = null;

				if(keyboard != null)
				{
					keyboard.active = false;

					keyboard.text = "";
					
                    keyboard = null;
				}
			}
        }

        public void BackToForum()
        {
            PostFrame.SetDraw(true);

            clipImage = null;

            video = null;

            if (keyboard != null)
            {
                keyboard.active = false;

                keyboard.text = "";

                keyboard = null;
            }

            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
            {
                this.SetDraw(false);
            });

            PostFrame.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
        }

        void CalcLayout()
        {
            
        }

        void Update()
        {
            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            this.baseRect = new Rect(0, createThreadTopMenu.height, Screen.width, Screen.height - createThreadTopMenu.height);

            inputArea = new Rect(sideMargin, topMargin, baseRect.width - 2f * sideMargin, inputAreaHeight - inputAreaBottomHeight);

            inputAreaBottom = new Rect(sideMargin, topMargin + inputArea.height, inputArea.width, inputAreaBottomHeight);

            chooseImagePosition = new Rect(sideMargin + guiStyleTextArea.padding.left, inputAreaBottom.y + 0.5f * (inputAreaBottom.height - chooseImageTexture.height), chooseImageTexture.width, chooseImageTexture.height);

            chooseImageButton = new Rect(inputAreaBottom.x, inputAreaBottom.y, inputAreaBottom.width * 0.5f, inputAreaBottom.height);

            chooseVideoPosition = new Rect(inputAreaBottom.x + 0.5f * inputAreaBottom.width + guiStyleTextArea.padding.left, inputAreaBottom.y + 0.5f * (inputAreaBottom.height - chooseImageTexture.height), chooseVideoTexture.width, chooseVideoTexture.height);

            chooseVideoButton = new Rect(inputAreaBottom.x + 0.5f * inputAreaBottom.width, inputAreaBottom.y, inputAreaBottom.width * 0.5f, inputAreaBottom.height);

            scrollViewRect = new Rect(Position.x + baseRect.x, Position.y + baseRect.y, baseRect.width, CalcScrollViewHeight());

            if (clipImage != null)
            {
                clipImagePosition = new Rect(inputArea.x + inputArea.width - clipImageSideMargin - clipImagePosition.width, inputArea.y + clipImageSideMargin, clipImagePosition.width, clipImagePosition.height);

                closeButtonPositon = new Rect(clipImagePosition.x + clipImagePosition.width - 0.5f * closeButton.width, clipImagePosition.y - closeButton.height * 0.5f, closeButton.width, closeButton.height);

                closeButtonHitArea = new Rect(closeButtonPositon.x - closeButtonPositon.width, closeButtonPositon.y - closeButton.height, 2f * closeButton.width, 2f * closeButton.height);
            }

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && !ControlLock)
            {
                BackToForum();
            }
#endif

        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.BeginGroup(scrollViewRect);

            Color tmpColor = GUI.color;

            GUI.color = inputAreaColor;

            FresviiGUIUtility.DrawMenuTopButtonFrame(inputArea, textFiled);

            GUI.color = inputAreaBottomColor;

            FresviiGUIUtility.DrawMenuBottomButtonFrame(inputAreaBottom, textFiled);

            GUI.color = tmpColor;

            Event e = Event.current;

            if (clipImage != null)
            {
                if (e.type == EventType.MouseDown && closeButtonHitArea.Contains(e.mousePosition))
                {
                    e.Use();

                    Destroy(clipImage);

                    clipImage = null;
                }
            }

            if (GUI.Button(chooseImageButton, "", guiStyleTextButtonImage))
            {
                Fresvii.AppSteroid.Util.GUIAnimation.TextButtonBold(1.0f, guiStyleTextButtonImage, this);

                if (clipImage != null)
                {
                    Destroy(clipImage);

                    clipImage = null;
                }

                Fresvii.AppSteroid.Util.ImagePicker.Show(this, Fresvii.AppSteroid.Util.ImagePicker.Type.Gallery, delegate(Texture2D loadedTexture)
                {
                    if (loadedTexture != null)
                    {
                        clipImage = loadedTexture;

                        this.video = null;
                    }
                });
            }

            #if UNITY_IOS
            if (GUI.Button(chooseVideoButton, "", guiStyleTextButtonMovie) && FASConfig.Instance.videoEnable)
            {
                Fresvii.AppSteroid.Util.GUIAnimation.TextButtonBold(1.0f, guiStyleTextButtonMovie, this);

                if (clipImage != null)
                {
                    Destroy(clipImage);

                    clipImage = null;
                }

				if(keyboard != null){
					keyboard.active = false;
				}


                showModal = true;

                FresviiGUIVideoList.Show(this, this.GuiDepth - 100, (_video, loadedTexture)=>
                {
                    showModal = false;

                    if (loadedTexture != null)
                    {
                        clipImage = loadedTexture;

                        this.video = _video;
                    }
                });
            }
            #endif

            if (e.type == EventType.MouseDown && inputArea.Contains(e.mousePosition))
            {
                e.Use();

				popUpShield.Enable(delegate() {
				
					if(keyboard != null){
						keyboard.active = false;
						keyboard.text = "";
						keyboard = null;
					}

				});

                keyboard = TouchScreenKeyboard.Open(inputString, TouchScreenKeyboardType.Default, false, true, false);
            }

            if (keyboard != null)
            {
                if (keyboard.active)
                {
                    inputString = keyboard.text;
                }
                else 
                {
                    if(string.IsNullOrEmpty(inputString))
                    {
                        guiStyleTextArea.normal.textColor = placeHolderTextColor;

                        guiStyleTextArea.alignment = TextAnchor.UpperLeft;

                        GUI.Label(inputArea, FresviiGUIText.Get("NewComment"), guiStyleTextArea);
                    }
                }

				if(keyboard.done || keyboard.wasCanceled || !keyboard.active)
				{
					gameObject.GetComponent<FresviiGUIPopUpShield>().Done();

					keyboard = null;
				}

            }
            else
            {
                if (string.IsNullOrEmpty(inputString))
                {
                    guiStyleTextArea.normal.textColor = placeHolderTextColor;

                    guiStyleTextArea.alignment = TextAnchor.UpperLeft;

                    GUI.Label(inputArea, FresviiGUIText.Get("NewComment"), guiStyleTextArea);
                }
            }

            guiStyleTextArea.normal.textColor = normalTextColor;

            if (!string.IsNullOrEmpty(inputString))
            {
                GUIContent guiContentInputString = new GUIContent(inputString);

                float inputStringHeight = guiStyleTextArea.CalcHeight(guiContentInputString, inputArea.width);

                guiStyleTextArea.alignment = (inputStringHeight > inputArea.height) ? TextAnchor.LowerLeft : TextAnchor.UpperLeft;

                GUI.Label(inputArea, guiContentInputString, guiStyleTextArea);
            }

            GUI.DrawTexture(chooseImagePosition, chooseImageTexture);

            GUI.Label(chooseImageButton, FresviiGUIText.Get("ChooseImage"), guiStyleTextButtonImage);

#if UNITY_IOS

            if (FASConfig.Instance.videoEnable)
            {
                GUI.DrawTexture(chooseVideoPosition, chooseVideoTexture);

                GUI.Label(chooseVideoButton, FresviiGUIText.Get("ChooseVideo"), guiStyleTextButtonMovie);
            }
#endif
            if (clipImage != null)
            {
                GUI.DrawTexture(clipImagePosition, clipImage, ScaleMode.ScaleAndCrop);

                GUI.DrawTexture(closeButtonPositon, closeButton);
            }
            
            GUI.EndGroup();            
        }
    }
}
