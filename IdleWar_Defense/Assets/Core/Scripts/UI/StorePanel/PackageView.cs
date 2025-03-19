using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using TMPro;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;
using Utilities.Service;
using EventDispatcher = Utilities.Common.EventDispatcher;
using UnityEngine.UI;
using FoodZombie.UI;

public class PackageView : MonoBehaviour
{
    public int limitShowReward = 10;
    public Transform transformPool;
    [SerializeField, Tooltip("Buildin Pool")] private List<RewardView> specialRewardViewsPool;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtCost;
    public SimpleTMPButton btnBuy;
    public SimpleTMPButton btnGoShop;

    public Sprite[] imgBGs;
    public Image imgBG;
    public Sprite[] imgGemSprites;
    public Image imgGem;
    public GameObject groupGem;
    public TextMeshProUGUI txtGemValue;

    public GameObject modelHS2;
    public GameObject modelHS3;
    public GameObject modelHA4;

    public PackageData packageData;

    public bool firstTimexValue_is;
    public GameObject firstTimexValue_view;

    public Action OnBuySuccess;

    private void Start()
    {
        btnBuy.onClick.AddListener(BtnBuy_Pressed);
        if (btnGoShop != null)
            btnGoShop.onClick.AddListener(() => { if (OnBuySuccess != null) OnBuySuccess(); MainPanel.instance.ShowStorePanel(); });
    }

    public void Init(PackageData _packageData)
    {
        packageData = _packageData;

        if (groupGem != null) groupGem.SetActive(false);
        if (txtGemValue != null) txtGemValue.text = "";
        specialRewardViewsPool.Free();
        var rewards = packageData.GetRewards();
        var count = rewards.Count;
        if (count > limitShowReward) count = limitShowReward;
        if (transformPool != null && specialRewardViewsPool != null && specialRewardViewsPool.Count != 0)
        {
            if (modelHS2 != null) modelHS2.SetActive(false);
            if (modelHS3 != null) modelHS3.SetActive(false);
            if (modelHA4 != null) modelHA4.SetActive(false);

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
                    if (rewardMaxGem == null) rewardMaxGem = reward;
                    else if (rewardMaxGem.Value < reward.Value) rewardMaxGem = reward;
                }
            }

            if (rewardUnlockHero != null)
            {
                if (rewardUnlockHero.Id == IDs.HS2 && modelHS2 != null) modelHS2.SetActive(true);
                else if (rewardUnlockHero.Id == IDs.HS3 && modelHS3 != null) modelHS3.SetActive(true);
                else if (rewardUnlockHero.Id == IDs.HA4 && modelHA4 != null) modelHA4.SetActive(true);
            }
            else if (rewardMaxGem != null)
            {
                groupGem.SetActive(true);
                txtGemValue.text = rewardMaxGem.Value + "";
                imgGem.sprite = imgGemSprites[packageData.ImgGemIndex];
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var reward = rewards[i];

                if (reward.Type == IDs.REWARD_TYPE_CURRENCY && reward.Id == IDs.CURRENCY_GEM)
                {
                    groupGem.SetActive(true);
                    txtGemValue.text = reward.Value + "";
                    imgGem.sprite = imgGemSprites[packageData.ImgGemIndex];
                    break;
                }
            }
        }

        Init();
    }

    private void Init()
    {
        if (txtName != null) txtName.text = packageData.Name;

        if (imgBG != null) imgBG.sprite = imgBGs[packageData.ImgBgIndex];
        btnBuy.gameObject.SetActive(true);
        if (btnGoShop != null)
            btnGoShop.gameObject.SetActive(false);
        if (firstTimexValue_view != null)
            firstTimexValue_view.SetActive(false);
        if (packageData.CanBuy)
        {
            if (firstTimexValue_is && firstTimexValue_view != null)
                firstTimexValue_view.SetActive(GameData.Instance.UserGroup.inappBuyGemCount.Value <= 0);

            var usd = packageData.Usd;
            if (usd != null && !usd.Equals(""))
            {
                PaymentHelper.SetTextLocalizedPriceString(txtCost, packageData);
                btnBuy.SetEnable(true);
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

                    if (btnGoShop != null)
                    {
                        btnBuy.gameObject.SetActive(false);
                        btnGoShop.gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            var usd = packageData.Usd;
            if (usd != null && !usd.Equals(""))
            {
                txtCost.text = "Bought";
                btnBuy.SetEnable(false);
            }
            else
            {
                txtCost.text = "Claimed";
                btnBuy.SetEnable(false);
            }
        }

        // if (packageData.Type == IDs.SPECIAL_PACK && packageData.Bought)
        // {
        //     gameObject.SetActive(false);
        // }
        // else
        // {
        //     gameObject.SetActive(true);
        // }
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
            var rewards = packageData.Buy();
            LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_FIRST_PURCHASE, true, OnBuySuccess);
        }
        Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X, packageData.LogName));
    }

    void PurchaseSuccess(bool success)
    {

        if (success)
        {
            List<RewardInfo> rewards = packageData.Buy();
            if (firstTimexValue_is)
            {
                if (GameData.Instance.UserGroup.inappBuyGemCount.Value <= 0)
                {
                    //x2 GEM
                    rewards[0].SetValue(rewards[0].Value * 2);
                    rewards.RemoveAt(1);
                    if (firstTimexValue_view != null)
                        firstTimexValue_view.SetActive(false);
                }

                GameData.Instance.UserGroup.InappBuyGem();
                EventDispatcher.Raise(new BuyCountChangeEvent());
            }
            LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_STORE, true, OnBuySuccess);

            Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_PASS, packageData.LogName));
            PaymentHelper.LogPurchase(packageData);
        }
        else
        {
            try
            {
                Config.LogEvent(string.Format(TrackingConstants.CLICK_IAP_X_FAIL, packageData.LogName));
                MainPanel.instance.ShowWarningPopup(ConstantsString.INAPP_FAILD);
                //   if (OnBuySuccess != null) OnBuySuccess();
            }
            catch (Exception ex)
            {

                Debug.LogError(ex.ToString());
            }
           
        }


    }
}
