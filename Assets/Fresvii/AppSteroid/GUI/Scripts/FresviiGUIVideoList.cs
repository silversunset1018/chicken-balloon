using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fresvii.AppSteroid.Gui
{
	public class FresviiGUIVideoList : FresviiGUIModalFrame
    {
        public enum Mode { FromUploded, Select, Share };

        public float sideMargin;

        private Texture2D palette;

        private FresviiGUIVideoListTop videoListTopMenu;

        private Rect baseRect;
        
        private Rect scrollViewRect;
        
        private float scaleFactor;

        private string postFix = "";
        
        public float topMargin;

        public float cardMargin;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;
        
        public Vector2 loadingSpinnerSize;

        private List<FresviiGUIVideoCard> cards = new List<FresviiGUIVideoCard>();

        public GameObject prfbVideoCard;

        private bool initialized;

		public float pollingInterval = 15f;

        public float pullRefleshHeight = 50f;

        private bool loading;

        private bool loadBlock;
		       
        private Fresvii.AppSteroid.Models.ListMeta videoListMeta = null;

        private bool isPullUp;

        public bool HasActionSheet = false;

        private Action<Fresvii.AppSteroid.Models.Video, Texture2D> callback;

        private Fresvii.AppSteroid.Models.Video video = null;

        private Texture2D videoThumbnail = null;

        public float videoNumLabelHeight = 36f;

        public GUIStyle guiStyleVideoNumLabel;

        public GUIContent contentUploaded;

        public static Texture2D TexVideoPlaybackIcon { get; protected set; }

        public static Texture2D TexVideoEyeIcon { get; protected set; }

        public static Texture2D TexVideoHeartIcon { get; protected set; }
        
        public static Texture2D TexButtonShare { get; protected set; }

        public static Texture2D TexButtonShareH { get; protected set; }

        public Mode mode = Mode.Select;

        public static Texture2D TexMenu { get; protected set; }

        public static Texture2D TexMenuH { get; protected set; }

        public bool IsModal = true;

        private FresviiGUITabBar tabBar;

        public Fresvii.AppSteroid.Models.User user;

        public bool IsMine { get; protected set; }

        public static void Show(FresviiGUIFrame postFrame, int guiDepth, Action<Fresvii.AppSteroid.Models.Video, Texture2D> callback, Mode mode = Mode.Select, Action shownCallback = null)
        {
			if(mode == FresviiGUIVideoList.Mode.FromUploded)
			{
				guiDepth += 1000;	
			}
            
            FresviiGUIVideoList frameVideoList = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/FresviiGUIFrameVideoList"))).GetComponent<FresviiGUIVideoList>();

            frameVideoList.IsModal = true;

			FresviiGUIManager.SetParameters();

			frameVideoList.Init(FASConfig.Instance.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, guiDepth);

            frameVideoList.PostFrame = postFrame;

			frameVideoList.SetCallback(callback);

            frameVideoList.SetDraw(true);

            frameVideoList.mode = mode;

            frameVideoList.Tween(new Vector2(0.0f, Screen.height), Vector2.zero, delegate()
            {
                if(postFrame != null)
                    postFrame.SetDraw(false);

				if(shownCallback != null)
					shownCallback();
            });
        }

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = guiDepth;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleVideoNumLabel.font = null;

                guiStyleVideoNumLabel.fontStyle = FontStyle.Bold;			
            }

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            videoListTopMenu = GetComponent<FresviiGUIVideoListTop>();

			videoListTopMenu.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

			tabBar = GetComponent<FresviiGUITabBar>();

            if (!IsModal)
            {
                tabBar.Init(postFix, scaleFactor, this.GuiDepth - 1);

                tabBar.GuiDepth = GuiDepth - 1;
            }
            else
            {
                tabBar.enabled = false;
            }

			this.scaleFactor = scaleFactor;

            this.postFix = postFix;
            
            loadingSpinnerSize *= scaleFactor;
            
            sideMargin *= scaleFactor;
            
            topMargin *= scaleFactor;

            cardMargin *= scaleFactor;

            videoNumLabelHeight *= scaleFactor;

            loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GuiDepth - 10);

            pullRefleshHeight *= scaleFactor;

            guiStyleVideoNumLabel.fontSize = (int)(guiStyleVideoNumLabel.fontSize * scaleFactor);

            SetScrollSlider(scaleFactor * 2.0f);

            TexVideoPlaybackIcon = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.VideoListIconPlayTextureName + postFix, false);

            TexVideoEyeIcon = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.VideoListIconEyeTextureName + postFix, false);

            TexVideoHeartIcon = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.VideoListIconHeaertTextureName + postFix, false);
            
            TexButtonShare = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ButtonShareTextureName + postFix, false);

			TexButtonShareH = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ButtonShareHTextureName + this.postFix, false);

            videoListLoading = true;

            if (user == null)
            {
                IsMine = true;

                FASVideo.GetCurrentUserVideoList(OnGetVideoList);
            }
            else
            {
                IsMine = false;

                string query = "{\"where\":[{\"collection\":\"users\", \"column\":\"id\", \"value\":\"" + user.Id + "\"}]}";

                FASVideo.GetVideoList(query, OnGetVideoList);
            }

			if(FASGesture.Instance == null)
            {
				gameObject.AddComponent<FASGesture>();
			}

            TexMenu = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumMenuTextureName + postFix, false);

            TexMenuH = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumMenuHTextureName + postFix, false);
		}

        public void OnUpdateScrollViewPosition(float value)
        {
            scrollViewRect.y = value;
        }

        public void OnScrollDelta(float delta)
        {
            scrollViewRect.y += delta;

            scrollViewRect.y = Mathf.Min(0f, scrollViewRect.y);
        }

        public void RemoveVideoCard(FresviiGUIVideoCard card)
        {
            cards.Remove(card);

            this.videoListMeta.TotalCount--;

            contentUploaded = new GUIContent(videoListMeta.TotalCount.ToString() + FresviiGUIText.Get("VideosUploaded"));
        }

        public void DeleteVideo(FresviiGUIVideoCard card)
        {
            FASVideo.DeleteVideo(card.Video.Id, (error) =>
            {
                if (error != null)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"), (del) => { });
                }
                else
                {
                    card.DeleteCardAnimation();
                }
            });
        }

        void OnGetVideoList(IList<Fresvii.AppSteroid.Models.Video> _videos, Fresvii.AppSteroid.Models.ListMeta _meta, Fresvii.AppSteroid.Models.Error _error)
        {
            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }

            if (bottomLoadingSpinner != null)
            {
                bottomLoadingSpinner.Hide();
            }

            videoListLoading = false;

            if(_error == null)
            {
                foreach (Fresvii.AppSteroid.Models.Video video in _videos)
                {
                    AddVideo(video);
                }

                this.videoListMeta = _meta;

                contentUploaded = new GUIContent(_meta.TotalCount.ToString() + FresviiGUIText.Get("VideosUploaded"));
            }
        }

        void AddVideo(Fresvii.AppSteroid.Models.Video video)
        {
            bool exists = false;

            foreach (FresviiGUIVideoCard card in cards)
            {
                if (card.Video.Id == video.Id)
                {
                    exists = true;

                    break;
                }
            }

            if (!exists)
            {
                FresviiGUIVideoCard card = ((GameObject)Instantiate(prfbVideoCard)).GetComponent<FresviiGUIVideoCard>();

                card.Init(video, scaleFactor, this, this.mode);

                card.transform.parent = this.transform;

                cards.Add(card);
            }
        }

        public void SetCallback(Action<Fresvii.AppSteroid.Models.Video, Texture2D> _callback)
        {
            this.callback = _callback;
        }
	
		void OnEnable()
		{
            ControlLock = false;
		}

        float CalcScrollViewHeight()
        {
            float height = videoListTopMenu.height + videoNumLabelHeight;

            foreach (FresviiGUIVideoCard card in cards)
                height += card.GetHeight() + cardMargin;

            if (bottomLoadingSpinner != null)
            {
                height += pullRefleshHeight;
            }

            return height;
        }

        private Rect videoUploadedLabel;

        void Update(){

            this.baseRect = new Rect(Position.x, Position.y, Screen.width, Screen.height - FresviiGUIFrame.OffsetPosition.y);

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            float scrollViewHeight = CalcScrollViewHeight();

            scrollViewRect.x = Position.x;

            scrollViewRect.width = Screen.width;

            scrollViewRect.height = scrollViewHeight;

            videoUploadedLabel = new Rect(cardMargin, videoListTopMenu.height, baseRect.width - 2f * cardMargin, videoNumLabelHeight);

			InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, pullRefleshHeight, videoListTopMenu.height, (tabBar.enabled) ? tabBar.height : 0.0f, null, OnPullUpReflesh);

            if (loadingSpinner != null)
            {
				loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && PostFrame != null && !ControlLock)
            {
                BackToPostFrame();
            }
