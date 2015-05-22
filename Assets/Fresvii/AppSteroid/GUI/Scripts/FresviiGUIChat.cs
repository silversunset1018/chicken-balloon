using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;



using Fresvii.AppSteroid.Services;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIChat : FresviiGUIFrame
    {
        public float sideMargin;

        private Texture2D palette;

        private FresviiGUIChatTopMenu chatTopMenu;

        private FresviiGUIAddCommentBottomMenu addCommentBottomMenu;

        private Rect baseRect;
        
        private Rect scrollViewRect;
        
        private float scaleFactor;
        
        private string postFix;

        public float topMargin;

        public float balloonMargin;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinnerTop;

        private Rect loadingSpinnerPosition;

        private Rect loadingSpinnerTopPosition;
        
        public Vector2 loadingSpinnerSize;
                
        public float pullResetTweenTime = 0.5f;
        
        public iTween.EaseType pullResetTweenEaseType = iTween.EaseType.easeOutExpo;

        public float pollingInterval = 3.0f;

        private IList<Fresvii.AppSteroid.Models.GroupMessage> groupMessages;

        public GameObject prfbChatBalloon;

        private List<FresviiGUIChatBalloon> chatBalloons = new List<FresviiGUIChatBalloon>();

        public Fresvii.AppSteroid.Models.User Other { get; protected set; }

        public Fresvii.AppSteroid.Models.Group Group { get; protected set; }

        [HideInInspector]
        public Texture2D balloonMatTexture;

        [HideInInspector]
        public Texture2D balloonTriangleTexture;

        [HideInInspector]
        public Texture2D videoPlaybackIconTexture;
        
        private Texture2D dateBgTexture;

        private bool initialized = false;

        public bool IsPair { get; protected set; }

        public event Action<Fresvii.AppSteroid.Models.GroupMessage> OnAddGroupMessage;

        //private uint loadedPage = 1;

        private Fresvii.AppSteroid.Models.ListMeta groupMessagesMeta;

        public float pullRefleshHeight = 50f;

        private bool loading;

        private bool loadBlock;

        public float heightDate = 20f;

        private FresviiGUIFrame frameEditGroupMember;

        private FresviiGUIFrame frameGroupMessageCreate;

        private FresviiGUIFrame frameGroupConference;

        private FresviiGUIFrame frameProfile;

        public GameObject prfbGUIFrameGroupEditMember;

        public GameObject prfbGUIFrameGroupMessageCreate;

        public GameObject prfbGUIFrameGroupCall;

        public GameObject prfbGUIFrameUserProfile;
        
        public GUIStyle guiStyleDate;

        private Texture2D chatDateBg;

        private Color chatBalloonColor;

        public bool IsModal { get; set; }

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            FASGesture.Resume();

            this.GuiDepth = guiDepth;

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            chatTopMenu = GetComponent<FresviiGUIChatTopMenu>();

            addCommentBottomMenu = GetComponent<FresviiGUIAddCommentBottomMenu>();

            chatTopMenu.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

            addCommentBottomMenu.Init(postFix, scaleFactor, GuiDepth - 1, this, AddComment);

            addCommentBottomMenu.autoSendImageLoaded = true;

            this.scaleFactor = scaleFactor;
            
            this.postFix = postFix;
			
            loadingSpinnerSize *= scaleFactor;
            
            sideMargin *= scaleFactor;
            
            topMargin *= scaleFactor;
            
            balloonMargin *= scaleFactor;

            balloonMatTexture = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ChatBalloonTextureName + this.postFix, false);

            balloonTriangleTexture = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ChatBalloonTriangleTextureName + this.postFix, false);

            videoPlaybackIconTexture = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.VideoPlaybackIconTextureName, false);

            chatDateBg = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ChatDateBgTextureName + postFix, false);

            loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, this.GuiDepth - 10);

            heightDate *= scaleFactor;

            guiStyleDate.fontSize = (int)(guiStyleDate.fontSize * scaleFactor);

            guiStyleDate.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText1);

            guiStyleDate.padding = FresviiGUIUtility.RectOffsetScale(guiStyleDate.padding, scaleFactor);

            chatBalloonColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ChatBalloon);

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleDate.font = null;
            }

            SetScrollSlider(scaleFactor * 2.0f);

            StartCoroutine(PollingGetMessages());

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });

                loadingSpinner.Hide();

                return;
            }
        }

        public void AddComment(string comment, Texture2D clipImage, Fresvii.AppSteroid.Models.Video video)
        {
            StartCoroutine(AddCommentCoroutine(comment, clipImage, video));
        }

        IEnumerator AddCommentCoroutine(string comment, Texture2D clipImage, Fresvii.AppSteroid.Models.Video video)
        {
            while (Group == null)
            {
                yield return 1;
            }

            //  Text
            if (!string.IsNullOrEmpty(comment))
            {
                FresviiGUIChatBalloon chatBalloon = ((GameObject)Instantiate(prfbChatBalloon)).GetComponent<FresviiGUIChatBalloon>();

                Fresvii.AppSteroid.Models.GroupMessage groupMessage = new Fresvii.AppSteroid.Models.GroupMessage(FAS.CurrentUser, comment, System.DateTime.Now, Fresvii.AppSteroid.Models.GroupMessage.ContentType.Text);

                chatBalloon.InitByLocal(groupMessage, scaleFactor, Screen.width, this, null);

                chatBalloons.Add(chatBalloon);

                FASGroup.SendGroupMessage(Group.Id, comment, delegate(Fresvii.AppSteroid.Models.GroupMessage _groupMessage, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error != null)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });

                        chatBalloons.Remove(chatBalloon);

                        Destroy(chatBalloon);
                    }
                    else
                    {
                        chatBalloon.GroupMessage = _groupMessage;

                        if (OnAddGroupMessage != null)
                        {
                            OnAddGroupMessage(_groupMessage);
                        }
                    }

                    Sort();
                });
            }

            //  Image
            if (clipImage != null && video == null)
            {
                FresviiGUIChatBalloon chatBalloon = ((GameObject)Instantiate(prfbChatBalloon)).GetComponent<FresviiGUIChatBalloon>();

                Fresvii.AppSteroid.Models.GroupMessage groupMessage = new Fresvii.AppSteroid.Models.GroupMessage(FAS.CurrentUser, comment, System.DateTime.Now, Fresvii.AppSteroid.Models.GroupMessage.ContentType.Image);

                chatBalloon.InitByLocal(groupMessage, scaleFactor, Screen.width, this, clipImage);

                chatBalloons.Add(chatBalloon);

				addCommentBottomMenu.ClearClipImage();

                FASGroup.SendGroupMessage(Group.Id, clipImage, delegate(Fresvii.AppSteroid.Models.GroupMessage _groupMessage, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error != null)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });

                        chatBalloons.Remove(chatBalloon);

                        Destroy(chatBalloon);

                        if (OnAddGroupMessage != null)
                        {
                            OnAddGroupMessage(_groupMessage);
                        }
                    }
                    else
                    {
                        chatBalloon.GroupMessage = _groupMessage;
                    }
                });
            }

            //  Video
            if (video != null)
            {
                FresviiGUIChatBalloon chatBalloon = ((GameObject)Instantiate(prfbChatBalloon)).GetComponent<FresviiGUIChatBalloon>();

                Fresvii.AppSteroid.Models.GroupMessage groupMessage = new Fresvii.AppSteroid.Models.GroupMessage(FAS.CurrentUser, comment, System.DateTime.Now, Fresvii.AppSteroid.Models.GroupMessage.ContentType.Video);

                chatBalloon.InitByLocal(groupMessage, scaleFactor, Screen.width, this, clipImage);

                chatBalloons.Add(chatBalloon);

                addCommentBottomMenu.ClearClipImage();

                chatBalloon.DataUploadProgressStart();
                
                FASGroup.SendGroupMessage(Group.Id, video, delegate(Fresvii.AppSteroid.Models.GroupMessage _groupMessage, Fresvii.AppSteroid.Models.Error error)
                {
                    chatBalloon.DataUploadProgressEnd();

                    if (error != null)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });

                        chatBalloons.Remove(chatBalloon);

                        Destroy(chatBalloon);

                        if (OnAddGroupMessage != null)
                        {
                            OnAddGroupMessage(_groupMessage);
                        }
                    }
                    else
                    {
                        chatBalloon.GroupMessage = _groupMessage;
                    }
                });
            }

            ScrollToLatestMessage();
        }

        IEnumerator PollingGetMessages()
        {
            if (IsPair)
            {
                while (Other == null) yield return 1;
            }

            bool groupIsReday = (Group != null);

            bool hasError = false;

            if (!groupIsReday && IsPair)
            {
                FASGroup.CreatePair(Other.Id, delegate(Fresvii.AppSteroid.Models.Group _group, Fresvii.AppSteroid.Models.Error _error)
                {
                    if (_error != null)
                    {
                        StartCoroutine(PollingGetMessages());

                        hasError = true;
                    }
                    else
                    {
                        this.Group = _group;
                    }

                    groupIsReday = true;
                });
            }

            while (!groupIsReday) yield return 1;

            if (hasError)
            {
                StartCoroutine(PollingGetMessages());

                yield break;
            }            

            while (this.gameObject.activeInHierarchy)
            {
                FASGroup.GetGroupMessageList(Group.Id, GroupService.DefaultPageNumber, OnGetGroupMessages);

                yield return new WaitForSeconds(pollingInterval);
            }
        }

		void AddGroupMessage(Fresvii.AppSteroid.Models.GroupMessage groupMessage)
		{
			bool exists = false;
			
			for (int i = 0; i < chatBalloons.Count; i++)
			{
				if (chatBalloons[i].GroupMessage.Id == groupMessage.Id)
				{
					exists = true;
					
					break;
				}
			}
			
			if (!exists)
			{
                groupMessages.Add(groupMessage);

				FresviiGUIChatBalloon chatBalloon = ((GameObject)Instantiate(prfbChatBalloon)).GetComponent<FresviiGUIChatBalloon>();
				
				chatBalloon.Init(groupMessage, scaleFactor, Screen.width, this);
				
				chatBalloons.Add(chatBalloon);				
			
				chatBalloons = chatBalloons.OrderBy(a => a.GroupMessage.CreatedAt).ToList();

                Sort();
			}

            if (chatBalloons.Count > 0)
            {
                if (Group.LastReadMessageId != chatBalloons[chatBalloons.Count - 1].GroupMessage.Id)
                {                    
                    FASGroup.MarkAsReadGroupMessage(Group.Id, chatBalloons[chatBalloons.Count - 1].GroupMessage.Id, (e) => 
                    {
                        if (e != null)
                        {
                            Debug.LogError(e.ToString());
                        }

                        FresviiGUIManager.Instance.RemoveUnreadGroupMessageGroupId(Group.Id);

                        Group.LastReadMessageId = chatBalloons[chatBalloons.Count - 1].GroupMessage.Id;
                    });
                }
            }

			ScrollToLatestMessage();
		}

        void OnGetGroupMessages(IList<Fresvii.AppSteroid.Models.GroupMessage> groupMessages, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
        {
            if (this == null) return;

            if (error == null)
            {
                this.groupMessages = groupMessages;

                if (groupMessagesMeta == null || meta.CurrentPage != GroupService.DefaultPageNumber)
                    groupMessagesMeta = meta;

                bool added = false;

                foreach (Fresvii.AppSteroid.Models.GroupMessage groupMessage in groupMessages)
                {
                    bool exists = false;

                    for (int i = 0; i < chatBalloons.Count; i++)
                    {
                        if (chatBalloons[i].GroupMessage.Id == groupMessage.Id)
                        {
                            exists = true;

                            break;
                        }
                    }

                    if (!exists)
                    {
                        FresviiGUIChatBalloon chatBalloon = ((GameObject)Instantiate(prfbChatBalloon)).GetComponent<FresviiGUIChatBalloon>();

                        chatBalloon.Init(groupMessage, scaleFactor, Screen.width, this);

                        chatBalloons.Add(chatBalloon);

                        added = true;
                    }
                }

                if (added)
                {
                    Sort();
                }

                if (!initialized)
                {
                    initialized = true;

                    loadingSpinner.Hide();

                    float scrollViewHeight = CalcScrollViewHeight();

                    if (baseRect.height < scrollViewHeight)
                    {
                        scrollViewRect.y = baseRect.height - scrollViewHeight;
                    }
                }
                else if (added)
                {
                    ScrollToLatestMessage();
                }

                if (chatBalloons.Count > 0)
                {
                    if (Group.LastReadMessageId != chatBalloons[chatBalloons.Count - 1].GroupMessage.Id)
                    {
                        if (string.IsNullOrEmpty(Group.Id) || string.IsNullOrEmpty(chatBalloons[chatBalloons.Count - 1].GroupMessage.Id))
                        {

                        }
                        else
                        {
                            FASGroup.MarkAsReadGroupMessage(Group.Id, chatBalloons[chatBalloons.Count - 1].GroupMessage.Id, (e) =>
                            {
                                if (e != null)
                                {
                                    Debug.LogError(e.ToString());
                                }
                            });
                        }

                        FresviiGUIManager.Instance.RemoveUnreadGroupMessageGroupId(Group.Id);

                        Group.LastReadMessageId = chatBalloons[chatBalloons.Count - 1].GroupMessage.Id;
                    }
                }
            }
            else
            {                
                if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NotFound || (Application.platform == RuntimePlatform.Android && error.Detail.IndexOf("FileNotFound") >= 0))
                {
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                    {
                        Debug.LogError(error.ToString());
                    }

                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("GroupNone"), delegate(bool del) { });

                    FresviiGUIGroupMessage groupMessage = PostFrame.gameObject.GetComponent<FresviiGUIGroupMessage>();
                    
                    if (groupMessage != null)
                    {
                        groupMessage.RemoveDeletedGroup(this.Group);
                    }

                    BackToPostFrame();               
                }
            }
        }

        void OnGetGroupMessagesWithoutScroll(IList<Fresvii.AppSteroid.Models.GroupMessage> groupMessages, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
        {
            if (this == null) return;

            if (loadingSpinnerTop != null)
                loadingSpinnerTop.Hide();

            if (error == null)
            {
                this.groupMessages = groupMessages;

                if (groupMessagesMeta == null || meta.CurrentPage != GroupService.DefaultPageNumber)
                    groupMessagesMeta = meta;

                foreach (Fresvii.AppSteroid.Models.GroupMessage groupMessage in groupMessages)
                {
                    bool exists = false;

                    for (int i = 0; i < chatBalloons.Count; i++)
                    {
                        if (chatBalloons[i].GroupMessage.Id == groupMessage.Id)
                        {
                            exists = true;

                            break;
                        }
                    }

                    if (!exists)
                    {
                        FresviiGUIChatBalloon chatBalloon = ((GameObject)Instantiate(prfbChatBalloon)).GetComponent<FresviiGUIChatBalloon>();

                        chatBalloon.Init(groupMessage, scaleFactor, Screen.width, this);

                        scrollViewRect.y -= chatBalloon.GetHeight() + balloonMargin;

                        chatBalloons.Add(chatBalloon);

                        Sort();
					}
                }
            }
            else
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                    Debug.LogError(error.ToString());
            }

            loading = false;

            pullRefleshing = false;

            if (scrollViewRect.y < - baseRect.y)
            {
                OnCompletePullReflesh(scrollViewRect, baseRect);
            }
        }

       	void OnEnable()
		{
			FASEvent.OnGroupMessageTextCreated += OnGroupMessageCreated;
			
			FASEvent.OnGroupMessageImageCreated += OnGroupMessageCreated;
		}
		
		void OnDisable()
		{
			FASEvent.OnGroupMessageTextCreated -= OnGroupMessageCreated;
			
			FASEvent.OnGroupMessageImageCreated -= OnGroupMessageCreated;
		}

