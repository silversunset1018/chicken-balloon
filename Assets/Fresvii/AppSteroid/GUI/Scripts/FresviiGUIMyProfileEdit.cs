using UnityEngine;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIMyProfileEdit : FresviiGUIFrame
    {
        public float sideMargin = 8;

        public float topMargin = 32;
        public float hMargin = 18;
        public float miniMargin = 9;
        public float profileImageBgMargin = 1;
        public float verticalMargin = 10;

        public bool Initialized { get; protected set; }

        private FresviiGUIMyProfileEditTopMenu myProfileEditTopMenu;
        private FresviiGUITabBar tabBar;

        private Rect baseRect;
        private Rect scrollViewRect;
        private float scaleFactor;
        private string postFix;

        private Vector2 scrollPosition = Vector2.zero;

        private Texture2D palette;
        private Rect texcoodsBg;

        public Vector2 myProfileImageSize;
        public Texture2D textureMyProfileMask;
        private Texture2D textureMyProfileCircle;
        private Texture2D textureButtonFriend;
        private Texture2D textureButtonMessage;

        public GUIStyle guiStyleLabelUserName;
        public GUIStyle guiStyleLabelUserDescription;

        public Vector2 profileImageMaxSize;

        private Fresvii.AppSteroid.Models.User me;

        private Texture2D textFiled;

        public GUIStyle guiStyleLabel;
        public GUIStyle guiStyleTextFiled;
        public float labelHeight = 24f;

        private TouchScreenKeyboard keyboard;

        private enum Inputing { None, UserName, Description };
        private Inputing currentInputing = Inputing.None;
        private FresviiGUIMyProfile guiMyProfile;

        private FresviiGUIPopUpShield popUpShield;

        private Texture2D userProfileImage;

        private Color bgColor;

        public Texture2D textureUserProfileMask;

        public int guiDepth = -100;
        public int blockerGUIDepth = -300;

        public float hMarginRate = 0.05f;

		private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        public Vector2 profileImageSize;

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = guiDepth;

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            bgColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.MainBackground);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleLabel.font = null;
                guiStyleLabelUserDescription.font = null;
                guiStyleLabelUserName.font = null;
                guiStyleTextFiled.font = null;
            }

            palette = FresviiGUIColorPalette.Palette;

            myProfileEditTopMenu = GetComponent<FresviiGUIMyProfileEditTopMenu>();
            popUpShield = GetComponent<FresviiGUIPopUpShield>();

            myProfileEditTopMenu.Init(appIcon, postFix, scaleFactor, this);            

            this.scaleFactor = scaleFactor;
            sideMargin *= scaleFactor;
            verticalMargin *= scaleFactor;
            miniMargin *= scaleFactor;
            profileImageBgMargin *= scaleFactor;
            myProfileImageSize *= scaleFactor;
            labelHeight *= scaleFactor;
            topMargin *= scaleFactor;
			loadingSpinnerSize *= scaleFactor;
            profileImageSize *= scaleFactor;

            guiStyleLabelUserName.fontSize = (int)(guiStyleLabelUserName.fontSize * scaleFactor);
            guiStyleLabelUserDescription.fontSize = (int)(guiStyleLabelUserDescription.fontSize * scaleFactor);
            guiStyleLabelUserDescription.padding.right = (int)( guiStyleLabelUserDescription.padding.right * scaleFactor);
            guiStyleLabelUserName.padding.right = (int)(guiStyleLabelUserName.padding.right * scaleFactor);

            texcoodsBg = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            textureMyProfileCircle = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.UserCircleTextureName + postFix, false);

            this.textFiled = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.TextFiledTextureName + postFix, false);

            guiStyleTextFiled.fontSize = (int)(guiStyleTextFiled.fontSize * scaleFactor);

            guiStyleTextFiled.contentOffset *= scaleFactor;

            guiStyleLabel.fontSize = (int)(guiStyleLabel.fontSize * scaleFactor);

            scrollPosition = Vector2.zero;

            myProfileEditTopMenu.GuiDepth = GuiDepth - 1;

            Initialized = true;
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            myProfileEditTopMenu.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                {
                    loadingSpinner.Hide();
                }
            }
        }


        void Update()
        {
            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            baseRect = new Rect(Position.x, Position.y + myProfileEditTopMenu.height, Screen.width, Screen.height - myProfileEditTopMenu.height - FresviiGUIFrame.OffsetPosition.y);

            scrollViewRect = new Rect(baseRect.x, baseRect.y, baseRect.width, baseRect.height);

            hMargin = baseRect.height * hMarginRate;

            if ((FASGesture.IsDragging && baseRect.Contains(FASGesture.TouchPosition)) || (FASGesture.Inertia && baseRect.Contains(FASGesture.DragEndPosition)))
            {
                scrollPosition -= FASGesture.Delta;

                if (scrollPosition.y < -scrollViewRect.height + sideMargin)
                {
                    scrollPosition.y = -scrollViewRect.height + sideMargin;
                }
                else if (scrollPosition.y > baseRect.height + scrollViewRect.height - sideMargin)
                {
                    scrollPosition.y = baseRect.height + scrollViewRect.height - sideMargin;
                }
            }

			if(loadingSpinner != null)
			{
        		loadingSpinner.Position = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
			}

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && !ControlLock)
            {
                BackToMyProfile();
            }
