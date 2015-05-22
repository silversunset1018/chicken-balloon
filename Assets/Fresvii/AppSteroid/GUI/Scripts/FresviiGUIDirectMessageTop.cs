using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIDirectMessageTop : MonoBehaviour
    {    
        private Rect menuRect;

        private Texture2D palette;

		private Rect texCoordsMenu;
        
        public float height;

        public GUIStyle guiStyleTitle;
        
		private string title = "DirectMessage";
				        
        public float hMargin;

		public float vMargin = 8f;

        private FresviiGUIDirectMessage frameDirectMessage;

        public int guiDepth = -30;

		public GUIStyle guiStyleTextButton;

        public Rect cancelButtonPosition;

        private Texture2D backIcon;

        private Rect backButtonPosition;

        private Rect backButtonHitPosition;

        private float scaleFactor;
        
        private Color normalColor;

        private GUIContent titleContent;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIDirectMessage frameDirectMessage)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;

                guiStyleTitle.fontStyle = FontStyle.Bold;
                
                guiStyleTextButton.font = null;
            }
            
            this.frameDirectMessage = frameDirectMessage;
            
            this.guiDepth = guiDepth;

            this.scaleFactor = scaleFactor;
            
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

            title = FresviiGUIText.Get("DirectMessage");
        }

        void Update()
        {
            menuRect = new Rect(frameDirectMessage.Position.x, frameDirectMessage.Position.y, Screen.width, height);

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

                frameDirectMessage.BackToPostFrame();
            }

            GUI.EndGroup();
        }

        
    }
}