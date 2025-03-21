﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities.AntiCheat;
using Utilities.Common;

namespace Utilities.Pattern.Data
{
    public class DataWindow : EditorWindow
    {
        private Dictionary<string, List<KeyValue>> mDictKeyValues;
        private Vector2 scrollPosition;

        private void OnEnable()
        {
            mDictKeyValues = DataSaverContainer.GetAllDataKeyValues();
        }

        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
            EditorHelper.BoxVertical("All Game Data", () =>
            {
                foreach (var keyValues in mDictKeyValues)
                {
                    var list = keyValues.Value;

                    ListKeyValues(ref list, keyValues.Key);
                }
            });
            EditorHelper.BoxVertical(() =>
            {
                var actions = new List<IDraw>();
                actions.Add(new EditorButton()
                {
                    label = "Clear",
                    onPressed = () =>
                    {
                        EditorHelper.ConfimPopup(() =>
                        {
                            DataSaverContainer.DeleteAll();
                            mDictKeyValues = DataSaverContainer.GetAllDataKeyValues();
                            Repaint();
                        });
                    }
                });
                actions.Add(new EditorButton()
                {
                    label = "Back Up",
                    onPressed = () =>
                    {
                        string path = EditorUtility.SaveFilePanelInProject("Save Backup", "GameData_" + System.DateTime.Now.ToString().Replace("/", "_").Replace(":", "_")
                            + ".txt", "txt", "Please enter a file name to save!");
                        if (!string.IsNullOrEmpty(path))
                        {
                            DataSaverContainer.BackupData(path);
                        }
                    }
                });
                actions.Add(new EditorButton()
                {
                    label = "Restore",
                    onPressed = () =>
                    {
                        string path = EditorUtility.OpenFilePanel("Select Backup Data File", Application.dataPath, "txt");
                        if (!string.IsNullOrEmpty(path))
                        {
                            DataSaverContainer.RestoreData(path);
                            mDictKeyValues = DataSaverContainer.GetAllDataKeyValues();
                            Repaint();
                        }
                    }
                });
                actions.Add(new EditorButton()
                {
                    label = "Log",
                    onPressed = DataSaverContainer.LogData
                });
                actions.Add(new EditorButton()
                {
                    label = "Save Manually (In Game)",
                    color = Application.isPlaying ? Color.yellow : Color.grey,
                    onPressed = () =>
                    {
                        if (!Application.isPlaying)
                        {
                            UnityEngine.Debug.Log("This Function should be called in Playing!");
                            return;
                        }
                        foreach (var saver in DataSaverContainer.savers)
                            saver.Value.Save(true);
                    },
                });
                EditorHelper.GridDraws(2, actions);
            }, Color.yellow, true);
            GUILayout.EndScrollView();
        }

        public void ListKeyValues(ref List<KeyValue> pList, string pSaverKey)
        {
            if (pList == null)
                return;

            GUILayout.Space(3);

            var prevColor = GUI.color;
            GUI.backgroundColor = new Color(1, 1, 0.5f);

            var show = EditorHelper.HeaderFoldout(string.Format("{0} ({1})", pSaverKey, pList.Count), pSaverKey);
            var list = pList;
            if (show)
            {
                int page = EditorPrefs.GetInt(pSaverKey + "_page", 0);
                int totalPages = Mathf.CeilToInt(list.Count * 1f / 20f);
                if (page < 0) page = 0;
                int from = page * 20;
                int to = page * 20 + 20;
                if (to >= list.Count)
                    to = list.Count - 1;

                EditorHelper.BoxVertical(() =>
                {
                    if (totalPages > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (EditorHelper.Button("<Prev<"))
                        {
                            if (page > 0) page--;
                            EditorPrefs.SetInt(pSaverKey + "_page", page);
                        }
                        EditorGUILayout.LabelField(string.Format("{0}-{1} ({2})", from + 1, to, list.Count));
                        if (EditorHelper.Button(">Next>"))
                        {
                            if (page < totalPages - 1) page++;
                            EditorPrefs.SetInt(pSaverKey + "_page", page);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorHelper.BoxHorizontal(() =>
                    {
                        EditorHelper.LabelField("#", 40);
                        EditorHelper.LabelField("Key", 100);
                        EditorHelper.LabelField("Value", 100);
                    });
                    for (int i = from; i <= to; i++)
                    {
                        EditorHelper.BoxHorizontal(() =>
                        {
                            EditorHelper.LabelField((from + i + 1).ToString(), 40);
                            list[i].k = EditorHelper.TextField(list[i].k, "", 0, 100);
                            list[i].v = EditorHelper.TextField(list[i].v, "", 0, 200);
                        });
                    }
                    if (totalPages > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (EditorHelper.Button("<Prev<"))
                        {
                            if (page > 0) page--;
                            EditorPrefs.SetInt(pSaverKey + "_page", page);
                        }
                        EditorGUILayout.LabelField(string.Format("{0}-{1} ({2})", from + 1, to, list.Count));
                        if (EditorHelper.Button(">Next>"))
                        {
                            if (page < totalPages - 1) page++;
                            EditorPrefs.SetInt(pSaverKey + "_page", page);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (EditorHelper.Button("Sort"))
                        list.Sort();
                    if (Application.isPlaying)
                        EditorGUILayout.HelpBox("Cannot Apply Changes In Play Mode", UnityEditor.MessageType.Warning);
                    else
                    {
                        if (EditorHelper.ButtonColor("Apply Changes", Color.green))
                        {
                            string dataStr = JsonHelper.ListToJson(list);
                            DataSaverContainer.SetData(pSaverKey, dataStr);
                            mDictKeyValues = DataSaverContainer.GetAllDataKeyValues();
                        }
                    }
                }, default(Color), true);
            }
            pList = list;

            if (GUI.changed)
                EditorPrefs.SetBool(pSaverKey, show);

            GUI.backgroundColor = prevColor;
        }
    }
}
