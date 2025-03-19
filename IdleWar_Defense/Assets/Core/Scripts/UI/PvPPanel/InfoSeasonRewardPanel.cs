using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using EnhancedUI.EnhancedScroller;


namespace FoodZombie.UI
{
    public class InfoSeasonRewardPanel : MonoBehaviour, IEnhancedScrollerDelegate
    {
        [SerializeField] private SimpleTMPButton btnExit;

		[Header("Scroll View ")]
		public EnhancedScroller sView;
		public EnhancedScrollerCellView cellViewPrefab;
		public List<PvPOneRankData> listDatas;

        private void Start()
        {
			listDatas = GameUnityData.instance.PvPRankData.Datas;
			sView.Delegate = this;
			sView.ReloadData();

			btnExit.SetUpEvent(Action_BtnExit);
		}
        public void CallStart()
        {
            gameObject.SetActive(true);
          
		}

        private void Action_BtnExit()
        {
            gameObject.SetActive(false);
        }


		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
		{
			Form_RankingReward cellView = scroller.GetCellView(cellViewPrefab) as Form_RankingReward;
			var data = listDatas[dataIndex];
			cellView.SetInit(data);

			return cellView;

		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
		{
			return 160;
		}

		public int GetNumberOfCells(EnhancedScroller scroller)
		{
			return listDatas.Count;
		}
	}

}
