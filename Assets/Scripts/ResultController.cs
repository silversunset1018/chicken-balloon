using UnityEngine;
using System.Collections;

/*
 * 結果表示クラス / View Results Class
 * 
 * 結果表示を制御する.
 * control the results display.
 * 
*/
public class ResultController : MonoBehaviour {
	private GameSettings settings;
	public static GameObject stageclearGUI;

	public Sprite[] numbers;
	public SpriteRenderer[] digits;

	private int addScore;

	private float timer = 0.2f;
	private float goalTimer = 0.0f;
	private float GOAL_TIME = 4.0f;
	private bool isGoal;
	private int beforeScore;
	private float remainTime;

	// Use this for initialization
	void Awake () {
		stageclearGUI = GameObject.Find("stageclearGUI");
		settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
	}
	
	// Update is called once per frame
	void Update () {
		if (timer > 0.2f) {
			SetScoreText();
			timer = 0.0f;
		} else {
			timer += Time.deltaTime;
		}

		//ステージクリア後、スコアを加算する時間を計測 / Make score add time when stage clear.
		if (isGoal) {
			if (goalTimer > 0.0f) {
				ShowTotalScore();
				goalTimer -= Time.deltaTime;
			} else {
				ShowTotalScoreEnd(true);
				isGoal = false;
			}
		}
	}

	//------------------------------
	//スコアの表示 / Score display
	//------------------------------
	private void SetScoreText() {
		//0で埋める / fill in 0
		string scoreStr = GameController.nowScore.ToString().PadLeft(digits.Length, '0');

		//数字画像を適用させる / adapt the number image
		int j = 0;
		for (int i = digits.Length - 1; i >= 0; i--) {
			string numStr = scoreStr.Substring(j, 1);
			digits[i].sprite = numbers[int.Parse(numStr)];
			j++;
		}
	}

	//------------------------------
	//ステージクリア / Stage clear
	//------------------------------
	public void StageClear() {
		beforeScore = GameController.nowScore;
		remainTime = GameController.timerCont.remainTime;
		SetTotalScore();
		goalTimer = GOAL_TIME;
		isGoal = true;
		
		stageclearGUI.collider.enabled = false;
		stageclearGUI.SetActive(true);

		//「ステージクリア時にタッチされる」メソッドはGameControllerで実装.
		// "Is touched clearing the stage during" method implemented in GameController.
		//GUIのオンオフのみ、このクラスで行う ->TouchStageClearGUI.
		//Only switch the GUI, I do in this class ->TouchStageClearGUI.
	}

	//------------------------------
	//ゲームオーバー / GameOver
	//------------------------------
	public void GameOver() {
		beforeScore = GameController.nowScore;
		SetTotalScore();
		ShowTotalScoreEnd(false);
	}


	//------------------------------
	//トータルスコア準備 / Total score setup
	//------------------------------
	public void SetTotalScore() {
		//トータルスコア計算 / Calculate total score
		int remainTimeScore = 0;
		float distance = transform.position.x;
		if (transform.position.x > StageController.stageLengthMax) {
			distance = StageController.stageLengthMax;
			remainTimeScore = (int)GameController.timerCont.remainTime * (int)settings.scoreTimeFactor;
		}
		float positionScore = distance / StageController.stageLengthMax * settings.scoreByMaxStage;
		addScore = remainTimeScore + (int)positionScore;
	}

	//------------------------------
	//トータルスコア表示 / Total score display
	//------------------------------
	public void ShowTotalScore() {
		float nowAdd = addScore / (GOAL_TIME * (1.0f / Time.deltaTime));
		GameController.nowScore += (int)nowAdd;

		if ((GameController.timerCont.remainTime - Time.deltaTime) > 0.0f) {
			GameController.timerCont.remainTime -= remainTime / (GOAL_TIME * (1.0f / Time.deltaTime));
		} else {
			GameController.timerCont.remainTime = 0.0f;
		}
		GameController.timerCont.SetTimeText();
	}
	//------------------------------
	//トータルスコア表示終了 / Total score display end
	//------------------------------
	public void ShowTotalScoreEnd(bool isStageClear) {
		//端数対策の為、確実な値を代入する / assign certain values
		GameController.nowScore = beforeScore + addScore;
		GameController.timerCont.remainTime = 0.0f;

		if (isStageClear) {
			stageclearGUI.collider.enabled = true;
		}
	}


	//------------------------------
	//ステージクリア時にタッチされた際のGUI切り替え / GUI switch when it is touched clearing the stage when
	//------------------------------
	public void TouchStageClearGUI () {
		stageclearGUI.SetActive(false);
	}
}
