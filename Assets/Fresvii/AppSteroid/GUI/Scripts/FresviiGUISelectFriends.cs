using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUISelectFriends : FresviiGUIFrame
    {
        public float sideMargin;

        private Texture2D palette;

        private FresviiGUISelectFriendsTop selectMemberTop;

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

        public List<Fresvii.AppSteroid.Models.User> selectedUsers = new List<Fresvii.AppSteroid.Models.User>();

        private GUIContent contentUserNames;

        private string userNames = "";

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

        private Action<List<Fresvii.AppSteroid.Models.User>> callback;

        public uint SelectableMaxCount = uint.MaxValue;

        List<Fresvii.AppSteroid.Models.User> initSelectedUsers;

        public void SetInitSelectedFriends(List<Fresvii.AppSteroid.Models.User> users)
        {
            initSelectedUsers = users;
        }

        public void SetCallback(Action<List<Fresvii.AppSteroid.Models.User>> callback)
        {
            this.callback = callback;
        }

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

            selectMemberTop = GetComponent<FresviiGUISelectFriendsTop>();

            selectMemberTop.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

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

        void OnGetFriends(IList<Fresvii.AppSteroid.Models.Friend> _friends, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
        {
            if (this == null) return;

			loadingSpinner.Hide();

            if (error == null)
            {
                foreach (Fresvii.AppSteroid.Models.Friend friend in _friends)
                {
                    this.friends.Add(friend);

                    FresviiGUIGroupMessageCreateFriendCell friendCell = ((GameObject)Instantiate(prfbGroupMessageFriendCell)).GetComponent<FresviiGUIGroupMessageCreateFriendCell>();

                    friendCell.transform.parent = this.transform;

                    friendCell.Init(friend, scaleFactor, this, textureCheckMark, AddSelectedFriend, RemoveSelectedFriend);

                    friendCells.Add(friendCell);

                    if (meta.NextPage.HasValue)
                    {
                        FASFriendship.GetAccountFriendList((uint)meta.NextPage, OnGetFriends);
                    }

                    Fresvii.AppSteroid.Models.User addedUser = null;

                    foreach (Fresvii.AppSteroid.Models.User _user in initSelectedUsers)
                    {
                        if (_user.Id == friend.Id)
                        {
                            friendCell.IsSelected = true;

                            addedUser = _user;

                            break;
                        }
                    }

                    if (addedUser != null)
                    {
                        initSelectedUsers.Remove(addedUser);
                    }

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
            this.baseRect = new Rect(Position.x, Position.y + selectMemberTop.height, Screen.width, Screen.height - selectMemberTop.height);

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

            for (int i = 0; i < selectedUsers.Count; i++)
            {
                userNames += selectedUsers[i].Name + ((i == selectedUsers.Count - 1) ? "" : ", ");
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

            selectedUsers.Clear();

            if(callback != null)
                callback(selectedUsers);

            this.Tween(Vector2.zero, new Vector2(0f, Screen.height), delegate()
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

            if (callback != null)
                callback(selectedUsers);

            this.Tween(Vector2.zero, new Vector2(0f, Screen.height), delegate()
            {
                ControlLock = false;

                PostFrame.ControlLock = false;

                Destroy(this.gameObject);
            });
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            selectMemberTop.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                    loadingSpinner.Hide();

                scrollPosition = Vector2.zero;
            }
        }

        public void AddSelectedFriend(Fresvii.AppSteroid.Models.User user, Action<bool> callback)
        {
            foreach (Fresvii.AppSteroid.Models.User _user in selectedUsers)
            {
                if (_user.Id == user.Id)
                {
                    return;
                }
            }

            if (selectedUsers.Count >= SelectableMaxCount)
            {
                string tooMuch = FresviiGUIText.Get("SelectFrindsOvered").Replace("%num", SelectableMaxCount.ToString());

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(tooMuch, FresviiGUIText.Get("OK"), FresviiGUIText.Get("OK"), FresviiGUIText.Get("OK"), (del) => { });

                if (callback != null)
                {
                    callback(false);
                }
            }
            else
            {
                selectedUsers.Add(user);

                if (callback != null)
                {
                    callback(true);
                }
            }

            selectMemberTop.SetSubmit((selectedUsers.Count > 0));
        }

        public void RemoveSelectedFriend(Fresvii.AppSteroid.Models.User user)
        {
            bool contain = false;

            foreach (Fresvii.AppSteroid.Models.User _user in selectedUsers)
            {
                if (_user.Id == user.Id)
                {
                    contain = true;

                    break;
                }
            }

            if(contain)
                selectedUsers.Remove(user);

            selectMemberTop.SetSubmit((selectedUsers.Count > 0));
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
