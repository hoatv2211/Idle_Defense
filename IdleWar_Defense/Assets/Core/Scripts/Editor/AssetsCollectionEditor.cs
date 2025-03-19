using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Utilities.Common;

namespace FoodZombie
{
    [CustomEditor(typeof(AssetsCollection))]
    public class AssetsCollectionEditor : Editor
    {
        private AssetsCollection mScript;
        private SerializedObject mSerializedScript;

        void OnEnable()
        {
            mScript = (AssetsCollection)target;
            mSerializedScript = new SerializedObject(mScript);
        }

        public override void OnInspectorGUI()
        {
            var tab = EditorHelper.Tabs(name, "Default", "Ordered");
            GUILayout.Space(5);

            switch (tab)
            {
                case "Default":
                    base.OnInspectorGUI();
                    break;

                case "Ordered":
                    EditorHelper.BoxVertical("Icons Managed By Name", () =>
                    {
                        ShowList(ref mScript.commonSprites, "commonSprites");
                    });
                    GUILayout.Space(5);
                    EditorHelper.BoxVertical("Icons Managed By Id", () =>
                    {
                        ShowList(ref mScript.heroIconSprites, "heroIconSprites");
                        ShowList(ref mScript.enemyIconSprites, "enemyIconSprites");
                        ShowList(ref mScript.gearIconSprites, "gearIconSprites");
                        ShowList(ref mScript.elementIconSprites, "elementIconSprites");
                        ShowList(ref mScript.rankSprites, "rankSprites");
                    });
                    EditorHelper.BoxVertical("Animations", () =>
                    {
                        ShowList(ref mScript.heroAnimationDataAssets, "heroAnimationDataAssets", false);
                        ShowList(ref mScript.heroMainAnimationDataAssets, "heroMainAnimationDataAssets", false);
                        ShowList(ref mScript.enemyAnimationDataAssets, "enemyAnimationDataAssets", false);
                    });
                    break;
            }

            EditorHelper.ButtonColor("Save", () =>
            {
                EditorUtility.SetDirty(mScript);
                AssetDatabase.SaveAssets();
            });

            mSerializedScript.ApplyModifiedProperties();
        }

        private void ShowList<T>(ref List<T> pList, string pPropertyName, bool pShowBox = true) where T : Object
        {
            EditorHelper.ListObjectsWithSearch<T>(ref pList, mSerializedScript.FindProperty(pPropertyName).displayName, pShowBox);
        }
    }
}