#if GROUP_CONFERENCE
        public void OnCallButtonTapped()
        {
            if (Group == null) return;

            if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
                return;

            if(Group.MembersCount > 4)
            {
                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("GroupConferenceMemberLimitation"), delegate(bool del)
                {

				});

				return;
            }

            //DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("ConfirmToCall"), FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), (del)=>
            //DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("ConfirmToCall"), delegate(bool del)
            {
                if (del)
                {
                    ControlLock = true;

                    frameGroupConference = ((GameObject)Instantiate(prfbGUIFrameGroupCall)).GetComponent<FresviiGUIGroupConference>();

                    frameGroupConference.gameObject.GetComponent<FresviiGUIGroupConference>().SetGroup(this.Group);

                    frameGroupConference.transform.parent = this.transform;

                    frameGroupConference.PostFrame = this;

                    frameGroupConference.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, this.GuiDepth - 10);

                    frameGroupConference.SetDraw(true);

                    frameGroupConference.Tween(new Vector2(0.0f, Screen.height), Vector2.zero, delegate()
                    {
                        this.SetDraw(false);

                        ControlLock = false;

                        frameGroupConference.ControlLock = false;
                    });
                }
            });
        }
#endif
        public GameObject prfbGroupMemeberAdd;

        private FresviiGUIGroupMemberAdd frameGroupMemberAdd;

        public void OnAddMemberTapped()
        {
            if (Group == null) return;

            if (Group.Members == null)
            {
                Group.FetchMembers(delegate(Fresvii.AppSteroid.Models.Error error)
                {

                });

                return;
            }

            frameGroupMemberAdd = ((GameObject)Instantiate(prfbGroupMemeberAdd)).GetComponent<FresviiGUIGroupMemberAdd>();

            frameGroupMemberAdd.transform.parent = this.transform;

            frameGroupMemberAdd.PostFrame = this;

            //frameGroupMemberAdd.frameEditGroupMember = this;

            frameGroupMemberAdd.Group = this.Group;

            frameGroupMemberAdd.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, this.GuiDepth - 10);

            frameGroupMemberAdd.SetDraw(true);

            ControlLock = true;

            frameGroupMemberAdd.Tween(new Vector2(0.0f, Screen.height), Vector2.zero, delegate()
            {
                this.SetDraw(false);

                ControlLock = false;
            });
        }

        public void OnEditMemberTapped()
        {
            if (Group == null) return;

            ControlLock = true;
            
            frameEditGroupMember = ((GameObject)Instantiate(prfbGUIFrameGroupEditMember)).GetComponent<FresviiGUIEditGroupMember>();

            frameEditGroupMember.transform.parent = this.transform;

            frameEditGroupMember.PostFrame = this;

            frameEditGroupMember.GetComponent<FresviiGUIEditGroupMember>().Group = this.Group;

            frameEditGroupMember.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, this.GuiDepth - 10);

            frameEditGroupMember.SetDraw(true);

            frameEditGroupMember.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate()
            {
                this.SetDraw(false);

                ControlLock = false;
            });                
           
        }

        public void OnProfileTapped()
        {
            if (Group.Pair)
            {
                frameProfile = ((GameObject)Instantiate(prfbGUIFrameUserProfile)).GetComponent<FresviiGUIFrame>();
                
                if (Other != null)
                {
                    frameProfile.gameObject.GetComponent<FresviiGUIUserProfile>().SetUser(Other);
                }
                else
                {
                    foreach (Fresvii.AppSteroid.Models.Member member in Group.Members)
                    {
                        if (member.Id != FAS.CurrentUser.Id)
                        {
                            Other = member.ToUser();
                        }
                    }

                    frameProfile.gameObject.GetComponent<FresviiGUIUserProfile>().SetUser(Other);
                }

                Debug.Log(Other.Id + ", " + Other.ProfileImageUrl);

                frameProfile.Init(null, postFix, scaleFactor, this.GuiDepth - 10);

                frameProfile.transform.parent = this.transform;

                frameProfile.SetDraw(true);

                frameProfile.PostFrame = this;

                this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
                {
                    this.SetDraw(false);
                });

                frameProfile.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
            }
        }

		void OnGroupMessageCreated(Fresvii.AppSteroid.Models.GroupMessage groupMessage)
		{
			if(this.Group != null)
			{
				if(this.Group.Id == groupMessage.GroupId)
				{
					AddGroupMessage(groupMessage);
				}
			}
		}

		public void SetPairUser(Fresvii.AppSteroid.Models.User other)
        {
            IsPair = true;

            this.Other = other;
        }

        public void SetGroup(Fresvii.AppSteroid.Models.Group group)
        {
            IsPair = false;

            this.Group = group;
        }

        void CalcLayout()
        {
            //this.baseRect = new Rect(Position.x, Position.y + chatTopMenu.height, Screen.width, Screen.height - chatTopMenu.height - addCommentBottomMenu.height - FresviiGUIFrame.OffsetPosition.y);

            this.baseRect = new Rect(Position.x, Position.y, Screen.width, Screen.height - addCommentBottomMenu.height - FresviiGUIFrame.OffsetPosition.y);
        }

        float CalcScrollViewHeight()
        {
            float height = chatTopMenu.height + topMargin;

            System.DateTime dt = System.DateTime.MinValue;

            foreach (FresviiGUIChatBalloon balloon in chatBalloons)
            {
				balloon.dateDraw = false;

                if (dt.Year != balloon.GroupMessage.CreatedAt.Year || dt.Month != balloon.GroupMessage.CreatedAt.Month || dt.Day != balloon.GroupMessage.CreatedAt.Day)
                {
					balloon.dateDraw = true;

                    dt = balloon.GroupMessage.CreatedAt;

					height += heightDate + balloonMargin;
                }

				Rect balloonPosition = new Rect(0f, height, baseRect.width, balloon.GetHeight());
				
				Rect drawPosition = new Rect(balloonPosition.x, scrollViewRect.y + balloonPosition.y, balloonPosition.width, balloonPosition.height);
				
				balloon.isDraw = (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height);

                height += balloon.GetHeight() + balloonMargin;                
            }

            return height;
        }

        private void ScrollToLatestMessage()
        {
            float scrollViewHeight = CalcScrollViewHeight();

            if (baseRect.height < scrollViewHeight)
            {
                iTween.ValueTo(this.gameObject, iTween.Hash("from", scrollViewRect.y, "to", baseRect.height - scrollViewHeight, "time", pullResetTweenTime, "easetype", pullResetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition"));
            }
        }

        uint preMembersCount = 0;

		void Update(){

            if (Group != null)
            {
                if (preMembersCount != Group.MembersCount)
                {
                    addCommentBottomMenu.SetSendEnableAtAction(this.Group.MembersCount > 1, FresviiGUIText.Get("LonelyGroupError"));

                    chatTopMenu.SetMemberNames();

                    preMembersCount = Group.MembersCount;
                }
            }

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            if (loadingSpinner != null)
            {
                loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);             
            }

            if (loadingSpinnerTop != null)
            {
                loadingSpinnerTop.Position = new Rect(baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + chatTopMenu.height + pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }

            CalcLayout();            

            float scrollViewHeight = CalcScrollViewHeight();

            scrollViewRect.x = Position.x;

            scrollViewRect.width = Screen.width;

            scrollViewRect.height = scrollViewHeight;

            InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, pullRefleshHeight, chatTopMenu.height, 0f, OnPullDownReflesh, null);

            if (loadBlock && FASGesture.IsTouchEnd)
            {
                loadBlock = false;
            }

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && !ControlLock)
            {
                BackToPostFrame();
            }
#endif
		}

        void OnPullDownReflesh()
        {
            if (loading || loadBlock) return;

            if (groupMessagesMeta != null)
            {
                uint? loadPage = groupMessagesMeta.NextPage;

                if (loadPage.HasValue)
                {
                    loading = true;

                    loadBlock = true;

                    loadingSpinnerTopPosition = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + baseRect.y + chatTopMenu.height + pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

                    loadingSpinnerTop = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerTopPosition, this.GuiDepth - 1);

                    pullRefleshing = true;

                    FASGroup.GetGroupMessageList(Group.Id, (uint)loadPage, OnGetGroupMessagesWithoutScroll);
                }
            }
        }

        public void OnUpdateScrollViewPosition(float value)
        {
            scrollViewRect.y = value;
        }

        void OnDestroy()
        {            
            if (loadingSpinner != null)
                loadingSpinner.Hide();

            if (loadingSpinnerTop != null)
                loadingSpinnerTop.Hide();
        }

        public void BackToPostFrame()
        {
            if (PostFrame == null)
            {
                return;
            }

            PostFrame.SetDraw(true);

            if (IsModal)
            {
                this.Tween(Vector2.zero, new Vector2(0.0f, Screen.height), delegate()
                {
                    Destroy(this.gameObject);
                });

                PostFrame.Position= Vector2.zero;
            }
            else
            {
                this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
                {
                    Destroy(this.gameObject);
                });

                PostFrame.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
            }
        }

        public void Sort()
        {
            chatBalloons.Sort((a, b) => System.DateTime.Compare(a.GroupMessage.CreatedAt, b.GroupMessage.CreatedAt));
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            addCommentBottomMenu.enabled = on;

            chatTopMenu.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                {
                    loadingSpinner.Hide();
                }
            }
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(scrollViewRect);

            float cardY = chatTopMenu.height + topMargin;
        
            foreach (FresviiGUIChatBalloon balloon in chatBalloons)
            {
				if (balloon.dateDraw)
                {        
                    string dtString = balloon.GroupMessage.CreatedAt.ToString("M/d");

                    GUIContent guiContentDate = new GUIContent(dtString);

                    Vector2 dtStringSize = guiStyleDate.CalcSize(guiContentDate);

                    Rect datePosition = new Rect(Screen.width * 0.5f - dtStringSize.x * 0.5f, cardY + 0.5f * (heightDate - chatDateBg.height), dtStringSize.x, chatDateBg.height);

                    Rect drawDatePosition = new Rect(datePosition.x, scrollViewRect.y + datePosition.y, datePosition.width, datePosition.height);

                    if (drawDatePosition.y + drawDatePosition.height > 0 && drawDatePosition.y < Screen.height)
                    {
                        Color tmp = GUI.color;

                        GUI.color = chatBalloonColor;

                        FresviiGUIUtility.DrawSplitTexture(datePosition, chatDateBg, 1.0f * scaleFactor);

                        GUI.color = tmp;

                        GUI.Label(datePosition, dtString, guiStyleDate);
                    }

					cardY += heightDate + balloonMargin;
                }

				Rect balloonPosition = new Rect(0f, cardY, baseRect.width, balloon.GetHeight());

				if (balloon.isDraw)
                {
                    balloon.Draw(balloonPosition);
                }

                cardY += balloonPosition.height + balloonMargin;
            }

            GUI.EndGroup();

            GUI.EndGroup();

        }       
    }
}
