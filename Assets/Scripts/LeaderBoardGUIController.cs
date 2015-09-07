using UnityEngine;
using System.Collections;

/*
 * リーダーボード用ユーザー名入力GUI / User name input GUI for LeaderBoard
 * 
*/
public class LeaderBoardGUIController : MonoBehaviour {
	public GUISkin mySkin;

	private const string USER_NAME_SAVE_KEY = "UserName";
	private const string DEFAULT_USER_NAME = "Chicken";

	private float userNameAreaWidth = 630.0f;
	private float userNameAreaHeight = 70.0f;
	private float contentOffsetY = -23.0f;

	private string userName = "";
	private Vector2 scaleFactor;
	private Vector2 centerOfWindow;
	private GUISkin useSkin;

	private FresviiLeaderBoardGUI leaderBoardComp;
	private bool isShow = false;

	// Use this for initialization
	void Start () {
		leaderBoardComp = gameObject.GetComponent<FresviiLeaderBoardGUI>();

		//サインアップ済みなら入力は無し / not input user name when signup already
		if (leaderBoardComp.GetAlreadySignup()) {
			//子オブジェクトを非表示にする / chidren objects set invisible
			foreach (Transform item in transform) {
				item.gameObject.SetActive(false);
			}
			
			//#FGC LeaderBoard Start――――――
			leaderBoardComp.SendPoint(GameController.nowScore, "");
			//#FGC LeaderBoard End――――――
			
			GameController.isCanInput = false;
			GameController.nowScore = 0;
			GameController.nowStage = 1;
		} else {
			isShow = true;

			//ウィンドウの拡大率と中心座標を取得する / get window scale factor and center position
			scaleFactor.x = Screen.width / 1136.0f;
			scaleFactor.y = Screen.height / 640.0f;
			centerOfWindow.x = Screen.width / 2.0f;
			centerOfWindow.y = Screen.height / 2.0f;

			userNameAreaWidth *= scaleFactor.x;
			userNameAreaHeight *= scaleFactor.y;

			useSkin = Instantiate(mySkin) as GUISkin;
			float fontSize = (float)mySkin.textField.fontSize;
			fontSize *= scaleFactor.x;
			useSkin.textField.fontSize = (int)fontSize;
		}
	}

	void OnGUI() {
		if (isShow) {
			GUI.skin = useSkin;
			userName = GUI.TextField(new Rect(centerOfWindow.x - userNameAreaWidth / 2.0f,
		                                  centerOfWindow.y - userNameAreaHeight / 2.0f + contentOffsetY * scaleFactor.y,
		                                  userNameAreaWidth, userNameAreaHeight), userName, 9);
		}
	}

	//------------------------------
	//OKが押された / push OK
	//------------------------------
	public void PushOK () {
		if (userName == "") {
			userName = DEFAULT_USER_NAME;
		}
		//#FGC LeaderBoard Start――――――
		leaderBoardComp.SendPoint(GameController.nowScore, userName);
		//#FGC LeaderBoard End――――――

		GameController.isCanInput = false;
		GameController.nowScore = 0;
		GameController.nowStage = 1;
	}
}
