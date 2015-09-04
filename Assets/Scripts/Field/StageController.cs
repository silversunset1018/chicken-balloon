using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * ステージ管理クラス / Area Control Class
 * 
 * ステージ構成を制御する.
 * control the stage configuration.
 * 
*/
public class StageController : MonoBehaviour {
	
	public float areaLength;

	private GameSettings settings;
	public GameObject player;
	public Transform frameTop;
	public Transform frameBottom;

	//private List<AreaObject> areas;
	//private List<AreaObject> areaItems;
	//private GameObject goalObj;
	//private List<GameObject> areaObjs;

	public static float stageLengthMax;

	//開始位置用マージン（この余白を開けてからオブジェクト配置開始）.
	//Object placement start from this margin.
	private const float START_MARGIN = 12.0f;

	// Use this for initialization
	void Start () {
		//areas = new List<AreaObject>();
		//areaItems = new List<AreaObject>();
		//areaObjs = new List<GameObject>();

		//ステージ全体の長さを計算する / calculate the length of the entire stage
		settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
		stageLengthMax = settings.charaMovePower * settings.stageLongFactor;
		Vector3 scale = new Vector3(stageLengthMax + areaLength*2 + START_MARGIN, 1.0f, 1.0f);
		frameTop.localScale = scale;
		frameBottom.localScale = scale;

		//全エリアオブジェクトを取得する / get all AreaObject
		/*
		foreach (Transform child in transform) {
			AreaObject areaComp = child.GetComponent<AreaObject>();
			if (child.name == "Goal") {
				goalObj = child.gameObject;
			} else if (areaComp != null) {
				if (areaComp.itemObj != null) {
					areaItems.Add(areaComp);
				} else {
					areas.Add(areaComp);
				}
			}
		}
		*/

		MakeStage();
	}

	//------------------------------
	//ステージ作成 / Make stage
	//------------------------------
	public void MakeStage() {
		//Transform nowArea = null;
		//今回選択したステージオブジェクト.
		GameObject nowObj = null;

		//今回使用するステージ番号（最後が「Stage0」になるので注意）.
		int num = GameController.nowStage % settings.maxStageNum;

		//全ての子オブジェクトを一旦オフ.
		foreach (Transform child in transform) {
			child.gameObject.SetActive(false);

			//前のステージを破棄.
			if (child.name.IndexOf("(Clone)") != -1) {
				Destroy( child.gameObject );
			}

			//現在のステージに相当するオブジェクトを探す.
			if (child.name == "Stage"+num) {
				nowObj = child.gameObject;
			}
		}

		//現在のステージに相当するオブジェクトをコピー.
		if (nowObj != null) {
			nowObj = Instantiate(nowObj) as GameObject;
			nowObj.transform.parent = transform;
			nowObj.transform.position = Vector3.zero;
			nowObj.SetActive(true);
		}

		/*
		//現在のステージに相当するオブジェクトをオン.
		nowArea.gameObject.SetActive(true);
		Debug.Log ( nowArea.name );

		//現在のステージに存在するアイテム（＝現在のステージに相当するオブジェクトの子の子）をオン.
		foreach (Transform child in nowArea) {
			foreach (Transform grandchild in child) {
				grandchild.gameObject.SetActive(true);
			}
		}
		*/

		/*
		areaObjs.Clear();
		//エリアオブジェクトの配置を決める / decide the placement of area object
		DecideAreaObject();
		//アイテムの配置を決める / decide the placement of item
		DecideItem();
		*/
	}
	
	/*
	//------------------------------
	//エリアオブジェクトの配置を決める / decide the placement of the area object
	//------------------------------
	private void DecideAreaObject() {
		float stageLengthNow = START_MARGIN;
		int beforeID = -1;	//1つ前に選ばれたID / ID that was chosen before one

		while (stageLengthNow < stageLengthMax) {
			//エリアをランダムに選択 / randomly selected area
			int chooseID = Random.Range(0, areas.Count);
			while (beforeID == chooseID) {
				chooseID = Random.Range(0, areas.Count);
			}
			AreaObject pickup = areas[chooseID];
			GameObject pickupObj = (GameObject)Instantiate(pickup.gameObject, new Vector3(stageLengthNow, 0,0), Quaternion.identity);
			pickupObj.SetActive(true);
			pickupObj.transform.parent = transform;
			areaObjs.Add(pickupObj);

			//エリアの長さを加算 / add to area length;
			stageLengthNow += areaLength;
			beforeID = chooseID;
		}

		//ゴールを配置 / place goal
		goalObj.transform.position = new Vector3(stageLengthNow, 0,0);
		goalObj.SetActive(true);
		goalObj.collider.enabled = true;
	}

	//------------------------------
	//アイテムの配置を決める / decide the placement of item
	//------------------------------
	private void DecideItem() {
		//並べたオブジェクトの内、規定数をランダムにピックアップする（同じ番号はピックアップしない）.
		//Among objects is arranged and randomly picked quorum (Do not pick up the same number)
		List<int> idList = new List<int>();
		for (int i = 0; i < areaObjs.Count; i++) {
			idList.Add(i);
		}

		int itemNumInStage = -1;
		if (settings.itemNumInStage.Length < GameController.nowStage) {
			itemNumInStage = settings.itemNumInStage.Length -1;
		} else {
			itemNumInStage = settings.itemNumInStage[GameController.nowStage - 1];
		}

		int[] chooseIDs = new int[itemNumInStage];
		for (int i = 0; i < chooseIDs.Length; i++) {
			int rand = Random.Range(1, idList.Count);
			chooseIDs[i] = idList[rand];
			idList.RemoveAt(rand);
		}

		//置かれている通常エリアと差し替える / replace the normal area
		int beforeID = -1;	//1つ前に選ばれたID / ID that was chosen before one
		
		for (int i = 0; i < chooseIDs.Length; i++) {
			//エリアをランダムに選択 / randomly selected area
			int chooseID = Random.Range(0, areaItems.Count);
			while (beforeID == chooseID) {
				chooseID = Random.Range(0, areaItems.Count);
			}
			AreaObject pickup = areaItems[chooseID];
			GameObject pickupObj = (GameObject)Instantiate(pickup.gameObject, new Vector3(areaLength * chooseIDs[i] + START_MARGIN, 0,0), Quaternion.identity);
			pickupObj.SetActive(true);
			pickupObj.transform.parent = transform;
			//pickup.itemObj.SetActive(true);
			
			Destroy(areaObjs[chooseIDs[i]]);
			beforeID = chooseID;
		}
	}
	*/
}
