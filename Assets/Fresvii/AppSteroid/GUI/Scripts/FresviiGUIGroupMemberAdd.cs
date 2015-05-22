using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIGroupMemberAdd : FresviiGUIFrame
    {
        public float sideMargin;

        private Texture2D palette;

        private FresviiGUIGroupMemberAddTop groupMemberAddTop;

        private Rect baseRect;
       
        private Rect scrollViewRect;
        
        private float scaleFactor;
        
        public float topMargin;

        public float cardMargin;

        private Vector2 scrollPosition = Vector2.zero;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;
        
        public Vector2 loadingSpinnerSize;
        
        [HideInInspector]
        public Fresvii.AppSteroid.Util.TextureLoader textureLoader;

        private IList<Fresvii.AppSteroid.Models.Friend> friends = new List<Fresvii.AppSteroid.Models.Friend>();

        private List<FresviiGUIGroupMessageCreateFriendCell> friendCells = new List<FresviiGUIGroupMessageCreateFriendCell>();

        public GameObject prfbGroupMessageFriendCell;

        private bool initialized;

		public float pollingInterval = 15f;

        [HideInInspector]
        public Texture2D textureCheckMark;

        public float heightMessageTo;

        private Rect textureCoordsSearchBg;

        private Rect searchBgPosition;

        private Rect textFieldPosition;

        public GUIStyle guiStyleLabelTo;

        public GUIStyle guiStyleLableToUsers;
        private Rect labelToUsersPosition;

        private GUIContent contentLabelTo;

        private GUIContent contentSearch;

        public float toMargin = 5f;

        public List<Fresvii.AppSteroid.Models.User> groupFriends = new List<Fresvii.AppSteroid.Models.User>();

        private GUIContent contentUserNames;

        private string userNames = "";

        public FresviiGUIChat prfbFrameChat;

        private Texture2D textureSearchArea;

        private string textInputField = "";

        public string TextInputField{ get{return textInputField;}}
        
        private TouchScreenKeyboard keyboard;

        public GUIStyle guiStyleTextFiled;

        public Texture2D searchIcon;
        public Rect searchIconPosition;
        private Rect searchIconLabelPosition;
        public GUIStyle guiStyleSearchLabel;

        private Color searchBgColor;

        private float serchObjLength;

        public FresviiGUIEditGroupMember frameEditGroupMember;

        public Fresvii.AppSteroid.Models.Group Group { get; set; }

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            ControlLock = false;

            this.GuiDepth = guiDepth;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleLabelTo.font = null;

                guiStyleLableToUsers.font = null;

                guiStyleTextFiled.font = null;

                guiStyleSearchLabel.font = null;
            }

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            groupMemberAddTop = GetComponent<FresviiGUIGroupMemberAddTop>();

            groupMemberAddTop.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

            guiStyleLabelTo.fontSize = (int)(guiStyleLabelTo.fontSize * scaleFactor);

            guiStyleLabelTo.padding = FresviiGUIUtility.RectOffsetScale(guiStyleLabelTo.padding, scaleFactor);

            guiStyleLableToUsers.fontSize = (int)(guiStyleLableToUsers.fontSize * scaleFactor);

            guiStyleTextFiled.fontSize = (int)(guiStyleTextFiled.fontSize * scaleFactor);

            guiStyleTextFiled.padding = FresviiGUIUtility.RectOffsetScale(guiStyleTextFiled.padding, scaleFactor);

            guiStyleSearchLabel.fontSize = (int)(guiStyleSearchLabel.fontSize * scaleFactor);

            searchBgColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.SearchBackground);

            guiStyleSearchLabel.normal.textColor = searchBgColor;

            this.scaleFactor = scaleFactor;
            
            toMargin *= scaleFactor;
			
            loadingSpinnerSize *= scaleFactor;
            
            sideMargin *= scaleFactor;
            
            topMargin = scaleFactor;

            heightMessageTo *= scaleFactor;

            textureLoader = Fresvii.AppSteroid.Util.TextureLoader.Create();

            textureLoader.transform.parent = this.transform;

            scrollPosition.y = 0.0f;

            textureCoordsSearchBg = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.SearchBackground);

            textureCheckMark = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconFriendTextureName + postFix, false);

            textureSearchArea = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.TextFiledWTextureName + postFix, false);

            searchIconPosition = FresviiGUIUtility.RectScale(searchIconPosition, scaleFactor);

            FASFriendship.GetAccountFriendList(1, OnGetFriends);

            contentLabelTo = new GUIContent(FresviiGUIText.Get("To"));

			loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
			
			loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GuiDepth - 10);

            contentSearch = new GUIContent(FresviiGUIText.Get("Search"));

            serchObjLength = searchIconPosition.width + sideMargin + guiStyleSearchLabel.CalcSize(contentSearch).x;

            SetScrollSlider(scaleFactor * 2.0f);
		}

        void OnGetFriends(IList<Fresvii.AppSteroid.Models.Friend> friends, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
        {
            if (this == null) return;

			loadingSpinner.Hide();

            if (error == null)
            {
                this.friends = friends;

                foreach (Fresvii.AppSteroid.Models.Friend friend in this.friends)
                {
					bool isGroupMember = false;

                    foreach (Fresvii.AppSteroid.Models.Member member in Group.Members)
                    {
                        if (friend.Id == member.Id)
                        {
                            isGroupMember = true;

                            break;
                        }
                    }

                    if (!isGroupMember)
                    {
						FresviiGUIGroupMessageCreateFriendCell friendCell = ((GameObject)Instantiate(prfbGroupMessageFriendCell)).GetComponent<FresviiGUIGroupMessageCreateFriendCell>();

                        friendCell.transform.parent = this.transform;

                        friendCell.Init(friend, scaleFactor, this, textureCheckMark, AddFriendToGroup, RemoveFriendToGroup);

                        friendCells.Add(friendCell);
                    }
                }

                if (meta.NextPage.HasValue)
                {
                    FASFriendship.GetAccountFriendList((uint)meta.NextPage, OnGetFriends);
                }
            }
            else
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                {
                    Debug.LogError(error.ToString());
                }
            }
            
        }

        void CalcLayout()
        {
            this.baseRect = new Rect(Position.x, Position.y + groupMemberAddTop.height, Screen.width, Screen.height - groupMemberAddTop.height);

            searchBgPosition = new Rect(0f, 0f, baseRect.width, heightMessageTo);

            textFieldPosition = new Rect(sideMargin, sideMargin, baseRect.width - 2f * sideMargin, heightMessageTo - 2f * sideMargin);

            labelToUsersPosition = new Rect(searchBgPosition.x + guiStyleLabelTo.CalcSize(contentLabelTo).x + toMargin, 0, baseRect.width - guiStyleLabelTo.padding.left - searchBgPosition.x - guiStyleLabelTo.CalcSize(contentLabelTo).x, heightMessageTo);

            searchIconPosition = new Rect(textFieldPosition.x + 0.5f * textFieldPosition.width - 0.5f * serchObjLength, heightMessageTo * 0.5f - searchIconPosition.height * 0.5f, searchIconPosition.width, searchIconPosition.height);

            searchIconLabelPosition = new Rect(searchIconPosition.x + searchIconPosition.width + sideMargin, 0f, serchObjLength, heightMessageTo);
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

		void Update(){

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            if (loadingSpinner != null)
            {
                loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);             
            }

            CalcLayout();

            if(!ControlLock)
                InertiaScrollView(ref scrollPosition, ref scrollViewRect, CalcScrollViewHeight(), new Rect(baseRect.x, baseRect.y + heightMessageTo, baseRect.width, baseRect.height - heightMessageTo));


            userNames = "";

            for (int i = 0; i < groupFriends.Count; i++)
            {
                userNames += groupFriends[i].Name + ((i == groupFriends.Count - 1) ? "" : ", ");
            }

            contentUserNames = new GUIContent(userNames);

            if (guiStyleLableToUsers.CalcSize(contentUserNames).x > labelToUsersPosition.width)
            {
                guiStyleLableToUsers.alignment = TextAnchor.MiddleRight;
            }
            else
            {
                guiStyleLableToUsers.alignment = TextAnchor.MiddleLeft;
            }
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
            textureLoader.Release();

            if (loadingSpinner != null)
                loadingSpinner.Hide();

            ControlLock = false;

            foreach (FresviiGUIGroupMessageCreateFriendCell cell in friendCells)
            {
                Destroy(cell.gameObject);
            }
        }

        public void Back()
        {
            textInputField = "";

            ControlLock = true;

            PostFrame.SetDraw(true);

            this.Tween(Vector2.zero, new Vector2(0.0f, Screen.height), delegate()
            {
                ControlLock = false;

                PostFrame.ControlLock = false;

                Destroy(this.gameObject);
            });
        }

        public void Done()
        {
            textInputField = "";

            ControlLock = true;

            PostFrame.SetDraw(true);

            AddGroupFriends(groupFriends);

            this.Tween(Vector2.zero, new Vector2(0.0f, Screen.height), delegate()
            {
                ControlLock = false;

                PostFrame.ControlLock = false;

                Destroy(this.gameObject);
            });
        }

        public void AddGroupFriends(List<Fresvii.AppSteroid.Models.User> groupFriends)
        {
            foreach (Fresvii.AppSteroid.Models.User user in groupFriends)
            {
                FAS.Instance.Client.GroupService.AddMember(this.Group.Id, user.Id, delegate(Fresvii.AppSteroid.Models.Member member, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error == null)
                    {
                        Group.Members.Add(member);

                        Group.MembersCount++;
                    }
                    else
                    {
                        Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                        if (error.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.NetworkNotReachable)
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("Offline"), delegate(bool del) { });
                        }
                        else
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(error.Detail, delegate(bool del) { });
                        }

                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        {
                            Debug.Log("GroupService.AddMember : " + error.ToString());
                        }
                    }
                });
            }
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            groupMemberAddTop.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                    loadingSpinner.Hide();

                scrollPosition = Vector2.zero;
            }
        }

        public void AddFriendToGroup(Fresvii.AppSteroid.Models.User user, Action<bool> callback)
        {
            groupFriends.Add(user);
        }

        public void RemoveFriendToGroup(Fresvii.AppSteroid.Models.User user)
        {
            groupFriends.Remove(user);
		}

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(new Rect(scrollViewRect.x, scrollViewRect.y + heightMessageTo - 2, scrollViewRect.width, scrollViewRect.height));

            //  Friend cards
            float cardY = topMargin;

            for(int i = 0; i < friendCells.Count; i++)
            {
                FresviiGUIGroupMessageCreateFriendCell cell = friendCells[i];

                if (cell.friend.Name.IndexOf(textInputField) != 0 && !string.IsNullOrEmpty(textInputField))
                {
                    continue;
                }
            
                Rect cellPosition = new Rect(0f, cardY, baseRect.width, cell.GetHeight());

                Rect drawPosition = new Rect(cellPosition.x, scrollViewRect.y + cellPosition.y, cellPosition.width, cellPosition.height);

                if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                {
                    cell.Draw(cellPosition, i != friendCells.Count - 1);
                }

                cardY += cellPosition.height + cardMargin;
            }

            GUI.EndGroup();

            // Search
            GUI.DrawTextureWithTexCoords(searchBgPosition, FresviiGUIColorPalette.Palette, textureCoordsSearchBg);

            FresviiGUIUtility.DrawButtonFrame(textFieldPosition, textureSearchArea, scaleFactor);

