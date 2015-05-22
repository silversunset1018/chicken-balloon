using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIThreadTopMenu : MonoBehaviour
    {        
        private Texture2D backIcon;

        private Texture2D navDownIcon;

        private Rect menuRect;

        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

		public float height;

        public GUIStyle guiStyleTitle;
        private string title;

        public float margin;
        public float miniMargin;
        private Rect backButtonPosition;
        private Rect backButtonHitPosition;

        private Rect navButtonPosition;
        private Rect navButtonHitPosition;
        private float scaleFactor;
        private string postFix;

        private FresviiGUIThread frameThread;

        public GUIStyle guiStyleForumLabel;
        private GUIContent forumLabelContent;
        private Rect forumLabelPosition;

        public int GuiDepth { get; set; }

        public FresviiGUIButton topMenuScrollResetButton;

        public Vector2 popUpOffset;

        private Fresvii.AppSteroid.Gui.PopUpBalloonMenu popUpBaloonMenu;

        private Color normalColor;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;
                guiStyleTitle.fontStyle = FontStyle.Bold;
                guiStyleForumLabel.font = null;
            }

            frameThread = GetComponent<FresviiGUIThread>();

            this.title = FresviiGUIText.Get("Thread");
            this.scaleFactor = scaleFactor;
            this.postFix = postFix;
            this.popUpOffset *= scaleFactor;

            palette = FresviiGUIColorPalette.Palette;

            normalColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            this.backIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);

            this.navDownIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.NavMenuTextureName + postFix, false);

            height *= scaleFactor;

            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);

            guiStyleForumLabel.fontSize = (int)(guiStyleForumLabel.fontSize * scaleFactor);

            guiStyleForumLabel.normal.textColor = normalColor;

            margin *= scaleFactor;
            miniMargin *= scaleFactor;
           
            forumLabelContent = new GUIContent(FresviiGUIText.Get("Forum"));

        }

        void Update()
        {
            menuRect = new Rect(frameThread.Position.x, frameThread.Position.y, Screen.width, height);

            backButtonPosition = new Rect(margin - 4f * scaleFactor, 0.5f * (height - backIcon.height), backIcon.width, backIcon.height);

            backButtonHitPosition = new Rect(0, 0, margin + backIcon.width + miniMargin + guiStyleForumLabel.CalcSize(forumLabelContent).x, height);

            forumLabelPosition = new Rect(backButtonPosition.x + backButtonPosition.width + miniMargin, 0f, menuRect.width, height);

            navButtonPosition = new Rect(menuRect.width - margin * 2f - navDownIcon.width - 1f * scaleFactor, 0.5f * (height - navDownIcon.height), navDownIcon.width, navDownIcon.height);

            navButtonHitPosition = new Rect(menuRect.width - margin * 2f - navDownIcon.width, 0f, navDownIcon.width + 2f * margin, height);

        }

        void OnDisable()
        {
            if (popUpBaloonMenu != null)
            {
                popUpBaloonMenu.Hide();
            }
        }

        void OnGUI()
        {
            Event e = Event.current;

            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(new Rect(0, menuRect.y + menuRect.height, Screen.width, 1), palette, texCoordsBorderLine);

            //  Mat
            GUI.DrawTextureWithTexCoords(menuRect, palette, texCoordsMenu);

            // Title
            GUI.Label(menuRect, title, guiStyleTitle);

            GUI.BeginGroup(menuRect);

            
            // BackIcon
            Color tempColor = GUI.color;

            GUI.color = normalColor;

            GUI.DrawTexture(backButtonPosition, backIcon);

            GUI.color = tempColor;

			if (e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
                e.Use();

                frameThread.BackToForum();
            }

            GUI.Label(forumLabelPosition, forumLabelContent.text, guiStyleForumLabel);

            // NavIcon
            GUI.color = normalColor;

            GUI.DrawTexture(navButtonPosition, navDownIcon);

            GUI.color = tempColor;

			if (e.type == EventType.MouseUp && navButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
                e.Use();

                Vector2 position = new Vector2(navButtonPosition.x, navButtonPosition.y);

                FresviiGUIThreadCard currentThreadCard = frameThread.threadCard;

                List<string> buttons = new List<string>();
                
                buttons.Add((frameThread.threadCard.Thread.Subscribed) ? FresviiGUIText.Get("Unsubscribe") : FresviiGUIText.Get("Subscribe"));

                if (frameThread.threadCard.Thread.User.Id == FAS.CurrentUser.Id)
                {
                    buttons.Add(FresviiGUIText.Get("Delete"));
                }

                popUpBaloonMenu = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/PopUpBalloonMenu"))).GetComponent<Fresvii.AppSteroid.Gui.PopUpBalloonMenu>();

                popUpBaloonMenu.Show(buttons.ToArray(), popUpOffset + position + FresviiGUIFrame.OffsetPosition, scaleFactor, postFix, this.GuiDepth - 10, Color.black, Color.white, Color.white, delegate(string selectedButton)
                {
                    if (selectedButton == FresviiGUIText.Get("Unsubscribe"))
                    {
                        currentThreadCard.Unsubscribe();
                    }
                    else if (selectedButton == FresviiGUIText.Get("Subscribe"))
                    {
                        currentThreadCard.Subscribe();
                    }
                    else if (selectedButton == FresviiGUIText.Get("Delete"))
                    {
                        currentThreadCard.DeleteThread(true);
                    }
                });
            }

            if (popUpBaloonMenu != null)
            {
                popUpBaloonMenu.SetPosition(popUpOffset + new Vector2(navButtonPosition.x, navButtonPosition.y) + FresviiGUIFrame.OffsetPosition);
            }

           

            if (topMenuScrollResetButton.IsTap(e, menuRect))
            {
                frameThread.ResetScrollPositionTween();
            }

            GUI.EndGroup();
        }
    }
}