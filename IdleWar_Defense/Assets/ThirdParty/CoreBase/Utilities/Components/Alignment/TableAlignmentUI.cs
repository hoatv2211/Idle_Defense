using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Inspector;
#if USE_DOTWEEN
using DG.Tweening;
#endif

namespace Utilities.Components
{

    public class TableAlignmentUI : MyAlignment
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
        public bool reverseY = true;
        public float tweenTime = 0.25f;

        [Space(10)]
        public int maxRow;

        [Space(10)]
        public int maxColumn;

        [Space(10)]
        public float columnDistance;
        public float rowDistance;

        private float mWidth;
        private float mHeight;
        private int mFirstGroupIndex;
        private int mLastGroupIndex;
        private Coroutine mCoroutine;

        private Dictionary<int, List<RectTransform>> childrenGroup;

        public void Init()
        {
            int totalRow = 0;
            int totalCol = 0;

            var allChildren = new List<RectTransform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.activeSelf)
                    allChildren.Add(transform.GetChild(i) as RectTransform);
            }

            childrenGroup = new Dictionary<int, List<RectTransform>>();
            if (tableLayoutType == TableLayoutType.Horizontal)
            {
                if (maxColumn == 0)
                    maxColumn = 1;

                totalRow = Mathf.CeilToInt(allChildren.Count * 1f / maxColumn);
                totalCol = Mathf.CeilToInt(allChildren.Count * 1f / totalRow);
                int row = 0;
                while (allChildren.Count > 0)
                {
                    if (row == 0) mFirstGroupIndex = row;
                    if (row > 0) mLastGroupIndex = row;
                    for (int i = 0; i < maxColumn; i++)
                    {
                        if (allChildren.Count == 0)
                            break;

                        if (!childrenGroup.ContainsKey(row))
                            childrenGroup.Add(row, new List<RectTransform>());
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
                    if (col == 0) mFirstGroupIndex = col;
                    if (col > 0) mLastGroupIndex = col;
                    for (int i = 0; i < maxRow; i++)
                    {
                        if (allChildren.Count == 0)
                            break;

                        if (!childrenGroup.ContainsKey(col))
                            childrenGroup.Add(col, new List<RectTransform>());
                        childrenGroup[col].Add(allChildren[0]);
                        allChildren.RemoveAt(0);
                    }
                    col++;
                }
            }

            mWidth = (totalCol - 1) * columnDistance;
            mHeight = (totalRow - 1) * rowDistance;
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
                    if (reverseY) y = -y + mHeight;

                    switch (alignmentType)
                    {
                        case Alignment.Left:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = i * new Vector3(columnDistance, 0, 0);
                                pos.y = y - mHeight / 2f;
                                children[i].anchoredPosition = pos;
                            }
                            break;

                        case Alignment.Right:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = (children.Count - 1 - i) * new Vector3(columnDistance, 0, 0) * -1;
                                pos.y = y - mHeight / 2f;
                                children[i].anchoredPosition = pos;
                            }
                            break;

                        case Alignment.Center:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = i * new Vector3(columnDistance, 0, 0);
                                pos.y = y - mHeight / 2f;
                                children[i].anchoredPosition = pos;
                            }
                            if (a.Key == mLastGroupIndex && mLastGroupIndex != mFirstGroupIndex)
                            {
                                for (int i = 0; i < children.Count; i++)
                                {
                                    var pos = children[i].anchoredPosition;
                                    pos.x -= (columnDistance * (maxColumn - 1)) / 2f;
                                    children[i].anchoredPosition = pos;
                                }
                            }
                            else
                                for (int i = 0; i < children.Count; i++)
                                {
                                    children[i].anchoredPosition = new Vector3(
                                        children[i].anchoredPosition.x - children[children.Count - 1].anchoredPosition.x / 2,
                                        children[i].anchoredPosition.y);
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
                                pos.x = x - mWidth / 2f;
                                children[i].anchoredPosition = pos;
                            }
                            break;

                        case Alignment.Bottom:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = i * new Vector3(0, rowDistance, 0);
                                pos.x = x - mWidth / 2f;
                                children[i].anchoredPosition = pos;
                            }
                            break;

                        case Alignment.Center:
                            for (int i = 0; i < children.Count; i++)
                            {
                                var pos = i * new Vector3(0, rowDistance, 0);
                                pos.x = x - mWidth / 2f;
                                children[i].anchoredPosition = pos;
                            }
                            if (a.Key == mLastGroupIndex && mLastGroupIndex != mFirstGroupIndex)
                            {
                                for (int i = 0; i < children.Count; i++)
                                {
                                    var pos = children[i].anchoredPosition;
                                    pos.y -= (rowDistance * (maxRow - 1)) / 2f;
                                    children[i].anchoredPosition = pos;
                                }
                            }
                            else
                                for (int i = 0; i < children.Count; i++)
                                {
                                    children[i].anchoredPosition = new Vector3(
                                        children[i].anchoredPosition.x,
                                        children[i].anchoredPosition.y - children[children.Count - 1].anchoredPosition.y / 2);
                                }
                            break;
                    }
                }
            }
        }

        public override void AlignByTweener(Action onFinish)
        {
            if (mCoroutine != null)
                StopCoroutine(mCoroutine);
            mCoroutine = StartCoroutine(IEAlignByTweener(onFinish));
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
                    if (reverseY) y = -y + mHeight;

                    Vector3[] childrenNewPosition = new Vector3[children.Count];
                    Vector3[] childrenPrePosition = new Vector3[children.Count];
                    switch (alignmentType)
                    {
                        case Alignment.Left:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].anchoredPosition;
                                var pos = i * new Vector3(columnDistance, 0, 0);
                                pos.y = y - mHeight / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            break;

                        case Alignment.Right:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].anchoredPosition;
                                var pos = (children.Count - 1 - i) * new Vector3(columnDistance, 0, 0) * -1;
                                pos.y = y - mHeight / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            break;

                        case Alignment.Center:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].anchoredPosition;
                                var pos = i * new Vector3(columnDistance, 0, 0);
                                pos.y = y - mHeight / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            if (a.Key == mLastGroupIndex && mLastGroupIndex != mFirstGroupIndex)
                            {
                                for (int i = 0; i < children.Count; i++)
                                {
                                    var pos = childrenNewPosition[i];
                                    pos.x -= (columnDistance * (maxColumn - 1)) / 2f;
                                    childrenNewPosition[i] = pos;
                                }
                            }
                            else
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
                LeanTween.value(gameObject, 0f, 1f, tweenTime)
                    .setOnUpdate((float val) =>
                    {
                        for (int j = 0; j < children.Count; j++)
                        {
                            var pos = Vector3.Lerp(childrenPrePosition[j], childrenNewPosition[j], val);
                            children[j].anchoredPosition = pos;
                        }
                    });
