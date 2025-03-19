using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FoodZombie
{
    [ExecuteInEditMode]
    public class LocalizedLabel : MonoBehaviour
    {
        public enum Style
        {
            None,
            UpperCase,
            LowerCase,
        }

        public string idString;
        public Text text;
        public TextMeshProUGUI textPro;
        public Style style;

        private Localization.ID mId = Localization.ID.NONE;

        private void OnEnable()
        {
            SetText();
        }

        public void SetText()
        {
            if (textPro == null)
                textPro = GetComponent<TextMeshProUGUI>();
            else if (text == null)
                text = GetComponent<Text>();

            string t = "";
            if (text != null)
            {
                if (mId != Localization.ID.NONE)
                    t = Localization.Get(mId);
                else
                    t = Localization.Get(idString, ref mId);
            }

            if (textPro != null)
            {
                if (mId != Localization.ID.NONE)
                    t = Localization.Get(mId);
                else
                    t = Localization.Get(idString, ref mId);
            }

            switch (style)
            {
                case Style.LowerCase:
                    t = t.ToLower();
                    break;
                case Style.UpperCase:
                    t = t.ToUpper();
                    break;
            }

            if (!string.IsNullOrEmpty(t))
            {
                if (text != null)
                    text.text = t;
                if (textPro != null)
                    textPro.text = t;
            }

#if UNITY_EDITOR
            mId = Localization.ID.NONE;
#endif
        }
    }

    //==========================================================================

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(LocalizedLabel))]
    public class LocalizedLabelEditor : UnityEditor.Editor
    {
        private LocalizedLabel mScript;

        private void OnEnable()
        {
            mScript = (LocalizedLabel)target;
        }

        public override void OnInspectorGUI()
        {
            var defaultColor = GUI.color;

            if (mScript.textPro == null && mScript.text == null)
            {
                mScript.text = mScript.GetComponent<Text>();
                mScript.textPro = mScript.GetComponent<TextMeshProUGUI>();
            }
            if (mScript.textPro == null && mScript.text == null)
            {
                GUI.color = Color.red;
                UnityEditor.EditorGUILayout.LabelField("Please add a Text or TextMeshProUGUI component", new GUIStyle(UnityEditor.EditorStyles.helpBox) { fontSize = 13 });
                GUI.color = defaultColor;
                return;
            }

            base.OnInspectorGUI();

            if (!string.IsNullOrEmpty(mScript.idString))
            {
                string value = Localization.Get(mScript.idString);
                UnityEditor.EditorGUILayout.LabelField(value, UnityEditor.EditorStyles.boldLabel);
            }

            UnityEditor.EditorGUILayout.BeginVertical("box");
            {
                string search = UnityEditor.EditorPrefs.GetString(mScript.gameObject.GetInstanceID() + "_search");
                UnityEditor.EditorGUILayout.BeginHorizontal();
                {
                    GUI.color = new Color(255f / 255f, 212 / 255f, 135f / 255f, 1);
                    UnityEditor.EditorGUILayout.LabelField("Search:", new GUIStyle() { fontStyle = FontStyle.Bold, }, GUILayout.Width(80));
                    search = UnityEditor.EditorGUILayout.TextField(search, new GUIStyle(UnityEditor.EditorStyles.textField) { fontStyle = FontStyle.Italic });
                    GUI.color = defaultColor;
                }
                UnityEditor.EditorGUILayout.EndHorizontal();

                if (!string.IsNullOrEmpty(search))
                {
                    int limit = 20;
                    var idString = Localization.idString;
                    for (int i = 0; i < idString.Length; i++)
                    {
                        if (limit <= 0)
                            break;
                        if (idString[i].ToLower().Contains(search.ToLower()))
                        {
                            GUI.color = new Color(114f / 255f, 255f / 255f, 232f / 255f, 1);
                            if (GUILayout.Button(idString[i]))
                            {
                                search = idString[i];
                                mScript.idString = idString[i];
                                mScript.SetText();
                            }
                            GUI.color = defaultColor;
                            limit--;
                        }
                    }
                }

                if (GUI.changed)
                    UnityEditor.EditorPrefs.SetString(mScript.gameObject.GetInstanceID() + "_search", search);
            }
            UnityEditor.EditorGUILayout.EndVertical();
        }
    }

#endif
}