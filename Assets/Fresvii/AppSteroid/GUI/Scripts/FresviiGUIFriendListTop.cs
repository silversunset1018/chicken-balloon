using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIFriendListTop : MonoBehaviour
    {        
        private Texture2D penButton;
        private Texture2D searchButton;

        private Rect menuRect;

        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;

        public float titleFontSize;

		public Rect appIconPosition;

        public GUIStyle guiStyleEditButton;
        public Rect editButtonPosition;
        
        public float hMargin;
		public float vMargin = 8f;
		
        private FresviiGUIFriendList frameFriendList;

        public int guiDepth = -30;

        private Texture2D backIcon;
        public GUIStyle guiStyleBackButton;
        private Rect backButtonPosition;
		private Rect backButtonHitPosition;

        private Color normalColor;

        public Texture2D plusIcon;

        private Rect plusButtonPosition;
        
        private Rect plusButtonHitPosition;

        private Fresvii.AppSteroid.Models.ListMeta friendsListMeta = null;

        private Fresvii.AppSteroid.Models.User user;

        private Rect titlePosition;

        private GUIContent titleContent = new GUIContent("");

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIFriendList frameFriendList)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;

                guiStyleTitle.fontStyle = FontStyle.Bold;
                
                guiStyleEditButton.font = null;
                
                guiStyleBackButton.font = null;
            }

            this.frameFriendList = frameFriendList;
            
            this.guiDepth = guiDepth;

            palette = FresviiGUIColorPalette.Palette;

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            normalColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            this.backIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);

            height *= scaleFactor;

            guiStyleTitle.fontSize = (int)(titleFontSize * scaleFactor);
            
            guiStyleEditButton.fontSize = (int)(guiStyleEditButton.fontSize * scaleFactor);
            
            hMargin *= scaleFactor;
			
            vMargin *= scaleFactor;

			appIconPosition = FresviiGUIUtility.RectScale(appIconPosition, scaleFactor);

            editButtonPosition = FresviiGUIUtility.RectScale(editButtonPosition, scaleFactor);

			backButtonPosition = new Rect(vMargin, 0.5f *(height - backIcon.height), backIcon.width, backIcon.height);

			backButtonHitPosition = new Rect(0, 0, backButtonPosition.x + backButtonPosition.width + vMargin, height);
        }

        public void SetTitle(Fresvii.AppSteroid.Models.User user, Fresvii.AppSteroid.Models.ListMeta meta)
        {
            this.user = user;

            this.friendsListMeta = meta;

            LayoutTitle();
        }

        void LayoutTitle()
        {
            if (user == null || friendsListMeta == null)
            {
                return;
            }

            string title = user.Name + " : " + friendsListMeta.TotalCount + " " + FresviiGUIText.Get("Friends");

            titleContent = new GUIContent(title);

            if(guiStyleTitle.CalcSize(titleContent).x > titlePosition.width)
            {
                string truncatedTitle = FresviiGUIUtility.Truncate(title, guiStyleTitle, titlePosition.width, "..." + " : " + friendsListMeta.TotalCount + " " + FresviiGUIText.Get("Friends"));
 
                titleContent = new GUIContent(truncatedTitle);
            }

        }

        private float postScreenWidth = 0f;

        void Update()
        {
            menuRect = new Rect(frameFriendList.Position.x, frameFriendList.Position.y, Screen.width, height);

            plusButtonPosition = new Rect(menuRect.width - backIcon.width - vMargin, 0.5f * (height - backIcon.height), backIcon.width, backIcon.height);

            plusButtonHitPosition = new Rect(menuRect.width - backIcon.width - 2f * vMargin, 0, backButtonPosition.width + 2f * vMargin, height);

            titlePosition = new Rect(backButtonPosition.x + backButtonPosition.width + vMargin, 0f, menuRect.width - 2f * plusButtonPosition.width - 4f * vMargin, height);

            if (postScreenWidth != Screen.width)
            {
                postScreenWidth = Screen.width;

                LayoutTitle();
            }

        }

        public void OnGUI()
        {
            GUI.depth = guiDepth;

            GUI.DrawTextureWithTexCoords(new Rect(menuRect.x, menuRect.height + menuRect.y, Screen.width, 1), palette, texCoordsBorderLine);

            editButtonPosition.x = Screen.width - editButtonPosition.width - hMargin;

            //  Mat
            GUI.DrawTextureWithTexCoords(menuRect, palette, texCoordsMenu);
           
            GUI.BeginGroup(menuRect);

            // Title
            GUI.Label(titlePosition, titleContent, guiStyleTitle);

            Color tempColor = GUI.color;

            GUI.color = normalColor;

			GUI.DrawTexture(backButtonPosition, backIcon);

            GUI.DrawTexture(plusButtonPosition, plusIcon);

            GUI.color = tempColor;

			Event e = Event.current;

			if(e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
				e.Use();

                frameFriendList.BackToPostFrame();
            }

            if (e.type == EventType.MouseUp && plusButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
                e.Use();

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowAlertPromptDialog(FresviiGUIText.Get("AddUser"), FresviiGUIText.Get("InputUserCode"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), delegate(bool ok, string text)
                {
                    if (ok)
                    {
                        FASUser.GetUser(text, delegate(Fresvii.AppSteroid.Models.User user, Fresvii.AppSteroid.Models.Error error)
                        {
                            if (error == null)
                            {
                                frameFriendList.GoToUserProfile(user);
                            }
                            else
                            {
                                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Error"),FresviiGUIText.Get("UserNotFound"), delegate(bool del){});
                            }
                        });
                    }
                });
            }

            GUI.EndGroup();
        }

        
    }
}