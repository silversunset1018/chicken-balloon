using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Gui;
using Fresvii.AppSteroid.Models;

//------------------------------
//Fresvii SDK：ログインとフォーラムGUI表示 / Login and Show Forum GUI
//------------------------------
public class FresviiForumGUI : MonoBehaviour
{
    public string defaultUserName = "Chicken";

	//------------------------------
	//サインアップ済みか確認 / Already signup?
	//------------------------------
	public bool GetAlreadySignup() {
		List<User> users = FASUser.LoadSignedUpUsers();
		Debug.LogWarning(users.Count);
		return (users.Count > 0);
	}
	

	//------------------------------
	//ログインを行ってからGUI表示 / Login and show GUI
	//userName: ログインに使用する名前/name used login
	//------------------------------
	public bool ShowFASGui(string userName)
	{
		bool success = false;
        //  Get signed up users list
        List<User> users = FASUser.LoadSignedUpUsers();

        //  If signed up user already exists
        if (users.Count > 0)
        {
            User user = users[users.Count - 1]; //  this case, we use latest signed up user account.

			Debug.Log("login");
            /*FASUser.LogIn(user.Id, user.Token, delegate(Error error){
				success = DoLogin(error, true);
			});*/

            //端末の回転を制御（縦持ちを許可する）/controll screen orientation in mobile
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;

            FASGui.ShowGUIWithLogin(user.Id, user.Token, FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile | FASGui.Mode.GroupMessage, FASGui.Mode.Forum);

			return true ;
        }
        //  If signed up user does not exist
        else
		{
			if (userName == "") {
				userName = defaultUserName;
			}
			Debug.Log("SignUp "+userName);
			FASUser.SignUp(userName, delegate(User user, Error error)
            {
                if (error == null)
                {
                    /*FASUser.LogIn(user.Id, user.Token, delegate(Error error2)
                    {
                        if (error2 == null)
                        {
							//端末の回転を制御（縦持ちを許可する）/controll screen orientation in mobile
							//if (!GameController.isIOS8) {
								Screen.autorotateToPortrait = true;
								Screen.autorotateToPortraitUpsideDown = true;
							//}
							//GUI表示/show GUI
							FASGui.ShowGUI(FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile | FASGui.Mode.GroupMessage, FASGui.Mode.Forum);
							success = true;
                        }
                        else
                        {
							Debug.LogError(error2.ToString());
							success = false;
                        }
                    });*/

                    //端末の回転を制御（縦持ちを許可する）/controll screen orientation in mobile
                    Screen.autorotateToPortrait = true;

                    Screen.autorotateToPortraitUpsideDown = true;

                    FASGui.ShowGUIWithLogin(user.Id, user.Token, FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile | FASGui.Mode.GroupMessage, FASGui.Mode.Forum);
                }
                else
                {
					Debug.LogError(error.ToString());
					success = false;
                }
            });
        }
		return success;
    }

	//------------------------------
	//ログイン時の処理.
	//------------------------------
	/*private bool DoLogin(Error error, bool isForum) {
		if (error == null)
		{
			if (isForum) {
				//端末の回転を制御（縦持ちを許可する）/controll screen orientation in mobile
				//if (!GameController.isIOS8) {
					Screen.autorotateToPortrait = true;
					Screen.autorotateToPortraitUpsideDown = true;
				//}
				//GUI表示/show GUI
				FASGui.ShowGUI(FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile| FASGui.Mode.GroupMessage, FASGui.Mode.Forum);
			} else {
				ShowMatchMakingGui();
			}
			return true;
		}
		else
		{
			Debug.LogError(error.ToString());
			return false;
		}
	}*/

	//------------------------------
	//マッチメイキング処理.
	//------------------------------
	public bool ShowFASMatchGui(string userName)
	{
		bool success = false;
		//  Get signed up users list
		List<User> users = FASUser.LoadSignedUpUsers();

		//  If signed up user already exists
		if (users.Count > 0)
		{
			User user = users[users.Count - 1]; //  this case, we use latest signed up user account.
			//Debug.Log("login");
			/*FASUser.LogIn(user.Id, user.Token, delegate(Error error){
				success = DoLogin(error, false);    
			});*/


            Debug.Log("ShowMatchMakingGuiWithLogin");

            FASGui.ShowMatchMakingGuiWithLogin(user.Id, user.Token, (uint)NetworkConnectByMatchMaking.MATCH_PLAYER_NUMBER_MIN, (uint)NetworkConnectByMatchMaking.MATCH_PLAYER_NUMBER_MAX, null, null, null, "StartScene");
		}
		//  If signed up user does not exist
		else
		{
			if (userName == "") 
            {
				userName = defaultUserName;
			}

            Debug.Log("SignUp "+userName);
			
            FASUser.SignUp(userName, delegate(User user, Error error)
			           {
				if (error == null)
				{
					/*FASUser.LogIn(user.Id, user.Token, delegate(Error error2)
					          {
						if (error2 == null)
						{
							//GUI表示/show GUI
							ShowMatchMakingGui();
							success = true;
						}
						else
						{
							Debug.LogError(error2.ToString());
							success = false;
						}
					});*/

                    FASGui.ShowMatchMakingGuiWithLogin(user.Id, user.Token, (uint)NetworkConnectByMatchMaking.MATCH_PLAYER_NUMBER_MIN, (uint)NetworkConnectByMatchMaking.MATCH_PLAYER_NUMBER_MAX, null, null, null, "StartScene");

                    success = true;
				}
				else
				{
					Debug.LogError(error.ToString());

					success = false;
				}
			});
		}
		return success;
	}

	/*public void ShowMatchMakingGui()
	{
		FASGui.ShowMatchMakingGui((uint)NetworkConnectByMatchMaking.MATCH_PLAYER_NUMBER_MIN, (uint)NetworkConnectByMatchMaking.MATCH_PLAYER_NUMBER_MAX, null, null, null, "StartScene");
	}*/
}
