using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIEditGroupMemberTop : MonoBehaviour
    {        
        private Rect baseRect;

        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;
        private string title = "Members";

        public GUIStyle guiStyleSubmitButton;

        private Vector2 submitLabelSize;
        
        public float hMargin;
		public float vMargin = 8f;

        private FresviiGUIEditGroupMember frameEditGroupMember;

        public int GuiDepth;

        public float minusMargin;

        private Texture2D backIcon;
        public GUIStyle guiStyleBackButton;
        private Rect backButtonPosition;
        private Rect backButtonHitPosition;

        private Color iconColor;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIEditGroupMember frameEditGroupMember)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;
                guiStyleTitle.fontStyle = FontStyle.Bold;

                guiStyleSubmitButton.font = null;
                guiStyleBackButton.font = null;
            }

            this.title = FresviiGUIText.Get("Members");

            this.frameEditGroupMember = frameEditGroupMember;

            this.GuiDepth = guiDepth;

            palette = FresviiGUIColorPalette.Palette;

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            height *= scaleFactor;

            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);
            
            guiStyleSubmitButton.fontSize = (int)(guiStyleSubmitButton.fontSize * scaleFactor);
            
            hMargin *= scaleFactor;
			
            vMargin *= scaleFactor;

            this.backIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);


            backButtonPosition = new Rect(vMargin, 0.5f * (height - backIcon.height), backIcon.width, backIcon.height);

            backButtonHitPosition = new Rect(0, 0, backButtonPosition.x + backButtonPosition.width + vMargin, height);

            guiStyleBackButton.fontSize = (int)(guiStyleBackButton.fontSize * scaleFactor);

            iconColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);
        }

        void Update()
        {
            baseRect = new Rect(frameEditGroupMember.Position.x, frameEditGroupMember.Position.y, Screen.width, height);
        }

        public void OnGUI()
        {
            GUI.depth = GuiDepth;

			Event e = Event.current;

            if (e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging && !frameEditGroupMember.ControlLock)
            {
                e.Use();

                frameEditGroupMember.Back();
            }

            //  Mat
            GUI.DrawTextureWithTexCoords(baseRect, palette, texCoordsMenu);

            GUI.DrawTextureWithTexCoords(new Rect(baseRect.x, baseRect.height + baseRect.y, Screen.width, 1), palette, texCoordsBorderLine);
            
            GUI.BeginGroup(baseRect);

            GUI.Label(new Rect(0f,0f,Screen.width,height), title, guiStyleTitle);

            Color tmp = GUI.color;

            GUI.color = iconColor;

            GUI.DrawTexture(backButtonPosition, backIcon);

            GUI.color = tmp;

            GUI.EndGroup();
            
        }
    }   
}