using UnityEngine;
using System.Collections;
using Soomla;
using Soomla.Example;
using Soomla.Store;
using System;
using System.Collections.Generic;
using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Gui;
using Fresvii.AppSteroid.Models;

/*
 * ゲーム管理クラス / Game Control Class
 * 
 * ゲーム全体を制御する.
 * control the entire game.
 * 
*/
public class GameController : MonoBehaviour {
	private static string returnSceneName = "StartScene";

	public string leaderboardId;
	public static string LEADERBOARD_ID;

	public static bool isCanInput = false;			//アニメーションなど、一時的な入力可否.
	public static bool isCanMasterInput = false;	//システムなど、絶対的な入力可否.
	public static int nowScore = 0;
	public static int nowStage = 1;
	public static bool isSoundOn = true;
	//public static int shopBalloon = 0;
	

	public static GameObject gamestartGUI;
	public static GameObject gameoverGUI;
	public static GameObject continueGUI;
	public static GameObject replayGUI;
	public static ResultController resultCont;
	public static PlayerController player;
	public static TimerController timerCont;
	public static FresviiLeaderBoardGUI leaderboardCont;
	public static CurrentBalloonController balloonCont;

	private static StageController stageCont;
	private static BackGroundController bgCont;
	private static GameSettings settings;

	//課金用.
	private static bool isIAPInit;
	private static ExampleEventHandler handler;
	private static int useBalloon;	//消費する風船.
	private const int MAX_USE_BALLOON = 9;	//消費する最大の風船数.
	private static Vector3 respawnPos;	//VSモードで復帰する時の位置.

	//iOSバージョン.
	public static bool isIOS8 = false;

	void Awake() {
		gamestartGUI = GameObject.Find("gamestartGUI");
		if (GameObject.Find("leaderboardGUI") != null) {
			leaderboardCont = GameObject.Find("leaderboardGUI").GetComponent<FresviiLeaderBoardGUI>();
		}
		gameoverGUI = GameObject.Find("gameoverGUI");
		continueGUI = GameObject.Find("continueGUI");
		if (continueGUI != null) {
			continueGUI.SetActive(false);
		}

		replayGUI = GameObject.Find("replayGUI");
		if (replayGUI != null) {
			replayGUI.SetActive(false);
		}

		if (GameObject.Find("Result") != null) {
			resultCont = GameObject.Find("Result").GetComponent<ResultController>();
		}
		if (GameObject.Find("Character") != null) {
			player = GameObject.Find("Character").GetComponent<PlayerController>();
		}
		stageCont = GameObject.Find("AreaObjects").GetComponent<StageController>();
		timerCont = GameObject.Find("Timer").GetComponent<TimerController>();
		balloonCont = GameObject.Find("CurrentBalloon").GetComponent<CurrentBalloonController>();
		bgCont = GameObject.Find("BackGround").GetComponent<BackGroundController>();
		settings = gameObject.GetComponent<GameSettings>();

		useBalloon = 1;
		nowScore = 0;
		nowStage = 1;
		
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
		isCanMasterInput = true;
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
		//録画中だったら録画終了.
		if (FASPlayVideo.IsRecording()) {
			FASPlayVideo.StopRecording();
		}

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
				//ボタン一覧を表示.
				TouchContinueNo();
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
		StoreInventory.BuyItem(itemName);
		//shopBalloon -= useBalloon;

		if (useBalloon < MAX_USE_BALLOON) {
			useBalloon += 2;
		}

		//コンティニュー処理.
		TouchStageClear(true);
	}

	//------------------------------
	//コンディニューしない（ボタン郡表示）.
	//------------------------------
	public static void TouchContinueNo () {
		//リプレイ画面を表示.
		leaderboardCont.gameObject.SetActive(true);
		//#FGC LeaderBoard Start――――――
		leaderboardCont.SendPoint(GameController.nowScore, "Chicken");
		replayGUI.SetActive(true);
	}

