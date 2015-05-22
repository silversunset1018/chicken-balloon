using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIChatTopMenu : MonoBehaviour
    {        
        private Texture2D navDownIcon;

        private Rect menuRect;

		private Rect titleRect;

        private Texture2D palette;
        
        private Rect texCoordsMenu;
        
        private Rect texCoordsBorderLine;

        public float height;

        public GUIStyle guiStyleTitle;
        
        private string title = "";
        
        public float titleFontSize;

        public Rect editButtonPosition;
       
        public float hMargin;
		
        public float vMargin = 8f;

        private FresviiGUIChat frameChat;

        public int guiDepth = -30;

        private Texture2D backIcon;

        private float scaleFactor;

        private string postFix;
        
        public GUIStyle guiStyleBackButton;
        
        private Rect backButtonPosition;
		
        private Rect backButtonHitPosition;

        private Fresvii.AppSteroid.Models.User other;

        private Color colorNormal;

        private Color iconColor;

        public Rect navButtonPosition;

        private Rect navButtonHitPosition;

        public Rect callIconPosition;

        private Rect callIconHitPosition;

        private Rect doneButtonPosition;

        public GUIStyle guiStyleDoneButton;

        private bool enableEditMember;

        private Fresvii.AppSteroid.Gui.PopUpBalloonMenu popUpBaloonMenu;

        public Vector2 popUpOffset;

        private int GuiDepth;

        public void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth, FresviiGUIChat frameChat)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleTitle.font = null;

                guiStyleTitle.fontStyle = FontStyle.Bold;
                
                guiStyleBackButton.font = null;

                guiStyleDoneButton.font = null;
            }

            this.frameChat = frameChat;
            
            this.other = frameChat.Other;

            this.scaleFactor = scaleFactor;

            this.postFix = postFix;

            this.GuiDepth = guiDepth;
           
            this.guiDepth = guiDepth;

            palette = FresviiGUIColorPalette.Palette;

            texCoordsMenu = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarBackground);

            texCoordsBorderLine = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.NavigationBarUnderLine);
            
            this.backIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.BackIconTextureName + postFix, false);

            this.navDownIcon = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.NavMenuTextureName + postFix, false);

            height *= scaleFactor;
            
            guiStyleTitle.fontSize = (int)(titleFontSize * scaleFactor);

            this.popUpOffset *= scaleFactor;

            guiStyleTitle.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

            guiStyleDoneButton.normal.textColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarPositive);

            hMargin *= scaleFactor;

            vMargin *= scaleFactor;

            editButtonPosition = FresviiGUIUtility.RectScale(editButtonPosition, scaleFactor);

            navButtonPosition = FresviiGUIUtility.RectScale(navButtonPosition, scaleFactor);

            callIconPosition = FresviiGUIUtility.RectScale(callIconPosition, scaleFactor);

            iconColor = colorNormal = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

            guiStyleDoneButton.fontSize = (int)(guiStyleDoneButton.fontSize * scaleFactor);

            guiStyleDoneButton.padding = FresviiGUIUtility.RectOffsetScale(guiStyleDoneButton.padding, scaleFactor);

            if (this.other != null)
            {
                this.title = FresviiGUIUtility.Truncate(other.Name, guiStyleTitle, Screen.width - vMargin * 4f - backIcon.width - navDownIcon.width, "...");
            }
            else
            {
                if (frameChat.Group.Members == null)
                {
                    frameChat.Group.FetchMembers((error) =>
                    {
                        if (error == null)
                        {
                            SetMemberNames();
                        }
                    });
                }
                else if (frameChat.Group.Members.Count == 0)
                {
                    frameChat.Group.FetchMembers(delegate(Fresvii.AppSteroid.Models.Error error)
                    {
                        if (error == null)
                        {
                            SetMemberNames();
                        }
                    });
                }
                else
                {
                    SetMemberNames();
                }
            }

            CalcLayout();

            if (frameChat.Group != null)
            {
                if (!frameChat.Group.Pair)
                    enableEditMember = true;
            }
        }

        public void SetMemberNames()
        {
            if (frameChat.Group == null) return;

            if (frameChat.Group.Members == null) return;

            if (frameChat.Group.Members.Count == 0) return;

            string memberNames = "";

            if (frameChat.Group.MembersCount <= 1 && !frameChat.Group.Pair)
            {
                this.title = FresviiGUIText.Get("NobodyElse");
            }
            else
            {
                if (frameChat.Group.Pair)
                {
                    for (int i = 0; i < frameChat.Group.Members.Count; i++)
                    {
                        if (frameChat.Group.Members[i].Id != FAS.CurrentUser.Id)
                        {
                            memberNames += frameChat.Group.Members[i].Name;

                            break;
                        }                    
                    }
                }
                else
                {
                    for (int i = 0; i < frameChat.Group.Members.Count; i++)
                    {
                        memberNames += frameChat.Group.Members[i].Name + ((i == frameChat.Group.Members.Count - 1) ? "" : ", ");

                        if (i == 5) break;
                    }
                }

                if (!frameChat.Group.Pair)
                {
                    memberNames += "(" + frameChat.Group.MembersCount + ")";
                }

                GUIContent content = new GUIContent(memberNames);

                if (guiStyleTitle.CalcSize(content).x <= titleRect.width)
                {
                    this.title = memberNames;
                }
                else
                {
                    if (frameChat.Group.Pair)
                    {
                        this.title = FresviiGUIUtility.Truncate(memberNames, guiStyleTitle, titleRect.width, "...");
                    }
                    else
                    {
                        this.title = FresviiGUIUtility.Truncate(memberNames, guiStyleTitle, titleRect.width, "...(" + frameChat.Group.MembersCount + ")");
                    }
                }
            }
        }

        private float postScreenWidth = 0;

        void OnEnable()
        {
            postScreenWidth = 0f;
        }

        void Update()
        {
            CalcLayout();
        }

        void CalcLayout()
        {
            menuRect = new Rect(frameChat.Position.x, frameChat.Position.y, Screen.width, height);

            if (postScreenWidth != Screen.width)
            {
                postScreenWidth = Screen.width;
              
                backButtonPosition = new Rect(vMargin, 0.5f * (height - backIcon.height), backIcon.width, backIcon.height);

                backButtonHitPosition = new Rect(0, 0, backButtonPosition.x + backButtonPosition.width + vMargin, height);

                navButtonPosition = new Rect(menuRect.width - vMargin - navDownIcon.width, 0.5f * (height - navDownIcon.height), navDownIcon.width, navDownIcon.height);

                navButtonHitPosition = new Rect(menuRect.width - vMargin * 2f - navDownIcon.width, 0f, navDownIcon.width + 2f * vMargin, height);

                titleRect = new Rect(backButtonPosition.x + backButtonPosition.width + vMargin, 0f, menuRect.width - backButtonPosition.width - navButtonPosition.width - 4f * vMargin, height);
           
                doneButtonPosition = new Rect(Screen.width - menuRect.width * 0.3f, 0f, menuRect.width * 0.3f, menuRect.height);

                if (other != null)
                {
                    this.title = FresviiGUIUtility.Truncate(other.Name, guiStyleTitle, titleRect.width, "...");
                }
                else
                {
                    SetMemberNames();
                }
            }

        }

        public void OnGUI()
        {
            GUI.depth = guiDepth;

            GUI.DrawTextureWithTexCoords(new Rect(menuRect.x, menuRect.height + menuRect.y, Screen.width, 1), palette, texCoordsBorderLine);

            editButtonPosition.x = Screen.width - editButtonPosition.width - hMargin;

            //  Mat
            GUI.DrawTextureWithTexCoords(menuRect, palette, texCoordsMenu);

            GUI.BeginGroup(menuRect);

            // Title
            GUI.Label(titleRect, title, guiStyleTitle);

            if (!frameChat.IsModal)
            {
                Color tmpColor = GUI.color;

                GUI.color = colorNormal;

                GUI.DrawTexture(backButtonPosition, backIcon);

                GUI.color = tmpColor;

                Event e = Event.current;

                if (e.type == EventType.MouseUp && backButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging && !frameChat.ControlLock)
                {
                    e.Use();

                    frameChat.BackToPostFrame();
                }

                Color tmp = GUI.color;

                GUI.color = iconColor;

                GUI.DrawTexture(navButtonPosition, navDownIcon);
                
                GUI.color = tmp;

                if (e.type == EventType.MouseUp && navButtonHitPosition.Contains(e.mousePosition) && !FASGesture.IsDragging && !frameChat.ControlLock)
                {
                    e.Use();

                    Vector2 position = new Vector2(navButtonPosition.x, navButtonPosition.y);

                    List<string> buttons = new List<string>();

                    if (enableEditMember)
                    {
                        buttons.Add(FresviiGUIText.Get("MemberList"));

                        buttons.Add(FresviiGUIText.Get("Add"));
                    }
                    else
                    {
                        buttons.Add(FresviiGUIText.Get("Profile"));
                    }

#if GROUP_CONFERENCE                    
                    if(!(frameChat.Group.Pair && frameChat.Group.Members[0].Official))
                        buttons.Add(FresviiGUIText.Get("Call"));
#endif

                    popUpBaloonMenu = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/PopUpBalloonMenu"))).GetComponent<Fresvii.AppSteroid.Gui.PopUpBalloonMenu>();

                    popUpBaloonMenu.Show(buttons.ToArray(), popUpOffset + position + FresviiGUIFrame.OffsetPosition, scaleFactor, postFix, this.GuiDepth - 10, Color.grey, Color.white, Color.white, delegate(string selectedButton)
                    {
                        if (selectedButton == FresviiGUIText.Get("MemberList"))
                        {
                            frameChat.OnEditMemberTapped();
                        }
                        else if (selectedButton == FresviiGUIText.Get("Add"))
                        {
                            frameChat.OnAddMemberTapped();
                        }
                        else if (selectedButton == FresviiGUIText.Get("Profile"))
                        {
                            frameChat.OnProfileTapped();
                        }
#if GROUP_CONFERENCE
                        else if (selectedButton == FresviiGUIText.Get("Call"))
                        {
                            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                            {
                                if (FASConference.IsCalling())
                                {
                                    Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("VoiceChatAlredayExists"), delegate(bool del) { });

                                    return;
                                }

                                frameChat.OnCallButtonTapped();
                            }
                        }
#endif
                    });                        
                }

                if (popUpBaloonMenu != null)
                {
                    popUpBaloonMenu.SetPosition(popUpOffset + new Vector2(navButtonPosition.x, navButtonPosition.y) + FresviiGUIFrame.OffsetPosition);
                }

            }
            else
            {
                if (GUI.Button(doneButtonPosition, FresviiGUIText.Get("Done"), guiStyleDoneButton))
                {
                    frameChat.BackToPostFrame();
                }
            }

            GUI.EndGroup();
        }        
    }
}