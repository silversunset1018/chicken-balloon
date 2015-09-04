using UnityEngine;
using System.Collections;

/*
 * カメラ管理クラス / Camera Control Class
 * 
 * カメラの移動を制御する.
 * control the movement of the camera.
 * 
*/
public class CameraController : MonoBehaviour {
	public bool isTrack = true;
	public Transform characterObj;
	private Vector3 initialPosition;
	private GameSettings settings;

	private bool[] isFrameLook;	//0:left  1:top  2:bottom	3:right
	private Vector3 prePosition;	//前フレームでの位置 / Position in the previous frame

	private float timer;
	private float START_TIME = 0.1f;

	// Use this for initialization
	void Start () {
		settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
	
	//画面縦横比確認.
	int sw = Screen.width;
	int sh = Screen.height;
	
	//2:3
		float stmp = (sh / 2.0f * 3.0f);
		if (stmp == sw) {
						settings.networkCameraPosition.x -= 1.5f;
						settings.networkCameraPosition.y += 1.5f;
		
		}
		
	//3:4
		stmp = (int)(sh / 3.0f * 4.0f);
		if (stmp == sw) {
						settings.networkCameraPosition.x -= 2.4f;
						settings.networkCameraPosition.y += 2.4f;
						}


		isFrameLook = new bool[4];
		timer = START_TIME;
		InitSet ();
	}
	
	public void Init() {
		transform.position = initialPosition;
	}
	public void InitSet() {
		initialPosition = transform.position;
		prePosition = transform.position;
	}

	//------------------------------
	//PlayerControllerから呼ばれる / Call by PlayerController
	//左右移動を行ったらtrueを返す / if move left or right, return true
	//------------------------------
	public bool Tracking() {
		bool result = false;
		if (isTrack) {
			Vector3 pos = characterObj.position;
			pos.z = transform.position.z;	//Z位置は変更しない / keep position.z

			//左の枠が映っていて、前フレームよりも左へ移動していたら、これ以上左に行かない.
			//When left pane is visible and character moved to the left than the previous frame, do not go left any more.
			if (isFrameLook[0] && pos.x <= prePosition.x) {
				pos.x = transform.position.x;
			} else if (!isFrameLook[0] && pos.x > prePosition.x) {
				result = true;
			}

			//上の枠が映っていて、前フレームよりも上へ移動していたら、これ以上上に行かない.
			//When top pane is visible and character moved to the up than the previous frame, do not go up any more.
			if (isFrameLook[1] && pos.y >= prePosition.y) {
				pos.y = transform.position.y;
			}
			
			//下の枠が映っていて、前フレームよりも下へ移動していたら、これ以上下に行かない.
			//When bottom pane is visible and character moved to the down than the previous frame, do not go down any more.
			if (isFrameLook[2] && pos.y <= prePosition.y) {
				pos.y = transform.position.y;
			}
			
			//右の枠が映っていて、前フレームよりも右へ移動していたら、これ以上右に行かない.
			if (isFrameLook[3] && pos.x >= prePosition.x) {
				pos.x = transform.position.x;
			} else if(!isFrameLook[3] && pos.x < prePosition.x) {
				result = true;
			}

			//通信対戦中で、枠が映っておらず、かつチキンの位置が設定位置よりも端にある場合は設定位置にする.
			if (settings.isNetworkScene) {
				if (!isFrameLook[0] && pos.x < settings.networkCameraPosition.x) {
					pos.x = settings.networkCameraPosition.x;
				}
				if (!isFrameLook[3] && pos.x > settings.networkCameraPosition.y) {
					pos.x = settings.networkCameraPosition.y;
				}
			}

			//移動する / Move
			if (timer < 0) {
				transform.position = pos;
			} else {
				transform.position = Vector3.Lerp(transform.position, pos, 0.1f);
				timer -= Time.deltaTime;
			}

			prePosition = transform.position;
		}
		return result;
	}

	//------------------------------
	//絶対値指定（横）.
	//ネットワーク時のループで使用する（PlayerControllerから呼ばれる）.
	//------------------------------
	public void SetX (float valX) {
		isTrack = false;
		transform.position = new Vector3(valX, transform.position.y, transform.position.z);
		prePosition = transform.position;
		isTrack = true;
	}


	//フレームの表示状態を渡される / Passed the display state of the frame
	public void LookFrame(AreaFrameType frameType, bool flag) {
		isFrameLook[(int)frameType] = flag;
	}
}
