using FoodZombie;
using FoodZombie.UI;
using HedgehogTeam.EasyTouch.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Components;
using DG.Tweening;
using System;

public class PvPEndGamePanel : MyGamesBasePanel
{
	public CurrencyView CurrencyHonor, CurrencyGem, CurrencyTicket;
	public Text Text_Title;
	public GameObject Panel_Win, panel_Lose;
	public UserView UserPlayer, UserEnemy;
	public Text Text_PlayerRank, Text_EnemyRank;
	public SimpleTMPButton btnHome;
	public SimpleTMPButton btnStatistic;
	public SimpleTMPButton btnRestart;
	public List<RewardView> rewardViews;
	public Text Txt_UserScore, Txt_UserScoreAdd, Txt_UserScoreFinal, Text_ELO;
	public Text Txt_EScore, Txt_EScoreAdd, Txt_EScoreFinal;
	public GameObject Panel_UserScore, Panel_EnemyScore;
	public Image Image_UserWL_Panel, Image_EnemyWL_Panel;
	public Text Txt_UserWL_Panel, Text_EnemyWL_Panel;
	public GameObject Panel_Rewards;
	public GameObject Next_BuyTicket, Next_BuyGEM;
	public GameObject[] Object_PLayerDead, Object_PlayerNotDead;
	void Start()
	{
		btnHome.SetUpEvent(BtnHome_Pressed);
		btnStatistic.onClick.AddListener(BtnStatistic_Pressed);
		btnRestart.onClick.AddListener(BtnRestart_Pressed);
	}

	public void Init(PvPScoreInGamePanel pvpScorePanel)
	{
		CurrencyHonor.Init(IDs.CURRENCY_HONOR);
		CurrencyGem.Init(IDs.CURRENCY_GEM);
		CurrencyTicket.Init(IDs.CURRENCY_TICKET_PVP);
		Image_UserWL_Panel.gameObject.SetActive(false);
		Image_EnemyWL_Panel.gameObject.SetActive(false);
		btnHome.gameObject.SetActive(false);
		btnStatistic.gameObject.SetActive(false);
		btnRestart.gameObject.SetActive(false);
		Panel_Rewards.SetActive(false);
		bool isWin = true;
		UserModel user = UserGroup.UserData;
		UserPlayer.Text_UserName.text = user.UserName;
		UserPlayer.Image_UserView.sprite = user.GetAvatar();
		Text_PlayerRank.text = user.GetRank().RankName;
		UserModel userEnemy = Config.PvpConfig.UserEnemy;
		UserEnemy.Text_UserName.text = userEnemy.UserName;
		UserEnemy.Image_UserView.sprite = userEnemy.GetAvatar(false, (s) =>
		{
			UserEnemy.Image_UserView.sprite = s;
		});
		Text_EnemyRank.text = userEnemy.GetRank().RankName;
		Lock(true);
		GameplayController.Instance.PauseGame();
		Text_ELO.text = "";



		foreach (GameObject item in Object_PLayerDead)
		{
			item.SetActive(pvpScorePanel.isPlayerDie);
		}
		foreach (GameObject item in Object_PlayerNotDead)
		{
			item.SetActive(!pvpScorePanel.isPlayerDie);
		}
		if (pvpScorePanel.isPlayerDie)
		{
			isWin = false;
			Panel_EnemyScore.SetActive(false);
			PlayWLEffect(false);
		}
		else
		{
			Txt_UserScore.text = "Score :0000";
			Txt_UserScoreAdd.text = "HP Bonus :0000";
			Txt_UserScoreFinal.text = "Final Score :0000";
			Txt_UserScore.TextRun("Score :{0}", 0, pvpScorePanel.score_player, 1, 0, () =>
			{
				Txt_UserScoreAdd.TextRun("HP Bonus :{0}", 0, pvpScorePanel.score_player_add, 1, 0, () =>
				{
					Txt_UserScoreFinal.TextRun("Final Score :{0}", 0, pvpScorePanel.score_player_final, 1, 0, () =>
					{
						PlayWLEffect(isWin);
					});
				});
			});
			Txt_EScore.text = "Score :0000";
			Txt_EScoreAdd.text = "HP Bonus :0000";
			Txt_EScoreFinal.text = "Final Score :0000";
			Txt_EScore.TextRun("Score :{0}", 0, pvpScorePanel.score_enemy, 1, 0, () =>
			{
				Txt_EScoreAdd.TextRun("HP Bonus :{0}", 0, pvpScorePanel.score_enemy_add, 1, 0, () =>
				{
					Txt_EScoreFinal.TextRun("Final Score :{0}", 0, pvpScorePanel.score_enemy_final, 1, 0, null);
				});
			});

			if (pvpScorePanel.score_player_final >= pvpScorePanel.score_enemy_final)
			{
				isWin = true;
			}
			else
			{
				isWin = false;
			}
		}
		if (isWin)
			SoundManager.Instance.PlaySFX(IDs.SOUND_VICTORY, 1f);
		else
			SoundManager.Instance.PlaySFX(IDs.SOUND_DEFEAT, 1f);
		base.Init();
	}

