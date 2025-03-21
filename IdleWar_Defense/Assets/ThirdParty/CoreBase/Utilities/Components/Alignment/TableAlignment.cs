﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Inspector;
#if USE_DOTWEEN
using DG.Tweening;
#endif

namespace Utilities.Components
{

    public class TableAlignment : MyAlignment
    {
        public enum TableLayoutType
        {
            Horizontal,
            Vertical
        }

        public enum Alignment
        {
            Top,
            Bottom,
            Left,
            Right,
            Center,
        }

        public TableLayoutType tableLayoutType;
        public Alignment alignmentType;

        [Space(10)]
        public int maxRow;

        [Space(10)]
        public int maxColumn;

        [Space(10)]
        public float columnDistance;
        public float rowDistance;

        [ReadOnly] public float width;
        [ReadOnly] public float height;

        private Dictionary<int, List<Transform>> childrenGroup;

        public void Init()
        {
            int totalRow = 0;
            int totalCol = 0;

            var allChildren = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.activeSelf)
                    allChildren.Add(transform.GetChild(i));
            }

            childrenGroup = new Dictionary<int, List<Transform>>();
            if (tableLayoutType == TableLayoutType.Horizontal)
            {
                if (maxColumn == 0)
                    maxColumn = 1;

                totalRow = Mathf.CeilToInt(allChildren.Count * 1f / maxColumn);
                totalCol = Mathf.CeilToInt(allChildren.Count * 1f / totalRow);
                int row = 0;
                while (allChildren.Count > 0)
                {
                    for (int i = 0; i < maxColumn; i++)
                    {
                        if (allChildren.Count == 0)
                            break;

                        if (!childrenGroup.ContainsKey(row))
                            childrenGroup.Add(row, new List<Transform>());
                        childrenGroup[row].Add(allChildren[0]);
                        allChildren.RemoveAt(0);
                    }
                    row++;
                }
            }
            else
            {
                if (maxRow == 0)
                    maxRow = 1;

                totalCol = Mathf.CeilToInt(allChildren.Count * 1f / maxRow);
                totalRow = Mathf.CeilToInt(allChildren.Count * 1f / totalCol);
                int col = 0;
                while (allChildren.Count > 0)
                {
                    for (int i = 0; i < maxRow; i++)
                    {
                        if (allChildren.Count == 0)
                            break;

                        if (!childrenGroup.ContainsKey(col))
                            childrenGroup.Add(col, new List<Transform>());
                        childrenGroup[col].Add(allChildren[0]);
                        allChildren.RemoveAt(0);
                    }
                    col++;
                }
            }

            width = (totalCol - 1) * columnDistance;
            height = (totalRow - 1) * rowDistance;
        }

        public override void Align()
        {
            Init();

            if (tableLayoutType == TableLayoutType.Horizontal)
            {
                foreach (var a in childrenGroup)
                {
                    var children = a.Value;
                    float y = a.Key * rowDistance;

                    switch (alignmentType)
                    {
                        case Alignment.Left:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = i * new Vector3(columnDistance, 0, 0);
                                pos.y = y - height / 2f;
                                children[i].localPosition = pos;
                            }
                            break;

                        case Alignment.Right:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = (children.Count - 1 - i) * new Vector3(columnDistance, 0, 0) * -1;
                                pos.y = y - height / 2f;
                                children[i].localPosition = pos;
                            }
                            break;

                        case Alignment.Center:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = i * new Vector3(columnDistance, 0, 0);
                                pos.y = y - height / 2f;
                                children[i].localPosition = pos;
                            }
                            for (int i = 0; i < children.Count; i++)
                            {
                                children[i].localPosition = new Vector3(
                                    children[i].localPosition.x - children[children.Count - 1].localPosition.x / 2,
                                    children[i].localPosition.y,
                                    children[i].localPosition.z);
                            }
                            break;
                    }
                }
            }
            else
            {
                foreach (var a in childrenGroup)
                {
                    var children = a.Value;
                    float x = a.Key * columnDistance;

                    switch (alignmentType)
                    {
                        case Alignment.Top:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = (children.Count - 1 - i) * new Vector3(0, rowDistance, 0) * -1;
                                pos.x = x - width / 2f;
                                children[i].transform.localPosition = pos;
                            }
                            break;

                        case Alignment.Bottom:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = i * new Vector3(0, rowDistance, 0);
                                pos.x = x - width / 2f;
                                children[i].transform.localPosition = pos;
                            }
                            break;

                        case Alignment.Center:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = i * new Vector3(0, rowDistance, 0);
                                pos.x = x - width / 2f;
                                children[i].transform.localPosition = pos;
                            }
                            for (int i = 0; i < children.Count; i++)
                            {
                                children[i].transform.localPosition = new Vector3(
                                    children[i].localPosition.x,
                                    children[i].localPosition.y - children[children.Count - 1].localPosition.y / 2,
                                    children[i].localPosition.z);
                            }
                            break;
                    }
                }
            }
        }

        public override void AlignByTweener(Action onFinish)
        {
            StartCoroutine(IEAlignByTweener(onFinish));
        }

        private IEnumerator IEAlignByTweener(Action onFinish)
        {
            Init();

            if (tableLayoutType == TableLayoutType.Horizontal)
            {
                foreach (var a in childrenGroup)
                {
                    var children = a.Value;
                    float y = a.Key * rowDistance;

                    Vector3[] childrenNewPosition = new Vector3[children.Count];
                    Vector3[] childrenPrePosition = new Vector3[children.Count];
                    switch (alignmentType)
                    {
                        case Alignment.Left:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].localPosition;
                                var pos = i * new Vector3(columnDistance, 0, 0);
                                pos.y = y - height / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            break;

                        case Alignment.Right:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].localPosition;
                                var pos = (children.Count - 1 - i) * new Vector3(columnDistance, 0, 0) * -1;
                                pos.y = y - height / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            break;

                        case Alignment.Center:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].localPosition;
                                var pos = i * new Vector3(columnDistance, 0, 0);
                                pos.y = y - height / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            for (int i = 0; i < childrenNewPosition.Length; i++)
                            {
                                childrenNewPosition[i] = new Vector3(
                                    childrenNewPosition[i].x - childrenNewPosition[children.Count - 1].x / 2,
                                    childrenNewPosition[i].y,
                                    childrenNewPosition[i].z);
                            }
                            break;
                    }

