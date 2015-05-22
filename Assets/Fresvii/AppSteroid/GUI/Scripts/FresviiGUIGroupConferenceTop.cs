#if GROUP_CONFERENCE
using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIGroupConferenceTop : MonoBehaviour
    {        
        private Rect baseRect;

        private Texture2D palette;

        private Rect texCoordsMenu;
        
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;
        
        private string title = "Group Calls";

        public float margin;
        
        private float scaleFactor;

        private FresviiGUIGroupConference frameGroupConference;

        private Rect doneButtonPosition;

        public GUIStyle guiStyleDoneButton;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, FresviiGUIGroupConference frameGroupConference)
        {
            this.frameGroupConference = frameGroupConference;

            this.scaleFactor = scaleFactor;

            this.title = FresviiGUIText.Get("GroupCalls");
            
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;

                guiStyleTitle.fontStyle = FontStyle.Bold;

                guiStyleDoneButton.font = null;
            }

            palette = FresviiGUIColorPalette.Palette;

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            guiStyleDoneButton.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarPositive);

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            margin *= this.scaleFactor;

            height *= this.scaleFactor;

            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * this.scaleFactor);

            guiStyleDoneButton.fontSize = (int)(guiStyleDoneButton.fontSize * this.scaleFactor);

            guiStyleDoneButton.padding = FresviiGUIUtility.RectOffsetScale(guiStyleDoneButton.padding, this.scaleFactor);
        
            baseRect = new Rect(0f, Screen.height, Screen.width, height);

            doneButtonPosition = new Rect(0f, 0f, baseRect.width * 0.3f, baseRect.height);

        }

        void Update()
        {
            if (frameGroupConference == null) return;

            baseRect = new Rect(frameGroupConference.Position.x, frameGroupConference.Position.y, Screen.width, height);

            doneButtonPosition = new Rect(0f, 0f, baseRect.width * 0.3f, baseRect.height);
        }

        public void OnGUI()
        {
            if (frameGroupConference == null) return;

            GUI.depth = frameGroupConference.GuiDepth - 1;

            GUI.DrawTextureWithTexCoords(new Rect(baseRect.x, baseRect.height + baseRect.y, Screen.width, 1), palette, texCoordsBorderLine);

            //  Mat
            GUI.DrawTextureWithTexCoords(baseRect, palette, texCoordsMenu);

            // Title
            GUI.Label(baseRect, title, guiStyleTitle);                    

            GUI.BeginGroup(baseRect);

            if (GUI.Button(doneButtonPosition, FresviiGUIText.Get("Done"), guiStyleDoneButton))
            {
                frameGroupConference.Done();
            }
            
            GUI.EndGroup();
        }

        
    }
}
#endif