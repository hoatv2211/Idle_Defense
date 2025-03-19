using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;
using DG.Tweening;
using UnityEngine.UI;
using EventDispatcher = Utilities.Common.EventDispatcher;

namespace FoodZombie.UI
{
    public class DiscoveryPanel : MyGamesBasePanel
    {
        public CurrencyView coinView;
        public CurrencyView gemView;
        public CurrencyView expHeroView;

        public SimpleTMPButton btnBackToDiscovery;
        
        public GameObject groupDiscovery;
        public GameObject groupDiscoveryLevel;
        
        //
        public List<DiscoveryView> discoveryViews;
        public List<DiscoveryLevelView> discoveryLevelViews;

        //level
        public TextMeshProUGUI txtDiscoveryName;
        public TextMeshProUGUI txtDiscoveryCount;
        public SimpleTMPButton btnBuyMoreCount;

        public RectTransform imgLock;
        
        private DiscoveryData discoveryData;
        
        private DiscoveriesGroup DiscoveriesGroup => GameData.Instance.DiscoveriesGroup;
        
        void Start()
        {
            btnBackToDiscovery.onClick.AddListener(BtnBackToDiscovery_Pressed);
            btnBuyMoreCount.onClick.AddListener(BtnBuyMoreCount_Pressed);
        }
        
        internal override void Init()
        {
            GameData.Instance.GameConfigGroup.CheckNewDay();

            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);

            RefreshDiscovery();
            
            //if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.DISCOVERY))
            //{
            //    EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.DISCOVERY));
            //}
        }

        private void RefreshDiscovery()
        {
            groupDiscovery.SetActive(true);
            groupDiscoveryLevel.SetActive(false);
            
            var today = GameData.Instance.GameConfigGroup.GetDayOfWeek();
            
            var discoveryDatas = DiscoveriesGroup.GetAllDiscoveryDatas();
            var count = discoveryViews.Count;
            for (int i = 0; i < count; i++)
            {
                var discoveryData = discoveryDatas[i];
                var discoveryView = discoveryViews[i];
                discoveryView.Init(discoveryData, RefreshDiscoveryLevel, today);
            }
        }

        public void RefreshDiscoveryLevel(DiscoveryData _discoveryData)
        {
            discoveryData = _discoveryData;
            groupDiscovery.SetActive(false);
            groupDiscoveryLevel.SetActive(true);

            btnBuyMoreCount.SetEnable(discoveryData.CountClaimInDay > 0);
            var levelChallenge = discoveryData.LevelChallenge;
            var countClaimInDay = discoveryData.CountClaimInDay;
            txtDiscoveryName.text = discoveryData.NameLocal;
            txtDiscoveryCount.text = /*Localization.Get(Localization.ID.TIME)+": " +*/ (discoveryData.LimitInDay - countClaimInDay) + "/" + discoveryData.LimitInDay;
            
            var discoveryLevels = discoveryData.discoveryLevels;
            var count = discoveryLevelViews.Count;
            for (int i = 0; i < count; i++)
            {
                var discoveryLevel = discoveryLevels[i];
                var discoveryLevelView = discoveryLevelViews[i];
                discoveryLevelView.Init(discoveryData, discoveryLevel, RefreshDiscoveryLevel);
            }
        }

        private void BtnBackToDiscovery_Pressed()
        {
            groupDiscovery.SetActive(true);
            groupDiscoveryLevel.SetActive(false);

            RefreshDiscovery();
        }

        private void BtnBuyMoreCount_Pressed()
        {
            if (!discoveryData.CanBuy)
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_12));
                //MainPanel.instance.ShowWarningPopup("You run out of daily purchases, upgrade VIP level to get more purchases");
                return;
            }
            
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, 100))
            {
                //MainPanel.instance.ShowWarningPopup("Not enough gem");
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
                GameData.Instance.UserGroup.MissGemCount++;
                return;
            }
            
            currenciesGroup.Pay(IDs.CURRENCY_GEM, 100, TrackingConstants.VALUE_DISCOVERY);
            discoveryData.BuyMoreCount();
            RefreshDiscoveryLevel(discoveryData);
        }

		protected override void AfterHiding()
		{
			base.AfterHiding();
            Config.LogScene(TrackingConstants.SceneName.Screen_Home_MainMenu);
        }
	}
}
