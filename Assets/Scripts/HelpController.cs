using UnityEngine;
using System.Collections;

/*
 * ヘルプ管理クラス / Help Control Class
 * 
 * ヘルプ画像の大きさを制御する.
 * control size of help image.
 * 
*/
public class HelpController : MonoBehaviour {

	private const float pixelsToUnits = 100.0f;
	private const float imgSize = 1024.0f;

	// Use this for initialization
	void Start () {
		//カメラの描画をピクセルパーフェクトにする / set camera view is pexel perfect
		transform.parent.camera.orthographicSize = Screen.height / pixelsToUnits / 2.0f;
		//画面に合わせて大きさ調整 / fix image size for screen size
		transform.localScale *= Screen.width / imgSize + 1 / pixelsToUnits;
	}
}
