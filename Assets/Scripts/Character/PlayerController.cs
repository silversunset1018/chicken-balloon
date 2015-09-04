using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * プレイヤーキャラクター管理クラス / Player Character Control Class
 * 
 * プレイヤーキャラを制御する.
 * control the player character.
 * 
*/
public class PlayerController : MonoBehaviour {
	public int balloonNum;
	public Sprite balloonSprite_green;
	public Sprite balloonSprite_yellow;
	public Sprite balloonSprite_red;

	private GameSettings settings;
	public BackGroundController bgCont;

	public SoundController soundBGM;
	public SoundController soundGoal;
	public SoundController soundWing;
	public SoundController[] soundDamage;
	public SoundController soundDead;
	public SoundController soundGameover;

	public Vector3 startPosition;
	public float stageClearTime = 3.0f;
	
	//ネットワークモード時のプレイヤー番号（シーン来訪時自動で割り振り）.
	public int netPlayerID;
	//ネットワークモード時の追随オブジェクト（手動で設定）.
	public Transform netMarker;
	public Transform netMarkerChild;
	//ネットワーク時の無敵モード.
	public bool isArmorMode;

	private CameraController cameraCont;
	private SpriteRenderer balloonSprite;
	private Animator animator;
	private Vector3 initialPosition;
	private Vector3 lateUpdatePos;
	private Vector3 lateUpdateMarkerPos;

	private bool[] isCollisionFrame;	//0:left  1:top  2:bottom
	private float jumpTimer;
	private int damageFrame = -1;
	private Vector3 hitNormal;
	private bool isRightBeforeFrame = true;
	private bool isStopping;
	private bool isLanding;

	private float stageClearTimer = -1.0f;
	private List<Transform> imgChildren;

	private float horizontalVelocity;
	private float verticalVelocity;

	private int networkFlag;	//0：オフライン　1：オンラインかつisMine　2：オンラインかつ!isMine.
	private VSResult vsResultComp;

	private bool isMoveInput;
	private bool isMoveDir;
	private bool isJumpInput;

	public List<SpriteRenderer> childSpriteRenderers;
	private float armorModeTimer;
	private float hitCoolTimer;		//風船が割れた後の無敵時間用タイマー.

