using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIGroupMessageCreateTop : MonoBehaviour
    {        
        private Rect baseRect;

        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;
        private string title = "My Profile";

        public GUIStyle guiStyleSubmitButton;
        private Vector2 submitLabelSize;
        
        public float hMargin;
		public float vMargin = 8f;

        private FresviiGUIGroupMessageCreate frameGroupMessageCreate;

        public int GuiDepth;

        public float minusMargin;

        public GUIStyle guiStyleCancelButton;
        private GUIContent cancelLabelContent;
        private Rect cancelLabelPosition;
        private Rect cancelButtonHitPosition;

        public GUIStyle guiStyleLiveHelpButton;
        private GUIContent liveHelpLabelContent;
        private Rect liveHelpLabelPosition;
        private Rect liveHelpButtonHitPosition;
        
        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIGroupMessageCreate frameGroupMessageCreate)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;
                guiStyleTitle.fontStyle = FontStyle.Bold;

                guiStyleSubmitButton.font = null;
                guiStyleCancelButton.font = null;
                guiStyleLiveHelpButton.font = null;
            }

            this.title = FresviiGUIText.Get("NewMessage");
            
            this.frameGroupMessageCreate = frameGroupMessageCreate;

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

            guiStyleLiveHelpButton.fontSize = guiStyleCancelButton.fontSize = (int)(guiStyleCancelButton.fontSize * scaleFactor);

            guiStyleLiveHelpButton.normal.textColor = guiStyleCancelButton.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);
            
            cancelLabelContent = new GUIContent(FresviiGUIText.Get("Cancel"));

            liveHelpLabelContent = new GUIContent(FresviiGUIText.Get("LiveHelp"));

        }

        void Update()
        {
            baseRect = new Rect(frameGroupMessageCreate.Position.x, frameGroupMessageCreate.Position.y, Screen.width, height);

            cancelButtonHitPosition = new Rect(0f, 0f, vMargin + guiStyleCancelButton.CalcSize(cancelLabelContent).x, height);
            
            cancelLabelPosition = new Rect(hMargin, 0f, baseRect.width, height);

            float w = guiStyleCancelButton.CalcSize(liveHelpLabelContent).x;

            liveHelpButtonHitPosition = new Rect(Screen.width - w - vMargin, 0f, vMargin + w, height);

            liveHelpLabelPosition = new Rect(Screen.width - w - vMargin, 0f, w, height);

        }

        public void LiveHelpChanged(bool isLiveHelp)
        {
            liveHelpLabelContent = (!isLiveHelp) ? new GUIContent(FresviiGUIText.Get("LiveHelp")) : new GUIContent(FresviiGUIText.Get("Friends"));
        }

        public void OnGUI()
        {
            GUI.depth = GuiDepth;

            //  Mat
            GUI.DrawTextureWithTexCoords(baseRect, palette, texCoordsMenu);

            GUI.DrawTextureWithTexCoords(new Rect(baseRect.x, baseRect.height + baseRect.y, Screen.width, 1), palette, texCoordsBorderLine);

            GUI.BeginGroup(baseRect);

            Event e = Event.current;

            if (e.type == EventType.MouseUp && cancelButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging && !frameGroupMessageCreate.ControlLock)
            {
                e.Use();

                frameGroupMessageCreate.Back();
            }
            else if (e.type == EventType.MouseUp && liveHelpButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging && !frameGroupMessageCreate.ControlLock && FASConfig.Instance.officialChat && frameGroupMessageCreate.OfficialUser != null)
            {
                e.Use();

                frameGroupMessageCreate.OnTapLiveHelp();
            }

            GUI.Label(cancelLabelPosition, cancelLabelContent, guiStyleCancelButton);

            GUI.Label(new Rect(0f,0f,Screen.width,height), title, guiStyleTitle);

            if (FASConfig.Instance.officialChat && frameGroupMessageCreate.OfficialUser != null)
            {
                GUI.Label(liveHelpLabelPosition, liveHelpLabelContent, guiStyleLiveHelpButton);
            }

            GUI.EndGroup();
            
        }
    }   
}