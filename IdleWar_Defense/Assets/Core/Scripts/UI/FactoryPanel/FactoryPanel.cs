using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;

namespace FoodZombie.UI
{
	public class FactoryPanel : MyGamesBasePanel
	{
		public CurrencyView coinView;
		public CurrencyView gemView;
		public CurrencyView materialView;

		public GameObject tabUpgrade;
		public GameObject tabDisassemble;

		public Toggle togUpgrade;
		public Toggle togDisassemble;

		//gear info
		public RectTransform groupGearInfo;
		public Image imgGearIcon;
		public Image[] imgStarGears;
		public Sprite imgStarOn;
		public Sprite imgStarOff;
		public TextMeshProUGUI txtGearName;
		public TextMeshProUGUI txtGearDamage;
		public TextMeshProUGUI txtGearHP;
		public TextMeshProUGUI txtGearArmor;
		public TextMeshProUGUI txtGearAttackSpeed;
		public TextMeshProUGUI txtGearCritRate;
		public TextMeshProUGUI txtGearAccuracy;
		//next star
		public TextMeshProUGUI txtGearDamageNextStar;
		public TextMeshProUGUI txtGearHPNextStar;
		public TextMeshProUGUI txtGearArmorNextStar;
		public TextMeshProUGUI txtGearAttackSpeedNextStar;
		public TextMeshProUGUI txtGearCritRateNextStar;
		public TextMeshProUGUI txtGearAccuracyNextStar;

		public TextMeshProUGUI txtCostMaterial;
		public TextMeshProUGUI txtCostCoin;
		public SimpleTMPButton btnUpgrade;

		[Separator("Upgrade")]
		public Transform transformPool;
		[SerializeField, Tooltip("Buildin Pool")] public List<HeroView> heroViewsPool;
		private HeroData heroData;

		public HeroGearSlot[] heroGearSlots;
		private HeroGearSlot choiceHeroGearSlot;

		[Separator("Disassemble")]
		public SimpleTMPButton btnDisassemble;
		public SimpleTMPButton btnSmart;
		public Transform transformDisassemblePool;
		[SerializeField, Tooltip("Buildin Pool")] public List<GearView> disassembleGearViewsPool;
		private List<GearData> listDisassembleGears;

		[Separator("Disassemble choice")]
		public Transform transformChoicePool;
		[SerializeField, Tooltip("Buildin Pool")] public List<GearView> choiceGearViewsPool;
		public Transform transformRewardViewPool;
		[SerializeField, Tooltip("Buildin Pool")] public List<RewardView> rewardViewsPool;

		[Separator("Smart")]
		public GameObject smartPanel;
		public Toggle togRankC;
		public Toggle togRankB;
		public Toggle togRankA;
		public Toggle togRankS;
		public Toggle togRankSS;
		public SimpleTMPButton btnCloseSmartPanel1;
		public SimpleTMPButton btnCloseSmartPanel2;
		public Transform transformSmartRewardViewPool;
		[SerializeField, Tooltip("Buildin Pool")] private List<RewardView> smartRewardViewsPool;
		public SimpleTMPButton btnSmartDisassemble;

		private List<int> smartRanks = new List<int>();
		private List<GearData> listSmartGearsDisassemble = new List<GearData>();

		void Start()
		{
			btnUpgrade.onClick.AddListener(BtnUpgrade_Pressed);
			btnDisassemble.onClick.AddListener(BtnDisassemble_Pressed);

			togUpgrade.onValueChanged.AddListener(TogUpgrade_Changed);
			togDisassemble.onValueChanged.AddListener(TogDisassemble_Changed);

			btnSmart.onClick.AddListener(BtnSmart_Pressed);
			btnCloseSmartPanel1.onClick.AddListener(BtnCloseSmartPanel_Pressed);
			btnCloseSmartPanel2.onClick.AddListener(BtnCloseSmartPanel_Pressed);

			togRankC.onValueChanged.AddListener(TogRankC_Changed);
			togRankB.onValueChanged.AddListener(TogRankB_Changed);
			togRankA.onValueChanged.AddListener(TogRankA_Changed);
			togRankS.onValueChanged.AddListener(TogRankS_Changed);
			togRankSS.onValueChanged.AddListener(TogRankSS_Changed);

			btnSmartDisassemble.onClick.AddListener(BtnSmartDisassemble_Pressed);
		}

