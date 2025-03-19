using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NPOI.SS.Formula.Functions;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Utilities.Pattern.UI;
using Utilities.Service.RFirebase;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace FoodZombie.UI
{
	public class HeroPanel : MyGamesBasePanel
	{
		public GameObject tabStats;
		public GameObject tabGears;

		public SimpleTMPButton btnNextScroll;
		public SimpleTMPButton btnPreScroll;

		public TextMeshProUGUI txtName;
		public TextMeshProUGUI txtLevel;
		public TextMeshProUGUI txtPower;

		public Toggle togStats;
		public Toggle togGears;
		public GameObject imgGearsNoti;
		public GameObject[] imgGearSlotNotis;

		#region info

		public Image[] imgStars;
		public Sprite imgStarOn, imgStarOff;
		public Image[] imgRanks;
		public Image imgElement;
		public Image[] imgRankBgs;

		public TextMeshProUGUI txtHP;
		public TextMeshProUGUI txtDamage;
		public TextMeshProUGUI txtArmor;
		// public TextMeshProUGUI txtAttackSpeed;
		// public TextMeshProUGUI txtAttackRange;
		// public TextMeshProUGUI txtAccuracy;
		// public TextMeshProUGUI txtDodge;
		// public TextMeshProUGUI txtCritRate;
		// public TextMeshProUGUI txtCritDamage;
		//
		public TextMeshProUGUI txtGearHP;
		public TextMeshProUGUI txtGearDamage;
		public TextMeshProUGUI txtGearArmor;
		// public TextMeshProUGUI txtGearAttackSpeed;
		// public TextMeshProUGUI txtGearAttackRange;
		// public TextMeshProUGUI txtGearAccuracy;
		// public TextMeshProUGUI txtGearDodge;
		// public TextMeshProUGUI txtGearCritRate;
		// public TextMeshProUGUI txtGearCritDamage;

		public TextMeshProUGUI txtLevelUpCoinCost;
		public TextMeshProUGUI txtLevelUpExpHeroCost;

		public HeroSkillView[] heroSkillViews;
		public TextMeshProUGUI txtCooldown;

		#endregion

		public GameObject groupModel;
		private int selectHeroModel = -1;
		public GameObject[] modelHeroes;
		public Canvas layoutPrecedeModel;
		public CurrencyView coinView;
		public CurrencyView gemView;
		public CurrencyView expHeroView;

		public SimpleTMPButton btnFormulaBook;
		public SimpleTMPButton btnEvolution;
		public GameObject imgEvolutionNoti;
		public SimpleTMPButton btnLevelUp;
		public GameObject imgLevelUpNoti;

		public SimpleTMPButton btnUnEquipAll;
		public SimpleTMPButton btnOneClickEquip;
		public GameObject imgOneClickEquipNoti;

		public Transform posFxLevelUp;
		[SerializeField, Tooltip("Buildin Pool")] private List<ParticleMan> fxLevelUpsPool;
		public Transform posFxEvolution;
		[SerializeField, Tooltip("Buildin Pool")] private List<ParticleMan> fxEvolutionsPool;


		public List<HeroData> heroDatas;
		public HeroData heroDataView=null;
		public HeroView heroView;
		public ScrollViewHero mScrollViewHero;


		public ScrollRect scroll;

		public HeroGearSlot[] heroGearSlots;
		private int slotIndex;

		[Separator("Bot-Center")]
		public SimpleTMPButton btnMain;
		public SimpleTMPButton btnFormation;
		public GameObject imgFormationNoti;
		public SimpleTMPButton btnInventory;
		public GameObject imgInventoryNoti;
		public SimpleTMPButton btnBase;
		public GameObject imgBaseNoti;
		public Image imgBasePicture;
		public UnityEngine.UI.Text txtBaseUnlock;

		private GearsGroup GearsGroup => GameData.Instance.GearsGroup;
		public bool btnLevelUpHold = false;
		private float timeBtnLevelUpHold = 0f; //duration gọi hàm LevelUp khi giữ nút btnLevelUp
		private int countGainBtnLevelUpHold = 0; //dùng để tăng nhanh duaration


		private void Start()
		{
			btnNextScroll.onClick.AddListener(BtnNextScroll_Pressed);
			btnPreScroll.onClick.AddListener(BtnPreScroll_Pressed);

			btnFormulaBook.onClick.AddListener(BtnFormulaBook_Pressed);
			btnEvolution.onClick.AddListener(BtnEvolution_Pressed);
			// btnLevelUp.onClick.AddListener(BtnLevelUp_Pressed);
			btnLevelUp.onPointerUp += BtnLevelUp_Up;
			btnLevelUp.onPointerDown += BtnLevelUp_Down;

			togStats.onValueChanged.AddListener(TogStats_Changed);
			togGears.onValueChanged.AddListener(TogGears_Changed);

			btnUnEquipAll.onClick.AddListener(BtnUnEquipAll_Pressed);
			btnOneClickEquip.onClick.AddListener(BtnOneClickEquip_Pressed);

			//bot-center
			btnMain.onClick.AddListener(BtnMain_Pressed);
			btnFormation.onClick.AddListener(BtnFormation_Pressed);
			btnBase.onClick.AddListener(BtnBase_Pressed);
			btnInventory.onClick.AddListener((BtnInventory_Pressed));

			EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
			EventDispatcher.AddListener<FormationChangeEvent>(OnFormationChange);
			EventDispatcher.AddListener<HeroFragmentChangeEvent>(OnHeroFragmentChange);
			EventDispatcher.AddListener<BaseLevelUpEvent>(OnBaseLevelUp);

			MainPanel.instance.onAnyChildHide += OnMainPanelChildHide;
			MainPanel.instance.onAnyChildShow += OnMainPanelChildShow;
		}

		private void OnDestroy()
		{
			EventDispatcher.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
			EventDispatcher.RemoveListener<FormationChangeEvent>(OnFormationChange);
			EventDispatcher.RemoveListener<HeroFragmentChangeEvent>(OnHeroFragmentChange);
			EventDispatcher.RemoveListener<BaseLevelUpEvent>(OnBaseLevelUp);

			MainPanel.instance.onAnyChildHide -= OnMainPanelChildHide;
			MainPanel.instance.onAnyChildShow -= OnMainPanelChildShow;
		}

		private void Update()
		{
			if (scroll.horizontalNormalizedPosition >= 1f)
			{
				btnPreScroll.SetEnable(true);
				btnNextScroll.SetEnable(false);
			}
			else if (scroll.horizontalNormalizedPosition <= 0f)
			{
				btnPreScroll.SetEnable(false);				
				//btnNextScroll.SetEnable(true);
				btnNextScroll.SetEnable(mScrollViewHero.heroDatas.Count > 5);
			}
			else
			{
				btnPreScroll.SetEnable(true);
				//btnNextScroll.SetEnable(true);
				btnNextScroll.SetEnable(mScrollViewHero.heroDatas.Count > 5);
			}
		}

        internal override void Init()
		{
			coinView.Init(IDs.CURRENCY_COIN);
			gemView.Init(IDs.CURRENCY_GEM);
			expHeroView.Init(IDs.CURRENCY_EXP_HERO);

			InitBtns();

			fxLevelUpsPool.Free();
			fxEvolutionsPool.Free();

			//if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.LEVELUP_HERO))
			//{
			//	EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.LEVELUP_HERO));
			//}
		}

		private void InitBtns()
		{
			ShowFormationBtn();
			ShowBaseBtn();
			ShowInventoryBtn();
		}

		private void FixedUpdate()
		{
			if (btnLevelUpHold)
			{
				if (timeBtnLevelUpHold <= 0f)
				{
					BtnLevelUp_Pressed();
					timeBtnLevelUpHold = 0.4f - countGainBtnLevelUpHold * 0.1f; // max time
					if (timeBtnLevelUpHold <= 0.1f) timeBtnLevelUpHold = 0.1f; //min time
					countGainBtnLevelUpHold++;
				}

				timeBtnLevelUpHold -= Time.fixedDeltaTime;
			}
		}


		private void OnMainPanelChildHide(PanelController pPanel)
		{
			if (MainPanel.instance.TopPanel is HeroPanel)
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
			if (MainPanel.instance.TopPanel is HeroPanel)
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

		private void ShowHero()
		{
			var heroData = heroDataView;

			if (selectHeroModel != heroData.baseId - 1)
			{
				if (selectHeroModel != -1) modelHeroes[selectHeroModel].SetActive(false);
				selectHeroModel = heroData.baseId - 1;
				modelHeroes[selectHeroModel].SetActive(true);
			}

			txtName.text = heroData.Name;
			txtLevel.text = "" + heroData.levelUnlocked + "";

			var maxStars = heroData.GetMaxStars();
			var star = heroData.star;
			var count = imgStars.Length;
			for (int i = 0; i < star; i++)
			{
				imgStars[i].SetActive(true);
				imgStars[i].sprite = imgStarOn;
			}
			for (int i = star; i < maxStars; i++)
			{
				imgStars[i].SetActive(true);
				imgStars[i].sprite = imgStarOff;
			}
			for (int i = maxStars; i < count; i++)
			{
				imgStars[i].SetActive(false);
			}

			//element
			imgElement.sprite = heroData.GetElementIcon();

			//rank
			count = imgRanks.Length;
			for (int i = 0; i < count; i++)
			{
				imgRanks[i].SetActive(false);
				imgRankBgs[i].SetActive(false);
			}
			imgRanks[heroData.Rank - 1].SetActive(true);
			imgRankBgs[heroData.Rank - 1].SetActive(true);

			if (heroData.IsMaxLevelUp())
			{
				txtLevelUpCoinCost.text = "Max Level";
				txtLevelUpExpHeroCost.text = "Max Level";
				btnLevelUp.SetEnable(false);
			}
			else
			{
				var heroLevelUpCost = heroData.GetHeroLevelUpCost();
				txtLevelUpCoinCost.text = Config.CurrencyAndCostToKKKString(GameData.Instance.CurrenciesGroup.GetCoin(), heroLevelUpCost.gold);
				txtLevelUpExpHeroCost.text = Config.CurrencyAndCostToKKKString(GameData.Instance.CurrenciesGroup.GetExpHero(), heroLevelUpCost.exp);
				btnLevelUp.SetEnable(true);
			}

			if (togStats.isOn) ShowStats();
			else ShowGears();

			//skills
			var heroBaseId = heroData.baseId;
			var heroSkillView = heroSkillViews[0];
			heroSkillView.SetActive(true);
			heroSkillView.Init(HeroSkillView.SKILL_TYPE_ACTIVE, AssetsCollection.instance.GetHeroSkillIcon(heroBaseId, 0), heroData.SkillName, heroData.SkillDescription, heroData.Cooldown + "s cooldown");
			txtCooldown.text = heroData.Cooldown.ToString("0") + "s";
			//PassiveSkill1
			var skillInfo = heroData.PassiveSkill1Description;
			if (skillInfo != null && !skillInfo.Equals(string.Empty))
			{
				heroSkillView = heroSkillViews[1];
				heroSkillView.Init(HeroSkillView.SKILL_TYPE_PASSIVE, AssetsCollection.instance.GetHeroSkillIcon(heroBaseId, 1), heroData.PassiveSkill1Name, "<color=#3EA43B>" + heroData.PassiveSkill1Description + "</color>");
			}
			else
			{
				heroSkillViews[1].Init();
			}
			//PassiveSkill2
			skillInfo = heroData.PassiveSkill2Description;
			if (skillInfo != null && !skillInfo.Equals(string.Empty))
			{
				heroSkillView = heroSkillViews[2];
				heroSkillView.Init(HeroSkillView.SKILL_TYPE_PASSIVE, AssetsCollection.instance.GetHeroSkillIcon(heroBaseId, 2), heroData.PassiveSkill2Name, "<color=#3EA43B>" + heroData.PassiveSkill2Description + "</color>");
			}
			else
			{
				heroSkillViews[2].Init();
			}
			//SpecialSkill
			skillInfo = heroData.SpecialSkillDescription;
			if (skillInfo != null && !skillInfo.Equals(string.Empty))
			{
				heroSkillView = heroSkillViews[3];
				heroSkillView.Init(HeroSkillView.SKILL_TYPE_PASSIVE, AssetsCollection.instance.GetHeroSkillIcon(heroBaseId, 3), heroData.SpecialSkillName, heroData.SpecialSkillDescription);
			}
			else
			{
				heroSkillViews[3].Init();
			}

			// //tut
			// if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.EQUIP_GEAR) && heroData.levelUnlocked >= 5)
			// {
			//     EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.EQUIP_GEAR));
			// }

			//noti
			imgLevelUpNoti.SetActive(heroData.CanLevelUp());

			//evolution
			var heroEvolutionInfos = GameData.Instance.HeroesGroup.GetAllHeroEvolutionInfos();
			var countHeroEvolutionInfos = heroEvolutionInfos.Count;
			bool inListEvolution = false;
			for (int i = 0; i < countHeroEvolutionInfos; i++)
			{
				var heroEvolutionInfo = heroEvolutionInfos[i];
				if (heroEvolutionInfo.idMaterial_1 == heroData.baseId)
				{
					inListEvolution = true;
					//checkEvolutionNoti
					// imgEvolutionNoti.SetActive(heroData.CanEvolution());
					break;
				}
			}

			btnEvolution.SetEnable(inListEvolution);
		}

		public void ChoiceHeroView(HeroView _heroView)
		{
			heroDataView = _heroView.heroData;

			for (int i = 0; i < mScrollViewHero.listHeroView.Count; i++)
            {
				var _hero = mScrollViewHero.listHeroView[i];
				if (_hero.heroData.id== heroDataView.id)
					_hero.Choice();
				else
					_hero.UnChoice();

			}
			

			ShowHero();
		}


		private void BtnNextScroll_Pressed()
		{
			mScrollViewHero.Action_NextScrollView();
        }

		private void BtnPreScroll_Pressed()
		{
			mScrollViewHero.Action_PreScrollView();
		}

		private void BtnFormulaBook_Pressed()
		{
			MainPanel.instance.ShowFormulaBookPanel(heroDataView);
		}

		private void BtnEvolution_Pressed()
		{
			MainPanel.instance.ShowEvolutionHeroPanel(heroDataView, EvolutionSuccess);
		}

		private void EvolutionSuccess(HeroData heroData)
		{
			ShowHero();

			StartCoroutine(IEFxEvolution());
			Config.LogEvent(TrackingConstants.CLICK_HEROES_STARUP_PASS);
		}

		private IEnumerator IEFxEvolution()
		{
			yield return null;

			var fxEvolution = fxEvolutionsPool.Obtain(posFxEvolution);
			fxEvolution.SetActive(true);
			fxEvolution.Play();
		}

		private void BtnLevelUp_Pressed()
		{
			
			if (heroDataView.IsMaxLevelUp())
			{
				btnLevelUpHold = false;
				return;
			}

			//nếu chưa xong tut thì up lên level user = 5
			if (TutorialsManager.tutorialActive && !TutorialsManager.Instance.IsCompleted(TutorialsGroup.LEVELUP_HERO)
				&& heroDataView.levelUnlocked >= 5)
			{
				return;
			}

			Config.LogEvent(TrackingConstants.CLICK_HEROES_LEVELUP);

			var heroLevelUpCost = heroDataView.GetHeroLevelUpCost();
			var coin = heroLevelUpCost.gold;
			var expHero = heroLevelUpCost.exp;
			var currenciesGroup = GameData.Instance.CurrenciesGroup;
			if (!currenciesGroup.CanPay(IDs.CURRENCY_COIN, coin))
			{
				//MainPanel.instance.ShowWarningPopup("Not enough coin");
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_9));
				BtnLevelUp_Up();
				return;
			}
			if (!currenciesGroup.CanPay(IDs.CURRENCY_EXP_HERO, expHero))
			{
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_16));
				//MainPanel.instance.ShowWarningPopup("Not enough exp hero");
				BtnLevelUp_Up();
				return;
			}
			// if (heroData.levelUnlocked >= GameData.Instance.UserGroup.Level)
			// {
			//     MainPanel.instance.ShowWarningPopup("Hero level is limited by the level of user");
			//     BtnLevelUp_Up();
			//     return;
			// }

			currenciesGroup.Pay(IDs.CURRENCY_COIN, coin, TrackingConstants.VALUE_LEVEL_UP_HERO);
			currenciesGroup.Pay(IDs.CURRENCY_EXP_HERO, expHero);
			heroDataView.LevelUp();

			SoundManager.Instance.PlaySFX(IDs.SOUND_UP_STAR);

			heroView= mScrollViewHero.listHeroView.Find(x => x.heroData.id == heroDataView.id);
			if(heroView!=null)
				heroView.Refresh();

			ShowHero();

			var fxLevelUp = fxLevelUpsPool.Obtain(posFxLevelUp);
			fxLevelUp.SetActive(true);
			fxLevelUp.Play();

			Config.LogEvent(TrackingConstants.CLICK_HEROES_LEVELUP_PASS);
		}

		private void BtnLevelUp_Up()
		{
			btnLevelUpHold = false;
			timeBtnLevelUpHold = 0f; //resetTime
			countGainBtnLevelUpHold = 0;
		}

		private void BtnLevelUp_Down()
		{
			btnLevelUpHold = true;
		}

		private void TogStats_Changed(bool value)
		{
			if (value)
			{
				ShowStats();
			}
		}

		private void TogGears_Changed(bool value)
		{
			if (value)
			{
				if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_GEAR_EQUIP)
				{
					//MainPanel.instance.ShowWarningPopup("Unlock " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_GEAR_EQUIP));
					MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.UNLOCK)+" " + GameData.Instance.LevelUnlockContentText(Constants.UNLOCK_GEAR_EQUIP));
					togStats.isOn = true;
					return;
				}

				ShowGears();
				Config.LogEvent(TrackingConstants.CLICK_HEROES_GEAR);
			}
		}

		private void ShowStats()
		{
			var heroData = heroDataView;

			tabStats.SetActive(true);
			tabGears.SetActive(false);

			txtPower.text = "" + heroData.Power + "";

			txtHP.text = heroData.HP.ToString("0") + "";
			txtDamage.text = heroData.Damage.ToString("0") + "";
			txtArmor.text = heroData.Armor.ToString("0") + "";
			// txtAttackSpeed.text = (heroData.AttackSpeed * 10f).ToString("0");
			// txtAttackRange.text = heroData.AttackRange + "";
			// txtAccuracy.text = heroData.Accuracy + "";
			// txtDodge.text = heroData.Dodge + "";
			// txtCritRate.text = heroData.CritRate + "";
			// txtCritDamage.text = heroData.CritDamage + "";
			//
			if (heroData.HPGear > 0f) txtGearHP.text = "+" + heroData.HPGear + "";
			else txtGearHP.text = "";
			if (heroData.DamageGear > 0f) txtGearDamage.text = "+" + heroData.DamageGear + "";
			else txtGearDamage.text = "";
			if (heroData.ArmorGear > 0f) txtGearArmor.text = "+" + heroData.ArmorGear + "";
			else txtGearArmor.text = "";
			// txtGearAttackSpeed.text = (heroData.AttackSpeedGear * 10f).ToString("0");
			// txtGearAttackRange.text = heroData.AttackRangeGear + "";
			// txtGearAccuracy.text = heroData.AccuracyGear + "";
			// txtGearDodge.text = heroData.DodgeGear + "";
			// txtGearCritRate.text = heroData.CritRateGear + "";
			// txtGearCritDamage.text = heroData.CritDamageGear + "";

			imgGearsNoti.SetActive(heroData.CanOneClickEquip());
			// imgOneClickEquipNoti.SetActive(false);
		}

		private void ShowGears()
		{
			tabStats.SetActive(false);
			tabGears.SetActive(true);

			var heroData = heroDataView;
			var count = heroGearSlots.Length;
			for (int i = 0; i < count; i++)
			{
				var gearData = heroData.GetEquippedGear(i + 1);
				heroGearSlots[i].Init(gearData, ChoiceGearSlot, i);
				imgGearSlotNotis[i].SetActive(heroData.CanOneClickEquip(i + 1));
			}

			imgGearsNoti.SetActive(false);
			// imgOneClickEquipNoti.SetActive(heroView.heroData.CanOneClickEquip());
		}

		private void ChoiceGearSlot(HeroGearSlot heroGearSlot)
		{
			if (heroGearSlot.gearData != null)
			{
				ShowGearDetailPanel(heroGearSlot);
			}
			else
			{
				ShowListUnEquippedGear(heroGearSlot);
			}
		}

		private void ShowGearDetailPanel(HeroGearSlot heroGearSlot)
		{
			MainPanel.instance.ShowGearDetailPanel(heroGearSlot, UnEquipGear, ShowListUnEquippedGear);
		}

		private void ShowListUnEquippedGear(HeroGearSlot heroGearSlot)
		{
			slotIndex = heroGearSlot.slotIndex;
			var gears = GearsGroup.GetAllGearDatasUnEquip(slotIndex + 1); //Slot ID = slot index + 1
			MainPanel.instance.ShowListGearsPanel(gears, BtnChoiceUnEquippedGears_Pressed);
		}

		private void BtnChoiceUnEquippedGears_Pressed(GearData gearData)
		{
			heroDataView.EquipGear(slotIndex + 1, gearData.id);

			ShowHero();
		}

		private void UnEquipGear(HeroGearSlot heroGearSlot)
		{
			slotIndex = heroGearSlot.slotIndex;
			heroDataView.UnEquipGear(slotIndex + 1);

			ShowHero();
		}

		private void BtnUnEquipAll_Pressed()
		{
			heroDataView.UnEquipAllGear();

			ShowHero();
			Config.LogEvent(TrackingConstants.CLICK_HEROES_GEAR_UNEQUIP_ALL);
		}

		private void BtnOneClickEquip_Pressed()
		{
			var heroData = heroDataView;
			var count = heroGearSlots.Length;
			for (int i = 0; i < count; i++)
			{
				//tìm trong mỗi slot một gear có power mạnh nhất thì mặc vào
				var gearDatas = GearsGroup.GetAllGearDatasUnEquip(i + 1);
				if (gearDatas != null && gearDatas.Count > 0)
				{
					gearDatas = gearDatas.OrderBy(o => -o.Power).ToList();
					var bestGearInSlot = gearDatas[0];
					var oldGear = heroData.GetEquippedGear(i + 1);
					if (oldGear == null || bestGearInSlot.Power > oldGear.Power)
					{
						heroData.EquipGear(i + 1, bestGearInSlot.id);
					}
				}
			}

			ShowHero();

			Config.LogEvent(TrackingConstants.CLICK_HEROES_GEAR_ONE_CLICK_EQUIP);
		}

		//bot-center
		private void OnCurrencyChanged(CurrencyChangedEvent e)
		{
			if (e.id == IDs.CURRENCY_COIN)
			{
				ShowBaseBtn();
			}
			else if (e.id == IDs.CURRENCY_BLUE_HERO_FRAGMENT
					 || e.id == IDs.CURRENCY_EPIC_HERO_FRAGMENT)
			{
				ShowInventoryBtn();
			}
		}

		private void BtnMain_Pressed()
		{
			Back();
			Config.LogEvent(TrackingConstants.CLICK_MAINMENU);
		}

		private void BtnBase_Pressed()
		{
			Back();
			MainPanel.instance.ShowBasePanel();
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
		private void OnFormationChange(FormationChangeEvent e)
		{
			ShowFormationBtn();
		}

		private void ShowFormationBtn()
		{
			imgFormationNoti.SetActive(GameData.Instance.HeroesGroup.CheckFormationNoti());
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

		private void OnHeroFragmentChange(HeroFragmentChangeEvent e)
		{
			ShowInventoryBtn();
		}

		private void ShowInventoryBtn()
		{
			imgInventoryNoti.SetActive(GameData.Instance.ItemsGroup.CheckNoti());
		}

		private void OnBaseLevelUp(BaseLevelUpEvent e)
		{
			ShowBaseBtn();
		}
	}
}