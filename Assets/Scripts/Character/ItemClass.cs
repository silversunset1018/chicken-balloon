using UnityEngine;
using System.Collections;

/*
 * アイテムクラス / Item Class
 * 
 * アイテム取得時の処理を行う.
 * processing of item when getting.
 * 
*/
public class ItemClass : MonoBehaviour {

	public ItemType itemType;
	public GameObject soundObj;
	public int netID;	//ネット用ID.

	private GameSettings settings;
	private PlayerController player;

	//------------------------------
	// Use this for initialization
	//------------------------------
	void Start () {
		settings = GameObject.Find("GameSettings").GetComponent<GameSettings>();
		player = GameController.player;
	}

	//------------------------------
	//衝突した / Start Collision
	//------------------------------
	void OnTriggerEnter (Collider other){
		if (other.tag == "Player" || (other.tag == "Body" && other.transform.parent.tag == "Player")) {
			//ネットワークでのコイン：RPCを飛ばして後はお任せ.
			if (itemType == ItemType.NetworkCoin) {
				PlayerController playerCont = other.gameObject.GetComponent<PlayerController>();
				if (playerCont == null) playerCont = other.transform.parent.gameObject.GetComponent<PlayerController>();
				transform.parent.networkView.RPC("GetItem", RPCMode.Server, playerCont.netPlayerID, netID);
				return;
			}

			//種類毎の処理.
			//コイン.
			if (itemType == ItemType.Coin) {
				player.GetAnimator().SetInteger("ItemID", 1);
				ScoreUp(settings.scoreByCoin);
			//帽子.
			} else if (itemType == ItemType.Hat) {
				player.GetAnimator().SetInteger("ItemID", 2);
				ScoreUp(settings.scoreByHat);
			//マラカス.
			} else if (itemType == ItemType.Maracas) {
				player.GetAnimator().SetInteger("ItemID", 3);
				TimeRecover(settings.timeByMaracas);
			//テキーラ.
			} else if (itemType == ItemType.Tequila) {
				player.GetAnimator().SetInteger("ItemID", 4);
				BalloonRecover(1, settings.scoreByTequila);
			//タコス.
			} else if (itemType == ItemType.Tacos) {
				player.GetAnimator().SetInteger("ItemID", 5);
				BalloonRecover(settings.maxBalloonNum, settings.scoreByTacos);
			//風船（課金アイテム）.
			} else if (itemType == ItemType.Balloon) {
				player.GetAnimator().SetInteger("ItemID", 6);
				Soomla.Store.StoreInventory.GiveItem("currency_balloon", 1);
				//GameController.shopBalloon ++;
				GameController.SetBalloonText();
			}

			//アニメと非表示とサウンド.
			player.GetAnimator().SetTrigger("isGetItem");
			gameObject.SetActive(false);
			Instantiate(soundObj, transform.position, transform.rotation);
		}
	}

	//------------------------------
	//スコアアップアイテムを取った時.
	//------------------------------
	private void ScoreUp(int score) {
		//スコア加算 / Add score
		GameController.nowScore += score;
	}
	//------------------------------
	//タイム加算アイテムを取った時.
	//------------------------------
	private void TimeRecover(float time) {
		GameController.TimeRecover(time);
	}
	//------------------------------
	//風船回復アイテムを取った時.
	//風船の量が最大だったらスコアを加算、そうでない時は風船を回復させる.
	//------------------------------
	private void BalloonRecover(int balloon, int score) {
		if (player.balloonNum == settings.maxBalloonNum) {
			GameController.nowScore += score;
		} else {
			player.balloonNum += balloon;
			if (player.balloonNum > settings.maxBalloonNum) {
				player.balloonNum = settings.maxBalloonNum;
			}
			player.ChangeBalloonColor();
		}
	}

	//------------------------------
	//ネットワークコインを取った時（RPCから呼ばれる）.
	//------------------------------
	[RPC]
	void GetNetworkCoin() {
		//gameObject.SetActive(false);
		if (gameObject.collider.enabled) {
			gameObject.collider.enabled = false;
			gameObject.renderer.enabled = false;
			Instantiate(soundObj, transform.position, transform.rotation);
		}
	}
}

//------------------------------
//アイテムの種類 / ItemType
//------------------------------
public enum ItemType {
	Coin, Hat, Maracas, Tequila, Tacos, Balloon, NetworkCoin
}
