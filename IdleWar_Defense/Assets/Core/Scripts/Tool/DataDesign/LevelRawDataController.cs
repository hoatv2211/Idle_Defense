using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class LevelRawDataController
{
	public static string GetFolderMap(int currentMapIndex)
	{
		return Application.dataPath + "/Core/Resources/LevelDesign/Map" + (currentMapIndex + 1);
	}
	public static string ReadString(string path)
	{
		//Read the text from directly from the test.txt file
		StreamReader reader = new StreamReader(path);
		var text = reader.ReadToEnd();
		reader.Close();

		return text;
	}

	//public static string RemoveSpecialCharacters(this string str)
	//{
	//	StringBuilder sb = new StringBuilder();
	//	foreach (char c in str)
	//	{
	//		if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
	//		{
	//			sb.Append(c);
	//		}
	//	}
	//	return sb.ToString();
	//}

	public static int MapNumber()
	{
		System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Application.dataPath + "/Core/Resources/LevelDesign/");
		int countMap = dir.GetDirectories().Length - 1; //trừ folder Discovery
		return countMap;
	}

	public static void UpdateMissionInfor(int currentMapIndex, int currentMissionIndex, MissionInfo missionInfo)
	{
		var json = JsonUtility.ToJson(missionInfo);
		string fileName = GetFolderMap(currentMapIndex) + "/mission_" + (currentMissionIndex + 1) + ".json";
		WriteString(fileName, json);

	}
	public static MissionInfo LoadMapMission(int mapIndex, int missionIndex)
	{
		MissionInfo missionInfo = null;
		MapInfo currentMapInfo = LoadMapData(mapIndex);
		if (currentMapInfo != null)
		{
			var lenght = currentMapInfo.missionNumber;
			missionInfo = JsonUtility.FromJson<MissionInfo>(LevelRawDataController.ReadString(LevelRawDataController.GetFolderMap(mapIndex) + "/mission_" + (missionIndex + 1) + ".json"));
		}
		return missionInfo;
	}

	/// <summary>
	/// Load Mapinfor by mapIndex from 0
	/// </summary>
	/// <param name="mapIndex">from 0</param>
	public static MapInfo LoadMapData(int mapIndex)
	{
		MapInfo currentMapInfo = null;
		if (File.Exists(LevelRawDataController.GetFolderMap(mapIndex) + "/mapInfo.json"))
		{
			currentMapInfo = JsonUtility.FromJson<MapInfo>(LevelRawDataController.ReadString(LevelRawDataController.GetFolderMap(mapIndex) + "/mapInfo.json"));
		}
		return currentMapInfo;
		if (currentMapInfo != null)
		{

			var lenght = currentMapInfo.missionNumber;
			for (int i = 0; i < lenght; i++)
			{
				int index = i;
				var missionInfo = JsonUtility.FromJson<MissionInfo>(LevelRawDataController.ReadString(LevelRawDataController.GetFolderMap(mapIndex) + "/mission_" + (index + 1) + ".json"));
				int totalPower = 0;
				if (missionInfo != null)
				{

					for (int j = 0; j < missionInfo.waveInfos.Count; j++)
					{
						var waveInfo = missionInfo.waveInfos[j];
						int eNumber = 0;
						for (int k = 0; k < waveInfo.enemyInfos.Length; k++)
						{
							var enemyInfo = waveInfo.enemyInfos[k];
							if (enemyInfo.id != 0)
							{
								eNumber++;
								//var enemyData = EnemiesGroup.GetEnemyData(enemyInfo.id);
								//var HP = enemyData.GetHP(enemyInfo.level);
								//var damage = enemyData.GetDamage(enemyInfo.level);
								//var armor = enemyData.GetArmor(enemyInfo.level);
								//var attackSpeed = enemyData.GetAttackSpeed(enemyInfo.level);
								//var critRate = enemyData.GetCritRate(enemyInfo.level);
								//var accuracy = enemyData.GetAccuracy(enemyInfo.level);
								//var dodge = enemyData.GetDodge(enemyInfo.level);
								//var critDamage = enemyData.GetCritDamage(enemyInfo.level);

								//var power = ConfigStats.GetPower(HP, damage, armor, attackSpeed, critRate, accuracy, dodge, critDamage);
								//totalPower += power;
							}
						}

					}
				}
			}
		}
	}

	public static void WriteString(string path, string text)
	{
		//Write some text to the test.txt file
		StreamWriter writer = new StreamWriter(path, false);
		writer.Write(text);
		writer.Close();
		Debug.Log("Write " + path);
		Debug.Log("============================");
	}

}
