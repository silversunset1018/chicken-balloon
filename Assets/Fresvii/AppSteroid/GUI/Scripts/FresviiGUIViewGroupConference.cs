using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIViewGroupConference : FresviiGUIView
    {
        public GameObject prfbGUIFrameGroupConference;

        [HideInInspector]
        public FresviiGUIFrame frameGroupConference;

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor)
        {
            this.appIcon = appIcon;

            this.postFix = postFix;
            
            this.scaleFactor = scaleFactor;

            frameGroupConference = ((GameObject)Instantiate(prfbGUIFrameGroupConference)).GetComponent<FresviiGUIFrame>();

            frameGroupConference.Init(appIcon, postFix, scaleFactor, FASGui.GuiDepthBase);

            frameGroupConference.transform.parent = this.transform;

            frameGroupConference.Position = Vector2.zero;

            CurrentFrame = frameGroupConference;
        }        
    }
}