using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Utilities.Pattern.UI;

namespace FoodZombie.UI
{
	public class LaboratoryPanel : MyGamesBasePanel
	{
		public GameObject groupStarUp;
		public GameObject groupDisassemble;
		public GameObject groupListHeroes;

		public CurrencyView coinView;
		public CurrencyView gemView;
		public CurrencyView expHeroView;

		public Toggle togStarUp;
		public Toggle togDisassemble;
		public GameObject imgStarUpNoti;

		[Separator("StarUp")]
		public GameObject groupModel;
		private int selectHeroModel = -1;
		public GameObject[] modelHeroes;
		public Canvas layoutPrecedeModel;
		public SimpleTMPButton btnStarUp;
		public TextMeshProUGUI txtElementCost;
		public Image imgElementCost;
		public Image[] imgStarsOld, imgStarsNew;
		public Sprite imgStarOn, imgStarOff;
		public Transform transformStarUpPool;
		public TextMeshProUGUI txtHPOld;
		public TextMeshProUGUI txtDamageOld;
		public TextMeshProUGUI txtArmorOld;
		public TextMeshProUGUI txtHPNew;
		public TextMeshProUGUI txtDamageNew;
		public TextMeshProUGUI txtArmorNew;
		[SerializeField, Tooltip("Buildin Pool")] public List<HeroView> starUpHeroViewsPool;
		private HeroView heroView;
		private int elementCost;
		private int currencyId;

		[Separator("Disassemble")]
		public SimpleTMPButton btnDisassemble;
		public SimpleTMPButton btnSmart;
		public Transform transformDisassemblePool;
		[SerializeField, Tooltip("Buildin Pool")] public List<HeroView> disassembleHeroViewsPool;
		private List<HeroData> listDisassembleHeroDatas;

		[Separator("Disassemble choice")]
		public Transform transformChoicePool;
		[SerializeField, Tooltip("Buildin Pool")] public List<HeroView> choiceHeroViewsPool;
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

		public RectTransform imgLockGroupInfo;

		private List<int> smartRanks = new List<int>();
		private List<HeroData> listSmartHeroesDisassemble = new List<HeroData>();

		//list all hero data
		private List<HeroData> heroDatas;
		private HeroEvolutionInfo heroEvolutionInfo;

		private void Start()
		{
			btnStarUp.onClick.AddListener(BtnStarUp_Pressed);
			btnDisassemble.onClick.AddListener(BtnDisassemble_Pressed);

			togStarUp.onValueChanged.AddListener(TogStarUp_Changed);
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

			MainPanel.instance.onAnyChildHide += OnMainPanelChildHide;
			MainPanel.instance.onAnyChildShow += OnMainPanelChildShow;

			EventDispatcher.AddListener<HeroStarUpEvent>(OnHeroStarUp);
			EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
		}

		private void OnDestroy()
		{
			MainPanel.instance.onAnyChildHide -= OnMainPanelChildHide;
			MainPanel.instance.onAnyChildShow -= OnMainPanelChildShow;

			EventDispatcher.RemoveListener<HeroStarUpEvent>(OnHeroStarUp);
			EventDispatcher.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
		}

		internal override void Init()
		{
			coinView.Init(IDs.CURRENCY_COIN);
			gemView.Init(IDs.CURRENCY_GEM);
			expHeroView.Init(IDs.CURRENCY_EXP_HERO);

			RefreshStarUp();
			RefreshDisassemble();
			if (togStarUp.isOn)
			{
				if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_STAR_UP_HERO)
				{
					MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK)+ " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_STAR_UP_HERO, true));
					togDisassemble.isOn = true;
					return;
				}

				groupStarUp.SetActive(true);
				groupDisassemble.SetActive(false);
			}
			else
			{
				groupStarUp.SetActive(false);
				groupDisassemble.SetActive(true);
			}

