#pragma warning disable 0649
//#define USE_DOTWEEN

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Debug = UnityEngine.Debug;
#if USE_DOTWEEN
using DG.Tweening;
using FoodZombie.UI;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utilities.Components
{
    /// <summary>
    /// Create a hole at a recttransform target
    /// And everythings outside that hole will not be interable
    /// </summary>
    public class HoledLayerMask : MonoBehaviour
    {
        public RectTransform rectContainer;
        public Image imgHole;
        public Image imgLeft;
        public Image imgRight;
        public Image imgTop;
        public Image imgBot;
        public RectTransform testTarget;

        private RectTransform mRectHole;
        private RectTransform mRectBorderLeft;
        private RectTransform mRectBorderRight;
        private RectTransform mRectBorderTop;
        private RectTransform mRectBorderBot;
        private RectTransform mCurrentTarget;
        private bool mTweening;

        private Bounds mBounds;

        float _temp_delayTouch = 1;
        private void OnEnable()
        {
            mRectHole = imgHole.rectTransform;
            mRectBorderLeft = imgLeft.rectTransform;
            mRectBorderRight = imgRight.rectTransform;
            mRectBorderTop = imgTop.rectTransform;
            mRectBorderBot = imgBot.rectTransform;

            mBounds = rectContainer.Bounds();
            mRectBorderTop.pivot = new Vector2(0.5f, 1f);
            mRectBorderBot.pivot = new Vector2(0.5f, 0f);
            mRectBorderLeft.pivot = new Vector2(0, 0.5f);
            mRectBorderRight.pivot = new Vector2(1f, 0.5f);
            _temp_delayTouch = 1;
        }

        private void Update()
        {
            //Change size of 4-border following the size and position of hole
            var canvasWidth = mBounds.size.x;
            var canvasHeight = mBounds.size.y;
            var holePosition = mRectHole.anchoredPosition;
            var holeHalfSize = mRectHole.sizeDelta / 2f;
            float borderLeft = mBounds.min.x;
            float borderRight = mBounds.max.x;
            float borderTop = mBounds.max.y;
            float borderBot = mBounds.min.y;
            float layerLeftW = (holePosition.x - holeHalfSize.x) - borderLeft;
            float layerRightW = borderRight - (holePosition.x + holeHalfSize.x);
            float layerLeftH = mRectHole.sizeDelta.y;
            float layerRightH = mRectHole.sizeDelta.y;
            float layerTopW = mBounds.size.x;
            float layerBotW = mBounds.size.x;
            float layerTopH = borderTop - (holePosition.y + holeHalfSize.y);
            float layerBotH = (holePosition.y - holeHalfSize.y) - borderBot;

            mRectBorderLeft.sizeDelta = new Vector2(layerLeftW, layerLeftH);
            mRectBorderRight.sizeDelta = new Vector2(layerRightW, layerRightH);
            mRectBorderTop.sizeDelta = new Vector2(layerTopW, layerTopH);
            mRectBorderBot.sizeDelta = new Vector2(layerBotW, layerBotH);

            var leftLayerPos = mRectBorderLeft.anchoredPosition;
            leftLayerPos.y = mRectHole.anchoredPosition.y;
            mRectBorderLeft.anchoredPosition = leftLayerPos;
            var rightLayerPos = mRectBorderRight.anchoredPosition;
            rightLayerPos.y = mRectHole.anchoredPosition.y;
            mRectBorderRight.anchoredPosition = rightLayerPos;

            if (mCurrentTarget != null && !mTweening)
                mRectHole.sizeDelta = new Vector2(mCurrentTarget.rect.width * mCurrentTarget.localScale.x, mCurrentTarget.rect.height * mCurrentTarget.localScale.y);

            if (_temp_delayTouch > 0)
                _temp_delayTouch -= Time.unscaledDeltaTime;
        }

        public void Active(bool pValue)
        {
            enabled = pValue;
            gameObject.SetActive(pValue);
        }

        public void SetColor(Color pColor)
        {
            imgLeft.color = pColor;
            imgTop.color = pColor;
            imgRight.color = pColor;
            imgBot.color = pColor;
        }

        public void Lock(bool pPostChecking = true)
        {
            Active(true);

            mBounds = rectContainer.Bounds();
            mCurrentTarget = null;

            var targetPivot = new Vector2(0.5f, 0.5f);
            mRectHole.position = new Vector2(0f, 0f);
            mRectHole.sizeDelta = new Vector2(0f, 0f);
            mRectHole.anchoredPosition = new Vector2(0f, 0f);

            imgHole.raycastTarget = false;

            if (pPostChecking)
                StartCoroutine(IELock());
        }

        public void FocusToTargetImmediately(RectTransform pTarget, bool pPostChecking = true)
        {
            Active(true);

            mBounds = rectContainer.Bounds();
            mCurrentTarget = pTarget;

            var targetPivot = pTarget.pivot;
            mRectHole.position = pTarget.position;
            mRectHole.sizeDelta = new Vector2(pTarget.rect.width, pTarget.rect.height);
            var x = mRectHole.anchoredPosition.x - pTarget.rect.width * targetPivot.x + pTarget.rect.width / 2f;
            var y = mRectHole.anchoredPosition.y - pTarget.rect.height * targetPivot.y + pTarget.rect.height / 2f;
            mRectHole.anchoredPosition = new Vector2(x, y);

            imgHole.raycastTarget = false;

            if (pPostChecking)
                StartCoroutine(IEPostValidating(pTarget));
        }

        /// <summary>
        /// NOTE: need more test
        /// Focus to a object in 2D world 
        /// </summary>
        public void FocusToTargetImmediately(SpriteRenderer pTarget, RectTransform pMainCanvas, Camera pWorldCamera, Vector2 pOFfset)
        {
            Active(true);

            mBounds = rectContainer.Bounds();
            var wRecSprite = new Vector2(pTarget.transform.lossyScale.x * pTarget.size.x * 100f + pOFfset.x,
                pTarget.transform.lossyScale.y * pTarget.size.y * 100f + pOFfset.y);
            Vector2 viewportPoint = WorldToCanvas(pMainCanvas, pTarget.transform.position, pWorldCamera);
            mRectHole.localPosition = new Vector3(viewportPoint.x, viewportPoint.y);
            mRectHole.sizeDelta = new Vector2(wRecSprite.x, wRecSprite.y);
            imgHole.raycastTarget = false;

            // hand.SetActive(true);
            // handFlip.SetActive(false);
        }

        public void FocusToTarget(RectTransform pTarget, float pTime)
        {
            Active(true);

            mBounds = rectContainer.Bounds();
            mCurrentTarget = pTarget;

            Vector2 fromSize = new Vector2(pTarget.rect.width, pTarget.rect.height) * 10;
            Vector2 toSize = new Vector2(pTarget.rect.width, pTarget.rect.height);

            mRectHole.position = pTarget.position;
            mRectHole.sizeDelta = fromSize;
            var targetPivot = pTarget.pivot;
            var x = mRectHole.anchoredPosition.x - pTarget.rect.width * targetPivot.x + pTarget.rect.width / 2f;
            var y = mRectHole.anchoredPosition.y - pTarget.rect.height * targetPivot.y + pTarget.rect.height / 2f;
            mRectHole.anchoredPosition = new Vector2(x, y);
#if USE_DOTWEEN
            DOTween.Kill(imgHole.GetInstanceID());
            if (pTime > 0)
            {
                imgHole.raycastTarget = true;
                float val = 0;
                DOTween.To(() => val, xx => val = xx, 1f, pTime)
                    .OnUpdate(() =>
                    {
                        mRectHole.sizeDelta = Vector2.Lerp(fromSize, toSize, val);
                    })
                    .OnComplete(() =>
                    {
                        imgHole.raycastTarget = false;
                    })
                    .SetUpdate(true)
                    .SetId(imgHole.GetInstanceID());
            }
            else
            {
                imgHole.raycastTarget = false;
                mRectHole.sizeDelta = toSize;
            }
#else
            mRectHole.sizeDelta = toSize;
#endif
        }

        //public void FocusToTargetImmediately(RectTransform pTarget, bool pPostChecking = true)
        //{
        //    Active(true);

        //    mBounds = rectContainer.Bounds();
        //    mCurrentTarget = pTarget;

        //    var targetPivot = pTarget.pivot;
        //    mRectHole.position = pTarget.position;
        //    mRectHole.sizeDelta = new Vector2(pTarget.rect.width, pTarget.rect.height);
        //    var x = mRectHole.anchoredPosition.x - pTarget.rect.width * targetPivot.x + pTarget.rect.width / 2f;
        //    var y = mRectHole.anchoredPosition.y - pTarget.rect.height * targetPivot.y + pTarget.rect.height / 2f;
        //    mRectHole.anchoredPosition = new Vector2(x, y);

        //    imgHole.raycastTarget = false;

        //    if (pPostChecking)
        //        StartCoroutine(IEPostValidating(pTarget));
        //}

        public RectTransform GetHoleTransform()
        {
            return mRectHole;
        }
        /// <summary>
        /// NOTE: need more test
        /// Focus to a object in 2D world 
        /// </summary>
        public void FocusToTargetImmediately(SpriteRenderer pTarget, Vector3 posTarget, RectTransform pMainCanvas, Camera pWorldCamera, Vector2 pOFfset)
        {
            Active(true);

            mBounds = rectContainer.Bounds();
            Vector2 viewportPoint = Vector2.zero;
            Vector2 wRecSprite = Vector2.zero;
            if (pTarget != null)
            {
                wRecSprite = new Vector2(pTarget.transform.lossyScale.x * pTarget.size.x * 100f + pOFfset.x,
                    pTarget.transform.lossyScale.y * pTarget.size.y * 100f + pOFfset.y);
                viewportPoint = WorldToCanvas(pMainCanvas, pTarget.transform.position, pWorldCamera);
            }
            else
            {
                wRecSprite = pOFfset;
                viewportPoint = WorldToCanvas(pMainCanvas, posTarget, pWorldCamera);
            }
            mRectHole.localPosition = new Vector3(viewportPoint.x, viewportPoint.y);
            mRectHole.sizeDelta = new Vector2(wRecSprite.x, wRecSprite.y);
            imgHole.raycastTarget = false;
        }
        public void FocusToTargetImmediately(RectTransform pTarget, RectTransform pMainCanvas, Camera pWorldCamera, Vector2 pOFfset)
        {
            Active(true);

            mBounds = rectContainer.Bounds();
            Vector2 viewportPoint = Vector2.zero;
            Vector2 wRecSprite = Vector2.zero;
            {
                wRecSprite = new Vector2(pTarget.transform.lossyScale.x + pOFfset.x,
                    pTarget.transform.lossyScale.y + pOFfset.y);
                viewportPoint = WorldToCanvas(pMainCanvas, pTarget.transform.position, pWorldCamera);
            }
            mRectHole.localPosition = new Vector3(viewportPoint.x, viewportPoint.y);
            mRectHole.sizeDelta = new Vector2(wRecSprite.x, wRecSprite.y);
            imgHole.raycastTarget = false;
        }

        private Vector2 WorldToCanvas(RectTransform pMainCanvas, Vector3 pWorldPosition, Camera pWorldCamera = null)
        {
            if (pWorldCamera == null)
                pWorldCamera = Camera.main;

            var viewport_position = pWorldCamera.WorldToViewportPoint(pWorldPosition);
            return new Vector2((viewport_position.x * pMainCanvas.sizeDelta.x) - (pMainCanvas.sizeDelta.x * 0.5f),
                               (viewport_position.y * pMainCanvas.sizeDelta.y) - (pMainCanvas.sizeDelta.y * 0.5f));
        }

        /// <summary>
        /// lockPanel
        /// We need to observer target longer
        /// </summary>
        ///
        private IEnumerator IELock()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
                Lock(false);
            }
        }

        /// <summary>
        /// Incase target is in a scrollview or some sort of UI element which take one or few frames to refresh
        /// We need to observer target longer
        /// </summary>
        ///
        private IEnumerator IEPostValidating(RectTransform pTarget)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return null;
                FocusToTargetImmediately(pTarget, false);
            }
        }

        /// <summary>
        /// Make a clone of sprite and use it as a mask to cover around target
        /// NOTE: condition is source sprite texture must be TRUE COLOR, and it's Read/Write enabled
        /// </summary>
        public void CreateHoleFromSprite(Sprite spriteToClone)
        {
            try
            {
                int posX = (int)spriteToClone.rect.x;
                int posY = (int)spriteToClone.rect.y;
                int sizeX = (int)(spriteToClone.bounds.size.x * spriteToClone.pixelsPerUnit);
                int sizeY = (int)(spriteToClone.bounds.size.y * spriteToClone.pixelsPerUnit);

                Texture2D newTex = new Texture2D(sizeX, sizeY, spriteToClone.texture.format, false);
                Color[] colors = spriteToClone.texture.GetPixels(posX, posY, sizeX, sizeY);

                for (int i = 0; i < colors.Length; i++)
                {
                    if (colors[i].a == 0)
                        colors[i] = Color.white;
                    else
                        colors[i] = Color.clear;
                }
                newTex.SetPixels(colors);
                newTex.Apply();

                Sprite sprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), spriteToClone.pivot, spriteToClone.pixelsPerUnit, 0, SpriteMeshType.Tight, spriteToClone.border);
                imgHole.sprite = sprite;
                imgHole.color = imgLeft.color;
                if (sprite.border != Vector4.zero)
                    imgHole.type = Image.Type.Sliced;
                else
                    imgHole.type = Image.Type.Simple;
            }
            catch (Exception ex)
            {
                imgHole.sprite = null;
                imgHole.color = Color.clear;
                Debug.LogError(ex.ToString());
            }
        }

        public void ClearSpriteMask()
        {
            imgHole.sprite = null;
            imgHole.color = Color.clear;
        }
        public void OnTouch(bool active)
        {
            // Debug.Log("On Touch " + active);
            if (active && _temp_delayTouch <= 0)
            {
                var s = Localization.Get(Localization.ID.TUTORIAL_97);
                FoodZombie.UI.MessageWithPointer board = null;
                //144,-120 or 144,-620
                if (MainGamePanel.instance != null)
                    board = MainGamePanel.instance.ShowNotificationBoard(this.rectContainer, Localization.Get(Localization.ID.TUTORIAL_97), Alignment.Bot,
                       new Vector2(144f, -120f), true, true, false, true);
                else
                    board = MainPanel.instance.ShowNotificationBoard(this.rectContainer, Localization.Get(Localization.ID.TUTORIAL_97), Alignment.Bot,
                     new Vector2(144f, -120f), true, true, false, true);
            }
            //if (active)
            //    Time.timeScale = 3;
            //else
            //{

            //    if (GameplayController.Instance != null)
            //        GameplayController.Instance.LoadTimeScale();
            //    else
            //        Time.timeScale = 1;
            //}
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(HoledLayerMask))]
    public class HoledLayerMaskEditor : Editor
    {
        private HoledLayerMask mScript;
        private Sprite mSprite;

        private void OnEnable()
        {
            mScript = (HoledLayerMask)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            mSprite = (Sprite)EditorGUILayout.ObjectField(mSprite, typeof(Sprite), true);

            if (GUILayout.Button("Clone Sprite"))
                mScript.CreateHoleFromSprite(mSprite);
            if (GUILayout.Button("Focus To Test Target"))
                mScript.FocusToTargetImmediately(mScript.testTarget);
        }
    }
#endif
}
