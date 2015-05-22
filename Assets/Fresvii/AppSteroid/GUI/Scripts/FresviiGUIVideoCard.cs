using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIVideoCard : MonoBehaviour
    {
        //  public
        private Texture palette;

        private Rect texCoordsBackground;

        private Rect texCoordsProgressBar;
        
        public float cardHeight = 100f;

        private float sideMargin = 10f;

        public Rect videoThumbnailPosition;
        
        private Texture2D videoThumbnail;
        
        public Texture2D videoThumbnailDefault;
        
        public Texture2D videoThumbnailMask;

        public Rect videoTitlePosition;

        public Rect uplodedDateTimePosition;

        public Rect durationPosition;

        private Rect watchedIconPosition;

        private Rect watchedCountPosition;

        private Rect likeIconPosition;

        private Rect likeCountPositon;

        private Rect menuPosition;
        
        public GUIStyle guiStyleVideoTitle;

        public GUIStyle guiStyleUpdatedDateTime;

        private bool thumbnailLoaded;

        private Rect buttonPosition;

        public float imageTweenTime = 0.5f;

        public iTween.EaseType imageTweenEasetype = iTween.EaseType.easeOutExpo;

        private GUIContent contentVideoTitle;

        public Fresvii.AppSteroid.Models.Video Video { get; set; }

        public FresviiGUIButton buttonShare;

        private FresviiGUIVideoList parentFrame;

        private Rect videoPlaybackIconPositon;

        public Rect videoShareButtonPosition;

        public GUIStyle guiStyleButtonShare;

        private GUIContent contentUpdatedDateTime;

        private GUIContent contentDuration;

        private GUIContent contentPlaybackCount;

        private GUIContent contentLikeCount;
        
        private FresviiGUIVideoList.Mode mode;

        public float menuButtonMargin = 16;

        private Rect menuButtonPosition;
        
        private Rect menuButtonHitPosition;

        private Fresvii.AppSteroid.Gui.PopUpBalloonMenu popUpBaloonMenu;

        public bool HasPopUp { get; protected set; }

        public FresviiGUIButton buttonMenu;

        public Vector2 popUpOffset;

        public float videoInfoMargin = 8f;

        private Rect progressBarPosition;

        private float downloadingProgress;

        public void Init(Fresvii.AppSteroid.Models.Video video, float scaleFactor, FresviiGUIVideoList parentFrame, FresviiGUIVideoList.Mode mode)
        {
            this.Video = video;

            this.parentFrame = parentFrame;

            this.mode = mode;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleVideoTitle.font = null;

                guiStyleVideoTitle.fontStyle = FontStyle.Bold;

                guiStyleButtonShare.font = null;

                guiStyleUpdatedDateTime.font = null;
            }

            videoThumbnailPosition = FresviiGUIUtility.RectScale(videoThumbnailPosition, scaleFactor);

            videoTitlePosition = FresviiGUIUtility.RectScale(videoTitlePosition, scaleFactor);

            sideMargin *= scaleFactor;

            cardHeight *= scaleFactor;

            menuButtonMargin *= scaleFactor;

            popUpOffset *= scaleFactor;

            videoInfoMargin *= scaleFactor;

            guiStyleVideoTitle.fontSize = (int)(guiStyleVideoTitle.fontSize * scaleFactor);

            palette = FresviiGUIColorPalette.Palette;

            guiStyleVideoTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            guiStyleButtonShare.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            guiStyleButtonShare.fontSize = (int)(guiStyleButtonShare.fontSize * scaleFactor);

            guiStyleUpdatedDateTime.fontSize = (int)(guiStyleUpdatedDateTime.fontSize * scaleFactor);
            
            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            texCoordsProgressBar = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardProgressBar);

            videoThumbnail = videoThumbnailDefault;

            contentVideoTitle = new GUIContent(FresviiGUIText.Get("Title") + " " + video.Title);

            //videoPlaybackIconPositon = new Rect(videoThumbnailPosition.x + videoThumbnailPosition.width * 0.25f, videoThumbnailPosition.y + videoThumbnailPosition.height * 0.25f, videoThumbnailPosition.width * 0.5f, videoThumbnailPosition.height * 0.5f);

            videoPlaybackIconPositon = new Rect(videoThumbnailPosition.x + videoThumbnailPosition.width * 0.5f - 0.5f * FresviiGUIVideoList.TexVideoPlaybackIcon.width, videoThumbnailPosition.y + videoThumbnailPosition.height * 0.5f - 0.5f * FresviiGUIVideoList.TexVideoPlaybackIcon.height, FresviiGUIVideoList.TexVideoPlaybackIcon.width, FresviiGUIVideoList.TexVideoPlaybackIcon.height);
            
            videoShareButtonPosition = FresviiGUIUtility.RectScale(videoShareButtonPosition, scaleFactor);

            uplodedDateTimePosition = FresviiGUIUtility.RectScale(uplodedDateTimePosition, scaleFactor);

            uint min = video.Duration / 60;

            uint sec = video.Duration % 60;

            contentUpdatedDateTime = new GUIContent(FresviiGUIUtility.CurrentTimeSpan(video.CreatedAt));

            contentDuration = new GUIContent(min + ":" + sec.ToString("00"));

            Vector2 sizeTitle = guiStyleVideoTitle.CalcSize(contentVideoTitle);

            videoTitlePosition = new Rect(videoTitlePosition.x, videoTitlePosition.y, sizeTitle.x, sizeTitle.y);

            // 2 line

            Vector2 sizeDuration = guiStyleUpdatedDateTime.CalcSize(contentDuration);

            durationPosition = new Rect(uplodedDateTimePosition.x, uplodedDateTimePosition.y, sizeDuration.x, sizeDuration.y);

            Vector2 sizeUpdated = guiStyleUpdatedDateTime.CalcSize(contentUpdatedDateTime);

            uplodedDateTimePosition = new Rect(uplodedDateTimePosition.x + durationPosition.width + videoInfoMargin, uplodedDateTimePosition.y, sizeUpdated.x, sizeUpdated.y);

            // 3 line

            watchedIconPosition = new Rect(durationPosition.x, videoShareButtonPosition.y + videoShareButtonPosition.height * 0.5f - 0.5f * FresviiGUIVideoList.TexVideoEyeIcon.height, FresviiGUIVideoList.TexVideoEyeIcon.width, FresviiGUIVideoList.TexVideoEyeIcon.height);

            contentPlaybackCount = new GUIContent(Video.PlaybackCount.ToString());

            Vector2 sizePlaybackCount = guiStyleUpdatedDateTime.CalcSize(contentPlaybackCount);

            watchedCountPosition = new Rect(watchedIconPosition.x + watchedIconPosition.width + videoInfoMargin, videoShareButtonPosition.y + videoShareButtonPosition.height * 0.5f - 0.5f * sizePlaybackCount.y, sizePlaybackCount.x, sizePlaybackCount.y);

            likeIconPosition = new Rect(watchedCountPosition.x + watchedCountPosition.width + videoInfoMargin, videoShareButtonPosition.y + videoShareButtonPosition.height * 0.5f - 0.5f * FresviiGUIVideoList.TexVideoHeartIcon.height, FresviiGUIVideoList.TexVideoHeartIcon.width, FresviiGUIVideoList.TexVideoHeartIcon.height);

            contentLikeCount = new GUIContent(Video.LikeCount.ToString());

            Vector2 sizeLikeCount = guiStyleUpdatedDateTime.CalcSize(contentLikeCount);

            likeCountPositon = new Rect(likeIconPosition.x + likeIconPosition.width + videoInfoMargin, videoShareButtonPosition.y + videoShareButtonPosition.height * 0.5f - 0.5f * sizeLikeCount.y, sizeLikeCount.x, sizeLikeCount.y);

        }

		float postScreenWidth;

        private bool downloading = false;

        private void CalcLayout(float width)
        {
			if(Screen.width != postScreenWidth){

				menuButtonPosition = new Rect(width - menuButtonMargin - FresviiGUIVideoList.TexMenu.width, menuButtonMargin, FresviiGUIVideoList.TexMenu.width, FresviiGUIVideoList.TexMenu.height);

                menuButtonHitPosition = new Rect(width - 44f * FresviiGUIManager.scaleFactor, 0f, 44f * FresviiGUIManager.scaleFactor, 44f * FresviiGUIManager.scaleFactor);

				contentVideoTitle = new GUIContent( FresviiGUIUtility.Truncate( Video.Title, guiStyleVideoTitle, width - videoTitlePosition.x - menuButtonHitPosition.width, "..."));

                postScreenWidth = Screen.width;

                videoShareButtonPosition.x = width - videoShareButtonPosition.width - menuButtonMargin;

			}
        }

        public float GetHeight()
        {
            if (deleteAnimation)
            {
                return deleteAnimationHeight;
            }
            else
            {
                return cardHeight;
            }
        }

        bool videoThumbnailLoading;

        public Material videoThumbnailMaterial;

        private void LoadVideoThumbnail()
        {
            videoThumbnailLoading = true;

			Fresvii.AppSteroid.Util.ResourceManager.Instance.TextureFromCacheOrDownloadOrMemory(Video.ThumbnailUrl, true, delegate(Texture2D texture)
            {
                videoThumbnailLoading = false;

                videoThumbnail = texture;

                thumbnailLoaded = true;
            });
        }

		void OnDestroy()
		{
			if(isSelected) return;

			Fresvii.AppSteroid.Util.ResourceManager.Instance.ReleaseTexture(Video.ThumbnailUrl);
		}

        private float alpha = 1.0f;

		private bool isSelected;

        public void Draw(Rect position, Rect scrollViewRect, int guiDepth, float topMenuHeight)
        {
            Color tmpColor = GUI.color;

            GUI.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, alpha);

            if (!string.IsNullOrEmpty(Video.ThumbnailUrl) && !thumbnailLoaded && !videoThumbnailLoading)
            {
                LoadVideoThumbnail();
            }

            CalcLayout(position.width);

            GUI.BeginGroup(position);
            
            // Background
            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), palette, texCoordsBackground);

            // Progressbar
            if (downloading)
            {
                progressBarPosition = new Rect(0, position.height - FresviiGUIManager.Instance.ScaleFactor, position.width * downloadingProgress, FresviiGUIManager.Instance.ScaleFactor);

                GUI.DrawTextureWithTexCoords(progressBarPosition, palette, texCoordsProgressBar);                
            }

            if (Event.current.type == EventType.Repaint)
            {
                if (videoThumbnail != null)
                {
                    videoThumbnailMaterial.color = new Color(1f, 1f, 1f, alpha);

                    if(videoThumbnail.width > videoThumbnail.height)
                    {
                        Vector2 offset = new Vector2((float)(videoThumbnail.width - videoThumbnail.height) / (float)videoThumbnail.width * 0.5f, 0f);

                        Vector2 scale = new Vector2( 1.0f - offset.x * 2f, 1.0f);

                        videoThumbnailMaterial.mainTextureOffset = offset;

                        videoThumbnailMaterial.mainTextureScale = scale;
                    }
                    else
                    {
                        Vector2 offset = new Vector2(0f, (float)(videoThumbnail.height - videoThumbnail.width) /(float) videoThumbnail.height * 0.5f);

                        Vector2 scale = new Vector2(1.0f, 1.0f - offset.y * 2f);

                        videoThumbnailMaterial.mainTextureOffset = offset;

                        videoThumbnailMaterial.mainTextureScale = scale;
                    }                   

                    Graphics.DrawTexture(videoThumbnailPosition, videoThumbnail, videoThumbnailMaterial);

                    //GUI.DrawTexture(videoThumbnailPosition, videoThumbnail, ScaleMode.ScaleAndCrop);
                }
            }

            GUI.DrawTexture(videoPlaybackIconPositon, FresviiGUIVideoList.TexVideoPlaybackIcon, ScaleMode.StretchToFill);

            //  videoTitle -----------------------
            GUI.Label(videoTitlePosition, contentVideoTitle, guiStyleVideoTitle);

            GUI.Label(uplodedDateTimePosition, contentUpdatedDateTime, guiStyleUpdatedDateTime);

            GUI.Label(durationPosition, contentDuration, guiStyleUpdatedDateTime);

            GUI.DrawTexture(watchedIconPosition, FresviiGUIVideoList.TexVideoEyeIcon);

            GUI.Label(watchedCountPosition, contentPlaybackCount, guiStyleUpdatedDateTime);

            GUI.DrawTexture(likeIconPosition, FresviiGUIVideoList.TexVideoHeartIcon);

            GUI.Label(likeCountPositon, contentLikeCount, guiStyleUpdatedDateTime);

            if (buttonShare.IsTap(Event.current, videoShareButtonPosition, videoShareButtonPosition,
                    FresviiGUIButton.ButtonType.FrameAndLabel,
                    FresviiGUIVideoList.TexButtonShare,
                    FresviiGUIVideoList.TexButtonShareH,
                    FresviiGUIVideoList.TexButtonShareH,
                    (mode == FresviiGUIVideoList.Mode.Share || mode == FresviiGUIVideoList.Mode.FromUploded) ? FresviiGUIText.Get("Share") : FresviiGUIText.Get("Select"),
                    guiStyleButtonShare))
            {
				isSelected = true;

				if (mode == FresviiGUIVideoList.Mode.Share || mode == FresviiGUIVideoList.Mode.FromUploded)
                {
                    parentFrame.Share(Video, videoThumbnail);
                }
                else if (mode == FresviiGUIVideoList.Mode.Select)
                {
                    parentFrame.Select(Video, videoThumbnail);
                }
            }

            if (GUI.Button(videoThumbnailPosition, "", GUIStyle.none))
            {
                FASVideo.Play(Video.VideoUrl);

                FASVideo.IncrementVideoPlaybackCount(Video.Id, (video, error) => 
                {
                    if (error == null)
                    {
                        Video = video;
                    }
                });
            }

            // Menu button           
            if (parentFrame.IsMine)
            {
                if (buttonMenu.IsTap(Event.current, menuButtonPosition, menuButtonHitPosition, FresviiGUIButton.ButtonType.TextureOnly, FresviiGUIVideoList.TexMenu, FresviiGUIVideoList.TexMenu, FresviiGUIVideoList.TexMenuH))
                {
                    List<string> buttons = new List<string>();

                    if(!downloading)
                        buttons.Add(FresviiGUIText.Get("CopyToCameraRoll"));

                    buttons.Add(FresviiGUIText.Get("Delete"));

                    popUpBaloonMenu = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/PopUpBalloonMenu"))).GetComponent<Fresvii.AppSteroid.Gui.PopUpBalloonMenu>();

                    Vector2 popupBalloonPosition = popUpOffset + new Vector2(scrollViewRect.x + menuButtonPosition.x, scrollViewRect.y - topMenuHeight + menuButtonPosition.y) + FresviiGUIFrame.OffsetPosition;

                    popUpBaloonMenu.Show(buttons.ToArray(), popupBalloonPosition, FresviiGUIManager.scaleFactor, FresviiGUIManager.postFix, guiDepth - 10, Color.black, Color.white, Color.white, delegate(string selectedButton)
                    {
                        if (selectedButton == FresviiGUIText.Get("CopyToCameraRoll"))
                        {
                            downloading = true;

                            FASVideo.DownloadAndCopyToAlbum(Video.VideoUrl, () =>
                            {
                                downloading = false;
                            }
                            ,

                            (progress) =>
                            {
                                downloadingProgress = progress;
                            }

                            );
                        }
                        else if (selectedButton == FresviiGUIText.Get("Delete"))
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Delete"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("ConfirmDeleteVideo"), delegate(bool del)
                            {
                                if (del)
                                {
                                    parentFrame.DeleteVideo(this);
                                }
                            });
                        }
                    });
                }
            }

            if (popUpBaloonMenu != null)
            {
                popUpBaloonMenu.SetPosition(popUpOffset + new Vector2(scrollViewRect.x + position.x + menuButtonPosition.x, scrollViewRect.y - topMenuHeight + position.y + menuButtonPosition.y + FresviiGUIFrame.OffsetPosition.y));
            }

            GUI.EndGroup();

            GUI.color = tmpColor;
        }

        private bool deleteAnimation;

        private float deleteAnimationHeight;

        public void DeleteCardAnimation()
        {
            deleteAnimationHeight = GetHeight();

            deleteAnimation = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("from", alpha, "to", 0.0f, "time", 0.25f, "onupdate", "OnUpdateAlpha", "oncomplete", "OnCompleteAlpha"));
        }

        void OnUpdateAlpha(float value)
        {
            alpha = value;
        }

        void OnCompleteAlpha()
        {
            alpha = 0.0f;

            iTween.ValueTo(this.gameObject, iTween.Hash("from", deleteAnimationHeight, "to", 0.0f, "time", 0.25f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateHeight", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteHeightDelete"));
        }

        void OnUpdateHeight(float value)
        {
            parentFrame.OnScrollDelta(deleteAnimationHeight - value);

            deleteAnimationHeight = value;
        }

        void OnCompleteHeightDelete()
        {
            parentFrame.RemoveVideoCard(this);
        }
    }
}