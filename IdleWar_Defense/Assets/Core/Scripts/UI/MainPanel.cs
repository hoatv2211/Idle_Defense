using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Pattern.UI;
using Utilities.Inspector;

using TMPro;
using UnityEngine.Events;
using Utilities.Components;
using UnityEngine.SceneManagement;

namespace FoodZombie.UI
{
	public class MyGamesBasePanel : PanelController
	{
		public bool playSound = true;

		protected override void BeforeShowing()
		{
			base.BeforeShowing();

			// if (playSound)
			//     SoundManager.Instance.PlaySFX(IDs.SOUND_POPUP_OPEN);
		}

		protected override void BeforeHiding()
		{
			base.BeforeHiding();

			// if (playSound)
			//     SoundManager.Instance.PlaySFX(IDs.SOUND_POPUP_CLOSE);
		}

		public bool IsActiveOrEnable()
		{
			return !gameObject.IsPrefab() && gameObject.activeSelf && this.isActiveAndEnabled;
		}
	}

	//=================================================

	public class MainPanel : MyGamesBasePanel
	{
		#region Members

		public static MainPanel mInstance;
		public static MainPanel instance
		{
			get
			{
				if (mInstance == null)
					mInstance = FindObjectOfType<MainPanel>();
				return mInstance;
			}
		}

		public Action<MyGamesBasePanel> onAnyChildShow;
		public Action<MyGamesBasePanel> onAnyChildHide;

		[Separator("Common")]
		[SerializeField] private Button mBtnBackLayer;

		[Separator("Build-in Panels")]
		[SerializeField] private MainMenuPanel mMainMenuPanel;

		[Separator("Panel Prefabs")]
		[SerializeField] private LoadingPanel mLoadingPanel;
		[SerializeField] private SettingPanel mSettingPanel;
		[SerializeField] private StorePanel mStorePanel;
		[SerializeField] private InappPopup mInappPopup;
		[SerializeField] private BuyCoinPanel mBuyCoinPanel;
		[SerializeField] private BuyTicketPanel mBuyTicketPanel;
		[SerializeField] private WheelsPanel mWheelsPanel;
		[SerializeField] private EventAllPanel mEventAllPanel;
		[SerializeField] private DailyQuestPanel mDailyQuestPanel;
		[SerializeField] private PremiumPassPanel mPremiumPassPanel;
		//[SerializeField] private SevenDaysBonusPanel mSevenDaysBonusPanel;
		//[SerializeField] private DailyLoginPanel mDailyLoginPanel;
		[SerializeField] private MessagesPopup mMessagesPopup;
		[SerializeField] private RewardsPopup mRewardsPopup;
		[SerializeField] private RatePanel mRatePanel;
		[SerializeField] private MissionDetailPanel mMissionDetailPanel;
		[SerializeField] private HeroPanel mHeroPanel;
		[SerializeField] private ListGearsPanel mListGearsPanel;
		[SerializeField] private BasePanel mBasePanel;
		[SerializeField] private InventoryPanel mInventoryPanel;
		[SerializeField] private GearDetailPanel mGearDetailPanel;
		[SerializeField] private SkillDetailPanel mSkillDetailPanel;
		[SerializeField] private ItemDetailPanel mItemDetailPanel;
		[SerializeField] private HeroFragmentDetailPanel mHeroFragmentDetailPanel;
		[SerializeField] private HeroStarUpPanel mHeroStarUpPanel;
		[SerializeField] private LaboratoryPanel mLaboratoryPanel;
		[SerializeField] private FormationPanel mFormationPanel;
		[SerializeField] private FactoryPanel mFactoryPanel;
		[SerializeField] private SummonGatePanel mSummonGatePanel;
		[SerializeField] private MapPanel mMapPanel;
		[SerializeField] private AFKRewardPanel mAFKRewardPanel;
		[SerializeField] private FastCollectPanel mFastCollectPanel;
		[SerializeField] private LevelUpPanel mLevelUpPanel;
		[SerializeField] private VipPanel mVipPanel;
		[SerializeField] private DailyGiftPanel mDailyGiftPanel;
		[SerializeField] private DiscoveryPanel mDiscoveryPanel;
		[SerializeField] private CampaignPanel mCampaignPanel;
		[SerializeField] private EvolutionPanel mEvolutionPanel;
		[SerializeField] private FormulaBookPanel mFormulaBookPanel;
		[SerializeField] private PackageDetailPanel mPackageDetailPanel;
		[SerializeField] private PvPMainPanel mPvPMainPanel;
		[SerializeField] private InfoUserPopup mInfoUserPopup;
		[SerializeField] private PvPSignup mPvPSignup;
		[SerializeField] private WaitingPanel mWaitingPanel;
		[Separator("UI Widgets")]
		[SerializeField] private UITooltips mUIWidgets;
		[SerializeField] private UIEffects mEffects;

