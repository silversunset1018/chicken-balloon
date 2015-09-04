using UnityEngine;
using System.Collections;

/*
 * マップの枠クラス / Frame of the map Class
 * 
 * インスペクタから設定し、各コンポーネントから参照して利用する.
 * Set from the inspector, to be used by reference from each component.
 * 
*/
public class AreaFrame : MonoBehaviour {
	public AreaFrameType frameType;
	private CameraController cameraCont;
	public bool isCameraUse = true;

	// Use this for initialization
	void Start () {
		cameraCont = Camera.main.GetComponent<CameraController>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnBecameVisible () {
		if (isCameraUse) {
			cameraCont.LookFrame(frameType, true);
		}
	}
	void OnBecameInvisible () {
		if (isCameraUse) {
			cameraCont.LookFrame(frameType, false);
		}
	}

}

/*
 * 枠の位置を示す / the position of the frame
 * 
*/
public enum AreaFrameType {
	left, top, bottom, right
}
