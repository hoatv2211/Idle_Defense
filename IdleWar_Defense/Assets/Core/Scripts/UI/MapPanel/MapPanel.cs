using System;
using System.Collections.Generic;
using NPOI.SS.Formula.Functions;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Pattern.UI;
using Utilities.Service.RFirebase;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace FoodZombie.UI
{
	public class MapPanel : MyGamesBasePanel
	{
		public CurrencyView coinView;
		public CurrencyView gemView;
		public CurrencyView expHeroView;

		public TextMeshProUGUI txtMapName;

		public SimpleTMPButton btnPre;
		public GameObject imgNotiPre;
		public SimpleTMPButton btnNext;
		public GameObject imgNotiNext;

		public GameObject[] mapGames;
		public MissionButton[] btnMissions;
		[SerializeField, Tooltip("Buildin Pool")] private List<Image> imgLinesPool;
		public Transform highlightMission;

		private List<MapInfo> mapInfos;
		private MapInfo selectedMapInfo;
		private MissionsGroup MissionsGroup => GameData.Instance.MissionsGroup;
		private void Start()
		{
			btnPre.onClick.AddListener(BtnPre_Pressed);
			btnNext.onClick.AddListener(BtnNext_Pressed);

			MainPanel.instance.onAnyChildHide += OnMainPanelChildHide;
			MainPanel.instance.onAnyChildShow += OnMainPanelChildShow;
		}

		private void OnDestroy()
		{
			MainPanel.instance.onAnyChildHide -= OnMainPanelChildHide;
			MainPanel.instance.onAnyChildShow -= OnMainPanelChildShow;
		}

		internal override void Init()
		{
			coinView.Init(IDs.CURRENCY_COIN);
			gemView.Init(IDs.CURRENCY_GEM);
			expHeroView.Init(IDs.CURRENCY_EXP_HERO);

			mapInfos = MissionsGroup.ListMaps;
			selectedMapInfo = MissionsGroup.GetCurrentMapInfo();

			ShowMap();

			imgLinesPool.Free();
			var currentMissionId = MissionsGroup.CurrentMissionId;
			var missionDatas = MissionsGroup.GetListMissionData();
			var max = missionDatas.Count;
			var count = btnMissions.Length;
			MissionData missionData;
			//đoạn này phải xử lý fake cho các mission chưa có data
			for (int i = 0; i < count; i++)
			{
				if (i < max) missionData = missionDatas[i];
				else missionData = null;

				var btnMission = btnMissions[i];
				btnMission.Init(missionData);

				//nếu cùng map thì mới có đường nối
				if (missionData != null && missionData.Id < currentMissionId && missionData.mapId == missionDatas[i + 1].mapId)
				{
					var btnMissionNext = btnMissions[i + 1];
					var pos1 = btnMission.transform.position;
					var pos2 = btnMissionNext.transform.position;

					var imgLine = imgLinesPool.Obtain(mapGames[missionData.mapId - 1].transform);
					imgLine.transform.position = btnMission.transform.position;
					imgLine.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Config.GetZangleFromTwoPosition(pos1, pos2)));
					imgLine.rectTransform.sizeDelta = new Vector2(Vector2.Distance(pos1, pos2) * 100f, 27f);
					imgLine.transform.SetAsFirstSibling();
					imgLine.SetActive(true);
				}

				if (missionData != null && missionData.Id == currentMissionId)
				{
					highlightMission.SetParent(btnMission.transform);
					highlightMission.localPosition = Vector3.zero;
				}
			}
		}

		private void OnMainPanelChildHide(PanelController pPanel)
		{
			if (MainPanel.instance.TopPanel is MapPanel)
			{
				highlightMission.SetActive(true);
			}
			else
			{
				highlightMission.SetActive(false);
			}
		}

		private void OnMainPanelChildShow(PanelController pPanel)
		{
			if (MainPanel.instance.TopPanel is MapPanel)
			{
				highlightMission.SetActive(true);
			}
			else
			{
				highlightMission.SetActive(false);
			}
		}

		public void ShowMap()
		{
			var currentMissionId = GameData.Instance.MissionsGroup.CurrentMissionId;
			MapInfo mapInfoPre = null;
			MapInfo mapInfoNext = null;

			int indexMap = mapInfos.IndexOf(selectedMapInfo);
			if (indexMap <= 0)
			{
				btnPre.SetActive(false);
				btnNext.SetActive(true);

				mapInfoNext = mapInfos[indexMap + 1];
				btnNext.labelTMP.text = mapInfoNext.mapName.ToUpper();
			}
			else if (indexMap >= mapInfos.Count - 1)
			{
				btnPre.SetActive(true);
				btnNext.SetActive(false);

				mapInfoPre = mapInfos[indexMap - 1];
				btnPre.labelTMP.text = mapInfoPre.mapName.ToUpper();
			}
			else
			{
				btnPre.SetActive(true);
				btnNext.SetActive(true);

				mapInfoPre = mapInfos[indexMap - 1];
				btnPre.labelTMP.text = mapInfoPre.mapName.ToUpper();
				mapInfoNext = mapInfos[indexMap + 1];
				btnNext.labelTMP.text = mapInfoNext.mapName.ToUpper();
			}

			//noti
			imgNotiPre.SetActive(false);
			if (mapInfoPre != null)
			{
				var missions = MissionsGroup.GetListMissionDataInMap(mapInfoPre.id);
				var countMission = missions.Count;
				for (int i = 0; i < countMission; i++)
				{
					var mission = missions[i];
					if (mission.Id < currentMissionId)
					{
						var bossRewardInfos = mission.GetBossRewardInfos();
						if (bossRewardInfos != null && bossRewardInfos.Count > 0)
						{
							if (!mission.ClaimedBossReward)
							{
								//  imgNotiPre.SetActive(true);
								imgNotiPre.SetActive(false);
								break;
							}
						}
					}
				}
			}

			imgNotiNext.SetActive(false);
			if (mapInfoNext != null)
			{
				var missions = MissionsGroup.GetListMissionDataInMap(mapInfoNext.id);
				var countMission = missions.Count;
				for (int i = 0; i < countMission; i++)
				{
					var mission = missions[i];
					if (mission.Id < currentMissionId)
					{
						var bossRewardInfos = mission.GetBossRewardInfos();
						if (bossRewardInfos != null && bossRewardInfos.Count > 0)
						{
							if (!mission.ClaimedBossReward)
							{
								//imgNotiNext.SetActive(true);
								imgNotiNext.SetActive(false);
								break;
							}
						}
					}
				}
			}

			//name
			txtMapName.text = selectedMapInfo.mapName.ToUpper();

			var count = mapGames.Length;
			for (int i = 0; i < count; i++)
			{
				mapGames[i].SetActive(false);
			}
			mapGames[indexMap].SetActive(true);
		}

		private void BtnPre_Pressed()
		{
			int indexMap = mapInfos.IndexOf(selectedMapInfo);
			selectedMapInfo = mapInfos[indexMap - 1];

			ShowMap();
		}

		private void BtnNext_Pressed()
		{
			int indexMap = mapInfos.IndexOf(selectedMapInfo);
			selectedMapInfo = mapInfos[indexMap + 1];

			ShowMap();
		}
	}
}