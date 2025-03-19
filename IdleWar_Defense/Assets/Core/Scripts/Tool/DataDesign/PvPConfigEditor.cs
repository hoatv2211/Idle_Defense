#if UNITY_EDITOR
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PvPConfigEditor
{
	public static string filePath = Application.dataPath + "/Core/Excels/PvP/PvP.xlsx";
	// Start is called before the first frame update
	public static void PvPData_UpdateRankData()
	{
		FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		XSSFWorkbook workbook = new XSSFWorkbook(file);
		ISheet sheet = workbook.GetSheet("PvP Rank");
		List<PvPOneRankData> DatasTemp = new List<PvPOneRankData>();
		for (int i = 2; i <= sheet.LastRowNum; i++)
		{
			IRow row = sheet.GetRow(i);
			if (row == null) break;
			PvPOneRankData data = new PvPOneRankData();
			ICell cell = row.GetCell(0);
			if (cell == null) break;
			data.ID = (int)cell.NumericCellValue;
			cell = row.GetCell(1);
			if (cell == null) break;
			data.RankName = cell.StringCellValue;
			cell = row.GetCell(2);
			if (cell == null) break;
			data.RankPointRequest = (int)cell.NumericCellValue;
			cell = row.GetCell(3);
			if (cell == null) break;
			data.RWGem = (int)cell.NumericCellValue;
			cell = row.GetCell(4);
			if (cell == null) break;
			data.RWSummonScroll = (int)cell.NumericCellValue;
			cell = row.GetCell(5);
			if (cell == null) break;
			data.RWHonor = (int)cell.NumericCellValue;
			cell = row.GetCell(6);
			if (cell == null) break;
			data.RankGroupID = (int)cell.NumericCellValue;
			cell = row.GetCell(7);
			if (cell == null) break;
			List<int> RankPointRange = new List<int>();
			string _data = (string)cell.StringCellValue.Trim();
			string[] datas = _data.Split(',');
			for (int j = 0; j < datas.Length; j++)
			{
				RankPointRange.Add(int.Parse(datas[j]));
			}
			data.RankPointRange = RankPointRange.ToArray();
			Debug.Log(data.ToString());
			if (data.ID > 0)
				DatasTemp.Add(data);
		}
		GameUnityData.instance.PvPRankData.Datas = DatasTemp.OrderBy(x => -x.ID).ToList();
		EditorUtility.SetDirty(GameUnityData.instance.PvPRankData);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		file.Close();
	}

	public static void PvPData_UpdateLevels()
	{
		FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		XSSFWorkbook workbook = new XSSFWorkbook(file);
		ISheet sheet = workbook.GetSheet("Levels");
		List<PvPLevelsDataOne> DatasTemp = new List<PvPLevelsDataOne>();
		for (int i = 1; i <= sheet.LastRowNum; i++)
		{
			IRow row = sheet.GetRow(i);
			if (row == null) break;
			PvPLevelsDataOne data = new PvPLevelsDataOne();
			ICell cell = row.GetCell(0);
			if (cell == null) break;
			data.ID = (int)cell.NumericCellValue;
			cell = row.GetCell(1);
			if (cell == null) break;
			data.RankName = cell.StringCellValue;
			cell = row.GetCell(2);
			if (cell == null) break;
			string levels = cell.StringCellValue;
			Debug.Log(levels);
			string[] lv = levels.Split(',');
			List<int> lvint = new List<int>();
			foreach (var item in lv)
			{
				if (item != null && item.Trim().Length > 0)
					lvint.Add(int.Parse(item));
			}
			data.levels = lvint.ToArray();
			Debug.Log(data.ToString());
			if (data.levels.Length > 0)
				DatasTemp.Add(data);
		}
		GameUnityData.instance.PvPLevelData.Datas = DatasTemp.OrderBy(x => -x.ID).ToList();
		EditorUtility.SetDirty(GameUnityData.instance.PvPLevelData);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		file.Close();
	}

	public static void PvPData_GetJSON()
	{
		string json = JsonUtility.ToJson(GameUnityData.instance.PvPRankData);
		Debug.Log(json);
		GUIUtility.systemCopyBuffer = json;
		Debug.Log("Copy To Buffer");
	}
	public static void PvPLevel_GetJSON()
	{
		string json = JsonUtility.ToJson(GameUnityData.instance.PvPLevelData);
		Debug.Log(json);
		GUIUtility.systemCopyBuffer = json;
		Debug.Log("Copy To Buffer");
	}
}
#endif