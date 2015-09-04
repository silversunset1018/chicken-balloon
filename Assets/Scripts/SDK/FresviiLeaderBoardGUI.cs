using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Gui;
using Fresvii.AppSteroid.Models;

//------------------------------
//Fresvii SDK：ログインとリーダーボードGUI表示 / Login and Show LeaderBoard GUI
//------------------------------
public class FresviiLeaderBoardGUI : MonoBehaviour
{
	private string returnSceneName = "StartScene";

	//------------------------------
	//サインアップ済みか確認 / Already signup?
	//------------------------------
    public bool GetAlreadySignup() {
		List<User> users = FASUser.LoadSignedUpUsers();
		return (users.Count > 0);
    }

	public bool SendPoint(int point, string userName)
    {
		Debug.Log("SendPoint");
		bool result = false;
        //  Get signed up users list
		List<User> users = FASUser.LoadSignedUpUsers();

        //  If signed up user already exists
        if (users.Count > 0)
        {
            User user = users[users.Count - 1]; //  this case, we use latest signed up user account.
			FASUser.LogIn(user.Id, user.Token, delegate(Error error)
            {
                if (error == null)
                {
					FASLeaderboard.ReportScore(GameController.LEADERBOARD_ID, point, delegate(Score score, Error error2)
					{
						result = true;
						
						//端末の回転を制御（縦持ちを許可する）/controll screen orientation in mobile
						//if (!GameController.isIOS8) {
						//	Screen.autorotateToPortrait = true;
						//	Screen.autorotateToPortraitUpsideDown = true;
						//}

						//GUI表示/show GUI
						//FASGui.ShowGUI(FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile | FASGui.Mode.GroupMessage, returnSceneName, FASGui.Mode.Leaderboards);
                        //FASGui.ShowGUIWithLogin(user.Id, user.Token, FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile | FASGui.Mode.GroupMessage, FASGui.Mode.Leaderboards, returnSceneName);
					});
				}
                else
                {
                    Debug.LogError(error.ToString());
                }
            });

        }
        //  If signed up user does not exist
        else
        {
			FASUser.SignUp(userName, delegate(User user, Error error)
			{
				FASUser.SaveSignUpUser(user);
                if (error == null)
                {
					FASUser.LogIn(user.Id, user.Token, delegate(Error error2)
                    {
                        if (error2 == null)
                        {
							FASLeaderboard.ReportScore(GameController.LEADERBOARD_ID, point, delegate(Score score, Error error3)
							{
								result = true;
								//端末の回転を制御（縦持ちを許可する）/controll screen orientation in mobile
								//if (!GameController.isIOS8) {
								//	Screen.autorotateToPortrait = true;
								//	Screen.autorotateToPortraitUpsideDown = true;
								//}
								
                                //GUI表示/show GUI
								//FASGui.ShowGUI(FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile | FASGui.Mode.GroupMessage, returnSceneName, FASGui.Mode.Leaderboards);

                                //FASGui.ShowGUIWithLogin(user.Id, user.Token, FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile | FASGui.Mode.GroupMessage, FASGui.Mode.Leaderboards, returnSceneName);
							});
						}
                        else
                        {
                            Debug.LogError(error2.ToString());
                        }
                    });

                }
                else
                {
                    Debug.LogError(error.ToString());
                }
            });
        }
		return result;
    }

	
	public bool ShowGUI(string userName)
	{
		Debug.Log("ShowGUI");
		bool result = false;
		//  Get signed up users list
		List<User> users = FASUser.LoadSignedUpUsers();
		
		//  If signed up user already exists
		if (users.Count > 0)
		{
			User user = users[users.Count - 1]; //  this case, we use latest signed up user account.
			FASUser.LogIn(user.Id, user.Token, delegate(Error error)
			              {
				if (error == null)
				{
					Screen.autorotateToPortrait = true;
					Screen.autorotateToPortraitUpsideDown = true;
					FASGui.ShowGUIWithLogin(user.Id, user.Token, FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile | FASGui.Mode.GroupMessage, FASGui.Mode.Leaderboards, returnSceneName);
				}
				else
				{
					Debug.LogError(error.ToString());
				}
			});
			
		}
		//  If signed up user does not exist
		else
		{
			FASUser.SignUp(userName, delegate(User user, Error error)
			               {
				FASUser.SaveSignUpUser(user);
				if (error == null)
				{
					FASUser.LogIn(user.Id, user.Token, delegate(Error error2)
					              {
						if (error2 == null)
						{
							
							Screen.autorotateToPortrait = true;
							Screen.autorotateToPortraitUpsideDown = true;
							FASGui.ShowGUIWithLogin(user.Id, user.Token, FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile | FASGui.Mode.GroupMessage, FASGui.Mode.Leaderboards, returnSceneName);
						}
						else
						{
							Debug.LogError(error2.ToString());
						}
					});
					
				}
				else
				{
					Debug.LogError(error.ToString());
				}
			});
		}
		return result;
	}
}
