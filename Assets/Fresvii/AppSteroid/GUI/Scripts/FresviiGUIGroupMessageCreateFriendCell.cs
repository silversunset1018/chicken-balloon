using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIGroupMessageCreateFriendCell : MonoBehaviour
    {
        public static bool imageLoadBlock = false;

        public static bool userIconLoading = false;

        //  public
        private Texture palette;
        private Rect texCoordsBackground;

        public float cardHeight = 55f;

        private float sideMargin;

        public Rect userIconPosition;
        private Texture2D userIcon;
        public Texture2D userIconDefault;
        //public Texture2D userIconMask;

        public Material userIconMask;

        public Rect userNamePosition;
        public GUIStyle guiStyleUserName;

        private float scaleFactor;

        private bool imageLoaded;

        public float imageTweenTime = 0.5f;
        public iTween.EaseType imageTweenEasetype = iTween.EaseType.easeOutExpo;

        private GUIContent contentUserName;

        private string userProfileUrl;

        public Fresvii.AppSteroid.Models.Friend friend { get; set; }

        private Fresvii.AppSteroid.Models.User user;

        public FresviiGUIButton buttonCard;

        private FresviiGUIFrame frameParent;

        private Rect checkMarkPosition;

        public bool IsSelected { get; set; }

        private Texture2D textureCheckMark;

        private Action<Fresvii.AppSteroid.Models.User, Action<bool>> AddFriend;

        private Action<Fresvii.AppSteroid.Models.User> RemoveFriend;

        private Rect textureCoordsSeparateLine;

        public Rect seperateLinePosition;

        private Rect cardPosition;

        public void Init(Fresvii.AppSteroid.Models.Friend friend, float scaleFactor, FresviiGUIFrame frameParent, Texture2D textureCheckMark, Action<Fresvii.AppSteroid.Models.User, Action<bool>> AddFriend, Action<Fresvii.AppSteroid.Models.User> RemoveFriend)
        {
            this.friend = friend;

            this.frameParent = frameParent;

            this.textureCheckMark = textureCheckMark;

            this.AddFriend = AddFriend;

            this.RemoveFriend = RemoveFriend;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleUserName.font = null;
                guiStyleUserName.fontStyle = FontStyle.Bold;
            }

            userIconPosition = FresviiGUIUtility.RectScale(userIconPosition, scaleFactor);
            userNamePosition = FresviiGUIUtility.RectScale(userNamePosition, scaleFactor);

            sideMargin = userIconPosition.x;

            cardHeight *= scaleFactor;

            guiStyleUserName.fontSize = (int)(guiStyleUserName.fontSize * scaleFactor);

            palette = FresviiGUIColorPalette.Palette;

            guiStyleUserName.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            textureCoordsSeparateLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardSeperateLine2);

            seperateLinePosition = FresviiGUIUtility.RectScale(seperateLinePosition, scaleFactor);

            GetUser();
        }

        void GetUser()
        {
            FASUser.GetUser(friend.Id, delegate(Fresvii.AppSteroid.Models.User _user, Fresvii.AppSteroid.Models.Error _error)
            {
                if (_error == null)
                {
                    this.user = _user;

                    Select(IsSelected);
                }
            });
        }

        private void CalcLayout(float width)
        {
            cardPosition = new Rect(0, 0, Screen.width, cardHeight);

            userNamePosition.width = width - userNamePosition.x - sideMargin * 2f - textureCheckMark.width;

            checkMarkPosition = new Rect(width - sideMargin - textureCheckMark.width, cardHeight * 0.5f - textureCheckMark.height * 0.5f, textureCheckMark.width, textureCheckMark.height);

            contentUserName = new GUIContent(friend.Name);

            int userNameDeleteStringNum = 2;

            while (guiStyleUserName.CalcSize(contentUserName).x > userNamePosition.width)
            {
                if (friend.Name.Length - userNameDeleteStringNum < 2) break;

                contentUserName = new GUIContent(friend.Name.Substring(0, friend.Name.Length - userNameDeleteStringNum) + "...");

                userNameDeleteStringNum++;
            }

            seperateLinePosition = new Rect(seperateLinePosition.x, cardPosition.height - 1, cardPosition.width, 1);
        }

        public float GetHeight()
        {
            return cardHeight;
        }

        private bool userIconLoaded;
        private bool clipImageLoaded;        

        private bool iconError;

        private void LoadUserIcon()
        {
            userIconLoading = true;

            userProfileUrl = user.ProfileImageUrl;

            FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(user.ProfileImageUrl, true, delegate(Texture2D texture)
            {
                userIcon = texture;

                userIconLoading = false;

                iconError = (userIcon == null);
            });
        }

        public void Draw(Rect position, bool drawSeperateLine)
        {

            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.ProfileImageUrl) && userIcon == null && !userIconLoading && !iconError)
                {
                    LoadUserIcon();
                }
                else if (user.ProfileImageUrl != userProfileUrl && !userIconLoading && !iconError)
                {
                    FresviiGUIManager.Instance.resourceManager.ReleaseTexture(userProfileUrl);

                    LoadUserIcon();
                }
            }

            CalcLayout(position.width);

            GUI.BeginGroup(position);

            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), palette, texCoordsBackground);

            if (drawSeperateLine)
                GUI.DrawTextureWithTexCoords(seperateLinePosition, palette, textureCoordsSeparateLine);

            //  UserIcon -----------------------
            if (Event.current.type == EventType.Repaint)
            {
                userIconMask.color = new Color(1f, 1f, 1f, 1f);

                Graphics.DrawTexture(userIconPosition, (userIcon == null ? userIconDefault : userIcon), userIconMask);
            }

            //  User name -----------------------
            GUI.Label(userNamePosition, contentUserName, guiStyleUserName);

            //   Check mark
            if (IsSelected)
            {
                GUI.DrawTexture(checkMarkPosition, textureCheckMark);
            }

            GUI.EndGroup();

            if (buttonCard.IsTap(Event.current, position) && !frameParent.ControlLock)
            {
                IsSelected = !IsSelected;

                Select(IsSelected);
            }
        }

        public void Select(bool on)
        {
            if (on)
            {
                if (AddFriend != null && this.user != null)
                {
                    AddFriend(this.user, (added) =>
                    {
                        if (!added)
                            IsSelected = false;
                    });
                }
            }
            else
            {
                if (RemoveFriend != null && this.user != null)
                    RemoveFriend(this.user);
            }
        }

        void OnDestroy()
        {
            if (userIcon != null)
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(userProfileUrl);

        }
    }
}