		internal override void Init()
		{
			coinView.Init(IDs.CURRENCY_COIN);
			gemView.Init(IDs.CURRENCY_GEM);
			materialView.Init(IDs.CURRENCY_MATERIAL);

			RefreshUpgradeTab();
			RefreshDisassembleTab();
			if (togUpgrade.isOn)
			{
				tabUpgrade.SetActive(true);
				tabDisassemble.SetActive(false);
			}
			else
			{
				tabUpgrade.SetActive(false);
				tabDisassemble.SetActive(true);
			}
		}
		protected override void AfterHiding()
		{
			base.AfterHiding();
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_MainMenu);
		}
		public void ShowUpgrade()
		{
			togUpgrade.isOn = true;
		}

		public void ShowDisassemble()
		{
			togDisassemble.isOn = true;
		}

		private void RefreshUpgradeTab()
		{
			heroViewsPool.Free();
			var heroes = GameData.Instance.HeroesGroup.GetAllHeroDatas();
			var count = heroes.Count;
			for (int i = 0; i < count; i++)
			{
				var heroData = heroes[i];
				if (heroData.GearDatas != null && heroData.GearDatas.Count > 0)
				{
					var heroView = heroViewsPool.Obtain(transformPool);
					heroView.Init(heroData, this, ChoiceHeroView);
					heroView.ActiveArrow(false);
					heroView.SetActive(true);
				}
			}
			if (TutorialsManager.Instance.IsCompleted(TutorialsGroup.DISSAMBLE_GEAR))
			{
				ChoiceHeroView(heroViewsPool[0]);
			}
			else
			{
				groupGearInfo.SetActive(false);
				count = heroGearSlots.Length;
				for (int i = 0; i < count; i++)
				{
					heroGearSlots[i].Init(null, ChoiceGearSlot);
				}
			}
		}

		private void ChoiceHeroView(HeroView heroView)
		{
			var count = heroViewsPool.Count;
			for (int i = 0; i < count; i++)
			{
				heroViewsPool[i].UnChoice();
			}

			heroView.Choice();
			ShowHero(heroView.heroData);
		}

		private void ShowHero(HeroData _heroData)
		{
			groupGearInfo.SetActive(false);

			heroData = _heroData;
			var count = heroGearSlots.Length;
			for (int i = 0; i < count; i++)
			{
				var gearData = heroData.GetEquippedGear(i + 1);
				heroGearSlots[i].Init(gearData, ChoiceGearSlot);
			}

			if (heroGearSlots[0].gearData != null) ChoiceGearSlot(heroGearSlots[0]);
			else
			{
				for (int i = 0; i < count; i++)
				{
					heroGearSlots[i].UnChoice();
				}
			}
		}

		private void ChoiceGearSlot(HeroGearSlot _heroGearSlot)
		{
			if (_heroGearSlot.gearData == null)
			{
				return;
			}
			else
			{
				choiceHeroGearSlot = _heroGearSlot;
				var count = heroGearSlots.Length;
				for (int i = 0; i < count; i++)
				{
					heroGearSlots[i].UnChoice();
				}
				choiceHeroGearSlot.Choice();

				ShowGearStats();
			}
		}

