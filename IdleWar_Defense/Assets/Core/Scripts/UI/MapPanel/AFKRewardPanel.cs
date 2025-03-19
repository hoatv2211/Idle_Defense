using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Inspector;
using Utilities.Components;
using System;
using Spine.Unity;
using Utilities.Common;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace FoodZombie.UI
{
    public class AFKRewardPanel : MyGamesBasePanel
    {
        private const float X_COLLECT = 1.5f;

        public TextMeshProUGUI txtTimeAFK;
        public TextMeshProUGUI txtCoinAFK;
        public TextMeshProUGUI txtHeroEXPAFK;
        public TextMeshProUGUI txtUserEXPAFK;
        public TextMeshProUGUI txtGemAFK;

        public Transform transformPool;
        // public Transform scrollPool;
        [SerializeField, Tooltip("Buildin Pool")] private List<RewardView> rewardViewsPool;
        
        public SimpleTMPButton btnCollect;
        public SimpleTMPButton btnXCoin;

        // public SimpleTMPButton btnNextScroll;
        // public SimpleTMPButton btnPreScroll;
        
        public List<RewardInfo> rewardsInfos;
        
        private void Start()
        {
            btnCollect.onClick.AddListener(BtnCollect_Pressed);
            btnXCoin.onClick.AddListener(BtnXCoin_Pressed);
            // btnNextScroll.onClick.AddListener(BtnNextScroll_Pressed);
            // btnPreScroll.onClick.AddListener(BtnPreScroll_Pressed);
        }

        private void Update()
        {
            //txtTimeAFK.text = "Insta idle duration: <color=#1EEE40>" + GameData.Instance.MissionsGroup.GetTimeAFK() + "</color>";
            txtTimeAFK.text = "<color=#1EEE40>" + GameData.Instance.MissionsGroup.GetTimeAFK() + "</color>";
        }
        
        // private void Update()
        // {
        //     if (scroll.horizontalNormalizedPosition >= 1f)
        //     {
        //         btnPreScroll.SetEnable(true);
        //         btnNextScroll.SetEnable(false);
        //     }
        //     else if (scroll.horizontalNormalizedPosition <= 0f)
        //     {
        //         btnPreScroll.SetEnable(false);
        //         btnNextScroll.SetEnable(true);
        //     }
        //     else
        //     {
        //         btnPreScroll.SetEnable(true);
        //         btnNextScroll.SetEnable(true);
        //     }
        // }

        public void Init(List<RewardInfo> _rewardsInfos)
        {
            var lastWinMissionData = GameData.Instance.MissionsGroup.GetLastWinMissionData();
            
            txtCoinAFK.text = BigNumberAlpha.Create(lastWinMissionData.CoinAFK).GetKKKString() + "/m";
            txtHeroEXPAFK.text = BigNumberAlpha.Create(lastWinMissionData.HeroEXPAFK).GetKKKString() + "/m";
            txtUserEXPAFK.text = BigNumberAlpha.Create(lastWinMissionData.UserEXPAFK).GetKKKString() + "/m";
            txtGemAFK.text = BigNumberAlpha.Create(lastWinMissionData.GemAFK).GetKKKString() + "/h";
            
            rewardViewsPool.Free();

            rewardsInfos = _rewardsInfos;
            int count = rewardsInfos.Count;
            for (int i = 0; i < count; i++)
            {
                var item = rewardsInfos[i];
                // Transform pool;
                // if (count > 5)
                // {
                //     pool = scrollPool;
                //     scroll.SetActive(true);
                // }
                // else
                // {
                //     pool = transformPool;
                //     scroll.SetActive(false);
                // }
                // var rewardView = rewardViewsPool.Obtain(pool);
                var rewardView = rewardViewsPool.Obtain(transformPool);
                rewardView.Init(item);
                rewardView.SetActive(true);
            }
        }
        
        private void BtnCollect_Pressed()
        {
            Back();
            LogicAPI.ClaimRewards(rewardsInfos, TrackingConstants.VALUE_AUTO_BATTLE);
            GameData.Instance.MissionsGroup.ClaimAFKRewards();
        }

        private void BtnXCoin_Pressed()
        {
            if (!AdsHelper.__IsVideoRewardedAdReady())
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_1));
                //MainPanel.instance.ShowWarningPopup("Ads not available");
            }
            else
            {
                AdsHelper.__ShowVideoRewardedAd(TrackingConstants.ADS_REWARD_AUTO_BATTLE_X, OnRewardedAdCompleted);
            }
        }

        private void OnRewardedAdCompleted(bool isCompleted)
        {
            if (isCompleted)
            {
                ClaimReward();
            }
        }

        private void ClaimReward()
        {
            var count = rewardsInfos.Count;
            for (int i = 0; i < count; i++)
            {
                var rewardInfo = rewardsInfos[i];
                if ((rewardInfo.Type == IDs.REWARD_TYPE_CURRENCY && (rewardInfo.Id == IDs.CURRENCY_COIN ||rewardInfo.Id == IDs.CURRENCY_EXP_HERO))
                    || rewardInfo.Type == IDs.REWARD_TYPE_EXP_USER)
                {
                    rewardsInfos[i] = new RewardInfo(rewardInfo.Type, rewardInfo.Id, (int) (rewardInfo.Value * X_COLLECT));
                }
            }
            
            Back();
            LogicAPI.ClaimRewards(rewardsInfos, TrackingConstants.VALUE_AUTO_BATTLE);
            GameData.Instance.MissionsGroup.ClaimAFKRewards();
        }

        // private void BtnNextScroll_Pressed()
        // {
        //     scroll.DOHorizontalNormalizedPos(1f, 0.5f);
        // }
        //
        // private void BtnPreScroll_Pressed()
        // {
        //     scroll.DOHorizontalNormalizedPos(0f, 0.5f);
        // }
    }
}