			ShowNoti();
		}

		protected override void AfterHiding()
		{
			base.AfterHiding();
			Config.LogScene(TrackingConstants.SceneName.Screen_Home_MainMenu);
		}
		private void OnMainPanelChildHide(PanelController pPanel)
		{
			if (MainPanel.instance.TopPanel is LaboratoryPanel)
			{
				groupModel.SetActive(true);
				layoutPrecedeModel.overrideSorting = true;
			}
			else
			{
				groupModel.SetActive(false);
				layoutPrecedeModel.overrideSorting = false;
			}
		}

		private void OnMainPanelChildShow(PanelController pPanel)
		{
			if (MainPanel.instance.TopPanel is LaboratoryPanel)
			{
				groupModel.SetActive(true);
				layoutPrecedeModel.overrideSorting = true;
			}
			else
			{
				groupModel.SetActive(false);
				layoutPrecedeModel.overrideSorting = false;
			}
		}

		public void ShowStarUp()
		{
			togStarUp.isOn = true;
		}

		public void ShowDisassemble()
		{
			togDisassemble.isOn = true;
		}

		private void TogStarUp_Changed(bool value)
		{
			if (value)
			{
				groupStarUp.SetActive(true);
				groupDisassemble.SetActive(false);
			}

			ShowNoti();
		}

		private void TogDisassemble_Changed(bool value)
		{
			if (value)
			{
				if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_DISASSEMBLE_HERO)
				{
					MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK) + " " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_DISASSEMBLE_HERO, true));
					togStarUp.isOn = true;
					return;
				}

				groupStarUp.SetActive(false);
				groupDisassemble.SetActive(true);
			}
		}

		#region Evolution

		private void RefreshStarUp()
		{
			heroDatas = GameData.Instance.HeroesGroup.GetAllHeroDatas();
			ShowHeroesForStarUp();
		}

		private void ShowHeroesForStarUp()
		{
			starUpHeroViewsPool.Free();

			var count = heroDatas.Count;
			for (int i = 0; i < count; i++)
			{
				var heroViewItem = starUpHeroViewsPool.Obtain(transformStarUpPool);
				heroViewItem.Init(heroDatas[i], this, ChoiceHeroView);
				heroViewItem.ActiveArrow(false);
				heroViewItem.SetActive(true);
			}
			ChoiceHeroView(starUpHeroViewsPool[0]);
		}

		private void ChoiceHeroView(HeroView _heroView)
		{
			heroView = _heroView;
			var count = starUpHeroViewsPool.Count;
			for (int i = 0; i < count; i++)
			{
				starUpHeroViewsPool[i].UnChoice();
			}

			ShowHero();
		}

		private void ShowHero()
		{
			var heroData = heroView.heroData;

			if (selectHeroModel != heroData.baseId - 1)
			{
				if (selectHeroModel != -1) modelHeroes[selectHeroModel].SetActive(false);
				selectHeroModel = heroData.baseId - 1;
				modelHeroes[selectHeroModel].SetActive(true);
			}

			heroView.Choice();

			txtHPOld.text = heroData.HPTotal.ToString("0") + "";
			txtDamageOld.text = heroData.DamageTotal.ToString("0") + "";
			txtArmorOld.text = heroData.ArmorTotal.ToString("0") + "";

			var maxStars = heroData.GetMaxStars();
			var star = heroData.star;
			var count = imgStarsOld.Length;
			for (int i = 0; i < star; i++)
			{
				imgStarsOld[i].SetActive(true);
				imgStarsOld[i].sprite = imgStarOn;
			}
			for (int i = star; i < maxStars; i++)
			{
				imgStarsOld[i].SetActive(true);
				imgStarsOld[i].sprite = imgStarOff;
			}
			for (int i = maxStars; i < count; i++)
			{
				imgStarsOld[i].SetActive(false);
			}

			//element
			if (heroData.IsMaxStarUp())
			{
				txtHPNew.text = "MAX";
				txtDamageNew.text = "MAX";
				txtArmorNew.text = "MAX";

				for (int i = 0; i < count; i++)
				{
					imgStarsNew[i].SetActive(false);
				}
				elementCost = 0;

				txtElementCost.transform.parent.SetActive(false);

				btnStarUp.SetEnable(false);
			}
			else
			{
				txtHPNew.text = heroData.HPTotalNextStar.ToString("0") + "";
				txtDamageNew.text = heroData.DamageTotalNextStar.ToString("0") + "";
				txtArmorNew.text = heroData.ArmorTotalNextStar.ToString("0") + "";

				star = heroData.star + 1;
				for (int i = 0; i < star; i++)
				{
					imgStarsNew[i].SetActive(true);
					imgStarsNew[i].sprite = imgStarOn;
				}
				for (int i = star; i < maxStars; i++)
				{
					imgStarsNew[i].SetActive(true);
					imgStarsNew[i].sprite = imgStarOff;
				}
				for (int i = maxStars; i < count; i++)
				{
					imgStarsNew[i].SetActive(false);
				}

				elementCost = heroData.GetHeroStarUpCost();
				currencyId = CurrenciesGroup.GetCurrencyIDFromElementID(heroData.Element);
				var currency = GameData.Instance.CurrenciesGroup.GetValue(currencyId);

				txtElementCost.transform.parent.SetActive(true);
				txtElementCost.text = Config.CurrencyAndCostToKKKString(currency, elementCost);
				imgElementCost.sprite = AssetsCollection.instance.GetElementIcon(heroData.Element);

				btnStarUp.SetEnable(true);
			}
		}

		private void BtnStarUp_Pressed()
		{
			var heroData = heroView.heroData;
			var currenciesGroup = GameData.Instance.CurrenciesGroup;
			if (!currenciesGroup.CanPay(currencyId, elementCost))
			{
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_23));
				//MainPanel.instance.ShowWarningPopup("Not enough element core");
				return;
			}

			currenciesGroup.Pay(currencyId, elementCost);
			heroData.StarUp();

			SoundManager.Instance.PlaySFX(IDs.SOUND_UP_STAR);

			heroView.Refresh();

			//fx
			ShowHero();
		}

		#endregion

		#region Disassemble

		private void RefreshDisassemble()
		{
			listDisassembleHeroDatas = new List<HeroData>();
			choiceHeroViewsPool.Free();
			disassembleHeroViewsPool.Free();

			var heroes = GameData.Instance.HeroesGroup.GetAllHeroDatasUnEquip();
			var count = heroes.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				var heroView = disassembleHeroViewsPool.Obtain(transformDisassemblePool);
				heroView.Init(heroes[i], this, ChoiceDisassembleheroView);
				heroView.ActiveArrow(false);
				heroView.SetActive(true);
			}

			ShowListHeroDisassembles();
		}

		private void ChoiceDisassembleheroView(HeroView heroView, bool isAdd)
		{
			if (isAdd) listDisassembleHeroDatas.Add(heroView.heroData);
			else listDisassembleHeroDatas.Remove(heroView.heroData);

			ShowListHeroDisassembles();
		}

		private void ShowListHeroDisassembles()
		{
			choiceHeroViewsPool.Free();

			var count = listDisassembleHeroDatas.Count;
			for (int i = 0; i < count; i++)
			{
				var heroView = choiceHeroViewsPool.Obtain(transformChoicePool);
				heroView.Init(listDisassembleHeroDatas[i], this);
				heroView.ActiveArrow(false);
				heroView.SetActive(true);
			}

			var rewardInfos = GameData.Instance.HeroesGroup.GetRewardFromDisassembleHeroes(listDisassembleHeroDatas);
			// if(rewardInfos.Count <= 0) //hiển thị 0 0
			// {
			//     rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_MATERIAL, 0));
			//     //coin reward
			//     rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, 0));
			// }

			rewardViewsPool.Free();
			count = rewardInfos.Count;
			for (int i = 0; i < count; i++)
			{
				var rewardView = rewardViewsPool.Obtain(transformRewardViewPool);
				rewardView.Init(rewardInfos[i]);
			//	heroView.ActiveArrow(false);
				rewardView.SetActive(true);
			}
		}

		private void BtnDisassemble_Pressed()
		{
			var rewardInfos = GameData.Instance.HeroesGroup.DisassembleHeroes(listDisassembleHeroDatas);
			LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_DISASSEMBLE_HERO);

			RefreshStarUp();
			listDisassembleHeroDatas.Clear();
			RefreshDisassemble();
		}

		private void BtnSmart_Pressed()
		{
			groupModel.SetActive(false);
			layoutPrecedeModel.overrideSorting = false;

			smartPanel.SetActive(true);

			ShowRewardDisassemble();
		}

		private void BtnCloseSmartPanel_Pressed()
		{
			groupModel.SetActive(true);
			layoutPrecedeModel.overrideSorting = true;

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
			listSmartHeroesDisassemble = new List<HeroData>();
			var heroDatasUnEquip = GameData.Instance.HeroesGroup.GetAllHeroDatasUnEquip();
			var count = smartRanks.Count;
			for (int i = 0; i < count; i++)
			{
				var rank = smartRanks[i];
				var countHeros = heroDatasUnEquip.Count;
				//tìm trong list gear chưa equip cái nào có rank đã chọn thì cho vào list phân rã rồi remove đi cho nhẹ vòng lặp
				for (int j = countHeros - 1; j >= 0; j--)
				{
					var heroData = heroDatasUnEquip[j];
					if (heroData.Rank == rank)
					{
						listSmartHeroesDisassemble.Add(heroData);
						heroDatasUnEquip.RemoveAt(j);
					}
				}
			}

			var rewardInfos = GameData.Instance.HeroesGroup.GetRewardFromDisassembleHeroes(listSmartHeroesDisassemble);
			// if (rewardInfos.Count <= 0) //hiển thị 0 0
			// {
			//     rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_MATERIAL, 0));
			//     //coin reward
			//     rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, 0));
			// }

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
			var rewardInfos = GameData.Instance.HeroesGroup.DisassembleHeroes(listSmartHeroesDisassemble);
			LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_DISASSEMBLE_HERO);

			RefreshStarUp();
			listSmartHeroesDisassemble.Clear();
			RefreshDisassemble();
			ShowRewardDisassemble();
		}

		#endregion

		//noti
		private void ShowNoti()
		{
			ShowStarUpNoti();
		}

		private void OnCurrencyChanged(CurrencyChangedEvent e)
		{
			if (e.id == IDs.CURRENCY_DUST_LAVA
					 || e.id == IDs.CURRENCY_DUST_METALIC
					 || e.id == IDs.CURRENCY_DUST_ELECTRIC
					 || e.id == IDs.CURRENCY_DUST_NITROGEN)
			{
				ShowStarUpNoti();
			}
		}

		private void OnHeroStarUp(HeroStarUpEvent e)
		{
			ShowStarUpNoti();
		}

		private void ShowStarUpNoti()
		{
			if (!togStarUp.isOn)
			{
				var unlocked = GameData.Instance.LevelUnlockContent >= Constants.UNLOCK_STAR_UP_HERO;
				imgStarUpNoti.SetActive(unlocked && GameData.Instance.HeroesGroup.CheckHeroStarUpNoti());
			}
			else
			{
				imgStarUpNoti.SetActive(false);
			}
		}
	}
}
