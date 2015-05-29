#pragma warning disable 0414
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIUserProfile : FresviiGUIFrame
    {
        public float topMargin = 32;
     
        public float margin = 18;
        
        public float miniMargin = 9;
        
        public float profileImageBgMargin = 1;
        
        public float verticalMargin = 10;

        public float hMarginRate = 0.05f;
        
        private float hMargin;
        
        public float sideMargin = 8;

        public bool Initialized { get; protected set; }

        private Texture2D palette;
        
        private Rect texCoordsFriendTitleBg;

        public Rect texCoordsFriendTitleBgH;
        
        private FresviiGUIUserProfileTopMenu userProfileTopMenu;
        
        private FresviiGUITabBar tabBar;

        private Rect baseRect;
        
        private Rect scrollViewRect;
        
        private float scaleFactor;
        
        private string postFix;

        public Fresvii.AppSteroid.Models.User user;

        private bool hasError;
        
        private bool isReady;

        private Vector2 scrollPosition = Vector2.zero;

        public Vector2 profileImageSize;
		
        private Texture2D textureUserProfileDefault;

        private Texture2D textureUserProfile;
        
        public Texture2D textureUserProfileMask;
        
        private Texture2D textureUserProfileCircle;
        
        private Texture2D textureButton01;
        
        private Texture2D textureButton01H;
        
        private Texture2D textureButtonFriendH;
        
        private Texture2D textureButtonMessage;
        
        private Texture2D textureIconMessage;

#if GROUP_CONFERENCE
        private Texture2D textureIconCall;

		private Texture2D textureIconCallD;
#endif
        private Texture2D textureButton03;

        private Texture2D textureRight;

        public GUIStyle guiStyleLabelUserName;

        public GUIStyle guiStyleLabelUserProfile;

        public GUIStyle guiStyleLabelUserCode;
        
        public GUIStyle guiStyleButtonFriend;
        
        public GUIStyle guiStyleButtonLabel;
        
        public GUIStyle guiStyleFriendMenuTitle;

        public Vector2 profileImageMaxSize;

        public FresviiGUIButton buttonFriend;
        
        private Rect friendButtonPosition;
        
        public Vector2 friendButtonSize;

        public FresviiGUIButton buttonMessage;
        
        private Rect messageButtonPosition;

        public FresviiGUIButton buttonCall;

        public FresviiGUIButton buttonFriendList;

        public FresviiGUIButton buttonVideoList;

        private Rect callButtonPosition;
        
        public Vector2 messageButtonSize;

        public Vector2 buttonIconRelativePosition;
        
        public Vector2 buttonLabelRelativePosition;

        private Rect userImagePosition;
        
        private Rect userNamePosition;
        
        private Rect userDescriptionPosition;

        private Rect userCodePosition;

        private GUIContent userNameContent = new GUIContent("");
        
        private GUIContent userDescriptionContent = new GUIContent("");

        private GUIContent userCodeContent = new GUIContent("");

        private GUIContent videoListLabelContent = new GUIContent("");

        private Rect friendIconPosition;
        
        private Rect messageIconPosition;
        
        private Rect messageLabelPosition;

        private Rect callLabelPosition;

#if GROUP_CONFERENCE
        private GUIContent callLabelContent;
#endif
        private GUIContent messageLabelContent;

        private GUIContent friendCountLabelContent = null;

        private Fresvii.AppSteroid.Models.ListMeta friendsMeta;

        private Fresvii.AppSteroid.Models.ListMeta VideosMeta = null;

        public float friendMenuTitleBarHeight = 33f;
        
        private Rect friendMenuTitleBarPosition;
        
        private Rect friendMenuTitleLabelPosition;

        private Rect friendMenuRightIconPosition;
        
        private Rect friendMenuTitleUnderLinePosition;

        public float videoTitleBarHeight = 33f;

        private Rect videoTitleBarPosition;

        private Rect videoRightIconPosition;

        public GameObject prfbFriendCard;
        
        private Rect loadingSpinnerPosition;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        public Vector2 loadingSpinnerSize;
                        
        public float pullResetTweenTime = 0.5f;
        
        public iTween.EaseType pullRestTweenEaseType = iTween.EaseType.easeOutExpo;

		private bool imageLoading;

        public GameObject prfbGUIFramePairChat;

        private bool userInfoEnable = false;

		private Color btnPositiveColor, btnNegativeColor;

        private FresviiGUIFrame frameGroupConference;

        public GameObject prfbGUIFrameGroupCall;

        private FresviiGUIFrame frameFriendList;

        public GameObject prfbFriendList;

        private Color bgColor;

        Fresvii.AppSteroid.Gui.PopOverBalloonMenu popOverBalloonMenu;

        public int deleteGroupRetryCount = 5;

        public float deleteGroupRetryInterval = 5f;

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = guiDepth;
		
            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            bgColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.MainBackground);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleLabelUserName.font = null;

                guiStyleLabelUserName.fontStyle = FontStyle.Bold;
                
                guiStyleLabelUserProfile.font = null;
                
                guiStyleButtonFriend.font = null;
                
                guiStyleButtonLabel.font = null;
                
                guiStyleFriendMenuTitle.font = null;
                
                guiStyleFriendMenuTitle.fontStyle = FontStyle.Bold;

                guiStyleLabelUserCode.font = null;
            }

            userProfileTopMenu = GetComponent<FresviiGUIUserProfileTopMenu>();

            tabBar = GetComponent<FresviiGUITabBar>();

            userProfileTopMenu.Init(appIcon, postFix, scaleFactor, guiDepth - 1, this);

            tabBar.Init(postFix, scaleFactor, this.GuiDepth - 1);

            this.scaleFactor = scaleFactor;

            this.postFix = postFix;
            
            verticalMargin *= scaleFactor;
            
            margin *= scaleFactor;
            
            miniMargin *= scaleFactor;
            
            profileImageBgMargin *= scaleFactor;
            
            profileImageSize *= scaleFactor;
            
            topMargin *= scaleFactor;
            
            friendButtonSize *= scaleFactor;
            
            messageButtonSize *= scaleFactor;
            
            buttonIconRelativePosition *= scaleFactor;
            
            buttonLabelRelativePosition *= scaleFactor;
            
            sideMargin *= scaleFactor;
            
            friendMenuTitleBarHeight *= scaleFactor;
			
            loadingSpinnerSize *= scaleFactor;

            guiStyleLabelUserName.fontSize = (int)(guiStyleLabelUserName.fontSize * scaleFactor);

            guiStyleLabelUserName.normal.textColor = guiStyleButtonLabel.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileUserName);

            guiStyleLabelUserCode.fontSize = (int)(guiStyleLabelUserCode.fontSize * scaleFactor);

            guiStyleLabelUserCode.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileUserName);

            guiStyleLabelUserProfile.fontSize = (int)(guiStyleLabelUserProfile.fontSize * scaleFactor);

            guiStyleLabelUserProfile.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileDescription);

            guiStyleButtonFriend.fontSize = (int)(guiStyleButtonFriend.fontSize * scaleFactor);

            guiStyleButtonLabel.fontSize = (int)(guiStyleButtonLabel.fontSize * scaleFactor);

			btnPositiveColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileButtonText);
            
			btnNegativeColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileButtonTextL);

			guiStyleFriendMenuTitle.fontSize = (int)(guiStyleFriendMenuTitle.fontSize * scaleFactor);
            
            guiStyleFriendMenuTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileFriendBarText);

            guiStyleFriendMenuTitle.padding = FresviiGUIUtility.RectOffsetScale(guiStyleFriendMenuTitle.padding, scaleFactor);

            palette = FresviiGUIColorPalette.Palette;

