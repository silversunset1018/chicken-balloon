using UnityEngine;
using System.Collections;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIButton : MonoBehaviour
    {
        public enum ButtonType { TextureOnly, FrameAndLabel, FrameIconAndLabel };

        public float activeTime = 0.5f;
        
        public ScaleMode scaleMode = ScaleMode.ScaleToFit;

        private bool touching;
        
        private bool isActive;

        public bool IsActive 
        {
            get {return isActive;}
        }

        public bool IsTap(Event e, Rect position, Rect hitPosition, ButtonType type, Texture2D textureNormal, Texture2D textureHover, Texture2D textureActive)
        {
            return IsTap(e, position, hitPosition, type, textureNormal, textureHover, textureActive, "", null);
        }

        public bool IsTap(Event e, Rect position, Rect hitPosition, ButtonType type, Texture2D textureNormal, Texture2D textureHover, Texture2D textureActive, string labelText, GUIStyle guiStyleLabel)
        {
            bool hitContain = (e.button == 0) && hitPosition.Contains(e.mousePosition);

            if (e.type == EventType.MouseDown && hitContain)
            {
                touching = true;
            }

            if (FASGesture.IsDragging || FASGesture.IsPinching)
            {
                touching = false;
            }

            if (e.type == EventType.MouseUp && hitContain && touching)
            {
                e.Use();

                if(!isActive)
                    StartCoroutine(ButtonIsActive());

                touching = false;

                return true;
            }

            Texture2D buttonTexture = (touching) ? textureHover : textureNormal;

            buttonTexture = (isActive) ? textureActive : buttonTexture;

            if (type == ButtonType.TextureOnly)
            {
                GUI.DrawTexture(position, buttonTexture, scaleMode);
            }
            else if (type == ButtonType.FrameAndLabel)
            {
                FresviiGUIUtility.DrawButtonFrame(position, buttonTexture, FresviiGUIManager.Instance.ScaleFactor);

                GUI.Label(position, labelText, guiStyleLabel);
            }

            return false;
        }

        public bool IsTap(Event e, Rect position, Rect hitPosition, Texture2D textureNormal, Texture2D textureHover, Texture2D textureActive, Rect iconPosition, Texture2D icon, Rect labelPosition, string labelText, GUIStyle guiStyleLabel)
        {
            bool hitContain = (e.button == 0) && hitPosition.Contains(e.mousePosition);

            if (e.type == EventType.MouseDown && hitContain)
            {
                touching = true;
            }

            if (FASGesture.IsDragging || FASGesture.IsPinching)
            {
                touching = false;
            }

            if (e.type == EventType.MouseUp && hitContain && touching)
            {
                e.Use();

                if (!isActive)
                    StartCoroutine(ButtonIsActive());

                touching = false;

                return true;
            }

            Texture2D buttonTexture = (touching) ? textureHover : textureNormal;

            buttonTexture = (isActive) ? textureActive : buttonTexture;

            FresviiGUIUtility.DrawButtonFrame(position, buttonTexture, FresviiGUIManager.Instance.ScaleFactor);

            GUI.DrawTexture(iconPosition, icon);

            GUI.Label(labelPosition, labelText, guiStyleLabel);

            return false;
        }

        public bool IsTap(Event e, Rect hitPosition){
		
            bool hitContain = (e.button == 0) && hitPosition.Contains(e.mousePosition);

            if (e.type == EventType.MouseDown && hitContain)
            {
                touching = true;

                isActive = true;
            }

            if (FASGesture.IsDragging || FASGesture.IsPinching)
            {
                touching = false;

                isActive = false;
            }

            if (e.type == EventType.MouseUp )
            {
                if (hitContain && touching)
                {
                    e.Use();

                    StartCoroutine(ButtonIsActive());

                    return true;
                }
                else
                {
                    isActive = false;
                }
            }

            

            return false;
        }

        private IEnumerator ButtonIsActive()
        {
            isActive = true;

            yield return new WaitForSeconds(activeTime);

            isActive = false;
        }
    }
}