using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIViewMyProfile : FresviiGUIView
    {
        public static FresviiGUIViewMyProfile Instance { get; protected set; }

        public GameObject prfbGUIFrameMyProfile;

        public FresviiGUIFrame frameMyProfile;
        
        private int postScreenWidth;

        public FresviiGUIThreadCard selectedThreadCard { get; protected set; }

        void Awake()
        {
            Instance = this;
        }

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor)
        {
            this.appIcon = appIcon;
            this.postFix = postFix;
            this.scaleFactor = scaleFactor;

            frameMyProfile = ((GameObject)Instantiate(prfbGUIFrameMyProfile)).GetComponent<FresviiGUIFrame>();

            frameMyProfile.GetComponent<FresviiGUIMyProfile>().IsOriginal = true;

            frameMyProfile.Init(appIcon, postFix, scaleFactor, FASGui.GuiDepthBase);

            frameMyProfile.transform.parent = this.transform;

            frameMyProfile.Position = Vector2.zero;

            CurrentFrame = frameMyProfile;
        }

        public void SetTopFrame()
        {
            frameMyProfile.Position = Vector2.zero;

            frameMyProfile.SetDraw(true);

            frameMyProfile.GetComponent<FresviiGUIMyProfile>().DestroySubFrames();
        }

    }
}