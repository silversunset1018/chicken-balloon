using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIDirectMessageCard : MonoBehaviour
    {
        //  public
        private Texture palette;

        private Rect texCoordsBackground;
    
        public float cardHeight = 100f;

        private float sideMargin = 10f;

        public Rect directMessageThumbnailPosition;
        
        public Rect directMessageTitlePosition;
        
        private Rect menuPosition;
        
        public GUIStyle guiStyleDirectMessageTitle;

        public GUIStyle guiStyleUpdatedDateTime;

        private bool thumbnailLoaded;

        private Rect buttonPosition;

        public float imageTweenTime = 0.5f;

        public iTween.EaseType imageTweenEasetype = iTween.EaseType.easeOutExpo;

        private GUIContent contentDirectMessageSubject;

        public Fresvii.AppSteroid.Models.DirectMessage DirectMessage { get; set; }

        public FresviiGUIButton buttonCard;

        private FresviiGUIDirectMessageList parentFrame;
        
        public GUIStyle guiStyleButtonShare;

        private GUIContent contentUpdatedDateTime;

        private FresviiGUIDirectMessageList.Mode mode;

        public float menuButtonMargin = 16;

        private Rect menuButtonPosition;
        
        private Rect menuButtonHitPosition;

        private Fresvii.AppSteroid.Gui.PopUpBalloonMenu popUpBaloonMenu;

        public bool HasPopUp { get; protected set; }

        public FresviiGUIButton buttonMenu;

        public Vector2 popUpOffset;

        public Rect unreadIconPosition;

        public void Init(Fresvii.AppSteroid.Models.DirectMessage directMessage, float scaleFactor, FresviiGUIDirectMessageList parentFrame)
        {
            this.DirectMessage = directMessage;

            this.parentFrame = parentFrame;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleDirectMessageTitle.font = null;

                guiStyleDirectMessageTitle.fontStyle = FontStyle.Bold;

                guiStyleButtonShare.font = null;

                guiStyleUpdatedDateTime.font = null;
            }

            directMessageTitlePosition = FresviiGUIUtility.RectScale(directMessageTitlePosition, scaleFactor);

            unreadIconPosition = FresviiGUIUtility.RectScale(unreadIconPosition, scaleFactor);

            sideMargin *= scaleFactor;

            cardHeight *= scaleFactor;

            menuButtonMargin *= scaleFactor;

            popUpOffset *= scaleFactor;

            guiStyleDirectMessageTitle.fontSize = (int)(guiStyleDirectMessageTitle.fontSize * scaleFactor);

            palette = FresviiGUIColorPalette.Palette;

            guiStyleDirectMessageTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            guiStyleButtonShare.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            guiStyleButtonShare.fontSize = (int)(guiStyleButtonShare.fontSize * scaleFactor);

            guiStyleUpdatedDateTime.fontSize = (int)(guiStyleUpdatedDateTime.fontSize * scaleFactor);

            guiStyleDirectMessageTitle.padding = FresviiGUIUtility.RectOffsetScale(guiStyleDirectMessageTitle.padding, scaleFactor);
            
            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            contentDirectMessageSubject = new GUIContent(directMessage.Subject);

            directMessageTitlePosition = new Rect(directMessageTitlePosition.x, directMessageTitlePosition.y, Screen.width, cardHeight);
        }

		float postScreenWidth;

        private void CalcLayout(float width)
        {
			if(Screen.width != postScreenWidth){

				contentDirectMessageSubject = new GUIContent( FresviiGUIUtility.Truncate( DirectMessage.Subject, guiStyleDirectMessageTitle, width - directMessageTitlePosition.x - menuButtonHitPosition.width, "..."));

                directMessageTitlePosition = new Rect(directMessageTitlePosition.x, directMessageTitlePosition.y, Screen.width, cardHeight);

                postScreenWidth = Screen.width;
			}
        }

        public float GetHeight()
        {
            if (deleteAnimation)
            {
                return deleteAnimationHeight;
            }
            else
            {
                return cardHeight;
            }
        }

        bool directMessageThumbnailLoading;

        public Material directMessageThumbnailMaterial;
    
        private float alpha = 1.0f;

        public void Draw(Rect position, Rect scrollViewRect, int guiDepth, float topMenuHeight)
        {
            Color tmpColor = GUI.color;

            GUI.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, alpha);

            CalcLayout(position.width);

            GUI.BeginGroup(position);
            
            // Background
            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), palette, texCoordsBackground);

            guiStyleDirectMessageTitle.fontStyle = (DirectMessage.Unread) ? FontStyle.Bold : FontStyle.Normal;            

            GUI.Label(directMessageTitlePosition, contentDirectMessageSubject, guiStyleDirectMessageTitle);

            if (DirectMessage.Unread)
            {
                GUI.DrawTexture(unreadIconPosition, FresviiGUIDirectMessageList.IconUnread);
            }

            GUI.EndGroup();

            GUI.color = tmpColor;

            if (buttonCard.IsTap(Event.current, position) && !parentFrame.ControlLock)
            {
                GoToDirectMessage(true);
            }
        }

        public FresviiGUIFrame frameDirectMessage;

        public GameObject prfbFrameDirectMessage;

        public void GoToDirectMessage(bool animation)
        {
            if (frameDirectMessage != null)
                Destroy(frameDirectMessage.gameObject);

            frameDirectMessage = ((GameObject)Instantiate(prfbFrameDirectMessage)).GetComponent<FresviiGUIFrame>();

            frameDirectMessage.gameObject.GetComponent<FresviiGUIDirectMessage>().DirectMessage = this.DirectMessage; 

            frameDirectMessage.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, parentFrame.GuiDepth - 1);

            frameDirectMessage.transform.parent = this.transform;

            frameDirectMessage.SetDraw(true);

            frameDirectMessage.PostFrame = parentFrame;

            frameDirectMessage.ControlLock = true;

            /*if (parentFrame.tabBar != null)
            {
                frameGroupMessage.tabBar.enabled = false;
            }*/

            parentFrame.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
            {
                parentFrame.SetDraw(false);

                parentFrame.ControlLock = false;
            });

            frameDirectMessage.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });

            if (DirectMessage.Unread)
            {
                DirectMessage.Unread = false;

                FresviiGUIManager.Instance.UnreadDirectMessageCount--;
            }
        }

        private bool deleteAnimation;

        private float deleteAnimationHeight;

        public void DeleteCardAnimation()
        {
            deleteAnimationHeight = GetHeight();

            deleteAnimation = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("from", alpha, "to", 0.0f, "time", 0.25f, "onupdate", "OnUpdateAlpha", "oncomplete", "OnCompleteAlpha"));
        }

        void OnUpdateAlpha(float value)
        {
            alpha = value;
        }

        void OnCompleteAlpha()
        {
            alpha = 0.0f;

            iTween.ValueTo(this.gameObject, iTween.Hash("from", deleteAnimationHeight, "to", 0.0f, "time", 0.25f, "easetype", imageTweenEasetype, "onupdatetarget", this.gameObject, "onupdate", "OnUpdateHeight", "oncompletetarget", this.gameObject, "oncomplete", "OnCompleteHeightDelete"));
        }

        void OnUpdateHeight(float value)
        {
            parentFrame.OnScrollDelta(deleteAnimationHeight - value);

            deleteAnimationHeight = value;
        }

    }
}