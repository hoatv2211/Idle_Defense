using FoodZombie;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPRecordPlayManager
{
	PvPScoreInGamePanel PvPScore;

	int[] levels;
	int levelIndex;
	int levelCurrent;
	PvPPlayRecordNote RecordCurrent;

	List<int> levels_playerPlayed;
	bool fakeMode = false;

	int userRankGroupID;
	public void Start(PvPScoreInGamePanel PvPScore)
	{
		levels_playerPlayed = new List<int>();
		this.PvPScore = PvPScore;
		levels = Config.PvpConfig.getLevesToPlay;
		levelIndex = -1;
		userRankGroupID = UserGroup.UserData.GetRank().RankGroupID;
		NextLevel();
	}
	public void Stop()
	{
		if (IEDoLevel != null)
		{
			PvPScore.StopCoroutine(IEDoLevel);
			IEDoLevel = null;
		}
	}
	void NextLevel()
	{
		fakeMode = false;
		levelIndex++;
		if (levelIndex < levels.Length)
		{
			levelCurrent = levels[levelIndex];
			PlayLevel(levelCurrent);
		}
	}
	void PlayLevel(int levelName)
	{
		Debug.Log("<color=yellow>RecordPlayer</color>:Try Play : " + levelName);
		RecordCurrent = Config.PvpConfig.GetRecordsByLevel(levelName);
		if (RecordCurrent != null)
		{
			Debug.Log("<color=yellow>RecordPlayer</color>: Get Record " + levelName);
			List<int> datas = new List<int>(RecordCurrent.datas);
			int totalTime = datas[0];
			datas.RemoveAt(0);
			int totalScore = datas[0];
			datas.RemoveAt(0);
			int isDead = datas[0];
			datas.RemoveAt(0);
			IEDoLevel = DoPlayLevel(datas, isDead);
			PvPScore.StartCoroutine(IEDoLevel);
		}
		else
		{
			Debug.Log("<color=yellow>RecordPlayer</color>: Record Null,Fake mode");
			fakeMode = true;
			IEDoBalanceScore = DoBalanceScore();
			PvPScore.StartCoroutine(IEDoBalanceScore);
		}
	}

	IEnumerator IEDoLevel;
	IEnumerator DoPlayLevel(List<int> datas, int isDead)
	{
		Debug.Log("<color=yellow>RecordPlayer</color>: Playing Server Record");
		int timeToPlay_before = 0;
		int timeWait = 0;
		while (datas.Count > 0)
		{
			int timeToPlay = datas[0];
			datas.RemoveAt(0);
			if (datas.Count <= 0) break;
			int dataToPlay = datas[0];
			datas.RemoveAt(0);
			timeWait = timeToPlay - timeToPlay_before;
			if (timeWait > 0)
				yield return Yielders.Get(timeWait);
			if (dataToPlay > 0)
				PvPScore.SetScore(1, dataToPlay);
			else
				PvPScore.SetDamageToPlayer(1, -dataToPlay);
			timeToPlay_before = timeToPlay;
		}
		if (isDead == 0)
			NextLevel();
		else
		{
			Debug.Log("RecordPlayer :Dead");
		}
	}


	public void OnPlayerLevelStart(int level)
	{

	}
	public void OnPlayerLevelEnd(int level)
	{
		if (levels_playerPlayed == null)
			levels_playerPlayed = new List<int>();
		levels_playerPlayed.Add(level);
		if (fakeMode)
		{
			if (IEDoBalanceScore != null)
			{
				PvPScore.StopCoroutine(IEDoBalanceScore); IEDoBalanceScore = null;
			}
			if (levels_playerPlayed.Contains(levelCurrent))
				NextLevel();
		}
	}

	public void FakeMode_SetScore(int scoreAdd)
	{
		if (fakeMode)
		{
			if (UnityEngine.Random.Range(0, 4) != 0)
			{
				//	PvPScore.SetScore(1, UnityEngine.Random.Range(0, scoreAdd * 2));
				PvPScore.StartCoroutine(DoSetScoreDelay(scoreAdd));
			}
		}
	}
	public void FakeMode_SetDamScore(int damAdd)
	{
		if (fakeMode)
		{
			if (UnityEngine.Random.Range(0, 4) != 0)
			{
				//	PvPScore.SetDamageToPlayer(1, UnityEngine.Random.Range(0, damAdd));
				PvPScore.StartCoroutine(DoSetDamDelay(damAdd));
			}
		}
	}

	IEnumerator DoSetScoreDelay(int scoreAdd)
	{
		yield return Yielders.Get(UnityEngine.Random.Range(0, 1.0f));
		PvPScore.SetScore(1, UnityEngine.Random.Range(0, scoreAdd * 2));
	}
	IEnumerator DoSetDamDelay(int scoreAdd)
	{
		yield return Yielders.Get(UnityEngine.Random.Range(0, 1.0f));
		PvPScore.SetScore(1, UnityEngine.Random.Range(0, scoreAdd * 2));
	}

	IEnumerator IEDoBalanceScore;
	IEnumerator DoBalanceScore()
	{
		while (true)
		{
			switch (userRankGroupID)
			{
				case 5:
					yield return Yielders.Get(UnityEngine.Random.Range(1.0f, 3.0f));
					break;
				case 4:
					yield return Yielders.Get(UnityEngine.Random.Range(1.0f, 2.0f));
					break;
				case 3:
					yield return Yielders.Get(UnityEngine.Random.Range(1.0f, 2.0f));
					break;
				case 2:
					yield return Yielders.Get(UnityEngine.Random.Range(1.0f, 2.0f));
					break;
				case 1:
					yield return Yielders.Get(UnityEngine.Random.Range(1.0f, 2.0f));
					break;
				default:
					yield return Yielders.Get(UnityEngine.Random.Range(1.0f, 2.0f));
					break;
			}

			if (PvPScore.score_enemy < PvPScore.score_player)
			{
				int scoreToAdd = 0;
				switch (userRankGroupID)
				{
					case 5:
						scoreToAdd = (PvPScore.score_player - PvPScore.score_enemy - 500);
						//PvPScore.SetScore(1, UnityEngine.Random.Range(0, (PvPScore.score_player - PvPScore.score_enemy - 100)));
						break;
					case 4:
						scoreToAdd = (PvPScore.score_player - PvPScore.score_enemy) - 100;
						//	PvPScore.SetScore(1, UnityEngine.Random.Range(0, (PvPScore.score_player - PvPScore.score_enemy) + 00));
						break;
					case 3:
						scoreToAdd = (PvPScore.score_player - PvPScore.score_enemy + 100);
						break;
					case 2:
						scoreToAdd = (PvPScore.score_player - PvPScore.score_enemy + 100);
						break;
					case 1:
						scoreToAdd = (PvPScore.score_player - PvPScore.score_enemy + 100);
						break;
					default:
						scoreToAdd = (PvPScore.score_player - PvPScore.score_enemy - 100);
						break;
				}
				if (scoreToAdd > 0)
					PvPScore.SetScore(1, scoreToAdd);

			}
		}
	}
}
