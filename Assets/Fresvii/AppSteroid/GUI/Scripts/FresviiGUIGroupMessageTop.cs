using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIGroupMessageTop : MonoBehaviour
    {        
        private Texture2D appIcon;

        private Rect menuRect;

        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;
        private string title = "Messages";

		public Rect appIconPosition;
        public Rect appIconButtonPosition;

        public GUIStyle guiStyleCreateButton;
        public Rect createButtonPosition;
        private Rect createButtonHitPosition;
        
        public float hMargin;
		public float vMargin = 8f;

        private FresviiGUIGroupMessage frameGroupMessage;

        public int guiDepth = -30;

        private Texture2D backIcon;
        public GUIStyle guiStyleBackButton;
        private Rect backButtonPosition;
		private Rect backButtonHitPosition;

        private Color colorNormal;

        private Color createIconColor;

        private Texture createIcon;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIGroupMessage frameGroupMessage)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;

                guiStyleTitle.fontStyle = FontStyle.Bold;
                
                guiStyleBackButton.font = null;

				guiStyleCreateButton.font = null;
            }

            this.appIcon = appIcon;
                        
            this.frameGroupMessage = frameGroupMessage;
            
            this.guiDepth = guiDepth;

            createIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.MessageCompose + postFix, false);

            title = FresviiGUIText.Get("Messages");

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

            createButtonPosition = FresviiGUIUtility.RectScale(createButtonPosition, scaleFactor);

            guiStyleCreateButton.fontSize = (int)(guiStyleCreateButton.fontSize * scaleFactor);

            createIconColor = guiStyleCreateButton.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            this.backIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);

            backButtonPosition = new Rect(vMargin, 0.5f * (height - backIcon.height), backIcon.width, backIcon.height);

            backButtonHitPosition = new Rect(0, 0, backButtonPosition.x + backButtonPosition.width + vMargin, height);

        }

        void Update()
        {
            menuRect = new Rect(frameGroupMessage.Position.x, frameGroupMessage.Position.y, Screen.width, height);

            createButtonPosition.x = Screen.width - createButtonPosition.width - hMargin;

            createButtonPosition.y = hMargin;

            createButtonHitPosition = new Rect(createButtonPosition.x - vMargin, 0, Screen.width - createButtonPosition.x + vMargin, height);
        }

        public void OnGUI()
        {
            GUI.depth = guiDepth;

            GUI.BeginGroup(menuRect);

            GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, Screen.width, 1), palette, texCoordsBorderLine);

            //  Mat
            GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, menuRect.width, menuRect.height), palette, texCoordsMenu);

            // AppIcon
            if (frameGroupMessage.PostFrame == null)
            {
                GUI.DrawTexture(appIconPosition, appIcon);
            }
            else
            {
                GUI.DrawTexture(backButtonPosition, backIcon);
            }

            // Title
            GUI.Label(new Rect(0f, 0f, menuRect.width, menuRect.height), title, guiStyleTitle);
            
            Color tmpColor = GUI.color;

            GUI.color = colorNormal;

            GUI.color = tmpColor;

			Event e = Event.current;

            if (frameGroupMessage.PostFrame == null)
            {
                if (e.type == EventType.MouseUp && appIconButtonPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
                {
                    e.Use();

                    FresviiGUIManager.Instance.LoadScene();
                }
            }
            else
            {
                if (e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
                {
                    e.Use();

                    frameGroupMessage.BackToPostFrame();
                }
            }


            Color tmp = GUI.color;

            GUI.color = createIconColor;

            GUI.DrawTexture(createButtonPosition, createIcon);

            GUI.color = tmp;

            if (e.type == EventType.MouseUp && createButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging && !frameGroupMessage.ControlLock)
            {
                e.Use();

                frameGroupMessage.OnCreateButtonTapped();
            }

            GUI.EndGroup();
        }

        
    }
}