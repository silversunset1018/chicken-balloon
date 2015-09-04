using UnityEngine;
using System.Collections;

// 自分がサーバーの場合、このオブジェクトは非アクティブになる.
//（ホームボタンをオフにする）.
public class ButtonActiveWithServer : MonoBehaviour {
	public bool isSaveMeBtn = true;		//SaveMeボタンだったらクリック.

	void Awake () {
		if (Network.isServer) {
			if (isSaveMeBtn) {
				if (!GameController.CheckRecoverInVS()) {
					gameObject.SetActive(false);
				}
			} else {
				gameObject.SetActive(false);
			}
		}

	}
}
