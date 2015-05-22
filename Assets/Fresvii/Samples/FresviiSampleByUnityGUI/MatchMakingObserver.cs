using UnityEngine;
using System.Collections;

using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Gui;



public class MatchMakingObserver : MonoBehaviour {
    
    public string loadSceneNameOnMatchCompleted;

    private static MatchMakingObserver matchMakingObserver;

    private static Fresvii.AppSteroid.Models.Match completeMatch;

	// Use this for initialization
	void Awake () {

        if (matchMakingObserver != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            matchMakingObserver = this;

            DontDestroyOnLoad(this.gameObject);
        }
	}
	
    void OnEnable()
    {
        FASEvent.OnMatchMakingMatchCompleted += OnMatchMakingMatchCompleted;
    }

    void OnDisable()
    {
        FASEvent.OnMatchMakingMatchCompleted -= OnMatchMakingMatchCompleted;
    }

    void OnMatchMakingMatchCompleted(Fresvii.AppSteroid.Models.Match match)
    {
        if (completeMatch == null)
        {
            completeMatch = match;
        }
        else if (completeMatch.Id == match.Id)
        {
            return;
        }
        else
        {
            completeMatch = match;
        }

        if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
        {
            Application.LoadLevel(loadSceneNameOnMatchCompleted);
        }
        else
        {
            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("Yes", "No", "Close");

            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog("Match completed! Sample Game?", delegate(bool del)
            {
                if (del)
                {
                    Application.LoadLevel(loadSceneNameOnMatchCompleted);
                }
            });
        }
    }
}
