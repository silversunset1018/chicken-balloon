using UnityEngine;
using System.Collections;


namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIForumTopMenu : MonoBehaviour
    {        
        private Texture2D appIcon;
        private Texture2D penButton;

        private Rect menuRect;

        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;
        private string title;

		public Rect appIconPosition;
        public Rect appIconButtonPosition;

        private Rect penButtonPosition;
        private Rect penButtonHitPosition;

        public GUIStyle guiStyleButton;
        public Vector2 searchButtonPosition;

        public int GuiDepth { get; set; }

        public float sideMargin = 12;

        private FresviiGUIForum frameForum;
        public FresviiGUIButton topMenuScrollResetButton;

        public bool draw;

        private Color iconColor;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, string title)
        {
            frameForum = GetComponent<FresviiGUIForum>();

            this.appIcon = appIcon;

            this.title = title;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;
                guiStyleTitle.fontStyle = FontStyle.Bold;
                guiStyleButton.font = null;
            }

            palette = FresviiGUIColorPalette.Palette;

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            iconColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            this.penButton = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.PenButtonTextureName + postFix, false);

            height *= scaleFactor;
            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);
            sideMargin *= scaleFactor;

			appIconPosition = FresviiGUIUtility.RectScale(appIconPosition, scaleFactor);
            appIconButtonPosition = FresviiGUIUtility.RectScale(appIconButtonPosition, scaleFactor);
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        void Update()
        {           
            menuRect = new Rect(frameForum.Position.x, frameForum.Position.y, Screen.width, height);

            penButtonPosition = new Rect(menuRect.width - sideMargin - penButton.width, height * 0.5f - penButton.height * 0.5f, penButton.width, penButton.height);

            penButtonHitPosition = new Rect(menuRect.width - 2f * sideMargin - penButton.width, 0f, 2f * sideMargin + penButton.width, height);            
        }

        public void OnGUI()
        {
            //if (frameForum.Draw)
            //{
                GUI.depth = GuiDepth;

                Event e = Event.current;

                GUI.DrawTextureWithTexCoords(new Rect(menuRect.x, menuRect.height + menuRect.y, Screen.width, 1), palette, texCoordsBorderLine);

                //  Mat
                GUI.DrawTextureWithTexCoords(menuRect, palette, texCoordsMenu);
                
                // Title
                GUI.Label(menuRect, title, guiStyleTitle);

                GUI.BeginGroup(menuRect);
                
                // AppIcon
                GUI.DrawTexture(appIconPosition, appIcon);
				if (e.type == EventType.MouseUp && appIconButtonPosition.Contains(e.mousePosition)&& !FASGesture.IsDragging)
                {
                    e.Use();

                    FresviiGUIManager.Instance.LoadScene();
                }

                Color tmpColor = GUI.color;

                GUI.color = iconColor;
                
                GUI.DrawTexture(penButtonPosition, penButton);

                GUI.color = tmpColor;

                if (e.type == EventType.MouseUp && penButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
                {
                    e.Use();
                
                    //FresviiGUIManager.Instance.SetMode(FresviiGUIManager.Mode.CreateThread);

                    frameForum.GoToCreateThread();
                }

                if (topMenuScrollResetButton.IsTap(e, menuRect))
                {
                    frameForum.ResetScrollPositionTween();
                }

                GUI.EndGroup();
            //}

        }
    }
}