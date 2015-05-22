using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUITabBar : MonoBehaviour
    {
        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        private Rect menuRect;

        private Texture2D forumButton;
        private Texture2D leaderboardButton;
        private Texture2D groupMessageButton;
        private Texture2D profileButton;

        public float height;

        private Color onColor;
        private Color offColor;

		private Vector2 buttonSize;

		public float topMargin = 12;
		public float middleMargin = 3;
		public float bottomMargin = 12;

		public GUIStyle guiStyleButton;
		public GUIStyle guiStyleLabel;

        public int GuiDepth {get;set;}

        public FASGui.Mode viewMode;

        public static uint profileBadgeCount = 0;

        public static uint messageBadgeCount = 0;

        private Texture2D textureBadge;

        public GUIStyle guiStyleBadge;

        private Color badgeBgColor;

        public void Init(string postFix, float scaleFactor, int guiDepth)
        {
            this.enabled = (FresviiGUIManager.Instance.ModeCount > 1);

            this.GuiDepth = guiDepth;

            if (!enabled)
            {
                this.height = 0.0f;

                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleLabel.font = null;

                guiStyleBadge.font = null;

                guiStyleBadge.fontStyle = FontStyle.Bold;
            }

            palette = FresviiGUIColorPalette.Palette;

            onColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TabBarPositive);

            offColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TabBarNegative);

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.TabBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.TabBarTopLine);

            this.forumButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.ForumTabButtonTextureName + postFix, false);

            this.leaderboardButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.LeaderboardsTabButtonTextureName + postFix, false);

            this.groupMessageButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.GroupChatTabButtonTextureName + postFix, false);
            
            this.profileButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.MyProfileTabButtonTextureName + postFix, false);

            this.textureBadge = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BadgeTextureName + postFix, false);

            height *= scaleFactor;
			            
			topMargin *= scaleFactor;
			middleMargin *= scaleFactor;
			bottomMargin *= scaleFactor;

			guiStyleLabel.fontSize = (int)(guiStyleLabel.fontSize * scaleFactor);

            guiStyleBadge.fontSize = (int)(guiStyleBadge.fontSize * scaleFactor);

            guiStyleBadge.padding = FresviiGUIUtility.RectOffsetScale(guiStyleBadge.padding, scaleFactor);

            guiStyleBadge.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TabBarBadgeTextColor);

            badgeBgColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TabBarBadgeBgColor);

        }

        void Update()
        {
            if(FresviiGUIManager.Instance.ModeCount < 2)
            {
                this.enabled = false;
                return;
            }


            menuRect = new Rect(0, Screen.height - height, Screen.width, height);

            buttonSize = new Vector2(menuRect.width / FresviiGUIManager.Instance.ModeCount, forumButton.height);

        }

        public void OnGUI()
        {
            GUI.depth = GuiDepth;

            Event e = Event.current;

            GUI.DrawTextureWithTexCoords(menuRect, palette, texCoordsMenu);

            GUI.DrawTextureWithTexCoords(new Rect(0, menuRect.y - 1, Screen.width, 1), palette, texCoordsBorderLine);

            GUILayout.BeginArea(menuRect);

            GUILayout.Space(topMargin);

            GUILayout.BeginHorizontal();

            int hpos = 0;

            if ((FresviiGUIManager.GuiMode & FASGui.Mode.Forum) > 0)
            {
                GUI.color = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.Forum) ? onColor : offColor;

                Rect buttonRect = new Rect(hpos * buttonSize.x, 0f, buttonSize.x, menuRect.height);
                if (buttonRect.Contains(e.mousePosition) && e.type == EventType.MouseUp && !FASGesture.IsDragging)
                {
                    e.Use();

                    bool goToTop = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.Forum);

                    FresviiGUIManager.Instance.SetViewMode(FASGui.Mode.Forum);

                    if (goToTop)
                    {
                        FresviiGUIManager.Instance.SetTopFrame();
                    }
                }

                GUILayout.Button(forumButton, guiStyleButton, GUILayout.Width(buttonSize.x));

                hpos++;
            }

            if ((FresviiGUIManager.GuiMode & FASGui.Mode.Leaderboards) > 0)
            {
                GUI.color = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.Leaderboards) ? onColor : offColor;

                Rect buttonRect = new Rect(hpos * buttonSize.x, 0f, buttonSize.x, menuRect.height);
                
                if (buttonRect.Contains(e.mousePosition) && e.type == EventType.MouseUp && !FASGesture.IsDragging)
                {
                    e.Use();

                    bool goToTop = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.Leaderboards);

                    FresviiGUIManager.Instance.SetViewMode(FASGui.Mode.Leaderboards);

                    if (goToTop)
                    {
                        FresviiGUIManager.Instance.SetTopFrame();
                    }
                }

                GUILayout.Button(leaderboardButton, guiStyleButton, GUILayout.Width(buttonSize.x));

                hpos++;
            }

            if ((FresviiGUIManager.GuiMode & FASGui.Mode.GroupMessage) > 0)
            {
                GUI.color = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.GroupMessage) ? onColor : offColor;

                Rect buttonRect = new Rect(hpos * buttonSize.x, 0f, buttonSize.x, menuRect.height);

                if (buttonRect.Contains(e.mousePosition) && e.type == EventType.MouseUp && !FASGesture.IsDragging)
                {
                    e.Use();

                    bool goToTop = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.GroupMessage);

                    FresviiGUIManager.Instance.SetViewMode(FASGui.Mode.GroupMessage);

                    if (goToTop)
                    {
                        FresviiGUIManager.Instance.SetTopFrame();
                    }
                }

                GUILayout.Button(groupMessageButton, guiStyleButton, GUILayout.Width(buttonSize.x));

                if (messageBadgeCount > 0)
                {
                    string badgeString = (messageBadgeCount > 20) ? "20+" : messageBadgeCount.ToString();

                    Vector2 countSize = guiStyleBadge.CalcSize(new GUIContent(badgeString));

                    float badgeWidth = Mathf.Max(countSize.x, textureBadge.width);

                    Rect badgePosition = new Rect(buttonRect.x + buttonRect.width * 0.5f + profileButton.width * 0.5f - badgeWidth * 0.5f, buttonRect.y + buttonRect.height * 0.5f - profileButton.height * 0.5f - textureBadge.height * 0.5f, badgeWidth, textureBadge.height);

                    Color tmp = GUI.color;

                    GUI.color = badgeBgColor;

                    FresviiGUIUtility.DrawSplitTexture(badgePosition, textureBadge, 2f);

                    GUI.color = Color.white;

                    GUI.Label(badgePosition, badgeString, guiStyleBadge);

                    GUI.color = tmp;
                }

                hpos++;
            }

            if ((FresviiGUIManager.GuiMode & FASGui.Mode.MyProfile) > 0)
            {
                GUI.color = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.MyProfile) ? onColor : offColor;

                Rect buttonRect = new Rect(hpos * buttonSize.x, 0f, buttonSize.x, menuRect.height);
             
                if (e.type == EventType.MouseUp && buttonRect.Contains(e.mousePosition) && !FASGesture.IsDragging)
                {
                    e.Use();

                    bool goToTop = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.MyProfile);

                    FresviiGUIManager.Instance.SetViewMode(FASGui.Mode.MyProfile);

                    if (goToTop)
                    {
                        FresviiGUIManager.Instance.SetTopFrame();
                    }
                }

                GUILayout.Button(profileButton, guiStyleButton, GUILayout.Width(buttonSize.x));

                if (profileBadgeCount > 0)
                {
                    string badgeString = (profileBadgeCount > 20) ? "20+" : profileBadgeCount.ToString();

                    Vector2 countSize = guiStyleBadge.CalcSize(new GUIContent(badgeString));

                    float badgeWidth = Mathf.Max(countSize.x, textureBadge.width);

                    Rect badgePosition = new Rect(buttonRect.x + buttonRect.width * 0.5f + profileButton.width * 0.5f - badgeWidth * 0.5f, buttonRect.y + buttonRect.height * 0.5f - profileButton.height * 0.5f - textureBadge.height * 0.5f, badgeWidth, textureBadge.height);

                    Color tmp = GUI.color;

                    GUI.color = badgeBgColor;

                    FresviiGUIUtility.DrawSplitTexture(badgePosition, textureBadge, 2f);

                    GUI.color = Color.white;

                    GUI.Label(badgePosition, badgeString, guiStyleBadge);

                    GUI.color = tmp;
                }
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(middleMargin);

            GUILayout.BeginHorizontal();

            if ((FresviiGUIManager.GuiMode & FASGui.Mode.Forum) > 0)
            {
                GUI.color = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.Forum) ? onColor : offColor;

                GUILayout.Label(FresviiGUIText.Get("Forum"), guiStyleLabel, GUILayout.Width(buttonSize.x));
            }

            if ((FresviiGUIManager.GuiMode & FASGui.Mode.Leaderboards) > 0)
            {
                GUI.color = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.Leaderboards) ? onColor : offColor;

                GUILayout.Label(FresviiGUIText.Get("Leaderboards"), guiStyleLabel, GUILayout.Width(buttonSize.x));
            }

            if ((FresviiGUIManager.GuiMode & FASGui.Mode.GroupMessage) > 0)
            {
                GUI.color = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.GroupMessage) ? onColor : offColor;

                GUILayout.Label(FresviiGUIText.Get("Messages"), guiStyleLabel, GUILayout.Width(buttonSize.x));
            }

            if ((FresviiGUIManager.GuiMode & FASGui.Mode.MyProfile) > 0)
            {
                GUI.color = (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.MyProfile) ? onColor : offColor;

                GUILayout.Label(FresviiGUIText.Get("MyProfile"), guiStyleLabel, GUILayout.Width(buttonSize.x));
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(bottomMargin);

            GUILayout.EndArea();
            

        }
    }
}