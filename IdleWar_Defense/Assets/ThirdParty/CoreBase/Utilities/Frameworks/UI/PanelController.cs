#pragma warning disable 0649
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;

namespace Utilities.Pattern.UI
{
    public class PanelController : PanelStack
    {
        #region Internal Class

        #endregion

        //==============================

        #region Members

        public bool useOnce = false;
        public bool enableFXTransition = false;
        public bool isAlone = false;
        public Button btnBack;

        internal Action onWillShow;
        internal Action onWillHide;
        internal Action onDidShow;
        internal Action onDidHide;

        private bool mIsShowing;
        private bool mIsTransiting;
        /// <summary>
        /// When panel is lock, any action pop from itseft or its parent will be restricted
        /// Note: in one momment, there should be no-more one locked child
        /// </summary>
        private bool mIsLock;
        private CanvasGroup mCanvasGroup;
        private Canvas mCanvas;

        internal CanvasGroup CanvasGroup
        {
            get
            {
                if (mCanvasGroup == null)
                {
#if UNITY_2019_2_OR_NEWER
                    TryGetComponent(out mCanvasGroup);
#else
                    mCanvasGroup = GetComponent<CanvasGroup>();
#endif
                    if (mCanvasGroup == null)
                        mCanvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                return mCanvasGroup;
            }
        }
        /// <summary>
        /// Optional, incase we need to control sorting order
        /// </summary>
        internal Canvas Canvas
        {
            get
            {
                if (mCanvas == null)
                {
#if UNITY_2019_2_OR_NEWER
                    GraphicRaycaster rayCaster = null;
                    TryGetComponent(out rayCaster);
#else
                    var rayCaster = GetComponent<GraphicRaycaster>();
#endif
                    if (rayCaster == null)
                        rayCaster = gameObject.AddComponent<GraphicRaycaster>();

#if UNITY_2019_2_OR_NEWER
                    TryGetComponent(out mCanvas);
#else
                    mCanvas = gameObject.GetComponent<Canvas>();
#endif
                    if (mCanvas == null)
                        mCanvas = gameObject.AddComponent<Canvas>();

                    //WaitUtil.Enqueue(() => { mCanvas.overrideSorting = true; }); //Quick-fix
                }
                return mCanvas;
            }
        }

        internal bool IsShowing { get { return mIsShowing; } }
        internal bool IsTransiting { get { return mIsTransiting; } }
        internal bool IsLock { get { return mIsLock; } }

        #endregion

        //=================================

        #region Hide

        internal virtual void Hide(UnityAction OnDidHide = null)
        {
            if (!gameObject.activeSelf)
            {
                Log(name + " Panel is hidden");
                return;
            }

            CoroutineUtil.StartCoroutine(IE_Hide(OnDidHide));
        }

        protected IEnumerator IE_Hide(UnityAction pOnDidHide)
        {
            if (this.onWillHide != null) this.onWillHide();

            BeforeHiding();

            //Wait till there is no sub active panel
            while (panelStack.Count > 0)
            {
                var subPanel = panelStack.Pop();
                subPanel.Hide();

                if (panelStack.Count == 0)
                    yield return new WaitUntil(() => !subPanel.gameObject.activeSelf);
                else
                    yield return null;
            }

            PopAllPanels();

            if (enableFXTransition)
                yield return CoroutineUtil.StartCoroutine(IE_HideFX());

            gameObject.SetActive(false);

            yield return null;

            AfterHiding();

            if (pOnDidHide != null) pOnDidHide();
            if (this.onDidHide != null) this.onDidHide();
        }

        protected virtual IEnumerator IE_HideFX()
        {
            yield break;
        }

        protected virtual void BeforeHiding()
        {
            LockWhileTransiting(true);
        }

        protected virtual void AfterHiding()
        {
            LockWhileTransiting(false);
            mIsShowing = false;

            if (useOnce)
                Destroy(gameObject, 0.1f);
        }

        #endregion

        //==================================

        #region Show

        internal virtual void Show(UnityAction pOnDidShow = null)
        {
            if (gameObject.activeSelf)
            {
                Log(name + " Panel is shown");
                return;
            }

            CoroutineUtil.StartCoroutine(IE_Show(pOnDidShow));
        }

        protected IEnumerator IE_Show(UnityAction pOnDidShow)
        {
            if (this.onWillShow != null) this.onWillShow();

            BeforeShowing();

            //Make the shown panel on the top of all other siblings
            transform.SetAsLastSibling();

            gameObject.SetActive(true);
            if (enableFXTransition)
                yield return CoroutineUtil.StartCoroutine(IE_ShowFX());

            yield return null;

            AfterShowing();

            if (pOnDidShow != null) pOnDidShow();
            if (this.onDidShow != null) this.onDidShow();
        }

        protected virtual IEnumerator IE_ShowFX()
        {
            yield break;
        }

        protected virtual void BeforeShowing()
        {
            LockWhileTransiting(true);
            mIsShowing = true;
        }

        protected virtual void AfterShowing()
        {
            LockWhileTransiting(false);
        }

        #endregion

        //===================================

        #region Monobehaviour

        protected override void Awake()
        {
            base.Awake();

            if (btnBack != null)
                btnBack.onClick.AddListener(BtnBack_Pressed);
        }

        protected virtual void OnDisable()
        {
            LockWhileTransiting(false);

            mIsShowing = false;
        }

        #endregion

        //======================================================

        #region Methods

        private void LockWhileTransiting(bool value)
        {
            mIsTransiting = value;

            if (enableFXTransition)
                CanvasGroup.interactable = !mIsTransiting;
        }

        internal virtual void Back()
        {
            if (mParentPanel == null)
            {
                if (TopPanel != null)
                    TopPanel.Back();
                else
                    LogError("There is nothing for Back");
            }
            else
                //parentPanel.PopPanel();
                mParentPanel.PopChildrenThenParent();
        }

        protected void BtnBack_Pressed()
        {
            Back();
        }

        internal bool CanPop()
        {
            foreach (var p in panelStack)
            {
                if (p.mIsLock)
                    return false;
            }
            if (mIsLock)
                return false;
            return true;
        }

        internal virtual void Init() { }

        internal void Lock(bool pLock)
        {
            mIsLock = pLock;
        }

        public bool IsActiveAndEnabled()
        {
            return !gameObject.IsPrefab() && this.isActiveAndEnabled;
        }

        #endregion

        //======================================================

#if UNITY_EDITOR
        [CustomEditor(typeof(PanelController), true)]
        public class PanelControllerEditor : Editor
        {
            private PanelController mScript;

            protected virtual void OnEnable()
            {
                mScript = target as PanelController;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Children Count: " + mScript.StackCount, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Index: " + mScript.Index, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Display Order: " + mScript.PanelOrder, EditorStyles.boldLabel);
                if (mScript.GetComponent<Canvas>() != null)
                    GUILayout.Label("NOTE: sub-panel should not have Canvas component!\nIt should be inherited from parent panel");

                EditorGUILayout.BeginVertical("box");
                if (mScript.TopPanel == null)
                    EditorGUILayout.LabelField($"TopPanel: Null");
                else
                    EditorGUILayout.LabelField($"TopPanel: {mScript.TopPanel.name}");
                ShowChildrenList(mScript, 0);
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndVertical();
            }

            private void ShowChildrenList(PanelController panel, int plevelIndent)
            {
                int levelIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = plevelIndent;
                foreach (var p in panel.panelStack)
                {
                    EditorGUILayout.LabelField($"{p.Index}: {p.name}");
                    if (p.StackCount > 0)
                    {
                        EditorGUI.indentLevel++;
                        levelIndent = EditorGUI.indentLevel;
                        ShowChildrenList(p, levelIndent);
                    }
                }
            }
        }
#endif
    }
}