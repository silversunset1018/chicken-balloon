using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUILeaderboardTop : MonoBehaviour
    {        
        private Texture2D appIcon;

        private Rect menuRect;

        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;
        private string title = "Leaderboards";

		public Rect appIconPosition;
        public Rect appIconButtonPosition;
       
        public float hMargin;
		public float vMargin = 8f;

        private FresviiGUILeaderboard frameLeaderboards;

        public int guiDepth = -30;

        private Texture2D backIcon;
        public GUIStyle guiStyleBackButton;
        private Rect backButtonPosition;
		private Rect backButtonHitPosition;

        private Color colorNormal;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUILeaderboard frameLeaderboards)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;

                guiStyleTitle.fontStyle = FontStyle.Bold;
                
                guiStyleBackButton.font = null;
            }

            this.appIcon = appIcon;
            
            this.frameLeaderboards = frameLeaderboards;
            
            this.guiDepth = guiDepth;

            title = FresviiGUIText.Get("Leaderboards");

            palette = FresviiGUIColorPalette.Palette;

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            height *= scaleFactor;

            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            hMargin *= scaleFactor;

            vMargin *= scaleFactor;

			appIconPosition = FresviiGUIUtility.RectScale(appIconPosition, scaleFactor);
           
			colorNormal = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            appIconButtonPosition = FresviiGUIUtility.RectScale(appIconButtonPosition, scaleFactor);

        }

        void Update()
        {
            menuRect = new Rect(frameLeaderboards.Position.x, frameLeaderboards.Position.y, Screen.width, height);
        }

        public void OnGUI()
        {
            GUI.depth = guiDepth;

            GUI.BeginGroup(menuRect);

            GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, Screen.width, 1), palette, texCoordsBorderLine);

            //  Mat
            GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, menuRect.width, menuRect.height), palette, texCoordsMenu);

            // AppIcon
            GUI.DrawTexture(appIconPosition, appIcon);

            // Title
            GUI.Label(new Rect(0f, 0f, menuRect.width, menuRect.height), title, guiStyleTitle);
            
            Color tmpColor = GUI.color;

            GUI.color = colorNormal;

            GUI.color = tmpColor;

			Event e = Event.current;

            if (e.type == EventType.MouseUp && appIconButtonPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
                e.Use();

                FresviiGUIManager.Instance.LoadScene();
            }


            GUI.EndGroup();
        }

        
    }
}