	void PlayWLEffect(bool isWin)
	{
		Config.LogEvent(isWin ? TrackingConstants.PVP_WIN_COUNT : TrackingConstants.PVP_LOSE_COUNT);
		Image_UserWL_Panel.color = isWin ? Color.white : Color.black;
		Text_Title.gameObject.SetActive(false);
		Panel_Win.SetActive(isWin);
		panel_Lose.SetActive(!isWin);
		Txt_UserWL_Panel.text = isWin ? "WIN" : "LOSE";
		Image_UserWL_Panel.gameObject.SetActive(true);
		Image_EnemyWL_Panel.gameObject.SetActive(true);
		if (isWin) Image_UserWL_Panel.transform.DOScale(1.1f * Vector3.one, 1).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
		else
			Image_EnemyWL_Panel.transform.DOScale(1.1f * Vector3.one, 1).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
		Image_EnemyWL_Panel.color = !isWin ? Color.white : Color.black;
		Text_EnemyWL_Panel.text = !isWin ? "WIN" : "LOSE";
		int scoreAdd = isWin ? GameUnityData.instance.PvPRankData.PointWin : -GameUnityData.instance.PvPRankData.PointLose;
		//MainGamePanel.instance.ShowWaitingPanel(true, "Update Score");
		GameRESTController.Instance.APIUser_AddScore(scoreAdd, OnAddScoreDone, OnAddScoreError);

		var currenciesGroup = GameData.Instance.CurrenciesGroup;
		if (currenciesGroup.CanPay(IDs.CURRENCY_TICKET_PVP, 1))
		{
			Next_BuyTicket.SetActive(true);
			Next_BuyGEM.SetActive(false);
		}
		else
		{
			Next_BuyTicket.SetActive(false);
			Next_BuyGEM.SetActive(true);
		}
	}

	private void OnAddScoreError(string obj)
	{
		//	MainGamePanel.instance.ShowWaitingPanel(false, "Update Score");
		MainGamePanel.instance.ShowWarningPopup(obj);
		btnHome.gameObject.SetActive(true);
		btnStatistic.gameObject.SetActive(true);
		btnRestart.gameObject.SetActive(true);
	}

