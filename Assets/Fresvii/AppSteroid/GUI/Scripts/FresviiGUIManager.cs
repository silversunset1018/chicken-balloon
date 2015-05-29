using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIManager : MonoBehaviour
    {
        public static FresviiGUIManager Instance;

        #region Defines
        

        #endregion

        #region public
       
        public GameObject prfbGUIViewForum;
       
        public GameObject prfbGUIViewLeaderboard;
        
        public GameObject prfbGUIViewMyProfile;
        
        public GameObject prfbGUIViewGroupMessage;

        public GameObject prfbGUIViewMatchMaking;

        public GameObject prfbGUIViewGroupConference;
        
        private static FASGui.Mode guiMode = FASGui.Mode.All;

        public Vector2 notificationOffset;
        
        public static FASGui.Mode GuiMode
        {
            get { return guiMode; }

            protected set
            {
                guiMode = value;
            }
        }

        public static string ReturnSceneName { get { return returnSceneName; } }

        private string LoginUserId;

        private string LoginUserToken;

        [HideInInspector]
        public FresviiGUIView CurrentView { get; protected set; }

        [HideInInspector]
        public FASGui.Mode CurrentViewMode { get; protected set; }

        [HideInInspector]
        public int ModeCount { get; protected set; }

        [HideInInspector]
        public Fresvii.AppSteroid.Util.ResourceManager resourceManager { get; protected set; }

        public static Texture2D appIcon;

        public GUIStyle guiStyleNotificationButton;

        public event Action<IList<Fresvii.AppSteroid.Models.Group>, Fresvii.AppSteroid.Models.ListMeta, Fresvii.AppSteroid.Models.Error> OnGetGroupMessageGroups;

        private uint friendRequestCount;

        public uint FriendRequestCount
        {
            get{return friendRequestCount;}

            set
            {
                friendRequestCount = value;

                FresviiGUITabBar.profileBadgeCount = (uint) Mathf.Max(0, friendRequestCount + unreadDirectMessageCount);
            }
        }

        private uint unreadDirectMessageCount;

        public uint UnreadDirectMessageCount
        {
            get { return unreadDirectMessageCount; }

            set
            {
                unreadDirectMessageCount = value;

                FresviiGUITabBar.profileBadgeCount = (uint)Mathf.Max(0, friendRequestCount + unreadDirectMessageCount);
            }
        }

#if GROUP_CONFERENCE
        private Rect textureCoordsNotificationBackGround;

        private Color notificationTextColor;
#endif
        #endregion

        #region private

        private static FASGui.Mode initialSelectedMode = FASGui.Mode.Forum;

        private static string returnSceneName;

        private FresviiGUIView viewForum;

        private FresviiGUIView viewLeaderboards;

        private FresviiGUIView viewMyProfile;

        private FresviiGUIView viewGroupMessage;

        private FresviiGUIView viewMatchMaking;

        private FresviiGUIView viewGroupConference;

        private enum ResolutinMode { X1 = 0, X2, X3 };
   
        private static ResolutinMode resolutionMode = ResolutinMode.X1;
        
        private static readonly string[] Postfixes = { "", "@2X", "@3X" };
        
        private static readonly float[] ResolutinoScaleFactors = { 1.0f, 2.0f, 3f };
        
        public static float scaleFactor = 1.0f;
        
        public float ScaleFactor { get{return scaleFactor;} protected set{ scaleFactor = value;}}

        public static string postFix = "";

        private static bool parametterSettle = false;

        private FresviiGUIFrame fresviiGUI;

		private bool initialized = false;

        private Fresvii.AppSteroid.Util.DeviceRotationSetting initDeviceRotationSetting;

        #endregion

        public static void SetParameters()
        {
            if (parametterSettle) return;

            //  Check Resolution mode            
            int width = Mathf.Min(Screen.width, Screen.height);

            if (width < FresviiGUIConstants.ResolutionWidthX1)
            {
                resolutionMode = ResolutinMode.X1;
            }
            else if (width < FresviiGUIConstants.ResolutionWidthX2)
            {
                resolutionMode = ResolutinMode.X2;
            }
            else
            {
                resolutionMode = ResolutinMode.X3;
            }

            Application.targetFrameRate = 60;

            scaleFactor = ResolutinoScaleFactors[(int)resolutionMode];

            postFix = Postfixes[(int)resolutionMode];

            parametterSettle = true;
        }

        IEnumerator Start()
        {
            float initTime = Time.time;

            while (!FAS.Initialized)
            {
                if (Time.time - initTime > 30f)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

                    if (FAS.initializeError != null)
                    {
                        if (FAS.initializeError.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NetworkNotReachable)
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), (del) =>
                            {

                            });
                        }
                        else
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), (del) =>
                            {

                            });
                        }

                        Debug.LogError("FAS Initilize error : " + FAS.initializeError.ToString());
                    }
                    else
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), (del) =>
                        {

                        });

                        Debug.LogError("FAS Initilize error : Unknown");
                    }

                    FresviiGUIManager.Instance.LoadScene();

                    yield break;
                }
                else
                {
                    yield return 1;
                }
            }

            initDeviceRotationSetting = new Fresvii.AppSteroid.Util.DeviceRotationSetting();

            initDeviceRotationSetting.orientation = ScreenOrientation.AutoRotation;

            initDeviceRotationSetting.portrait = Screen.autorotateToPortrait;

            initDeviceRotationSetting.portraitUpsideDown = Screen.autorotateToPortraitUpsideDown;

            initDeviceRotationSetting.landscapeLeft = Screen.autorotateToLandscapeLeft;

            initDeviceRotationSetting.landscapeRight = Screen.autorotateToLandscapeRight;

            Fresvii.AppSteroid.Util.DeviceRotationControll.Set(new Fresvii.AppSteroid.Util.DeviceRotationSetting(FASConfig.Instance.orientation, FASConfig.Instance.portrait, FASConfig.Instance.portraitUpsideDown, FASConfig.Instance.landscapeRight, FASConfig.Instance.landscapeLeft));    

            bool waitLogin = false;

			bool retryLogin = false;

            if (!string.IsNullOrEmpty(LoginUserId) && !string.IsNullOrEmpty(LoginUserToken) && !FASUser.IsLoggedIn())
            {
				waitLogin = true;

                FASUser.LogIn(LoginUserId, LoginUserToken, (error)=>
                {
                    if (error != null && error.Code != (int)Fresvii.AppSteroid.Models.Error.ErrorCode.Banned)
                    {
						Debug.LogError(error.ToString());

#if !UNITY_EDITOR
						Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

						Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("LoginErrorAndRetry"), (retry) =>
                        {
                            if (retry)
                            {
								retryLogin = true;

								waitLogin = false;

								StartCoroutine(Start());
                            }
                            else
                            {
								Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

                                LoginUserId = "";

                                LoginUserToken = "";

                                FresviiGUIManager.Instance.LoadScene();
                            }
                        });					
#else
						Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

						LoginUserId = "";
						
						LoginUserToken = "";
						
						FresviiGUIManager.Instance.LoadScene();
#endif
					}
                    else
                    {
                        FASUser.GetAccount((u, e) => { });

						waitLogin = false;
                    }
                });
            }
            else if (FASUser.LoadSignedUpUsers().Count == 0)
            {
                waitLogin = true;

                FASUser.SignUp((user, e1)=>
                {
                    if (e1 != null)
                    {
                        Debug.LogError("Sign up error " + e1.ToString());
#if !UNITY_EDITOR
						Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

						Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("LoginErrorAndRetry"), (retry) =>
                        {
                            if (retry)
                            {
								retryLogin = true;

								waitLogin = false;

								StartCoroutine(Start());
                            }
                            else
                            {
								Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

                                LoginUserId = "";

                                LoginUserToken = "";

                                FresviiGUIManager.Instance.LoadScene();
                            }
                        });					
#else
                        Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

                        LoginUserId = "";

                        LoginUserToken = "";

                        FresviiGUIManager.Instance.LoadScene();
#endif
                    }
                    else
                    {
                        FASUser.LogIn(user.Id, user.Token, (e2)=>
                        {
                            if (e2 != null)
                            {
                                Debug.LogError("Log in error " + e2.ToString());
#if !UNITY_EDITOR
        						Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

						        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("LoginErrorAndRetry"), (retry) =>
                                {
                                    if (retry)
                                    {
								        retryLogin = true;

								        waitLogin = false;

								        StartCoroutine(Start());
                                    }
                                    else
                                    {
								        Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

                                        LoginUserId = "";

                                        LoginUserToken = "";

                                        FresviiGUIManager.Instance.LoadScene();
                                    }
                                });					
#else
                                Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

                                LoginUserId = "";

                                LoginUserToken = "";

                                FresviiGUIManager.Instance.LoadScene();
#endif
                            }
                            else
                            {
                                FASUser.GetAccount((u, e) => { });

                                waitLogin = false;
                            }
                        });
                    }                 
                });
            }
            else if ((string.IsNullOrEmpty(LoginUserId) || string.IsNullOrEmpty(LoginUserToken)) && !FASUser.IsLoggedIn())
            {
                waitLogin = true;

                FASUser.RelogIn((error) =>
                {
                    if (error != null && error.Code != (int)Fresvii.AppSteroid.Models.Error.ErrorCode.Banned)
                    {
                        Debug.LogError(error.ToString());

#if !UNITY_EDITOR
						Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

						Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("LoginErrorAndRetry"), (retry) =>
                        {
                            if (retry)
                            {
								retryLogin = true;

								waitLogin = false;

								StartCoroutine(Start());
                            }
                            else
                            {
								Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

                                LoginUserId = "";

                                LoginUserToken = "";

                                FresviiGUIManager.Instance.LoadScene();
                            }
                        });					
#else
                        Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

                        LoginUserId = "";

                        LoginUserToken = "";

                        FresviiGUIManager.Instance.LoadScene();
#endif
                    }
                    else
                    {
                        FASUser.GetAccount((u, e) => { });

                        waitLogin = false;
                    }
                });
            }

			while (waitLogin)
			{
				yield return 1;
			}
			
			if(retryLogin || !FASUser.IsLoggedIn())
			{
				yield break;
			}

            LoginUserId = "";

            LoginUserToken = "";

            SetParameters();

            this.gameObject.AddComponent<FASGesture>();

            appIcon = FASConfig.Instance.appIcon;

            FresviiGUIText.SetUp(Application.systemLanguage);

            resourceManager = Fresvii.AppSteroid.Util.ResourceManager.Create();

            notificationOffset *= scaleFactor;

            guiStyleNotificationButton.fontSize = (int)(guiStyleNotificationButton.fontSize * scaleFactor);