#elif USE_DOTWEEN
                    float lerp = 0;
                    DOTween.Kill(GetInstanceID() + a.Key);
                    DOTween.To(val => lerp = val, 0f, 1f, tweenTime)
                        .OnUpdate(() =>
                        {
                            for (int j = 0; j < children.Count; j++)
                            {
                                Vector2 pos = Vector2.Lerp(childrenPrePosition[j], childrenNewPosition[j], lerp);
                                children[j].anchoredPosition = pos;
                            }
                        })
                        .SetId(GetInstanceID() + a.Key);
#else
                    StartCoroutine(IEArrangeChildren(children, childrenPrePosition, childrenNewPosition, tweenTime));
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
                                childrenPrePosition[i] = children[i].anchoredPosition;
                                var pos = i * new Vector3(0, rowDistance, 0);
                                pos.x = x - mWidth / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            break;

                        case Alignment.Bottom:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].anchoredPosition;
                                var pos = (childrenNewPosition.Length - 1 - i) * new Vector3(0, rowDistance, 0) * -1;
                                pos.x = x - mWidth / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            break;

                        case Alignment.Center:
                            for (int i = 0; i < children.Count; i++)
                            {
                                childrenPrePosition[i] = children[i].anchoredPosition;
                                var pos = i * new Vector3(0, rowDistance, 0);
                                pos.x = x - mWidth / 2f;
                                childrenNewPosition[i] = pos;
                            }
                            if (a.Key == mLastGroupIndex && mLastGroupIndex != mFirstGroupIndex)
                            {
                                for (int i = 0; i < children.Count; i++)
                                {
                                    var pos = childrenNewPosition[i];
                                    pos.y -= (rowDistance * (maxRow - 1)) / 2f;
                                    childrenNewPosition[i] = pos;
                                }
                            }
                            else
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
                    LeanTween.value(gameObject, 0f, 1f, tweenTime)
                        .setOnUpdate((float val) =>
                        {
                            for (int j = 0; j < children.Count; j++)
                            {
                                var pos = Vector3.Lerp(childrenPrePosition[j], childrenNewPosition[j], val);
                                children[j].anchoredPosition = pos;
                            }
                        });
#elif USE_DOTWEEN
                    float lerp = 0;
                    DOTween.Kill(GetInstanceID() + a.Key);
                    DOTween.To(val => lerp = val, 0f, 1f, tweenTime)
                        .OnUpdate(() =>
                        {
                            for (int j = 0; j < children.Count; j++)
                            {
                                var pos = Vector3.Lerp(childrenPrePosition[j], childrenNewPosition[j], lerp);
                                children[j].anchoredPosition = pos;
                            }
                        })
                        .SetId(GetInstanceID() + a.Key);
#else
                    StartCoroutine(IEArrangeChildren(children, childrenPrePosition, childrenNewPosition, tweenTime));
#endif
                    yield return null;
                }
            }

            yield return new WaitForSeconds(tweenTime);

            if (onFinish != null)
                onFinish();
        }

        private IEnumerator IEArrangeChildren(List<RectTransform> pObjs, Vector3[] pChildrenPrePosition, Vector3[] pChildrenNewPosition, float pDuration)
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
                    pObjs[j].anchoredPosition = pos;
                }

                if (lerp >= 1)
                    break;
            }
        }
    }
}