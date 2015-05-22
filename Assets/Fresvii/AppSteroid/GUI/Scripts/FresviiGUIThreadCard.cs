using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIThreadCard : MonoBehaviour
    {
        private bool userIconLoading = false;

        private bool clipImageLoading = false;

        //  public
        private Texture palette;
        private Rect texCoordsBackground;
        private Rect texCoordsLine;

        private Rect texCoodsProgressBar;

        private readonly float defaultImageHeight = 280;
        private float imageHeight = 280;

        public Rect userIconPosition;
        private Rect userIconHitPosition;
		public Texture2D userIconDefault;
		public Texture2D userIconMask;
		private Texture2D userIcon;

        public Rect userNamePosition;
        public GUIStyle guiStyleUserName;

        public Rect timespanPosition;
        public GUIStyle guiStyleTimeSpan;

        public Rect likeCountIconPosition;
        public Rect likeCountLabelPosition;

        public Rect commentCountIconPosition;
        public Rect commentCountLabelPosition;

        public Rect commentTextPosition;
        public GUIStyle guiStyleCommentText;

        public Rect commentLButtonPosition;
        //public GUIStyle guiStyleCommentL;

        public Rect likeLButtonPosition;
        //public GUIStyle guiStyleLikeL;

        public float miniMargin = 4;
        public float margin = 12;

        public Fresvii.AppSteroid.Models.Thread Thread { get; set; }
      
        //  private

        private float scaleFactor;

        private string postFix;

        private float cardAlpha = 1.0f;

        private GUIContent contentUserName = new GUIContent("");
        private GUIContent contentCommentText;
        private string userProfileUrl;

        private Rect clipImagePosition;

        private Rect videoPlaybackIconPosition;

        [HideInInspector]
        public Texture2D clipImage;
        private float clipAlpha = 0.0f;

        public FresviiGUIForum Forum { get; set; }

        public float menuButtonMargin = 16;
		private Rect menuButtonPosition;
        private Rect menuButtonHitPosition;

		private Rect position;
        private Rect scrollViewRect;

        public float lineWidth = 1.0f;
        public float bottomButtonHeight = 37f;

        private bool isLike = false;

		public float commentLineMaxNum = 4.0f;

        public FresviiGUIButton buttonMenu;
        public FresviiGUIButton buttonUserIcon;
        public FresviiGUIButton buttonLike;
        public FresviiGUIButton buttonComment;
        public FresviiGUIButton buttonCard;

		public float pollingInterval = 15f;

        public Vector2 popUpOffset;

        private Fresvii.AppSteroid.Gui.PopUpBalloonMenu popUpBaloonMenu;

        public GameObject prfbGUIFrameThread;

        [HideInInspector]
        public FresviiGUIThread frameThread;

        public GameObject prfbGUIFrameUserProfile;
        
        public GameObject prfbGUIFrameMyProfile;

        [HideInInspector]
        public FresviiGUIFrame frameProfile;

        public bool HasPopUp { get; protected set; }

        private bool dataUploading = false;

        private float dataUploadingProgress = 0.0f;

        private Rect progressBarPosition;

        public Material userIconMaterial;

        public void Init(Fresvii.AppSteroid.Models.Thread thread, float scaleFactor, string postFix, FresviiGUIForum forum, float cardWidth)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleUserName.font = null;
                guiStyleUserName.fontStyle = FontStyle.Bold;
                guiStyleTimeSpan.font = null;
                guiStyleCommentText.font = null;

				likeCountIconPosition.y += scaleFactor;

				commentCountIconPosition.y += scaleFactor;
            }

            this.Thread = thread;
            this.scaleFactor = scaleFactor;
            this.postFix = postFix;
            this.Forum = forum;
            this.popUpOffset *= scaleFactor;

            userIconPosition = FresviiGUIUtility.RectScale(userIconPosition, scaleFactor);
            commentLButtonPosition = FresviiGUIUtility.RectScale(commentLButtonPosition, scaleFactor);
            likeLButtonPosition = FresviiGUIUtility.RectScale(likeLButtonPosition, scaleFactor);
            userNamePosition = FresviiGUIUtility.RectScale(userNamePosition, scaleFactor);
            timespanPosition = FresviiGUIUtility.RectScale(timespanPosition, scaleFactor);
            commentTextPosition = FresviiGUIUtility.RectScale(commentTextPosition, scaleFactor);
            likeCountIconPosition = FresviiGUIUtility.RectScale(likeCountIconPosition, scaleFactor);
            likeCountLabelPosition = FresviiGUIUtility.RectScale(likeCountLabelPosition, scaleFactor);
            commentCountIconPosition = FresviiGUIUtility.RectScale(commentCountIconPosition, scaleFactor);
            commentCountLabelPosition = FresviiGUIUtility.RectScale(commentCountLabelPosition, scaleFactor);
            clipImagePosition = FresviiGUIUtility.RectScale(clipImagePosition, scaleFactor);

            margin *= scaleFactor;

            miniMargin *= scaleFactor;
            
            guiStyleUserName.fontSize = (int)(guiStyleUserName.fontSize * scaleFactor);

            guiStyleUserName.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName); 
            
            guiStyleTimeSpan.fontSize = (int)(guiStyleTimeSpan.fontSize * scaleFactor);

            guiStyleTimeSpan.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText2);

            guiStyleCommentText.fontSize = (int)(guiStyleCommentText.fontSize * scaleFactor);

            guiStyleCommentText.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText1);

            lineWidth *= scaleFactor;
            
            bottomButtonHeight *= scaleFactor;
            
            menuButtonMargin *= scaleFactor;

            imageHeight = defaultImageHeight * scaleFactor;

            SetCommentText(cardWidth);

            userIconHitPosition = new Rect(0, 0, userIconPosition.x + userIconPosition.width + margin, userIconPosition.y + userIconPosition.height + margin);

            palette = FresviiGUIColorPalette.Palette;

            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            texCoordsLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardSeperateLine1);

            texCoodsProgressBar = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardProgressBar);

            isLike = thread.Comment.Like;

            iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateCardAlpha", "oncompletetarget", this.gameObject));

            if(clipImage != null)
                iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateAlpha", "oncompletetarget", this.gameObject));
        }

        private void SetUserName()
        {
			if(Thread.User == null) return;

			contentUserName = new GUIContent(FresviiGUIUtility.Truncate(Thread.User.Name, guiStyleUserName, userNamePosition.width, "..."));
        }

        private void SetCommentText(float cardWidth)
        {
            commentTextPosition.x = margin;

            commentTextPosition.width = cardWidth - margin * 2f;

            
            contentCommentText = new GUIContent(Thread.Comment.Text);

            
            commentTextPosition.height = guiStyleCommentText.CalcHeight(contentCommentText, commentTextPosition.width);

            int deleteStringNum = 2;
            
			while (commentTextPosition.height > guiStyleCommentText.lineHeight * commentLineMaxNum)
            {
                contentCommentText = new GUIContent(Thread.Comment.Text.Substring(0, Thread.Comment.Text.Length - deleteStringNum) + "...");

                commentTextPosition.height = guiStyleCommentText.CalcHeight(contentCommentText, commentTextPosition.width);
                
				deleteStringNum++;
            }
        }

        public float imageTweenTime = 1.0f;
        public iTween.EaseType imageTweenEasetype;

        void OnUpdateCardAlpha(float value)
        {
            cardAlpha = value;
        }

        void OnCompleteAlphaShow()
        {
            cardAlpha = 1.0f;
        }

        public bool IsSubscribed()
        {
            return this.Thread.Subscribed;
        }

        public bool IsMine()
        {
            return Thread.User.Id == FAS.CurrentUser.Id;
        }

        public void SetLike(bool isLike)
        {
            this.isLike = isLike;
            this.Thread.Comment.Like = isLike;
        }

        public float GetHeight(){

			if(deleteAnimationDone) return 0.0f;

            if (heightTweening) return tweenHeight;

            float height = 0.0f;
            height += margin;
            height += userIconPosition.height;
            height += margin;

            if (!string.IsNullOrEmpty(contentCommentText.text))
            {
                height += commentTextPosition.height;
                height += margin + 1.0f * scaleFactor;
            }
            else
            {
                height += 1.0f * scaleFactor;
            }

			if ( !string.IsNullOrEmpty(Thread.Comment.ImageThumbnailUrl) || (Thread.Comment.VideoState != Fresvii.AppSteroid.Models.Comment.VideoStatus.NA) || clipImage != null)
            {                               
                clipImagePosition.y = height;

                height += imageHeight;
                
                height += margin;
            }

            commentLButtonPosition.y = likeLButtonPosition.y = height;
            height += lineWidth;
            height += bottomButtonHeight;
            return height;
        }

		public void SetPosition(Rect position, Rect scrollViewRect)
		{				
			this.position = position;

            this.scrollViewRect = scrollViewRect;
		}

        public Rect GetPosition()
        {
            return new Rect(scrollViewRect.x + position.x, scrollViewRect.y + position.y, position.width, position.height);            
        }

        public float GetScaleFactor()
        {
            return scaleFactor;
        }

        public Rect GetMenuButtonPosition()
        {
            return menuButtonPosition;
        }
		
        public void PopUpCanceled()
        {
            HasPopUp = false;
        }

        void OnCompleteAlphaDelete()
        {
            cardAlpha = 0.0f;

            tweenHeight = GetHeight();

            iTween.ValueTo(this.gameObject, iTween.Hash("from", tweenHeight, "to", 0.0f, "time", imageTweenTime * 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateCardHeight", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteCardHeight"));

            heightTweening = true;
        }

        private bool heightTweening;

        private float tweenHeight;

		private bool deleteThreadCardCoroutineDone;
		
        private bool deleteAnimationDone;

        void OnUpdateCardHeight(float value)
        {
            tweenHeight = value;
        }

        void OnCompleteCardHeight()
        {
            heightTweening = false;

			deleteAnimationDone = true;

			if(deleteThreadCardCoroutineDone)
                Forum.DeleteThread(this);
        }

        public void DeleteThread(bool backToForum)
        {
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });

				return;
			}

            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Delete"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("ConfirmDeleteThread"), delegate(bool del)
            {
                if (del)
                {
					deleteThreadCardCoroutineDone = false;

					iTween.ValueTo(this.gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateCardAlpha", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteAlphaDelete"));

					StartCoroutine(DeleteThreadCoroutine());

					if(backToForum)
	                    frameThread.BackToForum();
				}
            });
        }

		private IEnumerator DeleteThreadCoroutine()
		{
            while (string.IsNullOrEmpty(Thread.Id)) yield return 1;

            FASForum.DeleteThread(Thread.Id, delegate(Fresvii.AppSteroid.Models.Error error)
            {
				if(error != null)
				{
                    Forum.LoadLatestThreads();

					Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });
				}

				if(deleteAnimationDone)
                    Forum.DeleteThread(this);
				else
					deleteThreadCardCoroutineDone = true;

			});
		}

		public void DeleteThreadNonConfirm()
        {
			deleteThreadCardCoroutineDone = true;

			iTween.ValueTo(this.gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateCardAlpha", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteAlphaDelete"));

            frameThread.BackToForum();

        }

        public void Subscribe()
        {
            StartCoroutine(SubscribeCoroutine());
        }

        private IEnumerator SubscribeCoroutine()
        {
            while (string.IsNullOrEmpty(Thread.Id)) yield return 1;

            FASForum.Subscribe(Thread.Id, delegate(Fresvii.AppSteroid.Models.Thread _thread, Fresvii.AppSteroid.Models.Error _error)
            {
                if (_error == null)
                {
                    this.Thread = _thread;
                }
                else
                {
					Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });
				}
            });            
        }

        public void Unsubscribe()
        {
            StartCoroutine(UnsubscribeCoroutine());
        }

        private IEnumerator UnsubscribeCoroutine()
        {
            while (string.IsNullOrEmpty(Thread.Id)) yield return 1;

            FASForum.Unsubscribe(Thread.Id, delegate(Fresvii.AppSteroid.Models.Error error)
            {
                if (error == null)
                {
                    Thread.SetSubscribeParameter(false);
                }
                else
                {
					Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });
				}
            });
        }

        private bool commenButtonHitting;

        private bool clipImageLoaded;

        public bool IsVideo = false;

        void ClipImageLoad()
        {
            clipImageLoading = true;

            string url = "";

            if (Thread.Comment.VideoState == Fresvii.AppSteroid.Models.Comment.VideoStatus.Removed)
            {
                url = "";

                IsVideo = true;

                clipImage = textureRemovedVideo;

                clipImageLoading = false;

                iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateAlpha", "oncompletetarget", this.gameObject));

                return;
            }
            else if (!string.IsNullOrEmpty(Thread.Comment.ImageThumbnailUrl))
            {
                url = Thread.Comment.ImageThumbnailUrl;

                IsVideo = false;
            }
            else if(Thread.Comment.Video != null && Thread.Comment.VideoState == Fresvii.AppSteroid.Models.Comment.VideoStatus.Ready)
            {
                url = Thread.Comment.Video.ThumbnailUrl;

                IsVideo = true;
            }

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(url, true, delegate(Texture2D tex)
            {
                clipImage = tex;

                iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateAlpha", "oncompletetarget", this.gameObject));

                clipImageLoading = false;
            });
        }

        private void OnUpdateAlpha(float alpha)
        {
            clipAlpha = alpha;
        }

        public void GoToThreadByNotification()
        {
            if (frameThread != null)
            {
                Destroy(frameThread.gameObject);
            }

            frameThread = ((GameObject)Instantiate(prfbGUIFrameThread)).GetComponent<FresviiGUIThread>();

            frameThread.SetThread(this);

            frameThread.Init(null, postFix, scaleFactor, Forum.GuiDepth);

            frameThread.SetScrollPositionLast = true;

            frameThread.transform.parent = this.transform;

            Forum.GoToThread(frameThread, false);
        }

        private void LoadUserIcon()
        {
            userIconLoading = true;

            userProfileUrl = Thread.User.ProfileImageUrl;

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(Thread.User.ProfileImageUrl, true, delegate(Texture2D texture)
            {
                userIcon = texture;

                userIconLoading = false;
            });
        }

        private float preCardWidth;

        public Texture2D textureRemovedVideo;

		public void Draw(float cardWidth)
        {
			if(deleteAnimationDone) return;

			if(Thread.User != null)
			{
	            if (!string.IsNullOrEmpty(Thread.User.ProfileImageUrl) && userIcon == null && !userIconLoading)
	            {
	                LoadUserIcon();
	            }
	            else if (userProfileUrl != Thread.User.ProfileImageUrl && !userIconLoading)
	            {
	                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(userProfileUrl);

	                LoadUserIcon();
	            }
			}

            menuButtonPosition = new Rect(cardWidth - menuButtonMargin - FresviiGUIForum.TexForumMenu.width, menuButtonMargin, FresviiGUIForum.TexForumMenu.width, FresviiGUIForum.TexForumMenu.height);

            menuButtonHitPosition = new Rect(cardWidth - 44f * scaleFactor, 0f, 44f * scaleFactor, 44f * scaleFactor);

            userNamePosition.width = cardWidth - userNamePosition.x - menuButtonHitPosition.width;

            SetUserName();

			Rect drawPosition = GetPosition();

            if (drawPosition.y + drawPosition.height < 0 || drawPosition.y > Screen.height)
            {
			    return;
            }

            Event e = Event.current;
            
            Color origColor = GUI.color;

            GUI.color = new Color(origColor.r, origColor.g, origColor.b, cardAlpha);

            if ((!string.IsNullOrEmpty(Thread.Comment.ImageThumbnailUrl) || Thread.Comment.VideoState != Fresvii.AppSteroid.Models.Comment.VideoStatus.NA) && clipImage == null && !clipImageLoading)
            {
                ClipImageLoad();
            }

            //  Update proc
			if(Thread.User != null)
			{
	            if (contentUserName.text != Thread.User.Name || preCardWidth != cardWidth)
	            {
	                SetUserName();
	            }
			}

            if (contentCommentText.text != Thread.Comment.Text || preCardWidth != cardWidth)
            {
                SetCommentText(cardWidth);
            }

			GUI.BeginGroup(position);

			//	background
            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), palette, texCoordsLine);

            GUI.DrawTextureWithTexCoords(new Rect(position.width * 0.5f - lineWidth * 0.5f, position.height - bottomButtonHeight, lineWidth, bottomButtonHeight), palette, texCoordsLine);

            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height - bottomButtonHeight - lineWidth), palette, texCoordsBackground);

            GUI.DrawTextureWithTexCoords(new Rect(0f, position.height - bottomButtonHeight, position.width, bottomButtonHeight), palette, texCoordsBackground);

            if(buttonUserIcon.IsTap(e, userIconHitPosition))
            //if (buttonUserIcon.IsTap(e, userIconPosition, userIconHitPosition, FresviiGUIButton.ButtonType.TextureOnly, textureUserIcon, textureUserIcon, textureUserIcon))
            {
				if(Application.internetReachability == NetworkReachability.NotReachable){

					Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("OK"),delegate(bool del) { });
				
					return;
				}

                if (frameProfile == null)
                {
                    if (Thread.User.Id == FAS.CurrentUser.Id)
                    {
                        frameProfile = ((GameObject)Instantiate(prfbGUIFrameMyProfile)).GetComponent<FresviiGUIFrame>();
                    }
                    else
                    {
                        frameProfile = ((GameObject)Instantiate(prfbGUIFrameUserProfile)).GetComponent<FresviiGUIFrame>();

                        frameProfile.gameObject.GetComponent<FresviiGUIUserProfile>().SetUser(Thread.User);
                    }

                    frameProfile.Init(null, postFix, scaleFactor, Forum.GuiDepth);

                    frameProfile.transform.parent = this.transform;
                 }

                Forum.GoToUserProfile(frameProfile);
            }

            //  User Name
            GUI.Label(userNamePosition, contentUserName, guiStyleUserName);
            
			// Menu button
            if(buttonMenu.IsTap(e, menuButtonPosition, menuButtonHitPosition, FresviiGUIButton.ButtonType.TextureOnly, FresviiGUIForum.TexForumMenu, FresviiGUIForum.TexForumMenu, FresviiGUIForum.TexForumMenuH))
            {  
                List<string> buttons = new List<string>();
                
                buttons.Add((IsSubscribed()) ? FresviiGUIText.Get("Unsubscribe") : FresviiGUIText.Get("Subscribe"));

                if(IsMine())
                    buttons.Add(FresviiGUIText.Get("Delete"));

                popUpBaloonMenu = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/PopUpBalloonMenu"))).GetComponent<Fresvii.AppSteroid.Gui.PopUpBalloonMenu>();

                Vector2 popupBalloonPosition = popUpOffset + new Vector2(scrollViewRect.x + menuButtonPosition.x, scrollViewRect.y - Forum.forumTopMenu.height + menuButtonPosition.y) + FresviiGUIFrame.OffsetPosition;

                popUpBaloonMenu.Show(buttons.ToArray(), popupBalloonPosition, scaleFactor, postFix, Forum.GuiDepth - 10, Color.black, Color.white, Color.white, delegate(string selectedButton)
                {
                    if (selectedButton == FresviiGUIText.Get("Unsubscribe"))
                    {
                        this.Unsubscribe();
                    }
                    else if (selectedButton == FresviiGUIText.Get("Subscribe"))
                    {
                        this.Subscribe();
                    }
                    else if (selectedButton == FresviiGUIText.Get("Delete"))
                    {
                        this.DeleteThread(false);
                    }
                });
			}

            if (popUpBaloonMenu != null)
            {
                popUpBaloonMenu.SetPosition(popUpOffset + new Vector2(scrollViewRect.x + position.x + menuButtonPosition.x, scrollViewRect.y - Forum.forumTopMenu.height + position.y + menuButtonPosition.y + FresviiGUIFrame.OffsetPosition.y));
            }

            //  -----------------------
            System.TimeSpan ts = Thread.CreateTimeSpan();

            GUIContent contentTimeSpan;

            if (ts.Days > 1)
                contentTimeSpan = new GUIContent(ts.Days + " " + FresviiGUIText.Get("days") + FresviiGUIText.Get("ago"));
            else if (ts.Days > 0)
                contentTimeSpan = new GUIContent(ts.Days + " " + FresviiGUIText.Get("day") + FresviiGUIText.Get("ago"));
            else if (ts.Hours > 1)
                contentTimeSpan = new GUIContent(ts.Hours + " " + FresviiGUIText.Get("hours") + FresviiGUIText.Get("ago"));
            else if (ts.Hours > 0)
                contentTimeSpan = new GUIContent(ts.Hours + " " + FresviiGUIText.Get("hour") + FresviiGUIText.Get("ago"));
            else if (ts.Minutes > 1)
                contentTimeSpan = new GUIContent(ts.Minutes + " " + FresviiGUIText.Get("minutes") + FresviiGUIText.Get("ago"));
            else if (ts.Minutes > 0)
                contentTimeSpan = new GUIContent(ts.Minutes + " " + FresviiGUIText.Get("minute") + FresviiGUIText.Get("ago"));
            else
                contentTimeSpan = new GUIContent(FresviiGUIText.Get("now"));

            timespanPosition.width = guiStyleTimeSpan.CalcSize(contentTimeSpan).x;

            GUI.Label(timespanPosition, contentTimeSpan, guiStyleTimeSpan);

            likeCountIconPosition.x = timespanPosition.x + timespanPosition.width + margin;

            GUI.DrawTexture(likeCountIconPosition, FresviiGUIForum.TexForumLikeS);

            likeCountLabelPosition.x = likeCountIconPosition.x + FresviiGUIForum.TexForumLikeS.width + miniMargin;

            GUIContent contentLabelLikeCount = new GUIContent(Thread.Comment.LikeCount.ToString());

            GUI.Label(likeCountLabelPosition, contentLabelLikeCount, guiStyleTimeSpan);

            commentCountIconPosition.x = likeCountLabelPosition.x + guiStyleTimeSpan.CalcSize(contentLabelLikeCount).x + margin;

            GUI.DrawTexture(commentCountIconPosition, FresviiGUIForum.TexForumCommentS);

            commentCountLabelPosition.x = commentCountIconPosition.x + FresviiGUIForum.TexForumCommentS.width + miniMargin;

            GUI.Label(commentCountLabelPosition, Thread.CommentCount.ToString(), guiStyleTimeSpan);

            //  -----------------------
            GUI.Label(commentTextPosition, contentCommentText, guiStyleCommentText);

            if (clipImage != null)
            {               
                Color tmpColor = GUI.color;

                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, Mathf.Min(cardAlpha, clipAlpha));

                clipImagePosition.x = drawPosition.width * 0.5f - imageHeight * 0.5f;
             
                clipImagePosition.width = clipImagePosition.height = imageHeight;

                GUI.DrawTexture(clipImagePosition, clipImage, ScaleMode.ScaleAndCrop);

                GUI.color = tmpColor;

				if (IsVideo && clipAlpha >= 0.9f && Thread.Comment.VideoState != Fresvii.AppSteroid.Models.Comment.VideoStatus.Removed)
                {
                    videoPlaybackIconPosition = new Rect(clipImagePosition.x + 0.4f * clipImagePosition.width, clipImagePosition.y + 0.4f * clipImagePosition.height, 0.2f * clipImagePosition.width, 0.2f * clipImagePosition.height);

                    GUI.DrawTexture(videoPlaybackIconPosition, FresviiGUIForum.TexVideoPlaybackIcon, ScaleMode.ScaleToFit);
                }

                if (dataUploading)
                {
                    progressBarPosition = new Rect(clipImagePosition.x, clipImagePosition.y + clipImagePosition.height - scaleFactor * 1.0f, clipImagePosition.width * dataUploadingProgress, scaleFactor * 1.0f);

                    GUI.DrawTextureWithTexCoords(progressBarPosition, palette, texCoodsProgressBar);
                }
            }

            // Line
            //GUI.DrawTextureWithTexCoords(new Rect(0, position.height - bottomButtonHeight - lineWidth, position.width, lineWidth), palette, texCoordsLine);
                       
            // Comment button
            if (!string.IsNullOrEmpty(Thread.Id))
            {
                Texture2D texComenntButton = FresviiGUIForum.TexForumCommentL;

                Rect commentButtonPosition = new Rect(position.width * 0.25f - texComenntButton.width * 0.5f, position.height - 0.5f * bottomButtonHeight - texComenntButton.height * 0.5f, texComenntButton.width, texComenntButton.height);
                
                Rect commentButtonHitPosition = new Rect(0f, position.height - bottomButtonHeight, position.width * 0.5f, bottomButtonHeight);

                if (buttonComment.IsTap(e, commentButtonPosition, commentButtonHitPosition, FresviiGUIButton.ButtonType.TextureOnly, FresviiGUIForum.TexForumCommentL, FresviiGUIForum.TexForumCommentL, FresviiGUIForum.TexForumCommentLH))
                {
                    if (frameThread == null)
                    {
                        frameThread = ((GameObject)Instantiate(prfbGUIFrameThread)).GetComponent<FresviiGUIThread>();

                        frameThread.SetThread(this);

                        frameThread.Init(null, postFix, scaleFactor, Forum.GuiDepth);

                        frameThread.transform.parent = this.transform;
                    }

                    Forum.GoToThread(frameThread, true);
                }
            }

            Texture2D texLikeButton = (isLike) ? FresviiGUIForum.TexForumLikeLH : FresviiGUIForum.TexForumLikeL;
            
            Rect likeButtonHitPosition = new Rect(position.width * 0.5f, position.height - bottomButtonHeight, position.width * 0.5f, bottomButtonHeight);
            
            Rect likeButtonPosition = new Rect(position.width * 0.75f - texLikeButton.width * 0.5f, position.height - 0.5f * bottomButtonHeight - texLikeButton.height * 0.5f, texLikeButton.width, texLikeButton.height);
         
            if(buttonLike.IsTap(e, likeButtonPosition, likeButtonHitPosition, FresviiGUIButton.ButtonType.TextureOnly, texLikeButton, texLikeButton, texLikeButton))
            {
                isLike = !isLike;

                if (isLike)
                {
                    Thread.Comment.LikeCount++;

                    StartCoroutine(ThreadLikeCoroutine());
                }
                else
                {
                    if (Thread.Comment.LikeCount > 0) Thread.Comment.LikeCount--;

                    StartCoroutine(ThreadUnlikeCoroutine());
                }


                if (frameThread != null)
                {
                    if (frameThread.Thread.Id == Thread.Id)
                    {
                        if (frameThread.cards.Count > 0)
                        {
                            frameThread.cards[0].isLike = isLike;
                            if (isLike)
                            {
                                frameThread.cards[0].Comment.LikeCount++;
                            }
                            else
                            {
                                if (frameThread.cards[0].Comment.LikeCount > 0)
                                {
                                    frameThread.cards[0].Comment.LikeCount--;
                                }
                            }
                        }
                    }
                }
            }

            // Line
            GUI.DrawTextureWithTexCoords(new Rect(position.width * 0.5f - lineWidth * 0.5f, position.height - bottomButtonHeight, lineWidth, bottomButtonHeight), palette, texCoordsLine);

            if (buttonCard.IsTap(e, new Rect(0, 0, position.width, position.height)) && !string.IsNullOrEmpty(Thread.Id))
            {
				if(Application.internetReachability == NetworkReachability.NotReachable){
					
					Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("OK"),delegate(bool del) { });
					
					return;
				}

                if (frameThread == null)
                {
                    frameThread = ((GameObject)Instantiate(prfbGUIFrameThread)).GetComponent<FresviiGUIThread>();

                    frameThread.SetThread(this);

                    frameThread.Init(null, postFix, scaleFactor, Forum.GuiDepth);

                    frameThread.transform.parent = this.transform;
                }
                else
                {
                    frameThread.gameObject.SetActive(true);
                }

                Forum.GoToThread(frameThread, true);
            }

            //  User Icon
            if (Event.current.type == EventType.Repaint)
            {
                userIconMaterial.color = new Color(1f, 1f, 1f, cardAlpha);

                Graphics.DrawTexture(userIconPosition, ((userIcon == null) ? userIconDefault : userIcon), userIconMaterial);
            }


            GUI.EndGroup();

            GUI.color = origColor;

            preCardWidth = cardWidth;
        }

        IEnumerator ThreadLikeCoroutine()
        {
            while (string.IsNullOrEmpty(Thread.Id)) yield return 1;

            FASForum.LikeComment(Thread.Comment.Id, delegate(Fresvii.AppSteroid.Models.Comment _comment, Fresvii.AppSteroid.Models.Error _error)
            {
                if (_error != null)
                {
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        Debug.LogError(_error.ToString());
                }
            });

            if (Thread.Comment.Video != null)
            {
                FASVideo.LikeVideo(Thread.Comment.Video.Id, (video, error)=>
                {
                    if (error != null)
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                            Debug.LogError(error.ToString());
                    }      
                });
            }
        }

        IEnumerator ThreadUnlikeCoroutine()
        {
            while (string.IsNullOrEmpty(Thread.Id)) yield return 1;

            FASForum.UnlikeComment(Thread.Comment.Id, delegate(Fresvii.AppSteroid.Models.Error _error)
            {
                if (_error != null)
                {
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        Debug.LogError(_error.ToString());
                }
            });

            if (Thread.Comment.Video != null)
            {
                FASVideo.UnlikeVideo(Thread.Comment.Video.Id, (video, error) => {

                    if (error != null)
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                            Debug.LogError(error.ToString());
                    }
                });
            }

        }

		void OnDestroy()
        {
            if (clipImage != null)
            {
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(Thread.Comment.ImageThumbnailUrl);

                if (Thread.Comment.Video != null)
                    FresviiGUIManager.Instance.resourceManager.ReleaseTexture(Thread.Comment.Video.ThumbnailUrl);
            }
    
            if(userIcon != null)
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(userProfileUrl);
		}

        public void DataUploadProgressStart()
        {
            dataUploading = true;

            dataUploadingProgress = 0.0f;
        }

        public void DataUploadProgress(float progress)
        {
            dataUploadingProgress = progress;
        }

        public void DataUploadProgressEnd()
        {
            dataUploading = false;

            dataUploadingProgress = 0.0f;
        }
    }
}