using UnityEngine;
using System.Collections;

/*
 * 通過となる風船の個数表示.
 * 
*/
public class CurrentBalloonController : MonoBehaviour {
	public Sprite[] numbers;
	public SpriteRenderer[] digits;
	public SpriteRenderer header;

	// Use this for initialization
	void Start () {
		SetBalloonText();
	}

	//------------------------------
	//残り時間の表示（ステージクリア時も呼ぶ） / Remain time display (call when stage clear too)
	//------------------------------
	public void SetBalloonText() {
		//0で埋める / fill in 0
		Soomla.Example.ExampleLocalStoreInfo.UpdateBalances();
		int cur = Soomla.Example.ExampleLocalStoreInfo.CurrencyBalance;
		string scoreStr = cur.ToString().PadLeft(digits.Length, '0');
		
		//数字画像を適用させる / adapt the number image
		int j = 0;
		for (int i = digits.Length - 1; i >= 0; i--) {
			string numStr = scoreStr.Substring(j, 1);
			digits[i].sprite = numbers[int.Parse(numStr)];
			j++;
		}
	}

	//------------------------------
	//重ね順を直す.
	//------------------------------
	public void SetOrderForward() {
		foreach (SpriteRenderer item in digits) {
			item.sortingOrder = 200;
		}
		header.sortingOrder = 200;
	}
	public void SetOrderBack() {
		foreach (SpriteRenderer item in digits) {
			item.sortingOrder = 100;
		}
		header.sortingOrder = 100;
	}
}
