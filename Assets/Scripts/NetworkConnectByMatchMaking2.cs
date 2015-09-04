using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Models;
using Fresvii.AppSteroid.Util;
using Fresvii.AppSteroid.Gui;

public class NetworkHostInformation
{
    public bool userNat;

    public string guid = "";

    public string ip = "";

    public int port;

    public User user;
}

public class NetworkConnectByMatchMaking2 : MonoBehaviour
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

    //public Text logText;

    private bool connected = false;

    public Button buttonBack;

    private NetworkHostInformation networkHostInformation;

    private int retry = 0;

    public int retryLimit = 5;

    private bool isServer;

    public float timeoutSeconds = 30f;

    // Use this for initialization
    void Start()
    {
        currentMatch = FASMatchMaking.currentMatch;

        Debug.Log("FASMatchMaking.currentMatch " + FASMatchMaking.currentMatch);

        Debug.Log("Match " + currentMatch.Id);

        if (currentMatch.Players[0].User.Id == FAS.CurrentUser.Id)
        {
            isServer = true;

            Debug.Log("#### This is Server ####");

            //logText.text += "\n#### This is Server ####\n";

            LaunchServer();
        }
        else
        {
            Debug.Log("#### This is Client ####");

            //logText.text += "\n#### This is Client ####\n";
        }

        StartCoroutine("PollingGetGameContext");

        Application.RegisterLogCallback(LogCallback);

        StartCoroutine(Timeout());
    }

    IEnumerator Timeout()
    {
        yield return new WaitForSeconds(timeoutSeconds);

        DialogManager.Instance.SetLabel("Close", "Close", "Close");

        DialogManager.Instance.ShowSubmitDialog("Network connection error", (del) => 
        {        
        
        });

        OnClickBackButton();
    }

    private void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error)
        {
            // Check if it is the NATPunchthroughFailed error 
            const string MessageBeginning = "Receiving NAT punchthrough attempt from target";

            if (condition.StartsWith(MessageBeginning, StringComparison.Ordinal))
            {
                // Call the callback that Unity should be calling.
                OnFailedToConnect(NetworkConnectionError.NATPunchthroughFailed);
            }
        }
    }

    void OnEnable()
    {
        FASEvent.OnMatchMakingGameContextCreated += OnMatchMakingGameContextCreated;
    }

    void OnDisable()
    {
        FASEvent.OnMatchMakingGameContextCreated -= OnMatchMakingGameContextCreated;
    }

	void LaunchServer()
	{
	    Debug.Log("LaunchServer");

	    Network.incomingPassword = FAS.Instance.Uuid;
		
        useNat = !Network.HavePublicAddress();

		connectedPlayerNum = 1;
		
        Network.InitializeServer(currentMatch.Players.Count, port, useNat);
	}

    void OnFailedToConnectToMasterServer(NetworkConnectionError info)
    {
        retry++;

        Debug.Log("Could not connect to master server: " + info);

        //logText.text += "#### OnFailedToConnectToMasterServer\n";

        //logText.text += " Could not connect to master server: \n" + info + "\n";

        if (retry < retryLimit && isServer)
        {
            LaunchServer();
        }
        else
        {
            DialogManager.Instance.SetLabel("Close", "Close", "Close");

            DialogManager.Instance.ShowSubmitDialog("Network connection error", (del) => { });

            OnClickBackButton();
        }
    }

	void OnServerInitialized()
	{
        Debug.Log("OnServerInitialized");

        //logText.text += "#### OnServerInitialized\n";

        connected = true;

        Dictionary<string, string> networkSetting = new Dictionary<string, string>();

        networkSetting.Add("Type", "HostData");

        string ip = Network.player.ipAddress;

        if (useNat)
        {
            networkSetting.Add("guid", Network.player.guid);

            Debug.Log("Server guid: " + Network.player.guid);

            //logText.text += "Server guid: " + Network.player.guid;

            //logText.text += "\n";
        }
        else
        {
            networkSetting.Add("ip", ip);

            Debug.Log("Server IP: " + ip);

            //logText.text += "Server IP: " + ip;

            //logText.text += "\n";

            networkSetting.Add("port", Network.player.port.ToString());

            Debug.Log("Server port: " + Network.player.port);

            //logText.text += "Server port: " + Network.player.port;

            //logText.text += "\n";
        }

        string json = Json.Serialize(networkSetting);

        FASMatchMaking.UpdateGameContext(currentMatch.Id, json, delegate(GameContext gameContext, Error error)
        {
            if (error == null)
            {
                OnMatchMakingGameContextCreated(gameContext);
            }
            else
            {
                DialogManager.Instance.ShowSubmitDialog("Error", (del) => { });
            }
        });
	}

    void OnConnectedToServer()
    {
        connected = true;

        Debug.Log("OnConnectedToServer");

        //logText.text += "#### OnConnectedToServer\n";

        Dictionary<string, string> clientConnectedMessage = new Dictionary<string, string>();
    }

	void OnFailedToConnect(NetworkConnectionError error) 
	{
		Debug.Log("Could not connect to server: " + error);

        //logText.text += "#### OnFailedToConnect\n";

        //logText.text += " Could not connect to server: " + error + "\n";

        //logText.text += " Reconnect... \n";

        retry++;

        if (networkHostInformation != null && retry < retryLimit)
        {
            if (networkHostInformation.userNat)
            {
                Network.Connect(networkHostInformation.guid, FAS.Instance.Uuid); // useNat
            }
            else
            {
                Network.Connect(networkHostInformation.ip, networkHostInformation.port, FAS.Instance.Uuid);
            }
        }
        else
        {
            DialogManager.Instance.SetLabel("Close", "Close", "Close");

            DialogManager.Instance.ShowSubmitDialog("Network connection error", (del) => { });

            OnClickBackButton();
        }
	}

	// プレイヤーがサーバーに接続したとき、サーバー側で呼び出されます.
	void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log("Connected from " + player.ipAddress + ":" + player.port);

        //logText.text += "#### OnPlayerConnected\n";

        //logText.text += " Connected from " + player.ipAddress + ":" + player.port + "\n";
        
        connectedPlayerNum++;

        //logText.text += " Players : " + connectedPlayerNum + " / " + FASMatchMaking.currentMatch.Players.Count + "\n";

        if (connectedPlayerNum == FASMatchMaking.currentMatch.Players.Count) 
        {
            Debug.Log("Go To Battle Scene!");

            //logText.text += "#### Go to Battle Scene!! \n";

		    //ステージを決定.
		    BATTLE_SCENE_NAME = battleSceneNames[UnityEngine.Random.Range(0, battleSceneNames.Length)];

            //logText.text += " Scene name is " + BATTLE_SCENE_NAME + "\n";

		    //全員揃ったらシーン移動.
		    Dictionary<string, string> networkSetting = new Dictionary<string, string>();

		    networkSetting.Add("Type", "SceneMove");
		
            networkSetting.Add("BattleScene", BATTLE_SCENE_NAME);
		
            MakeContext (networkSetting);

            Network.maxConnections = currentMatch.Players.Count;

            Application.LoadLevel(BATTLE_SCENE_NAME);
         }
    }

	void OnMatchMakingGameContextCreated(GameContext gameContext)
	{
	    if (latestGameContext == null)
	    {
	        latestGameContext = gameContext;
	    }
	    else if (latestGameContext.UpdatedCount == gameContext.UpdatedCount)
		{
	        return;
	    }
	    else
	    {
	        latestGameContext = gameContext;
	    }

        //if(logText != null)
        //    logText.text += "#### OnMatchMakingGameContextCreated\n";

		IDictionary valueDict = (IDictionary)gameContext.Value;

        if (valueDict.Contains("Type"))
        {
            Debug.Log("has type : " + (string)valueDict["Type"]);

            if ((string)valueDict["Type"] == "HostData" && gameContext.UpdatedBy.Id != FAS.CurrentUser.Id)
            {
                networkHostInformation = new NetworkHostInformation();

                if (valueDict.Contains("guid"))
                {
                    Debug.Log((string)valueDict["guid"]);

                    networkHostInformation.userNat = true;

                    networkHostInformation.guid = (string)valueDict["guid"];

                    networkHostInformation.user = gameContext.UpdatedBy;

                    Network.Connect(networkHostInformation.guid, FAS.Instance.Uuid); // useNat
                }
                else
                {
                    if (valueDict.Contains("ip") && valueDict.Contains("port"))
                    {
                        Debug.Log((string)valueDict["ip"]);

                        Debug.Log((string)valueDict["port"]);

                        networkHostInformation.ip = (string)valueDict["ip"];

                        networkHostInformation.port = int.Parse((string)valueDict["port"]);

                        networkHostInformation.userNat = false;

                        networkHostInformation.user = gameContext.UpdatedBy;

                        Network.Connect(networkHostInformation.ip, networkHostInformation.port, FAS.Instance.Uuid);
                    }
                }

                //logText.text += "Host user is " + networkHostInformation.user.Name + "\n";
            }
			else if((string)valueDict["Type"] == "SceneMove") 
            {
                //if(logText != null)
                //    logText.text += "#### Load the Battle Scene!! \n";

				BATTLE_SCENE_NAME = (string)valueDict["BattleScene"];
			
                Network.maxConnections = currentMatch.Players.Count;
				
                Application.LoadLevel(BATTLE_SCENE_NAME);
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

    void OnDisconnectedFromServer()
    {
        connected = false;

        Debug.Log("OnDisconnectedFromServer");

        //logText.text += "#### OnDisconnectedFromServer\n";
    }

	//コンテキスト作成.
	private void MakeContext (Dictionary<string, string> dic) 
    {
		string json = Json.Serialize(dic);
	
        FASMatchMaking.UpdateGameContext(FASMatchMaking.currentMatch.Id, json, delegate(GameContext gameContext, Error error) {
			if (error == null) {
				OnMatchMakingGameContextCreated(gameContext);
			} else {
				DialogManager.Instance.ShowSubmitDialog("Error", (del)=>{});
			}
		});
	}

    public void OnClickBackButton()
    {
        if (connected)
        {
            Network.Disconnect();
        }

        Application.LoadLevel("StartScene");
    }
}