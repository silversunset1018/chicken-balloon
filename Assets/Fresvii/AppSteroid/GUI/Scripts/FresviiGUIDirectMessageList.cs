using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
	public class FresviiGUIDirectMessageList : FresviiGUIModalFrame
    {
        public enum Mode { FromUploded, Select, Share };

        public float sideMargin;

        private Texture2D palette;

        private FresviiGUIDirectMessageListTop directMessageListTopMenu;

        private Rect baseRect;
        
        private Rect scrollViewRect;
        
        private float scaleFactor;
        
        public float topMargin;

        public float cardMargin;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;
        
        public Vector2 loadingSpinnerSize;

        private List<FresviiGUIDirectMessageCard> cards = new List<FresviiGUIDirectMessageCard>();

        public GameObject prfbDirectMessageCard;

        private bool initialized;

		public float pollingInterval = 15f;

        public float pullRefleshHeight = 50f;

        private bool loading;

        private bool loadBlock;
		       
        private Fresvii.AppSteroid.Models.ListMeta directMessageListMeta = null;

        private bool isPullUp;

        public bool HasActionSheet = false;

        public static Texture2D TexDirectMessagePlaybackIcon { get; protected set; }

        public static Texture2D TexButtonShare { get; protected set; }

        public static Texture2D TexButtonShareH { get; protected set; }

        public Mode mode = Mode.Select;

        public static Texture2D TexMenu { get; protected set; }

        public static Texture2D TexMenuH { get; protected set; }

        private FresviiGUITabBar tabBar;

        public static Texture2D IconUnread { get; protected set; }

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = guiDepth;

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            directMessageListTopMenu = GetComponent<FresviiGUIDirectMessageListTop>();

			directMessageListTopMenu.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

			tabBar = GetComponent<FresviiGUITabBar>();

            tabBar.Init(postFix, scaleFactor, this.GuiDepth - 1);

            tabBar.GuiDepth = GuiDepth - 1;

			this.scaleFactor = scaleFactor;
            
            loadingSpinnerSize *= scaleFactor;
            
            sideMargin *= scaleFactor;
            
            topMargin *= scaleFactor;

            cardMargin *= scaleFactor;

            loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GuiDepth - 10);

            pullRefleshHeight *= scaleFactor;

            SetScrollSlider(scaleFactor * 2.0f);
			
            directMessageListLoading = true;

            FAS.Instance.Client.DirectMessageService.GetDirectMessageList(1, false, OnGetDirectMessageList);

			if(FASGesture.Instance == null)
            {
				gameObject.AddComponent<FASGesture>();
			}

            TexMenu = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumMenuTextureName + postFix, false);

            TexMenuH = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumMenuHTextureName + postFix, false);

            IconUnread = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconUnread + postFix, false);
		}

        public void OnUpdateScrollViewPosition(float value)
        {
            scrollViewRect.y = value;
        }

        public void OnScrollDelta(float delta)
        {
            scrollViewRect.y += delta;

            scrollViewRect.y = Mathf.Min(0f, scrollViewRect.y);
        }

        void OnGetDirectMessageList(IList<Fresvii.AppSteroid.Models.DirectMessage> _directMessages, Fresvii.AppSteroid.Models.ListMeta _meta, Fresvii.AppSteroid.Models.Error _error)
        {
            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }

            if (bottomLoadingSpinner != null)
            {
                bottomLoadingSpinner.Hide();
            }

            directMessageListLoading = false;


            bool added = false;

            if(_error == null)
            {

                foreach (Fresvii.AppSteroid.Models.DirectMessage directMessage in _directMessages)
                {
                    added |= AddDirectMessage(directMessage);
                }

                this.directMessageListMeta = _meta;
            }

            if (!added)
            {
                scrollViewRect.height = CalcScrollViewHeight();

                OnCompletePullReflesh(scrollViewRect, baseRect);
            }

        }

        bool AddDirectMessage(Fresvii.AppSteroid.Models.DirectMessage directMessage)
        {
            bool exists = false;

            foreach (FresviiGUIDirectMessageCard card in cards)
            {
                if (card.DirectMessage.Id == directMessage.Id)
                {
                    exists = true;

                    break;
                }
            }

            if (!exists)
            {
                FresviiGUIDirectMessageCard card = ((GameObject)Instantiate(prfbDirectMessageCard)).GetComponent<FresviiGUIDirectMessageCard>();

                card.Init(directMessage, scaleFactor, this);

                card.transform.parent = this.transform;

                cards.Add(card);

                return true;
            }

            return false;
        }
       
		void OnEnable()
		{
            ControlLock = false;
		}

        float CalcScrollViewHeight()
        {
            float height = directMessageListTopMenu.height + cardMargin;

            foreach (FresviiGUIDirectMessageCard card in cards)
                height += card.GetHeight() + cardMargin;

            if (bottomLoadingSpinner != null)
            {
                height += pullRefleshHeight;
            }

            return height;
        }

        private Rect directMessageUploadedLabel;

        void Update(){

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            this.baseRect = new Rect(Position.x, Position.y, Screen.width, Screen.height - OffsetPosition.y);


            float scrollViewHeight = CalcScrollViewHeight();

            scrollViewRect.x = Position.x;

            scrollViewRect.width = Screen.width;

            scrollViewRect.height = scrollViewHeight;

            InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, pullRefleshHeight, directMessageListTopMenu.height, tabBar.height, null, OnPullUpReflesh);

            if (loadingSpinner != null)
            {
				loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && PostFrame != null && !ControlLock)
            {
                BackToPostFrame();
            }
