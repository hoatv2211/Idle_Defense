using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class HeroFragmentDetailPanel : MyGamesBasePanel
    {
        public HeroFragmentView heroFragmentView;
        public TextMeshProUGUI txtName;
        public TextMeshProUGUI txtRank;
        public TextMeshProUGUI txtInfo;
        public SimpleTMPButton btnCraft;

        private HeroFragmentItemData heroFragmentItemData;

        private UnityAction refreshAction;
        
        private void Start()
        {
            btnCraft.onClick.AddListener(BtnCraft_Pressed);
        }

        public void Init(HeroFragmentItemData _heroFragmentItemData, UnityAction _refreshAction)
        {
            heroFragmentItemData = _heroFragmentItemData;
            refreshAction = _refreshAction;
            
            heroFragmentView.Init(heroFragmentItemData);
            txtName.text = heroFragmentItemData.GetName();
            txtRank.text = "Rank: " + heroFragmentItemData.GetRankName();
            var stockNumber = heroFragmentItemData.StockNumber;
            var requireFragment = heroFragmentItemData.baseData.requireFragment;
            txtInfo.text = Config.CurrencyAndCostToString(stockNumber, requireFragment);

            if (stockNumber >= requireFragment)
            {
                btnCraft.SetEnable(true);
            }
            else
            {
                btnCraft.SetEnable(false);
            }
        }

        private void BtnCraft_Pressed()
        {
            Back();
            
            var rewardInfos = heroFragmentItemData.CraftHero();
            LogicAPI.ClaimRewards(rewardInfos);

            if (refreshAction != null) refreshAction();
        }
    }
}
