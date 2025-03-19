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
using Utilities.Inspector;

namespace FoodZombie.UI
{
    public class BuyTicketPanel : MyGamesBasePanel
    {
        public CurrencyView ticketView;
        
        public TextMeshProUGUI txtGemCost;
        public TextMeshProUGUI txtTicketGet;
        public SimpleTMPButton btnBuy;
        
        void Start()
        {
            btnBuy.onClick.AddListener(BtnBuy_Pressed);
        }

        internal override void Init()
        {
            ticketView.Init(IDs.CURRENCY_TICKET);
            
            var gem = Constants.GEM_TO_TICKET_SHOP;
            var ticket = Constants.TICKET_BY_GEM_SHOP;

            txtGemCost.text = gem + "";
            txtTicketGet.text = ticket + "";
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if(currenciesGroup.GetGem() >= gem) txtGemCost.color = Color.green;
            else txtGemCost.color = Color.red;
        }

        public void BtnBuy_Pressed()
        {
            if (!GameData.Instance.UserGroup.CanBuyFastLoot())
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_32));
                //MainPanel.instance.ShowWarningPopup("You run out of daily purchases, upgrade VIP level to get more purchases");
                return;
            }
            
            var gem = Constants.GEM_TO_TICKET_SHOP;
            var ticket = Constants.TICKET_BY_GEM_SHOP;
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, gem))
            {
                MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
                //MainPanel.instance.ShowWarningPopup("Not enough gem");
                GameData.Instance.UserGroup.MissGemCount++;
                return;
            }
            
            currenciesGroup.Pay(IDs.CURRENCY_GEM, gem, TrackingConstants.VALUE_BUY_TICKET);
            LogicAPI.ClaimReward(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_TICKET, ticket), TrackingConstants.VALUE_BUY_TICKET);
            
            //Daily Quest và Achievement
            // GameData.Instance.DailyQuestsGroup.BuyCoin();
            GameData.Instance.UserGroup.BuyFastLoot();

            Init();
        }
    }
}
