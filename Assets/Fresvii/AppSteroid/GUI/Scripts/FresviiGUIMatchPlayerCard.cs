using UnityEngine;
using System.Collections;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIMatchPlayerCard : MonoBehaviour
    {
        private bool userIconLoading = false;

        //  public
        private Texture palette;

        private Rect texCoordsBackground;

        public float cardHeight = 55f;

        private float sideMargin;

        public Rect userIconPosition;
		
        private Texture2D userIcon;
        
        public Texture2D userIconDefault;
        
        public Texture2D userIconMask;

        public Rect userNamePosition;

        public Rect playerNamePosition;

        public Rect tagPosition;

        public Rect palyerStatusPosition;

        public GUIStyle guiStyleUserName;

        public GUIStyle guiStyleText;

        public GUIStyle guiStylePlayerStatus;

        private float scaleFactor;

		public float imageTweenTime = 0.5f;

        public iTween.EaseType imageTweenEasetype = iTween.EaseType.easeOutExpo;

        private GUIContent contentUserName;

        private string userProfileUrl;

        public Fresvii.AppSteroid.Models.Player Player { get; protected set; }

        public FresviiGUIButton buttonCard;

        public GameObject prfbGUIFrameUserProfile;

        public GameObject prfbGUIFrameMyProfile;
        
        private FresviiGUIFrame parentFrame;


        private FresviiGUIMatchMaking frameMatchMaking;

        public GUIStyle guiStyleTag;

        private string tagString;

        public FresviiGUIButton buttonUserIcon;

        public void Init(float scaleFactor, FresviiGUIFrame parentFrame, FresviiGUIMatchMaking frameMatchMaking)
        {            
            this.parentFrame = parentFrame;

            this.transform.parent = parentFrame.transform;

            this.frameMatchMaking = frameMatchMaking;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleUserName.font = null;

                guiStyleUserName.fontStyle = FontStyle.Bold;

                guiStyleText.font = null;

                guiStyleTag.font = null;

                guiStylePlayerStatus.font = null;
            }

            userIconPosition = FresviiGUIUtility.RectScale(userIconPosition, scaleFactor);

            tagPosition = FresviiGUIUtility.RectScale(tagPosition, scaleFactor);
            
            userNamePosition = FresviiGUIUtility.RectScale(userNamePosition, scaleFactor);

            playerNamePosition = FresviiGUIUtility.RectScale(playerNamePosition, scaleFactor);

            palyerStatusPosition = FresviiGUIUtility.RectScale(palyerStatusPosition, scaleFactor);

            sideMargin = userIconPosition.x;

            cardHeight *= scaleFactor;
            
            guiStyleUserName.fontSize = (int)(guiStyleUserName.fontSize * scaleFactor);

            guiStyleText.fontSize = (int)(guiStyleText.fontSize * scaleFactor);

            guiStyleTag.fontSize = (int)(guiStyleTag.fontSize * scaleFactor);

            guiStyleTag.padding = FresviiGUIUtility.RectOffsetScale(guiStyleTag.padding, scaleFactor);

            guiStylePlayerStatus.fontSize = (int)(guiStylePlayerStatus.fontSize * scaleFactor);

            palette = FresviiGUIColorPalette.Palette;

            guiStylePlayerStatus.normal.textColor = guiStyleUserName.normal.textColor = guiStyleText.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            if (frameMatchMaking.State == FresviiGUIMatchMaking.Mode.Setting)
            {
                contentUserName = new GUIContent(FresviiGUIText.Get("InviteFriend"));
            }
            else if (frameMatchMaking.State == FresviiGUIMatchMaking.Mode.Matching)
            {
                contentUserName = new GUIContent(FresviiGUIText.Get("AutoMatch"));
            }

            guiStyleTag.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardBackground);

            tagString = FresviiGUIText.Get("You");
        }

        public void SetPlayer(Fresvii.AppSteroid.Models.Player player)
        {
            if (player == null)
            {
                if (this.Player != null)
                {
                    if (this.Player.User != null && userIcon != null)
                    {
                        FresviiGUIManager.Instance.resourceManager.ReleaseTexture(Player.User.ProfileImageUrl);
                    }
                }

                userIcon = null;
            }

            this.Player = player;
        }
		
        private void CalcLayout(float width)
        {
            tagPosition.x = width - sideMargin - tagPosition.width;

            if (Player != null)
            {
                if (Player.User.Id == FAS.CurrentUser.Id)
                {
                    userNamePosition.width = width - userNamePosition.x - sideMargin - tagPosition.width;

                    contentUserName = new GUIContent(FresviiGUIUtility.Truncate(Player.User.Name, guiStyleUserName, userNamePosition.width, "..."));
                }
                else
                {
                    userNamePosition.width = width - userNamePosition.x - sideMargin;

                    playerNamePosition.width = width - userNamePosition.x - sideMargin;

                    contentUserName = new GUIContent(FresviiGUIUtility.Truncate(Player.User.Name, guiStyleUserName, playerNamePosition.width, "..."));
                }
            }
        }

        public float GetHeight()
        {
            return cardHeight;
        }

        private bool userIconLoaded;

        private bool clipImageLoaded;

        private void LoadUserIcon()
        {
            userIconLoading = true;

            userProfileUrl = Player.User.ProfileImageUrl;

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(Player.User.ProfileImageUrl, true, delegate(Texture2D texture)
            {
                userIcon = texture;

                userIconLoading = false;
            });
        }

        public Material userMask;

        private FresviiGUIFrame nextFrame;

        public void Draw(Rect position)
        {
            if (Player != null)
            {
                if (Player.User != null)
                {
                    if (!string.IsNullOrEmpty(Player.User.ProfileImageUrl) && userIcon == null && !userIconLoading)
                    {
                        LoadUserIcon();
                    }
                    else if (Player.User.ProfileImageUrl != userProfileUrl && !userIconLoading)
                    {
                        FresviiGUIManager.Instance.resourceManager.ReleaseTexture(userProfileUrl);

                        LoadUserIcon();
                    }
                }
            }

            CalcLayout(position.width);

            GUI.BeginGroup(position);

            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), palette, texCoordsBackground);

            //  UserIcon -----------------------
            if (Event.current.type == EventType.Repaint)
            {
                userMask.color = new Color(1f, 1f, 1f, 1f);

                Graphics.DrawTexture(userIconPosition, ((userIcon == null) ? userIconDefault : userIcon), userMask);
            }

            if (buttonUserIcon.IsTap(Event.current, userIconPosition))
            {
                if (Player != null)
                {
                    if (Player.User != null)
                    {
                        if (nextFrame == null)
                        {
                            if (Player.User.Id == FAS.CurrentUser.Id)
                            {
                                nextFrame = ((GameObject)Instantiate(prfbGUIFrameMyProfile)).GetComponent<FresviiGUIFrame>();
                            }
                            else
                            {
                                nextFrame = ((GameObject)Instantiate(prfbGUIFrameUserProfile)).GetComponent<FresviiGUIFrame>();

                                nextFrame.gameObject.GetComponent<FresviiGUIUserProfile>().SetUser(Player.User);
                            }

                            nextFrame.Init(null, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, parentFrame.GuiDepth - 100);
                        }

                        nextFrame.transform.parent = this.transform;

                        nextFrame.SetDraw(true);

                        nextFrame.PostFrame = parentFrame;

                        parentFrame.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
                        {
                            parentFrame.SetDraw(false);
                        });

                        nextFrame.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
                    }
                }
            }

            //  User name -----------------------
            if (Player != null)
            {
                contentUserName = new GUIContent(Player.User.Name);

                if (frameMatchMaking.State != FresviiGUIMatchMaking.Mode.Matching)
                {
                    guiStyleUserName.alignment = TextAnchor.MiddleLeft;

                    if (Player.User.Id == FAS.CurrentUser.Id)
                    {
                        GUI.Label(userNamePosition, contentUserName, guiStyleUserName);

                        GUI.DrawTexture(tagPosition, frameMatchMaking.textureTagYou);

                        GUI.Label(tagPosition, tagString, guiStyleTag);
                    }
                    else
                    {
                        GUI.Label(userNamePosition, contentUserName, guiStyleUserName);
                    }
                }
                else
                {
                    if (Player.User.Id == FAS.CurrentUser.Id)
                    {
                        guiStyleUserName.alignment = TextAnchor.MiddleLeft;

                        GUI.Label(userNamePosition, contentUserName, guiStyleUserName);

                        GUI.DrawTexture(tagPosition, frameMatchMaking.textureTagYou);

                        GUI.Label(tagPosition, tagString, guiStyleTag);
                    }
                    else
                    {
                        guiStyleUserName.alignment = TextAnchor.LowerLeft;

                        GUI.Label(playerNamePosition, contentUserName, guiStyleUserName);

                        if (Player.Status == Fresvii.AppSteroid.Models.Player.Statuses.Invited)
                        {
                            GUI.Label(palyerStatusPosition, FresviiGUIText.Get("PlayerStatusInvited"), guiStylePlayerStatus);
                        }
                        else if (Player.Status == Fresvii.AppSteroid.Models.Player.Statuses.Accepted)
                        {
                            GUI.Label(palyerStatusPosition, FresviiGUIText.Get("PlayerStatusAccepted"), guiStylePlayerStatus);
                        }
                        else if (Player.Status == Fresvii.AppSteroid.Models.Player.Statuses.Cancelled)
                        {
                            GUI.Label(palyerStatusPosition, FresviiGUIText.Get("PlayerStatusCancelled"), guiStylePlayerStatus);
                        }
                        else if (Player.Status == Fresvii.AppSteroid.Models.Player.Statuses.Matching)
                        {
                            GUI.Label(palyerStatusPosition, FresviiGUIText.Get("PlayerStatusMatching"), guiStylePlayerStatus);
                        }
                        else if (Player.Status == Fresvii.AppSteroid.Models.Player.Statuses.Declined)
                        {
                            GUI.Label(palyerStatusPosition, FresviiGUIText.Get("PlayerStatusDeclined"), guiStylePlayerStatus);
                        }
                    }
                }                
            }
            else if (frameMatchMaking.State == FresviiGUIMatchMaking.Mode.Setting)
            {
                contentUserName = new GUIContent(FresviiGUIText.Get("InviteFriend"));

                GUI.Label(userNamePosition, contentUserName, guiStyleText);
            }
            else if (frameMatchMaking.State == FresviiGUIMatchMaking.Mode.Matching && Player == null)
            {
                contentUserName = new GUIContent(FresviiGUIText.Get("AutoMatch"));

                GUI.Label(userNamePosition, contentUserName, guiStyleText);
            }
            
            GUI.EndGroup();

            if (buttonCard.IsTap(Event.current, position) && frameMatchMaking.State == FresviiGUIMatchMaking.Mode.Setting)
            {
                frameMatchMaking.SelectFriends();
            }
        }

        void OnDestroy()
        {
            if (Player != null)
            {
                if (Player.User != null && userIcon != null)
                {
                    FresviiGUIManager.Instance.resourceManager.ReleaseTexture(Player.User.ProfileImageUrl);
                }
            }
		}
	}
}