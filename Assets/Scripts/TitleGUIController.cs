using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;


/*
 * タイトル画面GUI管理クラス / GUI management class in title screen
 * 
 * 画面入力を制御する.
 * control screen input. 
 * 
*/
public class TitleGUIController : MonoBehaviour {
	public SpriteRenderer btnPlay;
	public SpriteRenderer btnForum;
	public SpriteRenderer btnHelp;
	public SpriteRenderer btnSound;
	public SpriteRenderer btnSignUpOK;
	public SpriteRenderer btnSignUpSkip;

	//quit
	public SpriteRenderer btnQuitYes;
	public SpriteRenderer btnQuitNo;

	public Transform helpObj;	//HelpCamera's child

	//shop
	public SpriteRenderer btnShop;
	public Sprite[] btnShopSprites;
	public GameObject shopObj;

	public Sprite[] btnPlaySprites;		//[0]=off [1]=on
	public Sprite[] btnForumSprites;
	public Sprite[] btnHelpSprites;
	public Sprite[] btnSoundSprites;	//[0]=on_off [1]=on_on [2]=off_off [3]=off_on
	
	public Sprite[] btnSignUpOKSprites;
	public Sprite[] btnSignUpSkipSprites;

	//quit btn
	public Sprite[] btnQuitYesSprites;
	public Sprite[] btnQuitNoSprites;


	public SoundController soundButton;
	public SoundController soundBGM;

	public GameObject signUpParent;
	public GameObject[] turnoffInSignUp;
	
	public GUISkin mySkin;
	public GameObject quitParent;
	public GameObject noConnectrionParent;

	private const string SOUND_SAVE_KEY = "IsSoundOn";
	private bool isCanInput = true;
	private bool isPusshing;	//押しっぱなし防止用フラグ. / flag for press keep prevention
	private bool isShowOnGUI;	//UnityGUI発動フラグ.
	private bool isCoroutine;	//コルーチン起動済みフラグ.

	private FresviiForumGUI fresviiComp;
	private string userName = "";

	//private float screenRotTimer = -1.0f;

	//------------------------------
	// Use this for initialization
	//------------------------------
	void Awake () {
		//iOS8かどうか判定する.
		#if UNITY_IPHONE && !UNITY_EDITOR
			string version_str = SystemInfo.operatingSystem.Replace("iPhone OS ", "");
			float version_float = -1.0f;
			float.TryParse(version_str.Substring(0,1), out version_float);
			if (version_float == 8.0f) {
				GameController.isIOS8 = true;
			}
		#endif
	
		//端末の回転を制御（Fresvii SDKに入る時に許可した縦持ちを禁止する）（iOS8でない時のみ）.
		//if (!GameController.isIOS8) {
			if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
				Screen.orientation = ScreenOrientation.LandscapeLeft;
			}
			Screen.autorotateToPortrait = false;
			Screen.autorotateToPortraitUpsideDown = false;
		//}

		//ゲームオーバー時のサウンドがまだ残っていたら消す.
		GameObject goBGM = GameObject.Find("GameOverBGM");
		if (goBGM != null) {
			Destroy(goBGM);
		}

		//サウンドオンオフを設定 / set sound on-off
		GameController.isSoundOn = (PlayerPrefs.GetInt(SOUND_SAVE_KEY, 1) == 1);
		if (GameController.isSoundOn) {
			btnSound.sprite = btnSoundSprites[0];
		} else {
			btnSound.sprite = btnSoundSprites[2];
		}

		Application.targetFrameRate = 60;
		isCanInput = true;

