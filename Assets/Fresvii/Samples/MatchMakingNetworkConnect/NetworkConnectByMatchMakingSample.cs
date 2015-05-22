using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using Fresvii.AppSteroid;



namespace Fresvii.AppSteroid.SampleGame
{
    public class NetworkHostInformation
    {
        public bool userNat;

        public string guid = "";

        public string ip = "";

        public int port;

        public Fresvii.AppSteroid.Models.User user;
    }

    public class NetworkConnectByMatchMakingSample : MonoBehaviour
    {
        private Fresvii.AppSteroid.Models.Match currentMatch;

        public int port = 4211;

		public int playerLimit = 2;

        private Fresvii.AppSteroid.Models.GameContext latestGameContext;

		private bool useNat = false;

        public Text logText;

        public Button buttonDisconnect;

        public Button buttonBack;

        private NetworkHostInformation networkHostInformation;
        
        // Use this for initialization
        void Start()
        {
            currentMatch = FASMatchMaking.currentMatch;

            Debug.Log("Match " + currentMatch.Id);

            if (currentMatch.Players[0].User.Id == FAS.CurrentUser.Id)
            {
                Debug.Log("#### This is Server ####");

                logText.text += "\n#### This is Server ####\n";
                
                LaunchServer();
            }
            else
            {
                Debug.Log("#### This is Client ####");

                logText.text += "\n#### This is Client ####\n";
			}

            StartCoroutine("PollingGetGameContext");
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

            logText.text += "#### LaunchServer\n";

            Network.incomingPassword = FASConfig.Instance.appId;

			useNat = !Network.HavePublicAddress();

			Network.InitializeServer(playerLimit, port, useNat);
        }

        void OnServerInitialized()
        {
            Debug.Log("OnServerInitialized");

            logText.text += "#### OnServerInitialized\n";

            logText.text += "Connected\n";

            buttonDisconnect.interactable = true;

            Dictionary<string, string> networkSetting = new Dictionary<string, string>();

            networkSetting.Add("Type", "HostData");

			string ip = Network.player.ipAddress;

            if (useNat)
            {
                networkSetting.Add("guid", Network.player.guid);

                Debug.Log("Server guid: " + Network.player.guid);

                logText.text += "Server guid: " + Network.player.guid;

                logText.text += "\n";
            }
            else
            {
                networkSetting.Add("ip", ip);

                Debug.Log("Server IP: " + ip);

                logText.text += "Server IP: " + ip;
                
                logText.text += "\n";

                networkSetting.Add("port", Network.player.port.ToString());

                Debug.Log("Server port: " + Network.player.port);

                logText.text += "Server port: " + Network.player.port;

                logText.text += "\n";
            }

			string json = Fresvii.AppSteroid.Util.Json.Serialize(networkSetting);

            FASMatchMaking.UpdateGameContext(currentMatch.Id, json, delegate(Fresvii.AppSteroid.Models.GameContext gameContext, Fresvii.AppSteroid.Models.Error error)
            {
                if (error == null)
                {
                    OnMatchMakingGameContextCreated(gameContext);
                }
                else
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Error", (del)=>{});
                }
            });
        }

        void OnConnectedToServer()
        {
            buttonDisconnect.interactable = true;

            Debug.Log("OnConnectedToServer");

            logText.text += "#### OnConnectedToServer\n";
        
            logText.text += "Connected\n";

            Dictionary<string, string> clientConnectedMessage = new Dictionary<string, string>();

            clientConnectedMessage.Add("Type", "ClientConnected");

            string json = Fresvii.AppSteroid.Util.Json.Serialize(clientConnectedMessage);

            Debug.Log(json);

            FASMatchMaking.UpdateGameContext(currentMatch.Id, json, delegate(Fresvii.AppSteroid.Models.GameContext gameContext, Fresvii.AppSteroid.Models.Error error)
            {
                if (error == null)
                {
                    OnMatchMakingGameContextCreated(gameContext);
                }
                else
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Error", (del) => { });
                }
            });
        }

		void OnFailedToConnect(NetworkConnectionError error) 
		{
			Debug.Log("Could not connect to server: " + error);

            logText.text += "#### OnFailedToConnect\n";

            logText.text += "Could not connect to server: " + error + "\n";

            logText.text += "Reconnect... \n";

            if (networkHostInformation != null)
            {
                if (networkHostInformation.userNat)
                {
                    Network.Connect(networkHostInformation.guid, FASConfig.Instance.appId); // useNat
                }
                else
                {
                    Network.Connect(networkHostInformation.ip, networkHostInformation.port, FASConfig.Instance.appId);
                }
            }
		}

        void OnMatchMakingGameContextCreated(Fresvii.AppSteroid.Models.GameContext gameContext)
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

            Debug.Log("OnMatchMakingGameContextCreated " + gameContext.UpdatedBy.Name);

            logText.text += "#### OnMatchMakingGameContextCreated\n";

            IDictionary valueDict = (IDictionary)gameContext.Value;

            if (valueDict.Contains("Type"))
            {
                Debug.Log("has type : " + (string)valueDict["Type"]);

                if((string)valueDict["Type"] == "HostData" && gameContext.UpdatedBy.Id != FAS.CurrentUser.Id)
                {
                    networkHostInformation = new NetworkHostInformation();

					if (valueDict.Contains("guid"))
					{
						Debug.Log((string)valueDict["guid"]);
                        
                        networkHostInformation.userNat = true;

                        networkHostInformation.guid = (string)valueDict["guid"];

                        networkHostInformation.user = gameContext.UpdatedBy;

                        Network.Connect(networkHostInformation.guid, FASConfig.Instance.appId); // useNat
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

                            Network.Connect(networkHostInformation.ip, networkHostInformation.port, FASConfig.Instance.appId);
						}
					}

                    logText.text += "Host user is " + networkHostInformation.user.Name + "\n";
                }
                else if((string)valueDict["Type"] == "ClientConnected")
                {
                    Debug.Log("ClientConnected" + gameContext.UpdatedBy.Name);

                    logText.text += "Client connected : " + gameContext.UpdatedBy.Name + "\n";
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
                    FASMatchMaking.GetGameContext(FASMatchMaking.currentMatch.Id, delegate(Fresvii.AppSteroid.Models.GameContext gameContext, Fresvii.AppSteroid.Models.Error error)
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
            buttonDisconnect.interactable = false;

            Debug.Log("OnDisconnectedFromServer");

            logText.text += "#### OnDisconnectedFromServer\n";
        }

        public void OnClickDisconnectButton()
        {
            FASMatchMaking.DisposeMatch(FASMatchMaking.currentMatch.Id, delegate(Fresvii.AppSteroid.Models.Match match, Fresvii.AppSteroid.Models.Error error) { });

            Network.Disconnect();
        }

        public void OnClickBackButton()
        {
            Application.LoadLevel(0);
        }
    }
}