		public MainMenuPanel MainMenuPanel => GetCachedPanel(mMainMenuPanel);
		public StorePanel StorePanel => GetCachedPanel(mStorePanel);
		public InappPopup InappPopup => GetCachedPanel(mInappPopup);
		public SummonGatePanel SummonGatePanel => GetCachedPanel(mSummonGatePanel);
		public BasePanel BasePanel => GetCachedPanel(mBasePanel);
		public FormationPanel FormationPanel => GetCachedPanel(mFormationPanel);
		public MapPanel MapPanel => GetCachedPanel(mMapPanel);
		public MissionDetailPanel MissionDetailPanel => GetCachedPanel(mMissionDetailPanel);
		public HeroPanel HeroPanel => GetCachedPanel(mHeroPanel);
		public RewardsPopup RewardsPopup => GetCachedPanel(mRewardsPopup);
		public MessagesPopup MessagesPopup => GetCachedPanel(mMessagesPopup);
		public WheelsPanel WheelsPanel => GetCachedPanel(mWheelsPanel);
		public EventAllPanel EventAllPanel => GetCachedPanel(mEventAllPanel);
		public DailyQuestPanel DailyQuestPanel => GetCachedPanel(mDailyQuestPanel);

		public PremiumPassPanel PremiumPassPanel => GetCachedPanel(mPremiumPassPanel);
		//  public SevenDaysBonusPanel SevenDaysBonusPanel => GetCachedPanel(mSevenDaysBonusPanel);
		//  public DailyLoginPanel DailyLoginPanel => GetCachedPanel(mDailyLoginPanel);
		public FastCollectPanel FastCollectPanel => GetCachedPanel(mFastCollectPanel);
		public AFKRewardPanel AFKRewardPanel => GetCachedPanel(mAFKRewardPanel);
		public DiscoveryPanel DiscoveryPanel => GetCachedPanel(mDiscoveryPanel);
		public FactoryPanel FactoryPanel => GetCachedPanel(mFactoryPanel);
		public LaboratoryPanel LaboratoryPanel => GetCachedPanel(mLaboratoryPanel);
		public EvolutionPanel EvolutionPanel => GetCachedPanel(mEvolutionPanel);
		public LevelUpPanel LevelUpPanel => GetCachedPanel(mLevelUpPanel);
		public InfoUserPopup InfoUserPopup => GetCachedPanel(mInfoUserPopup);

		public WaitingPanel WaitingPanel => GetCachedPanel(mWaitingPanel);
		public PvPSignup PvPSigup => GetCachedPanel(mPvPSignup);

		public UITooltips UITooltips => mUIWidgets;

		/// <summary>
		/// Cached for once use panel
		/// </summary>
		private Dictionary<int, MyGamesBasePanel> mCachedPanels;
		private Queue<MessagesPopup.Message> mMessagesQueue = new Queue<MessagesPopup.Message>();
		private bool mInitialized;

		private GameData GameData => GameData.Instance;

		public bool Initialized => mInitialized;

		#endregion

		//=============================================

		#region MonoBehaviour

		private void Start()
		{
//#if UNITY_EDITOR
//			Time.timeScale = 3;
//#endif

			mBtnBackLayer.onClick.AddListener(BtnBackLayer_Pressed);

			GameInitializer.Instance.SendLoadingTime();
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_MainMenu);
		}

