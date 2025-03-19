using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Utilities.Service;

namespace FoodZombie.UI
{
	public class StorePanel : MyGamesBasePanel
	{
		public CurrencyView coinView;
		public CurrencyView gemView;
		public CurrencyView expHeroView;

		public Toggle togSpecialPacks;
		public Toggle togPremiumPacks;
		public Toggle togDeals;
		public Toggle togBattlePacks;
		public Toggle togGemPacks;
		public Toggle togMarketPlace;
		public Toggle togFlashSale;

		public SimpleTMPButton btnTermOfService;
		public SimpleTMPButton btnPrivacyPolicy;
		public SimpleTMPButton btnRestorePurchase;

		//deal tabs;
		public Toggle togDailyDeal;
		public Toggle togWeeklyDeal;
		public Toggle togMonthlyDeal;

		public GameObject groupSpecialPacks;
		public GameObject groupPremiumPacks;
		public GameObject groupDeals;
		public GameObject groupBattlePacks;
		public GameObject groupGemPacks;
		public GameObject groupMarketPlace;
		public TextMeshProUGUI txtTimeNextMarket;
		public GameObject groupFlashSaleLabel;
		public GameObject groupFlashSale;
		public GameObject object1StPurchase;

		public TextMeshProUGUI txtFlashSaleTime;
		public SimpleTMPButton btnF5FlashSale;

		public SimpleTMPButton btnF5Market;

		public PremiumPackageView[] premiumPackageViews;

		public Transform transformSpecialPackPool;
		[SerializeField, Tooltip("Buildin Pool")] private List<PackageView> specialPackageViewsPool;
		public Transform transformDealPool;
		[SerializeField, Tooltip("Buildin Pool")] private List<PackageView> dealsPool;
		public Transform transformBattlePackPool;
		[SerializeField, Tooltip("Buildin Pool")] private List<PackageView> battlePackageViewsPool;
		public Transform transformGemPackPool;
		[SerializeField, Tooltip("Buildin Pool")] private List<PackageView> gemPackageViewsPool;
		public Transform transformMarketPool;
		[SerializeField, Tooltip("Buildin Pool")] private List<MarketItemView> marketItemViewsPool;
		public Transform transformFlashSalePool;
		[SerializeField, Tooltip("Buildin Pool")] private List<FlashSaleView> flashSaleViewsPool;

		private StoreGroup StoreGroup => GameData.Instance.StoreGroup;

		private double deltaTime;

		void Start()
		{
			btnTermOfService.onClick.AddListener(BtnTermOfService_Pressed);
			btnPrivacyPolicy.onClick.AddListener(BtnPrivacyPolicy_Pressed);
			btnRestorePurchase.onClick.AddListener(BtnRestorePurchase_Pressed);

			btnF5FlashSale.onClick.AddListener(BtnF5FlashSale_Pressed);
			btnF5Market.onClick.AddListener(BtnF5Market_Pressed);

			togSpecialPacks.onValueChanged.AddListener(TogSpecialPacks_Changed);
			togPremiumPacks.onValueChanged.AddListener(TogPremiumPacks_Changed);
			togDeals.onValueChanged.AddListener(TogDeals_Changed);
			togFlashSale.onValueChanged.AddListener(TogFlashSale_Changed);
			togGemPacks.onValueChanged.AddListener(TogGemPacks_Changed);
			togMarketPlace.onValueChanged.AddListener(TogMarketPlace_Changed);
			togBattlePacks.onValueChanged.AddListener(TogBattlePacks_Changed);

			togDailyDeal.onValueChanged.AddListener(TogDailyDeal_Changed);
			togWeeklyDeal.onValueChanged.AddListener(TogWeeklyDeal_Changed);
			togMonthlyDeal.onValueChanged.AddListener(TogMonthlyDeal_Changed);

			EventDispatcher.AddListener<FlashSaleRefreshEvent>(OnFlashSaleRefresh);
			EventDispatcher.AddListener<BuyCountChangeEvent>(OnBuyCountChange);
		}

