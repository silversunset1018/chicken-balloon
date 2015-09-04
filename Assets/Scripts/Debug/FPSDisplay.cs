using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour {

    int frameRateCount;

    bool reflesh;

    GUIContent frameRate = new GUIContent("");

    public GUIStyle guiStyle;

	// Use this for initialization
	IEnumerator Start () {

        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            reflesh = true;
        }

	}
	
	// Update is called once per frame
	void Update () {
        frameRateCount++;
	}

    void OnGUI()
    {
        if (reflesh)
        {
            frameRate = new GUIContent(frameRateCount + " fps");

            frameRateCount = -1;

            reflesh = false;
        }

        GUI.Label(new Rect(10, 10, 200, 50), frameRate, guiStyle); 
    }
}