#if GROUP_CONFERENCE
            callLabelContent = new GUIContent(FresviiGUIText.Get("Call"));
#endif
            messageLabelContent = new GUIContent(FresviiGUIText.Get("Message"));

            texCoordsFriendTitleBg = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.ProfileFriendBarBackground);

            texCoordsFriendTitleBgH = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.ProfileFriendBarBackgroundH);

            textureUserProfileDefault = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.UserTextureName + postFix, false);

            textureUserProfileCircle = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.UserCircleTextureName + postFix, false);
            
            textureButton01 = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button01TextureName + postFix, false);
            
            textureButton01H = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button01HTextureName + postFix, false);
            
			textureButton03 = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button03TextureName + postFix, false);

            textureIconMessage = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconProfileMessage + postFix, false);

#if GROUP_CONFERENCE
			textureIconCall = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconMessageCall + postFix, false);

			textureIconCallD = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconMessageCallD + postFix, false);
#endif
            textureRight = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.RightIconTextureName + postFix, false);
            
            tabBar.GuiDepth = GuiDepth - 1;

            Initialized = true;

            SetScrollSlider(scaleFactor * 2.0f);

            videoListLabelContent = new GUIContent(FresviiGUIText.Get("Videos"));

            GetUserVideoList();

            if(Application.internetReachability == NetworkReachability.NotReachable)
			{
				hasError = true;
				
				Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });
			}

        }

        void OnEnable()
        {
			imageLoading = false;
        }

        void GetUserVideoList()
        {
            string query = "{\"where\":[{\"collection\":\"users\", \"column\":\"id\", \"value\":\"" + user.Id + "\"}]}";

            FASVideo.GetVideoList( query, (videos, meta, error) =>
            {
                if (loadingSpinner != null)
                    loadingSpinner.Hide();

                if (error == null)
                {
                    VideosMeta = meta;

                    videoListLabelContent = new GUIContent(((meta.TotalCount > 0) ? (VideosMeta.TotalCount.ToString()) + " " : "") + FresviiGUIText.Get("Videos"));
                }
                else
                {
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                    {
                        Debug.LogError(error.ToString());
                    }
                }
            });
        }

        private float screenWidth;

        void CalcLayout()
        {
            hMargin = (baseRect.height + FresviiGUIFrame.OffsetPosition.y) * hMarginRate;

            userImagePosition = new Rect(Screen.width * 0.5f - profileImageSize.x * 0.5f, hMargin + userProfileTopMenu.height, profileImageSize.x, profileImageSize.y);

            string userNameTruncated = FresviiGUIUtility.Truncate(user.Name, guiStyleLabelUserName, Screen.width * 0.75f, "...");

            userNameContent = new GUIContent(userNameTruncated);

            userNamePosition = new Rect(Screen.width * 0.125f, userImagePosition.y + userImagePosition.height + hMargin, Screen.width * 0.75f, guiStyleLabelUserName.CalcHeight(userNameContent, Screen.width * 0.6f));

            userCodePosition = new Rect(0f, userNamePosition.y + userNamePosition.height + 0.5f * hMargin, Screen.width, userNamePosition.height);

            if (!string.IsNullOrEmpty(user.Description))
            {
                userDescriptionContent = new GUIContent(user.Description);

                userDescriptionPosition = new Rect(Screen.width * 0.2f, userCodePosition.y + userCodePosition.height + 0.5f * hMargin, Screen.width * 0.6f, guiStyleLabelUserProfile.CalcHeight(userDescriptionContent, Screen.width * 0.6f));
            }
            else
            {
                userDescriptionPosition = new Rect(0f, userCodePosition.y + userCodePosition.height + 0.5f * hMargin, 0f, 0f);
            }

            friendButtonPosition = new Rect(Screen.width * 0.5f - friendButtonSize.x * 0.5f, userDescriptionPosition.y + userDescriptionPosition.height + hMargin, friendButtonSize.x, friendButtonSize.y);

#if GROUP_CONFERENCE
            if (user.Official)
            {
                messageButtonPosition = new Rect(baseRect.width * 0.5f - messageButtonSize.x * 0.5f, friendButtonPosition.y + friendButtonPosition.height + 2f * hMargin, messageButtonSize.x, messageButtonSize.y);

                messageLabelPosition = new Rect(baseRect.width * 0.5f - baseRect.width * 0.25f, messageButtonPosition.y + messageButtonPosition.height + 0.5f * hMargin, baseRect.width * 0.5f, guiStyleButtonLabel.CalcHeight(messageLabelContent, baseRect.width * 0.5f));
            }
            else
            {
                messageButtonPosition = new Rect(baseRect.width * 0.33f - messageButtonSize.x * 0.5f, friendButtonPosition.y + friendButtonPosition.height + 2f * hMargin, messageButtonSize.x, messageButtonSize.y);

                messageLabelPosition = new Rect(baseRect.width * 0.33f - baseRect.width * 0.25f, messageButtonPosition.y + messageButtonPosition.height + 0.5f * hMargin, baseRect.width * 0.5f, guiStyleButtonLabel.CalcHeight(messageLabelContent, baseRect.width * 0.5f));

                callButtonPosition = new Rect(baseRect.width * 0.66f - messageButtonSize.x * 0.5f, friendButtonPosition.y + friendButtonPosition.height + 2f * hMargin, messageButtonSize.x, messageButtonSize.y);

                callLabelPosition = new Rect(baseRect.width * 0.66f - baseRect.width * 0.25f, callButtonPosition.y + callButtonPosition.height + 0.5f * hMargin, baseRect.width * 0.5f, guiStyleButtonLabel.CalcHeight(callLabelContent, baseRect.width * 0.5f));
            }
#else
            messageButtonPosition = new Rect(baseRect.width * 0.5f - messageButtonSize.x * 0.5f, friendButtonPosition.y + friendButtonPosition.height + 2f * hMargin, messageButtonSize.x, messageButtonSize.y);

            messageLabelPosition = new Rect(baseRect.width * 0.5f - baseRect.width * 0.25f, messageButtonPosition.y + messageButtonPosition.height + 0.5f * hMargin, baseRect.width * 0.5f, guiStyleButtonLabel.CalcHeight(messageLabelContent, baseRect.width * 0.5f));
#endif

            friendMenuTitleBarPosition = new Rect(0f, messageLabelPosition.y + messageLabelPosition.height + hMargin, baseRect.width, friendMenuTitleBarHeight);

            friendMenuTitleLabelPosition = new Rect(friendMenuTitleBarPosition.x + sideMargin, friendMenuTitleBarPosition.y, friendMenuTitleBarPosition.width, friendMenuTitleBarPosition.height);

            friendMenuRightIconPosition = new Rect(baseRect.width - sideMargin - textureRight.width, friendMenuTitleBarPosition.y + friendMenuTitleBarPosition.height * 0.5f - textureRight.height * 0.5f, textureRight.width, textureRight.height);

#if UNITY_IOS

            if (FASConfig.Instance.videoEnable)
            {
                videoTitleBarPosition = new Rect(0f, friendMenuTitleBarPosition.y + friendMenuTitleBarPosition.height + scaleFactor, baseRect.width, friendMenuTitleBarHeight);

                videoRightIconPosition = new Rect(baseRect.width - sideMargin - textureRight.width, videoTitleBarPosition.y + videoTitleBarPosition.height * 0.5f - textureRight.height * 0.5f, textureRight.width, textureRight.height);
            }

#elif UNITY_ANDROID

#endif

            if (popOverBalloonMenu != null)
            {
                if (popOverBalloonMenu.Name == "UserName")
                {
                    Vector2 postion = new Vector2(baseRect.x + scrollViewRect.x + userNamePosition.x + userNamePosition.width * 0.5f, baseRect.y + scrollViewRect.y + userNamePosition.y);

                    popOverBalloonMenu.SetPosition(postion);
                }
                else if (popOverBalloonMenu.Name == "UserCode")
                {
                    Vector2 postion = new Vector2(baseRect.x + scrollViewRect.x + userCodePosition.x + userCodePosition.width * 0.5f, baseRect.y + scrollViewRect.y + userCodePosition.y);

                    popOverBalloonMenu.SetPosition(postion);
                }
            }
        }

        float CalcScrollViewHeight()
        {
            if (user.Official)
            {
                return friendButtonPosition.y + tabBar.height;
            }
            else
            {
#if UNITY_IOS

                if (FASConfig.Instance.videoEnable)
                {
                    return videoTitleBarPosition.y + videoTitleBarPosition.height + tabBar.height;
                }
                else
                {
                    return friendMenuTitleBarPosition.y + friendMenuTitleBarPosition.height + tabBar.height;
                }
#else
                return friendMenuTitleBarPosition.y + friendMenuTitleBarPosition.height + tabBar.height;
#endif
            }
        }

		void Update(){

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            this.baseRect = new Rect(Position.x, Position.y, Screen.width, Screen.height - FresviiGUIFrame.OffsetPosition.y);

            if (user == null) return;

			if (!string.IsNullOrEmpty(user.ProfileImageUrl) && !imageLoading && textureUserProfile == null)
			{
                LoadUserIcon(user.ProfileImageUrl);
			}

            if (screenWidth != Screen.width)
            {
                CalcLayout();

                screenWidth = Screen.width;
            }

            InertiaScrollView(ref scrollPosition, ref scrollViewRect, CalcScrollViewHeight(), baseRect, userProfileTopMenu.height, tabBar.height);

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && !ControlLock)
            {
                BackToPostFrame();
            }
