﻿#if USE_DOTWEEN
using DG.Tweening;
#endif
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Debug = UnityEngine.Debug;

namespace Utilities.Pattern.UI
{
    public class TransitionFX
    {
        public PanelController panel;
        public Vector2 mDefaultAnchoredPosition;

        public TransitionFX(PanelController pPanel, Vector2 defaultPosition)
        {
            panel = pPanel;
            mDefaultAnchoredPosition = defaultPosition;
        }

        public void TransitLeftToMid(float pTime)
        {
            var rootPanel = panel.GetRootPanel();
            var screenWidth = (rootPanel.transform as RectTransform).rect.width;
            var panelRect = (panel.transform as RectTransform);
            var moveFrom = -(screenWidth / 2f + panelRect.rect.width * (1 - panelRect.pivot.x));
            var moveTo = mDefaultAnchoredPosition.x;
            panelRect.SetX(moveFrom);
#if USE_DOTWEEN
            panelRect.DOLocalMoveX(moveTo, pTime);
#else
            panelRect.SetX(moveTo);
#endif
        }

        public void TransitMidToRight(float pTime)
        {
            var rootPanel = panel.GetRootPanel();
            var screenWidth = (rootPanel.transform as RectTransform).rect.width;
            var panelRect = (panel.transform as RectTransform);
            var moveFrom = mDefaultAnchoredPosition.x;
            var moveTo = screenWidth / 2f + panelRect.rect.width * (1 - panelRect.pivot.x);
            panelRect.SetX(moveFrom);
#if USE_DOTWEEN
            panelRect.DOLocalMoveX(moveTo, pTime);
#else
            panelRect.SetX(moveTo);
#endif
        }

        public void Fade(float pFrom, float pTo, float pTime)
        {
#if USE_DOTWEEN
            panel.CanvasGroup.alpha = pFrom;
            panel.CanvasGroup.DOFade(pTo, pTime);
#else
            panel.CanvasGroup.alpha = 1;
#endif
        }
    }

    public class PanelStack : MonoBehaviour
    {
        protected Stack<PanelController> panelStack = new Stack<PanelController>();
        protected Dictionary<int, PanelController> mCachedOnceUsePanels = new Dictionary<int, PanelController>();

        protected PanelStack mParentPanel;
        /// <summary>
        /// Top child
        /// </summary>
        public PanelController TopPanel
        {
            get
            {
                if (panelStack.Count > 0)
                    return panelStack.Peek();
                return null;
            }
        }
        /// <summary>
        /// Index in stack
        /// </summary>
        public int Index
        {
            get
            {
                if (mParentPanel == null)
                    return 0;
                else
                {
                    int i = 0;
                    foreach (var p in mParentPanel.panelStack)
                    {
                        if (p == this)
                            return i;
                        i++;
                    }
                }
                return mParentPanel.panelStack.Count;
            }
        }
        /// <summary>
        /// Order base-on active sibling
        /// </summary>
        public int PanelOrder
        {
            get
            {
                if (mParentPanel == null)
                    return 1;
                return mParentPanel.panelStack.Count - Index;
            }
        }
        /// <summary>
        /// Total children panels
        /// </summary>
        public int StackCount { get { return panelStack.Count; } }

        protected virtual void Awake()
        {
            if (mParentPanel == null)
                mParentPanel = GetComponentInParent<PanelController>();
            if (mParentPanel == this)
                mParentPanel = null;
        }

        //=============================================================

        #region Create

        /// <summary>
        /// Create and init panel
        /// </summary>
        /// <typeparam name="T">Panels inherit PanelController</typeparam>
        /// <param name="pPanel">Can be prefab or buildin prefab</param>
        /// <returns></returns>
        protected T CreatePanel<T>(ref T pPanel) where T : PanelController
        {
            if (!pPanel.useOnce)
            {
                if (pPanel.gameObject.IsPrefab())
                {
                    pPanel = Instantiate(pPanel, transform);
                    pPanel.SetActive(false);
                    pPanel.Init();
                }
                return pPanel as T;
            }
            else
            {
                if (!pPanel.gameObject.IsPrefab())
                    Debug.LogWarning("One time use panel should be prefab!");

                var panel = Instantiate(pPanel, transform);
                panel.useOnce = true;
                panel.SetActive(false);
                panel.Init();

                if (!mCachedOnceUsePanels.ContainsKey(pPanel.GetInstanceID()))
                    mCachedOnceUsePanels.Add(pPanel.GetInstanceID(), panel);
                else
                    mCachedOnceUsePanels[pPanel.GetInstanceID()] = panel;

                return panel as T;
            }
        }

