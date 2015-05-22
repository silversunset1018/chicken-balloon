#if GROUP_CONFERENCE
#pragma warning disable 0219, 0414

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{    
    public class FresviiGUIGroupConference : FresviiGUIFrame
    {
        public static FresviiGUIGroupConference Instance;

        public enum Role { Host, Guest };

        private FresviiGUIGroupConferenceTop groupConferenceTop;

        private Rect baseRect;
        
        private float scaleFactor;
        
        private string postFix;

        private static Fresvii.AppSteroid.Models.GroupConference groupConference;

        public Fresvii.AppSteroid.Models.Group Group { get; protected set; }

        private FASConference.ConferenceStates conferenceState = FASConference.ConferenceStates.None;

        private FASConference.CallStates myCallState = FASConference.CallStates.Idle;

        public Rect callButtonPosition;

        public Rect messageButtonPosition;

        public float buttonVMargin = 65f;

        public float buttonHMargin = 40f;

        private Texture2D textureCallButton, textureCallEndButton, textureCallEndHButton, textureMessageButton;

        private Texture2D textureMicrophoneOn, textureMicrophoneOff, textureSpeakerOn, textureSpeakerOff;

        public GUIStyle guiStyleButtonLabel;

        private Rect callLabelPosition;

        private Rect messageLabelPosition;

        private Dictionary<string, Texture2D> userIcons = new Dictionary<string, Texture2D>();

        public Texture2D userIconDefault;

        public float userIconSideMargin = 30f;

        public float userIconCenterMargin = 15f;

        private float userIconCenterLine = 140f;

        private Rect[] userIconPositions;

        private Rect[] userNamePositions;

        private string[] displayUserNames;

        public Texture2D userIconMaskOutside;

        private Color bgColor;

        private Rect separateLine;

        private Texture2D palette;

        private Rect coodsSeperateLine;

        public GUIStyle guiStyleInfoLabel;

        public GUIStyle guiStyleUserName;

        private Rect infoLabelPosition;

        public FresviiGUIButton callButton;

        public FresviiGUIButton callEndButton;

        public FresviiGUIButton messageButton;

        public FresviiGUIButton speakerButton;

        public FresviiGUIButton microphoneButton;

        private Rect speakerButtonPosition;

        private Rect microphoneButtonPosition;

        public GameObject prfbFrameChat;

        private FresviiGUIFrame frameChat;

        private bool groupFetching;

        private bool groupMemberFetching;

        public Vector2 loadingSpinnerSize;

        private Rect loadingSpinnerPosition;

        Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private bool isRadey = false;

        public FresviiGUIRangeSlider microphoneSlider;

        public FresviiGUIRangeSlider speakerSlider;

        private static bool isMute;

        void Awake()
        {
            Instance = this;
        }

		IEnumerator Start()
		{
			yield return 1;
			
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				Handheld.StopActivityIndicator();
			}
			else if (Application.platform == RuntimePlatform.Android)
			{
				Fresvii.AppSteroid.Util.DialogManager.Instance.HideProgressSpinnerDialog();
			}
		}


        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            StartCoroutine(InitCoroutine(appIcon, postFix, scaleFactor, guiDepth));
        }

        public IEnumerator InitCoroutine(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            this.GuiDepth = guiDepth;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleButtonLabel.font = null;

                guiStyleInfoLabel.font = null;

                guiStyleUserName.font = null;
            }

            groupConferenceTop = GetComponent<FresviiGUIGroupConferenceTop>();
            
            groupConferenceTop.Init(appIcon, postFix, scaleFactor, this);

            this.scaleFactor = scaleFactor;

            this.postFix = postFix;

            guiStyleUserName.normal.textColor = guiStyleButtonLabel.normal.textColor = guiStyleInfoLabel.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.GroupCallText);

            bgColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.MainBackground);

            callButtonPosition = FresviiGUIUtility.RectScale(callButtonPosition, this.scaleFactor);

            float minScreenLength = Mathf.Min(Screen.width, Screen.height);

            float sizeWeight = minScreenLength / 640f;

            buttonVMargin *= sizeWeight;

            buttonHMargin *= sizeWeight;

            userIconSideMargin *= sizeWeight;

            userIconCenterMargin *= sizeWeight;

            loadingSpinnerSize *= scaleFactor;

            palette = FresviiGUIColorPalette.Palette;

            coodsSeperateLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.GroupCallText);

            textureCallButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconMessageCall + this.postFix, false);
            
            textureCallEndButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconCallEnd + this.postFix, false);

            textureCallEndHButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconCallEndH + this.postFix, false);

            textureMessageButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconMessage + this.postFix, false);

            textureMicrophoneOn = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconMuteOff + this.postFix, false);
            
            textureMicrophoneOff = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconMuteOn + this.postFix, false);
            
            textureSpeakerOn = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconSpeakerOn + this.postFix, false);

            textureSpeakerOff = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconSpeakerOff + this.postFix, false);

            bool groupGetting = false;

			string groupId = FASConference.GetGroupId();

			if (!string.IsNullOrEmpty(groupId))
            {
                groupGetting = true;

				Group = null;

                loadingSpinnerPosition = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

                loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GuiDepth - 10);

                FASGroup.GetGroup(FASConference.GetGroupId(), (group, error) =>
                {
                    groupGetting = false;

                    if (loadingSpinner != null)
                    {
                        loadingSpinner.Hide();
                    }

                    if (error != null)
                    {
            
                    }
                    else
                    {
                        Group = group;
                    }
                });
            }

            while (groupGetting)
            {
                yield return 1;
            }

            if (Group != null)
            {
                if (Group.Members.Count == 0)
                {
                    GroupFetchMembers();
                }
                else
                {
                    CalcLayout();

                    foreach (Fresvii.AppSteroid.Models.Member member in Group.Members)
                    {
                        if (!string.IsNullOrEmpty(member.ProfileImageUrl))
                        {
                            LoadUserIcon(member.ProfileImageUrl);
                        }
                    }

                    isRadey = true;
                }
            }
            else
            {
                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });

                yield break;
            }

            speakerSlider.GuiDepth = GuiDepth - 1;

            speakerSlider.Title = FresviiGUIText.Get("SpeakerVolume");

            microphoneSlider.GuiDepth = GuiDepth - 1;

            microphoneSlider.Title = FresviiGUIText.Get("MicrophoneVolume");

            if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            {
                Debug.LogWarning("Voice chat is unable on Editor");

                myCallState = FASConference.CallStates.Calling;
            }
            else
            {
                if (FASConference.IsConnected())
                {
                    myCallState = FASConference.CallStates.Connected;
                }
                else
                {
                    myCallState = FASConference.CallStates.Calling;

                    connectedDateTime = System.DateTime.MinValue;

                    FASConference.GetCurrentConference(Group.Id, delegate(Fresvii.AppSteroid.Models.GroupConference _groupConference, Fresvii.AppSteroid.Models.Error _error)
                    {
                        groupConference = _groupConference;

                        if (_error == null)
                        {
                            if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
                            {
                                Debug.Log("GetCurrentConference: current conference exists");
                            }

                            FASConference.JoinConference(_groupConference, FAS.OnVoiceChatError);
                        }
                        else
                        {
                            if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
                            {
                                Debug.Log("GetCurrentConference: Start new conference");
                            }

                            FASConference.StartConference(Group.Id, FAS.OnVoiceChatError, FAS.OnVoiceChatError);
                        }
                    });
                }
            }
        }

        public void GetGroup()
        {
            FAS.Instance.Client.GroupService.GetGroup(FASConference.GetGroupId(), OnGetGroup);                
        }

        public void SetGroup(Fresvii.AppSteroid.Models.Group group)
        {
            Group = group;
        }

        public void OnGetGroup(Fresvii.AppSteroid.Models.Group group, Fresvii.AppSteroid.Models.Error error)
        {
            if (error == null)
            {
                this.Group = group;

                GroupFetchMembers();
            }
            else
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                {
                    Debug.LogError("GetGroup Error: " + error.ToString());
                }
            }
        }

        void GroupFetchMembers()
        {
            Group.FetchMembers(OnFetchMembers);
        }

        void OnFetchMembers(Fresvii.AppSteroid.Models.Error error)
        {
            if (this == null) return;

            if (error != null)
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                {
                    Debug.LogError("Group.FetchMembers : " + error.ToString());
                }

                StartCoroutine(GroupFetchMembersWithDelay());
            }
            else
            {
                isRadey = true;

                if (loadingSpinner != null)
                {
                    loadingSpinner.Hide();
                }

                CalcLayout();

                foreach (Fresvii.AppSteroid.Models.Member member in Group.Members)
                {
                    if (!string.IsNullOrEmpty(member.ProfileImageUrl))
                    {
                        LoadUserIcon(member.ProfileImageUrl);
                    }
                }
            }
        }

        void CalcLayout()
        {
            if (loadingSpinner != null)
            {
                loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }

            if (Group == null) return;

            if (Group.Members == null) return;

            userIconPositions = new Rect[Group.Members.Count];

            userNamePositions = new Rect[Group.Members.Count];

            displayUserNames = new string[Group.Members.Count];

            float minScreenLength = Mathf.Min(Screen.width, Screen.height);

            guiStyleButtonLabel.fontSize = (int)(minScreenLength / 20f);

            guiStyleInfoLabel.fontSize = (int)(minScreenLength / 20f);

            guiStyleUserName.fontSize = (int)(minScreenLength / 25f);

            float sizeWeight = minScreenLength / 640f;

            if (Screen.height > Screen.width) // portrait
            {
                callButtonPosition.width = callButtonPosition.height = minScreenLength * 0.15f;

                callButtonPosition.x = baseRect.width * 0.67f - callButtonPosition.width * 0.5f;

                callButtonPosition.y = baseRect.height - userIconSideMargin * 2f - callButtonPosition.height;


                messageButtonPosition.width = messageButtonPosition.height = callButtonPosition.width;

                messageButtonPosition.x = baseRect.width * 0.33f - messageButtonPosition.width * 0.5f;

                messageButtonPosition.y = callButtonPosition.y;


                guiStyleButtonLabel.alignment = TextAnchor.MiddleCenter;

                callLabelPosition = new Rect(callButtonPosition.x + callButtonPosition.width * 0.5f - Screen.width * 0.25f, callButtonPosition.y + callButtonPosition.height, baseRect.width * 0.5f, baseRect.height - callButtonPosition.y - callButtonPosition.height);

                messageLabelPosition = new Rect(messageButtonPosition.x + messageButtonPosition.width * 0.5f - Screen.width * 0.25f, callLabelPosition.y, baseRect.width * 0.5f, callLabelPosition.height);

                float userIconLength = (Screen.width - 2f * userIconSideMargin - (Group.Members.Count - 1) * userIconCenterMargin) / Group.Members.Count;

                userIconLength = Mathf.Min(userIconLength, Screen.width * 0.25f);

                separateLine = new Rect(userIconSideMargin, baseRect.height - baseRect.width / 1.618f, baseRect.width - 2f * userIconSideMargin, 1.0f);

                userIconCenterLine = separateLine.y * 0.4f;

                float initX = Screen.width * 0.5f - userIconCenterMargin * (Group.Members.Count - 1f) * 0.5f -userIconLength * Group.Members.Count * 0.5f;

                for (int i = 0; i < Group.Members.Count; i++)
                {
                    //float x = ((i == 0) ? (userIconSideMargin) : (userIconPositions[i - 1].x + userIconCenterMargin + userIconLength));

                    float x = ((i == 0) ? (initX) : (userIconPositions[i - 1].x + userIconCenterMargin + userIconLength));

                    userIconPositions[i] = new Rect(x, userIconCenterLine - userIconLength * 0.5f, userIconLength, userIconLength);

                    displayUserNames[i] = FresviiGUIUtility.Truncate(Group.Members[i].Name, guiStyleUserName, userIconLength, "..."); 

                    float nameHeight = guiStyleUserName.CalcHeight(new GUIContent( Group.Members[i].Name), userIconLength);
                    
                    userNamePositions[i] = new Rect(x, userIconPositions[i].y + userIconPositions[i].height + userIconCenterMargin * 0.5f, userIconLength, nameHeight);
                }

                infoLabelPosition = new Rect(0f, userNamePositions[0].y + userNamePositions[0].height, baseRect.width, separateLine.y - userNamePositions[0].y - userNamePositions[0].height);

                float functionButtonLength = callButtonPosition.height / 1.618f;

                //microphoneButtonPosition = new Rect(baseRect.width * 0.33f - functionButtonLength * 0.5f, separateLine.y + (callButtonPosition.y - separateLine.y) * 0.5f - functionButtonLength * 0.5f, functionButtonLength, functionButtonLength);

                //speakerButtonPosition = new Rect(0.67f * baseRect.width - functionButtonLength * 0.5f, separateLine.y + (callButtonPosition.y - separateLine.y) * 0.5f - functionButtonLength * 0.5f, functionButtonLength, functionButtonLength);            

                microphoneButtonPosition = new Rect(baseRect.width * 0.5f - functionButtonLength * 0.5f, separateLine.y + (callButtonPosition.y - separateLine.y) * 0.5f - functionButtonLength * 0.5f, functionButtonLength, functionButtonLength);

            }
            else // landscape
            {                
                separateLine = new Rect(userIconSideMargin, baseRect.height / 1.618f, baseRect.width - 2f * userIconSideMargin, 1.0f);

                userIconCenterLine = separateLine.y * 0.35f;

                float userIconLength = 0f;

                float fullWidth = 0f;

                if (Group.Members.Count <= 4)
                {
                    userIconLength = separateLine.y * 0.5f;

                    fullWidth = Group.Members.Count * userIconLength + 3f * userIconCenterMargin * (Group.Members.Count - 1);

                    for (int i = 0; i < Group.Members.Count; i++)
                    {
                        float x = ((i == 0) ? (0.5f * (baseRect.width - fullWidth)) : (userIconPositions[i - 1].x + 3f * userIconCenterMargin + userIconLength));

                        userIconPositions[i] = new Rect(x, userIconCenterLine - userIconLength * 0.5f, userIconLength, userIconLength);

                        displayUserNames[i] = FresviiGUIUtility.Truncate(Group.Members[i].Name, guiStyleUserName, userIconLength, "..."); 

                        float nameHeight = guiStyleUserName.CalcHeight(new GUIContent(Group.Members[i].Name), userIconLength);

                        userNamePositions[i] = new Rect(x, userIconPositions[i].y + userIconPositions[i].height + userIconCenterMargin * 0.5f, userIconLength, nameHeight);
                    }
                }
                else
                {
                    userIconLength = (separateLine.width - userIconCenterMargin * (Group.Members.Count - 1)) / Group.Members.Count;

                    fullWidth = separateLine.width;

                    for (int i = 0; i < Group.Members.Count; i++)
                    {
                        float x = ((i == 0) ? separateLine.x : (userIconPositions[i - 1].x + userIconCenterMargin + userIconLength));

                        userIconPositions[i] = new Rect(x, userIconCenterLine - userIconLength * 0.5f, userIconLength, userIconLength);
                    }
                }

                infoLabelPosition = new Rect(0f, userNamePositions[0].y + userNamePositions[0].height, baseRect.width, separateLine.y - userNamePositions[0].y - userNamePositions[0].height);

                callButtonPosition.width = callButtonPosition.height = (baseRect.height - separateLine.y) * 0.33f;

                callButtonPosition.x = baseRect.width * 0.6f + userIconSideMargin - textureCallButton.width * 0.5f;

                callButtonPosition.y = baseRect.height - userIconSideMargin * 0.5f - callButtonPosition.height;

                callLabelPosition = new Rect(callButtonPosition.x + callButtonPosition.width + userIconSideMargin, callButtonPosition.y, Screen.width * 0.5f, callButtonPosition.height);


                messageButtonPosition.x = baseRect.width * 0.2f + userIconSideMargin - textureMessageButton.width * 0.5f;

                messageButtonPosition.y = callButtonPosition.y;

                messageButtonPosition.width = messageButtonPosition.height = callButtonPosition.width;

                messageLabelPosition = new Rect(messageButtonPosition.x + messageButtonPosition.width + userIconSideMargin, messageButtonPosition.y, Screen.width * 0.5f, callButtonPosition.height);


                guiStyleButtonLabel.alignment = TextAnchor.MiddleLeft;


                float functionButtonLength = callButtonPosition.height / 1.618f;
                                
                
                //microphoneButtonPosition = new Rect(baseRect.width * 0.33f - functionButtonLength * 0.5f, separateLine.y + (callButtonPosition.y - separateLine.y) * 0.5f - functionButtonLength * 0.5f, functionButtonLength, functionButtonLength);

                //speakerButtonPosition = new Rect(0.67f * baseRect.width - functionButtonLength * 0.5f * sizeWeight, separateLine.y + (callButtonPosition.y - separateLine.y) * 0.5f - functionButtonLength * 0.5f, functionButtonLength, functionButtonLength);

                microphoneButtonPosition = new Rect(baseRect.width * 0.5f - functionButtonLength * 0.5f, separateLine.y + (callButtonPosition.y - separateLine.y) * 0.5f - functionButtonLength * 0.5f, functionButtonLength, functionButtonLength);
            
            }
        }

        void LoadUserIcon(string url)
        {
            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(url, true, delegate(Texture2D texture)
            {
                if (texture == null)
                {
                    LoadUserIcon(url);
                }
                else
                {
                    if (!userIcons.ContainsKey(url))
                    {
                        userIcons.Add(url, texture);
                    }
                }
            });
        }

        public float groupFetchMembersWithDelayTime = 2.0f;

        IEnumerator GroupFetchMembersWithDelay()
        {
            yield return new WaitForSeconds(groupFetchMembersWithDelayTime);

            GroupFetchMembers();
        }

        void OnEnable()
        {
            FASConference.OnCallStateChanged += OnCallStateChanged;

            FASConference.OnConferenceStateChanged += OnConferenceStateChanged;

            microphoneSlider.OnValueChanged += VolumeChanged;

            speakerSlider.OnValueChanged += VolumeChanged;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Fresvii.AppSteroid.Util.Sensor.Init();

                Fresvii.AppSteroid.Util.Sensor.OnProximitySensorChanged += OnProximitySensorChanged;
            }
        }

        void VolumeChanged()
        {
             if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                 FASConference.Volume(microphoneSlider.Value, speakerSlider.Value);
        }

        void OnProximitySensorChanged(float value)
        {
            ControlLock = (value == 0.0f);

            if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
            {
                Debug.Log("OnProximitySensorChanged : " + value + "\nControl lock : " + ControlLock);
            }
        }

        void OnDisable()
        {
            FASConference.OnCallStateChanged -= OnCallStateChanged;

            FASConference.OnConferenceStateChanged -= OnConferenceStateChanged;

            microphoneSlider.OnValueChanged -= VolumeChanged;

            speakerSlider.OnValueChanged -= VolumeChanged;

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Fresvii.AppSteroid.Util.Sensor.OnProximitySensorChanged -= OnProximitySensorChanged;

                Fresvii.AppSteroid.Util.Sensor.Release();
            }

            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            groupConferenceTop.enabled = on;            
        }

        void OnConferenceStateChanged(FASConference.ConferenceStates conferenceState)
        {
            if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
            {
                Debug.Log("OnConferenceStateChanged : " + conferenceState.ToString());
            }

            this.conferenceState = conferenceState;

            if (conferenceState == FASConference.ConferenceStates.Destroyed)
            {
                myCallState = FASConference.CallStates.Idle;

                connectedDateTime = System.DateTime.MinValue;
            }
        }

        private static System.DateTime connectedDateTime;

        void OnCallStateChanged(string groupId, string userId, FASConference.CallStates callState)
        {
            if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
            {
                Debug.Log("OnCallStateChanged : " + callState.ToString() + " userId : " + userId);
            }

            if (callState == FASConference.CallStates.Connected)
            {
                FASConference.GetGroupConferenceParticipants(groupId, delegate(IList<Fresvii.AppSteroid.Models.GroupConferenceParticipant> participants, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error == null && meta != null)
                    {
                        if (meta.TotalCount >= 2)
                        {
                            foreach (Fresvii.AppSteroid.Models.GroupConferenceParticipant participant in participants)
                            {
                                if (participant.Id == FAS.CurrentUser.Id)
                                {
                                    myCallState = FASConference.CallStates.Connected;

                                    break;
                                }
                            }

                            if (myCallState == FASConference.CallStates.Connected)
                            {
                                if (connectedDateTime == System.DateTime.MinValue)
                                {
                                    connectedDateTime = System.DateTime.Now;
                                }
                            }
                            else
                            {
                                connectedDateTime = System.DateTime.MinValue;
                            }

                            if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
                            {
                                Debug.Log("OnCallStateChanged myCallState: " + myCallState.ToString());
                            }
                        }
                        else
                        {
                            myCallState = FASConference.CallStates.Idle;
                        }
                    }
                    else
                    {
                        myCallState = FASConference.CallStates.Idle;
                    }
                });
            }            
        }

        public void Done()
        {
            if (PostFrame != null)
            {
                PostFrame.SetDraw(true);

                this.Tween(Vector2.zero, new Vector2(0.0f, Screen.height), delegate()
                {
                    Destroy(this.gameObject);
                });
            }
            else
            {
                FASGui.ShowGUI(FASGui.Mode.GroupMessage, FresviiGUIManager.ReturnSceneName);
            }
        }

		public void DoneWithCallback(Action callback){

			if (PostFrame != null)
			{
				PostFrame.SetDraw(true);
				
				this.Tween(Vector2.zero, new Vector2(0.0f, Screen.height), delegate()
				{
					Destroy(this.gameObject);

					callback();
				});
			}
			else
			{
				FASGui.ShowGUI(FASGui.Mode.GroupMessage, FresviiGUIManager.ReturnSceneName);
			}
		}

        private float postScreenWidth = 0f;

        private bool volumeControl;

        Vector2 postOffset;

        void OnDestroy()
        {
            Instance = null;
        }

        void Update()
        {
            //backgroundRect = new Rect(Position.x, Position.y - FresviiGUIFrame.OffsetPosition.y, Screen.width, Screen.height);

            if (loadingSpinner != null)
            {
                loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }

            if (groupConferenceTop == null)
            {
                return;
            }

            this.baseRect = new Rect(Position.x, Position.y + groupConferenceTop.height, Screen.width, Screen.height - groupConferenceTop.height - FresviiGUIFrame.OffsetPosition.y);

            if (postScreenWidth != Screen.width || postOffset != FresviiGUIFrame.OffsetPosition)
            {
                postScreenWidth = Screen.width;

                postOffset = FresviiGUIFrame.OffsetPosition;

                CalcLayout();
            }
        }

        bool callEnding = false;

        IEnumerator DoneDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            callEnding = false;

            Done();
        }

        void OnGUI()
        {            
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(baseRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            if (!isRadey) return;
            
            GUI.BeginGroup(baseRect);

            {
                //int i = 0;

                for (int i = 0; i < Group.Members.Count; i++)
                {
                    Fresvii.AppSteroid.Models.Member member = Group.Members[i];

                    if (userIcons.ContainsKey(member.ProfileImageUrl))
                    {
                        GUI.DrawTexture(userIconPositions[i], userIcons[member.ProfileImageUrl]);
                    }
                    else
                    {
                        GUI.DrawTexture(userIconPositions[i], userIconDefault);
                    }

                    Color tmp = GUI.color;

                    GUI.color = bgColor;

                    GUI.DrawTexture(userIconPositions[i], userIconMaskOutside);

                    GUI.color = tmp;

                    GUI.Label(userNamePositions[i], displayUserNames[i], guiStyleUserName);

                    //i++;
                }
            }

            Event e = Event.current;

            if (myCallState != FASConference.CallStates.Idle)
            {                
                if (myCallState == FASConference.CallStates.Calling || myCallState == FASConference.CallStates.Progressing)
                {
                    GUI.Label(infoLabelPosition, FresviiGUIText.Get("Calling"), guiStyleInfoLabel);
                }
                /*else if (FASConference.IsConnected())
                {
                    ulong startTimeStamp = FASConference.GetConferenceStartTimestamp();

                    if (startTimeStamp != 0)
                    {
                        System.DateTime startTime = FASConference.GetStartTime();

                        if (startTime != System.DateTime.MinValue)
                            GUI.Label(infoLabelPosition, FresviiGUIUtility.TimeSpanWatch(startTime), guiStyleInfoLabel);
                    }
                }*/

                if (callEndButton.IsTap(e, callButtonPosition, callButtonPosition, FresviiGUIButton.ButtonType.TextureOnly, textureCallEndButton, textureCallEndHButton, textureCallEndHButton) && !callEnding)
                {
                    if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        FASConference.Leave();
                    }

                    callEnding = true;

                    StartCoroutine(DoneDelay(2.0f));
                }

                GUI.Label(callLabelPosition, FresviiGUIText.Get("End"), guiStyleButtonLabel);
            }
            else
            {
                ControlLock = false;

                if (callButton.IsTap(e, callButtonPosition, callButtonPosition, FresviiGUIButton.ButtonType.TextureOnly, textureCallButton, textureCallButton, textureCallButton))
                {
                    if (!ControlLock)
                    {
                        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                        {
                            FAS.Instance.Client.GroupConferenceService.GetCurrentConference(Group.Id, delegate(Fresvii.AppSteroid.Models.GroupConference _groupConference, Fresvii.AppSteroid.Models.Error _error)
                            {
                                if (_error == null)
                                {
                                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
                                    {
                                        Debug.Log("GetCurrentConference: current conference exists");
                                    }

                                    FAS.Instance.Client.GroupConferenceService.DeleteConference(Group.Id, delegate(Fresvii.AppSteroid.Models.Error _error2)
                                    {
                                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
                                        {
                                            Debug.Log("DeleteConference: current conference delete process done");
                                        }

                                        FASConference.StartConference(Group.Id, FAS.OnVoiceChatError, FAS.OnVoiceChatError);
                                    });
                                }
                                else
                                {
                                    FASConference.StartConference(Group.Id, FAS.OnVoiceChatError, FAS.OnVoiceChatError);
                                }
                            });
                        }
                        
                        myCallState = FASConference.CallStates.Calling;
                    }
                }

                GUI.Label(callLabelPosition, FresviiGUIText.Get("Call"), guiStyleButtonLabel);
            }

            if (messageButton.IsTap(e, messageButtonPosition, messageButtonPosition, FresviiGUIButton.ButtonType.TextureOnly, textureMessageButton, textureMessageButton, textureMessageButton))
            {
                if (!ControlLock)
                {
                    frameChat = ((GameObject)Instantiate(prfbFrameChat)).GetComponent<FresviiGUIFrame>();

                    frameChat.gameObject.GetComponent<FresviiGUIChat>().SetGroup(this.Group);

                    frameChat.gameObject.GetComponent<FresviiGUIChat>().IsModal = true;

                    frameChat.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, this.GuiDepth - 1);

                    frameChat.transform.parent = this.transform;

                    frameChat.SetDraw(true);

                    frameChat.PostFrame = this;

                    this.ControlLock = true;

                    frameChat.Tween(new Vector2(0.0f, Screen.height), Vector2.zero, delegate()
                    {
                        this.SetDraw(false);

                        this.ControlLock = false;
                    });
                }
            }

            GUI.Label(messageLabelPosition, FresviiGUIText.Get("Message"), guiStyleButtonLabel);

            /*Texture2D textureMicrophone = (microphoneSlider.Value == 0.0f) ? textureMicrophoneOff : textureMicrophoneOn;

            if (microphoneButton.IsTap(e, microphoneButtonPosition, microphoneButtonPosition, FresviiGUIButton.ButtonType.TextureOnly, textureMicrophone, textureMicrophone, textureMicrophone))
            {
                if (!ControlLock)
                {
                    microphoneSlider.enabled = !microphoneSlider.enabled;
                }
            }

            Texture2D textureSpeaker = (speakerSlider.Value == 0.0f) ? textureSpeakerOff : textureSpeakerOn;

            if (speakerButton.IsTap(e, speakerButtonPosition, speakerButtonPosition, FresviiGUIButton.ButtonType.TextureOnly, textureSpeaker, textureSpeaker, textureSpeaker))
            {
                if (!ControlLock)
                {
                    speakerSlider.enabled = !speakerSlider.enabled;
                }
            }*/

            Texture2D textureMicrophone = (isMute) ? textureMicrophoneOff : textureMicrophoneOn;

            if (microphoneButton.IsTap(e, microphoneButtonPosition, microphoneButtonPosition, FresviiGUIButton.ButtonType.TextureOnly, textureMicrophone, textureMicrophone, textureMicrophone))
            {
                if (!ControlLock)
                {
                    isMute = !isMute;

                    if (isMute)
                    {
                        FASConference.Mute();
                    }
                    else
                    {
                        FASConference.Unmute();
                    }
                }
            }

            GUI.DrawTextureWithTexCoords(separateLine, palette, coodsSeperateLine);
            
            GUI.EndGroup();

        }
    }
}
#endif