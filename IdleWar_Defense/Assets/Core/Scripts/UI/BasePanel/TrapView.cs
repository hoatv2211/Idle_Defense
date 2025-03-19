using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class TrapView : MonoBehaviour
{
	public Image imgIcon;
	public TextMeshProUGUI txtName;
	public TextMeshProUGUI txtLevel;
	public Text txtItemCount, txtGemBuy;
	public Image imgStatsView, imgStatsViewNext;
	public Sprite sprStatHP, sprStatDam;
	public TextMeshProUGUI txtStat, txtStatNext;
	public TextMeshProUGUI txtInfo;
	public Text txtLevelUpCoin;
	public TextMeshProUGUI txtLevelRequire;
	public SimpleTMPButton btnUpgrade, btnBuy, btnTry;
	public GameObject imgNoti, imgNextLV;
	public GameObject[] objectToNextLV;
	// [SerializeField]
	private TrapData trapData;
	private MarketItemData marketItemData;
	private StoreGroup StoreGroup => GameData.Instance.StoreGroup;

	private void Start()
	{
		btnUpgrade.onClick.AddListener(BtnUpgrade_Pressed);
		btnTry.onClick.AddListener(BtnTry_Pressed);
		btnBuy.onClick.AddListener(BtnBuy_Pressed);
		LoadTry();
		EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
		EventDispatcher.AddListener<UserTryTrapEvent>(OnTryTrapEvent);
	}

	private void OnTryTrapEvent(UserTryTrapEvent e)
	{
		LoadTry();
	}

	void LoadTry()
	{
		if (!GameData.Instance.UserGroup.CanTryTrap(trapData))
			btnTry.gameObject.SetActive(false);
	}
	private void OnDestroy()
	{
		EventDispatcher.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
		EventDispatcher.RemoveListener<UserTryTrapEvent>(OnTryTrapEvent);
	}

	public void Init(TrapData _trapData)
	{
		trapData = _trapData;
		var marketItemDatas = StoreGroup.GetAllMarketItemDatas();
		var count = marketItemDatas.Count;
		for (int i = 0; i < count; i++)
		{
			var marketItemData = marketItemDatas[i];
			RewardInfo ri = marketItemData.GetReward();
			if (ri.Type == IDs.REWARD_TYPE_TRAP && ri.Id == trapData.Id)
			{
				this.marketItemData = marketItemData;
				break;
			}
		}
		ShowTrap();
	}

	float hp, damage, HPRegen;
	public void ShowTrap()
	{
		imgIcon.sprite = trapData.GetIcon();
		//txtName.text = trapData.Name;
		txtName.text = trapData.NameLocal;
		txtLevel.text = "" + trapData.Level;
		hp = trapData.HP;
		damage = trapData.Damage;
		HPRegen = trapData.HPRegen;

		//foreach (GameObject item in objectToNextLV)
		//{
		//    item.SetActive(true);
		//}
		if (hp > 0f)
		{
			// groupHP.SetActive(true);
			imgStatsView.sprite = sprStatHP;
			imgStatsViewNext.sprite = sprStatHP;
			//txtStat.text = "" + hp;
			//txtStatNext.text = "" + trapData.HPnextLV;
			txtStat.text = BigNumberAlpha.Create(hp).GetKKKString();
			txtStatNext.text = BigNumberAlpha.Create(trapData.HPnextLV).GetKKKString();
			// txt
		}
		else
		{
			//  groupHP.SetActive(false);
		}

		if (damage > 0)
		{
			//     groupDamage.SetActive(true);
			imgStatsView.sprite = sprStatDam;
			imgStatsViewNext.sprite = sprStatDam;
			//txtStat.text = "" + damage;
			//txtStatNext.text = "" + trapData.DamagenextLV;
			txtStat.text = BigNumberAlpha.Create(damage).GetKKKString();
			txtStatNext.text = BigNumberAlpha.Create(trapData.DamagenextLV).GetKKKString();
		}
		else
		{
			//     groupDamage.SetActive(false);
		}
		if (damage <= 0 && hp <= 0 && HPRegen > 0f)
		{
			// groupHP.SetActive(true);
			imgStatsView.sprite = sprStatHP;
			imgStatsViewNext.sprite = sprStatHP;
			//txtStat.text = "" + HPRegen;
			//txtStatNext.text = "" + trapData.HPRegenNext;
			txtStat.text = BigNumberAlpha.Create(HPRegen).GetKKKString();
			txtStatNext.text = BigNumberAlpha.Create(trapData.HPRegenNext).GetKKKString();
			// txt
		}
		else
		{
			//  groupHP.SetActive(false);
		}
		//if (hp <= 0 && damage <= 0)
		//{
		//    imgStatsView.sprite = null;
		//    imgStatsViewNext.sprite = null;
		//    txtStat.text = "";
		//    txtStatNext.text = "";
		//    foreach (GameObject item in objectToNextLV)
		//    {
		//        item.SetActive(false);
		//    }
		//}

		txtInfo.text = trapData.Description;

		var unlocked = trapData.Unlocked;
		txtLevelUpCoin.transform.parent.SetActive(unlocked);
		txtLevelUpCoin.text = trapData.LevelUpCoin + "";
		txtLevelRequire.SetActive(!unlocked);
		txtLevelRequire.text = "Unlock at Base lv." + trapData.RequireLevelBase;
		txtItemCount.text = "x" + trapData.StockNumber;
		btnUpgrade.SetActive(unlocked);
		btnTry.SetActive(!unlocked && trapData.Id != IDs.ITEM_TRAP_CANON && GameData.Instance.UserGroup.CanTryTrap(trapData));
		txtItemCount.SetActive(marketItemData != null);
		btnBuy.SetActive(unlocked && marketItemData != null);

		if (marketItemData != null)
			txtGemBuy.text = marketItemData.GemCost.ToString();
		ShowTrapNoti();
	}
	public void BtnTry_Pressed()
	{
		if (!AdsHelper.__IsVideoRewardedAdReady())
		{
			//MainPanel.instance.ShowWarningPopup("Ads not available");
			MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_1));
		}
		else
		{
			AdsHelper.__ShowVideoRewardedAd(TrackingConstants.ADS_REWARD_TRY_TRAP, (isCompleted) =>
			{
				if (isCompleted)
				{
					RewardInfo _trap = new RewardInfo(IDs.REWARD_TYPE_TRAP, trapData.Id, 1);
					LogicAPI.ClaimReward(_trap, "TryTrap", true, RewardsPopup.AlignType.Horizontal, () => { });
					GameData.Instance.UserGroup.TryTrap(trapData);
					EventDispatcher.Raise(new UserTryTrapEvent());
					//EventDispatcher.Raise(new CurrencyChangedEvent());
					txtItemCount.text = "x" + trapData.StockNumber;
				}
			});
		}
	}
	public void BtnUpgrade_Pressed()
	{
		Config.LogEvent(string.Format(TrackingConstants.CLICK_BASE_X_LEVELUP, trapData.LogName));

		var coin = trapData.LevelUpCoin;
		var currenciesGroup = GameData.Instance.CurrenciesGroup;
		if (!currenciesGroup.CanPay(IDs.CURRENCY_COIN, coin))
		{
			//MainPanel.instance.ShowWarningPopup("Not enough coin");
			MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_9));
			return;
		}

		currenciesGroup.Pay(IDs.CURRENCY_COIN, coin, string.Format(TrackingConstants.VALUE_LEVEL_UP_TRAP_X, trapData.LogName));
		trapData.LevelUp();
		SoundManager.Instance.PlaySFX(IDs.SOUND_UP_STAR);
		ShowTrap();
		Config.LogEvent(string.Format(TrackingConstants.CLICK_BASE_X_LEVELUP_PASS, trapData.LogName));
	}
	private void BtnBuy_Pressed()
	{
		if (this.marketItemData == null)
		{
			return;
		}
		if (marketItemData.ByCoin)
		{
			int coin = marketItemData.CoinCost;
			var currenciesGroup = GameData.Instance.CurrenciesGroup;
			if (!currenciesGroup.CanPay(IDs.CURRENCY_COIN, coin))
			{
				//MainPanel.instance.ShowWarningPopup("Not enough coin");
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_9));
				return;
			}

			currenciesGroup.Pay(IDs.CURRENCY_COIN, coin, TrackingConstants.VALUE_STORE);
		}
		else
		{
			int gem = marketItemData.GemCost;
			var currenciesGroup = GameData.Instance.CurrenciesGroup;
			if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, gem))
			{
				//MainPanel.instance.ShowWarningPopup("Not enough gem");
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
				GameData.Instance.UserGroup.MissGemCount++;
				return;
			}

			currenciesGroup.Pay(IDs.CURRENCY_GEM, gem, TrackingConstants.VALUE_STORE);
		}

		var reward = marketItemData.Buy();
		LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_STORE, false);
		txtItemCount.text = "x" + trapData.StockNumber;
		//refreshMatket();
	}
	//noti
	private void OnCurrencyChanged(CurrencyChangedEvent e)
	{
		if (e.id == IDs.CURRENCY_COIN)
		{
			ShowTrapNoti();
		}
	}

	private void ShowTrapNoti()
	{
		imgNoti.SetActive(trapData.CheckNoti());
	}
}
