using UnityEngine;
using System.Collections;

/*
 * 入力管理クラス / Input Control Class
 * 
 * ボタン入力を制御する.
 * control the button input.
 * 
*/
public class InputControllerVSRecovery : MonoBehaviour {
	public float positionX = 100.0f;

	public SpriteRenderer btn;
	public Sprite[] btnSprites;		//[0]=off [1]=on

	public SoundController soundStart;
	public SoundController soundButton;

	private Transform gamestartGUI;
	private Transform gameoverGUI;
	private Transform stageclearGUI;
	private PlayerController player;

	private int jumpBtnFingerID = -1;	//ジャンプボタンを押したタッチID / FingerID to press the jump button
	private int leftBtnFingerID = -1;
	private int rightBtnFingerID = -1;
	private bool isPusshing;	//押しっぱなし防止用フラグ / flag for press keep prevention

	// Use this for initialization
	void Start () {
		player = GameController.player;
		//GUI取得 / get GUI object
		gamestartGUI = GameController.gamestartGUI.transform;
		gameoverGUI = GameController.gameoverGUI.transform;
		gameoverGUI.gameObject.SetActive(false);
		if (GameController.leaderboardCont != null) {
			GameController.leaderboardCont.gameObject.SetActive(false);
		}

		if (ResultController.stageclearGUI != null) {
			stageclearGUI = ResultController.stageclearGUI.transform;
			stageclearGUI.gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//ネットワーク時対策.
		if (GameController.player == null) {
			return;
		}

		if (player == null) {
			player = GameController.player;
		}

		CheckInput();
	}

	//入力チェック / check input
	void CheckInput() {
		//モバイルではタッチで操作 / Operation in touch on mobile
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++) {
				
				if (touches[i].phase == TouchPhase.Ended || touches[i].phase == TouchPhase.Canceled) {
					if (touches[i].fingerId == leftBtnFingerID) {
						leftBtnFingerID = -1;
						if (GameController.isCanInput && GameController.isCanMasterInput) {
							player.Stop();
						}
					}
					if (touches[i].fingerId == rightBtnFingerID) {
						rightBtnFingerID = -1;
						if (GameController.isCanInput && GameController.isCanMasterInput) {
							player.Stop();
						}
					}
					if (touches[i].fingerId == jumpBtnFingerID) {
						jumpBtnFingerID = -1;
						player.JumpInput(false);
					}
					break;
				}

				bool isBegan = false;
				if (touches[i].phase == TouchPhase.Began) {
					isBegan = true;
				}

				RayFromInput( touches[i].position, touches[i].fingerId, isBegan);
			}

			//全ての指が離された / all button released
			if (touches.Length == 0) {
				isPusshing = false;
			}

		//それ以外ではクリックで操作 / Operation Click otherwise
		} else {
			if (Input.GetMouseButton(0)) {
				RayFromInput(Input.mousePosition, 0, true);
			} else {
				if (leftBtnFingerID != -1) {
					leftBtnFingerID = -1;
					if (GameController.isCanInput && GameController.isCanMasterInput) {
						player.Stop();
					}
				}
				if (rightBtnFingerID != -1) {
					rightBtnFingerID = -1;
					if (GameController.isCanInput && GameController.isCanMasterInput) {
						player.Stop();
					}
				}
				if (jumpBtnFingerID != -1) {
					player.JumpInput(false);
					jumpBtnFingerID = -1;
				}
				isPusshing = false;
			}
		}
	}

	//入力座標からボタンが押されているか調べる / examine whether the button is pressed from the input coordinate
	void RayFromInput(Vector2 point, int fingerID, bool isBegan) {
		RaycastHit hit = new RaycastHit();
		Ray ray = Camera.main.ScreenPointToRay( point );
		if ( Physics.Raycast( ray, out hit ) ) {
			//リーダーボードボタン.
			if(hit.transform == btn.transform && !isPusshing && isBegan) {
				//ディレイ付きで実行.
				StartCoroutine("DelayBtn");
			}
		}
	}

	//ボタン処理.
	private void PushBtn() {
		//ボタン画像切り替え無しver（＝押しているフラグと音だけ）.
		PushBtn (null, null);
	}
	private void PushBtn(SpriteRenderer rend, Sprite onSprite) {
		//ボタン画像切り替えありver.
		isPusshing = true;
		soundButton.Play();
		if (rend != null && onSprite != null) {
			rend.sprite = onSprite;
		}
	}

	//リーダーボードボタン：ディレイ.
	private IEnumerator DelayBtn() {
		PushBtn(btn, btnSprites[1]);
		yield return new WaitForSeconds(0.1f);
		GameController.RecoverInVS();
		btn.sprite = btnSprites[0];
		btn.transform.parent.gameObject.SetActive(false);
	}
}
