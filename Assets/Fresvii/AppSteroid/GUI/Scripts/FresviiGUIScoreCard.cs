using UnityEngine;
using System.Collections;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIScoreCard : MonoBehaviour
    {
        private bool userIconLoading = false;

        private bool iconError = false;

        //  public
        private Texture palette;
        private Rect texCoordsBackground;

        public float cardHeight = 55f;

        private float sideMargin;

        public Rect medalPosition;

        public Rect rankPosition;

        public Rect userIconPosition;

		private Texture2D userIcon;
        
        public Texture2D userIconDefault;
        
        public Rect userNamePosition;

        public Rect scoreLabelPosition;

        public GUIStyle guiStyleRank;

        public GUIStyle guiStyleUserName;

        public GUIStyle guiStyleScore;

        private float scaleFactor;

        private bool imageLoaded;

		public float imageTweenTime = 0.5f;

		public iTween.EaseType imageTweenEasetype = iTween.EaseType.easeOutExpo;
		
        private GUIContent contentUserName;

        private string userProfileUrl;

        public Fresvii.AppSteroid.Models.Rank rank { get; set; }

        public FresviiGUIButton buttonCard;

        public GameObject prfbGUIFrameUserProfile;

        public GameObject prfbGUIFrameMyProfile;

        private FresviiGUIFrame parentFrameProfile;
        
        private FresviiGUIFrame nextFrameProfile;

        private FresviiGUILeaderboard frameLeaderboard;

        private Rect tagPosition;

        private enum TagKind { None, You, Friend }

        private TagKind tagKind = TagKind.None;

        public GUIStyle guiStyleTag;

        private string tagString = "";

        public Material userMask;

        public void Init(Fresvii.AppSteroid.Models.Rank rank, float scaleFactor, FresviiGUIFrame parentFrame)
        {
            this.rank = rank;

            this.parentFrameProfile = parentFrame;

            frameLeaderboard = parentFrame.gameObject.GetComponent<FresviiGUILeaderboard>();

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleUserName.font = null;

                guiStyleUserName.fontStyle = FontStyle.Bold;

                guiStyleRank.font = null;

                guiStyleRank.fontStyle = FontStyle.Bold;

                guiStyleScore.font = null;

                guiStyleTag.font = null;
            }

            rankPosition = FresviiGUIUtility.RectScale(rankPosition, scaleFactor);

            medalPosition = FresviiGUIUtility.RectScale(medalPosition, scaleFactor);

            userIconPosition = FresviiGUIUtility.RectScale(userIconPosition, scaleFactor);

            userNamePosition = FresviiGUIUtility.RectScale(userNamePosition, scaleFactor);

            scoreLabelPosition = FresviiGUIUtility.RectScale(scoreLabelPosition, scaleFactor);

            sideMargin = userIconPosition.x;

            cardHeight *= scaleFactor;

            guiStyleRank.fontSize = (int)(guiStyleRank.fontSize * scaleFactor);
            
            guiStyleUserName.fontSize = (int)(guiStyleUserName.fontSize * scaleFactor);

            guiStyleScore.fontSize = (int)(guiStyleScore.fontSize * scaleFactor);

            guiStyleTag.fontSize = (int)(guiStyleTag.fontSize * scaleFactor);

            guiStyleTag.padding = FresviiGUIUtility.RectOffsetScale(guiStyleTag.padding, scaleFactor);

            palette = FresviiGUIColorPalette.Palette;

            guiStyleUserName.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText1);

            guiStyleRank.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText1);

            guiStyleScore.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText2);

            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            if (rank.Score.User == null)
            {
                FASLeaderboard.GetScore(frameLeaderboard.Leaderboard.Id, rank.Score.Id, delegate(Fresvii.AppSteroid.Models.Score score, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error == null)
                    {
                        rank.Score = score;

                        SetTag();
                    }
                    else
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        {
                            Debug.LogError(error.ToString());
                        }
                    }
                });
            }
            else
            {
                SetTag();
            }
        }

        void SetTag()
        {
            if (rank.Score.User.Id == FAS.CurrentUser.Id)
            {
                tagKind = TagKind.You;

                guiStyleTag.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardBackground);

                tagString = FresviiGUIText.Get("You");
            }
            else if (rank.Score.User.FriendStatus == Fresvii.AppSteroid.Models.User.FriendStatuses.Friend)
            {
                tagKind = TagKind.Friend;

                guiStyleTag.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText1);

                tagString = FresviiGUIText.Get("Friend");
            }
        }

        void OnEnable()
        {
            userIconLoading = false;
        }

        private void CalcLayout(float width)
        {
            userNamePosition.width = width - userNamePosition.x - sideMargin;

            if (rank.Score.User.Id == FAS.CurrentUser.Id)
            {
                contentUserName = new GUIContent(FresviiGUIUtility.Truncate(FAS.CurrentUser.Name, guiStyleUserName, userNamePosition.width, "..."));
            }
            else
            {
                contentUserName = new GUIContent(FresviiGUIUtility.Truncate(rank.Score.User.Name, guiStyleUserName, userNamePosition.width, "..."));
            }

            tagPosition = new Rect(width - sideMargin, cardHeight * 0.5f - frameLeaderboard.textureTagYou.height * 0.5f, frameLeaderboard.textureTagYou.width, frameLeaderboard.textureTagYou.height);
        }

        public float GetHeight()
        {
            return cardHeight;
        }

        private void LoadUserIcon()
        {
            userIconLoading = true;

            if (rank.Score.User.Id == FAS.CurrentUser.Id)
            {
                userProfileUrl = FAS.CurrentUser.ProfileImageUrl;
            }
            else
            {
                userProfileUrl = rank.Score.User.ProfileImageUrl;
            }

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(userProfileUrl, true, delegate(Texture2D texture)
            {
                userIcon = texture;

                userIconLoading = false;

                iconError = (userIcon == null);
            });
        }

        public void Draw(Rect position, bool tagDraw)
        {
            if (!this.gameObject.activeInHierarchy) return;

            if (rank.Score.User != null)
            {
                if (rank.Score.User.Id == FAS.CurrentUser.Id)
                {
                    if (FAS.CurrentUser.ProfileImageUrl != userProfileUrl && !userIconLoading)
                    {
                        FresviiGUIManager.Instance.resourceManager.ReleaseTexture(userProfileUrl);

                        LoadUserIcon();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(rank.Score.User.ProfileImageUrl) && userIcon == null && !userIconLoading && !iconError)
                    {
                        LoadUserIcon();
                    }
                    else if (rank.Score.User.ProfileImageUrl != userProfileUrl && !userIconLoading && !iconError)
                    {
                        FresviiGUIManager.Instance.resourceManager.ReleaseTexture(userProfileUrl);

                        LoadUserIcon();
                    }
                }
            }

            CalcLayout(position.width);

            GUI.BeginGroup(position);

            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), palette, texCoordsBackground);

            //  rank
            if (rank.Ranking < 4)
            {
                GUI.DrawTexture(medalPosition, frameLeaderboard.textureMedal);
            }

            GUI.Label(rankPosition, rank.Ranking.ToString(), guiStyleRank);

            //  UserIcon -----------------------
            if (Event.current.type == EventType.Repaint)
            {
                userMask.color = new Color(1f, 1f, 1f, 1.0f);

                Graphics.DrawTexture(userIconPosition, ((userIcon == null) ? userIconDefault : userIcon), userMask);
            }

            //  User name -----------------------
            GUI.Label(userNamePosition, contentUserName, guiStyleUserName);

            GUI.Label(scoreLabelPosition, rank.Score.FormattedValue, guiStyleScore);

            if (tagDraw)
            {
                if (tagKind == TagKind.Friend)
                {
                    GUI.DrawTexture(tagPosition, frameLeaderboard.textureTagFriend);

                    GUI.Label(tagPosition, tagString, guiStyleTag);
                }
                else if (tagKind == TagKind.You)
                {
                    GUI.DrawTexture(tagPosition, frameLeaderboard.textureTagYou);

                    GUI.Label(tagPosition, tagString, guiStyleTag);
                }
            }
         
            GUI.EndGroup();

            if (buttonCard.IsTap(Event.current, position))
            {
                if (rank.Score.User != null)
                {
                    if (nextFrameProfile == null)
                    {
                        if (rank.Score.User.Id == FAS.CurrentUser.Id)
                        {
                            nextFrameProfile = ((GameObject)Instantiate(prfbGUIFrameMyProfile)).GetComponent<FresviiGUIFrame>();
                        }
                        else
                        {
                            nextFrameProfile = ((GameObject)Instantiate(prfbGUIFrameUserProfile)).GetComponent<FresviiGUIFrame>();

                            nextFrameProfile.gameObject.GetComponent<FresviiGUIUserProfile>().SetUser(rank.Score.User);
                        }

                        nextFrameProfile.Init(null, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, FASGui.GuiDepthBase);

                        nextFrameProfile.transform.parent = this.transform;
                    }

                    nextFrameProfile.SetDraw(true);

                    nextFrameProfile.PostFrame = parentFrameProfile;

                    parentFrameProfile.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
                    {
                        parentFrameProfile.SetDraw(false);
                    });

                    nextFrameProfile.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
                }
            }

        }

        public void DestroySubFrames()
        {
            if (nextFrameProfile != null)
            {
                Destroy(nextFrameProfile.gameObject);
            }
        }

        void OnDestroy(){

			if(userIcon != null)
				Destroy(userIcon);
		}
	}
}