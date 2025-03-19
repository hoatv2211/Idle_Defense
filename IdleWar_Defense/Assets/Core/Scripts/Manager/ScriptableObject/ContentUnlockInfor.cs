using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Service.RFirebase;

[CreateAssetMenu(fileName = "ContentUnlockInfor", menuName = "Assets/Scriptable Objects/ContentUnlockInfor", order = 3)]
public class ContentUnlockInfor : ScriptableObject
{
	public List<ContentUnlockInforData> Datas;

	[Button]
	public void Sort()
	{
		Debug.LogError("Sort: " + Datas.Count + " items");
		Datas = Datas.OrderBy(x => x.unlockLevel).ToList();
		int _lastUnlockLevel = 0;
		for (int i = 0; i < Datas.Count; i++)
		{
			ContentUnlockInforData content = Datas[i];
			if (_lastUnlockLevel == content.unlockLevel)
				Debug.LogError("Error:Same unlockLevel " + _lastUnlockLevel);
			_lastUnlockLevel = content.unlockLevel;
		}
		Debug.LogError("Sort: Done");
	}
}

[System.Serializable]
public class ContentUnlockInforData
{
	public int unlockLevel;
	public bool GetUnlockValueFromFirebase = false;
	public string unlockLevelFirebaseKey = "";
	[Multiline]
	public string unlockText;

	public int UnlockLevel
	{
		get
		{
			if (GetUnlockValueFromFirebase)
			{
				int _output = unlockLevel;
				try
				{
					_output = (int)RFirebaseRemote.Instance.GetNumberValue(unlockLevelFirebaseKey.Trim(), unlockLevel);
					return _output;
				}
				catch (Exception ex)
				{

					return unlockLevel;
				}
			}
			else
				return unlockLevel;
		}
	}
}