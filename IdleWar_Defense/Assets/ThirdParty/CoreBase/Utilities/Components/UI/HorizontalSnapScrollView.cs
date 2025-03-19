//#define USE_DOTWEEN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using System;
using UnityEngine.UI;
using Utilities.Inspector;
using Debug = UnityEngine.Debug;
using UnityEngine.EventSystems;
#if USE_DOTWEEN
using DG.Tweening;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utilities.Components
{
    public class HorizontalSnapScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        #region Members

        public Action<int> onIndexChanged;

#pragma warning disable 0649
        [SerializeField] private float m_MinSpringTime = 0.25f;
        [SerializeField] private float m_MaxSpringTime = 0.75f;
        [SerializeField] private float m_SpringThreshold = 15;
        [SerializeField] private bool m_AutoSetMinScollRection = true;
        [SerializeField] private float m_MinScrollReaction = 10;
        [SerializeField] private Vector2 m_TargetPosOffset;
        [SerializeField] private ScrollRect m_ScrollView;
        [SerializeField] private List<RectTransform> m_Items;
        [SerializeField] private bool m_ReverseList;  //TRUE: If the items are ordered from right to left
        [SerializeField] private RectTransform m_PointToCheckDistanceToCenter; //To find the nearest item
#pragma warning disable 0649

        [SerializeField, ReadOnly] private float m_ContentAnchoredXMin;
        [SerializeField, ReadOnly] private float m_ContentAnchoredXMax;
        [SerializeField, ReadOnly] private int m_FocusedItemIndex = -1;
        [SerializeField, ReadOnly] private bool m_IsSnapping;
        [SerializeField, ReadOnly] private bool m_IsDraging;

        private Vector2 m_PreviousPosition;
        private float m_DragDistance;
        private Vector2 m_Velocity;
        private float m_Distance;
        private Vector2 m_BeginDragPosition;
        private bool m_DragFromLeft;
        private bool m_CheckBoundary;
        private bool m_Validated;
        private bool m_CheckStop;

        private RectTransform Content => m_ScrollView.content;
        public int CurrentIndex => m_FocusedItemIndex;
        public int TotalItems => m_Items.Count;
        public bool IsSnapping => m_IsSnapping;
        public bool IsDragging => m_IsDraging;

        #endregion

        //=============================================

        #region MonoBehaviour

        private void OnEnable()
        {
            m_Validated = false;
            m_CheckBoundary = m_ScrollView.movementType == ScrollRect.MovementType.Unrestricted;
        }

        private void Update()
        {
            if (!m_Validated)
            {
                Validate();
                m_Validated = true;
            }

            m_Velocity = Content.anchoredPosition - m_PreviousPosition;
            m_PreviousPosition = Content.anchoredPosition;

            if (m_IsDraging || m_IsSnapping)
                return;

            float speedX = Mathf.Abs(m_Velocity.x);
            if (speedX == 0)
            {
                if (m_CheckStop)
                {
                    FindNearestItem();
                    m_CheckStop = false;
                }
                return;
            }
            else
                m_CheckStop = true;

            if (m_CheckBoundary && OutOfBoundary())
            {

            }
            else
            {
                if (speedX > 0 && speedX <= m_SpringThreshold)
                {
                    FindNearestItem();
                    int index = m_FocusedItemIndex;
                    var targetPos = m_Items[index].CovertAnchoredPosFromChildToParent(m_ScrollView.content);
                    if (m_DragFromLeft)
                    {
                        if (Content.anchoredPosition.x > targetPos.x + m_MinScrollReaction)
                        {
                            if (m_ReverseList)
                                index++;
                            else
                                index--;
                        }
                    }
                    else
                    {
                        if (Content.anchoredPosition.x < targetPos.x - m_MinScrollReaction)
                        {
                            if (m_ReverseList)
                                index--;
                            else
                                index++;
                        }
                    }
                    if (index < 0)
                        index = 0;
                    else if (index >= m_Items.Count - 1)
                        index = m_Items.Count - 1;
                    SetFocusedIndex(index);
                    MoveToFocusedItem(false, speedX);
                }
            }
        }

        private void OnValidate()
        {
            Validate();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
#if USE_DOTWEEN
            DOTween.Kill(GetInstanceID());
#endif
            m_IsDraging = true;
            m_IsSnapping = false;
            m_BeginDragPosition = Content.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_IsDraging = true;
            FindNearestItem();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_IsDraging = false;

            if (m_CheckBoundary && OutOfBoundary())
                return;

            var endDragPosition = Content.anchoredPosition;
            m_DragDistance = Mathf.Abs(m_BeginDragPosition.x - endDragPosition.x);
            m_DragFromLeft = m_BeginDragPosition.x < endDragPosition.x;
            //float speedX = Mathf.Abs(m_Velocity.x);
            //if (speedX <= m_SpringThreshold)
            //{
            //    FindNearestItem();
            //    CheckScrollReaction();
            //    MoveToFocusedItem(false, speedX);
            //}
        }

        #endregion

        //=============================================

        #region Public

        public void Init(List<RectTransform> pItems)
        {
            m_Items = pItems;

            Validate();
        }

        public void MoveToItem(int pIndex, bool pImmediately)
        {
            if (pIndex < 0 || pIndex >= m_Items.Count)
                return;

            if (m_Validated)
            {
                SetFocusedIndex(pIndex);
                MoveToFocusedItem(pImmediately, m_SpringThreshold);
            }
            else
            {
                WaitUtil.Start(() => m_Validated, () =>
                {
                    SetFocusedIndex(pIndex);
                    MoveToFocusedItem(pImmediately, m_SpringThreshold);
                });
            }
        }

        #endregion

        //==============================================

        #region Private

        private void Validate()
        {
#if UNITY_EDITOR
            string str = "Cotent Top Right: " + Content.TopRight()
                + "\nContent Bot Lert: " + Content.BotLeft()
                + "\nContent Center: " + Content.Center()
                + "\nContent Size" + Content.sizeDelta
                + "\nContent Pivot" + Content.pivot
                + "\nViewPort Size" + m_ScrollView.viewport.rect.size;
            //Debug.Log(str);
#endif
            var contentFilter = Content.GetComponent<ContentSizeFitter>();
            float contentWidth = 0;
            float contentHeight = 0;
            if (contentFilter != null)
            {
                var horizontalLayout = Content.GetComponent<HorizontalLayoutGroup>();
                float paddingLeft = 0;
                float paddingRight = 0;
                float spacing = 0;
                if (horizontalLayout != null)
                {
                    paddingLeft = horizontalLayout.padding.left;
                    paddingRight = horizontalLayout.padding.right;
                    spacing = horizontalLayout.spacing;
                }
                for (int i = 0; i < m_Items.Count; i++)
                {
                    if (m_Items[i].gameObject.activeSelf)
                    {
                        var itemSize = m_Items[i].rect.size;
                        contentWidth += itemSize.x;
                        if (contentHeight < itemSize.y)
                            contentHeight = itemSize.y;
                    }
                }
                contentWidth += (paddingLeft + paddingRight);
                contentWidth += spacing * (m_Items.Count - 1);
            }
            else
                contentWidth = Content.rect.width;

            float contentPivotX = Content.pivot.x;
            float viewPortOffsetX = m_ScrollView.viewport.rect.width / 2f;
            m_ContentAnchoredXMin = (contentWidth - contentWidth * contentPivotX - viewPortOffsetX) * -1;
            m_ContentAnchoredXMax = (0 - contentWidth * contentPivotX + viewPortOffsetX) * -1;

            if (m_MinScrollReaction < 10)
                m_MinScrollReaction = 10;
        }

        private void MoveToFocusedItem(bool pImmediately, float pSpeed)
        {
            m_ScrollView.StopMovement();

            Vector2 targetAnchored = m_Items[m_FocusedItemIndex].CovertAnchoredPosFromChildToParent(m_ScrollView.content);
            targetAnchored.x -= m_TargetPosOffset.x;
            targetAnchored.y -= m_TargetPosOffset.y;
            if (targetAnchored.x > m_ContentAnchoredXMax)
                targetAnchored.x = m_ContentAnchoredXMax;
            if (targetAnchored.x < m_ContentAnchoredXMin)
                targetAnchored.x = m_ContentAnchoredXMin;

            Vector2 contentAnchored = Content.anchoredPosition;
            if (pImmediately || !Application.isPlaying)
            {
                contentAnchored.x = targetAnchored.x;
                Content.anchoredPosition = contentAnchored;
            }
            else
            {
                m_IsSnapping = false;
#if USE_DOTWEEN
                if (m_Distance == 0)
                    m_Distance = Vector2.Distance(m_PointToCheckDistanceToCenter.position, m_Items[m_FocusedItemIndex].position);
                float time = m_Distance / (pSpeed / Time.deltaTime);
                if (time == 0)
                    return;
                if (time < m_MinSpringTime)
                    time = m_MinSpringTime;
                else if (time > m_MaxSpringTime)
                    time = m_MaxSpringTime;

                DOTween.Kill(GetInstanceID());
                Content.DOAnchorPosX(targetAnchored.x, time)
                    .OnStart(() => { m_IsSnapping = true; })
                    .OnComplete(() =>
                    {
                        m_IsSnapping = false;
                        contentAnchored.x = targetAnchored.x;
                        Content.anchoredPosition = contentAnchored;
                    }).SetId(GetInstanceID());
#else
                contentAnchored.x = targetAnchored.x;
                Content.anchoredPosition = contentAnchored;
#endif
            }
        }

        private void SetFocusedIndex(int pIndex)
        {
            if (m_FocusedItemIndex == pIndex)
                return;

            m_FocusedItemIndex = pIndex;
            onIndexChanged?.Invoke(pIndex);

            if (m_AutoSetMinScollRection)
                m_MinScrollReaction = m_Items[m_FocusedItemIndex].rect.width / 20f;
        }

        private void CheckScrollReaction()
        {
            if (m_DragDistance > m_MinScrollReaction)
            {
                int index = m_FocusedItemIndex;
                //Get one down item
                if (m_DragFromLeft)
                {
                    if (m_ReverseList)
                        index += 1;
                    else
                        index -= 1;
                }
                // Get one up item
                else
                {
                    if (m_ReverseList)
                        index -= 1;
                    else
                        index += 1;
                }
                if (index < 0)
                    index = 0;
                else if (index >= m_Items.Count - 1)
                    index = m_Items.Count - 1;
                SetFocusedIndex(index);
            }
        }

        private void FindNearestItem()
        {
            m_Distance = 1000000;
            int nearestItemIndex = 0;
            for (int i = 0; i < m_Items.Count; i++)
            {
                float distance = Vector2.Distance(m_PointToCheckDistanceToCenter.position, m_Items[i].position);
                distance = Mathf.Abs(distance);
                if (m_Distance > distance)
                {
                    m_Distance = distance;
                    nearestItemIndex = i;
                }
            }
            SetFocusedIndex(nearestItemIndex);
        }

        /// <summary>
        /// Used in case we have custom top/bottom/left/right border instead of auto size component of unity
        /// </summary>
        /// <returns></returns>
        private bool OutOfBoundary()
        {
            var contentAnchored = Content.anchoredPosition;
            if (contentAnchored.x < m_ContentAnchoredXMin || contentAnchored.x > m_ContentAnchoredXMax)
            {
                Debug.Log("Out of boundary");
                return true;
            }
            return false;
        }

        #endregion

#if UNITY_EDITOR
        [CustomEditor(typeof(HorizontalSnapScrollView))]
        private class HorizontalSnapScrollViewEditor : Editor
        {
            private HorizontalSnapScrollView m_Target;
            private int m_ItemIndex;

            private void OnEnable()
            {
                m_Target = target as HorizontalSnapScrollView;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (EditorHelper.Button("Validate"))
                    m_Target.Validate();
                EditorHelper.BoxHorizontal(() =>
                {
                    m_ItemIndex = EditorHelper.IntField(m_ItemIndex, "Item Index");
                    if (EditorHelper.Button("MoveToItem"))
                        m_Target.MoveToItem(m_ItemIndex, false);
                });
            }
        }
#endif
    }
}