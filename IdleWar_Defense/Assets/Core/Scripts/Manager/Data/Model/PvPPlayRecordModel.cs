using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PvPPlayRecordModel
{
	public int RankID;
	public int RankGroupID;
	public int CP;
	public List<PvPPlayRecordNote> datas = new List<PvPPlayRecordNote>();

	public override string ToString()
	{
		return JsonUtility.ToJson(this);
	}
}
[System.Serializable]
public class PvPPlayRecordNote
{
	public int level;
	//Time,TotalScore,Dead,TimeNode,ScoreNode
	public int[] datas;
	public override string ToString()
	{
		return JsonUtility.ToJson(this);
	}
}
