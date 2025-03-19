using EnhancedUI.EnhancedScroller;
using FoodZombie;
using FoodZombie.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Components;
using Utilities.Pattern.UI;

public class HerosInListScrollerController : MonoBehaviour
{

	//UnityAction<HeroView> _choiceAction = null

	public EnhancedScroller myScroller;
	public HerosInListView viewPrefab;
	public float cellSize = 250;
	List<HerosInListView> views = new List<HerosInListView>();
	public IHerosInListData Controller { get; set; }

	PanelController panel;
	UnityAction<HeroView> choiceHeroAction;


	// Start is called before the first frame update
	public void Show()
	{
		ExtendEnhancedScrollerDelegate<HerosInListView> scrollerDelegate
			= new ExtendEnhancedScrollerDelegate<HerosInListView>();
		List<HeroData> datas = Controller.allHeroesToView;
		scrollerDelegate.Init(cellSize, (datas.Count / 5) + 1, viewPrefab, (cellView, index) =>
			{
				if (!views.Contains(cellView))
					views.Add(cellView);
				cellView.SetData(this, index, panel, choiceHeroAction);
			});
		myScroller.Delegate = scrollerDelegate;
		myScroller.ReloadData();

	}

	public void Init(IHerosInListData Controller, PanelController panel, UnityAction<HeroView> choiceHeroAction)
	{
		this.Controller = Controller;
		this.panel = panel;
		this.choiceHeroAction = choiceHeroAction;
		Show();
	}

	public void UpdateData(List<HeroData> datas)
	{

	}

	//	public List<HeroData> Datas => datas;
	public void Reload()
	{
		foreach (HerosInListView herosInListView in views)
		{
			herosInListView.Reload();
		}
	}
	public void ChoiceHero(HeroData heroData)
	{
		this.Controller.ChoiceHeroData(heroData);
		Reload();
	}

	public JustButton GetFirstCellSelect()
	{
		return null;
	}
}
