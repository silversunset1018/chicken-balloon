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
public class QuitController : MonoBehaviour {	
	public SpriteRenderer btnQuit;
	public SpriteRenderer btnQuitYes;
	public SpriteRenderer btnQuitNo;

	public Sprite[] btnQuitSprites;
	public Sprite[] btnQuitYesSprites;
	public Sprite[] btnQuitNoSprites;

	public SoundController soundButton;

	public GameObject quitParent;

	private bool isCanInput = true;
	private bool isPusshing;	//押しっぱなし防止用フラグ / flag for press keep prevention
	private bool isShowOnGUI;	//UnityGUI発動フラグ.
	private bool isCoroutine;	//コルーチン起動済みフラグ.

	//------------------------------
	// Update is called once per frame
	//------------------------------
	void Update () {
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
				isPusshing = false;
			}
			
		//それ以外ではクリックで操作 / Operation Click otherwise
		} else {
			if (Input.GetMouseButton(0)) {
				RayFromInput(Input.mousePosition, 0);
			} else {
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

			//終了 / Quit
			if(hit.transform == btnQuit.transform && !isCoroutine) {
				/* 現在はAndroid版のみバックキーで終了する（Update内に記載）.
				btnQuit.sprite = btnQuitSprites[1];
				soundButton.Play();

				StartCoroutine("ShowQuitDelay", false);
				isCoroutine = true;

				btnQuitYes.sprite = btnQuitYesSprites[0];
				btnQuitNo.sprite = btnQuitNoSprites[0];
				*/
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
				btnQuit.sprite = btnQuitSprites[0];
			}
		}
	}

	//------------------------------
	//終了の画像表示.
	//（ボタンの変化を見せるため、一瞬遅れて実行する。）.
	//------------------------------
	private IEnumerator ShowQuitDelay (bool isNo) {
		yield return new WaitForSeconds (0.1f);
		//使用するフラグ（Noの時は逆転する）.
		bool isActive = false;
		if (isNo) isActive = !isActive;

		quitParent.SetActive(!isActive);
		isCoroutine = false;
	}
	private IEnumerator DoQuitDelay () {
		yield return new WaitForSeconds (0.1f);
		Application.Quit();
	}
}
