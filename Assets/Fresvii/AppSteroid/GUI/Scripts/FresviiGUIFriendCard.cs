using UnityEngine;
using System.Collections;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIFriendCard : MonoBehaviour
    {
        public bool userIconLoading = false;

        //  public
        private Texture palette;
        private Rect texCoordsBackground;
        private Rect texCoordsSeperateLine;

        public float cardHeight = 55f;

        private float sideMargin;

        public Rect userIconPosition;
		private Texture2D userIcon;
        public Texture2D userIconDefault;

        public Rect userNamePosition;
        public GUIStyle guiStyleUserName;

        private float scaleFactor;

        private bool imageLoaded;

		public float imageTweenTime = 0.5f;
		public iTween.EaseType imageTweenEasetype = iTween.EaseType.easeOutExpo;
		
        private Rect cardSeperateLinePosition;

        private GUIContent contentUserName;

        private string userProfileUrl;

        public Fresvii.AppSteroid.Models.Friend friend { get; set; }

        private Fresvii.AppSteroid.Models.User user;

        public FresviiGUIButton buttonCard;

        public GameObject prfbGUIFrameUserProfile;
        public GameObject prfbGUIFrameMyProfile;

        private FresviiGUIFrame parentFrameProfile;
        private FresviiGUIFrame nextFrameProfile;

        private Texture2D textureTagYou;
        private bool isMe;
        private Rect tagPosition;
        private string tagString = "";
        public GUIStyle guiStyleTag;

        private bool iconError;

        public void Init(Fresvii.AppSteroid.Models.Friend friend, float scaleFactor, FresviiGUIFrame parentFrame)
        {
            this.friend = friend;
            this.parentFrameProfile = parentFrame;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleUserName.font = null;
                guiStyleUserName.fontStyle = FontStyle.Bold;
                guiStyleTag.font = null;
            }

            userIconPosition = FresviiGUIUtility.RectScale(userIconPosition, scaleFactor);
            userNamePosition = FresviiGUIUtility.RectScale(userNamePosition, scaleFactor);

            sideMargin = userIconPosition.x;

            cardHeight *= scaleFactor;
            
            guiStyleUserName.fontSize = (int)(guiStyleUserName.fontSize * scaleFactor);

            palette = FresviiGUIColorPalette.Palette;

            guiStyleUserName.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            guiStyleTag.fontSize = (int)(guiStyleTag.fontSize * scaleFactor);

            guiStyleTag.padding = FresviiGUIUtility.RectOffsetScale(guiStyleTag.padding, scaleFactor);

            guiStyleTag.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardBackground);

            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            texCoordsSeperateLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardSeperateLine1);
                       
			GetUser ();    
        }

		void GetUser()
		{
            FASUser.GetUser(friend.Id, delegate(Fresvii.AppSteroid.Models.User _user, Fresvii.AppSteroid.Models.Error _error)
			{
				if(_error == null)
				{
					this.user = _user;

                    isMe = (this.user.Id == FAS.CurrentUser.Id);

                    if (isMe)
                    {
                        textureTagYou = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.TagYou + FresviiGUIManager.postFix, false);

                        tagString = FresviiGUIText.Get("You");
                    }
				}
			});
		}
		
        private void CalcLayout(float width)
        {
			userNamePosition.width = width - userNamePosition.x - sideMargin - ((isMe) ? (textureTagYou.width + sideMargin) : 0.0f);

            contentUserName = new GUIContent(friend.Name);

            int userNameDeleteStringNum = 2;

            while (guiStyleUserName.CalcSize(contentUserName).x > userNamePosition.width)
            {
				if(friend.Name.Length - userNameDeleteStringNum < 2 ) break;

                contentUserName = new GUIContent(friend.Name.Substring(0, friend.Name.Length - userNameDeleteStringNum) + "...");

                userNameDeleteStringNum++;
            }

            cardSeperateLinePosition = new Rect(userNamePosition.x, cardHeight - 10.0f, width - userNamePosition.x, 10.0f);

            if(isMe)
                tagPosition = new Rect(width - sideMargin - textureTagYou.width, cardHeight * 0.5f - textureTagYou.height * 0.5f, textureTagYou.width, textureTagYou.height);

        }

        public float GetHeight()
        {
            return cardHeight;
        }

        private bool userIconLoaded;
        private bool clipImageLoaded;

        public Material userIconMask;

        private void LoadUserIcon()
        {
            userIconLoading = true;

            userProfileUrl = user.ProfileImageUrl;

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(user.ProfileImageUrl, true, delegate(Texture2D texture)
            {
                userIcon = texture;

                userIconLoading = false;

                iconError = (userIcon == null);
            });
        }

        public void Draw(Rect position, bool drawSeperateLine)
        {
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.ProfileImageUrl) && userIcon == null && !userIconLoading && !iconError)
                {
                    LoadUserIcon();
                }
                else if (user.ProfileImageUrl != userProfileUrl && !userIconLoading && !iconError)
                {
                    FresviiGUIManager.Instance.resourceManager.ReleaseTexture(userProfileUrl);

                    LoadUserIcon();
                }
            }

            CalcLayout(position.width);

            GUI.BeginGroup(position);

            //  Line
            if (drawSeperateLine)
            {
                GUI.DrawTextureWithTexCoords(cardSeperateLinePosition, palette, texCoordsSeperateLine);

                // Background
                GUI.DrawTextureWithTexCoords(new Rect(cardSeperateLinePosition.x, 0, cardSeperateLinePosition.width, position.height - 1.0f), palette, texCoordsBackground);
                GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width - cardSeperateLinePosition.width, position.height), palette, texCoordsBackground);
            }
            else
            {
                // Background
                GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), palette, texCoordsBackground);
            }


            //  UserIcon -----------------------
            if (Event.current.type == EventType.Repaint)
            {
                userIconMask.color = new Color(1f, 1f, 1f, 1f);

                Graphics.DrawTexture(userIconPosition, (userIcon == null ? userIconDefault : userIcon), userIconMask);
            }

            //  User name -----------------------
            GUI.Label(userNamePosition, contentUserName, guiStyleUserName);

            if (isMe)
            {
                GUI.DrawTexture(tagPosition, textureTagYou);

                GUI.Label(tagPosition, tagString, guiStyleTag);
            }
            
            GUI.EndGroup();

            if (buttonCard.IsTap(Event.current, position))
            {
                if (this.user != null)
                {
                    if (nextFrameProfile == null)
                    {
                        if (this.user.Id == FAS.CurrentUser.Id)
                        {
                            nextFrameProfile = ((GameObject)Instantiate(prfbGUIFrameMyProfile)).GetComponent<FresviiGUIFrame>();
                        }
                        else
                        {
                            nextFrameProfile = ((GameObject)Instantiate(prfbGUIFrameUserProfile)).GetComponent<FresviiGUIFrame>();

                            nextFrameProfile.gameObject.GetComponent<FresviiGUIUserProfile>().SetUser(user);
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

        void OnDestroy()
        {
            if (FresviiGUIManager.Instance.resourceManager != null)
            {
                if(user != null)
                    FresviiGUIManager.Instance.resourceManager.ReleaseTexture(user.ProfileImageUrl);
            }
		}
	}
}