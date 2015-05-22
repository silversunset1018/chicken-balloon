using UnityEngine;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIGroupCard : MonoBehaviour
    {
        public bool imageLoadBlock = false;
        public bool userIconLoading = false;

        //  public
        private Texture palette;
        private Rect texCoordsBackground;
        private Rect texCoordsSeperateLine;

        public float cardHeight = 55f;

        public float sideMargin;

        public Rect userIconPosition;
		public Texture2D userIconDefault;
        public Texture2D userIconMask;

        public Rect memberNamesPosition;
        public GUIStyle guiStyleMemberNames;

        public Rect timeSpanPosition;
        public GUIStyle guiStyleTimeSpan;

        public Rect commentPosition;
        public GUIStyle guiStyleComment;
        
        private float scaleFactor;

        private bool imageLoaded;

		public float imageTweenTime = 0.5f;

		public iTween.EaseType imageTweenEasetype = iTween.EaseType.easeOutExpo;
		
        private Rect cardSeperateLinePosition;

        private GUIContent contentMemberNames = new GUIContent("");

        private GUIContent contentComment = new GUIContent("");

        private string userProfileUrl;

        public FresviiGUIButton buttonCard;

        public Rect unreadIconPosition;

        private Fresvii.AppSteroid.Models.Group group;

        public Fresvii.AppSteroid.Models.Group Group
        {
            get { return group; }

            set
            {
                group = value;

                SetUserIcons();

                CheckUnread();
            }
        }
       
        private string memberNames = "";

        public GameObject prfbFrameChat;

        public FresviiGUIFrame frameChat;

        private FresviiGUIGroupMessage frameGroupMessage;

        public float pollingInterval = 15f;

        private Color bgColor;

        private Dictionary<string, Texture2D> userIcons = new Dictionary<string, Texture2D>();

        public Texture2D iconMaskOutside;

        // Delete Slide button 
        public float deleteButtonWidth = 150f;

        private float cardXOffset;

        private bool isSlide;

        private bool isClose;

        private Rect buttonDeletePostion;

        private Rect textureCoordsDelete;

        public GUIStyle guiStyleDelete;

        private Fresvii.AppSteroid.Models.User other;

        public bool unread;

        public float iconMargin = 14;

        public void Init(Fresvii.AppSteroid.Models.Group group, float scaleFactor, FresviiGUIGroupMessage frameGroupMessage)
        {
            this.Group = group;

            this.frameGroupMessage = frameGroupMessage;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleMemberNames.font = null;

                guiStyleMemberNames.fontStyle = FontStyle.Bold;

                guiStyleTimeSpan.font = null;

                guiStyleComment.font = null;

                guiStyleDelete.font = null;
            }

            userIconPosition = FresviiGUIUtility.RectScale(userIconPosition, scaleFactor);

            memberNamesPosition = FresviiGUIUtility.RectScale(memberNamesPosition, scaleFactor);

            timeSpanPosition = FresviiGUIUtility.RectScale(timeSpanPosition, scaleFactor);

            commentPosition = FresviiGUIUtility.RectScale(commentPosition, scaleFactor);

            unreadIconPosition = FresviiGUIUtility.RectScale(unreadIconPosition, scaleFactor);

            iconMargin *= scaleFactor;

            sideMargin = userIconPosition.x;

            cardHeight *= scaleFactor;

            guiStyleMemberNames.fontSize = (int)(guiStyleMemberNames.fontSize * scaleFactor);

            palette = FresviiGUIColorPalette.Palette;

            guiStyleMemberNames.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

            texCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            bgColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardBackground);

            texCoordsSeperateLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.GroupCardTopShadowLine);

            guiStyleTimeSpan.fontSize = (int)(guiStyleTimeSpan.fontSize * scaleFactor);

            guiStyleComment.fontSize = (int)(guiStyleComment.fontSize * scaleFactor);

            guiStyleTimeSpan.normal.textColor = guiStyleComment.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText2);

            textureCoordsDelete = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardDeleteBackground);

            deleteButtonWidth *= scaleFactor;

            guiStyleDelete.fontSize = (int)(guiStyleDelete.fontSize * scaleFactor);

            guiStyleDelete.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText1);

			SetUserIcons();
            
        }

        void CheckUnread()
        {
            if (Group.LatestMessage == null)
            {
                unread = false;
            }
            else
            {
                unread = (Group.LastReadMessageId != Group.LatestMessage.Id) || string.IsNullOrEmpty(Group.LastReadMessageId);
            }

            if (Group.LatestMessage == null)
            {
                unread = false;
            }
            else if (Group.LatestMessage.User.Id == FAS.CurrentUser.Id)
            {
                unread = false;
            }        
        }

		public void SetUserIcons()
		{
            if (Group.Pair && Group.MessageMembers != null)
            {
                for (int i = 0; i < Group.MessageMembers.Count; i++)
                {
                    if (Group.MessageMembers[i].Id != FAS.CurrentUser.Id)
                    {
                        other = Group.MessageMembers[i].ToUser();

                        FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(Group.MessageMembers[i].ProfileImageUrl, true, delegate(Texture2D tex)
                        {
                            if (!userIcons.ContainsKey(Group.MessageMembers[i].ProfileImageUrl))
                            {
                                userIcons.Add(Group.MessageMembers[i].ProfileImageUrl, tex);
                            }
                        });

                        break;
                    }
                }
            }
			else if (Group.MessageMembers != null)
			{
                if (Group.MessageMembers.Count >= 1)
				{
                    FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(Group.MessageMembers[0].ProfileImageUrl, true, delegate(Texture2D tex)
					                                                               {
						if (!userIcons.ContainsKey(Group.MessageMembers[0].ProfileImageUrl))
						{
							userIcons.Add(Group.MessageMembers[0].ProfileImageUrl, tex);
						}
					});
				}

				if (Group.MessageMembers.Count >= 2)
				{
                    FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(Group.MessageMembers[1].ProfileImageUrl, true, delegate(Texture2D tex)
					                                                               {
						if (!userIcons.ContainsKey(Group.MessageMembers[1].ProfileImageUrl))
						{
							userIcons.Add(Group.MessageMembers[1].ProfileImageUrl, tex);
						}
					});
				}
				
                if (Group.MessageMembers.Count >= 3)
				{
                    FresviiGUIManager.Instance.resourceManager.TextureFromCacheOrDownloadOrMemory(Group.MessageMembers[2].ProfileImageUrl, true, delegate(Texture2D tex)
					{
						if (!userIcons.ContainsKey(Group.MessageMembers[2].ProfileImageUrl))
						{
							userIcons.Add(Group.MessageMembers[2].ProfileImageUrl, tex);
						}
					});
				}
			}

            SetGroupMemberNames();

            postScreenWidth = 0f;
		}

        void SetGroupMemberNames()
        {
            memberNames = "";

            if (Group.MembersCount <= 1)
            {
                memberNames = FresviiGUIText.Get("NobodyElse");

                contentMemberNames = new GUIContent(FresviiGUIUtility.Truncate(memberNames, guiStyleMemberNames, memberNamesPosition.width, "..."));
            }
            else
            {
                if (Group.Pair && other != null)
                {
                    memberNames = other.Name;

                    /*for (int i = 0; i < Group.MessageMembers.Count; i++)
                    {
                        if (Group.MessageMembers[i].Id != FAS.CurrentUser.Id)
                        {
                            memberNames += Group.MessageMembers[i].Name;

                            break;
                        }
                    }*/
                }
                else
                {
                    for (int i = 0; i < Group.MessageMembers.Count; i++)
                    {
                        memberNames += Group.MessageMembers[i].Name + ((i == Group.MessageMembers.Count - 1) ? "" : ", ");

                        if (i == 5) break;
                    }
                }

                if (!Group.Pair)
                {
                    memberNames += "(" + Group.MembersCount + ")";
                }

                GUIContent content = new GUIContent(memberNames);

                if (guiStyleMemberNames.CalcSize(content).x <= memberNamesPosition.width)
                {
                    contentMemberNames = new GUIContent(memberNames);
                }
                else
                {
                    if (Group.Pair)
                    {
                        contentMemberNames = new GUIContent(FresviiGUIUtility.Truncate(memberNames, guiStyleMemberNames, memberNamesPosition.width, "..."));
                    }
                    else
                    {
                        contentMemberNames = new GUIContent(FresviiGUIUtility.Truncate(memberNames, guiStyleMemberNames, memberNamesPosition.width, "...(" + Group.MembersCount + ")"));
                    }
                }
            }
        }

        public System.DateTime LatestMessageUpdatedAt()
        {
            if (Group.LatestMessage != null)
            {
                return Group.LatestMessage.UpdatedAt;
            }
            else
            {
                return System.DateTime.MinValue;
            }
        }

		public System.DateTime LatestMessageCreatedAt()
		{
			if (Group.LatestMessage != null)
			{
				return Group.LatestMessage.CreatedAt;
			}
			else
			{
				return System.DateTime.MinValue;
			}
		}

        

        private void CalcLayout(float width)
        {
            cardSeperateLinePosition = new Rect(0, 0, width, 20);

            memberNamesPosition.width = width - memberNamesPosition.x - timeSpanPosition.width - sideMargin;

            timeSpanPosition = new Rect(width - sideMargin - timeSpanPosition.width, timeSpanPosition.y, timeSpanPosition.width, timeSpanPosition.height);

            if (unread)
            {
                commentPosition.x = memberNamesPosition.x + unreadIconPosition.width + iconMargin;

                commentPosition.width = width - commentPosition.x - sideMargin;
            }
            else
            {
                commentPosition.x = memberNamesPosition.x;

                commentPosition.width = width - commentPosition.x - sideMargin;
            }

            if(Group.LatestMessage != null)
            {
                latestMessageId = Group.LatestMessage.Id;

                if (string.IsNullOrEmpty(Group.LatestMessage.ImageThumbnailUrl))
                {
                    string comment = FresviiGUIUtility.Truncate(Group.LatestMessage.Text, guiStyleComment, commentPosition.width, "...");

					comment = (comment.Length > 40) ? comment.Substring(0, 40) : comment;

                    contentComment = new GUIContent(comment);

                    Vector2 size = guiStyleComment.CalcSize(contentComment);

                    if (size.x < commentPosition.width && size.y > commentPosition.height)
                    {
                        string[] comments = comment.Split('\n');
   
                        contentComment = new GUIContent(FresviiGUIUtility.Truncate(comments[0] + "...", guiStyleComment, commentPosition.width, "..."));
                    }
                }
                else
                {
                    if (Group.LatestMessage.User.Id == FAS.CurrentUser.Id)
                    {
                        contentComment = new GUIContent(FresviiGUIUtility.Truncate(FresviiGUIText.Get("You") + FresviiGUIText.Get("SentAPhoto"), guiStyleComment, commentPosition.width, "..."));
                    }
                    else
                    {
                        contentComment = new GUIContent(FresviiGUIUtility.Truncate(Group.LatestMessage.User.Name + FresviiGUIText.Get("SentAPhoto"), guiStyleComment, commentPosition.width, "..."));
                    }
                }
            }
            else
            {
                contentComment = new GUIContent("");
            }

            SetGroupMemberNames();
        }

        public float GetHeight()
        {
            return cardHeight;
        }

        public void UpdateGroupCard()
        {
            CalcLayout(Screen.width);
        }

        private bool userIconLoaded;

        private bool clipImageLoaded;

		private bool isOpenAnimating = false;

        private float postScreenWidth;

        void Update()
        {
            bool preUnread = unread;

            if (isSlide && (!frameGroupMessage.CardIsOpen || cardXOffset > 0))
            {
                frameGroupMessage.OnCardOpenStateChanged(true);

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
                    frameGroupMessage.OnCardOpenStateChanged(false);
                }
            }

			if (cardXOffset > 0.0f && (FASGesture.IsTouchEnd) && !isClose && !isOpenAnimating)
            {
                CardOpenAnimation(cardXOffset > 0.5f * deleteButtonWidth);
            }

            if (isClose)
            {
                isClose = false;

                CardOpenAnimation(false);

                frameGroupMessage.OnCardOpenStateChanged(false);
            }

			if(postScreenWidth != Screen.width || preUnread != unread)
			{
				postScreenWidth = Screen.width;
	        
				CalcLayout(Screen.width);
			}
            else if (Group.LatestMessage != null)
            {
                if (latestMessageId != Group.LatestMessage.Id)
                {
                    latestMessageId = Group.LatestMessage.Id;

                    CalcLayout(Screen.width);
                }
            }

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
            frameGroupMessage.OnCardOpenStateChanged(open);

            cardXOffset = (open) ? deleteButtonWidth : 0.0f;

			isOpenAnimating = false;
        }

        private bool isDelete;

        private void DeleteAnimation()
        {
            isDelete = true;

            iTween.ValueTo(this.gameObject, iTween.Hash("time", 0.25f, "from", cardXOffset, "to", Screen.width, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "OnUpdateCardOffset", "oncomplete", "OnCardGone"));
        }

        public void OnCardGone()
        {
            iTween.ValueTo(this.gameObject, iTween.Hash("time", 0.25f, "from", cardHeight, "to", 0.0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "OnUpdateCardHeight", "oncomplete", "OnCompleteCardHeight"));
        }

        private void OnUpdateCardHeight(float value)
        {
            cardHeight = value;
        }

        private void OnCompleteCardHeight()
        {
            frameGroupMessage.DeleteGroupCard(this);

            frameGroupMessage.OnCardOpenStateChanged(false);
        }

        string latestMessageId = "";

        public void Draw(Rect position)
        {
            Event e = Event.current;

            isSlide = (FASGesture.IsTouching && position.Contains(e.mousePosition) && FASGesture.DragDirec == FASGesture.DragDirection.Horizontal);

            if (FASGesture.IsTouchBegin && cardXOffset > 0 && !position.Contains(e.mousePosition))
            {
                isClose = true;

				isSlide = false;
            }

            //  Holding  - menu apper
            if (position.Contains(e.mousePosition) && FASGesture.IsHolding && !FASGesture.IsDragging && !frameGroupMessage.HasActionSheet && !frameGroupMessage.ControlLock)
            {
                frameGroupMessage.ShowActionSheet(this);
            }                      

            Rect slidedPosition = new Rect(position.x - cardXOffset, position.y, position.width + cardXOffset, position.height);

            GUI.DrawTextureWithTexCoords(position, palette, texCoordsBackground);

            GUI.BeginGroup(slidedPosition);

            //  Delete button
            buttonDeletePostion = new Rect(slidedPosition.width - deleteButtonWidth, 0, deleteButtonWidth, cardHeight);

            GUI.DrawTextureWithTexCoords(buttonDeletePostion, palette, textureCoordsDelete);

            if (!isDelete && cardXOffset > 0)
                GUI.Label(buttonDeletePostion, FresviiGUIText.Get("Delete"), guiStyleDelete);

            if (cardXOffset == deleteButtonWidth)
            {
                if (e.type == EventType.MouseUp && buttonDeletePostion.Contains(e.mousePosition) && !FASGesture.IsDragging && !isDelete)
                {
                    e.Use();

                    if (this.Group.Pair)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("Yes"), FresviiGUIText.Get("No"), FresviiGUIText.Get("Close"));

                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("NotArrowDeletePair"), delegate(bool del) { });

                        isClose = true;

                        isSlide = false;
                    }
                    else
                    {
                        DeleteAnimation();
                    }
                }
            }

			GUI.DrawTextureWithTexCoords(cardSeperateLinePosition, palette, texCoordsSeperateLine);

			GUI.DrawTextureWithTexCoords(new Rect(0, 1, position.width, position.height - 1), palette, texCoordsBackground);


            //  UserIcon -----------------------
            #region UserIconSplitDraw
            if (Group.Pair && other != null)
            {
                Texture2D userIcon0 = null;

                if (userIcons.ContainsKey(other.ProfileImageUrl))
                {
                    userIcon0 = userIcons[other.ProfileImageUrl];
                }

                Texture2D textureUserIcon = (userIcon0 == null) ? userIconDefault : userIcon0;

                GUI.DrawTexture(userIconPosition, textureUserIcon, ScaleMode.ScaleToFit);
            }
            else if (Group.MessageMembers != null)
            {
                if (Group.MessageMembers.Count == 1)
                {
                    Texture2D userIcon0 = null;

                    if (userIcons.ContainsKey(Group.MessageMembers[0].ProfileImageUrl))
                    {
                        userIcon0 = userIcons[Group.MessageMembers[0].ProfileImageUrl];
                    }

                    Texture2D textureUserIcon = (userIcon0 == null) ? userIconDefault : userIcon0;

                    GUI.DrawTexture(userIconPosition, textureUserIcon, ScaleMode.ScaleToFit);
                }
                else if (Group.MessageMembers.Count == 2)
                {
                    Texture2D userIcon0 = null;

                    if (userIcons.ContainsKey(Group.MessageMembers[0].ProfileImageUrl))
                    {
                        userIcon0 = userIcons[Group.MessageMembers[0].ProfileImageUrl];
                    }

                    Texture2D textureUserIcon0 = (userIcon0 == null) ? userIconDefault : userIcon0;

                    GUI.DrawTexture(new Rect(userIconPosition.x, userIconPosition.y, userIconPosition.width * 0.5f, userIconPosition.height), textureUserIcon0, ScaleMode.ScaleAndCrop);

                    Texture2D userIcon1 = null;

                    if (userIcons.ContainsKey(Group.MessageMembers[1].ProfileImageUrl))
                    {
                        userIcon1 = userIcons[Group.MessageMembers[1].ProfileImageUrl];
                    }

                    Texture2D textureUserIcon1 = (userIcon1 == null) ? userIconDefault : userIcon1;

                    GUI.DrawTexture(new Rect(userIconPosition.x + userIconPosition.width * 0.5f, userIconPosition.y, userIconPosition.width * 0.5f, userIconPosition.height), textureUserIcon1, ScaleMode.ScaleAndCrop);
                }
                else if (Group.MessageMembers.Count >= 3)
                {
                    Texture2D userIcon0 = null;

                    if (userIcons.ContainsKey(Group.MessageMembers[0].ProfileImageUrl))
                    {
                        userIcon0 = userIcons[Group.MessageMembers[0].ProfileImageUrl];
                    }

                    Texture2D textureUserIcon0 = (userIcon0 == null) ? userIconDefault : userIcon0;

                    GUI.DrawTexture(new Rect(userIconPosition.x, userIconPosition.y, userIconPosition.width, 0.5f * userIconPosition.height), textureUserIcon0, ScaleMode.ScaleAndCrop);

                    Texture2D userIcon1 = null;

                    if (userIcons.ContainsKey(Group.MessageMembers[1].ProfileImageUrl))
                    {
                        userIcon1 = userIcons[Group.MessageMembers[1].ProfileImageUrl];
                    }

                    Texture2D textureUserIcon1 = (userIcon1 == null) ? userIconDefault : userIcon1;

                    GUI.DrawTexture(new Rect(userIconPosition.x, userIconPosition.y + 0.5f * userIconPosition.height, 0.5f * userIconPosition.width, 0.5f * userIconPosition.height), textureUserIcon1, ScaleMode.ScaleAndCrop);

                    Texture2D userIcon2 = null;

                    if (userIcons.ContainsKey(Group.MessageMembers[2].ProfileImageUrl))
                    {
                        userIcon2 = userIcons[Group.MessageMembers[2].ProfileImageUrl];
                    }

                    Texture2D textureUserIcon2 = (userIcon2 == null) ? userIconDefault : userIcon2;

                    GUI.DrawTexture(new Rect(userIconPosition.x + 0.5f * userIconPosition.width, userIconPosition.y + 0.5f * userIconPosition.height, 0.5f * userIconPosition.width, 0.5f * userIconPosition.height), textureUserIcon2, ScaleMode.ScaleAndCrop);
                }
                else
                {
                    GUI.DrawTexture(userIconPosition, userIconDefault, ScaleMode.ScaleToFit);
                }
            }
            #endregion

            //  Mask
            Color tmpColor = GUI.color;
            GUI.color = bgColor;
            GUI.DrawTexture(userIconPosition, iconMaskOutside, ScaleMode.ScaleToFit);
            GUI.color = tmpColor;
  
            //  Member names -----------------------
            GUI.Label(memberNamesPosition, contentMemberNames, guiStyleMemberNames);

            if (Group.LatestMessage != null)
            {
				//  Timespan
				GUI.Label(timeSpanPosition, FresviiGUIUtility.CurrentTimeSpanShort(Group.LatestMessage.UpdatedAt), guiStyleTimeSpan);
				//	comment
                guiStyleComment.fontStyle = unread ? FontStyle.Bold : FontStyle.Normal;

                GUI.Label(commentPosition, contentComment, guiStyleComment);

                if (unread)
                {
                    GUI.DrawTexture(unreadIconPosition, FresviiGUIGroupMessage.IconUnread);
                }
            }

            GUI.EndGroup();

            if (buttonCard.IsTap(Event.current, slidedPosition) && !frameGroupMessage.ControlLock)
            {
                if (cardXOffset > 0)
                {
                    isClose = true;
                }
                else if(!frameGroupMessage.CardIsOpen)
                {
                    GoToGroupChat(true);
                }
            }
        }

        public void GoToGroupChat(bool animation)
        {
            if (frameChat != null)
                Destroy(frameChat.gameObject);

            frameChat = ((GameObject)Instantiate(prfbFrameChat)).GetComponent<FresviiGUIFrame>();

            frameChat.gameObject.GetComponent<FresviiGUIChat>().SetGroup(this.Group);

            frameChat.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, frameGroupMessage.GuiDepth - 1);

            frameChat.transform.parent = this.transform;

            frameChat.SetDraw(true);

            frameChat.PostFrame = frameGroupMessage;

			frameGroupMessage.ControlLock = true;

            if (frameGroupMessage.tabBar != null)
            {
                frameGroupMessage.tabBar.enabled = false;
            }

            if (animation)
            {
                frameGroupMessage.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
                {
                    frameGroupMessage.SetDraw(false);

                    frameGroupMessage.ControlLock = false;
                });

                frameChat.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
            }
            else
            {
                frameGroupMessage.Tween(new Vector2(-Screen.width, 0.0f), new Vector2(-Screen.width, 0.0f), delegate()
                {
                    frameGroupMessage.SetDraw(false);

                    frameGroupMessage.ControlLock = false;
                });

                frameChat.Tween(Vector2.zero, Vector2.zero, delegate() { });
            }

            frameChat.gameObject.GetComponent<FresviiGUIChat>().OnAddGroupMessage += OnAddGroupMessage;
        }

        void OnAddGroupMessage(Fresvii.AppSteroid.Models.GroupMessage groupMessage)
        {
			this.Group.LatestMessage = groupMessage;

			SetUserIcons();

            frameGroupMessage.Sort();
        }

        void OnDestroy(){

            if (frameChat != null)
                frameChat.gameObject.GetComponent<FresviiGUIChat>().OnAddGroupMessage -= OnAddGroupMessage;

            if (frameChat != null)
                Destroy(frameChat);

            for(int i = 0; i < Group.MessageMembers.Count; i++)
            {
                FresviiGUIManager.Instance.resourceManager.ReleaseTexture(Group.MessageMembers[i].ProfileImageUrl);
            }
		}
	}
}