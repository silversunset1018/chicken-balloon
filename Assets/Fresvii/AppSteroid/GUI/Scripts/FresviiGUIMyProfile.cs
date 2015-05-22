#pragma warning disable 0414
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIMyProfile : FresviiGUIFrame
    {
        public enum Page { Top, FriendList, FriendRequest, VideoList };

        private Texture2D palette;

        public Rect texCoordsNotificationBg;
        
        public Rect texCoordsFriendTitleBg;

        public Rect texCoordsFriendTitleBgH;

        public Rect texCoordsFriendTitleLine;

        public float sideMargin = 8;

        public float topMargin = 16;
        
        public float margin = 18;
        
        public float miniMargin = 9;
        
        public float profileImageBgMargin = 1;
        
        public float vMargin = 10;
        
        public float hMargin = 8;

        private FresviiGUIMyProfileTopMenu userProfileTopMenu;
        
        private FresviiGUITabBar tabBar;

        private Rect baseRect;
        
        private Rect scrollViewRect;
        
        private float scaleFactor;
        
        private string postFix;

        public static Fresvii.AppSteroid.Models.User currentUser;

        private bool isReady;

        private Vector2 scrollPosition = Vector2.zero;

        public Vector2 myProfileImageSize;

		private Texture2D textureMyProfileDefault;
        
        [HideInInspector]        
        public static Texture2D textureMyProfile;
        
        public Texture2D textureMyProfileMask;
        
        private Texture2D textureMyProfileCircle;

        private Texture2D textureRight;

        public GUIStyle guiStyleLabelUserName;
        public GUIStyle guiStyleLabelUserProfile;
        public GUIStyle guiStyleButtonFriend;
        public GUIStyle guiStyleButtonMessage;
        public GUIStyle guiStyleFriendMenuTitle;
        public GUIStyle guiStyleLabelUserCode;

        public Vector2 profileImageMaxSize;

        private FresviiGUIMyProfileEdit guiEdit;
        private Rect userImagePosition;
        private Rect userNamePosition;
        private Rect userDescriptionPosition;

        private Rect userCodePosition;

        public bool IsOriginal { get; set; }

        private Fresvii.AppSteroid.Models.ListMeta FriendsMeta = null;

        public Fresvii.AppSteroid.Models.ListMeta RequestedFriendsListMeta { get; protected set; }

        private Fresvii.AppSteroid.Models.ListMeta VideosMeta = null;

        private Fresvii.AppSteroid.Models.ListMeta DirectMessagesMeta = null;

        private GUIContent userNameContent = new GUIContent("");

        private GUIContent userDescriptionContent = new GUIContent("");
        
        private GUIContent userCodeContent = new GUIContent("");

        public Vector2 profileImageSize;

        public float friendRequestPollingInterval = 15f;
        private Rect friendNotificationPosition;
        public GUIStyle guiStyleFriendNotification;
        public float friendNotificationHeight;
        public FresviiGUIButton buttonFriendNotification;

        public float friendMenuTitleBarHeight = 33f;
        
        private Rect friendMenuTitleBarPosition;
        
        public float groupListPollingInterval = 17f;

        public float directMessagePollingInterval = 300f;
        
        public float videoTitleBarHeight = 33f;

        private Rect videoTitleBarPosition;

        private Rect videoRightIconPosition;

        public float directMessageTitleBarHeight = 33f;

        private Rect directMessageTitleBarPosition;

        private Rect directMessageRightIconPosition;

        private Rect requestTitleBarPosition;

        private Rect requestRightIconPosition;

        private Rect friendMenuTitleUnderLinePosition;

        private Rect friendMenuRightIconPosition;

        public float pullResetTweenTime = 0.5f;
        
        public iTween.EaseType pullRestTweenEaseType = iTween.EaseType.easeOutExpo;

        public GameObject prfbFriendCard;

        public Texture2D textureUserProfileMask;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;
        public Vector2 loadingSpinnerSize;

        public GameObject prfbGUIFrameFriendRequests;

        public GameObject prfbGUIFrameMyProfileEdit;

        public GameObject prfbGUIFrameVideoList;

        public GameObject prfbGUIFrameDirectMessageList;

        private FresviiGUIFrame frameMyProfileEdit;

        private FresviiGUIFrame frameFriendRequests;

        private FresviiGUIFrame frameVideoList;

        private FresviiGUIFrame frameDirectMessageList;

        public float hMarginRate = 0.05f;

        public FresviiGUIButton buttonFriendList;

        public FresviiGUIButton buttonVideoList;

        public FresviiGUIButton buttonDirectMessageList;

        public FresviiGUIButton buttonFriendshipRequestList;

        private GUIContent friendCountLabelContent = null;

        private GUIContent requestLabelContent;

        private GUIContent videoListLabelContent;

        private GUIContent directMessageListLabelContent;
        
        private FresviiGUIFrame frameFriendList;

        private FresviiGUIFrame frameGroupList;

        public GameObject prfbFriendList;

        public GameObject prfbGroupList;

        private Fresvii.AppSteroid.Gui.PopOverBalloonMenu popOverBalloonMenu;

        public static Page StartPage = Page.Top;

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = guiDepth;

            this.scaleFactor = scaleFactor;

            this.postFix = postFix;

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            userProfileTopMenu = GetComponent<FresviiGUIMyProfileTopMenu>();

            tabBar = GetComponent<FresviiGUITabBar>();

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleLabelUserName.font = null;
                guiStyleLabelUserName.fontStyle = FontStyle.Bold;
                guiStyleLabelUserProfile.font = null;
                guiStyleButtonFriend.font = null;
                guiStyleButtonMessage.font = null;
                guiStyleFriendNotification.font = null;
                guiStyleFriendMenuTitle.font = null;
                guiStyleFriendMenuTitle.fontStyle = FontStyle.Bold;
                guiStyleLabelUserCode.font = null;
            }

            palette = FresviiGUIColorPalette.Palette;

            userProfileTopMenu.Init(appIcon, postFix, scaleFactor,this, GuiDepth - 1);

            tabBar.Init(postFix, scaleFactor, this.GuiDepth - 1);

            sideMargin *= scaleFactor;
            vMargin *= scaleFactor;
            margin *= scaleFactor;
            miniMargin *= scaleFactor;
            profileImageBgMargin *= scaleFactor;
            myProfileImageSize *= scaleFactor;
            topMargin *= scaleFactor;
            friendNotificationHeight *= scaleFactor;
            friendMenuTitleBarHeight *= scaleFactor;
            hMargin *= scaleFactor;
            loadingSpinnerSize *= scaleFactor;

            profileImageSize *= scaleFactor;

            guiStyleLabelUserName.fontSize = (int)(guiStyleLabelUserName.fontSize * scaleFactor);
            guiStyleLabelUserName.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileUserName);

            guiStyleLabelUserCode.fontSize = (int)(guiStyleLabelUserCode.fontSize * scaleFactor);

            guiStyleLabelUserCode.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileUserName);

            guiStyleLabelUserProfile.fontSize = (int)(guiStyleLabelUserProfile.fontSize * scaleFactor);
            guiStyleLabelUserProfile.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileDescription);
            
            guiStyleFriendNotification.fontSize = (int)(guiStyleFriendNotification.fontSize * scaleFactor);
            guiStyleFriendNotification.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileNotificationText);
            
            guiStyleFriendMenuTitle.fontSize = (int)(guiStyleFriendMenuTitle.fontSize * scaleFactor);

            guiStyleFriendMenuTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileFriendBarText);

            guiStyleFriendMenuTitle.padding = FresviiGUIUtility.RectOffsetScale(guiStyleFriendMenuTitle.padding, scaleFactor);

            textureMyProfileDefault = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.UserTextureName + postFix, false);

            texCoordsNotificationBg = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.ProfileNotificationBackground);

            texCoordsFriendTitleBg = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.ProfileFriendBarBackground);

            texCoordsFriendTitleBgH = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.ProfileFriendBarBackgroundH);

            texCoordsFriendTitleLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.ProfileFriendBarLine);

            textureRight = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.RightIconTextureName + postFix, false);

            scrollPosition = Vector2.zero;

            frameMyProfileEdit = ((GameObject)Instantiate(prfbGUIFrameMyProfileEdit)).GetComponent<FresviiGUIFrame>();

            guiEdit = frameMyProfileEdit.gameObject.GetComponent<FresviiGUIMyProfileEdit>();

            frameMyProfileEdit.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, GuiDepth - 20);
            
            frameMyProfileEdit.GetComponent<FresviiGUIMyProfileEdit>().SetGUIMyProfile(this);
            
            frameMyProfileEdit.transform.parent = this.transform;
            
            frameMyProfileEdit.SetDraw(false);
           
            textureMyProfileCircle = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.UserCircleTextureName + postFix, false);

            loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, FASGui.GuiDepthBase);

            GetAccout();

            SetScrollSlider(scaleFactor * 2.0f);

            requestLabelContent = new GUIContent(FresviiGUIText.Get("FriendRequests"));

            friendCountLabelContent = new GUIContent(FresviiGUIText.Get("Friends"));

            videoListLabelContent = new GUIContent(FresviiGUIText.Get("Videos"));

            directMessageListLabelContent = new GUIContent(FresviiGUIText.Get("DirectMessages"));
            
        }

        void OnDestroy()
        {
            if (textureMyProfile != null)
            {
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(currentUser.ProfileImageUrl);
            }
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            tabBar.enabled = on;

            userProfileTopMenu.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                {
                    loadingSpinner.Hide();
                }
            }
        }

        public void BackToPostFrame()
        {
            if (PostFrame == null)
            {
                return;
            }

            tabBar.enabled = false;

            PostFrame.SetDraw(true);

            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
            {
                Destroy(this.gameObject);
            });

            PostFrame.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
        }

        private Rect loadingSpinnerPosition;

        void GetAccout()
        {
            FASUser.GetAccount(delegate(Fresvii.AppSteroid.Models.User user, Fresvii.AppSteroid.Models.Error error)
            {
                loadingSpinner.Hide();

                if (error != null)
                {
                    FASUser.GetAccountFromCache(delegate(Fresvii.AppSteroid.Models.User _user, Fresvii.AppSteroid.Models.Error _error)
                    {
                        currentUser = _user;

						if(currentUser == null) return;

                        userCodeContent = new GUIContent(currentUser.UserCode);

                        isReady = true;

                        FASFriendship.GetFriendshipRequestedUsersList(currentUser.Id, OnGetFriendshipRequestedUsersList);

                        GetAccountFriendList();
                    });
                }
                else
                {
                    currentUser = user;

                    userCodeContent = new GUIContent(currentUser.UserCode);

                    isReady = true;

                    FASFriendship.GetFriendshipRequestedUsersList(currentUser.Id, OnGetFriendshipRequestedUsersList);

                    GetAccountFriendList();
                }

                CalcLayout();
            });
        }

        void OnGetFriendshipRequestedUsersList(IList<Fresvii.AppSteroid.Models.Friend> _requestedList, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error _error)
        {
            if (_error == null)
            {
                RequestedFriendsListMeta = meta;

                if (RequestedFriendsListMeta.TotalCount > 0)
                {
                    requestLabelContent = new GUIContent(FresviiGUIText.Get("FriendRequests") + ((meta.TotalCount > 0) ? (" (" + RequestedFriendsListMeta.TotalCount + ")") : ""));
                }
                else
                {
                    requestLabelContent = new GUIContent(FresviiGUIText.Get("FriendRequests"));
                }

                FresviiGUIManager.Instance.FriendRequestCount = meta.TotalCount;

                foreach (Fresvii.AppSteroid.Models.Friend friend in _requestedList)
                {
                    AddFriendshipRequest(friend);
                }
            }
            else
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                {
                    Debug.LogError("GetFriendshipRequestedList Error : " + _error.ToString());
                }
            }
        }

        public void AddFriendshipRequest(Fresvii.AppSteroid.Models.Friend friend)
        {
           
        }

        void OnEnable()
        {            
			userIconLoading = false;

			//	for push notification version 0.1
            StartCoroutine(PollingFriendRequest());

			//	for push notification version 0.1
			StartCoroutine(PollingFriendList());

            StartCoroutine(PollingVideoList());

            StartCoroutine(PollingDirectMessageList());

			FASEvent.OnFriendshipRequestCreated += OnFriendshipRequestCreated;

			FASEvent.OnFriendshipRequestUpdated += OnFriendshipRequestUpdated;
        }

		void OnDisable()
		{
			FASEvent.OnFriendshipRequestCreated -= OnFriendshipRequestCreated;
			
			FASEvent.OnFriendshipRequestUpdated -= OnFriendshipRequestUpdated;

            if (loadingSpinner != null)
                loadingSpinner.Hide();
		}

        void OnFriendshipRequestCreated(Fresvii.AppSteroid.Models.User invitingUser)
		{
            RequestedFriendsListMeta.TotalCount++;
		}

        void OnFriendshipRequestUpdated(Fresvii.AppSteroid.Models.User acceptedUser)
		{
            
		}

        public void AddFriend(Fresvii.AppSteroid.Models.Friend friend)
        {

        }

        void GetAccountFriendList()
        {
            FASFriendship.GetAccountFriendList(delegate(IList<Fresvii.AppSteroid.Models.Friend> friends, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
            {
                if (loadingSpinner != null)
                    loadingSpinner.Hide();

                if (error == null)
                {
                    FriendsMeta = meta;

                    friendCountLabelContent = new GUIContent(((meta.TotalCount > 0) ? (FriendsMeta.TotalCount.ToString()) + " " : "") + FresviiGUIText.Get("Friends"));
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

        void GetAccountVideoList()
        {
            FASVideo.GetCurrentUserVideoList((videos, meta, error) =>
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

        void GetDirectMessageList()
        {
            FAS.Instance.Client.DirectMessageService.GetDirectMessageList(1, true, (directMessages, meta, error) =>
            {
                if (error == null)
                {
                    DirectMessagesMeta = meta;

                    FresviiGUIManager.Instance.UnreadDirectMessageCount = meta.TotalCount;

                    directMessageListLabelContent = new GUIContent(FresviiGUIText.Get("DirectMessages") + ((meta.TotalCount > 0) ? (" (" + DirectMessagesMeta.TotalCount + ")") : ""));
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

        IEnumerator PollingFriendList()
        {
            while (this.enabled)
            {
                if (currentUser != null)
                {
                    GetAccountFriendList();
                }

				yield return new WaitForSeconds(friendRequestPollingInterval);
            }
        }

        IEnumerator PollingVideoList()
        {
            while (this.enabled)
            {
                if (currentUser != null)
                {
                    GetAccountVideoList();
                }

                yield return new WaitForSeconds(friendRequestPollingInterval);
            }
        }

        IEnumerator PollingDirectMessageList()
        {
            while (this.enabled)
            {
                GetDirectMessageList();

                yield return new WaitForSeconds(directMessagePollingInterval);
            }
        }

        IEnumerator PollingFriendRequest()
        {
            while (this.enabled)
            {
                if (currentUser != null)
                {
                    FASFriendship.GetFriendshipRequestedUsersList(currentUser.Id, OnGetFriendshipRequestedUsersList);
                }

				yield return new WaitForSeconds(friendRequestPollingInterval);
            }
        }



        private float screenWidth;

        void CalcLayout()
        {
			if(this == null || currentUser == null) return;

            hMargin = (baseRect.height + FresviiGUIFrame.OffsetPosition.y) * hMarginRate;

            if (RequestedFriendsListMeta != null)
            {
                if (RequestedFriendsListMeta.TotalCount > 0)
                {
                    friendNotificationPosition = new Rect(Position.x, Position.y + userProfileTopMenu.height, baseRect.width, friendNotificationHeight);
                }
                else
                {
                    friendNotificationPosition = new Rect(0, 0, 0, 0);
                }
            }
            else
            {
                friendNotificationPosition = new Rect(0, 0, 0, 0);
            }

            userImagePosition = new Rect(Screen.width * 0.5f - profileImageSize.x * 0.5f, hMargin + userProfileTopMenu.height, profileImageSize.x, profileImageSize.y);

            string userNameTruncated = FresviiGUIUtility.Truncate(currentUser.Name, guiStyleLabelUserName, Screen.width * 0.75f, "...");

            userNameContent = new GUIContent(userNameTruncated);

            userNamePosition = new Rect(Screen.width * 0.125f, userImagePosition.y + userImagePosition.height + hMargin, Screen.width * 0.75f, guiStyleLabelUserName.CalcHeight(userNameContent, Screen.width * 0.6f));

            userCodePosition = new Rect(0f, userNamePosition.y + userNamePosition.height + 0.5f * hMargin, Screen.width, userNamePosition.height);

            userDescriptionContent = new GUIContent(currentUser.Description == null ? " " : currentUser.Description);

            userDescriptionPosition = new Rect(Screen.width * 0.2f, userCodePosition.y + userCodePosition.height + hMargin, Screen.width * 0.6f, guiStyleLabelUserProfile.CalcHeight(userDescriptionContent, Screen.width * 0.6f));

            friendMenuTitleBarPosition = new Rect(0f, userDescriptionPosition.y + userDescriptionPosition.height + hMargin, baseRect.width, friendMenuTitleBarHeight);

            friendMenuRightIconPosition = new Rect(baseRect.width - sideMargin - textureRight.width, friendMenuTitleBarPosition.y + friendMenuTitleBarPosition.height * 0.5f - textureRight.height * 0.5f, textureRight.width, textureRight.height);

            requestTitleBarPosition = new Rect(0f, friendMenuTitleBarPosition.y + friendMenuTitleBarPosition.height + scaleFactor, baseRect.width, friendMenuTitleBarHeight);

            requestRightIconPosition = new Rect(baseRect.width - sideMargin - textureRight.width, requestTitleBarPosition.y + requestTitleBarPosition.height * 0.5f - textureRight.height * 0.5f, textureRight.width, textureRight.height);

#if UNITY_IOS

            if (FASConfig.Instance.videoEnable)
            {
                videoTitleBarPosition = new Rect(0f, requestTitleBarPosition.y + requestTitleBarPosition.height + scaleFactor, baseRect.width, friendMenuTitleBarHeight);

                videoRightIconPosition = new Rect(baseRect.width - sideMargin - textureRight.width, videoTitleBarPosition.y + videoTitleBarPosition.height * 0.5f - textureRight.height * 0.5f, textureRight.width, textureRight.height);

                directMessageTitleBarPosition = new Rect(0f, videoTitleBarPosition.y + videoTitleBarPosition.height + scaleFactor, baseRect.width, friendMenuTitleBarHeight);

                directMessageRightIconPosition = new Rect(baseRect.width - sideMargin - textureRight.width, directMessageTitleBarPosition.y + directMessageTitleBarPosition.height * 0.5f - textureRight.height * 0.5f, textureRight.width, textureRight.height);
            }
            else
            {
                directMessageTitleBarPosition = new Rect(0f, requestTitleBarPosition.y + requestTitleBarPosition.height + scaleFactor, baseRect.width, friendMenuTitleBarHeight);

                directMessageRightIconPosition = new Rect(baseRect.width - sideMargin - textureRight.width, directMessageTitleBarPosition.y + directMessageTitleBarPosition.height * 0.5f - textureRight.height * 0.5f, textureRight.width, textureRight.height);
            }
            
#elif UNITY_ANDROID

            directMessageTitleBarPosition = new Rect(0f, requestTitleBarPosition.y + requestTitleBarPosition.height + scaleFactor, baseRect.width, friendMenuTitleBarHeight);

            directMessageRightIconPosition = new Rect(baseRect.width - sideMargin - textureRight.width, directMessageTitleBarPosition.y + directMessageTitleBarPosition.height * 0.5f - textureRight.height * 0.5f, textureRight.width, textureRight.height);
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
            return directMessageTitleBarPosition.y + directMessageTitleBarPosition.height + tabBar.height + scaleFactor;
        }

		private bool userIconLoading = false;

		private string profileImageUrl;

        private void LoadUserIcon(string url)
        {
            userIconLoading = true;

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(url, true, delegate(Texture2D texture)
            {
                if (this == null) return;

                textureMyProfile = texture;

                profileImageUrl = currentUser.ProfileImageUrl;

                userIconLoading = false;
            });
        }

		void Update(){

			if(currentUser == null)
				return;

            if (StartPage == Page.VideoList)
            {
                StartPage = Page.Top;

                GoToVideoList(false);
            }

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            baseRect = new Rect(Position.x, Position.y + friendNotificationPosition.height, Screen.width, Screen.height - friendNotificationPosition.height - FresviiGUIFrame.OffsetPosition.y);

            if (currentUser == null)
            {
                return;
            }

            if (!userIconLoading && !string.IsNullOrEmpty(currentUser.ProfileImageUrl) && (textureMyProfile == null || profileImageUrl != currentUser.ProfileImageUrl))
            {
                if (textureMyProfile != null && profileImageUrl != currentUser.ProfileImageUrl)
                {
                    FresviiGUIManager.Instance.resourceManager.ReleaseTexture(profileImageUrl);
                }

                LoadUserIcon(currentUser.ProfileImageUrl);
            }

            CalcLayout();

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

        public void OnEditButtonTapped()
        {
            guiEdit.SetCurrentUserProfile(currentUser, (textureMyProfile == null ) ? textureMyProfileDefault : textureMyProfile);

            tabBar.enabled = false;

            frameMyProfileEdit.SetDraw(true);

            frameMyProfileEdit.Tween(new Vector2(0.0f, Screen.height), Vector2.zero, delegate()
            {
                this.SetDraw(false);
            });
        }

        public void SetCurrentUser(Fresvii.AppSteroid.Models.User user)
        {
            currentUser = user;
        }

        public void RemoveRequestedUser(Fresvii.AppSteroid.Models.User user)
        {

        }

        public void DestroySubFrames()
        {
            if (frameFriendList != null)
            {
                Destroy(frameFriendList.gameObject);
            }

            if (frameFriendRequests != null)
            {
                Destroy(frameFriendRequests.gameObject);
            }

            if (frameGroupList != null)
            {
                Destroy(frameGroupList.gameObject);
            }

            if (frameVideoList != null)
            {
                Destroy(frameVideoList.gameObject);
            }

            if (frameDirectMessageList != null)
            {
                Destroy(frameDirectMessageList.gameObject);
            }
        }

        public float moveDelayTime = 0.1f;

        public Material userMask;

        void OnGUI()
        {
            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            if (isReady)
            {
                GUI.BeginGroup(baseRect);

				GUI.BeginGroup(scrollViewRect);

                //	User Image
                if (Event.current.type == EventType.Repaint)
                {
                    userMask.color = Color.white;

                    Graphics.DrawTexture(userImagePosition, (textureMyProfile == null) ? textureMyProfileDefault : textureMyProfile, userMask);
                }
                
                Event e = Event.current;

                GUI.DrawTexture(userImagePosition, textureMyProfileCircle);

                //  User Name
                GUI.Label(userNamePosition, userNameContent, guiStyleLabelUserName);

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
                            Fresvii.AppSteroid.Util.Clipboard.SetText(currentUser.Name);
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
                            Fresvii.AppSteroid.Util.Clipboard.SetText(currentUser.UserCode);
                        }
                    });

                    popOverBalloonMenu.Name = "UserCode";
                }      

                //  User Description
                GUI.Label(userDescriptionPosition, currentUser.Description, guiStyleLabelUserProfile);

                //  User FriendTitleBar
                GUI.DrawTextureWithTexCoords(friendMenuTitleBarPosition, palette, buttonFriendList.IsActive ? texCoordsFriendTitleBgH : texCoordsFriendTitleBg);

                GUI.Label(friendMenuTitleBarPosition, friendCountLabelContent, guiStyleFriendMenuTitle);

                GUI.DrawTexture(friendMenuRightIconPosition, textureRight);

                if (buttonFriendList.IsTap(e, friendMenuTitleBarPosition))
                {
                    frameFriendList = ((GameObject)Instantiate(prfbFriendList)).GetComponent<FresviiGUIFrame>();

                    frameFriendList.transform.parent = this.transform;

                    frameFriendList.gameObject.GetComponent<FresviiGUIFriendList>().SetUser(currentUser);

                    frameFriendList.Init(null, postFix, this.scaleFactor, this.GuiDepth - 1);

                    frameFriendList.SetDraw(true);

                    frameFriendList.PostFrame = this;

                    this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), moveDelayTime, delegate()
                    {
                        this.SetDraw(false);
                    });

                    frameFriendList.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, moveDelayTime, delegate() { });
                }

                //  RequestTitleBar
                GUI.DrawTextureWithTexCoords(requestTitleBarPosition, palette, buttonFriendshipRequestList.IsActive ? texCoordsFriendTitleBgH : texCoordsFriendTitleBg);

                GUI.Label(requestTitleBarPosition, requestLabelContent, guiStyleFriendMenuTitle);

                GUI.DrawTexture(requestRightIconPosition, textureRight);

                if (buttonFriendshipRequestList.IsTap(e, requestTitleBarPosition))
                {
                    GoToFriendRequests();
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

                //  DirectMessageTitleBar
                GUI.DrawTextureWithTexCoords(directMessageTitleBarPosition, palette, buttonDirectMessageList.IsActive ? texCoordsFriendTitleBgH : texCoordsFriendTitleBg);

                GUI.Label(directMessageTitleBarPosition, directMessageListLabelContent, guiStyleFriendMenuTitle);

                GUI.DrawTexture(directMessageRightIconPosition, textureRight);

                if (buttonDirectMessageList.IsTap(e, directMessageTitleBarPosition))
                {
                    GoToDirectMessageList(true);
                }

                GUI.EndGroup(); // scroll view rect               

                GUI.EndGroup(); // base rect

                //	Notification
                if (RequestedFriendsListMeta != null)
                {
                    if (RequestedFriendsListMeta.TotalCount > 0)
                    {
                        GUI.DrawTextureWithTexCoords(friendNotificationPosition, palette, texCoordsNotificationBg);

                        string friendRequestMessage = FresviiGUIText.Get("FriendRequestCount").Replace("#", RequestedFriendsListMeta.TotalCount.ToString());

                        GUI.Label(friendNotificationPosition, friendRequestMessage, guiStyleFriendNotification);

                        if (buttonFriendNotification.IsTap(Event.current, friendNotificationPosition))
                        {
                            GoToFriendRequests();
                        }
                    }
                }
            }
        }

        private void GoToGroupMessage(bool animation)
        {
            frameGroupList = ((GameObject)Instantiate(prfbGroupList)).GetComponent<FresviiGUIFrame>();

            frameGroupList.transform.parent = this.transform;

            frameGroupList.Init(null, postFix, this.scaleFactor, this.GuiDepth - 1);

            frameGroupList.SetDraw(true);

            frameGroupList.PostFrame = this;

            if (animation)
            {
                this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), moveDelayTime, delegate()
                {
                    this.SetDraw(false);
                });

                frameGroupList.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, moveDelayTime, delegate() { });
            }
            else
            {
                this.Position = new Vector2(-Screen.width, 0.0f);

                this.SetDraw(false);

                frameGroupList.Position = Vector2.zero;
            }
        }

        private void GoToVideoList(bool animation)
        {
            frameVideoList = ((GameObject)Instantiate(prfbGUIFrameVideoList)).GetComponent<FresviiGUIFrame>();

            FresviiGUIVideoList fresviiGUIVideoList = frameVideoList.gameObject.GetComponent<FresviiGUIVideoList>();

            fresviiGUIVideoList.IsModal = false;

            fresviiGUIVideoList.mode = FresviiGUIVideoList.Mode.Share;

            frameVideoList.transform.parent = this.transform;

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

        private void GoToDirectMessageList(bool animation)
        {
            frameDirectMessageList = ((GameObject)Instantiate(prfbGUIFrameDirectMessageList)).GetComponent<FresviiGUIFrame>();

            frameDirectMessageList.transform.parent = this.transform;

            frameDirectMessageList.Init(null, postFix, this.scaleFactor, this.GuiDepth - 1);

            frameDirectMessageList.SetDraw(true);

            frameDirectMessageList.PostFrame = this;

            if (animation)
            {
                this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), moveDelayTime, delegate()
                {
                    this.SetDraw(false);
                });

                frameDirectMessageList.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, moveDelayTime, delegate() { });
            }
            else
            {
                frameDirectMessageList.Position = Vector2.zero;

                this.SetDraw(false);
            }
        }

        public void GoToFriendRequests()
        {
            frameFriendRequests = ((GameObject)Instantiate(prfbGUIFrameFriendRequests)).GetComponent<FresviiGUIFriendRequests>();

            frameFriendRequests.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, GuiDepth - 10);

            frameFriendRequests.GetComponent<FresviiGUIFriendRequests>().SetGUIMyProfile(this);

            frameFriendRequests.transform.parent = this.transform;

            frameFriendRequests.SetDraw(true);

            tabBar.enabled = false;

            this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), moveDelayTime, delegate()
            {
                this.SetDraw(false);
            });

            frameFriendRequests.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, moveDelayTime, delegate() { });
        }

        public void SetGUIMyProfileEdit(FresviiGUIMyProfileEdit guiEdit)
        {
            this.guiEdit = guiEdit;
        }

        public void SetUserProfile(string username, string description, Texture2D userProfileImage)
        {
            if(!string.IsNullOrEmpty(username))
                currentUser.Name = username;

            if (!string.IsNullOrEmpty(description))
                currentUser.Description = description;

            if (userProfileImage != null)
            {
                Destroy(textureMyProfile);

                textureMyProfile = userProfileImage;
            }
        }
    }
}
