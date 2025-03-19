using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;
using DG.Tweening;
public class CircleAlignmentUI : MyAlignment
{
    public float tweenTime = 1.0f;
    public Transform rootPoint;
    private List<RectTransform> mChildren = new List<RectTransform>();
    private List<bool> mIndexesChanged = new List<bool>();
    private List<int> mChildrenId = new List<int>();


    //List<RectTransform> childs = new List<RectTransform>();
    //List<Vector3> pos;
    RectTransform mRect;
    Vector3 circlerRoot;
    private Tweener mTweener;
    float radius = 400;
    void Init()
    {
        if (mRect == null)
        {
            mRect = GetComponent<RectTransform>();
            circlerRoot = mRect.anchoredPosition + new Vector2(0, -radius - 150);
        }
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
    public override Vector3 SpawnPos()
    {
        return rootPoint.position;

        if (mRect == null)
        {
            mRect = GetComponent<RectTransform>();
            // circlerRoot = mRect.anchoredPosition;
            circlerRoot = mRect.anchoredPosition + new Vector2(0, -radius - 150);
        }
        Vector3 output;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(mRect, Vector2.zero, Camera.main, out output);
        return output;
        //return circlerRoot;
    }
    public override void AlignByTweener(Action onFinish)
    {

        StartCoroutine(IEAlignByTweener(onFinish));
    }

    private IEnumerator IEAlignByTweener(Action onFinish)
    {
        Init();




        Vector2[] childrenNewPosition = new Vector2[mChildren.Count];
        Vector2[] childrenPrePosition = new Vector2[mChildren.Count];

        for (int i = 0; i < mChildren.Count; i++)
        {
            //if (mChildren[i].transform.position == SpawnPos())
            //{
            //    mChildren[i].anchoredPosition = circlerRoot;
            //    childrenPrePosition[i] = circlerRoot;
            //}
            //else
            childrenPrePosition[i] = mChildren[i].anchoredPosition;
            if (i < 10)
                childrenNewPosition[i] = RandomCircle(circlerRoot, radius, i * 360 / 10);
            else
                childrenNewPosition[i] = circlerRoot;
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

    Vector3 RandomCircle(Vector3 center, float radius, float ang)
    {
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }
}
