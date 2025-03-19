using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PvPRankData", menuName = "Assets/Scriptable Objects/PvPRankData")]
public class PvPRankData : ScriptableObject
{
	public int PointWin, PointLose;
	public List<PvPOneRankData> Datas;
	public void LoadData(string data)
	{
		try
		{
			if (data == null || data.Trim().Length <= 0)
				return;
			PvPRankData newData = JsonUtility.FromJson<PvPRankData>(data);
			this.Datas = newData.Datas;
		}
		catch (System.Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	Dictionary<int, List<PvPOneRankData>> allRanksByGroupID;
	public Dictionary<int, List<PvPOneRankData>> AllRanksByGroupID
	{
		get
		{
			if (allRanksByGroupID == null)
			{
				allRanksByGroupID = new Dictionary<int, List<PvPOneRankData>>();
				foreach (PvPOneRankData rank in Datas)
				{
					int keyValue = rank.RankGroupID;
					List<PvPOneRankData> _currentList = null;
					if (!allRanksByGroupID.TryGetValue(keyValue, out _currentList))
					{
						_currentList = new List<PvPOneRankData>();
						allRanksByGroupID.Add(keyValue, _currentList);
					}
					_currentList.Add(rank);
				}
			}
			return allRanksByGroupID;
		}
	}
}

[System.Serializable]
public class PvPOneRankData
{
	public int ID;
	public string RankName;
	/// <summary>
	/// 1 for Challenger,5 for Siler
	/// </summary>
	public int RankGroupID;
	public int RankPointRequest;
	public int[] RankPointRange;
	[Space]
	public int RWGem;
	public int RWSummonScroll;
	public int RWHonor;

	public int[] Levels
	{
		get
		{
			List<PvPLevelsDataOne> levelDatas = GameUnityData.instance.PvPLevelData.Datas;
			for (int i = 0; i < levelDatas.Count - 1; i++)
			{
				PvPLevelsDataOne levelData = levelDatas[i];
				if (this.RankName.Contains(levelData.RankName))
				{
					return levelData.levels;
				}
			}
			return null;
		}
	}

	List<PvPLevelsDataOne> _LevelGroup_AllRanks;
	//public List<PvPLevelsDataOne> GetLevelGroup_AllRanks
	//{
	//	get
	//	{
	//		if (_LevelGroup_AllRanks == null)
	//		{
	//			_LevelGroup_AllRanks = new List<PvPLevelsDataOne>();
	//			foreach (var ranks in GameUnityData.instance.PvPRankData.Datas)
	//			{

	//			}	
	//		}
	//	}
	//}

	public string ToString()
	{
		return JsonUtility.ToJson(this);
	}

}