#if GROUP_CONFERENCE
            textureCoordsNotificationBackGround = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NotificatinoBackground);

            voiceChatConnectingNotificationMessage = FresviiGUIText.Get("VoiceChatConnectingNotification");

            notificationTextColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NotificatinoText);
#endif

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleNotificationButton.font = null;
            }

            // View (tab view) Set up
			if ((int)FresviiGUIManager.GuiMode == (int)FASGui.Mode.All) 
			{
				FresviiGUIManager.GuiMode -= FASGui.Mode.MatchMaking;

				FresviiGUIManager.GuiMode -= FASGui.Mode.GroupConference;
			}

            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.MatchMaking) > 0)  // MatchMaking is exclusive value.
            {
                FresviiGUIManager.GuiMode &= FASGui.Mode.MatchMaking;
            }

            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.GroupConference) > 0)  // GroupConference is exclusive value.
            {
                FresviiGUIManager.GuiMode &= FASGui.Mode.GroupConference;
            }

            ModeCount = 0;

            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.Leaderboards) > 0)
            {
                ModeCount++;
            }
            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.MyProfile) > 0)
            {
                ModeCount++;
            }
            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.Forum) > 0)
            {
                ModeCount++;
            }
            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.GroupMessage) > 0)
            {
                ModeCount++;
            }
            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.MatchMaking) > 0)
            {
                ModeCount++;
            }
            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.GroupConference) > 0)
            {
                ModeCount++;
            }


            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.Leaderboards) > 0)
            {
                CurrentViewMode = FASGui.Mode.Leaderboards;

                viewLeaderboards = ((GameObject)Instantiate(prfbGUIViewLeaderboard)).GetComponent<FresviiGUIViewLeaderboard>();

                viewLeaderboards.transform.parent = this.transform;

                viewLeaderboards.Init(appIcon, postFix, scaleFactor);
            }

            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.MyProfile) > 0)
            {
                CurrentViewMode = FASGui.Mode.MyProfile;

                viewMyProfile = ((GameObject)Instantiate(prfbGUIViewMyProfile)).GetComponent<FresviiGUIViewMyProfile>();

                viewMyProfile.transform.parent = this.transform;

                viewMyProfile.Init(appIcon, postFix, scaleFactor);
            }

            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.Forum) > 0)
            {
                CurrentViewMode = FASGui.Mode.Forum;

                viewForum = ((GameObject)Instantiate(prfbGUIViewForum)).GetComponent<FresviiGUIViewForum>();

                viewForum.transform.parent = this.transform;

                viewForum.Init(appIcon, postFix, scaleFactor);
            }

            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.GroupMessage) > 0)
            {
                CurrentViewMode = FASGui.Mode.GroupMessage;

                viewGroupMessage = ((GameObject)Instantiate(prfbGUIViewGroupMessage)).GetComponent<FresviiGUIViewGroupMessage>();

                viewGroupMessage.transform.parent = this.transform;

                viewGroupMessage.Init(appIcon, postFix, scaleFactor);

                StartCoroutine(PollingGetGroupMessageGroupList());
            }

            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.MatchMaking) > 0)
            {
                CurrentViewMode = FASGui.Mode.MatchMaking;

                viewMatchMaking = ((GameObject)Instantiate(prfbGUIViewMatchMaking)).GetComponent<FresviiGUIViewMatchMaking>();

                viewMatchMaking.transform.parent = this.transform;

                viewMatchMaking.Init(appIcon, postFix, scaleFactor);
            }

            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.GroupConference) > 0)
            {
                CurrentViewMode = FASGui.Mode.GroupConference;

                viewGroupConference = ((GameObject)Instantiate(prfbGUIViewGroupConference)).GetComponent<FresviiGUIViewGroupConference>();

                viewGroupConference.transform.parent = this.transform;

                viewGroupConference.Init(appIcon, postFix, scaleFactor);
            }

            if (((int)FresviiGUIManager.GuiMode & (int)initialSelectedMode) > 0)
            {
                CurrentViewMode = initialSelectedMode;
            }

            SetViewMode(CurrentViewMode);

            yield return 1;

            Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

			initialized = true;
        }

		void OnEnable()
		{
            FASEvent.OnTapForumCommentCreated += OnTapForumCommentCreated;
            
            FASEvent.OnFriendshipRequestCreated += OnFriendshipRequestCreated;

            FASEvent.OnTapFriendshipRequestCreated += OnTapFriendshipRequestCreated;

            FASEvent.OnTapFriendshipRequestUpdated += OnTapFriendshipRequestUpdated;
            
            FASEvent.OnTapGroupMessageImageCreated += OnTapGroupMessageCreated;

            FASEvent.OnTapGroupMessageTextCreated += OnTapGroupMessageCreated;

            FASEvent.OnTapForumOfficialThreadCreated += OnTapForumOfficialThreadCreated;

            FASEvent.OnDirectMessageCreated += OnDirectMessageCreated;

            FASEvent.OnUserAgreementRequired += ShowUserAgreementDialog;

            FASEvent.OnUserBanned += OnUserBanned;
		}

		void OnDisable()
		{
            FASEvent.OnTapForumCommentCreated -= OnTapForumCommentCreated;

            FASEvent.OnFriendshipRequestCreated -= OnFriendshipRequestCreated;

            FASEvent.OnTapFriendshipRequestCreated -= OnTapFriendshipRequestCreated;

            FASEvent.OnTapFriendshipRequestUpdated -= OnTapFriendshipRequestUpdated;

            FASEvent.OnTapGroupMessageImageCreated -= OnTapGroupMessageCreated;

            FASEvent.OnTapGroupMessageTextCreated -= OnTapGroupMessageCreated;

            FASEvent.OnTapForumOfficialThreadCreated -= OnTapForumOfficialThreadCreated;

            FASEvent.OnDirectMessageCreated -= OnDirectMessageCreated;

            FASEvent.OnUserAgreementRequired -= ShowUserAgreementDialog;

            FASEvent.OnUserBanned -= OnUserBanned;
        }

        void ShowUserAgreementDialog(Fresvii.AppSteroid.Models.UserAgreement userAgreement)
        {
            Debug.Log(userAgreement.Text);

#if !UNITY_EDITOR
            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(userAgreement.Text, FresviiGUIText.Get("Accept"), FresviiGUIText.Get("Decline"), (del) =>
            {
                if (del)
                {
#endif
                    FASUtility.AcceptUserAgreement((userAgreementState, error) => 
                    {
                        if (error != null)
                        {
                            Debug.LogError("EULA Accept Error: " + error.ToString());
                        }
                        else
                        {
                            Debug.Log("EULA Accepted");
                        }
                    
                    });
#if !UNITY_EDITOR
                }
                else
                {
                    FASUtility.DeclineUserAgreement((userAgreementState, error) => { });

					FASUser.LogOut();

                    Application.LoadLevel(0);
                }
            });
#endif
        }

        void OnUserBanned(string message)
        {
            Debug.LogWarning(message);

            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(message, FresviiGUIText.Get("Close"), FresviiGUIText.Get("Close"),  FresviiGUIText.Get("Close"), (del) =>
            {
				FASUser.LogOut();
            });

			Application.LoadLevel(0);
        }

        void OnDestroy()
        {
            if(resourceManager != null)
                resourceManager.Release();

            Fresvii.AppSteroid.Util.DeviceRotationControll.Set(initDeviceRotationSetting);

  			Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();
        }

        public void SetViewMode(FASGui.Mode viewMode)
        {
            CurrentViewMode = viewMode;

            if (viewForum != null)
            {
                viewForum.gameObject.SetActive(viewMode == FASGui.Mode.Forum);
            }

            if (viewLeaderboards != null)
            {
                viewLeaderboards.gameObject.SetActive(viewMode == FASGui.Mode.Leaderboards);
            }

            if (viewMyProfile != null)
            {
                viewMyProfile.gameObject.SetActive(viewMode == FASGui.Mode.MyProfile);
            }

            if (viewGroupMessage != null)
            {
                viewGroupMessage.gameObject.SetActive(viewMode == FASGui.Mode.GroupMessage);
            }

            if (viewMatchMaking != null)
            {
                viewMatchMaking.gameObject.SetActive(viewMode == FASGui.Mode.MatchMaking);
            }

            if (viewGroupConference != null)
            {
                viewGroupConference.gameObject.SetActive(viewMode == FASGui.Mode.GroupConference);
            }

            if (CurrentViewMode == FASGui.Mode.Forum)
            {
                CurrentView = viewForum;
            }
            else if (CurrentViewMode == FASGui.Mode.GroupConference)
            {
                CurrentView = viewGroupConference;
            }
            else if (CurrentViewMode == FASGui.Mode.GroupMessage)
            {
                CurrentView = viewGroupMessage;
            }
            else if (CurrentViewMode == FASGui.Mode.Leaderboards)
            {
                CurrentView = viewLeaderboards;
            }
            else if (CurrentViewMode == FASGui.Mode.MatchMaking)
            {
                CurrentView = viewMatchMaking;
            }
            else if (CurrentViewMode == FASGui.Mode.MyProfile)
            {
                CurrentView = viewMyProfile;
            }

            FASGesture.Stop();
        }

        public void SetTopFrame()
        {
            if (CurrentViewMode == FASGui.Mode.Forum)
            {
                viewForum.GetComponent<FresviiGUIViewForum>().SetTopFrame();
            }
            else if (CurrentViewMode == FASGui.Mode.Leaderboards)
            {
                viewLeaderboards.GetComponent<FresviiGUIViewLeaderboard>().SetTopFrame();
            }
            else if (CurrentViewMode == FASGui.Mode.GroupMessage)
            {
                viewGroupMessage.GetComponent<FresviiGUIViewGroupMessage>().SetTopFrame();
            }
            else if (CurrentViewMode == FASGui.Mode.MyProfile)
            {
                viewMyProfile.GetComponent<FresviiGUIViewMyProfile>().SetTopFrame();
            }
        }
      
        void Awake()
        {
            FASConfig.Instance.systemLanguage = Application.systemLanguage;

            Instance = this;

            GuiMode = FASGui.mode;

            returnSceneName = FASGui.ReturnSceneName;

            initialSelectedMode = FASGui.SelectedMode;

            LoginUserId = FASGui.LoginUserId;

            LoginUserToken = FASGui.LoginUserToken;
        }

        public void LoadScene()
        {
            if(string.IsNullOrEmpty(returnSceneName))
            {
                if (Application.levelCount == 0 || Application.loadedLevel == 0)
                {
                    Debug.LogError("Return scene does not exists.");

#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Return scene does not exists.", "Please set the return scene. \nThe build scenes count of this project is 0 or only this AppSteroid GUI scene.", "OK");
#endif
                }
                else
                {
                    Application.LoadLevel(0);
                }
            }
            else
            {
                Application.LoadLevel(returnSceneName);
            }
        }

        public void OnTapForumCommentCreated(Fresvii.AppSteroid.Models.Comment comment)
        {
			if(viewForum != null)
			{
				FresviiGUIViewForum forum = viewForum.gameObject.GetComponent<FresviiGUIViewForum>();

				if(forum != null)
				{
					forum.ShowThreadByNotification(comment.ThreadId);
				}
			}
        }

        public void OnFriendshipRequestCreated(Fresvii.AppSteroid.Models.User user)
        {
            FriendRequestCount++;

            FresviiGUIViewMyProfile.Instance.frameMyProfile.GetComponent<FresviiGUIMyProfile>().AddFriend(user.ToFriend());
        }

        public void OnDirectMessageCreated(Fresvii.AppSteroid.Models.DirectMessage dm)
        {
            UnreadDirectMessageCount++;
        }

        public List<string> unreadGroupIds = new List<string>();

        public void AddUnreadGroupMessageGroupId(string groupId)
        {
            if (!unreadGroupIds.Contains(groupId))
            {
                unreadGroupIds.Add(groupId);
            }

            FresviiGUITabBar.messageBadgeCount = (uint)unreadGroupIds.Count;
        }

        public void RemoveUnreadGroupMessageGroupId(string groupId)
        {
            if (unreadGroupIds.Contains(groupId))
            {
                unreadGroupIds.Remove(groupId);
            }

            FresviiGUITabBar.messageBadgeCount = (uint)unreadGroupIds.Count;
        }

        public void ClearUnreadGroupMessageGroupId()
        {
            unreadGroupIds.Clear();

            FresviiGUITabBar.messageBadgeCount = 0;
        }

        public void OnTapFriendshipRequestCreated(Fresvii.AppSteroid.Models.User user)
        {
            ShowMyProfile();
        }

        public void OnTapFriendshipRequestUpdated(Fresvii.AppSteroid.Models.User user)
        {
            ShowMyProfile();
        }

        public void OnTapGroupMessageCreated(Fresvii.AppSteroid.Models.GroupMessage groupMessage)
        {
            ShowGroupMessage(groupMessage);
        }

        void OnTapForumOfficialThreadCreated(Fresvii.AppSteroid.Models.Thread thread)
        {
            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.Forum) > 0)
            {
                if (viewForum != null)
                {
                    FresviiGUIViewForum forum = viewForum.gameObject.GetComponent<FresviiGUIViewForum>();

                    if (forum != null)
                    {
                        forum.AddThread(thread);

                        StartCoroutine(WaitShowThread(thread));
                    }
                }
            }
        }

        private IEnumerator WaitShowThread(Fresvii.AppSteroid.Models.Thread thread)
        {
            yield return 1;

            viewForum.gameObject.GetComponent<FresviiGUIViewForum>().ShowThreadByNotification(thread.Id);
        }

        public void ShowMyProfile()
        {
            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.MyProfile) > 0)
            {
                SetViewMode(FASGui.Mode.MyProfile);

                SetTopFrame();               
            }
        }

        public void ShowGroupMessage(Fresvii.AppSteroid.Models.GroupMessage groupMessage)
        {
            FresviiGUIGroupMessage.initialGroupMessage = groupMessage;

            if (((int)FresviiGUIManager.GuiMode & (int)FASGui.Mode.GroupMessage) > 0)
            {
                SetViewMode(FASGui.Mode.GroupMessage);

                SetTopFrame();
            }
        }

        public float pollingInterval = 15f;

        IEnumerator PollingGetGroupMessageGroupList()
        {
            yield return new WaitForSeconds(3f);

            while (true)
            {
                if(OnGetGroupMessageGroups != null)
                    FASGroup.GetGroupMessageGroupList(OnGetGroupMessageGroups);

                yield return new WaitForSeconds(pollingInterval);
            }
        }

