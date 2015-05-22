using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIScrollviewSlider : MonoBehaviour
    {
        public Texture2D textureSlider;

        private Rect position;

        private bool showing = false;

        private float alpha = 0.0f;

        public float tweenTime = 0.25f;

        public float delayTime = 0.5f;

        private bool touching;

        [HideInInspector]
        public int GuiDepth;

        void OnEnable()
        {
            touching = false;

            alpha = 0.0f;
        }

        void OnDisable()
        {
            touching = false;

            alpha = 0.0f;
        }

        public void Invisible()
        {
            alpha = 0.0f;
        }

        public void SetShow(Rect position)
        {
            if (FASGesture.IsTouching && FASGesture.IsDragging)
                touching = true;

            touching &= FASGesture.IsTouching;

            bool show = touching || FASGesture.IsDragging || FASGesture.Inertia;

            if (showing != show)
            {
                iTween.Stop(this.gameObject);

                if (show)
                {
                    iTween.ValueTo(this.gameObject, iTween.Hash("from", alpha, "to", 1.0f, "time", tweenTime, "onupdate", "OnUpdateAlpha"));
                }
                else
                {
                    iTween.ValueTo(this.gameObject, iTween.Hash("from", alpha, "to", 0.0f, "time", tweenTime, "delay", delayTime, "onupdate", "OnUpdateAlpha"));
                }

                showing = show;
            }

            this.position = position;
        }

        void OnUpdateAlpha(float value)
        {
            alpha = value;
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            Color tmp = GUI.color;

            GUI.color = new Color(tmp.r, tmp.g, tmp.b, alpha);

            FresviiGUIUtility.DrawSplitTexture(position, textureSlider, 1.0f);

            GUI.color = tmp;
        }
    }
}