		private void OnEnable()
		{
			StartCoroutine(IECustomUpdate());
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

		private IEnumerator IECustomUpdate()
		{
			var interval = new WaitForSeconds(5f);
			bool processing = false;
			while (true)
			{
				yield return interval;

				if (!processing && GameData.Instance.WaitingForAutoBackup)
				{
					processing = true;

					GameInitializer.Instance.BackupGameData((success) =>
					{
						processing = false;
						GameData.Instance.WaitForAutoBackup(false);
					});
				}
			}
		}

		#endregion

		//=============================================

		#region Public

		internal override void Init()
		{
			InitPanels();
			//EventDispatcher.AddListener<ShowRewardsEvent>(OnShowRewardsEvent);
			mInitialized = true;
		}

		private IEnumerator IEDelay(Action action)
		{
			yield return null;
			action?.Invoke();
		}

		public void ShowMainMenuPanel()
		{
			if (!mInitialized) PushPanelToTop(ref mMainMenuPanel);
			else
			{
				while (TopPanel != null && !(TopPanel is MainMenuPanel))
				{
					TopPanel.Back();
				}
			}
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_MainMenu);
		}

		//Fix login
		public void ShowMainMenuPanel2()
		{
			PushPanelToTop(ref mMainMenuPanel);
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_MainMenu);
		}

		public void ShowLoadingPanel()
		{
			PushPanelToTop(ref mLoadingPanel);
		}

		public void LoadGamePlayScreen()
		{
			PushPanelToTop(ref mLoadingPanel);
			mLoadingPanel.LoadGamePlayScreenNotTap();
		}


		public void ShowInfoUserPanel()
		{
			PushPanelToTop(ref mInfoUserPopup);
			mInfoUserPopup.Init();
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
		public void ShowPvPSignup(Action OnNext)
		{
			PushPanelToTop(ref mPvPSignup);
			mPvPSignup.OnNext = OnNext;
		}
		public void ShowSettingPanel()
		{
			PushPanelToTop(ref mSettingPanel);
			SettingPanel.loadingNewLanguage = false;
			Config.LogEvent(TrackingConstants.CLICK_USERSETTING);
		}

		public void ShowStorePanel()
		{
			PushPanelToTop(ref mStorePanel);
			mStorePanel.Init();
			Config.LogEvent(TrackingConstants.CLICK_STORE);
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_Store);
		}
		public void ShowInappPopup(Action OnDone)
		{
			PushPanelToTop(ref mInappPopup);
			mInappPopup.SetAction(OnDone);
			mInappPopup.Init();
			mInappPopup.LoadViewPack();
			//    Config.LogEvent(TrackingConstants.CLICK_STORE);
		}
		public void ShowBuySpecialPackPanel()
		{
			PushPanelToTop(ref mStorePanel);
			mStorePanel.Init();
			StartCoroutine(IEDelay(mStorePanel.ShowSpecialPacks));
			Config.LogEvent(TrackingConstants.CLICK_STORE);
		}

		public void ShowBuyGemPanel()
		{
			PushPanelToTop(ref mStorePanel);
			mStorePanel.Init();
			StartCoroutine(IEDelay(mStorePanel.ShowGemPacks));
			Config.LogEvent(TrackingConstants.CLICK_STORE);
		}

		public void ShowBuyPremiumPackPanel()
		{
			PushPanelToTop(ref mStorePanel);
			mStorePanel.Init();
			StartCoroutine(IEDelay(mStorePanel.ShowPremiumPacks));
			Config.LogEvent(TrackingConstants.CLICK_STORE);
		}

		public void ShowBuyCoinPanel()
		{
			PushPanelToTop(ref mBuyCoinPanel);
			mBuyCoinPanel.Init();
		}

		public void ShowBuyTicketPanel()
		{
			PushPanelToTop(ref mBuyTicketPanel);
			mBuyTicketPanel.Init();
		}

		public void ShowWheelsPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_WHEELS)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK)+" " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_WHEELS, true));
				return;
			}
			PushPanelToTop(ref mWheelsPanel);
			Config.LogEvent(TrackingConstants.CLICK_WHEEL);
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_LuckyWheel);
		}

		public void ShowDailyQuestPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_QUEST)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_QUEST, true));
				return;
			}
			PushPanelToTop(ref mDailyQuestPanel);
			mDailyQuestPanel.Init();
			StartCoroutine(IEDelay(mDailyQuestPanel.ShowDailyQuest));
			Config.LogEvent(TrackingConstants.CLICK_QUEST);
		}

		//public void ShowAchievementPanel()
		//{
		//    if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_ACHIEVEMENT)
		//    {
		//        ShowWarningPopup("Unlock " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_ACHIEVEMENT, true));
		//        return;
		//    }
		//    PushPanelToTop(ref mDailyQuestPanel);
		//    mDailyQuestPanel.Init();
		//    StartCoroutine(IEDelay(mDailyQuestPanel.ShowAchievement));
		//    Config.LogEvent(TrackingConstants.CLICK_ACHIEVEMENT);
		//}
		public void ShowPremiumPassPanel()
		{
			PushPanelToTop(ref mPremiumPassPanel);
			mPremiumPassPanel.Init();
			//    StartCoroutine(IEDelay(mPremiumPassPanel.ShowAchievement));
		}
		public void ShowEventAllPanel()
		{
			PushPanelToTop(ref mEventAllPanel);
			mEventAllPanel.Init();
			mEventAllPanel.Show();
		}

		//public void ShowSevenDaysBonusPanel()
		//{
		//    if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_7_DAYS_BONUS)
		//    {
		//        ShowWarningPopup("Unlock " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_7_DAYS_BONUS, true));
		//        return;
		//    }
		//    PushPanelToTop(ref mSevenDaysBonusPanel);
		//    mSevenDaysBonusPanel.Show();
		//}

		//public void ShowDailyLoginPanel()
		//{
		//    if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_DAILY_LOGIN)
		//    {
		//        ShowWarningPopup("Unlock " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_DAILY_LOGIN, true));
		//        return;
		//    }
		//    PushPanelToTop(ref mDailyLoginPanel);
		//    mDailyLoginPanel.Init();
		//    Config.LogEvent(TrackingConstants.CLICK_DAILY_LOGIN);
		//}

		//public void ShowDailyGiftPanel()
		//{
		//    // if (GameData.Instance.UserGroup.Level < Constants.UNLOCK_DAILY_BONUS)
		//    // {
		//    //     ShowWarningPopup("Unlock at level " + Constants.UNLOCK_DAILY_BONUS);
		//    //     return;
		//    // }
		//    PushPanelToTop(ref mDailyGiftPanel);
		//    mDailyGiftPanel.Init();
		//    Config.LogEvent(TrackingConstants.CLICK_DAILY_GIFT);
		//}

		public void ShowEditTeamInMainMenu(HeroData hero)
		{
			// mMainMenuPanel.ShowEditTeam(hero);
		}

		public void ShowMissionDetailPanel()
		{
			GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.CLICK_MISSION);
			PushPanelToTop(ref mMissionDetailPanel);
			mMissionDetailPanel.Init();
		}

		public void ShowHeroPanel()
		{
			var userGroupLevel = GameData.Instance.LevelUnlockContent;
			var min = Mathf.Min(Constants.UNLOCK_UPGRADE_HERO, Constants.UNLOCK_GEAR_EQUIP);
			if (userGroupLevel < min)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(min, true));
				return;
			}

			PushPanelToTop(ref mHeroPanel);
			mHeroPanel.Init();
			Config.LogEvent(TrackingConstants.CLICK_HEROES);
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_Hero);
		}

		public void ShowListGearsPanel(List<GearData> gearDatas, UnityAction<GearData> choiceAction)
		{
			PushPanelToTop(ref mListGearsPanel);
			mListGearsPanel.Init(gearDatas, choiceAction);
		}

		public void ShowBasePanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_BASE)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_BASE, true));
				return;
			}

			PushPanelToTop(ref mBasePanel);
			mBasePanel.Init();
			//if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.USE_BASE_HOME))
			//{
			//	EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.USE_BASE_HOME));
			//}
			Config.LogEvent(TrackingConstants.CLICK_BASE);
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_Base);
		}

		public void ShowInventoryPanel()
		{
			PushPanelToTop(ref mInventoryPanel);
			mInventoryPanel.Init();
			Config.LogEvent(TrackingConstants.CLICK_INVENTORY);
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_Inventory);
		}

		public void ShowGearDetailPanel(GearData gearData, UnityAction refreshAction, bool showbtnDisassemble = true)
		{
			PushPanelToTop(ref mGearDetailPanel);
			mGearDetailPanel.Init(gearData, refreshAction, showbtnDisassemble);
		}

		public void ShowGearDetailPanel(HeroGearSlot heroGearSlot, UnityAction<HeroGearSlot> unEquipAction, UnityAction<HeroGearSlot> changeAction)
		{
			PushPanelToTop(ref mGearDetailPanel);
			mGearDetailPanel.Init(heroGearSlot, unEquipAction, changeAction);
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


		public void ShowHeroFragmentDetailPanel(HeroFragmentItemData heroFragmentItemData, UnityAction refreshAction)
		{
			PushPanelToTop(ref mHeroFragmentDetailPanel);
			mHeroFragmentDetailPanel.Init(heroFragmentItemData, refreshAction);
		}

		public void ShowSkillDetailPanel(int skillType, Sprite sprite, string skillName, string skillInfo, string cooldown)
		{
			PushPanelToTop(ref mSkillDetailPanel);
			mSkillDetailPanel.Init(skillType, sprite, skillName, skillInfo, cooldown);
		}

		public void ShowFactoryPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_FACTORY)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_FACTORY, true));
				return;
			}
			PushPanelToTop(ref mFactoryPanel);
			mFactoryPanel.Init();
			Config.LogEvent(TrackingConstants.CLICK_FACTORY);
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_WorkShop);
		}

		public void ShowUpgradeGearPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_FACTORY)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_FACTORY, true));
				return;
			}
			PushPanelToTop(ref mFactoryPanel);
			mFactoryPanel.Init();
			StartCoroutine(IEDelay(mFactoryPanel.ShowUpgrade));
			Config.LogEvent(TrackingConstants.CLICK_FACTORY);
		}

		public void ShowDisassembleGearPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_FACTORY)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_FACTORY, true));
				return;
			}
			PushPanelToTop(ref mFactoryPanel);
			mFactoryPanel.Init();
			StartCoroutine(IEDelay(mFactoryPanel.ShowDisassemble));
			Config.LogEvent(TrackingConstants.CLICK_FACTORY);
		}

		public void ShowSummonGatePanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_SUMMON)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_SUMMON, true));
				return;
			}
			PushPanelToTop(ref mSummonGatePanel);
			Config.LogEvent(TrackingConstants.CLICK_SUMMON);
		}

		public void ShowPvPMainPanel()
		{
			if (DongNHEditor.PVP_VERSION < Constants.PVP_VERSION_ONBOARD)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_24));
				//ShowWarningPopup("Update game to new version to play PvP Mode");
				return;
			}

			PushPanelToTop(ref mPvPMainPanel);
			mPvPMainPanel.CallStartInHome();

		}
		public void ShowHeroStarUpPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_STAR_UP_HERO)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_STAR_UP_HERO, true));
				return;
			}
			PushPanelToTop(ref mLaboratoryPanel);
			StartCoroutine(IEDelay(mLaboratoryPanel.ShowStarUp));
			Config.LogEvent(TrackingConstants.CLICK_HERO_LAB);
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_HeroLab);
		}

		public void ShowLaboratoryPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_STAR_UP_HERO)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_STAR_UP_HERO, true));
				return;
			}
			PushPanelToTop(ref mLaboratoryPanel);
			mLaboratoryPanel.Init();
			Config.LogEvent(TrackingConstants.CLICK_HERO_LAB);
		}

		public void ShowDisassembleHeroPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_DISASSEMBLE_HERO)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_DISASSEMBLE_HERO, true));
				return;
			}
			PushPanelToTop(ref mLaboratoryPanel);
			mLaboratoryPanel.Init();
			StartCoroutine(IEDelay(mLaboratoryPanel.ShowDisassemble));
			Config.LogEvent(TrackingConstants.CLICK_HERO_LAB);
		}

		public void ShowEvolutionHeroPanel(HeroData heroData, UnityAction<HeroData> evolutionSuccess)
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_HERO_EVOLUTION)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_HERO_EVOLUTION, true));
				return;
			}
			PushPanelToTop(ref mEvolutionPanel);
			mEvolutionPanel.Init(heroData, evolutionSuccess);
			Config.LogEvent(TrackingConstants.CLICK_EVOLUTION);
		}

		public void ShowFormulaBookPanel(HeroData heroData)
		{
			PushPanelToTop(ref mFormulaBookPanel);
			mFormulaBookPanel.Init(heroData);
			Config.LogEvent(TrackingConstants.CLICK_FORMULA);
		}

		public void ShowPackageDetailPanel(int packId)
		{
			var packageData = GameData.Instance.StoreGroup.GetSpecialPackageData(packId);
			PushPanelToTop(ref mPackageDetailPanel);
			mPackageDetailPanel.Init(packageData);
			Config.LogEvent(TrackingConstants.CLICK_PACKAGE, TrackingConstants.PARAM_SOURCE, packageData.LogName);
		}

		public void ShowFormationPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_FORMATION)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_FORMATION, true));
				return;
			}
			PushPanelToTop(ref mFormationPanel);
			mFormationPanel.Init();
			Config.LogEvent(TrackingConstants.CLICK_FORMATION);
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_Formation);
		}

		public void ShowMapPanel()
		{
			Config.typeModeInGame = Config.TYPE_MODE_NORMAL;
			GameData.Instance.HeroesGroup.SetCurrentFormation(0);

			if (GameUnityData.instance.GameRemoteConfig.Function_ShowMap)
			{
				PushPanelToTop(ref mMapPanel);
			}
			else
			{
				if (CheckCanStartGame())
				{
					GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.CLICK_BATTLE);
					StartGame();
					Config.LogEvent(TrackingConstants.CLICK_BATTLE);
				}
			}
		}
		public void StartGamePvP()
		{
			Config.typeModeInGame = Config.TYPE_MODE_PvP;
			MainPanel.instance.LoadGamePlayScreen();
			Config.LogEvent(TrackingConstants.CLICK_PVP_BATTLE);
		}
		bool CheckCanStartGame()
		{
			var formationData = GameData.Instance.HeroesGroup.GetEquippedHeroes();
			var hasHero = false;
			var count = formationData.Length;
			for (int i = 0; i < count; i++)
			{
				var item = formationData[i];
				if (item != null)
				{
					hasHero = true;
					break;
				}
			}
			if (!hasHero)
			{
				var message = new MessagesPopup.Message()
				{
					yesAction = () =>
					{
						Back();
						// MainPanel.instance.TopPanel.Back();
						MainPanel.instance.ShowFormationPanel();
					},
					noAction = () => { },
					additionalAction = null,
					yesActionLabel = "FORMATION",
					noActionLabel = "BACK",
					allowIgnore = true,
					title = "",
					content = "You must choose a formation with some heroes to fight.",
					popupSize = new Vector2(900, 660),
					contentAignment = TextAlignmentOptions.Center
				};
				MainPanel.instance.ShowMessagePopup(message);
				return false;
			}
			return true;
		}

		void StartGame()
		{
			//Daily Quest và Achievement
			GameData.Instance.DailyQuestsGroup.PlayMission();
			MissionData currentMission = GameData.Instance.MissionsGroup.GetCurrentMissionData();
#if DEVELOPMENT
			//  int indexMap = int.Parse(txtIdMap.text);
			//   int indexMission = int.Parse(txtIdMission.text) + indexMap * 1000;
			//GameData.Instance.MissionsGroup.SetCurrentMission(indexMission);
#endif
			//var currentMissionId = currentMission.Id;
			//// if (currentMissionId <= 4000 || currentMissionId % 5 == 0)
			//{
			//	Config.LogEvent(TrackingConstants.MISSION_PLAY_COUNT, TrackingConstants.PARAM_MISSION, currentMissionId);
			//	AppsFlyerObjectScript.LogLevelAchieved(currentMissionId);
			//}
			Config.typeModeInGame = Config.TYPE_MODE_NORMAL;
			MainPanel.instance.LoadGamePlayScreen();
		}
		public void ShowAFKRewardPanel(List<RewardInfo> rewardInfos)
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_AFK_REWARD)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_AFK_REWARD, true));
				return;
			}
			PushPanelToTop(ref mAFKRewardPanel);
			mAFKRewardPanel.Init(rewardInfos);
			//if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.AUTO_BATTLE_REWARD))
			//{
			//	EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.AUTO_BATTLE_REWARD));
			//}
			Config.LogEvent(TrackingConstants.CLICK_AUTO_BATTLE_REWARD);
		}

		public void ShowFastCollectPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_AFK_REWARD)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_AFK_REWARD, true));
				return;
			}
			PushPanelToTop(ref mFastCollectPanel);
			mFastCollectPanel.Init();
			Config.LogEvent(TrackingConstants.CLICK_FAST);
		}

		public void ShowLevelUpPanel(Action OnShowDone = null)
		{
			PushPanelToTop(ref mLevelUpPanel);
			mLevelUpPanel.SetAction(OnShowDone);
			mLevelUpPanel.Init();
		}

		public void ShowVipPanel()
		{
			//var vipLevel = GameData.Instance.UserGroup.VipLevel;
			//if (vipLevel <= 0)
			//{
			//    ShowWarningPopup("Collect VIP exp to unlock this feature");
			//    return;
			//}
			PushPanelToTop(ref mVipPanel);
			mVipPanel.Init();
		}

		public void ShowDiscoveryPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_DISCOVERY)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_DISCOVERY, true));
				return;
			}
			PushPanelToTop(ref mDiscoveryPanel);
			mDiscoveryPanel.Init();
			Config.LogEvent(TrackingConstants.CLICK_DISCOVERY);
		}

		public void ShowCampaignPanel()
		{
			PushPanelToTop(ref mCampaignPanel);
			mCampaignPanel.Init();
			Config.LogEvent(TrackingConstants.CLICK_CAMPAIGN);
		}

		public void ShowDiscoveryLevelPanel()
		{
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_DISCOVERY)
			{
				ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_DISCOVERY, true));
				return;
			}
			PushPanelToTop(ref mDiscoveryPanel);
			mDiscoveryPanel.Init();
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_Discovery);
			Config.LogEvent(TrackingConstants.CLICK_DISCOVERY);
			if (DiscoveriesGroup.lastDiscoveryData == null)
			{
				return;
			}
			else
			{
				var lastDiscoveryData = DiscoveriesGroup.lastDiscoveryData;
				DiscoveriesGroup.lastDiscoveryData = null;
				StartCoroutine(IEDelay(() => { mDiscoveryPanel.RefreshDiscoveryLevel(lastDiscoveryData); }));
			}
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

		public MessageWithPointer ShowNotificationBoard(RectTransform pTarget, string pMessage, Alignment pAlign, Vector2 pPos, bool pDisplayArrow = true, bool pDisplayMessage = true,bool pDisplaySkip=false,bool pNotUpdateTouch=false)
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
				notUpdatePoint = pNotUpdateTouch
			}) ;
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

		public void ShowLockFeaturePopup()
		{
			ShowMessagePopup(MessagesPopup.LockFeatureMessage);
		}

		public void ShowWarningPopup(string txtMessage)
		{
			var message = new MessagesPopup.Message()
			{
				yesAction = () => { },
				noAction = null,
				additionalAction = null,
				yesActionLabel = "Ok",
				allowIgnore = true,
				title = "MESSAGE",
				content = txtMessage,
				popupSize = new Vector2(960, 570),
				contentAignment = TextAlignmentOptions.Center
			};
			ShowMessagePopup(message);
		}

		public void ShowQuitPopup()
		{
			var message = new MessagesPopup.Message()
			{
				yesAction = null,
				noAction = () =>
				{
					GameExtension.QuitGame();
				},
				additionalAction = null,
				noActionLabel = "QUIT",
				allowIgnore = true,
				title = "QUIT GAME",
				content = "Are you sure you want to quit?",
				popupSize = new Vector2(960, 570),
				contentAignment = TextAlignmentOptions.Center
			};
			ShowMessagePopup(message);
		}

		public void ShowMessagePopup(MessagesPopup.Message pMessage)
		{
			if (mMessagesQueue.Contains(pMessage))
				return;

			mMessagesQueue.Enqueue(pMessage);
			if (mMessagesPopup.IsShowing)
				return;

			ShowMessagePopup();
		}

		public void ShowRewardPopup(RewardInfo pReward, RectTransform pSpawnPosition = null, RewardsPopup.AlignType myAlign = RewardsPopup.AlignType.Horizontal, Action OnShowDone = null)
		{
			ShowRewardsPopup(new List<RewardInfo>() { pReward }, pSpawnPosition, myAlign, OnShowDone);
		}

		public void ShowRewardsPopup(List<RewardInfo> pRewards, RectTransform pSpawnPosition = null, RewardsPopup.AlignType myAlign = RewardsPopup.AlignType.Horizontal, Action OnShowDone = null)
		{
			PushPanelToTop(ref mRewardsPopup);
			mRewardsPopup.SetRewards(pRewards, pSpawnPosition, myAlign, OnShowDone);
		}

		public void ShowRatePanelIfAvailable()
		{
			//  if (GameData.Instance.UserGroup.rateShowCount.Value == 0)
			if (GameData.Instance.GameConfigGroup.ShowedDailyRate)
			{
				GameData.Instance.UserGroup.RateCanOpen = false;
			}
			if (GameData.Instance.UserGroup.RateCanOpen)
			{
				PushPanelToTop(ref mRatePanel);
				GameData.GameConfigGroup.AddCountShowRate();
				GameData.Instance.UserGroup.RateCanOpen = false;
			}
			//GameData.Instance.UserGroup.rateShowCount.Value++;
			//if (GameData.Instance.UserGroup.rateShowCount.Value >= 5)
			//    GameData.Instance.UserGroup.rateShowCount.Value = 0;
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

			ShowMainMenuPanel();
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
			if (TopPanel == mMainMenuPanel)
			{
				ShowQuitPopup();
				return;
			}

			if (TopPanel != null)
				TopPanel.Back();
		}

		protected override void OnAnyChildHide(PanelController pLastTop)
		{
			base.OnAnyChildHide(pLastTop);

			if (TopPanel == null || TopPanel is MainMenuPanel)
				mBtnBackLayer.SetActive(false);
			else
			{
				mBtnBackLayer.SetActive(true);
				mBtnBackLayer.transform.SetSiblingIndex(0);//reset về 0 để tránh tình trạng lùi BackLayer quá MainMenuPanel
				mBtnBackLayer.transform.SetSiblingIndex(TopPanel.transform.GetSiblingIndex() - 1);
			}

			onAnyChildHide.Raise(pLastTop as MyGamesBasePanel);
		}

		protected override void OnAnyChildShow(PanelController pPanel)
		{
			base.OnAnyChildShow(pPanel);

			if (TopPanel == null || TopPanel is MainMenuPanel)
				mBtnBackLayer.SetActive(false);
			else
			{
				mBtnBackLayer.SetActive(true);
				mBtnBackLayer.transform.SetSiblingIndex(0);//reset về 0 để tránh tình trạng lùi BackLayer quá MainMenuPanel
				mBtnBackLayer.transform.SetSiblingIndex(TopPanel.transform.GetSiblingIndex() - 1);
			}

			onAnyChildShow.Raise(TopPanel as MyGamesBasePanel);
		}

		private void ShowMessagePopup()
		{
			PushPanelToTop(ref mMessagesPopup);
			mMessagesPopup.InitMessage(mMessagesQueue.Peek());
			mMessagesPopup.onDidHide += OnMessagesPopupHidden;
		}

		private void OnMessagesPopupHidden()
		{
			mMessagesPopup.onDidHide -= OnMessagesPopupHidden;
			if (mMessagesQueue != null && mMessagesQueue.Count > 0)
				mMessagesQueue.Dequeue();

			if (mMessagesQueue.Count > 0)
				ShowMessagePopup();
		}

		//private void OnShowRewardsEvent(ShowRewardsEvent e)
		//{
		//    ShowLevelUpPanel(e.rewards, null);
		//}

		#endregion
	}
}