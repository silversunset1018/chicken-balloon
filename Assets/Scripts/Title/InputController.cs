using UnityEngine;
using System.Collections;
using Fresvii.AppSteroid;

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
	public SpriteRenderer btnSaveMe;
	public SpriteRenderer btnSaveMeYes;
	public SpriteRenderer btnSaveMeNo;
	public SpriteRenderer btnReplay;
	public SpriteRenderer btnLeaderboard;
	public SpriteRenderer btnHome;
	public GameObject btnVideo;
	public SpriteRenderer btnShare;

	public Sprite[] btnLeftSprites;		//[0]=off [1]=on
	public Sprite[] btnRightSprites;
	public Sprite[] btnUpSprites;
	
	public Sprite[] btnGameOverOKSprites;
	public Sprite[] btnGameOverSkipSprites;
	public Sprite[] btnSaveMeSprites;
	public Sprite[] btnSaveMeYesSprites;
	public Sprite[] btnSaveMeNoSprites;
	public Sprite[] btnReplaySprites;
	public Sprite[] btnLeaderboardSprites;
	public Sprite[] btnHomeSprites;
	public Sprite[] btnShareSprites;

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
		positionX *= Screen.width / 1136.0f;
		float marginOfLeftRight = btnRight.transform.position.x - btnLeft.transform.position.x;
        if (btnRight != null && btnLeft != null && btnUp != null)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector2(positionX, 0));
            btnLeft.transform.position = new Vector3(pos.x, btnLeft.transform.position.y, btnLeft.transform.position.z);
            btnRight.transform.position = new Vector3(pos.x + marginOfLeftRight, btnRight.transform.position.y, btnRight.transform.position.z);
            pos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - positionX, 0));

            btnUp.transform.position = new Vector3(pos.x, btnUp.transform.position.y, btnUp.transform.position.z);
        }

        if (btnVideo != null)
        {
			Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - positionX, 0));
            btnVideo.transform.position = new Vector3(pos.x - marginOfLeftRight, btnVideo.transform.position.y, btnVideo.transform.position.z);
        }
		
		//GUI取得 / get GUI object
		gamestartGUI = GameController.gamestartGUI.transform;
		gameoverGUI = GameController.gameoverGUI.transform;
		gameoverGUI.gameObject.SetActive(false);
		if (GameController.leaderboardCont != null) {
			//GameController.leaderboardCont.gameObject.SetActive(false);
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
    bool canInput = true;

	void CheckInput() {

        if (!canInput)
        {
            return;
        }

		//モバイルではタッチで操作 / Operation in touch on mobile
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++) {
				
				if (touches[i].phase == TouchPhase.Ended || touches[i].phase == TouchPhase.Canceled) {
					if (touches[i].fingerId == leftBtnFingerID) {
						btnLeft.sprite = btnLeftSprites[0];
						leftBtnFingerID = -1;
						if (GameController.isCanInput && GameController.isCanMasterInput) {
							player.Stop();
						}
					}
					if (touches[i].fingerId == rightBtnFingerID) {
						btnRight.sprite = btnRightSprites[0];
						rightBtnFingerID = -1;
						if (GameController.isCanInput && GameController.isCanMasterInput) {
							player.Stop();
						}
					}
					if (touches[i].fingerId == jumpBtnFingerID) {
						btnUp.sprite = btnUpSprites[0];
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
					btnLeft.sprite = btnLeftSprites[0];
					leftBtnFingerID = -1;
					if (GameController.isCanInput && GameController.isCanMasterInput) {
						player.Stop();
					}
				}
				if (rightBtnFingerID != -1) {
					btnRight.sprite = btnRightSprites[0];
					rightBtnFingerID = -1;
					if (GameController.isCanInput && GameController.isCanMasterInput) {
						player.Stop();
					}
				}
				if (jumpBtnFingerID != -1) {
					player.JumpInput(false);
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
			if (GameController.isCanInput && GameController.isCanMasterInput) {
				//左移動 / Move left
				if(hit.transform == btnLeft.transform) {
					if (fingerID != leftBtnFingerID) {
						btnLeft.sprite = btnLeftSprites[1];
						leftBtnFingerID = fingerID;
					}
					player.MoveInput(false);
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
					player.MoveInput(true);
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
					player.JumpInput(true);
					jumpBtnFingerID = fingerID;
					isPusshing = true;
				} else if (jumpBtnFingerID == fingerID) {
					player.JumpInput(false);
					jumpBtnFingerID = -1;
					btnUp.sprite = btnUpSprites[0];
				}
			}

			
			//SaveMeボタン.
			if(hit.transform == btnSaveMe.transform && !isPusshing && isBegan) {
				//ディレイ付きで実行.
				StartCoroutine("DelaySaveMeBtn");
			}
			//SaveMeボタン後のYes.
			if(hit.transform == btnSaveMeYes.transform && !isPusshing && isBegan) {
				//ディレイ付きで実行.
				StartCoroutine("DelaySaveMeYesBtn");
			}
			//SaveMeボタン後のNo.
			if(hit.transform == btnSaveMeNo.transform && !isPusshing && isBegan) {
				//ディレイ付きで実行.
				StartCoroutine("DelaySaveMeNoBtn");
			}
			//ホームボタン.
			if(hit.transform == btnHome.transform && !isPusshing && isBegan) {
				PushBtn(btnHome, btnHomeSprites[1]);
				GameController.TouchGameover("StartScene");
			}

			//ネットモードの時はここで終わり.
			if (player.gameObject.networkView != null) {
				return;
			}
			
			//ゲーム開始 / Game start
			if(hit.transform == gamestartGUI && !isPusshing && isBegan) {
				GameController.TouchGamestart();
				soundStart.Play();
			}
			//ステージクリア / StageClear
			if(hit.transform == stageclearGUI && !isPusshing && isBegan) {
				GameController.TouchStageClear(false);
				PushBtn();
			}

			//録画ボタン.
			if(hit.transform == btnVideo.transform && !isPusshing && isBegan && !gamestartGUI.gameObject.activeSelf) {
				PushBtn();
				btnVideo.SendMessage("PushButton");
			}
			//シェアボタン.
			if(hit.transform == btnShare.transform && !isPusshing && isBegan && FASPlayVideo.LatestVideoExists()) {
				//ディレイ付きで実行.
				StartCoroutine("DelayShareBtn");
			}

			//リトライボタン.
			if(hit.transform == btnReplay.transform && !isPusshing && isBegan) {
				PushBtn(btnReplay, btnReplaySprites[1]);
				GameController.TouchGameover("GameScene");
			}
			//リーダーボードボタン.
			if(hit.transform == btnLeaderboard.transform && !isPusshing && isBegan) {
				//ディレイ付きで実行.
				StartCoroutine("DelayLeaderboardBtn");
			}

			//ゲームオーバー時：OK / GameOver : OK
			if(hit.transform == btnGameOverOK.transform && !isPusshing && isBegan) {
				PushBtn(btnGameOverOK, btnGameOverOKSprites[1]);
				//GameController.leaderboardCont.PushOK();
			}
			//ゲームオーバー時：skip / GameOver : skip
			if(hit.transform == btnGameOverSkip.transform && !isPusshing && isBegan) {
				PushBtn(btnGameOverSkip, btnGameOverSkipSprites[1]);
				GameController.TouchContinueNo();
			}

			//ゲームオーバー時：全画面（オフライン時のみ動作）.
			if(hit.transform == btnGameOver.transform && !isPusshing && isBegan) {
				btnGameOver.collider.enabled = false;
				PushBtn();
				GameController.TouchContinueNo();
			}

		} else {
			if (jumpBtnFingerID == fingerID) {
				player.JumpInput(false);
				jumpBtnFingerID = -1;
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
	private IEnumerator DelayLeaderboardBtn() {
		PushBtn(btnLeaderboard, btnLeaderboardSprites[1]);
		yield return new WaitForSeconds(0.1f);
		GameController.TouchGotoLeaderboard();
		btnLeaderboard.sprite = btnLeaderboardSprites[0];
	}
	
	//ビデオアップロード：ディレイ.
	private IEnumerator DelayShareBtn() {
        canInput = false;
		PushBtn(btnShare, btnShareSprites[1]);
		yield return new WaitForSeconds(0.1f);
		FASPlayVideo.ShowLatestVideoSharingGUI("StartScene", () =>
        {
            canInput = true;
        });
		btnShare.sprite = btnShareSprites[0];
	}
	
	//SaveMe：ディレイ.
	private IEnumerator DelaySaveMeBtn() {
		PushBtn(btnSaveMe, btnSaveMeSprites[1]);
		yield return new WaitForSeconds(0.1f);
		GameController.TouchSaveMe();
		btnSaveMe.sprite = btnSaveMeSprites[0];
	}
	//SaveMeYes：ディレイ.
	private IEnumerator DelaySaveMeYesBtn() {
		PushBtn(btnSaveMeYes, btnSaveMeYesSprites[1]);
		yield return new WaitForSeconds(0.1f);
		GameController.TouchSaveMeYesNo(true);
		btnSaveMeYes.sprite = btnSaveMeYesSprites[0];
	}
	//SaveMeNo：ディレイ.
	private IEnumerator DelaySaveMeNoBtn() {
		PushBtn(btnSaveMeNo, btnSaveMeNoSprites[1]);
		yield return new WaitForSeconds(0.1f);
		GameController.TouchSaveMeYesNo(false);
		btnSaveMeNo.sprite = btnSaveMeNoSprites[0];
	}
}
