using UnityEngine;
using System;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public abstract class FresviiGUIFrame : MonoBehaviour
    {
        private Vector2 position;

        public Vector2 Position 
        {
            get { return position + OffsetPosition; }

            set { position = value; }
        }

        public static Vector2 OffsetPosition { get; set; }

        public bool ControlLock { get; set; }
        
        private static readonly string TweenName = "FlameSlide";

        private readonly string ResetPositionTweenName = "PullResetPosition";

        private float resetTweenTime = 0.5f;

        private iTween.EaseType resetTweenEaseType = iTween.EaseType.easeOutExpo;

        private Action callback;

        public static readonly float frameSlideTime = 0.5f;

        public static readonly iTween.EaseType frameEaseType = iTween.EaseType.easeOutExpo;

        public int GuiDepth { get; set; }

        public FresviiGUIFrame PostFrame { get; set; }

        protected Rect backgroundRect;

        protected Rect textureCoordsBackground;

        private bool postDrawed;

        protected bool inertiaEscapeThisFrame;

        private FresviiGUIScrollviewSlider scrollviewSlider;

        private bool useSlider = false;

        private float sliderMargin;

        [HideInInspector]
        public bool pullRefleshing;

        public static void TweenStop()
        {
            iTween.StopByName(TweenName);
        }

        public void Tween(Vector2 from, Vector2 to, Action onComplete)
        {
            Position = from;

            this.callback = onComplete;

            iTween.ValueTo(this.gameObject, iTween.Hash("name", TweenName, "from", from, "to", to, "time", frameSlideTime, "easetype", frameEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnPivotMoveUpdate", "oncompletetarget", this.gameObject, "oncomplete", "OnPivotMoveComplete"));
        }

        public void Tween(Vector2 from, Vector2 to, float delay, Action onComplete)
        {
            Position = from;

            this.callback = onComplete;

            iTween.ValueTo(this.gameObject, iTween.Hash("name", TweenName, "from", from, "to", to, "time", frameSlideTime, "delay", delay, "easetype", frameEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnPivotMoveUpdate", "oncompletetarget", this.gameObject, "oncomplete", "OnPivotMoveComplete"));
        }

        void OnPivotMoveUpdate(Vector2 value)
        {
            Position = value;
        }

        void OnPivotMoveComplete()
        {
            FASGesture.Stop();

            callback();
        }

        public FresviiGUIScrollviewSlider SetScrollSlider(float sliderMargin)
        {
            useSlider = true;

            this.sliderMargin = sliderMargin;

            if (this.scrollviewSlider == null)
            {
                scrollviewSlider = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/ScrollViewSlider"))).GetComponent<FresviiGUIScrollviewSlider>();

                scrollviewSlider.transform.parent = this.transform;

                scrollviewSlider.GuiDepth = this.GuiDepth - 1;
            }

            return scrollviewSlider;
        }

        public void InertiaScrollView(ref Vector2 scrollPosition, ref Rect scrollViewRect, float scrollViewHeight, Rect baseRect)
        {
            InertiaScrollView(ref scrollPosition, ref scrollViewRect, scrollViewHeight, baseRect, 0f, 0f);
        }

        public void InertiaScrollView(ref Vector2 scrollPosition, ref Rect scrollViewRect, float scrollViewHeight, Rect baseRect, float topMargin, float bottomMargin)
        {
            if (ControlLock)
            {
                scrollviewSlider.Invisible();

                return;
            }

            if (FASGesture.DragDirec != FASGesture.DragDirection.Vertical)
            {
                scrollViewRect = new Rect(0f, scrollPosition.y, baseRect.width, scrollViewHeight);

                return;
            }

            if (inertiaEscapeThisFrame)
            {
                inertiaEscapeThisFrame = false;

                return;
            }

            //  Scroll process
            if (postDrawed && ((FASGesture.IsDragging && baseRect.Contains(FASGesture.TouchPosition)) || (FASGesture.Inertia && (baseRect.Contains(FASGesture.DragStartPosition) || baseRect.Contains(FASGesture.DragEndPosition)))))
            {
                scrollPosition -= FASGesture.Delta;

                if (scrollPosition.y > 0.0f)
                {
                    scrollPosition.y += FASGesture.Delta.y * scrollPosition.y / baseRect.height;
                }
                else if (scrollPosition.y < baseRect.height - scrollViewRect.height)
                {
                    scrollPosition.y += FASGesture.Delta.y * Mathf.Abs(scrollViewRect.height + scrollPosition.y - baseRect.height) / baseRect.height;
                }
            }
            else if (postDrawed && (FASGesture.IsTouchEnd && !FASGesture.Inertia) || FASGesture.IsInertiaEnd)
            {
                iTween.StopByName(ResetPositionTweenName);

                if (baseRect.height > scrollViewHeight)
                {
                    iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollPosition, "to", Vector2.zero, "time", resetTweenTime, "easetype",resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompletePull"));
                }
                else if (scrollPosition.y > 0.0f && baseRect.height < scrollViewHeight)
                {
                    iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollPosition, "to", Vector2.zero, "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompletePull"));
                }
                else if (scrollPosition.y < baseRect.height - scrollViewRect.height && baseRect.height < scrollViewHeight)
                {
                    iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollPosition, "to", new Vector2(0.0f, baseRect.height - scrollViewHeight), "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompletePull"));
                }
            }

            if (postDrawed && FASGesture.Inertia)
            {
                if (scrollPosition.y > 0f)
                {
                    FASGesture.InertiaBrake(scrollPosition.y);
                }
                else if (scrollPosition.y < -scrollViewHeight + baseRect.height)
                {
                    FASGesture.InertiaBrake(scrollPosition.y + scrollViewHeight - baseRect.height);
                }
            }

            postDrawed = true;

            scrollViewRect = new Rect(0f, scrollPosition.y, baseRect.width, scrollViewHeight);

            if (useSlider)
            {
                if (baseRect.height < scrollViewRect.height)
                {
                    Rect sliderPosition = new Rect();

                    sliderPosition.x = Position.x + Screen.width - sliderMargin - scrollviewSlider.textureSlider.width;

                    sliderPosition.y = Mathf.Max(baseRect.y + topMargin + sliderMargin, baseRect.y + topMargin - scrollViewRect.y / scrollViewRect.height * (baseRect.height - topMargin - bottomMargin));

                    sliderPosition.width = scrollviewSlider.textureSlider.width;

                    sliderPosition.height = baseRect.height / scrollViewRect.height * (baseRect.height - topMargin - bottomMargin) + Mathf.Min(0f, -scrollViewRect.y / scrollViewRect.height * (baseRect.height - topMargin - bottomMargin));

                    if (sliderPosition.y + sliderPosition.height + sliderMargin > baseRect.y + (baseRect.height - bottomMargin))
                    {
                        sliderPosition.height = baseRect.y + (baseRect.height - bottomMargin) - sliderPosition.y - sliderMargin;
                    }

                    scrollviewSlider.SetShow(sliderPosition);
                }
            }
        }

        public void InertiaScrollViewWithPullRefresh(ref Rect scrollViewRect, Rect baseRect, float pullRefleshHeight, Action OnPullDownReflesh, Action OnPullUpReflesh)
        {
            InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, pullRefleshHeight, 0f, 0f, OnPullDownReflesh, OnPullUpReflesh);
        }

        public void InertiaScrollViewWithPullRefresh(ref Rect scrollViewRect, Rect baseRect, float pullRefleshHeight, float topMargin, float bottomMargin, Action OnPullDownReflesh, Action OnPullUpReflesh)
        {
            if (ControlLock)
            {
                scrollviewSlider.Invisible();

                return;
            }

            if (FASGesture.DragDirec != FASGesture.DragDirection.Vertical)
            {
                return;
            }

            if (inertiaEscapeThisFrame)
            {
                inertiaEscapeThisFrame = false;

                return;
            }


            //  Scroll process
			if ( postDrawed && ( (FASGesture.IsDragging && baseRect.Contains(FASGesture.TouchPosition)) || ( FASGesture.Inertia && ( baseRect.Contains(FASGesture.DragStartPosition) || baseRect.Contains(FASGesture.DragEndPosition)))))
            {
                scrollViewRect.y -= FASGesture.Delta.y;

                if (scrollViewRect.y > 0.0f)
                {
                    scrollViewRect.y += FASGesture.Delta.y * scrollViewRect.y / baseRect.height;
                }
                else if (scrollViewRect.y < baseRect.height - scrollViewRect.height)
                {
                    scrollViewRect.y += FASGesture.Delta.y * Mathf.Abs(scrollViewRect.height + scrollViewRect.y - baseRect.height) / baseRect.height;
                }

                if (FASGesture.IsDragging)
                {
                    if (OnPullDownReflesh != null && scrollViewRect.height < baseRect.height)
                    {
                        if (scrollViewRect.y > pullRefleshHeight)
                        {
                            OnPullDownReflesh();
                        }
                    }
                    else if (OnPullDownReflesh != null && scrollViewRect.y > pullRefleshHeight)
                    {
                        OnPullDownReflesh();
                    }
                    else if (OnPullUpReflesh != null && scrollViewRect.y + scrollViewRect.height < baseRect.height - pullRefleshHeight)
                    {
                        OnPullUpReflesh();
                    }
                }
            }
            else if (postDrawed && (FASGesture.IsTouchEnd && !FASGesture.Inertia) || FASGesture.IsInertiaEnd)
            {
                iTween.StopByName(ResetPositionTweenName);
                
                if (baseRect.height > scrollViewRect.height)
                {
                    if (OnPullDownReflesh != null && scrollViewRect.y > pullRefleshHeight && pullRefleshing)
                    {
                        iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollViewRect.y, "to", pullRefleshHeight, "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteScrollViewPosition"));
                    }
                    else
                    {
                        iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollViewRect.y, "to", 0f, "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteScrollViewPosition"));
                    }
                }
                else if (scrollViewRect.y > 0.0f && baseRect.height < scrollViewRect.height)
                {
                    if (OnPullDownReflesh != null && scrollViewRect.y > pullRefleshHeight && pullRefleshing)
                    {
                        iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollViewRect.y, "to", pullRefleshHeight, "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteScrollViewPosition"));
                    }
                    else
                    {
                        iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollViewRect.y, "to", 0f, "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteScrollViewPosition"));
                    }
                }
                else if (scrollViewRect.y < baseRect.height - scrollViewRect.height && baseRect.height < scrollViewRect.height)
                {
                    if (OnPullUpReflesh != null && scrollViewRect.y + scrollViewRect.height < baseRect.height - pullRefleshHeight && pullRefleshing)
                    {
                        iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollViewRect.y, "to", baseRect.height - scrollViewRect.height - pullRefleshHeight - bottomMargin, "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteScrollViewPosition"));
                    }
                    else
                    {
                        iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollViewRect.y, "to", baseRect.height - scrollViewRect.height - bottomMargin, "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteScrollViewPosition"));
                    }
                }
            }            

            if (postDrawed && FASGesture.Inertia)
            {
                if (scrollViewRect.y > 0f)
                {
                    FASGesture.InertiaBrake(scrollViewRect.y);
                }
                else if (scrollViewRect.y < -scrollViewRect.height + baseRect.height)
                {
                    FASGesture.InertiaBrake(scrollViewRect.y + scrollViewRect.height - baseRect.height);
                }
            }

            postDrawed = true;

            if (useSlider)
            {
                if (baseRect.height < scrollViewRect.height)
                {
                    Rect sliderPosition = new Rect();

                    sliderPosition.x = Position.x + Screen.width - sliderMargin - scrollviewSlider.textureSlider.width;
                    
                    sliderPosition.y = Mathf.Max(baseRect.y + topMargin + sliderMargin, baseRect.y + topMargin - scrollViewRect.y / scrollViewRect.height * (baseRect.height - topMargin - bottomMargin));
                    
                    sliderPosition.width = scrollviewSlider.textureSlider.width;
                    
                    sliderPosition.height = baseRect.height / scrollViewRect.height * (baseRect.height - topMargin - bottomMargin) + Mathf.Min(0f, -scrollViewRect.y / scrollViewRect.height * (baseRect.height - topMargin - bottomMargin));

                    if (sliderPosition.y + sliderPosition.height + sliderMargin > baseRect.y + (baseRect.height - bottomMargin))
                    {
                        sliderPosition.height = baseRect.y + (baseRect.height - bottomMargin) - sliderPosition.y - sliderMargin;
                    }

                    scrollviewSlider.SetShow(sliderPosition);
                }
            }
        }

        public void OnCompletePullReflesh(Rect scrollViewRect, Rect baseRect, float topMargin, float bottomMargin)
        {
            if (!FASGesture.IsTouching)
            {
                iTween.StopByName(ResetPositionTweenName);

                if (baseRect.height > scrollViewRect.height)
                {
                    iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollViewRect.y, "to", topMargin, "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteScrollViewPosition"));
                }
                else if (scrollViewRect.y > topMargin && baseRect.height < scrollViewRect.height)
                {
                    iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollViewRect.y, "to", topMargin, "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteScrollViewPosition"));
                }
                else if (scrollViewRect.y < baseRect.height - scrollViewRect.height - bottomMargin && baseRect.height < scrollViewRect.height)
                {
                    iTween.ValueTo(this.gameObject, iTween.Hash("name", ResetPositionTweenName, "from", scrollViewRect.y, "to", baseRect.height - scrollViewRect.height - bottomMargin, "time", resetTweenTime, "easetype", resetTweenEaseType, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateScrollViewPosition", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteScrollViewPosition"));
                }
            }
        }

        public void OnCompletePullReflesh(Rect scrollViewRect, Rect baseRect)
        {
            OnCompletePullReflesh(scrollViewRect, baseRect, 0f, 0f);
        }

        public abstract void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth);

        public abstract void SetDraw(bool on);
    }
}
