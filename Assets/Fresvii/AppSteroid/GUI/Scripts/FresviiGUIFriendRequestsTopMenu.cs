using UnityEngine;
using System.Collections;


namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIFriendRequestsTopMenu : MonoBehaviour
    {        
        private Rect baseRect;

        private Texture2D palette;
        
        private Rect texCoordsMenu;
        
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;

        private string title = "Friend Requests";

        public GUIStyle guiStyleBackButton;

		private Rect backButtonPosition;
		
        private Rect backButtonHitPosition;
        
        private float scaleFactor;
		
        public float vMargin = 8f;

        private FresviiGUIFriendRequests frameFriendRequests;

        public int guiDepth = -30;

        private Texture2D backIcon;

        private Color colorNormal;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIFriendRequests frameFriendRequests)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;
             
                guiStyleTitle.fontStyle = FontStyle.Bold;
                
                guiStyleBackButton.font = null;
            }

            this.scaleFactor = scaleFactor;
        
            this.title = FresviiGUIText.Get("FriendRequests");
            
            this.frameFriendRequests = frameFriendRequests;

            this.guiDepth = guiDepth;

            palette = FresviiGUIColorPalette.Palette;

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            this.backIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);

            height *= scaleFactor;
            
            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);

            vMargin *= scaleFactor;

            colorNormal = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        void Update()
        {
            baseRect = new Rect(frameFriendRequests.Position.x, frameFriendRequests.Position.y, Screen.width, height);
           
            backButtonPosition = new Rect(vMargin - 4f * scaleFactor, (height - backIcon.height) * 0.5f, backIcon.width, backIcon.height);

            backButtonHitPosition = new Rect(0f, 0f, vMargin - 4f * scaleFactor + backIcon.width, height);
        }

        void OnGUI()
        {
            GUI.depth = guiDepth;

			Event e = Event.current;

            GUI.DrawTextureWithTexCoords(new Rect(baseRect.x, baseRect.height + baseRect.y, Screen.width, 1), palette, texCoordsBorderLine);

            //  Mat
            GUI.DrawTextureWithTexCoords(baseRect, palette, texCoordsMenu);
            
            // Title
            GUI.Label(baseRect, title, guiStyleTitle);

            GUI.BeginGroup(baseRect);

            Color tmpColor = GUI.color;

            GUI.color = colorNormal;

            // BackIcon
			GUI.DrawTexture(backButtonPosition, backIcon);

            GUI.color = tmpColor;

			if(e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
				e.Use();

                frameFriendRequests.GoToMyProfile();
            }
            
            GUI.EndGroup();

        }
    }
}