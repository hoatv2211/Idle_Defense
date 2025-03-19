using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Inspector;
using Utilities.Components;
using System;
using Spine.Unity;
using Utilities.Common;
using UnityEngine.SceneManagement;
using DG.Tweening;
using HedgehogTeam.EasyTouch.UI;

namespace FoodZombie.UI
{
	public class WinPanel : MyGamesBasePanel
	{
		public Transform transformPool;
		[SerializeField, Tooltip("Buildin Pool")]
		private List<RewardView> mainRewardViewsPool;
		public List<RewardView> bonusRewardViews;
		public GameObject groupBonus;

		public SimpleTMPButton btnHome;
		public SimpleTMPButton btnXCoin;
		public SimpleTMPButton btnNextMission;
		public SimpleTMPButton btnStatistic;
		public GameObject imgAds;
		public TextMeshProUGUI txtMissionName;
		public TextMeshProUGUI txtUnlock;
		private List<RewardInfo> mainRewards;


		private void Start()
		{
			btnHome.SetUpEvent(BtnHome_Pressed);
			btnXCoin.SetUpEvent(BtnXCoin_Pressed);
			btnNextMission.SetUpEvent(BtnNextMission_Pressed);
			btnStatistic.SetUpEvent(BtnStatistic_Pressed);

		}

		public void Init(List<RewardInfo> _mainRewards, List<RewardInfo> bonusRewards)
		{
			Lock(true);

			GameplayController.Instance.PauseGame();
			SoundManager.Instance.PlaySFX(IDs.SOUND_VICTORY, 1f);
			mainRewards = _mainRewards;

			var count = bonusRewardViews.Count;
			for (int i = 0; i < count; i++)
			{
				var rewardView = bonusRewardViews[i];
				rewardView.gameObject.SetActive(false);
			}

			var lastWinMissionData = GameData.Instance.MissionsGroup.GetLastWinMissionData();
			switch (Config.typeModeInGame)
			{
				case Config.TYPE_MODE_NORMAL:

					if (lastWinMissionData != null)
					{
						txtMissionName.text = Localization.Get(Localization.ID.MISSION)+" " + lastWinMissionData.GetName();
					}
					else
					{
						txtMissionName.text = Localization.Get(Localization.ID.REWARD_1);
					}

					var lastWinMissionId = GameData.Instance.MissionsGroup.LastWinMissionId;
					count = mainRewards.Count;
					mainRewardViewsPool.Free();
					for (int i = 0; i < count; i++)
					{
						var item = mainRewards[i];
						LogicAPI.ClaimReward(item, string.Format(TrackingConstants.VALUE_MISSION, lastWinMissionId), false);

						var mainRewardView = mainRewardViewsPool.Obtain(transformPool);
						mainRewardView.Init(item);
						mainRewardView.gameObject.SetActive(true);
					}

					count = bonusRewards.Count;
					for (int i = 0; i < count; i++)
					{
						var item = bonusRewards[i];
						LogicAPI.ClaimReward(item, string.Format(TrackingConstants.VALUE_MISSION, lastWinMissionId), false);

						var rewardView = bonusRewardViews[i];
						rewardView.Init(item);
						rewardView.SetActive(true);
					}

					groupBonus.SetActive(true);

					//	var currentMissionId = GameData.Instance.MissionsGroup.CurrentMissionId;
					//if (currentMissionId == 1005) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY));
					// if (currentMissionId == 1008) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.QUEST_GAMEPLAY));
					// if (currentMissionId == 1010) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.AUTO_BATTLE_REWARD_GAMEPLAY));
					//if (currentMissionId == 2001) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.USE_BASE_GAMEPLAY));
					//if (currentMissionId == 2002) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.USE_BASEPLAY_GAMEPLAY));
					if (lastWinMissionId == 1001)
					{
						//Debug.Log("Auto Open");
						//var heroesGroup = GameData.Instance.HeroesGroup;
						//var allHeroes = heroesGroup.GetAllHeroDatas();
						//int heroUnlockedNumber = 0;
						//if (allHeroes != null && allHeroes.Count > 0) heroUnlockedNumber = allHeroes.Count;

						//if (heroUnlockedNumber <= 1)
						//{
						//	//Load if only John
						//	GameData.Instance.HeroesGroup.ClaimFreePowerCrystalSummon();
						//	var rewardInfo = GameData.Instance.HeroesGroup.SummonByPowerCrytal();
						//	LogicAPI.ClaimReward(rewardInfo);
						//	LogicAPI.LoadAllHeroToFormation();
						//}
					}
					break;
				default:
					//var lastWinMissionData = GameData.Instance.DiscoveriesGroup.GetLastMissionData();
					if (lastWinMissionData != null)
					{
						txtMissionName.text = lastWinMissionData.GetName();
					}
					else
					{
						txtMissionName.text = Localization.Get(Localization.ID.REWARD_1);
					}

					count = mainRewards.Count;
					mainRewardViewsPool.Free();
					for (int i = 0; i < count; i++)
					{
						var item = mainRewards[i];
						LogicAPI.ClaimReward(item, TrackingConstants.VALUE_DISCOVERY, false);

						var mainRewardView = mainRewardViewsPool.Obtain(transformPool);
						mainRewardView.Init(item);
						mainRewardView.SetActive(true);
					}

					groupBonus.SetActive(false);
					break;

			}

			Config.LastGameResult = 1;
		}

