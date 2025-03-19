
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Utilities.Service;

namespace FoodZombie.UI
{
    public class MainMenuPanel : MyGamesBasePanel
    {
        [Separator("Center")]
        public Material mainModelMaterial;
        public Material greyModelMaterial;
        public SimpleTMPButton btnLaboratory;
        public RectTransform imgLaboratory;
        public SkeletonGraphic modelLaboratory;
        public GameObject imgLaboratoryLock;
        public GameObject imgLaboratoryNoti;
        public SimpleTMPButton btnFactory;
        public RectTransform imgFactory;
        public GameObject imgFactoryLock;
        public SimpleTMPButton btnSummonGate;
        public RectTransform imgSummonGate;
        public GameObject imgSummonGateNoti;
        public SimpleTMPButton btnStore;
        public GameObject imgStoreNoti;
        public SimpleTMPButton btnWheels;
        public GameObject imgWheelsLock;
        public GameObject imgWheelsNoti;
        public SimpleTMPButton btnDiscovery;
        public SkeletonGraphic modelDiscovery;
        public GameObject imgDiscoveryLock;
        public GameObject imgDiscoveryNoti;
        // public TextMeshProUGUI txtTimeAFK;
        public SimpleTMPButton btnTimeCollect;
        public GameObject imgTimeCollectLock;
        public Image imgTimeCollect;
        public GameObject timeCollectNotif;
        public Sprite[] imgTimeCollects;

        public GameObject imgPvPLock;
        public TextMeshProUGUI txtPvpUnlock;

        // public Image imgShopBuzz;

        [Separator("Top-Left")]
        public SimpleTMPButton btnSetting;
        public Image imgAvatar;
        public TextMeshProUGUI txtLevel;
        public SimpleTMPButton btnInfo;
        public SimpleTMPButton btnVip;
        public GameObject imgVipNoti;
        public TextMeshProUGUI txtVip;
        public Image imgExp;
        public SimpleTMPButton btnDailyGifts;
        public GameObject imgDailyGiftsNoti;
        public SimpleTMPButton btnSpecialPack;
        public SimpleTMPButton btnStarterPack;
        public SimpleTMPButton btnBeginerPack;
        public SimpleTMPButton btnLevelPack;
        public TextMeshProUGUI txtLevelPack;
        public SimpleTMPButton btnFirstTimePurchasePack;
        public GameObject imgFirstTimePurchaseNoti;
        public SimpleTMPButton btnCampaign;
        public TextMeshProUGUI txtCampaignInfo;
        public TextMeshProUGUI txtCampaignCount;

        [Separator("Top-Right")]
        public CurrencyView coinView;
        public CurrencyView gemView;
        public CurrencyView expHeroView;
        public SimpleTMPButton btnQuest;
        public GameObject imgQuestLock;
        public GameObject imgQuestNoti;
        public Button btnEventAll;
        public SimpleTMPButton btnDailyLogin;
        public GameObject imgDailyLoginLock;
        public GameObject imgDailyLoginNoti;
        public SimpleTMPButton btnDailyBonus;
        public GameObject imgDailyBonusLock;
        public GameObject imgDailyBonusNoti;
        public SimpleTMPButton btnTimeBuff;
        public GameObject imgTimeBuffLock;
        public GameObject imgTimeBuffNoti;

        public GameObject imgAllEventNoti;

        [Separator("Bot-Center")]
        public SimpleTMPButton btnFight;
        public SimpleTMPButton btnPvP;
        public Text txtPvPTime;
        public Text btnFight_CurrentLevel;
        public SimpleTMPButton btnBase;
        public GameObject imgBaseNoti;
        public Image imgBasePicture;
        public Text txtBaseUnlock;
        public SimpleTMPButton btnFormation;
        public GameObject imgFormationNoti;
        public SimpleTMPButton btnInventory;
        public GameObject imgInventoryNoti;
        public SimpleTMPButton btnHero;
        public SimpleTMPButton btnMainMenu;
        public GameObject imgHeroNoti;

        private bool initialized;

        private GameData GameData => GameData.Instance;
        private HeroData mHero = null;

        private static int showCount = 0;
        private static bool showSpecial = false;

        private bool showNotiVip = false;

        [Separator("Loading or Tutorial")]
        public GameObject imgLockForLoading;

        private void Awake()
        {
            canShowFirstPopup = true;
        }
        private void Start()
        {
            btnInfo.SetUpEvent(BtnInfoUser_Pressed);
            btnSetting.onClick.AddListener(BtnSetting_Pressed);
            btnVip.onClick.AddListener(BtnVip_Pressed);
            btnStore.onClick.AddListener(BtnStore_Pressed);
            btnFight.onClick.AddListener(BtnFight_Pressed);
            btnPvP.onClick.AddListener(BtnPvP_Pressed);
            // btnFight_CurrentLevel.text = Localization.Get(Localization.ID.STAGE) + " " + GameData.MissionLevelToString(GameData.Instance.MissionsGroup.CurrentMissionId);
            btnFight_CurrentLevel.text = Localization.Get(Localization.ID.STAGE) + " " + GameData.Instance.MissionsGroup.GetCurrentMissionData().GetName();
            btnFormation.onClick.AddListener(BtnFormation_Pressed);
            btnHero.onClick.AddListener(BtnHero_Pressed);
            btnBase.onClick.AddListener(BtnBase_Pressed);
            btnInventory.onClick.AddListener((BtnInventory_Pressed));
            btnLaboratory.onClick.AddListener(BtnLaboratory_Pressed);
            btnFactory.onClick.AddListener(BtnFactory_Pressed);
            btnSummonGate.onClick.AddListener(BtnSummonGate_Pressed);
            btnWheels.onClick.AddListener(BtnWheels_Pressed);
            btnDiscovery.onClick.AddListener(BtnDiscovery_Pressed);
            btnQuest.onClick.AddListener(BtnQuest_Pressed);
            // btnAchievement.onClick.AddListener(BtnAchievement_Pressed);
            btnEventAll.onClick.AddListener(BtnEventAllPanel_Pressed);
            //btnDailyLogin.onClick.AddListener(BtnDailyLogin_Pressed);
            //btnDailyBonus.onClick.AddListener(BtnDailyBonus_Pressed);
            //btnDailyGifts.onClick.AddListener(BtnDailyGifts_Pressed);
            btnSpecialPack.onClick.AddListener(BtnSpecialPack_Pressed);
            btnStarterPack.onClick.AddListener(BtnStarterPack_Pressed);
            btnBeginerPack.onClick.AddListener(BtnBeginerPack_Pressed);
            btnLevelPack.onClick.AddListener(BtnLevelPack_Pressed);
            btnFirstTimePurchasePack.onClick.AddListener(BtnFirstTimePurchasePack_Pressed);
            btnCampaign.onClick.AddListener(BtnCampaign_Pressed);

            btnTimeCollect.onClick.AddListener(BtnTimeCollect_Pressed);
            btnTimeBuff.onClick.AddListener(BtnTimeBuff_Pressed);

            ShowAvatar();
            ShowLevel();
            ShowVip();

            GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.OPEN_GAME);

