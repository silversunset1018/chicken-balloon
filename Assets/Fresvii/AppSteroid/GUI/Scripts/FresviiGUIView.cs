using UnityEngine;
using System;
using System.Collections;


namespace Fresvii.AppSteroid.Gui
{
    public abstract class FresviiGUIView : MonoBehaviour
    {
        public enum SlideDirection { LeftToRight, RightToLeft }

        public float tweenTime = 0.5f;
        
        public iTween.EaseType tweenEaseType = iTween.EaseType.easeOutExpo;

        protected Texture2D appIcon;

        protected float scaleFactor;
        
        protected string postFix;

        [HideInInspector]
        public FresviiGUIFrame CurrentFrame;

        public abstract void Init(Texture2D appIcon, string postFix, float scaleFactor);       
    }
}