        /// <summary>
        /// Find child panel of this Panel
        /// </summary>
        /// <typeparam name="T">Panels inherit PanelController</typeparam>
        /// <param name="pOriginal">Can be prefab or buildin prefab</param>
        /// <returns></returns>
        protected T GetCachedPanel<T>(T pOriginal) where T : PanelController
        {
            if (pOriginal.useOnce)
            {
                if (mCachedOnceUsePanels.ContainsKey(pOriginal.GetInstanceID()))
                    return mCachedOnceUsePanels[pOriginal.GetInstanceID()] as T;
                else
                    return null;
            }
            else
            {
                return pOriginal;
            }
        }

        public PanelStack GetRootPanel()
        {
            if (mParentPanel != null)
                return mParentPanel.GetRootPanel();
            return this;
        }

        public PanelStack GetHighestPanel()
        {
            if (TopPanel != null)
                return TopPanel.GetHighestPanel();
            return this;
        }

        #endregion

        //=============================================================

        #region Single

        /// <summary>
        /// Check if panel is prefab or build-in prefab then create and init
        /// </summary>
        internal virtual T PushPanel<T>(ref T pPanel, bool keepCurrentInStack, bool onlyInactivePanel = true, bool sameTimePopAndPush = true) where T : PanelController
        {
            var panel = CreatePanel(ref pPanel);
            PushPanel(panel, keepCurrentInStack, onlyInactivePanel, sameTimePopAndPush);
            return panel;
        }

        /// <summary>
        /// Push new panel will hide the current top panel
        /// </summary>
        /// <param name="panel">New Top Panel</param>
        /// <param name="onlyDisablePanel">Do nothing if panel is currently active</param>
        /// <param name="sameTimePopAndPush">Allow pop current panel and push new </param>
        internal virtual void PushPanel(PanelController panel, bool keepCurrentInStack, bool onlyInactivePanel = true, bool sameTimePopAndPush = true)
        {
            if (panel == null)
            {
                Log("Panel is null");
                return;
            }

            if (onlyInactivePanel && panel.gameObject.activeSelf && panelStack.Contains(panel))
            {
                Log("Panel is already active " + panel.name);
                return;
            }

            if (TopPanel == panel)
                return;

            if (TopPanel != null && !TopPanel.CanPop())
            {
                //If top panel is locked we must keep it
                PushPanelToTop(panel);
                return;
            }

            panel.mParentPanel = this;
            if (TopPanel != null)
            {
                PanelController currentTopPanel = TopPanel;
                if (currentTopPanel.IsShowing)
                {
                    currentTopPanel.Hide(() =>
                    {
                        if (!sameTimePopAndPush)
                        {
                            if (!keepCurrentInStack)
                                panelStack.Pop();
                            panelStack.Push(panel);
                            panel.Show();
                            OnAnyChildShow(panel);
                        }

                        OnAnyChildHide(currentTopPanel);
                    });

                    if (sameTimePopAndPush)
                    {
                        if (!keepCurrentInStack)
                            panelStack.Pop();
                        panelStack.Push(panel);
                        panel.Show();
                        OnAnyChildShow(panel);
                    }
                }
                else
                {
                    if (!keepCurrentInStack)
                        panelStack.Pop();
                    panelStack.Push(panel);
                    panel.Show();
                    OnAnyChildShow(panel);
                }
            }
            else
            {
                panelStack.Push(panel);
                panel.Show();
                OnAnyChildShow(panel);
            }
        }

