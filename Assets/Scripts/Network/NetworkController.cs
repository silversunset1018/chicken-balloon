using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Fresvii.AppSteroid.Gui;
using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Models;
using Fresvii.AppSteroid.Util;

public class NetworkController : MonoBehaviour {
	public GameObject[] playerObjs;
	public VSResult vsResult;
	
	public Animation blackChickenAnimation;
	public static Animation BLACK_CHICKEN_ANIMATION;
	public Animation cuctusAnimation;
	public static Animation CUCTUS_ANIMATION;

	private FresviiGUIMatchMaking matchGUI;
	private List<NetworkPlayer> netPlayers;

	public Vector3[] startCameraPositions;

	private const float MAIN_CAMERA_POSITION_Y = 32.0f;	//プレイヤー3と4の初期カメラ位置.
	private const float SCENE_MOVE_WAIT = 3.0f;	//シーン確認の頻度（秒）.

	// Use this for initialization
	void Awake() {
		//ブラックチキンセット.
		BLACK_CHICKEN_ANIMATION = blackChickenAnimation;
		CUCTUS_ANIMATION = cuctusAnimation;
	}

	void Start () {
		netPlayers = new List<NetworkPlayer> ();
		if (Network.isServer) {
			netPlayers.Add(Network.player);	//最初の1人（＝自分）は普通に入れる.
			InvokeRepeating ("CallPlayerScene", SCENE_MOVE_WAIT, SCENE_MOVE_WAIT);
		}
		
	//画面縦横比確認.
	int sw = Screen.width;
	int sh = Screen.height;
	
	//2:3
		float stmp = (sh / 2.0f * 3.0f);
		if (stmp == sw) {
			startCameraPositions[0].x -= 1.5f;
			startCameraPositions[1].x += 1.5f;
			startCameraPositions[2].x -= 1.5f;
			startCameraPositions[3].x += 1.5f;
		}
		
	//3:4
		stmp = (int)(sh / 3.0f * 4.0f);
		if (stmp == sw) {
			startCameraPositions[0].x -= 2.4f;
			startCameraPositions[1].x += 2.4f;
			startCameraPositions[2].x -= 2.4f;
			startCameraPositions[3].x += 2.4f;
						}

        Application.RegisterLogCallback(LogCallback);
    }

    private void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error)
        {
            // Check if it isRPC invoke error
            const string MessageBeginning = "Could't invoke RPC function";

            if (condition.StartsWith(MessageBeginning, System.StringComparison.Ordinal))
            {
                DialogManager.Instance.SetLabel("Close", "Close", "Close");

                DialogManager.Instance.ShowSubmitDialog("Unity - Could't invoke RPC function", (del) => 
                {
                    Application.LoadLevel("StartScene");
                });

                DisconnectFromApplication();
            }
        }
    }

	//シーン遷移が完了したか確認.
	//サーバー側：定期的に確認する.
	void CallPlayerScene() {

		networkView.RPC ("NowSceneForClient", RPCMode.OthersBuffered);
	}

	//クライアント側：コールを受けて投げ返す.
	[RPC]
	void NowSceneForClient() {
		networkView.RPC ("NowSceneForServer", RPCMode.Server, Application.loadedLevelName);
	}

	//サーバー側：クライアントからコールを投げ返される.
	[RPC]
	void NowSceneForServer (string scene_name, NetworkMessageInfo info) {
		//バトルシーンに到達していたらプレイヤーリストに入れる.
		if (scene_name == NetworkConnectByMatchMaking2.BATTLE_SCENE_NAME) {
			if (!netPlayers.Contains(info.sender)) {
				netPlayers.Add(info.sender);
			}
		}

		//全員揃っていたらプレイヤー生成.
		if (netPlayers.Count == Network.maxConnections) {
			MakePlayer(0);
			if (Network.connections.Length >= 1) { networkView.RPC ("MakePlayer", Network.connections [0], 1);	}
			if (Network.connections.Length >= 2) { networkView.RPC ("MakePlayer", Network.connections [1], 2); }
			if (Network.connections.Length >= 3) { networkView.RPC ("MakePlayer", Network.connections [2], 3); }
		}
		CancelInvoke ();
	}

	[RPC]
	void MakePlayer (int playerID) {
		//プレイヤー生成.
		Vector3 pos = Vector3.zero;
		if (playerID == 2 || playerID == 3) {
			if (Application.loadedLevelName == "VSScene 1") {
				pos.y = 7.0f;
			}
			if (Application.loadedLevelName == "VSScene 2") {
				pos.y = 7.0f;
			}
			if (Application.loadedLevelName == "VSScene 3") {
				pos.y = 7.0f;
			}
			if (Application.loadedLevelName == "VSScene 4") {
				pos.y = 7.0f;
			}
			if (Application.loadedLevelName == "VSScene 5") {
				pos.y = 7.0f;
			}
			if (Application.loadedLevelName == "VSScene 6") {
				pos.y = 7.0f;
			}
		}
		GameObject obj = Network.Instantiate(playerObjs[playerID], pos, Quaternion.identity, 0) as GameObject;
		obj.SetActive(true);
		GameController.player = obj.GetComponentInChildren<PlayerController>();
		GameController.player.netPlayerID = playerID;

		Camera.main.transform.position = startCameraPositions[playerID];
		Camera.main.GetComponent<CameraController>().InitSet();
	}

	//プレイヤーが切断されたとき、サーバー側で呼び出されます.
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Clean up after player " + player);
		Network.CloseConnection(player, true);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);

		vsResult.networkView.RPC("Disconnect", RPCMode.AllBuffered, netPlayers.IndexOf (player));
	}
	
	//サーバーから切断したとき、クライアント側で呼び出されます.
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer)
		{  
			Debug.Log("Local server connection disconnected");
		}
		else if (info == NetworkDisconnection.LostConnection)
		{
			Debug.Log("Lost connection to the server");
			vsResult.Lose();
		}
		else
		{
			Debug.Log("Successfully diconnected from the server");
		}
	}

	//バックグラウンド移行の検知（移行したら切断扱い＝負け）.
	void OnApplicationPause (bool pauseStatus) {
		DisconnectFromApplication ();
	}
	void OnApplicationQuit () {
		DisconnectFromApplication ();
	}
	void OnApplicationFocus (bool focusStatus) {
		DisconnectFromApplication ();
	}
	private void DisconnectFromApplication() {
		if (Application.platform == RuntimePlatform.WindowsEditor) {
			return;
		}

		if (Network.isClient) {
			Network.Disconnect();
			vsResult.Lose();
		} else if (Network.isServer) {
			vsResult.networkView.RPC("Win", RPCMode.OthersBuffered);
			vsResult.Lose();	//この先でServer.Discconectしている.
		}
	}
}
