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
using Utilities.Pattern.UI;
using Utilities.Service.RFirebase;
using Debug = UnityEngine.Debug;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace FoodZombie.UI
{
	public class FormationPanel : MyGamesBasePanel, IHerosInListData
	{
		public CurrencyView coinView;
		public CurrencyView gemView;
		public CurrencyView expHeroView;

		public SimpleTMPButton btnAuto;
		public SimpleTMPButton[] btnFormations;
		public GameObject[] imgFormationsActive;

		public GameObject groupModel;
		// public Canvas layoutPrecedeModel;

		public TeamMemberSlot[] teamMemberSlots;
		public HerosInListScrollerController HerosInListScrollerController;
		private HeroData heroData;

		public SimpleTMPButton btnPre;
		public SimpleTMPButton btnNext;
		public TextMeshProUGUI txtPower;

		[Separator("Bot-Center")]
		public SimpleTMPButton btnMain;
		public SimpleTMPButton btnBase;
		public GameObject imgBaseNoti;
		public Image imgBasePicture;
		public UnityEngine.UI.Text txtBaseUnlock;
		public SimpleTMPButton btnInventory;
		public GameObject imgInventoryNoti;
		public SimpleTMPButton btnHero;
		public GameObject imgHeroNoti;

		private HeroData[][] formations; //-1 -1 -1 -1 -1 -1



		private int currentFormation = 0;
		private HeroesGroup heroesGroup => GameData.Instance.HeroesGroup;
		#region Interface
		List<HeroData> _allHeroes;
		List<HeroData> _allHeroesSelect;
		int minPOWonSelect;
		public List<HeroData> allHeroes
		{
			get
			{
				if (_allHeroes == null) _allHeroes = new List<HeroData>();
				return _allHeroes;
			}
			set => _allHeroes = value;
		}
		public List<HeroData> allHeroesSelect
		{
			get
			{
				if (_allHeroesSelect == null) _allHeroesSelect = new List<HeroData>();
				return _allHeroesSelect;
			}
			set => _allHeroesSelect = value;
		}
		public List<HeroData> allHeroesToView
		{
			get
			{
				return allHeroes;
			}
		}
		public int MinPOWonSelect
		{
			get { return minPOWonSelect; }
			//	set { minPOWonSelect = value; }
		}
		public bool HaveOtherStrongerNotSelect(HeroData data)
		{
			for (int i = 0; i < allHeroes.Count; i++)
			{
				if (allHeroesSelect.Contains(allHeroes[i])) continue;
				if (allHeroes[i].Power > data.Power) return true;
			}
			return false;
			//	set { minPOWonSelect = value; }
		}
		#endregion

		private int limit;

		private void Start()
		{
			btnAuto.onClick.AddListener(BtnAuto_Pressed);
			var count = btnFormations.Length;
			for (int i = 0; i < count; i++)
			{
				int index = i;
				btnFormations[i].onClick.AddListener(() =>
				{
					BtnFormation_Pressed(index);
				});
			}

			btnPre.onClick.AddListener(BtnPre_Pressed);
			btnNext.onClick.AddListener(BtnNext_Pressed);

			//bot-center
			btnMain.onClick.AddListener(BtnMain_Pressed);
			btnBase.onClick.AddListener(BtnBase_Pressed);
			btnHero.onClick.AddListener(BtnHero_Pressed);
			btnInventory.onClick.AddListener((BtnInventory_Pressed));


			EventDispatcher.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
			EventDispatcher.AddListener<ChangeGearEvent>(OnChangeGear);
			EventDispatcher.AddListener<FormationChangeEvent>(OnFormationChange);
			EventDispatcher.AddListener<HeroStarUpEvent>(OnHeroStarUp);
			EventDispatcher.AddListener<HeroLevelUpEvent>(OnHeroLevelUp);
			EventDispatcher.AddListener<HeroEvolutionEvent>(OnHeroEvolution);
			EventDispatcher.AddListener<HeroFragmentChangeEvent>(OnHeroFragmentChange);
			EventDispatcher.AddListener<BaseLevelUpEvent>(OnBaseLevelUp);

			MainPanel.instance.onAnyChildHide += OnMainPanelChildHide;
			MainPanel.instance.onAnyChildShow += OnMainPanelChildShow;
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
			EventDispatcher.RemoveListener<BaseLevelUpEvent>(OnBaseLevelUp);

			MainPanel.instance.onAnyChildHide -= OnMainPanelChildHide;
			MainPanel.instance.onAnyChildShow -= OnMainPanelChildShow;
		}

		internal override void Init()
		{
			GameData.Instance.HeroesGroup.SetCurrentFormation(0);
			coinView.Init(IDs.CURRENCY_COIN);
			gemView.Init(IDs.CURRENCY_GEM);
			expHeroView.Init(IDs.CURRENCY_EXP_HERO);

			InitBtns();
			heroesGroup.CheckFormation();

			currentFormation = heroesGroup.CurrentFormation;

			formations = new HeroData[HeroesGroup.MAX_FORMATION][];
			for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
			{
				formations[i] = heroesGroup.GetEquippedHeroes(i);
			}
			allHeroes = heroesGroup.GetAllHeroDatas();



			UpdateHeroTOP();
			ShowFormation();
			ShowListHeroes();
		}

		public void QuickEquip()
		{
			formations = new HeroData[HeroesGroup.MAX_FORMATION][];
			for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
			{
				formations[i] = heroesGroup.GetEquippedHeroes(i);
			}
			allHeroes = heroesGroup.GetAllHeroDatas();
			UpdateHeroTOP();
			for (int i = 0; i < allHeroes.Count; i++)
			{
				if (i < formations[0].Length)
					formations[0][i] = allHeroes[i];
			}
			SaveFormations();
		}

		private void InitBtns()
		{
			ShowHeroBtn();
			ShowBaseBtn();
			ShowInventoryBtn();
		}

		private void OnMainPanelChildHide(PanelController pPanel)
		{
			if (MainPanel.instance.TopPanel is FormationPanel)
			{
				groupModel.SetActive(true);
				// layoutPrecedeModel.overrideSorting = true;
			}
			else
			{
				groupModel.SetActive(false);
				// layoutPrecedeModel.overrideSorting = false;
			}
		}

		private void OnMainPanelChildShow(PanelController pPanel)
		{
			if (MainPanel.instance.TopPanel is FormationPanel)
			{
				groupModel.SetActive(true);
				// layoutPrecedeModel.overrideSorting = true;
			}
			else
			{
				groupModel.SetActive(false);
				// layoutPrecedeModel.overrideSorting = false;
			}
		}

		private void ChangeMemberAction(TeamMemberSlot teamMemberSlot)
		{
			var teamMemberSlotId = teamMemberSlot.heroData.id;
			var formation = formations[currentFormation];

			var pos = (Vector2)teamMemberSlot.transform.position;
			var count = teamMemberSlots.Length;
			var iSlot = -1;
			var iTarget = -1;
			//lấy ra vị trí của team member đang kéo drag trong formation
			for (int i = 0; i < count; i++)
			{
				if (formation[i] != null && formation[i].id == teamMemberSlotId)
				{
					iSlot = i;
					break;
				}
			}

			for (int i = 0; i < count; i++)
			{
				var teamMemberTarget = teamMemberSlots[i];
				var posTarget = (Vector2)teamMemberTarget.posStart;
				if (pos.x < posTarget.x + 1.2f && pos.x > posTarget.x - 1.2f
					&& pos.y < posTarget.y + 1.2f && pos.y > posTarget.y - 1.2f)
				{
					//reset vị trí của team member 
					teamMemberSlot.Repos();

					if (teamMemberTarget.unlocked)
					{
						//replace 2 member với nhau
						iTarget = i;
						formation[iSlot] = teamMemberTarget.heroData;
						formation[iTarget] = teamMemberSlot.heroData;

						SaveFormations();

						teamMemberSlot.ShowHero(formation[iSlot]);
						teamMemberTarget.ShowHero(formation[iTarget]);
					}

					return;
				}
			}

			RemoveMemberAction(teamMemberSlot);
		}

		private void RemoveMemberAction(TeamMemberSlot teamMemberSlot)
		{
			teamMemberSlot.Repos();

			var teamMemberSlotId = teamMemberSlot.heroData.id;

			var count = allHeroes.Count;
			for (int i = 0; i < count; i++)
			{
				var heroData = allHeroes[i];
				if (heroData.id == teamMemberSlotId)
				{
					HerosInListScrollerController.ChoiceHero(heroData);
					//ChoiceHeroView(heroView);
				}
			}
		}
		public void ChoiceHeroData(HeroData heroData)
		{
			if (allHeroesSelect.Contains(heroData))
				allHeroesSelect.Remove(heroData);
			else
				allHeroesSelect.Add(heroData);

			ChoiceHeroView(null, heroData);
		}
		private void ChoiceHeroView(HeroView heroView)
		{
			ChoiceHeroView(heroView, null);
		}
		private void ChoiceHeroView(HeroView heroView, HeroData heroDataInput = null)
		{
			var formation = formations[currentFormation];
			if (heroView != null)
				heroData = heroView.heroData;
			else
				heroData = heroDataInput;
			if (heroData.IsEquipped(currentFormation))
			{
				for (int i = 0; i < limit; i++)
				{
					var equippedHero = formation[i];
					if (equippedHero != null && equippedHero.id == heroData.id)
					{
						if (heroView != null)
						{
							heroView.UnChoice();

						}
						if (allHeroesSelect.Contains(heroData))
						{
							allHeroesSelect.Remove(heroData);
						}
						formation[i] = null;
						SaveFormations();
						if (heroView != null)
							heroView.Refresh();
						ShowTotalPower();
						teamMemberSlots[i].ShowHero();
						UpdateHeroTOP();
						return;
					}
				}
			}
			else
			{
				for (int i = 0; i < limit; i++)
				{
					var equippedHero = formation[i];
					if (equippedHero == null)
					{
						if (heroView != null)
						{
							heroView.Choice();

						}
						if (!allHeroesSelect.Contains(heroData))
						{
							allHeroesSelect.Add(heroData);
						}
						formation[i] = heroData;
						SaveFormations();
						if (heroView != null)
							heroView.Refresh();
						ShowTotalPower();
						teamMemberSlots[i].ShowHero(formation[i]);
						if (heroView != null)
						{
							if (heroView.isTOPHero)
								teamMemberSlots[i].ShowArrowDown(false);
							else
								teamMemberSlots[i].ShowArrowDown(true);
						}
						UpdateHeroTOP();

						return;
					}
				}

				//if (limit < 6) MainPanel.instance.ShowWarningPopup("Clear Mission Levels to get more slots in formation");
				if (limit < 6) MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_14));
			}

		}

		private void BtnAuto_Pressed()
		{
			var count = teamMemberSlots.Length;
			for (int i = 0; i < count; i++)
			{
				var teamMemberSlot = teamMemberSlots[i];
				if (teamMemberSlot.heroData != null) RemoveMemberAction(teamMemberSlots[i]);
			}

			//re-oder
			allHeroes = allHeroes.OrderBy(o => -o.Power).ToList();


			//oder theo attack type
			//var listHeroViews = new List<HeroView>();
			count = allHeroes.Count;
			var min = limit;
			if (count < min) min = count;
			for (int i = 0; i < min - 1; i++)
			{
				var heroViewA = allHeroes[i];
				for (int j = i + 1; j < min; j++)
				{
					var heroViewB = allHeroes[j];
					if (!heroViewA.AttackTypes.Contains(IDs.ATTACK_TYPE_MELEE)
						&& heroViewB.AttackTypes.Contains(IDs.ATTACK_TYPE_MELEE))
					{
						HeroData temp = allHeroes[i];
						allHeroes[i] = allHeroes[j];
						allHeroes[j] = temp;
					}
				}
			}

			//}

			//for (int i = 0; i < min; i++)
			//{
			//	listHeroViews.Add(formationHeroViewsPool[i]);
			//}
			////ưu tiên vị trí đầu theo attack type
			//HeroView temp;
			//for (int i = 0; i < min - 1; i++)
			//{
			//	var heroViewA = listHeroViews[i];
			//	for (int j = i + 1; j < min; j++)
			//	{
			//		var heroViewB = listHeroViews[j];
			//		if (!heroViewA.heroData.AttackTypes.Contains(IDs.ATTACK_TYPE_MELEE)
			//			&& heroViewB.heroData.AttackTypes.Contains(IDs.ATTACK_TYPE_MELEE))
			//		{
			//			temp = listHeroViews[i];
			//			listHeroViews[i] = listHeroViews[j];
			//			listHeroViews[j] = temp;
			//		}
			//	}
			//}
			for (int i = 0; i < min; i++)
			{
				//allHeroesSelect.Add(allHeroes[i]);
				ChoiceHeroData(allHeroes[i]);
				//var heroView = listHeroViews[i];
				//ChoiceHeroView(heroView);
			}

			ShowFormation();
			ShowListHeroes();
		}

		private void SaveFormations()
		{
			// //kiểm tra trong có ít nhất một hero trong một đội hình hay chưa
			// var hasHero = false;
			var count = formations.Length;
			// for (int i = 0; i < count; i++)
			// {
			//     var formation = formations[i];
			//     var fcount = formation.Length;
			//     for (int j = 0; j < fcount; j++)
			//     {
			//         var item = formation[j];
			//         if (item != null)
			//         {
			//             hasHero = true;
			//             break;
			//             break;
			//         }
			//     }
			// }

			//có hero thì mới cho lưu
			// if (hasHero)
			// {
			var newFormations = new List<List<string>>();
			for (int i = 0; i < count; i++)
			{
				var newFormation = new List<string>();
				var formation = formations[i];
				var fCount = formation.Length;
				for (int j = 0; j < fCount; j++)
				{
					var heroData = formation[j];
					if (heroData == null) newFormation.Add("-1");
					else newFormation.Add(formation[j].id + "");
				}

				newFormations.Add(newFormation);
			}

			heroesGroup.SaveEquippedHeros(newFormations);
			// }
			// else
			// {
			//     MainPanel.instance.ShowWarningPopup("You need to have a formation with some heroes to fight.");
			// }
		}

		private void BtnFormation_Pressed(int index)
		{
			currentFormation = index;

			ShowFormation();
		}

		private void BtnPre_Pressed()
		{
			if (currentFormation > 0)
			{
				currentFormation--;
				ShowFormation();
			}
		}

		private void BtnNext_Pressed()
		{
			if (currentFormation < formations.Length - 1)
			{
				currentFormation++;
				ShowFormation();
			}
		}

		private void ShowFormation()
		{
			//tab
			btnPre.SetEnable(currentFormation > 0);
			btnNext.SetEnable(currentFormation < formations.Length - 1);

			var count = imgFormationsActive.Length;
			for (int i = 0; i < count; i++)
			{
				imgFormationsActive[i].SetActive(false);
			}
			imgFormationsActive[currentFormation].SetActive(true);

			//team member
			var formation = formations[currentFormation];
			var fcount = formation.Length;
			//  limit = GameData.Instance.BaseGroup.GetCurrentBase().slotLimit;
			limit = Constants.FORMATION_SLOT_NUMBER;
			for (int i = 0; i < limit; i++)
			{
				teamMemberSlots[i].Init(i + 1, true, ChangeMemberAction, RemoveMemberAction);
				HeroData current = formation[i];
				teamMemberSlots[i].ShowHero(current);

				//	teamMemberSlots[i].ShowArrowDown(current != null && !isTOPHero(current));
			}
			for (int i = limit; i < fcount; i++)
			{
				teamMemberSlots[i].Init(i + 1, false, ChangeMemberAction, RemoveMemberAction);
				teamMemberSlots[i].ShowHero(formation[i]);
				//	teamMemberSlots[i].ShowArrowDown(false);
			}
			ShowTotalPower();
		}

		private void ShowListHeroes()
		{
			//  allHeroes = heroesGroup.GetAllHeroDatas();
			//=====================
			//Update allHerosSelect:
			this.allHeroesSelect.Clear();
			var formation = formations[currentFormation];
			var fcount = formation.Length;
			//	formationHeroViewsPool.Free();
			var count = allHeroes.Count;
			// int maxHeroOnFormation = Constants.FORMATION_SLOT_NUMBER;
			for (int i = 0; i < count; i++)
			{
				//	var heroView = formationHeroViewsPool.Obtain(transformPool);
				HeroData currentHeroData = allHeroes[i];

				//	bool equiped = false;
				//check xem formation hiện tại có chứa heroData này không
				for (int j = 0; j < fcount; j++)
				{
					HeroData equipHeroData = formation[j];
					if (equipHeroData != null && equipHeroData.id == currentHeroData.id)
					{
						//equiped = true;
						this.allHeroesSelect.Add(equipHeroData);
						break;
					}
				}
				//heroView.isTOPHero = allHeroesTOP.Contains(heroData);
				//heroView.Init(heroData, this, ChoiceHeroView, equiped);
				//heroView.SetActive(true);

			}
			//Init List:
			UpdateHeroTOP();
			HerosInListScrollerController.Init(this, this, ChoiceHeroView);
		}

		internal override void Back()
		{
			//có hero thì mới cho lưu
			if (CheckHasHero())
			{
				base.Back();
				Config.LogEvent(TrackingConstants.CLICK_MAINMENU);
			}
			else
			{
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_15));
				//MainPanel.instance.ShowWarningPopup("You need to have a formation with some heroes to fight.");
			}
		}

		//bot-center
		private void BtnMain_Pressed()
		{
			//có hero thì mới cho lưu
			if (CheckHasHero())
			{
				base.Back();
				Config.LogEvent(TrackingConstants.CLICK_MAINMENU);
			}
			else
			{
				//MainPanel.instance.ShowWarningPopup("You need to have a formation with some heroes to fight.");
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_15));
			}
		}

		private void BtnHero_Pressed()
		{
			//có hero thì mới cho lưu
			if (CheckHasHero())
			{
				base.Back();
				MainPanel.instance.ShowHeroPanel();
			}
			else
			{
				//MainPanel.instance.ShowWarningPopup("You need to have a formation with some heroes to fight.");
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_15));
			}
		}

		private void BtnInventory_Pressed()
		{
			//có hero thì mới cho lưu
			if (CheckHasHero())
			{
				base.Back();
				MainPanel.instance.ShowInventoryPanel();
			}
			else
			{
				//MainPanel.instance.ShowWarningPopup("You need to have a formation with some heroes to fight.");
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_15));
			}
		}

		private void BtnBase_Pressed()
		{
			//có hero thì mới cho lưu
			if (CheckHasHero())
			{
				base.Back();
				MainPanel.instance.ShowBasePanel();
			}
			else
			{
				//MainPanel.instance.ShowWarningPopup("You need to have a formation with some heroes to fight.");
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_15));
			}
		}

		private bool CheckHasHero()
		{
			//kiểm tra trong có ít nhất một hero trong một đội hình hay chưa
			var hasHero = false;
			var count = formations.Length;
			for (int i = 0; i < 1; i++)
			{
				var formation = formations[i];
				var fcount = formation.Length;
				for (int j = 0; j < fcount; j++)
				{
					var item = formation[j];
					if (item != null)
					{
						hasHero = true;
						break;
						break;
					}
				}
			}

			return hasHero;
		}
		private void UpdateHeroTOP()
		{
			this.minPOWonSelect = int.MaxValue;
			if (allHeroesSelect == null || allHeroesSelect.Count <= 0)
			{
				this.HerosInListScrollerController.Reload();
				return;
			}

			//int minIndex = -1;
			for (int i = 0; i < allHeroesSelect.Count; i++)
			{
				if (allHeroesSelect[i].Power < this.minPOWonSelect)
				{
					this.minPOWonSelect = allHeroesSelect[i].Power;
					//	minIndex = i;
				}
			}

			this.HerosInListScrollerController.Reload();
			//List<HeroData> _temp = allHeroes.OrderBy(o => -o.Power).ToList();
			//limit = Constants.FORMATION_SLOT_NUMBER;
			//for (int i = 0; i < limit; i++)
			//{
			//	if (i < _temp.Count)
			//		allHeroesTOP.Add(_temp[i]);
			//	else
			//		break;
			//}
		}
		private void ShowTotalPower()
		{
			float total = 0f;
			var formation = formations[currentFormation];
			var count = formation.Length;
			for (int i = 0; i < count; i++)
			{
				if (formation[i] != null) total += formation[i].Power;
			}
			txtPower.text = total + "";
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
			ShowHeroBtn();
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
				txtBaseUnlock.text = Localization.Get(Localization.ID.CLEAR).ToUpper()+"\n" + GameData.Instance.MissionLevelToString(Constants.UNLOCK_BASE);
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