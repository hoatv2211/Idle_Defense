using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;

public class LevelUpPanel : MyGamesBasePanel
{
	public Transform imgLevelUp;
	public TextMeshProUGUI txtOldLevel;
	public TextMeshProUGUI txtNewLevel;

	public Transform transformPool;
	// public Transform scrollPool;
	[SerializeField, Tooltip("Buildin Pool")] private List<RewardView> rewardViewsPool;
	public TextMeshProUGUI txtUnlock;

	public SimpleTMPButton btnClaim;

	private UserGroup UserGroup => GameData.Instance.UserGroup;
	private Action OnShowDone;
	private void Start()
	{
		btnClaim.onClick.AddListener(BtnClaim_Pressed);

		StartCoroutine(IEPlayFX(imgLevelUp));
	}
	public void SetAction(System.Action OnShowDone)
	{
		this.OnShowDone = OnShowDone;
	}
	internal override void Init()
	{
		Lock(true);
		// PlayFX(imgLevelUp);

		int lastClaimLevel = UserGroup.LastClaimLevel;

		txtOldLevel.text = "Level " + lastClaimLevel;
		var newLevel = lastClaimLevel + 1;
		txtNewLevel.text = "Level " + newLevel;

		rewardViewsPool.Free();

		var levelInfo = UserGroup.GetLevelInfo(newLevel);
		GameData.Instance.UserGroup.LevelShowup = newLevel;
		if (Constants.UNLOCK_CONTENT_TYPE == 0)
			txtUnlock.text = levelInfo.unlock;
		else
			txtUnlock.text = "";
		var rewardsInfos = levelInfo.GetRewards();
		int count = rewardsInfos.Count;
		for (int i = 0; i < count; i++)
		{
			var item = rewardsInfos[i];
			var rewardView = rewardViewsPool.Obtain(transformPool);
			rewardView.Init(item);
			rewardView.SetActive(true);
		}
	}

	private IEnumerator IEPlayFX(Transform pTarget)
	{
		yield return null;
		PlayFX(pTarget);
	}

	private void PlayFX(Transform pTarget)
	{
		SoundManager.Instance.PlaySFX(IDs.SOUND_UP_STAR);
		pTarget.transform.localScale = Vector3.one;
		UIFXManager.instance.PlayStarFX(pTarget);
		SimpleLeanFX.instance.Bubble(pTarget, 0.5f);
	}

	private void BtnClaim_Pressed()
	{
		var rewardInfos = UserGroup.ClaimLevel();
		if (UserGroup.CanShowLevelUp())
		{
			Init();
			StartCoroutine(IEPlayFX(imgLevelUp));
		}
		else
		{
			Lock(false);
			Back();
			if (OnShowDone != null)
				OnShowDone();
		}
		LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_LEVEL_UP, false);
	}
}
