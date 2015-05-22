using UnityEngine;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIEditGroupMember : FresviiGUIFrame
    {
        public float sideMargin;

        private Texture2D palette;

        private FresviiGUIEditGroupMemberTop editGroupMemberTopMenu;

        private Rect baseRect;
       
        private Rect scrollViewRect;
        
        private float scaleFactor;

        private string postFix;
        
        public float topMargin;

        public float cardMargin;

        private Vector2 scrollPosition = Vector2.zero;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;
        
        public Vector2 loadingSpinnerSize;
        
        private List<FresviiGUIEditGroupMemberCell> memberCells = new List<FresviiGUIEditGroupMemberCell>();

        public GameObject prfbEditGroupMemberCell;

        private bool initialized;

		public float pollingInterval = 15f;

        [HideInInspector]
        public Texture2D textureCheckMark;

        public float heightAddContact;

        public GUIStyle guiStyleLabelAddContact;
        public Rect labelAddContactPosition;

        public GUIStyle guiStyleLableToUsers;
        private Rect labelToUsersPosition;

        public float toMargin = 5f;

        public IList<Fresvii.AppSteroid.Models.Member> members = new List<Fresvii.AppSteroid.Models.Member>();

        private GUIContent contentUserNames;

        private string userNames = "";

        public Fresvii.AppSteroid.Models.Group Group { get; set; }

        public Rect plusIconPosition;

        public Texture2D plusIcon;

        public GameObject prfbGroupMemeberAdd;

        private FresviiGUIGroupMemberAdd frameGroupMemberAdd;
        
        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            ControlLock = false;

            this.GuiDepth = guiDepth;

            this.postFix = postFix;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleLabelAddContact.font = null;

                guiStyleLableToUsers.font = null;
            }

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            editGroupMemberTopMenu = GetComponent<FresviiGUIEditGroupMemberTop>();

            editGroupMemberTopMenu.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

            guiStyleLabelAddContact.fontSize = (int)(guiStyleLabelAddContact.fontSize * scaleFactor);

            guiStyleLabelAddContact.padding = FresviiGUIUtility.RectOffsetScale(guiStyleLabelAddContact.padding, scaleFactor);

            guiStyleLableToUsers.fontSize = (int)(guiStyleLableToUsers.fontSize * scaleFactor);

            this.scaleFactor = scaleFactor;
            
            toMargin *= scaleFactor;
			
            loadingSpinnerSize *= scaleFactor;
            
            sideMargin *= scaleFactor;
            
            topMargin = scaleFactor;

            heightAddContact *= scaleFactor;

            scrollPosition.y = 0.0f;

            guiStyleLabelAddContact.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            textureCheckMark = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.IconFriendTextureName + postFix, false);

			loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
			
			loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GuiDepth - 10);

            labelAddContactPosition = FresviiGUIUtility.RectScale(labelAddContactPosition, scaleFactor);

            plusIconPosition = FresviiGUIUtility.RectScale(plusIconPosition, scaleFactor);

            FASGroup.GetGroupMemberList(Group.Id, OnGetMembers);

            SetScrollSlider(scaleFactor * 2.0f);
		}

        void OnGetMembers(IList<Fresvii.AppSteroid.Models.Member> members, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error)
        {
            if (this == null) return;

			loadingSpinner.Hide();

            if (error == null)
            {
                this.members = members;

                foreach (Fresvii.AppSteroid.Models.Member member in this.members)
                {
                    if (member.Id == FAS.CurrentUser.Id) continue;

                    AddMember(member);
                }
            }
            else
            {
                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                    Debug.LogError(error.ToString());
            }            
        }

        void AddMember(Fresvii.AppSteroid.Models.Member member)
        {
            FresviiGUIEditGroupMemberCell memberCell = ((GameObject)Instantiate(prfbEditGroupMemberCell)).GetComponent<FresviiGUIEditGroupMemberCell>();

            memberCell.transform.parent = this.transform;

            memberCell.Init(member, scaleFactor, postFix, this);

            memberCells.Add(memberCell);
        }

        void CalcLayout()
        {
            this.baseRect = new Rect(Position.x, Position.y + editGroupMemberTopMenu.height, Screen.width, Screen.height - editGroupMemberTopMenu.height);

            labelAddContactPosition = new Rect(0, 0, baseRect.width, heightAddContact+1);
        }

        float CalcScrollViewHeight()
        {
            float height = topMargin;

            foreach (FresviiGUIEditGroupMemberCell cell in memberCells)
            {
                height += cell.GetHeight() + cardMargin;
            }

            return height;
        }

        public void DeleteMemeberCell(FresviiGUIEditGroupMemberCell cell)
        {
            FAS.Instance.Client.GroupService.DeleteMember(this.Group.Id, cell.member.Id, delegate(Fresvii.AppSteroid.Models.Error error)
            {
                int index = -1;

                for(int i = 0;i < Group.Members.Count; i++)
                {
                    if(cell.member.Id == Group.Members[i].Id)
                    {
                        index = i;

                        break;
                    }
                }

                if (index >= 0)
                {
                    Group.Members.RemoveAt(index);

					Group.MembersCount--;
                }
            });

            memberCells.Remove(cell);

            Destroy(cell);
        }

		void Update(){

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            if (loadingSpinner != null)
            {
                loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);             
            }

            CalcLayout();

            if(!ControlLock)
                InertiaScrollView(ref scrollPosition, ref scrollViewRect, CalcScrollViewHeight(), new Rect(baseRect.x, baseRect.y, baseRect.width, baseRect.height));

            userNames = "";

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

        void OnDisable()
        {

        }

        public bool CardIsOpen { get; protected set; }

        public void OnCardOpenStateChanged(bool open)
        {
            CardIsOpen = open;
        }

        public void AddGroupFriends(List<Fresvii.AppSteroid.Models.Friend> groupFriends)
        {
            foreach (Fresvii.AppSteroid.Models.Friend friend in groupFriends)
            {
                bool exist = false;

                foreach (FresviiGUIEditGroupMemberCell cell in memberCells)
                {
                    if (cell.member.Id == friend.Id)
                    {
                        exist = true;

                        break;
                    }
                }

                if (!exist)
                {
                    FAS.Instance.Client.GroupService.AddMember(this.Group.Id, friend.Id, delegate(Fresvii.AppSteroid.Models.Member member, Fresvii.AppSteroid.Models.Error error)
                    {
                        if (error == null)
                        {
                            AddMember(member);

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
        }

        void OnDestroy()
        {
            if (loadingSpinner != null)
            {
                loadingSpinner.Hide();
            }

            ControlLock = false;

            foreach (FresviiGUIEditGroupMemberCell cell in memberCells)
                Destroy(cell.gameObject);
        }

        public void Back()
        {
            ControlLock = true;

            PostFrame.SetDraw(true);

            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
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

            editGroupMemberTopMenu.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                    loadingSpinner.Hide();

                scrollPosition = Vector2.zero;
            }
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(new Rect(scrollViewRect.x, scrollViewRect.y, scrollViewRect.width, scrollViewRect.height));

            //  Member cards
            float cardY = topMargin;
        
            //foreach (FresviiGUIEditGroupMemberCell cell in memberCells)
            for(int i = 0; i < memberCells.Count; i++)
            {
                FresviiGUIEditGroupMemberCell cell = memberCells[i]; 

                Rect cellPosition = new Rect(0f, cardY, baseRect.width, cell.GetHeight());

                Rect drawPosition = new Rect(cellPosition.x, scrollViewRect.y + cellPosition.y, cellPosition.width, cellPosition.height);

                if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                {
                    cell.Draw(cellPosition, i != memberCells.Count - 1);
                }

                cardY += cellPosition.height + cardMargin;
            }

            GUI.EndGroup();

            GUI.EndGroup();

        }       
    }
}