	//------------------------------
	// Use this for initialization
	//------------------------------
	void Start () {
		animator = gameObject.GetComponent<Animator>();
		isCollisionFrame = new bool[4];
		settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
		balloonNum = settings.maxBalloonNum;
		balloonSprite = transform.Find("balloon").GetComponent<SpriteRenderer>();
		balloonSprite.sprite = balloonSprite_green;
		initialPosition = transform.position;

		imgChildren = new List<Transform>();
		//childSpriteRenderers = new List<SpriteRenderer>();
		foreach (Transform child in transform) {
			if (!child.gameObject.activeSelf) {
				imgChildren.Add(child);
			}
			if (child.GetComponent<SpriteRenderer>() != null) {
				//childSpriteRenderers.Add(child.GetComponent<SpriteRenderer>());
			}
		}

		//サウンド取得（プレハブ用）.
		if (soundBGM == null) {
			soundBGM = GameObject.Find("BGM_intro").GetComponent<SoundController>();
			soundGameover = GameObject.Find("BGM_loop").GetComponent<SoundController>();	//変数代用.
			soundGoal = GameObject.Find("Start").GetComponent<SoundController>();			//変数代用.
			soundWing = GameObject.Find("Wing").GetComponent<SoundController>();
			soundDamage[0] = GameObject.Find("Damage4").GetComponent<SoundController>();
			soundDamage[1] = GameObject.Find("Damage3").GetComponent<SoundController>();
			soundDamage[2] = GameObject.Find("Damage2").GetComponent<SoundController>();
			soundDamage[3] = GameObject.Find("Damage1").GetComponent<SoundController>();
			soundDead = GameObject.Find("Dead").GetComponent<SoundController>();
		}

		cameraCont = Camera.main.GetComponent<CameraController>();
	
		//ネットワークフラグ設定.
		if (networkView != null) {
			vsResultComp = GameObject.Find("Result_network").GetComponent<VSResult>();
			if (networkView.isMine) {
				networkFlag = 1;
				vsResultComp.player = this;
				cameraCont.characterObj = transform;
				//Playerを示すフキダシを表示.
				foreach (Transform child in imgChildren) {
					child.gameObject.SetActive(true);
				}
			} else {
				networkFlag = 2;
				gameObject.tag = "Enemy";
				gameObject.collider.isTrigger = true;
			}
			StartCoroutine("NetworkStart");
		} else {
			stageClearTimer = 0.0f;
			if (cameraCont.characterObj == null) {
				cameraCont.characterObj = transform;
			}
		}

		//iPhoneなら位置補正.
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			//settings.networkCameraPosition.x += 0.4f;
			//settings.networkCameraPosition.y -= 0.45f;
		}
	}

	//------------------------------
	//ネットワーク時の開始処理.
	//------------------------------
	private IEnumerator NetworkStart() {
		GameController.isCanInput = false;
		GameController.isCanMasterInput = false;
		yield return new WaitForSeconds (1.0f);
		//BGM開始.
		GameController.player.soundBGM.Play();
		GameController.player.soundGameover.Play();

		//キャラ移動開始.
		stageClearTimer = 0.0f;

		yield return new WaitForSeconds (6.0f);

		//向き判定.
		if (transform.parent.name == "CharaParent1(Clone)" || transform.parent.name == "CharaParent3(Clone)") {
			isRightBeforeFrame = false;
		}
		
		//Playerを示すフキダシを非表示.
		foreach (Transform child in imgChildren) {
			child.gameObject.SetActive(false);
		}

		soundGoal.Play();
		GameController.gamestartGUI.SetActive(false);
		GameController.isCanInput = true;
		GameController.isCanMasterInput = true;
		if (NetworkController.BLACK_CHICKEN_ANIMATION != null) {
			NetworkController.BLACK_CHICKEN_ANIMATION.Play ();
		}
		if (NetworkController.CUCTUS_ANIMATION != null) {
			NetworkController.CUCTUS_ANIMATION.Play ();
		}
	}

	//Getter
	public Animator GetAnimator() {
		return animator;
	}
	//------------------------------
	//風船の数シンクロ用RPC
	//------------------------------
	[RPC]
	void NetSetBalloonNum (int num, float _hitCoolTimer) {
		balloonNum = num;
		hitCoolTimer = _hitCoolTimer;
	}
	//------------------------------
	//アニメーション用RPC
	//------------------------------
	[RPC]
	void NetAnimeSetBool (string paramName, int flag) {
		animator.SetBool(paramName, (flag == 1));
	}
	[RPC]
	void NetAnimeSetInt (string paramName, int num) {
		animator.SetInteger(paramName, num);
	}
	[RPC]
	void NetAnimeSetFloat (string paramName, float num) {
		animator.SetFloat(paramName, num);
	}
	[RPC]
	void NetAnimeSetTrigger (string paramName) {
		animator.SetTrigger(paramName);
	}

	//------------------------------
	//初期化（ステージリスタート用）/ Initialization for stage restart
	//------------------------------
	public void Init() {
		transform.position = initialPosition;
		Quaternion rot = transform.rotation;
		Vector3 rotChild = Vector3.zero;
		if (!isRightBeforeFrame) {
			rot.y = 0.0f;
			rotChild = Vector3.up * 180.0f;
			horizontalVelocity = 0.0f;
		}
		transform.rotation = rot;
		foreach (Transform child in imgChildren) {
			child.Rotate(rotChild, Space.World);
		}
		isRightBeforeFrame = true;

		cameraCont.Tracking();
		damageFrame = -1;
		stageClearTimer = 0.0f;
		GameController.gamestartGUI.collider.enabled = false;
		horizontalVelocity = 0.0f;
		verticalVelocity = 0.0f;
		isLanding = false;
		animator.SetBool("isFall", false);
		soundGoal.Stop();
		soundGameover.Stop();
		soundBGM.Stop();	//フェード中でも停止させる / Stop bgm during fadeout
		soundBGM.Play();
	}

	//------------------------------
	//FixedUpdate
	//------------------------------
	void FixedUpdate () {
		if (GameController.isCanInput && GameController.isCanMasterInput) {
			Fall();
		}
		if (isStopping) {
			Stop();
		}
		
		//ダメージ演出終了チェック（エラー防止の為、数フレーム分の間を空ける）.
		//Damage end checking (For error prevention, wait for a few frames)
		if (damageFrame > 1) {
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.chicken_normal") || 
			    animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.chicken_no_balloon2")) {
				DamageEnd();
			} else {
				DamageMove();
			}
		} else if (damageFrame > -1) {
			damageFrame ++;
		}
		
		//ステージクリア時は右へ移動させる / move to the right stage when Clearing
		if (stageClearTimer > -1.0f && stageClearTimer < stageClearTime) {
			if (networkFlag == 1) {
				netMarker.transform.Translate(settings.charaMovePower * Time.fixedDeltaTime, 0.0f, 0.0f);
				networkView.RPC("NetAnimeSetFloat", RPCMode.OthersBuffered, "nowMove", 1.0f);
			}
			if (networkFlag != 2) {
				transform.Translate(settings.charaMovePower * Time.fixedDeltaTime, 0.0f, 0.0f);
				animator.SetFloat("nowMove", 1.0f);
			}
			stageClearTimer += Time.fixedDeltaTime;
		} else if (stageClearTimer > stageClearTime) {
			//移動完了後 / move complete
			stageClearTimer = -1.0f;
			if (networkFlag == 1) {
				networkView.RPC("NetAnimeSetFloat", RPCMode.OthersBuffered, "nowMove", 0.0f);
			}
			if (networkFlag != 2) {
				transform.localPosition = startPosition;
				if(networkFlag == 1) netMarker.localPosition = startPosition;
				animator.SetFloat("nowMove", 0.0f);
			}
			if (networkFlag == 0) {
				GameController.gamestartGUI.collider.enabled = true;
			}
		}
		
		//ネットワーク時：位置トレース.
		PositionTraceInVS();

		//移動.
		Move();
		Jump();

		//常にZ値は固定する / fixed Z position
		lateUpdatePos = transform.position;
		lateUpdatePos.z = 0.0f;
		transform.position = lateUpdatePos;

		//風船が割れた際の無敵時間タイマー更新.
		if (hitCoolTimer > 0.0f) {
			hitCoolTimer -= Time.fixedDeltaTime;
		}
	}

	//------------------------------
	//ネットワーク時の位置トレース.
	//------------------------------
	private void PositionTraceInVS() {
		if (networkFlag == 2) {
			lateUpdateMarkerPos = netMarker.transform.position;
			lateUpdateMarkerPos.z = 0.0f;
			//遠い時はワープ、近い時は補間移動（距離判定はざっくり計算）.
			float distanceX = lateUpdateMarkerPos.x - transform.position.x;
			float distanceY = lateUpdateMarkerPos.y - transform.position.y;
			if (Mathf.Abs(distanceX) > 1.0f || Mathf.Abs(distanceY) > 1.0f) {
				transform.position = lateUpdateMarkerPos;
			} else {
				transform.position = Vector3.Lerp(transform.position, lateUpdateMarkerPos, 0.8f);
			}
			
			transform.rotation = netMarker.transform.rotation;
			foreach (Transform child in imgChildren) {
				child.rotation = netMarkerChild.rotation;
			}
		}
	}



	//------------------------------
	//左右移動（入力受取） / Move horizontal
	//dir=true:right dir=false:left
	//------------------------------
	public void MoveInput (bool dir) {
		isMoveInput = true;
		isMoveDir = dir;
	}
	//左右移動（実装）：FixedUpdateから呼ぶ.
	private void Move () {
		if (isMoveInput) {
			//減速中、同じ方向に入力があったら減速中止 / when be slowly, if there is same direction input, slow down is stop.
			if (isStopping && isRightBeforeFrame == isMoveDir) {
				isStopping = false;
			}
			
			float moveX = settings.charaMovePower * Time.fixedDeltaTime * 0.06f;
			
			//横方向の速度を増加 / add to horizontalVelocity
			if (horizontalVelocity < settings.charaMovePower * Time.fixedDeltaTime * 0.8f) {
				horizontalVelocity += moveX;
			}
			//枠に当たっている時：オフライン時はそこで止まる、オンライン時はループする.
			if (!isMoveDir && isCollisionFrame[0]) {
				//左.
				horizontalVelocity = 0.0f;
				if (networkFlag == 1 && GameController.isCanMasterInput) {
					cameraCont.SetX(settings.networkCameraPosition.y);
					cameraCont.LookFrame(AreaFrameType.right, true);
					transform.position = new Vector3(settings.networkStageEdge.y, transform.position.y, 0.0f);
					netMarker.position = new Vector3(settings.networkStageEdge.y, transform.position.y, 0.0f);
					isCollisionFrame[0] = false;
				}
			}
			if (isMoveDir && isCollisionFrame[3]) {
				//右.
				horizontalVelocity = 0.0f;
				if (networkFlag == 1 && GameController.isCanMasterInput) {
					cameraCont.SetX(settings.networkCameraPosition.x);
					cameraCont.LookFrame(AreaFrameType.left, true);
					transform.position = new Vector3(settings.networkStageEdge.x, transform.position.y, 0.0f);
					netMarker.position = new Vector3(settings.networkStageEdge.x, transform.position.y, 0.0f);
					isCollisionFrame[3] = false;
				}
			}
			
			//移動する / Moving
			if (networkFlag == 1) {
				netMarker.Translate(horizontalVelocity, 0.0f, 0.0f);
				if (animator.GetFloat("nowMove") != 1.0f) {
					networkView.RPC("NetAnimeSetFloat", RPCMode.OthersBuffered, "nowMove", 1.0f);
				}
			}
			if (networkFlag != 2){
				transform.Translate(horizontalVelocity, 0.0f, 0.0f);
				if (cameraCont.Tracking()) {
					float bgSpeed = horizontalVelocity / settings.charaMovePower * 50.0f;
					Mathf.Clamp01(bgSpeed);
					if (bgCont != null) {
						bgCont.Move(isMoveDir, bgSpeed);
					}
				}
				if (animator.GetFloat("nowMove") != 1.0f) {
					animator.SetFloat("nowMove", 1.0f);
				}
			}
			
			//画像の方向を変える / change direction in image.
			//このオブジェクトを反転させた後、アイテム取得時のGUIだけ更に反転させる / After reverse the this object, to reverse further only GUI of item acquisition.
			Quaternion rot = transform.rotation;
			Vector3 rotChild = Vector3.zero;
			if (isMoveDir && !isRightBeforeFrame) {
				rot.y = 0.0f;
				rotChild = Vector3.up * 180.0f;
				horizontalVelocity = 0.0f;
			} else if (!isMoveDir && isRightBeforeFrame) {
				rot.y = 180.0f;
				rotChild = Vector3.up * -180.0f;
				horizontalVelocity = 0.0f;
			}
			
			//回転する.
			if (networkFlag == 1) {
				netMarker.rotation = rot;
				netMarkerChild.Rotate(rotChild);
			}
			if (networkFlag != 2){
				transform.rotation = rot;
				foreach (Transform child in imgChildren) {
					child.Rotate(rotChild, Space.World);
				}
			}
			
			isRightBeforeFrame = isMoveDir;
			isMoveInput = false;
		}
	}

	//------------------------------
	// 左右移動停止 / Stop moving horizontal
	//------------------------------
	public void Stop() {
		if (horizontalVelocity > 0.0f) {
			horizontalVelocity -= settings.charaMovePower * Time.fixedDeltaTime * 0.06f;
			isStopping = true;
		} else {
			isStopping = false;
		}

		if (networkFlag == 1) {
			netMarker.Translate(horizontalVelocity, 0.0f, 0.0f);
			if (animator.GetFloat("nowMove") != 0.0f) {
				networkView.RPC("NetAnimeSetFloat", RPCMode.OthersBuffered, "nowMove", 0.0f);
			}
		}
		if (networkFlag != 2){
			transform.Translate(horizontalVelocity, 0.0f, 0.0f);
			if (cameraCont.Tracking()) {
				float bgSpeed = horizontalVelocity / settings.charaMovePower * 50.0f;
				Mathf.Clamp01(bgSpeed);
				if (bgCont != null) {
					bgCont.Move(isRightBeforeFrame, bgSpeed);
				}
			}
			if (animator.GetFloat("nowMove") != 0.0f) {
				animator.SetFloat("nowMove", 0.0f);
			}
		}
	}

	//------------------------------
	//ジャンプ / Jump up
	//push=true:pussing push=false:release
	//------------------------------
	public void JumpInput (bool push) {
		isJumpInput = push;
	}
	//ジャンプ（実装）：FixedUpdateから呼ぶ.
	private void Jump () {
		if (isJumpInput) {
			if (jumpTimer < settings.charaJumpTime) {
				float downPower = settings.charaJumpPower * Time.fixedDeltaTime;
				//上の枠に当たっている時はジャンプしない / Does not jump when you are hitting the frame of the top
				if (isCollisionFrame[1]) {
					downPower = 0.0f;
				}
				
				//風船が0の時はジャンプしない / Does not jump when balloon 0
				if (balloonNum == 0) {
					downPower = 0.0f;
				}
				
				if (networkFlag == 1) {
					netMarker.Translate(0.0f, downPower, 0.0f);
					if (!animator.GetBool("isJump")) {
						networkView.RPC("NetAnimeSetBool", RPCMode.OthersBuffered, "isJump", 1);
					}
				}
				if (networkFlag != 2) {
					transform.Translate(0.0f, downPower, 0.0f);
					cameraCont.Tracking();
					if (!animator.GetBool("isJump")) {
						animator.SetBool("isJump", true);
					}
				}
				jumpTimer += Time.fixedDeltaTime;
				soundWing.Play();
			} else {
				if (networkFlag == 1) {
					if (animator.GetBool("isJump")) {
						networkView.RPC("NetAnimeSetBool", RPCMode.OthersBuffered, "isJump", 0);
					}
				}
				if (networkFlag != 2){
					if (animator.GetBool("isJump")) {
						animator.SetBool("isJump", false);
					}
				}
			}
			isJumpInput = false;
		} else if (jumpTimer != 0.0f) {
			jumpTimer = 0.0f;
			if (networkFlag == 1) {
				if (animator.GetBool("isJump")) {
					networkView.RPC("NetAnimeSetBool", RPCMode.OthersBuffered, "isJump", 0);
				}
			}
			if (networkFlag != 2){
				if (animator.GetBool("isJump")) {
					animator.SetBool("isJump", false);
				}
			}
			
			//縦方向の速度を消す / reset to verticalVelocity
			verticalVelocity = 0.0f;
		}
	}
	

	//------------------------------
	//落下 / Fall down
	//------------------------------
	private void Fall() {
		float downPower = settings.gravity * Time.fixedDeltaTime;
		float maxDownPower = settings.gravity * -1.0f;
		//風船がある時は落下速度減少 / when character has some balloon, slow down
		float gravity = settings.gravityFactorWithBalloon;
		if (balloonNum == 2) {
			gravity *= 1.5f;
		} else if (balloonNum == 1) {
			gravity *= 2.0f;
		} else if (balloonNum == 0) {
			gravity = 1.0f;
			//減少しない / not slow down
		}
		downPower *= gravity;
		maxDownPower *= gravity;

		//縦方向の速度から引く / subtracte from verticalVelocity
		if (verticalVelocity > maxDownPower) {
			verticalVelocity -= downPower;
		}

		//移動する.
		if (networkFlag == 1) {
			netMarker.Translate(0.0f, verticalVelocity, 0.0f);
		}
		if (networkFlag != 2) {
			transform.Translate(0.0f, verticalVelocity, 0.0f);
			cameraCont.Tracking();
		}
	}

	//------------------------------
	//ダメージを受ける / Suffer damage
	//------------------------------
	private void Damage(Collider _collider, bool isBalloonBrake) {
		Damage (_collider, isBalloonBrake, 1);
	}

	public void Damage(Collider _collider, bool isBalloonBrake, int brakeBalloonNum) {
		if (hitCoolTimer > 0.0f) {
			isBalloonBrake = false;
		} else {
			hitCoolTimer = settings.networkHitCoolTime;
		}

		//ダメージモーション中or既に風船が0の時は処理しない.
		if (damageFrame != -1) {
			return;
		}
		
		//無敵モード中は風船を割らない.
		if (isArmorMode) {
			isBalloonBrake = false;
		}

		//衝突方向を取得 / Get the collision direction
		Ray ray = new Ray(transform.position, _collider.bounds.center - transform.position);
		RaycastHit hit;
		if (_collider.Raycast(ray, out hit, 100.0f)) {
			hitNormal = hit.normal;
		}

		damageFrame = 0;
		if (networkFlag == 1) {
			if (isBalloonBrake) {
				networkView.RPC("NetAnimeSetTrigger", RPCMode.OthersBuffered, "isDamageTrigger");
			} else {
				networkView.RPC("NetAnimeSetTrigger", RPCMode.OthersBuffered, "isDamageTriggerVS");
			}
		}
		if (networkFlag != 2) {
			if (isBalloonBrake) {
				animator.SetTrigger("isDamageTrigger");
			} else {
				animator.SetTrigger("isDamageTriggerVS");
			}
			GameController.isCanInput = false;
		}

		horizontalVelocity = 0.0f;
		verticalVelocity = 0.0f;

		//SE
		if (isBalloonBrake) { soundDamage[balloonNum].Play(); }

		if (isBalloonBrake) {
			//風船を割る / break a balloon
			if (balloonNum > 0) {
				balloonNum -= brakeBalloonNum;
				if (balloonNum < 0) {
					balloonNum = 0;
				}
			}
			ChangeBalloonColor();
		}
	}
	//------------------------------
	//ダメージによる移動 / Moving by damage
	//------------------------------
	private void DamageMove() {
		Vector3 hitVector = hitNormal * Time.fixedDeltaTime;
		if (transform.rotation.eulerAngles.y > 0.0f) {
			hitVector.x *= -1.0f;
		}
		
		if (networkFlag == 1) {
			netMarker.Translate(hitVector);
		}
		if (networkFlag != 2) {
			transform.Translate(hitVector);
			bool cameraMove = cameraCont.Tracking();
			
			float bgMoveFactor = hitNormal.x / settings.charaMovePower;
			
			if (bgCont != null) {
				if (hitNormal.x > 0.0f && cameraMove) {
					bgCont.Move(true, Mathf.Abs(bgMoveFactor));
				} else if (hitNormal.x < 0.0f && cameraMove) {
					bgCont.Move(false, Mathf.Abs(bgMoveFactor));
				}
			}
		}
	}
	//------------------------------
	//ダメージエフェクト終了 / Damage effect end
	//------------------------------
	private void DamageEnd() {
		if (networkFlag != 2) {
			GameController.isCanInput = true;
		}
		damageFrame = -1;
	}

	//------------------------------
	//風船の色変更 / Color change of the balloon
	//------------------------------
	public void ChangeBalloonColor() {
		//風船の数シンクロ.
		if (networkFlag == 1) {
			networkView.RPC("NetSetBalloonNum", RPCMode.OthersBuffered, balloonNum, hitCoolTimer);
			networkView.RPC("NetAnimeSetInt", RPCMode.OthersBuffered, "balloonNum", balloonNum);
		}
		if (networkFlag != 2) {
			animator.SetInteger("balloonNum", balloonNum);
		}
	}
	
	//------------------------------
	//ゴール / Goal
	//------------------------------
	private void Goal() {
		GameController.isCanInput = false;
		GameController.isCanMasterInput = false;
		GameController.timerCont.isCountdown = false;
		MoveInput(true);
		Stop();
		GameController.resultCont.StageClear();
		stageClearTimer = 0.0f;
		
		soundBGM.FadeOut(0.01f);
		soundGoal.Play();
	}

	//------------------------------
	//地面に落下（ゲームオーバー） / fall to the ground (gameover)
	//------------------------------
	private void Landing() {
		if (networkFlag == 1) {
			networkView.RPC("NetAnimeSetBool", RPCMode.OthersBuffered, "isFall", 1);
		}
		if (networkFlag != 2) {
			isLanding = true;
			damageFrame = -1;
			GameController.isCanInput = false;
			GameController.isCanMasterInput = false;
			animator.SetBool("isFall", true);
			
			//SE
			soundDead.Play();	
		}
		
		soundBGM.FadeOut(0.01f);

		//持っている速度を0にする（復活時に正常にするため）.
		horizontalVelocity = 0.0f;
		verticalVelocity = 0.0f;

		//ゲームオーバー処理.
		if (networkFlag == 0) {
			soundGameover.Play();
			GameController.GameOver(1);
			Invoke("GameOver2", 2.0f);
		} else if (networkFlag == 1) {
			soundGameover.FadeOut(0.01f);
			
			Debug.Log("netplayerID "+netPlayerID);
			vsResultComp.gameObject.networkView.RPC("VSDead", RPCMode.AllBuffered, netPlayerID);
		}
	}
	private void GameOver2() {
		GameController.GameOver(2);
	}
	//------------------------------
	//タイムアップによるゲームオーバー / GameOver by time up
	//------------------------------
	public void GameOverByTimeup () {
		animator.SetBool("isTimeUp", true);

		//SE
		soundBGM.FadeOut(0.1f);
		soundGameover.Play();

		GameController.GameOver(1);
		Invoke("GameOver2", 2.0f);
	}

	//------------------------------
	//衝突した / Start Collision
	//------------------------------
	void OnTriggerEnter (Collider other) {
		TriggerEnter(other, gameObject.tag, collider);
	}
	//衝突時の処理一式：other=衝突相手、senderTag=衝突を受けたオブジェクトのタグ.
	//gameObjet.tagは"衝突されたプレイヤー"を指す、Playerなら自分、Enemyなら敵.
	//senderTagは"衝突されたオブジェクト"を指す、Bodyなら身体（子オブジェクト）、それ以外なら風船。条件分岐に入れなければどちらの場合でも発動.
	//myColliderは衝突を受けた側のCollider.
	public void TriggerEnter (Collider other, string senderTag, Collider myCollider) {
		//マップ枠 / Map Frame
		if (other.tag == "Frame" && gameObject.tag == "Player") {
			isCollisionFrame[(int)other.gameObject.GetComponent<AreaFrame>().frameType] = true;
		}

		//サボテン / Cuctus
		if (other.tag == "Cuctus" && !isLanding && damageFrame == -1) {
			//風船が0の状態だったら、ゲームオーバー.
			if (balloonNum <= 0 && gameObject.tag == "Player") {
				Landing();
			} else {
				Damage(other, true);
			}
		}
		
		//ゴール / Goal
		if (other.tag == "Goal") {
			Goal();
			other.collider.enabled = false;
		}
		
		//これ以降の衝突を制限
		if (networkFlag == 2 || isLanding) {
			return;
		}

		//自分のボディと敵のボディが衝突：お互いの風船が割れる.
		//（前回割れてから一定時間内は無効）.
		if (gameObject.tag == "Player" && senderTag == "Body" && other.tag == "Body") {
			//相手が風船無しだったら、巻き添えでこちらの風船も0になる.
			PlayerController playerCont = other.gameObject.GetComponent<PlayerController>();
			if (playerCont == null) playerCont = other.transform.parent.gameObject.GetComponent<PlayerController>();
			if (playerCont.balloonNum == 0) {
				Damage(other, true, settings.maxBalloonNum);
			} else if (balloonNum == 0) {
				playerCont.Damage(myCollider, true, settings.maxBalloonNum);
			//そうでなければダメージを受ける.
			} else {
				Damage(other, true);
				playerCont.Damage(myCollider, true);
			}
		}

		//自分の風船と敵のボディが衝突：自分の風船が割れる（敵はノーリアクション）.
		//（前回割れてから一定時間内は無効）.
		if (senderTag == "Player" && other.tag == "Body") {
			Damage(other, true);
		}
	}

	//------------------------------
	//離れた / End Collision
	//------------------------------
	void OnTriggerExit (Collider other)	{
		TriggerExit(other, gameObject.tag);
	}
	public void TriggerExit (Collider other, string senderTag) {
		//マップ枠 / Map Frame
		if (other.tag == "Frame" && gameObject.tag == "Player") {
			isCollisionFrame[(int)other.gameObject.GetComponent<AreaFrame>().frameType] = false;
		}
	}

	//------------------------------
	//触れっぱなし / Stay Collision
	//------------------------------
	void OnTriggerStay (Collider other)	{
		TriggerStay(other, gameObject.tag);
	}
	public void TriggerStay (Collider other, string senderTag) {
		//地面 / Ground
		if (other.tag == "Ground" && !isLanding && gameObject.tag == "Player") {
			Landing();
		}
	}

	//------------------------------
	//復活後の無敵時間処理.
	//------------------------------
	public void ArmorModeStart() {
		StartCoroutine(ArmorMode());
	}
	private IEnumerator ArmorMode() {
		soundBGM.Play();
		isArmorMode = true;
		armorModeTimer = settings.networkRecoverArmorModeTime;
		vsResultComp.gameObject.networkView.RPC("VSAlive", RPCMode.AllBuffered, netPlayerID);

		//初期化.
		GameController.isCanInput = true;
		GameController.isCanMasterInput = true;
		balloonNum = settings.maxBalloonNum;
		ChangeBalloonColor();
		isLanding = false;
		networkView.RPC("NetAnimeSetBool", RPCMode.OthersBuffered, "isFall", 0);
		animator.SetBool("isFall", false);

		networkView.RPC("RecoverBlinkStart", RPCMode.OthersBuffered);

		//点滅ループ.
		Color show = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		Color hide = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		while (armorModeTimer > 0.0f) {
			foreach (SpriteRenderer child in childSpriteRenderers) {
				if (child.color.a == 1.0f) {
					child.color = hide;
				} else {
					child.color = show;
				}
			}

			armorModeTimer -= settings.networkRecoverArmorModeBlinkTime;
			yield return new WaitForSeconds (settings.networkRecoverArmorModeBlinkTime);
		}

		//終了.
		foreach (SpriteRenderer child in childSpriteRenderers) {
			child.color = show;
		}
		isArmorMode = false;
	}
	//------------------------------
	//点滅シンクロ用RPC（発動した人以外の人で実行される）.
	//------------------------------
	[RPC]
	public void RecoverBlinkStart() {
		StartCoroutine(RecoverBlink());
	}
	private IEnumerator RecoverBlink () {
		//点滅ループ.
		armorModeTimer = settings.networkRecoverArmorModeTime;
		Color show = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		Color hide = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		while (armorModeTimer > 0.0f) {
			foreach (SpriteRenderer child in childSpriteRenderers) {
				if (child.color.a == 1.0f) {
					child.color = hide;
				} else {
					child.color = show;
				}
			}
			
			armorModeTimer -= settings.networkRecoverArmorModeBlinkTime;
			yield return new WaitForSeconds (settings.networkRecoverArmorModeBlinkTime);
		}
		foreach (SpriteRenderer child in childSpriteRenderers) {
			child.color = show;
		}
	}
}
