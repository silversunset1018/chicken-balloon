using System.Collections;
using System.Collections.Generic;
using Fresvii.AppSteroid.Services;
using UnityEngine;

namespace Fresvii.AppSteroid.Gui
{
    [RequireComponent (typeof(FresviiGUIThreadTopMenu))]
    [RequireComponent(typeof(FresviiGUIThreadBottomMenu))]
    public class FresviiGUIThread : FresviiGUIFrame
    {
        private static readonly int cardMaxNum = 100;

        private float sideMargin = 8;

        public bool Initialized { get; protected set; }

        public float verticalMargin = 10;

        private Rect baseRect;
        private Rect scrollViewRect;
        private float scaleFactor;
        private string postFix;

        public Fresvii.AppSteroid.Models.Thread Thread { get; protected set; }
        private IList<Fresvii.AppSteroid.Models.Comment> comments;
        private List<string> commentIds = new List<string>();
        [HideInInspector]
		public List<FresviiGUICommentCard> cards = new List<FresviiGUICommentCard>();
        public int CommentCount { get { return cards.Count; } }

        private bool isReady;
        private bool isVisualReady;

        public static Texture2D TexForumLikeM { get; protected set; }
        public static Texture2D TexForumLikeMH { get; protected set; }
        public static Texture2D TexForumCommentL { get; protected set; }
        public static Texture2D TexForumCommentLH { get; protected set; }
        public static Texture2D TexForumLikeL { get; protected set; }
        public static Texture2D TexForumLikeLH { get; protected set; }

        private FresviiGUIThreadTopMenu threadTopMenu;
        private FresviiGUIThreadBottomMenu threadBottomMenu;

        [HideInInspector]
        public FresviiGUIThreadCard threadCard;

		public float pollingInterval = 15f;

        public bool HasPopUp { get; protected set; }

        private int pageLoaded = 0;

        public bool SetScrollPositionLast { get; set; }

        public Vector2 loadingSpinnerSize;

        private FresviiGUIScrollviewSlider slider;

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            threadTopMenu.enabled = on;

            threadBottomMenu.enabled = on;

            if (!on)                
            {
                if (loadingSpinner != null)
                {
                    loadingSpinner.Hide();
                }

                postDraw = false;
            }
        }

