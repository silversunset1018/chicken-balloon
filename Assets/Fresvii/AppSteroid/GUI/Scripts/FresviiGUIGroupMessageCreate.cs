using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIGroupMessageCreate : FresviiGUIFrame
    {
        public float sideMargin;

        private Texture2D palette;

        private FresviiGUIGroupMessageCreateTop groupMessageCreateTopMenu;

        private Rect baseRect;
       
        private Rect scrollViewRect;
        
        private float scaleFactor;
        
        public float topMargin;

        public float cardMargin;

        private Vector2 scrollPosition = Vector2.zero;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;
        
        public Vector2 loadingSpinnerSize;
        
        private IList<Fresvii.AppSteroid.Models.Friend> friends = new List<Fresvii.AppSteroid.Models.Friend>();

        private List<FresviiGUIGroupMessageCreateFriendCell> friendCells = new List<FresviiGUIGroupMessageCreateFriendCell>();

        public GameObject prfbGroupMessageFriendCell;

        private bool initialized;

		public float pollingInterval = 15f;

        private FresviiGUIAddCommentBottomMenu addCommentBottomMenu;

        [HideInInspector]
        public Texture2D textureCheckMark;

        public float heightMessageTo;

        private Rect textureCoordsTo;

        private Rect toPositon;

        public GUIStyle guiStyleLabelTo;

        private Rect labelToPosition;

        public GUIStyle guiStyleLableToUsers;

        private Rect labelToUsersPosition;
        
        private GUIContent contentLabelTo = new GUIContent("");

        public float toMargin = 5f;

        public List<Fresvii.AppSteroid.Models.User> groupFriends = new List<Fresvii.AppSteroid.Models.User>();

        private GUIContent contentUserNames;

        private string userNames = "";

        public GameObject prfbFrameChat;

        public bool IsCreateFromPairGroup { get; set; }

        private FresviiGUIFrame frameChat;

        private Fresvii.AppSteroid.Models.User initCheckedUser;

        private TouchScreenKeyboard keyboard;

        public string searchingText = "";

        public Color userNameColor, searchTextColor;

        public Fresvii.AppSteroid.Models.User OfficialUser;
        
        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            ControlLock = false;

            this.GuiDepth = guiDepth;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleLabelTo.font = null;

                guiStyleLableToUsers.font = null;
            }

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            groupMessageCreateTopMenu = GetComponent<FresviiGUIGroupMessageCreateTop>();

            groupMessageCreateTopMenu.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

            guiStyleLabelTo.fontSize = (int)(guiStyleLabelTo.fontSize * scaleFactor);

            guiStyleLabelTo.padding = FresviiGUIUtility.RectOffsetScale(guiStyleLabelTo.padding, scaleFactor);

            guiStyleLableToUsers.fontSize = (int)(guiStyleLableToUsers.fontSize * scaleFactor);

            addCommentBottomMenu = GetComponent<FresviiGUIAddCommentBottomMenu>();

            addCommentBottomMenu.Init(postFix, scaleFactor, GuiDepth - 1, this, AddComment);

            addCommentBottomMenu.autoSendImageLoaded = true;

            this.scaleFactor = scaleFactor;
            
            toMargin *= scaleFactor;
			
            loadingSpinnerSize *= scaleFactor;
            
            sideMargin *= scaleFactor;
            
            topMargin = scaleFactor;

            heightMessageTo *= scaleFactor;

            scrollPosition.y = 0.0f;

            textureCoordsTo = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.CardBackground);

            textureCheckMark = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconFriendTextureName + postFix, false);

            FASFriendship.GetAccountFriendList(OnGetFriends);

            contentLabelTo = new GUIContent(FresviiGUIText.Get("To"));

			loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
			
			loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GuiDepth - 10);

            SetScrollSlider(scaleFactor * 2.0f);

            addCommentBottomMenu.SetSendEnableAtAction(false, FresviiGUIText.Get("Error"), FresviiGUIText.Get("NoSelectedMember"));

            FASUser.GetOfficialUser((user, error) =>
            {
                if (error == null)
                {
                    OfficialUser = user;
                }
            });
        }
        
        public void SetInitCheckedUser(Fresvii.AppSteroid.Models.User user)
        {
            if (user != null)
            {
                initCheckedUser = user;

                AddFriendToGroup(initCheckedUser, null);
            }
        }

        void OnGetFriends(IList<Fresvii.AppSteroid.Models.Friend> friends, Fresvii.AppSteroid.Models.Error error)
        {
            if (this == null) return;

			loadingSpinner.Hide();

            if (error == null)
            {
                this.friends = friends;

                if (initCheckedUser != null)
                {
                    bool isExist = false;

                    foreach (Fresvii.AppSteroid.Models.Friend friend in this.friends)
                    {
                        if (friend.Id == initCheckedUser.Id)
                        {
                            isExist = true;

                            break;
                        }
                    }

                    if (!isExist)
                    {
                        this.friends.Insert(0, initCheckedUser.ToFriend());
                    }
                }

                foreach (Fresvii.AppSteroid.Models.Friend friend in this.friends)
                {
                    FresviiGUIGroupMessageCreateFriendCell friendCell = ((GameObject)Instantiate(prfbGroupMessageFriendCell)).GetComponent<FresviiGUIGroupMessageCreateFriendCell>();

                    friendCell.transform.parent = this.transform;

                    friendCell.Init(friend, scaleFactor, this, textureCheckMark, AddFriendToGroup, RemoveFriendToGroup);

                    if(initCheckedUser != null)
                        friendCell.IsSelected = (friend.Id == initCheckedUser.Id);

                    friendCells.Add(friendCell);
                }
            }
            else
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                    Debug.LogError(error.ToString());
            }
            
        }

        void CalcLayout()
        {
            toPositon = new Rect(0f, 0f, baseRect.width, heightMessageTo);

            labelToPosition = toPositon;

            labelToUsersPosition = new Rect(toPositon.x + guiStyleLabelTo.CalcSize(contentLabelTo).x + toMargin, 0, baseRect.width - guiStyleLabelTo.padding.left - toPositon.x - guiStyleLabelTo.CalcSize(contentLabelTo).x, heightMessageTo);
        }

        float CalcScrollViewHeight()
        {
            float height = topMargin;

            foreach (FresviiGUIGroupMessageCreateFriendCell cell in friendCells)
            {
                height += cell.GetHeight() + cardMargin;
            }

            return height;
        }

        private int postGroupCount = 0;

        private int screenWidth = 0;

        private bool postIsLiveHelp = false;

		void Update(){

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            this.baseRect = new Rect(Position.x, Position.y + groupMessageCreateTopMenu.height, Screen.width, Screen.height - groupMessageCreateTopMenu.height - addCommentBottomMenu.height - FresviiGUIFrame.OffsetPosition.y);

            if (loadingSpinner != null)
            {
                loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);             
            }

            if (screenWidth != Screen.width)
            {
                CalcLayout();
            }

            if (!ControlLock)
            {
                InertiaScrollView(ref scrollPosition, ref scrollViewRect, CalcScrollViewHeight(), new Rect(baseRect.x, baseRect.y, baseRect.width, baseRect.height - heightMessageTo));
            }

            if (postGroupCount != groupFriends.Count || screenWidth != Screen.width || postIsLiveHelp != isLiveHelp)
            {
                postGroupCount = groupFriends.Count;

                postIsLiveHelp = isLiveHelp;

                screenWidth = Screen.width;

                userNames = "";

                for (int i = 0; i < groupFriends.Count; i++)
                {
                    userNames += groupFriends[i].Name + ", ";
                }
            }

            if (keyboard != null || !string.IsNullOrEmpty(searchingText))
            {
                string cont = "<color=" + ColorToHex(userNameColor) + ">" + userNames + "</color>" + "<color=" + ColorToHex(searchTextColor) + ">" + searchingText + "</color>";

                contentUserNames = new GUIContent(cont);
            }
            else
            {
                string cont = "<color=" + ColorToHex(userNameColor) + ">" + userNames + "</color>";

                contentUserNames = new GUIContent(cont);
            }

            if (guiStyleLableToUsers.CalcSize(contentUserNames).x > labelToUsersPosition.width)
            {
                guiStyleLableToUsers.alignment = TextAnchor.MiddleRight;
            }
            else
            {
                guiStyleLableToUsers.alignment = TextAnchor.MiddleLeft;
            }