            MainPanel.instance.onAnyChildHide += OnMainPanelChildHide;
            MainPanel.instance.onAnyChildShow += OnMainPanelChildShow;
            EventDispatcher.AddListener<GamePaymentInitializedEvent>(OnGamePaymentInitialized);
            EventDispatcher.AddListener<UserExpChangeEvent>(OnUserExpChange);
            EventDispatcher.AddListener<UserLevelUpEvent>(OnUserLevelUp);
            EventDispatcher.AddListener<VipLevelUpEvent>(OnVipLevelUp);
            EventDispatcher.AddListener<UIChangeEvent>(OnUIChangeEvent);
            EventDispatcher.AddListener<DailyQuestChangeEvent>(OnDailyQuestChange);
            EventDispatcher.AddListener<AchievementChangeEvent>(OnAchievementChange);
            EventDispatcher.AddListener<DailyLoginChangeEvent>(OnDailyLoginChange);
            EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
            EventDispatcher.AddListener<ChangeGearEvent>(OnChangeGear);
            EventDispatcher.AddListener<FormationChangeEvent>(OnFormationChange);
            EventDispatcher.AddListener<HeroStarUpEvent>(OnHeroStarUp);
            EventDispatcher.AddListener<HeroLevelUpEvent>(OnHeroLevelUp);
            EventDispatcher.AddListener<HeroEvolutionEvent>(OnHeroEvolution);
            EventDispatcher.AddListener<BaseLevelUpEvent>(OnBaseLevelUp);
            EventDispatcher.AddListener<DailyBonusChangeEvent>(OnDailyBonusChange);
            EventDispatcher.AddListener<DailyGiftsChangeEvent>(OnDailyGiftsChange);
            EventDispatcher.AddListener<SpecialPackBoughtEvent>(OnSpecialPackBought);
            EventDispatcher.AddListener<RefreshFlashSaleEvent>(OnRefreshFlashSale);
            EventDispatcher.AddListener<HeroFragmentChangeEvent>(OnHeroFragmentChange);
            EventDispatcher.AddListener<FBChangeEvent>(OnFBChange);

            SoundManager.Instance.PlayMusic(IDs.MUSIC_MAIN_MENU);

            ShowFirstPanel();

            btnStore.SetActive(GamePayment.Instance.Initialized);

            showCount++;

            StartCoroutine(IEUnlockLoading());

            //if (GameUnityData.instance.GameRemoteConfig.Function_PvP_Active)
            //	GameData.Instance.HeroesGroup.SaveEquippedHeros(new List<string>() { }, 1);
        }
        private void Update()
        {
            if (txtPvPTime.gameObject.activeSelf)
            {
                if (GameUnityData.instance.GameRemoteConfig.Function_PvP_Active)
                    txtPvPTime.text = Config.PvpConfig.DeadlineTimeString();
                else
                    txtPvPTime.text = "We'll Be Back Soon";
            }
        }
        private IEnumerator IEUnlockLoading()
        {
            yield return new WaitForSeconds(1f);
            imgLockForLoading.SetActive(false);
        }

        public static bool canShowFirstPopup = true;
        public void ShowFirstPanel()
        {
            //ưu tiên cao nhất là chuyển ngôn ngữ
            if (SettingPanel.loadingNewLanguage)
            {
                MainPanel.instance.ShowSettingPanel();
                return;
            }


            //từ màn hình gameplay
            if (Config.backToHomePanel == SceneName.MapPanel)
            {
                MainPanel.instance.ShowMapPanel();
                Config.backToHomePanel = SceneName.NONE;
                return;
            }
            if (Config.backToHomePanel == SceneName.HeroPanel)
            {
                MainPanel.instance.ShowHeroPanel();
                Config.backToHomePanel = SceneName.NONE;
                return;
            }
            if (Config.backToHomePanel == SceneName.FactoryPanel)
            {
                MainPanel.instance.ShowFactoryPanel();
                Config.backToHomePanel = SceneName.NONE;
                return;
            }
            if (Config.backToHomePanel == SceneName.SummonGatePanel)
            {
                MainPanel.instance.ShowSummonGatePanel();
                Config.backToHomePanel = SceneName.NONE;
                return;
            }
            if (Config.backToHomePanel == SceneName.PvPMainPanel)
            {
                MainPanel.instance.ShowPvPMainPanel();
                Config.backToHomePanel = SceneName.NONE;
                return;
            }

            //Debug.LogError("Check init with " + MainPanel.instance.TopPanel);
            //Fix daily gift auto show on new day

            if (!canShowFirstPopup)
                return;
            bool notif7DayBonus = !GameData.Instance.DailyQuestsGroup.ClaimedToday;
            if (notif7DayBonus && GameData.Instance.MissionsGroup.CurrentMissionId > 1003)
            {
                MainPanel.instance.ShowEventAllPanel();
                return;
            }


            if (InappPopup.CheckConditionToShow() > 0)
            {
                MainPanel.instance.ShowInappPopup(ShowFirstPanel); return;
            }

            //nếu lên level
            if (GameData.Instance.UserGroup.CanShowLevelUp())
            {
                MainPanel.instance.ShowLevelUpPanel(ShowFirstPanel);
                return;
            }


            //đã qua tut và vào điểm danh buổi trưa
            var eatNoon = GameData.Instance.DailyQuestsGroup.EatNoonToday;
            if (showCount <= 0
                //   && TutorialsManager.Instance.IsCompleted(TutorialsGroup.SUMMON_HERO_X10_HOME)
                && !eatNoon)
            {
                if (CheckNoon()) return;
            }

            //đã qua tut và vào điểm danh toi
            var eatDinner = GameData.Instance.DailyQuestsGroup.EatDinnerToday;
            if (showCount <= 0
                //    && TutorialsManager.Instance.IsCompleted(TutorialsGroup.SUMMON_HERO_X10_HOME)
                && !eatDinner)
            {
                if (CheckDinner()) return;
            }



            //

            // if (TutorialsManager.Instance.IsCompleted(TutorialsGroup.SUMMON_HERO_X10_HOME))
            //{
            if (DiscoveriesGroup.lastDiscoveryData != null)
            {
                MainPanel.instance.ShowDiscoveryLevelPanel();
                return;
            }


            //show Special
            if (!showSpecial && LogicAPI.CanShowSpecialPackInMainMenu() && GamePayment.Instance.Initialized)
            {
                var storeGroup = GameData.Instance.StoreGroup;

                //đã từng mua IAP thì hiện lên first Time Purchase Pack, hoặc đợi qua mission 1-10 mới hiện
                //nếu mua first Time Purchase Pack rồi thì ko hiện
                var buyCount = storeGroup.BuyCount;
                var firstTimePurchasePack = storeGroup.GetSpecialPackageData(1);
                if (buyCount > 0 && firstTimePurchasePack.CanBuy)
                {
                    MainPanel.instance.ShowPackageDetailPanel(1);
                    showSpecial = true;
                    return;
                }

                var userLevel = GameData.Instance.UserGroup.Level;
                if (userLevel >= 10)
                {
                    int packId = 6;
                    if (userLevel < 20) packId = 6;
                    else if (userLevel < 30) packId = 7;
                    else if (userLevel < 40) packId = 8;
                    else if (userLevel < 50) packId = 9;
                    else if (userLevel < 60) packId = 10;
                    else if (userLevel < 70) packId = 11;
                    else if (userLevel < 80) packId = 12;
                    else if (userLevel < 90) packId = 13;
                    else if (userLevel < 100) packId = 14;
                    else packId = 15;

                    var pack = storeGroup.GetSpecialPackageData(packId);
                    btnLevelPack.SetActive(LogicAPI.CanShowSpecialPackInMainMenu() && pack.CanBuy);
                    if (pack.CanBuy)
                    {
                        MainPanel.instance.ShowPackageDetailPanel(packId);
                        showSpecial = true;
                        return;
                    }
                }

                var starterPack = storeGroup.GetSpecialPackageData(2);
                if (starterPack.CanBuy)
                {
                    MainPanel.instance.ShowPackageDetailPanel(2);
                    showSpecial = true;
                    return;
                }
                var beginerPack = storeGroup.GetSpecialPackageData(3);
                if (beginerPack.CanBuy)
                {
                    MainPanel.instance.ShowPackageDetailPanel(3);
                    showSpecial = true;
                    return;
                }
            }
            //}


            //show Rate
            if (LogicAPI.CanShowRatePanel())
            {
                MainPanel.instance.ShowRatePanelIfAvailable();
                return;
            }





            // if (AdsHelper.Instance.CanShowInterstitial())
            // {
            //     AdsHelper.__ShowInterstitialAd();
            // }
            //xong hết tutorial mới gọi mấy popup linh tinh
        }

