using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIMatchMaking : FresviiGUIFrame
    {
        public enum Mode { Initializing, Setting, Matching, None };

        public Mode State = Mode.Initializing;

        public float sideMargin;

        private Texture2D palette;

        private FresviiGUIMatchMakingTop matchMakingTopMenu;

        private Rect baseRect;
        
        private Rect scrollViewRect;
        
        private float scaleFactor;
        
        public float topMargin;

        public float cardMargin;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;
        
        public Vector2 loadingSpinnerSize;

        private List<FresviiGUIMatchPlayerCard> playerCards = new List<FresviiGUIMatchPlayerCard>();

        public GameObject prfbPlayerCard;

        private bool initialized;

		public float pollingInterval = 10f;

        private static uint? minNumberOfPlayers = 2;

        private static uint? maxNumberOfPlayers = 2;
        
        private static string[] inviteUsers;
        
        private static string invitationMessage;

        private static string segment;

        private static FASMatchMaking.Recipient recipient;

        private Fresvii.AppSteroid.Models.MatchMakingRequest matchMakingRequest;

        private Fresvii.AppSteroid.Models.Match match;

        private Fresvii.AppSteroid.Models.MatchMakingInvitation invitaion;

        private Vector2 scrollPosition = Vector2.zero;

        public List<Fresvii.AppSteroid.Models.User> SelectedUsers { get; protected set; }

        public Texture2D textureTagYou;

        public static bool IsShowing = false;

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            SelectedUsers = new List<Fresvii.AppSteroid.Models.User>();

            if (FASGui.invitedMatch != null)
            {
                this.match = FASGui.invitedMatch;

                FASGui.invitedMatch = null;

                this.invitaion = FASGui.matchMakingInvitation;

                FASGui.matchMakingInvitation = null;

                FresviiGUIMatchMaking.minNumberOfPlayers = (uint) match.CurrentMinPlayers;

                FresviiGUIMatchMaking.maxNumberOfPlayers = (uint) match.CurrentMaxPlayers;

                State = Mode.Matching;
            }
            else
            {
                FresviiGUIMatchMaking.minNumberOfPlayers = FASGui.MatchMakingMinNumberOfPlaysers;

                FresviiGUIMatchMaking.maxNumberOfPlayers = FASGui.MatchMakingMaxNumberOfPlaysers;

                FresviiGUIMatchMaking.inviteUsers = FASGui.MatchMakingInviteUsers;

                FresviiGUIMatchMaking.invitationMessage = FASGui.MatchMakingInvitationMessage;

                FresviiGUIMatchMaking.segment = FASGui.MatchMakingSegment;

                FresviiGUIMatchMaking.recipient = FASGui.MatchMakingRecipient;
            }

            this.GuiDepth = FASGui.GuiDepthBase;

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            matchMakingTopMenu = GetComponent<FresviiGUIMatchMakingTop>();

            matchMakingTopMenu.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

            this.scaleFactor = scaleFactor;
            
            loadingSpinnerSize *= scaleFactor;
            
            sideMargin *= scaleFactor;
            
            topMargin = scaleFactor;

            loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, GuiDepth - 10);

            SetScrollSlider(this.scaleFactor * 2.0f);

            textureTagYou = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.TagYou + postFix, false);

            for (int i = 0; i < FresviiGUIMatchMaking.maxNumberOfPlayers; i++)
            {
                FresviiGUIMatchPlayerCard card = ((GameObject)Instantiate(prfbPlayerCard)).GetComponent<FresviiGUIMatchPlayerCard>();

                card.Init(this.scaleFactor, this, this);

                playerCards.Add(card);
            }

            if (this.match == null) 
            {
                FASMatchMaking.GetMatchMakingRequest(OnGetMatchMakingRequest);
            }
            else // invitied match
            {
                loadingSpinner.Hide();

                UpdatePlayers();

				State = Mode.Matching;

				matchMakingTopMenu.SetRightButtonLabel(State);

                StartCoroutine(MatchMakingGetMatchPolling(this.match.Id));
            }
		
            matchMakingTopMenu.SetSubTitle((uint)minNumberOfPlayers, (uint)maxNumberOfPlayers);
		}

        void OnEnable()
        {
            FASEvent.OnMatchMakingMatchUpdated += OnMatchMakingMatchUpdated;

			FASEvent.OnMatchMakingGameContextCreated += OnMatchMakingGameContextCreated;

			FASMatchMaking.latestGameContext = null;

            IsShowing = true;
        }

        void OnDisable()
        {
            FASEvent.OnMatchMakingMatchUpdated -= OnMatchMakingMatchUpdated;
		
			FASEvent.OnMatchMakingGameContextCreated -= OnMatchMakingGameContextCreated;
        }

		void OnMatchMakingGameContextCreated(Fresvii.AppSteroid.Models.GameContext gameContext)
		{
			FASMatchMaking.latestGameContext = gameContext;
		}

        public uint GetMaxNumberOfPlayers()
        {
            return (uint) maxNumberOfPlayers;
        }

        public void SetInviteFriends(List<Fresvii.AppSteroid.Models.User> selectedUsers)
        {
            for (int j = 1; j < playerCards.Count; j++)
            {
                playerCards[j].SetPlayer(null);
            }


            int i = 1;

            this.SelectedUsers = selectedUsers;

            foreach (Fresvii.AppSteroid.Models.User user in selectedUsers)
            {
                playerCards[i].SetPlayer(new Fresvii.AppSteroid.Models.Player(user));

                i++;

                if (i > playerCards.Count) break;
            }
        }

        public bool CanStartMatchMaking()
        {
            if (recipient == FASMatchMaking.Recipient.FriendOnly && SelectedUsers.Count + 1 < minNumberOfPlayers)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void OnGetMatchMakingRequest(Fresvii.AppSteroid.Models.MatchMakingRequest matchMakingRequest, Fresvii.AppSteroid.Models.Error error)
        {
			if(this == null) return;

            loadingSpinner.Hide();

            if (error == null)
            {
                this.matchMakingRequest = matchMakingRequest;

                State = Mode.Matching;

                matchMakingTopMenu.SetRightButtonLabel(State);

                matchMakingTopMenu.SetSubTitle((uint)FresviiGUIMatchMaking.minNumberOfPlayers, (uint)FresviiGUIMatchMaking.maxNumberOfPlayers);

                StartCoroutine(MatchMakingGetMatchPolling(matchMakingRequest.Match.Id));
            }
            else
            {
                State = Mode.Setting;

                matchMakingTopMenu.SetRightButtonLabel(State);

                StartCoroutine(SetAccountPlayer());
            }

            matchMakingTopMenu.SetRightButtonLabel(State);
        }

        IEnumerator SetAccountPlayer()
        {
            while (FAS.CurrentUser == null)
            {
                yield return 1;
            }

            playerCards[0].SetPlayer(new Fresvii.AppSteroid.Models.Player(FAS.CurrentUser));
        }

        private void OnCreateMatchMakingRequest(Fresvii.AppSteroid.Models.MatchMakingRequest matchMakingRequest, Fresvii.AppSteroid.Models.Error error)
        {
            if (error == null)
            {
                this.matchMakingRequest = matchMakingRequest;

				matchMakingTopMenu.SetSubTitle(matchMakingRequest.MinNumberOfPlayers, matchMakingRequest.MaxNumberOfPlayers);

                StartCoroutine(MatchMakingGetMatchPolling(matchMakingRequest.Match.Id));

                State = Mode.Matching;

                matchMakingTopMenu.SetRightButtonLabel(State);
            }
            else
            {
                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("CreateMatchMakingRequestError"), delegate(bool del) { });

                if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                {
                    Debug.LogError("CreateMatchMakingRequest Error : " + error.ToString());
                }

                State = Mode.Setting;

                matchMakingTopMenu.SetRightButtonLabel(State);
            }
        }

        IEnumerator MatchMakingGetMatchPolling(string matchId)
		{
            bool polling = true;

            while (this.gameObject != null && this.gameObject.activeInHierarchy && polling)
			{
                // GetMatch
                FASMatchMaking.GetMatch(matchId, (match, error)=>
                {
                    if (this == null || !polling) return;

                    if (error == null && match != null)
                    {
                        OnMatchMakingMatchUpdated(match);

                        if (match.Status == Fresvii.AppSteroid.Models.Match.Statuses.Complete)
                        {
                            FASEvent.OnMatchMakingCompleteByPolling(match);
                        }
                        else if (match.Status == Fresvii.AppSteroid.Models.Match.Statuses.Disposed)
                        {
                            MatchDisposed();

                            polling = false;
                        }
                    }
                    else
                    {
                        MatchDisposed();

                        polling = false;
                    }
                });

				yield return new WaitForSeconds(pollingInterval);
			}
		}

        void MatchDisposed()
        {
            if (this.gameObject == null) return;

            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

            if (!matchDisposed)
            {
                if (this.invitaion == null)
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("MatchDisposed"), delegate(bool del)
                    {
                        if (del)
                        {
                            FASGui.ShowMatchMakingGui(
                                FresviiGUIMatchMaking.minNumberOfPlayers,
                                FresviiGUIMatchMaking.maxNumberOfPlayers,
                                FresviiGUIMatchMaking.inviteUsers,
                                FresviiGUIMatchMaking.invitationMessage,
                                FresviiGUIMatchMaking.segment,
                                FresviiGUIMatchMaking.recipient
                            );
                        }
                        else
                        {
                            FresviiGUIManager.Instance.LoadScene();
                        }
                    });
                }
                else
                {
                    Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("MatchMakingCanceled"), delegate(bool del)
                    {
                        if (del)
                        {
                            FASGui.ShowMatchMakingGui(
                                FresviiGUIMatchMaking.minNumberOfPlayers,
                                FresviiGUIMatchMaking.maxNumberOfPlayers,
                                FresviiGUIMatchMaking.inviteUsers,
                                FresviiGUIMatchMaking.invitationMessage,
                                FresviiGUIMatchMaking.segment,
                                FresviiGUIMatchMaking.recipient
                            );
                        }
                        else
                        {
                            FresviiGUIManager.Instance.LoadScene();
                        }
                    });
                }

                SelectedUsers.Clear();

                matchDisposed = true;
            }
        }

        public void OnTapStartMatchMaking()
        {
            if (SelectedUsers != null)
            {
                List<string> users = new List<string>();

                foreach (Fresvii.AppSteroid.Models.User selectedUser in SelectedUsers)
                {
                    users.Add(selectedUser.Id);

                    inviteUsers = users.ToArray();
                }
            }

            matchMakingTopMenu.SetRightButtonLabel(Mode.None);

            if (recipient == FASMatchMaking.Recipient.FriendOnly)
            {
                FASMatchMaking.CreateMatchMakingRequest(minNumberOfPlayers, (uint?)(inviteUsers.Length + 1), inviteUsers, invitationMessage, segment, recipient, OnCreateMatchMakingRequest);

                if (playerCards.Count > (inviteUsers.Length + 1))
                {
                    for (int i = playerCards.Count - 1; i >= inviteUsers.Length + 1; i--)
                    {
                        FresviiGUIMatchPlayerCard card = playerCards[i];

                        playerCards.RemoveAt(i);

                        Destroy(card.gameObject);
                    }
                }
            }
            else
            {
                FASMatchMaking.CreateMatchMakingRequest(minNumberOfPlayers, maxNumberOfPlayers, inviteUsers, invitationMessage, segment, recipient, OnCreateMatchMakingRequest);
            }
        }

        public void OnTapCancelMatch()
        {
            matchMakingTopMenu.SetRightButtonLabel(Mode.None);

            if (invitaion != null)
            {
                ControlLock = true;

                loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition);

                FASMatchMaking.DeclineMatchMakingInvitation(invitaion.Id, (e) => 
                {
                    ControlLock = false;

                    loadingSpinner.Hide();

                    if (e != null)
                    {
                        if (e.Code == (int)Fresvii.AppSteroid.Models.Error.ErrorCode.CannotCancelMatch)
                        {
                            FresviiGUIManager.Instance.LoadScene();
                        }
                        else
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), (del) => { });
                        }

                        Debug.LogError("DeclineMatchMakingInvitation : " + e.ToString());

                        matchMakingTopMenu.SetRightButtonLabel(State);
                    }
                    else
                    {
                        FresviiGUIManager.Instance.LoadScene();

                        Debug.Log("Invited match cancelled");
                    }
                });

            }
            else if (matchMakingRequest != null)
            {
                ControlLock = true;

                loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition);

                FASMatchMaking.CancelMatchMakingRequest(matchMakingRequest.Id, (e) =>
                {
                    ControlLock = false;

                    loadingSpinner.Hide();

                    if (e != null)
                    {
                        if (e.Code == (int) Fresvii.AppSteroid.Models.Error.ErrorCode.CannotCancelMatch)
                        {
                            FresviiGUIManager.Instance.LoadScene();
                        }
                        else
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("UnknownError"), (del) => { });
                        }

                        matchMakingTopMenu.SetRightButtonLabel(State);

                        Debug.LogError("CancelMatchMakingRequest : " + e.ToString());
                    }
                    else
                    {
                        FresviiGUIManager.Instance.LoadScene();

                        Debug.Log("Matchmaking was cancelled");
                    }
                });
            }
            else
            {
                FresviiGUIManager.Instance.LoadScene();
            }
        }

        void OnDestroy()
        {
            if (matchMakingRequest != null)
            {
                FASMatchMaking.CancelMatchMakingRequest(matchMakingRequest.Id, (e)=>{});

                matchMakingRequest = null;
            }

            IsShowing = false;
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            matchMakingTopMenu.enabled = on;

            if (!on)
            {
                if (loadingSpinner != null)
                    loadingSpinner.Hide();

                scrollViewRect.y = 0f;
            }
        }

        private bool matchDisposed = false;

        public void OnMatchMakingMatchUpdated(Fresvii.AppSteroid.Models.Match match)
        {
            if (matchDisposed) return;

            if (match.Status == Fresvii.AppSteroid.Models.Match.Statuses.Disposed)
            {
				matchDisposed = true;

                SelectedUsers.Clear();

                Fresvii.AppSteroid.Util.DialogManager.Instance.SetLabel(FresviiGUIText.Get("OK"), FresviiGUIText.Get("Cancel"), FresviiGUIText.Get("Close"));

                Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog(FresviiGUIText.Get("MatchDisposed"), delegate(bool del)
                {
                    FresviiGUIManager.Instance.LoadScene();
                });

                return;
            }

            this.match = match;
		
			UpdatePlayers();
		}

        IEnumerator DelaySetPlayer(FresviiGUIMatchPlayerCard card, Fresvii.AppSteroid.Models.Player player, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            card.SetPlayer(player);
        }

        void UpdatePlayers()
        {
            // Player cancelled?
            foreach (FresviiGUIMatchPlayerCard card in playerCards)
            {
                if (card.Player == null)
                {
                    continue;
                }
                else
                {
                    bool playerExists = false;

                    foreach (Fresvii.AppSteroid.Models.Player player in this.match.Players)
                    {
                        if (card.Player.User.Id == player.User.Id)
                        {
                            playerExists = true;

                            break;
                        }
                    }

                    if (!playerExists) // this card player was cancelled
                    {
                        card.Player.Status = Fresvii.AppSteroid.Models.Player.Statuses.Cancelled;

                        if (recipient == FASMatchMaking.Recipient.Everyone)
                        {
                            StartCoroutine(DelaySetPlayer(card, null, 2.0f));
                        }
                    }
                }
            }

            foreach (Fresvii.AppSteroid.Models.Player player in this.match.Players)
            {
                bool playerExists = false;

                foreach (FresviiGUIMatchPlayerCard card in playerCards)
                {
                    if (card.Player == null) continue;

                    if (card.Player.User.Id == player.User.Id)
                    {
                        playerExists = true;

                        card.SetPlayer(player);

                        break;
                    }
                }

                if (!playerExists)
                {
                    foreach (FresviiGUIMatchPlayerCard card in playerCards)
                    {
                        if (card.Player == null)
                        {
                            card.SetPlayer(player);

                            break;
                        }
                    }
                }
            }
        }        

        float CalcScrollViewHeight()
        {
            float height = 0.0f;

            height = topMargin;

            foreach (FresviiGUIMatchPlayerCard card in playerCards)
            {
                height += card.GetHeight() + cardMargin;
            }

            return height;
        }

        void Update()
        {
            this.baseRect = new Rect(Position.x, Position.y + matchMakingTopMenu.height, Screen.width, Screen.height - matchMakingTopMenu.height - FresviiGUIFrame.OffsetPosition.y);

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            if(loadingSpinner != null)
                loadingSpinnerPosition = new Rect(Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

            float scrollViewHeight = CalcScrollViewHeight();

            InertiaScrollView(ref scrollPosition, ref scrollViewRect, scrollViewHeight, baseRect);
        }

        void OnUpdateScrollPosition(Vector2 value)
        {
            scrollPosition = value;
        }

        private FresviiGUIFrame nextFrame;

        public GameObject prfbGUISelectFriends;

        public void SelectFriends()
        {
            nextFrame = ((GameObject)Instantiate(prfbGUISelectFriends)).GetComponent<FresviiGUIFrame>();

            FresviiGUISelectFriends selectFriends = nextFrame.GetComponent<FresviiGUISelectFriends>();

            selectFriends.SetInitSelectedFriends(this.SelectedUsers);

            selectFriends.SetCallback((selectedFriends) =>
            {
                this.SetInviteFriends(selectedFriends);
            });

            selectFriends.SelectableMaxCount = this.GetMaxNumberOfPlayers() - 1; // Reduce my count

            nextFrame.Init(null, FresviiGUIManager.postFix, FresviiGUIManager.scaleFactor, this.GuiDepth - 100);

            nextFrame.transform.parent = this.transform;

            nextFrame.SetDraw(true);

            nextFrame.PostFrame = this;

            nextFrame.Tween(new Vector2(0.0f, Screen.height), Vector2.zero, delegate() { this.SetDraw(false); });                
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(scrollViewRect);

            //  Friend cards
            float cardY = topMargin;
           
            foreach (FresviiGUIMatchPlayerCard card in playerCards)
            {
                Rect cardPosition = new Rect(0f, cardY, backgroundRect.width, card.GetHeight());

                Rect drawPosition = new Rect(cardPosition.x, scrollViewRect.y + cardPosition.y, cardPosition.width, cardPosition.height);

                if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                {
                    card.Draw(cardPosition);
                }

                cardY += cardPosition.height + cardMargin;                
            }

            GUI.EndGroup();

            GUI.EndGroup();

        }

        public float MatchMakingProgress()
        {
            if (match == null)
                return 0.0f;

            float timeoutSeconds = FASMatchMaking.GetMatchMakingTimeOutSeconds();

            System.TimeSpan ts = System.DateTime.Now.Subtract(match.CreatedAt);

            float subtractMilliSeconds = ts.Seconds * 1000f + ts.Milliseconds;

            subtractMilliSeconds = Mathf.Clamp(subtractMilliSeconds, 0f, (float)timeoutSeconds * 1000f);

            return subtractMilliSeconds / (timeoutSeconds * 1000f);
        }

        float MatchMakingRemainingTimeSconds()
        {
            if (match == null)
                return 0.0f;

            float timeoutSeconds = FASMatchMaking.GetMatchMakingTimeOutSeconds();

            System.TimeSpan ts = System.DateTime.Now.Subtract(match.CreatedAt);

            float subtractSeconds = Mathf.Clamp(0f, timeoutSeconds, ts.Seconds);

            return timeoutSeconds - subtractSeconds;
        }
    }
}
