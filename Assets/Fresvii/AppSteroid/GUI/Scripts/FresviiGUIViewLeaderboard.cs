using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIViewLeaderboard : FresviiGUIView
    {
        public static FresviiGUIViewLeaderboard Instance { get; protected set; }

        public GameObject prfbGUIFrameLeaderboard;

        private FresviiGUIFrame frameLeaderboard;
        
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

            frameLeaderboard = ((GameObject)Instantiate(prfbGUIFrameLeaderboard)).GetComponent<FresviiGUIFrame>();

            frameLeaderboard.Init(appIcon, postFix, scaleFactor, FASGui.GuiDepthBase);

            frameLeaderboard.transform.parent = this.transform;

            frameLeaderboard.Position = Vector2.zero;

            CurrentFrame = frameLeaderboard;
        }

        public void SetTopFrame()
        {
            frameLeaderboard.Position = Vector2.zero;

            frameLeaderboard.SetDraw(true);

            frameLeaderboard.GetComponent<FresviiGUILeaderboard>().DestroySubFrames();
        }

    }
}