		void Update()
		{
			//flashSale
			txtFlashSaleTime.text = "Reset in " + StoreGroup.GetTimeRefresh();

			//market
			txtTimeNextMarket.text = "Reset in " + TimeHelper.FormatHHMMss(deltaTime, true);
			deltaTime -= Time.deltaTime;

			if (deltaTime <= 0f)
			{
				Init();
			}
		}

		private void CheckTime()
		{
			//tính time sang ngày tiếp theo
			//DateTime now = DateTime.Now;
			DateTime now = UnbiasedTime.Instance.Now(); //Fix cheat time

			DateTime tomorrow = now.AddDays(1).Date.Add(new TimeSpan(0, 7, 0, 0));
			deltaTime = (tomorrow - now).TotalSeconds + 1f; //thêm 1s delay cho chắc lúc f5 sang ngày
		}

		private void OnDestroy()
		{
			EventDispatcher.RemoveListener<FlashSaleRefreshEvent>(OnFlashSaleRefresh);
			EventDispatcher.RemoveListener<BuyCountChangeEvent>(OnBuyCountChange);
		}

		internal override void Init()
		{
			GameData.Instance.GameConfigGroup.CheckNewDay();

			coinView.Init(IDs.CURRENCY_COIN);
			gemView.Init(IDs.CURRENCY_GEM);
			expHeroView.Init(IDs.CURRENCY_EXP_HERO);

			groupSpecialPacks.SetActive(togSpecialPacks.isOn);
			groupPremiumPacks.SetActive(togPremiumPacks.isOn);
			groupDeals.SetActive(togDeals.isOn);
			groupBattlePacks.SetActive(togBattlePacks.isOn);
			groupGemPacks.SetActive(togGemPacks.isOn);
			groupMarketPlace.SetActive(togMarketPlace.isOn);
			groupFlashSaleLabel.SetActive(togFlashSale.isOn);
			groupFlashSale.SetActive(togFlashSale.isOn);

			Refresh();

			StoreGroup.CheckRefreshTime();

			CheckTime();
		}
		protected override void AfterShowing()
		{
			base.AfterShowing();
		//	AppsFlyerObjectScript.LogVisitShop();
		
		}

		protected override void AfterHiding()
		{
			base.AfterHiding();
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_MainMenu);
		}
		private void Refresh()
		{
			//special
			PackageData firstTimePurchasePack = null;
			specialPackageViewsPool.Free();
			var packageDatas = StoreGroup.GetAllSpecialPackageDatas();
			var count = packageDatas.Count;
			for (int i = 0; i < count; i++)
			{
				var packageData = packageDatas[i];

				if (packageData.CanShow && packageData.CanBuy)
				{
					if (packageData.Id == 1)
					{
						firstTimePurchasePack = packageData;
					}
					else
					{
						var packageView = specialPackageViewsPool.Obtain(transformSpecialPackPool);
						packageView.Init(packageData);
						packageView.SetActive(true);
					}
				}
			}
			//add cái này cuối list
			if (firstTimePurchasePack != null)
			{
				var packageView = specialPackageViewsPool.Obtain(transformSpecialPackPool);
				packageView.Init(firstTimePurchasePack);
				packageView.SetActive(true);
			}

			//premium
			var premiumPackageDatas = StoreGroup.GetAllPremiumPackageDatas();
			count = premiumPackageDatas.Count;
			for (int i = 0; i < count; i++)
			{
				premiumPackageViews[i].Init(premiumPackageDatas[i]);
			}

			//deal
			ShowDeals();

			//gem
			gemPackageViewsPool.Free();
			packageDatas = StoreGroup.GetAllGemPackageDatas();
			count = packageDatas.Count;
			for (int i = 0; i < count; i++)
			{
				var packageData = packageDatas[i];

				var packageView = gemPackageViewsPool.Obtain(transformGemPackPool);
				packageView.Init(packageData);
				packageView.SetActive(true);

			}

			//battle
			battlePackageViewsPool.Free();
			packageDatas = StoreGroup.GetAllBattlePackageDatas();
			count = packageDatas.Count;
			for (int i = 0; i < count; i++)
			{
				var packageData = packageDatas[i];

				var packageView = battlePackageViewsPool.Obtain(transformBattlePackPool);
				packageView.Init(packageData);
				packageView.SetActive(true);
			}

			//market place
			RefreshMarket();

			//flash sale
			RefreshFlashSale();
		}

