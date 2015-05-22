using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;


using Fresvii.AppSteroid.Gui;

namespace Fresvii.AppSteroid.SampleGame
{
    public class MatchMakingMultiPlayerSample : MonoBehaviour
    {
        private enum Mode { Game, Done, Quit }

        public float pollingInterval = 5f;

        private Fresvii.AppSteroid.Models.GameContext latestGameContext = null;

        private Fresvii.AppSteroid.Models.Match currentMatch;
       
        private string logMessage = "GameContexts\n";

        public GUISkin guiSkin;

        float lineHeight;

        // Use this for initialization
        void Start()
        {
            currentMatch = FASMatchMaking.currentMatch;

            int largeLength = Mathf.Max(Screen.height, Screen.width);

            lineHeight = largeLength / 12;
           
            guiSkin.label.fontSize = (int)(lineHeight / 3);
            
            guiSkin.button.fontSize = (int)(lineHeight / 3);
            
            guiSkin.button.fixedHeight = lineHeight;
            
            guiSkin.button.fixedHeight = Screen.width / 5;
            
            guiSkin.textArea.fontSize = (int)(lineHeight / 3);

            guiSkin.textField.fontSize = (int)(lineHeight / 3);
            
            guiSkin.textField.fixedHeight = lineHeight;

            StartCoroutine(PollingGetGameContext());
        }

        void OnEnable()
        {
            FASEvent.OnMatchMakingGameContextCreated += OnMatchMakingGameContextCreated;
        }

        void OnDestroy()
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

            if (valueDict.Contains("message"))
            {
                string msg = ((string)valueDict["message"]);

                logMessage += gameContext.UpdatedCount + ". " + gameContext.UpdatedBy.Name + " :\n" + msg + "\n\n";
            }
            else if (valueDict.Contains("quit"))
            {
                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("This match has quited.", delegate(bool del)
                {
                    
                });

                Application.LoadLevel("FresviiSample");
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
                        else
                        {
                            Debug.LogError(error.ToString());

                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("This match has quited.", delegate(bool del)
                            {

                            });

                            Application.LoadLevel("FresviiSample");
                        }
                    });
                }

                yield return new WaitForSeconds(pollingInterval);
            }
        }

        string textMessage = "";

        Vector2 scrollPosition = Vector2.zero;

        void Update()
        {
            if (FASGesture.IsDragging || FASGesture.Inertia)
            {
                Rect rectLogArea = new Rect(0, 0, Screen.width, Screen.height * 0.5f);

    
                if (rectLogArea.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    scrollPosition += FASGesture.Delta;
                }
            }
        }
        
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, guiSkin.scrollView, GUILayout.Height(Screen.height * 0.5f));

                GUILayout.Label(logMessage, guiSkin.label);

            GUILayout.EndScrollView();

            textMessage = GUILayout.TextField(textMessage, guiSkin.textField, GUILayout.Height(lineHeight));

            if (GUILayout.Button("Game Context Post", guiSkin.button))
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();

                dict.Add("message", textMessage);

                string json = Fresvii.AppSteroid.Util.Json.Serialize(dict);

                FASMatchMaking.UpdateGameContext(currentMatch.Id, json, delegate(Fresvii.AppSteroid.Models.GameContext gameContext, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error == null)
                    {
                        OnMatchMakingGameContextCreated(gameContext);

                        textMessage = "";
                    }
                    else
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Error", delegate(bool del)
                        {

                        });
                    }
                });
            }

            GUILayout.Space(lineHeight);

            if (GUILayout.Button("Quit", guiSkin.button))
            {
                Dictionary<string, bool> dict = new Dictionary<string, bool>();

                dict.Add("quit", true);

                string json = Fresvii.AppSteroid.Util.Json.Serialize(dict);

                FASMatchMaking.UpdateGameContext(currentMatch.Id, json, delegate(Fresvii.AppSteroid.Models.GameContext gameContext, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error == null)
                    {
                        FASMatchMaking.DisposeMatch(currentMatch.Id, delegate(Fresvii.AppSteroid.Models.Match _match, Fresvii.AppSteroid.Models.Error _error)
                        {
                            if (_error == null)
                            {
                                OnMatchMakingGameContextCreated(gameContext);
                            }
                            else
                            {
                                Application.LoadLevel("FresviiSample");
                            }
                        });
                    }
                    else
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Error", delegate(bool del)
                        {

                        });
                    }
                });
            }

            GUILayout.EndArea();
        }
    }
}