using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using UnityEngine;

public class LevelConfigExtend
{

    public static string filePath = Application.dataPath + "/Core/Excels/LevelConfig/LevelData.xlsx";
    public static string filePathTest = Application.dataPath + "/Core/Excels/LevelConfig/LevelData1.xlsx";
    public static string fileLink = "https://docs.google.com/spreadsheets/d/1EV6eg6qOcOcpNes7ha1b3PjiS8zhp_0Z/edit#gid=1944640069";

    public static void UpdateDataLevelValues()
    {
        FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        XSSFWorkbook workbook = new XSSFWorkbook(file);
        ISheet sheet = workbook.GetSheet("LevelValues");
        for (int i = 1; i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null) break;
            ICell cell = row.GetCell(0);
            if (cell == null) break;
            int levelToChange = (int)cell.NumericCellValue;
            float hpx = 1;
            if (row.GetCell(1).StringCellValue.Trim().Length > 0)
            {
                string valueString = row.GetCell(1).StringCellValue.Trim();
                //   Debug.Log(valueString);
                float.TryParse(valueString, out hpx);
                hpx = Mathf.Round(hpx * 10.0f) * 0.1f;
            }
            float damx = 1;
            if (row.GetCell(2).StringCellValue.Trim().Length > 0)
            {
                string valueString = row.GetCell(2).StringCellValue.Trim();
                // Debug.Log(valueString);
                float.TryParse(valueString, out damx);
                damx = Mathf.Round(damx * 10.0f) * 0.1f;
                //    damx = (float)row.GetCell(2).NumericCellValue;
            }
            int mapIndex = levelToChange / 1000 - 1;
            int missionIndex = levelToChange % 1000 - 1;

            //  if (hpx > 0 && damx > 0)
            {
                if (hpx <= 0) hpx = 1;
                if (damx <= 0) damx = 1;
                Debug.Log("Update " + mapIndex + "_" + missionIndex + ":hpx=" + hpx + ",damx=" + damx);
                MissionInfo missionInfor = LevelRawDataController.LoadMapMission(mapIndex, missionIndex);
                missionInfor.hpx = hpx;
                missionInfor.damx = damx;
                
                LevelRawDataController.UpdateMissionInfor(mapIndex, missionIndex, missionInfor);
            }
        }
        file.Close();
    }
    public static void UpdateDataLevelPOW()
    {
        FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        XSSFWorkbook workbook = new XSSFWorkbook(file);
        ISheet sheet = workbook.GetSheet("LevelPOW");
        for (int i = 1; i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            string levelToChange = (string)row.GetCell(0).StringCellValue;
            string POWbool = row.GetCell(2).StringCellValue;
            bool havePOW = (POWbool != null && POWbool.Trim().Length > 0) ? true : false;
            string[] levelChangeStrings = levelToChange.Split('-');
            int mapIndex = -1;
            int missionIndex = -1;
            int waveIndex = -1;
            int.TryParse(levelChangeStrings[0], out mapIndex);
            int.TryParse(levelChangeStrings[1], out missionIndex);
            int.TryParse(levelChangeStrings[2], out waveIndex);
            if (mapIndex < 0 || missionIndex < 0 || waveIndex < 0)
            {
                Debug.LogError("Error: Wave Index is wrong: " + levelToChange); return;
            }
            mapIndex--; missionIndex--; waveIndex--;
            //  if (hpx > 0 && damx > 0)
            {

                Debug.Log("Update " + mapIndex + "_" + missionIndex + "_" + waveIndex + "_have POW" + havePOW);
                MissionInfo missionInfor = LevelRawDataController.LoadMapMission(mapIndex, missionIndex);
                WaveInfo waveInfor = missionInfor.waveInfos[waveIndex];
                waveInfor.powActive = havePOW;
                waveInfor.powValue = 3;
                LevelRawDataController.UpdateMissionInfor(mapIndex, missionIndex, missionInfor);
            }
        }
        file.Close();
    }

    public static void GetDataToFirebase()
    {
        List<LevelConfigObject> datas = new List<LevelConfigObject>();
        FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        XSSFWorkbook workbook = new XSSFWorkbook(file);
        ISheet sheet = workbook.GetSheet("LevelValues");

        IRow rowHeader = sheet.GetRow(0);
        string versionData = "";
        for (int i = 0; i < rowHeader.LastCellNum; i++)
        {
            versionData = rowHeader.GetCell(i).StringCellValue;
            if (versionData.Contains("DataVersion")) break;
        }
        int dataVersion = -1;
        if (versionData.Contains("DataVersion"))
        {
            int.TryParse(versionData.Replace("DataVersion", "").Trim(), out dataVersion);
        }
        if (dataVersion == -1)
        {
            Debug.LogError("Warning: Have no DATAVERSION");
        }
        else
            Debug.Log("DATAVERSION: " + dataVersion);
        for (int i = 1; i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null) break;
            if (row.GetCell(0) == null) break;
            int levelToChange = (int)row.GetCell(0).NumericCellValue;
            float hpx = 1;
            if (row.GetCell(1).StringCellValue.Trim().Length > 0)
            {
                string valueString = row.GetCell(1).StringCellValue.Trim();
                //   Debug.Log(valueString);
                float.TryParse(valueString, out hpx);
                hpx = Mathf.Round(hpx * 10.0f) * 0.1f;
            }
            float damx = 1;
            if (row.GetCell(2).StringCellValue.Trim().Length > 0)
            {
                string valueString = row.GetCell(2).StringCellValue.Trim();
                // Debug.Log(valueString);
                float.TryParse(valueString, out damx);
                damx = Mathf.Round(damx * 10.0f) * 0.1f;
                //    damx = (float)row.GetCell(2).NumericCellValue;
            }
            int mapIndex = levelToChange / 1000 - 1;
            int missionIndex = levelToChange % 1000 - 1;
            if (levelToChange <= 0) break;
            if (hpx > 0 && damx > 0)
            {

                //MissionInfo missionInfor = LevelRawDataController.LoadMapMission(mapIndex, missionIndex);
                if (!(hpx == 1 && damx == 1))
                {
                    Debug.Log("Update " + mapIndex + "_" + missionIndex + ":hpx=" + hpx + ",damx=" + damx);
                    datas.Add(new LevelConfigObject() { levelToChange = levelToChange, hpx = hpx, damx = damx });
                }
                //LevelRawDataController.UpdateMissionInfor(mapIndex, missionIndex, missionInfor);
            }
        }

        if (datas.Count > 0)
        {
            LevelConfigFirebaseObject levelFirebaseObject = new LevelConfigFirebaseObject();
            levelFirebaseObject.Version = dataVersion;
            levelFirebaseObject.LevelConfigDatas = datas.ToArray();
            string outputData = JsonUtility.ToJson(levelFirebaseObject);
            Debug.Log(outputData);
            GUIUtility.systemCopyBuffer = outputData;
            Debug.Log("Copy To Buffer");
        }
        else
        {
            Debug.Log("Nothing to Do");
        }
        file.Close();
    }

    public static void FillDataFromIngame()
    {
        FileStream file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        XSSFWorkbook workbook = new XSSFWorkbook(file);
        ISheet sheet = workbook.GetSheet("LevelValues");
        List<int> missionIndexs = new List<int>();
        int mapNumber = LevelRawDataController.MapNumber();
        for (int i = 0; i < mapNumber; i++)
        {
            MapInfo map = LevelRawDataController.LoadMapData(i);
            int missionNumber = map.missionNumber;
            for (int j = 0; j < missionNumber; j++)
            {
                int missionIndex = (i + 1) * 1000 + (j + 1);
                missionIndexs.Add(missionIndex);
            }
        }
        for (int i = 0; i < missionIndexs.Count; i++)
        {
            IRow row = sheet.GetRow(i + 1);
            if (row == null)
                row = sheet.CreateRow(i + 1);
            ICell cell = row.GetCell(0);
            if (cell == null)
                cell = row.CreateCell(0);
            Debug.Log("set Cell " + missionIndexs[i]);
            cell.SetCellValue(missionIndexs[i]);
        }

        //string output = Application.dataPath + "/Core/Excels/Report/LevelDataReport" + date + ".xlsx";
        file.Close();
        FileStream fileWrite = new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite);
        workbook.Write(fileWrite);
        //fileWrite.Close();
        Debug.Log("Save " + filePath);
    }

    public static void DownloadFromExcel()
    {
        using (WebClient wc = new WebClient())
        {
            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            wc.DownloadFileAsync(
                // Param1 = Link of file
                new System.Uri(fileLink),
                // Param2 = Path to save
                filePathTest
            );
        }
    }
    static void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        Debug.Log("Dowloaded " + e.ProgressPercentage);
    }
}

[System.Serializable]
public class LevelConfigFirebaseObject
{
    public int Version;
    public LevelConfigObject[] LevelConfigDatas;
}

[Serializable]
public class LevelConfigObject
{
    public int levelToChange;
    public float hpx;
    public float damx;
    public string ToString()
    {
        return levelToChange + "_" + hpx + "_" + damx;
    }
}
