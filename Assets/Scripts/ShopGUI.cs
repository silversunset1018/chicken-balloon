using UnityEngine;
using System.Collections;
using Soomla;
using Soomla.Example;
using Soomla.Store;
using System;

public class ShopGUI : MonoBehaviour {
	//shop
	public SpriteRenderer btnShopBack;
	public SpriteRenderer btnShop7;
	public SpriteRenderer btnShop40;
	public SpriteRenderer btnShop100;

	//shop btn
	public Sprite[] btnShopBackSprites;
	public Sprite[] btnShop7Sprites;
	public Sprite[] btnShop40Sprites;
	public Sprite[] btnShop100Sprites;

	//sound
	public SoundController soundButton;

	private bool isPusshing;	//押しっぱなし防止用フラグ / flag for press keep prevention
	private bool isCanInput = true;
	private bool isCoroutine;	//コルーチン起動済みフラグ.

	// Update is called once per frame
	void Update () {
		CheckInput();	
	}
	
	//------------------------------
	//入力チェック / check input
	//------------------------------
	void CheckInput() {
		//モバイルではタッチで操作 / Operation in touch on mobile
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++) {
				if( touches[i].phase == TouchPhase.Began) {
					RayFromInput( touches[i].position, touches[i].fingerId );
				}
			}
			if (touches.Length == 0) {
				btnShopBack.sprite = btnShopBackSprites[0];
				btnShop7.sprite = btnShop7Sprites[0];
				btnShop40.sprite = btnShop40Sprites[0];
				btnShop100.sprite = btnShop100Sprites[0];
				isPusshing = false;
			}
			
			//それ以外ではクリックで操作 / Operation Click otherwise
		} else {
			if (Input.GetMouseButton(0)) {
				RayFromInput(Input.mousePosition, 0);
			} else {
				btnShopBack.sprite = btnShopBackSprites[0];
				btnShop7.sprite = btnShop7Sprites[0];
				btnShop40.sprite = btnShop40Sprites[0];
				btnShop100.sprite = btnShop100Sprites[0];
				isPusshing = false;
			}
		}
	}
	
	//------------------------------
	//入力座標からボタンが押されているか調べる / examine whether the button is pressed from the input coordinate
	//------------------------------
	void RayFromInput(Vector2 point, int fingerID) {
		RaycastHit hit = new RaycastHit();
		Ray ray = Camera.main.ScreenPointToRay( point );
		if ( Physics.Raycast( ray, out hit ) && !isPusshing && isCanInput) {
			isPusshing = true;

			//戻る.
			if(hit.transform == btnShopBack.transform) {
				soundButton.Play();
				btnShopBack.sprite = btnShopBackSprites[1];
				StartCoroutine("ShowBackDelay");
			}


			//購入.
			if(hit.transform == btnShop7.transform) {
				soundButton.Play();
				btnShop7.sprite = btnShop7Sprites[1];

				try {
					StoreInventory.BuyItem("balloons_7");
				} catch (Exception e) {
					Debug.Log ("SOOMLA/UNITY " + e.Message);
				}
			}
			if(hit.transform == btnShop40.transform) {
				btnShop40.sprite = btnShop40Sprites[1];
				
				try {
					StoreInventory.BuyItem("balloons_40");
				} catch (Exception e) {
					Debug.Log ("SOOMLA/UNITY " + e.Message);
				}
			}
			if(hit.transform == btnShop100.transform) {	
				btnShop100.sprite = btnShop100Sprites[1];
				
				try {
					StoreInventory.BuyItem("balloons_100");
				} catch (Exception e) {
					Debug.Log ("SOOMLA/UNITY " + e.Message);
				}
			}
		}
	}

	//------------------------------
	//ボタンの変化を見せるため、一瞬遅れて実行する.
	//------------------------------
	private IEnumerator ShowBackDelay () {
		yield return new WaitForSeconds (0.1f);
		gameObject.SetActive(false);
		isCoroutine = false;
	}
}
