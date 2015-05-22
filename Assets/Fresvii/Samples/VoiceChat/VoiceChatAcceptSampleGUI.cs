#if GROUP_CONFERENCE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Services;


public class VoiceChatAcceptSampleGUI : MonoBehaviour
{
    private enum Mode { Dial, Calling }

    public GUISkin guiSkin;

	private Vector2 scrollPosition;   

    void OnGUI()
    {
        int space = 10;

        int largeLength = Mathf.Max(Screen.height, Screen.width);

        int lineHeight = largeLength / 12;

        guiSkin.label.fontSize = lineHeight / 3;
    
        guiSkin.button.fontSize = lineHeight / 3;
        
        guiSkin.button.fixedHeight = lineHeight;
        
        guiSkin.button.fixedHeight = Screen.width / 8;
        
        guiSkin.textArea.fontSize = lineHeight / 3;

        guiSkin.verticalScrollbar.fixedWidth = 0;
        
        guiSkin.horizontalScrollbar.fixedWidth = 0;

        guiSkin.textField.fontSize = lineHeight / 3;
        
        guiSkin.textField.fixedHeight = lineHeight;

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

        GUILayout.Label("Voice chat", guiSkin.label);

        GUILayout.Space(space * 2f);

        if (GUILayout.Button("Mute", guiSkin.button))
        {
            FASConference.Mute();
        }

        GUILayout.Space(space);

        if (GUILayout.Button("Unmute", guiSkin.button))
        {
            FASConference.Unmute();
        }

        GUILayout.Space(space);

        if (GUILayout.Button("End", guiSkin.button))
        {
            FASConference.Leave();

            Application.LoadLevel("FresviiSample");
        }
       
        GUILayout.EndArea();
	}
}
#endif