		private void ShowGearStats()
		{
			groupGearInfo.SetActive(true);

			var gearData = choiceHeroGearSlot.gearData;
			txtGearName.text = gearData.Name;
			imgGearIcon.sprite = gearData.GetIcon();

			var maxStars = gearData.GetMaxStars();
			var star = gearData.star;
			var count = imgStarGears.Length;
			for (int i = 0; i < star; i++)
			{
				imgStarGears[i].SetActive(true);
				imgStarGears[i].sprite = imgStarOn;
			}

			for (int i = star; i < maxStars; i++)
			{
				imgStarGears[i].SetActive(true);
				imgStarGears[i].sprite = imgStarOff;
			}

			for (int i = maxStars; i < count; i++)
			{
				imgStarGears[i].SetActive(false);
			}

			var isMaxStarUp = gearData.IsMaxStarUp();
			if (!isMaxStarUp)
			{
				txtCostMaterial.transform.parent.SetActive(true);
				txtCostCoin.transform.parent.SetActive(true);
				var starUpCost = gearData.GetGearStarUpCost();
				if (starUpCost != null)
				{
					txtCostMaterial.text = starUpCost.material + "";
					txtCostCoin.text = starUpCost.coin + "";

					btnUpgrade.SetEnable(true);
				}
				else
				{
					txtCostMaterial.transform.parent.SetActive(false);
					txtCostCoin.transform.parent.SetActive(false);

					btnUpgrade.SetEnable(false);
				}
			}
			else
			{
				txtCostMaterial.transform.parent.SetActive(false);
				txtCostCoin.transform.parent.SetActive(false);

				btnUpgrade.SetEnable(false);
			}

			//damage
			if (gearData.Damage > 0f || gearData.BonusDamage > 0f)
			{
				txtGearDamage.transform.parent.SetActive(true);
				txtGearDamage.text = "" + (gearData.Damage + gearData.BonusDamage);

				if (!isMaxStarUp)
				{
					txtGearDamageNextStar.SetActive(true);
					txtGearDamageNextStar.text = "" + (gearData.DamageNextStar + gearData.BonusDamageNextStar);
				}
				else
				{
					txtGearDamageNextStar.SetActive(false);
				}
			}
			else
			{
				txtGearDamage.transform.parent.SetActive(false);
			}

			//HP
			if (gearData.HP > 0f || gearData.BonusHp > 0f)
			{
				txtGearHP.transform.parent.SetActive(true);
				txtGearHP.text = "" + (gearData.HP + gearData.BonusHp);

				if (!isMaxStarUp)
				{
					txtGearHPNextStar.SetActive(true);
					txtGearHPNextStar.text = "" + (gearData.HPNextStar + gearData.BonusHpNextStar);
				}
				else
				{
					txtGearHPNextStar.SetActive(false);
				}
			}
			else
			{
				txtGearHP.transform.parent.SetActive(false);
			}

			//Armor
			if (gearData.Armor > 0f || gearData.BonusArmor > 0f)
			{
				txtGearArmor.transform.parent.SetActive(true);
				txtGearArmor.text = "" + (gearData.Armor + gearData.BonusArmor);

				if (!isMaxStarUp)
				{
					txtGearArmorNextStar.SetActive(true);
					txtGearArmorNextStar.text = "" + (gearData.ArmorNextStar + gearData.BonusArmorNextStar);
				}
				else
				{
					txtGearArmorNextStar.SetActive(false);
				}
			}
			else
			{
				txtGearArmor.transform.parent.SetActive(false);
			}

			//BonusAttackSpeed
			if (gearData.BonusAttackSpeed > 0f)
			{
				txtGearAttackSpeed.transform.parent.SetActive(true);
				txtGearAttackSpeed.text = "" + gearData.BonusAttackSpeed;

				if (!isMaxStarUp)
				{
					txtGearAttackSpeedNextStar.SetActive(true);
					txtGearAttackSpeedNextStar.text = "" + gearData.BonusAttackSpeedNextStar;
				}
				else
				{
					txtGearAttackSpeedNextStar.SetActive(false);
				}
			}
			else
			{
				txtGearAttackSpeed.transform.parent.SetActive(false);
			}

			//CritRate
			if (gearData.CritRate > 0f || gearData.BonusCritRate > 0f)
			{
				txtGearCritRate.transform.parent.SetActive(true);
				txtGearCritRate.text = "" + (gearData.CritRate + gearData.BonusCritRate);

				if (!isMaxStarUp)
				{
					txtGearCritRateNextStar.SetActive(true);
					txtGearCritRateNextStar.text = "" + (gearData.CritRateNextStar + gearData.BonusCritRateNextStar);
				}
				else
				{
					txtGearCritRateNextStar.SetActive(false);
				}
			}
			else
			{
				txtGearCritRate.transform.parent.SetActive(false);
			}

			//Accuracy
			if (gearData.Accuracy > 0f || gearData.BonusAccuracy > 0f)
			{
				txtGearAccuracy.transform.parent.SetActive(true);
				txtGearAccuracy.text = "" + (gearData.Accuracy + gearData.BonusAccuracy);

				if (!isMaxStarUp)
				{
					txtGearAccuracyNextStar.SetActive(true);
					txtGearAccuracyNextStar.text = "" + (gearData.AccuracyNextStar + gearData.BonusAccuracyNextStar);
				}
				else
				{
					txtGearAccuracyNextStar.SetActive(false);
				}
			}
			else
			{
				txtGearAccuracy.transform.parent.SetActive(false);
			}
		}

