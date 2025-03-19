using UnityEngine;
using UnityEditor;
using Utilities.Common;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using TMPro;
using System;

namespace Utilities.Editor
{
    public class ToolsCollectionWindow : EditorWindow
    {
        private Vector2 mScrollPosition;
        private void OnGUI()
        {
            mScrollPosition = GUILayout.BeginScrollView(mScrollPosition, false, false);
            FindGameObjectsMissingScript();
            GUILayout.Space(5);
            ReplaceGameobjects();
            GUILayout.Space(5);
            DisplayMeshInfos();
            GUILayout.Space(5);
            FormatTexts();
            GUILayout.Space(5);
            SketchImages();
            GUILayout.Space(5);
            TestFormula();
            GUILayout.EndScrollView();
        }

        //==========================

        public List<GameObject> sources = new List<GameObject>();
        public List<GameObject> prefabs = new List<GameObject>();
        private void ReplaceGameobjects()
        {
            if (EditorHelper.HeaderFoldout("Replace gameobjects"))
                EditorHelper.BoxVertical(() =>
                {
                    if (sources == null || sources.Count == 0)
                        EditorGUILayout.HelpBox("Select at least one Object to see how it work", MessageType.Info);

                    EditorHelper.ListObjects(ref sources, "Replaceable Objects", false);
                    EditorHelper.ListObjects(ref prefabs, "Prefabs", false);

                    if (GUILayout.Button("Replace"))
                        EditorHelper.ReplaceGameobjectsInScene(ref sources, prefabs);
                }, Color.white, true);
        }

        //==========================

        private int mMeshCount = 1;
        private int mVertexCount;
        private int mSubmeshCount;
        private int mTriangleCount;
        private void DisplayMeshInfos()
        {
            if (EditorHelper.HeaderFoldout("Mesh Info"))
                EditorHelper.BoxVertical(() =>
                {
                    if (mMeshCount == 0)
                        EditorGUILayout.HelpBox("Select at least one Mesh Object to see how it work", MessageType.Info);

                    if (mMeshCount > 1)
                    {
                        EditorGUILayout.LabelField("Total Vertices: ", mVertexCount.ToString());
                        EditorGUILayout.LabelField("Total Triangles: ", mTriangleCount.ToString());
                        EditorGUILayout.LabelField("Total SubMeshes: ", mSubmeshCount.ToString());
                        EditorGUILayout.LabelField("Avr Vertices: ", (mVertexCount / mMeshCount).ToString());
                        EditorGUILayout.LabelField("Avr Triangles: ", (mTriangleCount / mMeshCount).ToString());
                    }

                    mVertexCount = 0;
                    mTriangleCount = 0;
                    mSubmeshCount = 0;
                    mMeshCount = 0;

                    foreach (GameObject g in Selection.gameObjects)
                    {
                        var filter = g.GetComponent<MeshFilter>();

                        if (filter != null && filter.sharedMesh != null)
                        {
                            var a = filter.sharedMesh.vertexCount;
                            var b = filter.sharedMesh.triangles.Length / 3;
                            var c = filter.sharedMesh.subMeshCount;
                            mVertexCount += a;
                            mTriangleCount += b;
                            mSubmeshCount += c;
                            mMeshCount += 1;

                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField(g.name);
                            EditorGUILayout.LabelField("Vertices: ", a.ToString());
                            EditorGUILayout.LabelField("Triangles: ", b.ToString());
                            EditorGUILayout.LabelField("SubMeshes: ", c.ToString());
                            return;
                        }
                        var objs = g.FindComponentsInChildren<SkinnedMeshRenderer>();
                        if (objs != null)
                        {
                            int a = 0, b = 0, c = 0;
                            foreach (var obj in objs)
                            {
                                if (obj.sharedMesh == null)
                                    continue;

                                a += obj.sharedMesh.vertexCount;
                                b += obj.sharedMesh.triangles.Length / 3;
                                c += obj.sharedMesh.subMeshCount;
                            }
                            mVertexCount += a;
                            mTriangleCount += b;
                            mSubmeshCount += c;
                            mMeshCount += 1;
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField(g.name);
                            EditorGUILayout.LabelField("Vertices: ", a.ToString());
                            EditorGUILayout.LabelField("Triangles: ", b.ToString());
                            EditorGUILayout.LabelField("SubMeshes: ", c.ToString());
                        }
                    }
                }, Color.white, true);
        }

