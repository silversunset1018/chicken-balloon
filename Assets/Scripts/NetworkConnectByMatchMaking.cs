using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Models;
using Fresvii.AppSteroid.Util;
using Fresvii.AppSteroid.Gui;

public class NetworkConnectByMatchMaking : MonoBehaviour
{
	public int port = 4211;
	public string[] battleSceneNames;

	public static int MATCH_PLAYER_NUMBER_MIN = 2;
	public static int MATCH_PLAYER_NUMBER_MAX = 4;
	public static string BATTLE_SCENE_NAME = "";

	private Match currentMatch;
	private List<string> userIDs;
	private string serverID;
	private GameContext latestGameContext;
	private bool useNat = false;
	private int connectedPlayerNum;
	private int connectionErrorNum;	//接続時にエラーが発生した時、タイトルに戻り済みのプレイヤーの数.
	private bool isConnectionError;	//接続時にエラーが発生した時、タイトルに戻り済みか.
	private bool isConnectionCountDown;	//3名以上の接続待ち.
	private const float COUNT_DOWN_TIME = 30.0f;
	private float connectionCountDownTimer;
	private MatchMakingRequest matchMakingRequest;
	private bool isMatchComplete;

	void OnEnable()
	{
		FASEvent.OnMatchMakingGameContextCreated += OnMatchMakingGameContextCreated;
		FASEvent.OnMatchMakingMatchCompleted += OnMatchMakingMatchCompleted;
	}

	void OnDisable()
	{
		FASEvent.OnMatchMakingGameContextCreated -= OnMatchMakingGameContextCreated;
		FASEvent.OnMatchMakingMatchCompleted -= OnMatchMakingMatchCompleted;
	}

	void Start() {
		//StartCoroutine(MatchMakingGetMatchRequestPolling());
	}

	void Update() {
		if (FASMatchMaking.currentMatch == null) {
			if (isConnectionCountDown) {
				if (connectionCountDownTimer < 0.0f) {
					OnMatchMakingMatchCompleted(currentMatch);
				} else {
					connectionCountDownTimer -= Time.deltaTime;
				}
			} else if (currentMatch != null && currentMatch.Players.Count >= MATCH_PLAYER_NUMBER_MIN) {
				connectionCountDownTimer = COUNT_DOWN_TIME;
				isConnectionCountDown = true;
			}
		}
	}

	void LaunchServer()
	{
	    Debug.Log("LaunchServer");

	    Network.incomingPassword = FAS.Instance.Uuid;
		useNat = !Network.HavePublicAddress();

		connectedPlayerNum = 1;
		Network.InitializeServer(currentMatch.Players.Count, port, useNat);
	}

	void OnServerInitialized()
	{
	    Dictionary<string, string> networkSetting = new Dictionary<string, string>();
	    networkSetting.Add("Type", "HostData");
		string ip = Network.player.ipAddress;

	    if (useNat)
	    {
	        networkSetting.Add("guid", Network.player.guid);
	        Debug.Log("Server guid: " + Network.player.guid);
	    }
	    else
	    {
	        networkSetting.Add("ip", ip);
	        Debug.Log("Server IP: " + ip);
	        networkSetting.Add("port", Network.player.port.ToString());
	        Debug.Log("Server port: " + Network.player.port);
		}

		//ユーザーIDを明記.
		if (userIDs == null) {
			userIDs = new List<string>();
			for (int i = 0; i < FASMatchMaking.currentMatch.Players.Count; i++) {
				userIDs.Add(FASMatchMaking.currentMatch.Players[i].User.Id);
			}
		}
		for (int i = 0; i < userIDs.Count; i++) {
			networkSetting.Add("user_id"+i, userIDs[i]);
			if (userIDs[i] == FAS.CurrentUser.Id) {
				networkSetting.Add("ServerID", i.ToString());
			}
		}

		MakeContext (networkSetting);
	}