#if GROUP_CONFERENCE
        private static bool showVoiceChatNotification = false;

        private string voiceChatConnectingNotificationMessage = "";
#endif
        void Update()
        {
			if(!initialized) return;
#if GROUP_CONFERENCE
            showVoiceChatNotification = FASConference.IsCalling();

            if (showVoiceChatNotification)
            {
                FresviiGUIFrame.OffsetPosition = notificationOffset;
            }
            else
            {
                FresviiGUIFrame.OffsetPosition = Vector2.zero;
            }
#endif
        }

        private FresviiGUIFrame frameGroupConference;
        public GameObject prfbGUIFrameGroupCall;

        private bool controlLock;

#if GROUP_CONFERENCE

        public void ShowGroupConferenceGUI()
        {
			frameGroupConference = ((GameObject)Instantiate(prfbGUIFrameGroupCall)).GetComponent<FresviiGUIGroupConference>();
			
			frameGroupConference.transform.parent = this.transform;
			
			frameGroupConference.Position = new Vector2(0.0f, Screen.height);
			
			frameGroupConference.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, CurrentView.CurrentFrame.GuiDepth - 100);

			FresviiGUIFrame postFrame = CurrentView.CurrentFrame;
			
			frameGroupConference.SetDraw(true);
			
			frameGroupConference.PostFrame = postFrame;
			
			controlLock = true;
			
			frameGroupConference.Tween(new Vector2(0.0f, Screen.height), Vector2.zero, delegate()
			{
				controlLock = false;

                frameGroupConference.ControlLock = false;
				
				postFrame.SetDraw(false);
			});
        }

