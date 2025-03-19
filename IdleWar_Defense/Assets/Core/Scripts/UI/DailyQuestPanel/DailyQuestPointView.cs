using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class DailyQuestPointView : MonoBehaviour
{
    public RewardView rewardView;
    public SimpleTMPButton btnClaim;
    public Image imgComplete;
    public GameObject imgShadown;
    public GameObject imgLock;
    public GameObject imgNoti;

    private DailyQuestPointData dailyQuestPointData;

    private void Start()
    {
        btnClaim.onClick.AddListener(BtnClaim_Pressed);
    }

    public void Init(DailyQuestPointData _dailyQuestPointData)
    {
        dailyQuestPointData = _dailyQuestPointData;
        rewardView.Init(dailyQuestPointData.GetReward());
        rewardView.CanShowInfor = false;
        Init();
    }

    private void Init()
    {
        if (dailyQuestPointData.Claimed)
        {
            imgComplete.SetActive(true);
            btnClaim.interactable = false;
            imgShadown.SetActive(true);
            imgLock.SetActive(false);
            imgNoti.SetActive(false);
        }
        else
        {
            imgComplete.SetActive(false);
            if (dailyQuestPointData.CanClaim)
            {
                btnClaim.interactable = true;
                imgShadown.SetActive(false);
                imgLock.SetActive(false);
                imgNoti.SetActive(true);
            }
            else
            {
                btnClaim.interactable = false;
                imgShadown.SetActive(true);
                imgLock.SetActive(false);//tạm ko hiện khóa
                imgNoti.SetActive(false);
            }
        }
    }

    private void BtnClaim_Pressed()
    {
        var reward = dailyQuestPointData.ClaimReward();
        LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_DAILY_QUEST_POINT);

        Init();
    }
}
