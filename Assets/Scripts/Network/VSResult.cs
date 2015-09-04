using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Models;

public class VSResult : MonoBehaviour {
	public PlayerController player;		//自分のプレイヤーコンポ.

	public int[] winCoinNums;	//勝利に必要なコインの数.
	private int winCoinNum;

	//勝敗時の画像.
	public Sprite winSprite;
	public Sprite loseSprite;
	//勝敗時のサウンド.
	public SoundController winSE;
	public SoundController loseSE;

	public Sprite[] numbers;	//各数字の画像.
	
	public NetworkPlayerScore[] playerScores;

	//復活時の位置.
	public Vector3[] respawnPosition;

	private List<int> beforeItemId;
	private bool isBattleEnd;

	// Use this for initialization
	void Start () {
		//使わない画像はオフ.
		if (Network.maxConnections <= 3) {
			playerScores[3].isAlive = false;
			playerScores[3].parentObj.SetActive (false);
		}
		if (Network.maxConnections <= 2) {
			playerScores[2].isAlive = false;
			playerScores[2].parentObj.SetActive (false);
		}

		beforeItemId = new List<int>();
	}

	//------------------------------
	//コインを取得した（ItemClassから呼ばれる）.
	//引数：取得したプレイヤーID.
	//------------------------------
	[RPC]
	void GetCoin (int playerID, int itemID) {
		foreach (int item in beforeItemId) {
			if (item == itemID) {
				return;
			}
		}
		beforeItemId.Add(itemID);

		playerScores[playerID].score ++;

		//スコアの表示.
		//0で埋める / fill in 0
		string scoreStr = playerScores[playerID].score.ToString().PadLeft(playerScores[playerID].digits.Length, '0');
		
		//数字画像を適用させる / adapt the number image
		int j = 0;
		for (int i = playerScores[playerID].digits.Length - 1; i >= 0; i--) {
			string numStr = scoreStr.Substring(j, 1);
			playerScores[playerID].digits[i].sprite = numbers[int.Parse(numStr)];
			j++;
		}

		//接続人数により勝利コイン枚数判定.
		if (playerScores[playerID].score >= winCoinNums[Network.maxConnections-2]) {
			networkView.RPC("ConclusionFromCoin", RPCMode.AllBuffered, playerID);
		}
	}

	//------------------------------
	//死亡判定（全員のところで発動）.
	//------------------------------
	[RPC]
	void VSDead (int deadPlayerID) {
		playerScores[deadPlayerID].isAlive = false;
		//playerScores[deadPlayerID].isCanContinue = isCanContinue;

		//1人だけ生き残っているか？.
		int alivePlayerID = -1;
		int alivePlayerNum = 0;
		for (int i = 0; i < playerScores.Length; i++) {
			if (playerScores[i].isAlive) {
				alivePlayerID = i;
				alivePlayerNum ++;
			}
		}
		if (alivePlayerNum == 1) {
			//勝敗決定.
			if (alivePlayerID == player.netPlayerID) {
				StartCoroutine("VSConclusionLocal", true);
			} else {
				StartCoroutine("VSConclusionLocal", false);
			}
		} else if (deadPlayerID == player.netPlayerID) {
			//GameController.CheckRecoverInVS(respawnPosition[Random.Range(0, respawnPosition.Length)]);
			if (GameController.CheckRecoverInVS()) {
				GameController.CheckRecoverInVS(respawnPosition[Random.Range(0, respawnPosition.Length)]);
			} else {
				StartCoroutine("LoseNoBalloon");
			}
		}
	}
	//------------------------------
	//復活した（全員の所で発動）.
	//------------------------------
	[RPC]
	void VSAlive (int playerID) {
		playerScores[playerID].isAlive = true;
	}


	//------------------------------
	//負け処理（切断時等／ローカルで呼ばれる）.
	//------------------------------
	public void Lose () {
		StartCoroutine("VSConclusionLocal", false);
	}

	//------------------------------
	//勝ち処理（サーバー役が故意に切断／サーバーから全プレイヤーへ送信される）.
	//------------------------------
	[RPC]
	public void Win () {
		StartCoroutine ("VSConclusionLocal", true);
	}

	//------------------------------
	//１人の回線が切断された（残った全員のところで発動。切断した人はローカルから負け処理を自分で行う）.
	//引数：切断した人のID.
	//------------------------------
	[RPC]
	public void Disconnect (int playerID) {
		//負けた人のフラグを立てる＆残り人数カウント.
		int concPlayerNum = 0;
		for (int i = 0; i < playerScores.Length; i++) {
			if (i == playerID) {
				playerScores[i].isConclusion = true;
			}
			if (playerScores[i].isConclusion) {
				concPlayerNum ++;
			}
		}
		Debug.Log (concPlayerNum);

		//残り1人＝勝敗付いた.
		if (concPlayerNum == Network.maxConnections - 1) {
			StartCoroutine ("VSConclusionLocal", true);
		}
	}

