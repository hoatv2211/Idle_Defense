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
    public class FastCollectPanel : MyGamesBasePanel
    {
        public CurrencyView ticketView;
        public SimpleTMPButton btnUse;
        public GameObject imgTicket;
        public TextMeshProUGUI txtTimeNextTicket;

        private double deltaTime;

        private void Start()
        {
            btnUse.onClick.AddListener(BtnUse_Pressed);
        }
        
        private void FixedUpdate()
        {
            txtTimeNextTicket.text = "Free reset: <color=#1EEE40>" + TimeHelper.FormatHHMMss(deltaTime, true) + "</color>";
            deltaTime -= Time.fixedDeltaTime;
            
            if (deltaTime <= 0f)
            {
                Init();
            }
        }
        
        private void CheckTime()
        { 
            //tính time sang ngày tiếp theo
            //DateTime now = DateTime.Now;
            DateTime now = UnbiasedTime.Instance.Now(); //Fix cheat time

            //7h sáng mới reset day
            DateTime tomorrow = now.AddDays(1).Date.Add(new TimeSpan(0, 7, 0, 0));
            deltaTime = (tomorrow - now).TotalSeconds + 1f; //thêm 1s delay cho chắc lúc f5 sang ngày
        }

        internal override void Init()
        {
            GameData.Instance.GameConfigGroup.CheckNewDay();

            if (GameData.Instance.MissionsGroup.FreeTicket)
            {
                btnUse.labelTMP.text = /*"FREE";*/ Localization.Get(Localization.ID.COMMON_BUTTON_78);
                imgTicket.SetActive(false);
            }
            else
            {
                btnUse.labelTMP.text = /*"Use";*/ Localization.Get(Localization.ID.COMMON_BUTTON_81);
                imgTicket.SetActive(true);
            }
            
            ticketView.Init(IDs.CURRENCY_TICKET);
            CheckTime();
        }
        
        private void BtnUse_Pressed()
        {
            if (!GameData.Instance.MissionsGroup.FreeTicket)
            {
                if (GameData.Instance.CurrenciesGroup.CanPay(IDs.CURRENCY_TICKET, 1))
                {
                    GameData.Instance.CurrenciesGroup.Pay(IDs.CURRENCY_TICKET, 1);
                    ClaimReward();
                }
                else
                {
                    MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_25));
                   // MainPanel.instance.ShowWarningPopup("Not enough ticket");
                    // if (!AdsHelper.__IsVideoRewardedAdReady())
                    // { 
                    //     MainPanel.instance.ShowWarningPopup("Ads not available");
                    // }
                    // else
                    // {
                    //     AdsHelper.__ShowVideoRewardedAd(TrackingConstants.ADS_REWARD_FAST_TICKET, OnRewardedAdCompleted);
                    // }
                }
            }
            else
            {
                GameData.Instance.MissionsGroup.ClaimFreeTicket();
                ClaimReward();
            }
        }
        
        // private void OnRewardedAdCompleted(bool isCompleted)
        // {
        //     if (isCompleted)
        //     {
        //         ClaimReward();
        //     }
        // }

        private void ClaimReward()
        {
            Back();
            var rewardInfos = GameData.Instance.MissionsGroup.GetBuffTimeRewards();
            LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_FAST_COLLECT);

            Init();
        }
    }
}
