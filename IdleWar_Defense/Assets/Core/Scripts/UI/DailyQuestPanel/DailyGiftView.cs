using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class DailyGiftView : MonoBehaviour
{
    public RewardView rewardView;
    public Image imgBound;
    public Sprite sprBoundNonSelect, sprBoundSelect;
    public TextMeshProUGUI txtRewardName;
    public GameObject imgComplete;
    // public GameObject imgCurrentGift;
    public SimpleTMPButton btnClaim;
    public GameObject imgShadown;

    private DailyGift dailyGift;
    private int index = -1;

    private UnityAction refreshDailyGift;

    public void Start()
    {
        btnClaim.onClick.AddListener(BtnClaim_Pressed);
    }

    public void Init(DailyGift _dailyGift, int _index, UnityAction _refreshDailyGift)
    {
        dailyGift = _dailyGift;
        index = _index;
        refreshDailyGift = _refreshDailyGift;
        rewardView.Init(dailyGift.GetReward());

        Init();
    }

    private void Init()
    {
        var dailyGiftCountInDay = GameData.Instance.DailyQuestsGroup.DailyGiftCountInDay;

        txtRewardName.text = dailyGift.GetReward().GetName();
        imgComplete.SetActive(index < dailyGiftCountInDay);
        var current = index == dailyGiftCountInDay;
        //  imgCurrentGift.SetActive(current);
        btnClaim.SetActive(current);
        imgShadown.SetActive(!current);
        imgBound.sprite = current ? sprBoundSelect : sprBoundNonSelect;
    }

    private void BtnClaim_Pressed()
    {
        if (!AdsHelper.__IsVideoRewardedAdReady())
        {
            MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_1));
            //MainPanel.instance.ShowWarningPopup("Ads not available");
        }
        else
        {
            AdsHelper.__ShowVideoRewardedAd(TrackingConstants.ADS_REWARD_DAILY_GIFT, OnRewardedAdCompleted);
        }
    }

    private void OnRewardedAdCompleted(bool isCompleted)
    {
        if (isCompleted)
        {
            GameData.Instance.DailyQuestsGroup.DailyGiftClaim();
            LogicAPI.ClaimReward(dailyGift.GetReward(), TrackingConstants.VALUE_DAILY_GIFT);

            refreshDailyGift();
        }
    }
}