#endif
		}

        void OnUpdateScrollPosition(Vector2 value)
        {
            scrollPosition = value;
        }

        void OnCompletePull()
        {

        }

        public void SetUser(Fresvii.AppSteroid.Models.User user)
        {
			hasError = false;

            if (textureUserProfile != null)
                DestroyImmediate(textureUserProfile);

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(user.ProfileImageUrl, true, delegate(Texture2D tex)
            {
                textureUserProfile = tex;
            });

            this.user = user;

            userInfoEnable = false;

            FASUser.GetUser(user.Id, delegate(Fresvii.AppSteroid.Models.User _user, Fresvii.AppSteroid.Models.Error error)
            {
                if (error != null)
                {
					this.user = null;

					hasError = true;

                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });

					BackToPostFrame();

					return;
                }

                this.user = _user;

                userInfoEnable = true;

                scrollPosition = Vector2.zero;

                CalcLayout();
				                
            });

            userCodeContent = new GUIContent(this.user.UserCode);

			GetUserFriendList();
        }

		void GetUserFriendList(){

            FASFriendship.GetUserFriendList(user.Id, delegate(IList<Fresvii.AppSteroid.Models.Friend> friends, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
			{
#if GROUP_CONFERENCE
				if (loadingSpinner != null)
					loadingSpinner.Hide();
#endif
				if (error != null)
				{
                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));
                    
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });
				
					return;
				}
				else
				{
                    this.friendsMeta = meta;

                    friendCountLabelContent = new GUIContent(friendsMeta.TotalCount + " " + FresviiGUIText.Get("Friends"));
				}				
			});
		}

        private void LoadUserIcon(string url)
        {
            imageLoading = true;

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(url, true, delegate(Texture2D texture)
            {
                textureUserProfile = texture;

                imageLoading = false;
            });
        }

        void OnDestroy()
        {
            if (textureUserProfile != null)
            {
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(user.ProfileImageUrl);
            }
        }

        public void BackToPostFrame()
        {
            tabBar.enabled = false;

            PostFrame.SetDraw(true);            

            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
            {
                Destroy(this.gameObject);
            });

            PostFrame.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
        }

        public override void SetDraw(bool on)
        {
            if(on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            tabBar.enabled = on;

            userProfileTopMenu.enabled = on;

            if (!on)
            {
#if GROUP_CONFERENCE
                if (loadingSpinner != null)
                    loadingSpinner.Hide();
#endif
                scrollPosition = Vector2.zero;
            }
        }

#if GROUP_CONFERENCE
        void OnCallButtonTapped(Fresvii.AppSteroid.Models.Group group)
        {

#if !UNITY_EDITOR
            if(group.MembersCount > 4)
            {
				Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("GroupConferenceMemberLimitation"), delegate(bool del)
                {

				});

				return;
            }
#endif

            ControlLock = true;

            frameGroupConference = ((GameObject)Instantiate(prfbGUIFrameGroupCall)).GetComponent<FresviiGUIGroupConference>();

            frameGroupConference.gameObject.GetComponent<FresviiGUIGroupConference>().SetGroup(group);

            frameGroupConference.transform.parent = this.transform;

            frameGroupConference.PostFrame = this;

            frameGroupConference.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, this.GuiDepth - 10);

            frameGroupConference.SetDraw(true);

            frameGroupConference.Tween(new Vector2(0.0f, Screen.height), Vector2.zero, delegate()
            {
                this.SetDraw(false);

                ControlLock = false;
            });

        }
