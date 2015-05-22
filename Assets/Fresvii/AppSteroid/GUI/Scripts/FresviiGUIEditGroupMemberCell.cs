using UnityEngine;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIEditGroupMemberCell : MonoBehaviour
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
        public Texture2D userIconMask;

        public Rect userNamePosition;
        public GUIStyle guiStyleUserName;

        public GUIStyle guiStyleDelete;

        private float scaleFactor;

        private string postFix;

        private bool imageLoaded;

        public float imageTweenTime = 0.5f;
        public iTween.EaseType imageTweenEasetype = iTween.EaseType.easeOutExpo;

        private GUIContent contentUserName;

        private string userProfileUrl;

        public Fresvii.AppSteroid.Models.Member member { get; set; }

        private Fresvii.AppSteroid.Models.User user;

        public FresviiGUIButton buttonCard;

        private FresviiGUIEditGroupMember frameEditGroupMembers;

        private Rect textureCoordsSeparateLine;

        private Rect textureCoordsDelete;

        public Rect seperateLinePosition;

        private Rect cardPosition;

        public Texture buttonRightIcon;

        public Rect buttonRightPosition;

        private Rect buttonDeletePostion;

        public GameObject prfbGUIFrameUserProfile;

        private FresviiGUIFrame nextFrameProfile;

        public bool IsSelected { get; protected set; }

        public float deleteButtonWidth = 150f;

        private float cardXOffset;

        private bool postIsSlide;

        private bool isSlide;

        private bool isClose;

        private Fresvii.AppSteroid.Gui.ActionSheet actionSheet;

        public Material userMask;

        public void Init(Fresvii.AppSteroid.Models.Member member, float scaleFactor, string postFix,  FresviiGUIEditGroupMember frameEditGroupMembers)
        {
            this.member = member;

            this.frameEditGroupMembers = frameEditGroupMembers;

            this.postFix = postFix;

            this.scaleFactor = scaleFactor;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleUserName.font = null;

                guiStyleUserName.fontStyle = FontStyle.Bold;

                guiStyleDelete.font = null;
            }

            userIconPosition = FresviiGUIUtility.RectScale(userIconPosition, scaleFactor);
            
            userNamePosition = FresviiGUIUtility.RectScale(userNamePosition, scaleFactor);

            sideMargin = userIconPosition.x;

            cardHeight *= scaleFactor;

            deleteButtonWidth *= scaleFactor;

            guiStyleUserName.fontSize = (int)(guiStyleUserName.fontSize * scaleFactor);

            guiStyleDelete.fontSize = (int)(guiStyleDelete.fontSize * scaleFactor);

            palette = FresviiGUIColorPalette.Palette;

            guiStyleUserName.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            guiStyleDelete.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText1);

            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            textureCoordsSeparateLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardSeperateLine2);

            textureCoordsDelete = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardDeleteBackground);

            seperateLinePosition = FresviiGUIUtility.RectScale(seperateLinePosition, scaleFactor);

            GetUser();
        }

        void GetUser()
        {
            FASUser.GetUser(member.Id, delegate(Fresvii.AppSteroid.Models.User _user, Fresvii.AppSteroid.Models.Error _error)
            {
                if (_error == null)
                {
                    this.user = _user;
                }
            });
        }

        private void CalcLayout(float width)
        {
            cardPosition = new Rect(0, 0, Screen.width, cardHeight);

            userNamePosition.width = width - userNamePosition.x - sideMargin * 2f - frameEditGroupMembers.textureCheckMark.width;

            contentUserName = new GUIContent(member.Name);

            int userNameDeleteStringNum = 2;

            while (guiStyleUserName.CalcSize(contentUserName).x > userNamePosition.width)
            {
                if (member.Name.Length - userNameDeleteStringNum < 2) break;

                contentUserName = new GUIContent(member.Name.Substring(0, member.Name.Length - userNameDeleteStringNum) + "...");

                userNameDeleteStringNum++;
            }

            seperateLinePosition = new Rect(seperateLinePosition.x, cardPosition.height - 1, cardPosition.width + cardXOffset, 1);

            buttonRightPosition = new Rect(width - buttonRightPosition.width - sideMargin, cardHeight * 0.5f - buttonRightPosition.height * 0.5f, buttonRightPosition.width, buttonRightPosition.height);
        }

        public float GetHeight()
        {
            return cardHeight;
        }

        private bool userIconLoaded;
        private bool clipImageLoaded;

        IEnumerator UserIconLoad()
        {
            if (imageLoadBlock) yield break;

            userIconLoading = true;

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                userProfileUrl = user.ProfileImageUrl;

                string url = user.ProfileImageUrl;

                string fileName = System.IO.Path.GetFileName(url);

                bool isDownload = false;

                if (Fresvii.AppSteroid.Util.ImageCache.IsExist(fileName))
                {
                    url = "file://" + Fresvii.AppSteroid.Util.ImageCache.GetFullPath(fileName);
                }
                else
                {
                    isDownload = true;
                }

                WWW www = new WWW(url);

                yield return www;

                if (string.IsNullOrEmpty(www.error))
                {
                    Texture2D tmpUserIcon = new Texture2D(1, 1);

                    www.LoadImageIntoTexture(tmpUserIcon);

                    if (userIcon != null)
                    {
                        Destroy(userIcon);
                    }

                    Fresvii.AppSteroid.Util.TextureLoader.LoadIntoTextureAlpha(ref tmpUserIcon, userIconMask);

                    userIcon = tmpUserIcon;

                    if (isDownload)
                    {
                        Fresvii.AppSteroid.Util.ImageCache.Save(fileName, www.bytes);
                    }

                    userProfileUrl = user.ProfileImageUrl;

                    userIconLoaded = true;
                }
            }

            userIconLoading = false;
        }

        public void OnSwipe()
        {
            if (user != null)
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Verbose)
                    Debug.Log("Swiped " + user.Name);
            }
        }
        
		private bool isOpenAnimating;

        void Update()
        {
			if (isSlide && (!frameEditGroupMembers.CardIsOpen || cardXOffset > 0))
			{
                frameEditGroupMembers.OnCardOpenStateChanged(true);

                if (cardXOffset > deleteButtonWidth)
                {
                    cardXOffset -= FASGesture.Delta.x - FASGesture.Delta.x * (cardXOffset - deleteButtonWidth) / deleteButtonWidth;
                }
                else
                {
                    cardXOffset -= FASGesture.Delta.x;
                }

                cardXOffset = Mathf.Clamp(cardXOffset, 0.0f, Screen.width * 0.5f);

                if (cardXOffset == 0.0f)
                {
                    frameEditGroupMembers.OnCardOpenStateChanged(false);
                }
            }

			if (cardXOffset > 0.0f && (FASGesture.IsTouchEnd || postIsSlide && !isSlide) && !isClose && !isOpenAnimating)
			{
                CardOpenAnimation(cardXOffset > 0.5f * deleteButtonWidth );
            }

            if (isClose)
            {
                isClose = false;

                CardOpenAnimation(false);

                frameEditGroupMembers.OnCardOpenStateChanged(false);
            }

            postIsSlide = isSlide;
        }

        private void CardOpenAnimation(bool open)
        {
			isOpenAnimating = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("time", 0.5f, "from", cardXOffset, "to", (open) ? deleteButtonWidth : 0.0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "OnUpdateCardOffset", "oncomplete", "OnCompleteCardOffset", "oncompleteparams", open));
        }

        private void OnUpdateCardOffset(float value)
        {
            cardXOffset = value;
        }

        private void OnCompleteCardOffset(bool open)
        {
            frameEditGroupMembers.OnCardOpenStateChanged(open);

            cardXOffset = (open) ? deleteButtonWidth : 0.0f;

			isOpenAnimating = false;
        }

        private bool isDelete;

        private void DeleteAnimation()
        {
            isDelete = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("time", 0.25f, "from", cardXOffset, "to", Screen.width, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "OnUpdateCardOffset", "oncomplete", "OnCardGone"));
        }

        void OnCardGone()
        {
            iTween.ValueTo(this.gameObject, iTween.Hash("time", 0.25f, "from", cardHeight, "to", 0.0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "OnUpdateCardHeight", "oncomplete", "OnCompleteCardHeight"));
        }

        private void OnUpdateCardHeight(float value)
        {
            cardHeight = value;
        }

        private void OnCompleteCardHeight()
        {
            frameEditGroupMembers.DeleteMemeberCell(this);

            frameEditGroupMembers.OnCardOpenStateChanged(false);
        }

        private bool ValidateDelete()
        {
            if (frameEditGroupMembers.Group.Pair)
            {
                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("NotArrowDeletePair"), delegate(bool del) { });
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        void OnDisable()
        {
            if (actionSheet != null)
            {
                actionSheet.Hide();
            }
        }

        public void Draw(Rect position, bool drawSeperateLine)
        {
            if (user != null)
            {
                if (!userIconLoaded && !userIconLoading)
                {
                    StartCoroutine(UserIconLoad());
                }
                else if (userIconLoaded && user.ProfileImageUrl != userProfileUrl)
                {
                    StartCoroutine(UserIconLoad());
                }
            }

            CalcLayout(position.width);

            Event e = Event.current;

            if (position.Contains(e.mousePosition) && FASGesture.IsHolding && !FASGesture.IsDragging && !frameEditGroupMembers.ControlLock)
            {
                List<string> buttons = new List<string>();

                buttons.Add(FresviiGUIText.Get("Delete"));

                frameEditGroupMembers.ControlLock = true;

                actionSheet = Fresvii.AppSteroid.Gui.ActionSheet.Show(this.scaleFactor, this.postFix, frameEditGroupMembers.GuiDepth - 10, buttons.ToArray(), (selectedButton) =>
                {                   
                    frameEditGroupMembers.ControlLock = false;

                    if (selectedButton == FresviiGUIText.Get("Delete"))
                    {
                        if (ValidateDelete())
                        {
                            if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
                            {
                                OnCardGone();
                            }
                            else
                            {

                                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Delete"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSelectDialog(FresviiGUIText.Get("ConfirmRemoveMember"), delegate(bool del)
                                {
                                    if (del)
                                    {
                                        OnCardGone();
                                    }
                                });
                            }
                        }
                    }
                });
            }

            isSlide = (FASGesture.IsTouching && position.Contains(e.mousePosition) && FASGesture.DragDirec == FASGesture.DragDirection.Horizontal);

            if (FASGesture.IsTouchBegin && cardXOffset > 0 && !position.Contains(e.mousePosition))
            {
                isClose = true;

				isSlide = false;
            }

            Rect slidedPosition = new Rect(position.x - cardXOffset, position.y, position.width + cardXOffset, position.height);

            GUI.DrawTextureWithTexCoords(position, palette, texCoordsBackground);

            GUI.BeginGroup(slidedPosition);

            //  Delete button
            buttonDeletePostion = new Rect(slidedPosition.width - deleteButtonWidth, 0, deleteButtonWidth, cardHeight);

            GUI.DrawTextureWithTexCoords(buttonDeletePostion, palette, textureCoordsDelete);

            if(!isDelete && cardXOffset > 0)
                GUI.Label(buttonDeletePostion, FresviiGUIText.Get("Delete"), guiStyleDelete);

            if (cardXOffset == deleteButtonWidth)
            {
                if (e.type == EventType.MouseUp && buttonDeletePostion.Contains(e.mousePosition) && !FASGesture.IsDragging && !isDelete && !frameEditGroupMembers.ControlLock)
                {
                    e.Use();

                    if (ValidateDelete())
                    {
                        DeleteAnimation();
                    }
                    else
                    {
                        isClose = true;
                    }
                }
            }

            if (e.type == EventType.MouseUp && cardPosition.Contains(e.mousePosition) && !FASGesture.IsDragging && !isDelete && !frameEditGroupMembers.ControlLock)
            {
                e.Use();

                if (cardXOffset > 0)
                {
                    isClose = true;
                }
                else if(!frameEditGroupMembers.CardIsOpen)
                {
                    if (this.user != null)
                    {
                        if (nextFrameProfile == null)
                        {
                            nextFrameProfile = ((GameObject)Instantiate(prfbGUIFrameUserProfile)).GetComponent<FresviiGUIFrame>();

                            nextFrameProfile.gameObject.GetComponent<FresviiGUIUserProfile>().SetUser(user);

                            nextFrameProfile.Init(null, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, FASGui.GuiDepthBase);

                            nextFrameProfile.transform.parent = this.transform;
                        }

                        nextFrameProfile.SetDraw(true);

                        nextFrameProfile.PostFrame = frameEditGroupMembers;

                        frameEditGroupMembers.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
                        {
                            frameEditGroupMembers.SetDraw(false);
                        });

                        nextFrameProfile.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
                    }
                }
            }

            GUI.DrawTextureWithTexCoords(cardPosition, palette, texCoordsBackground);

            GUI.DrawTexture(buttonRightPosition, buttonRightIcon);

            if(drawSeperateLine)
                GUI.DrawTextureWithTexCoords(seperateLinePosition, palette, textureCoordsSeparateLine);

            //  UserIcon -----------------------
            if (Event.current.type == EventType.Repaint)
            {
                userMask.color = new Color(1f, 1f, 1f, 1.0f);

                Graphics.DrawTexture(userIconPosition, ((userIcon == null) ? userIconDefault : userIcon), userMask);
            }

            //  User name -----------------------
            GUI.Label(userNamePosition, contentUserName, guiStyleUserName);

            GUI.EndGroup();
        }

        void OnDestroy()
        {
            if (userIcon != null)
                Destroy(userIcon);
        }
    }
}