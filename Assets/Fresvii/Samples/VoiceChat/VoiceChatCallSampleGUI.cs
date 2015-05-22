#if GROUP_CONFERENCE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Fresvii.AppSteroid;
using Fresvii.AppSteroid.Services;


public class VoiceChatCallSampleGUI : MonoBehaviour {

    private enum Mode { Dial, Calling }

    public GUISkin guiSkin;

    private IList<Fresvii.AppSteroid.Models.Group> groups = new List<Fresvii.AppSteroid.Models.Group>();

    private Mode mode = Mode.Dial;

	private Vector2 scrollPosition;

    void Start()
    {
        FASGroup.GetGroupMessageGroupList(delegate(IList<Fresvii.AppSteroid.Models.Group> groups, Fresvii.AppSteroid.Models.Error error)
        {
            this.groups = groups;

            foreach (Fresvii.AppSteroid.Models.Group group in this.groups)
            {
				string name = "";

                foreach(Fresvii.AppSteroid.Models.Member member in group.MessageMembers)
				{
					name += member.Name + ", ";
				}

				group.Name = name;
            }
        });
    }

    void OnGUI()
    {
        int space = 20;

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

        GUILayout.BeginArea(new Rect(space, 0, Screen.width * 0.95f, Screen.height));

        GUILayout.Label("Voice chat", guiSkin.label);

        if (mode == Mode.Dial)
        {
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(space);

			GUILayout.Label("Call: ", guiSkin.label, GUILayout.Width(Screen.width * 0.65f));

			GUILayout.Space(space);

			foreach (Fresvii.AppSteroid.Models.Group group in this.groups)
            {
				GUILayout.BeginHorizontal();

				GUILayout.Space(space);

                if (GUILayout.Button((group.Name == null) ? "" : group.Name, guiSkin.button, GUILayout.Width(Screen.width * 0.6f)))
                {
                    FASConference.StartConference(group.Id, 
					
					    delegate(Fresvii.AppSteroid.Models.Error error)
						{
	                   		if(error != null)
							{
								Debug.LogError(error);
							}
	                    },

						delegate(Fresvii.AppSteroid.Models.Error error)
						{
							if(error != null)
							{
								Debug.LogError(error);
							}
						}
					);

                    mode = Mode.Calling;
                }

				GUILayout.Space(space);

                if (GUILayout.Button("Delete", guiSkin.button))
                {
                    FAS.Instance.Client.GroupConferenceService.DeleteConference(group.Id, delegate(Fresvii.AppSteroid.Models.Error error) 
					{ 
						if(error != null){
							Debug.LogError("DeleteConference : " + error.ToString());
						}
						else{
							Debug.Log("DeleteConference : Success");
						}
					});
                }

				GUILayout.Space(space * 2f);

				GUILayout.EndHorizontal();

				GUILayout.Space(space);
            }

			GUILayout.Space(space * 2f);
			
			if (GUILayout.Button("Back", guiSkin.button))
			{
				Application.LoadLevel("FresviiSample");
			}

			GUILayout.EndScrollView();
        }
        else if (mode == Mode.Calling)
        {
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

                mode = Mode.Dial;
            }
        }

        GUILayout.EndArea();
	}
}
#endif