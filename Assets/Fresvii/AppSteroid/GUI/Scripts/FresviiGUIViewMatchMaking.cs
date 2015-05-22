using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIViewMatchMaking : FresviiGUIView
    {
        public GameObject prfbGUIFrameMatchMaking;

        [HideInInspector]
        public FresviiGUIFrame frameMatchMaking;

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor)
        {
            this.appIcon = appIcon;

            this.postFix = postFix;
            
            this.scaleFactor = scaleFactor;

            frameMatchMaking = ((GameObject)Instantiate(prfbGUIFrameMatchMaking)).GetComponent<FresviiGUIFrame>();

            frameMatchMaking.Init(appIcon, postFix, scaleFactor, FASGui.GuiDepthBase);

            frameMatchMaking.transform.parent = this.transform;

            frameMatchMaking.Position = Vector2.zero;

            CurrentFrame = frameMatchMaking;
        }       
    }
}