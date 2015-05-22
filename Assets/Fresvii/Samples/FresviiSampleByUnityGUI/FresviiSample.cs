using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Gui;


using Fresvii.AppSteroid.Services;

public class FresviiSample : MonoBehaviour {

    enum GUIMode { CommonService, Forum, Thread, CreateGroup, InGameChat, Groups };

    public static string logMessage = "";

    private Vector2 logScrollPosition = Vector2.zero;
    private Vector2 uiScrollPosition = Vector2.zero;

    private GUIMode mode = GUIMode.CommonService;
    private bool modeChanged = false;

    private IList<Fresvii.AppSteroid.Models.Thread> threads;
    private Fresvii.AppSteroid.Models.Thread selectedThread;
    private IList<Fresvii.AppSteroid.Models.Comment> comments;

    public GUISkin guiSkin;

    private Fresvii.AppSteroid.Models.User currentUser;

    private string userName = "username";

    public string returnSceneName;

    private IList<Fresvii.AppSteroid.Models.Group> groups = new List<Fresvii.AppSteroid.Models.Group>();

    public string leaderboardId;

    private Fresvii.AppSteroid.Models.Leaderboard leaderboard;

    private bool[] groupAdd;

    private Fresvii.AppSteroid.Models.Group inGameChatGroup;

    private string chat = "";

	private bool createGroupConference;

    List<Fresvii.AppSteroid.Models.User> signedUpUsers;

    private string matchMemberMinNumString = "2";

    private string matchMemberMaxNumString = "4";

    public string movieUrl = "";
   
    int matchMinMemberNum = 2;

    int matchMaxMemberNum = 4;

    IEnumerator Start()
    {
		if(FASGesture.Instance == null)
        {
			gameObject.AddComponent<FASGesture>();			
		}

        logMessage = "SDK Version: " + FAS.Version + "\n";

        logMessage += "UUID: " + FASConfig.Instance.appId + "\n\n";

        signedUpUsers = FASUser.LoadSignedUpUsers();

        if (signedUpUsers.Count > 0)
        {
            currentUser = signedUpUsers[signedUpUsers.Count - 1]; // saved user has only id and token data.

            logMessage += "current user id: " + currentUser.Id;

			Debug.Log("current user id: " + currentUser.Id);
		}
		else
		{
		    Debug.Log("#########  Save User is none");
		}

        FASGui.SetLeaderboardId(leaderboardId);

        while (string.IsNullOrEmpty(FAS.Host))
        {
            yield return 1;
        }

        logMessage = "SDK Version: " + FAS.Version + "\n";

        logMessage += "UUID: " + FASConfig.Instance.appId + "\n\n";

        logMessage += "Host: " + FAS.Host + "\n";

        if (signedUpUsers.Count > 0)
        {
            currentUser = signedUpUsers[signedUpUsers.Count - 1]; // saved user has only id and token data.

            logMessage += "current user id: " + currentUser.Id;
        }
    }

    void OnEnable()
	{
		FASEvent.OnGroupMessageInGameCreated += OnGroupMessageInGameCreated;
	}

	void OnDisable()
	{
		FASEvent.OnGroupMessageInGameCreated -= OnGroupMessageInGameCreated;
    }

	void Update(){

		int space = 10;

		int logAreaHeight = Screen.height / 3;

		Rect rectUiArea = new Rect(0, logAreaHeight + space * 3, Screen.width, Screen.height - logAreaHeight - space * 4);

		if(FASGesture.IsDragging || FASGesture.Inertia){
			
			Rect rectLogArea = new Rect(space, space, Screen.width, logAreaHeight);        
			
			if(rectLogArea.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y))){
				
				logScrollPosition += FASGesture.Delta;
			}
			
