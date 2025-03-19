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
using Utilities.Service;

namespace FoodZombie.UI
{
    public class PackageDetailPanel : MyGamesBasePanel
    {
        public TextMeshProUGUI txtPackageName;
        public TextMeshProUGUI txtMainRewardInfo1;
        public TextMeshProUGUI txtMainRewardInfo2;
        public GameObject imgGemIcon;
        public GameObject modelHS2;
        public GameObject modelHS3;
        public GameObject modelHA4;
        public GameObject heroSlot;

        public Image imgHeroBG;
        public Image imgGemBG;
        public TextMeshProUGUI txtDetailFirstTimePurchase;
        
        public Transform transformPool;
        [SerializeField, Tooltip("Buildin Pool")] private List<RewardView> specialRewardViewsPool;
        public Sprite[] imgGemSprites;
        public Image imgGem;

        public RectTransform vipReward;
        public TextMeshProUGUI txtVipValue;
        
        public TextMeshProUGUI txtCost;
        public SimpleTMPButton btnBuy;
        
        public PackageData packageData;

        void Start()
        {
            btnBuy.onClick.AddListener(BtnBuy_Pressed);
        }

        public void Init(PackageData _packageData)
        {
            packageData = _packageData;

            //reward list
            specialRewardViewsPool.Free();
            var rewards = packageData.GetRewards();
            var count = rewards.Count;
            //tìm reward unlock hero và reward max gem
            RewardInfo rewardUnlockHero = null;
            RewardInfo rewardMaxGem = null;
            for (int i = 0; i < count; i++)
            {
                var reward = rewards[i];
                
                var rewardView = specialRewardViewsPool.Obtain(transformPool);
                rewardView.Init(reward);
                rewardView.SetActive(true);
                
                if (reward.Type == IDs.REWARD_TYPE_UNLOCK_CHARACTER)
                {
                    rewardUnlockHero = reward;
                }
                else if (reward.Type == IDs.REWARD_TYPE_CURRENCY && reward.Id == IDs.CURRENCY_GEM)
                {
                    if(rewardMaxGem == null) rewardMaxGem = reward;
                    else if (rewardMaxGem.Value < reward.Value) rewardMaxGem = reward;
                }
            }

            //reset
            modelHS2.SetActive(false);
            modelHS3.SetActive(false);
            modelHA4.SetActive(false);
            heroSlot.SetActive(false);
            txtMainRewardInfo2.SetActive(false);
            imgGem.SetActive(false);
            imgGemIcon.SetActive(false);
            imgHeroBG.SetActive(false);
            imgGemBG.SetActive(false);
            if (rewardUnlockHero != null)
            {
                if (rewardUnlockHero.Id == IDs.HS2) modelHS2.SetActive(true);
                else if (rewardUnlockHero.Id == IDs.HS3) modelHS3.SetActive(true);
                else if (rewardUnlockHero.Id == IDs.HA4) modelHA4.SetActive(true);
                heroSlot.SetActive(true);

                var heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(rewardUnlockHero.Id);
                txtMainRewardInfo1.text = heroDefinition.name;
                txtMainRewardInfo2.text = heroDefinition.GetRankName() + " rank hero";
                txtMainRewardInfo2.SetActive(true);
                imgHeroBG.SetActive(true);
            }
            else if (rewardMaxGem != null)
            {
                txtMainRewardInfo1.text = rewardMaxGem.Value + "";
                imgGem.sprite = imgGemSprites[packageData.ImgGemIndex];
                
                imgGem.SetActive(true);
                imgGemIcon.SetActive(true);
                imgGemBG.SetActive(true);
            }
            
            //text info
            txtPackageName.text = packageData.Name;
            var usd = packageData.Usd;
            if (usd != null && !usd.Equals(""))
            {
                PaymentHelper.SetTextLocalizedPriceString(txtCost, packageData);
                btnBuy.SetEnable(true);
                txtDetailFirstTimePurchase.SetActive(false);
            }
            else
            {
                //Claim khi user thanh toán thành công gói bất kỳ
                txtCost.text = "Claim";
                if (GameData.Instance.StoreGroup.BuyCount > 0)
                {
                    btnBuy.SetEnable(true);
                }
                else
                {
                    btnBuy.SetEnable(false);
                }
                txtDetailFirstTimePurchase.SetActive(true);
            }

            if (packageData.Type == IDs.SPECIAL_PACK && !packageData.CanBuy)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }

            //main reward
        }

        private void BtnBuy_Pressed()
        {
            var usd = packageData.Usd;
            if (usd != null && !usd.Equals(""))
            {
                PaymentHelper.Purchase(packageData, PurchaseSuccess);
            }
            else
            {
                Back();
                
                var rewards = packageData.Buy();
                LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_FIRST_PURCHASE);
            }
            Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X, packageData.LogName));
        }

        void PurchaseSuccess(bool success)
        {
            if (success)
            {
                Back();
                
                var rewards = packageData.Buy();
                LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_STORE);
            
                Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_PASS, packageData.LogName));
                PaymentHelper.LogPurchase(packageData);
            }
            else
            {
                Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_FAIL, packageData.LogName));
                MainPanel.instance.ShowWarningPopup(ConstantsString.INAPP_FAILD);
            }
        }
    }
}