using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Utilities.Service;

namespace FoodZombie.UI
{
    public class DailyGiftPanel : MonoBehaviour
    {
        EventAllPanel evenAllPanel;
        public CurrencyView coinView => MainPanel.instance.MainMenuPanel.coinView;
        public CurrencyView gemView => MainPanel.instance.MainMenuPanel.gemView;
        public CurrencyView expHeroView => MainPanel.instance.MainMenuPanel.expHeroView;


        [SerializeField, Tooltip("Buildin Pool")] private List<DailyGiftView> dailyGiftViewsPool;

        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        private PackageData packageData;

        public void Init(EventAllPanel evenAllPanel)
        {
            this.evenAllPanel = evenAllPanel;
            GameData.Instance.GameConfigGroup.CheckNewDay();

            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);

            RefreshDailyLoginViews();
        }

        private void RefreshDailyLoginViews()
        {
            //list
            dailyGiftViewsPool.Free();
            var dailyGifts = GameData.Instance.DailyQuestsGroup.DailyGifts;
            var count = dailyGifts.Count;
            for (int i = 0; i < count; i++)
            {
                var dailyGiftView = dailyGiftViewsPool[i];
                dailyGiftView.Init(dailyGifts[i], i, RefreshDailyLoginViews);
                dailyGiftView.SetActive(true);
               
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
