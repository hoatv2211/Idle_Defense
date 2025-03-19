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

public class DailyQuestView : MonoBehaviour
{
    public RewardView rewardView;
    public TextMeshProUGUI txtDescription;
    public TextMeshProUGUI txtTime;
    public SimpleTMPButton btnGo;
    public SimpleTMPButton btnClaim;
    public GameObject imgNoti;

    public bool isClaimed => dailyQuestData != null && dailyQuestData.Claimed;
    private DailyQuestData dailyQuestData;
    private UnityAction refreshDailyQuest;
    private void Start()
    {
        btnGo.onClick.AddListener(BtnGo_Pressed);
        btnClaim.onClick.AddListener(BtnClaim_Pressed);
    }

    public void Init(DailyQuestData _dailyQuestData, UnityAction _refreshDailyQuest)
    {
        dailyQuestData = _dailyQuestData;
        refreshDailyQuest = _refreshDailyQuest;
        rewardView.Init(dailyQuestData.GetRewards()[0]);

        Init();
    }

    private void Init()
    {
        txtDescription.text = dailyQuestData.Description;

        if (dailyQuestData.CanClaim)
        {
            var timeMax = dailyQuestData.TimeMax;
            txtTime.text = timeMax + "/" + timeMax;

            btnGo.SetActive(false);
            btnClaim.SetActive(true);
            if (dailyQuestData.Claimed)
            {
                btnClaim.SetEnable(false);
                btnClaim.labelTMP.text = Localization.Get(Localization.ID.CLAIMED); /*"Claimed";*/
                // imgNoti.SetActive(false);
            }
            else
            {
                btnClaim.SetEnable(true);
                //btnClaim.labelTMP.text = "Claim";
                btnClaim.labelTMP.text = Localization.Get(Localization.ID.CLAIM);
                // imgNoti.SetActive(true);
            }
        }
        else
        {
            txtTime.text = dailyQuestData.Time + "/" + dailyQuestData.TimeMax;

            btnGo.SetActive(true);
            btnClaim.SetActive(false);
            // imgNoti.SetActive(false);
        }
    }

    private void BtnGo_Pressed()
    {
        var dailyQuestId = dailyQuestData.Id;
        switch (dailyQuestId)
        {
            case IDs.QUEST_BUY_COIN:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowBuyCoinPanel();
                break;
            case IDs.QUEST_SPIN_IN_WHEEL:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowWheelsPanel();
                break;
            case IDs.QUEST_DISCOVERY:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowDiscoveryPanel();
                break;
            case IDs.QUEST_DISASSEMBLE_GEAR:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowDisassembleGearPanel();
                break;
            case IDs.QUEST_BASIC_SUMMON:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowSummonGatePanel();
                break;
            case IDs.QUEST_SENIOR_SUMMON:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowSummonGatePanel();
                break;
            case IDs.QUEST_FAST_TRAVEL:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowFastCollectPanel();
                break;
            case IDs.QUEST_LEVEL_UP_HERO:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowHeroPanel();
                break;
            case IDs.QUEST_PLAY_MISSION:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowMapPanel();
                break;
            case IDs.QUEST_BUY_ANYTHING_IN_STORE:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowStorePanel();
                break;
            case IDs.QUEST_UPGRADE_GEAR:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowUpgradeGearPanel();
                break;
            case IDs.QUEST_TAKE_VIP_BONUS:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowVipPanel();
                break;
            case IDs.QUEST_TAKE_SUBSCRIBLE_BONUS:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowBuyPremiumPackPanel();
                break;
        }
    }

    private void BtnClaim_Pressed()
    {
        var rewards = dailyQuestData.ClaimRewards();
        LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_DAILY_QUEST);

        // Init();
        refreshDailyQuest();
    }
}