#endif

        private IEnumerator DeleteGroupMessgesRetryCoroutine(string userId, int retry, float retryInterval)
        {
            yield return new WaitForSeconds(retryInterval);

            DeletePairGroupMessages(userId, retry, retryInterval);
        }

        private void DeletePairGroupMessages(string userId, int retry, float retryInterval)
        {
            FASGroup.CreatePair(this.user.Id, (group, error) =>
            {
                if (error == null)
                {
                    DeleteGroupMessages(group.Id, deleteGroupRetryCount, deleteGroupRetryInterval);
                }
                else
                {
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                    {
                        Debug.LogError(error.ToString());
                    }

                    if(--retry >= 0)
                    {
                        StartCoroutine(DeleteGroupMessgesRetryCoroutine(userId, retry, retryInterval));
                    }
                }
            });
        }

        private IEnumerator DeleteGroupRetryCoroutine(string groupId, int retry, float retryInterval)
        {
            yield return new WaitForSeconds(retryInterval);

            DeleteGroupMessages(groupId, retry, retryInterval);
        }

        private FresviiGUIFrame frameVideoList;

        public GameObject prfbGUIFrameVideoList;

        private void GoToVideoList(bool animation)
        {
            frameVideoList = ((GameObject)Instantiate(prfbGUIFrameVideoList)).GetComponent<FresviiGUIFrame>();

            FresviiGUIVideoList fresviiGUIVideoList = frameVideoList.gameObject.GetComponent<FresviiGUIVideoList>();

            fresviiGUIVideoList.IsModal = false;

            fresviiGUIVideoList.mode = FresviiGUIVideoList.Mode.Share;

            frameVideoList.transform.parent = this.transform;

            fresviiGUIVideoList.user = user;

            frameVideoList.Init(null, postFix, this.scaleFactor, this.GuiDepth - 1);

            frameVideoList.SetDraw(true);

            frameVideoList.PostFrame = this;

            if (animation)
            {
                this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), moveDelayTime, delegate()
                {
                    this.SetDraw(false);
                });

                frameVideoList.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, moveDelayTime, delegate() { });
            }
            else
            {
                frameVideoList.Position = Vector2.zero;

                this.SetDraw(false);
            }
        }

        private void DeleteGroupMessages(string groupId, int retry, float retryInterval)
        {
            FASGroup.DeleteMessages(groupId, (error) =>
            {
                if (error != null)
                {
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                    {
                        Debug.LogError(error.ToString());
                    }

                    if (--retry >= 0)
                    {
                        StartCoroutine(DeleteGroupRetryCoroutine(groupId, retry, retryInterval));
                    }
                }
                else
                {
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
                    {
                        Debug.Log("Success: DeleteGroupMessages");
                    }
                }
            });
        }

        public float moveDelayTime = 0.1f;

        void OnGUI()
        {
            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

			if(user == null) return;

            if (!hasError)
            {
                GUI.depth = GuiDepth;

                GUI.BeginGroup(baseRect);

                GUI.BeginGroup(scrollViewRect);

       			//	User Image
                GUI.DrawTexture(userImagePosition, (textureUserProfile == null) ? textureUserProfileDefault : textureUserProfile, ScaleMode.ScaleToFit);

                Color tmp = GUI.color;

                GUI.color = bgColor;

                GUI.DrawTexture(userImagePosition, textureUserProfileMask, ScaleMode.ScaleToFit);

                GUI.color = tmp;

                GUI.DrawTexture(userImagePosition, textureUserProfileCircle, ScaleMode.ScaleToFit);

                //  User name
                GUI.Label(userNamePosition, userNameContent, guiStyleLabelUserName);

                if (!string.IsNullOrEmpty(user.Description))
                {
                    GUI.Label(userDescriptionPosition, userDescriptionContent, guiStyleLabelUserProfile);
                }

                if (user.Official)
                {
                    GUI.EndGroup();

                    GUI.EndGroup();

                    return;
                }

                Event e = Event.current;

                //  Holding  - menu apper
                if (userNamePosition.Contains(e.mousePosition) && FASGesture.IsHolding && !FASGesture.IsDragging && !ControlLock)
                {
                    List<string> buttons = new List<string>();

                    buttons.Add(FresviiGUIText.Get("Copy"));

                    Vector2 postion = new Vector2(baseRect.x + scrollViewRect.x + userNamePosition.x + userNamePosition.width * 0.5f, baseRect.y + scrollViewRect.y + userNamePosition.y);

                    ControlLock = true;

                    popOverBalloonMenu = Fresvii.AppSteroid.Gui.PopOverBalloonMenu.Show(scaleFactor, postFix, this.GuiDepth - 30, postion, buttons.ToArray(), (selectedButton) =>
                    {
                        ControlLock = false;

                        if (selectedButton == FresviiGUIText.Get("Copy"))
                        {
                            Fresvii.AppSteroid.Util.Clipboard.SetText(user.Name);
                        }
                    });

                    popOverBalloonMenu.Name = "UserName";
                }      

                // User Code
                GUI.Label(userCodePosition, userCodeContent, guiStyleLabelUserCode);


                //  Holding  - menu apper
                if (userCodePosition.Contains(e.mousePosition) && FASGesture.IsHolding && !FASGesture.IsDragging && !ControlLock)
                {
                    List<string> buttons = new List<string>();

                    buttons.Add(FresviiGUIText.Get("Copy"));

                    Vector2 postion = new Vector2(baseRect.x + scrollViewRect.x + userCodePosition.x + userCodePosition.width * 0.5f, baseRect.y + scrollViewRect.y + userCodePosition.y);

                    ControlLock = true;

                    popOverBalloonMenu = Fresvii.AppSteroid.Gui.PopOverBalloonMenu.Show(scaleFactor, postFix, this.GuiDepth - 30, postion, buttons.ToArray(), (selectedButton) =>
                    {
                        ControlLock = false;

                        if (selectedButton == FresviiGUIText.Get("Copy"))
                        {
                            Fresvii.AppSteroid.Util.Clipboard.SetText(user.UserCode);
                        }
                    });

                    popOverBalloonMenu.Name = "UserCode";
                }

                if (userInfoEnable)
                {
                    #region Friend button

                    if (user.FriendStatus == Fresvii.AppSteroid.Models.User.FriendStatuses.Requesting)
                    {
						guiStyleButtonFriend.normal.textColor = btnNegativeColor;

                        buttonFriend.IsTap(e, friendButtonPosition, friendButtonPosition, FresviiGUIButton.ButtonType.FrameAndLabel, textureButton03, textureButton03, textureButton03, FresviiGUIText.Get("RequestSent"), guiStyleButtonFriend);
                    }
                    /*else if (user.FriendStatus == User.FriendStatuses.Requested)
                    {
						guiStyleButtonFriend.normal.textColor = btnNegativeColor;

						buttonFriend.IsTap(e, friendButtonPosition, friendButtonPosition, FresviiGUIButton.ButtonType.FrameAndLabel, textureButton03, textureButton03, textureButton03, FresviiGUIText.Get("Requested"), guiStyleButtonFriend);
                    }*/
                    else if (user.FriendStatus == Fresvii.AppSteroid.Models.User.FriendStatuses.Friend)
                    {
						guiStyleButtonFriend.normal.textColor = btnPositiveColor;

                        if (buttonFriend.IsTap(e, friendButtonPosition, friendButtonPosition, FresviiGUIButton.ButtonType.FrameAndLabel, textureButton01, textureButton01H, textureButton01H, FresviiGUIText.Get("Unfriend"), guiStyleButtonFriend))
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Unfriend"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("ConfirmUnfriend"), delegate(bool del)
                            {
#if UNITY_EDITOR
                                if (true)
#else
                            	if(del)
#endif
                                {
                                    if (Application.internetReachability == NetworkReachability.NotReachable)
                                    {
                                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool _del) { });
                                    }
                                    else
                                    {
                                        FASFriendship.UnFriend(this.user.Id, delegate(Fresvii.AppSteroid.Models.Error error)
                                        {
                                            if (error == null)
                                            {
                                                user.SetFriendStatus(Fresvii.AppSteroid.Models.FriendshipRequest.Statuses.none.ToString());

                                                DeletePairGroupMessages(user.Id, deleteGroupRetryCount, deleteGroupRetryInterval);
                                            }
                                            else
                                            {
                                                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                                                {
                                                    Debug.LogError(error.ToString());
                                                }

                                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool _del) { });
                                            }
                                        });
                                    }
                                }
                            });

                        }
                    }
                    else
                    {
						guiStyleButtonFriend.normal.textColor = btnPositiveColor;

                        if (buttonFriend.IsTap(e, friendButtonPosition, friendButtonPosition, FresviiGUIButton.ButtonType.FrameAndLabel, textureButton01, textureButton01H, textureButton01H, FresviiGUIText.Get("AddFriend"), guiStyleButtonFriend))
                        {
                            if (Application.internetReachability == NetworkReachability.NotReachable)
                            {
                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });
                            }
                            else
                            {
                                user.SetFriendStatus(Fresvii.AppSteroid.Models.FriendshipRequest.Statuses.requesting.ToString());

                                FASFriendship.SendFriendshipRequest(this.user.Id, delegate(Fresvii.AppSteroid.Models.FriendshipRequest friendshipRequest, Fresvii.AppSteroid.Models.Error error)
                                {
                                    if (error == null)
                                    {
                                        user.SetFriendStatus(friendshipRequest.Status.ToString());
                                    }
                                    else
                                    {
                                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                                            Debug.LogError(error.ToString());

                                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });

                                        user.SetFriendStatus(Fresvii.AppSteroid.Models.FriendshipRequest.Statuses.none.ToString());
                                    }
                                });
                            }
                        }
                    }
                }
                #endregion

                #region Message button

                if (buttonMessage.IsTap(e, messageButtonPosition, messageButtonPosition, FresviiGUIButton.ButtonType.TextureOnly, textureIconMessage, textureIconMessage, textureIconMessage))                  
                {
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });
                    }
                    else
                    {
                        FresviiGUIFrame framePairChat = ((GameObject)Instantiate(prfbGUIFramePairChat)).GetComponent<FresviiGUIFrame>();

                        framePairChat.transform.parent = this.transform;

                        framePairChat.gameObject.GetComponent<FresviiGUIChat>().SetPairUser(this.user);

                        framePairChat.Init(null, postFix, scaleFactor, this.GuiDepth - 1);

                        framePairChat.SetDraw(true);

                        framePairChat.PostFrame = this;

                        this.tabBar.enabled = false;

                        this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), moveDelayTime, delegate()
                        {
                            this.SetDraw(false);
                        });

                        framePairChat.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, moveDelayTime, delegate() { });
                    }
                }

                GUI.Label(messageLabelPosition, messageLabelContent, guiStyleButtonLabel);

                #endregion

                #region Call button