        // private void Update()
        // {
        //     txtTimeAFK.text = MissionsGroup.GetTimeAFK();
        //     btnTimeBuff.SetActive(MissionsGroup.isTimeAFKRunning);
        // }

        private void OnDestroy()
        {
            try
            {
                MainPanel.instance.onAnyChildHide -= OnMainPanelChildHide;
            }
            catch (Exception ex)
            {

            }
            try
            {
                MainPanel.instance.onAnyChildShow -= OnMainPanelChildShow;
            }
            catch (Exception ex)
            {
            }


            EventDispatcher.RemoveListener<GamePaymentInitializedEvent>(OnGamePaymentInitialized);
            EventDispatcher.RemoveListener<UserExpChangeEvent>(OnUserExpChange);
            EventDispatcher.RemoveListener<UserLevelUpEvent>(OnUserLevelUp);
            EventDispatcher.RemoveListener<VipLevelUpEvent>(OnVipLevelUp);
            EventDispatcher.RemoveListener<UIChangeEvent>(OnUIChangeEvent);
            EventDispatcher.RemoveListener<DailyQuestChangeEvent>(OnDailyQuestChange);
            EventDispatcher.RemoveListener<AchievementChangeEvent>(OnAchievementChange);
            EventDispatcher.RemoveListener<DailyLoginChangeEvent>(OnDailyLoginChange);
            EventDispatcher.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
            EventDispatcher.RemoveListener<ChangeGearEvent>(OnChangeGear);
            EventDispatcher.RemoveListener<FormationChangeEvent>(OnFormationChange);
            EventDispatcher.RemoveListener<HeroStarUpEvent>(OnHeroStarUp);
            EventDispatcher.RemoveListener<HeroLevelUpEvent>(OnHeroLevelUp);
            EventDispatcher.RemoveListener<HeroEvolutionEvent>(OnHeroEvolution);
            EventDispatcher.RemoveListener<BaseLevelUpEvent>(OnBaseLevelUp);
            EventDispatcher.RemoveListener<DailyBonusChangeEvent>(OnDailyBonusChange);
            EventDispatcher.RemoveListener<DailyGiftsChangeEvent>(OnDailyGiftsChange);
            EventDispatcher.RemoveListener<SpecialPackBoughtEvent>(OnSpecialPackBought);
            EventDispatcher.RemoveListener<RefreshFlashSaleEvent>(OnRefreshFlashSale);
            EventDispatcher.RemoveListener<HeroFragmentChangeEvent>(OnHeroFragmentChange);
            EventDispatcher.RemoveListener<FBChangeEvent>(OnFBChange);
        }

        internal override void Init()
        {
            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);

            // imgShopBuzz.SetActive(false);

            InitBtns();

            // //phải win một ván mới cho afk reward
            // if(GameData.Instance.MissionsGroup.IsFirstMission() || GameData.Instance.UserGroup.Level < Constants.UNLOCK_AFK_REWARD)
            // {
            //     // txtTimeAFK.SetActive(false);
            //     btnTimeCollect.SetActive(false);
            // }
            // else
            // {
            //     // txtTimeAFK.SetActive(true);
            //     btnTimeCollect.SetActive(true);
            // }

