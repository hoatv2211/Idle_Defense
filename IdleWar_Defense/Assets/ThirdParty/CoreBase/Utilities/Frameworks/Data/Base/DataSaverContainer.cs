﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utilities.AntiCheat;
using Utilities.Common;
using Debug = UnityEngine.Debug;

namespace Utilities.Pattern.Data
{
    /// <summary>
    /// NOTE: this class used string mostly to load data because It should be friendly with Unity Editor
    /// </summary>
    public class DataSaverContainer
    {
        /// <summary>
        /// Used to save Key or Data Saver, which lately used for indexing data list
        /// </summary>
        private static readonly string NOTHING = "6597123654782136";
        public static readonly bool USE_PLAYERPREFS = true;
        public static readonly bool USE_BINARY = false;

        public static Dictionary<string, DataSaver> savers = new Dictionary<string, DataSaver>();

        #region Runtime only

        public static DataSaver GetSaver(string pIdString)
        {
            if (savers.ContainsKey(pIdString))
                return savers[pIdString];
            return null;
        }

        public static DataSaver CreateSaver(string pIdString, IEncryption pEncryption)
        {
            if (savers.ContainsKey(pIdString))
            {
                Debug.LogError("Saver " + pIdString + " Existed");
                return null;
            }

            var saver = new DataSaver(pIdString, pEncryption);

            StoreSaverKey(pIdString);
            savers.Add(pIdString, saver);

            return saver;
        }

        private static void StoreSaverKey(string pSaverKey)
        {
            string saverKeysStr = PlayerPrefs.GetString(NOTHING);
            string[] saverKeys = saverKeysStr.Split(':');
            for (int i = 0; i < saverKeys.Length; i++)
                if (saverKeys[i] == pSaverKey)
                    return;

            if (saverKeys.Length == 0)
                saverKeysStr += pSaverKey;
            else
                saverKeysStr += ":" + pSaverKey;

            PlayerPrefs.SetString(NOTHING, saverKeysStr);
            PlayerPrefs.Save();
        }

        #endregion

        //================================================

        #region Editor Friendly but also work on runtime

        public static string[] GetSaverKeys()
        {
            string saverKeysStr = PlayerPrefs.GetString(NOTHING);
            if (string.IsNullOrEmpty(saverKeysStr))
                return new string[0];

            string[] saverKeys = saverKeysStr.Split(':');
            return saverKeys;
        }

        public static void SetData(string pSaverKey, string pData)
        {
            if (USE_PLAYERPREFS)
            {
                PlayerPrefs.SetString(pSaverKey, pData);
                PlayerPrefs.Save();
            }
            if (USE_BINARY)
            {
                DataBasic.BinarySaver.SaveBinary(pData, pSaverKey);
            }
#if UNITY_EDITOR
            Debug.Log(string.Format("Saved Key: {0}\nData: {1}", pSaverKey, pData));
#endif
        }

        public static string GetData(string pSaverKey)
        {
            string dataStr = "";
            if (USE_PLAYERPREFS)
                dataStr = PlayerPrefs.GetString(pSaverKey, "");
            if (USE_BINARY)
                dataStr = DataBasic.BinarySaver.LoadBinary(pSaverKey);
            return dataStr;
        }

        /// <summary>
        /// Get json data from all savers
        /// </summary>
        public static string GetAllData()
        {
            var saverKeys = GetSaverKeys();
            var saverBrands = new List<KeyValue>();
            foreach (var saverKey in saverKeys)
            {
                if (string.IsNullOrEmpty(saverKey))
                    continue;

                string data = GetData(saverKey);
                saverBrands.Add(new KeyValue(saverKey, data));
            }
            string jsonData = JsonHelper.ListToJson(saverBrands);
            return jsonData;
        }

        public static void DeleteAll()
        {
            if (Application.isPlaying)
            {
                foreach (var saver in savers)
                    saver.Value.RemoveAll();
                //NOTE: we need some kind of reload function after deleting in realtime
            }
            else
            {
                var saverKeys = GetSaverKeys();
                for (int i = 0; i < saverKeys.Length; i++)
                {
                    if (USE_PLAYERPREFS)
                        PlayerPrefs.DeleteKey(saverKeys[i]);
                    if (USE_BINARY)
                        DataBasic.BinarySaver.DeleteFile(saverKeys[i]);
                }
            }
        }

        public static void ImportData(string pJsonData)
        {
            var saverBrands = JsonHelper.GetJsonList<KeyValue>(pJsonData);
            if (saverBrands != null)
            {
                foreach (var brand in saverBrands)
                {
                    if (USE_PLAYERPREFS)
                        PlayerPrefs.SetString(brand.k, brand.v);
                    if (USE_BINARY)
                        DataBasic.BinarySaver.SaveBinary(brand.v, brand.k);
                    Debug.Log(string.Format("Restored {0}\n{1}", brand.k, brand.v));
                }
                if (USE_PLAYERPREFS)
                    PlayerPrefs.Save();
            }

        }

        public static void LogData()
        {
            bool hasData = false;
            var saverKeys = GetSaverKeys();
            foreach (var k in saverKeys)
            {
                if (string.IsNullOrEmpty(k))
                    continue;

                hasData = true;
                Debug.Log(string.Format("Key {0}: {1}", k, GetData(k)));
            }
            if (!hasData)
                Debug.Log("No Data");
        }

        /// <summary>
        /// Get keys and values from data saver from saver
        /// </summary>
        public static List<KeyValue> GetAllDataKeyValues(string pSaverKey)
        {
            string data = GetData(pSaverKey);
            return JsonHelper.GetJsonList<KeyValue>(data);
        }

        /// <summary>
        /// Get keys and values from all data saver from savers
        /// </summary>
        public static Dictionary<string, List<KeyValue>> GetAllDataKeyValues()
        {
            var saverKeys = GetSaverKeys();
            var dictKeyValues = new Dictionary<string, List<KeyValue>>();
            foreach (var saverKey in saverKeys)
            {
                if (string.IsNullOrEmpty(saverKey))
                    continue;

                var keyValues = GetAllDataKeyValues(saverKey);
                dictKeyValues.Add(saverKey, keyValues);
            }
            return dictKeyValues;
        }

        #endregion

        //================================================

        #region Editor Only

        public static void BackupData(string pFilePath)
        {
            string jsonData = GetAllData();
            using (StreamWriter sw = new StreamWriter(pFilePath))
            {
                sw.WriteLine(jsonData);
                sw.Close();
                Debug.Log("Backup Success full\n" + jsonData);
            }
        }

        public static void RestoreData(string pFilePath)
        {
            using (StreamReader sw = new StreamReader(pFilePath))
            {
                var jsonData = sw.ReadToEnd();
                if (!string.IsNullOrEmpty(jsonData))
                    ImportData(jsonData);
            }
        }

        #endregion
    }
}