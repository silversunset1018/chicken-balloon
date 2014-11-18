using UnityEngine;
using System.Collections;

public class ItemController : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		GameSettings settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
		
		GameObject itemObj = null;
		//レアアイテムが出るか判定.
		if (Random.value < settings.rarity) {
			//レアアイテムからランダムで選択.
			itemObj = settings.rareItemPrefab[ Random.Range(0, settings.rareItemPrefab.Length - 1) ];
		} else {
			itemObj = settings.normalItemPrefab[ Random.Range(0, settings.normalItemPrefab.Length - 1) ];
		}
		
		//アイテム生成.
		GameObject obj = Instantiate(itemObj) as GameObject;
		obj.transform.parent = transform;
		obj.transform.localPosition = Vector3.zero;
	}
}