	//------------------------------
	//Save Meボタンが押された.
	//------------------------------
	public static void TouchSaveMe () {
		gameoverGUI.SetActive(false);
		replayGUI.SetActive(false);

		ExampleLocalStoreInfo.UpdateBalances();
		int shopBalloon = ExampleLocalStoreInfo.CurrencyBalance;
		//風船足りる？.
		if (shopBalloon >= useBalloon) {
			//足りる時はコンティニュー実行（1Pか対戦で分岐）.
			if (settings.isNetworkScene) {
				RecoverInVS();
			} else {
				TouchContinueYes();
			}
		} else {
			//足りない時は選択肢を出す.
			balloonCont.SetOrderForward();	//風船の数GUIを前面へ.
			continueGUI.SetActive(true);
		}
	}

	//------------------------------
	//Save Meボタンが押された後の選択肢.
	//　flag　true:YESが押された　false:NOが押された.
	//------------------------------
	public static void TouchSaveMeYesNo (bool flag) {
		if (flag) {
			//ショップに移動.
			ShopFlagController.isOpenShop = true;
		}

		//対戦時は接続解除.
		if (settings.isNetworkScene) {	
			if (Network.isServer) {
				Network.Disconnect();
			}
			if (FASMatchMaking.currentMatch.Status != Match.Statuses.Disposed) {
				FASMatchMaking.DisposeMatch (FASMatchMaking.currentMatch.Id, delegate(Match match, Error error) {});
			}
		}

		//タイトルへ.
		TouchGameover("StartScene");
	}

	//------------------------------
	//ネットワークモードにおけるコンティニュー発動判定.
	//------------------------------
	public static bool CheckRecoverInVS() {
		ExampleLocalStoreInfo.UpdateBalances();
		int shopBalloon = ExampleLocalStoreInfo.CurrencyBalance;
		//風船足りる？.
		return (shopBalloon >= useBalloon);
	}
	//------------------------------
	//ネットワークモードにおけるコンティニュー画像表示.
	//------------------------------
	public static void CheckRecoverInVS(Vector3 respawnPosition) {
		respawnPos = respawnPosition;
		ExampleLocalStoreInfo.UpdateBalances();
		int shopBalloon = ExampleLocalStoreInfo.CurrencyBalance;

		if (!gameoverGUI.activeInHierarchy) {
			replayGUI.SetActive(true);
		}
	}
	//------------------------------
	//ネットワークモードにおけるコンティニュー発動（ボタンから呼ばれる）.
	//------------------------------
	public static void RecoverInVS() {
		if (gameoverGUI.activeInHierarchy) {
			return;
		}

		Debug.Log ("SOOMLA Start");
		
		//風船減算処理.
		string itemName = "continue_item" + useBalloon;
		StoreInventory.BuyItem(itemName);
		//shopBalloon -= useBalloon;
		
		if (useBalloon < MAX_USE_BALLOON) {
			useBalloon += 2;
		}
		
		//無敵モード開始.
		player.transform.localPosition = respawnPos;
		player.netMarker.transform.localPosition = respawnPos;
		player.ArmorModeStart();
	}
	
	//------------------------------
	//リーダーボード画面へ.
	//------------------------------
	public static void TouchGotoLeaderboard () {
		//リーダーボード入力画面を表示して待つ / show LeaderBoard GUI and wait
		replayGUI.SetActive(false);
		leaderboardCont.gameObject.SetActive(true);
		Debug.Log(leaderboardCont);
		//#FGC LeaderBoard Start――――――
		leaderboardCont.ShowGUI("Chicken");
		
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
	//ゲームオーバー→タイトルorリプレイ / touch when GameOver
	//------------------------------
	public static void TouchGameover (String sceneName) {
		isCanInput = false;
		isCanMasterInput = false;
		nowScore = 0;
		nowStage = 1;
		player.soundGameover.Stop ();
		Application.LoadLevel(sceneName);
	}

	//------------------------------
	//タイムアップによるゲームオーバー / GameOver by time up
	//------------------------------
	public static void GameOverByTimeup () {
		isCanInput = false;
		isCanMasterInput = false;
		player.GameOverByTimeup();
	}

	//------------------------------
	//タイムの増加 / Remain Time Recover
	//------------------------------
	public static void TimeRecover (float recover) {
		timerCont.remainTime += recover;
	}

	//------------------------------
	//風船の数参照（ResultControllerから呼ぶ）.
	//------------------------------
	public static int GetNowBalloonNum () {
		return player.balloonNum;
	}
}
