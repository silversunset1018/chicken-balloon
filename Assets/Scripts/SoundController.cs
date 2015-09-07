using UnityEngine;
using System.Collections;

/*
 * サウンド管理クラス / Sound Controller Class
 * 
*/
public class SoundController : MonoBehaviour {

	public bool playOnAwake;	//一度だけ再生
	public bool dontDestroyChangeScene;	//シーンが変わっても破棄しない
	public bool autoDestract;	//再生が終わったら自殺する

	private float defVolume;

	// Use this for initialization
	void Start () {
		defVolume = audio.volume;

		if (playOnAwake) {
			Play ();
		}
		if (dontDestroyChangeScene) {
			DontDestroyOnLoad(gameObject);
		}
	}
	
	//------------------------------
	//再生 / Sound Play
	//------------------------------
	public void Play() {
		if (GameController.isSoundOn) {
			if (!audio.isPlaying) {
				audio.volume = defVolume;
				audio.Play();
			}
			if (autoDestract) {
				StartCoroutine("NowPlaying");
			}
		}
	}
	IEnumerator NowPlaying (){
		while (audio.isPlaying) {
			yield return 0;
		}
		Destroy(gameObject);
	}

	//------------------------------
	//停止 / Sound Stop
	//------------------------------
	public void Stop() {
		if (audio.isPlaying) {
			audio.Stop();
		}
	}
	
	//------------------------------
	//フェードアウト / Sound Fade Out
	//------------------------------
	public void FadeOut(float speed) {
		if (audio.isPlaying) {
			StartCoroutine("DoFadeOut", speed);
		}
	}
	IEnumerator DoFadeOut ( float speed ){
		while (audio.volume > 0.0f) {
			audio.volume -= speed;
			if (audio.volume <= 0.0f) {
				audio.volume = 0.0f;
				audio.Stop();
			}
			yield return 0;
		}
	}
}
