using UnityEngine;
using System.Collections;

//------------------------------
//ネット用にアイテムを管理する.
//------------------------------
public class VSItem : MonoBehaviour {
	private ItemClass[] items;
	private GameObject netScore;

	// Use this for initialization
	void Start () {
		netScore = GameObject.Find("Result_network");

		//子オブジェクトのアイテムクラスを保存.
		items = new ItemClass[transform.childCount];
		int num = 0;
		foreach (Transform item in transform) {
			items[num] = item.GetComponent<ItemClass>();
			num ++;
		}
	}

	//------------------------------
	//アイテム取得時、各端末から呼び出される（サーバーのみ）.
	//------------------------------
	[RPC]
	void GetItem(int playerID, int itemID) {
		//対象のアイテムを探す.
		foreach (ItemClass item in items) {
			if (item.netID == itemID) {
				item.networkView.RPC("GetNetworkCoin", RPCMode.AllBuffered);
				netScore.networkView.RPC("GetCoin", RPCMode.AllBuffered, playerID, itemID);
			}
		}
	}
}
