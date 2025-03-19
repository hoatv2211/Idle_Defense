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

public class FlashSaleView : MonoBehaviour
{
    public RewardView rewardView;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtCost;
    public TextMeshProUGUI txtCostSale;
    public Image imgCost;
    public Image imgCostSale;
    public TextMeshProUGUI txtPercentSale;
    public SimpleTMPButton btnBuy;
    
    private FlashSaleData flashSaleData;
    private UnityAction refreshFlashSale;
    
    private void Start()
    {
        btnBuy.onClick.AddListener(BtnBuy_Pressed);
    }

    public void Init(FlashSaleData _flashSaleData, UnityAction _refreshFlashSale)
    {
        flashSaleData = _flashSaleData;
        refreshFlashSale = _refreshFlashSale;
        
        rewardView.Init(flashSaleData.GetReward());

        //txtName.text = flashSaleData.Name;
        txtName.text = flashSaleData.NameLocal;
        if (flashSaleData.ByCoin)
        {
            //vì nhiều đoạn GD lười ko nhập giá sale nên phải kiểm tra
            if (flashSaleData.CoinCostSale > 0)
            {
                txtCost.transform.parent.SetActive(true);
                txtCost.text = BigNumberAlpha.Create(flashSaleData.CoinCost).GetKKKString();
                txtCostSale.text = BigNumberAlpha.Create(flashSaleData.CoinCostSale).GetKKKString();
                txtPercentSale.transform.parent.SetActive(true);
                txtPercentSale.text = (- (1f - (float) flashSaleData.CoinCostSale / (float) flashSaleData.CoinCost) * 100f).ToString("0") + "%";
            }
            else
            {
                txtCost.transform.parent.SetActive(false);
                txtCostSale.text = BigNumberAlpha.Create(flashSaleData.CoinCost).GetKKKString();
                txtPercentSale.transform.parent.SetActive(false);
            }
            imgCost.sprite = AssetsCollection.instance.GetCoinIcon();
            imgCostSale.sprite = AssetsCollection.instance.GetCoinIcon();
        }
        else
        {
            if (flashSaleData.GemCostSale > 0)
            {
                txtCost.transform.parent.SetActive(true);
                txtCost.text = BigNumberAlpha.Create(flashSaleData.GemCost).GetKKKString();
                txtCostSale.text = BigNumberAlpha.Create(flashSaleData.GemCostSale).GetKKKString();
                txtPercentSale.transform.parent.SetActive(true);
                txtPercentSale.text = (- (1f - (float) flashSaleData.GemCostSale / (float) flashSaleData.GemCost) * 100f).ToString("0") + "%";
            }
            else
            {
                txtCost.transform.parent.SetActive(false);
                txtCostSale.text = BigNumberAlpha.Create(flashSaleData.GemCost).GetKKKString();
                txtPercentSale.transform.parent.SetActive(false);
            }
            imgCost.sprite = AssetsCollection.instance.GetGemIcon();
            imgCostSale.sprite = AssetsCollection.instance.GetGemIcon();
        }

        if (flashSaleData.Bought)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
    
    private void BtnBuy_Pressed()
    {
        if (flashSaleData.ByCoin)
        {
            //vì nhiều đoạn GD lười ko nhập giá sale nên phải kiểm tra
            int coin;
            if(flashSaleData.CoinCostSale > 0) coin = flashSaleData.CoinCostSale;
            else coin = flashSaleData.CoinCost;
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
            int gem;
            if(flashSaleData.GemCostSale > 0) gem = flashSaleData.GemCostSale;
            else gem = flashSaleData.GemCost;
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
        
        var reward = flashSaleData.Buy();
        LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_STORE);

        refreshFlashSale();
    }
}