		//コンポーネント取得 / get component
		fresviiComp = gameObject.GetComponent<FresviiForumGUI>();
	}

	//------------------------------
	// Update is called once per frame
	//------------------------------
	void Update () {
		//端末の画面回転を許可する.
		//if (!GameController.isIOS8) {
			if (Screen.orientation == ScreenOrientation.LandscapeLeft) {
				Screen.orientation = ScreenOrientation.AutoRotation;
			}
		//}

		CheckInput();

		//Androidで戻るキーでアプリを終了させる為のダイアログ呼び出し.
		if(Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape)){
			StartCoroutine("ShowQuitDelay", false);
			isCoroutine = true;
			
			btnQuitYes.sprite = btnQuitYesSprites[0];
			btnQuitNo.sprite = btnQuitNoSprites[0];
		}
	}

	//------------------------------
	//OnGUI
	//------------------------------
	void OnGUI() {
		if (isShowOnGUI) {
			//ウィンドウの拡大率と中心座標を取得する / get window scale factor and center position
			Vector2 scaleFactor = new Vector2(Screen.width / 1136.0f, Screen.height / 640.0f);
			Vector2 centerOfWindow = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
			
			float userNameAreaWidth = 630.0f * scaleFactor.x;
			float userNameAreaHeight = 70.0f * scaleFactor.y;
			float contentOffsetY = -23.0f;
			
			GUISkin useSkin = Instantiate(mySkin) as GUISkin;
			float fontSize = (float)mySkin.textField.fontSize;
			fontSize *= scaleFactor.x;
			useSkin.textField.fontSize = (int)fontSize;


			GUI.skin = useSkin;
			userName = GUI.TextField(new Rect(centerOfWindow.x - userNameAreaWidth / 2.0f,
			                                  centerOfWindow.y - userNameAreaHeight / 2.0f + contentOffsetY * scaleFactor.y,
			                                  userNameAreaWidth, userNameAreaHeight), userName, 9);
		}
	}

	//------------------------------
	//入力チェック / check input
	//------------------------------
	void CheckInput() {
		//モバイルではタッチで操作 / Operation in touch on mobile
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {

			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++) {
				if( touches[i].phase == TouchPhase.Began) {
					RayFromInput( touches[i].position, touches[i].fingerId );
				}
			}
			if (touches.Length == 0) {
				btnForum.sprite = btnForumSprites[0];
				if (GameController.isSoundOn) {
					btnSound.sprite = btnSoundSprites[0];
				} else {
					btnSound.sprite = btnSoundSprites[2];
				}
				isPusshing = false;
			}
			
		//それ以外ではクリックで操作 / Operation Click otherwise
		} else {
			if (Input.GetMouseButton(0)) {
				RayFromInput(Input.mousePosition, 0);
			} else {
				btnForum.sprite = btnForumSprites[0];
				
				if (GameController.isSoundOn) {
					btnSound.sprite = btnSoundSprites[0];
				} else {
					btnSound.sprite = btnSoundSprites[2];
				}
				isPusshing = false;
			}
		}
	}

	//------------------------------
	//入力座標からボタンが押されているか調べる / examine whether the button is pressed from the input coordinate
	//------------------------------
	void RayFromInput(Vector2 point, int fingerID) {
		RaycastHit hit = new RaycastHit();
		Ray ray = Camera.main.ScreenPointToRay( point );
		if ( Physics.Raycast( ray, out hit ) && !isPusshing && isCanInput) {
			isPusshing = true;

			//ゲーム開始 / Game start
			if(hit.transform == btnPlay.transform) {
				soundButton.Play();
				btnPlay.sprite = btnPlaySprites[1];
				Application.LoadLevel("GameScene");
			}

			//フォーラム / Forum
			if(hit.transform == btnForum.transform) {
				btnForum.sprite = btnForumSprites[1];
				soundButton.Play();
				
				//ネットワークが繋がっていたらフォーラムへ、いなかったらGUI表示.
				if (Application.internetReachability != NetworkReachability.NotReachable){
					if (fresviiComp.GetAlreadySignup()) {
						//isCanInput = false;
						//#FGC Forum Start――――――
						fresviiComp.ShowFGCGui("");
						//#FGC Forum End――――――
					} else {
						foreach (GameObject item in turnoffInSignUp) {
							item.SetActive(false);
						}
						signUpParent.SetActive(true);
						isShowOnGUI = true;
					}
				} else {
					noConnectrionParent.SetActive(true);
					StartCoroutine("HideNoConnection");
				}
			}

			//ヘルプ（表示） / Help visible
			if(hit.transform == btnHelp.transform && !isCoroutine) {
				soundButton.Play();
				btnHelp.sprite = btnHelpSprites[1];
				isCoroutine = true;
				StartCoroutine("ShowHelpDelay");
			}
			//ヘルプ（非表示） / Help unvisible
			if(hit.transform == helpObj) {
				soundButton.Play();
				btnHelp.sprite = btnHelpSprites[0];
				helpObj.parent.gameObject.SetActive(false);
			}

			//サウンド / Sound
			if(hit.transform == btnSound.transform) {
				GameController.isSoundOn = !GameController.isSoundOn;
				if (GameController.isSoundOn) {
					PlayerPrefs.SetInt(SOUND_SAVE_KEY, 1);
					btnSound.sprite = btnSoundSprites[1];
					soundBGM.Play();
				} else {	
					PlayerPrefs.SetInt(SOUND_SAVE_KEY, 0);
					btnSound.sprite = btnSoundSprites[3];
					soundBGM.Stop();
				}
				soundButton.Play();
				PlayerPrefs.Save();
			}

			
			//サインアップ時：OK / SignUp : OK
			if(hit.transform == btnSignUpOK.transform) {
				soundButton.Play();
				btnSignUpOK.sprite = btnSignUpOKSprites[1];
				isShowOnGUI = false;
				isCanInput = false;

				//#FGC Forum Start――――――
				fresviiComp.ShowFGCGui(userName);
				//#FGC Forum End――――――
			}
			//サインアップ時：skip / SignUp : skip
			if(hit.transform == btnSignUpSkip.transform) {
				soundButton.Play();
				btnSignUpSkip.sprite = btnSignUpSkipSprites[1];
				isShowOnGUI = false;
				isCanInput = false;
				
				//#FGC Forum Start――――――
				fresviiComp.ShowFGCGui("");
				//#FGC Forum End――――――
			}
			
			//ショップ / Shop
			if(hit.transform == btnShop.transform && !isCoroutine) {
				soundButton.Play();
				btnShop.sprite = btnShopSprites[1];
				StartCoroutine("ShowShopDelay");
			}


			//終了：OK / Quit : Yes
			if(hit.transform == btnQuitYes.transform) {
				soundButton.Play();
				btnQuitYes.sprite = btnQuitYesSprites[1];

				//終了処理.
				StartCoroutine("DoQuitDelay");
			}
			//終了：No / Quit : No
			if(hit.transform == btnQuitNo.transform && !isCoroutine) {
				soundButton.Play();
				btnQuitNo.sprite = btnQuitNoSprites[1];

				StartCoroutine("ShowQuitDelay", true);
				isCoroutine = true;
				btnQuitNo.sprite = btnQuitNoSprites[0];
			}
		}
	}

	//------------------------------
	//ヘルプと終了の画像表示.
	//（ボタンの変化を見せるため、一瞬遅れて実行する。）.
	//------------------------------
	private IEnumerator ShowHelpDelay () {
		yield return new WaitForSeconds (0.1f);
		helpObj.parent.gameObject.SetActive(true);
		isCoroutine = false;
	}
	private IEnumerator ShowQuitDelay (bool isNo) {
		yield return new WaitForSeconds (0.1f);
		//使用するフラグ（Noの時は逆転する）.
		bool isActive = false;
		if (isNo) isActive = !isActive;

		//オブジェクトの切り替え.
		foreach (GameObject item in turnoffInSignUp) {
			item.SetActive(isActive);
		}
		quitParent.SetActive(!isActive);
		isCoroutine = false;
	}
	private IEnumerator DoQuitDelay () {
		yield return new WaitForSeconds (0.1f);
		Application.Quit();
	}
	private IEnumerator ShowShopDelay () {
		yield return new WaitForSeconds (0.1f);
		shopObj.SetActive(true);
		yield return new WaitForSeconds (0.1f);
		GameController.SetBalloonText();	//風船の数セット.
		isCoroutine = false;
		btnShop.sprite = btnShopSprites[0];
	}

	//------------------------------
	//No Connection画像表示をオン・オフ.
	//------------------------------
	private IEnumerator HideNoConnection() {
		yield return new WaitForSeconds (2.5f);
		noConnectrionParent.SetActive (false);
	}
}