        /// <summary>
        /// Pop the top panel off the stack and show the one beheath it
        /// </summary>
        internal virtual void PopPanel(bool actionSameTime = true)
        {
            if (TopPanel == null)
            {
                Log("Top Panel is null");
                return;
            }

            if (TopPanel != null && !TopPanel.CanPop())
            {
                Log("Current top panel is locked");
                return;
            }

            var topStack = panelStack.Pop();
            if (topStack.IsShowing)
            {
                topStack.Hide(() =>
                {
                    if (!actionSameTime)
                    {
                        var newPanel = this.TopPanel;
                        if (newPanel != null && !newPanel.IsShowing)
                        {
                            newPanel.Show();
                            OnAnyChildShow(newPanel);
                        }
                    }

                    OnAnyChildHide(topStack);
                });

                if (actionSameTime)
                {
                    var newPanel = this.TopPanel;
                    if (newPanel != null && !newPanel.IsShowing)
                    {
                        newPanel.Show();
                        OnAnyChildShow(newPanel);
                    }
                }
            }
            else
            {
                var newPanel = this.TopPanel;
                if (newPanel != null && !newPanel.IsShowing)
                {
                    newPanel.Show();
                    OnAnyChildShow(newPanel);
                }
            }
        }

        /// <summary>
        /// Check if panel is prefab or build-in prefab then create and init
        /// </summary>
        internal virtual T PushPanelToTop<T>(ref T pPanel) where T : PanelController
        {
            var panel = CreatePanel(ref pPanel);
            PushPanelToTop(panel);
            return panel;
        }

        /// <summary>
        /// Push panel without hiding panel is under it
        /// </summary>
        internal virtual void PushPanelToTop(PanelController panel)
        {
            if (this.TopPanel == panel)
                return;

            if (panel.isAlone)
            {
                foreach (var k in panelStack)
                    k.SetActive(false);
            }

            panelStack.Push(panel);
            panel.mParentPanel = this;
            panel.Show();
            OnAnyChildShow(panel);
        }

        #endregion

        //=============================================================

        #region Multi

        /// <summary>
        /// Keep only one panel in stack
        /// </summary>
        internal virtual void PopAllThenPush(PanelController panel)
        {
            PopAllPanels();
            PushPanel(panel, false);
        }

        /// <summary>
        /// Pop all panels till there is only one panel left in the stack
        /// </summary>
        internal virtual void PopTillOneLeft()
        {
            var lockedPanels = new List<PanelController>();
            PanelController oldTopPanel = null;
            while (panelStack.Count > 1)
            {
                oldTopPanel = panelStack.Pop();
                if (!oldTopPanel.CanPop())
                    //Locked panel should not be hide
                    lockedPanels.Add(oldTopPanel);
                else
                    oldTopPanel.Hide();
            }

            //Resign every locked panels, because we removed them temporarity above
            if (lockedPanels.Count > 0)
            {
                for (int i = lockedPanels.Count - 1; i >= 0; i--)
                    panelStack.Push(lockedPanels[i]);
            }

            if (!TopPanel.IsShowing)
            {
                TopPanel.Show();
                OnAnyChildShow(TopPanel);
            }

            if (oldTopPanel != null)
                OnAnyChildHide(oldTopPanel);
        }

        /// <summary>
        /// Pop till we remove specific panel
        /// </summary>
        internal virtual void PopTillNoPanel(PanelController panel)
        {
            if (!panelStack.Contains(panel))
            {
                Log("Panel is not showing or is not under parent");
                return;
            }

            var lockedPanels = new List<PanelController>();
            PanelController oldTopPanel = null;

            //Pop panels until we find the right one we're trying to pop
            do
            {
                oldTopPanel = panelStack.Pop();
                if (!oldTopPanel.CanPop())
                    //Locked panel should not be hide
                    lockedPanels.Add(oldTopPanel);
                else
                    oldTopPanel.Hide();

            } while (oldTopPanel.GetInstanceID() != panel.GetInstanceID() && panelStack.Count > 0);

            //Resign every locked panels, because we removed them temporarity above
            if (lockedPanels.Count > 0)
            {
                for (int i = lockedPanels.Count - 1; i >= 0; i--)
                    panelStack.Push(lockedPanels[i]);
            }

            var newPanel = TopPanel;
            if (newPanel != null && !newPanel.IsShowing)
            {
                newPanel.Show();
                OnAnyChildShow(newPanel);
            }

            if (oldTopPanel != null)
                OnAnyChildHide(oldTopPanel);
        }