#if GROUP_CONFERENCE
                if (!user.Official)
                {
                    bool isCalling = FASConference.IsCalling();

                    Texture2D textureCall = (isCalling) ? textureIconCallD : textureIconCall;

                    if (buttonMessage.IsTap(e, callButtonPosition, callButtonPosition, FresviiGUIButton.ButtonType.TextureOnly, textureCall, textureCall, textureCall))
                    {

#if !UNITY_EDITOR
					if (isCalling)
                    {
						Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

						Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("VoiceChatAlredayExists"), delegate(bool del) { });

                        return;
                    }
#endif
                        if (Application.internetReachability == NetworkReachability.NotReachable)
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });
                        }
                        else
                        {
                            loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

                            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, this.GuiDepth - 10);

                            FASGroup.CreatePair(user.Id, delegate(Fresvii.AppSteroid.Models.Group group, Fresvii.AppSteroid.Models.Error error)
                            {
                                if (error != null)
                                {
                                    loadingSpinner.Hide();

                                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                                    {
                                        Debug.LogError("CreatePair : " + error.ToString());
                                    }

                                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Error : CreatePair " + error.ToString(), delegate(bool del) { });
                                }
                                else
                                {
                                    loadingSpinner.Hide();

                                    group.FetchMembers(delegate(Fresvii.AppSteroid.Models.Error error2)
                                    {
                                        if (error2 != null)
                                        {
                                            if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                                            {
                                                Debug.LogError("FetchMembers : " + error.ToString());
                                            }

                                            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                            if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NetworkNotReachable)
                                            {
                                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });
                                            }
                                            else
                                            {
                                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(error.Detail, delegate(bool del) { });
                                            }
                                        }
                                        else
                                        {
                                            OnCallButtonTapped(group);
                                        }
                                    });
                                }
                            });
                        }
                    }

                    GUI.Label(callLabelPosition, callLabelContent, guiStyleButtonLabel);
                }
