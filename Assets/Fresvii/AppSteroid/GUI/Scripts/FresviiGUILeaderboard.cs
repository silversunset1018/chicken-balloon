using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUILeaderboard : FresviiGUIFrame
    {
        public static string leaderboardName = "";

        public static string leaderboardId = "";

        public float sideMargin;

        private Texture2D palette;

        private FresviiGUILeaderboardTop leaderboardsTopMenu;

        private FresviiGUITabBar tabBar;

        private Rect baseRect;
        
        private Rect scrollViewRect;

        private Rect scrollViewRectPos;
        
        private float scaleFactor;
        
        public float topMargin;

        public float cardMargin;

        public float segmentedCtrlHeight = 30f;

        public FresviiGUISegmentedControl segmentedCtrl;

        private FresviiGUIScoreCard[] myScoreCards;

        private List<FresviiGUIScoreCard>[] scoreCards;

        private Fresvii.AppSteroid.Models.ListMeta[] metaRankings;

        private GameObject[] rankingObjs;

        public GameObject prfbPlayerCard;

        private bool initialized;

		public float pollingInterval = 15f;

        public float pullRefleshHeight = 50f;

        private bool loading;

        private Rect segmentedCtrlPosition;

        private Rect segmentedCtrlBgPosition;

        private List<string> labels;

        private Fresvii.AppSteroid.Models.Leaderboard leaderboard;

        public Fresvii.AppSteroid.Models.Leaderboard Leaderboard { get { return leaderboard; } }

        public float hMargin = 24f;

        public GUIStyle guiStyleLabel;

        private Rect youLabelPosition;

        private Rect allPlayersLabelPosition;

        private string strYou = "";
        private string strAllPlayers = "";

        public Texture2D textureMedal;

        [HideInInspector]
        public Texture2D textureTagFriend;

        [HideInInspector]
        public Texture2D textureTagYou;

        private Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

        private Rect loadingSpinnerPosition;

        public Vector2 loadingSpinnerSize;

        private static int loadedCount = 0;

        private bool loadBlock;

        private Fresvii.AppSteroid.Gui.LoadingSpinner bottomLoadingSpinner;

        private Rect bottomLoadingSpinnerPosition;

        public override void Init(Texture2D appIcon, string postFix, float scaleFactor, int guiDepth)
        {
            this.GuiDepth = FASGui.GuiDepthBase;

            leaderboardId = FASGui.LeaderboardId;

            leaderboardName = FASGui.LeaderboardName;

            FASLeaderboard.SetTotalizationClockUtcOffset(9);

            FASLeaderboard.SetDailyTotalizationStartTime(18, 0);

            FASLeaderboard.SetWeeklyTotalizationStartTime(System.DayOfWeek.Sunday, 18, 0);

            textureCoordsBackground = FresviiGUIColorPalette.GetTextureCoods(FresviiGUIColorPalette.MainBackground);

            this.scaleFactor = scaleFactor;
            
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                guiStyleLabel.font = null;

                guiStyleLabel.fontStyle = FontStyle.Bold;
            }

            guiStyleLabel.fontSize = (int)(guiStyleLabel.fontSize * scaleFactor);
            
            sideMargin *= scaleFactor;
            
            topMargin = scaleFactor;

            segmentedCtrlHeight *= scaleFactor;

            hMargin *= scaleFactor;

            loadingSpinnerSize *= scaleFactor;

            leaderboardsTopMenu = GetComponent<FresviiGUILeaderboardTop>();

            leaderboardsTopMenu.Init(appIcon, postFix, scaleFactor, GuiDepth - 1, this);

            tabBar = GetComponent<FresviiGUITabBar>();

            tabBar.Init(postFix, scaleFactor, this.GuiDepth - 1);

            labels = new List<string>();

            labels.Add(FresviiGUIText.Get("Today"));

            labels.Add(FresviiGUIText.Get("Weekly"));
            
            labels.Add(FresviiGUIText.Get("Total"));

            segmentedCtrl.Init(scaleFactor, postFix, labels, OnTapSegmentedControl);

            scoreCards = new List<FresviiGUIScoreCard>[labels.Count];

            pullRefleshHeight *= scaleFactor;

			loadedCount = 0;

            for (int i = 0; i < labels.Count; i++)
            {
                scoreCards[i] = new List<FresviiGUIScoreCard>();
            }

            metaRankings = new Fresvii.AppSteroid.Models.ListMeta[labels.Count];

            rankingObjs = new GameObject[labels.Count];

            for (int i = 0; i < labels.Count; i++)
            {
                rankingObjs[i] = new GameObject(labels[i]);

                rankingObjs[i].transform.parent = this.transform;

                rankingObjs[i].SetActive(false);
            }

            rankingObjs[segmentedCtrl.SelectedIndex].SetActive(true);

            myScoreCards = new FresviiGUIScoreCard[labels.Count];

            strYou = FresviiGUIText.Get("You");

            strAllPlayers = FresviiGUIText.Get("AllPlayers");

            textureMedal = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.LeaderboardMedal + postFix, false);

            textureTagFriend = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.TagFriend + postFix, false);

            textureTagYou = FresviiGUIManager.Instance.resourceManager.LoadTextureFromResource(FresviiGUIConstants.ResouceTextureFolderName + "/" + FresviiGUIConstants.TagYou + postFix, false);

            SetScrollSlider(scaleFactor * 2.0f);
        }

        void GetLeaderboard()
        {
            if (!string.IsNullOrEmpty(leaderboardId))
            {
                FASLeaderboard.GetLeaderboard(leaderboardId, delegate(Fresvii.AppSteroid.Models.Leaderboard leaderboard, Fresvii.AppSteroid.Models.Error error)
                {
                    if (error == null)
                    {
                        this.leaderboard = leaderboard;
                    }
                    else
                    {
                        if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        {
                            Debug.LogError("GetLeaderboard : " + error.ToString());
                        }
                    }
                });
            }
            else
            {
                FASLeaderboard.GetLeaderboards(delegate(IList<Fresvii.AppSteroid.Models.Leaderboard> leaderboards, Fresvii.AppSteroid.Models.Error error)
                {
					if(this == null) return;

                    if (error == null)
                    {
                        if (!string.IsNullOrEmpty(leaderboardName))
                        {
                            foreach (Fresvii.AppSteroid.Models.Leaderboard leaderboard in leaderboards)
                            {
                                if (leaderboard.Name == leaderboardName)
                                {
                                    this.leaderboard = leaderboard;

                                    return;
                                }
                            }
                        }

                        if (leaderboards.Count > 0)
                        {
                            this.leaderboard = leaderboards[0];
                        }
                    }
                    else
                    {
                        if (this.gameObject.activeInHierarchy)
                        {
                            Fresvii.AppSteroid.Util.DialogManager.Instance.ShowSubmitDialog("GetLeaderboards Error : " + error.ToString(), delegate(bool del) { });

                            if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                                Debug.LogError(error.ToString());
                        }
                    }
                });
            }
        }

        void OnEnable()
        {
            ControlLock = false;

            if (leaderboard == null)
            {
                GetLeaderboard();
            }

			StartCoroutine(Polling());
        }

        void OnDisable()
        {
            if (loadingSpinner != null)
                loadingSpinner.Hide();
    
            if (bottomLoadingSpinner != null)
                bottomLoadingSpinner.Hide();
    
        }

		IEnumerator Polling()
		{
			if (loadedCount < 3)
			{
				loadingSpinnerPosition = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
				
				loadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(loadingSpinnerPosition, this.GuiDepth - 20);
			}

            while (FAS.CurrentUser == null)
                yield return 1;

			while (leaderboard == null)
                yield return 1;

            if(initialized)
                yield return new WaitForSeconds(pollingInterval);

            while (this.gameObject.activeInHierarchy)
            {
                GetRankings();

                GetMyRanking();

                initialized = true;

                yield return new WaitForSeconds(pollingInterval);
            }

       }

        private void GetRanking(int index, uint page, Action callback)
        {
            FASLeaderboard.Period span = FASLeaderboard.Period.Whole;

            if (index == 0)
            {
                span = FASLeaderboard.Period.Daily;
            }
            else if (index == 1)
            {
                span = FASLeaderboard.Period.Weekly;
            }

            FASLeaderboard.GetRanking(leaderboard.Id, span, page, delegate(IList<Fresvii.AppSteroid.Models.Score> ranking, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error2)
            {
                if (error2 == null)
                {
                    metaRankings[index] = meta;

                    OnGetRanking(index, ranking);
                }
                else
                {
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        Debug.LogError(error2.ToString());
                }

                callback();
            });
        }

        private void GetRankings()
        {
            FASLeaderboard.GetRanking(leaderboard.Id, FASLeaderboard.Period.Daily, delegate(IList<Fresvii.AppSteroid.Models.Score> ranking, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error2)
            {
                if (error2 == null)
                {
                    if (metaRankings[0] == null)
                        metaRankings[0] = meta;

                    OnGetRanking(0, ranking);
                }
				else
				{
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
    					Debug.LogError(error2.ToString());
				}
            });

            FASLeaderboard.GetRanking(leaderboard.Id, FASLeaderboard.Period.Weekly, delegate(IList<Fresvii.AppSteroid.Models.Score> ranking, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error2)
            {
                if (error2 == null)
                {
                    if (metaRankings[1] == null)
                        metaRankings[1] = meta;

                    OnGetRanking(1, ranking);
                }
				else
				{
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
    					Debug.LogError(error2.ToString());
				}
			});

            FASLeaderboard.GetRanking(leaderboard.Id, delegate(IList<Fresvii.AppSteroid.Models.Score> ranking, Fresvii.AppSteroid.Models.ListMeta meta, Fresvii.AppSteroid.Models.Error error2)
            {
                if (error2 == null)
                {
                    if (metaRankings[2] == null)
                        metaRankings[2] = meta;

                    OnGetRanking(2, ranking);
                }
				else
				{
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
    					Debug.LogError(error2.ToString());
				}
			});
        }        

        private void GetMyRanking()
        {
            FASLeaderboard.GetUserRanking(leaderboard.Id, FAS.CurrentUser.Id, FASLeaderboard.Period.Daily, delegate(Fresvii.AppSteroid.Models.Rank rank, Fresvii.AppSteroid.Models.Error error)
            {
                if (error == null)
                {
                    if (rank != null)
                        OnGetMyRank(rank, 0);
                }
				else
				{
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        Debug.LogError(error.ToString());
				}

            });

            FASLeaderboard.GetUserRanking(leaderboard.Id, FAS.CurrentUser.Id, FASLeaderboard.Period.Weekly, delegate(Fresvii.AppSteroid.Models.Rank rank, Fresvii.AppSteroid.Models.Error error)
            {
                if (error == null)
                {
                    if (rank != null)
                        OnGetMyRank(rank, 1);
                }
				else
				{
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error)
                        Debug.LogError(error.ToString());
				}
			});

            FASLeaderboard.GetUserRanking(leaderboard.Id, FAS.CurrentUser.Id, delegate(Fresvii.AppSteroid.Models.Rank rank, Fresvii.AppSteroid.Models.Error error)
            {
                if (error == null)
                {
                    if(rank != null)
                        OnGetMyRank(rank, 2);
                }
				else
				{
                    if (FASConfig.Instance.logLevel <= FAS.LogLevels.Error) 
                        Debug.LogError(error.ToString());
				}
			});
        }

        void OnGetMyRank(Fresvii.AppSteroid.Models.Rank rank, int index)
        {
            if (rank == null) return;

            rank.Score.User = FAS.CurrentUser;

            if (myScoreCards[index] != null)
            {
                if(myScoreCards[index].rank.Score.User.ProfileImageUrl != rank.Score.User.ProfileImageUrl)
                    Destroy(myScoreCards[index]);
            }

            if (myScoreCards[index] == null)
            {
                myScoreCards[index] = ((GameObject)Instantiate(prfbPlayerCard)).GetComponent<FresviiGUIScoreCard>();

                myScoreCards[index].Init(rank, scaleFactor, this);

                myScoreCards[index].transform.parent = rankingObjs[index].transform;
            }
            else
            {
                myScoreCards[index].rank = rank;
            }
        }

        void OnGetRanking(int index, IList<Fresvii.AppSteroid.Models.Score> ranking)
        {
            if (++loadedCount >= 3)
            {
                if (loadingSpinner != null)
                {
                    loadingSpinner.Hide();
                }
            }


            foreach (Fresvii.AppSteroid.Models.Score score in ranking)
            {
                int existUserCardIndex = -1;

                for (int i = 0; i < scoreCards[index].Count; i++)
                {
                    if (scoreCards[index][i].rank.Score.User.Id == score.User.Id)
                    {
                        if (scoreCards[index][i].rank.Score.Value != score.Value)
                        {
                            existUserCardIndex = i;

                            break;
                        }
                    }
                }

                if (existUserCardIndex >= 0)
                {
                    FresviiGUIScoreCard card = scoreCards[index][existUserCardIndex];

                    scoreCards[index].RemoveAt(existUserCardIndex);

                    Destroy(card);
                }


                bool exists = false;

                foreach (FresviiGUIScoreCard card in scoreCards[index])
                {
                    if (card.rank.Score.Id == score.Id)
                    {
                        exists = true;

                        break;
                    }
                }

                if (!exists)
                {
                    FresviiGUIScoreCard card = ((GameObject)Instantiate(prfbPlayerCard)).GetComponent<FresviiGUIScoreCard>();

                    Fresvii.AppSteroid.Models.Rank rank = new Fresvii.AppSteroid.Models.Rank();

                    rank.Score = score;

                    card.rank = rank;

                    card.Init(rank, scaleFactor, this);

                    card.transform.parent = rankingObjs[index].transform;

                    scoreCards[index].Add(card);
                }
            }

            scoreCards[index].Sort(CompareScore);

            int preScoreValue = int.MinValue;

            for (int i = 0; i < scoreCards[index].Count; i++)
            {
                if (preScoreValue == scoreCards[index][i].rank.Score.Value)
                {
                    if (i != 0)
                    {
                        scoreCards[index][i].rank.Ranking = scoreCards[index][i - 1].rank.Ranking;
                    }
                    else
                    {
                        scoreCards[index][i].rank.Ranking = i + 1;
                    }
                }
                else
                {
                    scoreCards[index][i].rank.Ranking = i + 1;
                }

                preScoreValue = scoreCards[index][i].rank.Score.Value;
			}
        }

        int CompareScore(FresviiGUIScoreCard a, FresviiGUIScoreCard b)
        {
            if (b.rank.Score.Value == a.rank.Score.Value)
            {
                return System.DateTime.Compare(b.rank.Score.CreatedAt, a.rank.Score.CreatedAt);
            }
            else
            {
                return b.rank.Score.Value - a.rank.Score.Value;
            }
        }

        void CalcLayout()
        {
            //this.baseRect = new Rect(Position.x, Position.y + leaderboardsTopMenu.height, Screen.width, Screen.height - leaderboardsTopMenu.height - tabBar.height - FresviiGUIFrame.OffsetPosition.y);
            this.baseRect = new Rect(Position.x, Position.y + leaderboardsTopMenu.height, Screen.width, Screen.height - leaderboardsTopMenu.height - FresviiGUIFrame.OffsetPosition.y);

            segmentedCtrlPosition = new Rect(sideMargin, sideMargin, Screen.width - 2f * sideMargin, segmentedCtrlHeight);

            segmentedCtrlBgPosition = new Rect(0f, 0f, Screen.width, segmentedCtrlHeight + 2f * sideMargin);

            float height = 0.0f;

            if (myScoreCards[segmentedCtrl.SelectedIndex] != null)
            {
                youLabelPosition = new Rect(sideMargin, 0f, Screen.width, hMargin);

                height = youLabelPosition.height + myScoreCards[segmentedCtrl.SelectedIndex].GetHeight() + hMargin;
            }

            allPlayersLabelPosition = new Rect(sideMargin, height, Screen.width, hMargin);
        }

        float CalcScrollViewHeight()
        {
            float height = segmentedCtrlBgPosition.height;

            if (myScoreCards[segmentedCtrl.SelectedIndex] != null)
            {
                height += youLabelPosition.height;

                height += myScoreCards[segmentedCtrl.SelectedIndex].GetHeight() + hMargin;
            }

            height += hMargin;

            foreach (FresviiGUIScoreCard card in scoreCards[segmentedCtrl.SelectedIndex])
                height += card.GetHeight() + cardMargin;
            
            return height;
        }

        void Update(){

            backgroundRect = new Rect(Position.x, Position.y, Screen.width, Screen.height);

            CalcLayout();

            if (loadingSpinner != null)
            {
                loadingSpinner.Position = new Rect(Position.x + Screen.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + Screen.height * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }

            if (bottomLoadingSpinner != null)
            {
                bottomLoadingSpinner.Position = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, baseRect.y + baseRect.height - tabBar.height - pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);
            }

            float scrollViewHeight = CalcScrollViewHeight();

            scrollViewRectPos = new Rect(baseRect.x, baseRect.y + segmentedCtrlBgPosition.height, baseRect.width, baseRect.height - segmentedCtrlBgPosition.height);

            scrollViewRect.x = Position.x;

            scrollViewRect.width = Screen.width;

            scrollViewRect.height = scrollViewHeight;

            //InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, pullRefleshHeight, null, OnPullUpReflesh);

            InertiaScrollViewWithPullRefresh(ref scrollViewRect, baseRect, pullRefleshHeight, 0f, tabBar.height, null, OnPullUpReflesh);

            if (loadBlock && !FASGesture.IsDragging)
            {
                loadBlock = false;
            }
        }

        public void OnPullUpReflesh()
        {
            if (loading || loadBlock) return;

            loading = true;

            loadBlock = true;

            pullRefleshing = true;

            int index = segmentedCtrl.SelectedIndex;

            bool getData = false;

            if (metaRankings[index] != null)
            {
                if (metaRankings[index].NextPage.HasValue)
                {
                    getData = true;

                    bottomLoadingSpinnerPosition = new Rect(Position.x + baseRect.x + baseRect.width * 0.5f - loadingSpinnerSize.x * 0.5f, Position.y + baseRect.y + baseRect.height - pullRefleshHeight * 0.5f - loadingSpinnerSize.y * 0.5f, loadingSpinnerSize.x, loadingSpinnerSize.y);

                    bottomLoadingSpinner = Fresvii.AppSteroid.Gui.LoadingSpinner.Show(bottomLoadingSpinnerPosition, GuiDepth - 5);

                    GetRanking(index, (uint)metaRankings[index].NextPage, delegate()
                    {
                        loading = false;

                        pullRefleshing = false;
                        
                        if(bottomLoadingSpinner != null)
                            bottomLoadingSpinner.Hide();
                    });
                }
            }
           
            if(!getData)
            {
                loading = false;

                loadBlock = false;

                pullRefleshing = false;

                //OnCompletePullReflesh(scrollViewRect, scrollViewRectPos);
            }
        }

        public void OnUpdateScrollViewPosition(float value)
        {
            scrollViewRect.y = value;
        }
     
        void OnCompletePull()
        {

        }

        void OnTapSegmentedControl(int index)
        {
            for(int i = 0; i < rankingObjs.Length; i++)
            {
                rankingObjs[i].SetActive(i == index);
            }

            scrollViewRect.y = 0f;

            inertiaEscapeThisFrame = true;
        }

        public void BackToPostFrame()
        {            
            PostFrame.SetDraw(true);            

            this.Tween(Vector2.zero, new Vector2(Screen.width, 0.0f), delegate()
            {
                Destroy(this.gameObject);
            });

            PostFrame.Tween(new Vector2(-Screen.width, 0.0f), Vector2.zero, delegate() { });
        }

        public override void SetDraw(bool on)
        {
            if (on)
                FresviiGUIManager.Instance.CurrentView.CurrentFrame = this;

            this.enabled = on;

            tabBar.enabled = on;

            leaderboardsTopMenu.enabled = on;

            if (!on)
            {
                scrollViewRect.y = 0f;
            }
        }

        void OnGUI()
        {
            GUI.depth = GuiDepth;

            GUI.DrawTextureWithTexCoords(backgroundRect, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            if (loadedCount < 3)
            {                
                return;
            }

            GUI.BeginGroup(baseRect);

            GUI.BeginGroup(new Rect(0f, segmentedCtrlBgPosition.height, scrollViewRectPos.width, scrollViewRectPos.height));

            GUI.BeginGroup(new Rect(scrollViewRect.x, scrollViewRect.y, scrollViewRect.width, scrollViewRect.height));

            //  Player cards
            float cardY = hMargin;

            // My score
            if (myScoreCards[segmentedCtrl.SelectedIndex] != null)
            {
                GUI.Label(youLabelPosition, strYou, guiStyleLabel);

                Rect cardPosition = new Rect(0f, cardY, baseRect.width, myScoreCards[segmentedCtrl.SelectedIndex].GetHeight());

                Rect drawPosition = new Rect(cardPosition.x, scrollViewRect.y + cardPosition.y, cardPosition.width, cardPosition.height);

                if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                {
                    myScoreCards[segmentedCtrl.SelectedIndex].Draw(cardPosition, false);
                }

                cardY += youLabelPosition.height + cardPosition.height + hMargin;
            }

            // scores
            GUI.Label(allPlayersLabelPosition, strAllPlayers, guiStyleLabel);

            foreach (FresviiGUIScoreCard playerCard in scoreCards[segmentedCtrl.SelectedIndex])
            {
                Rect cardPosition = new Rect(0f, cardY, baseRect.width, playerCard.GetHeight());

                Rect drawPosition = new Rect(cardPosition.x, scrollViewRect.y + cardPosition.y, cardPosition.width, cardPosition.height);

                if (drawPosition.y + drawPosition.height > 0 && drawPosition.y < Screen.height)
                {
                    playerCard.Draw(cardPosition, true);
                }

                cardY += cardPosition.height + cardMargin;
            }

            GUI.EndGroup();

            GUI.EndGroup();

            GUI.DrawTextureWithTexCoords(segmentedCtrlBgPosition, FresviiGUIColorPalette.Palette, textureCoordsBackground);

            segmentedCtrl.Draw(segmentedCtrlPosition, Event.current);

            GUI.EndGroup();

        }       

        public void DestroySubFrames()
        {
            for (int i = 0; i < scoreCards.Length; i++)
            {
                foreach (FresviiGUIScoreCard playerCard in scoreCards[i])
                {
                    playerCard.DestroySubFrames();
                }
            }
        }
    }
}
