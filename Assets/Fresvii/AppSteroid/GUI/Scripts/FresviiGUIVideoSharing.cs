#pragma warning disable 0414
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIVideoSharing : MonoBehaviour
    {
		#if UNITY_IOS
		[DllImport ("__Internal")]    
		private static extern string _FasGetVideoThumbnailImagePath(string videoFilePath);

		[DllImport ("__Internal")]    
		private static extern void _FasSendMail(string title, string message, string to, string callbackObjectName);

		#endif

        private static FresviiGUIVideoSharing instance;

        public enum ShareService { Facebook = 0, Twitter, YouTube, Email };

        public int guiDepth;

        public Camera videoGuiCamera;

        public Vector2 landscapeMargin;

        public Vector2 portraitMargin;

        private int screenWidth;

        private string videoFilePath;

        public RectTransform guiPivot;

        public CanvasScaler canvasScaler;

        public Vector2 canvasScalerMatch;

        public FresviiGUIToggleButton[] shareServiceButtons;

        public RawImage videoThumbnailImage;

        private Texture2D thumbnailTexture;

        private bool isUploading;

        public Canvas canvas;

        public InputField commentInputField;

        public Image progressBar;

        private string returnSceneName;

        public EventSystem eventSystem;

        private EventSystem postEventSystem;

        private Fresvii.AppSteroid.Models.Video video;

        private bool isOnlySharingProcess;

		public GameObject toneDownUpper, toneDownBottom;

		private static bool hasThumbnail = false;

		private static readonly int TweetMaxLength = 116;

		public static void Show(int guiDepth, Fresvii.AppSteroid.Models.Video video, Texture2D videoThumbnail)
        {
            if (instance != null)
            {
				Destroy(instance.gameObject);
            }

			hasThumbnail = true;

			((GameObject)Instantiate(Resources.Load("GuiPrefabs/VideoSharingGUI"))).GetComponent<FresviiGUIVideoSharing>();

            instance.InitWithTexture(guiDepth, video, videoThumbnail);
        }

		private bool goToUploaded = false;

        public static void Hide()
        {
            if (instance == null) return;

            if (instance.isUploading)
            {
				return;
            }
            else
            {
				if(instance.isOnlySharingProcess)
				{
					instance.thumbnailTexture = null;
					
					instance.videoThumbnailImage.material.mainTexture = null;
				}
				else if (instance.thumbnailTexture != null && !instance.goToUploaded)
                {
                    if (!hasThumbnail)
                        Destroy(instance.thumbnailTexture);

                    instance.thumbnailTexture = null;

                    instance.videoThumbnailImage.material.mainTexture = null;
                }

                if (System.IO.File.Exists(instance.videoFilePath))
                {
                    System.IO.File.Delete(instance.videoFilePath);
                }

                Destroy(instance.canvas.gameObject);
            }
        }

        void Awake()
        {
			FresviiGUIText.SetUp(Application.systemLanguage);

			if(instance != null && !hasThumbnail)
			{
				Destroy(instance.gameObject);
			}

            instance = this;

			if(!hasThumbnail)
				instance.InitByNewGameObject(FASVideo.ModalGuiDepth, FASVideo.VideoFilePath, FASGui.ReturnSceneName, EventSystem.current);       

			if(FASPlayVideo.videoShareGuiMode == FASPlayVideo.VideoShareGuiMode.WithLegacyGUI)
			{
				instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
			}
			else if(FASPlayVideo.videoShareGuiMode == FASPlayVideo.VideoShareGuiMode.WithUGUI)
			{
				instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;				
			}

			instance.gameObject.transform.SetAsLastSibling();

        }

		IEnumerator Start(){

			while(!FAS.Initialized)
			{
				yield return 1;
			}

			Login(); 
		}

        void Login()
        {
            if (!FASUser.IsLoggedIn())
            {
                FASUser.RelogIn(

                    (error) =>
                    {
                        List<Fresvii.AppSteroid.Models.User> signedUpUsers = FASUser.LoadSignedUpUsers();

                        Fresvii.AppSteroid.Models.User currentUser = null;

                        if (signedUpUsers.Count > 0)
                        {
                            currentUser = signedUpUsers[signedUpUsers.Count - 1]; // saved user has only id and token data.

                            FASUser.LogIn(currentUser.Id, currentUser.Token, (e)=>
                            {
                                if (e != null)
                                {
                                    Debug.LogError("Video sharing : Not signed up");

                                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");

                                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(" Not signed up", (del) => 
                                    {
                                        Hide();                 

										FASPlayVideo.GuiEndCallback();
                                    });
                                }
                            });
                        }
                   }

                );
            }
        }

        public void Close()
        {
            Hide();
        }

		void OnDestroy()
        {
            if (!goToUploaded)
            {
                FASPlayVideo.GuiEndCallback();
            }

			hasThumbnail = false;

			FASGesture.Resume();
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

        bool wantToTweet = false;

        public void OnClickShareButton(int type)
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                return;
            }

            if (type == 0) // Facebook
            {
                if (!Fresvii.AppSteroid.Util.SocialNetworkingService.IsFacebookEnable())
                {
                    Debug.LogError("FASSNS.ShareFacebook : error");

                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");

                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("FacebookError"), (del) => { });

                    return;
                }
            }
            else if (type == 1) // Twitter
            {
                if (!Fresvii.AppSteroid.Util.SocialNetworkingService.IsTwitterEnable())
                {
                    Debug.LogError("FASSNS.ShareTwitter : error");

                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");

                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TwitterError"), (del) => { });

                    return;
                }
            }

            /*if (isOnlySharingProcess)
            {
                shareButton.interactable = (shareServiceButtons[0].IsOn) || (shareServiceButtons[1].IsOn) || (shareServiceButtons[2].IsOn) || (shareServiceButtons[3].IsOn);
            }*/

            shareServiceButtons[type].IsOn = !shareServiceButtons[type].IsOn;

            wantToTweet = shareServiceButtons[1].IsOn;

			if(shareServiceButtons[1].IsOn && (commentInputField.text.Length > TweetMaxLength)){

				shareServiceButtons[1].IsOn = false;

				Debug.LogError("FASSNS.ShareTwitter : text too long");
				
				Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TweetIsTooLong") + commentInputField.text.Length +".", (del) => { });
			}
        }

        private void InitByNewGameObject(int guiDepth, string videoFilePath, string returnSceneName, EventSystem postEventSystem)
        {
			if(!System.IO.File.Exists(videoFilePath))
			{
				goToUploaded = false;

				Hide();
			
				return;
			}

            this.guiDepth = guiDepth;

            this.videoFilePath = videoFilePath;

            this.returnSceneName = returnSceneName;

            this.postEventSystem = postEventSystem;

			StartCoroutine(GetVideoThumbnailCoroutine(videoFilePath));
        }

        private void InitWithTexture(int guiDepth, Fresvii.AppSteroid.Models.Video video, Texture2D videoThumbnail)
        {
			this.guiDepth = guiDepth;

            this.videoFilePath = "";

            this.returnSceneName = "";

            this.postEventSystem = EventSystem.current;

            this.thumbnailTexture = videoThumbnail;

            this.videoThumbnailImage.material.mainTexture = videoThumbnail;

            this.video = video;

            this.isOnlySharingProcess = true;

            //shareButton.interactable = false;
        }

		private IEnumerator GetVideoThumbnailCoroutine(string videoFilePath)
		{

#if UNITY_IOS && !UNITY_EDITOR

			string thumbnailPath = _FasGetVideoThumbnailImagePath(videoFilePath);

			yield return new WaitForSeconds(0.1f);

			Texture2D thumbnail = new Texture2D(4,4);
			
			byte[] imageBytes = System.IO.File.ReadAllBytes(thumbnailPath);
			
			thumbnail.LoadImage(imageBytes);

			/*if(thumbnail.width > 256 || thumbnail.height > 256){
				
				Color32[] colors = ScaleUnityTexture.ScaleAndSquareCropLnaczos(thumbnail.GetPixels32(), thumbnail.width, thumbnail.height, 256);

				thumbnail = new Texture2D(256, 256);
				
				thumbnail.SetPixels32(colors);
				
				thumbnail.Apply();
			}*/

			this.thumbnailTexture = thumbnail;

			this.videoThumbnailImage.material.mainTexture = thumbnail;

			if(thumbnail.width > thumbnail.height){

				float offsetPixelX = 0.5f * (thumbnail.width - thumbnail.height);
				float offsetX = offsetPixelX / thumbnail.width;
				float scaleX = (thumbnail.width - 2f * offsetPixelX) / thumbnail.width;
				this.videoThumbnailImage.material.mainTextureOffset = new Vector2(offsetX, 0f);
				this.videoThumbnailImage.material.mainTextureScale = new Vector2(scaleX, 1f);
			}
			else{
				float offsetPixelY = 0.5f * (thumbnail.height - thumbnail.width);
				float offsetY = offsetPixelY / thumbnail.height;
				float scaleY = (thumbnail.height - 2f * offsetPixelY) / thumbnail.height;
				this.videoThumbnailImage.material.mainTextureOffset = new Vector2(0f, offsetY);
				this.videoThumbnailImage.material.mainTextureScale = new Vector2(1f, scaleY);

			}

#else
			yield return 1;
#endif
		}


        public void Progress(float progress)
        {
            progressBar.fillAmount = progress;
        }

		public Button shareButton;

		public Text shareButtonText;

		void OnEmailDone(string msg)
		{
			shareProcessing = false;

			if(msg=="Sent")
			{
				isSomethingSent = true;
			}
		}

        private bool tweetOverAlerted = false;

        public void OnEditInputField()
        {
            if (wantToTweet && (commentInputField.text.Length > TweetMaxLength) && !tweetOverAlerted)
            {
                tweetOverAlerted = true;

                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TweetOvered"), delegate(bool del) { });
            }

            if (commentInputField.text.Length <= TweetMaxLength)
            {
                tweetOverAlerted = false;
            }

			shareServiceButtons[1].IsOn = wantToTweet && (commentInputField.text.Length <= TweetMaxLength);
        }

        public void OnClickShare()
        {
            if (isUploading) return;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });

                return;
            }

			isUploading = true;

            if (video == null)
            {

#if UNITY_EDITOR
                OnCompleteUploadVideo(null, null);
#else
                FASVideo.UploadVideo(videoFilePath, null, commentInputField.text, OnCompleteUploadVideo, Progress);
#endif
                shareButtonText.text = "Uploading...";
            }
            else
            {
                OnCompleteUploadVideo(video, null);
            }

			shareButton.interactable = false;

			commentInputField.interactable = false;

			foreach(FresviiGUIToggleButton toggleButton in shareServiceButtons)
				toggleButton.button.interactable = false;
        }

        void OnCompleteUploadVideo(Fresvii.AppSteroid.Models.Video video, Fresvii.AppSteroid.Models.Error error)
        {
			isUploading = false;

			if (error != null)
            {
                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Cancel"));

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("VideoUploadErrorAndRetry"), 
             
                    (del) => 
                    {
                        if (del)
                        {
                            OnClickShare();
                        }
                        else
                        {
                            if (System.IO.File.Exists(videoFilePath))
                            {
                                System.IO.File.Delete(videoFilePath);
                            }

							FresviiGUIVideoSharing.Hide();

							FASGesture.Resume();
                        }
                    });

				if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
				{
					Debug.LogError(error.ToString());
				}
            }
            else
            {
				StartCoroutine(ShareCoroutine(video));
			}
        }

		private bool shareProcessing;

		private bool coroutineRunning = false;

		bool isSomethingSent = false;

		IEnumerator ShareCoroutine(Fresvii.AppSteroid.Models.Video video)
		{
			if(coroutineRunning)
			{
				yield break;
			}            

			coroutineRunning = true;

			byte[] videoData = null;

			string tmpFilePath = "";

            isSomethingSent = false;

			//  Facebook
			if (shareServiceButtons[0].IsOn)
			{
                shareProcessing = true;

				Fresvii.AppSteroid.Util.SocialNetworkingService.ShareFacebook(commentInputField.text, video.VideoUrl, (result) => { 

                    shareProcessing = false;

                    if (result == Fresvii.AppSteroid.Util.SocialNetworkingService.Result.Done)
                    {
                        isSomethingSent = true;
                    }

                    if (result == Fresvii.AppSteroid.Util.SocialNetworkingService.Result.Error)
                    {
                        Debug.LogError("FASSNS.ShareFacebook : error");

                        Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");

                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("FacebookError"), (del) => { });
                    }
                
                });

			}

            while (shareProcessing) yield return 1;

			if(System.IO.File.Exists(tmpFilePath))
				System.IO.File.Delete(tmpFilePath);


			// Twitter
			if (shareServiceButtons[1].IsOn)
			{
				shareProcessing = true;

				Fresvii.AppSteroid.Util.SocialNetworkingService.ShareTwitter(commentInputField.text + " " + video.VideoUrl, (result) =>
                {
                    shareProcessing = false;

                    if (result == Fresvii.AppSteroid.Util.SocialNetworkingService.Result.Done)
                    {
                        isSomethingSent = true;
                    }
                    else if (result == Fresvii.AppSteroid.Util.SocialNetworkingService.Result.Error)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

						Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TweetError"), (del) => { });
                    }
                });
			}

            while (shareProcessing) yield return 1;

			// YouTube
            if (shareServiceButtons[2].IsOn)
            {
                shareProcessing = true;

       			shareButtonText.text = "Uploading to Youtube...";

                progressBar.fillAmount = 0f;

				Fresvii.AppSteroid.Util.DialogManager.Instance.ShowProgressSpinnerDialog("", "Uploading ...", false);

				toneDownUpper.SetActive(true);

				toneDownBottom.SetActive(false);

				toneDownUpper.GetComponent<RectTransform>().localScale = Vector3.one;

                if (isOnlySharingProcess && videoData == null)
                {
                    FAS.Instance.Client.VideoService.DownloadVideo(video.VideoUrl, (data, error) =>
                    {
                        shareProcessing = false;

                        if (error != null)
                        {
                            Debug.LogError("FASSNS.ShareYoutube : " + error.ToString());

                            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");

                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Youtube Upload Error.", (del) => { });
                        }
                        else
                        {
                            shareProcessing = true;

                            Fresvii.AppSteroid.Util.SocialNetworkingService.ShareYoutube(data, FASConfig.Instance.appName, commentInputField.text,

			                    (result) =>
                                {
                                    shareProcessing = false;

									if (result == Fresvii.AppSteroid.Util.SocialNetworkingService.Result.Error)
                                    {
                                        Debug.LogError("FASSNS.ShareYoutube : Error");

                                        Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");

                                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Youtube Upload Error.", (del) => { });
                                    }
									else if (result == Fresvii.AppSteroid.Util.SocialNetworkingService.Result.Done)
                                    {
                                        isSomethingSent = true;
                                    }
                                },

                                null

                            );
                        }
                    });
                }
				else if(isOnlySharingProcess && videoData != null)
				{
                    Fresvii.AppSteroid.Util.SocialNetworkingService.ShareYoutube(videoData, FASConfig.Instance.appName, commentInputField.text,
					                   
					(result) =>
					{
						shareProcessing = false;

						if (result == Fresvii.AppSteroid.Util.SocialNetworkingService.Result.Error)
						{
                            Debug.LogError("FASSNS.ShareYoutube : Error");

                            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");

                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Youtube Upload Error.", (del) => { });
                        }
						else if (result == Fresvii.AppSteroid.Util.SocialNetworkingService.Result.Done)
                        {
                            isSomethingSent = true;
                        }
					},
					
					null
					
					);
				}
                else
                {
                    Fresvii.AppSteroid.Util.SocialNetworkingService.ShareYoutube(videoFilePath, FASConfig.Instance.appName, commentInputField.text,

                        (result) =>
                        {
                            shareProcessing = false;

                            if (result == Fresvii.AppSteroid.Util.SocialNetworkingService.Result.Error)
                            {
                                Debug.LogError("FASSNS.ShareYoutube : Error");

                                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");

                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Youtube Upload Error.", (del) => { });
                            }
							else if (result == Fresvii.AppSteroid.Util.SocialNetworkingService.Result.Done)
                            {
                                isSomethingSent = true;
                            }
                        },

                        null

                    );
                }
            }

            while (shareProcessing)
            {
                yield return 1;
            }

			toneDownUpper.SetActive(false);

			toneDownUpper.GetComponent<RectTransform>().localScale = Vector3.zero;

			toneDownBottom.SetActive(true);

			Fresvii.AppSteroid.Util.DialogManager.Instance.HideProgressSpinnerDialog();

			// Email
			if (shareServiceButtons[3].IsOn)
			{
				TextAsset textAsset = Resources.Load("Texts/SharingVideoByEmailHTMLTemplete") as TextAsset;

                string body = textAsset.text;

                body = body.Replace("_comment_", commentInputField.text);

                body = body.Replace("_video_url_", video.VideoUrl);

				#if UNITY_EDITOR
				
				#elif UNITY_IOS 
				_FasSendMail(FresviiGUIText.Get("VideoSharingEmailSubjectPrefix") + FASSettings.Instance.appName, body, "", this.gameObject.name);
				#elif UNITY_ANDROID 

				#endif
								                
				shareProcessing = true;
            }


			while (shareProcessing)
			{
				yield return 1;
			}


			this.videoThumbnailImage.material.mainTexture = null;

            if (!isOnlySharingProcess)
            {
				goToUploaded  = true;

				FresviiGUIVideoUploaded.Show(guiDepth, videoFilePath, thumbnailTexture, video, returnSceneName);
            }
			else
			{
                if (isSomethingSent)
				{
					Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");
					
					Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Video sent", (del) => { });
				}
			}

			coroutineRunning = false;

			FresviiGUIVideoSharing.Hide();
		}
    }
}