using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PvPLevelsData", menuName = "Assets/Scriptable Objects/PvPLevelsData")]
public class PvPLevelsData : ScriptableObject
{
	public List<PvPLevelsDataOne> Datas;
}

[System.Serializable]
public class PvPLevelsDataOne
{
	public int ID;
	public string RankName;
	public int[] levels;
}