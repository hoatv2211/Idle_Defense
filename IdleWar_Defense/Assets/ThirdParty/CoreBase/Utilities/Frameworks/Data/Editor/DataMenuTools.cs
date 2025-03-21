﻿using UnityEditor;
using UnityEngine;
using Utilities.Common;
using Debug = UnityEngine.Debug;

namespace Utilities.Pattern.Data
{
    public class DataMenuTools
    {
        [MenuItem("DevTools/Data/Open Data Window %_#_'")]
        private static void OpenDataWindow()
        {
            var window = EditorWindow.GetWindow<DataWindow>("Game Data", true);
            window.Show();
        }

        [MenuItem("DevTools/Data/Clear PlayerPrefs")]
        private static void ClearPlayerPrefs()
        {
            EditorHelper.ConfimPopup(() => { PlayerPrefs.DeleteAll(); });
        }

        /*
        [MenuItem("DevTools/Data/Clear Game Data")]
        private static void ClearSaveData()
        {
            EditorHelper.ConfimPopup(() => { DataSaverContainer.DeleteAll(); });
        }

        [MenuItem("DevTools/Data/Backup Game Data")]
        private static void BackUpData()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Backup", "GameData_" + System.DateTime.Now.ToString().Replace("/", "_").Replace(":", "_")
                + ".txt", "txt", "Please enter a file name to save!");
            if (!string.IsNullOrEmpty(path))
            {
                DataSaverContainer.BackupData(path);
            }
        }

        [MenuItem("DevTools/Data/Restore Game Data")]
        private static void RestoreData()
        {
            string path = EditorUtility.OpenFilePanel("Select Backup Data File", Application.dataPath, "txt");
            if (!string.IsNullOrEmpty(path))
            {
                DataSaverContainer.RestoreData(path);
            }
        }

        [MenuItem("DevTools/Data/Log Game Data")]
        private static void LogData()
        {
            DataSaverContainer.LogData();
        }

        [MenuItem("DevTools/Data/Save Game Data (In Game)")]
        private static void Save()
        {
            if (!Application.isPlaying)
            {
                Debug.Log("This Function should be called in Playing!");
                return;
            }
            DataManager.Instance.Save(true);
        }
        */
    }
}