        //==========================

        public enum FormatType
        {
            UpperCase,
            Lowercase,
            CapitalizeEachWord,
            SentenceCase
        }

        private FormatType mFormatType;
        private int mTextCount;

        private void FormatTexts()
        {
            if (EditorHelper.HeaderFoldout("Format Texts"))
            {
                EditorHelper.BoxVertical(() =>
                {
                    if (mTextCount == 0)
                        EditorGUILayout.HelpBox("Select at least one Text Object to see how it work", MessageType.Info);
                    else
                        EditorGUILayout.LabelField("Text Count: ", mTextCount.ToString());

                    mFormatType = EditorHelper.DropdownListEnum(mFormatType, "Format Type");

                    mTextCount = 0;
                    var allTexts = new List<Text>();
                    var allTextPros = new List<TextMeshProUGUI>();
                    foreach (GameObject g in Selection.gameObjects)
                    {
                        var texts = g.FindComponentsInChildren<Text>();
                        var textPros = g.FindComponentsInChildren<TextMeshProUGUI>();
                        if (texts.Count > 0)
                        {
                            mTextCount += texts.Count;
                            allTexts.AddRange(texts);
                            foreach (var t in allTexts)
                                EditorGUILayout.LabelField("Text: ", t.name.ToString());
                        }
                        if (textPros.Count > 0)
                        {
                            mTextCount += textPros.Count;
                            allTextPros.AddRange(textPros);
                            foreach (var t in allTextPros)
                                EditorGUILayout.LabelField("Text Mesh Pro: ", t.name.ToString());
                        }
                    }

                    if (EditorHelper.Button("Format"))
                    {
                        foreach (var t in allTexts)
                        {
                            switch (mFormatType)
                            {
                                case FormatType.UpperCase:
                                    t.text = t.text.ToUpper();
                                    break;
                                case FormatType.SentenceCase:
                                    t.text = t.text.ToSentenceCase();
                                    break;
                                case FormatType.Lowercase:
                                    t.text = t.text.ToLower();
                                    break;
                                case FormatType.CapitalizeEachWord:
                                    t.text = t.text.ToCapitalizeEachWord();
                                    break;
                            }
                        }
                        foreach (var t in allTextPros)
                        {
                            switch (mFormatType)
                            {
                                case FormatType.UpperCase:
                                    t.text = t.text.ToUpper();
                                    break;
                                case FormatType.SentenceCase:
                                    t.text = t.text.ToSentenceCase();
                                    break;
                                case FormatType.Lowercase:
                                    t.text = t.text.ToLower();
                                    break;
                                case FormatType.CapitalizeEachWord:
                                    t.text = t.text.ToCapitalizeEachWord();
                                    break;
                            }
                        }
                    }
                }, Color.white, true);
            }
        }

        //==========================

        private float mImgWidth;
        private float mImgHeight;
        private int mCountImgs;

