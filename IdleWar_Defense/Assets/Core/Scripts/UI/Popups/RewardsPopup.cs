using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class RewardsPopup : MyGamesBasePanel
    {
        public static bool skipTweenTime = false;
        //  [Header("Build-in prefab")]
        [SerializeField] RewardView RewardViewPrefab;
        private List<RewardView> mRewardViewsNoAnimation = new List<RewardView>();
        [SerializeField] private HorizontalAlignmentUI mHorizontalAlignment;
        [SerializeField] private CircleAlignmentUI mCiclerAlignment;
        [SerializeField] private Image mImgBg;
        [SerializeField] private GameObject objTapToNext;
        [SerializeField] private GameObject SummonGr;
        [SerializeField] private RewardHeroSRPopup rewardHeroSRPopup;
        [SerializeField] private SummonNextPopup summonNextPopup;


        MyAlignment mAlignment;
        private List<RewardInfo> mRewards;
        private bool mShowing;
        private RectTransform mPreferedSpawnPos;
        private Color mCoverColor = Color.white;

        public System.Action OnShowDone { get; set; }
        public enum AlignType
        {
            Horizontal, Cicler
        }

        AlignType myAlign = AlignType.Horizontal;

        internal override void Init()
        {
            //  mRewardViewsNoAnimation.Free();
            foreach (RewardView item in mRewardViewsNoAnimation)
            {
                item.transform.parent = null;
            }
            foreach (RewardView item in mRewardViewsNoAnimation)
            {
                SimplePool.Despawn(item.gameObject);
            }
            mRewardViewsNoAnimation.Clear();
            mRewards = new List<RewardInfo>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            foreach (RewardView item in mRewardViewsNoAnimation)
            {
                item.transform.parent = null;
            }
            foreach (RewardView item in mRewardViewsNoAnimation)
            {
                SimplePool.Despawn(item.gameObject);
            }
            mRewardViewsNoAnimation.Clear();

            mShowing = false;
            mRewards = new List<RewardInfo>();
        }

        bool canBack = true;
        internal override void Back()
        {
            if (!canBack) return;
            if (GameplayController.Instance != null)
            {
                GameplayController.Instance.ResumeGame();
            }

            base.Back();
        }

        private void OnEnable()
        {
            if (GameplayController.Instance != null)
            {
                GameplayController.Instance.PauseGame();
            }

            if (!mShowing && mRewards != null)
                StartCoroutine(IEShowRewards(mRewards));
        }

        public void SetRewards(List<RewardInfo> pRewards, RectTransform pSpawnPosition, AlignType myAlign = AlignType.Horizontal, System.Action OnShowDone = null)
        {
            this.myAlign = myAlign;
            mCoverColor = Color.clear;

            mRewards.AddRange(pRewards);
            mPreferedSpawnPos = pSpawnPosition;
            this.OnShowDone = OnShowDone;
            //if (!mShowing && gameObject.activeSelf)
            //    StartCoroutine(IEShowRewards(mRewards));
        }

        public void SetRewards(List<RewardInfo> pRewards, AlignType myAlign = AlignType.Horizontal)
        {
            this.myAlign = myAlign;
            mCoverColor = Color.clear;

            mRewards.AddRange(pRewards);
            mPreferedSpawnPos = null;

            //if (!mShowing && gameObject.activeSelf)
            //    StartCoroutine(IEShowRewards(mRewards));
        }

        private IEnumerator IEShowRewards(List<RewardInfo> pRewards)
        {
            //Check show summonNext button
            bool isSummon = false;
            if (SceneManager.GetActiveScene().name.Equals("Home") && MainPanel.instance.SummonGatePanel.isActiveAndEnabled == true
                && !MainPanel.instance.SummonGatePanel.isTut)
            {
                summonNextPopup.gameObject.SetActive(true);
                summonNextPopup.CallStart(MainPanel.instance.SummonGatePanel.TypeSummon);
                isSummon = true;
            }
            else
            {
                summonNextPopup.gameObject.SetActive(false);
                isSummon = false;
            }


            objTapToNext.SetActive(true);
            SummonGr.SetActive(false);
            Lock(true);

            yield return null;
            yield return null;

            mShowing = true;
            mAlignment = mHorizontalAlignment;
            switch (myAlign)
            {
                case AlignType.Horizontal:
                    mAlignment = mHorizontalAlignment;
                    break;
                case AlignType.Cicler:
                    mAlignment = mCiclerAlignment;
                    break;
                default:
                    mAlignment = mHorizontalAlignment;
                    break;
            }
            var count = pRewards.Count;
            var sizeDelta = mImgBg.rectTransform.sizeDelta;
            if (count <= 4) mImgBg.rectTransform.sizeDelta = new Vector2(sizeDelta.x, 248f);
            else if (count <= 8) mImgBg.rectTransform.sizeDelta = new Vector2(sizeDelta.x, 468f);
            else mImgBg.rectTransform.sizeDelta = new Vector2(sizeDelta.x, 690f);
            canBack = false;
            objTapToNext.SetActive(false);
            SummonGr.SetActive(false);
            if (myAlign == AlignType.Cicler)
            {
                canBack = false;
                objTapToNext.SetActive(false);
                SummonGr.SetActive(false);
                mImgBg.rectTransform.sizeDelta = new Vector2(sizeDelta.x, 1000f);
                //WARNING:HardCode:Only use for SummonHero
                //Sort: Bonus Item  move to Circle:
                RewardInfo _reward = null;
                for (int i = 0; i < count; i++)
                {
                    _reward = pRewards[i];
                    if (_reward.Type != IDs.REWARD_TYPE_UNLOCK_CHARACTER)
                    {
                        pRewards.RemoveAt(i); break;
                    }
                }
                if (_reward != null)
                    pRewards.Add(_reward);
                //End HardCode
            }

            for (int i = 0; i < count; i++)
            {
                var reward = pRewards[i];

                Vector2 pSpawnPosition = mAlignment.SpawnPos();
                if (mPreferedSpawnPos != null)
                    pSpawnPosition = mPreferedSpawnPos.position;

                RewardView obj = null;

                obj = SimplePool.Spawn(RewardViewPrefab, Vector3.zero, Quaternion.identity);
                obj.transform.parent = mAlignment.transform;
                mRewardViewsNoAnimation.Add(obj);
                //  Debug.Log(obj.name + " showing");
                obj.Init(reward);
                obj.SetActive(true);
                obj.transform.Reset();
                obj.transform.position = pSpawnPosition;
                Vector3 _lo = obj.transform.localPosition;
                _lo.z = 0;
                obj.transform.localPosition = _lo;
                float tweenTime = 0.1f * i;
                if (pSpawnPosition != (Vector2)mAlignment.SpawnPos())
                    tweenTime = 0.75f + 0.1f * i;


                //Check unlock character -> show

                if (reward.Type == IDs.REWARD_TYPE_UNLOCK_CHARACTER)
                {
                    //  Debug.Log("REWARD_TYPE_UNLOCK_CHARACTER");
                    var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(reward.Id);
                    //    Debug.Log(heroDefinition.rank);
                    if (heroDefinition.rank >= IDs.RANK_S || count == 1)
                    {
                        rewardHeroSRPopup.CallStart(heroDefinition);
                        yield return new WaitUntil(() => rewardHeroSRPopup.isActiveAndEnabled == false);
                    }
                }

                if (tweenTime == 0 && myAlign != AlignType.Cicler)
                {
                    PlayFX(reward, obj.transform);

                    //obj.PlayFlipWhoEffect();
                }
                else
                {
                    obj.transform.position = pSpawnPosition;
                    _lo = obj.transform.localPosition;
                    _lo.z = 0;
                    obj.transform.localPosition = _lo;
                    mAlignment.SetTweenTime(tweenTime);
                    obj.PlayGuestWhoEffect(true);
                    mAlignment.AlignByTweener(() =>
                    {
                        PlayFX(reward, obj.transform);
                        obj.PlayFlipWhoEffect();
                    });
                }

                if (!skipTweenTime)
                    yield return new WaitForSecondsRealtime(tweenTime);
                else
                    yield return new WaitForSecondsRealtime(0.1f);
            }

      

            mShowing = false;
            //foreach (RewardView item in mRewardViewsNoAnimation)
            //{
            //    // item.ReloadEffect();
            //    item.PlayEffectLoop();
            //}
            yield return new WaitForSecondsRealtime(0.5f);
            objTapToNext.SetActive(true);
            SummonGr.SetActive(isSummon);
            Lock(false);
            canBack = true;
        }
        protected override void AfterHiding()
        {
            base.AfterHiding();
            if (OnShowDone != null)
                OnShowDone();
        }
        private void PlayFX(RewardInfo r, Transform pTarget)
        {
            SoundManager.Instance.PlaySFX(IDs.SOUND_CLAIM);
            UIFXManager.instance.PlayStarFX(pTarget);
            SimpleLeanFX.instance.Bubble(pTarget, 0.5f);
        }


    }
}