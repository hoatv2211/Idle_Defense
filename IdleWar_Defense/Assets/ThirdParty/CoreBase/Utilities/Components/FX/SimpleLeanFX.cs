﻿//#define USE_LEANTWEEN
//#define USE_DOTWEEN

using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if USE_DOTWEEN
using DG.Tweening;
#endif

namespace Utilities.Components
{
    /// <summary>
    /// Just and example of some simple fx
    /// </summary>
    public class SimpleLeanFX : MonoBehaviour
    {
        private static SimpleLeanFX mInstance;
        public static SimpleLeanFX instance { get { return mInstance; } }

        [SerializeField] AnimationCurve mBubbleAnim;
        [SerializeField] AnimationCurve mFadeInAndOutAnim;
        [SerializeField] AnimationCurve mShakeAnim;

        public Image imgTest;

        private void Start()
        {
            if (mInstance == null)
                mInstance = this;
            else if (mInstance != this)
                Destroy(gameObject);
        }

        public int Bubble(Transform pTarget, float time = 0.5f, Action pOnFinished = null)
        {
            return SimulateBubble(pTarget, 0, 1, instance.mBubbleAnim, time, pOnFinished);
        }

        public int SingleHightLight(Image pTarget, float time = 0.5f, System.Action pOnFinished = null)
        {
            var defaultAlpha = Color.white.a;
            int id = 0;
#if USE_LEANTWEEN
            id = LeanTween.value(0, 1, time)
                .setOnUpdate((float val) =>
                {
                    float curve = instance.mHightLightAnim.Evaluate(val);
                    Color color = pTarget.color;
                    color.a = defaultAlpha * curve;
                    pTarget.color = color;
                })
                .setOnComplete(() =>
                {
                    if (pOnFinished != null) pOnFinished();
                }).id;
#elif USE_DOTWEEN
            float val = 0;
            DOTween.Kill(pTarget.GetInstanceID() + GetInstanceID());
            //var tween = DOTween.To(() => val, x => val = x, 1f, time)
            var tween = DOTween.To(tweenVal => val = tweenVal, 0f, 1f, time)
                .OnUpdate(() =>
                {
                    float curve = instance.mFadeInAndOutAnim.Evaluate(val);
                    Color color = pTarget.color;
                    color.a = defaultAlpha * curve;
                    pTarget.color = color;
                })
                .OnComplete(() =>
                {
                    if (pOnFinished != null) pOnFinished();
                })
                .SetId(pTarget.GetInstanceID() + GetInstanceID()).SetUpdate(true);
            id = (int)tween.intId;
#endif
            return id;
        }

        private int SimulateBubble(Transform pTarget, float pFrom, float pTo, AnimationCurve pAnim, float time, Action pOnFinished)
        {
            var defaulScale = pTarget.transform.localScale;
            pTarget.transform.localScale = Vector3.zero;
            int id = 0;
#if USE_LEANTWEEN
            id = LeanTween.value(pTarget.gameObject, pFrom, pTo, time)
                .setOnUpdate((float val) =>
                {
                    float curve = pAnim.Evaluate(val);

                    pTarget.transform.localScale = defaulScale * curve;
                })
                .setOnComplete(() =>
                {
                    pTarget.transform.localScale = defaulScale;

                    if (pOnFinished != null) pOnFinished();
                }).id;
#elif USE_DOTWEEN
            float val = 0;
            DOTween.Kill(pTarget.GetInstanceID() + GetInstanceID());
            //var tween = DOTween.To(() => val, x => val = x, 1f, time)
            var tween = DOTween.To(tweenVal => val = tweenVal, 0f, 1f, time)
                .OnUpdate(() =>
                {
                    float curve = pAnim.Evaluate(val);

                    pTarget.transform.localScale = defaulScale * curve;
                })
                .OnComplete(() =>
                {
                    pTarget.transform.localScale = defaulScale;

                    if (pOnFinished != null) pOnFinished();
                })
                .SetId(pTarget.GetInstanceID() + GetInstanceID()).SetUpdate(true);

            id = (int)tween.intId;
#endif
            return id;
        }