        public void SetThread(FresviiGUIThreadCard threadCard)
        {
            this.threadCard = threadCard;

            this.Thread = threadCard.Thread;
        }

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = guiDepth;
            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);
            threadTopMenu = GetComponent<FresviiGUIThreadTopMenu>();
            threadBottomMenu = GetComponent<FresviiGUIThreadBottomMenu>();
            this.postFix = postFix;

            threadTopMenu.Init(appIcon, postFix, scaleFactor);
            threadBottomMenu.Init(postFix, scaleFactor, this);

            this.scaleFactor = scaleFactor;
            this.sideMargin *= scaleFactor;
            this.verticalMargin = verticalMargin * scaleFactor;
            this.loadingSpinnerSize *= scaleFactor;
            this.reloadHeight *= scaleFactor;

            TexForumLikeM = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumLikeMTextureName + postFix, false);
            TexForumLikeMH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumLikeMHTextureName + postFix, false);
            TexForumLikeL = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumLikeLTextureName + postFix, false);
            TexForumLikeLH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumLikeLHTextureName + postFix, false);
            
            this.baseRect = new Rect(Position.x, Position.y + threadTopMenu.height, Screen.width, Screen.height - threadTopMenu.height - threadBottomMenu.height);

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });

                return;
            }
            else
            {
                if (cards == null)
                {
                    cards = new List<FresviiGUICommentCard>();
                }
                else
                {
                    foreach (FresviiGUICommentCard card in cards)
                    {
                        Destroy(card.gameObject);
                    }

                    cards.Clear();
                }

                pageLoaded = 0;

                int pageNum = (int)(Thread.CommentCount / 25 + 1);

                for (uint i = 1; i <= pageNum; i++)
                {
                    FASForum.GetThreadComments(this.Thread.Id, i, OnGetThreadComments);
                }
            }

            Initialized = true;

			loadingSpinnerPlace = LoadingSpinnerPlace.Center;
			
			loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, FASGui.GuiDepthBase);

            threadTopMenu.GuiDepth = GuiDepth - 1;

            threadBottomMenu.GuiDepth = GuiDepth - 1;

            SetScrollSlider(scaleFactor * 2.0f);
		}

        void OnEnable()
        {
            StartCoroutine(ReloadPolling());
        }

        private void OnGetThreadComments(IList<Fresvii.AppSteroid.Models.Comment> comments, Fresvii.AppSteroid.Models.Error error)
        {
            if (error != null)
            {
				loadingSpinner.Hide();

                if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.WWWTimeOut)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TimeOut"), delegate(bool del) { });

                    BackToForum();
                }
                else if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NotFound || (Application.platform == RuntimePlatform.Android && error.Detail.IndexOf("FileNotFound") >= 0))
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ThreadNone"), delegate(bool del) { });

                    threadCard.DeleteThreadNonConfirm();
                }
                else
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });

                    BackToForum();

                    Destroy(this.gameObject);
                }

                return;
            }

            this.comments = comments;

            scrollViewRect.y = 0f;

            foreach (Fresvii.AppSteroid.Models.Comment comment in comments)
            {
                FresviiGUICommentCard card = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/CommentCard"))).GetComponent<FresviiGUICommentCard>();

                card.transform.parent = this.transform;

                card.Init(comment, scaleFactor, Thread, Screen.width - 2f * sideMargin, this);

                cards.Add(card);

                commentIds.Add(comment.Id);
            }

            cards.Sort((a, b) => System.DateTime.Compare(a.Comment.CreatedAt, b.Comment.CreatedAt));

            scrollViewRect.y = 0.0f;

            pageLoaded++;

            if (pageLoaded == (int)(Thread.CommentCount / 25 + 1))
            {
				loadingSpinner.Hide();

                if (SetScrollPositionLast)
                {
                    SetScrollPositionLast = false;

                    if (CalcScrollViewHeight() > baseRect.height)
                        scrollViewRect.y = -CalcScrollViewHeight() + baseRect.height;
                }

                isReady = true;
            }
        }

		private IEnumerator ReloadPolling(){

			while(this.enabled){

				yield return new WaitForSeconds(pollingInterval);

                if (pollingWait)
                {
                    yield return new WaitForSeconds(pollingInterval);

                    pollingWait = false;
                }                

                uint loadPage = Thread.CommentCount / 25 + 1;

                FASForum.GetThreadComments(Thread.Id, loadPage, delegate(IList<Fresvii.AppSteroid.Models.Comment> _comments, Fresvii.AppSteroid.Models.Error _error)
                {
                    bool autoScroll = (scrollViewRect.y == -CalcScrollViewHeight() + baseRect.height);
                    
					if(_error != null){
							
						if (_error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NotFound || (Application.platform == RuntimePlatform.Android && _error.Detail.IndexOf("FileNotFound") >= 0))
						{
                            return;
						}							
					}
					else
					{
						foreach (Fresvii.AppSteroid.Models.Comment _comment in _comments)
						{
							if (!commentIds.Contains(_comment.Id))
							{
								FresviiGUICommentCard card = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/CommentCard"))).GetComponent<FresviiGUICommentCard>();
								
								card.transform.parent = this.transform;

                                card.Init(_comment, scaleFactor, Thread, Screen.width - 2f * sideMargin, this);
								
								cards.Add(card);
								
								comments.Add(_comment);
								
								commentIds.Add(_comment.Id);
								
							}
							else
							{
								foreach (FresviiGUICommentCard card in cards)
								{
									if (card.Comment.Id == _comment.Id)
									{
										card.Comment = _comment;
										break;
									}
								}
							}
						}
						
						cards.Sort((a, b) => System.DateTime.Compare(a.Comment.CreatedAt, b.Comment.CreatedAt));
						
						if (cards.Count > cardMaxNum)
						{
							for (int i = 0; i < cardMaxNum - cards.Count; i++)
							{
								FresviiGUICommentCard card = cards[0];
								cards.RemoveAt(0);
								Destroy(card.gameObject);
							}
						}

                        if (autoScroll)
                        {
                            iTween.ValueTo(this.gameObject, iTween.Hash("name", "AutoScroll", "from", scrollViewRect.y, "to", -CalcScrollViewHeight() + baseRect.height, "time", tweenTime, "easetype", tweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollPosition"));
                        }
					}
				});
			}			
		}
        
        private void RefleshAllComments()
        {
            int pageNum = (int)(Thread.CommentCount / 25 + 1);

            for (uint i = 1; i <= pageNum; i++)
            {
                FASForum.GetThreadComments(this.Thread.Id, i, OnRefleshComments);
            }
        }

        private void OnRefleshComments(IList<Fresvii.AppSteroid.Models.Comment> _comments, Fresvii.AppSteroid.Models.Error error)
        {
            if (error != null)
            {
                if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.WWWTimeOut)
                {
                    if (this != null && this.gameObject != null)
                    {
                        if (this.gameObject.activeInHierarchy && this.enabled)
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TimeOut"), delegate(bool del) { });
                        }
                    }
                }
                else if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NotFound || (Application.platform == RuntimePlatform.Android && error.Detail.IndexOf("FileNotFound") >= 0))
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ThreadNone"), delegate(bool del) { });

                    threadCard.DeleteThreadNonConfirm();
                }

				return;
            }

            foreach (Fresvii.AppSteroid.Models.Comment _comment in _comments)
            {
                if (!commentIds.Contains(_comment.Id))
                {
                    FresviiGUICommentCard card = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/CommentCard"))).GetComponent<FresviiGUICommentCard>();

                    card.transform.parent = this.transform;

                    card.Init(_comment, scaleFactor, Thread, Screen.width - 2f * sideMargin, this);

                    cards.Add(card);

                    comments.Add(_comment);

                    commentIds.Add(_comment.Id);

                }
                else
                {
                    foreach (FresviiGUICommentCard card in cards)
                    {
                        if (card.Comment.Id == _comment.Id)
                        {
                            card.Comment = _comment;

                            break;
                        }
                    }
                }
            }

            cards.Sort((a, b) => System.DateTime.Compare(a.Comment.CreatedAt, b.Comment.CreatedAt));
        }

        public void GoToUserProfile(FresviiGUIFrame frameProfile)
        {
            frameProfile.SetDraw(true);

            frameProfile.PostFrame = this;

            this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
            {
                this.SetDraw(false);
            });

            frameProfile.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
        }

        float CalcScrollViewHeight()
        {
            float scrollViewHeight = threadTopMenu.height + sideMargin;

            foreach (FresviiGUICommentCard card in cards)
            {
                scrollViewHeight += card.GetHeight() + verticalMargin;
            }

			scrollViewHeight += sideMargin;

            return scrollViewHeight;
        }

        public float tweenTime = 1.0f;
        
        public iTween.EaseType tweenEaseType = iTween.EaseType.easeOutExpo;
        
        public float reloadHeight = 50f;

        private bool loadBlock;

        private bool loading = false;

        private bool pollingWait = false;

        private Rect loadingLabelPosition;

        public int guiDepth = 1;

        private uint loadedPage = 1;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

		private Rect loadingSpinnerPosition = new Rect();

		private enum LoadingSpinnerPlace { Top, Center, Bottom };
		private LoadingSpinnerPlace loadingSpinnerPlace = LoadingSpinnerPlace.Center;

        private bool postDraw = false;

		private bool lagRotationProc;

        private Fresvii.AppSteroid.Models.ListMeta pullUpListMeta;

        void Update()
        {
            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            //this.baseRect = new Rect(Position.x, Position.y + threadTopMenu.height, Screen.width, Screen.height - threadTopMenu.height - threadBottomMenu.height  - OffsetPosition.y);

            baseRect = new Rect(Position.x, Position.y, Screen.width, Screen.height - OffsetPosition.y);

            if (loadingSpinner != null)
            {
                if (loadingSpinnerPlace == LoadingSpinnerPlace.Center)
                {
                    loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
                else if (loadingSpinnerPlace == LoadingSpinnerPlace.Top)
                {
                    loadingSpinner.Position = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + threadTopMenu.height + reloadHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
                else if (loadingSpinnerPlace == LoadingSpinnerPlace.Bottom)
                {
                    loadingSpinner.Position = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + baseRect.height - reloadHeight * 0.5f - loadingSpinnerSize.y * 0.5f - threadBottomMenu.height, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
            }

            if (comments == null)
            {
                return;
            }

            float scrollViewHeight = CalcScrollViewHeight();

            scrollViewRect.x = Position.x;

            scrollViewRect.width = Screen.width;

            scrollViewRect.height = scrollViewHeight;

            InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, reloadHeight, threadTopMenu.height, threadBottomMenu.height, OnPullDownReflesh, OnPullUpReflesh);

            if (!postDraw)
            {
                postDraw = true;

                RefleshAllComments();
            }

            if (loadBlock && !FASGesture.IsDragging)
            {
                loadBlock = false;
            }

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && !ControlLock)
            {
                BackToForum();
            }
#endif
        }

        public void OnUpdateScrollViewPosition(float value)
        {
            scrollViewRect.y = value;
        }

        public void OnPullDownReflesh()
        {
            if (loading || loadBlock) return;

            loading = true;

            loadBlock = true;

            loadingSpinnerPlace = LoadingSpinnerPlace.Top;

            loadingSpinnerPosition = new Rect(baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + reloadHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, FASGui.GuiDepthBase);

            pullRefleshing = true;

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

            FASForum.GetThreadComments(Thread.Id, loadPage, delegate(IList<Fresvii.AppSteroid.Models.Comment> _comments, Fresvii.AppSteroid.Models.ListMeta _meta, Fresvii.AppSteroid.Models.Error _error)
            {
                pullUpListMeta = _meta;

                pullRefleshing = false;

                bool added = false;

                loadingSpinner.Hide();

                OnCompletePullReflesh(scrollViewRect, baseRect);

                if (_error == null)
                {
                    loading = false;

                    int insertIndex = 0;

                    foreach (Fresvii.AppSteroid.Models.Comment _comment in _comments)
                    {
                        if (!commentIds.Contains(_comment.Id))
                        {
                            FresviiGUICommentCard card = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/CommentCard"))).GetComponent<FresviiGUICommentCard>();
                            card.transform.parent = this.transform;
                            card.Init(_comment, scaleFactor, Thread, Screen.width - 2f * sideMargin, this);
                            cards.Add(card);

                            //scrollPosition.y -= card.GetHeight();
                            comments.Add(_comment);
                            commentIds.Add(_comment.Id);
                            insertIndex++;
                            added = true;
                        }
                        else
                        {
                            foreach (FresviiGUICommentCard card in cards)
                            {
                                if (card.Comment.Id == _comment.Id)
                                {
                                    card.Comment = _comment;
                                    break;
                                }
                            }
                        }
                    }
                }

                cards.Sort((a, b) => System.DateTime.Compare(a.Comment.CreatedAt, b.Comment.CreatedAt));

                if (cards.Count > cardMaxNum)
                {
                    for (int i = 0; i < cardMaxNum - cards.Count; i++)
                    {
                        FresviiGUICommentCard card = cards[cards.Count - 1];

                        cards.RemoveAt(cards.Count - 1);

                        Destroy(card.gameObject);
                    }
                }

                if (!FASGesture.IsTouching && !added)
                {
                    iTween.ValueTo(this.gameObject, iTween.Hash("from", scrollViewRect.y, "to", 0.0f, "time", tweenTime, "easetype", tweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollPosition"));
                }
            });
        }

        public void OnPullUpReflesh()
        {
            if (loading || loadBlock) return;

            loading = true;

            loadBlock = true;

            loadingSpinnerPlace = LoadingSpinnerPlace.Bottom;

            loadingSpinnerPosition = new Rect(baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + baseRect.height - reloadHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, FASGui.GuiDepthBase);

            loadedPage++;

            pullRefleshing = true;

            FASForum.GetThreadComments(Thread.Id, loadedPage, delegate(IList<Fresvii.AppSteroid.Models.Comment> _comments, Fresvii.AppSteroid.Models.Error _error)
            {
                bool added = false;

                pullRefleshing = false;

                loadingSpinner.Hide();

                OnCompletePullReflesh(scrollViewRect, baseRect, threadTopMenu.height, threadBottomMenu.height);

                if (_error == null)
                {
                    foreach (Fresvii.AppSteroid.Models.Comment _comment in _comments)
                    {
                        if (!commentIds.Contains(_comment.Id))
                        {
                            FresviiGUICommentCard card = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/CommentCard"))).GetComponent<FresviiGUICommentCard>();

                            card.transform.parent = this.transform;

                            card.Init(_comment, scaleFactor, Thread, Screen.width - 2f * sideMargin, this);

                            cards.Add(card);

                            comments.Add(_comment);

                            commentIds.Add(_comment.Id);

                            added = true;
                        }
                        else
                        {
                            foreach (FresviiGUICommentCard card in cards)
                            {
                                if (card.Comment.Id == _comment.Id)
                                {
                                    card.Comment = _comment;
                                    break;
                                }
                            }
                        }
                    }

                    cards.Sort((a, b) => System.DateTime.Compare(a.Comment.CreatedAt, b.Comment.CreatedAt));

                    if (cards.Count > cardMaxNum)
                    {
                        for (int i = 0; i < cardMaxNum - cards.Count; i++)
                        {
                            FresviiGUICommentCard card = cards[0];
                            cards.RemoveAt(0);
                            Destroy(card.gameObject);
                        }
                    }

                }
                else
                {
                    loadedPage--;
                }

                if (!added) loadedPage--;

                loading = false;

                if (!FASGesture.IsTouching && !added)
                {
                    iTween.ValueTo(this.gameObject, iTween.Hash("from", scrollViewRect.y, "to", - scrollViewRect.height + baseRect.height - threadBottomMenu.height, "time", tweenTime, "easetype", tweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollPosition"));
                }
            });
        }

        public void ResetScrollPosition()
        {
            scrollViewRect.y = 0.0f;
        }

        public void ResetScrollPositionTween()
        {
            iTween.ValueTo(this.gameObject, iTween.Hash("from", scrollViewRect.y, "to", 0.0f, "time", 1.0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "OnUpdateScrollPosition"));
        }

        void OnUpdateScrollPosition(float value)
        {
            scrollViewRect.y = value;
        }

        public void LoadLatestComments()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                uint pageNum = Thread.CommentCount / 25 + 1;

                FASForum.GetThreadComments(Thread.Id, pageNum, delegate(IList<Fresvii.AppSteroid.Models.Comment> _comments, Fresvii.AppSteroid.Models.Error _error)
                {
                    if (_error != null)
                    {
                        if (_error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.WWWTimeOut)
                        {
                            if (this != null && this.gameObject != null)
                            {
                                if (this.gameObject.activeInHierarchy && this.enabled)
                                {
                                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TimeOut"), delegate(bool del) { });
                                }
                            }
                        }
                        else if (_error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NotFound || (Application.platform == RuntimePlatform.Android && _error.Detail.IndexOf("FileNotFound") >= 0))
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ThreadNone"), delegate(bool del) { });

                            threadCard.DeleteThreadNonConfirm();

                            return;
                        }
                    }
                    else
                    {
                        foreach (Fresvii.AppSteroid.Models.Comment _comment in _comments)
                        {
                            if (!commentIds.Contains(_comment.Id))
                            {
                                FresviiGUICommentCard card = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/CommentCard"))).GetComponent<FresviiGUICommentCard>();

                                card.transform.parent = this.transform;

                                card.Init(_comment, scaleFactor, Thread, Screen.width - 2f * sideMargin, this);

                                cards.Add(card);

                                comments.Add(_comment);

                                commentIds.Add(_comment.Id);

                            }
                            else
                            {
                                foreach (FresviiGUICommentCard card in cards)
                                {
                                    if (card.Comment.Id == _comment.Id)
                                    {
                                        card.Comment = _comment;
                                        break;
                                    }
                                }
                            }
                        }

                        cards.Sort((a, b) => System.DateTime.Compare(a.Comment.CreatedAt, b.Comment.CreatedAt));                     
                    }
                });
            }
        }

        public void AddComment(string threadId, string text, Texture2D clipImage, Fresvii.AppSteroid.Models.Video video)
        {
            if (text.Length > Fresvii.AppSteroid.Models.Comment.TextMaxLength)
            {
                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TextTooLong"), delegate(bool del) { });

                return;
            }

            pollingWait = true;

            FresviiGUICommentCard card = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/CommentCard"))).GetComponent<FresviiGUICommentCard>();

            card.transform.parent = this.transform;

            Fresvii.AppSteroid.Models.Comment comment = new Fresvii.AppSteroid.Models.Comment();

            comment.Text = text;
            
            comment.User = FAS.CurrentUser;
           
            comment.CreatedAt = comment.UpdateAt = System.DateTime.Now;

            card.ClipImage = clipImage;

            card.Init(comment, scaleFactor, Thread, Screen.width - 2f * sideMargin, this);

            if (video != null)
            {
                if (card.Comment.Video == null)
                {
                    card.Comment.Video = new Fresvii.AppSteroid.Models.Video();
                }

                card.Comment.Video.VideoUrl = video.VideoUrl;

                card.IsVideo = true;
            }

            cards.Add(card);
            
            comments.Add(comment);

			float scrollViewHeight = CalcScrollViewHeight();

			scrollViewHeight += threadBottomMenu.height;

			if(baseRect.height < scrollViewHeight)
				iTween.ValueTo(this.gameObject, iTween.Hash("from", scrollViewRect.y, "to", - scrollViewHeight + baseRect.height, "time", tweenTime, "easetype", tweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollPosition"));

            threadCard.Thread.LastUpdateAt = System.DateTime.Now;

            threadCard.Forum.SortCards();

            threadCard.Forum.ResetScrollPosition();

            if (video == null)
            {
                FASForum.AddComment(threadId, text, clipImage, delegate(Fresvii.AppSteroid.Models.Comment _comment, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error != null)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                        if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.WWWTimeOut)
                        {
                            if (this != null && this.gameObject != null)
                            {
                                if (this.gameObject.activeInHierarchy && this.enabled)
                                {
                                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TimeOut"), delegate(bool del) { });
                                }
                            }
                        }
                        else if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NotFound || (Application.platform == RuntimePlatform.Android && error.Detail.IndexOf("FileNotFound") >= 0))
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ThreadNone"), delegate(bool del) { });

                            threadCard.DeleteThreadNonConfirm();
                        }
                        else
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("CommentCreateError"), delegate(bool del) { });
                        }

                        cards.Remove(card);

                        Destroy(card.gameObject);
                    }
                    else
                    {
                        if (card == null)
                        {
                            return;
                        }
                        else
                        {
                            card.Comment = _comment;

                            commentIds.Add(_comment.Id);

                            LoadLatestComments();
                        }
                    }
                });
            }
            else
            {
                FASForum.AddComment(threadId, text, video.Id,

                    delegate(Fresvii.AppSteroid.Models.Comment _comment, Fresvii.AppSteroid.Models.Error error)
                    {
                        if (error != null)
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                            if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.WWWTimeOut)
                            {
                                if (this != null && this.gameObject != null)
                                {
                                    if (this.gameObject.activeInHierarchy && this.enabled)
                                    {
                                        Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TimeOut"), delegate(bool del) { });
                                    }
                                }
                            }
                            else if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NotFound || (Application.platform == RuntimePlatform.Android && error.Detail.IndexOf("FileNotFound") >= 0))
                            {
                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ThreadNone"), delegate(bool del) { });

                                threadCard.DeleteThreadNonConfirm();
                            }
                            else
                            {
                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("CommentCreateError"), delegate(bool del) { });
                            }

                            cards.Remove(card);

                            Destroy(card.gameObject);
                        }
                        else
                        {
                            if (card == null)
                            {
                                return;
                            }
                            else
                            {
                                card.Comment = _comment;

                                commentIds.Add(_comment.Id);

                                LoadLatestComments();
                            }
                        }
                    });
            }

            Thread.Subscribed = true;

            threadCard.Thread.Subscribed = true;

            threadCard.Thread.CommentCount += 1;
        }

        void OnGUI()
        {
            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            if (isReady)
            {
                GUI.BeginGroup(baseRect);

                GUI.BeginGroup(scrollViewRect);

                float cardVpos = sideMargin + threadTopMenu.height;

                foreach (FresviiGUICommentCard card in cards)
                {
                    card.Draw(new Rect(sideMargin, cardVpos, Screen.width - 2 * sideMargin, card.GetHeight()), scrollViewRect, Screen.width - 2 * sideMargin);

                    cardVpos += card.GetHeight() + verticalMargin;
                }

                GUI.EndGroup();

                GUI.EndGroup();
            }			

        }

        public void BackToForum()
        {
            threadCard.Forum.SetDraw(true);
            
            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
            {
                this.SetDraw(false);
            });

            threadCard.Forum.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
        }

        public void Reload()
        {
            FASForum.GetThreadComments(threadCard.Thread.Id, OnGetThreadComments);
        }

        private Fresvii.AppSteroid.Gui.ActionSheet actionSheet;

        private Fresvii.AppSteroid.Gui.ActionSheet actionSheetReport;

        void OnDisable()
        {
            if (actionSheet != null)
            {
                actionSheet.Hide();
            }

            if (actionSheetReport != null)
            {
                actionSheetReport.Hide();
            }
        }

        public void ShowActionSheet(FresviiGUICommentCard card)
        {
            HasPopUp = true;            

            List<string> buttons = new List<string>();

            if (!string.IsNullOrEmpty(card.Comment.Text))
            {
                buttons.Add(FresviiGUIText.Get("CopyText"));
            }

            if(card.ClipImage != null)
            {
                buttons.Add(FresviiGUIText.Get("SaveImage"));
            }

            if (card.IsMine())
            {
                buttons.Add(FresviiGUIText.Get("EditText"));

                if (card.ClipImage != null)
                {
                    buttons.Add(FresviiGUIText.Get("ChangeImageFromLibrary"));

                    buttons.Add(FresviiGUIText.Get("ChangeImageFromCamera"));
                }
                else
                {
                    buttons.Add(FresviiGUIText.Get("AddImageFromLibrary"));

                    buttons.Add(FresviiGUIText.Get("AddImageFromCamera"));
                }

                if (card.Comment.Id != Thread.Comment.Id)
                {
                    buttons.Add(FresviiGUIText.Get("Delete"));
                }
            }

            if (!string.IsNullOrEmpty(card.IncludingUrl))
            {
                buttons.Add(FresviiGUIText.Get("OpenLink"));
            }

            buttons.Add(FresviiGUIText.Get("Report"));

            actionSheet = Fresvii.AppSteroid.Gui.ActionSheet.Show(scaleFactor, postFix, this.GuiDepth - 30, buttons.ToArray(), (selectedButton) =>
            {
                HasPopUp = false;

                if (selectedButton == FresviiGUIText.Get("CopyText")) // copy
                {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS) 
					Fresvii.AppSteroid.Util.Clipboard.SetText(card.Comment.Text);
#endif
                }
                else if (selectedButton == FresviiGUIText.Get("SaveImage"))
                {
                    Fresvii.AppSteroid.Util.ImagePicker.SaveImageData(this, card.ClipImage.EncodeToPNG(), System.IO.Path.GetFileName(card.Comment.ImageThumbnailUrl));
                }
                else if (selectedButton == FresviiGUIText.Get("EditText")) // edit
                {
                    FASGesture.Pause();

                    GetComponent<FresviiGUIPopUpTextInput>().Show(card.Comment.Text, true, delegate(string text)
                    {
                        FASGesture.Resume();

                        string origText = card.Comment.Text;

                        card.Comment.Text = text;

                        threadBottomMenu.ClearTextInputField();

                        FASForum.EditComment(card.Comment.Id, text, "", delegate(Fresvii.AppSteroid.Models.Comment comment, Fresvii.AppSteroid.Models.Error error)
                        {
                            if (error == null)
                            {
                                card.Comment = comment;
                            }
                            else if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.WWWTimeOut)
                            {
                                if (this != null && this.gameObject != null)
                                {
                                    if (this.gameObject.activeInHierarchy && this.enabled)
                                    {
                                        Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TimeOut"), delegate(bool del) { });
                                    }
                                }
                            }
                            else if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NotFound || (Application.platform == RuntimePlatform.Android && error.Detail.IndexOf("FileNotFound") >= 0))
                            {
                                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ThreadNone"), delegate(bool del) { });

                                threadCard.DeleteThreadNonConfirm();
                            }
                            else
                            {
                                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("CommentCreateError"), delegate(bool del) { });

                                card.Comment.Text = origText;
                            }

                        });
                    });
                }
                else if (selectedButton == FresviiGUIText.Get("ChangeImageFromLibrary") || selectedButton == FresviiGUIText.Get("ChangeImageFromCamera")
                    || selectedButton == FresviiGUIText.Get("AddImageFromLibrary") || selectedButton == FresviiGUIText.Get("AddImageFromCamera"))
                {
                    Fresvii.AppSteroid.Util.ImagePicker.Type type = (selectedButton == FresviiGUIText.Get("ChangeImageFromLibrary") || selectedButton == FresviiGUIText.Get("AddImageFromLibrary")) ? Fresvii.AppSteroid.Util.ImagePicker.Type.Gallery : Fresvii.AppSteroid.Util.ImagePicker.Type.Camera;

                    Fresvii.AppSteroid.Util.ImagePicker.Show(this, type, (image) =>
                    {
                        Texture2D origImage = card.ClipImage;

                        if (image != null)
                        {
                            card.ClipImage = image;

                            FASForum.EditComment(card.Comment.Id, "", image, (comment, error) =>
                            {
                                if (error == null)
                                {
                                    if (!string.IsNullOrEmpty(card.Comment.ImageThumbnailUrl))
                                    {
                                        FresviiGUIManager.Instance.resourceManager.ReleaseTexture(card.Comment.ImageThumbnailUrl);
                                    }

                                    card.Comment = comment;
                                }
                                else if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.WWWTimeOut)
                                {
                                    if (this != null && this.gameObject != null)
                                    {
                                        if (this.gameObject.activeInHierarchy && this.enabled)
                                        {
                                            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TimeOut"), delegate(bool del) { });
                                        }
                                    }
                                }
                                else if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NotFound || (Application.platform == RuntimePlatform.Android && error.Detail.IndexOf("FileNotFound") >= 0))
                                {
                                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ThreadNone"), delegate(bool del) { });

                                    threadCard.DeleteThreadNonConfirm();
                                }
                                else
                                {
                                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("CommentEditError"), delegate(bool del) { });

                                    card.ClipImage = origImage;
                                }
                            });
                        }
                    });
                }
                else if (selectedButton == FresviiGUIText.Get("Delete"))
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Delete"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("ConfirmDeleteComment"), delegate(bool del)
                    {
                        if (del)
                        {
                            if (threadCard.Thread.CommentCount >= 1)
                            {
                                threadCard.Thread.CommentCount -= 1;
                            }

                            card.DeleteCard();
                        }
                    });
                }    
                else if(selectedButton == FresviiGUIText.Get("Report"))
                {
                    StartCoroutine(ReportDelay(card.Comment.Id));
                }
                else if (selectedButton == FresviiGUIText.Get("OpenLink"))
                {
                    Application.OpenURL(card.IncludingUrl);
                }

            });

            FASGesture.Stop();
        }

        IEnumerator ReportDelay(string commentId)
        {
            yield return new WaitForSeconds(1.0f);

            Report(commentId);
        }

        public void Report(string commentId)
        {
            HasPopUp = true;

            List<string> buttons = new List<string>();

            foreach (ForumService.ReportKind kind in System.Enum.GetValues(typeof(ForumService.ReportKind)))
            {
                buttons.Add(FresviiGUIText.Get(kind.ToString()));
            }

            actionSheetReport = Fresvii.AppSteroid.Gui.ActionSheet.Show(scaleFactor, postFix, this.GuiDepth - 30, buttons.ToArray(), (selectedButton) =>
            {
                HasPopUp = false;

                for (int i = 0; i < buttons.Count; i++)
                {
                    if (selectedButton == FresviiGUIText.Get(((ForumService.ReportKind)i).ToString()))
                    {
                        FASForum.ReportComment(commentId, (ForumService.ReportKind)i, "", (comment, error) => 
                        {
                            if (error != null && error.Code != (int)Fresvii.AppSteroid.Models.Error.ErrorCode.ArgumentError) // 402
                            {
                                Debug.LogError("ReportComment :" + error.ToString());

                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("OK"), (del) => { });
                            }
                            else
                            {
                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ReportSent"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("OK"), (del) => { });
                            }
                        
                        });

                        return;
                    }
                }
            });
        }

        public void OnScrollDelta(float delta)
        {
            scrollViewRect.y += delta;

            scrollViewRect.y = Mathf.Min(0f, scrollViewRect.y);
        }

        public void RemoveCard(FresviiGUICommentCard card)
        {
            cards.Remove(card);

            Destroy(card.gameObject);

			if(cards.Count == 0){

				FresviiGUIForum.Instance.DeleteThread(threadCard);

				BackToForum();
			}
        }

        public void DestroySubFrames()
        {
            foreach (FresviiGUICommentCard card in cards)
            {
                if(card.frameProfile != null)
                    Destroy(card.frameProfile.gameObject);
            }
        }
    }
}
