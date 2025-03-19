//#define USE_LEANTWEEN
//#define USE_DOTWEEN

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if USE_DOTWEEN
using DG.Tweening;
#endif

namespace Utilities.Components
{
    public class HorizontalAlignment : MyAlignment
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
        [Header("Optional")]
        public float yOffset;

        private Transform[] mChildren;
        private Coroutine mCoroutine;

        private void Start()
        {
            Align();
        }

        private void Init()
        {
            var list = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.activeSelf)
                    list.Add(transform.GetChild(i));
            }
            mChildren = list.ToArray();
        }

        public override void Align()
        {
            Init();

            if (Math.Abs(mChildren.Length * cellDistance) > maxContainerWidth && maxContainerWidth > 0)
            {
                cellDistance *= maxContainerWidth / (Math.Abs(mChildren.Length * cellDistance));
            }

            switch (alignmentType)
            {
                case Alignment.Left:
                    for (int i = 0; i < mChildren.Length; i++)
                    {
                        mChildren[i].localPosition = i * new Vector3(cellDistance, yOffset, 0);
                    }
                    break;

                case Alignment.Right:
                    for (int i = 0; i < mChildren.Length; i++)
                    {
                        mChildren[i].localPosition = (mChildren.Length - 1 - i) * new Vector3(cellDistance, yOffset, 0) * -1;
                    }
                    break;

                case Alignment.Center:
                    for (int i = 0; i < mChildren.Length; i++)
                    {
                        mChildren[i].localPosition = i * new Vector3(cellDistance, yOffset, 0);
                    }
                    for (int i = 0; i < mChildren.Length; i++)
                    {
                        mChildren[i].localPosition = new Vector3(
                            mChildren[i].localPosition.x - mChildren[mChildren.Length - 1].localPosition.x / 2,
                            mChildren[i].localPosition.y + yOffset,
                            mChildren[i].localPosition.z);
                    }
                    break;
            }
        }

        public override void AlignByTweener(Action onFinish)
        {
            Debug.Log("OK run");
            StartCoroutine(IEAlignByTweener(onFinish));
        }

        private IEnumerator IEAlignByTweener(Action onFinish)
        {
            Init();

            if (Math.Abs(mChildren.Length * cellDistance) > maxContainerWidth && maxContainerWidth > 0)
            {
                cellDistance *= maxContainerWidth / (Math.Abs(mChildren.Length * cellDistance));
            }

            Vector3[] childrenNewPosition = new Vector3[mChildren.Length];
            Vector3[] childrenPrePosition = new Vector3[mChildren.Length];
            switch (alignmentType)
            {
                case Alignment.Left:
                    for (int i = 0; i < mChildren.Length; i++)
                    {
                        childrenPrePosition[i] = mChildren[i].localPosition;
                        childrenNewPosition[i] = i * new Vector3(cellDistance, yOffset, 0);
                    }
                    break;

                case Alignment.Right:
                    for (int i = 0; i < mChildren.Length; i++)
                    {
                        childrenPrePosition[i] = mChildren[i].localPosition;
                        childrenNewPosition[i] = (mChildren.Length - 1 - i) * new Vector3(cellDistance, yOffset, 0) * -1;
                    }
                    break;

                case Alignment.Center:
                    for (int i = 0; i < mChildren.Length; i++)
                    {
                        childrenPrePosition[i] = mChildren[i].localPosition;
                        childrenNewPosition[i] = i * new Vector3(cellDistance, yOffset, 0);
                    }
                    for (int i = 0; i < mChildren.Length; i++)
                    {
                        childrenNewPosition[i] = new Vector3(
                            childrenNewPosition[i].x - childrenNewPosition[mChildren.Length - 1].x / 2,
                            childrenNewPosition[i].y + yOffset,
                            childrenNewPosition[i].z);
                    }
                    break;
            }

#if USE_LEANTWEEN
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, 0f, 1f, 0.25f)
                .setOnUpdate((float val) =>
                {
                    for (int j = 0; j < mChildren.Length; j++)
                    {
                        var pos = Vector3.Lerp(childrenPrePosition[j], childrenNewPosition[j], val);
                        mChildren[j].localPosition = pos;
                    }
                });
#elif USE_DOTWEEN
            float lerp = 0;
            DOTween.Kill(GetInstanceID());
            DOTween.To(tweenVal => lerp = tweenVal, 0f, 1f, 0.25f)
                .OnUpdate(() =>
                {
                    for (int j = 0; j < mChildren.Length; j++)
                    {
                        Vector2 pos = Vector2.Lerp(childrenPrePosition[j], childrenNewPosition[j], lerp);
                        mChildren[j].localPosition = pos;
                    }
                })
                .SetEase(Ease.InQuint)
                .SetId(GetInstanceID()).SetUpdate(UpdateType.Normal,true);
#else
            if (mCoroutine != null)
                StopCoroutine(mCoroutine);
            mCoroutine = StartCoroutine(IEArrangeChildren(childrenPrePosition, childrenNewPosition, 0.25f));
#endif

            yield return new WaitForSecondsRealtime(0.25f);

            if (onFinish != null)
                onFinish();
        }

        private IEnumerator IEArrangeChildren(Vector3[] pChildrenPrePosition, Vector3[] pChildrenNewPosition, float pDuration)
        {
            float time = 0;
            while (true)
            {
                yield return null;
                time += Time.deltaTime;
                if (time >= pDuration)
                    time = pDuration;
                float lerp = time / pDuration;

                for (int j = 0; j < mChildren.Length; j++)
                {
                    var pos = Vector3.Lerp(pChildrenPrePosition[j], pChildrenNewPosition[j], lerp);
                    mChildren[j].localPosition = pos;
                }

                if (lerp >= 1)
                    break;
            }
        }
    }
}