﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Utilities.AntiCheat;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utilities.Pattern.Data
{
    public class DataManager : MonoBehaviour
    {
        #region Members

        private const float MIN_TIME_BETWEEN_SAVES = 10;

        private float mLastTimeSave;
        private DataGroup mExampleGroup;
        /// <summary>
        /// Key is saver id string
        /// Children are first generations of this saver
        /// </summary>
        private Dictionary<string, List<DataGroup>> mMainGroups = new Dictionary<string, List<DataGroup>>();

        #endregion

        //===========================================

        #region MonoBehaviour

        public void OnApplicationPause(bool paused)
        {
            foreach (var item in mMainGroups)
                foreach (var g in item.Value)
                    g.OnApplicationPaused(paused);

            if (paused)
                Save();
        }

        public void OnApplicationQuit()
        {
            foreach (var item in mMainGroups)
                foreach (var g in item.Value)
                    g.OnApplicationQuit();
            Save(true);
        }

        #endregion

        //===========================================

        #region Public

        /// <summary>
        /// Step 0: Preparation, add all main data groups to the manager
        /// </summary>
        protected T AddMainDataGroup<T>(T pDataGroup, DataSaver pSaver) where T : DataGroup
        {
            if (!mMainGroups.ContainsKey(pSaver.idString))
                mMainGroups.Add(pSaver.idString, new List<DataGroup>());
            else
            {
                var list = mMainGroups[pSaver.idString];
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Id == pDataGroup.Id)
                        Debug.LogError($"Main Data Group with id {pDataGroup.Id} is already existed");
                }
            }
            mMainGroups[pSaver.idString].Add(pDataGroup);
            return pDataGroup;
        }

        /// <summary>
        /// Step 1: Init main data groups
        /// </summary>
        public virtual void Init()
        {
            Load();
            PostLoad();
        }

        /// <summary>
        /// Discard all changes, back to last data save
        /// </summary>
        public void Reload(bool pClearCache)
        {
            foreach (var item in mMainGroups)
                foreach (var g in item.Value)
                    g.Reload(pClearCache);
        }

        public void Save(bool pForce = false)
        {
            if (pForce || Time.time - mLastTimeSave > MIN_TIME_BETWEEN_SAVES)
            {
                var groups = new List<DataGroup>();
                foreach (var item in mMainGroups)
                    foreach (var g in item.Value)
                    {
                        if (g.Stage())
                            groups.Add(g);
                    }

                foreach (var g in groups)
                    g.DataSaver.Save(false);

                if (groups.Count > 0)
                    mLastTimeSave = Time.time;
            }
        }

        public void Import(string pJsonData)
        {
            DataSaverContainer.ImportData(pJsonData);
            Reload(true);
        }

        #endregion

        //=================================================

        #region Private

        /// <summary>
        /// Step 2: Load Data Saver
        /// </summary>
        private void Load()
        {
            foreach (var item in mMainGroups)
                foreach (var g in item.Value)
                    g.Load("", item.Key);
        }

        /// <summary>
        /// Step 3: Verify all data
        /// </summary>
        private void PostLoad()
        {
            foreach (var item in mMainGroups)
                foreach (var g in item.Value)
                    g.PostLoad();
        }

        #endregion

        //=================================================

        #region Utilities

        public static string LoadFile(string pPath, IEncryption pEncryption)
        {
            var textAsset = Resources.Load<TextAsset>(pPath);
            if (textAsset != null)
            {
                string content = "";
                if (pEncryption != null)
                    content = pEncryption.Decrypt(textAsset.text);
                else
                    content = textAsset.text;
                Resources.UnloadAsset(textAsset);
                return content;
            }
            else
                Debug.LogError($"File {pPath} not found");
            return "";
        }

        #endregion
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(DataManager), true)]
    public class DataManagerEditor : Editor
    {
        protected DataManager mTarget;

        private void OnEnable()
        {
            mTarget = target as DataManager;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Clear"))
            {
                DataSaverContainer.DeleteAll();
            }
            if (GUILayout.Button("Back Up"))
            {
                string path = EditorUtility.SaveFilePanelInProject("Save Backup", "GameData_" + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_")
                            + ".txt", "txt", "Please enter a file name to save!");
                if (!string.IsNullOrEmpty(path))
                    DataSaverContainer.BackupData(path);
            }
            if (GUILayout.Button("Restore"))
            {
                string jsonData = EditorUtility.OpenFilePanel("Select Backup Data File", Application.dataPath, "txt");
                if (!string.IsNullOrEmpty(jsonData))
                    DataSaverContainer.RestoreData(jsonData);
            }
            if (GUILayout.Button("Log"))
            {
                DataSaverContainer.LogData();
            }
            if (GUILayout.Button("Save (In Game)"))
            {
                if (!Application.isPlaying)
                    return;

                foreach (var saver in DataSaverContainer.savers)
                    saver.Value.Save(true);
            }
        }
    }
#endif
}