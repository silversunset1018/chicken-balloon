using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIFriendRequests : FresviiGUIFrame
    {
        public float sideMargin = 8;

        public float topMargin = 32;
        
        public float hMargin = 18;
        
        public float miniMargin = 9;
        
        public float verticalMargin = 10;

        public bool Initialized { get; protected set; }

        private FresviiGUIFriendRequestsTopMenu friendRequestTopMenu;
        
        private FresviiGUITabBar tabBar;

        private Rect baseRect;
        
        private Rect scrollViewRect;
        
        private float scaleFactor;
        
        private string postFix;

        public FresviiGUIMyProfile frameMyProfile;

        private List<FresviiGUIFriendRequestCard> selectedCards;

        private Fresvii.AppSteroid.Models.ListMeta requestedMeta;

        private List<FresviiGUIFriendRequestCard> requestedCards = new List<FresviiGUIFriendRequestCard>();

        private Fresvii.AppSteroid.Models.ListMeta hiddenMeta;

        private List<FresviiGUIFriendRequestCard> hiddenCards = new List<FresviiGUIFriendRequestCard>();

        public GameObject prfbFriendRequestCard;

        public float pullResetTweenTime;
        
        public iTween.EaseType pullRestTweenEaseType;

        public static Texture2D TexButtonAdd { get; protected set; }

        public static Texture2D TexButtonAddH { get; protected set; }
        
        public static Texture2D TexButtonHide { get; protected set; }
        
        public static Texture2D TexButtonHideH { get; protected set; }

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        public Vector2 loadingSpinnerSize;

        private Rect loadingSpinnerPosition;

        public FresviiGUISegmentedControl segmentedCtrl;

        private List<string> labels;

        private Rect segmentedCtrlPosition;

        private Rect segmentedCtrlBgPosition;

        public float segmentedCtrlHeight = 30f;

        public float pullRefleshHeight = 50f;

        private int selectedIndex = 0;

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = guiDepth;

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            friendRequestTopMenu = GetComponent<FresviiGUIFriendRequestsTopMenu>();

            friendRequestTopMenu.Init(appIcon, postFix, scaleFactor, this.GuiDepth - 10, this);

            tabBar = GetComponent<FresviiGUITabBar>();

            tabBar.Init(postFix, scaleFactor, this.GuiDepth - 1);

            tabBar.GuiDepth = GuiDepth - 1;

            this.scaleFactor = scaleFactor;
        
            sideMargin *= scaleFactor;
            
            verticalMargin *= scaleFactor;
            
            miniMargin *= scaleFactor;
            
            topMargin *= scaleFactor;
            
            loadingSpinnerSize *= scaleFactor;

            segmentedCtrlHeight *= scaleFactor;

            pullRefleshHeight *= scaleFactor;

            TexButtonAdd = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button02TextureName + postFix, false);

            TexButtonAddH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button02HTextureName + postFix, false);

            TexButtonHide = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button03TextureName + postFix, false);
            
            TexButtonHideH = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.Button03HTextureName + postFix, false);

            Initialized = true;

            SetScrollSlider(scaleFactor * 2.0f);

            FASFriendship.GetFriendshipRequestedUsersList(FAS.CurrentUser.Id, OnGetFriendshipRequestedUsersList);

            FASFriendship.GetHiddenFriendshipRequestedUsersList(OnGetHiddenFriendshipRequestedUsersList);

            loadingSpinnerPlace = LoadingSpinnerPlace.Center;

            loadingSpinnerPosition = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition);

            labels = new List<string>();

            labels.Add(FresviiGUIText.Get("Requested"));

            labels.Add(FresviiGUIText.Get("NotNow"));

            selectedCards = requestedCards;

            segmentedCtrl.Init(scaleFactor, postFix, labels, OnTapSegmentedControl);
        }

        void OnGetFriendshipRequestedUsersList(IList<Fresvii.AppSteroid.Models.Friend> requestedList, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
        {
            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }

            loading = false;

            if (pullRefleshing)
            {
                if (isPullUp)
                {

                }
                else
                {
                    OnCompletePullReflesh(scrollViewRect, baseRect);
                }
            }

            pullRefleshing = false;

            isPullUp = false;

            if (error == null)
            {
                if (requestedMeta == null)
                {
                    requestedMeta = meta;
                }
                else if (requestedMeta.CurrentPage != 1)
                {
                    requestedMeta = meta;
                }

                foreach (Fresvii.AppSteroid.Models.Friend friend in requestedList)
                {
                    bool exists = false;

                    foreach (FresviiGUIFriendRequestCard card in requestedCards)
                    {
                        if (card.User.Id == friend.Id)
                        {
                            exists = true;

                            break;
                        }
                    }

                    if (!exists)
                    {
                        CreateCard(friend, false, (card) =>
                        {
                            requestedCards.Add(card);
                        });
                    }
                }
            }
            else
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                {
                    Debug.LogError(error.ToString());
                }
            }
        }

        void OnGetHiddenFriendshipRequestedUsersList(IList<Fresvii.AppSteroid.Models.Friend> requestedList, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
        {
            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }

            loading = false;

            if (pullRefleshing)
            {
                if (isPullUp)
                {

                }
                else
                {
                    OnCompletePullReflesh(scrollViewRect, baseRect);
                }
            }

            pullRefleshing = false;

            isPullUp = false;

            if (error == null)
            {
                if (hiddenMeta == null)
                {
                    hiddenMeta = meta;
                }
                else if (hiddenMeta.CurrentPage != 1)
                {
                    hiddenMeta = meta;
                }

                foreach (Fresvii.AppSteroid.Models.Friend friend in requestedList)
                {
                    bool exists = false;

                    foreach (FresviiGUIFriendRequestCard card in hiddenCards)
                    {
                        if (card.User.Id == friend.Id)
                        {
                            exists = true;

                            break;
                        }
                    }

                    if (!exists)
                    {
                        CreateCard(friend, true, (card) =>
                        {
                            hiddenCards.Add(card);
                        });
                    }
                }
            }
            else
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                {
                    Debug.LogError(error.ToString());
                }
            }
        }

        public void OnUpdateScrollViewPosition(float value)
        {
            scrollViewRect.y = value;
        }

        void CreateCard(Fresvii.AppSteroid.Models.Friend friend, bool isHiddenCard, Action<FresviiGUIFriendRequestCard> callback)
        {
            FASUser.GetUser(friend.Id, delegate(Fresvii.AppSteroid.Models.User user, Fresvii.AppSteroid.Models.Error error)
            {
                if (error == null)
                {
                    FresviiGUIFriendRequestCard card = ((GameObject)Instantiate(prfbFriendRequestCard)).GetComponent<FresviiGUIFriendRequestCard>();

                    card.Init(user, scaleFactor, isHiddenCard, this);

                    callback(card);
                }
            });
        }

        void OnTapSegmentedControl(int index)
        {
            selectedIndex = index;

            selectedCards = (selectedIndex == 0) ? requestedCards : hiddenCards;
        }

        float screenWidth;

        void CalcLayout()
        {
            segmentedCtrlPosition = new Rect(sideMargin, sideMargin, Screen.width - 2f * sideMargin, segmentedCtrlHeight);

            segmentedCtrlBgPosition = new Rect(0f, 0f, Screen.width, segmentedCtrlHeight + 2f * sideMargin);
        }

        private bool loadBlock;

        private bool loading;

        void Update()
        {
            backgroundRect = new Rect(Position.x, Position.y + friendRequestTopMenu.height, Screen.width, Screen.height - friendRequestTopMenu.height);
            
            if (screenWidth != Screen.width)
            {
                CalcLayout();

                screenWidth = Screen.width;
            }

            baseRect = new Rect(0f, segmentedCtrlBgPosition.height, Screen.width, Screen.height - friendRequestTopMenu.height - segmentedCtrlBgPosition.height - FresviiGUIFrame.OffsetPosition.y);

            if (loadingSpinner != null)
            {
                if (loadingSpinnerPlace == LoadingSpinnerPlace.Center)
                {
                    loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
                else if (loadingSpinnerPlace == LoadingSpinnerPlace.Top)
                {
                    loadingSpinner.Position = new Rect(baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + segmentedCtrlBgPosition.height + pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
                else if (loadingSpinnerPlace == LoadingSpinnerPlace.Bottom)
                {
                    loadingSpinner.Position = new Rect(baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + baseRect.height - pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
                }
            }

            float scrollViewHeight = CalcScrollViewHeight();

            scrollViewRect.x = Position.x;

            scrollViewRect.width = Screen.width;

            scrollViewRect.height = scrollViewHeight;

            InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, pullRefleshHeight, OnPullDownReflesh, OnPullUpReflesh);

            if (loadBlock && FASGesture.IsTouchEnd)
            {
                loadBlock = false;
            }

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && !ControlLock)
            {
                GoToMyProfile();
            }
#endif
        }
        
        private enum LoadingSpinnerPlace { Top, Center, Bottom };

        private LoadingSpinnerPlace loadingSpinnerPlace = LoadingSpinnerPlace.Center;

        void OnPullDownReflesh()
        {
            if (loading || loadBlock) return;

            loading = true;

            loadBlock = true;

            loadingSpinnerPlace = LoadingSpinnerPlace.Top;

            loadingSpinner.Position = new Rect(baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + segmentedCtrlBgPosition.height + pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinner.Position, this.GuiDepth - 1);

            pullRefleshing = true;

            if (selectedIndex == 0)
            {
                FASFriendship.GetFriendshipRequestedUsersList(FAS.CurrentUser.Id, OnGetFriendshipRequestedUsersList); 
            }
            else
            {
                FASFriendship.GetHiddenFriendshipRequestedUsersList(OnGetHiddenFriendshipRequestedUsersList);
            }
        }

        private bool isPullUp;

        void OnPullUpReflesh()
        {
            uint loadPage = 2;

            Fresvii.AppSteroid.Models.ListMeta selectedMeta = (selectedIndex == 0 ) ? requestedMeta : hiddenMeta;

            if (selectedMeta != null)
            {
                if (selectedMeta.NextPage.HasValue)
                {
                    loadPage = (uint)selectedMeta.NextPage;
                }
                else
                {
                    loadPage = selectedMeta.TotalCount / selectedMeta.PerPage + 1;
                }
            }

            if (loading || loadBlock) return;

            loading = true;

            loadBlock = true;

            loadingSpinnerPlace = LoadingSpinnerPlace.Bottom;

            loadingSpinner.Position = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + baseRect.y + baseRect.height - pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinner.Position, this.GuiDepth - 1);

            pullRefleshing = true;

            isPullUp = true;

            if (selectedIndex == 0)
            {
                FASFriendship.GetFriendshipRequestedUsersList(FAS.CurrentUser.Id, loadPage, false, OnGetFriendshipRequestedUsersList);
            }
            else
            {
                FASFriendship.GetHiddenFriendshipRequestedUsersList(loadPage, OnGetHiddenFriendshipRequestedUsersList);
            }
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            friendRequestTopMenu.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                {
                    loadingSpinner.Hide();
                }
            }
        }

        void OnCompletePull()
        {

        }

        float CalcScrollViewHeight()
        {           
            float height = sideMargin;

            foreach (FresviiGUIFriendRequestCard card in selectedCards)
            {
                height += card.GetHeight();

                height += sideMargin;
            }

            height += sideMargin;

            return height;
        }

        public void SetGUIMyProfile(FresviiGUIMyProfile frameMyProfile)
        {
            this.frameMyProfile = frameMyProfile;
        }

        public void RemoveCard(FresviiGUIFriendRequestCard card, bool hidden)
        {
            if (hidden)
            {
                requestedCards.Remove(card);

                hiddenCards.Add(card);

                card.Reflesh();

                card.isHiddenCard = true;

                requestedMeta.TotalCount--;
            }
            else // confirm
            {
                if (!card.isHiddenCard)
                {
                    frameMyProfile.RemoveRequestedUser(card.User);

                    requestedCards.Remove(card);
                }

                Destroy(card.gameObject);
            }
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.BeginGroup(backgroundRect);

            segmentedCtrl.Draw(segmentedCtrlPosition, Event.current);

            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(scrollViewRect);

            float cardVpos = sideMargin;

            foreach (FresviiGUIFriendRequestCard card in selectedCards)
            {
                Rect cardPosition = new Rect(sideMargin, cardVpos, Screen.width - 2 * sideMargin, card.GetHeight());

      			Rect drawPosition = new Rect(cardPosition.x, scrollViewRect.y + cardPosition.y, cardPosition.width, cardPosition.height);

                if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                {
                    card.Draw(cardPosition);
                }

                cardVpos += cardPosition.height + verticalMargin;
            }

            GUI.EndGroup();

            GUI.EndGroup();

            GUI.EndGroup();
        }

        public void GoToMyProfile()
        {
            frameMyProfile.SetDraw(true);

            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
            {
                Destroy(this.gameObject);
            });

            frameMyProfile.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });

            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }
        }
    }
}
