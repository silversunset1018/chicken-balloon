using UnityEngine;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIDirectMessageListTop : MonoBehaviour
    {    
        private Rect menuRect;

        private Texture2D palette;

		private Rect texCoordsMenu;
        
        public float height;

        public GUIStyle guiStyleTitle;
        
		private string title = "DirectMessages";
				        
        public float hMargin;

		public float vMargin = 8f;

        private FresviiGUIDirectMessageList frameDirectMessageList;

        public int guiDepth = -30;

		public GUIStyle guiStyleTextButton;

        public Rect cancelButtonPosition;

        private Texture2D backIcon;

        private Rect backButtonPosition;

        private Rect backButtonHitPosition;

        private float scaleFactor;

        private Color normalColor;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIDirectMessageList frameDirectMessageList)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;

                guiStyleTitle.fontStyle = FontStyle.Bold;
                
                guiStyleTextButton.font = null;
            }

            this.frameDirectMessageList = frameDirectMessageList;
            
            this.guiDepth = guiDepth;

            this.scaleFactor = scaleFactor;

            title = FresviiGUIText.Get("DirectMessages");

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            height *= scaleFactor;

            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);

			guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

			guiStyleTextButton.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            normalColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

			FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            guiStyleTextButton.fontSize = (int)(guiStyleTextButton.fontSize * scaleFactor);

            hMargin *= scaleFactor;

            vMargin *= scaleFactor;

			cancelButtonPosition = FresviiGUIUtility.RectScale(cancelButtonPosition, scaleFactor);

			this.backIcon = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);
        }

        void Update()
        {
            menuRect = new Rect(frameDirectMessageList.Position.x, frameDirectMessageList.Position.y, Screen.width, height);

            cancelButtonPosition.x = menuRect.width - cancelButtonPosition.width - hMargin;

            backButtonPosition = new Rect(vMargin - 4f * scaleFactor, (height - backIcon.height) * 0.5f, backIcon.width, backIcon.height);

            backButtonHitPosition = new Rect(0f, 0f, vMargin - 4f * scaleFactor + backIcon.width, height);
        }

        public void OnGUI()
        {
            GUI.depth = guiDepth;

            GUI.BeginGroup(menuRect);

            //  Mat
			GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, menuRect.width, menuRect.height), FresviiGUIColorPalette.Palette, texCoordsMenu);

            Color tempColor = GUI.color;

            GUI.color = normalColor;

            GUI.DrawTexture(backButtonPosition, backIcon);

            GUI.color = tempColor;

            // Title
            GUI.Label(new Rect(0f, 0f, menuRect.width, menuRect.height), title, guiStyleTitle);

            Event e = Event.current;

            if (e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
                e.Use();

                frameDirectMessageList.BackToPostFrame();
            }

            GUI.EndGroup();
        }

        
    }
}