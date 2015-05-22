using UnityEngine;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIGroupMessage : FresviiGUIFrame
    {
        public float sideMargin;

        private Texture2D palette;

        private FresviiGUIGroupMessageTop groupMessageTopMenu;

        private Rect baseRect;
        
        private Rect scrollViewRect;
        
        private float scaleFactor;

        private string postFix = "";
        
        public float topMargin;

        public float cardMargin;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;
        
        public Vector2 loadingSpinnerSize;
        
        private List<FresviiGUIGroupCard> groupCards = new List<FresviiGUIGroupCard>();

        public GameObject prfbGroupCard;

        private bool initialized;

		public float pollingInterval = 15f;

        public GameObject prfbGUIFrameGroupMessageCreate;

        private FresviiGUIFrame frameGroupMessageCreate;

        public float pullRefleshHeight = 50f;

        private bool loading;

        private bool loadBlock;

        private enum LoadingSpinnerPlace { Top, Center, Bottom };

        private LoadingSpinnerPlace loadingSpinnerPlace = LoadingSpinnerPlace.Center;

        private Fresvii.AppSteroid.Models.ListMeta groupListMeta = null;

        public static Fresvii.AppSteroid.Models.GroupMessage initialGroupMessage;

        private bool isPullUp;

        public FresviiGUITabBar tabBar;

        public bool HasActionSheet = false;

        public static Texture2D IconUnread { get; protected set; }


        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = FASGui.GuiDepthBase;

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            groupMessageTopMenu = GetComponent<FresviiGUIGroupMessageTop>();

            groupMessageTopMenu.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

            tabBar = GetComponent<FresviiGUITabBar>();

            tabBar.Init(postFix, scaleFactor, this.GuiDepth - 1);

            this.scaleFactor = scaleFactor;

            this.postFix = postFix;
            
            loadingSpinnerSize *= scaleFactor;
            
            sideMargin *= scaleFactor;
            
            topMargin = scaleFactor;

            loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GuiDepth - 10);

            pullRefleshHeight *= scaleFactor;

            FASUser.GetAccount(delegate(Fresvii.AppSteroid.Models.User _user, Fresvii.AppSteroid.Models.Error _error) { });

            SetScrollSlider(scaleFactor * 2.0f);

            FASGroup.GetGroupMessageGroupListFromCache(OnGetGroups);

            IconUnread = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconUnread + postFix, false);
		}

        void Awake()
        {
            FASEvent.OnGroupMessageTextCreated += OnGroupMessageCreated;

            FASEvent.OnGroupMessageImageCreated += OnGroupMessageCreated;

            FresviiGUIManager.Instance.OnGetGroupMessageGroups += OnGetGroups;
        }

        void OnDestroy()
        {
            FASEvent.OnGroupMessageTextCreated -= OnGroupMessageCreated;

            FASEvent.OnGroupMessageImageCreated -= OnGroupMessageCreated;

            FresviiGUIManager.Instance.OnGetGroupMessageGroups -= OnGetGroups;

            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }
        }

        public void OnGroupMessageCreated(Fresvii.AppSteroid.Models.GroupMessage gm)
        {
            FASGroup.GetGroupMessageGroupList(OnGetGroups);
        }

		IEnumerator Polling()
		{
			while(this.gameObject.activeInHierarchy)
			{
                FASGroup.GetGroupMessageGroupList(OnGetGroups);

				yield return new WaitForSeconds(pollingInterval);
			}
		}

        void OnGetGroups(IList<Fresvii.AppSteroid.Models.Group> groups, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
        {
			if(loadingSpinner != null && !initialized)
				loadingSpinner.Hide();

            if(loadingSpinner != null && loading)
                loadingSpinner.Hide();

            float addedHeight = 0.0f;

            if (error == null)
            {
                if (groupListMeta == null)
                {
                    groupListMeta = meta;
                }
                else if (groupListMeta.CurrentPage != 1)
                {
                    groupListMeta = meta;
                }

                foreach (Fresvii.AppSteroid.Models.Group group in groups)
                {
                   addedHeight += AddGroup(group);
                }

                if (!initialized)
                {
                    initialized = true;

                    loadingSpinner.Hide();
                }              
            }
            else
            {
                if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.CacheNotExists && FASConfig.Instance.logLevel <= FAS.LogLevels.Warning)
                {
                    Debug.LogWarning(error.Detail);
                }
                else if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                {
                    Debug.LogError(error.ToString());
                }         
            }

            loading = false;

            if (pullRefleshing)
            {
                if (isPullUp && addedHeight > 0.0f)
                {
                    
                }
                else
                {
                    OnCompletePullReflesh(scrollViewRect, baseRect);
                }
            }

            pullRefleshing = false;

            isPullUp = false;

			Sort();

            if (initialGroupMessage != null)
            {
                foreach (FresviiGUIGroupCard card in groupCards)
                {
                    if (initialGroupMessage.GroupId == card.Group.Id)
                    {
                        this.SetDraw(false);

                        this.Position = new Vector2(-Screen.width, 0.0f);

                        card.GoToGroupChat(false);

                        break;
                    }
                }
            }

            initialGroupMessage = null;

            uint unreadMessageCount = 0;

            foreach (FresviiGUIGroupCard card in groupCards)
            {
                if (card.Group.LatestMessage != null)
                {
                    if (card.Group.LatestMessage.Id != card.Group.LastReadMessageId && card.Group.LatestMessage.User.Id != FAS.CurrentUser.Id)
                    {
                        FresviiGUIManager.Instance.AddUnreadGroupMessageGroupId(card.Group.Id);

                        unreadMessageCount++;
                    }
                }
            }

            if (unreadMessageCount == 0)
            {
                FresviiGUIManager.Instance.ClearUnreadGroupMessageGroupId();
            }
        }

        private float AddGroup(Fresvii.AppSteroid.Models.Group group)
        {
            if (this == null) return 0.0f;

            bool exists = false;

            for (int i = 0; i < groupCards.Count; i++)
            {
                if (groupCards[i].Group.Id == group.Id)
                {
                    exists = true;

                    bool updated = (groupCards[i].Group.UpdatedAt != group.UpdatedAt);

					groupCards[i].Group = group;

                    if (updated)
                        groupCards[i].UpdateGroupCard();

					groupCards[i].SetUserIcons();

                    return 0.0f;
                }
            }

            if (!exists)
            {
                FresviiGUIGroupCard groupCard = ((GameObject)Instantiate(prfbGroupCard)).GetComponent<FresviiGUIGroupCard>();

                groupCard.transform.parent = this.transform;

                groupCard.Init(group, scaleFactor, this);

                groupCards.Add(groupCard);

                return groupCard.GetHeight();
            }

            return 0.0f;
        }

        public void GoToGroupChat(Fresvii.AppSteroid.Models.Group group)
        {
            AddGroup(group);

            for (int i = 0; i < groupCards.Count; i++)
            {
                if (groupCards[i].Group.Id == group.Id)
                {
                    groupCards[i].GoToGroupChat(true);

                    break;
                }
            }
        }

        public void DeleteGroupCard(FresviiGUIGroupCard card)
        {
            if (card.Group.MembersCount <= 1)
            {
                FASGroup.DeleteGroup(card.Group.Id, delegate(Fresvii.AppSteroid.Models.Error error)
                {
                    if (error != null)
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        {
                            Debug.LogError("GroupService.DeleteGroup : " + error.ToString());
                        }
                    }
                });
            }
            else
            {
                FASGroup.DeleteMember(card.Group.Id, FAS.CurrentUser.Id, delegate(Fresvii.AppSteroid.Models.Error error)
                {
                    if (error != null)
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        {
                            Debug.LogError("GroupService.DeleteMember : " + error.ToString());
                        }
                    }
                });
            }

            groupCards.Remove(card);

            Destroy(card);
        }

        public void RemoveDeletedGroup(Fresvii.AppSteroid.Models.Group group)
        {
            foreach (FresviiGUIGroupCard card in groupCards)
            {
                if (card.Group.Id == group.Id)
                {
                    groupCards.Remove(card);

                    Destroy(card.gameObject);

                    break;
                }
            }
        }

        public void Sort()
        {
			groupCards.Sort((a, b) => System.DateTime.Compare(b.LatestMessageCreatedAt(), a.LatestMessageCreatedAt()));
        }

		void OnEnable()
		{
			if(groupCards != null)
                groupCards.Sort((a, b) => System.DateTime.Compare(b.LatestMessageUpdatedAt(), a.LatestMessageUpdatedAt()));

			//StartCoroutine(Polling());

            FASGroup.GetGroupMessageGroupList(OnGetGroups);

            ControlLock = false;
		}

        public bool CardIsOpen { get; protected set; }

        public void OnCardOpenStateChanged(bool open)
        {
            CardIsOpen = open;
        }

        void CalcLayout()
        {
            if(tabBar.enabled)
                this.baseRect = new Rect(Position.x, Position.y + groupMessageTopMenu.height, Screen.width, Screen.height - groupMessageTopMenu.height - tabBar.height - FresviiGUIFrame.OffsetPosition.y);
            else
                this.baseRect = new Rect(Position.x, Position.y + groupMessageTopMenu.height, Screen.width, Screen.height - groupMessageTopMenu.height - FresviiGUIFrame.OffsetPosition.y);
        }

        float CalcScrollViewHeight()
        {
            float height = topMargin;

            foreach (FresviiGUIGroupCard groupCard in groupCards)
            {
                height += groupCard.GetHeight() + cardMargin;
            }

            return height;
        }

        void Update(){

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            if (loadingSpinner != null)
            {
                if (loadingSpinnerPlace == LoadingSpinnerPlace.Center)
                {
                    loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
                else if (loadingSpinnerPlace == LoadingSpinnerPlace.Top)
                {
                    loadingSpinner.Position = new Rect(baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
                else if (loadingSpinnerPlace == LoadingSpinnerPlace.Bottom)
                {
                    loadingSpinner.Position = new Rect(baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + baseRect.height - pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
            }

            CalcLayout();

            float scrollViewHeight = CalcScrollViewHeight();

            scrollViewRect.x = Position.x;

            scrollViewRect.width = Screen.width;

            scrollViewRect.height = scrollViewHeight;

            InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, pullRefleshHeight, OnPullDownReflesh, OnPullUpReflesh);

            if (loadBlock && FASGesture.IsTouchEnd)
            {
                loadBlock = false;
            }

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && PostFrame != null && !ControlLock)
            {
                BackToPostFrame();
            }
#endif
		}

        void LateUpdate()
        {
            if (initialGroupMessage != null)
            {
                foreach (FresviiGUIGroupCard card in groupCards)
                {
                    if (initialGroupMessage.GroupId == card.Group.Id)
                    {
                        this.SetDraw(false);

                        this.Position = new Vector2(-Screen.width, 0.0f);

                        card.GoToGroupChat(false);

                        initialGroupMessage = null;

                        break;
                    }
                }
            }
        }

        void OnPullDownReflesh()
        {
            if (loading || loadBlock) return;

            loading = true;

            loadBlock = true;

            loadingSpinnerPlace = LoadingSpinnerPlace.Top;

            loadingSpinner.Position = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + baseRect.y + pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinner.Position, this.GuiDepth - 1);

            pullRefleshing = true;

            FASGroup.GetGroupMessageGroupList(OnGetGroups);
        }

        void OnPullUpReflesh()
        {
            uint loadPage = 2;

            if (groupListMeta != null)
            {
                if (groupListMeta.NextPage.HasValue)
                {
                    loadPage = (uint)groupListMeta.NextPage;
                }
                else
                {
                    loadPage = groupListMeta.TotalCount / groupListMeta.PerPage + 1;
                }
            }

            if (loading || loadBlock) return;

            loading = true;

            loadBlock = true;

            loadingSpinnerPlace = LoadingSpinnerPlace.Bottom;

            loadingSpinner.Position = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + baseRect.y + baseRect.height - pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinner.Position, this.GuiDepth - 1);

            pullRefleshing = true;

            isPullUp = true;

            FASGroup.GetGroupMessageGroupList(loadPage, OnGetGroups);
        }

        public void OnUpdateScrollViewPosition(float value)
        {
            scrollViewRect.y = value;
        }

        
        void OnCompletePull()
        {

        }

        public void BackToPostFrame()
        {            
            PostFrame.SetDraw(true);            

            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
            {
                Destroy(this.gameObject);
            });

            PostFrame.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
        }

        public void OnCreateButtonTapped()
        {
            ControlLock = true;

            frameGroupMessageCreate = ((GameObject)Instantiate(prfbGUIFrameGroupMessageCreate)).GetComponent<FresviiGUIGroupMessageCreate>();

            frameGroupMessageCreate.transform.parent = this.transform;

            frameGroupMessageCreate.PostFrame = this;

            frameGroupMessageCreate.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, this.GuiDepth - 10);

            frameGroupMessageCreate.SetDraw(true);

            frameGroupMessageCreate.Tween(new Vector2(0.0f, Screen.height), Vector2.zero, delegate()
            {
                this.SetDraw(false);

                ControlLock = false;
            });
        }

        public void DestroySubFrames()
        {
            if (frameGroupMessageCreate != null)
            {
                Destroy(frameGroupMessageCreate.gameObject);
            }

            foreach (FresviiGUIGroupCard groupCard in groupCards)
            {
                if (groupCard.frameChat != null)
                {
                    Destroy(groupCard.frameChat.gameObject);
                }
            }
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            tabBar.enabled = on;

            groupMessageTopMenu.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                    loadingSpinner.Hide();
            }
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(scrollViewRect);

            //  Friend cards
            float cardY = topMargin;
           
            foreach (FresviiGUIGroupCard groupCard in groupCards)
            {
                Rect groupCardPosition = new Rect(0f, cardY, baseRect.width, groupCard.GetHeight());

                Rect drawPosition = new Rect(groupCardPosition.x, scrollViewRect.y + groupCardPosition.y, groupCardPosition.width, groupCardPosition.height);

                if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                {
                    groupCard.enabled = true;

                    groupCard.Draw(groupCardPosition);
                }
                else
                {
                    groupCard.enabled = false;
                }

                cardY += groupCardPosition.height + cardMargin;                
            }

            GUI.EndGroup();

            GUI.EndGroup();
        }

        private Fresvii.AppSteroid.Gui.ActionSheet actionSheet;

        void OnDisable()
        {
            if (actionSheet != null)
            {
                actionSheet.Hide();
            }
        }

        public void ShowActionSheet(FresviiGUIGroupCard card)
        {
            List<string> buttons = new List<string>();

            buttons.Add(FresviiGUIText.Get("Delete"));

            ControlLock = true;

            actionSheet = Fresvii.AppSteroid.Gui.ActionSheet.Show(this.scaleFactor, this.postFix, GuiDepth - 10, buttons.ToArray(), (selectedButton) =>
            {
                ControlLock = false;

                if (selectedButton == FresviiGUIText.Get("Delete"))
                {
                    if (card.Group.Pair)
                    {
#if !UNITY_EDITOR
						Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

						Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("NotArrowDeletePair"), delegate(bool del) { });
#endif
                        return;
                    }
                    else
                    {
#if !UNITY_EDITOR
						Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Delete"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

						Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("ConfirmDeleteGroup"), delegate(bool del)
                        {
                            if (del)
                            {
#endif
                                card.OnCardGone();
#if !UNITY_EDITOR
                            }
                        });
#endif
                    }
                }
            });
        }        
    }
}
	