using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using UnityEngine;

namespace FoodZombie.UI
{
	public class ScrollViewLeadBoard : MonoBehaviour, IEnhancedScrollerDelegate
	{
		[Header("Scroll View ")]
		public EnhancedScroller sView;
		public EnhancedScrollerCellView cellViewPrefab;

		public List<HttpResultData> ResultData;
		public List<UserModel> Data;

		[Header("TEST DATA User")]
		public int rankUser;
		public Form_LeadBoardItem form_User;

		[SerializeField] private PvPMainPanel mPvPMainPanel;
		public void CallStart()
		{

			if (sView != null)
				sView.JumpToDataIndex(0, EnhancedScroller.CellViewPositionEnum.Before);

			//Get Data
			Action_User_GetRank();
			Action_User_GetLeaderboard();

			////Check
			//if (sView.StartCellViewIndex <= rankUser - 1 && sView.EndCellViewIndex >= rankUser - 1)
			//	form_User.gameObject.SetActive(false);
			//else
			//	form_User.gameObject.SetActive(true);


		}


		public void Action_User_GetRank()
		{
			GameRESTController.Instance.APIUser_GetRank((o) =>
			{
				if (o.Data != null)
				{
					//Debug.Log("Done: " + o.Data[0].RankCurrentPlay + "/" + o.Data[0].TotalPlayer);
					rankUser = o.Data[0].RankCurrentPlay;
					form_User.SetSTTRank(rankUser - 1);

				}

			},
			(s) => { Debug.Log("Error " + s); });
		}
		public void UpdateUserView()
		{
			form_User.UpdateUserData();
		}
		public void Action_User_GetLeaderboard()
		{
			GameRESTController.Instance.APIUser_GetLeaderboard(15, 0, (o) =>
			{
				if (o != null && o.Data != null)
				{
					Debug.Log("Done 15-0: " + o.Data.Length);

					for (int i = 0; i < o.Data.Length; i++)
					{
						HttpResultData data = o.Data[i];

						if (!HaveDataInCurrentDatas(data)) ResultData.Add(o.Data[i]);
					}



					sView.Delegate = this;
					sView.ReloadData();

				}

			},
			(s) => { Debug.Log("Error " + s); });

		}

		bool HaveDataInCurrentDatas(HttpResultData data)
		{
			if (ResultData == null || ResultData.Count == 0) return false;
			foreach (var item in ResultData)
			{
				if (item._id.Equals(data._id)) return true;
			}
			return false;
		}


		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
		{
			try
			{
				Form_LeadBoardItem cellView = scroller.GetCellView(cellViewPrefab) as Form_LeadBoardItem;
				cellView.SetData(ResultData[dataIndex], dataIndex == rankUser - 1, dataIndex);
				//Check load
				if (dataIndex >= ResultData.Count - 1)
				{
					var count = ResultData.Count;
					Debug.LogError("Load Next " + count + "--->" + (count + 10));

					mPvPMainPanel.mWaitingPanel.gameObject.SetActive(true);
					mPvPMainPanel.mWaitingPanel.SetText("Get data");

					GameRESTController.Instance.APIUser_GetLeaderboard(10, count, (o) =>
					{
						if (o != null && o.Data != null && o.Data.Length > 0)
						{
							for (int i = 0; i < o.Data.Length; i++)
							{
								var _data = o.Data[i];
								if (!ResultData.Contains(_data))
									ResultData.Add(_data);
								else
								{
									//Ko load duoc nua
									Debug.LogError("Limited");
									mPvPMainPanel.mWaitingPanel.gameObject.SetActive(false);
									return;
								}

							}

							mPvPMainPanel.mWaitingPanel.gameObject.SetActive(false);
							sView.ReloadData();
							sView.Delegate = this;
						}
						else
						{
							mPvPMainPanel.mWaitingPanel.gameObject.SetActive(false);
						}


					},
						(s) => { Debug.Log("Error " + s); });
				}

				//Check Show Form User
				if (scroller.StartCellViewIndex <= rankUser - 1 && scroller.EndCellViewIndex >= rankUser - 1)
					form_User.gameObject.SetActive(false);
				else
					form_User.gameObject.SetActive(true);

				return cellView;
			}
			catch (System.Exception)
			{

				return null;
			}


		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
		{
			return 140;
		}

		public int GetNumberOfCells(EnhancedScroller scroller)
		{
			return ResultData.Count;
		}
	}


}
