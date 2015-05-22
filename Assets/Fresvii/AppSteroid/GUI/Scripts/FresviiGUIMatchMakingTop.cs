using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIMatchMakingTop : MonoBehaviour
    {        
        private Texture2D appIcon;

        private Rect menuRect;

        private Texture2D palette;
        private Rect texCoordsMenu;
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;

        public GUIStyle guiStyleSubTitle;

		private string title = "Select Opponents";

        private string subTitle = "";

		public Rect appIconPosition;
        public Rect appIconButtonPosition;
        
        public float hMargin;
		public float vMargin = 8f;

        private FresviiGUIMatchMaking frameMatchMaking;

        public int guiDepth = -30;

        private Texture2D backIcon;

        public GUIStyle guiStyleBackButton;
        
        private Rect backButtonPosition;
		
        private Rect backButtonHitPosition;

        public GUIStyle guiStyleCancelButton = null;

        private GUIContent rightLabelContent;

        private Rect rightLabelPosition;

        private Rect progressBarPosition;

        private Rect texCoodsProgressBar;

        private float scaleFactor;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIMatchMaking frameMatchMaking)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;

                guiStyleSubTitle.font = null;

                guiStyleTitle.fontStyle = FontStyle.Bold;
                
                guiStyleBackButton.font = null;

                guiStyleCancelButton.font = null;
            }

            this.appIcon = appIcon;

            this.frameMatchMaking = frameMatchMaking;
            
            this.guiDepth = guiDepth;

            this.scaleFactor = scaleFactor;

            title = FresviiGUIText.Get("SelectOpponents");            

            palette = FresviiGUIColorPalette.Palette;

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            height *= scaleFactor;

            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);

            guiStyleSubTitle.fontSize = (int)(guiStyleSubTitle.fontSize * scaleFactor);

            guiStyleTitle.normal.textColor = guiStyleSubTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            guiStyleTitle.padding = FresviiGUIUtility.RectOffsetScale(guiStyleTitle.padding, scaleFactor);

            guiStyleSubTitle.padding = FresviiGUIUtility.RectOffsetScale(guiStyleSubTitle.padding, scaleFactor);

            hMargin *= scaleFactor;

            vMargin *= scaleFactor;

			appIconPosition = FresviiGUIUtility.RectScale(appIconPosition, scaleFactor);
          
            appIconButtonPosition = FresviiGUIUtility.RectScale(appIconButtonPosition, scaleFactor);

            guiStyleCancelButton.fontSize = (int)(guiStyleCancelButton.fontSize * scaleFactor);

            guiStyleCancelButton.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            rightLabelContent = new GUIContent("");

            rightLabelPosition.width = guiStyleCancelButton.CalcSize(rightLabelContent).x + hMargin;

            texCoodsProgressBar = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardProgressBar);

        }

		public void SetSubTitle(uint minNumberOfPlayers, uint maxNumberOfPlayers)
        {
			if(minNumberOfPlayers == maxNumberOfPlayers)
                subTitle = minNumberOfPlayers.ToString() + " " + FresviiGUIText.Get("PlayersAvailable");
			else
                subTitle = minNumberOfPlayers.ToString() + " " + FresviiGUIText.Get("to") + " " + maxNumberOfPlayers.ToString() + " " + FresviiGUIText.Get("PlayersAvailable");
        }

        void Update()
        {
            menuRect = new Rect(frameMatchMaking.Position.x, frameMatchMaking.Position.y, Screen.width, height);

            rightLabelPosition.width = guiStyleCancelButton.CalcSize(rightLabelContent).x + hMargin;

            rightLabelPosition = new Rect(menuRect.width - rightLabelPosition.width, 0f, rightLabelPosition.width, height);

            progressBarPosition = new Rect(0f, height - scaleFactor, menuRect.width * frameMatchMaking.MatchMakingProgress(), scaleFactor);
        }

        public void SetRightButtonLabel(FresviiGUIMatchMaking.Mode state)
        {
            if (state == FresviiGUIMatchMaking.Mode.Setting)
            {
                rightLabelContent = new GUIContent(FresviiGUIText.Get("Start"));

                title = FresviiGUIText.Get("SelectOpponents");        
            }
            else if (state == FresviiGUIMatchMaking.Mode.Matching)
            {
                rightLabelContent = new GUIContent(FresviiGUIText.Get("Cancel"));

                title = FresviiGUIText.Get("WaitingForPlayers");
            }
            else
            {
                title = FresviiGUIText.Get("SelectOpponents");  

                rightLabelContent = new GUIContent("");
            }
        }

        public void OnGUI()
        {
            GUI.depth = guiDepth;

            GUI.BeginGroup(menuRect);

            GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, Screen.width, 1), palette, texCoordsBorderLine);

            //  Mat
            GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, menuRect.width, menuRect.height), palette, texCoordsMenu);

            // AppIcon
            GUI.DrawTexture(appIconPosition, appIcon);

            // Title
            GUI.Label(new Rect(0f, 0f, menuRect.width, menuRect.height), title, guiStyleTitle);

            // Subtitle
            GUI.Label(new Rect(0f, 0f, menuRect.width, menuRect.height), subTitle, guiStyleSubTitle);

            // rightbutton
            if (frameMatchMaking.State == FresviiGUIMatchMaking.Mode.Matching || (frameMatchMaking.State == FresviiGUIMatchMaking.Mode.Setting && frameMatchMaking.CanStartMatchMaking()))
            {
                GUI.Label(rightLabelPosition, rightLabelContent, guiStyleCancelButton);
            }

			Event e = Event.current;

            if (e.type == EventType.MouseUp && appIconButtonPosition.Contains(e.mousePosition) && !FASGesture.IsDragging && !frameMatchMaking.ControlLock)
            {
                e.Use();

                FresviiGUIManager.Instance.LoadScene();
            }

            if (e.type == EventType.MouseUp && rightLabelPosition.Contains(e.mousePosition) && !FASGesture.IsDragging && !frameMatchMaking.ControlLock)
            {
                e.Use();

                if (frameMatchMaking.State == FresviiGUIMatchMaking.Mode.Matching)
                {
                    frameMatchMaking.OnTapCancelMatch();
                }
                else if(frameMatchMaking.State == FresviiGUIMatchMaking.Mode.Setting)
                {
                    if (frameMatchMaking.CanStartMatchMaking())
                    {
                        frameMatchMaking.OnTapStartMatchMaking();
                    }
                }
            }

            // Progress bar
            GUI.DrawTextureWithTexCoords(progressBarPosition, FresviiGUIColorPalette.Palette, texCoodsProgressBar);

            GUI.EndGroup();
        }      
    }
}