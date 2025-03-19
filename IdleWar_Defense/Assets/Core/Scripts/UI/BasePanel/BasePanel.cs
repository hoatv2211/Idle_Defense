using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.Formula.Functions;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;

namespace FoodZombie.UI
{
    public class BasePanel : MyGamesBasePanel
    {
        public CurrencyView coinView;
        public CurrencyView gemView;
        public CurrencyView expHeroView;

        public Image[] imgBaseLevels;
        public Sprite imgStarOn, imgStarOff;
        public SimpleTMPButton btnUpgradeBase;
        public GameObject imgBaseNoti;
        public TextMeshProUGUI txtBaseLevel;
        public TextMeshProUGUI txtUserLevelRequest;
        public TextMeshProUGUI txtUpgradeCost;
        public Transform TrapViewPanels;


        [Separator("Bot-Center")]
        public SimpleTMPButton btnMain;
        public SimpleTMPButton btnFormation;
        public GameObject imgFormationNoti;
        public SimpleTMPButton btnInventory;
        public GameObject imgInventoryNoti;
        public SimpleTMPButton btnHero;
        public GameObject imgHeroNoti;

        private BaseGroup BaseGroup => GameData.Instance.BaseGroup;
        TrapView[] _trapViews;
        public TrapView[] trapViews
        {
            get
            {

                if (_trapViews == null || _trapViews.Length <= 0)
                {
                    //trapViews = new List<TrapView>();
                    _trapViews = TrapViewPanels.GetComponentsInChildren<TrapView>();
                }
                return _trapViews;
            }
        }
        private void Start()
        {
            btnUpgradeBase.onClick.AddListener(BtnUpgradeBase_Pressed);

            //bot-center
            btnMain.onClick.AddListener(BtnMain_Pressed);
            btnFormation.onClick.AddListener(BtnFormation_Pressed);
            btnHero.onClick.AddListener(BtnHero_Pressed);
            btnInventory.onClick.AddListener((BtnInventory_Pressed));

            EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
            EventDispatcher.AddListener<ChangeGearEvent>(OnChangeGear);
            EventDispatcher.AddListener<FormationChangeEvent>(OnFormationChange);
            EventDispatcher.AddListener<HeroStarUpEvent>(OnHeroStarUp);
            EventDispatcher.AddListener<HeroLevelUpEvent>(OnHeroLevelUp);
            EventDispatcher.AddListener<HeroEvolutionEvent>(OnHeroEvolution);
            EventDispatcher.AddListener<HeroFragmentChangeEvent>(OnHeroFragmentChange);
        }

        private void OnDestroy()
        {
            EventDispatcher.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
            EventDispatcher.RemoveListener<ChangeGearEvent>(OnChangeGear);
            EventDispatcher.RemoveListener<FormationChangeEvent>(OnFormationChange);
            EventDispatcher.RemoveListener<HeroStarUpEvent>(OnHeroStarUp);
            EventDispatcher.RemoveListener<HeroLevelUpEvent>(OnHeroLevelUp);
            EventDispatcher.RemoveListener<HeroEvolutionEvent>(OnHeroEvolution);
            EventDispatcher.RemoveListener<HeroFragmentChangeEvent>(OnHeroFragmentChange);
        }

        internal override void Init()
        {
            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);

            InitBtns();

            ShowBase();
            ShowTrapViews();
        }
        private void InitBtns()
        {
            ShowHeroBtn();
            ShowFormationBtn();
            ShowInventoryBtn();
        }
        private void ShowBase()
        {
            var userLevel = GameData.Instance.UserGroup.Level;

            var baseLevel = BaseGroup.Level;
            txtBaseLevel.text = "" + baseLevel;
            var count = imgBaseLevels.Length;
            for (int i = 0; i < baseLevel; i++)
            {
                imgBaseLevels[i].sprite = imgStarOn;
            }
            for (int i = baseLevel; i < count; i++)
            {
                imgBaseLevels[i].sprite = imgStarOff;
            }

            if (BaseGroup.IsMaxLevel())
            {
                btnUpgradeBase.SetActive(false);
                txtUserLevelRequest.SetActive(false);
                txtUpgradeCost.transform.parent.SetActive(false);
            }
            else
            {
                var userLevelRequest = BaseGroup.GetUserLevelRequest();
                if (userLevel < userLevelRequest)
                {
                    btnUpgradeBase.SetEnable(false);
                    txtUserLevelRequest.SetActive(true);
                    txtUserLevelRequest.text = "Need user lv " + userLevelRequest;
                }
                else
                {
                    btnUpgradeBase.SetEnable(true);
                    txtUserLevelRequest.SetActive(false);
                }
                txtUpgradeCost.transform.parent.SetActive(true);
                txtUpgradeCost.text = BaseGroup.GetLevelUpCost() + "";
            }

            ShowBaseBtn();
        }

        private void ShowTrapViews()
        {
            var trapDatas = BaseGroup.GetAllTrapDatas();
            var count = trapViews.Length;
            for (int i = 0; i < count; i++)
            {
                trapViews[i].Init(trapDatas[i]);
            }
        }

        private void BtnUpgradeBase_Pressed()
        {
            Config.LogEvent(TrackingConstants.CLICK_BASE_LEVELUP);

            var coin = BaseGroup.GetLevelUpCost();
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_COIN, coin))
            {
                //MainPanel.instance.ShowWarningPopup("Not enough coin");
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_9));
                return;
            }

            currenciesGroup.Pay(IDs.CURRENCY_COIN, coin, TrackingConstants.VALUE_LEVEL_UP_BASE);
            BaseGroup.LevelUp();

            SoundManager.Instance.PlaySFX(IDs.SOUND_UP_STAR);
            ShowBase();
            ShowTrapViews();

            Config.LogEvent(TrackingConstants.CLICK_BASE_LEVELUP_PASS);
        }

        //bot-center
        private void BtnMain_Pressed()
        {
            Back();
            Config.LogEvent(TrackingConstants.CLICK_MAINMENU);
        }

        private void BtnHero_Pressed()
        {
            Back();
            MainPanel.instance.ShowHeroPanel();
        }

        private void BtnInventory_Pressed()
        {
            Back();
            MainPanel.instance.ShowInventoryPanel();
        }

        private void BtnFormation_Pressed()
        {
            Back();
            MainPanel.instance.ShowFormationPanel();
        }

        //noti
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
            else if (e.id == IDs.CURRENCY_BLUE_HERO_FRAGMENT
                     || e.id == IDs.CURRENCY_EPIC_HERO_FRAGMENT)
            {
                ShowInventoryBtn();
            }
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
        }

        private void ShowFormationBtn()
        {
            imgFormationNoti.SetActive(GameData.Instance.HeroesGroup.CheckFormationNoti());
        }

        private void OnHeroStarUp(HeroStarUpEvent e)
        {
            ShowHeroBtn();
        }

        private void OnHeroLevelUp(HeroLevelUpEvent e)
        {
            ShowHeroBtn();
        }

        private void OnHeroEvolution(HeroEvolutionEvent e)
        {
            ShowHeroBtn();
        }

        private void ShowBaseBtn()
        {
            imgBaseNoti.SetActive(GameData.Instance.BaseGroup.CheckNoti());
        }

        private void OnHeroFragmentChange(HeroFragmentChangeEvent e)
        {
            ShowInventoryBtn();
        }

        private void ShowInventoryBtn()
        {
            imgInventoryNoti.SetActive(GameData.Instance.ItemsGroup.CheckNoti());
        }
    }
}