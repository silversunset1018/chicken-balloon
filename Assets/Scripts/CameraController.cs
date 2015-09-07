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

	private bool[] isFrameLook;	//0:left  1:top  2:bottom
	private Vector3 prePosition;	//前フレームでの位置 / Position in the previous frame

	// Use this for initialization
	void Start () {
		isFrameLook = new bool[3];
		initialPosition = transform.position;
	}
	
	public void Init() {
		transform.position = initialPosition;
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
			} else {
				result = true;
			}

			//上の枠が映っていて、前フレームよりも上へ移動していたら、これ以上上に行かない.
			//When top pane is visible and character moved to the up than the previous frame, do not go up any more.
			if (isFrameLook[1] && pos.y >= prePosition.y) {
				pos.y = transform.position.y;
			}
			
			//下の枠が映っていて、前フレームよりも下へ移動していたら、これ以上下に行かない.
			//When bottom pane is visible and character moved to the down than the previous frame, do not go down any more.
			if (isFrameLook[2] && pos.y < prePosition.y) {
				pos.y = transform.position.y;
			}

			//移動する / Move
			transform.position = pos;

			prePosition = transform.position;
		}
		return result;
	}

	//フレームの表示状態を渡される / Passed the display state of the frame
	public void LookFrame(AreaFrameType frameType, bool flag) {
		isFrameLook[(int)frameType] = flag;
	}
}