#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape) && !ControlLock)
            {
                Back();
            }
#endif
		}

        private string ColorToHex(Color col)
        {
            return "#" + ((int)(col.r * 255f)).ToString("x2") + ((int)(col.g * 255f)).ToString("x2") + ((int)(col.b * 255f)).ToString("x2");
        }

        void OnUpdateScrollPosition(Vector2 value)
        {
            scrollPosition = value;
        }

        void OnCompletePull()
        {

        }

        void OnEnable()
        {
            ControlLock = false;
        }

        void OnDestroy()
        {
            if (loadingSpinner != null)
                loadingSpinner.Hide();

            ControlLock = false;

            foreach (FresviiGUIGroupMessageCreateFriendCell cell in friendCells)
                Destroy(cell.gameObject);
        }

        public void Back()
        {
            ControlLock = true;

            PostFrame.SetDraw(true);

            this.Tween(Vector2.zero, new Vector2(0.0f, Screen.height), delegate()
            {
                ControlLock = false;

                PostFrame.ControlLock = false;

                Destroy(this.gameObject);
            });
        }

        private bool isLiveHelp = false;

        public void OnTapLiveHelp()
        {
            isLiveHelp = !isLiveHelp;

            RemoveAllFriendTo();

            if (isLiveHelp)
            {
                AddFriendToGroup(OfficialUser, null);
            }

            groupMessageCreateTopMenu.LiveHelpChanged(isLiveHelp);
        }

        public void AddComment(string comment, Texture2D clipImage, Fresvii.AppSteroid.Models.Video video)
        {
            if (groupFriends.Count > 0)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });

                    return;
                }

                List<string> userIds = new List<string>();
                
                foreach (Fresvii.AppSteroid.Models.User friend in groupFriends)
                {
                    userIds.Add(friend.Id);
                }

                userIds.Add(FAS.CurrentUser.Id);

                loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

                loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, this.GuiDepth - 10);

                ControlLock = true;

                if (groupFriends.Count > 1)
                {
                    FASGroup.CreateGroup(userIds.ToArray(), (group, error) =>
                    {
                        OnGroupCreated(group, error, comment, clipImage, video);
                    });
                }
                else
                {
                    FASGroup.CreatePair(groupFriends[0].Id, (group, error) =>
                    {
                        OnGroupCreated(group, error, comment, clipImage, video);
                    });
                }
            }
        }

        private void OnGroupCreated(Fresvii.AppSteroid.Models.Group group, Fresvii.AppSteroid.Models.Error error, string comment, Texture2D clipImage, Fresvii.AppSteroid.Models.Video video)
        {
            loadingSpinner.Hide();

            imageSending = commentSending = false;

            ControlLock = false;

            if (error != null)
            {
                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.WWWTimeOut)
                {
                    if (this.gameObject.activeInHierarchy && this.enabled)
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("TimeOut"), delegate(bool del) { });
                    }
                }
                else
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), delegate(bool del) { });
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(comment))
                {
                    commentSending = true;

                    FASGroup.SendGroupMessage(group.Id, comment, delegate(Fresvii.AppSteroid.Models.GroupMessage message, Fresvii.AppSteroid.Models.Error _error)
                    {
                        commentSending = false;
                    });
                }

                if (clipImage != null && video == null)
                {
                    imageSending = true;

                    FASGroup.SendGroupMessage(group.Id, clipImage, delegate(Fresvii.AppSteroid.Models.GroupMessage message, Fresvii.AppSteroid.Models.Error _error)
                    {
                        imageSending = false;

                        addCommentBottomMenu.ClearClipImage();
                    });
                }

                if (video != null)
                {
                    imageSending = true;

                    FASGroup.SendGroupMessage(group.Id, video, delegate(Fresvii.AppSteroid.Models.GroupMessage message, Fresvii.AppSteroid.Models.Error _error)
                    {
                        imageSending = false;

                        addCommentBottomMenu.ClearClipImage();
                    });
                }

                StartCoroutine(WaitForSend(group));
            }
        }

        private bool commentSending;

        private bool imageSending;

        private IEnumerator WaitForSend(Fresvii.AppSteroid.Models.Group group)
        {
            while (imageSending || commentSending) yield return 1;

            loadingSpinner.Hide();

            ControlLock = false;

            Back();

            if (!IsCreateFromPairGroup)
            {
                PostFrame.gameObject.GetComponent<FresviiGUIGroupMessage>().GoToGroupChat(group);
            }
            else
            {
                if (frameChat != null)
                    Destroy(frameChat);

                frameChat = ((GameObject)Instantiate(prfbFrameChat)).GetComponent<FresviiGUIFrame>();

                frameChat.gameObject.GetComponent<FresviiGUIChat>().SetGroup(group);

                frameChat.Init(FresviiGUIManager.appIcon, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, this.PostFrame.GuiDepth - 1);

                frameChat.transform.parent = this.transform;

                frameChat.SetDraw(true);

                frameChat.PostFrame = this.PostFrame;

                this.ControlLock = true;

                this.Tween(Vector2.zero, new Vector2(-Screen.width, 0.0f), delegate()
                {
                    this.SetDraw(false);

                    this.ControlLock = false;
                });

                frameChat.Tween(new Vector2(Screen.width, 0.0f), Vector2.zero, delegate() { });
            }
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            groupMessageCreateTopMenu.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                    loadingSpinner.Hide();

                scrollPosition = Vector2.zero;
            }
        }

        public void AddFriendToGroup(Fresvii.AppSteroid.Models.User user, Action<bool> callback)
        {
            bool isExist = false;

            foreach (Fresvii.AppSteroid.Models.User _friend in groupFriends)
            {
                if (user.Id == _friend.Id)
                {
                    isExist = true;

                    break;
                }
            }

            if (!isExist)
            {
                groupFriends.Add(user);
            }

            addCommentBottomMenu.SetSendEnableAtAction(true, "", "");

            searchingText = "";

            if (keyboard != null)
            {
                keyboard.active = false;

                keyboard = null;
            }
        }

        public void RemoveFriendToGroup(Fresvii.AppSteroid.Models.User user)
        {
            Fresvii.AppSteroid.Models.User removeFriend = null;

            foreach (Fresvii.AppSteroid.Models.User _friend in groupFriends)
            {
                if (user.Id == _friend.Id)
                {
                    removeFriend = _friend;

                    break;
                }
            }

            if (removeFriend != null)
            {
                groupFriends.Remove(removeFriend);
            }

            if (groupFriends.Count < 1)
            {
                addCommentBottomMenu.SetSendEnableAtAction(false, FresviiGUIText.Get("Error"), FresviiGUIText.Get("NoSelectedMember"));
            }
		}

        void RemoveAllFriendTo()
        {
            foreach (FresviiGUIGroupMessageCreateFriendCell cell in friendCells)
            {
                cell.IsSelected = false;

                cell.Select(cell.IsSelected);
            }

            groupFriends.Clear();
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(new Rect(scrollViewRect.x, scrollViewRect.y + heightMessageTo, scrollViewRect.width, scrollViewRect.height));

            if (!isLiveHelp)
            {
                //  Friend cards
                float cardY = topMargin;

                foreach (FresviiGUIGroupMessageCreateFriendCell cell in friendCells)
                {
                    if (string.IsNullOrEmpty(searchingText) || (!string.IsNullOrEmpty(searchingText) && cell.friend.Name.IndexOf(searchingText) >= 0))
                    {
                        Rect cellPosition = new Rect(0f, cardY, baseRect.width, cell.GetHeight());

                        Rect drawPosition = new Rect(cellPosition.x, scrollViewRect.y + cellPosition.y, cellPosition.width, cellPosition.height);

                        if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                        {
                            cell.Draw(cellPosition, false);
                        }

                        cardY += cellPosition.height + cardMargin;
                    }
                }
            }

            GUI.EndGroup();

            // To
            GUI.DrawTextureWithTexCoords(toPositon, FresviiGUIColorPalette.Palette, textureCoordsTo);

            if (GUI.Button(toPositon, "", GUIStyle.none) && keyboard == null)
            {
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
            }

            if (keyboard != null)
            {
                if (keyboard.wasCanceled)
                {
                    keyboard.active = false;

                    keyboard = null;

                    searchingText = "";
                }
                if (keyboard.done)
                {
                    searchingText = keyboard.text;

                    keyboard.active = false;

                    keyboard = null;
                }
                else
                {
                    searchingText = keyboard.text;
                }
            }

            GUI.Label(labelToPosition, contentLabelTo, guiStyleLabelTo);

            GUI.BeginGroup(labelToUsersPosition);

            if(contentUserNames != null)
                GUI.Label(new Rect(0f, 0f, labelToUsersPosition.width, labelToUsersPosition.height), contentUserNames, guiStyleLableToUsers);

            GUI.EndGroup();

            GUI.EndGroup();
        }       
    }
}
