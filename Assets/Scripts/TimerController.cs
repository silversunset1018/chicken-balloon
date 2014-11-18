using UnityEngine;
using System.Collections;

/*
 * 制限時間管理クラス / Limit time management Class
 * 
*/
public class TimerController : MonoBehaviour {
	public float remainTime;
	public bool isCountdown;

	public Sprite[] numbers;
	public SpriteRenderer[] digits;

	private GameSettings settings;

	// Use this for initialization
	void Start () {
		settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
		Init();
	}

	//------------------------------
	//初期化/ Initialization
	//------------------------------
	public void Init() {
		if (GameController.nowStage > settings.timeLimit.Length) {
			remainTime = settings.timeLimit[settings.timeLimit.Length - 1];
		} else {
			remainTime = settings.timeLimit[GameController.nowStage - 1];
		}
		SetTimeText();
	}

	// Update is called once per frame
	void Update () {
		if (isCountdown) {
			remainTime -= Time.deltaTime;

			if (remainTime < 1.0f) {
				remainTime = 0.0f;
				isCountdown = false;
				GameController.GameOverByTimeup();
			}
			SetTimeText();
		}
	}

	//------------------------------
	//残り時間の表示（ステージクリア時も呼ぶ） / Remain time display (call when stage clear too)
	//------------------------------
	public void SetTimeText() {
		//0で埋める / fill in 0
		int time = (int)remainTime;
		string scoreStr = time.ToString().PadLeft(digits.Length, '0');
		
		//数字画像を適用させる / adapt the number image
		int j = 0;
		for (int i = digits.Length - 1; i >= 0; i--) {
			string numStr = scoreStr.Substring(j, 1);
			digits[i].sprite = numbers[int.Parse(numStr)];
			j++;
		}
	}
}