        public void Shake(Transform pTarget, float pTime, float pIntensity, Action pOnFinished = null)
        {
            Vector3 defaultPos = pTarget.position;
            Vector3 defaultScale = pTarget.localScale;
            Quaternion defaultRotaion = pTarget.rotation;

#if USE_LEANTWEEN
            LeanTween.cancel(pTarget.gameObject);
            LeanTween.value(pTarget.gameObject, 1, 0, pTime)
                .setOnUpdate((float val) =>
                {
                    float curve = instance.mShakeAnim.Evaluate(val);
                    float intensity = pIntensity * curve;

                    var shakingPos = defaultPos + Random.insideUnitSphere * intensity;
                    pTarget.position = shakingPos;

                    var z = defaultRotaion.z + Random.Range(-intensity, intensity);
                    var w = defaultRotaion.w + Random.Range(-intensity, intensity);
                    pTarget.rotation = new Quaternion(0, 0, z, w);
                })
                .setOnComplete(() =>
                {
                    pTarget.position = defaultPos;
                    pTarget.localScale = defaultScale;
                    pTarget.rotation = defaultRotaion;

                    if (pOnFinished != null) pOnFinished();
                });
#elif USE_DOTWEEN
            float val = 0;
            DOTween.Kill(pTarget.GetInstanceID() + GetInstanceID());
            //var tween = DOTween.To(() => val, x => val = x, 1f, time)
            var tween = DOTween.To(tweenVal => val = tweenVal, 0f, 1f, pTime)
                .OnUpdate(() =>
                {
                    float curve = instance.mShakeAnim.Evaluate(val);
                    float intensity = pIntensity * curve;

                    var shakingPos = defaultPos + Random.insideUnitSphere * intensity;
                    pTarget.position = shakingPos;

                    var z = defaultRotaion.z + Random.Range(-intensity, intensity);
                    var w = defaultRotaion.w + Random.Range(-intensity, intensity);
                    pTarget.rotation = new Quaternion(0, 0, z, w);
                })
                .OnComplete(() =>
                {
                    pTarget.position = defaultPos;
                    pTarget.localScale = defaultScale;
                    pTarget.rotation = defaultRotaion;

                    if (pOnFinished != null) pOnFinished();
                })
                .SetId(pTarget.GetInstanceID() + GetInstanceID()).SetUpdate(true);
#endif
        }

        public void FadeIn(Image pImage, float pTime, Action pOnFinished = null)
        {
            Color color = pImage.color;
#if USE_LEANTWEEN
            LeanTween.value(pImage.gameObject, 0f, 1f, pTime)
                .setOnUpdate((float val) =>
                {
                    color.a = val;
                    pImage.color = color;
                })
                .setOnComplete(() =>
                {
                    if (pOnFinished != null)
                        pOnFinished();
                });
#elif USE_DOTWEEN
            color.a = 0f;
            pImage.color = color;
            pImage.DOFade(1f, pTime)
                .OnComplete(() =>
                {
                    if (pOnFinished != null)
                        pOnFinished();
                })
                .SetId(pImage.GetInstanceID() + GetInstanceID()).SetUpdate(true);
#endif
        }

        public void FadeOut(Image pImage, float pTime, Action pOnFinished = null)
        {
            Color color = pImage.color;
#if USE_LEANTWEEN
            LeanTween.value(pImage.gameObject, 1f, 0f, pTime)
                .setOnUpdate((float val) =>
                {
                    color.a = val;
                    pImage.color = color;
                })
                .setOnComplete(() =>
                {
                    if (pOnFinished != null)
                        pOnFinished();
                });
#elif USE_DOTWEEN
            color.a = 1f;
            pImage.color = color;
            pImage.DOFade(0f, pTime)
                .OnComplete(() =>
                {
                    if (pOnFinished != null)
                        pOnFinished();
                })
                .SetId(pImage.GetInstanceID() + GetInstanceID()).SetUpdate(true);
#endif
        }

        public void CreateAnimationCurves()
        {
            mBubbleAnim = new AnimationCurve();
            mBubbleAnim.keys = new Keyframe[] {
                new Keyframe(0, 1f),
                new Keyframe(0.25f, 1.1f),
                new Keyframe(0.5f, 1f),
                new Keyframe(0.75f, 0.9f),
                new Keyframe(1f, 1f),
            };

            mFadeInAndOutAnim = new AnimationCurve();
            mFadeInAndOutAnim.keys = new Keyframe[] {
                new Keyframe(0, 0f),
                new Keyframe(0.5f, 1f),
                new Keyframe(1, 0f)
            };

            mShakeAnim = new AnimationCurve();
            mShakeAnim.keys = new Keyframe[] {
                new Keyframe(0, 1f),
                new Keyframe(0.25f, 1.1f),
                new Keyframe(0.5f, 1f),
                new Keyframe(0.75f, 0.9f),
                new Keyframe(1f, 1f),
            };
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SimpleLeanFX), true)]
    public class SimpleLeanFXEditor : UnityEditor.Editor
    {
        private SimpleLeanFX mObj;

        private void OnEnable()
        {
            mObj = (SimpleLeanFX)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Bubble"))
                mObj.Bubble(mObj.imgTest.transform);

            if (GUILayout.Button("SingleHightLight"))
                mObj.SingleHightLight(mObj.imgTest);

            if (GUILayout.Button("Shake"))
                mObj.Shake(mObj.imgTest.transform, 1f, 0.1f);

            if (GUILayout.Button("FadeIn"))
                mObj.FadeIn(mObj.imgTest, 2f);

            if (GUILayout.Button("FadeOut"))
                mObj.FadeOut(mObj.imgTest, 2f);

            if (GUILayout.Button("CreateAnimationCurves"))
                mObj.CreateAnimationCurves();
        }
    }

#endif
}