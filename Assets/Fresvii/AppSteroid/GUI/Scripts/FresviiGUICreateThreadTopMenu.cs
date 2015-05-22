using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUICreateThreadTopMenu : MonoBehaviour
    {        
        private Rect baseRect;

        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;
        private string title = "New Thread";

        public float margin;
        private Rect backButtonPosition;
        private Rect backButtonHitPosition;

        public GUIStyle guiStyleTextButton;
        public Rect submitButtonPosition;
        
        private float scaleFactor;

        public float hMargin;
        public float miniMargin;

        private FresviiGUICreateThread frameCreateThread;

        public int guiDepth = -30;

        private Texture2D backIcon;

        private Color colorNormal, colorPositive, colorNegative;

        public GUIStyle guiStyleForumLabel;
        private GUIContent forumLabelContent;
        private Rect forumLabelPosition;

        public bool draw;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, FresviiGUICreateThread frameCreateThread)
        {
            frameCreateThread = GetComponent<FresviiGUICreateThread>();

            this.scaleFactor = scaleFactor;
            this.title = FresviiGUIText.Get("NewThread");
            this.frameCreateThread = frameCreateThread;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;
                guiStyleTitle.fontStyle = FontStyle.Bold;
                guiStyleTextButton.font = null;
                guiStyleForumLabel.font = null;
            }

            palette = FresviiGUIColorPalette.Palette;

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            colorNormal = guiStyleForumLabel.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            margin *= scaleFactor;

            height *= scaleFactor;
            
            hMargin *= scaleFactor;

            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);
            
            guiStyleTextButton.fontSize = (int)(guiStyleTextButton.fontSize * scaleFactor);
            
            guiStyleForumLabel.fontSize = (int)(guiStyleForumLabel.fontSize * scaleFactor);

            colorPositive = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarPositive);

            colorNegative = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNegative);

            submitButtonPosition = FresviiGUIUtility.RectScale(submitButtonPosition, scaleFactor);

            this.backIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);

            forumLabelContent = new GUIContent(FresviiGUIText.Get("Forum"));
        }

        void Update()
        {
            baseRect = new Rect(frameCreateThread.Position.x, frameCreateThread.Position.y, Screen.width, height);

            backButtonPosition = new Rect(margin - 4f * scaleFactor, 0.5f * (height - backIcon.height), backIcon.width, backIcon.height);

            backButtonHitPosition = new Rect(0, 0, margin + backIcon.width + miniMargin + guiStyleForumLabel.CalcSize(forumLabelContent).x, height);

            forumLabelPosition = new Rect(backButtonPosition.x + backButtonPosition.width + miniMargin, 0f, baseRect.width, height);

            submitButtonPosition.x = baseRect.width - submitButtonPosition.width - hMargin;
        }

        public void OnGUI()
        {
            GUI.depth = frameCreateThread.GuiDepth - 1;

            Event e = Event.current;

            GUI.DrawTextureWithTexCoords(new Rect(baseRect.x, baseRect.height + baseRect.y, Screen.width, 1), palette, texCoordsBorderLine);

            //  Mat
            GUI.DrawTextureWithTexCoords(baseRect, palette, texCoordsMenu);

            // Title
            GUI.Label(baseRect, title, guiStyleTitle);

            GUI.BeginGroup(baseRect);
                
            // BackIcon
            Color tmpColor = GUI.color;

            GUI.color = colorNormal;

            GUI.DrawTexture(backButtonPosition, backIcon);

            GUI.color = tmpColor;

            if (e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition))
            {
                e.Use();

                frameCreateThread.BackToForum();
            }

            GUI.Label(forumLabelPosition, forumLabelContent.text, guiStyleForumLabel);

            bool wasCreated = frameCreateThread.WasCreated();

            guiStyleTextButton.normal.textColor = wasCreated ? colorPositive : colorNegative;

            if (GUI.Button(submitButtonPosition, FresviiGUIText.Get("Create"), guiStyleTextButton) && wasCreated)
            {
                frameCreateThread.Create();
            }

            GUI.EndGroup();
        
        }

        
    }
}