#if USE_LEANTWEEN
                LeanTween.value(gameObject, 0f, 1f, 0.25f)
                    .setOnUpdate((float val) =>
                    {
                        for (int j = 0; j < children.Count; j++)
                        {
                            var pos = Vector3.Lerp(childrenPrePosition[j], childrenNewPosition[j], val);
                            children[j].localPosition = pos;
                        }
                    });
#elif USE_DOTWEEN
                    float lerp = 0;
                    DOTween.Kill(GetInstanceID() + a.Key);
                    DOTween.To(val => lerp = val, 0f, 1f, 0.25f)
                        .OnUpdate(() =>
                        {
                            for (int j = 0; j < children.Count; j++)
                            {
                                Vector2 pos = Vector2.Lerp(childrenPrePosition[j], childrenNewPosition[j], lerp);
                                children[j].localPosition = pos;
                            }
                        })
                        .SetEase(Ease.InQuint)
                        .SetId(GetInstanceID() + a.Key);
#else
                    StartCoroutine(IEArrangeChildren(children, childrenPrePosition, childrenNewPosition, 0.25f));
#endif
                    yield return null;
                }
            }
            else
            {
                foreach (var a in childrenGroup)
                {
                    var children = a.Value;
                    float x = a.Key * columnDistance;

                    Vector3[] childrenPrePosition = new Vector3[children.Count];
                    Vector3[] childrenNewPosition = new Vector3[children.Count];
                    switch (alignmentType)
                    {
                        case Alignment.Top:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].localPosition;
                                var pos = i * new Vector3(0, rowDistance, 0);
                                pos.x = x - width / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            break;

                        case Alignment.Bottom:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].localPosition;
                                var pos = (childrenNewPosition.Length - 1 - i) * new Vector3(0, rowDistance, 0) * -1;
                                pos.x = x - width / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            break;

                        case Alignment.Center:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].localPosition;
                                var pos = i * new Vector3(0, rowDistance, 0);
                                pos.x = x - width / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            for (int i = 0; i < childrenNewPosition.Length; i++)
                            {
                                childrenNewPosition[i] = new Vector3(
                                    childrenNewPosition[i].x,
                                    childrenNewPosition[i].y - childrenNewPosition[childrenNewPosition.Length - 1].y / 2,
                                    childrenNewPosition[i].z);
                            }
                            break;
                    }

#if USE_LEANTWEEN
                    LeanTween.value(gameObject, 0f, 1f, 0.25f)
                        .setOnUpdate((float val) =>
                        {
                            for (int j = 0; j < children.Count; j++)
                            {
                                var pos = Vector3.Lerp(childrenPrePosition[j], childrenNewPosition[j], val);
                                children[j].localPosition = pos;
                            }
                        });
#elif USE_DOTWEEN
                    float lerp = 0;
                    DOTween.Kill(GetInstanceID() + a.Key);
                    DOTween.To(val => lerp = val, 0f, 1f, 0.25f)
                        .OnUpdate(() =>
                        {
                            for (int j = 0; j < children.Count; j++)
                            {
                                var pos = Vector3.Lerp(childrenPrePosition[j], childrenNewPosition[j], lerp);
                                children[j].localPosition = pos;
                            }
                        })
                        .SetEase(Ease.InQuint)
                        .SetId(GetInstanceID() + a.Key);
#else
                    StartCoroutine(IEArrangeChildren(children, childrenPrePosition, childrenNewPosition, 0.25f));
#endif
                    yield return null;
                }
            }

            yield return new WaitForSeconds(0.25f);

            if (onFinish != null)
                onFinish();
        }

        private IEnumerator IEArrangeChildren(List<Transform> pObjs, Vector3[] pChildrenPrePosition, Vector3[] pChildrenNewPosition, float pDuration)
        {
            float time = 0;
            while (true)
            {
                yield return null;
                time += Time.deltaTime;
                if (time >= pDuration)
                    time = pDuration;
                float lerp = time / pDuration;

                for (int j = 0; j < pObjs.Count; j++)
                {
                    var pos = Vector3.Lerp(pChildrenPrePosition[j], pChildrenNewPosition[j], lerp);
                    pObjs[j].localPosition = pos;
                }

                if (lerp >= 1)
                    break;
            }
        }
    }
}