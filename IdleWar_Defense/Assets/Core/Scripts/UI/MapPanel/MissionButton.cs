using FoodZombie;
using FoodZombie.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;

public class MissionButton : MonoBehaviour
{
	public TextMeshProUGUI txtMissionName;
	public SimpleTMPButton btnPlay;
	public Image imgComplete;

	[Separator("Reward")]
	public Image imgLineBossReward;
	public RewardView bossRewardView;
	public SimpleTMPButton btnClaimBossReward;
	public Image imgClaimed;
	public Image imgNoti;

	private MissionData missionData;

	// Start is called before the first frame update
	void Start()
	{
		btnPlay.onClick.AddListener(BtnPlay_Pressed);
		//if (btnClaimBossReward != null)
		//	btnClaimBossReward.onClick.AddListener(BtnClaimBossReward_Pressed);
	}

	public void Init(MissionData _missionData)
	{
		if (_missionData != null)
		{
			missionData = _missionData;
			txtMissionName.text = missionData.GetName();
			if (missionData.IsWin)
			{
				imgComplete.SetActive(true);
				btnPlay.SetEnable(false);
			}
			else
			{
				imgComplete.SetActive(false);
				btnPlay.SetEnable(true);
			}

			var currentMissionId = GameData.Instance.MissionsGroup.CurrentMissionId;
			if (missionData.Id > currentMissionId)
			{
				if (imgLineBossReward != null)
				{
					imgLineBossReward.SetActive(false);
					bossRewardView.SetActive(false);
					var bossRewardInfos = missionData.GetBossRewardInfos();
					if (bossRewardInfos != null && bossRewardInfos.Count > 0)
					{
						imgLineBossReward.SetActive(true);
						bossRewardView.SetActive(true);
						bossRewardView.Init(bossRewardInfos[0]);
						imgClaimed.SetActive(false);
						btnClaimBossReward.SetEnable(true);
						imgNoti.SetActive(false);
					}
					//    else

				}
			}
			else
			{
				var bossRewardInfos = missionData.GetBossRewardInfos();
				if (bossRewardInfos != null && bossRewardInfos.Count > 0 && imgLineBossReward != null)
				{
					imgLineBossReward.SetActive(true);
					bossRewardView.SetActive(true);
					bossRewardView.Init(bossRewardInfos[0]);

					//if (missionData.ClaimedBossReward)
					{
						imgClaimed.SetActive(true);
						btnClaimBossReward.SetEnable(false);
						imgNoti.SetActive(false);
					}
					//else
					//{
					//	imgClaimed.SetActive(false);
					//	btnClaimBossReward.SetEnable(true);
					//	if (missionData.Id == currentMissionId) imgNoti.SetActive(false);
					//	else imgNoti.SetActive(true);
					//}
				}
			}
		}
		else
		{
			txtMissionName.text = (transform.parent.GetSiblingIndex() + 1) + "-" + (transform.GetSiblingIndex() + 1);
		}
	}

	private void BtnPlay_Pressed()
	{
		MainPanel.instance.ShowMissionDetailPanel();
	}

	private void BtnClaimBossReward_Pressed()
	{
		if (!missionData.IsWin)
		{
			MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_26));
			//MainPanel.instance.ShowWarningPopup("To get the bonus rewards, you must win this mission first");
			return;
		}

		var rewardInfos = missionData.ClaimBossReward();
		LogicAPI.ClaimRewards(rewardInfos, string.Format(TrackingConstants.VALUE_MAP_X, missionData.mapId));

		Init(missionData);
	}

	private void OnValidate()
	{
		txtMissionName = GetComponentInChildren<TextMeshProUGUI>();
		btnPlay = GetComponent<SimpleTMPButton>();
	}
}
