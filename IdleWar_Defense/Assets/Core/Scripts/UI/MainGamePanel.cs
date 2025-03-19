using System;
using System.Collections;
using System.Collections.Generic;
using Core.Scripts.UI.Drone_Panel;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Pattern.UI;
using Utilities.Inspector;

using TMPro;
using Utilities.Components;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace FoodZombie.UI
{
    public class MainGamePanel : MyGamesBasePanel
    {
        #region Members

        public static MainGamePanel mInstance;
        public static MainGamePanel instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = FindObjectOfType<MainGamePanel>();
                return mInstance;
            }
        }

        public Action<MyGamesBasePanel> onAnyChildShow;
        public Action<MyGamesBasePanel> onAnyChildHide;

        [Separator("Common")]
        [SerializeField] private Button mBtnBackLayer;

        [Separator("Build-in Panels")]
        [SerializeField] private HubPanel mHubPanel;

        [Separator("Panel Prefabs")]
        [SerializeField] private MissionDetailPanel mMissionDetailPanel;
        [SerializeField] private MissionDetailPanel mMissionDetailPvPPanel;
        [SerializeField] private PvPMainPanel mPvPMainPanel;
        [SerializeField] private PausePanel mPausePanel;
        [SerializeField] private WinPanel mWinPanel;
        [SerializeField] private LosePanel mLosePanel;
        [SerializeField] private PvPEndGamePanel mPvPEndGamePanel;
        [SerializeField] private HeroDamageStatisticsPanel mHeroDamagePanel;
        [SerializeField] private MessagesPopup mMessagePopup;
        [SerializeField] private RewardsPopup mRewardsPopup;
        [SerializeField] private WaitingPanel mWaitingPanel;
        [SerializeField] private DroneConfirmWatchAdPanel mDroneWatchAdPopup;
        [SerializeField] private DronePayGemPanel mDronePayGemPopup;
        [SerializeField] private ItemDetailPanel mItemDetailPanel;
        [SerializeField] private GearDetailPanel mGearDetailPanel;
        [Separator("UI Widgets")]
        [SerializeField] private UITooltips mUIWidgets;
        [SerializeField] private UIEffects mEffects;

        public HubPanel HubPanel => GetCachedPanel(mHubPanel);
        public MissionDetailPanel MissionDetailPanel => GetCachedPanel(mMissionDetailPanel);
        public WinPanel WinPanel => GetCachedPanel(mWinPanel);

        public PvPEndGamePanel PvPEndGamePanel => GetCachedPanel(mPvPEndGamePanel);
        public LosePanel LosePanel => GetCachedPanel(mLosePanel);
        public UITooltips UITooltips => mUIWidgets;

        /// <summary>
        /// Cached for once use panel
        /// </summary>
        private Dictionary<int, MyGamesBasePanel> mCachedPanels;
        private Queue<MessagesPopup.Message> mMessageQueue = new Queue<MessagesPopup.Message>();
        private bool mInitialized;

        private GameData GameData => GameData.Instance;
        private GameplayController GameplayController => GameplayController.Instance;

        public bool Initialized => mInitialized;

        #endregion

        //=============================================

        #region MonoBehaviour

        private void Start()
        {
            mBtnBackLayer.onClick.AddListener(BtnBackLayer_Pressed);

            var r = UnityEngine.Random.Range(0, 5);
            if (r == 0) SoundManager.Instance.PlayMusic(IDs.MUSIC_BATTLE_1);
            else if (r == 1) SoundManager.Instance.PlayMusic(IDs.MUSIC_BATTLE_2);
            else if (r == 2) SoundManager.Instance.PlayMusic(IDs.MUSIC_BATTLE_3);
            else if (r == 3) SoundManager.Instance.PlayMusic(IDs.MUSIC_BATTLE_4);
            else if (r == 4) SoundManager.Instance.PlayMusic(IDs.MUSIC_BATTLE_5);

            Config.backToHomePanel = SceneName.NONE;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //chỉ xử lý cho Trường hợp tutorial nên ko đưa vào hàm BtnBackLayer_Pressed
                if (!TutorialsManager.Instance.IsShowingFixedTut) BtnBackLayer_Pressed();
            }
        }

        private void OnDestroy()
        {
            //EventDispatcher.RemoveListener<ShowRewardsEvent>(OnShowRewardsEvent);
        }

        #endregion

        //=============================================

        #region Public

        internal override void Init()
        {
            InitPanels();
            //ShowMissionDetailPanel(() =>
            //{
            //	GameplayController.Init(GetCachedPanel(mHubPanel));
            //});
            GameplayController.Init(GetCachedPanel(mHubPanel));
            //EventDispatcher.AddListener<ShowRewardsEvent>(OnShowRewardsEvent);

            mInitialized = true;
        }

        public void ShowHubPanel()
        {
            PushPanelToTop(ref mHubPanel);
        }
        public void ShowMissionDetailPanel(Action OnShowDone)
        {
            //   GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.CLICK_MISSION);
            if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
            {
                PushPanelToTop(ref mPvPMainPanel);
                mPvPMainPanel.CallStartInGame();
                mPvPMainPanel.OnShowDone = OnShowDone;
            }
            else
            {
                PushPanelToTop(ref mMissionDetailPanel);
                mMissionDetailPanel.OnShowDone = OnShowDone;
            }

            //	mMissionDetailPanel.Init();
        }
        public void ShowPausePanel()
        {
            PushPanelToTop(ref mPausePanel);
        }

        public void ShowWinPanel(List<RewardInfo> mainRewardInfos, List<RewardInfo> bonusRewardInfos)
        {
            PushPanelToTop(ref mWinPanel);
            mWinPanel.Init(mainRewardInfos, bonusRewardInfos);
        }

        public void ShowPvPEndgamePanel(PvPScoreInGamePanel pvpScoreIngame)
        {
            PushPanelToTop(ref mPvPEndGamePanel);
            mPvPEndGamePanel.Init(pvpScoreIngame);
        }

        public void ShowHeroStatisticsPanel(Dictionary<int, double> heroDamageDict)
        {
            PushPanelToTop(ref mHeroDamagePanel);
            mHeroDamagePanel.Init(heroDamageDict);
        }
        public void ShowLosePanel(RewardInfo reward)
        {
            PushPanelToTop(ref mLosePanel);
            mLosePanel.Init(reward);
        }

        public void ShowMessageTooltip(RectTransform pTarget, UITooltips.Message pMessage)
        {
            if (mUIWidgets.gameObject.IsPrefab())
                mUIWidgets = Instantiate(mUIWidgets, transform);

            mUIWidgets.SetActive(true);
            mUIWidgets.ShowSimpleMessage(pTarget, pMessage);
            mUIWidgets.transform.SetAsLastSibling();
        }

        internal bool IsBusy()
        {
            foreach (var p in panelStack)
                if (!p.CanPop())
                    return true;
            return false;
        }

        public void ShowWarningTooltip(RectTransform pTarget, UITooltips.Message pMessage)
        {
            if (mUIWidgets.gameObject.IsPrefab())
                mUIWidgets = Instantiate(mUIWidgets, transform);

            mUIWidgets.SetActive(true);
            mUIWidgets.ShowWarning(pTarget, pMessage);
            mUIWidgets.transform.SetAsLastSibling();
        }

        //public void ShowLockReasonMessage(RectTransform pTarget, LockReason pReason)
        //{
        //    if (mUIWidgets.gameObject.IsPrefab())
        //        mUIWidgets = Instantiate(mUIWidgets, transform);

        //    var message = new UITooltips.Message();
        //    message.size = new Vector2(500, 100);
        //    switch (pReason.lockReasonType)
        //    {
        //        case LockReasonType.NotEnoughItems:
        //            message.message = $"Not enough items"; break;
        //        case LockReasonType.NotEnoughVehicleLevel:
        //            message.message = $"Need Plane Level {pReason.requiredValue}"; break;
        //        case LockReasonType.NotEnoughPlayerLevel:
        //            message.message = $"Need Player Level {pReason.requiredValue}"; break;
        //        case LockReasonType.NotEnoughMoney:
        //            message.message = $"Not enough resources"; break;
        //        case LockReasonType.NotHired:
        //            message.message = $"Unit is not recruited yet"; break;
        //        case LockReasonType.NotUnlocked:
        //            message.message = $"Unit is not unlocked yet"; break;
        //        case LockReasonType.FullSlot:
        //            message.message = "Full Slot"; break;
        //        case LockReasonType.Existed:
        //            message.message = "Existed"; break;
        //    }

        //    mUIWidgets.SetActive(true);
        //    mUIWidgets.ShowWarning(pTarget, message);
        //    mUIWidgets.transform.SetAsLastSibling();
        //}

        public MessageWithPointer ShowNotificationBoard(RectTransform pTarget, string pMessage, Alignment pAlign, Vector2 pPos, bool pDisplayArrow = true, bool pDisplayMessage = true, bool pDisplaySkip = false, bool pnotUpdatePoint = false)
        {
            return ShowNotificationBoard(new UITooltips.Notification(0)
            {
                target = pTarget,
                alignment = pAlign,
                message = pMessage,
                pos = pPos,
                displayArrow = pDisplayArrow,
                displayMessage = pDisplayMessage,
                displaySkip = pDisplaySkip,
                notUpdatePoint= pnotUpdatePoint
            });
        }

        public MessageWithPointer ShowNotificationBoard(UITooltips.Notification pNotification)
        {
            if (mUIWidgets.gameObject.IsPrefab())
                mUIWidgets = Instantiate(mUIWidgets, transform);
            mUIWidgets.SetActive(true);
            mUIWidgets.transform.SetAsLastSibling();
            var notiBoard = mUIWidgets.ShowNotificationBoard(pNotification);
            return notiBoard;
        }

        public void HideNotificationBoard(int pId)
        {
            mUIWidgets.HideNotificationBoard(pId);
        }

        public void ShowOptionsToolTip(RectTransform pTarget, UITooltips.Option[] pOptions, float pWidth, Action pCancelAction = null)
        {
            if (mUIWidgets.gameObject.IsPrefab())
                mUIWidgets = Instantiate(mUIWidgets, transform);

            mUIWidgets.SetActive(true);
            mUIWidgets.ShowOptions(pTarget, pOptions, pWidth, pCancelAction);
            mUIWidgets.transform.SetAsLastSibling();
        }

        public void ShowTradeToolTip(RectTransform pTarget, UITooltips.QuickTrade pOption)
        {
            if (mUIWidgets.gameObject.IsPrefab())
                mUIWidgets = Instantiate(mUIWidgets, transform);

            mUIWidgets.SetActive(true);
            mUIWidgets.ShowTradeOption(pTarget, pOption);
            mUIWidgets.transform.SetAsLastSibling();
        }

        public void SpawnEffect(RectTransform pFrom, UIEffects.Info pInfo)
        {
            if (mEffects.gameObject.IsPrefab())
                mEffects = Instantiate(mEffects, transform);

            mEffects.SetActive(true);
            mEffects.SpawnEffect(pFrom, pInfo);
            mEffects.transform.SetAsLastSibling();
        }

        public void SpawnEffect(RectTransform pFrom, RewardInfo pReward)
        {
            if (mEffects.gameObject.IsPrefab())
                mEffects = Instantiate(mEffects, transform);

            mEffects.SetActive(true);
            mEffects.SpawnEffectRandomNearByFrom(pFrom, pReward);
            mEffects.transform.SetAsLastSibling();
        }

        public void ShowWarningPopup(string txtMessage, Action OnConfirm = null, bool canIgnore = true)
        {
            var message = new MessagesPopup.Message()
            {
                yesAction = () => { if (OnConfirm != null) OnConfirm(); },
                noAction = null,
                additionalAction = null,
                yesActionLabel = "Ok",
                allowIgnore = canIgnore,
                title = "MESSAGE",
                content = txtMessage,
                popupSize = new Vector2(960, 570),
                contentAignment = TextAlignmentOptions.Center
            };
            ShowMessagePopup(message);
        }

        public void ShowMessagePopup(MessagesPopup.Message pMessage)
        {
            if (mMessageQueue.Contains(pMessage))
                return;

            mMessageQueue.Enqueue(pMessage);
            if (mMessagePopup.IsShowing)
                return;

            ShowMessagePopup();
        }
        public void ShowWaitingPanel(bool show, string infor = "")
        {
            if (show)
            {
                PushPanelToTop(ref mWaitingPanel);
                mWaitingPanel.SetText(infor);
            }
            else
            {
                mWaitingPanel.NotShow();
            }
        }
        public void ShowRewardPopup(RewardInfo pReward, RectTransform pSpawnPosition = null, RewardsPopup.AlignType myAlign = RewardsPopup.AlignType.Horizontal, System.Action OnShowDone = null)
        {
            ShowRewardsPopup(new List<RewardInfo>() { pReward }, pSpawnPosition, myAlign, OnShowDone);
        }

        public void ShowRewardsPopup(List<RewardInfo> pRewards, RectTransform pSpawnPosition = null, RewardsPopup.AlignType myAlign = RewardsPopup.AlignType.Horizontal, System.Action OnShowDone = null)
        {
            PushPanelToTop(ref mRewardsPopup);
            mRewardsPopup.SetRewards(pRewards, pSpawnPosition, myAlign, OnShowDone);
        }

        public void ShowDroneWatchAdPopup(Action callback)
        {
            PushPanelToTop(ref mDroneWatchAdPopup);
            mDroneWatchAdPopup.Init(callback);
        }

        public void ShowDronePayGemPopup(Action callback)
        {
            PushPanelToTop(ref mDronePayGemPopup);
            mDronePayGemPopup.Init(callback);
        }

        public void ShowItemDetailPanel(ItemView itemView, UnityAction refreshAction)
        {
            PushPanelToTop(ref mItemDetailPanel);
            mItemDetailPanel.Init(itemView, refreshAction);
        }
        public void ShowItemOtherDetailPanel(RewardInfo rewardInfor, UnityAction refreshAction)
        {
            if (mItemDetailPanel.InitPlayerEXP(rewardInfor, refreshAction)) PushPanelToTop(ref mItemDetailPanel);
        }
        public void ShowItemDetailPanel(TrapData trapData, int currencyId, UnityAction refreshAction)
        {
            if (mItemDetailPanel.Init(trapData, currencyId, refreshAction)) PushPanelToTop(ref mItemDetailPanel);
        }
        public void ShowGearDetailPanel(GearData gearData, UnityAction refreshAction, bool showbtnDisassemble = true)
        {
            PushPanelToTop(ref mGearDetailPanel);
            mGearDetailPanel.Init(gearData, refreshAction, showbtnDisassemble);
        }
        #endregion

        //==============================================

        #region Private

        private void InitPanels()
        {
            mCachedPanels = new Dictionary<int, MyGamesBasePanel>();

            var panels = new List<MyGamesBasePanel>();
            foreach (Transform t in transform)
            {
                var panel = t.GetComponent<MyGamesBasePanel>();
                if (panel != null)
                    panels.Add(panel);
            }

            foreach (var panel in panels)
                if (panel != this)
                    InitPanel(panel);

            ShowHubPanel();
        }

        private void InitPanel<T>(T pPanel) where T : MyGamesBasePanel
        {
            if (!pPanel.gameObject.IsPrefab())
            {
                pPanel.SetActive(false);
                pPanel.Init();
            }
        }

        private void BtnBackLayer_Pressed()
        {
            if (TopPanel == mHubPanel)
            {
                var isWinIntroMission = GameData.Instance.MissionsGroup.IsWinIntroMission();
                var currentMissionId = GameData.Instance.MissionsGroup.CurrentMissionId;
                if (isWinIntroMission && currentMissionId != 1001 && currentMissionId != 1002)
                {
                    ShowPausePanel();
                }
                return;
            }

            if (TopPanel != null)
                TopPanel.Back();
        }

        protected override void OnAnyChildHide(PanelController pLastTop)
        {
            base.OnAnyChildHide(pLastTop);

            if (TopPanel == null || TopPanel is HubPanel)
                mBtnBackLayer.SetActive(false);
            else
            {
                mBtnBackLayer.SetActive(true);
                mBtnBackLayer.transform.SetSiblingIndex(0);//reset về 0 để tránh tình trạng lùi BackLayer quá HubPanel
                mBtnBackLayer.transform.SetSiblingIndex(TopPanel.transform.GetSiblingIndex() - 1);
            }

            onAnyChildHide.Raise(pLastTop as MyGamesBasePanel);
        }

        protected override void OnAnyChildShow(PanelController pPanel)
        {
            base.OnAnyChildShow(pPanel);

            if (TopPanel == null || TopPanel is HubPanel)
                mBtnBackLayer.SetActive(false);
            else
            {
                mBtnBackLayer.SetActive(true);
                mBtnBackLayer.transform.SetSiblingIndex(0);//reset về 0 để tránh tình trạng lùi BackLayer quá HubPanel
                mBtnBackLayer.transform.SetSiblingIndex(TopPanel.transform.GetSiblingIndex() - 1);
            }

            onAnyChildShow.Raise(TopPanel as MyGamesBasePanel);
        }

        private void ShowMessagePopup()
        {
            PushPanelToTop(ref mMessagePopup);
            mMessagePopup.InitMessage(mMessageQueue.Peek());
            mMessagePopup.onDidHide += OnMessagePopupHidden;
        }

        private void OnMessagePopupHidden()
        {
            mMessagePopup.onDidHide -= OnMessagePopupHidden;
            mMessageQueue.Dequeue();

            if (mMessageQueue.Count > 0)
                ShowMessagePopup();
        }

        //private void OnShowRewardsEvent(ShowRewardsEvent e)
        //{
        //    ShowLevelUpPanel(e.rewards, null);
        //}

        #endregion
    }
}