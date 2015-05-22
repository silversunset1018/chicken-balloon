using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIViewForum : FresviiGUIView
    {
        public static FresviiGUIViewForum Instance { get; protected set; }

        public GameObject prfbGUIFrameForum;

        [HideInInspector]
        public FresviiGUIFrame frameForum;
        
        private int postScreenWidth;

        void Awake()
        {
            Instance = this;
        }

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor)
        {
            this.appIcon = appIcon;

            this.postFix = postFix;
            
            this.scaleFactor = scaleFactor;

            frameForum = ((GameObject)Instantiate(prfbGUIFrameForum)).GetComponent<FresviiGUIFrame>();

            frameForum.Init(appIcon, postFix, scaleFactor, FASGui.GuiDepthBase);

            frameForum.transform.parent = this.transform;

            frameForum.Position = Vector2.zero;

            CurrentFrame = frameForum;
        }

        public void ShowThreadByNotification(string threadId)
		{
            frameForum.GetComponent<FresviiGUIForum>().ShowThreadByNotification(threadId);
		}

        public void SetTopFrame()
        {
            frameForum.Position = Vector2.zero;

            frameForum.SetDraw(true);

            frameForum.GetComponent<FresviiGUIForum>().DestroySubFrames();
        }

        public void AddThread(Fresvii.AppSteroid.Models.Thread thread)
        {
            frameForum.GetComponent<FresviiGUIForum>().UpdateThread(thread);
        }
    }
}