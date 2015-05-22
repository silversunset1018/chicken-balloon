using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIVideoListTop : MonoBehaviour
    {    
        private Rect menuRect;

        private Texture2D palette;

		private Rect texCoordsMenu;
        
        public float height;

        public GUIStyle guiStyleTitle;
        
		private string title = "Videos";
				        
        public float hMargin;

		public float vMargin = 8f;

        private FresviiGUIVideoList frameVideoList;

        public int guiDepth = -30;

		public GUIStyle guiStyleTextButton;

        public Rect cancelButtonPosition;

        private Texture2D backIcon;

        private Rect backButtonPosition;

        private Rect backButtonHitPosition;

        private float scaleFactor;

		private Color colorNormal;

        private Texture2D appIcon;

        public Rect appIconPosition;

        public Rect appIconButtonPosition;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIVideoList frameVideoList)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;

                guiStyleTitle.fontStyle = FontStyle.Bold;
                
                guiStyleTextButton.font = null;
            }
            
            this.appIcon = appIcon;

            this.frameVideoList = frameVideoList;
            
            this.guiDepth = guiDepth;

            this.scaleFactor = scaleFactor;

            title = FresviiGUIText.Get("Videos");

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            height *= scaleFactor;

            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);

			guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

			guiStyleTextButton.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

			FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            guiStyleTextButton.fontSize = (int)(guiStyleTextButton.fontSize * scaleFactor);

            hMargin *= scaleFactor;

            vMargin *= scaleFactor;

			cancelButtonPosition = FresviiGUIUtility.RectScale(cancelButtonPosition, scaleFactor);
			
			this.backIcon = Fresvii.AppSteroid.Util.ResourceManager.Instance.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);

			colorNormal = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            appIconPosition = FresviiGUIUtility.RectScale(appIconPosition, scaleFactor);

            appIconButtonPosition = FresviiGUIUtility.RectScale(appIconButtonPosition, scaleFactor);

        }

        void Update()
        {
            menuRect = new Rect(frameVideoList.Position.x, frameVideoList.Position.y, Screen.width, height);

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

            // Title
            GUI.Label(new Rect(0f, 0f, menuRect.width, menuRect.height), title, guiStyleTitle);

            Event e = Event.current;

            if (frameVideoList.mode == FresviiGUIVideoList.Mode.FromUploded)
            {
                // AppIcon
                GUI.DrawTexture(appIconPosition, appIcon);

                if (e.type == EventType.MouseUp && appIconButtonPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
                {
                    e.Use();

                    if (Application.loadedLevelName == FASGui.ReturnSceneName)
                    {
                        frameVideoList.BackToPostFrame();
                    }
                    else
                    {
                        FresviiGUIManager.Instance.LoadScene();
                    }
                }
            }
            else
            {
                if (frameVideoList.IsModal)
                {
                    if (GUI.Button(cancelButtonPosition, FresviiGUIText.Get("Cancel"), guiStyleTextButton))
                    {
                        frameVideoList.BackToPostFrame();
                    }
                }
                else
                {
                    Color tmpColor = GUI.color;

                    GUI.color = colorNormal;

                    GUI.DrawTexture(backButtonPosition, backIcon);

                    GUI.color = tmpColor;

                    if (e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
                    {
                        e.Use();

                        frameVideoList.BackToPostFrame();
                    }
                }
            }

            GUI.EndGroup();
        }

        
    }
}