#endif

			FASGesture.Use();
		}

        private bool videoListLoading;

        private Rect bottomLoadingSpinnerPosition;

        private Fresvii.AppSteroid.Gui.LoadingSpinner bottomLoadingSpinner;

        void OnPullUpReflesh()
        {
            if (videoListMeta != null)
            {
                if (videoListMeta.NextPage != null && !videoListLoading)
                {
                    bottomLoadingSpinnerPosition = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + baseRect.y + baseRect.height - pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

                    bottomLoadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(bottomLoadingSpinnerPosition, GuiDepth - 5);

                    videoListLoading = true;

                    if (user == null)
                    {
                        FASVideo.GetCurrentUserVideoList((uint)videoListMeta.NextPage, OnGetVideoList);
                    }
                    else
                    {
                        string query = "{\"where\":[{\"collection\":\"users\", \"column\":\"id\", \"value\":\"" + user.Id + "\"}]}";

                        FASVideo.GetVideoList((uint)videoListMeta.NextPage, query, OnGetVideoList);
                    }
                }
            }
        }

        public void Select(Fresvii.AppSteroid.Models.Video _video, Texture2D _videoThumbnail)
        {
            video = _video;

            videoThumbnail = (Texture2D) Instantiate(_videoThumbnail);

            BackToPostFrame();
        }

        public void Share(Fresvii.AppSteroid.Models.Video _video, Texture2D _videoThumbnail)
        {
			FASPlayVideo.videoShareGuiMode = FASPlayVideo.VideoShareGuiMode.WithLegacyGUI;

            FresviiGUIVideoSharing.Show(int.MinValue + 1000, _video, _videoThumbnail);
        }

        public void BackToPostFrame()
        {
            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }

            if(PostFrame != null)
                PostFrame.SetDraw(true);

            if (IsModal)
            {
                this.Tween(Vector2.zero, new Vector2(0.0f, Screen.height), delegate()
                {
                    Destroy(this.gameObject);
                });
            }
            else
            {
                this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
                {
                    Destroy(this.gameObject);
                });

                if(PostFrame != null)
                    PostFrame.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
            }

            if (this.callback != null)
            {
                this.callback(video, videoThumbnail);
            }
        }


        public override void SetDraw(bool on)
        {
			if(FresviiGUIManager.Instance != null)
			{
	            if (on)
    	            FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;
			}

            this.enabled = on;
            
            videoListTopMenu.enabled = on;

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

            if (contentUploaded != null)
            {
                GUI.Label(videoUploadedLabel, contentUploaded, guiStyleVideoNumLabel);
            }

            float cardY = videoListTopMenu.height + videoNumLabelHeight;

            foreach (FresviiGUIVideoCard card in cards)
            {
                Rect cardPosition = new Rect(cardMargin, cardY, baseRect.width - 2f * cardMargin, card.GetHeight());

                Rect drawPosition = new Rect(cardPosition.x, scrollViewRect.y + cardPosition.y, cardPosition.width, cardPosition.height);

                if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                {
                    card.Draw(cardPosition, scrollViewRect, GuiDepth, videoListTopMenu.height);
                }

                cardY += cardPosition.height + cardMargin;
            }

            GUI.EndGroup();

            GUI.EndGroup();

			if (Event.current.isMouse && Event.current.button >= 0)
			{
				Event.current.Use();
			}
        }
    }
}
	