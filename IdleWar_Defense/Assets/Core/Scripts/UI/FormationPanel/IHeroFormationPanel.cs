using FoodZombie;
using FoodZombie.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IHeroFormationPanel : MyGamesBasePanel, IHerosInListData
{
	[Header("Formation Setup")]
	public GameObject FormationSetupPanel;
	public HerosInListScrollerController HerosInListScrollerController;
	public Button btnFormationAuto;
	protected int currentFormationSlotIndexSelect;
	protected HeroesGroup heroesGroup => GameData.Instance.HeroesGroup;
	public virtual void ShowCurrentFormation()
	{ }
	#region FormationSetup
	#region Interface
	List<HeroData> _allHeroes;
	List<HeroData> _allHeroesSelect;
	List<HeroData> _allHeroToView = new List<HeroData>();
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
			//_allHeroToView.Clear();
			//foreach (HeroData item in allHeroes)
			//{
			//	if (!allHeroesSelect.Contains(item))
			//	{
			//		_allHeroToView.Add(item);
			//	}
			//}
			//return _allHeroToView;
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
	protected int currentFormationIndex = 0;
	protected HeroData[][] formationsDatas;

	public void FormationInit()
	{
		formationsDatas = new HeroData[HeroesGroup.MAX_FORMATION][];
		for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
		{
			formationsDatas[i] = heroesGroup.GetEquippedHeroes(i);
		}
		currentFormationIndex = heroesGroup.CurrentFormation;
		allHeroes = heroesGroup.GetAllHeroDatas();
		ShowListHeroes();
	}
	private void ShowListHeroes()
	{
		//  allHeroes = heroesGroup.GetAllHeroDatas();
		//=====================
		//Update allHerosSelect:
		this.allHeroesSelect.Clear();
		var formation = formationsDatas[currentFormationIndex];
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
		if (HerosInListScrollerController.gameObject.activeInHierarchy)
			HerosInListScrollerController.Init(this, this, ChoiceHeroView);
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
		HeroData[] formation = formationsDatas[currentFormationIndex];
		HeroData heroData = null;
		int limit = Constants.FORMATION_SLOT_NUMBER;
		if (heroView != null)
			heroData = heroView.heroData;
		else
			heroData = heroDataInput;
		if (heroData.IsEquipped(currentFormationIndex))
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
					//ShowTotalPower();
					//teamMemberSlots[i].ShowHero();
					UpdateHeroTOP();
					ShowCurrentFormation();
					//	FormationSetupPanel.SetActive(false);
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
					//ShowTotalPower();
					//teamMemberSlots[i].ShowHero(formation[i]);
					//if (heroView != null)
					//{
					//	if (heroView.isTOPHero)
					//		teamMemberSlots[i].ShowArrowDown(false);
					//	else
					//		teamMemberSlots[i].ShowArrowDown(true);
					//}
					UpdateHeroTOP();
					ShowCurrentFormation();
					//	FormationSetupPanel.SetActive(false);
					return;
				}
			}

			if (limit < 6)
            {
				if(MainGamePanel.instance)
					MainGamePanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_4));
					//MainGamePanel.instance.ShowWarningPopup("Formation is full!\nRemove one Hero for Empty Slot\nor Clear Mission Levels to get more slots in formation");
				else
					MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_4));
					//MainPanel.instance.ShowWarningPopup("Formation is full!\nRemove one Hero for Empty Slot\nor Clear Mission Levels to get more slots in formation");
			}
            else
            {
				if (MainGamePanel.instance)
					MainGamePanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_5));
					//MainGamePanel.instance.ShowWarningPopup("Formation is full!\nRemove one Hero for Empty Slot");
				else
					MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_5));
					//MainPanel.instance.ShowWarningPopup("Formation is full!\nRemove one Hero for Empty Slot");
			}
				
		}

	}

	protected void SaveFormations()
	{
		// //kiểm tra trong có ít nhất một hero trong một đội hình hay chưa
		// var hasHero = false;
		var count = formationsDatas.Length;
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
			var formation = formationsDatas[i];
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
	public void QuickEquip()
	{
		List<HeroData> cache = new List<HeroData>();
		foreach (HeroData item in allHeroesSelect)
		{
			cache.Add(item);
			//	ChoiceHeroData(item);
		}
		foreach (HeroData item in cache)
		{
			//cache.Add(item);
			ChoiceHeroData(item);
		}
		allHeroes = heroesGroup.GetAllHeroDatas();
		allHeroes = allHeroes.OrderBy(o => -o.Power).ToList();
		int count = allHeroes.Count;
		var min = Constants.FORMATION_SLOT_NUMBER;
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

		for (int i = 0; i < min; i++)
		{
			//allHeroesSelect.Add(allHeroes[i]);
			ChoiceHeroData(allHeroes[i]);
			//var heroView = listHeroViews[i];
			//ChoiceHeroView(heroView);
		}

		UpdateHeroTOP();

		SaveFormations();
		ShowCurrentFormation();
		FormationSetupPanel.gameObject.SetActive(false);
	}

	#endregion
}
