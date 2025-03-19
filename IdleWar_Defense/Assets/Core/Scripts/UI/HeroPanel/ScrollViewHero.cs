using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;

namespace FoodZombie.UI
{
	public class ScrollViewHero : MonoBehaviour, IEnhancedScrollerDelegate
	{
		public HeroPanel mHeroPanel;
		[Header("Scroll View ")]
		public EnhancedScroller sView;
		public EnhancedScrollerCellView cellViewPrefab;
		public List<HeroData> heroDatas;

		public List<HeroView> listHeroView;

		private void OnEnable()
		{
			heroDatas = GameData.Instance.HeroesGroup.GetAllHeroDatas();
			//heroDatas.Sort((a, b) => b.Power.CompareTo(a.Power));
			//mHeroPanel.heroDataView = null;
			sView.ReloadData();
			sView.Delegate = this;


			if (sView.Delegate != null)
			{
				if (!heroDatas.Contains(mHeroPanel.heroDataView))
					mHeroPanel.heroDataView.id = 0;

				sView.ReloadData();
				var index = heroDatas.IndexOf(mHeroPanel.heroDataView);
				sView.JumpToDataIndex(index, 0.5f, 0.5f);
			}


			EventDispatcher.AddListener<HeroEvolutionEvent>(OnHeroEvolution);
		}


		private void OnHeroEvolution(HeroEvolutionEvent e)
		{
			Refresh();
		}
		private void OnDisable()
		{
			EventDispatcher.RemoveListener<HeroEvolutionEvent>(OnHeroEvolution);
		}
		public void Refresh()
		{
			int idEvolution = mHeroPanel.heroDataView.id;
			heroDatas = GameData.Instance.HeroesGroup.GetAllHeroDatas();
			//heroDatas.Sort((a, b) => b.Power.CompareTo(a.Power));

			sView.ReloadData();


			mHeroPanel.heroDataView = GameData.Instance.HeroesGroup.GetHeroData(idEvolution);
			var index = heroDatas.IndexOf(mHeroPanel.heroDataView);
			sView.JumpToDataIndex(index, 0.5f, 0.5f);

		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
		{
			HeroView cellView = scroller.GetCellView(cellViewPrefab) as HeroView;
			var data = heroDatas[dataIndex];

			if (mHeroPanel.heroDataView == null || mHeroPanel.heroDataView.id == 0)
			{
				cellView.Init(data, mHeroPanel, mHeroPanel.ChoiceHeroView, true);
				cellView.ActiveArrow(false);
				mHeroPanel.ChoiceHeroView(cellView);

			}
			else
				cellView.Init(data, mHeroPanel, mHeroPanel.ChoiceHeroView, data.id == mHeroPanel.heroDataView.id);


			if (!listHeroView.Contains(cellView))
				listHeroView.Add(cellView);
			cellView.ActiveArrow(false);
			return cellView;

		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
		{
			return 160;
		}

		public int GetNumberOfCells(EnhancedScroller scroller)
		{
			return heroDatas.Count;
		}

		public void Action_NextScrollView()
		{
			sView.JumpToDataIndex(sView.EndCellViewIndex, 0f, 0f, false, EnhancedScroller.TweenType.easeOutSine);
		}

		public void Action_PreScrollView()
		{
			sView.JumpToDataIndex(sView.StartCellViewIndex, 1f, 1f, false, EnhancedScroller.TweenType.easeOutSine);
		}
	}

}
