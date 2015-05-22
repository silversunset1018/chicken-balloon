using UnityEngine;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIForum : FresviiGUIFrame
    {
        public static FresviiGUIForum Instance { get; protected set; }

        private float sideMargin = 8;

        public float verticalMargin = 10;

        public bool Initialized { get; protected set; }
        
        public FresviiGUIForumTopMenu forumTopMenu;
        private FresviiGUITabBar tabBar;

		public Rect baseRect;
        private Rect scrollViewRect;
        private float scaleFactor;
        private string postFix;

        private IList<Fresvii.AppSteroid.Models.Thread> threads = new List<Fresvii.AppSteroid.Models.Thread>();
        private List<string> threadIds = new List<string>();
		private List<FresviiGUIThreadCard> cards = new List<FresviiGUIThreadCard>();

		private bool hasError;
        private bool isReady;

        public static Texture2D TexForumCommentS { get; protected set; }
        public static Texture2D TexForumCommentSH { get; protected set; }
        public static Texture2D TexForumLikeS { get; protected set; }
        public static Texture2D TexForumLikeSH { get; protected set; }
        public static Texture2D TexForumCommentL { get; protected set; }
        public static Texture2D TexForumCommentLH { get; protected set; }
        public static Texture2D TexForumLikeL { get; protected set; }
        public static Texture2D TexForumLikeLH { get; protected set; }
        public static Texture2D TexForumLikeM { get; protected set; }
        public static Texture2D TexForumLikeMH { get; protected set; }
		public static Texture2D TexForumMenu { get; protected set;}
		public static Texture2D TexForumMenuH { get; protected set;}
		public static Texture2D TexVideoIcon { get; protected set; }
        public static Texture2D TexVideoPlaybackIcon { get; protected set; }


		public float reloadHeight = 50f;

		public float pollingInterval = 15f;

        public Vector2 loadingSpinnerSize;

		private enum LoadingSpinnerPlace { Top, Center, Bottom };
		private LoadingSpinnerPlace loadingSpinnerPlace = LoadingSpinnerPlace.Center;

        public GameObject prfbGUIFrameCreateThread;

        private FresviiGUIFrame frameCreateThread;

        private FresviiGUIScrollviewSlider slider;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = guiDepth;

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

			loadingSpinnerPlace = LoadingSpinnerPlace.Center;

			loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
			
			loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, FASGui.GuiDepthBase);
					
			forumTopMenu = GetComponent<FresviiGUIForumTopMenu>();

            tabBar = GetComponent<FresviiGUITabBar>();

            forumTopMenu.Init(appIcon, postFix, scaleFactor, FresviiGUIText.Get("Forum"));

            tabBar.Init(postFix, scaleFactor, this.GuiDepth - 1);

			this.scaleFactor = scaleFactor;
            this.verticalMargin = verticalMargin * scaleFactor;
			this.reloadHeight *= scaleFactor;
            this.sideMargin *= scaleFactor;
            this.loadingSpinnerSize *= scaleFactor;
            this.postFix = postFix;
			
            TexForumCommentS = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumCommentSTextureName + postFix, false);
            TexForumCommentSH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumCommentSHTextureName + postFix, false);

            TexForumLikeS = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumLikeSTextureName + postFix, false);
            TexForumLikeSH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumLikeSHTextureName + postFix, false);
            
            TexForumCommentL = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumCommentLTextureName + postFix, false);
            TexForumCommentLH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumCommentLHTextureName + postFix, false);
            
            TexForumLikeL = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumLikeLTextureName + postFix, false);
            TexForumLikeLH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumLikeLHTextureName + postFix, false);
            
            TexForumLikeM = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumLikeMTextureName + postFix, false);
            TexForumLikeMH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumLikeMHTextureName + postFix, false);

            TexForumMenu = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumMenuTextureName + postFix, false);

            TexForumMenuH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumMenuHTextureName + postFix, false);

            TexVideoPlaybackIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.VideoPlaybackIconTextureName, false);

			TexVideoIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.VideoIconTextureName, false);

            FASForum.GetForumThreadsFromCache(OnGetForumThreads);

            GetForumThreads();

            Initialized = true;

            frameCreateThread = ((GameObject)Instantiate(prfbGUIFrameCreateThread)).GetComponent<FresviiGUIFrame>();

            frameCreateThread.transform.parent = this.transform;

            frameCreateThread.Init(appIcon, postFix, scaleFactor, this.GuiDepth - 10);

            frameCreateThread.PostFrame = this;

            frameCreateThread.SetDraw(false);

            tabBar.GuiDepth = this.GuiDepth - 1;

            SetScrollSlider(scaleFactor * 2.0f);
		}

        void GetForumThreads()
        {
            //  Offline from cache
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                FASForum.GetForumThreadsFromCache(OnGetForumThreads);

                return;
            }
            //  Online from server
            else
            {
                FASForum.GetForumThreads(OnGetForumThreads);
            }
        }

        void OnEnable()
        {
            StartCoroutine(ReloadPolling());
        }

        void OnDisable()
        {
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

            tabBar.enabled = on;

            forumTopMenu.enabled = on;

            if (loadingSpinner != null)
            {
                if (!on)
                {
                    loadingSpinner.Hide();
                }
            }
        }

        public void GoToCreateThread()
        {
            tabBar.enabled = false;

            frameCreateThread.SetDraw(true);

            this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
            {
                this.SetDraw(false);
            });

            frameCreateThread.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
        }

        public void GoToThread(FresviiGUIThread frameThread, bool animation)
        {
            tabBar.enabled = false;

            frameThread.SetDraw(true);

            if (animation)
            {
                this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
                {
                    this.SetDraw(false);
                });

                frameThread.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
            }
            else
            {
                this.SetDraw(false);

                this.Position = new Vector2(-Screen.width, 0.0f);

                frameThread.Position = Vector2.zero;
            }
        }

        public void GoToUserProfile(FresviiGUIFrame frameProfile)
        {
            tabBar.enabled = false;

            frameProfile.SetDraw(true);

            frameProfile.PostFrame = this;

            this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
            {
                this.SetDraw(false);
            });

            frameProfile.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
        }
		
		private IEnumerator ReloadPolling(){

            yield return new WaitForSeconds(pollingInterval);
            
            while (this.enabled)
            {				
				LoadLatestThreads();

                GetDeletedResources();

                yield return new WaitForSeconds(pollingInterval);
			}			
		}

        private void GetDeletedResources()
        {
            string dateString = PlayerPrefs.GetString("LatestCheckDateOfGetDeletedThreads", "");

            System.DateTime dateOffset = (string.IsNullOrEmpty(dateString)) ? System.DateTime.Now.AddDays(-30) : System.DateTime.Parse(dateString);

            FASUtility.GetDeletedResources(1, Fresvii.AppSteroid.Models.DeletedResource.ResourceForumThread, dateOffset, OnGetDeletedResources);

            PlayerPrefs.SetString("LatestCheckDateOfGetDeletedThreads", System.DateTime.Now.ToString());
        }

        void OnGetDeletedResources(IList<Fresvii.AppSteroid.Models.DeletedResource> deletedResources, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
        {
            if (this == null)
            {
                return;
            }

            if (!this.enabled)
            {
                return;
            }

            if (error == null)
            {
                foreach (Fresvii.AppSteroid.Models.DeletedResource deletedResource in deletedResources)
                {
                    int index = -1;

                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (cards[i].Thread.Id == deletedResource.Id)
                        {
                            index = i;

                            break;
                        }
                    }

                    if (index >= 0)
                    {
                        FresviiGUIThreadCard card = cards[index];

                        cards.RemoveAt(index);

                        Destroy(card.gameObject);
                    }
                }
            }           
        }

        private void OnGetForumThreads(IList<Fresvii.AppSteroid.Models.Thread> threads, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
        {
            if (this == null)
            {
                return;
            }

            if (this.enabled == false)
            {
                return;
            }

			loadingSpinner.Hide();

            if (error != null)
            {
                if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.CacheNotExists && FASConfig.Instance.logLevel <= FAS.LogLevels.Warning)
                {
                    Debug.LogWarning(error.Detail);
                }
                else if(FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                {
                        Debug.LogError(error.ToString());
                }                

                return;
            }
            
            foreach (Fresvii.AppSteroid.Models.Thread thread in threads)
            {
                UpdateThread(thread);
            }

            SortCards();

            scrollViewRect.y = 0.0f;
        }

        public void DestroySubFrames()
        {
            foreach (FresviiGUIThreadCard card in cards)
            {
                if (card.frameThread != null)
                {
                    card.frameThread.DestroySubFrames();
                }

                if (card.frameProfile != null)
                {
                    Destroy(card.frameProfile.gameObject);
                }
            }
        }

		public void LoadLatestThreads()
		{
            FASForum.GetForumThreads(delegate(IList<Fresvii.AppSteroid.Models.Thread> _threads, Fresvii.AppSteroid.Models.Error _error)
            {
                if (this == null) return;

                if (!this.enabled) return;

                if (_error == null)
                {
                    foreach (Fresvii.AppSteroid.Models.Thread _thread in _threads)
                    {
                        UpdateThread(_thread);
                    }
                }
            });
		}

        float CalcScrollViewHeight()
        {
            float scrollViewHeight = forumTopMenu.height + verticalMargin;

            foreach (FresviiGUIThreadCard card in cards)
            {
                scrollViewHeight += card.GetHeight() + verticalMargin;
            }

            return scrollViewHeight;
        }

		private bool loading = false;

        private bool loadBlock = false;

		private Rect loadingLabelPosition;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

		private Rect loadingSpinnerPosition;

        private Fresvii.AppSteroid.Models.ListMeta pullUpListMeta = null;

        void Update()
        {
            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            //            baseRect = new Rect(Position.x, Position.y + forumTopMenu.height, Screen.width, Screen.height - forumTopMenu.height - OffsetPosition.y - ((FresviiGUIManager.Instance.ModeCount > 1) ? tabBar.height : 0.0f));
            baseRect = new Rect(Position.x, Position.y, Screen.width, Screen.height - OffsetPosition.y );

            float scrollViewHeight = CalcScrollViewHeight();

            if (loadingSpinner != null)
            {
                if (loadingSpinnerPlace == LoadingSpinnerPlace.Center)
                {
                    loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
                else if (loadingSpinnerPlace == LoadingSpinnerPlace.Top)
                {
                    loadingSpinner.Position = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + forumTopMenu.height + reloadHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
                else if (loadingSpinnerPlace == LoadingSpinnerPlace.Bottom)
                {
                    loadingSpinner.Position = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + baseRect.height - reloadHeight * 0.5f - loadingSpinnerSize.y * 0.5f - ((FresviiGUIManager.Instance.ModeCount > 1) ? tabBar.height : 0.0f), loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
            }

            //  on orientation device
            
            scrollViewRect.x = Position.x;

            scrollViewRect.width = Screen.width;

            scrollViewRect.height = scrollViewHeight;

            InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, reloadHeight, forumTopMenu.height, tabBar.height, OnPullDownReflesh, OnPullUpReflesh);
        
            if (loadBlock && !FASGesture.IsDragging)
            {
                loadBlock = false;
            }
		}

        public void OnUpdateScrollViewPosition(float value)
        {
            scrollViewRect.y = value;
        }

        public float UpdateThread(Fresvii.AppSteroid.Models.Thread thread)
        {
            foreach (FresviiGUIThreadCard _card in cards)
            {
                if (_card.Thread.Id == thread.Id)
                {
                    _card.Thread = thread;

                    return 0.0f;
                }
            }

            FresviiGUIThreadCard card = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/ThreadCard"))).GetComponent<FresviiGUIThreadCard>();

            card.transform.parent = this.transform;

            card.Init(thread, scaleFactor, postFix, this, Screen.width - 2 * sideMargin);
			            
            threads.Add(thread);

            cards.Add(card);

            threadIds.Add(thread.Id);

			SortCards();

            return card.GetHeight();
        }

        public void OnPullDownReflesh()
        {
            if (loading || loadBlock) return;

            loading = true;

            loadBlock = true;

            loadingSpinnerPlace = LoadingSpinnerPlace.Top;

            loadingSpinner.Position = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + baseRect.y + reloadHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinner.Position);

            pullRefleshing = true;

            FASForum.GetForumThreads(delegate(IList<Fresvii.AppSteroid.Models.Thread> _threads, Fresvii.AppSteroid.Models.Error error)
            {
                pullRefleshing = false;

                loading = false;

                OnCompletePullReflesh(scrollViewRect, baseRect);

                loadingSpinner.Hide();

                if (error == null)
                {
                    foreach (Fresvii.AppSteroid.Models.Thread thread in _threads)
                    {
                        UpdateThread(thread);
                    }

                    SortCards();
                }
            });
        }

        public void OnPullUpReflesh()
        {
            if (loading || loadBlock) return;

            uint loadPage = 2;

            if (pullUpListMeta != null)
            {
                if (pullUpListMeta.NextPage.HasValue)
                {
                    loadPage = (uint)pullUpListMeta.NextPage;
                }
                else
                {
                    loadPage = pullUpListMeta.TotalCount / pullUpListMeta.PerPage + 1;
                }
            }            

            loading = true;

            loadBlock = true;
            
            loadingSpinnerPlace = LoadingSpinnerPlace.Bottom;

            loadingSpinner.Position = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + baseRect.y + baseRect.height - reloadHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            
            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinner.Position);

            pullRefleshing = true;
            
            FASForum.GetForumThreads(loadPage, delegate(IList<Fresvii.AppSteroid.Models.Thread> _threads, Fresvii.AppSteroid.Models.ListMeta _meta, Fresvii.AppSteroid.Models.Error _error)
            {
                pullUpListMeta = _meta;

                pullRefleshing = false;

                loading = false;

                loadingSpinner.Hide();

                if (_error == null)
                {
                    float addedHeight = 0.0f;

                    foreach (Fresvii.AppSteroid.Models.Thread thread in _threads)
                    {
                        addedHeight += UpdateThread(thread);
                    }

                    SortCards();

                    if (addedHeight > 0.0f)
                    {
                        //scrollViewRect.y -= addedHeight;
                    }
                    else
                    {
                        OnCompletePullReflesh(scrollViewRect, baseRect, forumTopMenu.height, tabBar.height);
                    }
                }
                else
                {
                    OnCompletePullReflesh(scrollViewRect, baseRect, forumTopMenu.height, tabBar.height);
                }
            });
        }

        public float tweenTime = 1.0f;

        public iTween.EaseType tweenEaseType = iTween.EaseType.easeOutExpo;       
	
		private bool popUp = false;

        private FresviiGUIThreadCard popUpCard;
		
        public void PopUpDialog(FresviiGUIThreadCard card){
	
            this.popUpCard = card;
			
            popUp = true;
			
			FASGesture.Stop();
		}

        public void CancelPopUpDialog()
        {
            this.popUpCard = null;

            popUp = false;
        }

		public Rect GetRect()
        {
			return baseRect;
		}

        public void DeleteThread(FresviiGUIThreadCard card){

            cards.Remove(card);
        
			if(card.gameObject != null)
	            Destroy(card.gameObject);
        }

        public void AddThread(Fresvii.AppSteroid.Models.Thread thread)
        {
            threads.Add(thread);

            FresviiGUIThreadCard card = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/ThreadCard"))).GetComponent<FresviiGUIThreadCard>();
            
            card.transform.parent = this.transform;
            
            card.Init(thread, scaleFactor, postFix, this, Screen.width - 2 * sideMargin);
            
            cards.Insert(0, card);

            threadIds.Add(thread.Id);
        }

        public void CreateThread(string text, Texture2D clipImage, Fresvii.AppSteroid.Models.Video video)
        {
            Fresvii.AppSteroid.Models.Thread thread = new Fresvii.AppSteroid.Models.Thread();
            thread.User = FAS.CurrentUser;

            Fresvii.AppSteroid.Models.Comment comment = new Fresvii.AppSteroid.Models.Comment();
            
            thread.Comment = comment;
            
            thread.Comment.Text = text;

            if (video != null)
            {
                if (thread.Comment.Video == null)
                {
                    thread.Comment.Video = new Fresvii.AppSteroid.Models.Video();
                }

                thread.Comment.Video.VideoUrl = video.VideoUrl;
            }

			thread.Comment.User = FAS.CurrentUser;

            thread.CreatedAt = thread.UpdateAt = thread.LastUpdateAt = System.DateTime.Now;
            
            thread.Subscribed = true;

            FresviiGUIThreadCard card = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/ThreadCard"))).GetComponent<FresviiGUIThreadCard>();
            
            card.transform.parent = this.transform;
            
            card.clipImage = clipImage;

            card.Init(thread, scaleFactor, postFix, this, Screen.width - 2 * sideMargin);
            
			card.IsVideo = 	(video != null);

			cards.Add(card);

            SortCards();

            iTween.ValueTo(this.gameObject, iTween.Hash("from", scrollViewRect.y, "to", 0.0f, "time", tweenTime, "easetype", tweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition", "oncompletetarget", this.gameObject));

            if (video == null)
            {
                FASForum.CreateThread(text, clipImage, delegate(Fresvii.AppSteroid.Models.Thread _thread, Fresvii.AppSteroid.Models.Error _error)
                {
                    if (_error != null)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ThreadCreateError"), delegate(bool del) { });

                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                            Debug.LogError(_error.ToString());
                    }
                    else
                    {
                        card.Thread = _thread;
                        threads.Add(_thread);
                        threadIds.Add(_thread.Id);
                    }
                });
            }
            else
            {
                card.DataUploadProgressStart();

                FASForum.CreateVideoThread(text, video.Id, delegate(Fresvii.AppSteroid.Models.Thread _thread, Fresvii.AppSteroid.Models.Error _error)
                {
                    if (_error != null)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ThreadCreateError"), delegate(bool del) { });

                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                            Debug.LogError(_error.ToString());

                        cards.Remove(card);

                        Destroy(card.gameObject);
                    }
                    else
                    {
                        card.Thread = _thread;

                        threads.Add(_thread);

                        threadIds.Add(_thread.Id);

                        card.DataUploadProgressEnd();
                    }
                });
            }
        }

        public void SortCards()
        {
            cards.Sort((a, b) => System.DateTime.Compare(b.Thread.LastUpdateAt, a.Thread.LastUpdateAt));
        }

        public void ResetScrollPosition()
        {
            scrollViewRect.y = 0f;
        }

        public void ResetScrollPositionTween()
        {
			iTween.ValueTo(this.gameObject, iTween.Hash("from", scrollViewRect.y, "to", 0f, "time", 1.0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "OnUpdateScrollViewPosition"));
        }

        public void ShowThreadByNotification(string threadId)
        {
            FresviiGUIManager.Instance.SetViewMode(FASGui.Mode.Forum);

            if (frameCreateThread != null)
                frameCreateThread.SetDraw(false);

            foreach (FresviiGUIThreadCard card in cards)
            {
                if (card.frameThread != null)
                {
                    card.frameThread.SetDraw(false);

                    card.frameThread.DestroySubFrames();
                }

                if(card.frameProfile != null)
                    Destroy(card.frameProfile.gameObject);
            }

            foreach (FresviiGUIThreadCard card in cards)
            {
                if (card.Thread.Id == threadId)
                {
                    card.GoToThreadByNotification();

                    return;
                }
            }

            //  Load latest threads
            FASForum.GetForumThreads(delegate(IList<Fresvii.AppSteroid.Models.Thread> _threads, Fresvii.AppSteroid.Models.Error _error)
            {
                if (_error == null)
                {
                    foreach (Fresvii.AppSteroid.Models.Thread _thread in _threads)
                    {
                        UpdateThread(_thread);
                    }
                }
            });		
            
        }

        public void ReleaseAllThreadFrames()
        {
            foreach (FresviiGUIThreadCard card in cards)
            {
                if (card.frameThread != null)
                {
                    card.frameThread.SetDraw(false);

                    card.frameThread.DestroySubFrames();
                }

                if (card.frameProfile != null)
                    Destroy(card.frameProfile.gameObject);
            }
        }

        void OnGUI()
        {
            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);
            
            if (popUp)
            {
                Rect cancelRect = new Rect(0, 0, Screen.width, Screen.height);

                if (Event.current.type == EventType.MouseDown && cancelRect.Contains(Event.current.mousePosition))
                {
                    popUp = false;

                    popUpCard.PopUpCanceled();

                    Event.current.Use();
                }
            }
                
            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(scrollViewRect);

            float cardVpos = forumTopMenu.height + sideMargin;
                
            foreach (FresviiGUIThreadCard card in cards)
			{
                float cardHeight = card.GetHeight();

                card.SetPosition(new Rect(sideMargin, cardVpos, Screen.width - 2 * sideMargin, cardHeight), scrollViewRect);

                if (cardHeight > 0)
                    cardVpos += cardHeight + verticalMargin;

                card.Draw(Screen.width - 2 * sideMargin);
			}                

            GUI.EndGroup();

            GUI.EndGroup();
			

        }
    }
}
