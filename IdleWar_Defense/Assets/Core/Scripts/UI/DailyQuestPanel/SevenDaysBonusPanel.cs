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
using Utilities.Inspector;

namespace FoodZombie.UI
{
    public class SevenDaysBonusPanel : MonoBehaviour
    {
        EventAllPanel evenAllPanel;
        public RewardView[] rewardViews;
        public Image[] imgShadows;
        public Image[] imgCompletes;
        public Image[] imgHighlights;
        public GameObject[] dayShows;
        // public SimpleTMPButton[] btnClaims;

        private void Start()
        {

            foreach (Image imgHighlight in imgHighlights)
            {
                imgHighlight.GetComponent<SimpleTMPButton>().onClick.AddListener(BtnClaim_Pressed);
            }
        }

        public void Show(bool reloadShow = true)
        {
            if (reloadShow)
                this.gameObject.SetActive(true);
            evenAllPanel.Lock(true);
            //Lock(true);
            var dailyQuestsGroup = GameData.Instance.DailyQuestsGroup;
            var daysBonus = dailyQuestsGroup.DaysBonus;
            var claimedToday = dailyQuestsGroup.ClaimedToday;
            var dayCount = dailyQuestsGroup.DayCount;
            var count = rewardViews.Length;

            if (dayCount >= 7) dayCount = 7;

            for (int i = 0; i < dayCount; i++)
            {
                rewardViews[i].Init(daysBonus[i].GetReward());
                rewardViews[i].CanShowInfor = true;
                if (i == dayCount - 1)
                {
                    imgShadows[i].SetActive(claimedToday);
                    imgCompletes[i].SetActive(claimedToday);

                }
                else
                {
                    imgShadows[i].SetActive(true);
                    imgCompletes[i].SetActive(true);
                    // dayShows[i].SetActive(true);
                }
                dayShows[i].SetActive(false);
            }

            for (int i = dayCount; i < count; i++)
            {
                rewardViews[i].Init(daysBonus[i].GetReward());
                rewardViews[i].CanShowInfor = true;
                imgShadows[i].SetActive(true);
                imgCompletes[i].SetActive(false);
            }

            count = imgHighlights.Length;
            for (int i = 0; i < count; i++)
            {
                imgHighlights[i].SetActive(false);
            }
            imgHighlights[dayCount - 1].SetActive(!claimedToday);
            // btnClaim.SetEnable(!claimedToday);
            var reward = dailyQuestsGroup.ClaimToDayBonus();
            if (reward != null)
            {
                CoroutineUtil.StartCoroutine(IEClaimReward(reward));
            }
            else
            {
                evenAllPanel.Lock(false);
                // Lock(false);
            }
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator IEClaimReward(RewardInfo reward)
        {
            yield return null;
            yield return new WaitForSeconds(1f);
            // Lock(false);
            evenAllPanel.Lock(false);
            Show(false);
            LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_7DAYS_BONUS);
        }

        private void BtnClaim_Pressed()
        {
            var reward = GameData.Instance.DailyQuestsGroup.ClaimToDayBonus();
            if (reward != null)
                LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_7DAYS_BONUS);

            // Init();
        }

        public void Init(EventAllPanel evenAllPanel)
        {
            this.evenAllPanel = evenAllPanel;
        }
    }
}
