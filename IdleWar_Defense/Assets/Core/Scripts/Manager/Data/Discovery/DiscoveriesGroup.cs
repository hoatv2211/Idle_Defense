
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Utilities.Service.RFirebase;
using Zirpl.CalcEngine;
using Random = System.Random;

namespace FoodZombie
{
    [System.Serializable]
    public class DiscoveryLevel
    {
        public int id;
        public string name;
        public int level;
        public int missionUnlock;
        public int levelUnlock;
        public int[] daysOpen;
        public int rewardType;
        public int rewardId;
        public int rewardValueMin;
        public int rewardValueMax;
        public int limitInDay;

        public RewardInfo GetReward()
        {
            return new RewardInfo(rewardType, rewardId, Config.EasyRandom(rewardValueMin, rewardValueMax));
        }
    }

    public class DiscoveryData : DataGroup
    {
        public List<DiscoveryLevel> discoveryLevels { get; private set; }
        private IntegerData countClaimInDay; //ăn bao nhiêu lần trong ngày rồi
        private IntegerData levelChallenge; //level đã challenge
        private IntegerData boughtMoreCount;

        public int CountClaimInDay => countClaimInDay.Value;
        public int LevelChallenge => levelChallenge.Value;

        public string Name => discoveryLevels[0].name;
        public string NameLocal => Localization.Get("PANEL_DISCOVERY_" + discoveryLevels[0].id);
        public int[] DaysOpen => discoveryLevels[0].daysOpen;

        public int LimitInDay
        {
            get
            {
                var vipInfo = GameData.Instance.UserGroup.GetCurrentVipInfo();
                if (vipInfo == null) return discoveryLevels[0].limitInDay;
                return discoveryLevels[0].limitInDay + vipInfo.discoveryTime;
            }
        }
        public bool CanClaim => countClaimInDay.Value < LimitInDay;

        public bool CanBuy
        {
            get
            {
                var vipInfo = GameData.Instance.UserGroup.GetCurrentVipInfo();
                if (vipInfo == null) return countClaimInDay.Value > 0 && boughtMoreCount.Value < Constants.BUY_DISCOVERY_TIME;
                return countClaimInDay.Value > 0 && boughtMoreCount.Value < Constants.BUY_DISCOVERY_TIME + vipInfo.discoveryBuyTime;
            }
        }

        public DiscoveryData(int _id, List<DiscoveryLevel> _discoveryLevels) : base(_id)
        {
            discoveryLevels = _discoveryLevels;
            countClaimInDay = AddData(new IntegerData(0, 0));
            levelChallenge = AddData(new IntegerData(1, 1));
            // boughtMoreCount = AddData(new BoolData(2, false));
            boughtMoreCount = AddData(new IntegerData(3, 0));
        }

        public void Clear()
        {
            countClaimInDay.Value = 0;
            boughtMoreCount.Value = 0;
        }

        public RewardInfo ClaimRewardAndLevelUp()
        {
            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.Discovery();
            GameData.Instance.CampaignsGroup.AddDiscoveryCount();
            countClaimInDay.Value++;
            var rewards = discoveryLevels[levelChallenge.Value - 1].GetReward();
            LevelUp();
            return rewards;
        }

        public RewardInfo ClaimReward(int level)
        {
            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.Discovery();
            GameData.Instance.CampaignsGroup.AddDiscoveryCount();
            countClaimInDay.Value++;
            return discoveryLevels[level - 1].GetReward();
        }

        private void LevelUp()
        {
            levelChallenge.Value++;
        }

        public void BuyMoreCount()
        {
            boughtMoreCount.Value++;
            countClaimInDay.Value--;
        }
    }

    //==================================

    public class DiscoveriesGroup : DataGroup
    {
        #region Members

        private DataGroup discoverysGroup;
        private DataGroup missionsGroup;

        public static DiscoveryData lastDiscoveryData = null;

        #endregion

        //=============================================

        #region Public

        public DiscoveriesGroup(int pId) : base(pId)
        {
            discoverysGroup = AddData(new DataGroup(0));
            missionsGroup = AddData(new DataGroup(1));

            InitDiscoveries();
            InitMapsMissionsGroup();
        }

        public void NewDay()
        {
            var discoveryDatas = GetAllDiscoveryDatas();
            var count = discoveryDatas.Count;
            for (int i = 0; i < count; i++)
            {
                discoveryDatas[i].Clear();
            }
        }

        public List<DiscoveryData> GetAllDiscoveryDatas()
        {
            var list = new List<DiscoveryData>();
            foreach (DiscoveryData item in discoverysGroup.Children)
                list.Add(item);
            return list;
        }

