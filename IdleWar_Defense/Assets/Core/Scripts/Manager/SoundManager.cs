using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FoodZombie
{
    public class SoundManager : MonoBehaviour
    {
        [System.Serializable]
        public class Sound : IComparable<Sound>
        {
            public string fileName;
            public int id;
            public int limitNumber = 1;
            public AudioClip clip;

            public int CompareTo(Sound other)
            {
                return id.CompareTo(other.id);
            }
        }

        #region Members

        private static SoundManager mInstance;
        public static SoundManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = FindObjectOfType<SoundManager>();
                }
                return mInstance;
            }
        }

        private const int TOTAL_SOURCE = 60;

        public List<Sound> SFXClips;
        public List<Sound> musicClips;

        private AudioSource mMusicSource;
        private List<AudioSource> mSFXSources = new List<AudioSource>();
        private Queue<Action> mDelayedAction = new Queue<Action>();

        private bool mEnableMusic = true;
        private bool mEnableSFX = true;
        private bool mInitializedMusic;
        private bool mInitialziedSFX;
        private Tweener mMusicTweener;

        private static Dictionary<string, Sound> cachedSounds = new Dictionary<string, Sound>();

        #endregion

        //=============================================

        #region MonoBehaviour

        private void Awake()
        {
            if (mInstance == null)
                mInstance = this;
            else if (mInstance != this)
                Destroy(gameObject);
        }

        private IEnumerator Start()
        {
            mMusicSource = new GameObject("Music").AddComponent<AudioSource>();
            mMusicSource.transform.SetParent(transform);
            mMusicSource.loop = true;
            mMusicSource.playOnAwake = false;
            mMusicSource.mute = !mEnableMusic;
            mInitializedMusic = true;

            yield return null;

            mSFXSources = new List<AudioSource>();
            for (int i = 0; i < TOTAL_SOURCE; i++)
            {
                var audioSource = new GameObject("Sfx").AddComponent<AudioSource>();
                audioSource.transform.SetParent(transform);
                audioSource.loop = false;
                audioSource.playOnAwake = false;
                audioSource.mute = !mEnableSFX;
                mSFXSources.Add(audioSource);

                yield return null;
            }

            mInitialziedSFX = true;

            GameData.Instance.GameConfigGroup.onEnableMuisic += OnEnableMusic;
            GameData.Instance.GameConfigGroup.onEnableSFX += OnEnableSFX;

            if (mDelayedAction != null && mDelayedAction.Count > 0)
            {
                foreach (var aaction in mDelayedAction)
                    aaction();

                mDelayedAction.Clear();
            }

            yield return new WaitForSeconds(1f);

            mEnableMusic = GameData.Instance.GameConfigGroup.EnableMusic;
            mEnableSFX = GameData.Instance.GameConfigGroup.EnableSFX;
            mMusicSource.mute = !mEnableMusic;
            foreach (var s in mSFXSources)
                s.mute = !mEnableSFX;
        }

        #endregion

        //=============================================

        #region Public

        //SFX

        public void PlaySFX(string pFileName, float volume = 0.35f, bool pLoop = false)
        {
            if (!cachedSounds.ContainsKey(pFileName))
            {
                var sound = GetSound(pFileName, false);
                if (sound != null)
                    cachedSounds.Add(pFileName, sound);
                PlaySFX(sound, volume, pLoop);
            }
            else
            {
                var sound = cachedSounds[pFileName];
                PlaySFX(sound, volume, pLoop);
            }
        }

        public void PlaySFX(int pId, float volume = 0.35f, bool pLoop = false)
        {
            PlaySFX(GetSound(pId, false), volume, pLoop);
        }

        public void StopSFX(int pId)
        {
            StopSFX(GetSound(pId, false));
        }
        public void StopAllSFX(int pId)
        {
            StopAllSFX(GetSound(pId, false));
        }

        //MUSIC

        public void PlayMusic(string pFileName, bool pFade = true)
        {
            PlayMusic(GetSound(pFileName, true), pFade);
        }

        public void PlayMusic(int pId, bool pFade = true)
        {
            PlayMusic(GetSound(pId, true), pFade);
        }

        public void StopMusic(bool pFade = false)
        {
            if (mMusicTweener != null)
                mMusicTweener.Kill();

            if (!pFade)
            {
                mMusicSource.Stop();
            }
            else
            {
                mMusicTweener = mMusicSource.DOFade(0, 1f)
                    .OnComplete(() =>
                    {
                        mMusicSource.volume = 1;
                        mMusicSource.Stop();
                    });
            }
        }

        //=========

        #endregion

        //==============================================

        #region Private

        private Sound GetSound(int pId, bool isMusic)
        {
            if (isMusic)
            {
                foreach (var m in musicClips)
                    if (m.id == pId)
                    {
#if UNITY_EDITOR
                        if (m.clip == null)
                            UnityEngine.Debug.LogError("No Sound for " + m.fileName);
#endif
                        return m;
                    }
            }
            else
            {
                foreach (var m in SFXClips)
                    if (m.id == pId)
                    {
#if UNITY_EDITOR
                        if (m.clip == null)
                            UnityEngine.Debug.LogError("No Sound for " + m.fileName);
#endif
                        return m;
                    }
            }
            return null;
        }

        private Sound GetSound(string pFileName, bool isMusic)
        {
            if (isMusic)
            {
                foreach (var m in musicClips)
                    if (m.fileName == pFileName)
                        return m;
            }
            else
            {
                foreach (var m in SFXClips)
                    if (m.fileName == pFileName)
                        return m;
            }
            return null;
        }

        private AudioSource GetSFXSource(AudioClip pClip, int pLimit)
        {
            return GetSFXSource(pClip.GetInstanceID(), pLimit);
        }

        private AudioSource GetSFXSource(int pClipInstanceId, int pLimit)
        {
            //Find same sfx
            int countSame = 0;
            for (int i = mSFXSources.Count - 1; i >= 0; i--)
            {
                if (!mSFXSources[i].isPlaying)
                {
                    mSFXSources[i].clip = null;
                }

                if (mSFXSources[i].clip != null && mSFXSources[i].clip.GetInstanceID() == pClipInstanceId)
                {
                    countSame++;
                    if (countSame >= pLimit)
                    {
                        var source = mSFXSources[i];
                        mSFXSources.Remove(source);
                        mSFXSources.Add(source);
                        return source;
                    }
                }
            }

            //Find emppty sfx source
            for (int i = 0; i < mSFXSources.Count; i++)
            {
                if (!mSFXSources[i].isPlaying)
                {
                    //Move new SFX to last position
                    var source = mSFXSources[i];
                    mSFXSources.Remove(source);
                    mSFXSources.Add(source);
                    return source;
                }
            }

            return mSFXSources[0];
        }

        private void OnEnableSFX(bool pActive)
        {
            mEnableSFX = pActive;
            foreach (var s in mSFXSources)
                s.mute = !mEnableSFX;
        }

        private void OnEnableMusic(bool pActive)
        {
            mEnableMusic = pActive;
            if (pActive)
            {
                mMusicSource.mute = false;
                mMusicSource.Play();
            }
            else
            {
                mMusicSource.mute = true;
                mMusicSource.Stop();
            }
        }

        private void PlayMusic(Sound pSound, bool pFade)
        {
            if (!mInitializedMusic)
            {
                mDelayedAction.Enqueue(() => { PlayMusic(pSound, pFade); });
                return;
            }

            try
            {
                if (pSound == null) return;

                mMusicSource.clip = pSound.clip;

                if (!mEnableMusic || pSound.clip == null || !mInitializedMusic)
                    return;

                if (mMusicTweener != null)
                    mMusicTweener.Kill();
                if (!pFade)
                {
                    mMusicSource.volume = 1;
                }
                else
                {
                    mMusicSource.volume = 0;
                    mMusicTweener = mMusicSource.DOFade(1f, 3f);
                }
                mMusicSource.Play();
            }
            catch { }
        }

        private void PlaySFX(Sound pSound, float volume, bool pLoop)
        {
            if (!mInitialziedSFX)
                return;

            if (!mEnableSFX || pSound == null || pSound.clip == null || !mInitialziedSFX)
                return;

            var source = GetSFXSource(pSound.clip, pSound.limitNumber);
            source.volume = volume;
            source.loop = pLoop;
            source.clip = pSound.clip;
            if (!pLoop)
                source.PlayOneShot(pSound.clip);
            else
                source.Play();
#if UNITY_EDITOR
            source.name = "SFX" + pSound.fileName;
#endif
        }

        private void StopSFX(Sound pSound)
        {
            if (pSound == null || pSound.clip == null)
                return;

            for (int i = mSFXSources.Count - 1; i >= 0; i--)
            {
                if (mSFXSources[i].clip == pSound.clip)
                {
                    mSFXSources[i].Stop();
                    mSFXSources[i].clip = null;

                    break;
                }
            }
        }
        private void StopAllSFX(Sound pSound)
        {
            if (pSound == null || pSound.clip == null)
                return;

            for (int i = mSFXSources.Count - 1; i >= 0; i--)
            {
                if (mSFXSources[i].clip == pSound.clip)
                {
                    mSFXSources[i].Stop();
                    mSFXSources[i].clip = null;
                }
            }
        }

        #endregion
    }
}