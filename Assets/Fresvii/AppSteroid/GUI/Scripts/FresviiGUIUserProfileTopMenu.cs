using UnityEngine;
using System.Collections;


namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIUserProfileTopMenu : MonoBehaviour
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
		
        private FresviiGUIUserProfile frameUserProfile;

        public int guiDepth = -30;

        private Texture2D backIcon;
        public GUIStyle guiStyleBackButton;
        private Rect backButtonPosition;
		private Rect backButtonHitPosition;

        private Color normalColor;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIUserProfile frameUserProfile)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;
                guiStyleTitle.fontStyle = FontStyle.Bold;
                guiStyleEditButton.font = null;
                guiStyleBackButton.font = null;
            }

            this.frameUserProfile = frameUserProfile;
            
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

			backButtonHitPosition = new Rect(0,0,backButtonPosition.x + backButtonPosition.width + vMargin, height);
        }

        void Update()
        {
            menuRect = new Rect(frameUserProfile.Position.x, frameUserProfile.Position.y, Screen.width, height);
        }

        public void OnGUI()
        {
            GUI.depth = guiDepth;

            GUI.DrawTextureWithTexCoords(new Rect(menuRect.x, menuRect.height + menuRect.y, Screen.width, 1), palette, texCoordsBorderLine);

            editButtonPosition.x = Screen.width - editButtonPosition.width - hMargin;

            //  Mat
            GUI.DrawTextureWithTexCoords(menuRect, palette, texCoordsMenu);

            // Title
            GUI.Label(menuRect, FresviiGUIText.Get("Profile"), guiStyleTitle);

            GUI.BeginGroup(menuRect);

            Color tempColor = GUI.color;

            GUI.color = normalColor;

			GUI.DrawTexture(backButtonPosition, backIcon);

            GUI.color = tempColor;

			Event e = Event.current;
			if(e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
				e.Use();

                frameUserProfile.BackToPostFrame();
            }

            GUI.EndGroup();
        }

        
    }
}