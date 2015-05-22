using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;



namespace Fresvii.AppSteroid.SampleGame
{
    public class TicTacToe : MonoBehaviour
    {
        private enum Players { None, Player1, Player2 }

        private Players turn = Players.Player1;

        private enum Mode { Game, Done, Quit }

        public GameObject prfbCircle;

        public GameObject prfbCross;

        public Square[] squares;

        public LayerMask layer;

        private Fresvii.AppSteroid.Models.User turnUser; 

        private GameObject currentPiece;

        private GameObject piecies;

        private Players[][] board;

        private int gameCount = 0;

        public TextMesh message;

        private Mode mode = Mode.Game;

        public float pollingInterval = 5f;

        private Fresvii.AppSteroid.Models.GameContext latestGameContext = null;

        private Fresvii.AppSteroid.Models.Match currentMatch;

        private bool polling;

        public TextMesh tmPlayer1, tmPlayer2;

        // Use this for initialization
        void Start()
        {
            currentMatch = FASMatchMaking.currentMatch;

            mode = Mode.Game;

            if (piecies != null)
            {
                Destroy(piecies);
            }

            piecies = new GameObject("Pieces");

            board = new Players[3][];

            for (int i = 0; i < 3; i++)
            {
                board[i] = new Players[3];
            }

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    board[x][y] = Players.None;

            gameCount = 0;

            ChangeTurn();

            if (!polling)
                StartCoroutine(PollingGetGameContext());

            tmPlayer1.text = currentMatch.Players[0].User.Name + ((currentMatch.Players[0].User.Id == FAS.CurrentUser.Id) ? " (You)" : "");

            tmPlayer2.text = currentMatch.Players[1].User.Name + ((currentMatch.Players[1].User.Id == FAS.CurrentUser.Id) ? " (You)" : "");
        }

        void OnEnable()
        {
            FASEvent.OnMatchMakingGameContextCreated += OnMatchMakingGameContextCreated;
        }

        void OnDisable()
        {
            FASEvent.OnMatchMakingGameContextCreated -= OnMatchMakingGameContextCreated;
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

            IDictionary valueDict = (IDictionary)gameContext.Value;

            if (valueDict.Contains("x") && valueDict.Contains("y"))
            {
                int x = (int)((long)valueDict["x"]);

                int y = (int)((long)valueDict["y"]);

                board[x][y] = turn;
            
                gameCount++;

                currentPiece.transform.position = squares[x + y * 3].transform.position;

                currentPiece.GetComponent<Renderer>().enabled = true;

                currentPiece.GetComponent<Renderer>().material.color = new Color(currentPiece.GetComponent<Renderer>().material.color.r, currentPiece.GetComponent<Renderer>().material.color.g, currentPiece.GetComponent<Renderer>().material.color.b, 1.0f);

                if (CheckWin())
                {
                    message.text = gameContext.UpdatedBy.Name + " Win!";

                    mode = Mode.Done;
                }
                else if (gameCount == 9)
                {
                    mode = Mode.Done;

                    message.text = "Draw";
                }
                else
                {
                    ChangeTurn();
                }
            }
            else if (valueDict.Contains("quit"))
            {
                mode = Mode.Quit;
            }
            else if (valueDict.Contains("restart"))
            {
                Start();
            }
        }

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

            polling = false;
        }

