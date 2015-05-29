using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Fresvii.AppSteroid;

public class AppSteroidSampleUGUI : MonoBehaviour {

    public InputField inputFieldMinPlayerNumber;

    public InputField inputFieldMaxPlayerNumber;

    public GameObject videoSettings;

    public Button buttonStartVideoRecording, buttonStopVideoRecording, buttonShareVideo;

    private int matchMinPlayerNum = 2, matchMaxPlayerNum = 4;

    public InputField inputFieldSignUpUserName;

    public Text textLog;

    //------------------------------------------
    // Public methods for UI
    //------------------------------------------
    public void OnClickShowGUI()
    {
        FASGui.ShowGUI(FASGui.Mode.All, FASGui.Mode.Forum);
    }

    public void OnClickMatchMakingEveryone()
    {
        if (!ValidateMatchMakingParameter())
        {
            MatchMakingParameterError();

            return;
        }

        FASGui.ShowMatchMakingGui((uint)matchMinPlayerNum, (uint)matchMaxPlayerNum, null, null, null, FASMatchMaking.Recipient.Everyone, "");
    }

    public string leaderboardId;

    public void OnClickReportScore()
    {
        FASUser.RelogIn((error) =>
        {
            if (error == null)
            {
                FASLeaderboard.ReportScore(leaderboardId, Random.Range(0, 2000), (score, error2) =>
                {
                    if (error2 == null)
                    {
                        textLog.text += "Report score : " + score.Value;
                    }
                    else
                    {
                        textLog.text += "Report score error";

                        Debug.LogError(error2.ToString());
                    }
                });                  
            }
            else
            {
                textLog.text += "Login error";

                Debug.LogError(error.ToString());
            }
        });
    }

    public void OnClickMatchMakingFriendOnly()
    {
        if (!ValidateMatchMakingParameter())
        {
            MatchMakingParameterError();

            return;
        }

        FASGui.ShowMatchMakingGui((uint)matchMinPlayerNum, (uint)matchMaxPlayerNum, null, null, null, FASMatchMaking.Recipient.FriendOnly, "");
    }

    public void OnClickStartVideoRecording()
    {
        if (FASPlayVideo.StartRecording())
        {
            buttonStartVideoRecording.gameObject.SetActive(false);

            buttonStopVideoRecording.gameObject.SetActive(true);
        }
    }

    public void OnClickStopVideoRecording()
    {
        FASPlayVideo.StopRecording();

        buttonStartVideoRecording.gameObject.SetActive(true);

        buttonStopVideoRecording.gameObject.SetActive(false);

        buttonShareVideo.interactable = true;
    }

	public void OnClickShareVideo()
	{
		if (!FASPlayVideo.ShowLatestVideoSharingGUIWithUGUI(Application.loadedLevelName))
		{
			Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Error : Recorded video does not exist", delegate(bool del) { });
		}
	}

    public void OnClickSignUp()
    {
        //  Show confirming dialog and sign up 
        string username = inputFieldSignUpUserName.text;

        if (string.IsNullOrEmpty(username)) username = "username";

#if !UNITY_EDITOR
		Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel("OK", "Cancel", "Close");

		Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog("Sign up : Name = " + username, delegate(bool del)
		{
			if(del)
			{
#endif
                textLog.text += "Sign up start\n";

                Fresvii.AppSteroid.Util.DialogManager.ShowProgressSpinnerDialog("Sign up", "Processing...", false);

                FASUser.SignUp(username, (user, error) =>
                {
                    Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

                    if (error != null)
                    {
                        textLog.text += "Sign up error : " + error.ToString() + "\n";

                        Debug.LogError(error.ToString());
                    }
                    else
                    {
                        textLog.text += "Sign up success : " + user.Name;
                    }
                });
#if !UNITY_EDITOR
			}					
		});
#endif
    }

    public void OnClickLogIn()
    {
        textLog.text += "Start : Log in\n";

        Fresvii.AppSteroid.Util.DialogManager.ShowProgressSpinnerDialog("Log in", "Processing...", false);

        FASUser.LogIn((error) =>
        {
            Fresvii.AppSteroid.Util.DialogManager.HideProgressSpinnerDialog();

            if (error != null)
            {
                textLog.text += textLog.text += "Log in error : " + error.ToString() + "\n";

                Debug.LogError(error.ToString());
            }
            else
            {
                textLog.text += "Log in success : " + FAS.CurrentUser.Name;
            }
        });
    }

    public void OnClickLogOut()
    {
        FASUser.LogOut();
    }

    #region Private Methods
    //------------------------------------------
    // Private methods
    //------------------------------------------

    void MatchMakingParameterError()
    {
        Debug.LogError("Match Member count is invalid");

#if !UNITY_EDITOR
        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("Match Member count is invalid", delegate(bool del)
        {

        });
#endif
    }

    void ResetMatchMakingParameter()
    {
        matchMinPlayerNum = 2;

        matchMaxPlayerNum = 4;

        inputFieldMinPlayerNumber.text = matchMinPlayerNum.ToString();

        inputFieldMaxPlayerNumber.text = matchMinPlayerNum.ToString();
    }

    bool ValidateMatchMakingParameter()
    {
        try
        {
            matchMinPlayerNum = int.Parse(inputFieldMinPlayerNumber.text);

            matchMaxPlayerNum = int.Parse(inputFieldMaxPlayerNumber.text);
        }
        catch
        {
            ResetMatchMakingParameter();

            return false;
        }

        if (matchMinPlayerNum < 2 || matchMaxPlayerNum > 16 || matchMinPlayerNum < 2 || matchMaxPlayerNum > 16 || matchMinPlayerNum > matchMaxPlayerNum)
        {
            ResetMatchMakingParameter();

            return false;
        }

        return true;
    }

    IEnumerator Start()
    {
        textLog.text += "AppSteroid Version : " + FAS.Version.ToString() + "\n";

        textLog.text += "UUID : " + FASConfig.Instance.appId + "\n";

        buttonStartVideoRecording.gameObject.SetActive(!FASPlayVideo.IsRecording());

        buttonStopVideoRecording.gameObject.SetActive(FASPlayVideo.IsRecording());

        buttonShareVideo.interactable = false;

#if UNITY_ANDROID
        videoSettings.SetActive(false);
#endif

        while (!FAS.Initialized)
        {
            yield return 1;
        }

        textLog.text += "Host : " + FAS.Host + "\n";
    }
    #endregion
}
