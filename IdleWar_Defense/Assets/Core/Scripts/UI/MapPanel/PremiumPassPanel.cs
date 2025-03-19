using FoodZombie;
using FoodZombie.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Service;

public class PremiumPassPanel : MyGamesBasePanel
{
    public TextMeshProUGUI txtPremiumPassRewardValue;
    //  public TextMeshProUGUI txtTimeNextReward;
    public TextMeshProUGUI txtPremiumPassCost;
    public TextMeshProUGUI txtPremiumVipExp;
    public SimpleTMPButton btnBuy;
    private PackageData packageData;


    private void Start()
    {
        btnBuy.onClick.AddListener(BtnBuy_Pressed);
    }
    internal override void Init()
    {
        base.Init();
        InitView();
    }
    public void InitView()
    {
        Debug.Log("Init Premium Pass Panel");
        packageData = GameData.Instance.StoreGroup.PremiumPass;
        var rewards = packageData.GetRewards();
        txtPremiumPassRewardValue.text = rewards[0].Value + "";

        // txtPremiumPassName.text = packageData.Name;
        // var isVip = GameData.Instance.UserGroup.VipLevel > 0 || packageData.Bought;
        txtPremiumVipExp.text = packageData.GetVipExp() + "";
        if (packageData.CanBuy)
        {
            var usd = packageData.Usd;
            txtPremiumPassCost.text = usd;
            //đề phòng crash, éo hiểu
            //load trước đề phòng
            if (txtPremiumPassCost != null) txtPremiumPassCost.text = usd;
            PaymentHelper.SetTextLocalizedPriceString(txtPremiumPassCost, packageData);
            btnBuy.SetActive(true);
            //  imgVipUnlocked.SetActive(false);
        }
        else
        {
            btnBuy.SetActive(false);
            //  imgVipUnlocked.SetActive(true);
        }
    }
    Action OnPurchaseSuccessEvent;
    public void Init(Action OnPurchaseSuccess)
    {
        this.OnPurchaseSuccessEvent = OnPurchaseSuccess;
        //Init();
        //       Show();
    }

    private void BtnBuy_Pressed()
    {
        PaymentHelper.Purchase(packageData, PurchaseSuccess);
        Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X, packageData.LogName));
    }

    void PurchaseSuccess(bool success)
    {
        if (success)
        {
            var rewards = packageData.Buy();
            LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_PREMIUM_PASS);
            if (this.OnPurchaseSuccessEvent != null)
                this.OnPurchaseSuccessEvent();
            Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_PASS, packageData.LogName));
            PaymentHelper.LogPurchase(packageData);
            Back();
        }
        else
        {
            Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_FAIL, packageData.LogName));
            MainPanel.instance.ShowWarningPopup(ConstantsString.INAPP_FAILD);
        }
       
    }
}