#if UNITY_EDITOR

            textInputField = GUI.TextField(textFieldPosition, textInputField, guiStyleTextFiled);

#elif UNITY_ANDROID || UNITY_IOS

            Event e = Event.current;

            if (e.type == EventType.MouseUp && textFieldPosition.Contains(e.mousePosition) && !FASGesture.IsDragging)
            {
                e.Use();

				gameObject.GetComponent<FresviiGUIPopUpShield>().Enable(delegate (){ 
					if(keyboard != null)
					{
						keyboard.active = false;
						
                        keyboard.text = "";
						
                        keyboard = null;
					}			
				});

                keyboard = TouchScreenKeyboard.Open(textInputField, TouchScreenKeyboardType.Default, false, true, false, false);
            }

            if (keyboard != null)
            {
                textInputField = keyboard.text;

				if(keyboard.done || keyboard.wasCanceled || !keyboard.active)
				{
					gameObject.GetComponent<FresviiGUIPopUpShield>().Done();

					keyboard = null;
				}
            }

            GUI.Label(textFieldPosition, textInputField, guiStyleTextFiled);

#endif
            if (string.IsNullOrEmpty(textInputField))
            {
                Color tmp = GUI.color;

                GUI.color = searchBgColor;

                GUI.DrawTexture(searchIconPosition, searchIcon, ScaleMode.ScaleToFit);

                GUI.color = tmp;

                GUI.Label(searchIconLabelPosition, contentSearch, guiStyleSearchLabel);
            }

            GUI.EndGroup();

        }       
    }
}