		public void ShowSpecialPacks()
		{
			togSpecialPacks.isOn = true;
		}

		public void ShowFlashSale()
		{
			togFlashSale.isOn = true;
		}

		public void ShowGemPacks()
		{
			togGemPacks.isOn = true;
		}

		public void ShowBattlePacks()
		{
			togBattlePacks.isOn = true;
		}

		public void ShowPremiumPacks()
		{
			togPremiumPacks.isOn = true;
		}

		private void RefreshMarket()
		{
			if (object1StPurchase != null)
				object1StPurchase.SetActive(GameData.Instance.UserGroup.inappTime.Value <= 0);
			marketItemViewsPool.Free();
			var marketItemDatas = StoreGroup.GetAllMarketItemDatas();
			var count = marketItemDatas.Count;
			for (int i = 0; i < count; i++)
			{
				var marketItemData = marketItemDatas[i];
				if (marketItemData.GetReward().Type.Equals(IDs.REWARD_TYPE_TRAP))
					break;
				var marketItemView = marketItemViewsPool.Obtain(transformMarketPool);
				marketItemView.Init(marketItemData, RefreshMarket);
				marketItemView.SetActive(true);
			}
		}

		private void RefreshFlashSale()
		{
			var flashSaleDatas = StoreGroup.GetFlashSaleDataChoices();
			var count = flashSaleDatas.Count;
			flashSaleViewsPool.Free();
			for (int i = 0; i < count; i++)
			{
				var flashSaleData = flashSaleDatas[i];
				if (!flashSaleData.Bought)
				{
					var flashSaleView = flashSaleViewsPool.Obtain(transformFlashSalePool);
					flashSaleView.Init(flashSaleData, RefreshFlashSale);
					flashSaleView.SetActive(true);
				}
			}
		}

		//private void OnBuyGemEvent(PackageView packView)
		//{

		//}

		private void OnFlashSaleRefresh(FlashSaleRefreshEvent e)
		{
			RefreshFlashSale();
		}

		private void OnBuyCountChange(BuyCountChangeEvent e)
		{
			Refresh();
		}

		private void BtnTermOfService_Pressed()
		{
			Application.OpenURL("https://www.beemob.vn/terms-of-use/");
		}

		private void BtnPrivacyPolicy_Pressed()
		{
			Application.OpenURL("https://www.beemob.vn/privacy-policy/");
		}

		private void BtnRestorePurchase_Pressed()
		{
			PaymentHelper.RestorePurchase(RestoreSuccess);
		}

