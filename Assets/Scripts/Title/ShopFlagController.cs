using UnityEngine;
using System.Collections;

//------------------------------
//Shopに直行する為のフラグを管理する.
//------------------------------
public class ShopFlagController : MonoBehaviour {
	public static bool isOpenShop;
	public GameObject shopObj;

	void Start () {
		//フラグが立っていたらショップを開く.
		if (isOpenShop) {
			shopObj.SetActive(true);
			isOpenShop = false;
		}

		//同名のオブジェクトが既にいたら自殺する（判別の為に自分は一度リネーム）.
		string beforeName = gameObject.name;
		gameObject.name = beforeName + "_";
		if (GameObject.Find(beforeName)) {
			Destroy(gameObject);
		} else {
			gameObject.name = beforeName;
			DontDestroyOnLoad(this.gameObject);
		}
	}
}
