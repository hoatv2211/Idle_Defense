using EnhancedUI.EnhancedScroller;
using FoodZombie;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Pattern.UI;

public class HerosInListView : EnhancedScrollerCellView
{
	HerosInListScrollerController Controller;
	PanelController panel;
	UnityAction<HeroView> choiceAction;
	public HeroView[] heroViews;
	public void SetData(HerosInListScrollerController Controller, int index, PanelController _panel, UnityAction<HeroView> _choiceAction = null)
	{
		this.Controller = Controller;
		this.panel = _panel;
		this.choiceAction = _choiceAction;
		int startIndex = heroViews.Length * index;
		List<HeroData> heroDatas = Controller.Controller.allHeroesToView;
		for (int i = 0; i < heroViews.Length; i++)
		{
			HeroView _current = heroViews[i];
			int heroDataIndex = startIndex + i;
			if (heroDataIndex < heroDatas.Count)
			{
				_current.gameObject.SetActive(true);
				HeroData _currentHero = heroDatas[startIndex + i];
				bool select = Controller.Controller.allHeroesSelect.Contains(_currentHero);
				CheckTOPHero(_current, _currentHero, select);
				_current.Init(heroDatas[startIndex + i], _panel, _choiceAction, select);

			}
			else
				_current.gameObject.SetActive(false);
		}
	}

	public void Reload()
	{
		//Debug.LogError("Reload");
		for (int i = 0; i < heroViews.Length; i++)
		{
			HeroView _current = heroViews[i];
			if (_current.gameObject.activeSelf)
			{
				_current.gameObject.SetActive(true);
				HeroData _currentHero = _current.heroData;
				bool select = Controller.Controller.allHeroesSelect.Contains(_currentHero);
				CheckTOPHero(_current, _currentHero, select);
				_current.UpdateAdd(select);

			}

		}
	}
	void CheckTOPHero(HeroView heroView, HeroData data, bool isSelect)
	{
		if (Controller.Controller.allHeroesSelect.Count <= 0)
		{
			if (isSelect)
				heroView.isTOPHero = true;
			else
				heroView.isTOPHero = true;
			return;
		}
		if (!isSelect)
			heroView.isTOPHero = data.Power > Controller.Controller.MinPOWonSelect;
		else
			heroView.isTOPHero = !Controller.Controller.HaveOtherStrongerNotSelect(data);
	}
#if UNITY_EDITOR
	private void OnValidate()
	{
		List<HeroView> v = new List<HeroView>();
		foreach (Transform item in transform)
		{
			v.Add(item.GetComponent<HeroView>());
		}
		heroViews = v.ToArray();
	}
#endif
}
