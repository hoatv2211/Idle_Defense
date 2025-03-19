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

public class VipPanel : MyGamesBasePanel
{
    public TextMeshProUGUI txtVipTitle;
    public TextMeshProUGUI txtVipExp;
    public TextMeshProUGUI txtVipPercent;
    public Image imgVipPercent;

    //info
    // public TextMeshProUGUI txtHeroSlot;
    public TextMeshProUGUI txtBuyCoinLimit;
    public TextMeshProUGUI txtBuyFastLoot;
    public TextMeshProUGUI txtDiscoveryTime;
    public TextMeshProUGUI txtDiscoveryBuyTime;
    public TextMeshProUGUI txtAfkBonusCoin;
    public TextMeshProUGUI txtAfkHeroExpBonus;
    public TextMeshProUGUI txtUnlock;
    public GameObject ObjectLock;

    public Transform transformPool;
    [SerializeField, Tooltip("Buildin Pool")] private List<RewardView> rewardViewsPool;

    public SimpleTMPButton btnClaim;

    public SimpleTMPButton btnNext;
    public SimpleTMPButton btnPre;

    private UserGroup UserGroup => GameData.Instance.UserGroup;

    private int currentVipShow;

    private void Start()
    {
        btnClaim.onClick.AddListener(BtnClaim_Pressed);
        btnNext.onClick.AddListener(BtnNext_Pressed);
        btnPre.onClick.AddListener(BtnPre_Pressed);
    }

    internal override void Init()
    {
        var vipInfo = UserGroup.GetCurrentVipInfo();
        ObjectLock.SetActive(vipInfo == null);
        Init(vipInfo);
    }

    internal void Init(VipUser vipInfo)
    {
        if (vipInfo == null)
        {
            // ObjectLock.SetActive(true);
            btnClaim.SetActive(false);
            txtVipPercent.text = "0/100%";
            imgVipPercent.fillAmount = 0;
            currentVipShow = 0;
            Init(UserGroup.VipUsers[currentVipShow]);
            return;
        }
        var vipLevel = UserGroup.VipLevel;
        txtVipExp.text = Localization.Get(Localization.ID.PANEL_VIP_TITLE_43) +": " + UserGroup.VipExp;
        //if(vipL)
        // ObjectLock.SetActive(vipLevel > 0);
        if (vipInfo.vip == vipLevel)
        {
            btnClaim.SetActive(true);
            var percent = UserGroup.GetPercentVipExp();
            txtVipPercent.text = (percent * 100f).ToString("0") + "/100%";
            imgVipPercent.fillAmount = percent;
        }
        else if (vipInfo.vip < vipLevel)
        {
            btnClaim.SetActive(false);
            txtVipPercent.text = "100/100%";
            imgVipPercent.fillAmount = 1f;
        }
        else
        {
            btnClaim.SetActive(false);
            txtVipPercent.text = "0/100%";
            imgVipPercent.fillAmount = 0f;
        }

        rewardViewsPool.Free();

        txtVipTitle.text = "VIP " + vipInfo.vip;

        txtBuyCoinLimit.text = Constants.BUY_COIN_LIMIT + " +<color=#FFE822>" + vipInfo.buyCoinLimit + "</color>";
        txtBuyFastLoot.text = Constants.BUY_FAST_LOOT_LIMIT + " +<color=#FFE822>" + vipInfo.buyFastLoot + "</color>";
        txtDiscoveryTime.text = Constants.DISCOVERY_LIMIT + " +<color=#FFE822>" + vipInfo.discoveryTime + "</color>";
        txtDiscoveryBuyTime.text = Constants.BUY_DISCOVERY_TIME + " +<color=#FFE822>" + vipInfo.discoveryBuyTime + "</color>";
        txtAfkBonusCoin.text = vipInfo.afkBonusCoin + "%";
        txtAfkHeroExpBonus.text = vipInfo.afkHeroExpBonus + "%";
        if (vipInfo.unlockFeature != null && !vipInfo.unlockFeature.Equals(""))
        {
            txtUnlock.text = vipInfo.unlockFeature;
            txtUnlock.transform.parent.SetActive(true);
        }
        else
        {
            txtUnlock.transform.parent.SetActive(false);
        }

        var rewardsInfos = vipInfo.GetRewards();
        int count = rewardsInfos.Count;
        for (int i = 0; i < count; i++)
        {
            var item = rewardsInfos[i];
            var rewardView = rewardViewsPool.Obtain(transformPool);
            rewardView.Init(item);
            rewardView.SetActive(true);
        }

        btnClaim.SetEnable(!UserGroup.ClaimedVipToDay);

        if (vipInfo.vip <= 1)
        {
            btnNext.SetActive(true);
            btnPre.SetActive(false);
        }
        else if (vipInfo.vip >= UserGroup.VipUsers.Count)
        {
            btnNext.SetActive(false);
            btnPre.SetActive(true);
        }
        else
        {
            btnNext.SetActive(true);
            btnPre.SetActive(true);
        }

        currentVipShow = vipInfo.vip;
    }

    private void BtnClaim_Pressed()
    {
        Back();
        var rewardInfos = UserGroup.ClaimVip();
        LogicAPI.ClaimRewards(rewardInfos, TrackingConstants.VALUE_LEVEL_UP);
    }

    private void BtnNext_Pressed()
    {
        Init(UserGroup.VipUsers[currentVipShow]);
    }

    private void BtnPre_Pressed()
    {
        Init(UserGroup.VipUsers[currentVipShow - 2]);
    }
}
