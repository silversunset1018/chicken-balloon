using UnityEngine;
using System.Collections;

/*
 * フォーラムログイン用ユーザー名入力GUI / User name input GUI for Forum Login
 * 
*/
public class SignUpGUIController : MonoBehaviour {
	public GUISkin mySkin;

	private float userNameAreaWidth = 700.0f;
	private float userNameAreaHeight = 70.0f;
	private float contentOffsetY = 133.0f;

	private string userName = "";
	private Vector2 scaleFactor;
	private Vector2 centerOfWindow;
	private GUISkin useSkin;

	// Use this for initialization
	void Start () {
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

	void OnGUI() {
		GUI.skin = useSkin;
		userName = GUI.TextField(new Rect(centerOfWindow.x - userNameAreaWidth / 2.0f,
		                                  centerOfWindow.y - userNameAreaHeight / 2.0f + contentOffsetY * scaleFactor.y,
		                                  userNameAreaWidth, userNameAreaHeight), userName);
	}

	//Getter
	public string GetUserName() {
		return userName;
	}
}