		private void OnEnable()
		{
			switch (Config.typeModeInGame)
			{
				case Config.TYPE_MODE_NORMAL:
					var lastWinMissionId = GameData.Instance.MissionsGroup.LastWinMissionId;
					if (!TutorialsManager.SKIP_MISSION_INTRO && (lastWinMissionId <= 0 || lastWinMissionId == 1001))
					{
						btnNextMission.SetActive(false);
						btnXCoin.SetActive(false);
						btnHome.SetActive(true);
					}
					else
					{

						btnXCoin.SetActive(true);
						btnHome.SetActive(false);
						// StartCoroutine(IEShowBtn());
						btnNextMission.SetEnable(true);
						btnHome.SetActive(true);
					}

					if (Constants.UNLOCK_CONTENT_TYPE == 1)
					{
						txtUnlock.text = "";
						List<ContentUnlockInforData> _contentUnlock = GameUnityData.instance.ContentUnlockInfor.Datas;
						for (int i = 0; i < _contentUnlock.Count; i++)
						{
							if (lastWinMissionId == _contentUnlock[i].UnlockLevel)
							{
								txtUnlock.text = _contentUnlock[i].unlockText;
								break;
							}
						}
						//  txtUnlock.text = GameData.Instance.MissionsGroup.;
					}
					else
						txtUnlock.text = "";
					break;


				default:
					btnNextMission.SetActive(false);
					btnXCoin.SetActive(false);
					btnHome.SetActive(true);
					txtUnlock.text = "";
					break;

			}
		}

		private IEnumerator IEShowBtn()
		{
			var nextlabel = btnNextMission.labelTMP;
			nextlabel.color = new Color(45f / 255f, 45f / 255f, 69f / 255f);
			nextlabel.text = "Next (3)";
			yield return new WaitForSecondsRealtime(1f);
			nextlabel.text = "Next (2)";
			yield return new WaitForSecondsRealtime(1f);
			nextlabel.text = "Next (1)";
			yield return new WaitForSecondsRealtime(1f);
			nextlabel.text = "Next";
			nextlabel.color = Color.white;
			btnNextMission.SetEnable(true);
			btnHome.SetActive(true);
		}

		private void BtnHome_Pressed()
		{
			AdsHelper.Instance.CheckAndShowAdsEndGame(TrackingConstants.ADS_INTER_END_GAME, () => { Time.timeScale = 0; }, (show) =>
			{
				Time.timeScale = 1;
				Lock(false);
				GameplayController.Instance.BackToHome();
			});
		}

		private void BtnXCoin_Pressed()
		{
			var lastWinMissionId = GameData.Instance.MissionsGroup.LastWinMissionId;

			if (!AdsHelper.__IsVideoRewardedAdReady())
			{
				MainGamePanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_1));
				//MainGamePanel.instance.ShowWarningPopup("Ads not available");
			}
			else
			{
				AdsHelper.__ShowVideoRewardedAd(TrackingConstants.ADS_REWARD_WIN_X, OnRewardedAdCompleted);
			}
		}

		private void BtnNextMission_Pressed()
		{
			AdsHelper.Instance.CheckAndShowAdsEndGame(TrackingConstants.ADS_INTER_END_GAME, () => { Time.timeScale = 0; }, (show) =>
			{
				Time.timeScale = 1;
				Lock(false);
				var currentMissionId = GameData.Instance.MissionsGroup.CurrentMissionId;
				{
					Config.LogEvent(TrackingConstants.MISSION_PLAY_COUNT, TrackingConstants.PARAM_MISSION, currentMissionId);
					Config.LogEvent(TrackingConstants.MISSION_PLAY_DAY_COUNT, TrackingConstants.PARAM_MISSION, currentMissionId.ToString(), TrackingConstants.PARAM_DAY, GameData.Instance.UserGroup.playDayCount.Value);
					//AppsFlyerObjectScript.LogLevelAchieved(currentMissionId);
					

				}

				GameplayController.Instance.NextGame();
			});
		}

		private void BtnStatistic_Pressed()
		{
			MainGamePanel.instance.ShowHeroStatisticsPanel(DamageTracker.instance.heroDamageInBattle);
		}


		private void BtnPvPBack_Pressed()
		{
			Config.backToHomePanel = SceneName.PvPMainPanel;
			GameplayController.Instance.BackToHome();
		}

		private void BtnPvPRematch_Pressed()
		{
			Debug.Log("Set ReMatch Here");
			GameplayController.Instance.NextGame();
		}

		private void BtnPvPQuickMatch_Pressed()
		{
			Debug.Log("Set Quick Match Here");
			GameplayController.Instance.NextGame();
		}

		private void OnRewardedAdCompleted(bool isCompleted)
		{
			if (isCompleted)
			{
				ClaimReward();
			}
		}

		private void ClaimReward()
		{
			var count = mainRewards.Count;
			var rewardInfo = mainRewards[0];
			var rewardView = mainRewardViewsPool[0];
			if (rewardInfo.Type == IDs.REWARD_TYPE_CURRENCY && rewardInfo.Id == IDs.CURRENCY_COIN)
			{
				int startCoin = rewardInfo.Value;
				float coin = rewardInfo.Value;
				int endCoin = startCoin * 2; //x2 coin
				DOTween.To(tweenVal => coin = tweenVal, startCoin, endCoin, 1f)
					.OnUpdate(() => { rewardView.txtQuantity.text = "" + coin.ToString("0"); })
					.OnComplete(() => { rewardView.txtQuantity.text = "" + endCoin; })
					.SetUpdate(true);

				btnXCoin.SetEnable(false);
				btnHome.SetActive(true);
				GameData.Instance.CurrenciesGroup.Add(IDs.CURRENCY_COIN, endCoin - startCoin, string.Format(TrackingConstants.VALUE_MISSION, GameData.Instance.MissionsGroup.LastWinMissionId));
			}
		}
	}
}
