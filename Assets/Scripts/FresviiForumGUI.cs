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
		List<User> users = FAS.LoadSignedUpUsers();
		Debug.LogWarning(users.Count);
		return (users.Count > 0);
	}
	

	//------------------------------
	//ログインを行ってからGUI表示 / Login and show GUI
	//userName: ログインに使用する名前/name used login
	//------------------------------
	public void ShowFGCGui(string userName)
    {
        //  Get signed up users list
        List<User> users = FAS.LoadSignedUpUsers();

        //  If signed up user already exists
        if (users.Count > 0)
        {
            User user = users[users.Count - 1]; //  this case, we use latest signed up user account.

			Debug.Log("login");
            FAS.LogIn(user.Id, user.Token, delegate(Error error){
				OnLogin(error);    
            });

            return;
        }
        //  If signed up user does not exist
        else
		{
			if (userName == "") {
				userName = defaultUserName;
			}
			Debug.Log("SignUp "+userName);
			FAS.SignUp(userName, delegate(User user, Error error)
            {
                if (error == null)
                {
                    FAS.LogIn(user.Id, user.Token, delegate(Error error2)
                    {
                        if (error2 == null)
                        {
           
							//端末の回転を制御（縦持ちを許可する）/controll screen orientation in mobile
							if (!GameController.isIOS8) {
								Screen.autorotateToPortrait = true;
								Screen.autorotateToPortraitUpsideDown = true;
							}
							//GUI表示/show GUI
							FASGui.ShowGUI(FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile, FASGui.Mode.Forum);
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
    }

	//------------------------------
	//ログイン時の処理.
	//------------------------------
	void OnLogin(Error error) {
		if (error == null)
		{
			//端末の回転を制御（縦持ちを許可する）/controll screen orientation in mobile
			if (!GameController.isIOS8) {
				Screen.autorotateToPortrait = true;
				Screen.autorotateToPortraitUpsideDown = true;
			}
			//GUI表示/show GUI
			FASGui.ShowGUI(FASGui.Mode.Forum | FASGui.Mode.Leaderboards | FASGui.Mode.MyProfile, FASGui.Mode.Forum);
		}
		else
		{
			Debug.LogError(error.ToString());
		}
	}
}