	//------------------------------
	//コインゲットによる勝敗（全員のところで発動、１人が勝ちでそれ以外が負け）.
	//引数：勝った人のID.
	//------------------------------
	[RPC]
	public void ConclusionFromCoin(int winID) {
		if (winID == player.netPlayerID) {
			StartCoroutine ("VSConclusionLocal", true);
		} else {
			StartCoroutine ("VSConclusionLocal", false);
		}
	}

	//------------------------------
	//勝敗処理.
	//引数：ture＝勝利.
	//------------------------------
	private IEnumerator VSConclusionLocal(bool isWin) {
		//もし起動済みなら何もしない.
		if (!isBattleEnd) {
			isBattleEnd = true;
			if (player != null && player.soundBGM != null) {
				player.soundBGM.Stop();
			}
			//移動禁止.
			GameController.isCanMasterInput = false;
			GameController.isCanInput = false;
			
			//勝敗画像表示＆サウンド.
			if (isWin) {
				GameController.gameoverGUI.GetComponent<SpriteRenderer>().sprite = winSprite;
				winSE.Play();
			} else {
				GameController.gameoverGUI.GetComponent<SpriteRenderer>().sprite = loseSprite;
				loseSE.Play();
			}
			GameController.gameoverGUI.SetActive(true);

			//SaveMeボタンか購入催促テキストが表示中かどうかのフラグを持っておく.
			bool isAlreadyLose = (GameController.replayGUI.activeInHierarchy || GameController.continueGUI.activeInHierarchy);

			GameController.replayGUI.SetActive(false);
			GameController.continueGUI.SetActive(false);

			yield return new WaitForSeconds (6.0f);

			//Ａ：「復活できる状態で決着した時、風船催促テキストを表示する（復活できない状態だった場合はテキストを表示せず直接ホームへ）」.
			//とする場合は、下記のコメントを解除し、ＢとＣをコメントアウトしてください.
			/*
			if (GameController.CheckRecoverInVS()) {
				//復活できる場合：催促テキスト表示.
				GameController.gameoverGUI.SetActive(false);
				GameController.continueGUI.SetActive(true);
			} else {
				//復活できない場合：ネットワークをオフにしてホームへ.
				if (Network.isServer) {
					Network.Disconnect();
				}
				if (FASMatchMaking.currentMatch.Status != Match.Statuses.Disposed) {
					FASMatchMaking.DisposeMatch (FASMatchMaking.currentMatch.Id, delegate(Match match, Error error) {});
				}
				Application.LoadLevel ("StartScene");
			}
			*/
			//Ａここまで.

			
			//Ｂ：「事前に負けていたら（＝SaveMeボタンか購入催促テキスト表示中だったら）風船催促テキストを表示する。そうでない場合は直接ホームへ」.
			//とする場合は、下記のコメントを解除し、ＡとＣをコメントアウトしてください.
			/*
			if (isAlreadyLose) {
				GameController.gameoverGUI.SetActive(false);
				GameController.continueGUI.SetActive(true);
			} else {
				if (Network.isServer) {
					Network.Disconnect();
				}
				if (FASMatchMaking.currentMatch.Status != Match.Statuses.Disposed) {
					FASMatchMaking.DisposeMatch (FASMatchMaking.currentMatch.Id, delegate(Match match, Error error) {});
				}
				Application.LoadLevel ("StartScene");
			}
			*/
			//Ｂここまで.

			//Ｃ：「どんな場合でも風船催促テキストを表示する」.
			//とする場合は、下記のコメントを解除し、ＡとＢをコメントアウトしてください.
			GameController.gameoverGUI.SetActive(false);
			GameController.continueGUI.SetActive(true);
			//Ｃここまで.

		}
	}
	
	//------------------------------
	//ゲームオーバー表示.
	//（風船が無い状態で死んだ場合）.
	//------------------------------
	private IEnumerator LoseNoBalloon() {
		GameController.gameoverGUI.SetActive(true);

		yield return new WaitForSeconds (6.0f);

		if (!Network.isServer) {
			GameController.gameoverGUI.SetActive(false);
			GameController.continueGUI.SetActive(true);
		}
	}
}

[System.Serializable]
public class NetworkPlayerScore {
	public string name;
	public int score;
	public GameObject parentObj;
	public SpriteRenderer[] digits;
	public bool isAlive = true;	//生存しているか.
	public bool isConclusion = false;	//勝敗決定済みか.
	public bool isCanContinue = false;	//課金による復活可能か.
}