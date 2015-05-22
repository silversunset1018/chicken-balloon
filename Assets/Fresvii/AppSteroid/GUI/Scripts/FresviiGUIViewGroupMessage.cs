using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIViewGroupMessage : FresviiGUIView
    {
        public GameObject prfbGUIFrameGroupMessage;

        [HideInInspector]
        public FresviiGUIFrame frameGroupMessage;

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor)
        {
            this.appIcon = appIcon;

            this.postFix = postFix;
            
            this.scaleFactor = scaleFactor;

            frameGroupMessage = ((GameObject)Instantiate(prfbGUIFrameGroupMessage)).GetComponent<FresviiGUIFrame>();

            frameGroupMessage.Init(appIcon, postFix, scaleFactor, FASGui.GuiDepthBase);

            frameGroupMessage.transform.parent = this.transform;

            frameGroupMessage.Position = Vector2.zero;

            CurrentFrame = frameGroupMessage;
        }

        public void SetTopFrame()
        {
            frameGroupMessage.Position = Vector2.zero;

            frameGroupMessage.SetDraw(true);

            frameGroupMessage.GetComponent<FresviiGUIGroupMessage>().DestroySubFrames();
        }
    }
}