			if(rectUiArea.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y))){
				
				uiScrollPosition += FASGesture.Delta;
				
			}
		}
	}
			
    void OnGUI()
    {
		int space = 10;
		int largeLength = Mathf.Max(Screen.height, Screen.width);
		int lineHeight = largeLength / 12;

        int logAreaHeight = largeLength / 3;

		guiSkin.label.fontSize = lineHeight / 3;
		guiSkin.button.fontSize = lineHeight / 3;
		guiSkin.button.fixedHeight = lineHeight;
		guiSkin.button.fixedHeight = Screen.width / 5;
		guiSkin.textArea.fontSize = lineHeight / 3;
		
		guiSkin.verticalScrollbar.fixedWidth = 0;
		guiSkin.horizontalScrollbar.fixedWidth = 0;

		guiSkin.textField.fontSize = lineHeight / 3;
		guiSkin.textField.fixedHeight = lineHeight;

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

        #region Log
				
		logScrollPosition = GUILayout.BeginScrollView(logScrollPosition, guiSkin.scrollView, GUILayout.Height(logAreaHeight));
            GUILayout.Label("---------- Log ----------", guiSkin.label);
            GUILayout.Space(space);
			GUILayout.Label(logMessage, guiSkin.textArea, GUILayout.Height(lineHeight * 10));
        GUILayout.EndScrollView();

		#endregion

		uiScrollPosition = GUILayout.BeginScrollView(uiScrollPosition, false, false, guiSkin.horizontalScrollbar, guiSkin.verticalScrollbar, guiSkin.scrollView);

        #region Buttons        

        #region CommonService
        if (mode == GUIMode.CommonService)
        {
            if (modeChanged)
            {
                uiScrollPosition = Vector2.zero;

                modeChanged = false;
            }
       
            GUILayout.BeginHorizontal();

            GUILayout.Label("Tabs GUI", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            if (GUILayout.Button("Show", guiSkin.button))
            {
                FASGui.ShowGUI(FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.GroupMessage | FASGui.Mode.MyProfile, FASGui.Mode.Forum);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(space);

            //-----------------------------------------------
            GUILayout.BeginHorizontal();

            GUILayout.Label("Match making (Everyone)", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            if (GUILayout.Button("Show", guiSkin.button))
            {
                SetMatchMakingParameter();

                FASGui.ShowMatchMakingGui((uint)matchMinMemberNum, (uint)matchMaxMemberNum, null, null, null, FASMatchMaking.Recipient.Everyone, "");
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(space);

            //-----------------------------------------------
            GUILayout.BeginHorizontal();

            GUILayout.Label("Match making (Friend Only)", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            if (GUILayout.Button("Show", guiSkin.button))
            {
                SetMatchMakingParameter();

                FASGui.ShowMatchMakingGui((uint)matchMinMemberNum, (uint)matchMaxMemberNum, null, null, null, FASMatchMaking.Recipient.FriendOnly, "");
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(space);

            //-----------------------------------------------
            GUILayout.BeginHorizontal();

            GUILayout.Label("MatchMaking Min Number(2-16)", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            matchMemberMinNumString = GUILayout.TextField(matchMemberMinNumString, guiSkin.textField);

            GUILayout.EndHorizontal();

            GUILayout.Space(space * 4f);

            //-----------------------------------------------
            GUILayout.BeginHorizontal();

            GUILayout.Label("MatchMaking Max Number(2-16)", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            matchMemberMaxNumString = GUILayout.TextField(matchMemberMaxNumString, guiSkin.textField);

            GUILayout.EndHorizontal();

            GUILayout.Space(space * 4f);

            //-----------------------------------------------
#if UNITY_IOS
            GUILayout.BeginHorizontal();

            GUILayout.Label("Video Record", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            if (GUILayout.Button((FASPlayVideo.IsRecording()) ? "Stop" : "Start", guiSkin.button))
            {
                if (FASVideo.isRecording)
                {
                    FASPlayVideo.StopRecording();
                }
                else
                {
                    FASPlayVideo.StartRecording();
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(space);

            //-----------------------------------------------
            GUILayout.BeginHorizontal();

            GUILayout.Label("Latest Video Sharing", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            if (GUILayout.Button("Share", guiSkin.button))
            {
                if (!FASPlayVideo.ShowLatestVideoSharingGUI(Application.loadedLevelName))
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Error : Recorded video does not exist", delegate(bool del) { });
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(space);
#endif
            //-----------------------------------------------
            GUILayout.BeginHorizontal();

            GUILayout.Label("Report Score random(1,2000)", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            if (GUILayout.Button("Report", guiSkin.button))
            {
                if (FASUser.IsLoggedIn())
                {
                    if (this.leaderboard == null)
                    {
                        if (string.IsNullOrEmpty(leaderboardId))
                        {
                            logMessage = "leaderboardId is null or empty";

                            return;
                        }

                        FASLeaderboard.GetLeaderboard(leaderboardId, delegate(Fresvii.AppSteroid.Models.Leaderboard leaderboard, Fresvii.AppSteroid.Models.Error error)
                        {
                            if (error == null)
                            {
                                this.leaderboard = leaderboard;

                                FASLeaderboard.ReportScore(leaderboard.Id, ((Random.Range(1, 10) <= 1) ? Random.Range(1, 2000) : Random.Range(1, 1000)), delegate(Fresvii.AppSteroid.Models.Score score, Fresvii.AppSteroid.Models.Error error2)
                                {
                                    if (error2 == null)
                                    {
                                        logMessage = "Report score : " + score.Value;
                                    }
                                    else
                                    {
                                        logMessage = "Report score error";

                                        Debug.LogError(error2.ToString());
                                    }
                                });
                            }
                            else
                            {
                                logMessage = error.ToString();
                            }
                        });
                    }
                    else
                    {
                        FASLeaderboard.ReportScore(this.leaderboard.Id, ((Random.Range(1, 10) <= 1) ? Random.Range(1, 2000) : Random.Range(1, 1000)), delegate(Fresvii.AppSteroid.Models.Score score, Fresvii.AppSteroid.Models.Error error2)
                        {
                            if (error2 == null)
                            {
                                Debug.Log(score.User.Name + " : " + score.Value);

                                logMessage = "Report score : " + score.Value;
                            }
                            else
                            {
                                logMessage = "Report score error";

                                Debug.LogError(error2.ToString());
                            }
                        });
                    }
                }
                else
                {
                    logMessage = "Not login";
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(space);

            //-----------------------------------------------
            GUILayout.BeginHorizontal();

            GUILayout.Label("Get Custom Messages", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            if (GUILayout.Button("Get", guiSkin.button))
            {
                if (FASUser.IsLoggedIn())
                {
                    FASCustomMessage.GetCustomMessageList(delegate(IList<Fresvii.AppSteroid.Models.CustomMessage> customMessges, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
                    {
                        Debug.Log("============ Custom Messages == " + meta.TotalCount);

                        foreach (Fresvii.AppSteroid.Models.CustomMessage cm in customMessges)
                        {
                            Debug.Log(cm.Action);

                            if (cm.Params != null)
                            {

                                foreach (DictionaryEntry de in cm.Params)
                                {
                                    Debug.Log(de.Key + ", " + de.Value);
                                }
                            }
                        }
                    });
                    
                }
                else
                {
                    logMessage = "Not login";
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(space);

            GUILayout.Space(space * 3f);
                        
            //-----------------------------------------------
            GUILayout.BeginHorizontal();

            GUILayout.Label("LogIn", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            if (GUILayout.Button("LogIn", guiSkin.button))
            {
                logMessage = "LogIn start";

                if (currentUser != null)
                    FASUser.LogIn(currentUser.Id, currentUser.Token, true, OnSignIn);
                else
                    logMessage = "Not sign up";
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(2f * space);
					           
            //-----------------------------------------------
            GUILayout.BeginHorizontal();

            GUILayout.Label("User Name", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            userName = GUILayout.TextField(userName, guiSkin.textField);

            GUILayout.EndHorizontal();

            GUILayout.Space(space * 4f);
            
            //-----------------------------------------------
			GUILayout.BeginHorizontal();

				GUILayout.Label("SignUp", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

				GUILayout.Space(space);

				if (GUILayout.Button("SignUp", guiSkin.button))
	            {
#if !UNITY_EDITOR
					Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("OK", "Cancel", "Close");

					Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog("Sign up : Name = " + userName, delegate(bool del)
				    {
						if(del)
						{
#endif
							logMessage = "SignUp start";

#if UNITY_IOS
#if UNITY_5
                            Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.Gray);
#else
                            Handheld.SetActivityIndicatorStyle(iOSActivityIndicatorStyle.Gray);
#endif
#elif UNITY_ANDROID
			                Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
#endif
                            Handheld.StartActivityIndicator();

                            FASUser.LogOut();

                            FASUser.SignUp(userName, delegate(Fresvii.AppSteroid.Models.User user, Fresvii.AppSteroid.Models.Error error)
							{
                                Handheld.StopActivityIndicator();

								if (error != null)
								{
									logMessage = error.ToString();
	
                                    Debug.LogError(logMessage);
								}
								else
								{
									logMessage = "Sign up success: " + userName;
									
									currentUser = user;                            
								}
							});
#if !UNITY_EDITOR
						}					
					});
#endif
                }

			GUILayout.EndHorizontal();

            GUILayout.Space(space);

			


            //-----------------------------------------------
            GUILayout.BeginHorizontal();

            GUILayout.Label("LogOut", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));

            GUILayout.Space(space);

            if (GUILayout.Button("LogOut", guiSkin.button))
            {
                logMessage = "Log out";
                if (currentUser != null)
                    FASUser.LogOut();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(space);
			
			//-----------------------------------------------
			GUILayout.BeginHorizontal();
			
			GUILayout.Label("Register Notification", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));
			
			GUILayout.Space(space);

			if (GUILayout.Button("Register", guiSkin.button))
            {
                logMessage = "Register Notification";

                FASNotification.RegisterRemoteNotification((info, error) =>
                {
                    if (error != null)
                    {
                        Debug.LogError(error.ToString());

                        logMessage = "Register Notification Error : " + error.ToString();
                    }
                    else
                    {
                        Debug.Log("#### Register Notification Success : " + info.CertificateType);

                        logMessage = "Register Notification Success";
                    }       
                });
            }

			GUILayout.EndHorizontal();

            GUILayout.Space(space);

			//-----------------------------------------------
			GUILayout.BeginHorizontal();
			
			GUILayout.Label("Unregister Notification", guiSkin.label, GUILayout.Height(lineHeight), GUILayout.Width(Screen.width * .55f));
			
			GUILayout.Space(space);

			if (GUILayout.Button("Unregister", guiSkin.button))
            {
				logMessage = "Unregister Notification";

                FASNotification.UnregisterRemoteNotification(OnUnregisterRemoteNotification);
            }

			GUILayout.EndHorizontal();

            GUILayout.Space(space);
            
            //-----------------------------------------------
            if (GUILayout.Button("In Game Chat", guiSkin.button))
            {
                mode = GUIMode.Groups;

                modeChanged = true;
            }
            GUILayout.Space(space);

            // ----------------------
        }
#endregion
		else if (mode == GUIMode.Groups)
		{
			if (modeChanged)
			{
				uiScrollPosition = Vector2.zero;
				
				modeChanged = false;

                FASGroup.GetGroupList(delegate(IList<Fresvii.AppSteroid.Models.Group> groups, Fresvii.AppSteroid.Models.Error error)
				{
					if(error == null){

						this.groups = groups;


						foreach(Fresvii.AppSteroid.Models.Group group in this.groups)
						{
							group.FetchMembers(delegate {});
						}
					}
					else
					{
						logMessage = error.ToString();
						Debug.LogError(error.ToString());
					}
				});
			}

			if(this.groups != null)
			{
				foreach(Fresvii.AppSteroid.Models.Group group in this.groups){

					GUILayout.Space(space);

					string members = "";

					if(group.Members != null)
					{
						foreach(Fresvii.AppSteroid.Models.Member member in group.Members)
						{
							members += member.Name + ", ";
						}
					}
					
					if (GUILayout.Button(members, guiSkin.button))
					{
						modeChanged = true;

						mode = GUIMode.InGameChat;

						inGameChatGroup = group;
					}				
				}
			}

			GUILayout.Space(space * 2f);
			
			if (GUILayout.Button("Back", guiSkin.button))
			{
				mode = GUIMode.CommonService;
				
				modeChanged = true;
			}
		}
        
        else if (mode == GUIMode.InGameChat)
        {
            if (modeChanged)
            {
                uiScrollPosition = Vector2.zero;

                modeChanged = false;

				logMessage = "";
            }

            GUILayout.Space(space);

            GUILayout.Label("Chat", guiSkin.label, GUILayout.Height(lineHeight));

            chat = GUILayout.TextArea(chat, guiSkin.textArea, GUILayout.Height(lineHeight));

            GUILayout.Space(space);

            if (GUILayout.Button("Send", guiSkin.button))
            {
                FASGroup.SendGroupMessageInGames(inGameChatGroup.Id, chat, delegate(Fresvii.AppSteroid.Models.GroupMessage groupMessage, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error != null)
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error) 
                            Debug.LogError(error.ToString());
                    }
                    else
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
                            Debug.Log(groupMessage.Text);
                    }
                });

                chat = "";
            }

            GUILayout.Space(space * 2f);

            if (GUILayout.Button("Back", guiSkin.button))
            {
                mode = GUIMode.CommonService;

                modeChanged = true;
            }
        }

        GUILayout.EndScrollView();		

        #endregion

        GUILayout.EndArea();
    }

    void ScoreReport()
    {

    }

    bool SetMatchMakingParameter()
    {        
        try
        {
            matchMinMemberNum = int.Parse(matchMemberMinNumString);

            matchMaxMemberNum = int.Parse(matchMemberMaxNumString);
        }
        catch
        {
            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Match Member count is invalid", delegate(bool del)
            {

            });

            Debug.LogError("Match Member count is invalid");

            matchMemberMinNumString = "2";

            matchMemberMaxNumString = "4";

            return false; 
        }

        if (matchMinMemberNum < 2 || matchMinMemberNum > 16 || matchMaxMemberNum < 2 || matchMaxMemberNum > 16 || matchMinMemberNum > matchMaxMemberNum)
        {
            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Match Member count is invalid", delegate(bool del)
            {

            });

            Debug.LogError("Match Member count is invalid");

            matchMemberMinNumString = "2";

            matchMemberMaxNumString = "4";

            return false;
        }

        return true;
    }

    //--------------------------------------------------------------------
    // Callbacks
    //--------------------------------------------------------------------

    void OnSignIn(Fresvii.AppSteroid.Models.Error error)
    {
        if (error != null)
        {
            logMessage = error.ToString();
            Debug.LogError(logMessage);
            return;
        }

        logMessage = FAS.Instance.Client.AccessToken.Token + ", " +  FAS.Instance.Client.AccessToken.ExpiresAt;

        FASVideo.UploadVideo(movieUrl, (video, e) => { }, (p) => { Debug.Log(p.ToString()); });
    }

    void OnGetSnsAccountList(IList<Fresvii.AppSteroid.Models.SnsAccount> snsAccountList, Fresvii.AppSteroid.Models.Error error)
    {
        if (error != null)
        {
            Debug.LogError(error.ToString());
            return;
        }

        logMessage = "";
        foreach (Fresvii.AppSteroid.Models.SnsAccount snsAccount in FAS.Instance.Client.User.SnsAccounts)
            logMessage += snsAccount.Id + ", " + snsAccount.Provider + ", " + snsAccount.Uid + ", " + snsAccount.CreatedAt + ", " + snsAccount.UpdatedAt + "\n";

        Debug.Log(logMessage);
    }

    void OnSetSnsAccount(Fresvii.AppSteroid.Models.SnsAccount snsAccount, Fresvii.AppSteroid.Models.Error error)
    {
        if (error != null)
        {
            logMessage = error.ToString();
            Debug.LogError(logMessage);
            return;
        }

        logMessage = snsAccount.Id + ", " + snsAccount.Provider + ", " + snsAccount.Uid + ", " + snsAccount.CreatedAt + ", " + snsAccount.UpdatedAt;
        Debug.Log(logMessage);
    }

    void OnDeleteSnsAccount(Fresvii.AppSteroid.Models.Error error)
    {
        if (error != null)
        {
            logMessage = error.ToString(); ;
            Debug.LogError(logMessage);
            return;
        }

        logMessage = "Delete Success";
        Debug.Log(logMessage);
    }

    void OnGetUser(Fresvii.AppSteroid.Models.User user, Fresvii.AppSteroid.Models.Error error)
    {
        if (error != null)
        {
            logMessage = error.ToString();
            Debug.LogError(logMessage);
            return;
        }

        logMessage = user.Name + ", " + user.UserCode + ", " + user.Id + ", " + user.CreatedAt + ", " + user.UpdatedAt;
        Debug.Log(logMessage);       
    }

    void OnGetUserList(IList<Fresvii.AppSteroid.Models.User> users, Fresvii.AppSteroid.Models.Error error)
    {
        if (error != null)
        {
            logMessage = error.ToString();
            Debug.LogError(logMessage);
            return;
        }

        logMessage = "";
        foreach (Fresvii.AppSteroid.Models.User user in users)
            logMessage += user.Name + ", " + user.UserCode + ", " + user.Id + ", " + user.CreatedAt + ", " + user.UpdatedAt + "\n";

        Debug.Log(logMessage);

    }  

    void OnUnregisterRemoteNotification(Fresvii.AppSteroid.Models.Error error)
    {
        if (error != null)
        {
            logMessage = error.ToString();
            Debug.LogError(logMessage);
        }
        else
        {
            logMessage = "Success to unregister notification";

            Debug.Log(logMessage);
        }
    }

	void OnGroupMessageInGameCreated(Fresvii.AppSteroid.Models.GroupMessage groupMessage)
    {
		Debug.Log("In Game chat : " + groupMessage.User.Name + " : " + groupMessage.Text);

		logMessage += groupMessage.User.Name + " : " + groupMessage.Text + "\n";
	}

}
