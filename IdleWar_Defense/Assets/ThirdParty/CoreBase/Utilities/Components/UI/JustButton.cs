using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Utilities.Common;
using System.Collections.Generic;
using FoodZombie;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace Utilities.Components
{
    [AddComponentMenu("Utitlies/UI/JustButton")]
    public class JustButton : Button
    {
        public enum PivotForScale
        {
            Bot,
            Top,
            TopLeft,
            BotLeft,
            TopRight,
            BotRight,
            Center,
        }

        private static Material mGreyMat;

        [SerializeField] protected PivotForScale mPivotForFX = PivotForScale.Center;
        [SerializeField] protected bool mEnabledFX = true;
        [SerializeField] protected Image mImg;
        [SerializeField] protected RectTransform[] mRelatedObjects;
        [SerializeField] protected Vector3 mInitialScale = Vector3.one;
        [SerializeField] protected bool mPlaySound = true;

        [SerializeField] protected bool mImgSwapEnabled;
        [SerializeField] protected Sprite mImgActive;
        [SerializeField] protected Sprite mImgInactive;

        public Image img
        {
            get
            {
                if (mImg == null)
                    mImg = targetGraphic as Image;
                return mImg;
            }
        }
        public Material imgMaterial
        {
            get { return img.material; }
            set { img.material = value; }
        }
        public RectTransform rectTransform
        {
            get { return targetGraphic.rectTransform; }
        }
        public RectTransform[] relatedObjects { get { return mRelatedObjects; } }

        private PivotForScale mPrePivot;
        private Action mInactionStateAction;
        private bool mActive = true;

        public Action onPointerDown;
        public Action onPointerUp;

        public virtual void SetEnable(bool pValue)
        {
            try
            {
                mActive = pValue;
                enabled = pValue || mInactionStateAction != null;
                if (pValue)
                {
                    if (mImgSwapEnabled && mImg != null)
                        mImg.sprite = mImgActive;
                    else
                        if (imgMaterial != null)
                        imgMaterial = null;
                }
                else
                {
                    if (mImgSwapEnabled && mImg != null)
                    {
                        mImg.sprite = mImgInactive;
                    }
                    else
                    {
                        //Use grey material here
                        transform.localScale = mInitialScale;
                        foreach (var obj in relatedObjects)
                            obj.localScale = mInitialScale;
                        if (imgMaterial != null)
                            imgMaterial = GetGreyMat();
                    }
                }
            }
            catch (Exception ex)
            {

                Debug.LogError(ex.ToString());
            }
          
        }

        public virtual void SetInactiveStateAction(Action pAction)
        {
            mInactionStateAction = pAction;
            enabled = mActive || mInactionStateAction != null;
        }

        protected override void Start()
        {
            base.Start();

            mPrePivot = mPivotForFX;
            if (mEnabledFX)
            {
                RefreshPivot(rectTransform);

                if (relatedObjects != null)
                    foreach (var obj in relatedObjects)
                        RefreshPivot(obj);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (mEnabledFX)
            {
                transform.localScale = mInitialScale;
                if (relatedObjects != null)
                    foreach (var obj in relatedObjects)
                        obj.localScale = mInitialScale;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (targetGraphic == null)
            {
                var images = gameObject.GetComponentsInChildren<Image>();
                if (images.Length > 0)
                {
                    targetGraphic = images[0];
                    mImg = targetGraphic as Image;
                }
            }
            else if (mImg == null)
                mImg = targetGraphic as Image;

            RefreshPivot();
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();

            if (mEnabledFX)
            {
                transform.localScale = mInitialScale;
                if (relatedObjects != null)
                    foreach (var obj in relatedObjects)
                        obj.localScale = mInitialScale;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!mActive && mInactionStateAction != null)
                mInactionStateAction();

            if (mActive)
            {
                base.OnPointerDown(eventData);
                if (onPointerDown != null)
                    onPointerDown();
            }

            if (mEnabledFX)
            {
                if (mPivotForFX != mPrePivot)
                {
                    mPrePivot = mPivotForFX;
                    RefreshPivot(rectTransform);
                    if (relatedObjects != null)
                    {
                        foreach (var obj in relatedObjects)
                            RefreshPivot(obj);
                    }
                }

                transform.localScale = mInitialScale * 0.95f;
                if (relatedObjects != null)
                    if (relatedObjects != null)
                    {
                        foreach (var obj in relatedObjects)
                            obj.localScale = mInitialScale * 0.95f;
                    }
            }

            if (mPlaySound)
            {
                if (mActive) SoundManager.Instance.PlaySFX(IDs.SOUND_BUTTON_CLICK, 1f);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (mActive)
            {
                base.OnPointerUp(eventData);
                if (onPointerUp != null)
                    onPointerUp();
            }

            if (mEnabledFX)
            {
                transform.localScale = mInitialScale;
                if (relatedObjects != null)
                    foreach (var obj in relatedObjects)
                        obj.localScale = mInitialScale;
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (mActive)
                base.OnPointerClick(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (mActive)
                base.OnSelect(eventData);
        }

        public void RefreshPivot()
        {
            RefreshPivot(rectTransform);
            if (relatedObjects != null)
            {
                foreach (var obj in relatedObjects)
                    RefreshPivot(obj);
            }
        }

        private void RefreshPivot(RectTransform pRect)
        {
            switch (mPivotForFX)
            {
                case PivotForScale.Bot:
                    SetPivot(pRect, new Vector2(0.5f, 0));
                    break;
                case PivotForScale.BotLeft:
                    SetPivot(pRect, new Vector2(0, 0));
                    break;
                case PivotForScale.BotRight:
                    SetPivot(pRect, new Vector2(1, 0));
                    break;
                case PivotForScale.Top:
                    SetPivot(pRect, new Vector2(0.5f, 1));
                    break;
                case PivotForScale.TopLeft:
                    SetPivot(pRect, new Vector2(0, 1f));
                    break;
                case PivotForScale.TopRight:
                    SetPivot(pRect, new Vector2(1, 1f));
                    break;
                case PivotForScale.Center:
                    SetPivot(pRect, new Vector2(0.5f, 0.5f));
                    break;
            }
        }

        public void SetPivot(RectTransform rectTransform, Vector2 pivot)
        {
            if (rectTransform == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        public Material GetGreyMat()
        {
            if (mGreyMat == null)
                //mGreyMat = new Material(Shader.Find("NBCustom/Sprites/Greyscale"));
                mGreyMat = Resources.Load<Material>("Greyscale");
            return mGreyMat;
        }

        // public void GreyOn()
        // {
        //     imgMaterial = GetGreyMat();
        // }
        //
        // public void GreyOff()
        // {
        //     imgMaterial = null;
        // }

        public bool Enabled() { return enabled && mActive; }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(JustButton), true)]
    class JustButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical("box");
            {
                EditorHelper.SerializeField(serializedObject, "mImg");
                EditorHelper.SerializeField(serializedObject, "mPivotForFX");
                EditorHelper.SerializeField(serializedObject, "mEnabledFX");
                EditorHelper.SerializeField(serializedObject, "mPlaySound");
                var imgSwapEnabled = EditorHelper.SerializeField(serializedObject, "mImgSwapEnabled");
                if (imgSwapEnabled.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical("box");
                    EditorHelper.SerializeField(serializedObject, "mImgActive");
                    EditorHelper.SerializeField(serializedObject, "mImgInactive");
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }

                SerializedProperty relatedObjs = serializedObject.FindProperty("mRelatedObjects");
                if (relatedObjs.isExpanded)
                    EditorGUILayout.PropertyField(relatedObjs, true);
                else
                    EditorGUILayout.PropertyField(relatedObjs, new GUIContent(relatedObjs.displayName));
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        [UnityEditor.MenuItem("DevTools/UI/Replace Button By JustButton")]
        private static void ReplaceButton()
        {
            var gameobjects = UnityEditor.Selection.gameObjects;
            for (int i = 0; i < gameobjects.Length; i++)
            {
                var btns = gameobjects[i].FindComponentsInChildren<Button>();
                for (int j = 0; j < btns.Count; j++)
                {
                    var btn = btns[j];
                    if (!(btn is JustButton))
                    {
                        var obj = btn.gameObject;
                        DestroyImmediate(btn);
                        obj.AddComponent<JustButton>();
                    }
                }
            }
        }
    }
#endif
}
