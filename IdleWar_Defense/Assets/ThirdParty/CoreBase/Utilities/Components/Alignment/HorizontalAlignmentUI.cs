#pragma warning disable 0649
//#define USE_LEANTWEEN
//#define USE_DOTWEEN

#if USE_DOTWEEN
using DG.Tweening;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Components
{
    class HorizontalAlignmentUI : MyAlignment
    {
        public enum Alignment
        {
            Left,
            Right,
            Center,
        }

        public float maxContainerWidth;
        public Alignment alignmentType;
        public float cellDistance;
        public float tweenTime = 0.25f;
        public bool autoWhenStart;
        [Header("Optional")]
        public float yOffset;

        private List<RectTransform> mChildren = new List<RectTransform>();
        private List<bool> mIndexesChanged = new List<bool>();
        private List<int> mChildrenId = new List<int>();
        private Coroutine mCoroutine;
#if USE_DOTWEEN
        private Tweener mTweener;
#endif
        private void Start()
        {
            if (autoWhenStart)
                Align();
        }

        public override void Align()
        {
            Init();

            int maxCellInLine = 10;
            if (Math.Abs(mChildren.Count * cellDistance) > maxContainerWidth && maxContainerWidth > 0)
            {
                // cellDistance *= maxContainerWidth / (Math.Abs(mChildren.Count * cellDistance));
                maxCellInLine = (int)(maxContainerWidth / cellDistance);
            }

            switch (alignmentType)
            {
                case Alignment.Left:
                    for (int i = 0; i < mChildren.Count; i++)
                    {
                        mChildren[i].anchoredPosition = -new Vector2(maxContainerWidth / 2f, 0f)
                                                        + (i % maxCellInLine) * new Vector2(cellDistance, yOffset)
                                                        - (i / maxCellInLine) * new Vector2(0, cellDistance);
                    }
                    break;

                case Alignment.Right:
                    for (int i = 0; i < mChildren.Count; i++)
                    {
                        mChildren[i].anchoredPosition = new Vector2(maxContainerWidth / 2f, 0f)
                                                        + (maxCellInLine - 1 - (i % maxCellInLine)) * new Vector2(cellDistance, yOffset) * -1
                                                        - (i / maxCellInLine) * new Vector2(0, cellDistance);
                    }
                    break;

                case Alignment.Center:
                    for (int i = 0; i < mChildren.Count; i++)
                    {
                        mChildren[i].anchoredPosition = (i % maxCellInLine) * new Vector3(cellDistance, yOffset, 0) - (i / maxCellInLine) * new Vector3(0, cellDistance, 0);
                    }
                    for (int i = 0; i < mChildren.Count; i++)
                    {
                        mChildren[i].anchoredPosition = new Vector3(
                            mChildren[i].anchoredPosition.x - mChildren[mChildren.Count - 1].anchoredPosition.x / 2,
                            mChildren[i].anchoredPosition.y + yOffset);
                    }
                    break;
            }
        }

        public override void AlignByTweener(Action onFinish)
        {
            StartCoroutine(IEAlignByTweener(onFinish));
        }

        private IEnumerator IEAlignByTweener(Action onFinish)
        {
            Init();

            int maxCellInLine = 10;
            if (Math.Abs(mChildren.Count * cellDistance) > maxContainerWidth && maxContainerWidth > 0)
            {
                // cellDistance *= maxContainerWidth / (Math.Abs(mChildren.Count * cellDistance));
                maxCellInLine = (int)(maxContainerWidth / cellDistance);
            }

            Vector2[] childrenNewPosition = new Vector2[mChildren.Count];
            Vector2[] childrenPrePosition = new Vector2[mChildren.Count];
            switch (alignmentType)
            {
                case Alignment.Left:
                    for (int i = 0; i < mChildren.Count; i++)
                    {
                        childrenPrePosition[i] = mChildren[i].anchoredPosition;
                        childrenNewPosition[i] = -new Vector2(maxContainerWidth / 2f, 0f)
                                                 + (i % maxCellInLine) * new Vector2(cellDistance, yOffset)
                                                 - (i / maxCellInLine) * new Vector2(0, cellDistance);
                    }
                    break;

                case Alignment.Right:
                    for (int i = 0; i < mChildren.Count; i++)
                    {
                        childrenPrePosition[i] = mChildren[i].anchoredPosition;
                        childrenNewPosition[i] = new Vector2(maxContainerWidth / 2f, 0f)
                                                 + (maxCellInLine - 1 - (i % maxCellInLine)) * new Vector2(cellDistance, yOffset) * -1
                                                 - (i / maxCellInLine) * new Vector2(0, cellDistance);
                    }
                    break;

                case Alignment.Center:
                    for (int i = 0; i < mChildren.Count; i++)
                    {
                        childrenPrePosition[i] = mChildren[i].anchoredPosition;
                        childrenNewPosition[i] = (i % maxCellInLine) * new Vector2(cellDistance, yOffset) - (i / maxCellInLine) * new Vector2(0, cellDistance);
                    }
                    for (int i = 0; i < mChildren.Count; i++)
                    {
                        var space = i / maxCellInLine;
                        var a = (space + 1) * maxCellInLine - 1;
                        if (a > mChildren.Count - 1) a = mChildren.Count - 1;
                        childrenNewPosition[i] = new Vector2(
                            childrenNewPosition[i].x - childrenNewPosition[a].x / 2,
                            childrenNewPosition[i].y + yOffset);
                    }
                    break;
            }

#if USE_LEANTWEEN
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, 0, 1, tweenTime)
                .setOnUpdate((float val) =>
                {
                    for (int j = 0; j < mChildren.Count; j++)
                    {
                        Vector2 pos = Vector2.Lerp(childrenPrePosition[j], childrenNewPosition[j], val);
                        mChildren[j].anchoredPosition = pos;
                    }
                });
#elif USE_DOTWEEN
            float lerp = 0;
            mTweener.Kill();
            mTweener = DOTween.To(tweenVal => lerp = tweenVal, 0f, 1f, tweenTime)
                .OnUpdate(() =>
                {
                    for (int j = 0; j < mChildren.Count; j++)
                    {
                        Vector2 pos = Vector2.Lerp(childrenPrePosition[j], childrenNewPosition[j], lerp);
                        mChildren[j].anchoredPosition = pos;
                    }
                })
                .SetEase(Ease.Linear)
                .SetUpdate(true);
#else
            if (mCoroutine != null)
                StopCoroutine(mCoroutine);
            mCoroutine = StartCoroutine(IEArrangeChildren(childrenPrePosition, childrenNewPosition, tweenTime));
#endif

            yield return new WaitForSecondsRealtime(tweenTime);

            if (onFinish != null)
                onFinish();
        }

        private void Init()
        {
            var childrenTemp = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.activeSelf)
                    childrenTemp.Add(child);
            }
            for (int i = 0; i < childrenTemp.Count; i++)
            {
                if (i > mChildren.Count - 1)
                    mChildren.Add(null);

                if (i > mIndexesChanged.Count - 1)
                    mIndexesChanged.Add(true);

                if (i > mChildrenId.Count - 1)
                    mChildrenId.Add(0);

                if (mChildrenId[i] != childrenTemp[i].gameObject.GetInstanceID())
                {
                    mChildrenId[i] = childrenTemp[i].gameObject.GetInstanceID();
                    mIndexesChanged[i] = true;
                }
                else
                {
                    mIndexesChanged[i] = false;
                }
            }
            for (int i = mChildrenId.Count - 1; i >= 0; i--)
            {
                if (i > childrenTemp.Count - 1)
                {
                    mChildrenId.RemoveAt(i);
                    mChildren.RemoveAt(i);
                    mIndexesChanged.RemoveAt(i);
                    continue;
                }
                if (mIndexesChanged[i] || mChildren[i] == null)
                {
#if UNITY_2019_2_OR_NEWER
                    childrenTemp[i].TryGetComponent(out RectTransform component);
                    mChildren[i] = component;
#else
                    mChildren[i] = childrenTemp[i].GetComponent<RectTransform>();
#endif
                    mIndexesChanged[i] = false;
                }
            }
        }

        private IEnumerator IEArrangeChildren(Vector2[] pChildrenPrePosition, Vector2[] pChildrenNewPosition, float pDuration)
        {
            float time = 0;
            while (true)
            {
                yield return null;
                time += Time.deltaTime;
                if (time >= pDuration)
                    time = pDuration;
                float lerp = time / pDuration;

                for (int j = 0; j < mChildren.Count; j++)
                {
                    var pos = Vector3.Lerp(pChildrenPrePosition[j], pChildrenNewPosition[j], lerp);
                    mChildren[j].anchoredPosition = pos;
                }

                if (lerp >= 1)
                    break;
            }
        }


        public override Vector3 SpawnPos()
        {
            return transform.position;
        }
    }
}
