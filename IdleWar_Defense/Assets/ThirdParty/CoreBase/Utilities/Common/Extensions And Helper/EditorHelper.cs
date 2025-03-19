#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.Common
{
    public interface IDraw
    {
        void Draw();
    }

    public class EditorColor : IDraw
    {
        public string label;
        public int labelWidth = 80;
        public int valueWidth;
        public Color value;
        public Color outputValue;

        public void Draw()
        {
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }

            if (value == null)
                value = default(Color);

            Color color;
            if (valueWidth == 0)
                color = EditorGUILayout.ColorField(value, GUILayout.Height(16), GUILayout.MinWidth(40));
            else
                color = EditorGUILayout.ColorField(value, GUILayout.Height(20), GUILayout.MinWidth(40), GUILayout.Width(valueWidth));
            outputValue = color;

            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.EndHorizontal();
        }
    }

    public class EditorButton : IDraw
    {
        public string label;
        public Color color;
        public int width;
        public Action onPressed;
        public bool isPressed { get; private set; }

        public void Draw()
        {
            var defaultColor = GUI.backgroundColor;
            var style = new GUIStyle("Button");
            if (width > 0)
                style.fixedWidth = width;

            if (color != default(Color))
                GUI.backgroundColor = color;
            isPressed = GUILayout.Button(label, style);
            if (isPressed && onPressed != null)
                onPressed();
            GUI.backgroundColor = defaultColor;
        }
    }

    public class EditorText : IDraw
    {
        public string label;
        public int labelWidth = 80;
        public int valueWidth;
        public string value;
        public string outputValue { get; private set; }
        public bool readOnly;

        public void Draw()
        {
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }

            if (value == null)
                value = "";

            var style = new GUIStyle(EditorStyles.textField);
            style.alignment = TextAnchor.MiddleLeft;
            style.margin = new RectOffset(0, 0, 4, 4);
            Color normalColor = style.normal.textColor;
            normalColor.a = readOnly ? 0.5f : 1;
            style.normal.textColor = normalColor;

            string str;
            if (valueWidth == 0)
                str = EditorGUILayout.TextField(value, style, GUILayout.Height(20), GUILayout.MinWidth(40));
            else
                str = EditorGUILayout.TextField(value, style, GUILayout.Height(20), GUILayout.MinWidth(40), GUILayout.Width(valueWidth));
            outputValue = str;

            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.EndHorizontal();
        }
    }

    public class EditorDropdownListString : IDraw
    {
        public string label;
        public int labelWidth = 80;
        public string[] selections;
        public string value;
        public int valueWidth;
        public string outputValue { get; private set; }

        public void Draw()
        {
            if (selections.Length == 0)
            {
                outputValue = "";
                return;
            }

            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }

            int index = 0;

            for (int i = 0; i < selections.Length; i++)
            {
                if (value == selections[i])
                    index = i;
            }

            if (valueWidth != 0)
                index = EditorGUILayout.Popup(index, selections, "DropDown", GUILayout.Width(valueWidth));
            else
                index = EditorGUILayout.Popup(index, selections, "DropDown");

            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.EndHorizontal();

            outputValue = selections[index] == null ? "" : selections[index];
        }
    }

    public class EditorDropdownListInt : IDraw
    {
        public string label;
        public int labelWidth = 80;
        public int[] selections;
        public int value;
        public int valueWidth;
        public int outputValue { get; private set; }

        public void Draw()
        {
            if (selections.Length == 0)
            {
                outputValue = -1;
                return;
            }

            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }

            int index = 0;

            string[] selectsionsStr = new string[selections.Length];
            for (int i = 0; i < selections.Length; i++)
            {
                if (value == selections[i])
                    index = i;
                selectsionsStr[i] = selections[i].ToString();
            }

            if (valueWidth != 0)
                index = EditorGUILayout.Popup(index, selectsionsStr, GUILayout.Width(valueWidth));
            else
                index = EditorGUILayout.Popup(index, selectsionsStr);

            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.EndHorizontal();

            outputValue = selections[index];
        }
    }

    public class EditorDropdownListEnum<T> : IDraw
    {
        public string label;
        public int labelWidth;
        public T value;
        public int valueWidth;
        public T outputValue { get; private set; }

        public void Draw()
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }

            var enumValues = Enum.GetValues(typeof(T));
            string[] selections = new string[enumValues.Length];

            int i = 0;
            foreach (T item in enumValues)
            {
                selections[i] = item.ToString();
                i++;
            }

            int index = 0;
            for (i = 0; i < selections.Length; i++)
            {
                if (value.ToString() == selections[i])
                {
                    index = i;
                }
            }

            if (valueWidth != 0)
                index = EditorGUILayout.Popup(index, selections, GUILayout.Width(valueWidth));
            else
                index = EditorGUILayout.Popup(index, selections);

            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.EndHorizontal();

            i = 0;
            foreach (T item in enumValues)
            {
                if (i == index)
                {
                    outputValue = item;
                    return;
                }
                i++;
            }

            outputValue = default(T);
        }
    }

    public class EditorToggle : IDraw
    {
        public string label;
        public int labelWidth = 80;
        public bool value;
        public int valueWidth;
        public bool readOnly;
        public bool outputValue { get; private set; }

        public void Draw()
        {
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }
            bool result;

            var style = new GUIStyle(EditorStyles.toggle);
            style.alignment = TextAnchor.MiddleCenter;
            style.fixedWidth = 20;
            style.fixedHeight = 20;
            Color normalColor = style.normal.textColor;
            normalColor.a = readOnly ? 0.5f : 1;
            style.normal.textColor = normalColor;

            if (valueWidth == 0)
                result = EditorGUILayout.Toggle(value, style, GUILayout.Height(20), GUILayout.MinWidth(40));
            else
                result = EditorGUILayout.Toggle(value, style, GUILayout.Height(20), GUILayout.MinWidth(40), GUILayout.Width(valueWidth));

            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.EndHorizontal();

            outputValue = result;
        }
    }

    public class EditorFoldout : IDraw
    {
        public string label;
        public Action onFoldout;
        internal bool isFoldout { get; private set; }

        public void Draw()
        {
            isFoldout = EditorPrefs.GetBool(label + "foldout", false);
            isFoldout = EditorGUILayout.Foldout(isFoldout, label);
            if (isFoldout && onFoldout != null)
                onFoldout();
            if (GUI.changed)
                EditorPrefs.SetBool(label + "foldout", isFoldout);
        }
    }

    public class EditorTabs : IDraw
    {
        public string key;
        public string[] tabsName;
        public string currentTab { get; private set; }

        public void Draw()
        {
            currentTab = EditorPrefs.GetString(string.Format("{0}_current_tab", key), tabsName[0]);

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            foreach (var tabName in tabsName)
            {
                var buttonStyle = new GUIStyle(EditorStyles.toolbarButton);
                buttonStyle.fixedHeight = 0;
                buttonStyle.padding = new RectOffset(4, 4, 4, 4);
                buttonStyle.normal.textColor = Color.white;
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.fontSize = 13;

                var preColor = GUI.color;
                var color = currentTab == tabName ? Color.yellow : new Color(0.5f, 0.5f, 0.5f);
                GUI.color = color;

                if (GUILayout.Button(tabName, buttonStyle))
                {
                    currentTab = tabName;
                    EditorPrefs.SetString(string.Format("{0}_current_tab", key), currentTab);
                }
                GUI.color = preColor;
            }
            GUILayout.EndHorizontal();
        }
    }

    public class EditorHeaderFoldout : IDraw
    {
        public string key;
        public bool minimalistic = false;
        public string label;
        public Action onFoldout;
        public bool isFoldout { get; private set; }

        public void Draw()
        {
            isFoldout = EditorPrefs.GetBool(key, false);

            if (!minimalistic) GUILayout.Space(3f);
            if (!isFoldout) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;

            if (minimalistic)
            {
                if (isFoldout) label = "\u25BC" + (char)0x200a + label;
                else label = "\u25BA" + (char)0x200a + label;

                GUILayout.BeginHorizontal();
                GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
                if (!GUILayout.Toggle(true, label, "PreToolbar2", GUILayout.MinWidth(20f)))
                    isFoldout = !isFoldout;
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                if (isFoldout) label = "\u25BC " + label;
                else label = "\u25BA " + label;
                string styleString = isFoldout ? "Button" : "DropDownButton";
                var style = new GUIStyle(styleString);
                style.alignment = TextAnchor.MiddleLeft;
                style.fontSize = 11;
                style.fontStyle = isFoldout ? FontStyle.Bold : FontStyle.Normal;
                if (!GUILayout.Toggle(true, label, style, GUILayout.MinWidth(20f)))
                    isFoldout = !isFoldout;
            }

            if (isFoldout && onFoldout != null)
                onFoldout();

            if (GUI.changed) EditorPrefs.SetBool(key, isFoldout);

            if (!minimalistic) GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!isFoldout) GUILayout.Space(3f);
        }
    }

    public class EditorObject<T> : IDraw
    {
        public Object value;
        public string label;
        public int labelWidth = 80;
        public int valueWidth;
        public bool showAsBox;
        public Object outputValue { get; private set; }

        public void Draw()
        {
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }

            if (valueWidth == 0 && showAsBox)
                valueWidth = 50;

            Object result;

            if (showAsBox)
                result = EditorGUILayout.ObjectField(value, typeof(T), true, GUILayout.Width(valueWidth), GUILayout.Height(valueWidth));
            else
            {
                if (valueWidth == 0)
                    result = EditorGUILayout.ObjectField(value, typeof(T), true);
                else
                    result = EditorGUILayout.ObjectField(value, typeof(T), true, GUILayout.Width(valueWidth));
            }

            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.EndHorizontal();

            outputValue = result;
        }
    }

    public class EditorInt : IDraw
    {
        public string label;
        public int labelWidth = 80;
        public int valueWidth;
        public int value;
        public bool readOnly;
        public int outputValue { get; private set; }

        public void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));

            var style = new GUIStyle(EditorStyles.textField);
            style.alignment = TextAnchor.MiddleLeft;
            style.margin = new RectOffset(0, 0, 4, 4);
            Color normalColor = style.normal.textColor;
            normalColor.a = readOnly ? 0.5f : 1;
            style.normal.textColor = normalColor;

            int result;
            if (valueWidth == 0)
                result = EditorGUILayout.IntField(value, style, GUILayout.Height(20), GUILayout.MinWidth(40));
            else
                result = EditorGUILayout.IntField(value, style, GUILayout.Height(20), GUILayout.MinWidth(40), GUILayout.Width(valueWidth));

            EditorGUILayout.EndHorizontal();

            outputValue = result;
        }
    }

    public static class EditorHelper
    {
        #region File Utilities

        /// <summary>
        /// T must be Serializable
        /// </summary>
        public static void SaveJsonPanel<T>(string pMainDirectory, string defaultName, T obj)
        {
            if (string.IsNullOrEmpty(pMainDirectory))
                pMainDirectory = Application.dataPath;

            string path = EditorUtility.SaveFilePanel("Save File", pMainDirectory, defaultName, "json");
            if (!string.IsNullOrEmpty(path))
                SaveToJsonFile(path, obj);
        }

        public static void SaveToJsonFile<T>(string pPath, T pObj)
        {
            string jsonString = JsonUtility.ToJson(pObj);
            if (!string.IsNullOrEmpty(jsonString) && jsonString != "{}")
            {
                if (File.Exists(pPath))
                    File.Delete(pPath);
                File.WriteAllText(pPath, jsonString);
            }
        }

        /// <summary>
        /// T must be Serializable
        /// </summary>
        public static bool LoadJsonPanel<T>(string pMainDirectory, ref T pOutput)
        {
            if (string.IsNullOrEmpty(pMainDirectory))
                pMainDirectory = Application.dataPath;

            string path = EditorUtility.OpenFilePanel("Open File", pMainDirectory, "json");
            if (string.IsNullOrEmpty(path))
                return false;
            else
                return LoadJsonFromFile(path, ref pOutput);
        }

        public static bool LoadJsonFromFile<T>(string pPath, ref T pOutput)
        {
            if (!string.IsNullOrEmpty(pPath))
            {
                pOutput = JsonUtility.FromJson<T>(File.ReadAllText(pPath));
                return true;
            }
            return false;
        }

        public static void SaveToXMLFile<T>(string pPath, T pObj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StreamWriter(pPath))
            {
                if (File.Exists(pPath))
                    File.Delete(pPath);
                serializer.Serialize(writer, pObj);
            }
        }

        public static void LoadFromXMLFile<T>(string pPath, ref T pObj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StreamReader(pPath))
                pObj = (T)serializer.Deserialize(reader);
        }

        #endregion

        //========================================

        #region Quick Shortcut

        /// <summary>
        /// Find all scene components, active or inactive.
        /// </summary>
        public static List<T> FindAll<T>() where T : Component
        {
            T[] comps = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];

            List<T> list = new List<T>();

            foreach (T comp in comps)
            {
                if (comp.gameObject.hideFlags == 0)
                {
                    string path = AssetDatabase.GetAssetPath(comp.gameObject);
                    if (string.IsNullOrEmpty(path)) list.Add(comp);
                }
            }
            return list;
        }

        public static void Save()
        {
            AssetDatabase.SaveAssets();
        }

        public static T CreateScriptableAsset<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            var directoryPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
            return asset;
        }

        public static string GetObjectPath(Object pObj)
        {
            return AssetDatabase.GetAssetPath(pObj);
        }

        public static Object LoadAsset(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return AssetDatabase.LoadMainAssetAtPath(path);
        }

        /// <summary>
        /// Convenience function to load an asset of specified type, given the full path to it.
        /// </summary>
        public static T LoadAsset<T>(string path) where T : Object
        {
            Object obj = LoadAsset(path);
            if (obj == null) return null;

            T val = obj as T;
            if (val != null) return val;

            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                if (obj.GetType() == typeof(GameObject))
                {
                    GameObject go = obj as GameObject;
                    return go.GetComponent(typeof(T)) as T;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the specified object's GUID.
        /// </summary>
        public static string ObjectToGUID(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return (!string.IsNullOrEmpty(path)) ? AssetDatabase.AssetPathToGUID(path) : null;
        }

        #endregion

        //=============================

        #region Layout

        public static void BoxVertical(Action doSomthing, Color color = default(Color), bool isBox = false, float pFixedWith = 0)
        {
            var defaultColor = GUI.backgroundColor;
            if (color != default(Color))
                GUI.backgroundColor = color;

            if (!isBox)
            {
                if (pFixedWith > 0) EditorGUILayout.BeginVertical(GUILayout.Width(pFixedWith));
                else EditorGUILayout.BeginVertical();
            }
            else
            {
                var style = new GUIStyle(EditorStyles.helpBox);
                if (pFixedWith > 0) style.fixedWidth = pFixedWith;
                EditorGUILayout.BeginVertical(style);
            }

            doSomthing();

            EditorGUILayout.EndVertical();
            if (color != default(Color))
                GUI.backgroundColor = defaultColor;
        }

        public static void BoxVertical(string pTitle, Action doSomthing, Color color = default(Color), bool isBox = false, float pFixedWith = 0)
        {
            var defaultColor = GUI.backgroundColor;
            if (color != default(Color))
                GUI.backgroundColor = color;

            if (!isBox)
            {
                if (pFixedWith > 0) EditorGUILayout.BeginVertical(GUILayout.Width(pFixedWith));
                else EditorGUILayout.BeginVertical();
            }
            else
            {
                var style = new GUIStyle(EditorStyles.helpBox);
                if (pFixedWith > 0) style.fixedWidth = pFixedWith;
                EditorGUILayout.BeginVertical(style);
            }

            if (!string.IsNullOrEmpty(pTitle))
                DrawHeaderTitle(pTitle);

            doSomthing();

            EditorGUILayout.EndVertical();
            if (color != default(Color))
                GUI.backgroundColor = defaultColor;
        }

        public static void BoxHorizontal(Action doSomthing, Color color = default(Color), bool isBox = false, float pFixedWith = 0)
        {
            var defaultColor = GUI.backgroundColor;
            if (color != default(Color))
                GUI.backgroundColor = color;

            if (!isBox)
            {
                if (pFixedWith > 0) EditorGUILayout.BeginHorizontal(GUILayout.Width(pFixedWith));
                else EditorGUILayout.BeginHorizontal();
            }
            else
            {
                var style = new GUIStyle(EditorStyles.helpBox);
                if (pFixedWith > 0) style.fixedWidth = pFixedWith;
                EditorGUILayout.BeginHorizontal(style);
            }

            doSomthing();

            EditorGUILayout.EndHorizontal();

            if (color != default(Color))
                GUI.backgroundColor = defaultColor;
        }

        public static void BoxHorizontal(string pTitle, Action doSomthing, Color color = default(Color), bool isBox = false, float pFixedWith = 0)
        {
            var defaultColor = GUI.backgroundColor;
            if (color != default(Color))
                GUI.backgroundColor = color;

            if (!string.IsNullOrEmpty(pTitle))
            {
                EditorGUILayout.BeginVertical();
                DrawHeaderTitle(pTitle);
            }

            if (!isBox)
            {
                if (pFixedWith > 0) EditorGUILayout.BeginHorizontal(GUILayout.Width(pFixedWith));
                else EditorGUILayout.BeginHorizontal();
            }
            else
            {
                var style = new GUIStyle(EditorStyles.helpBox);
                if (pFixedWith > 0) style.fixedWidth = pFixedWith;
                EditorGUILayout.BeginHorizontal(style);
            }

            doSomthing();

            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(pTitle))
                EditorGUILayout.EndVertical();

            if (color != default(Color))
                GUI.backgroundColor = defaultColor;
        }

        public static void GridDraws(int pCell, List<IDraw> pDraws)
        {
            int row = Mathf.CeilToInt(pDraws.Count * 1f / pCell);
            for (int i = 0; i < row; i++)
            {
                BoxHorizontal(() =>
                {
                    for (int j = 0; j < pCell; j++)
                    {
                        int index = i * pCell + j;
                        if (index < pDraws.Count)
                            pDraws[index].Draw();
                    }
                });
            }
        }

        public static void Seperator()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        /// <summary>
        /// Draw a visible separator in addition to adding some padding.
        /// </summary>
        public static void SeperatorBox()
        {
            GUILayout.Space(10);

            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = EditorGUIUtility.whiteTexture;
                Rect rect = GUILayoutUtility.GetLastRect();
                GUI.color = new Color(0f, 0f, 0f, 0.25f);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
                GUI.color = Color.white;
            }
        }

        public static Vector2 ScrollBar(ref Vector2 scrollPos, float width, float height, string label, Action action)
        {
            EditorGUILayout.BeginVertical("box");
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(height));
            action();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            return scrollPos;
        }

        #endregion

        //=============================

        #region Tools

        public static bool Button(string pLabel, int pWidth = 0)
        {
            var button = new EditorButton()
            {
                label = pLabel,
                width = pWidth
            };
            button.Draw();
            return button.isPressed;
        }

        public static void Button(string pLabel, Action pOnPressed, int pWidth = 0)
        {
            var button = new EditorButton()
            {
                label = pLabel,
                width = pWidth,
                onPressed = pOnPressed,
            };
            button.Draw();
        }

        public static bool ButtonColor(string pLabel, Color pColor = default(Color), int pWidth = 0)
        {
            var button = new EditorButton()
            {
                label = pLabel,
                width = pWidth,
                color = pColor,
            };
            button.Draw();
            return button.isPressed;
        }

        public static void ButtonColor(string pLabel, Action pOnPressed, Color pColor = default(Color), int pWidth = 0)
        {
            var button = new EditorButton()
            {
                label = pLabel,
                width = pWidth,
                color = pColor,
                onPressed = pOnPressed
            };
            button.Draw();
        }

        public static string FolderSelector(string label, string pSavingKey, string mDefaultPath = null, bool pFormatToUnityPath = true)
        {
            if (mDefaultPath == null)
                mDefaultPath = Application.dataPath;
            if (pFormatToUnityPath)
                mDefaultPath = FormatPathToUnityPath(mDefaultPath).Replace("Assets", "");
            string savedPath = EditorPrefs.GetString(label + pSavingKey, mDefaultPath);
            var text = new EditorText()
            {
                label = label,
                value = savedPath,
                labelWidth = label.Length * 8
            };
            var button = new EditorButton()
            {
                label = "...",
                width = 25,
                onPressed = () =>
                {
                    string path = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (pFormatToUnityPath)
                            path = FormatPathToUnityPath(path).Replace("Assets", "");
                        EditorPrefs.SetString(label + pSavingKey, path);
                        savedPath = path;
                    }
                }
            };

            BoxHorizontal(() =>
            {
                text.Draw();
                button.Draw();
            });
            return savedPath;
        }

        public static string FileSelector(string label, string pSavingKey, string extension, bool pFormatToUnityPath = true)
        {
            string savedPath = EditorPrefs.GetString(label + pSavingKey);
            var text = new EditorText()
            {
                label = label,
                value = savedPath,
                labelWidth = label.Length * 8
            };
            var button = new EditorButton()
            {
                label = "...",
                width = 25,
                onPressed = () =>
                {
                    string path = EditorUtility.OpenFilePanel("Select File", Application.dataPath, extension);
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (pFormatToUnityPath)
                            path = FormatPathToUnityPath(path).Replace("Assets", "");
                        EditorPrefs.SetString(label + pSavingKey, path);
                        savedPath = path;
                    }
                }
            };

            BoxHorizontal(() =>
            {
                text.Draw();
                button.Draw();
            });
            return savedPath;
        }

        public static bool Foldout(string label)
        {
            var foldout = new EditorFoldout()
            {
                label = label,
            };
            foldout.Draw();
            return foldout.isFoldout;
        }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>
        public static bool HeaderFoldout(string label, string key = "", bool minimalistic = false, params IDraw[] pHorizontalDraws)
        {
            var headerFoldout = new EditorHeaderFoldout()
            {
                key = string.IsNullOrEmpty(key) ? label : key,
                label = label,
                minimalistic = minimalistic,
            };
            if (pHorizontalDraws != null)
                GUILayout.BeginHorizontal();

            headerFoldout.Draw();

            if (pHorizontalDraws != null && headerFoldout.isFoldout)
                foreach (var d in pHorizontalDraws)
                    d.Draw();

            if (pHorizontalDraws != null)
                GUILayout.EndHorizontal();

            return headerFoldout.isFoldout;
        }

        public static void ConfimPopup(Action pOnYes, Action pOnNo = null)
        {
            if (EditorUtility.DisplayDialog("Confirm your action", "Are you sure you want to do this", "Yes", "No"))
                pOnYes();
            else
                if (pOnNo != null) pOnNo();
        }

        public static void ListReadonlyObjects<T>(List<T> pList, string pName, bool pShowObjectBox = true) where T : UnityEngine.Object
        {
            ListObjects(ref pList, pName, pShowObjectBox, true);
        }

        public static bool ListObjects<T>(ref List<T> pList, string pName, bool pShowObjectBox = true, bool pReadOnly = false, IDraw[] pAdditionalDraws = null) where T : UnityEngine.Object
        {
            GUILayout.Space(3);

            var prevColor = GUI.color;
            GUI.backgroundColor = new Color(1, 1, 0.5f);

            var show = HeaderFoldout(string.Format("{0} ({1})", pName, pList.Count), pName);

            var list = pList;
            if (show)
            {
                int page = EditorPrefs.GetInt(pName + "_page", 0);
                int totalPages = Mathf.CeilToInt(list.Count * 1f / 20f);
                if (totalPages == 0) totalPages = 1;
                if (page < 0) page = 0;
                if (page >= totalPages) page = totalPages - 1;
                int from = page * 20;
                int to = page * 20 + 20;
                if (to >= list.Count)
                    to = list.Count - 1;

                BoxVertical(() =>
                {
                    if (!pReadOnly)
                    {
                        DragDropBox<T>(pName, (objs) =>
                        {
                            list.AddRange(objs);
                        });
                    }

                    if (totalPages > 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (Button("<Prev<"))
                        {
                            if (page > 0) page--;
                            EditorPrefs.SetInt(pName + "_page", page);
                        }
                        EditorGUILayout.LabelField(string.Format("{0}-{1} ({2})", from, to, list.Count));
                        if (Button(">Next>"))
                        {
                            if (page < totalPages - 1) page++;
                            EditorPrefs.SetInt(pName + "_page", page);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    for (int i = from; i <= to; i++)
                    {
                        BoxHorizontal(() =>
                        {
                            if (pShowObjectBox)
                                list[i] = (T)ObjectField<T>(list[i], "", 0, 50, true);
                            EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(25));
                            list[i] = (T)ObjectField<T>(list[i], "");
                            if (ButtonColor("x", Color.red, 25))
                                list.RemoveAt(i);
                        });
                    }
                    if (totalPages > 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (Button("<Prev<"))
                        {
                            if (page > 0) page--;
                            EditorPrefs.SetInt(pName + "_page", page);
                        }
                        EditorGUILayout.LabelField(string.Format("{0}-{1} ({2})", from, to, list.Count));
                        if (Button(">Next>"))
                        {
                            if (page < totalPages - 1) page++;
                            EditorPrefs.SetInt(pName + "_page", page);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (!pReadOnly)
                    {
                        BoxHorizontal(() =>
                        {
                            if (ButtonColor("+1", Color.green, 30))
                            {
                                list.Add(null);
                                page = totalPages - 1;
                                EditorPrefs.SetInt(pName + "_page", page);
                            }
                            if (Button("Sort By Name"))
                                list = list.OrderBy(m => m.name).ToList();
                            if (Button("Remove Duplicate"))
                            {
                                List<int> duplicate = new List<int>();
                                for (int i = 0; i < list.Count; i++)
                                {
                                    int count = 0;
                                    for (int j = list.Count - 1; j >= 0; j--)
                                    {
                                        if (list[j] == list[i])
                                        {
                                            count++;
                                            if (count > 1)
                                                duplicate.Add(j);
                                        }
                                    }
                                }
                                for (int j = list.Count - 1; j >= 0; j--)
                                {
                                    if (duplicate.Contains(j))
                                        list.Remove(list[j]);
                                }
                            }
                            if (ButtonColor("Clear", Color.red, 50))
                                ConfimPopup(() => { list = new List<T>(); });
                        });

                    }

                    if (pAdditionalDraws != null)
                        foreach (var draw in pAdditionalDraws)
                            draw.Draw();

                }, default(Color), true);
            }
            pList = list;

            if (GUI.changed)
                EditorPrefs.SetBool(pName, show);

            GUI.backgroundColor = prevColor;

            return show;
        }

        public static void ListObjectsWithSearch<T>(ref List<T> pList, string pName, bool pShowBox = true) where T : UnityEngine.Object
        {
            var prevColor = GUI.color;
            GUI.backgroundColor = new Color(1, 1, 0.5f);

            //bool show = EditorPrefs.GetBool(pName, false);
            //GUIContent content = new GUIContent(pName);
            //GUIStyle style = new GUIStyle(EditorStyles.foldout);
            //style.margin = new RectOffset(pInBox ? 13 : 0, 0, 0, 0);
            //show = EditorGUILayout.Foldout(show, content, style);

            string search = EditorPrefs.GetString(pName + "_search");
            var show = HeaderFoldout(string.Format("{0} ({1})", pName, pList.Count), pName);
            var list = pList;

            if (show)
            {
                int page = EditorPrefs.GetInt(pName + "_page", 0);
                int totalPages = Mathf.CeilToInt(list.Count * 1f / 20f);
                if (totalPages == 0) totalPages = 1;
                if (page < 0) page = 0;
                if (page >= totalPages) page = totalPages - 1;
                int from = page * 20;
                int to = page * 20 + 20;
                if (to >= list.Count)
                    to = list.Count - 1;

                BoxVertical(() =>
                {
                    DragDropBox<T>(pName, (objs) =>
                    {
                        list.AddRange(objs);
                    });

                    if (totalPages > 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (Button("<Prev<"))
                        {
                            if (page > 0) page--;
                            EditorPrefs.SetInt(pName + "_page", page);
                        }
                        EditorGUILayout.LabelField(string.Format("{0}-{1} ({2})", from, to, list.Count));
                        if (Button("<Next<"))
                        {
                            if (page < totalPages - 1) page++;
                            EditorPrefs.SetInt(pName + "_page", page);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    search = GUILayout.TextField(search);

                    bool searching = !string.IsNullOrEmpty(search);
                    for (int i = from; i <= to; i++)
                    {
                        if (searching && !list[i].name.Contains(search))
                            continue;

                        BoxHorizontal(() =>
                        {
                            if (pShowBox)
                                list[i] = (T)ObjectField<T>(list[i], "", 0, 50, true);
                            EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(25));
                            list[i] = (T)ObjectField<T>(list[i], "");
                            if (ButtonColor("x", Color.red, 25))
                                list.RemoveAt(i);
                        });

                    }
                    if (totalPages > 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (Button("<Prev<"))
                        {
                            if (page > 0) page--;
                            EditorPrefs.SetInt(pName + "_page", page);
                        }
                        EditorGUILayout.LabelField(string.Format("{0}-{1} ({2})", from, to, list.Count));
                        if (Button(">Next>"))
                        {
                            if (page < totalPages - 1) page++;
                            EditorPrefs.SetInt(pName + "_page", page);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    BoxHorizontal(() =>
                    {
                        if (ButtonColor("+1", Color.green, 30))
                        {
                            list.Add(null);
                            page = totalPages - 1;
                            EditorPrefs.SetInt(pName + "_page", page);
                        }
                        if (Button("Sort By Name"))
                            list = list.OrderBy(m => m.name).ToList();
                        if (Button("Remove Duplicate"))
                        {
                            List<int> duplicate = new List<int>();
                            for (int i = 0; i < list.Count; i++)
                            {
                                int count = 0;
                                for (int j = list.Count - 1; j >= 0; j--)
                                {
                                    if (list[j] == list[i])
                                    {
                                        count++;
                                        if (count > 1)
                                            duplicate.Add(j);
                                    }
                                }
                            }
                            for (int j = list.Count - 1; j >= 0; j--)
                            {
                                if (duplicate.Contains(j))
                                    list.Remove(list[j]);
                            }
                        }
                        if (ButtonColor("Clear", Color.red, 50))
                            ConfimPopup(() => { list = new List<T>(); });
                    });

                }, default(Color), true);
            }
            pList = list;

            if (GUI.changed)
            {
                EditorPrefs.SetBool(pName, show);
                EditorPrefs.SetString(pName + "_search", search);
            }

            GUI.backgroundColor = prevColor;
        }

        public static string Tabs(string pKey, params string[] pTabsName)
        {
            var tabs = new EditorTabs()
            {
                key = pKey,
                tabsName = pTabsName,
            };
            tabs.Draw();
            return tabs.currentTab;
        }

        private static void DrawHeaderTitle(string pHeader)
        {
            Color prevColor = GUI.color;

            var boxStyle = new GUIStyle(EditorStyles.toolbar);
            boxStyle.fixedHeight = 0;
            boxStyle.padding = new RectOffset(5, 5, 5, 5);

            var titleStyle = new GUIStyle(EditorStyles.largeLabel);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = Color.white;
            titleStyle.alignment = TextAnchor.MiddleCenter;

            GUI.color = new Color(0.5f, 0.5f, 0.5f);
            EditorGUILayout.BeginHorizontal(boxStyle, GUILayout.Height(20));
            {
                GUI.color = prevColor;
                EditorGUILayout.LabelField(pHeader, titleStyle, GUILayout.Height(20));
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void DragDropBox<T>(string pName, Action<T[]> pOnDrop) where T : Object
        {
            Event evt = Event.current;
            var style = new GUIStyle("Toolbar");
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 30, style, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area, "Drag drop " + pName);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        var objs = new List<T>();
                        try
                        {
                            foreach (T obj in DragAndDrop.objectReferences)
                            {
                                if (obj != null)
                                    objs.Add(obj);
                            }
                        }
                        catch
                        {
                            //Incase we drag drop gameobject
                            foreach (GameObject obj in DragAndDrop.objectReferences)
                            {
                                var component = obj.GetComponent<T>();
                                if (component != null)
                                    objs.Add(component);
                            }
                        }
                        pOnDrop(objs.ToArray());
                    }
                    break;
            }
        }

        public static void DrawTextureIcon(Texture pTexture, Vector2 pSize)
        {
            var rect = EditorGUILayout.GetControlRect(GUILayout.Width(pSize.x), GUILayout.Height(pSize.y));
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
            if (pTexture != null)
                GUI.DrawTexture(rect, pTexture, ScaleMode.ScaleToFit);
            else
                GUI.DrawTexture(rect, EditorGUIUtility.FindTexture("console.warnicon"), ScaleMode.ScaleToFit);
        }

        public static ReorderableList CreateReorderableList<T>(T[] pObjects, string pName) where T : Object
        {
            var reorderableList = new ReorderableList(pObjects, typeof(T), true, false, true, true);
            reorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                pObjects[index] = (T)EditorGUI.ObjectField(rect, pObjects[index], typeof(T), true);
            };
            reorderableList.elementHeight = 17f;
            reorderableList.headerHeight = 17f;
            reorderableList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, pName);
            };
            return reorderableList;
        }

        public static ReorderableList CreateReorderableList<T>(List<T> pObjects, string pName) where T : Object
        {
            var reorderableList = new ReorderableList(pObjects, typeof(T), true, false, true, true);
            reorderableList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                pObjects[index] = (T)EditorGUI.ObjectField(rect, pObjects[index], typeof(T), true);
            };
            reorderableList.elementHeight = 17f;
            reorderableList.headerHeight = 17f;
            reorderableList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, pName);
            };
            return reorderableList;
        }

        public static List<AnimationClip> GetAnimClipsFromFBX()
        {
            var list = new List<AnimationClip>();
            var selections = Selection.objects;
            for (int i = 0; i < selections.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(selections[i]);
                var representations = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                foreach (var asset in representations)
                {
                    var clip = asset as AnimationClip;
                    if (clip != null)
                        list.Add(clip);
                }
            }
            return list;
        }

        public static void ReplaceGameobjectsInScene(ref List<GameObject> selections, List<GameObject> prefabs)
        {
            for (var i = selections.Count - 1; i >= 0; --i)
            {
                GameObject newObject = null;
                var selected = selections[i];
                var prefab = prefabs[UnityEngine.Random.Range(0, prefabs.Count)];
                if (prefab.IsPrefab())
                {
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                }
                else
                {
                    newObject = GameObject.Instantiate(prefab);
                    newObject.name = prefab.name;
                }
                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                newObject.transform.parent = selected.transform.parent;
                newObject.transform.localPosition = selected.transform.localPosition;
                newObject.transform.localRotation = selected.transform.localRotation;
                newObject.transform.localScale = selected.transform.localScale;
                newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                Undo.DestroyObjectImmediate(selected);
                selections[i] = newObject;
            }
        }

        #endregion

        //=============================

        #region Input Fields

        public static Color DisableCoor = ColorHelper.HexToColor("#787878");

        public static string TextField(string value, string label, int labelWidth = 80, int valueWidth = 0, bool readOnly = false)
        {
            Color defaultColor = GUI.color;
            EditorText text = new EditorText()
            {
                value = value,
                label = label,
                labelWidth = labelWidth,
                valueWidth = valueWidth,
                readOnly = readOnly,
            };
            text.Draw();
            return text.outputValue;
        }

        public static string DropdownList(string value, string label, string[] selections, int labelWidth = 80, int valueWidth = 0)
        {
            var dropdownList = new EditorDropdownListString()
            {
                label = label,
                labelWidth = labelWidth,
                selections = selections,
                value = value,
                valueWidth = valueWidth,
            };
            dropdownList.Draw();
            return dropdownList.outputValue;
        }

        public static int DropdownList(int value, string label, int[] selections, int labelWidth = 80, int valueWidth = 0)
        {
            var dropdownList = new EditorDropdownListInt()
            {
                label = label,
                labelWidth = labelWidth,
                value = value,
                valueWidth = valueWidth,
                selections = selections
            };
            dropdownList.Draw();
            return dropdownList.outputValue;
        }

        public static T DropdownListEnum<T>(T value, string label, int labelWidth = 80, int valueWidth = 0) where T : struct, IConvertible
        {
            var dropdownList = new EditorDropdownListEnum<T>()
            {
                label = label,
                labelWidth = labelWidth,
                value = value,
                valueWidth = valueWidth,
            };
            dropdownList.Draw();
            return dropdownList.outputValue;
        }

        public static T DropdownList<T>(T selectedObj, string label, List<T> pOptions) where T : UnityEngine.Object
        {
            string selectedName = selectedObj == null ? "None" : selectedObj.name;
            string[] options = new string[pOptions.Count + 1];
            options[0] = "None";
            for (int i = 1; i < pOptions.Count + 1; i++)
            {
                if (pOptions[i - 1] != null)
                    options[i] = pOptions[i - 1].name;
                else
                    options[i] = "NULL";
            }

            var selected = DropdownList(selectedName, label, options);
            if (selectedName != selected)
            {
                selectedName = selected;
                for (int i = 0; i < pOptions.Count; i++)
                {
                    if (pOptions[i].name == selectedName)
                    {
                        selectedObj = pOptions[i];
                        break;
                    }
                }
                if (selectedName == "None")
                    selectedObj = null;
            }
            return selectedObj;
        }

        public static bool Toggle(bool value, string label, int labelWidth = 80, int valueWidth = 0)
        {
            var toggle = new EditorToggle()
            {
                label = label,
                labelWidth = labelWidth,
                value = value,
                valueWidth = valueWidth,
            };
            toggle.Draw();
            return toggle.outputValue;
        }

        public static int IntField(int value, string label, int labelWidth = 80, int valueWidth = 0, bool readOnly = false)
        {
            var intField = new EditorInt()
            {
                value = value,
                label = label,
                labelWidth = labelWidth,
                valueWidth = valueWidth,
                readOnly = readOnly,
            };
            intField.Draw();
            return intField.outputValue;
        }

        public static float FloatField(float value, string label, int labelWidth = 80, int valueWidth = 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }

            float result;
            if (valueWidth == 0)
                result = EditorGUILayout.FloatField(value, GUILayout.Height(20));
            else
                result = EditorGUILayout.FloatField(value, GUILayout.Height(20), GUILayout.Width(valueWidth));

            EditorGUILayout.EndHorizontal();

            return result;
        }

        public static Object ObjectField<T>(Object value, string label, int labelWidth = 80, int valueWidth = 0, bool showAsBox = false)
        {
            var obj = new EditorObject<T>()
            {
                value = value,
                label = label,
                labelWidth = labelWidth,
                valueWidth = valueWidth,
                showAsBox = showAsBox
            };
            obj.Draw();
            return obj.outputValue;
        }

        public static void LabelField(string label, int width = 0, bool isBold = true)
        {
            var style = new GUIStyle(isBold ? EditorStyles.boldLabel : EditorStyles.label);
            if (width > 0)
                EditorGUILayout.LabelField(label, style, GUILayout.Width(width));
            else
                EditorGUILayout.LabelField(label, style);
        }

        public static Color ColorField(Color value, string label, int labelWidth = 80, int valueWidth = 0)
        {
            var colorField = new EditorColor()
            {
                value = value,
                label = label,
                labelWidth = labelWidth,
                valueWidth = valueWidth
            };
            colorField.Draw();
            return colorField.outputValue;
        }

        public static Vector2 Vector2Field(Vector2 value, string label, int labelWidth = 80, int valueWidth = 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }

            Vector2 result;
            if (valueWidth == 0)
                result = EditorGUILayout.Vector2Field("", value, GUILayout.Height(20), GUILayout.MinWidth(40));
            else
                result = EditorGUILayout.Vector2Field("", value, GUILayout.Height(20), GUILayout.MinWidth(40), GUILayout.Width(valueWidth));

            EditorGUILayout.EndHorizontal();

            return result;
        }

        public static Vector3 Vector3Field(Vector3 value, string label, int labelWidth = 80, int valueWidth = 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }

            Vector3 result;
            if (valueWidth == 0)
                result = EditorGUILayout.Vector3Field("", value, GUILayout.Height(20), GUILayout.MinWidth(40));
            else
                result = EditorGUILayout.Vector3Field("", value, GUILayout.Height(20), GUILayout.MinWidth(40), GUILayout.Width(valueWidth));

            EditorGUILayout.EndHorizontal();

            return result;
        }

        public static float[] ArrayField(float[] values, string label, bool showHorizontal = true, int labelWidth = 80, int valueWidth = 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            }

            if (showHorizontal)
                EditorGUILayout.BeginHorizontal();
            else
                EditorGUILayout.BeginVertical();
            float[] results = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                float result;
                if (valueWidth == 0)
                    result = EditorGUILayout.FloatField(values[i], GUILayout.Height(20));
                else
                    result = EditorGUILayout.FloatField(values[i], GUILayout.Height(20), GUILayout.Width(valueWidth));

                results[i] = result;
            }
            if (showHorizontal)
                EditorGUILayout.EndHorizontal();
            else
                EditorGUILayout.EndVertical();


            EditorGUILayout.EndHorizontal();

            return results;
        }

        public static void SerializeFields(SerializedProperty pProperty, params string[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                var item = pProperty.FindPropertyRelative(properties[i]);
                EditorGUILayout.PropertyField(item, true);
            }
        }

        public static void SerializeFields(SerializedObject pObj, params string[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                var item = pObj.FindProperty(properties[i]);
                EditorGUILayout.PropertyField(item, true);
            }
        }

        public static SerializedProperty SerializeField(SerializedObject pObj, string pPropertyName, string pDisplayName = null, params GUILayoutOption[] options)
        {
            SerializedProperty property = pObj.FindProperty(pPropertyName);
            if (property == null)
            {
                Debug.Log("Not found property " + pPropertyName);
                return null;
            }

            if (!property.isArray)
            {
                EditorGUILayout.PropertyField(property, new GUIContent(string.IsNullOrEmpty(pDisplayName) ? property.displayName : pDisplayName));
                return property;
            }
            else
            {
                if (property.isExpanded)
                    EditorGUILayout.PropertyField(property, true, options);
                else
                    EditorGUILayout.PropertyField(property, new GUIContent(property.displayName), options);
                return property;
            }
        }

        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }

        #endregion

        //===============

        #region Build

        public static void RemoveDirective(string pSymbol, BuildTargetGroup pTarget = BuildTargetGroup.Unknown)
        {
            var taget = pTarget == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : pTarget;
            string directives = PlayerSettings.GetScriptingDefineSymbolsForGroup(taget);
            directives = directives.Replace(pSymbol, "");
            if (directives.Length > 1 && directives[directives.Length - 1] == ';')
                directives = directives.Remove(directives.Length - 1, 1);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(taget, directives);
        }

        public static void RemoveDirective(List<string> pSymbols, BuildTargetGroup pTarget = BuildTargetGroup.Unknown)
        {
            var taget = pTarget == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : pTarget;
            string directives = PlayerSettings.GetScriptingDefineSymbolsForGroup(taget);
            for (int i = 0; i < pSymbols.Count; i++)
            {
                if (directives.Contains(pSymbols[i] + ";"))
                    directives = directives.Replace(pSymbols[i] + ";", "");
                else if (directives.Contains(pSymbols[i]))
                    directives = directives.Replace(pSymbols[i], "");
            }
            if (directives.Length > 1 && directives[directives.Length - 1] == ';')
                directives = directives.Remove(directives.Length - 1, 1);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(taget, directives);
        }

        public static void AddDirective(string pSymbol, BuildTargetGroup pTarget = BuildTargetGroup.Unknown)
        {
            var taget = pTarget == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : pTarget;
            string directivesStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(taget);
            string[] directives = directivesStr.Split(';');
            for (int j = 0; j < directives.Length; j++)
                if (directives[j] == pSymbol)
                    return;

            if (string.IsNullOrEmpty(directivesStr))
                directivesStr += pSymbol;
            else
                directivesStr += ";" + pSymbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(taget, directivesStr);
        }

        public static void AddDirectives(List<string> pSymbols, BuildTargetGroup pTarget = BuildTargetGroup.Unknown)
        {
            var taget = pTarget == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : pTarget;
            string directivesStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(taget);
            string[] directives = directivesStr.Split(';');
            for (int i = 0; i < pSymbols.Count; i++)
            {
                bool existed = false;
                for (int j = 0; j < directives.Length; j++)
                    if (directives[j] == pSymbols[i])
                    {
                        existed = true;
                        break;
                    }
                if (existed)
                    continue;

                if (string.IsNullOrEmpty(directivesStr))
                    directivesStr += pSymbols[i];
                else
                    directivesStr += ";" + pSymbols[i];
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(taget, directivesStr);
        }

        public static string[] GetDirectives(BuildTargetGroup pTarget = BuildTargetGroup.Unknown)
        {
            var taget = pTarget == BuildTargetGroup.Unknown ? EditorUserBuildSettings.selectedBuildTargetGroup : pTarget;
            string defineStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(taget);
            string[] currentDefines = defineStr.Split(';');
            for (int i = 0; i < currentDefines.Length; i++)
                currentDefines[i] = currentDefines[i].Trim();
            return currentDefines;
        }

        public static bool ContainDirective(string pSymbol, BuildTargetGroup pTarget = BuildTargetGroup.Unknown)
        {
            var directives = GetDirectives(pTarget);
            foreach (var d in directives)
                if (d == pSymbol)
                    return true;
            return false;
        }

        public static string[] GetScenePaths()
        {
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            string[] scenes = new string[sceneCount];
            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
            }
            return scenes;
        }

        #endregion

        //=============================

        #region Get / Load Assets

        public static string OpenFolderPanel(string pFolderPath = null)
        {
            if (pFolderPath == null)
                pFolderPath = Application.dataPath;
            string path = EditorUtility.OpenFolderPanel("Select Folder", pFolderPath, "");
            return path;
        }

        public static string FormatPathToUnityPath(string path)
        {
            string[] paths = path.Split('/');

            int startJoint = -1;
            string realPath = "";

            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == "Assets")
                {
                    startJoint = i;
                }
                if (startJoint != -1 && i >= startJoint)
                {
                    if (i == paths.Length - 1)
                        realPath += paths[i];
                    else
                        realPath += paths[i] + "/";
                }
            }
            return realPath;
        }

        public static string[] GetDirectories(string path)
        {
            var directories = Directory.GetDirectories(path);

            for (int i = 0; i < directories.Length; i++)
                directories[i] = FormatPathToUnityPath(directories[i]);

            return directories;
        }

        private static T Assign<T>(string pPath) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath(pPath, typeof(T)) as T;
        }

        /// <summary>
        /// Example: GetObjects<AudioClip>(@"Assets\Game\Sounds\Musics", "t:AudioClip")
        /// </summary>
        /// <returns></returns>
        public static List<T> GetObjects<T>(string pPath, string filter, bool getChild = true) where T : UnityEngine.Object
        {
            var directories = GetDirectories(pPath);

            List<T> list = new List<T>();

            var resources = AssetDatabase.FindAssets(filter, directories);

            for (int i = 0; i < resources.Length; i++)
            {
                if (getChild)
                {
                    var childAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(resources[i]));
                    for (int j = 0; j < childAssets.Length; j++)
                    {
                        if (childAssets[j] is T)
                        {
                            list.Add(childAssets[j] as T);
                        }
                    }
                }
                else
                {
                    list.Add(Assign<T>(AssetDatabase.GUIDToAssetPath(resources[i])));
                }
            }

            return list;
        }

        #endregion
    }

    //===================================================================
    // Custom Editor PlayerPrefs
    //===================================================================

    public class EditorPrebs
    {
        protected string mMainKey;
        protected int mSubKey;
        protected string Key { get { return mMainKey + "_" + mSubKey; } }
        public EditorPrebs(int pMainKey, int pSubKey = 0)
        {
            mMainKey = pMainKey + "_" + pSubKey;
        }
        public EditorPrebs(string pMainKey, int pSubKey = 0)
        {
            mMainKey = pMainKey + "_" + pSubKey;
        }
    }

    public class EditorPrefsBool : EditorPrebs
    {
        private bool mValue;
        public EditorPrefsBool(int pMainKey, int pSubKey = 0) : base(pMainKey, pSubKey)
        {
            mValue = EditorPrefs.GetBool(Key);
        }
        public EditorPrefsBool(string pMainKey, int pSubKey = 0) : base(pMainKey, pSubKey)
        {
            mValue = EditorPrefs.GetBool(Key);
        }
        public bool Value
        {
            get { return mValue; }
            set
            {
                if (mValue != value)
                {
                    mValue = value;
                    EditorPrefs.SetBool(Key, value);
                }
            }
        }
        public override string ToString()
        {
            return mValue.ToString();
        }
    }

    public class EditorPrefsString : EditorPrebs
    {
        private string mValue;
        public EditorPrefsString(int pMainKey, int pSubKey = 0) : base(pMainKey, pSubKey)
        {
            mValue = EditorPrefs.GetString(Key);
        }
        public EditorPrefsString(string pMainKey, int pSubKey = 0) : base(pMainKey, pSubKey)
        {
            mValue = EditorPrefs.GetString(Key);
        }
        public string Value
        {
            get { return mValue; }
            set
            {
                if (mValue != value)
                {
                    mValue = value;
                    EditorPrefs.SetString(Key, value);
                }
            }
        }
        public override string ToString()
        {
            return mValue.ToString();
        }
    }

    public class EditorPrefsVector : EditorPrebs
    {
        private Vector3 mValue;
        public EditorPrefsVector(int pMainKey, int pSubKey = 0) : base(pMainKey, pSubKey)
        {
            float x = EditorPrefs.GetFloat(Key + "x");
            float y = EditorPrefs.GetFloat(Key + "y");
            float z = EditorPrefs.GetFloat(Key + "z");
            mValue = new Vector3(x, y, z);
        }
        public EditorPrefsVector(string pMainKey, int pSubKey = 0) : base(pMainKey, pSubKey)
        {
            float x = EditorPrefs.GetFloat(Key + "x");
            float y = EditorPrefs.GetFloat(Key + "y");
            float z = EditorPrefs.GetFloat(Key + "z");
            mValue = new Vector3(x, y, z);
        }
        public Vector3 Value
        {
            get { return mValue; }
            set
            {
                if (mValue != value)
                {
                    mValue = value;
                    EditorPrefs.SetFloat(Key + "x", value.x);
                    EditorPrefs.SetFloat(Key + "y", value.y);
                    EditorPrefs.SetFloat(Key + "z", value.z);
                }
            }
        }
        public override string ToString()
        {
            return mValue.ToString();
        }
    }

    public class EditorPrefsEnum<T> : EditorPrebs
    {
        private T mValue;
        public EditorPrefsEnum(int pMainKey, int pSubKey = 0) : base(pMainKey, pSubKey)
        {
            string strValue = EditorPrefs.GetString(Key);
            foreach (T item in Enum.GetValues(typeof(T)))
                if (item.ToString() == strValue)
                    mValue = item;
        }
        public EditorPrefsEnum(string pMainKey, int pSubKey = 0) : base(pMainKey, pSubKey)
        {
            string strValue = EditorPrefs.GetString(Key);
            foreach (T item in Enum.GetValues(typeof(T)))
                if (item.ToString() == strValue)
                    mValue = item;
        }
        public T Value
        {
            get { return mValue; }
            set
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T must be an enumerated type");
                var inputValue = value.ToString();
                var strValue = mValue.ToString();
                if (strValue != inputValue)
                {
                    mValue = value;
                    EditorPrefs.SetString(Key, inputValue);
                }
            }
        }
        public override string ToString()
        {
            return mValue.ToString();
        }
    }

    public class EditorPrefsFloat : EditorPrebs
    {
        private float mValue;
        public EditorPrefsFloat(int pMainKey, int pSubKey = 0) : base(pMainKey, pSubKey)
        {
            mValue = EditorPrefs.GetFloat(Key);
        }
        public EditorPrefsFloat(string pMainKey, int pSubKey = 0) : base(pMainKey, pSubKey)
        {
            mValue = EditorPrefs.GetFloat(Key);
        }
        public float Value
        {
            get { return mValue; }
            set
            {
                if (mValue != value)
                {
                    mValue = value;
                    EditorPrefs.SetFloat(Key, value);
                }
            }
        }
        public override string ToString()
        {
            return mValue.ToString();
        }
    }

    //===================================================================
    // Custom GUIStyle
    //===================================================================

    public class GUIStyleHelper
    {
        public static GUIStyle HeaderTitle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            fixedHeight = 30,
        };
    }
}
#endif