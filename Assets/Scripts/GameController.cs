using UnityEngine;
using System.Collections;
using Soomla;
using Soomla.Example;
using Soomla.Store;
using System;
using Fresvii.AppSteroid;

/*
 * ゲーム管理クラス / Game Control Class
 * 
 * ゲーム全体を制御する.
 * control the entire game.
 * 
*/
public class GameController : MonoBehaviour {
	public string leaderboardId;
	public static string LEADERBOARD_ID;

	public static bool isCanInput = false;
	public static int nowScore = 0;
	public static int nowStage = 1;
	public static bool isSoundOn = true;
	public static int shopBalloon = 0;
	

	public static GameObject gamestartGUI;
	public static GameObject gameoverGUI;
	public static GameObject continueGUI;
	public static ResultController resultCont;
	public static PlayerController player;
	public static TimerController timerCont;
	public static LeaderBoardGUIController leaderboardCont;
	public static CurrentBalloonController balloonCont;

	private static StageController stageCont;
	private static BackGroundController bgCont;
	private static GameSettings settings;

	//課金用.
	private static bool isIAPInit;
	private static ExampleEventHandler handler;
	private static int useBalloon;	//消費する風船.
	private const int MAX_USE_BALLOON = 9;	//消費する最大の風船数.

	//iOSバージョン.
	public static bool isIOS8 = false;

	void Awake() {
	
		gamestartGUI = GameObject.Find("gamestartGUI");
		leaderboardCont = GameObject.Find("leaderboardGUI").GetComponent<LeaderBoardGUIController>();
		gameoverGUI = GameObject.Find("gameoverGUI");
		continueGUI = GameObject.Find("continueGUI");
		continueGUI.SetActive(false);

		resultCont = GameObject.Find("Result").GetComponent<ResultController>();
		player = GameObject.Find("Character").GetComponent<PlayerController>();
		stageCont = GameObject.Find("AreaObjects").GetComponent<StageController>();
		timerCont = GameObject.Find("Timer").GetComponent<TimerController>();
		balloonCont = GameObject.Find("CurrentBalloon").GetComponent<CurrentBalloonController>();
		bgCont = GameObject.Find("BackGround").GetComponent<BackGroundController>();
		settings = gameObject.GetComponent<GameSettings>();

		useBalloon = 1;
		
		//リーダーボードのIDセット.
		LEADERBOARD_ID = leaderboardId;
		FASGui.SetLeaderboardId(LEADERBOARD_ID);
	}

	//------------------------------
	//ゲーム開始時にタッチされた / touch when game start
	//------------------------------
	public static void TouchGamestart () {
		gamestartGUI.SetActive(false);
		isCanInput = true;
		timerCont.isCountdown = true;
		gamestartGUI.collider.enabled = false;
	}
	
	//------------------------------
	//ステージクリア時にタッチされた / touch when stage clear
	//------------------------------
	public static void TouchStageClear (bool isContinue) {
		//課金時に二重呼び出しされるので、gamestartGUIのオンオフで判定する.
		if (!gamestartGUI.activeSelf) {
			//課金による継続だったら風船回復
			if (isContinue) {
				player.balloonNum = settings.maxBalloonNum;
				player.ChangeBalloonColor();
			} else {
				//通常のクリアだったらステージを進める.
				nowStage ++;
			}

			//ステージ準備 / stage setup
			player.Init();
			stageCont.MakeStage();
			timerCont.Init();
			bgCont.Init();
			Camera.main.GetComponent<CameraController>().Init();
			
			//GUI表示 / display
			resultCont.TouchStageClearGUI();
			gamestartGUI.SetActive(true);
			SetBalloonText();
		}
	}

	//------------------------------
	//ゲームオーバーGUI表示 / visible GameOver GUI
	//------------------------------
	public static void GameOver (int phase) {
		if (phase == 1) {
			//phsse 1 : ゲームオーバーを表示して待つ / show "GameOver" and wait
			timerCont.isCountdown = false;
			gameoverGUI.SetActive(true);
			resultCont.GameOver();
		} else {
			//ネットワークが繋がっていなかったら画面タップをアクティブに→タップでタイトルへ.
			if (Application.internetReachability == NetworkReachability.NotReachable){
				gameoverGUI.collider.enabled = true;
			} else {
				//phsse 2 : コンティニュー準備.
				ExampleLocalStoreInfo.UpdateBalances();
				//風船足りる？.
				if (ExampleLocalStoreInfo.CurrencyBalance >= useBalloon) {
					//足りる時はコンティニュー表示.
					gameoverGUI.SetActive(false);
					continueGUI.SetActive(true);
					balloonCont.SetOrderForward();	//風船の数GUIを前面へ.
				} else {
					//足りない時はリーダーボード直接表示
					TouchContinueNo();
				}
			}
		}
	}

	//------------------------------
	//コンティニュー発動.
	//------------------------------
	public static void TouchContinueYes () {
		Debug.Log ("SOOMLA Start");
		continueGUI.SetActive(false);
		
		//風船減算処理.
		string itemName = "continue_item" + useBalloon;
		if (useBalloon < MAX_USE_BALLOON) {
			useBalloon += 2;
		}

		StoreInventory.BuyItem(itemName);

		//コンティニュー処理.
		TouchStageClear(true);
	}

	//------------------------------
	//コンディニューしない.
	//------------------------------
	public static void TouchContinueNo () {
		balloonCont.SetOrderBack();	//風船の数GUIを背面へ.

		//リーダーボード入力画面を表示して待つ / show LeaderBoard GUI and wait
		continueGUI.SetActive(false);
		leaderboardCont.gameObject.SetActive(true);

		//ゲームオーバーBGMの自動消去スイッチオン.
		player.soundGameover.autoDestract = true;
	}

	//------------------------------
	//風船の数リライト（IAPのイベントハンドラーから呼び出す用）.
	//------------------------------
	public static void SetBalloonText() {
		GameObject.Find("CurrentBalloon").GetComponent<CurrentBalloonController>().SetBalloonText();
	}

	//------------------------------
	//ゲームオーバー→タイトルへ / touch when GameOver
	//------------------------------
	public static void TouchGameover () {
		isCanInput = false;
		nowScore = 0;
		nowStage = 1;
		Application.LoadLevel("StartScene");
	}

	//------------------------------
	//タイムアップによるゲームオーバー / GameOver by time up
	//------------------------------
	public static void GameOverByTimeup () {
		isCanInput = false;
		player.GameOverByTimeup();
	}

	//------------------------------
	//タイムの増加 / Remain Time Recover
	//------------------------------
	public static void TimeRecover (float recover) {
		timerCont.remainTime += recover;
	}
}
