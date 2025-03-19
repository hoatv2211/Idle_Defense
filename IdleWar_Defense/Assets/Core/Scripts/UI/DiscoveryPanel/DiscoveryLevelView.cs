using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Common;
using Utilities.Components;

public class DiscoveryLevelView : MonoBehaviour
{
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtRewardValues;
    public TextMeshProUGUI txtUnlock;
    public RewardView rewardView;

    public SimpleTMPButton btnRaid;
    public SimpleTMPButton btnChallenge;

    private DiscoveryLevel discoveryLevel;
    private DiscoveryData discoveryData;
    private UnityAction<DiscoveryData> refreshDiscoveryLevel;

    private void Start()
    {
        btnRaid.onClick.AddListener(BtnRaid_Pressed);
        btnChallenge.onClick.AddListener(BtnChallenge_Pressed);
    }

    public void Init(DiscoveryData _discoveryData, DiscoveryLevel _discoveryLevel, UnityAction<DiscoveryData> _refreshDiscoveryLevel)
    {
        discoveryData = _discoveryData;
        discoveryLevel = _discoveryLevel;
        refreshDiscoveryLevel = _refreshDiscoveryLevel;

        var level = discoveryLevel.level;
        txtName.text = Localization.Get(Localization.ID.LEVEL)+ " " + discoveryLevel.level;

        var levelChallenge = discoveryData.LevelChallenge;
        var canClaim = discoveryData.CanClaim;
        int userLevel = GameData.Instance.LevelUnlockContent;
        // int misisonLevel = GameData.Instance.MissionsGroup.CurrentMissionId;
        int levelUnlock = discoveryLevel.levelUnlock;
        if (Constants.UNLOCK_CONTENT_TYPE == 1)
            levelUnlock = discoveryLevel.missionUnlock;
        // if(Constants.Conte)
        if (userLevel >= levelUnlock)
        {
            if (level < levelChallenge)
            {
                btnRaid.SetActive(true);
                btnChallenge.SetActive(false);
                txtUnlock.SetActive(false);
            }
            else if (level == levelChallenge)
            {
                btnRaid.SetActive(false);
                btnChallenge.SetActive(true);
                txtUnlock.SetActive(false);
            }
            else
            {
                btnRaid.SetActive(false);
                btnChallenge.SetActive(false);
                txtUnlock.text = string.Format(Localization.Get(Localization.ID.PANEL_TITLE_69), level - 1);
                //txtUnlock.text = "Challenge level " + (level - 1) + " to unlock";
                txtUnlock.SetActive(true);
            }
            txtRewardValues.text = discoveryLevel.rewardValueMin + "~" + discoveryLevel.rewardValueMax;
            txtRewardValues.SetActive(true);
        }
        else
        {
            btnRaid.SetActive(false);
            btnChallenge.SetActive(false);
            txtUnlock.text = string.Format(Localization.Get(Localization.ID.PANEL_TITLE_70), levelUnlock);
            //txtUnlock.text = "Unlock at User Level " + levelUnlock;
            if (Constants.UNLOCK_CONTENT_TYPE == 1)
                txtUnlock.text = string.Format(Localization.Get(Localization.ID.PANEL_TITLE_71), GameData.Instance.LevelUnlockContentText(levelUnlock));
            //txtUnlock.text = "Unlock after " + GameData.Instance.LevelUnlockContentText(levelUnlock);
            txtUnlock.SetActive(true);
            txtRewardValues.SetActive(false);
        }
        btnRaid.SetEnable(canClaim);
        btnChallenge.SetEnable(canClaim);

        rewardView.Init(discoveryLevel.GetReward(), false);
    }

    private void BtnRaid_Pressed()
    {
        var rewardInfo = discoveryData.ClaimReward(discoveryLevel.level);
        LogicAPI.ClaimReward(rewardInfo);
        refreshDiscoveryLevel(discoveryData);
    }

    private void BtnChallenge_Pressed()
    {
        Config.typeModeInGame = Config.TYPE_MODE_DISCOVERY;
        DiscoveriesGroup.lastDiscoveryData = discoveryData;
        MainPanel.instance.LoadGamePlayScreen();
    }
}