            initialized = true;
        }

        private void InitBtns()
        {
            //left
            ShowSpecialBtns();
            ShowDailyGiftsBtn();
            ShowBtnCampaign();

            //right
            ShowDailyQuestBtn();
            ShowDailyLoginBtn();
            ShowDailyBonusBtn();

            //center
            ShowWheelsBtn();
            ShowDiscoveryBtn();
            ShowFactoryBtn();
            ShowLaboratoryBtn();
            ShowSummonGateBtn();
            CheckImgTimeCollect();
            ShowTimeBuffBtn();
            ShowPvpBtn();

            //footer
            ShowBaseBtn();
            ShowFormationBtn();
            ShowInventoryBtn();
            ShowHeroBtn();
        }

        private void BtnStore_Pressed()
        {
            MainPanel.instance.ShowStorePanel();
        }

        private void BtnFight_Pressed()
        {
            GameData.Instance.HeroesGroup.SetCurrentFormation(0);
            MainPanel.instance.ShowMapPanel();
        }
        private void BtnPvP_Pressed()
        {
            Config.LogEvent(TrackingConstants.CLICK_PVP);
            if (UserGroup.UserData != null)
            {
                if (GameRESTController.Instance.TOKEN.Trim().Length <= 0)
                {
                    //have no token,login to get
                    MainPanel.instance.ShowWaitingPanel(true, "login");
                    GameRESTController.Instance.APIUser_login(UserGroup.UserData, (data) =>
                    {
                        UserModel user = new UserModel();
                        HttpResultData result = data.Data[0];
                        user.ID = result._id;
                        user.UserName = result.UserName;
                        user.Formation = result.Formation;
                        user.CP = result.CP;
                        user.IDFacebook = result.IDFacebook;
                        user.IDApple = result.IDApple;
                        user.ScorePvPRank = result.ScorePvPRank;
                        user.IDAvatar = result.IDAvatar;
                        GameRESTController.Instance.TOKEN = result.Token;
                        GameRESTController.Instance.REFESH_TOKEN = result.refreshToken;
                        user.SaveToGameSave();

                        MainPanel.instance.ShowWaitingPanel(false);
                        MainPanel.instance.ShowPvPMainPanel();
                    }, (error) =>
                    {
                        MainPanel.instance.ShowWaitingPanel(false);
                        MainPanel.instance.ShowWarningPopup(error);
                    });
                }
                else
                    MainPanel.instance.ShowPvPMainPanel();
                //MainPanel.instance.StartGamePvP();
            }
            else
            {
                if (FBManager.Instance.IsLoggedIn)
                {
                    UserModel user = new UserModel() { IDFacebook = FBManager.Instance.FBID, UserName = FBManager.Instance.FBName };
                    MainPanel.instance.ShowWaitingPanel(true, "signin");
                    GameRESTController.Instance.APIUser_signup(user, (o) =>
                    {
                        MainPanel.instance.ShowWaitingPanel(false, "signup");
                        HttpResultData result = o.Data[0];
                        UserModel userNew = new UserModel();
                        userNew.ID = result._id;
                        userNew.UserName = result.UserName;
                        userNew.IDFacebook = result.IDFacebook;
                        userNew.IDApple = result.IDApple;
                        userNew.ScorePvPRank = result.ScorePvPRank;
                        userNew.IDAvatar = result.IDAvatar;
                        GameRESTController.Instance.TOKEN = result.Token;
                        GameRESTController.Instance.REFESH_TOKEN = result.refreshToken;
                        UserGroup.UserData = user;
                        UserGroup.UserData.SaveToGameSave();
                        MainPanel.instance.ShowPvPMainPanel();
                    }, (err) =>
                    {
                        MainPanel.instance.ShowWaitingPanel(false, "signup");
                        MainPanel.instance.ShowWarningPopup(err.ToString());
                    });
                }
                else
                {
                    MainPanel.instance.ShowPvPSignup(() =>
                    {
                        MainPanel.instance.ShowPvPMainPanel();
                        //MainPanel.instance.StartGamePvP(); 
                    });
                }
            }
        }
        private void BtnHero_Pressed()
        {
            MainPanel.instance.ShowHeroPanel();
        }

        private void BtnBase_Pressed()
        {
            MainPanel.instance.ShowBasePanel();
        }

        private void BtnInventory_Pressed()
        {
            MainPanel.instance.ShowInventoryPanel();
        }

        private void BtnFormation_Pressed()
        {
            MainPanel.instance.ShowFormationPanel();
        }

        private void BtnLaboratory_Pressed()
        {
            MainPanel.instance.ShowLaboratoryPanel();
        }

        private void BtnFactory_Pressed()
        {
            MainPanel.instance.ShowFactoryPanel();
        }

        private void BtnSummonGate_Pressed()
        {
            MainPanel.instance.ShowSummonGatePanel();
        }

        private void BtnWheels_Pressed()
        {
            MainPanel.instance.ShowWheelsPanel();
        }

        private void BtnDiscovery_Pressed()
        {
            MainPanel.instance.ShowDiscoveryPanel();
        }

        private void BtnInfoUser_Pressed()
        {
            MainPanel.instance.ShowInfoUserPanel();
        }

        private void BtnSetting_Pressed()
        {
            MainPanel.instance.ShowSettingPanel();
        }

        private void BtnVip_Pressed()
        {
            MainPanel.instance.ShowVipPanel();
        }

        private void BtnQuest_Pressed()
        {
            MainPanel.instance.ShowDailyQuestPanel();
        }

        //private void BtnAchievement_Pressed()
        //{
        //	MainPanel.instance.ShowAchievementPanel();
        //}

        private void BtnEventAllPanel_Pressed()
        {
            MainPanel.instance.ShowEventAllPanel();
        }

        //private void BtnDailyLogin_Pressed()
        //{
        //    MainPanel.instance.ShowDailyLoginPanel();
        //}

        //private void BtnDailyBonus_Pressed()
        //{
        //    MainPanel.instance.ShowSevenDaysBonusPanel();
        //}

        //private void BtnDailyGifts_Pressed()
        //{
        //    MainPanel.instance.ShowDailyGiftPanel();
        //}
        private void BtnSpecialPack_Pressed()
        {
            MainPanel.instance.ShowBuySpecialPackPanel();
        }
        private void BtnStarterPack_Pressed()
        {
            MainPanel.instance.ShowPackageDetailPanel(2);
        }

        private void BtnBeginerPack_Pressed()
        {
            MainPanel.instance.ShowPackageDetailPanel(3);
        }

        private void BtnLevelPack_Pressed()
        {
            // 6	SPECIAL_PACK	mergedefend.lv10.pack	Lv 10 special pack
            // 7	SPECIAL_PACK	mergedefend.lv20.pack	Lv 20 special pack
            // 8	SPECIAL_PACK	mergedefend.lv30.pack	Lv 30 special pack
            // 9	SPECIAL_PACK	mergedefend.lv40.pack	Lv 40 special pack
            // 10	SPECIAL_PACK	mergedefend.lv50.pack	Lv 50 special pack
            // 11	SPECIAL_PACK	mergedefend.lv60.pack	Lv 60 special pack
            // 12	SPECIAL_PACK	mergedefend.lv70.pack	Lv 70 special pack
            // 13	SPECIAL_PACK	mergedefend.lv80.pack	Lv 80 special pack
            // 14	SPECIAL_PACK	mergedefend.lv90.pack	Lv 90 special pack
            // 15	SPECIAL_PACK	mergedefend.lv100.pack	Lv 100 special pack

            int packId = 6;
            var userLevel = GameData.Instance.UserGroup.Level;
            if (userLevel < 20) packId = 6;
            else if (userLevel < 30) packId = 7;
            else if (userLevel < 40) packId = 8;
            else if (userLevel < 50) packId = 9;
            else if (userLevel < 60) packId = 10;
            else if (userLevel < 70) packId = 11;
            else if (userLevel < 80) packId = 12;
            else if (userLevel < 90) packId = 13;
            else if (userLevel < 100) packId = 14;
            else packId = 15;
            MainPanel.instance.ShowPackageDetailPanel(packId);
        }

        private void BtnCampaign_Pressed()
        {
            MainPanel.instance.ShowCampaignPanel();
        }

        private void BtnFirstTimePurchasePack_Pressed()
        {
            MainPanel.instance.ShowPackageDetailPanel(1);
        }

        private void BtnTimeCollect_Pressed()
        {
            MainPanel.instance.ShowAFKRewardPanel(GameData.Instance.MissionsGroup.GetAFKRewards());
        }

        private void BtnTimeBuff_Pressed()
        {
            MainPanel.instance.ShowFastCollectPanel();
        }

        private void OnMainPanelChildShow(MyGamesBasePanel pPanel)
        {
            if (pPanel is StorePanel)
            {
                imgStoreNoti.SetActive(false);
            }
            //else 
            // if (pPanel == MainPanel.instance.ZombiaryPanel)
            // {
            //     imgCollectionBuzz.SetActive(false);
            // }
            // else if (pPanel == MainPanel.instance.SafePopup)
            // {
            //     imgSafeBuzz.SetActive(false);
            // }
            //else if (pPanel == MainPanel.instance.TeamPanel)
            //{
            //    imgTroopBuzz.SetActive(false);
            //}
        }

        private void OnMainPanelChildHide(MyGamesBasePanel pPanel)
        {
            ShowBtnCampaign();

            if (pPanel is WheelsPanel)
            {
                ShowWheelsBtn();
            }
            else if (pPanel is FastCollectPanel)
            {
                ShowTimeBuffBtn();
                // CheckLevelUp();
            }
            else if (pPanel is AFKRewardPanel)
            {
                CheckImgTimeCollect();
                // CheckLevelUp();
            }
            else if (pPanel is DiscoveryPanel)
            {
                ShowDiscoveryBtn();
            }
            else if (pPanel is RewardsPopup)
            {
                CheckLevelUp();
            }
            else if (pPanel is VipPanel)
            {
                showNotiVip = false;
                ShowVip();
            }
        }

        private void OnGamePaymentInitialized(GamePaymentInitializedEvent e)
        {
            btnStore.SetActive(GamePayment.Instance.Initialized);
        }

        private void ShowAvatar()
        {
            UserModel user = UserGroup.UserData;
            if (user != null)
            {
                imgAvatar.sprite = user.GetAvatar();
            }
            else
            if (FBManager.Instance.FBAvatar != null)
                imgAvatar.sprite = FBManager.Instance.FBAvatar;
        }

        private void ShowLevel()
        {
            txtLevel.text = "" + GameData.Instance.UserGroup.Level;
            imgExp.fillAmount = GameData.Instance.UserGroup.GetPercentExp();
        }

        private void ShowVip()
        {
            var vipLevel = GameData.Instance.UserGroup.VipLevel;
            if (vipLevel > 0)
            {
                txtVip.text = "VIP " + GameData.Instance.UserGroup.VipLevel;
            }
            else
            {
                txtVip.text = "VIP 0";
            }

            if (showNotiVip || (vipLevel > 0 && !GameData.Instance.UserGroup.ClaimedVipToDay))
            {
                imgVipNoti.SetActive(true);
            }
            else
            {
                imgVipNoti.SetActive(false);
            }
        }

        private void OnUserLevelUp(UserLevelUpEvent e)
        {
            ShowLevel();
            InitBtns();
        }

        private void OnVipLevelUp(VipLevelUpEvent e)
        {
            showNotiVip = true;
            ShowVip();
        }

        private void OnUserExpChange(UserExpChangeEvent e)
        {
            ShowLevel();
        }

        private void CheckImgTimeCollect()
        {
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_AFK_REWARD;
            if (unlocked)
            {
                btnTimeCollect.labelTMP.text = "AFK Rewards";
                imgTimeCollectLock.transform.parent.SetActive(false);

                //if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.AUTO_BATTLE_REWARD))
                //{
                //	EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.AUTO_BATTLE_REWARD));
                //}
            }
            else
            {
                btnTimeCollect.labelTMP.text = GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_AFK_REWARD);
                imgTimeCollectLock.transform.parent.SetActive(true);
            }
            btnTimeCollect.SetEnable(unlocked);

            var timeAfk = GameData.Instance.MissionsGroup.GetNextTimeAFK();
            if (timeAfk <= 2 * 60 * 60)
            {
                imgTimeCollect.sprite = imgTimeCollects[0];
                timeCollectNotif.SetActive(false);
            }
            else if (timeAfk <= 5 * 60 * 60)
            {
                imgTimeCollect.sprite = imgTimeCollects[1];
                timeCollectNotif.SetActive(true);
            }
            else
            {
                imgTimeCollect.sprite = imgTimeCollects[2];
                timeCollectNotif.SetActive(true);
            }
            if (unlocked && !TutorialsManager.Instance.IsCompleted(TutorialsGroup.AUTO_BATTLE_REWARD))
            {
                timeCollectNotif.SetActive(true);
            }
        }

        private void CheckLevelUp()
        {
            if (GameData.Instance.UserGroup.CanShowLevelUp()
                && !TutorialsManager.Instance.IsShowingFixedTut)
            {
                MainPanel.instance.ShowLevelUpPanel();
            }
        }

        //noti
        private void OnUIChangeEvent(UIChangeEvent e)
        {
            btnFight_CurrentLevel.text = Localization.Get(Localization.ID.STAGE) + " " + GameData.Instance.MissionsGroup.GetCurrentMissionData().GetName();
            CheckNotif();
            if (MainPanel.instance.MapPanel.isActiveAndEnabled)
                MainPanel.instance.MapPanel.Init();
            if (MainPanel.instance.SummonGatePanel.isActiveAndEnabled)
            {
                MainPanel.instance.SummonGatePanel.Init();
                MainPanel.instance.SummonGatePanel.Refresh();
            }
            if (MainPanel.instance.HeroPanel.isActiveAndEnabled)
                MainPanel.instance.HeroPanel.Init();
        }
        private void OnDailyQuestChange(DailyQuestChangeEvent e)
        {
            ShowDailyQuestBtn();
        }

        private void OnAchievementChange(AchievementChangeEvent e)
        {
            ShowDailyQuestBtn();
        }

        private void ShowDailyQuestBtn()
        {
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_QUEST;
            if (unlocked)
            {
                btnQuest.labelTMP.text = "QUEST";
                imgQuestLock.SetActive(false);
            }
            else
            {
                btnQuest.labelTMP.text = GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_QUEST);
                imgQuestLock.SetActive(true);
            }
            btnQuest.SetEnable(unlocked);
            imgQuestNoti.SetActive(unlocked && (GameData.Instance.DailyQuestsGroup.CheckDailyQuestNoti()
                                   || GameData.Instance.AchievementsGroup.CheckNoti()));
        }

        private void OnDailyLoginChange(DailyLoginChangeEvent e)
        {
            ShowDailyLoginBtn();
            CheckNotif();
        }

        private void ShowDailyLoginBtn()
        {
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_DAILY_LOGIN;
            if (unlocked)
            {
                btnDailyLogin.labelTMP.text = "DAILY LOGIN";
                imgDailyLoginLock.SetActive(false);
            }
            else
            {
                btnDailyLogin.labelTMP.text = GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_DAILY_LOGIN);
                imgDailyLoginLock.SetActive(true);
            }
            // btnDailyLogin.SetEnable(unlocked);
            imgDailyLoginNoti.SetActive(unlocked && GameData.Instance.DailyQuestsGroup.CheckDailyLoginNoti());
        }



        private void OnCurrencyChanged(CurrencyChangedEvent e)
        {
            if (e.id == IDs.CURRENCY_COIN)
            {
                ShowHeroBtn();
                ShowBaseBtn();
            }
            else if (e.id == IDs.CURRENCY_EXP_HERO)
            {
                ShowHeroBtn();
            }
            // else if (e.id == IDs.CURRENCY_BLUE_CHIP
            //          || e.id == IDs.CURRENCY_GOLDEN_CHIP)
            // {
            //     ShowWheelsBtn();
            // }
            else if (e.id == IDs.CURRENCY_POWER_FRAGMENT
                     || e.id == IDs.CURRENCY_POWER_CRYSTAL
                     || e.id == IDs.CURRENCY_DEVINE_CRYSTAL)
            {
                ShowSummonGateBtn();
            }
            else if (e.id == IDs.CURRENCY_BLUE_HERO_FRAGMENT
                     || e.id == IDs.CURRENCY_EPIC_HERO_FRAGMENT)
            {
                ShowInventoryBtn();
            }
            else if (e.id == IDs.CURRENCY_DUST_LAVA
                     || e.id == IDs.CURRENCY_DUST_METALIC
                     || e.id == IDs.CURRENCY_DUST_ELECTRIC
                     || e.id == IDs.CURRENCY_DUST_NITROGEN)
            {
                ShowLaboratoryBtn();
            }
            //Check gói premium
            ShowDailyLoginBtn();
        }

        private void ShowHeroBtn()
        {
            imgHeroNoti.SetActive(GameData.Instance.HeroesGroup.CheckHeroUpgradeNoti());
        }

        private void OnChangeGear(ChangeGearEvent e)
        {
            ShowHeroBtn();
        }

        private void OnFormationChange(FormationChangeEvent e)
        {
            ShowFormationBtn();
            ShowHeroBtn();
            ShowLaboratoryBtn();
        }

        private void ShowFormationBtn()
        {
            imgFormationNoti.SetActive(GameData.Instance.HeroesGroup.CheckFormationNoti());
        }

        private void OnHeroStarUp(HeroStarUpEvent e)
        {
            ShowLaboratoryBtn();
        }

        private void OnHeroLevelUp(HeroLevelUpEvent e)
        {
            ShowHeroBtn();
        }

        private void OnHeroEvolution(HeroEvolutionEvent e)
        {
            ShowHeroBtn();
        }

        private void OnDailyBonusChange(DailyBonusChangeEvent e)
        {
            ShowDailyBonusBtn();
            CheckNotif();
        }

        private void ShowDailyBonusBtn()
        {
            var is7Day = GameData.Instance.DailyQuestsGroup.DayCount <= 7;
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_7_DAYS_BONUS;
            //btnDailyBonus.SetActive(is7Day);
            btnDailyBonus.SetActive(false);
            if (unlocked)
            {
                btnDailyBonus.labelTMP.text = "7 DAY BONUS";
                imgDailyBonusLock.SetActive(false);
            }
            else
            {
                btnDailyBonus.labelTMP.text = GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_7_DAYS_BONUS);
                imgDailyBonusLock.SetActive(true);
            }
            //    btnDailyBonus.SetEnable(unlocked);
            imgDailyBonusNoti.SetActive(unlocked && !GameData.Instance.DailyQuestsGroup.ClaimedToday);
        }

        private void OnDailyGiftsChange(DailyGiftsChangeEvent e)
        {
            ShowDailyGiftsBtn();
            CheckNotif();
        }

        private void ShowDailyGiftsBtn()
        {
            imgDailyGiftsNoti.SetActive(!GameData.Instance.DailyQuestsGroup.MaxDailyGiftCountInDay);
        }

        private void OnSpecialPackBought(SpecialPackBoughtEvent e)
        {
            ShowSpecialBtns();
        }

        private void ShowSpecialBtns()
        {
            var storeGroup = GameData.Instance.StoreGroup;
            var starterPack = storeGroup.GetSpecialPackageData(2);
            btnStarterPack.SetActive(LogicAPI.CanShowSpecialPackInMainMenu() && starterPack.CanBuy);
            var beginerPack = storeGroup.GetSpecialPackageData(3);
            btnBeginerPack.SetActive(LogicAPI.CanShowSpecialPackInMainMenu() && beginerPack.CanBuy);

            // 6	SPECIAL_PACK	mergedefend.lv10.pack	Lv 10 special pack
            // 7	SPECIAL_PACK	mergedefend.lv20.pack	Lv 20 special pack
            // 8	SPECIAL_PACK	mergedefend.lv30.pack	Lv 30 special pack
            // 9	SPECIAL_PACK	mergedefend.lv40.pack	Lv 40 special pack
            // 10	SPECIAL_PACK	mergedefend.lv50.pack	Lv 50 special pack
            // 11	SPECIAL_PACK	mergedefend.lv60.pack	Lv 60 special pack
            // 12	SPECIAL_PACK	mergedefend.lv70.pack	Lv 70 special pack
            // 13	SPECIAL_PACK	mergedefend.lv80.pack	Lv 80 special pack
            // 14	SPECIAL_PACK	mergedefend.lv90.pack	Lv 90 special pack
            // 15	SPECIAL_PACK	mergedefend.lv100.pack	Lv 100 special pack

            var userLevel = GameData.Instance.UserGroup.Level;
            if (userLevel >= 10)
            {
                int packId = 6;
                int packLevel = 10;
                if (userLevel < 20) { packId = 6; packLevel = 10; }
                else if (userLevel < 30) { packId = 7; packLevel = 20; }
                else if (userLevel < 40) { packId = 8; packLevel = 30; }
                else if (userLevel < 50) { packId = 9; packLevel = 40; }
                else if (userLevel < 60) { packId = 10; packLevel = 50; }
                else if (userLevel < 70) { packId = 11; packLevel = 60; }
                else if (userLevel < 80) { packId = 12; packLevel = 70; }
                else if (userLevel < 90) { packId = 13; packLevel = 80; }
                else if (userLevel < 100) { packId = 14; packLevel = 90; }
                else { packId = 15; packLevel = 100; }

                var pack = storeGroup.GetSpecialPackageData(packId);
                btnLevelPack.SetActive(LogicAPI.CanShowSpecialPackInMainMenu() && pack.CanBuy);
                btnLevelPack.labelTMP.text = "Lv." + packLevel + " PACK";
                txtLevelPack.text = "" + packLevel;
            }
            else
            {
                btnLevelPack.SetActive(false);
            }

            //đã từng mua IAP thì hiện lên first Time Purchase Pack, hoặc đợi qua mission 1-10 mới hiện
            //nếu mua first Time Purchase Pack rồi thì ko hiện
            var buyCount = storeGroup.BuyCount;
            var firstTimePurchasePack = storeGroup.GetSpecialPackageData(1);
            btnFirstTimePurchasePack.SetActive((LogicAPI.CanShowSpecialPackInMainMenu() || buyCount > 0) && firstTimePurchasePack.CanBuy);
            imgFirstTimePurchaseNoti.SetActive(buyCount > 0);
        }

        private void OnRefreshFlashSale(RefreshFlashSaleEvent e)
        {
            ShowRefreshFlashSaleNoti();
        }

        private void ShowRefreshFlashSaleNoti()
        {
            imgStoreNoti.SetActive(true);
        }

        private void ShowWheelsBtn()
        {
            // var currenciesGroup = GameData.Instance.CurrenciesGroup;
            // var blueChip = currenciesGroup.GetValue(IDs.CURRENCY_BLUE_CHIP);
            // var goldenChip = currenciesGroup.GetValue(IDs.CURRENCY_GOLDEN_CHIP);

            // imgWheelsNoti.SetActive((blueChip > 0 && userLevel >= Constants.UNLOCK_WHEELS)
            //                         || (goldenChip > 0 && userLevel >= Constants.UNLOCK_WHEELS_ROYALE));
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_WHEELS;
            if (unlocked)
            {
                btnWheels.labelTMP.text = "Lucky Wheel";
                imgWheelsLock.SetActive(false);
            }
            else
            {
                btnWheels.labelTMP.text = GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_WHEELS);
                imgWheelsLock.SetActive(true);
            }
            btnWheels.SetEnable(unlocked);
            var wheelData = GameData.Instance.WheelData;
            imgWheelsNoti.SetActive(unlocked && wheelData.SpinCountByAdsInDay < 5);
        }

        private void ShowDiscoveryBtn()
        {
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_DISCOVERY;
            if (unlocked)
            {
                btnDiscovery.labelTMP.text = "Discovery";
                imgDiscoveryLock.SetActive(false);
                modelDiscovery.timeScale = 1f;
                modelDiscovery.material = mainModelMaterial;

                // if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.DISCOVERY))
                // {
                //     EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.DISCOVERY));
                // }
            }
            else
            {
                btnDiscovery.labelTMP.text = GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_DISCOVERY);
                imgDiscoveryLock.SetActive(true);
                modelDiscovery.timeScale = 0f;
                modelDiscovery.material = greyModelMaterial;
            }
            btnDiscovery.SetEnable(unlocked);
            var discoveriesGroup = GameData.Instance.DiscoveriesGroup;
            imgDiscoveryNoti.SetActive(unlocked && discoveriesGroup.CheckNoti());

        }

        private void ShowSummonGateBtn()
        {
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            // var powerFragment = currenciesGroup.GetValue(IDs.CURRENCY_POWER_FRAGMENT);
            // var powerCrytal = currenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL);
            // var devineCrytal = currenciesGroup.GetValue(IDs.CURRENCY_DEVINE_CRYSTAL);

            CheckNotif();
        }
        public void CheckNotif()
        {
            //Summon Gate Notif:
            var heroesGroup = GameData.Instance.HeroesGroup;
            var freePowerFragmentSummon = heroesGroup.FreePowerFragmentSummon;
            var freePowerCrystalSummon = heroesGroup.FreePowerCrystalSummon;
            var powerFragment = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_POWER_FRAGMENT);
            var powerCrytal = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL);
            var devineCrytal = GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_DEVINE_CRYSTAL);
            imgSummonGateNoti.SetActive(freePowerFragmentSummon
                                    || freePowerCrystalSummon
                                    || powerFragment > 0
                                    || powerCrytal > 0
                                    || devineCrytal > 0);

            //Event Notif:
            bool notifDailyLogin = GameData.Instance.DailyQuestsGroup.CheckDailyLoginNoti();
            bool notif7DayBonus = !GameData.Instance.DailyQuestsGroup.ClaimedToday;
            bool dailyGift = !GameData.Instance.DailyQuestsGroup.MaxDailyGiftCountInDay;
            imgAllEventNoti.SetActive(notifDailyLogin || notif7DayBonus || dailyGift);
            MainPanel.instance.EventAllPanel.SetNotif(notif7DayBonus, notifDailyLogin, dailyGift);

        }

        public void ShowPvpBtn()
        {
            bool isActive = GameUnityData.instance.GameRemoteConfig.Function_PvP_Active;
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_PVP && isActive;
            if (unlocked)
            {
                btnPvP.image.material = null;
                if (Config.PvpConfig.LoadCache())
                    txtPvPTime.gameObject.SetActive(true);
                else
                    txtPvPTime.gameObject.SetActive(false);

            }
            else
            {
                btnPvP.image.material = greyModelMaterial;
                if (isActive)
                {
                    txtPvpUnlock.text = GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_PVP);
                    txtPvPTime.gameObject.SetActive(false);
                }
                else
                {
                    txtPvpUnlock.text = Localization.Get(Localization.ID.MAINTENANCE).ToUpper();
                    txtPvPTime.gameObject.SetActive(false);
                }
            }
            btnPvP.SetEnable(unlocked);
            imgPvPLock.SetActive(!unlocked);
        }

        private void ShowBaseBtn()
        {
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_BASE;
            if (unlocked)
            {
                btnBase.interactable = true;
                imgBasePicture.color = Color.white;
                txtBaseUnlock.gameObject.SetActive(false);
            }
            else
            {
                btnBase.interactable = false;
                imgBasePicture.color = Color.gray;
                txtBaseUnlock.text = Localization.Get(Localization.ID.CLEAR).ToUpper() + "\n" + GameData.Instance.MissionLevelToString(Constants.UNLOCK_BASE);
                txtBaseUnlock.gameObject.SetActive(true);
            }

            imgBaseNoti.SetActive(unlocked && GameData.Instance.BaseGroup.CheckNoti());
        }

        private void ShowTimeBuffBtn()
        {
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_AFK_REWARD;
            if (unlocked)
            {
                btnTimeBuff.labelTMP.text = Localization.Get(Localization.ID.HOME_BUTTON_7).ToUpper();
                imgTimeBuffLock.SetActive(false);
            }
            else
            {
                btnTimeBuff.labelTMP.text = GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_AFK_REWARD);
                imgTimeBuffLock.SetActive(true);
            }
            btnTimeBuff.SetEnable(unlocked);
            imgTimeBuffNoti.SetActive(unlocked && GameData.Instance.MissionsGroup.FreeTicket);
        }

        private void OnHeroFragmentChange(HeroFragmentChangeEvent e)
        {
            ShowInventoryBtn();
        }

        private void OnFBChange(FBChangeEvent e)
        {
            ShowAvatar();
        }

        private void ShowInventoryBtn()
        {
            imgInventoryNoti.SetActive(GameData.Instance.ItemsGroup.CheckNoti());
        }

        private void ShowFactoryBtn()
        {
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_FACTORY;
            if (unlocked)
            {
                btnFactory.labelTMP.text = Localization.Get(Localization.ID.HOME_BUTTON_12); /*"Workshop";*/
                imgFactoryLock.SetActive(false);

                //if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.DISSAMBLE_GEAR))
                //{
                //    var gearDatasEquip = GameData.Instance.GearsGroup.GetAllGearDatasEquip();
                //    bool hasGearToStarUp = false;
                //    var count = gearDatasEquip.Count;
                //    for (int i = 0; i < count; i++)
                //    {
                //        if (gearDatasEquip[i].star <= 1)
                //        {
                //            hasGearToStarUp = true;
                //            break;
                //        }
                //    }
                //    if (hasGearToStarUp) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.DISSAMBLE_GEAR));
                //}
            }
            else
            {
                btnFactory.labelTMP.text = GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_FACTORY);
                imgFactoryLock.SetActive(true);
            }
            btnFactory.SetEnable(unlocked);
        }

        private void ShowLaboratoryBtn()
        {
            var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_STAR_UP_HERO;
            if (unlocked)
            {
                btnLaboratory.labelTMP.text = Localization.Get(Localization.ID.HOME_BUTTON_9);/*"Hero Lab";*/
                imgLaboratoryLock.SetActive(false);
                modelLaboratory.timeScale = 1f;
                modelLaboratory.material = mainModelMaterial;

                //if (GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_DISASSEMBLE_HERO
                //    && !TutorialsManager.Instance.IsCompleted(TutorialsGroup.DISSAMBLE_HERO))
                //{
                //    var heroDatasEquip = GameData.Instance.HeroesGroup.GetEquippedHeroes();
                //    bool hasHeroToStarUp = false;
                //    var count = heroDatasEquip.Length;
                //    for (int i = 0; i < count; i++)
                //    {
                //        if (heroDatasEquip[i] != null && heroDatasEquip[i].star <= 1)
                //        {
                //            hasHeroToStarUp = true;
                //            break;
                //        }
                //    }
                //    if (hasHeroToStarUp) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.DISSAMBLE_HERO));
                //}
            }
            else
            {
                btnLaboratory.labelTMP.text = GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_STAR_UP_HERO);
                imgLaboratoryLock.SetActive(true);
                modelLaboratory.timeScale = 0f;
                modelLaboratory.material = greyModelMaterial;
            }
            btnLaboratory.SetEnable(unlocked);
            imgLaboratoryNoti.SetActive(unlocked && GameData.Instance.HeroesGroup.CheckHeroStarUpNoti());
        }

        private void OnBaseLevelUp(BaseLevelUpEvent e)
        {
            ShowBaseBtn();
        }

        private void ShowBtnCampaign()
        {
            GameData.Instance.CampaignsGroup.CheckCurrentCampaign();

            var campaignsGroup = GameData.Instance.CampaignsGroup;
            var currentCampaign = GameData.Instance.CampaignsGroup.CurrentCampaign;

            var campaignCount = campaignsGroup.CampaignCount;
            var maxCount = currentCampaign.GetMaxCount();
            if (campaignCount < maxCount)
            {
                txtCampaignCount.text = campaignCount + "/" + maxCount;
                txtCampaignCount.color = Color.red;
                txtCampaignInfo.color = Color.white;
            }
            else
            {
                txtCampaignCount.text = maxCount + "/" + maxCount;
                txtCampaignCount.color = Color.green;
                txtCampaignInfo.color = Color.green;
            }
            txtCampaignInfo.text = currentCampaign.GetDescription();
        }

        //Add 1 notif lúc 12h bắn cho user kèm quà tặng 100k gold , 50 gem , 300 scrap với nội dung : Comeback and take free gem ! We need you now !
        private bool CheckNoon()
        {
            int currentSecondInDay = (int)(UnbiasedTime.Instance.Now() - UnbiasedTime.Instance.Today()).TotalSeconds;
            int before12h = 11 * 60 * 60 + 45 * 60;
            int after12h = 12 * 60 * 60 + 15 * 60;

            if (currentSecondInDay > before12h && currentSecondInDay < after12h)
            {
                var rewardInfos = new List<RewardInfo>();
                rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, 100000));
                rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_GEM, 50));
                rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_MATERIAL, 300));

                LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_NOTIFI_12H);
                GameData.Instance.DailyQuestsGroup.EatNoon();

                return true;
            }

            return false;
        }

        private bool CheckDinner()
        {
            int currentSecondInDay = (int)(UnbiasedTime.Instance.Now() - UnbiasedTime.Instance.Today()).TotalSeconds;
            int before20h = 19 * 60 * 60 + 45 * 60;
            int after20h = 20 * 60 * 60 + 15 * 60;

            if (currentSecondInDay > before20h && currentSecondInDay < after20h)
            {
                var rewardInfos = new List<RewardInfo>();
                rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, 100000));
                rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_GEM, 100));
                // rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_MATERIAL, 300));

                LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_NOTIFI_20H);
                GameData.Instance.DailyQuestsGroup.EatDinner();

                return true;
            }

            return false;
        }
    }
}