	void OnFailedToConnect(NetworkConnectionError error) 
	{
		Debug.Log("Could not connect to server: " + error);
		//エラーをコール.
		Dictionary<string, string> networkSetting = new Dictionary<string, string>();
		networkSetting.Add("Type", "ConnectionError");
		networkSetting.Add("Message", error.ToString());
		MakeContext (networkSetting);
	}

	// プレイヤーがサーバーに接続したとき、サーバー側で呼び出されます.
	void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log("Connected from " + player.ipAddress + ":" + player.port);
		connectedPlayerNum ++;
		if (connectedPlayerNum < FASMatchMaking.currentMatch.Players.Count) {
			return;
		}

		//ステージを決定.
		BATTLE_SCENE_NAME = battleSceneNames[Random.Range(0, battleSceneNames.Length)];

		//全員揃ったらシーン移動.
		Dictionary<string, string> networkSetting = new Dictionary<string, string>();
		networkSetting.Add("Type", "SceneMove");
		networkSetting.Add("BattleScene", BATTLE_SCENE_NAME);
		MakeContext (networkSetting);
	}

	void OnMatchMakingGameContextCreated(GameContext gameContext)
	{
	    if (latestGameContext == null)
	    {
	        latestGameContext = gameContext;
	    }
	    else if (latestGameContext.UpdatedCount == gameContext.UpdatedCount)
		{
			IDictionary errorDict = (IDictionary)gameContext.Value;
			if (errorDict.Contains("Type") && (string)errorDict["Type"] == "ConnectionError") {
				ConnectionError(errorDict);
			}
			//Debug.Log("latestGameContext " + errorDict["Message"] +" "+errorDict["Type"]);
	        return;
	    }
	    else
	    {
	        latestGameContext = gameContext;
	    }

		IDictionary valueDict = (IDictionary)gameContext.Value;

	    if (valueDict.Contains("Type"))
		{
			//サーバーへ接続.
			Debug.Log(valueDict["Type"] + " " +gameContext.UpdatedBy.Id +" "+FAS.CurrentUser.Id);
	        if((string)valueDict["Type"] == "HostData") {
				//ユーザーID一覧更新.
				if (userIDs == null) {
					userIDs = new List<string>();
					for (int i = 0; i < FASMatchMaking.currentMatch.Players.Count; i++) {
						userIDs.Add( (string)valueDict["user_id"+i]);
					}
				}
				serverID = (string)valueDict["ServerID"];

				//自分が送ったのではない場合、クライアントとして接続.
			   if (gameContext.UpdatedBy.Id != FAS.CurrentUser.Id) {
					if (valueDict.Contains("guid"))	{
						Debug.Log((string)valueDict["guid"]);
						string guid = (string)valueDict["guid"];
						Network.Connect(guid, FAS.Instance.Uuid); // useNat
					} else {
						if (valueDict.Contains("ip") && valueDict.Contains("port"))
						{
							Debug.Log((string)valueDict["ip"]);
							Debug.Log((string)valueDict["port"]);
							string ip = (string)valueDict["ip"];
							int port = int.Parse((string)valueDict["port"]);
							Network.Connect(ip, port, FAS.Instance.Uuid);
						}
					}
	       		}
			}

			//シーンの移動.
			if((string)valueDict["Type"] == "SceneMove") {
				BATTLE_SCENE_NAME = (string)valueDict["BattleScene"];
				Network.maxConnections = currentMatch.Players.Count;
				Application.LoadLevel(BATTLE_SCENE_NAME);
			}

			//接続エラー.
			if ((string)valueDict["Type"] == "ConnectionError") {
				ConnectionError(valueDict);
			}
	    }
	}

	//接続エラー時の処理.
	private void ConnectionError(IDictionary valueDict) {
		if (isConnectionError) {
			return;
		}

		//サーバーになっていたのが最後の1人だったら終了処理.
		if (serverID == (FASMatchMaking.currentMatch.Players.Count - 1).ToString()) { 
			if(Network.isServer) {
				if (connectionErrorNum == Network.maxConnections - 1) {
					isConnectionError = true;
					//全員退室済み：タイトルへ戻る.
					FASMatchMaking.DisposeMatch(FASMatchMaking.currentMatch.Id, delegate(Match match, Error error) { });
					Network.Disconnect();
					Application.LoadLevel ("StartScene");
				} else {
					connectionErrorNum ++;
				}
			} else {
				isConnectionError = true;
				Dictionary<string, string> networkSetting = new Dictionary<string, string>();
				networkSetting.Add("Type", "ConnectionError");
				networkSetting.Add("Message", valueDict["Message"].ToString());
				MakeContext (networkSetting);
				Application.LoadLevel ("StartScene");
			}
		//サーバーになっていない人がまだ居るならその人をサーバーに.
		} else {
			if (Network.isServer) {
				Network.Disconnect();
			}
			int server_id = int.Parse(serverID) + 1;
			if (userIDs[server_id] == FAS.CurrentUser.Id) {
				Debug.Log("Next Server "+server_id);
				LaunchServer();
			}
		}
	}

	public float pollingInterval = 3f;
	IEnumerator PollingGetGameContext()
	{
	    while (this.enabled)
	    {
	        if (FASMatchMaking.currentMatch != null)
	        {
	            FASMatchMaking.GetGameContext(FASMatchMaking.currentMatch.Id, delegate(GameContext gameContext, Error error)
	            {
	                if (error == null)
	                {
	                    OnMatchMakingGameContextCreated(gameContext);
	                }
	            });
	        }

	        yield return new WaitForSeconds(pollingInterval);
	    }
	}

	void OnMatchMakingMatchCompleted(Match match)
	{
		if(FASMatchMaking.currentMatch != null)
		{
			//if(FASMatchMaking.currentMatch.Id == match.Id)
				//return;
		}
		FASMatchMaking.currentMatch = match;
		currentMatch = match;
		
		Debug.Log(FASMatchMaking.currentMatch.Players.Count);

		if (currentMatch.Players [0].User.Id == FAS.CurrentUser.Id) {
			Debug.Log ("#### This is Server ####");
			LaunchServer ();
		} else {
			Debug.Log ("#### This is Client ####");
		}
		StartCoroutine ("PollingGetGameContext");

		Debug.Log ("MatchMaking Completed!");
	}

	//コンテキスト作成.
	private void MakeContext (Dictionary<string, string> dic) {
		string json = Json.Serialize(dic);
		FASMatchMaking.UpdateGameContext(FASMatchMaking.currentMatch.Id, json, delegate(GameContext gameContext, Error error) {
			if (error == null) {
				OnMatchMakingGameContextCreated(gameContext);
			} else {
				DialogManager.Instance.ShowSubmitDialog("Error", (del)=>{});
			}
		});
	}

	IEnumerator MatchMakingGetMatchRequestPolling()
	{
		while(this.gameObject.activeInHierarchy && matchMakingRequest == null)
		{
			FASMatchMaking.GetMatchMakingRequest(OnGetMatchMakingRequest);
			yield return new WaitForSeconds(pollingInterval);
		}
	}

	private void OnGetMatchMakingRequest(MatchMakingRequest _matchMakingRequest, Error error)
	{
		if (error == null)
		{
			this.matchMakingRequest = _matchMakingRequest;
			Debug.Log(matchMakingRequest);
			StartCoroutine(MatchMakingGetMatchPolling());
		}
	}

	IEnumerator MatchMakingGetMatchPolling()
	{
		while(this.gameObject.activeInHierarchy && FASMatchMaking.currentMatch == null)
		{
			// GetMatch
			FASMatchMaking.GetMatch(matchMakingRequest.Match.Id, delegate(Match match, Error error) {
				currentMatch = match;
			});
			yield return new WaitForSeconds(pollingInterval);
		}
	}
}