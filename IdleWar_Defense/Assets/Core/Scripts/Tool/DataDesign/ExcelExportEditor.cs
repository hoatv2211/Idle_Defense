using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;
public static class ExcelExportEditor
{
	public static void ExportLevelData()
	{
		List<List<System.Object>> DatasToWrite = new List<List<System.Object>>();
		DatasToWrite = GetMapInfor();


		//   string filePath = Application.dataPath + "/Core/Excels/Report/LevelDataReport.xlsx";
		// FileStream file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		//    if()
		XSSFWorkbook workbook = new XSSFWorkbook();

		Debug.Log("Report Create:LevelData");
		ISheet sheet = workbook.GetSheet("LevelData");
		if (sheet == null)
			sheet = workbook.CreateSheet("LevelData");

		ICellStyle cellStypeGreen = workbook.CreateCellStyle();
		cellStypeGreen.FillForegroundColor = IndexedColors.Green.Index;
		cellStypeGreen.FillPattern = FillPattern.SolidForeground;
		ICellStyle cellStypeBlue = workbook.CreateCellStyle();
		cellStypeBlue.FillForegroundColor = IndexedColors.White.Index;
		cellStypeBlue.FillPattern = FillPattern.SolidForeground;
		//cellStype.FillPattern = FillPattern.BigSpots;
		ICellStyle currentStyle = null;
		int rowCount = 0;
		foreach (var DataRow in DatasToWrite)
		{
			IRow row = sheet.CreateRow(rowCount);

			int columnCount = 0;
			foreach (var DataColumn in DataRow)
			{
				if (columnCount == 0)
				{
					int Color = (int)DataColumn;
					if (Color == 0)
					{
						currentStyle = cellStypeGreen;
					}
					else
						currentStyle = cellStypeBlue;
					columnCount++;
					continue;
				}

				ICell cell = row.CreateCell(columnCount - 1);

				if (DataColumn is string)
					cell.SetCellValue((string)DataColumn);
				if (DataColumn is int)
					cell.SetCellValue((int)DataColumn);
				//   Debug.Log(columnCount % 2);
				//   cellStype.FillForegroundColor = columnCount % 2 == 0 ? cellStypeGreen : cellStypeBlue;
				//  cellStype.FillPattern = FillPattern.SolidForeground;
				cell.CellStyle = currentStyle;


				columnCount++;
			}
			rowCount++;
		}
		System.DateTime localDate = System.DateTime.Now;
		string date = localDate.ToString();
		date = date.RemoveSpecialCharacters();
		date = date.Replace('\\', '_');
		date = date.Replace(':', '_');
		date = date.Replace(' ', '_');
		// FileOutputStream
		string output = Application.dataPath + "/Core/Excels/Report/LevelDataReport" + date + ".xlsx";
		FileStream fileWrite = new FileStream(output, FileMode.Append, FileAccess.Write);
		workbook.Write(fileWrite);

		Debug.Log("Report Done");
	}
	static List<List<System.Object>> GetMapInfor()
	{
		List<List<System.Object>> DatasToWrite = new List<List<System.Object>>();
		List<System.Object> newCell = new List<System.Object>();
		newCell.Add((System.Object)ColorIndex);
		newCell.Add((System.Object)"Wave");
		newCell.Add((System.Object)"Number of Unit");
		//Debug.Log(MapIndex + "-" + MissionIndex + "-" + WaveIndex + "_" + eNumber);
		DatasToWrite.Add(newCell);

		System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Application.dataPath + "/Core/Resources/LevelDesign/");
		int countMap = dir.GetDirectories().Length - 1; //trừ folder Discovery
		for (int i = 0; i < countMap; i++)
		{
			//currentMapIndex = i;
			Debug.Log("Get Data from map " + i);
			LoadMapData(i, DatasToWrite);
		}
		return DatasToWrite;
	}

	static int ColorIndex = 0;

	private static void LoadMapData(int mapIndex, List<List<System.Object>> DatasToWrite)
	{
		int MapIndex;
		int MissionIndex;
		int WaveIndex;


		MapInfo currentMapInfo = null;
		if (File.Exists(LevelRawDataController.GetFolderMap(mapIndex) + "/mapInfo.json"))
		{
			currentMapInfo = JsonUtility.FromJson<MapInfo>(LevelRawDataController.ReadString(LevelRawDataController.GetFolderMap(mapIndex) + "/mapInfo.json"));
		}
		if (currentMapInfo != null)
		{

			MapIndex = mapIndex;
			var lenght = currentMapInfo.missionNumber;
			for (int i = 0; i < lenght; i++)
			{
				int index = i;
				MissionIndex = index;
				var missionInfo = JsonUtility.FromJson<MissionInfo>(LevelRawDataController.ReadString(LevelRawDataController.GetFolderMap(mapIndex) + "/mission_" + (index + 1) + ".json"));
				int totalPower = 0;
				if (missionInfo != null)
				{
					if (ColorIndex == 0) ColorIndex = 1;
					else ColorIndex = 0;
					for (int j = 0; j < missionInfo.waveInfos.Count; j++)
					{
						var waveInfo = missionInfo.waveInfos[j];
						WaveIndex = j;
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
						List<System.Object> newCell = new List<System.Object>();
						//  newCell.Add((System.Object)(MapIndex);
						newCell.Add((System.Object)ColorIndex);
						newCell.Add((System.Object)((MapIndex + 1) + "-" + (MissionIndex + 1) + "-" + (WaveIndex + 1)));
						newCell.Add((System.Object)eNumber);
						Debug.Log(MapIndex + "-" + MissionIndex + "-" + WaveIndex + "_" + eNumber);
						DatasToWrite.Add(newCell);
					}
				}
			}
		}
	}


}
