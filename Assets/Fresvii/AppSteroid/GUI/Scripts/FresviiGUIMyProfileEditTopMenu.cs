using UnityEngine;
using System.Collections;


namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIMyProfileEditTopMenu : MonoBehaviour
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
		private Rect submitButtonHitPosition;
       
        public float hMargin;
		public float vMargin = 8f;

        public int GuiDepth;

        private Texture2D backIcon;

        private FresviiGUIMyProfileEdit guiEdit;

        private Color submitEnableColor, submitUnableColor;

        public float minusMargin;

        public GUIStyle guiStyleCancelButton;
        private GUIContent cancelLabelContent;
        private GUIContent submitLabelContent;
        private Rect cancelLabelPosition;
        private Rect cancelButtonHitPosition;
        private Rect submitLabelPosition;
        
        public void Init(Texture2D appIcon, string postFix, float scaleFactor, FresviiGUIMyProfileEdit myProfileEditMain)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;
 
                guiStyleTitle.fontStyle = FontStyle.Bold;
                
                guiStyleSubmitButton.font = null;
                
                guiStyleCancelButton.font = null;
            }

            guiEdit = GetComponent<FresviiGUIMyProfileEdit>();

            this.title = FresviiGUIText.Get("MyProfile");

            palette = FresviiGUIColorPalette.Palette;

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);

            this.backIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);

            height *= scaleFactor;
            
            guiStyleTitle.fontSize = (int)(guiStyleTitle.fontSize * scaleFactor);
            
            guiStyleSubmitButton.fontSize = (int)(guiStyleSubmitButton.fontSize * scaleFactor);
            
            hMargin *= scaleFactor;
			
            vMargin *= scaleFactor;

            submitLabelContent = new GUIContent(FresviiGUIText.Get("Submit"));

            submitLabelSize = guiStyleSubmitButton.CalcSize(submitLabelContent);
            
            guiStyleCancelButton.fontSize = (int)(guiStyleCancelButton.fontSize * scaleFactor);

            guiStyleCancelButton.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);
            
            cancelLabelContent = new GUIContent(FresviiGUIText.Get("Cancel"));

            submitEnableColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            submitUnableColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNegative);
        }

        void Update()
        {
            baseRect = new Rect(guiEdit.Position.x, guiEdit.Position.y, Screen.width, height);

            submitLabelPosition = new Rect(baseRect.width - submitLabelSize.x - hMargin, 0f, submitLabelSize.x, height);
            
            submitButtonHitPosition = new Rect(baseRect.width - submitLabelSize.x - hMargin * 2f, 0f, submitLabelSize.x + 2f * hMargin, height);

            cancelButtonHitPosition = new Rect(0f, 0f, vMargin + backIcon.width + minusMargin + guiStyleCancelButton.CalcSize(cancelLabelContent).x, height);
            
            cancelLabelPosition = new Rect(hMargin, 0f, baseRect.width, height);
        }

        public void OnGUI()
        {
            GUI.depth = GuiDepth;

            //  Mat
            GUI.DrawTextureWithTexCoords(baseRect, palette, texCoordsMenu);

            GUI.DrawTextureWithTexCoords(new Rect(baseRect.x, baseRect.height + baseRect.y, Screen.width, 1), palette, texCoordsBorderLine);

            GUI.BeginGroup(baseRect);
            
            Event e = Event.current;

            if (e.type == EventType.MouseUp && cancelButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
                e.Use();

                guiEdit.BackToMyProfile();
            }

            bool submitEnable = guiEdit.IsChanged();

            if (e.type == EventType.MouseUp && submitButtonHitPosition.Contains(e.mousePosition) && submitEnable && !FASGesture.IsDragging)
            {
                e.Use();

                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });

                    return;
                }

                Vector3 submitEnableVec = new Vector3(submitEnableColor.r, submitEnableColor.g, submitEnableColor.b);

                Vector3 submitUnableVec = new Vector3(submitUnableColor.r, submitUnableColor.g, submitUnableColor.b);

                iTween.StopByName("Submit");

                iTween.ValueTo(this.gameObject, iTween.Hash("name", "Submit", "duraion", 1.0f, "from", submitEnableVec, "to", submitUnableVec, "onupdate", "OnUpdateTextButton", "oncomplete", "OnCompleteTextButton"));

                guiEdit.Submit();
            }

            GUI.Label(cancelLabelPosition, cancelLabelContent, guiStyleCancelButton);

            GUI.Label(new Rect(0f,0f,Screen.width,height), title, guiStyleTitle);

            guiStyleSubmitButton.normal.textColor = (submitEnable) ? submitEnableColor : submitUnableColor;

            GUI.Label(submitLabelPosition, FresviiGUIText.Get("Submit"), guiStyleSubmitButton);

            GUI.EndGroup();
            
        }

        void OnUpdateSubmitText(Vector3 color)
        {
            guiStyleSubmitButton.normal.textColor = new Color(color.x, color.y, color.z);
        }

        void OnCompleteTextButton()
        {
            guiStyleSubmitButton.normal.textColor = submitUnableColor;
        }
    }
}