	private void OnAddScoreDone(HttpREsultObject obj)
	{
		//MainGamePanel.instance.ShowWaitingPanel(false, "Update Score");
		int oldScore = UserGroup.UserData.ScorePvPRank;
		int oldRankid = UserGroup.UserData.GetRank().ID;
		HttpResultData result = obj.Data[0];
		UserGroup.UserData.UpdateBaseData(result);
		UserGroup.UserData.SaveToGameSave();
		btnHome.gameObject.SetActive(true);
		btnStatistic.gameObject.SetActive(true);
		btnRestart.gameObject.SetActive(true);
		if (UserGroup.UserData.ScorePvPRank != oldScore)
		{
			int diff = (UserGroup.UserData.ScorePvPRank - oldScore);
			Text_ELO.text = String.Format("ELO: {0} {1}", oldScore, diff > 0 ? "+" + diff : "-" + diff);
			Text_ELO.TextRun("ELO: {0}", oldScore, UserGroup.UserData.ScorePvPRank, 1, 1, null);
		}
		else
			Text_ELO.text = String.Format("ELO: {0}", oldScore);
		int newRankid = UserGroup.UserData.GetRank().ID;
		Text_PlayerRank.text = UserGroup.UserData.GetRank().RankName;
		if (newRankid != oldRankid)
		{
			//Change Rank,Clear all Match:
			//Config.PvpConfig.EnemyIDs_history.Clear();
			Config.PvpConfig.EnemyIDs_future.Clear();
		}
		if (newRankid < oldRankid)
		{
			Debug.LogError("Rank up");

			if (newRankid < UserGroup.UserData.BestRankID)
			{
				UserGroup.UserData.BestRankID = newRankid;
				UserGroup.UserData.SaveToGameSave();
				//	MainGamePanel.instance.ShowWarningPopup("Congratulations,you reach Rank");
				StartCoroutine(DoShowRewards(oldRankid));
				//PvPOneRankData _currentRank = UserGroup.UserData.GetRank();
				//if (!UserGroup.UserData.RWisClaim(_currentRank.ID))
				//{
				//	if (_currentRank.RWGem > 0)
				//	{
				//		RewardInfo rw = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_GEM, _currentRank.RWGem);
				//		rewardViews[0].Init(rw);
				//		LogicAPI.ClaimReward(rw, TrackingConstants.VALUE_PVP, false);
				//	}
				//	if (_currentRank.RWSummonScroll > 0)
				//	{
				//		RewardInfo rw = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_POWER_FRAGMENT, _currentRank.RWSummonScroll);
				//		rewardViews[1].Init(rw);
				//		LogicAPI.ClaimReward(rw, TrackingConstants.VALUE_PVP, false);
				//	}
				//	if (_currentRank.RWHonor > 0)
				//	{
				//		RewardInfo rw = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_HONOR, _currentRank.RWHonor);
				//		rewardViews[2].Init(rw);
				//		LogicAPI.ClaimReward(rw, TrackingConstants.VALUE_PVP, false);
				//	}
				//	Panel_Rewards.SetActive(true);
				//	UserGroup.UserData.RWAdd(_currentRank.ID);
				//}
			}
		}
	}

