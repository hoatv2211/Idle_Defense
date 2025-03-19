using System;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using Utilities.Components;
using Utilities.Service;
using Random = UnityEngine.Random;

namespace Core.Scripts.UI.Drone_Panel
{
    public class DronePayGemPanel : MyGamesBasePanel
    {
        public RewardView[] items;
        public SimpleTMPButton btnGem, btnIap;
        private List<int> _itemIds;
        private List<RewardInfo> _rewardInfos;

        private Action callBackAction;
        private void Start()
        {
            btnGem.onClick.AddListener(BtnGem_Pressed);
            btnIap.onClick.AddListener(BtnIap_Pressed);
            btnGem.labelTMP.text = Constants.PREMIUM_DRONE_GEM_COST.ToString();
            btnIap.labelTMP.text = Constants.PREMIUM_DRONE_USD_COST;

            PaymentHelper.SetTextLocalizedPriceString(btnIap.labelTMP, Constants.PREMIUM_DRONE_GEM_SKU);
        }

        private void BtnGem_Pressed()
        {
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            if (!currenciesGroup.CanPay(IDs.CURRENCY_GEM, Constants.PREMIUM_DRONE_GEM_COST))
            {
                MainGamePanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_2));
                //MainGamePanel.instance.ShowWarningPopup("Not enough Gems");
                GameData.Instance.UserGroup.MissGemCount++;
                return;
            }
            currenciesGroup.Pay(IDs.CURRENCY_GEM, Constants.PREMIUM_DRONE_GEM_COST, TrackingConstants.VALUE_BUY_DRONE);
            Back();
            ClaimRewards();
        }

        private void BtnIap_Pressed()
        {
            Back();
            PaymentHelper.Purchase(Constants.PREMIUM_DRONE_GEM_SKU, PurchaseSuccess);
        }

        private void ClaimRewards()
        {
            LogicAPI.ClaimRewards(_rewardInfos);
            GameplayController.Instance.ShowTrapDatas();
        }

        private void PurchaseSuccess(bool success)
        {
            if (success)
            {
                Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_PASS, Constants.PREMIUM_DRONE_GEM_SKU));
                PaymentHelper.LogPurchase(Constants.PREMIUM_DRONE_GEM_SKU);
            }
            else
            {
                Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_FAIL, Constants.PREMIUM_DRONE_GEM_SKU));
                //  MainPanel.instance.ShowWarningPopup(ConstantsString.INAPP_FAILD);
                MainGamePanel.instance.ShowWarningPopup(ConstantsString.INAPP_FAILD);
                return;
            }

            ClaimRewards();
        }
        private void OnEnable()
        {
            GameplayController.Instance.PauseGame();
        }

        internal override void Back()
        {
            GameplayController.Instance.ResumeGame();
            callBackAction?.Invoke();
            base.Back();
        }

        internal void Init(Action callback)
        {
            callBackAction = callback;
            SelectRandomTrapId();
            CreateRandomReward();

            for (var i = 0; i < items.Length; i++)
            {
                var itemView = items[i];
                itemView.Init(_rewardInfos[i]);
            }
        }
        private void CreateRandomReward()
        {
            _rewardInfos = new List<RewardInfo>();
            var premiumDroneItems = GameData.Instance.StoreGroup.PremiumDroneItems;
            var count = _itemIds.Count;
            for (var i = 0; i < count; i++)
            {
                _rewardInfos.Add(premiumDroneItems[_itemIds[i]].GetReward());
            }
        }

        private void SelectRandomTrapId()
        {
            var premiumDroneItems = GameData.Instance.StoreGroup.PremiumDroneItems;
            var count = premiumDroneItems.Count;
            _itemIds = new List<int>();
            for (var i = 0; i < 2; i++)
            {
                int rd;
                do
                {
                    rd = Random.Range(0, count);
                } while (_itemIds.Contains(rd) && _itemIds.Count < 2);
                _itemIds.Add(rd);
            }
        }
    }
}