        private DiscoveryData GetDiscoveryData(int pId)
        {
            return discoverysGroup.GetData<DiscoveryData>(pId);
        }

        public MissionData GetMissionData(int pId)
        {
            return missionsGroup.GetData<MissionData>(pId);
        }

        public MissionData GetLastMissionData()
        {
            var lastLevelChallenge = lastDiscoveryData.LevelChallenge - 1;
            if (lastLevelChallenge < 1) lastLevelChallenge = 1;
            return GetMissionData(lastLevelChallenge);
        }

        public MissionData GetCurrentMissionData()
        {
            return GetMissionData(lastDiscoveryData.LevelChallenge);
        }

        public void Cleared(ref List<RewardInfo> mainRewardInfos, ref List<RewardInfo> bonusRewardInfos, Dictionary<int, int> totalKills)
        {
            //count enemies
            int enemyId;
            int killsCount = 0;
            foreach (var kill in totalKills)
            {
                enemyId = kill.Key;
                killsCount = kill.Value;
                var enemyData = GameData.Instance.EnemiesGroup.GetEnemyData(enemyId);
                enemyData.AddKillsCount(killsCount);

                if (enemyId == IDs.MB1
                    || enemyId == IDs.MB2
                    || enemyId == IDs.MB3
                    || enemyId == IDs.MB4
                    || enemyId == IDs.B1
                    || enemyId == IDs.B2
                    || enemyId == IDs.B3
                    || enemyId == IDs.B4)
                {
                    //Daily Quest và Achievement
                    GameData.Instance.AchievementsGroup.KillBoss();
                }
            }

            //reward
            mainRewardInfos = new List<RewardInfo> { lastDiscoveryData.ClaimRewardAndLevelUp() };
        }

        public RewardInfo Lose()
        {
            //gold reward
            var coin = 100;

            return new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, coin);
        }

        //noti
        private bool IsAnyDiscoveryCanClaim()
        {
            var today = GameData.Instance.GameConfigGroup.GetDayOfWeek();
            var userLevel = GameData.Instance.UserGroup.Level;

            var list = GetAllDiscoveryDatas();
            for (var index = 0; index < list.Count; index++)
            {
                var discoveryData = list[index];
                if (userLevel >= discoveryData.discoveryLevels[0].levelUnlock && discoveryData.CanClaim && discoveryData.DaysOpen.Contains(today)) return true;
            }

            return false;
        }

        public bool CheckNoti()
        {
            return IsAnyDiscoveryCanClaim();
        }

        #endregion

        //==============================================

        #region Private

        private void InitDiscoveries()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/Discovery");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<DiscoveryLevel>(dataContent);
            if (collection != null)
            {
                DiscoveryData lastDiscoveyData = null;
                // collection.Sort();
                foreach (var item in collection)
                {
                    if (lastDiscoveyData == null || lastDiscoveyData.Id != item.id)
                    {
                        var discoveryLevels = new List<DiscoveryLevel>();
                        discoveryLevels.Add(item);
                        lastDiscoveyData = new DiscoveryData(item.id, discoveryLevels);
                        discoverysGroup.AddData(lastDiscoveyData);
                    }
                    else
                    {
                        lastDiscoveyData.discoveryLevels.Add(item);
                    }
                }
            }

            //Debug.Log(collection);
        }

        private void InitMapsMissionsGroup()
        {
            //init map
            string dataContent = GameData.GetTextContent("LevelDesign/totalInfo");
            if (dataContent != null && !dataContent.Equals(""))
            {
                var totalInfo = JsonUtility.FromJson<TotalInfo>(dataContent);
                var countDiscovery = totalInfo.countDiscovery;
                //get tổng map

                for (int i = 0; i < countDiscovery; i++)
                {
                    dataContent = GameData.GetTextContent("LevelDesign/Discovery/mission_discovery_" + (i + 1));
                    if (dataContent != null && !dataContent.Equals(""))
                    {
                        //đọc thông tin MissionInfo
                        //vì mission id theo kiểu 1001->1010: tức là đang ở map 1 và mission thứ 1 -> mission 10
                        //phải chơi kiểu 1000 này rồi map 2 là 2000 vì sau có thể tăng giảm map mà nó ko ảnh hưởng order
                        //nên phải get theo id
                        var missionInfo = JsonUtility.FromJson<MissionInfo>(dataContent);
                        var data = new MissionData(missionInfo.id, missionInfo, 1, Config.TYPE_MODE_DISCOVERY);
                        missionsGroup.AddData(data);
                    }
                }
            }
        }

        #endregion
    }
}