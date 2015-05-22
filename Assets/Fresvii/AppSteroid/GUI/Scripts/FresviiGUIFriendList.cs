using UnityEngine;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIFriendList : FresviiGUIFrame
    {   
        private FresviiGUIFriendListTop friendListTop;
        
        private FresviiGUITabBar tabBar;

        private Rect baseRect;
        
        private Rect scrollViewRect;
        
        private float scaleFactor;
       
        public Fresvii.AppSteroid.Models.User user;

        public Vector2 profileImageSize;

        public Vector2 profileImageMaxSize;

        public FresviiGUIButton buttonFriend;
        
        private Rect friendButtonPosition;
        
        public Vector2 friendButtonSize;

        public FresviiGUIButton buttonMessage;
        
        private Rect messageButtonPosition;

        public FresviiGUIButton buttonCall;

        public FresviiGUIButton buttonFriendList;

        private Rect callButtonPosition;
        
        public Vector2 messageButtonSize;

        public Vector2 buttonIconRelativePosition;
        
        public Vector2 buttonLabelRelativePosition;

        private Rect userImagePosition;
        
        private Rect userNamePosition;
        
        private Rect userDescriptionPosition;

        private Fresvii.AppSteroid.Models.ListMeta friendsMeta;

        private IList<Fresvii.AppSteroid.Models.Friend> friends = new List<Fresvii.AppSteroid.Models.Friend>();

        public float friendMenuTitleBarHeight = 33f;
        
        private Rect friendMenuTitleBarPosition;
        
        private Rect friendMenuTitleLabelPosition;

        private Rect friendMenuRightIconPosition;
        
        private Rect friendMenuTitleUnderLinePosition;

        public GameObject prfbFriendCard;
        
        private List<FresviiGUIFriendCard> cards = new List<FresviiGUIFriendCard>();

        private Fresvii.AppSteroid.Gui.LoadingSpinner bottomLoadingSpinner;

        private Rect bottomLoadingSpinnerPosition;
        
        public Vector2 loadingSpinnerSize;

        private bool friendListLoading = false;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;
        
        public float pullResetTweenTime = 0.5f;
        
        public iTween.EaseType pullRestTweenEaseType = iTween.EaseType.easeOutExpo;

        public GameObject prfbGUIFramePairChat;

		private Color btnPositiveColor, btnNegativeColor;

        public FresviiGUIScrollviewSlider slider;

        private FresviiGUIFrame frameGroupConference;

        public GameObject prfbGUIFrameGroupCall;

        public float pullRefleshHeight = 50f;

        private FresviiGUIFrame nextFrameProfile;

        public GameObject prfbGUIFrameUserProfile;

        public GameObject prfbGUIFrameMyProfile;

        public bool IsMyList { get; protected set; }

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = guiDepth;

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            friendListTop = GetComponent<FresviiGUIFriendListTop>();

            friendListTop.Init(appIcon, postFix, scaleFactor, guiDepth - 1, this);

            tabBar = GetComponent<FresviiGUITabBar>();

            tabBar.Init(postFix, scaleFactor, this.GuiDepth - 1);

            tabBar.GuiDepth = GuiDepth - 1;

            this.scaleFactor = scaleFactor;
                                  
            friendMenuTitleBarHeight *= scaleFactor;
			
            loadingSpinnerSize *= scaleFactor;

            pullRefleshHeight *= this.scaleFactor;
            
            SetScrollSlider(scaleFactor * 2.0f);
            
            loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GuiDepth - 10);

            GetUserFriendList(1);
        }

        void CalcLayout()
        {
            this.baseRect = new Rect(Position.x, Position.y + friendListTop.height, Screen.width, Screen.height - friendListTop.height - tabBar.height - FresviiGUIFrame.OffsetPosition.y);
        
            if (bottomLoadingSpinner != null)
            {
                bottomLoadingSpinner.Position = new Rect(baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + baseRect.height - pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }

            if (loadingSpinner != null)
            {
                loadingSpinner.Position = new Rect(baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + baseRect.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }
        }

        float CalcScrollViewHeight()
        {
            float height = 0.0f;

            foreach (FresviiGUIFriendCard card in cards)
                height += card.GetHeight();

            return height;

        }

		void Update(){

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            if (user == null) return;

            CalcLayout();

            float scrollViewHeight = CalcScrollViewHeight();

            scrollViewRect.x = Position.x;

            scrollViewRect.width = Screen.width;

            scrollViewRect.height = scrollViewHeight;

            InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, pullRefleshHeight, null, OnPullUpReflesh);

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && !ControlLock)
            {
                BackToPostFrame();
            }
#endif
		}

        public void OnUpdateScrollViewPosition(float value)
        {
            scrollViewRect.y = value;
        }

        void OnPullUpReflesh()
        {
            if (friendsMeta != null)
            {
                if (friendsMeta.NextPage != null && !friendListLoading)
                {
                    bottomLoadingSpinnerPosition = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + baseRect.y + baseRect.height - pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

                    bottomLoadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(bottomLoadingSpinnerPosition, GuiDepth - 5);

                    GetUserFriendList((uint)friendsMeta.NextPage);
                }
            }
        }

        void OnCompletePull()
        {

        }

        public void SetUser(Fresvii.AppSteroid.Models.User user)
        {
            this.user = user;

            IsMyList = (user.Id == FAS.CurrentUser.Id);         
        }

		void GetUserFriendList(uint page)
        {
            friendListLoading = true;

            FASFriendship.GetUserFriendList(user.Id, delegate(IList<Fresvii.AppSteroid.Models.Friend> friends, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
			{
				friendListLoading = false;

                if (bottomLoadingSpinner != null)
                {
                    bottomLoadingSpinner.Hide();
                }

				if (error != null && this.gameObject != null)
				{
                    if (this.gameObject.activeInHierarchy)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                        if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NetworkNotReachable)
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });
                        }
                        else
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(error.Detail, delegate(bool del) { });
                        }
                    }
				
					return;
				}
				else
				{
                    this.friendsMeta = meta;

                    foreach (Fresvii.AppSteroid.Models.Friend friend in friends)
                    {
                        AddFriend(friend);
                    }

                    if (page == 1)
                    {
                        friendListTop.SetTitle(this.user, this.friendsMeta);

                        if (loadingSpinner != null)
                            loadingSpinner.Hide();
                    }
				}				
			});
		}

        void AddFriend(Fresvii.AppSteroid.Models.Friend friend)
        {
            bool exists = false;

            foreach(Fresvii.AppSteroid.Models.Friend _friend in friends)
            {
                if (_friend.Id == friend.Id)
                {
                    exists = true;

                    break;
                }
            }

            if (!exists)
            {
                FresviiGUIFriendCard card = ((GameObject)Instantiate(prfbFriendCard)).GetComponent<FresviiGUIFriendCard>();

                card.Init(friend, scaleFactor, this);

                card.transform.parent = this.transform;

                cards.Add(card);
            }
        }

        void OnDestroy()
        {
            foreach (FresviiGUIFriendCard card in cards)
            {
                if(card.gameObject != null)
                    Destroy(card.gameObject);
            }

            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }

            if (bottomLoadingSpinner != null)
            {
                bottomLoadingSpinner.Hide();
            }
		}

		void OnDisable(){

			if (loadingSpinner != null)
			{
				loadingSpinner.Hide();
			}

		}

        public void BackToPostFrame()
        {
			if (loadingSpinner != null)
			{
				loadingSpinner.Hide();
			}

            tabBar.enabled = false;

            PostFrame.SetDraw(true);            

            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
            {
                Destroy(this.gameObject);
            });

            PostFrame.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            tabBar.enabled = on;

            friendListTop.enabled = on;

            if (!on)
            {
                if (bottomLoadingSpinner != null)
                    bottomLoadingSpinner.Hide();
            }
        }

        public void GoToUserProfile(Fresvii.AppSteroid.Models.User user)
        {            
            if (user.Id == FAS.CurrentUser.Id)
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
            
            nextFrameProfile.SetDraw(true);

            nextFrameProfile.PostFrame = this;

            this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
            {
                this.SetDraw(false);
            });

            nextFrameProfile.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
        }

        void OnGUI()
        {
            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

			if(user == null) return;

            GUI.depth = GuiDepth;

            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(scrollViewRect);

            //  Friend cards
            float cardY = friendMenuTitleUnderLinePosition.y + friendMenuTitleUnderLinePosition.height;

            int count = 0;

            foreach (FresviiGUIFriendCard card in cards)
            {
                count++;

                Rect cardPosition = new Rect(0f, cardY, baseRect.width, card.GetHeight());

                Rect drawPosition = new Rect(cardPosition.x, scrollViewRect.y + cardPosition.y, cardPosition.width, cardPosition.height);

                if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                {
                    card.Draw(cardPosition, count != cards.Count);
                }

                cardY += cardPosition.height;
            }

            GUI.EndGroup();

            GUI.EndGroup();
        }       
    }
}