        internal virtual void PopTillPanel(PanelController panel)
        {
            if (!panelStack.Contains(panel))
            {
                Log("Panel is not showing or is not under parent");
                return;
            }

            var lockedPanels = new List<PanelController>();
            PanelController curTopPanel = null;

            while (panelStack.Count > 0)
            {
                curTopPanel = panelStack.Peek();
                if (curTopPanel.GetInstanceID() == panel.GetInstanceID())
                    break;

                panelStack.Pop();
                if (!curTopPanel.CanPop())
                    //Locked panel should not be hide
                    lockedPanels.Add(curTopPanel);
                else
                    curTopPanel.Hide();
            }

            //Resign every locked panels, because we removed them temporarity above
            if (lockedPanels.Count > 0)
            {
                for (int i = lockedPanels.Count - 1; i >= 0; i--)
                    panelStack.Push(lockedPanels[i]);
            }

            var newPanel = TopPanel;
            if (newPanel != null && !newPanel.IsShowing)
            {
                newPanel.Show();
                OnAnyChildShow(newPanel);
            }

            if (curTopPanel != null)
                OnAnyChildHide(curTopPanel);
        }

        /// <summary>
        /// Pop and hide all panels in stack, at the same time
        /// </summary>
        internal virtual void PopAllPanels()
        {
            var lockedPanels = new List<PanelController>();
            PanelController oldTopPanel = null;
            while (panelStack.Count > 0)
            {
                oldTopPanel = panelStack.Pop();
                if (!oldTopPanel.CanPop())
                    //Locked panel should not be hide
                    lockedPanels.Add(oldTopPanel);
                else
                    oldTopPanel.Hide();
            }

            //Resign every locked panel, because we removed them temporarity above
            if (lockedPanels.Count > 0)
            {
                for (int i = lockedPanels.Count - 1; i >= 0; i--)
                    panelStack.Push(lockedPanels[i]);
            }

            if (oldTopPanel != null)
                OnAnyChildHide(oldTopPanel);
        }

        /// <summary>
        /// Pop one by one, chilren then parent
        /// </summary>
        internal virtual void PopChildrenThenParent()
        {
            if (this.TopPanel == null)
                return;

            if (this.TopPanel.TopPanel != null)
                this.TopPanel.PopChildrenThenParent();
            else
                PopPanel();
        }

        #endregion

        //==============================================================

        protected virtual void OnAnyChildHide(PanelController pPanel)
        {
            //Parent notifies to grandparent of hidden panel
            if (mParentPanel != null)
                mParentPanel.OnAnyChildHide(pPanel);
            
            //for UI FX particle order
            SetOldPanelToBack();
        }
        protected virtual void OnAnyChildShow(PanelController pPanel)
        {
            if (mParentPanel != null)
                mParentPanel.OnAnyChildShow(pPanel);
            
            //for UI FX particle order
            SetOldPanelToBack();
        }

        private void SetOldPanelToBack()
        {
            if(this.TopPanel == null) return;
            
            //for UI FX particle order
            Vector3 pos;
            foreach (var item in panelStack)
            {
                pos = item.transform.localPosition;
                pos.z = 100f;
                item.transform.localPosition = pos;
            }
            
            //for UI FX particle order
            pos = this.TopPanel.transform.localPosition;
            pos.z = -100f;
            this.TopPanel.transform.localPosition = pos;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        protected void Log(string pMessage)
        {
            Debug.Log(string.Format("<color=yellow><b>[{1}]:</b></color>{1}", gameObject.name, pMessage));
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        protected void LogError(string pMessage)
        {
            Debug.LogError(string.Format("<color=red><b>[{1}]:</b></color>{1}", gameObject.name, pMessage));
        }
    }
}