		private void RestoreSuccess(bool success)
		{
			if (success) MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_33));
			//if (success) MainPanel.instance.ShowWarningPopup("Restoration process succeeded");
		}

		private void BtnF5FlashSale_Pressed()
		{
			var currenciesGroup = GameData.Instance.CurrenciesGroup;
			if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, 100))
			{
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
				//MainPanel.instance.ShowWarningPopup("Not enough gem");
				GameData.Instance.UserGroup.MissGemCount++;
				return;
			}

			currenciesGroup.Pay(IDs.CURRENCY_GEM, 100, TrackingConstants.VALUE_FLASH_SALE_F5);
			StoreGroup.F5FlashSale();
		}

		private void BtnF5Market_Pressed()
		{
			var currenciesGroup = GameData.Instance.CurrenciesGroup;
			if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, 100))
			{
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
				//MainPanel.instance.ShowWarningPopup("Not enough gem");
				GameData.Instance.UserGroup.MissGemCount++;
				return;
			}

			currenciesGroup.Pay(IDs.CURRENCY_GEM, 100, TrackingConstants.VALUE_MARKET_F5);
			StoreGroup.F5Market();

			RefreshMarket();
		}

		private void TogSpecialPacks_Changed(bool value)
		{
			if (value)
			{
				groupSpecialPacks.SetActive(true);
				groupPremiumPacks.SetActive(false);
				groupDeals.SetActive(false);
				groupBattlePacks.SetActive(false);
				groupGemPacks.SetActive(false);
				groupMarketPlace.SetActive(false);
				groupFlashSaleLabel.SetActive(false);
				groupFlashSale.SetActive(false);
			}
		}

		private void TogPremiumPacks_Changed(bool value)
		{
			if (value)
			{
				groupSpecialPacks.SetActive(false);
				groupPremiumPacks.SetActive(true);
				groupDeals.SetActive(false);
				groupBattlePacks.SetActive(false);
				groupGemPacks.SetActive(false);
				groupMarketPlace.SetActive(false);
				groupFlashSaleLabel.SetActive(false);
				groupFlashSale.SetActive(false);
			}
		}

		private void TogDeals_Changed(bool value)
		{
			if (value)
			{
				groupSpecialPacks.SetActive(false);
				groupPremiumPacks.SetActive(false);
				groupDeals.SetActive(true);
				groupBattlePacks.SetActive(false);
				groupGemPacks.SetActive(false);
				groupMarketPlace.SetActive(false);
				groupFlashSaleLabel.SetActive(false);
				groupFlashSale.SetActive(false);
			}
		}

		private void TogBattlePacks_Changed(bool value)
		{
			if (value)
			{
				groupSpecialPacks.SetActive(false);
				groupPremiumPacks.SetActive(false);
				groupDeals.SetActive(false);
				groupBattlePacks.SetActive(true);
				groupGemPacks.SetActive(false);
				groupMarketPlace.SetActive(false);
				groupFlashSaleLabel.SetActive(false);
				groupFlashSale.SetActive(false);
			}
		}

		private void TogGemPacks_Changed(bool value)
		{
			if (value)
			{
				groupSpecialPacks.SetActive(false);
				groupPremiumPacks.SetActive(false);
				groupDeals.SetActive(false);
				groupBattlePacks.SetActive(false);
				groupGemPacks.SetActive(true);
				groupMarketPlace.SetActive(false);
				groupFlashSaleLabel.SetActive(false);
				groupFlashSale.SetActive(false);
			}
		}

		private void TogMarketPlace_Changed(bool value)
		{
			if (value)
			{
				groupSpecialPacks.SetActive(false);
				groupPremiumPacks.SetActive(false);
				groupDeals.SetActive(false);
				groupBattlePacks.SetActive(false);
				groupGemPacks.SetActive(false);
				groupMarketPlace.SetActive(true);
				groupFlashSaleLabel.SetActive(false);
				groupFlashSale.SetActive(false);
			}
		}

		private void TogFlashSale_Changed(bool value)
		{
			if (value)
			{
				groupSpecialPacks.SetActive(false);
				groupPremiumPacks.SetActive(false);
				groupDeals.SetActive(false);
				groupBattlePacks.SetActive(false);
				groupGemPacks.SetActive(false);
				groupMarketPlace.SetActive(false);
				groupFlashSaleLabel.SetActive(true);
				groupFlashSale.SetActive(true);
			}
		}

		//deal tabs
		private void TogDailyDeal_Changed(bool value)
		{
			ShowDeals();
		}

		private void TogWeeklyDeal_Changed(bool value)
		{
			ShowDeals();
		}

		private void TogMonthlyDeal_Changed(bool value)
		{
			ShowDeals();
		}

		private void ShowDeals()
		{
			dealsPool.Free();
			var packageDatas = StoreGroup.GetAllDealPackageDatas();
			var count = packageDatas.Count;
			for (int i = 0; i < count; i++)
			{
				var packageData = packageDatas[i];

				if (packageData.CanShow)
				{
					if (togDailyDeal.isOn && packageData.Type == IDs.DAILY_PACK)
					{
						var packageView = dealsPool.Obtain(transformDealPool);
						packageView.Init(packageData);
						packageView.SetActive(true);
					}
					else if (togWeeklyDeal.isOn && packageData.Type == IDs.WEEKLY_PACK)
					{
						var packageView = dealsPool.Obtain(transformDealPool);
						packageView.Init(packageData);
						packageView.SetActive(true);
					}
					else if (togMonthlyDeal.isOn && packageData.Type == IDs.MONTHLY_PACK)
					{
						var packageView = dealsPool.Obtain(transformDealPool);
						packageView.Init(packageData);
						packageView.SetActive(true);
					}
				}
			}
		}
	}
}