	IEnumerator DoShowRewards(int rankID)
	{
		UserModel user = UserGroup.UserData;
		PvPOneRankData _currentRank = user.GetRankByID(rankID);
		btnHome.gameObject.SetActive(false);
		btnStatistic.gameObject.SetActive(false);
		btnRestart.gameObject.SetActive(false);
		if (!user.RWisClaim(_currentRank.ID))
		{
			List<RewardInfo> rws = new List<RewardInfo>();
			//not claim,we claim this Rank:
			if (_currentRank.RWGem > 0)
			{
				RewardInfo rw = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_GEM, _currentRank.RWGem);
				rewardViews[0].Init(rw);
				rws.Add(rw);
				LogicAPI.ClaimReward(rw, TrackingConstants.VALUE_PVP, false);
			}
			if (_currentRank.RWSummonScroll > 0)
			{
				RewardInfo rw = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_POWER_FRAGMENT, _currentRank.RWSummonScroll);
				rewardViews[1].Init(rw);
				rws.Add(rw);
				LogicAPI.ClaimReward(rw, TrackingConstants.VALUE_PVP, false);
			}
			if (_currentRank.RWHonor > 0)
			{
				RewardInfo rw = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_HONOR, _currentRank.RWHonor);
				rewardViews[2].Init(rw);
				rws.Add(rw);
				LogicAPI.ClaimReward(rw, TrackingConstants.VALUE_PVP, false);
			}
			Panel_Rewards.SetActive(true);
			UserGroup.UserData.RWAdd(_currentRank.ID);
			bool pressedBtn = false;
			yield return new WaitForSecondsRealtime(1);
			string str = "Congratulations,you reached <color=red>" + _currentRank.RankName + "</color> rank";
			//string str = String.Format(Localization.Get(Localization.ID.MESSAGE_43), _currentRank.RankName);
			MainGamePanel.instance.ShowWarningPopup(str, () =>
			{
				pressedBtn = true;

			}, false);
			yield return new WaitUntil(() => pressedBtn);
			GameplayController.Instance.PauseGame();
			LogicAPI.ClaimRewards(rws, TrackingConstants.VALUE_PVP, true, () =>
			{
				pressedBtn = true;
				GameplayController.Instance.PauseGame();
				if (user.GetRank().ID == rankID)
				{
					//reach User Rank,stop
					btnHome.gameObject.SetActive(true);
					btnStatistic.gameObject.SetActive(true);
					btnRestart.gameObject.SetActive(true);
				}
				else
				{
					//not reach,keep going
					StartCoroutine(DoShowRewards(rankID - 1));
				}
			});
			GameplayController.Instance.PauseGame();
			yield return new WaitUntil(() => pressedBtn);


		}
		else
		{
			if (user.GetRank().ID == rankID)
			{
				//reach User Rank,stop
				btnHome.gameObject.SetActive(true);
				btnStatistic.gameObject.SetActive(true);
				btnRestart.gameObject.SetActive(true);
				GameplayController.Instance.PauseGame();
			}
			else
			{
				//not reach,keep going
				StartCoroutine(DoShowRewards(rankID - 1));
			}
		}

	}

	private void BtnRestart_Pressed()
	{

		var currenciesGroup = GameData.Instance.CurrenciesGroup;
		if (currenciesGroup.CanPay(IDs.CURRENCY_TICKET_PVP, 1))
		{
			
			AdsHelper.Instance.CheckAndShowAdsEndGame(TrackingConstants.ADS_INTER_END_GAME, () => { Time.timeScale = 0; }, (show) =>
			{
				Time.timeScale = 1;
				Config.PvpConfig.Pay_Type = IDs.CURRENCY_TICKET_PVP;
				Config.PvpConfig.Pay_Value = 1;
				//	currenciesGroup.Pay(IDs.CURRENCY_TICKET_PVP, 1);
				Lock(false);
				GameplayController.Instance.RestartGame();
				Config.LogEvent(TrackingConstants.CLICK_PVP_RESTART);
			});
		}
		else
		{

			var gem = Constants.GEM_TO_TICKET_PVP_SHOP;
			if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, gem))
			{
				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
				//MainPanel.instance.ShowWarningPopup("Not enough gem");
				return;
			}
			AdsHelper.Instance.CheckAndShowAdsEndGame(TrackingConstants.ADS_INTER_END_GAME, () => { Time.timeScale = 0; }, (show) =>
			{
				Time.timeScale = 1;
				Config.PvpConfig.Pay_Type = IDs.CURRENCY_GEM;
				Config.PvpConfig.Pay_Value = gem;
				//	currenciesGroup.Pay(IDs.CURRENCY_GEM, gem);
				Lock(false);
				GameplayController.Instance.RestartGame();
				Config.LogEvent(TrackingConstants.CLICK_PVP_RESTART);
			});
		}


	}
	private void BtnHome_Pressed()
	{
		AdsHelper.Instance.CheckAndShowAdsEndGame(TrackingConstants.ADS_INTER_END_GAME, () => { Time.timeScale = 0; }, (show) =>
		{
			Time.timeScale = 1;
			Lock(false);
			Config.backToHomePanel = SceneName.PvPMainPanel;
			GameplayController.Instance.BackToHome();
		});
	}
	private void BtnStatistic_Pressed()
	{
		MainGamePanel.instance.ShowHeroStatisticsPanel(DamageTracker.instance.heroDamageInBattle);
	}
}
