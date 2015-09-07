using UnityEngine;
using System.Collections;

/*
 * 入力管理クラス / Input Control Class
 * 
 * ボタン入力を制御する.
 * control the button input.
 * 
*/
public class InputController : MonoBehaviour {
	public float positionX = 100.0f;

	public SpriteRenderer btnLeft;
	public SpriteRenderer btnRight;
	public SpriteRenderer btnUp;
	public SpriteRenderer btnGameOver;
	public SpriteRenderer btnGameOverOK;
	public SpriteRenderer btnGameOverSkip;
	public SpriteRenderer btnContinueYes;
	public SpriteRenderer btnContinueNo;

	public Sprite[] btnLeftSprites;		//[0]=off [1]=on
	public Sprite[] btnRightSprites;
	public Sprite[] btnUpSprites;
	
	public Sprite[] btnGameOverOKSprites;
	public Sprite[] btnGameOverSkipSprites;
	public Sprite[] btnContinueYesSprites;
	public Sprite[] btnContinueNoSprites;

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

		//画面解像度に合わせて位置調整する / Located scales to fit the screen resolution
		float marginOfLeftRight = btnRight.transform.position.x - btnLeft.transform.position.x;
		positionX *= Screen.width / 1136.0f;
		Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector2(positionX,0));
		btnLeft.transform.position = new Vector3(pos.x, btnLeft.transform.position.y, btnLeft.transform.position.z);
		btnRight.transform.position = new Vector3(pos.x + marginOfLeftRight, btnRight.transform.position.y, btnRight.transform.position.z);
		pos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - positionX,0));
		btnUp.transform.position = new Vector3(pos.x, btnUp.transform.position.y, btnUp.transform.position.z);

		//GUI取得 / get GUI object
		gamestartGUI = GameController.gamestartGUI.transform;
		gameoverGUI = GameController.gameoverGUI.transform;
		gameoverGUI.gameObject.SetActive(false);
		GameController.leaderboardCont.gameObject.SetActive(false);
		stageclearGUI = ResultController.stageclearGUI.transform;
		stageclearGUI.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
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
						btnLeft.sprite = btnLeftSprites[0];
						leftBtnFingerID = -1;
						if (GameController.isCanInput) {
							player.Stop();
						}
					}
					if (touches[i].fingerId == rightBtnFingerID) {
						btnRight.sprite = btnRightSprites[0];
						rightBtnFingerID = -1;
						if (GameController.isCanInput) {
							player.Stop();
						}
					}
					if (touches[i].fingerId == jumpBtnFingerID) {
						btnUp.sprite = btnUpSprites[0];
						jumpBtnFingerID = -1;
						player.Jump(false);
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
					btnLeft.sprite = btnLeftSprites[0];
					leftBtnFingerID = -1;
					if (GameController.isCanInput) {
						player.Stop();
					}
				}
				if (rightBtnFingerID != -1) {
					btnRight.sprite = btnRightSprites[0];
					rightBtnFingerID = -1;
					if (GameController.isCanInput) {
						player.Stop();
					}
				}
				if (jumpBtnFingerID != -1) {
					player.Jump(false);
					btnUp.sprite = btnUpSprites[0];
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
			if (GameController.isCanInput) {
				//左移動 / Move left
				if(hit.transform == btnLeft.transform) {
					if (fingerID != leftBtnFingerID) {
						btnLeft.sprite = btnLeftSprites[1];
						leftBtnFingerID = fingerID;
					}
					player.Move(false);
					isPusshing = true;
				} else if (fingerID == leftBtnFingerID) {
					btnLeft.sprite = btnLeftSprites[0];
					leftBtnFingerID = -1;
					player.Stop();
				}
				
				//右移動 / Move right
				if(hit.transform == btnRight.transform) {
					if (fingerID != rightBtnFingerID) {
						btnRight.sprite = btnRightSprites[1];
						rightBtnFingerID = fingerID;
					}
					player.Move(true);
					isPusshing = true;
				} else if (fingerID == rightBtnFingerID) {
					btnRight.sprite = btnRightSprites[0];
					rightBtnFingerID = -1;
					player.Stop();
				}

				//ジャンプ / Jump
				if(hit.transform == btnUp.transform) {
					if (fingerID != jumpBtnFingerID) {
						btnUp.sprite = btnUpSprites[1];
					}
					player.Jump(true);
					jumpBtnFingerID = fingerID;
					isPusshing = true;
				} else if (jumpBtnFingerID == fingerID) {
					player.Jump(false);
					jumpBtnFingerID = -1;
					btnUp.sprite = btnUpSprites[0];
				}
			}
			
			//ゲーム開始 / Game start
			if(hit.transform == gamestartGUI && !isPusshing && isBegan) {
				GameController.TouchGamestart();
				isPusshing = true;
				soundStart.Play();
			}
			//ステージクリア / StageClear
			if(hit.transform == stageclearGUI && !isPusshing && isBegan) {
				GameController.TouchStageClear(false);
				isPusshing = true;
				soundButton.Play();
			}
			
			//コンティニューの時：yes
			if(hit.transform == btnContinueYes.transform && !isPusshing && isBegan) {
				isPusshing = true;
				soundButton.Play();
				btnContinueYes.sprite = btnContinueYesSprites[1];
				GameController.TouchContinueYes();
			}
			//コンティニューの時：no
			if(hit.transform == btnContinueNo.transform && !isPusshing && isBegan) {
				isPusshing = true;
				soundButton.Play();
				btnContinueNo.sprite = btnContinueNoSprites[1];
				GameController.TouchContinueNo();
			}

			//ゲームオーバー時：OK / GameOver : OK
			if(hit.transform == btnGameOverOK.transform && !isPusshing && isBegan) {
				isPusshing = true;
				soundButton.Play();
				btnGameOverOK.sprite = btnGameOverOKSprites[1];
				GameController.leaderboardCont.PushOK();
			}
			//ゲームオーバー時：skip / GameOver : skip
			if(hit.transform == btnGameOverSkip.transform && !isPusshing && isBegan) {
				isPusshing = true;
				soundButton.Play();
				btnGameOverSkip.sprite = btnGameOverSkipSprites[1];
				GameController.TouchGameover();
			}

			//ゲームオーバー時：全画面（オフライン時のみ動作）.
			if(hit.transform == btnGameOver.transform && !isPusshing && isBegan) {
				btnGameOver.collider.enabled = false;
				isPusshing = true;
				soundButton.Play();
				GameController.TouchGameover();
			}

		} else {
			if (jumpBtnFingerID == fingerID) {
				player.Jump(false);
				jumpBtnFingerID = -1;
			}
		}
	}
}
