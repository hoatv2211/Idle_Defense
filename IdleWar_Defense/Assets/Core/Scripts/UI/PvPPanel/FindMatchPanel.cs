using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Utilities.Components;
using System;

namespace FoodZombie.UI
{
	public class FindMatchPanel : MonoBehaviour
	{
		[SerializeField] private SimpleTMPButton btnExit;
		[SerializeField] private Text txtDesc;

		Tween twDelayFindMatch = null;

		bool isGetEnemisInfor = false;
		bool isGetRecord = false;
		bool canStartGame = false;
		Action action;
		public void CallStart(Action _action)
		{
			this.action = _action;
			gameObject.SetActive(true);
			btnExit.SetUpEvent(Action_BtnExit);
			txtDesc.DOText("SEARCHING FOR OPPONENT.....", 1).SetLoops(-1, LoopType.Restart);
			DoMatching();
			//twDelayFindMatch = DOVirtual.DelayedCall(5, delegate
			//{
			//	StartMatch();
			//});
		}
		void DoMatching()
		{
			isGetEnemisInfor = false; isGetRecord = false;
			UserModel user = UserGroup.UserData;
			string formation = GameData.Instance.HeroesGroup.GetEquippedHeroesString();
			int CP = GameData.Instance.HeroesGroup.GetEquippedHeroesCP();
			user.Formation = formation;
			user.CP = CP;
			int[] rankRange = user.GetRank().RankPointRange;
			//Get Enemy:
			if (Config.PvpConfig.EnemyIDs_future.Count > 0)
			{
				LoadEnemyFromCache(UnityEngine.Random.Range(2.0f, 10.0f));
			}
			if (!isGetEnemisInfor)
			{
				List<string> noID = new List<string>();
				foreach (UserModel item in Config.PvpConfig.EnemyIDs_history)
				{
					noID.Add(item.ID);
				}
				int scoreMax, scoreMin;
				if (rankRange.Length == 1)
				{ scoreMax = -1; scoreMin = rankRange[0]; }
				else
				{ scoreMax = rankRange[1]; ; scoreMin = rankRange[0]; }
				GameRESTController.Instance.APIUser_GetEnemies(scoreMax, scoreMin, noID.ToArray(), OnGetEnemiesDone, OnGetEnemiesError);
			}
			//Get Enemy Done

			int[] levels = user.GetRank().Levels;
			//Shuffle Level List
			for (int t = 0; t < levels.Length; t++)
			{
				int tmp = levels[t];
				int r = UnityEngine.Random.Range(t, levels.Length);
				levels[t] = levels[r];
				levels[r] = tmp;
			}
			//Config.PvpConfig.UserEnemy = user;
			Config.PvpConfig.InitLevelToPlay(levels);

			//Get Record
			int[] levelsToGet = Config.PvpConfig.getLevesToPlay;
			List<string> levelstring = new List<string>();
			for (int i = 0; i < levelsToGet.Length; i++)
			{
				levelstring.Add(levelsToGet[i].ToString());
			}
			GameRESTController.Instance.APIRecord_GetRecord(user.GetRank().RankGroupID, levelstring.ToArray(), OnGetRecordDone, OnGetEnemiesError);
			//Get Record Done


			canStartGame = true;
			//Get EnemiesList:
			//GameRESTController.Instance.APIUser_GetEnemies

		}

		private void Update()
		{
			if (canStartGame && isGetEnemisInfor && isGetRecord)
			{
				StartMatch(); canStartGame = false;
			}
		}

		private void OnGetEnemiesError(string obj)
		{
			Debug.LogError(obj);
			txtDesc.DOKill();
			txtDesc.text = Localization.Get(Localization.ID.MESSAGE_42);
			MainGamePanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_41), () =>
			{
				Action_BtnExit();
			}, false);
		}
		private void OnGetRecordDone(HttpREsultObject obj)
		{
			HttpResultData[] datas = obj.Data;
			List<PvPPlayRecordNote> Records = new List<PvPPlayRecordNote>();
			foreach (HttpResultData item in datas)
			{
				int level = -1;
				if (int.TryParse(item.LevelName.Trim(), out level))
				{
					PvPPlayRecordNote record = new PvPPlayRecordNote();
					record.level = level;
					record.datas = item.data;
					Records.Add(record);
					Debug.Log("<color=green>Get SV Record:</color>" + level);
				}
			}
			Config.PvpConfig.Records = Records;
			Debug.Log("<color=green>Update SV Record:</color>" + Records.Count + " Records");
			isGetRecord = true;
		}
		private void OnGetEnemiesDone(HttpREsultObject obj)
		{
			Config.PvpConfig.EnemyIDs_future.Clear();
			Config.PvpConfig.EnemyIDs_history.Clear();
			if (obj != null && obj.Data != null)
				foreach (HttpResultData item in obj.Data)
				{
					UserModel user = new UserModel();
					user.UpdateBaseData(item);
					Config.PvpConfig.EnemyIDs_future.Add(user);
				}
			if (Config.PvpConfig.EnemyIDs_future.Count > 0)
				LoadEnemyFromCache(0);
			else
			{
				MainGamePanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_41), () =>
				{
					Action_BtnExit();
				}, false);
				Config.LogEvent(TrackingConstants.CLICK_PVP_FINDMATCH_FAILD);
			}

			if (isGetEnemisInfor) StartMatch();
		}

		private void LoadEnemyFromCache(float delay)
		{
			StartCoroutine(IELoadEnemyFromCache(delay));
		}

		IEnumerator IELoadEnemyFromCache(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			try
			{
				Config.PvpConfig.UserEnemy = Config.PvpConfig.EnemyIDs_future[0];
				Config.PvpConfig.EnemyIDs_future.RemoveAt(0);
				Config.PvpConfig.EnemyIDs_history.Add(Config.PvpConfig.UserEnemy);
				if (Config.PvpConfig.EnemyIDs_history.Count > 5) Config.PvpConfig.EnemyIDs_history.RemoveAt(0);
				if (Config.PvpConfig.UserEnemy != null)
					isGetEnemisInfor = true;
			}
			catch (Exception ex)
			{

				MainGamePanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_41), () =>
				{
					Action_BtnExit();
				}, false);
				Config.LogEvent(TrackingConstants.CLICK_PVP_FINDMATCH_FAILD);
			}
			
		}

		private void StartMatch()
		{
			if (gameObject.activeInHierarchy)
			{
				gameObject.SetActive(false);
				if (action != null)
					action.Invoke();
				Config.LogEvent(TrackingConstants.CLICK_PVP_FINDMATCH_SUCCESS);
			}
		}

		private void Action_BtnExit()
		{
			twDelayFindMatch.Kill();
			Config.backToHomePanel = SceneName.PvPMainPanel;
			GameplayController.Instance.BackToHome();
			//transform.GetComponentInParent<PvPMainPanel>().mMainPanel.SetActive(true);
			//gameObject.SetActive(false);
		}

	}

}
