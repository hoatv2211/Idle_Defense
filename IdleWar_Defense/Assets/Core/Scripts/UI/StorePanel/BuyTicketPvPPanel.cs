using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;

public class BuyTicketPvPPanel : MonoBehaviour
{
    public CurrencyView ticketView;

    public TextMeshProUGUI txtGemCost;
    public TextMeshProUGUI txtTicketGet;
    public SimpleTMPButton btnBuy;

    void Start()
    {
        btnBuy.SetUpEvent(Action_BtnBuy);
    }

    internal void Init()
    {
        ticketView.Init(IDs.CURRENCY_TICKET_PVP);

        var gem = Constants.GEM_TO_TICKET_PVP_SHOP;
        var ticket = Constants.TICKET_BY_GEM_SHOP;

        txtGemCost.text = gem + "";
        txtTicketGet.text = ticket + "";
        var currenciesGroup = GameData.Instance.CurrenciesGroup;
        if (currenciesGroup.GetGem() >= gem) txtGemCost.color = Color.green;
        else txtGemCost.color = Color.red;
    }

    public void Action_BtnBuy()
    {
        //if (!GameData.Instance.UserGroup.CanBuyFastLoot())
        //{
        //    MainPanel.instance.ShowWarningPopup("You run out of daily purchases, upgrade VIP level to get more purchases");
        //    return;
        //}

        var gem = Constants.GEM_TO_TICKET_PVP_SHOP;
        var ticket = Constants.TICKET_BY_GEM_SHOP;
        var currenciesGroup = GameData.Instance.CurrenciesGroup;
        if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, gem))
        {
            MainPanel.instance.ShowWarningPopup("Not enough gem");
            GameData.Instance.UserGroup.MissGemCount++;
            return;
        }

        currenciesGroup.Pay(IDs.CURRENCY_GEM, gem, TrackingConstants.VALUE_BUY_TICKET);
        LogicAPI.ClaimReward(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_TICKET_PVP, ticket), TrackingConstants.VALUE_BUY_TICKET);

        GameData.Instance.UserGroup.BuyFastLoot();

        Init();
    }

}
