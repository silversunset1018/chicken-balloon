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

	private CameraController cameraCont;
	private SpriteRenderer balloonSprite;
	private Animator animator;
	private Vector3 initialPosition;

	private bool[] isCollisionFrame;	//0:left  1:top  2:bottom
	private float jumpTimer;
	private int damageFrame = -1;
	private Vector3 hitNormal;
	private bool isRightBeforeFrame = true;
	private bool isStopping;
	private bool isLanding;

	private float stageClearTimer = 0.0f;
	private List<Transform> imgChildren;

	private float horizontalVelocity;
	private float verticalVelocity;

	//まばたき.
//	public Sprite blinkSprite;
//	private Sprite normalSprite;
//	private SpriteRenderer chickenRenderer;
//	public float blinkTime;			//まばたきの際、眼を閉じている時間。単位：秒.
//	public Vector2 blinkMargin;		//まばたきをする頻度。単位：秒。この範囲でランダムな値を作る.
//	private float blinkTimer;
//	private bool isBlink;

	//------------------------------
	// Use this for initialization
	//------------------------------
	void Start () {
		cameraCont = Camera.main.GetComponent<CameraController>();
		animator = gameObject.GetComponent<Animator>();
		isCollisionFrame = new bool[3];
		settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
		balloonNum = settings.maxBalloonNum;
		balloonSprite = transform.Find("balloon").GetComponent<SpriteRenderer>();
		balloonSprite.sprite = balloonSprite_green;
		initialPosition = transform.position;

		imgChildren = new List<Transform>();
		foreach (Transform child in transform) {
			if (!child.gameObject.activeSelf) {
				imgChildren.Add(child);
			}
		}

		//まばたき用.
//		chickenRenderer = transform.Find("chicken_a").GetComponent<SpriteRenderer>();
//		normalSprite = chickenRenderer.sprite;
//		blinkTimer = Random.Range(blinkMargin.x, blinkMargin.y);
	}

	//Getter
	public Animator GetAnimator() {
		return animator;
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
		animator.SetBool("isTimeUp", false);
		soundGoal.Stop();
		soundGameover.Stop();
		soundBGM.Stop();	//フェード中でも停止させる / Stop bgm during fadeout
		soundBGM.Play();
	}

	//------------------------------
	// Update is called once per frame
	//------------------------------
	void Update () {
		if (GameController.isCanInput) {
			Fall();
		}
		if (isStopping) {
			Stop();
		}

		//ダメージ演出終了チェック（エラー防止の為、数フレーム分の間を空ける）.
		//Damage end checking (For error prevention, wait for a few frames)
		if (damageFrame > 2) {
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
		if (stageClearTimer > -1.0f && stageClearTimer < 3.0f) {
			transform.Translate(settings.charaMovePower * Time.deltaTime, 0.0f, 0.0f);
			stageClearTimer += Time.deltaTime;
			animator.SetFloat("nowMove", 1.0f);
		} else if (stageClearTimer > 3.0f) {
			//移動完了後 / move complete
			stageClearTimer = -1.0f;
			transform.position = startPosition;
			animator.SetFloat("nowMove", 0.0f);
			GameController.gamestartGUI.collider.enabled = true;
		}
		
//		Debug.Log("! "+chickenRenderer.sprite);
//		//まばたき.
//		if (blinkTimer >= 0.0f) {
//			blinkTimer -= Time.deltaTime;
//		} else {
//			//ノーマル時のみまばたきする.
//			if (animator.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.chicken_normal")) {
//				//まばたき中（眼を閉じている）だったら開ける.
//				if (isBlink) {
//					chickenRenderer.sprite = normalSprite;
//					blinkTimer = Random.Range(blinkMargin.x, blinkMargin.y);
//					Debug.Log(chickenRenderer.sprite);
//					isBlink = false;
//				//閉じる.
//				} else {
//					chickenRenderer.sprite = blinkSprite;
//					blinkTimer = blinkTime;
//					isBlink = true;
//					Debug.Log(chickenRenderer.sprite);
//				}
//			}
//		}
	}

	//------------------------------
	//Z値は固定する / fixed Z position
	//------------------------------
	void LateUpdate () {
		Vector3 pos = transform.position;
		pos.z = 0.0f;
		transform.position = pos;
	}

	//------------------------------
	//左右移動 / Move horizontal
	//dir=true:right dir=false:left
	//------------------------------
	public void Move (bool dir) {
		//減速中、同じ方向に入力があったら減速中止 / when be slowly, if there is same direction input, slow down is stop.
		if (isStopping && isRightBeforeFrame == dir) {
			isStopping = false;
		}

		float moveX = settings.charaMovePower * Time.deltaTime * 0.06f;
		
		//横方向の速度を増加 / add to horizontalVelocity
		if (horizontalVelocity < settings.charaMovePower * Time.deltaTime * 0.8f) {
			horizontalVelocity += moveX;
		}
		//左の枠に当たっている時は左へ移動しない / Does not move to left when you are hitting the frame of the left
		if (!dir && isCollisionFrame[0]) {
			horizontalVelocity = 0.0f;
		}

		//移動する / Moving
		transform.Translate(horizontalVelocity, 0.0f, 0.0f);
		if (cameraCont.Tracking()) {
			float bgSpeed = horizontalVelocity / settings.charaMovePower * 50.0f;
			Mathf.Clamp01(bgSpeed);
			bgCont.Move(dir, bgSpeed);
		}

		//画像の方向を変える / change direction in image.
		//このオブジェクトを反転させた後、アイテム取得時のGUIだけ更に反転させる / After reverse the this object, to reverse further only GUI of item acquisition.
		Quaternion rot = transform.rotation;
		Vector3 rotChild = Vector3.zero;
		if (dir && !isRightBeforeFrame) {
			rot.y = 0.0f;
			rotChild = Vector3.up * 180.0f;
			horizontalVelocity = 0.0f;
		} else if (!dir && isRightBeforeFrame) {
			rot.y = 180.0f;
			rotChild = Vector3.up * -180.0f;
			horizontalVelocity = 0.0f;
		}
		transform.rotation = rot;
		foreach (Transform child in imgChildren) {
			child.Rotate(rotChild, Space.World);
		}
		isRightBeforeFrame = dir;
		
		animator.SetFloat("nowMove", 1.0f);
	}

	//------------------------------
	// 左右移動停止 / Stop moving horizontal
	//------------------------------
	public void Stop() {
		if (horizontalVelocity > 0.0f) {
			horizontalVelocity -= settings.charaMovePower * Time.deltaTime * 0.06f;
			isStopping = true;
		} else {
			isStopping = false;
		}
		transform.Translate(horizontalVelocity, 0.0f, 0.0f);
		if (cameraCont.Tracking()) {
			float bgSpeed = horizontalVelocity / settings.charaMovePower * 50.0f;
			Mathf.Clamp01(bgSpeed);
			bgCont.Move(isRightBeforeFrame, bgSpeed);
		}
		animator.SetFloat("nowMove", 0.0f);
	}

	//------------------------------
	//ジャンプ / Jump up
	//push=true:pussing push=false:release
	//------------------------------
	public void Jump (bool push) {
		if (!push) {
			jumpTimer = 0.0f;
			animator.SetBool("isJump", false);

			//縦方向の速度を消す / reset to verticalVelocity
			verticalVelocity = 0.0f;
			return;
		}
		if (jumpTimer < settings.charaJumpTime) {
			float downPower = settings.charaJumpPower * Time.deltaTime;
			//上の枠に当たっている時はジャンプしない / Does not jump when you are hitting the frame of the top
			if (isCollisionFrame[1]) {
				downPower = 0.0f;
			}

			//風船が0の時はジャンプしない / Does not jump when balloon 0
			if (balloonNum == 0) {
				downPower = 0.0f;
			}

			transform.Translate(0.0f, downPower, 0.0f);
			cameraCont.Tracking();
			jumpTimer += Time.deltaTime;
			animator.SetBool("isJump", true);
			soundWing.Play();
		} else {
			animator.SetBool("isJump", false);
		}
	}

	//------------------------------
	//落下 / Fall down
	//------------------------------
	private void Fall() {
		float downPower = settings.gravity * Time.deltaTime;
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

		transform.Translate(0.0f, verticalVelocity, 0.0f);
		cameraCont.Tracking();
	}

	//------------------------------
	//ダメージを受ける / Suffer damage
	//------------------------------
	private void Damage(Collider collider) {
		damageFrame = 0;
		animator.SetTrigger("isDamageTrigger");
		GameController.isCanInput = false;
		horizontalVelocity = 0.0f;
		verticalVelocity = 0.0f;

		//衝突方向を取得 / Get the collision direction
		Ray ray = new Ray(transform.position, collider.bounds.center - transform.position);
		RaycastHit hit;
		if (collider.Raycast(ray, out hit, 100.0f)) {
			hitNormal = hit.normal;
		}

		//SE
		soundDamage[balloonNum].Play();

		//風船を割る / break a balloon
		if (balloonNum > 0) {
			balloonNum --;
		}
		ChangeBalloonColor();

	}
	//------------------------------
	//ダメージによる移動 / Moving by damage
	//------------------------------
	private void DamageMove() {
		Vector3 hitVector = hitNormal * Time.deltaTime;
		if (transform.rotation.eulerAngles.y > 0.0f) {
			hitVector.x *= -1.0f;
		}

		transform.Translate(hitVector);
		bool cameraMove = cameraCont.Tracking();

		float bgMoveFactor = hitNormal.x / settings.charaMovePower;
		if (hitNormal.x > 0.0f && cameraMove) {
			bgCont.Move(true, Mathf.Abs(bgMoveFactor));
		} else if (hitNormal.x < 0.0f && cameraMove) {
			bgCont.Move(false, Mathf.Abs(bgMoveFactor));
		}
	}
	//------------------------------
	//ダメージエフェクト終了 / Damage effect end
	//------------------------------
	private void DamageEnd() {
		GameController.isCanInput = true;
		damageFrame = -1;
	}

	//------------------------------
	//風船の色変更 / Color change of the balloon
	//------------------------------
	public void ChangeBalloonColor() {
		animator.SetInteger("balloonNum", balloonNum);
	}
	
	//------------------------------
	//ゴール / Goal
	//------------------------------
	private void Goal() {
		GameController.isCanInput = false;
		GameController.timerCont.isCountdown = false;
		Move(true);
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
		isLanding = true;
		damageFrame = -1;
		GameController.isCanInput = false;
		animator.SetBool("isFall", true);

		//SE
		soundBGM.FadeOut(0.01f);
		soundDead.Play();
		soundGameover.Play();

		GameController.GameOver(1);
		Invoke("GameOver2", 2.0f);
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
		//マップ枠 / Map Frame
		if (other.tag == "Frame") {
			isCollisionFrame[(int)other.gameObject.GetComponent<AreaFrame>().frameType] = true;
		}

		//サボテン / Cuctus
		if (other.tag == "Cuctus" && !isLanding && damageFrame == -1) {
			//風船が0の状態だったら、ゲームオーバー.
			if (balloonNum <= 0) {
				Landing();
			} else {
				Damage(other);
			}
		}
		
		//ゴール / Goal
		if (other.tag == "Goal") {
			Goal();
			other.collider.enabled = false;
		}
	}

	//------------------------------
	//離れた / End Collision
	//------------------------------
	void OnTriggerExit (Collider other)	{
		//マップ枠 / Map Frame
		if (other.tag == "Frame") {
			isCollisionFrame[(int)other.gameObject.GetComponent<AreaFrame>().frameType] = false;
		}
	}

	//------------------------------
	//触れっぱなし / Stay Collision
	//------------------------------
	void OnTriggerStay (Collider other)	{
		//地面 / Ground
		if (other.tag == "Ground" && !isLanding) {
			Landing();
		}
	}
}
