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

public class MarketItemView : MonoBehaviour
{
    public RewardView rewardView;
    public TextMeshProUGUI txtBuyCount;
    public TextMeshProUGUI txtCost;
    public Image imgCost;
    private MarketItemData marketItemData;
    private UnityAction refreshMatket;

    public SimpleTMPButton btnBuy;
    
    private void Start()
    {
        btnBuy.onClick.AddListener(BtnBuy_Pressed);
    }

    public void Init(MarketItemData _marketItemData, UnityAction _refreshMatket)
    {
        marketItemData = _marketItemData;
        refreshMatket = _refreshMatket;
        
        rewardView.Init(marketItemData.GetReward());
        txtBuyCount.text = (marketItemData.BuyLimit - marketItemData.BuyCount) + "/" + marketItemData.BuyLimit;

        if (marketItemData.ByCoin)
        {
            txtCost.text = BigNumberAlpha.Create(marketItemData.CoinCost).GetKKKString();
            imgCost.sprite = AssetsCollection.instance.GetCoinIcon();
        }
        else
        {
            txtCost.text = BigNumberAlpha.Create(marketItemData.GemCost).GetKKKString();
            imgCost.sprite = AssetsCollection.instance.GetGemIcon();
        }

        if (marketItemData.CanBuy)
        {
            btnBuy.SetEnable(true);
        }
        else
        {
            btnBuy.SetEnable(false);
        }
    }
    
    private void BtnBuy_Pressed()
    {
        if (marketItemData.ByCoin)
        {
            int coin = marketItemData.CoinCost;
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_COIN, coin))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_9));
                //MainPanel.instance.ShowWarningPopup("Not enough coin");
                return;
            }
            
            currenciesGroup.Pay(IDs.CURRENCY_COIN, coin, TrackingConstants.VALUE_STORE);
        }
        else
        {
            int gem = marketItemData.GemCost;
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, gem))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
                //MainPanel.instance.ShowWarningPopup("Not enough gem");
                GameData.Instance.UserGroup.MissGemCount++;
                return;
            }
            
            currenciesGroup.Pay(IDs.CURRENCY_GEM, gem, TrackingConstants.VALUE_STORE);
        }
        
        var reward = marketItemData.Buy();
        LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_STORE);

        refreshMatket();
    }
}