#endif

			FASGesture.Use();
		}

        private bool directMessageListLoading;

        private Rect bottomLoadingSpinnerPosition;

        private Fresvii.AppSteroid.Gui.LoadingSpinner bottomLoadingSpinner;

        void OnPullUpReflesh()
        {
            if (directMessageListMeta != null)
            {
                if (directMessageListMeta.NextPage != null && !directMessageListLoading)
                {
                    bottomLoadingSpinnerPosition = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + baseRect.y + baseRect.height - pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f - tabBar.height, loadingSpinnerSize.x, loadingSpinnerSize.y);

                    bottomLoadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(bottomLoadingSpinnerPosition, GuiDepth - 5);

                    directMessageListLoading = true;

                    FAS.Instance.Client.DirectMessageService.GetDirectMessageList((uint)directMessageListMeta.NextPage, false, OnGetDirectMessageList);
                }
            }
        }

        public void BackToPostFrame()
        {
            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }

            if(PostFrame != null)
                PostFrame.SetDraw(true);

            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
            {
                Destroy(this.gameObject);
            });

            if(PostFrame != null)
                PostFrame.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
        }


        public override void SetDraw(bool on)
        {
			if(FresviiGUIManager.Instance != null)
			{
	            if (on)
    	            FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;
			}

            this.enabled = on;
            
            directMessageListTopMenu.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                    loadingSpinner.Hide();
            }
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(scrollViewRect);

            float cardY = directMessageListTopMenu.height + cardMargin;

            foreach (FresviiGUIDirectMessageCard card in cards)
            {
                Rect cardPosition = new Rect(0f, cardY, baseRect.width, card.GetHeight());

                Rect drawPosition = new Rect(cardPosition.x, scrollViewRect.y + cardPosition.y, cardPosition.width, cardPosition.height);

                if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                {
                    card.Draw(cardPosition, scrollViewRect, GuiDepth, directMessageListTopMenu.height);
                }

                cardY += cardPosition.height + cardMargin;
            }

            GUI.EndGroup();

            GUI.EndGroup();

			if (Event.current.isMouse && Event.current.button >= 0)
			{
				Event.current.Use();
			}
        }
    }
}
	