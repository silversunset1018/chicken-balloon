using UnityEngine;
using System.Collections;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIFriendRequestCard : MonoBehaviour
    {
        private bool userIconLoading = false;

        //  public
        private Texture palette;
        private Rect texCoordsBackground;

        private float sideMargin;
        public float hMargin = 10;

        public Rect userIconPosition;
		private Texture2D userIcon;
        public Texture2D userIconDefault;
        public Texture2D userIconMask;

        public Rect userNamePosition;
        public GUIStyle guiStyleUserName;

        public Rect userDescriptionPosition;
        public GUIStyle guiStyleDescription;

        public Vector2 buttonSize;

        private float scaleFactor;

        private bool imageLoaded;

		public float imageTweenTime = 0.5f;
		public iTween.EaseType imageTweenEasetype = iTween.EaseType.easeOutExpo;
		
		private float cardAlpha = 1.0f;

        private GUIContent contentUserName;
        private GUIContent contentUserDescription;

        public Fresvii.AppSteroid.Models.User User { get; protected set; }

        private FresviiGUIFriendRequests guiFriendRequest;

        public FresviiGUIButton buttonAdd, buttonHide;
        Rect buttonAddPosition, buttonHidePosition;
        public GUIStyle guiStyleButtonAdd;
        public GUIStyle guiStyleButtonHide;

        private float tweenHeight;
        private bool heightTweening;

        public bool isHiddenCard;

		public Material userMask;

        public void Init(Fresvii.AppSteroid.Models.User user, float scaleFactor, bool isHiddenCard, FresviiGUIFriendRequests guiFriendRequest)
        {
            this.User = user;

            this.guiFriendRequest = guiFriendRequest;

            this.isHiddenCard = isHiddenCard;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleUserName.font = null;
                
                guiStyleUserName.fontStyle = FontStyle.Bold;

                guiStyleDescription.font = null;

                guiStyleButtonAdd.font = null;
                
                guiStyleButtonHide.font = null;
            }

            palette = FresviiGUIColorPalette.Palette;

            userIconPosition = FresviiGUIUtility.RectScale(userIconPosition, scaleFactor);
            
            userNamePosition = FresviiGUIUtility.RectScale(userNamePosition, scaleFactor);
            
            userDescriptionPosition = FresviiGUIUtility.RectScale(userDescriptionPosition, scaleFactor);

            sideMargin = userIconPosition.x;

            hMargin *= scaleFactor;
            
            buttonSize *= scaleFactor;

            guiStyleUserName.fontSize = (int)(guiStyleUserName.fontSize * scaleFactor);

            guiStyleUserName.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            guiStyleDescription.fontSize = (int)(guiStyleDescription.fontSize * scaleFactor);

            guiStyleDescription.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText1);
            
            guiStyleButtonAdd.fontSize = (int)(guiStyleButtonAdd.fontSize * scaleFactor);

            guiStyleButtonAdd.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardAddButtonText);           

            guiStyleButtonHide.fontSize = (int)(guiStyleButtonHide.fontSize * scaleFactor);

            guiStyleButtonHide.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardHideButtonText);

            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);
        }
		
        private void CalcLayout(float width)
        {
            userNamePosition.width = width - userNamePosition.x - sideMargin;
            
            userDescriptionPosition.width = width - userDescriptionPosition.x - sideMargin;

            buttonSize.x = (width - 3.0f * sideMargin) / 2.0f;

            contentUserName = new GUIContent(FresviiGUIUtility.Truncate(User.Name, guiStyleUserName, userNamePosition.width, "..."));

            contentUserDescription = new GUIContent(User.Description);

            int userDescDeleteStringNum = 2;

            while (guiStyleDescription.CalcHeight(contentUserDescription, userDescriptionPosition.width) > userDescriptionPosition.height)
            {
                contentUserDescription = new GUIContent(User.Description.Substring(0, User.Description.Length - userDescDeleteStringNum) + "...");

                userDescDeleteStringNum++;
            }

            if (!isHiddenCard)
            {
                buttonAddPosition = new Rect(sideMargin, userDescriptionPosition.y + userDescriptionPosition.height + hMargin, buttonSize.x, buttonSize.y);

                buttonHidePosition = new Rect(buttonAddPosition.x + buttonSize.x + sideMargin, buttonAddPosition.y, buttonSize.x, buttonSize.y);
            }
            else
            {
                buttonAddPosition = new Rect((width - buttonSize.x) * 0.5f, userDescriptionPosition.y + userDescriptionPosition.height + hMargin, buttonSize.x, buttonSize.y);
            }

        }

        public float GetHeight()
        {
            if (heightTweening) return tweenHeight;

            float height = 0.0f;

            height += userDescriptionPosition.y + userDescriptionPosition.height;
            height += hMargin;
            height += buttonSize.y;
            height += userIconPosition.y;

            return height;
        }

        private bool userIconLoaded;
        private bool clipImageLoaded;

        public float hideTweenTime = 0.25f;

        private bool cardHidden = false;

        private void DeleteCard()
        {
            if (FresviiGUIManager.Instance.FriendRequestCount > 0 && !isHiddenCard)
            {
                FresviiGUIManager.Instance.FriendRequestCount--;
            }

            iTween.ValueTo(this.gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", hideTweenTime, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateCardAlpha", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteAlphaDelete"));
        }

        private void HideCard()
        {
            cardHidden = true;

            DeleteCard();
        }

        void OnEnable()
        {
            userIconLoading = false;
        }

        void OnCompleteAlphaDelete()
        {
            cardAlpha = 0.0f;

            tweenHeight = GetHeight();

            iTween.ValueTo(this.gameObject, iTween.Hash("from", tweenHeight, "to", 0.0f, "time", imageTweenTime * 0.5f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateCardHeight", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteCardHeight"));

            heightTweening = true;
        }

        private void OnUpdateCardAlpha(float alpha)
        {
            cardAlpha = alpha;
        }

        void OnUpdateCardHeight(float value)
        {
            tweenHeight = value;
        }

        void OnCompleteCardHeight()
        {
            tweenHeight = 0.0f;

            guiFriendRequest.RemoveCard(this, cardHidden);
        }

        public void Reflesh()
        {
            cardAlpha = 1.0f;

            heightTweening = false;

            postWidth = 0f;

            cardHidden = false;
        }

        private void LoadUserIcon()
        {
            userIconLoading = true;

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(User.ProfileImageUrl, true, delegate(Texture2D texture)
            {
                userIcon = texture;

                userIconLoading = false;
            });
        }

        float postWidth;

        public void Draw(Rect position)
        {
            Color origColor = GUI.color;

            Color cardColor = origColor;

            cardColor.a = cardAlpha;
            
            GUI.color = cardColor;

            if (!string.IsNullOrEmpty(User.ProfileImageUrl) && userIcon == null && !userIconLoading)
            {
                LoadUserIcon();
            }

            if (postWidth != position.width)
            {
                CalcLayout(position.width);

                postWidth = position.width;
            }

            GUI.BeginGroup(position);

            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), palette, texCoordsBackground);

            //  UserIcon -----------------------
            if (Event.current.type == EventType.Repaint)
            {
				userMask.color = new Color(1f,1f,1f,cardAlpha);

                Graphics.DrawTexture(userIconPosition, ((userIcon == null) ? userIconDefault : userIcon), userMask);
            }

            //  User name -----------------------
            GUI.Label(userNamePosition, contentUserName, guiStyleUserName);

            //  User Description -----------------------
            GUI.Label(userDescriptionPosition, contentUserDescription, guiStyleDescription);

            Event e = Event.current;

            //  Button Confirm
            if(buttonAdd.IsTap(e, buttonAddPosition, buttonAddPosition, 
                FresviiGUIButton.ButtonType.FrameAndLabel, 
                FresviiGUIFriendRequests.TexButtonAdd, 
                FresviiGUIFriendRequests.TexButtonAddH,
                FresviiGUIFriendRequests.TexButtonAddH,
                FresviiGUIText.Get("Confirm"),
                guiStyleButtonAdd))
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });

                    return;
                }

                FASFriendship.AcceptFriendshipRequest(User.Id, delegate(Fresvii.AppSteroid.Models.FriendshipRequest friendshipRequest, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error == null)
                    {
                        guiFriendRequest.frameMyProfile.AddFriend(friendshipRequest.InvitingUser);

                        DeleteCard();
                    }
                    else
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        {
                            Debug.LogError(error.ToString());
                        }

                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool ans) { });
                    }
                });
            }

            //  Button Hide
            if (!isHiddenCard)
            {
                if (buttonHide.IsTap(e, buttonHidePosition, buttonHidePosition,
                    FresviiGUIButton.ButtonType.FrameAndLabel,
                    FresviiGUIFriendRequests.TexButtonHide,
                    FresviiGUIFriendRequests.TexButtonHideH,
                    FresviiGUIFriendRequests.TexButtonHideH,
                    FresviiGUIText.Get("NotNow"),
                    guiStyleButtonHide))
                {
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });

                        return;
                    }

                    FASFriendship.HideFriendshipRequest(User.Id, delegate(Fresvii.AppSteroid.Models.FriendshipRequest friendshipRequest, Fresvii.AppSteroid.Models.Error error)
                    {
                        if (error == null)
                        {
                            HideCard();
                        }
                        else
                        {
                            if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                            {
                                Debug.LogError(error.ToString());
                            }

                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool ans) { });
                        }
                    });
                }
            }

            GUI.EndGroup();

            GUI.color = origColor;
        }

        void OnDestroy()
        {
            if (userIcon != null)
            {
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(User.ProfileImageUrl);
            }
		}
	}
}