#endif
        }
        

        float CalcScrollViewHeight()
        {            
            // Change this method after implementing friends list

            return Screen.height;
        }

        private static string inputUsername = "";
        private static string inputDescription = "";

        public void SetCurrentUserProfile(Fresvii.AppSteroid.Models.User user, Texture2D userProfileImage)
        {
            this.me = user;

            this.userProfileImage = userProfileImage;
            
            inputUsername = me.Name;
         
            inputDescription = me.Description;
        }

        public void SetGUIMyProfile(FresviiGUIMyProfile guiEdit)
        {
            this.guiMyProfile = guiEdit;
        }

        public void BackToMyProfile()
        {
            if (loadedTexture != null)
                Destroy(loadedTexture);

            if (loadedUserIcon != null)
                Destroy(loadedUserIcon);

            guiMyProfile.SetDraw(true);

            this.Tween(Vector2.zero, new Vector2(0.0f, Screen.height), delegate()
            {
                this.SetDraw(false);
            });
        }

		public Vector2 loadingSpinnerSize;

        public void Submit()
        {
			loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y), FASGui.GuiDepthBase - 100);

            FresviiGUIBlocker blocker = this.gameObject.AddComponent<FresviiGUIBlocker>();

            blocker.guiDepth = FASGui.GuiDepthBase + blockerGUIDepth;

            FASUser.PatchAccount(inputUsername, inputDescription, (loadedTexture != null) ? loadedTexture : null, delegate(Fresvii.AppSteroid.Models.User user, Fresvii.AppSteroid.Models.Error error)
            {
				loadingSpinner.Hide();

                Destroy(blocker);

				Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                if (error == null)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ProfileUpdateSuccess"), delegate(bool del) { });

                    guiMyProfile.SetUserProfile(inputUsername, inputDescription, loadedUserIcon);
                    
                    FresviiGUIMyProfile.currentUser = user;

                    guiMyProfile.SetDraw(true);

                    this.Tween(Vector2.zero, new Vector2(0.0f, Screen.height), delegate()
                    {
                        this.SetDraw(false);
                    });

                    Destroy(loadedTexture);

                    loadedTexture = null;

                    loadedUserIcon = null;
                }
                else
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ProfileUpdateError"), delegate(bool del) { });
                }
            });
        }

        public bool IsChanged()
        {
            if(me == null) return false;

            return me.Name != inputUsername || me.Description != inputDescription || (loadedTexture != null);
        }

        public bool ProfileImageIsChanged()
        {
            return (loadedTexture != null);
        }

        private Texture2D loadedTexture;

        private Texture2D loadedUserIcon;

        void OnGUI()
        {
            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.depth = GuiDepth;

            //  Background Mat
            GUI.DrawTextureWithTexCoords(new Rect(Position.x, Position.y, Screen.width, Screen.height), palette, texcoodsBg);

            if (keyboard != null)
            {
                if (keyboard.active)
                {
                    if (currentInputing == Inputing.UserName)
                        inputUsername = keyboard.text;
                    else if (currentInputing == Inputing.Description)
                        inputDescription = keyboard.text;
                }
                else
                {
                    currentInputing = Inputing.None;
                }
            }
            else
            {
                currentInputing = Inputing.None;
            }


            GUI.BeginGroup(scrollViewRect);

            //	User Image
            Rect userImagePosition = new Rect(Screen.width * 0.5f - profileImageSize.x * 0.5f, hMargin, profileImageSize.x, profileImageSize.y);

            //	User Image
            GUI.DrawTexture(userImagePosition, (loadedUserIcon == null) ? userProfileImage : loadedUserIcon, ScaleMode.ScaleAndCrop);

            Color tmp = GUI.color;

            GUI.color = bgColor;

            GUI.DrawTexture(userImagePosition, textureUserProfileMask, ScaleMode.ScaleToFit);

            GUI.color = tmp;

            GUI.DrawTexture(userImagePosition, textureMyProfileCircle);

            Event e = Event.current;

            if (e.type == EventType.MouseDown && userImagePosition.Contains(e.mousePosition))
            {
                Fresvii.AppSteroid.Util.ImagePicker.Show(this, Fresvii.AppSteroid.Util.ImagePicker.Type.Gallery, delegate(Texture2D _loadedTexture){

                    loadedTexture = _loadedTexture;

                    if (loadedTexture != null)
                    {
                        if (loadedTexture.width > profileImageMaxSize.x || loadedTexture.height > profileImageMaxSize.y)
                        {
                            if (loadedUserIcon != null)
                            {
                                Destroy(loadedUserIcon);
                            }

                            Texture2D shrinkedTexture = new Texture2D(128, 128);

                            shrinkedTexture.SetPixels32(Fresvii.AppSteroid.Util.ScaleUnityTexture.ScaleAndSquareCropLnaczos(loadedTexture.GetPixels32(), loadedTexture.width, loadedTexture.height, 128));

                            shrinkedTexture.Apply();

                            //loadedUserIcon = TextureLoader.LoadIntoTextureAlpha(shrinkedTexture, textureMyProfileMask);
                            //Destroy(shrinkedTexture);

                            loadedUserIcon = shrinkedTexture;
                            
                            Debug.Log("shrink " + shrinkedTexture == null);
                        }
                        else
                        {
                            Debug.Log("not shrink " + loadedTexture == null);

                            loadedUserIcon = loadedTexture;
                        }
                    }
                });
            }

            float vPos = userImagePosition.y + userImagePosition.height;

            //---- username
            vPos += hMargin;

            //  Username text field
            Rect userNameTextFieldRect = new Rect(sideMargin, vPos, baseRect.width - 2f * sideMargin, labelHeight);

            if (GUI.Button(userNameTextFieldRect, "", GUIStyle.none) && currentInputing == Inputing.None)
            {
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    popUpShield.Enable(OnKeyboardCanceled);
                    currentInputing = Inputing.UserName;
                    keyboard = TouchScreenKeyboard.Open(inputUsername, TouchScreenKeyboardType.Default, false, false, false, false);
                }
            }

            Rect textFieldUsernameRect = userNameTextFieldRect;

            FresviiGUIUtility.DrawButtonFrame(userNameTextFieldRect, textFiled, scaleFactor);

            textFieldUsernameRect.x += guiStyleTextFiled.padding.left;
            textFieldUsernameRect.width -= 2f * guiStyleTextFiled.padding.left;

            GUI.Label(textFieldUsernameRect, inputUsername, guiStyleLabelUserName);

            vPos += textFieldUsernameRect.height + hMargin;

            //----description            
            Rect descriptionTextFieldRect = new Rect(sideMargin, vPos, baseRect.width - 2f * sideMargin, baseRect.height - vPos - hMargin);

            descriptionTextFieldRect.height = (Screen.width > Screen.height) ? baseRect.height - vPos - hMargin : baseRect.height * 0.336f;

            if (GUI.Button(descriptionTextFieldRect, "", GUIStyle.none) && currentInputing == Inputing.None)
            {
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    currentInputing = Inputing.Description;
                    popUpShield.Enable(OnKeyboardCanceled);
                    keyboard = TouchScreenKeyboard.Open(inputDescription, TouchScreenKeyboardType.Default, false, true, false, false);
                }
            }

            FresviiGUIUtility.DrawButtonFrame(descriptionTextFieldRect, textFiled, scaleFactor);

            descriptionTextFieldRect.x += guiStyleTextFiled.padding.left;

            descriptionTextFieldRect.width -= 2f * guiStyleTextFiled.padding.left;

            string text = (string.IsNullOrEmpty(inputDescription) && currentInputing != Inputing.Description) ? FresviiGUIText.Get("Description") : inputDescription;

            GUI.Label(descriptionTextFieldRect, text, guiStyleLabelUserDescription);
            
            GUI.EndGroup();           
        }

        private void OnKeyboardCanceled()
        {
            if (keyboard != null)
            {
                keyboard.active = false;
            }

            currentInputing = Inputing.None;
        }
    }
}