        private void SketchImages()
        {
            if (EditorHelper.HeaderFoldout("Sketch Images"))
            {
                EditorHelper.BoxVertical(() =>
                {
                    if (mCountImgs == 0)
                        EditorGUILayout.HelpBox("Select at least one Image Object to see how it work", MessageType.Info);
                    else
                        EditorGUILayout.LabelField("Image Count: ", mCountImgs.ToString());

                    mImgWidth = EditorHelper.FloatField(mImgWidth, "Width");
                    mImgHeight = EditorHelper.FloatField(mImgHeight, "Height");

                    mCountImgs = 0;
                    var allImages = new List<Image>();
                    foreach (GameObject g in Selection.gameObjects)
                    {
                        var img = g.GetComponent<Image>();
                        if (img != null)
                        {
                            allImages.Add(img);
                            EditorGUILayout.LabelField("Image: ", img.ToString());
                        }
                    }
                    var buttons = new List<IDraw>();
                    buttons.Add(new EditorButton()
                    {
                        label = "Sketch By Height",
                        onPressed = () =>
                        {
                            foreach (var img in allImages)
                                img.SketchByHeight(mImgHeight);
                        }
                    });
                    buttons.Add(new EditorButton()
                    {
                        label = "Sketch By Width",
                        onPressed = () =>
                        {
                            foreach (var img in allImages)
                                img.SketchByWidth(mImgWidth);
                        }
                    });
                    buttons.Add(new EditorButton()
                    {
                        label = "Sketch By Fixed Height",
                        onPressed = () =>
                        {
                            foreach (var img in allImages)
                                img.SketchByFixedHeight(mImgHeight);
                        }
                    });
                    buttons.Add(new EditorButton()
                    {
                        label = "Sketch By Fixed Width",
                        onPressed = () =>
                        {
                            foreach (var img in allImages)
                                img.SketchByFixedWidth(mImgWidth);
                        }
                    });
                    buttons.Add(new EditorButton()
                    {
                        label = "Sketch",
                        onPressed = () =>
                        {
                            foreach (var img in allImages)
                                img.Sketch(new Vector2(mImgWidth, mImgHeight));
                        }
                    });
                    EditorHelper.GridDraws(2, buttons);
                }, Color.white, true);
            }
        }

        //==========================

        private bool mAlsoChildren;
        private void FindGameObjectsMissingScript()
        {
            if (EditorHelper.HeaderFoldout("Find Gameobjects missing script"))
            {
                mAlsoChildren = EditorHelper.Toggle(mAlsoChildren, "Also Children of children");
                if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
                {
                    EditorGUILayout.HelpBox("Select at least one GameObject to see how it work", MessageType.Info);
                    return;
                }
                if (EditorHelper.Button("Scan"))
                {
                    var invalidObjs = new List<GameObject>();
                    var objs = Selection.gameObjects;
                    for (int i = 0; i < objs.Length; i++)
                    {
                        var components = objs[i].GetComponents<Component>();
                        for (int j = components.Length - 1; j >= 0; j--)
                        {
                            if (components[j] == null)
                            {
                                Debug.Log(objs[i].gameObject.name + " is missing component! Let clear it!");
                                invalidObjs.Add(objs[i]);
                            }
                        }

                        if (mAlsoChildren)
                        {
                            var children = objs[i].GetAllChildren();
                            for (int k = children.Count - 1; k >= 0; k--)
                            {
                                var childComponents = children[k].GetComponents<Component>();
                                for (int j = childComponents.Length - 1; j >= 0; j--)
                                {
                                    if (childComponents[j] == null)
                                    {
                                        Debug.Log(children[k].gameObject.name + " is missing component! Let clear it!");
                                        invalidObjs.Add(objs[i]);
                                    }
                                }
                            }
                        }
                    }
                    Selection.objects = invalidObjs.ToArray();
                }
            }
        }

        //==========================

        private System.DayOfWeek mNextDayOfWeeok;
        private void TestFormula()
        {
            if (EditorHelper.HeaderFoldout("Test Formula"))
            {
                mNextDayOfWeeok = EditorHelper.DropdownListEnum<System.DayOfWeek>(mNextDayOfWeeok, "Day Of Week");
                if (EditorHelper.Button("Test Seconds Till Day Of Week"))
                {
                    var seconds = TimeHelper.GetSecondsTillDayOfWeek(mNextDayOfWeeok, DateTime.Now);
                    Debug.Log(seconds);
                    seconds = TimeHelper.GetSecondsTillEndDayOfWeek(mNextDayOfWeeok, DateTime.Now);
                    Debug.Log(seconds);
                }
            }
        }

        //==========================

        [MenuItem("DevTools/Tools/Tools Collection")]
        private static void OpenDevEditorWindow()
        {
            var window = GetWindow<ToolsCollectionWindow>("Tools Collection", true);
            window.Show();
        }

        //==========================
    }
}