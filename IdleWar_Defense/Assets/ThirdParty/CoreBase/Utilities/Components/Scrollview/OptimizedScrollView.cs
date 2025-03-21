﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;

namespace Utilities.Components
{
    public enum ScrollDirection
    {
        Horizontal = 0,
        Vertical = 1
    }

    public class OptimizedScrollView : MonoBehaviour
    {
        public ScrollRect scrollView;
        public RectTransform container;
        public Mask mask;
        public OptimizedScrollItem prefab;
        public int total = 1;
        public float spacing;
        public ScrollDirection Direction = ScrollDirection.Horizontal;

        private RectTransform mMaskRect;
        private int mTotalVisible;
        private int mTotalBuffer = 2;
        private float mHalftSizeContainer;
        private float mPrefabSize;

        private List<RectTransform> mListItemRect = new List<RectTransform>();
        private List<OptimizedScrollItem> mListItem = new List<OptimizedScrollItem>();
        private int mOptimizedTotal = 0;
        private Vector3 mStartPos;
        private Vector3 mOffsetVec;

        private void Start()
        {
            if (Direction == ScrollDirection.Vertical)
                scrollView.verticalScrollbar.onValueChanged.AddListener(ScrollBarChanged);
            else if (Direction == ScrollDirection.Horizontal)
                scrollView.horizontalScrollbar.onValueChanged.AddListener(ScrollBarChanged);

            Initialize(total);
        }

        public void Initialize(int pTotalItems)
        {
            if (pTotalItems == total)
                return;

            mListItemRect = new List<RectTransform>();

            if (mListItem == null || mListItem.Count == 0)
            {
                mListItem = new List<OptimizedScrollItem>();
                mListItem.Prepare(prefab, container.parent, 5);
            }
            else
            {
                mListItem.Free(container);
            }

            total = pTotalItems;

            container.anchoredPosition3D = new Vector3(0, 0, 0);

            if (mMaskRect == null)
                mMaskRect = mask.GetComponent<RectTransform>();

            Vector2 prefabScale = mListItem[0].GetComponent<RectTransform>().rect.size;
            mPrefabSize = (Direction == ScrollDirection.Horizontal ? prefabScale.x : prefabScale.y) + spacing;

            container.sizeDelta = Direction == ScrollDirection.Horizontal ? (new Vector2(mPrefabSize * total, prefabScale.y)) : (new Vector2(prefabScale.x, mPrefabSize * total));
            mHalftSizeContainer = Direction == ScrollDirection.Horizontal ? (container.rect.size.x * 0.5f) : (container.rect.size.y * 0.5f);

            mTotalVisible = Mathf.CeilToInt((Direction == ScrollDirection.Horizontal ? mMaskRect.rect.size.x : mMaskRect.rect.size.y) / mPrefabSize);

            mOffsetVec = Direction == ScrollDirection.Horizontal ? Vector3.right : Vector3.down;
            mStartPos = container.anchoredPosition3D - (mOffsetVec * mHalftSizeContainer) + (mOffsetVec * ((Direction == ScrollDirection.Horizontal ? prefabScale.x : prefabScale.y) * 0.5f));
            mOptimizedTotal = Mathf.Min(total, mTotalVisible + mTotalBuffer);
            for (int i = 0; i < mOptimizedTotal; i++)
            {
                OptimizedScrollItem item = mListItem.Obtain(container);
                RectTransform rt = item.GetComponent<RectTransform>();
                rt.anchoredPosition3D = mStartPos + (mOffsetVec * i * mPrefabSize);
                mListItemRect.Add(rt);

                item.SetActive(true);
                item.UpdateContent(i);
            }

            prefab.gameObject.SetActive(false);
            container.anchoredPosition3D += mOffsetVec * (mHalftSizeContainer - ((Direction == ScrollDirection.Horizontal ? mMaskRect.rect.size.x : mMaskRect.rect.size.y) * 0.5f));
        }

        public void ScrollBarChanged(float pNormPos)
        {
            if (Direction == ScrollDirection.Vertical)
                pNormPos = 1f - pNormPos;

            if (pNormPos <= 0)
                return;

            if (pNormPos > 1)
                pNormPos = 1f;

            int numOutOfView = Mathf.CeilToInt(pNormPos * (total - mTotalVisible));   //number of elements beyond the left boundary (or top)
            int firstIndex = Mathf.Max(0, numOutOfView - mTotalBuffer);   //index of first element beyond the left boundary (or top)
            int originalIndex = firstIndex % mOptimizedTotal;

            int newIndex = firstIndex;
            for (int i = originalIndex; i < mOptimizedTotal; i++)
            {
                moveItemByIndex(mListItemRect[i], newIndex);
                mListItem[i].UpdateContent(newIndex);
                newIndex++;
            }
            for (int i = 0; i < originalIndex; i++)
            {
                moveItemByIndex(mListItemRect[i], newIndex);
                mListItem[i].UpdateContent(newIndex);
                newIndex++;
            }
        }

        private void moveItemByIndex(RectTransform item, int index)
        {
            item.anchoredPosition3D = mStartPos + (mOffsetVec * index * mPrefabSize);
        }

        public List<OptimizedScrollItem> GetListItem()
        {
            return mListItem;
        }
    }
}