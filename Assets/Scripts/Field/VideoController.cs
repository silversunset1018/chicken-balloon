using UnityEngine;
using System.Collections;
using Fresvii.AppSteroid;

/*
 * ビデオ管理クラス / Limit time management Class
 * 
*/
public class VideoController : MonoBehaviour {
	public GameObject videoIconObj;
	public GameObject timerParentObj;
	public GameObject shareBtnObj;
	public GameObject replayBtnObj;
	public GameObject savemeBtnObj;

	private float remainTime;
	private bool isCountdown;
	private const float SHARE_BTN_POS_X = 2.3f;
	private const float SHARE_BTN_POS_Y = 1.0f;
	private const float SHARE_BTN_POS_Z = 1.0f;

	public Sprite[] numbers;
	public SpriteRenderer[] digits;

	private bool isRecoding;	//SDKから録画状態の取得もできるが（FASPlayVideo.IsRecording）デバッグしにくいので独自で持つ.

	// Use this for initialization
	void Start () {
		if (Application.platform == RuntimePlatform.Android) {
			gameObject.SetActive(false);
			if (shareBtnObj != null) { shareBtnObj.SetActive(false); }
			replayBtnObj.transform.localPosition = new Vector3(-SHARE_BTN_POS_X, SHARE_BTN_POS_Y, SHARE_BTN_POS_Z);
			savemeBtnObj.transform.localPosition = new Vector3(SHARE_BTN_POS_X, SHARE_BTN_POS_Y, SHARE_BTN_POS_Z);
		} else {
            if (shareBtnObj != null && replayBtnObj != null)
            {
                shareBtnObj.transform.localPosition = new Vector3(SHARE_BTN_POS_X*2.0f, SHARE_BTN_POS_Y, SHARE_BTN_POS_Z);
                replayBtnObj.transform.localPosition = new Vector3(-SHARE_BTN_POS_X*2.0f, SHARE_BTN_POS_Y, SHARE_BTN_POS_Z);
                shareBtnObj.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
                shareBtnObj.collider.enabled = false;
            }
		}
	}

	//------------------------------
	//ボタンが押された/ Initialization
	//------------------------------
	public void PushButton() {
		//録画中なら止める.
		if (isRecoding) {
			StopPlay ();
		//そうでないなら録画開始.
		} else {
			StartPlay();
		}
	}

	// Update is called once per frame
	void Update () {
		if (isCountdown) {
			remainTime -= Time.deltaTime;

			//時間が来た時の処理.
			if (remainTime < 1.0f) {
				remainTime = 0.0f;
				SetTimeText();
				StopPlay();
			}
			SetTimeText();

			//止まっていたら終了処理.
			if (!FASPlayVideo.IsRecording() && Application.platform == RuntimePlatform.IPhonePlayer) {
				StopPlay();
			}
		}
	}

	//------------------------------
	//残り時間の表示 / Remain time display
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

	//------------------------------
	//録画の開始・終了.
	//------------------------------
	private void StartPlay() {
		FASPlayVideo.StartRecording();
		remainTime = FASPlayVideo.GetMaxRecordingSecondsLength();
		SetTimeText();
		isCountdown = true;
		timerParentObj.SetActive(true);
		videoIconObj.SetActive(false);
		isRecoding = true;

		if (!shareBtnObj.collider.enabled) {
			shareBtnObj.renderer.material.color = new Color(1f,1f,1f,1f);
			shareBtnObj.collider.enabled = true;
		}
	}
	private void StopPlay() {
		if (FASPlayVideo.IsRecording()) {
			FASPlayVideo.StopRecording();
		}
		isCountdown = false;
		timerParentObj.SetActive(false);
		videoIconObj.SetActive(true);
		isRecoding = false;
	}
}