        // Update is called once per frame
        void Update()
        {           
            #region Game            
            if (mode == Mode.Game)
            {
                if (turnUser.Id == FAS.CurrentUser.Id) // Me
                {
                    message.text = "Your turn";

                    if (Input.GetMouseButton(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, layer))
                        {
                            currentPiece.transform.position = hit.transform.position;

                            currentPiece.GetComponent<Renderer>().enabled = true;
                        }
                        else
                        {
                            currentPiece.GetComponent<Renderer>().enabled = false;
                        }
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, layer))
                        {
                            Square square = hit.transform.gameObject.GetComponent<Square>();

							int x = (int)square.pos.x;
							
							int y = (int)square.pos.y;

							if(board[x][y] != Players.None) 
							{
								currentPiece.GetComponent<Renderer>().enabled = false;

								return;
							}

                            Dictionary<string, int> dict = new Dictionary<string, int>();

                            dict.Add("x", x);

                            dict.Add("y", y);

                            string json = Fresvii.AppSteroid.Util.Json.Serialize(dict);

                            FASMatchMaking.UpdateGameContext(currentMatch.Id, json, delegate(Fresvii.AppSteroid.Models.GameContext gameContext, Fresvii.AppSteroid.Models.Error error)
                            {
                                if (error == null)
                                {
                                    OnMatchMakingGameContextCreated(gameContext);
                                }
                                else
                                {
                                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Error", delegate(bool del)
                                    {

                                    });
                                }
                            });
                        }
                    }
                }
                else
                {
                    message.text = turnUser.Name + "'s turn";
                }
            }
            #endregion
            else if (mode == Mode.Quit)
            {
                message.text = "Other user quited this game";
            }
        }

        void OnGUI()
        {
            if(GUI.Button(new Rect(Screen.width * 0.05f, Screen.width * 0.05f, Screen.width * 0.2f, Screen.height * 0.05f), "Back"))
            {
                if (mode != Mode.Quit)
                {
                    Dictionary<string, bool> dict = new Dictionary<string, bool>();

                    dict.Add("quit", true);

                    string json = Fresvii.AppSteroid.Util.Json.Serialize(dict);

                    FASMatchMaking.UpdateGameContext(currentMatch.Id, json, delegate(Fresvii.AppSteroid.Models.GameContext gameContext, Fresvii.AppSteroid.Models.Error error)
                    {
                        if (error == null)
                        {
                            OnMatchMakingGameContextCreated(gameContext);

                            Application.LoadLevel("FresviiSample");
                        }
                        else
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Error", delegate(bool del)
                            {

                            });
                        }
                    });
                }
                else
                {
                    FASMatchMaking.DisposeMatch(currentMatch.Id, delegate(Fresvii.AppSteroid.Models.Match match, Fresvii.AppSteroid.Models.Error error)
                    {

                    });

                    Application.LoadLevel("FresviiSample");

                }

            }

            if (mode == Mode.Done)
            {
                if (GUI.Button(new Rect(Screen.width * 0.05f, Screen.width * 0.05f, Screen.width * 0.2f, Screen.height * 0.05f), "Restart"))
                {
                    Dictionary<string, bool> dict = new Dictionary<string, bool>();

                    dict.Add("restart", true);

                    string json = Fresvii.AppSteroid.Util.Json.Serialize(dict);

                    FASMatchMaking.UpdateGameContext(currentMatch.Id, json, delegate(Fresvii.AppSteroid.Models.GameContext gameContext, Fresvii.AppSteroid.Models.Error error)
                    {
                        if (error == null)
                        {
                            OnMatchMakingGameContextCreated(gameContext);

                            Start();
                        }
                        else
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Error", delegate(bool del)
                            {

                            });
                        }
                    });
                }
            }
        }

        private bool CheckWin()
        {
            for (int x = 0; x < 3; x++)
            {
                bool lined = true;

                for (int y = 0; y < 3; y++)
                {
                    if (board[x][y] != turn)
                    {
                        lined = false;
                    }
                }

                if (lined)
                    return true;
            }

            for (int y = 0; y < 3; y++)
            {
                bool lined = true;

                for (int x = 0; x < 3; x++)
                {
                    if (board[x][y] != turn)
                    {
                        lined = false;

                        break;
                    }
                }

                if (lined)
                    return true;
            }

            if(board[0][0] == turn && board[1][1] == turn && board[2][2] == turn)
                return true;

            if (board[2][0] == turn && board[1][1] == turn && board[0][2] == turn)
                return true;

            return false;
        }

        void ChangeTurn()
        {
            if (latestGameContext == null)
            {
                turnUser = currentMatch.Players[0].User;

                turn = Players.Player1;
            }
            else
            {
                turnUser = currentMatch.Players[(int)latestGameContext.UpdatedCount % 2].User;

                turn = (latestGameContext.UpdatedCount % 2 == 0) ? Players.Player1 : Players.Player2;
            }

            currentPiece = (GameObject)Instantiate((turn == Players.Player1) ? prfbCircle : prfbCross);

            currentPiece.transform.parent = piecies.transform;

            currentPiece.GetComponent<Renderer>().material.color = new Color(currentPiece.GetComponent<Renderer>().material.color.r, currentPiece.GetComponent<Renderer>().material.color.g, currentPiece.GetComponent<Renderer>().material.color.b, 0.4f);

            currentPiece.GetComponent<Renderer>().enabled = false;
        }
    }
}