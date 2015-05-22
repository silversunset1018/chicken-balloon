using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUICommentCard : MonoBehaviour
    {
        private float imageHeight = 280;

        private bool userIconLoading = false;

        private bool clipImageLoading = false;

        //  public
        private Texture palette;
        private Rect texCoordsBackground;

        public Rect userIconPosition;
        public Texture2D userIconDefault;
		public Texture2D userIconMask;
		private Texture2D userIcon;

        public Rect userNamePosition;
        public GUIStyle guiStyleUserName;

        public Rect timespanPosition;
        public GUIStyle guiStyleTimeSpan;

        public Rect likeCountIconPosition;
        public Rect likeCountLabelPosition;

        public Rect commentTextPosition;
        public GUIStyle guiStyleCommentText;

        public float miniMargin = 4;
        public float margin = 12;

        //  private
        public Fresvii.AppSteroid.Models.Comment Comment { get;  set;}

        private float scaleFactor;

        private bool imageLoaded;

        private GUIContent contentCommentText;

        public Rect clipImagePosition;

        private Rect videoPlaybackIconPosition;

        private Texture2D clipImage;
        
        public Texture2D ClipImage 
        {
            get
            {
                iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateAlpha", "oncompletetarget", this.gameObject));

                return clipImage;
            } 
            set
            {
                clipImage = value;
            }
        }

		public float imageTweenTime = 0.5f;

		public iTween.EaseType imageTweenEasetype = iTween.EaseType.easeOutExpo;
		
		private float clipAlpha = 0.0f;
		
        private float cardAlpha = 1.0f;

        [HideInInspector]
        public bool isLike = false;

        private FresviiGUIThread frameThread;

        private GUIContent contentUserName;

        private Texture2D likeButtonTexture;
        private Rect likeButtonPosition;
        private Rect likeButtonHitPositon;

        public FresviiGUIButton buttonUserIcon;
        public FresviiGUIButton buttonLike;
        public FresviiGUIButton buttonImage;

        private string userProfileUrl;

        public GameObject prfbGUIFrameUserProfile;
        public GameObject prfbGUIFrameMyProfile;
        
        [HideInInspector]
        public FresviiGUIFrame frameProfile;

        public GameObject prfbImageViewer;
        private FresviiGUIImageViewer imageViewer;

        private Rect progressBarPosition;

        private Rect texCoodsProgressBar;

        public Material userIconMaskMat;

        private string showingText;

        public Color linkTextColor;
        
        public void Init(Fresvii.AppSteroid.Models.Comment comment, float scaleFactor, Fresvii.AppSteroid.Models.Thread thread, float cardWidth, FresviiGUIThread frameThread)
        {
			cardAlpha = 0.0f;

            this.Comment = comment;
            this.scaleFactor = scaleFactor;
            this.frameThread = frameThread;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleUserName.font = null;
                guiStyleUserName.fontStyle = FontStyle.Bold;
                guiStyleTimeSpan.font = null;
                guiStyleCommentText.font = null;

				likeCountIconPosition.y += scaleFactor;
            }

            userIconPosition = FresviiGUIUtility.RectScale(userIconPosition, scaleFactor);            
            userNamePosition = FresviiGUIUtility.RectScale(userNamePosition, scaleFactor);
            timespanPosition = FresviiGUIUtility.RectScale(timespanPosition, scaleFactor);
            commentTextPosition = FresviiGUIUtility.RectScale(commentTextPosition, scaleFactor);
            likeCountIconPosition = FresviiGUIUtility.RectScale(likeCountIconPosition, scaleFactor);
            likeCountLabelPosition = FresviiGUIUtility.RectScale(likeCountLabelPosition, scaleFactor);
            clipImagePosition = FresviiGUIUtility.RectScale(clipImagePosition, scaleFactor);

            margin *= scaleFactor;
            
            miniMargin *= scaleFactor;
            
            guiStyleUserName.fontSize = (int)(guiStyleUserName.fontSize * scaleFactor);

            guiStyleUserName.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            guiStyleTimeSpan.fontSize = (int)(guiStyleTimeSpan.fontSize * scaleFactor);

            guiStyleTimeSpan.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText2);
            
            guiStyleCommentText.fontSize = (int)(guiStyleCommentText.fontSize * scaleFactor);

            guiStyleCommentText.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText1);
            
            imageHeight *= scaleFactor;
            
            isLike = comment.Like;

            likeButtonTexture = (isLike) ? FresviiGUIForum.TexForumLikeMH : FresviiGUIForum.TexForumLikeM;

            commentTextPosition.x = margin;
            
            commentTextPosition.width = cardWidth - 2f * margin;
            
            contentCommentText = new GUIContent(comment.Text);
            
            commentTextPosition.height = guiStyleCommentText.CalcHeight(contentCommentText, commentTextPosition.width);

            palette = FresviiGUIColorPalette.Palette;

            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            texCoodsProgressBar = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardProgressBar);

            likeButtonPosition = new Rect(cardWidth - margin - likeButtonTexture.width, margin, likeButtonTexture.width, likeButtonTexture.height);
        
            likeButtonHitPositon = new Rect(cardWidth - 2f * margin - likeButtonTexture.width, 0, likeButtonTexture.width + 2f * margin, likeButtonTexture.height + 2f * margin);

            userNamePosition.width = cardWidth - userNamePosition.x - likeButtonPosition.x - margin;

            if (comment.User == null)
            {
                contentUserName = new GUIContent("");
            }
            else
            {
                contentUserName = new GUIContent(FresviiGUIUtility.Truncate(comment.User.Name, guiStyleUserName, userNamePosition.width, "..."));
            }

			iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", imageTweenTime, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateCardAlpha", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteAlpha"));

            if (clipImage != null)
                iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateAlpha", "oncompletetarget", this.gameObject));
       
        }
		
		private void OnCompleteAlpha(){

		}

		private void OnUpdateCardAlpha(float alpha){
			cardAlpha = alpha;
		}

        private void OnUpdateAlpha(float alpha)
        {
            clipAlpha = alpha;
        }

        public string IncludingUrl;

        public float GetHeight()
        {
            if (deleteAnimation)
                return deleteAnimationHeight;

            if (showingText != Comment.Text)
            {
                showingText = Comment.Text;

                List<string> urls = FresviiGUIUtility.GetUrls(Comment.Text);

                string coloredText = Comment.Text;

                string hex = FresviiGUIUtility.ColorToHex(linkTextColor);

                foreach (string url in urls)
                {
                    coloredText = coloredText.Insert(coloredText.IndexOf(url), "<color=" + hex + ">");

                    coloredText = coloredText.Insert(coloredText.IndexOf(url) + url.Length, "</color>");

                    IncludingUrl = url;
                }

                contentCommentText = new GUIContent(coloredText);

                commentTextPosition.height = guiStyleCommentText.CalcHeight(contentCommentText, commentTextPosition.width);
            }

            float height = 0.0f;

            height += margin;
            
            height += userIconPosition.height;
            
            height += margin;

			if(!string.IsNullOrEmpty(contentCommentText.text)){
                height += commentTextPosition.height;
                height += margin + 1.0f * scaleFactor;
			}
            else
            {
                height += 1.0f * scaleFactor;
            }


			if (!string.IsNullOrEmpty(Comment.ImageThumbnailUrl) || (Comment.VideoState != Fresvii.AppSteroid.Models.Comment.VideoStatus.NA) || clipImage != null)
            {
                clipImagePosition.y = height;

                height += margin;
                
                height += imageHeight;
            }

            height += margin;

            return height;

        }

        private bool clipImageLoaded;

        private string clipImageUrl;

        public bool IsVideo = false;

        void ClipImageLoad()
        {
            clipImageLoading = true;

            string url = "";

            if (Comment.VideoState == Fresvii.AppSteroid.Models.Comment.VideoStatus.Removed)
            {
                url = "";

                IsVideo = true;

                clipImage = textureRemovedVideo;

                clipImageLoading = false;

                iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateAlpha", "oncompletetarget", this.gameObject));

                return;
            }
            else if (!string.IsNullOrEmpty(Comment.ImageThumbnailUrl))
            {
                url = Comment.ImageThumbnailUrl;

                IsVideo = false;
            }
            else if (Comment.Video != null && Comment.VideoState == Fresvii.AppSteroid.Models.Comment.VideoStatus.Ready)
            {
                url = Comment.Video.ThumbnailUrl;

                IsVideo = true;
            }


            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(url, true, delegate(Texture2D tex)
            {
                clipImage = tex;

                iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateAlpha", "oncompletetarget", this.gameObject));

                clipImageLoading = false;

                clipImageUrl = url;
            });
        }

        void OnEnable()
        {
            userIconLoading = false;
        }

        private void LoadUserIcon()
        {
            userIconLoading = true;

            userProfileUrl = Comment.User.ProfileImageUrl;

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(Comment.User.ProfileImageUrl, true, delegate(Texture2D texture)
            {
                userIcon = texture;

                userIconLoading = false;

            });
        }

        public Texture2D textureRemovedVideo;

        public void Draw(Rect position, Rect scrollViewRect, float cardWidth)
        {            
			Rect drawPosition = new Rect(position.x, scrollViewRect.y + position.y, position.width, position.height);

			if(Comment.User != null)
			{
	            if (!string.IsNullOrEmpty(Comment.User.ProfileImageUrl) && userIcon == null && !userIconLoading)
	            {
	                LoadUserIcon();
	            }
	            else if (userProfileUrl != Comment.User.ProfileImageUrl && !userIconLoading)
	            {
	                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(userProfileUrl);

	                LoadUserIcon();
	            }
			}

			if(drawPosition.y + drawPosition.height < 0 || drawPosition.y > Screen.height) return;

            commentTextPosition.width = cardWidth - 2f * margin;
            
            commentTextPosition.height = guiStyleCommentText.CalcHeight(contentCommentText, commentTextPosition.width);

            likeButtonPosition = new Rect(cardWidth - margin - likeButtonTexture.width, margin, likeButtonTexture.width, likeButtonTexture.height);

            likeButtonHitPositon = new Rect(cardWidth - 2f * margin - likeButtonTexture.width, 0, likeButtonTexture.width + 2f * margin, likeButtonTexture.height + 2f * margin);

            userNamePosition.width = cardWidth - userNamePosition.x - margin - likeButtonPosition.width - margin;

            if (Comment.User == null)
            {
                contentUserName = new GUIContent("");
            }
            else
            {
                contentUserName = new GUIContent(FresviiGUIUtility.Truncate(Comment.User.Name, guiStyleUserName, userNamePosition.width, "..."));
            }


            Color origColor = GUI.color;

            GUI.color = new Color(origColor.r, origColor.g, origColor.b, cardAlpha);

            if ((!string.IsNullOrEmpty(Comment.ImageThumbnailUrl) || Comment.VideoState != Fresvii.AppSteroid.Models.Comment.VideoStatus.NA) && clipImage == null && !clipImageLoading)
            {
                ClipImageLoad();
            }
			else if (!string.IsNullOrEmpty(Comment.ImageThumbnailUrl) && clipImageUrl != Comment.ImageThumbnailUrl && !clipImageLoading)
            {
                if (!string.IsNullOrEmpty(clipImageUrl))
                {
                    FresviiGUIManager.Instance.resourceManager.ReleaseTexture(clipImageUrl);
                }

				ClipImageLoad();
            }


            Event e = Event.current;

            //  Holding  - menu apper
            if (position.Contains(e.mousePosition) && FASGesture.IsHolding && !FASGesture.IsDragging && !frameThread.HasPopUp && !frameThread.ControlLock)
            {
                frameThread.ShowActionSheet(this);
            }                      

            GUI.BeginGroup(position);

            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), palette, texCoordsBackground);

            //  UserIcon -----------------------
            if (Event.current.type == EventType.Repaint)
            {
                userIconMaskMat.color = new Color(1f, 1f, 1f, cardAlpha);

                Graphics.DrawTexture(userIconPosition, (userIcon == null) ? userIconDefault : userIcon, userIconMaskMat);
            }

            if (buttonUserIcon.IsTap(e, userIconPosition))
            {
                if (frameProfile == null)
                {
                    if (Comment.User.Id == FAS.CurrentUser.Id)
                    {
                        frameProfile = ((GameObject)Instantiate(prfbGUIFrameMyProfile)).GetComponent<FresviiGUIFrame>();
                    }
                    else
                    {
                        frameProfile = ((GameObject)Instantiate(prfbGUIFrameUserProfile)).GetComponent<FresviiGUIFrame>();

                        frameProfile.gameObject.GetComponent<FresviiGUIUserProfile>().SetUser(Comment.User);
                    }

                    frameProfile.Init(null, FresviiGUIManager.postFix, scaleFactor, FASGui.GuiDepthBase);

                    frameProfile.transform.parent = this.transform;
                }

                frameThread.GoToUserProfile(frameProfile);
            }

            //  User name -----------------------
            GUI.Label(userNamePosition, contentUserName, guiStyleUserName);

            //  Time span -----------------------
            System.TimeSpan ts = Comment.CreateTimeSpan();

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
                contentTimeSpan = new GUIContent(ts.Minutes+ " " + FresviiGUIText.Get("minutes") + FresviiGUIText.Get("ago"));
            else if (ts.Minutes > 0)
				contentTimeSpan = new GUIContent(ts.Minutes + " " + FresviiGUIText.Get("minute") + FresviiGUIText.Get("ago"));
            else
                contentTimeSpan = new GUIContent(FresviiGUIText.Get("now"));

            timespanPosition.width = guiStyleTimeSpan.CalcSize(contentTimeSpan).x;


            GUI.Label(timespanPosition, contentTimeSpan, guiStyleTimeSpan);
           
            likeCountIconPosition.x = timespanPosition.x + timespanPosition.width + margin;

            GUI.DrawTexture(likeCountIconPosition, FresviiGUIForum.TexForumLikeS);

            likeCountLabelPosition.x = likeCountIconPosition.x + FresviiGUIForum.TexForumLikeS.width + miniMargin;

            GUIContent contentLabelLikeCount = new GUIContent(Comment.LikeCount.ToString());

            GUI.Label(likeCountLabelPosition, contentLabelLikeCount, guiStyleTimeSpan);
         
            GUI.Label(commentTextPosition, contentCommentText, guiStyleCommentText);

            if (clipImage != null)
            {
				Color tmpColor = GUI.color;

                if(!deleteAnimation)
    				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, Mathf.Min(cardAlpha, clipAlpha));

                clipImagePosition.x = drawPosition.width * 0.5f - imageHeight * 0.5f;
                
				clipImagePosition.width = clipImagePosition.height = imageHeight;
                
				GUI.DrawTexture(clipImagePosition, clipImage, ScaleMode.ScaleAndCrop);
                
				if(!deleteAnimation)
					GUI.color = tmpColor;

				if(!string.IsNullOrEmpty(Comment.ImageUrl) || (Comment.Video != null && !string.IsNullOrEmpty(Comment.Video.VideoUrl)))
				{
	                if(buttonImage.IsTap(e, clipImagePosition) && !frameThread.HasPopUp)
	                {
                        if (IsVideo)
                        {
                            if (Comment.VideoState == Fresvii.AppSteroid.Models.Comment.VideoStatus.Removed)
                            {
                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("VideoNone"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"), (del) => { });
                            }
                            else
                            {
                                FASVideo.Play(Comment.Video.VideoUrl);

                                FASVideo.IncrementVideoPlaybackCount(Comment.Video.Id, (video, error) =>
                                {
                                    if (error == null)
                                    {
                                        Comment.Video = video;
                                    }
                                });
                            }
                        }
                        else
                        {
                            imageViewer = (Instantiate(prfbImageViewer) as GameObject).GetComponent<FresviiGUIImageViewer>();

                            frameThread.ControlLock = true;

                            imageViewer.Show(Comment.ImageUrl, frameThread.GuiDepth - 5, delegate()
                            {
                                frameThread.ControlLock = false;
                            });
                        }
	                }
				}

				if (IsVideo && clipAlpha >= 0.9f && Comment.VideoState != Fresvii.AppSteroid.Models.Comment.VideoStatus.Removed)
                {
                    videoPlaybackIconPosition = new Rect(clipImagePosition.x + 0.4f * clipImagePosition.width, clipImagePosition.y + 0.4f * clipImagePosition.height, 0.2f * clipImagePosition.width, 0.2f * clipImagePosition.height);

                    GUI.DrawTexture(videoPlaybackIconPosition, FresviiGUIForum.TexVideoPlaybackIcon, ScaleMode.ScaleAndCrop);
                }

                if (dataUploading)
                {
                    progressBarPosition = new Rect(clipImagePosition.x, clipImagePosition.y + clipImagePosition.height - scaleFactor * 1.0f, clipImagePosition.width * dataUploadingProgress, scaleFactor * 1.0f);

                    GUI.DrawTextureWithTexCoords(progressBarPosition, palette, texCoodsProgressBar);
                }
            }

            likeButtonTexture = (isLike) ? FresviiGUIForum.TexForumLikeMH : FresviiGUIForum.TexForumLikeM;

            if(buttonLike.IsTap(e, likeButtonPosition, likeButtonHitPositon, FresviiGUIButton.ButtonType.TextureOnly, likeButtonTexture, likeButtonTexture, likeButtonTexture))
			{
                if (isLike)
                {
                    StartCoroutine(CommentUnlikeCoroutine());

                    if(Comment.LikeCount > 0)
                        Comment.LikeCount--;
                }
                else
                {
                    StartCoroutine(CommentLikeCoroutine());
                    
                    Comment.LikeCount++;
                }
                    
                isLike = !isLike;

                if (Comment.Id == frameThread.threadCard.Thread.Comment.Id)
                {
                    frameThread.threadCard.SetLike(isLike);
                    
                    if (isLike)
                    {
                        frameThread.threadCard.Thread.Comment.LikeCount++;
                    }
                    else
                    {
                        if (frameThread.threadCard.Thread.Comment.LikeCount > 0)
                           frameThread.threadCard.Thread.Comment.LikeCount--;
                    }
                }
			}

            GUI.EndGroup();

            GUI.color = origColor;
        }

        IEnumerator CommentLikeCoroutine()
        {
            while (string.IsNullOrEmpty(Comment.Id)) yield return 1;

            FASForum.LikeComment(Comment.Id, OnLikeComment);

            if (Comment.Video != null)
            {
                FASVideo.LikeVideo(Comment.Video.Id, (video, error) => 
                {
                    if (error != null)
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                            Debug.LogError(error.ToString());
                    }                
                });
            }
        }

        IEnumerator CommentUnlikeCoroutine()
        {
            while (string.IsNullOrEmpty(Comment.Id)) yield return 1;

            FASForum.UnlikeComment(Comment.Id, OnUnlikeComment);

            if (Comment.Video != null)
            {
                FASVideo.UnlikeVideo(Comment.Video.Id, (video, error) =>
                {
                    if (error != null)
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                            Debug.LogError(error.ToString());
                    }      
                });
            }
        }

		void OnLikeComment(Fresvii.AppSteroid.Models.Comment comment, Fresvii.AppSteroid.Models.Error error){
			
			if(error != null)
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
    				Debug.LogError(error.ToString());
				
                return;
			}

        }
		
		void OnUnlikeComment(Fresvii.AppSteroid.Models.Error error)
        {
			if(error != null)
			{
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
    				Debug.LogError(error.ToString());
	
                return;
			}
		}

        void OnDestroy()
        {
            if (clipImage != null)
            {
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(Comment.ImageThumbnailUrl);

                if(Comment.Video != null)
                    FresviiGUIManager.Instance.resourceManager.ReleaseTexture(Comment.Video.ThumbnailUrl);
            }
    
            if (userIcon != null)
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(userProfileUrl);
		}

        public bool IsMine()
        {
            return Comment.User.Id == FAS.CurrentUser.Id;
        }

        private bool deleteAnimation;
        private float deleteAnimationHeight;

        public void DeleteCard()
        {
            FASGesture.Stop();

            StartCoroutine(DeleteCommentCoroutine());

            cardAlpha = 1.0f;

            iTween.ValueTo(this.gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", imageTweenTime * 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateCardAlpha", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteAlphaDelete"));
        }

        private IEnumerator DeleteCommentCoroutine()
        {
            while (string.IsNullOrEmpty(Comment.Id))
            {
                yield return 1;
            }

            FASForum.DeleteComment(Comment.Id, delegate(Fresvii.AppSteroid.Models.Error error)
            {
                if (error != null)
                {
                    if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NotFound || (Application.platform == RuntimePlatform.Android && error.Detail.IndexOf("FileNotFound") >= 0))
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("ThreadNone"), delegate(bool del) { });

                        frameThread.threadCard.DeleteThreadNonConfirm();
                    }
                }
            });
        }

        void OnCompleteAlphaDelete()
        {
            deleteAnimationHeight = GetHeight();

            deleteAnimation = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("from", deleteAnimationHeight, "to", 0.0f, "time", imageTweenTime * 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateHeight", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteHeightDelete"));
        }

        void OnUpdateHeight(float value)
        {
            frameThread.OnScrollDelta(deleteAnimationHeight - value);

            deleteAnimationHeight = value;
        }

        void OnCompleteHeightDelete()
        {
            frameThread.RemoveCard(this);
        }

        private bool dataUploading;

        private float dataUploadingProgress = 0.0f;

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