#endif

                #endregion

                //  User FriendTitleBar
                if (friendCountLabelContent != null)
                {
                    GUI.DrawTextureWithTexCoords(friendMenuTitleBarPosition, palette, buttonFriendList.IsActive ? texCoordsFriendTitleBgH : texCoordsFriendTitleBg);

                    GUI.Label(friendMenuTitleBarPosition, friendCountLabelContent, guiStyleFriendMenuTitle);

                    GUI.DrawTexture(friendMenuRightIconPosition, textureRight);
                }

#if UNITY_IOS
                //  VideoTitleBar
                if (FASConfig.Instance.videoEnable)
                {
                    GUI.DrawTextureWithTexCoords(videoTitleBarPosition, palette, buttonVideoList.IsActive ? texCoordsFriendTitleBgH : texCoordsFriendTitleBg);

                    GUI.Label(videoTitleBarPosition, videoListLabelContent, guiStyleFriendMenuTitle);

                    GUI.DrawTexture(videoRightIconPosition, textureRight);

                    if (buttonVideoList.IsTap(e, videoTitleBarPosition))
                    {
                        GoToVideoList(true);
                    }
                }
#endif

                if(buttonFriendList.IsTap(e, friendMenuTitleBarPosition))
                {
                    frameFriendList = ((GameObject)Instantiate(prfbFriendList)).GetComponent<FresviiGUIFrame>();

                    frameFriendList.transform.parent = this.transform;

                    frameFriendList.gameObject.GetComponent<FresviiGUIFriendList>().SetUser(this.user);

                    frameFriendList.Init(null, postFix, scaleFactor, this.GuiDepth - 1);

                    frameFriendList.SetDraw(true);

                    frameFriendList.PostFrame = this;

                    this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
                    {
                        this.SetDraw(false);
                    });

                    frameFriendList.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });

                }

                GUI.EndGroup();

                GUI.EndGroup();
            }
        }       
    }
}
