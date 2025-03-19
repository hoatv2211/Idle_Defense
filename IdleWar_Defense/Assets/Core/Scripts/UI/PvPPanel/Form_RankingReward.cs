using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FoodZombie.UI
{
    public class Form_RankingReward : EnhancedScrollerCellView
    {

        public PvPOneRankData Info;
        [SerializeField] private GameObject imgRankCurrent;
        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private TextMeshProUGUI txtRankPointRequest;
        [SerializeField] private List<RewardView> listRewards;

        public void SetInit(PvPOneRankData _info)
        {
            Info = _info;
            //imgRankCurrent.SetActive(GameData.Instance.UserGroup.UserData.ScorePvPRank >= Info.RankPointRequest);
            txtName.text = Info.RankName;
            txtRankPointRequest.text = "ELO~"+Info.RankPointRequest;

            bool isRewarded = false;
            isRewarded = UserGroup.UserData.RWisClaim(_info.ID);

            Debug.Log(_info.ID + " : " + isRewarded);

            listRewards[2].gameObject.SetActive(Info.RWGem > 0);
            if (Info.RWGem>0)
            {
                RewardInfo reward = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_GEM, Info.RWGem);
                listRewards[2].SetRewadInPvP(reward, isRewarded) ;
            }

            listRewards[0].gameObject.SetActive(Info.RWHonor > 0);
            if (Info.RWHonor > 0)
            {
                RewardInfo reward = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_HONOR, Info.RWHonor);
                listRewards[0].SetRewadInPvP(reward, isRewarded);
            }

            listRewards[1].gameObject.SetActive(Info.RWSummonScroll > 0);
            if (Info.RWSummonScroll > 0)
            {
                RewardInfo reward = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_POWER_FRAGMENT, Info.RWSummonScroll);
                listRewards[1].SetRewadInPvP(reward, isRewarded);
            }

        }

    }
}