#endif

        void OnGUI()
        {
			if(!initialized) return;

            #if GROUP_CONFERENCE
			GUI.depth = CurrentView.CurrentFrame.GuiDepth - 500;

            if (showVoiceChatNotification)
            {
                Rect notificationPosition = new Rect(0, 0, Screen.width, notificationOffset.y);

                guiStyleNotificationButton.normal.textColor = new Color(notificationTextColor.r, notificationTextColor.g, notificationTextColor.b, 0.5f + 0.5f * Mathf.Cos(Time.time * Mathf.PI));

                GUI.DrawTextureWithTexCoords(notificationPosition, FresviiGUIColorPalette.Palette, textureCoordsNotificationBackGround);

                if (GUI.Button(notificationPosition, voiceChatConnectingNotificationMessage, guiStyleNotificationButton) && !controlLock)
                {
                    if (FresviiGUIGroupConference.Instance != null)
                    {
                        return;
                    }

                    frameGroupConference = ((GameObject)Instantiate(prfbGUIFrameGroupCall)).GetComponent<FresviiGUIGroupConference>();

                    frameGroupConference.transform.parent = this.transform;

                    frameGroupConference.Position = new Vector2(0.0f, Screen.height);
                        
                    frameGroupConference.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, CurrentView.CurrentFrame.GuiDepth - 100);

                    FresviiGUIFrame postFrame = CurrentView.CurrentFrame;

                    frameGroupConference.SetDraw(true);

                    frameGroupConference.PostFrame = postFrame;

                    controlLock = true;

                    frameGroupConference.Tween(new Vector2(0.0f, Screen.height), Vector2.zero, delegate()
                    {
                        controlLock = false;

                        postFrame.SetDraw(false);
                    });
                }
            }

            if (FASGui.showGuestGroupConferenceGui)
            {
                FASGui.showGuestGroupConferenceGui = false;

                if (FresviiGUIGroupConference.Instance != null)
                {
					FresviiGUIGroupConference.Instance.DoneWithCallback(()=>
					{
						ShowGroupConferenceGUI();
					});
                }
				else
				{
                    ShowGroupConferenceGUI();
				}                
            }

            #endif
        }
    }
}
