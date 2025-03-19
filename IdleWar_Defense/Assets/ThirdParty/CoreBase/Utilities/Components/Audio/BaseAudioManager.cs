//#define USE_DOTWEEN

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using System;
#if USE_DOTWEEN
using DG.Tweening;
#endif
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utilities.Components
{
    public class BaseAudioManager : MonoBehaviour
    {
        [SerializeField] protected bool mEnabledSFX = true;
        [SerializeField] protected bool mEnabledMusic = true;
        [SerializeField] protected AudioSource[] mSFXSources;
        [SerializeField] protected AudioSource mSFXSourceUnlimited;
        [SerializeField] protected AudioSource mMusicSource;

        public bool EnabledSFX => mEnabledSFX;
        public bool EnabledMusic => mEnabledMusic;
        public float MusicVolume => mMusicSource.volume;

        private bool mStopPlaying;

        public void EnableMusic(bool pValue)
        {
            mEnabledMusic = pValue;
            mMusicSource.mute = !pValue;

            if (!mMusicSource.isPlaying)
                mMusicSource.Play();
        }

        public void EnableSFX(bool pValue)
        {
            mEnabledSFX = pValue;
            foreach (var s in mSFXSources)
                s.mute = !pValue;
        }

        public void SetMusicVolume(float pValue, float pFadeDuration = 0, Action pOnComplete = null)
        {
            if (mStopPlaying)
                return;

#if USE_DOTWEEN
            DOTween.Kill(mMusicSource.GetInstanceID());
            if (pFadeDuration <= 0)
            {
                mMusicSource.volume = pValue;
                pOnComplete?.Invoke();
            }
            else
                mMusicSource.DOFade(pValue, pFadeDuration)
                    .SetUpdate(true)
                    .SetId(mMusicSource.GetInstanceID())
                    .OnComplete(() => { pOnComplete?.Invoke(); });
#else
            float from = mMusicSource.volume;
            StartCoroutine(IELerp(pFadeDuration, (lerp) =>
            {
                mMusicSource.volume = from + (lerp * (pValue - from));
            }, () =>
            {
                pOnComplete?.Invoke();
            }));
#endif
        }

        public void SetSFXVolume(float pValue, float pFadeDuration = 0, Action pOnComplete = null)
        {
            if (mStopPlaying)
                return;

            for (int i = 0; i < mSFXSources.Length; i++)
            {
#if USE_DOTWEEN
                if (mSFXSources[i].isPlaying)
                {
                    DOTween.Kill(mSFXSources[i].GetInstanceID());
                    if (pFadeDuration <= 0)
                    {
                        mSFXSources[i].volume = pValue;
                        pOnComplete?.Invoke();
                    }
                    else
                        mSFXSources[i].DOFade(pValue, pFadeDuration)
                            .SetUpdate(true)
                            .SetId(mSFXSources[i].GetInstanceID())
                            .OnUpdate(() => { pOnComplete?.Invoke(); });
                }
#else
                mSFXSources[i].volume = pValue;
                pOnComplete?.Invoke();
#endif
            }
        }

        public void StopMusic(float pFadeDuration = 0, Action pOnComplete = null)
        {
            if (!mMusicSource.isPlaying)
                return;

            mStopPlaying = true;

#if USE_DOTWEEN
            DOTween.Kill(mMusicSource.GetInstanceID());
            if (pFadeDuration <= 0)
            {
                mMusicSource.Stop();
                pOnComplete?.Invoke();
            }
            else
            {
                mMusicSource.DOFade(0, pFadeDuration)
                    .OnComplete(() =>
                    {
                        mMusicSource.volume = 1;
                        mMusicSource.Stop();
                        pOnComplete?.Invoke();
                    }).SetId(mMusicSource.GetInstanceID()).SetUpdate(true);
            }
#else
            if (pFadeDuration <= 0)
                mMusicSource.Stop();
            else
            {
                StartCoroutine(IELerp(1f,
                    (lerp) =>
                    {
                        mMusicSource.volume = 1 - lerp;
                    }, () =>
                    {
                        mMusicSource.volume = 1;
                        mMusicSource.Stop();
                        pOnComplete?.Invoke();
                    }));
            }
#endif
        }

        public void StopSFX(AudioClip pClip)
        {
            if (pClip == null)
                return;
            for (int i = 0; i < mSFXSources.Length; i++)
            {
                if (mSFXSources[i].clip != null && mSFXSources[i].clip.GetInstanceID() == pClip.GetInstanceID())
                {
                    mSFXSources[i].Stop();
                    mSFXSources[i].clip = null;
                }
            }
        }

        public void StopSFXs()
        {
            for (int i = 0; i < mSFXSources.Length; i++)
            {
                mSFXSources[i].Stop();
                mSFXSources[i].clip = null;
            }
        }

        protected void CreateAudioSources()
        {
            var sfxSources = new List<AudioSource>();
            var audioSources = gameObject.FindComponentsInChildren<AudioSource>();
            for (int i = 0; i < audioSources.Count; i++)
            {
                if (i == 0)
                {
                    mMusicSource = audioSources[i];
                    mMusicSource.name = "Music";
                }
                else
                {
                    sfxSources.Add(audioSources[i]);
                    audioSources[i].name = "SFX_" + i;
                }
            }
            if (sfxSources.Count < 5)
                for (int i = sfxSources.Count; i <= 15; i++)
                {
                    var obj = new GameObject("SFX_" + i);
                    obj.transform.SetParent(transform);
                    sfxSources.Add(obj.AddComponent<AudioSource>());
                }
            mSFXSources = sfxSources.ToArray();
        }

        protected AudioSource CreateMoreSFXSource()
        {
            var obj = new GameObject("SFX_" + mSFXSources.Length);
            obj.transform.SetParent(transform);
            var newAudioSource = obj.AddComponent<AudioSource>();
            mSFXSources = mSFXSources.Add(newAudioSource);
            return newAudioSource;
        }

        protected IEnumerator IELerp(float pTime, Action<float> pOnUpdate, Action pOnFinished)
        {
            float time = 0;
            while (true)
            {
                time += Time.deltaTime;
                if (pTime > time)
                    break;
                pOnUpdate.Raise(time / pTime);
                yield return null;
            }
            pOnFinished.Raise();
        }

        protected AudioSource GetSFXSouce(AudioClip pClip, int pLimitNumber, bool pLoop)
        {
            try
            {
                if (pLimitNumber > 0 || pLoop)
                {
                    if (!pLoop)
                    {
                        int countSameClips = 0;
                        for (int i = mSFXSources.Length - 1; i >= 0; i--)
                        {
                            if (mSFXSources[i].isPlaying && mSFXSources[i].clip != null && mSFXSources[i].clip.GetInstanceID() == pClip.GetInstanceID())
                                countSameClips++;
                            else if (!mSFXSources[i].isPlaying)
                                mSFXSources[i].clip = null;
                        }
                        if (countSameClips < pLimitNumber)
                        {
                            for (int i = mSFXSources.Length - 1; i >= 0; i--)
                                if (mSFXSources[i].clip == null)
                                    return mSFXSources[i];

                            return CreateMoreSFXSource();
                        }
                    }
                    else
                    {
                        for (int i = mSFXSources.Length - 1; i >= 0; i--)
                            if (mSFXSources[i].clip == null || !mSFXSources[i].isPlaying
                                || mSFXSources[i].clip.GetInstanceID() == pClip.GetInstanceID())
                                return mSFXSources[i];
                    }
                }
                else
                {
                    return mSFXSourceUnlimited;
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return null;
            }
        }

        public void PlayMusic(float pFadeDuration = 0, float pVolume = 1f)
        {
            mMusicSource.Play();
            mStopPlaying = false;
#if USE_DOTWEEN
            DOTween.Kill(mMusicSource.GetInstanceID());
            if (pFadeDuration <= 0)
                mMusicSource.volume = pVolume;
            else
            {
                mMusicSource.volume = 0;
                mMusicSource.DOFade(pVolume, pFadeDuration).SetUpdate(true)
                    .SetId(mMusicSource.GetInstanceID());
            }
#else
            if (pFadeDuration <= 0)
                mMusicSource.volume = pVolume;
            else
            {
                mMusicSource.volume = 0;
                StartCoroutine(IELerp(3f, (lerp) =>
                {
                    mMusicSource.volume = lerp * pVolume;
                }, () =>
                {
                    mMusicSource.volume = pVolume;
                }));
            }
#endif
        }

        public void PlayMusic(AudioClip pClip, bool pLoop, float pFadeDuration = 0, float pVolume = 1f)
        {
            mStopPlaying = false;

            if (pClip == null)
                return;

            mMusicSource.clip = pClip;
            mMusicSource.loop = pLoop;
            if (!mEnabledMusic) return;
            mMusicSource.Play();

#if USE_DOTWEEN
            DOTween.Kill(mMusicSource.GetInstanceID());
            if (pFadeDuration <= 0)
                mMusicSource.volume = pVolume;
            else
            {
                mMusicSource.volume = 0;
                mMusicSource.DOFade(pVolume, pFadeDuration).SetUpdate(true)
                    .SetId(mMusicSource.GetInstanceID());
            }
#else
            if (pFadeDuration <= 0)
                mMusicSource.volume = pVolume;
            else
            {
                mMusicSource.volume = 0;
                StartCoroutine(IELerp(3f, (lerp) =>
                {
                    mMusicSource.volume = lerp * pVolume;
                }, () =>
                {
                    mMusicSource.volume = pVolume;
                }));
            }
#endif
        }

        public void PlaySFX(AudioClip pClip, int limitNumber, bool pLoop, float pPitchRandomMultiplier = 1)
        {
            if (pClip == null)
                return;
            var source = GetSFXSouce(pClip, limitNumber, pLoop);
            if (source == null)
                return;
            source.volume = 1;
            source.loop = pLoop;
            source.clip = pClip;
            source.pitch = 1;
            if (pPitchRandomMultiplier != 1)
            {
                if (Random.value < .5)
                    source.pitch *= Random.Range(1 / pPitchRandomMultiplier, 1);
                else
                    source.pitch *= Random.Range(1, pPitchRandomMultiplier);
            }
            if (!pLoop)
                source.PlayOneShot(pClip);
            else
                source.Play();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (mSFXSources == null)
                mSFXSources = new AudioSource[0];
            var audioSources = gameObject.FindComponentsInChildren<AudioSource>();
            for (int i = mSFXSources.Length - 1; i >= 0; i--)
            {
                if (audioSources.Contains(mSFXSources[i]))
                    audioSources.Remove(mSFXSources[i]);
            }
            if (mMusicSource == null && audioSources.Count > 0)
            {
                mMusicSource = audioSources[0];
                audioSources.RemoveAt(0);
                mMusicSource.name = "Music";
            }
            else if (mMusicSource == null)
            {
                var obj = new GameObject("Music");
                obj.AddComponent<AudioSource>();
                obj.transform.SetParent(transform);
                mMusicSource = obj.GetComponent<AudioSource>();
            }
            if (mSFXSourceUnlimited == null && audioSources.Count > 0)
            {
                mSFXSourceUnlimited = audioSources[0];
                audioSources.RemoveAt(0);
                mSFXSourceUnlimited.name = "SFXUnlimited";
            }
            else if (mSFXSourceUnlimited == null || mSFXSourceUnlimited == mMusicSource)
            {
                var obj = new GameObject("SFXUnlimited");
                obj.AddComponent<AudioSource>();
                obj.transform.SetParent(transform);
                mSFXSourceUnlimited = obj.GetComponent<AudioSource>();
            }
        }

        [CustomEditor(typeof(BaseAudioManager), true)]
        protected class BaseAudioManagerEditor : Editor
        {
            private BaseAudioManager mScript;

            private void OnEnable()
            {
                mScript = target as BaseAudioManager;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (EditorHelper.ButtonColor("Add Music Audio Source", mScript.mMusicSource == null ? Color.green : Color.grey))
                {
                    if (mScript.mMusicSource == null)
                    {
                        var obj = new GameObject("Music");
                        obj.transform.SetParent(mScript.transform);
                        obj.AddComponent<AudioSource>();
                        mScript.mMusicSource = obj.GetComponent<AudioSource>();
                    }
                    if (mScript.mSFXSourceUnlimited == null)
                    {
                        var obj = new GameObject("SFX_Unlimited");
                        obj.transform.SetParent(mScript.transform);
                        obj.AddComponent<AudioSource>();
                        mScript.mMusicSource = obj.GetComponent<AudioSource>();
                    }
                }
                if (EditorHelper.ButtonColor("Add SFX Audio Source"))
                    mScript.CreateMoreSFXSource();
                if (EditorHelper.ButtonColor("Create Audio Sources", Color.green))
                    mScript.CreateAudioSources();
                if (EditorHelper.Button("Stop Music"))
                    mScript.StopMusic(1f);
                if (EditorHelper.Button("Play Music"))
                    mScript.PlayMusic();

                if (GUI.changed)
                    EditorUtility.SetDirty(mScript);
            }
        }
#endif
    }
}