using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using TMPro;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;
using Utilities.Service;
using EventDispatcher = Utilities.Common.EventDispatcher;
using UnityEngine.UI;
using FoodZombie.UI;

public class PremiumPackageView : MonoBehaviour
{
    public TextMeshProUGUI txtGem;
    public TextMeshProUGUI txtDailyGem;
    public TextMeshProUGUI txtVip;
    public TextMeshProUGUI txtCost;
    public SimpleTMPButton btnBuy;
    
    public PremiumPackageData premiumPackageData;

    private bool isSubscribed = false;
    private void Start()
    {
        btnBuy.onClick.AddListener(BtnBuy_Pressed);
    }

    private void Init()
    {
        var rewards = premiumPackageData.GetRewards();
        var count = rewards.Count;
        for (int i = 0; i < count; i++)
        {
            var reward = rewards[i];
            if (reward.Type == IDs.REWARD_TYPE_CURRENCY && reward.Type == IDs.CURRENCY_GEM)
            {
                txtGem.text = reward.Value + "";
            }
            else if (reward.Type == IDs.REWARD_TYPE_VIP)
            {
                txtVip.text = "+" + reward.Value + " VIP";
            }
        }
        
        rewards = premiumPackageData.GetDailyRewards();
        count = rewards.Count;
        for (int i = 0; i < count; i++)
        {
            var reward = rewards[i];
            if (reward.Type == IDs.REWARD_TYPE_CURRENCY && reward.Type == IDs.CURRENCY_GEM)
            {
                txtDailyGem.text = reward.Value + "";
            }
        }
        
        isSubscribed = PaymentHelper.IsSubscribed(premiumPackageData);
        if (isSubscribed)
        {
            if (premiumPackageData.ClaimedToday)
            {
                txtCost.text = "Claimed";
                btnBuy.SetEnable(false);
            }
            else
            {
                txtCost.text = "Claim";
                btnBuy.SetEnable(true);
            }
        }
        else
        {
            PaymentHelper.SetTextLocalizedPriceString(txtCost, premiumPackageData);
            btnBuy.SetEnable(true);
        }
    }

    public void Init(PremiumPackageData _premiumPackageData)
    {
        premiumPackageData = _premiumPackageData;
        Init();
    }
    
    private void BtnBuy_Pressed()
    {
        if (isSubscribed)
        {
            var rewards = premiumPackageData.Claim();
            LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_STORE);
            
            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.Subscrible();
            
            Init();
        }
        else
        {
            PaymentHelper.Purchase(premiumPackageData, PurchaseSuccess);
            Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X, premiumPackageData.LogName));
        }
    }

    void PurchaseSuccess(bool success)
    {
        if (success)
        {
            var rewards = premiumPackageData.Buy();
            LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_STORE);
            
            Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_PASS, premiumPackageData.LogName));
            PaymentHelper.LogPurchase(premiumPackageData);
        }
        else
        {
            Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_FAIL, premiumPackageData.LogName));
            MainPanel.instance.ShowWarningPopup(ConstantsString.INAPP_FAILD);
        }
    }
}
