using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class DailyLoginView : MonoBehaviour
{
    public Text txtDescription;
    public GameObject imgDayShadown;

    public GameObject imgFreeShadown;
    public RewardView freeRewardView;
    public SimpleTMPButton btnClaimFree;
    public Image imgFreeComplete;
    public Image imgFreeNoti;
    public GameObject objClaim;

    public GameObject[] imgVipShadown;
    public GameObject[] imgLock;
    public RewardView[] vipRewardViews;
    public SimpleTMPButton[] btnClaimVips;
    public Image[] imgVipCompletes;
    public Image[] imgVipNotis;
    public GameObject[] objVipClaim;

    public Image imgFreeBound;
    public Image[] imgVipBound;
    public Sprite sprNonSelect, sprSelect;

    private DailyLoginData dailyLoginData;
    private int dayCount;

    private void Start()
    {
        btnClaimFree.onClick.AddListener(BtnClaimFree_Pressed);
        var count = btnClaimVips.Length;
        for (int i = 0; i < count; i++)
        {
            btnClaimVips[i].onClick.AddListener(BtnClaimVip_Pressed);
        }
    }

    public void Init(DailyLoginData _dailyLoginData)
    {
        dailyLoginData = _dailyLoginData;

        freeRewardView.Init(dailyLoginData.GetFreeReward());
        var vipRewards = dailyLoginData.GetVipRewards();
        var count = vipRewards.Count;
        var countVipRewardViews = vipRewardViews.Length;
        for (int i = 0; i < count; i++)
        {
            var vipRewardView = vipRewardViews[i];
            vipRewardView.Init(vipRewards[i]);
            vipRewardView.SetActive(true);
            imgVipBound[i].gameObject.SetActive(true);
        }

        for (int i = count; i < countVipRewardViews; i++)
        {
            vipRewardViews[i].SetActive(false);
            imgVipBound[i].gameObject.SetActive(false);
        }

        Init();
    }

    private void Init()
    {
        dayCount = GameData.Instance.DailyQuestsGroup.DayCount;
        txtDescription.text = "DAY " + dailyLoginData.Day;

        //highlight ngày hôm nay lên
        imgDayShadown.SetActive(dailyLoginData.Day != dayCount);


        var moreDay = dailyLoginData.Day > dayCount;

        //free
        var canClaim = dailyLoginData.CanClaim; //đã đến ngày claim chưa

        var claimedFree = dailyLoginData.ClaimedFree;
        imgFreeComplete.SetActive(claimedFree);
        btnClaimFree.interactable = !claimedFree;
        imgFreeShadown.SetActive(moreDay || claimedFree || !canClaim);
        objClaim.SetActive(canClaim && !claimedFree);
        imgFreeBound.sprite = canClaim && !claimedFree ? sprSelect : sprNonSelect;
        // imgFreeNoti.SetActive(canClaim && !claimedFree);

        //vip
        var claimedVip = dailyLoginData.ClaimedVip;
        var count = imgVipCompletes.Length;
        for (int i = 0; i < count; i++)
        {
            imgVipCompletes[i].SetActive(claimedVip);
        }
        count = btnClaimVips.Length;
        for (int i = 0; i < count; i++)
        {
            btnClaimVips[i].interactable = !claimedVip;
        }
        var isVip = /*GameData.Instance.UserGroup.VipLevel > 0 ||*/ !GameData.Instance.StoreGroup.PremiumPass.CanBuy;
        foreach (var item in imgLock)
        {
            item.SetActive(!isVip);
        }


        foreach (var item in imgVipShadown)
        {
            item.SetActive(moreDay || claimedVip || !canClaim);
        }

        // count = imgVipNotis.Length;
        // for (int i = 0; i < count; i++)
        // {
        //     imgVipNotis[i].SetActive(!claimedVip && canClaim && isVip);
        // }
        count = objVipClaim.Length;
        for (int i = 0; i < count; i++)
        {
            objVipClaim[i].SetActive(!claimedVip && canClaim && isVip);
            imgVipBound[i].sprite = !claimedVip && canClaim && isVip ? sprSelect : sprNonSelect;
        }
    }

    private void BtnClaimFree_Pressed()
    {
        var day = dailyLoginData.Day;
        if (dayCount < day)
        {
            //MainPanel.instance.ShowWarningPopup("Wait to day " + day + " to receive the reward");
            MainPanel.instance.ShowWarningPopup(string.Format(Localization.Get(Localization.ID.MESSAGE_10),day));
            return;
        }

        var reward = dailyLoginData.ClaimFreeReward();
        LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_DAILY_LOGIN_FREE);

        //nếu đã là vip thì claim tất
        if (/*GameData.Instance.UserGroup.VipLevel > 0 ||*/ !GameData.Instance.StoreGroup.PremiumPass.CanBuy)
        {
            var rewards = dailyLoginData.ClaimVipRewards();
            LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_DAILY_LOGIN_VIP);
        }

        Init();
    }

    private void BtnClaimVip_Pressed()
    {
        //nếu đã là vip thì claim tất
        if (/*GameData.Instance.UserGroup.VipLevel > 0 ||*/ !GameData.Instance.StoreGroup.PremiumPass.CanBuy)
        {
            var day = dailyLoginData.Day;
            if (dayCount < day)
            {
                //MainPanel.instance.ShowWarningPopup("Wait to day " + day + " to receive the reward");
                MainPanel.instance.ShowWarningPopup(string.Format(Localization.Get(Localization.ID.MESSAGE_10), day));
                return;
            }

            if (!dailyLoginData.ClaimedFree)
            {
                var reward = dailyLoginData.ClaimFreeReward();
                LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_DAILY_LOGIN_FREE);
            }

            var rewards = dailyLoginData.ClaimVipRewards();
            LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_DAILY_LOGIN_VIP);
        }
        else
        {
            MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_11));
            //MainPanel.instance.ShowWarningPopup("Get VIP or unlock Premium Pass");
        }

        Init();
    }
}
