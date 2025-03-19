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

public class AchievementView : MonoBehaviour
{
    public RewardView rewardView;
    public TextMeshProUGUI txtDescription;
    public TextMeshProUGUI txtTime;
    public SimpleTMPButton btnGo;
    public SimpleTMPButton btnClaim;
    
    private AchievementData achievementData;
    private UnityAction refreshAchievement;

    private void Start()
    {
        btnGo.onClick.AddListener(BtnGo_Pressed);
        btnClaim.onClick.AddListener(BtnClaim_Pressed);
    }

    public void Init(AchievementData _achievementData, UnityAction _refreshAchievement)
    {
        achievementData = _achievementData;
        refreshAchievement = _refreshAchievement;
        rewardView.Init(achievementData.GetRewards()[0]);

        Init();
    }

    private void Init()
    {
        txtDescription.text = achievementData.Description;
        txtTime.text = achievementData.Value + "/" + achievementData.ValueMax;

        if (achievementData.CanClaim)
        {
            btnGo.SetActive(false);
            btnClaim.SetActive(true);
        }
        else
        {
            btnGo.SetActive(true);
            btnClaim.SetActive(false);
        }
    }

    private void BtnGo_Pressed()
    {
        var achievementId = achievementData.Id;
        switch (achievementId)
        {
            case IDs.CHALLENGE_COMPLETE_MAP:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowMapPanel();
                break;
            case IDs.CHALLENGE_SUMMON_ABC:
            case IDs.CHALLENGE_SUMMON_S:
            case IDs.CHALLENGE_SUMMON_SS:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowSummonGatePanel();
                break;
            case IDs.CHALLENGE_GEAR_S:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowStorePanel();
                break;
            case IDs.CHALLENGE_GEAR_SS:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowStorePanel();
                break;
            case IDs.CHALLENGE_WHEELS_COUNT:
                MainPanel.instance.DailyQuestPanel.Back();
                MainPanel.instance.ShowWheelsPanel();
                break;
        }
    }
    
    private void BtnClaim_Pressed()
    {
        var rewards = achievementData.ClaimRewards();
        LogicAPI.ClaimRewards(rewards, TrackingConstants.VALUE_ACHIEVEMENT);

        // Init();
        refreshAchievement();
    }
}
