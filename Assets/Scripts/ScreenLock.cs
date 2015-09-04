using UnityEngine;
using System.Collections;

public class ScreenLock : MonoBehaviour {

	// Use this for initialization
	void Awake () 
    {
        if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        Screen.autorotateToPortrait = false;
        
        Screen.autorotateToPortraitUpsideDown = false;
    }

    /*void Update()
    {
        if (Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }*/

}
