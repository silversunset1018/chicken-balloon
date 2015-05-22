using UnityEngine;
using System;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIImageViewer : MonoBehaviour
    {
        public Vector2 loadingSpinnerSize;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;

        private int guiDepth;

        private Texture2D texture;

        private Rect textureCoordsBackground;

        private Rect backgroundRect;

        private string imageUrl;

        public FresviiGUIButton buttonClose;

        private Action OnDoneCallback;

        private Vector2 imagePosition;

        private Rect imageRect;

        private Rect pinchStartRect;

        public float maxDragSpeed = 100f;

        public void Show(string imageUrl, int guiDepth, Action onDoneCallback)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            this.imageUrl = imageUrl;

            OnDoneCallback = onDoneCallback;

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.ModalBackground);

            this.guiDepth = guiDepth;

            loadingSpinnerSize *= FresviiGUIManager.Instance.ScaleFactor;

            loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, guiDepth - 10);

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(this.imageUrl, true, delegate(Texture2D tex)
            {
                loadingSpinner.Hide();

                this.texture = tex;

                imagePosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

                imageRect = new Rect(0f, 0f, Screen.width, Screen.width * texture.height / texture.width);

                if (imageRect.height > Screen.height)
                {
                    float w = texture.width * Screen.height / texture.height;

                    imageRect = new Rect(0.5f * (Screen.width - w), 0f, w, Screen.height);
                }
            });
        }

        void OnEnable()
        {
            FASGesture.OnPinchStart += OnPinchStart;

            FASGesture.OnPinch += OnPinch;

            FASGesture.Stop();
        }

        void OnDisable()
        {
            FASGesture.OnPinchStart -= OnPinchStart;

            FASGesture.OnPinch -= OnPinch;

            FASGesture.Stop();
        }

        void OnPinchStart(Fresvii.AppSteroid.Gui.FASPinchInfo pi)
        {
            pinchStartRect = imageRect;
        }

        void OnPinch(Fresvii.AppSteroid.Gui.FASPinchInfo pi)
        {
            float scale = Vector2.Distance(pi.position1, pi.position2) / Vector2.Distance(pi.startPosition1, pi.startPosition2);

            Rect preImageRect = imageRect;

            imageRect = FresviiGUIUtility.RectScale(pinchStartRect, scale);

            if (imageRect.width < Screen.width)
            {
                imageRect = pinchStartRect;
            }

            if (imageRect.width > 6f * Screen.width)
            {
                imageRect = preImageRect;
            }
        }

        private float postScrennWidth = 0f;

        void Update()
        {
#if UNITY_EDITOR

            float enlarge = Input.GetAxis("Mouse ScrollWheel");

            if (enlarge > 0f)
            {
                imageRect = FresviiGUIUtility.RectScale(imageRect, 1.05f);
            }
            if (enlarge < 0f)
            {
                imageRect = FresviiGUIUtility.RectScale(imageRect, 0.95f);
            }

#endif

            if (loadingSpinner != null)
            {
                loadingSpinner.Position = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }

            if (postScrennWidth != Screen.width)
            {
                backgroundRect = new Rect(0f, 0f, Screen.width, Screen.height);

                if (texture != null)
                {
                    imagePosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

                    imageRect = new Rect(0f, 0f, Screen.width, Screen.width * texture.height / texture.width);

                    if (imageRect.height > Screen.height)
                    {
                        float w = texture.width * Screen.height / texture.height;

                        imageRect = new Rect(0.5f * (Screen.width - w), 0f, w, Screen.height);
                    }
                }

                postScrennWidth = Screen.width;
            }

            if (FASGesture.IsDragging || FASGesture.Inertia)
            {
                imagePosition = new Vector2(imagePosition.x + FASGesture.Delta.x, imagePosition.y - FASGesture.Delta.y);
            }

            /*if (imageRect.width >= Screen.width)
            {
                if (imageRect.x > 0f && imageRect.x + imageRect.width < Screen.width)
                {

                }
                else if (imageRect.x > 0f && imageRect.x + imageRect.width > Screen.width)
                {
                    imageRect.x = 0f;

                    imagePosition.x = imageRect.x + imageRect.width * 0.5f;
                }
                else if (imageRect.x + imageRect.width < Screen.width && imageRect.x < 0f)
                {
                    imageRect.x = Screen.width - imageRect.width;

                    imagePosition.x = imageRect.x + imageRect.width * 0.5f;
                }


                if (imageRect.y < 0f && imageRect.y + imageRect.height < Screen.height)
                {
                    if (imageRect.height < Screen.height)
                    {
                        imageRect.y = 0f;

                        imagePosition.y = imageRect.y + imageRect.height * 0.5f;
                    }
                    else
                    {
                        imageRect.y = Screen.height - imageRect.height;

                        imagePosition.y = imageRect.y + imageRect.height * 0.5f;
                    }
                }
                else if (imageRect.y > 0f && imageRect.y + imageRect.height > Screen.height)
                {
                    if (imageRect.height < Screen.height)
                    {
                        imageRect.y = Screen.height - imageRect.height;

                        imagePosition.y = imageRect.y + imageRect.height * 0.5f;
                    }
                    else
                    {
                        imageRect.y = 0f;

                        imagePosition.y = imageRect.y + imageRect.height * 0.5f;
                    }
                }
            }*/

            if (imagePosition.x + imageRect.width * 0.5f < 0.2f * Screen.width)
            {
                imagePosition.x = 0.2f * Screen.width - imageRect.width * 0.5f;          
            }

            if (imagePosition.x - imageRect.width * 0.5f > 0.8f * Screen.width)
            {
                imagePosition.x = 0.8f * Screen.width + imageRect.width * 0.5f;   
            }

            if (imagePosition.y + imageRect.height * 0.5f < 0.2f * Screen.height)
            {
                imagePosition.y = 0.2f * Screen.height - imageRect.height * 0.5f;
            }

            if (imagePosition.y - imageRect.height * 0.5f > 0.8f * Screen.height)
            {
                imagePosition.y = 0.8f * Screen.height + imageRect.height * 0.5f;
            }

            imageRect = new Rect(imagePosition.x - imageRect.width * 0.5f, imagePosition.y - imageRect.height * 0.5f, imageRect.width, imageRect.height);


#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Destroy(this.gameObject);

                OnDoneCallback();
            }
#endif
        }

        void OnGUI()
        {
            GUI.depth = guiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            if (buttonClose.IsTap(Event.current, backgroundRect))
            {
                Destroy(this.gameObject);

                if (loadingSpinner != null)
                    loadingSpinner.Hide();

                OnDoneCallback();
            }

            if (this.texture != null)
            {
                GUI.DrawTexture(imageRect, texture, ScaleMode.ScaleToFit);
            }

			Event.current.Use();
        }

        /*void OnDestroy()
        {
            if (texture != null)
            {
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(imageUrl);
            }
        }*/
    }
}