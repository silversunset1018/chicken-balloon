using UnityEngine;
using System.Collections;


namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIMyProfileTopMenu : MonoBehaviour
    {        
        private Texture2D appIcon;

        private Rect menuRect;

        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;
        private string title = "My Profile";

		public Rect appIconPosition;
		private Rect appIconHitPosition;

        public GUIStyle guiStyleEditButton;
        public Rect editButtonPosition;
		public Rect editButtonHitPosition;
       
        public float hMargin;
		public float vMargin = 8;
		
        private FresviiGUIMyProfile frameMyProfile;

        public int GuiDepth;

        public Color textButtonNormalColor, textButtonDownColor;

        private Texture2D backIcon;
        private Rect backButtonPosition;
        private Rect backButtonHitPosition;

        private Color normalColor;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, FresviiGUIMyProfile frameMyProfile, int guiDepth)
        {
            this.GuiDepth = guiDepth;
            this.appIcon = appIcon;
            this.title = FresviiGUIText.Get("MyProfile");
            this.frameMyProfile = frameMyProfile;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;
                guiStyleTitle.fontStyle = FontStyle.Bold;
                guiStyleEditButton.font = null;
            }

            palette = FresviiGUIColorPalette.Palette;

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            normalColor = guiStyleEditButton.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            this.backIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);

            height *= scaleFactor;

            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);
            
            guiStyleEditButton.fontSize = (int)(guiStyleEditButton.fontSize * scaleFactor);
            
            hMargin *= scaleFactor;
			
            vMargin *= scaleFactor;

			appIconPosition = FresviiGUIUtility.RectScale(appIconPosition, scaleFactor);

            editButtonPosition = FresviiGUIUtility.RectScale(editButtonPosition, scaleFactor);

            backButtonPosition = new Rect(vMargin, 0.5f * (height - backIcon.height), backIcon.width, backIcon.height);

            backButtonHitPosition = new Rect(0, 0, backButtonPosition.x + backButtonPosition.width + vMargin, height);
            
        }

        void Update()
        {
            menuRect = new Rect(frameMyProfile.Position.x, frameMyProfile.Position.y, Screen.width, height);

            editButtonPosition.x = Screen.width - editButtonPosition.width - hMargin;

            appIconHitPosition = new Rect(0, 0, appIconPosition.width + 2f * vMargin, height);

            editButtonHitPosition = new Rect(editButtonPosition.x - vMargin, 0, Screen.width - editButtonPosition.x + vMargin, height);
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

			Event e = Event.current;

            GUI.DrawTextureWithTexCoords(new Rect(menuRect.x, menuRect.height + menuRect.y, Screen.width, 1), palette, texCoordsBorderLine);

            //  Mat
            GUI.DrawTextureWithTexCoords(menuRect, palette, texCoordsMenu);


            // Title
            GUI.Label(menuRect, title, guiStyleTitle);

            GUI.BeginGroup(menuRect);

            if (FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.MyProfile && frameMyProfile.IsOriginal)
            {
                // AppIcon
                GUI.DrawTexture(appIconPosition, appIcon);

                if (e.type == EventType.MouseUp && appIconHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
                {
                    e.Use();
                    FresviiGUIManager.Instance.LoadScene();
                }
            }
            else if (!frameMyProfile.IsOriginal || FresviiGUIManager.Instance.CurrentViewMode == FASGui.Mode.Forum)
            {
                Color tempColor = GUI.color;

                GUI.color = normalColor;

                GUI.DrawTexture(backButtonPosition, backIcon);

                GUI.color = tempColor;

                if (e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
                {
                    e.Use();

                    frameMyProfile.BackToPostFrame();
                }
            }

			GUI.Label(editButtonPosition, FresviiGUIText.Get("Edit"), guiStyleEditButton);

			if(e.type == EventType.MouseUp && editButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
				e.Use();

                Vector3 textNormalVec = new Vector3(textButtonNormalColor.r, textButtonNormalColor.g, textButtonNormalColor.b);

                Vector3 textDownVec = new Vector3(textButtonDownColor.r, textButtonDownColor.g, textButtonDownColor.b);

                iTween.StopByName("Edit");

                iTween.ValueTo(this.gameObject, iTween.Hash("name", "Edit", "duraion", 1.0f, "from", textNormalVec, "to", textDownVec, "onupdate", "OnUpdateTextButton", "oncomplete", "OnCompleteTextButton"));

                frameMyProfile.OnEditButtonTapped();
            }

            GUI.EndGroup();
            
        }

        void OnUpdateSubmitText(Vector3 color)
        {
            guiStyleEditButton.normal.textColor = new Color(color.x, color.y, color.z);
        }

        void OnCompleteTextButton()
        {
            guiStyleEditButton.normal.textColor = normalColor;
        }
    }
}