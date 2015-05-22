#pragma warning disable 0414
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Runtime.InteropServices;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIVideoUploaded : MonoBehaviour
    {
        private static FresviiGUIVideoUploaded instance;

        public int guiDepth;

        public Camera videoGuiCamera;

        public Vector2 landscapeMargin;

        public Vector2 portraitMargin;

        private int screenWidth;

        private string videoFilePath;

        public RectTransform guiPivot;

        public CanvasScaler canvasScaler;

        public Vector2 canvasScalerMatch;

        public RawImage videoThumbnailImage;

        private Texture2D thumbnailTexture;

        private Fresvii.AppSteroid.Models.Video video;

        public Canvas canvas;

        public Text textUrl;

        public Text textCommunity;

        private string returnSceneName;

        public EventSystem eventSystem;
		
        public static void Show(int guiDepth, string videoFilePath, Texture2D videoThumbnail, Fresvii.AppSteroid.Models.Video video, string returnSceneName)
        {
            if (instance != null)
            {
                return;
            }

            instance = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/VideoUploadedGUI"))).GetComponent<FresviiGUIVideoUploaded>();

            instance.Init(guiDepth, videoFilePath, videoThumbnail, video, returnSceneName);
        }

        public static void Hide()
        {            
            if (instance != null)
            {
                if (System.IO.File.Exists(instance.videoFilePath))
                {
                    System.IO.File.Delete(instance.videoFilePath);
                }

				instance.videoThumbnailImage.material.mainTexture = null;

				if(instance.thumbnailTexture != null)
					Destroy(instance.thumbnailTexture);

				Destroy(instance.canvas.gameObject);
			}
        }

		void Start()
		{
			if(FASPlayVideo.videoShareGuiMode == FASPlayVideo.VideoShareGuiMode.WithLegacyGUI)
			{
				instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
			}
			else if(FASPlayVideo.videoShareGuiMode == FASPlayVideo.VideoShareGuiMode.WithUGUI)
			{
				instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				
				instance.gameObject.transform.SetAsLastSibling();
			}
		}

        public void Close()
        {
            FresviiGUIVideoUploaded.Hide();
        }

        public void OnClickCommunityButton()
        {
			FASGui.ShowGUI(FASGui.Mode.All, FASGui.ReturnSceneName, FASGui.Mode.Forum);
		}

        private int postGUIDepth;

        public void OnClickVideoListButton()
        {
            /*postGUIDepth = this.guiDepth;

            if(this.guiDepth == int.MinValue)
            {
                this.guiDepth += 1000;
            }

            FresviiGUIVideoList.Show(null, this.guiDepth - 10, 
			    
			    (video, tex) =>
	            {
	                this.guiDepth = postGUIDepth;

					videoListShowing = false;

					this.gameObject.SetActive(true);
	            }, 
	            
            FresviiGUIVideoList.Mode.FromUploded, 

			()=>
			{
				FASGesture.Resume();

				videoListShowing = true;

				this.gameObject.SetActive(false);

                Hide();
			}

			);*/

            FresviiGUIMyProfile.StartPage = FresviiGUIMyProfile.Page.VideoList;

			FASGui.ShowGUI(FASGui.Mode.All, FASGui.ReturnSceneName, FASGui.Mode.MyProfile);
        }

		void OnDestroy()
        {
			FASGesture.Resume();

            FASPlayVideo.GuiEndCallback();

            if(thumbnailTexture)
                Destroy(thumbnailTexture);
		}

        public void OnClickURLCopy()
        {
            Fresvii.AppSteroid.Util.Clipboard.SetText(video.VideoUrl);
        }

        void Update()
        {            
            FASGesture.Pause();

			if(EventSystem.current == null){
				
				eventSystem.gameObject.SetActive(true);
				
				EventSystem.current = eventSystem;
			}

			if (screenWidth != Screen.width)
            {
                screenWidth = Screen.width;

                if (Screen.width > Screen.height)
                {
                    guiPivot.offsetMin = new Vector2(landscapeMargin.x, landscapeMargin.y);

                    guiPivot.offsetMax = new Vector2(-landscapeMargin.x, -landscapeMargin.y);

                    canvasScaler.matchWidthOrHeight = canvasScalerMatch.y;
                }
                else
                {
                    guiPivot.offsetMin = new Vector2(portraitMargin.x, portraitMargin.y);

                    guiPivot.offsetMax = new Vector2(-portraitMargin.x, -portraitMargin.y);

                    canvasScaler.matchWidthOrHeight = canvasScalerMatch.x;
                }
            }
        }

        void OnGUI()
        {            
			if(FASPlayVideo.videoShareGuiMode != FASPlayVideo.VideoShareGuiMode.WithLegacyGUI)
				return;
			
			GUI.depth = guiDepth;

            if (Event.current.type == EventType.Repaint)
            {
                videoGuiCamera.Render();
            }

            if (Event.current.isMouse && Event.current.button >= 0)
            {
                Event.current.Use();
            }
		}

        public void Init(int guiDepth, string videoFilePath, Texture2D videoThumbnail, Fresvii.AppSteroid.Models.Video video, string returnSceneName)
        {
            this.guiDepth = guiDepth;

            this.videoFilePath = videoFilePath;

            this.thumbnailTexture = videoThumbnail;

            this.videoThumbnailImage.material.mainTexture = videoThumbnail;

            this.returnSceneName = returnSceneName;

            this.video = video;

            textCommunity.text = FASConfig.Instance.appName + " Community";

            if (videoThumbnail == null || video == null)
                return;

            if (videoThumbnail.width * 0.6f > videoThumbnail.height)
            {
                float offsetPixelX = 0.5f * (videoThumbnail.width - videoThumbnail.height * 5f / 3f);

                float offsetX = offsetPixelX / videoThumbnail.width;

                float scaleX = (videoThumbnail.width - 2f * offsetPixelX) / videoThumbnail.width;

                this.videoThumbnailImage.material.mainTextureOffset = new Vector2(offsetX, 0f);

                this.videoThumbnailImage.material.mainTextureScale = new Vector2(scaleX, 1f);
            }
            else
            {
                float offsetPixelY = 0.5f * (videoThumbnail.height - videoThumbnail.width * 0.6f);

                float offsetY = offsetPixelY / videoThumbnail.height;

                float scaleY = (videoThumbnail.height - 2f * offsetPixelY) / videoThumbnail.height;

                this.videoThumbnailImage.material.mainTextureOffset = new Vector2(0f, offsetY);

                this.videoThumbnailImage.material.mainTextureScale = new Vector2(1f, scaleY);
            }

            textUrl.text = video.VideoUrl;
        }
    }
}