		//Disassemble
		private void ShowDisassembleTab()
		{
			tabUpgrade.SetActive(false);
			tabDisassemble.SetActive(true);

			RefreshDisassembleTab();
		}

		private void RefreshDisassembleTab()
		{
			listDisassembleGears = new List<GearData>();
			choiceGearViewsPool.Free();
			disassembleGearViewsPool.Free();

			var gearDatasUnEquip = GameData.Instance.GearsGroup.GetAllGearDatasUnEquip();
			var count = gearDatasUnEquip.Count;
			for (int i = 0; i < count; i++)
			{
				var gearView = disassembleGearViewsPool.Obtain(transformDisassemblePool);
				gearView.Init(gearDatasUnEquip[i], ChoiceDisassembleGearView);
				gearView.SetActive(true);
			}

			ShowListGearDisassembles();
		}

		private void ChoiceDisassembleGearView(GearView gearView, bool isAdd)
		{
			if (isAdd) listDisassembleGears.Add(gearView.gearData);
			else listDisassembleGears.Remove(gearView.gearData);

			ShowListGearDisassembles();
		}

		private void ShowListGearDisassembles()
		{
			choiceGearViewsPool.Free();

			var count = listDisassembleGears.Count;
			for (int i = 0; i < count; i++)
			{
				var labChoiceSlot = choiceGearViewsPool.Obtain(transformChoicePool);
				labChoiceSlot.Init(listDisassembleGears[i]);
				labChoiceSlot.SetActive(true);
			}

			var rewardInfos = GameData.Instance.GearsGroup.GetRewardFromDisassembleGears(listDisassembleGears);
			if (rewardInfos.Count <= 0) //hiển thị 0 0
			{
				rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, 0));
				rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_MATERIAL, 0));
			}

			rewardViewsPool.Free();
			count = rewardInfos.Count;
			for (int i = 0; i < count; i++)
			{
				var rewardView = rewardViewsPool.Obtain(transformRewardViewPool);
				rewardView.Init(rewardInfos[i]);
				rewardView.SetActive(true);
			}
		}

		private void TogUpgrade_Changed(bool value)
		{
			if (value)
			{
				tabUpgrade.SetActive(true);
				tabDisassemble.SetActive(false);
			}
		}

		private void TogDisassemble_Changed(bool value)
		{
			if (value)
			{
				tabUpgrade.SetActive(false);
				tabDisassemble.SetActive(true);
			}
		}

		private void BtnUpgrade_Pressed()
		{
			var gearData = choiceHeroGearSlot.gearData;
			var starUpCost = gearData.GetGearStarUpCost();
			if (starUpCost != null)
			{
				var material = starUpCost.material;
				var coin = starUpCost.coin;
				var currenciesGroup = GameData.Instance.CurrenciesGroup;
				if (!currenciesGroup.CanPay(IDs.CURRENCY_COIN, coin))
				{
					//MainPanel.instance.ShowWarningPopup("Not enough coins");
					MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_9));
					return;
				}

				if (!currenciesGroup.CanPay(IDs.CURRENCY_MATERIAL, material))
				{
					MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_13));
					//MainPanel.instance.ShowWarningPopup("Not enough scraps");
					return;
				}

				currenciesGroup.Pay(IDs.CURRENCY_COIN, coin, TrackingConstants.VALUE_LEVEL_UP_GEAR);
				currenciesGroup.Pay(IDs.CURRENCY_MATERIAL, material);
				gearData.StarUp();

				SoundManager.Instance.PlaySFX(IDs.SOUND_UP_STAR);
				choiceHeroGearSlot.Init(gearData, ChoiceGearSlot);
				ShowGearStats();
			}
		}

		private void BtnDisassemble_Pressed()
		{
			var rewardInfos = GameData.Instance.GearsGroup.DisassembleGears(listDisassembleGears);
			LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_DISASSEMBLE_GEAR);

			listDisassembleGears.Clear();
			ShowDisassembleTab();
		}

		private void BtnSmart_Pressed()
		{
			smartPanel.SetActive(true);

			ShowRewardDisassemble();
		}

		private void BtnCloseSmartPanel_Pressed()
		{
			smartPanel.SetActive(false);
		}

		private void TogRankC_Changed(bool value)
		{
			if (value) smartRanks.Add(IDs.RANK_C);
			else smartRanks.Remove(IDs.RANK_C);

			ShowRewardDisassemble();
		}

		private void TogRankB_Changed(bool value)
		{
			if (value) smartRanks.Add(IDs.RANK_B);
			else smartRanks.Remove(IDs.RANK_B);

			ShowRewardDisassemble();
		}

		private void TogRankA_Changed(bool value)
		{
			if (value) smartRanks.Add(IDs.RANK_A);
			else smartRanks.Remove(IDs.RANK_A);

			ShowRewardDisassemble();
		}

		private void TogRankS_Changed(bool value)
		{
			if (value) smartRanks.Add(IDs.RANK_S);
			else smartRanks.Remove(IDs.RANK_S);

			ShowRewardDisassemble();
		}

		private void TogRankSS_Changed(bool value)
		{
			if (value) smartRanks.Add(IDs.RANK_SS);
			else smartRanks.Remove(IDs.RANK_SS);

			ShowRewardDisassemble();
		}

		private void ShowRewardDisassemble()
		{
			var gearDatasUnEquip = GameData.Instance.GearsGroup.GetAllGearDatasUnEquip();

			listSmartGearsDisassemble = new List<GearData>();
			var count = smartRanks.Count;
			for (int i = 0; i < count; i++)
			{
				var rank = smartRanks[i];
				var countGears = gearDatasUnEquip.Count;
				//tìm trong list gear chưa equip cái nào có rank đã chọn thì cho vào list phân rã rồi remove đi cho nhẹ vòng lặp
				for (int j = countGears - 1; j >= 0; j--)
				{
					var gearData = gearDatasUnEquip[j];
					if (gearData.Rank == rank)
					{
						listSmartGearsDisassemble.Add(gearData);
						gearDatasUnEquip.RemoveAt(j);
					}
				}
			}

			var rewardInfos = GameData.Instance.GearsGroup.GetRewardFromDisassembleGears(listSmartGearsDisassemble);
			if (rewardInfos.Count <= 0) //hiển thị 0 0
			{
				rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, 0));
				rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_MATERIAL, 0));
			}

			smartRewardViewsPool.Free();
			count = rewardInfos.Count;
			for (int i = 0; i < count; i++)
			{
				var rewardView = smartRewardViewsPool.Obtain(transformSmartRewardViewPool);
				rewardView.Init(rewardInfos[i]);
				rewardView.SetActive(true);
			}
		}

		private void BtnSmartDisassemble_Pressed()
		{
			var rewardInfos = GameData.Instance.GearsGroup.DisassembleGears(listSmartGearsDisassemble);
			LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_DISASSEMBLE_GEAR);

			listSmartGearsDisassemble.Clear();
			ShowDisassembleTab();
			ShowRewardDisassemble();
		}
	}
}
