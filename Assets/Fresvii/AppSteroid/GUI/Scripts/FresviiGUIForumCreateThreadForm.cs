using UnityEngine;
using System.Collections;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIForumCreateThreadForm : MonoBehaviour
    {
        public int guiDepth = -100;

        private bool show = false;

        public Rect baseRect;

        public Texture pallete;

        public Rect coordsBase;

        public Rect coordsLine;

        public FresviiGUIPopUpShield cancel;

        private string inputText = "";

        private Texture2D texAddButton;

        public GUIStyle guiStyleTextArea;
        public GUIStyle guiStyleAddButton;
        public GUIStyle guiStyleCreate;

        private Texture2D clipImage;

        public Vector2 clipImageMaxSize;

        public FresviiGUIForum forum;

        public float sideMargin = 8;

        private bool initialized;

        public void Init(float scaleFactor, string postFix)
        {
            if (!initialized)
            {
                coordsBase = new Rect(coordsBase.x / pallete.width, coordsBase.y / pallete.height, coordsBase.width / pallete.width, coordsBase.height / pallete.height);
                coordsLine = new Rect(coordsLine.x / pallete.width, coordsLine.y / pallete.height, coordsLine.width / pallete.width, coordsLine.height / pallete.height);
                this.baseRect = FresviiGUIUtility.RectScale(baseRect, scaleFactor);
                this.baseRect.x = Screen.width * 0.5f - baseRect.width * 0.5f;
                sideMargin *= scaleFactor;
                guiStyleCreate.fontSize = (int)(guiStyleCreate.fontSize * scaleFactor);
                guiStyleTextArea.fontSize = (int)(guiStyleTextArea.fontSize * scaleFactor);
                initialized = true;
            }
        }

        public void Show()
        {
            inputText = FresviiGUIText.Get("WriteAComment");

            show = true;

            cancel.Enable(Hide);

            init = false;

            TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, true, false, false);
        }

        public void Hide()
        {
            show = false;

            if (clipImage != null)
                Destroy(clipImage);

            cancel.Done();

           
        }

        private bool init = false;

        public void OnGUI()
        {
            if (!show) return;

            GUI.depth = FASGui.GuiDepthBase + guiDepth;

            GUI.DrawTextureWithTexCoords(baseRect, pallete, coordsBase);
            
            GUILayout.BeginArea(baseRect);
            
            GUILayout.Space(sideMargin);

            GUILayout.BeginHorizontal();

            GUILayout.Space(sideMargin);

            inputText = GUILayout.TextArea(inputText, guiStyleTextArea, GUILayout.Height(3f * baseRect.height / 4f - 2f * sideMargin));

            if (Event.current.isMouse && !init)
            {
                Rect rectTextArea = GUILayoutUtility.GetLastRect();

                if (rectTextArea.Contains(Event.current.mousePosition))
                {
                    inputText = "";
                }

                init = true;
            }

            GUILayout.Space(sideMargin);

            GUILayout.EndHorizontal();

            GUILayout.Space(sideMargin);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add photo", guiStyleCreate, GUILayout.Width(baseRect.width * 0.5f), GUILayout.Height(baseRect.height / 4f)))
            {
                if (clipImage != null)
                {
                    Destroy(clipImage);
                    clipImage = null;
                }

                Fresvii.AppSteroid.Util.ImagePicker.Show(this, Fresvii.AppSteroid.Util.ImagePicker.Type.Gallery, delegate(Texture2D loadedTexture)
                {
                    if (loadedTexture != null)
                    {
                        clipImage = loadedTexture;
                        guiStyleAddButton.normal.background = clipImage;
                    }

                });

            }

            if (GUILayout.Button(FresviiGUIText.Get("NewThread"), guiStyleCreate, GUILayout.Width(baseRect.width * 0.5f), GUILayout.Height(baseRect.height / 4f)))
            {
                if (!string.IsNullOrEmpty(inputText) | clipImage != null)
                {
                    FASForum.CreateThread(inputText, clipImage, delegate(Fresvii.AppSteroid.Models.Thread thread, Fresvii.AppSteroid.Models.Error error)
                    {
                        if (error == null)
                        {
                            forum.AddThread(thread);
                        }
                    });
                }
                    
                Hide();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            GUI.DrawTextureWithTexCoords(new Rect(baseRect.x, baseRect.y + 3f * baseRect.height / 4f, baseRect.width, 1f), pallete, coordsLine);

            GUI.DrawTextureWithTexCoords(new Rect(baseRect.x + baseRect.width * 0.5f, baseRect.y + 3f * baseRect.height / 4f, 1f, baseRect.height / 4f), pallete, coordsLine);

        }

        void OnDestroy()
        {
            if (clipImage != null)
                Destroy(clipImage);
        }

    }
}