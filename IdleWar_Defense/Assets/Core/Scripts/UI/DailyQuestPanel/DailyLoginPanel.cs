using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using Facebook.Unity;
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
    public class DailyLoginPanel : MonoBehaviour
    {
        EventAllPanel evenAllPanel;
        CurrencyView coinView => MainPanel.instance.MainMenuPanel.coinView;
        CurrencyView gemView => MainPanel.instance.MainMenuPanel.gemView;
        CurrencyView expHeroView => MainPanel.instance.MainMenuPanel.expHeroView;

        public ScrollRect scroll;
        public RectTransform rectDayLine;
        public Transform transformPool;
        PremiumPassPanel PassPanel => MainPanel.instance.PremiumPassPanel;
        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        private List<DailyLoginView> dailyLoginViewsPool;

        // public TextMeshProUGUI txtPremiumPassName;
        public TextMeshProUGUI txtPremiumPassCost;
        public TextMeshProUGUI txtPremiumVipExp;

        internal void Show()
        {
            this.gameObject.SetActive(true);
        }

        public TextMeshProUGUI txtPremiumPassRewardValue;
        public TextMeshProUGUI txtTimeNextReward;
        public SimpleTMPButton btnBuy;
        public GameObject imgVipUnlocked;

        private PackageData packageData;
        private double deltaTime;

        private void Start()
        {
            btnBuy.onClick.AddListener(BtnBuy_Pressed);
        }

        public void Init(EventAllPanel evenAllPanel)
        {
            this.evenAllPanel = evenAllPanel;
            GameData.Instance.GameConfigGroup.CheckNewDay();

            coinView.Init(IDs.CURRENCY_COIN);
            gemView.Init(IDs.CURRENCY_GEM);
            expHeroView.Init(IDs.CURRENCY_EXP_HERO);

            InitPremiumPass();
            InitDailyLoginViews();

            CheckTime();
        }

        private void FixedUpdate()
        {
            if (txtTimeNextReward != null)
                txtTimeNextReward.text = "" + TimeHelper.FormatHHMMss(deltaTime, true);
            deltaTime -= Time.fixedDeltaTime;

            if (deltaTime <= 0f)
            {
                Init(this.evenAllPanel);
            }
        }

        private void CheckTime()
        {
            //tính time sang ngày tiếp theo
            //DateTime now = DateTime.Now;
            DateTime now = UnbiasedTime.Instance.Now(); //Fix cheat time

            DateTime tomorrow = now.AddDays(1).Date.Add(new TimeSpan(0, 7, 0, 0));
            deltaTime = (tomorrow - now).TotalSeconds + 1f; //thêm 1s delay cho chắc lúc f5 sang ngày
        }

        private void InitPremiumPass()
        {
            PassPanel.InitView();
            //  return;
            packageData = GameData.Instance.StoreGroup.PremiumPass;
            var rewards = packageData.GetRewards();
            //  txtPremiumPassRewardValue.text = rewards[0].Value + "";

            // txtPremiumPassName.text = packageData.Name;
            // var isVip = GameData.Instance.UserGroup.VipLevel > 0 || packageData.Bought;
            //  txtPremiumVipExp.text = packageData.GetVipExp() + "";
            if (packageData.CanBuy)
            {
                //  var usd = packageData.Usd;
                //  txtPremiumPassCost.text = usd;
                //đề phòng crash, éo hiểu
                //load trước đề phòng
                //  if (txtPremiumPassCost != null) txtPremiumPassCost.text = usd;
                // PaymentHelper.SetTextLocalizedPriceString(txtPremiumPassCost, packageData);
                btnBuy.SetActive(true);
                imgVipUnlocked.SetActive(false);
            }
            else
            {
                btnBuy.SetActive(false);
                imgVipUnlocked.SetActive(true);
            }
        }

        private void InitDailyLoginViews()
        {
            if (dailyLoginViewsPool == null || dailyLoginViewsPool.Count < 0)
            {
                dailyLoginViewsPool = new List<DailyLoginView>();
                foreach (Transform item in transformPool)
                {
                    dailyLoginViewsPool.Add(item.GetComponent<DailyLoginView>());
                }
            }
            //list
            dailyLoginViewsPool.Free();
            var dailyLoginDatas = GameData.Instance.DailyQuestsGroup.GetAllDailyLoginDatas();
            var count = dailyLoginDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var dailyLoginView = dailyLoginViewsPool.Obtain(transformPool);
                dailyLoginView.Init(dailyLoginDatas[i]);
                dailyLoginView.SetActive(true);
            }

            var dayCount = GameData.Instance.DailyQuestsGroup.DayCount - 1;
            if (dayCount > count) dayCount = count;
            //highlight ngày hôm nay lên
            scroll.DOVerticalNormalizedPos(1f - ((float)dayCount / (float)count), 0f);
            rectDayLine.sizeDelta = new Vector2(rectDayLine.sizeDelta.x, 50 + dayCount * 220);
        }

        private void BtnBuy_Pressed()
        {
            MainPanel.instance.ShowPremiumPassPanel();
            PassPanel.Init(() =>
            {
                InitPremiumPass();
                InitDailyLoginViews();
                imgVipUnlocked.SetActive(true);
            });

            // PaymentHelper.Purchase(packageData, PurchaseSuccess);
            // Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X, packageData.LogName));
        }

        //void PurchaseSuccess(bool success)
        //{
        //    if (success)
        //    {
        //        var rewards = packageData.Buy();
        //        LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_PREMIUM_PASS);

        //        InitPremiumPass();
        //        InitDailyLoginViews();

        //        Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_PASS, packageData.LogName));
        //        PaymentHelper.LogPurchase(packageData);
        //    }
        //    else
        //    {
        //        Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_FAIL, packageData.LogName));
        //    }
        //}
    }
}
