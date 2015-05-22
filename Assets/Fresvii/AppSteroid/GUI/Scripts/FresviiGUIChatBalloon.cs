using UnityEngine;
using System.Collections;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIChatBalloon : MonoBehaviour
    {
        private static readonly float ImageHeight = 125f;

        private float imageHeight;

        public float imageMargin;

        private float height = 100f;

        public float sideSmallMargin;

        public float sideLargeMargin;

        public float centerMargin;

        public Vector2 userIconSize;

        public float userIconOffsetY;

        public float clipImageUserIconOffset = 22f;

        private Rect userIconPosition;

        public GUIStyle guiStyleText;

        public GUIStyle guiStyleTime;

        public GUIStyle guiStyleUserName;
        
        private float scaleFactor;
	
		private float clipAlpha = 1.0f;

        private float balloonAlpha = 1.0f;

        public Material userIconMask;

        private Fresvii.AppSteroid.Models.GroupMessage groupMessage;

        public Fresvii.AppSteroid.Models.GroupMessage GroupMessage {

            get
            {
                return groupMessage;
            }
            
            set
            {
                groupMessage = value;

                OnGroupMessageUpdated();
            ;} 
        }

        private FresviiGUIChat frameChat;

        private Texture2D userIcon;

        private Texture2D clipImage;

        private bool isMine;

        public Texture2D userIconDefault;

        private GUIContent guiContentText;

        private GUIContent guiContentTime;

        private GUIContent guiContentUserName;

        private Rect balloonPosition;

        private Rect balloonTrianglePosition;

        private Rect timePosition;

        private Rect userNamePosition;

        private string displayUserName = "";

        private Rect clipImagePosition;

        private Color chatBalloonColor;

		private float postWidth;

		public bool isDraw;

		public bool dateDraw;

		public float balloonWidthRate = 0.65f;

        public GameObject prfbImageViewer;

        private FresviiGUIImageViewer imageViewer;

        public FresviiGUIButton buttonImage;

        private Rect videoPlaybackIconPosition;

        public Texture2D textureRemovedVideo;

        public void Init(Fresvii.AppSteroid.Models.GroupMessage groupMessage, float scaleFactor, float width, FresviiGUIChat frameChat)
        {
            if (frameChat == null) return;
                       
            isMine = (FAS.CurrentUser.Id == groupMessage.User.Id);

            Initialize(groupMessage, scaleFactor, width, frameChat);

            if (groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Image)
            {
                LoadClipImage(groupMessage.ImageThumbnailUrl);
            }
            else if (groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Video)
            {
                if (groupMessage.VideoStatus == Fresvii.AppSteroid.Models.GroupMessage.VideoStatuses.Ready && groupMessage.Video != null && !string.IsNullOrEmpty(groupMessage.Video.ThumbnailUrl))
                {
                    LoadClipImage(groupMessage.Video.ThumbnailUrl);
                }
                else
                {
                    clipImage = textureRemovedVideo;
                }
            }
        }

        public void InitByLocal(Fresvii.AppSteroid.Models.GroupMessage groupMessage, float scaleFactor, float width, FresviiGUIChat frameChat, Texture2D _clipImage)
        {
            isMine = true;
            
            this.clipImage = _clipImage;

            Initialize(groupMessage, scaleFactor, width, frameChat);
        }

        private void Initialize(Fresvii.AppSteroid.Models.GroupMessage groupMessage, float scaleFactor, float width, FresviiGUIChat frameChat)
        {
            this.transform.parent = frameChat.transform;

            this.GroupMessage = groupMessage;

            this.frameChat = frameChat;

            this.scaleFactor = scaleFactor;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleText.font = null;

                guiStyleTime.font = null;

                guiStyleUserName.font = null;
            }

            sideSmallMargin *= this.scaleFactor;

            sideLargeMargin *= this.scaleFactor;

            centerMargin *= this.scaleFactor;

            userIconSize *= this.scaleFactor;

            clipImageUserIconOffset *= this.scaleFactor;

            height *= this.scaleFactor;

            imageHeight = ImageHeight * this.scaleFactor;

            imageMargin *= this.scaleFactor;

            userIconOffsetY *= this.scaleFactor;

            guiStyleText.fontSize = (int)(guiStyleText.fontSize * this.scaleFactor);

            guiStyleText.padding = FresviiGUIUtility.RectOffsetScale(guiStyleText.padding, this.scaleFactor);

            guiStyleText.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ChatBalloonText);

            guiStyleTime.fontSize = (int)(guiStyleTime.fontSize * this.scaleFactor);

            guiStyleTime.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ChatBalloonText);

            guiStyleUserName.fontSize = (int)(guiStyleUserName.fontSize * this.scaleFactor);

            guiStyleUserName.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ChatBalloonText);

            guiStyleUserName.padding = FresviiGUIUtility.RectOffsetScale(guiStyleUserName.padding, this.scaleFactor);

            guiContentText = new GUIContent(GroupMessage.Text);

            guiContentTime = new GUIContent(GroupMessage.CreatedAt.ToString("H:mm"));

            Vector2 timeSize = guiStyleTime.CalcSize(guiContentTime);

            texCoodsProgressBar = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardProgressBar);

            timePosition.width = timeSize.x;

            timePosition.height = timeSize.y;

            if (!string.IsNullOrEmpty(groupMessage.User.ProfileImageUrl))
                LoadUserIcon(GroupMessage.User.ProfileImageUrl);

            CalcLayout(width);

            balloonAlpha = 0.0f;

            chatBalloonColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ChatBalloon);

            iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.5f, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateBalloonAlpha", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteBalloonAlpha"));
        }

        private void OnGroupMessageUpdated()
        {
            guiContentText = new GUIContent(GroupMessage.Text);

            guiContentTime = new GUIContent(GroupMessage.CreatedAt.ToString("H:mm"));

            Vector2 timeSize = guiStyleTime.CalcSize(guiContentTime);

            timePosition.width = timeSize.x;

            timePosition.height = timeSize.y;

            postWidth = 0.0f;
        }

        private void OnUpdateBalloonAlpha(float alpha)
        {
            balloonAlpha = alpha;
        }

        private void OnCompleteBalloonAlpha()
        {
            balloonAlpha = 1.0f;
        }

        private void LoadUserIcon(string url)
        {
            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(url, true, delegate(Texture2D texture)
            {
                userIcon = texture;

                if (userIcon == null)
                    LoadUserIcon(url);
            });
        }

        private void LoadClipImage(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(url, true, delegate(Texture2D texture)
            {
                if (texture == null)
                {
                    LoadClipImage(url);
                }
                else
                {
                    clipImage = texture;

                    clipAlpha = 0.0f;

                    iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.5f, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateClipAlpha", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteClipAlpha"));
                }
            });
        }

        private void OnUpdateClipAlpha(float alpha)
        {
            clipAlpha = alpha;
        }

        private void OnCompleteClipAlpha()
        {
            clipAlpha = 1.0f;
        }

        public void CalcLayout(float width)
        {
			if(Screen.width != postWidth)
			{
                postWidth = Screen.width;

	            float balloonWidth = 0f, balloonHeight = 0f;

	            if (groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Text)
	            {
					balloonWidth = Mathf.Min(guiStyleText.CalcSize(guiContentText).x, width * balloonWidthRate);

	                height = balloonHeight =  Mathf.Max(guiStyleText.CalcHeight(guiContentText, balloonWidth), userIconSize.y);
	            }
                else if (groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Image || groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Video)
	            {
	                height = balloonWidth = balloonHeight = imageMargin * 2f + imageHeight;
	            }

	            if (isMine)
	            {
					balloonTrianglePosition = new Rect(width - sideSmallMargin, (height == userIconSize.y) ? 0.5f * height - frameChat.balloonTriangleTexture.height * 0.5f : height - userIconSize.y * 0.5f - frameChat.balloonTriangleTexture.height * 0.5f, frameChat.balloonTriangleTexture.width, frameChat.balloonTriangleTexture.height);

	                balloonPosition = new Rect(balloonTrianglePosition.x - balloonWidth, 0f, balloonWidth, balloonHeight);

	                timePosition = new Rect(balloonPosition.x - timePosition.width - sideSmallMargin * 0.5f, balloonPosition.y + balloonPosition.height - timePosition.height, timePosition.width, timePosition.height);
	            }
	            else
	            {
	                balloonPosition = new Rect(sideSmallMargin + userIconSize.x + centerMargin, 0f, balloonWidth, balloonHeight);

	                userIconPosition = new Rect(sideSmallMargin, balloonPosition.y + balloonPosition.height - userIconSize.y, userIconSize.x, userIconSize.y);

	                balloonTrianglePosition = new Rect(sideSmallMargin + userIconSize.x + centerMargin - frameChat.balloonTriangleTexture.width, userIconPosition.y + 0.5f * userIconPosition.height - frameChat.balloonTriangleTexture.height * 0.5f, frameChat.balloonTriangleTexture.width, frameChat.balloonTriangleTexture.height);

	                timePosition = new Rect(balloonPosition.x + balloonPosition.width + sideSmallMargin * 0.5f, balloonPosition.y + balloonPosition.height - timePosition.height, timePosition.width, timePosition.height);

					displayUserName = FresviiGUIUtility.Truncate(GroupMessage.User.Name, guiStyleUserName, width * balloonWidthRate, "...");

					userNamePosition = new Rect(balloonPosition.x, height, width * balloonWidthRate, guiStyleUserName.CalcHeight(new GUIContent(displayUserName), width * 0.6f));

	                height += userNamePosition.height;
	            }

                if (groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Image || groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Video)
	            {
	                clipImagePosition = new Rect(balloonPosition.x + balloonWidth * 0.5f - imageHeight * 0.5f, balloonPosition.y + imageMargin, imageHeight, imageHeight);

                    videoPlaybackIconPosition = new Rect(clipImagePosition.x + 0.4f * clipImagePosition.width, clipImagePosition.y + 0.4f * clipImagePosition.height, 0.2f * clipImagePosition.width, 0.2f * clipImagePosition.height);
	            }
			}
        }

        public float GetHeight()
        {
            return height;
        }

        public void Draw(Rect position)
        {
            Color tmpColor = GUI.color;

            GUI.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, balloonAlpha);

            CalcLayout(position.width);

            GUI.BeginGroup(position);

            GUI.color = new Color(chatBalloonColor.r, chatBalloonColor.g, chatBalloonColor.b, balloonAlpha);

            FresviiGUIUtility.DrawSplitTexture(balloonPosition, frameChat.balloonMatTexture, scaleFactor);

            if (isMine)
            {
                GUI.DrawTexture(balloonTrianglePosition, frameChat.balloonTriangleTexture);
            }
            else
            {
                Matrix4x4 matrixBackup = GUI.matrix;

                GUIUtility.RotateAroundPivot(180.0f, new Vector2(balloonTrianglePosition.x + balloonTrianglePosition.width * 0.5f, balloonTrianglePosition.y + balloonTrianglePosition.height * 0.5f));

                GUI.DrawTexture(balloonTrianglePosition, frameChat.balloonTriangleTexture);

                GUI.matrix = matrixBackup;
            }

            GUI.color = tmpColor;

            if (groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Image || groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Video)
            {
                if (clipImage != null)
                {
                    Color tmpClipColor = GUI.color;

                    GUI.color = new Color(tmpClipColor.r, tmpClipColor.g, tmpClipColor.b, clipAlpha);

                    GUI.DrawTexture(clipImagePosition, clipImage, ScaleMode.ScaleAndCrop);

                    GUI.color = tmpClipColor;

                    if (buttonImage.IsTap(Event.current, clipImagePosition))
                    {
                        if (groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Image)
                        {
                            imageViewer = (Instantiate(prfbImageViewer) as GameObject).GetComponent<FresviiGUIImageViewer>();

                            frameChat.ControlLock = true;

                            imageViewer.Show(groupMessage.ImageUrl, frameChat.GuiDepth - 5, delegate()
                            {
                                frameChat.ControlLock = false;
                            });
                        }
                        else
                        {
                            if (groupMessage.VideoStatus == Fresvii.AppSteroid.Models.GroupMessage.VideoStatuses.Removed)
                            {
                                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("OK", "Cancel", "close");

                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("VideoNone"), (del) => { });
                            }
                            else
                            {
                                FASVideo.Play(groupMessage.Video.VideoUrl);

                                FASVideo.IncrementVideoPlaybackCount(groupMessage.Video.Id, (video, error) =>
                                {
                                    if (error == null)
                                    {
                                        groupMessage.Video = video;
                                    }
                                });
                            }
                        }
                    }

					if(groupMessage.Type == Fresvii.AppSteroid.Models.GroupMessage.ContentType.Video)
					{
                        if (groupMessage.VideoStatus == Fresvii.AppSteroid.Models.GroupMessage.VideoStatuses.Ready)
                        {
                            GUI.DrawTexture(videoPlaybackIconPosition, frameChat.videoPlaybackIconTexture);
                        }
					}

                    if (dataUploading)
                    {
                        progressBarPosition = new Rect(clipImagePosition.x, clipImagePosition.y + clipImagePosition.height - scaleFactor * 1.0f, clipImagePosition.width * dataUploadingProgress, scaleFactor * 1.0f);

                        GUI.DrawTextureWithTexCoords(progressBarPosition, FresviiGUIColorPalette.Palette, texCoodsProgressBar);
                    }
                }
            }
            else
            {
                GUI.Label(balloonPosition, guiContentText, guiStyleText);
            }

            if (!isMine)
            {
                //  UserIcon -----------------------
                if (Event.current.type == EventType.Repaint)
                {
                    userIconMask.color = new Color(1f, 1f, 1f, balloonAlpha);

                    Graphics.DrawTexture(userIconPosition, ((userIcon == null) ? userIconDefault : userIcon), userIconMask);
                }

                GUI.Label(userNamePosition, displayUserName, guiStyleUserName);
            }

            GUI.Label(timePosition, guiContentTime, guiStyleTime);

            GUI.EndGroup();

            GUI.color = tmpColor;
        }

        void OnDestroy()
        {
            if (clipImage != null)
            {
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(GroupMessage.ImageThumbnailUrl);

                if (GroupMessage.Video != null)
                    FresviiGUIManager.Instance.resourceManager.ReleaseTexture(GroupMessage.Video.ThumbnailUrl);
            }
        }

        private bool dataUploading;

        private float dataUploadingProgress = 0.0f;

        private Rect progressBarPosition;

        private Rect texCoodsProgressBar;

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