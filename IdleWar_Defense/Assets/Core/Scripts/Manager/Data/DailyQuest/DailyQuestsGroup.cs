
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Utilities.Service.RFirebase;
using Random = System.Random;

namespace FoodZombie
{
    [System.Serializable]
    public class DailyQuestDefinition
    {
        public int id;
        public string description;
        public int timeMax;
        public int[] rewardTypes;
        public int[] rewardIds;
        public int[] rewardValues;

        public List<RewardInfo> GetRewards()
        {
            var rewards = new List<RewardInfo>();
            var count = rewardTypes.Length;
            for (int i = 0; i < count; i++)
            {
                rewards.Add(new RewardInfo(rewardTypes[i], rewardIds[i], rewardValues[i]));
            }

            return rewards;
        }

        public string GetDescription()
        {
            return string.Format(Localization.Get("QUEST_DESC_"+id)/*description*/, timeMax);
        }
    }

    public class DailyQuestData : DataGroup, IComparable<DailyQuestData>
    {
        public DailyQuestDefinition baseData { get; private set; }
        private BoolData claimed;
        private IntegerData time;

        public bool Claimed => claimed.Value;
        public int Time => time.Value;
        public int TimeMax => baseData.timeMax;
        public bool CanClaim => Time >= TimeMax;
        public string Description => baseData.GetDescription();

        public DailyQuestData(int _id, DailyQuestDefinition _baseData) : base(_id)
        {
            baseData = _baseData;
            claimed = AddData(new BoolData(0, false));
            time = AddData(new IntegerData(1, 0));
        }

        public void AddTime(int addValue = 1)
        {
            //chưa max thì add thêm
            if (!CanClaim) time.Value += addValue;

            EventDispatcher.Raise(new DailyQuestChangeEvent());
        }

        //clear for new day
        public void Clear()
        {
            claimed.Value = false;
            time.Value = 0;

            EventDispatcher.Raise(new DailyQuestChangeEvent());
        }

        public List<RewardInfo> GetRewards()
        {
            return baseData.GetRewards();
        }

        public List<RewardInfo> ClaimRewards()
        {
            claimed.Value = true;
            GameData.Instance.DailyQuestsGroup.AddPoint();

            EventDispatcher.Raise(new DailyQuestChangeEvent());
            return GetRewards();
        }

        public int CompareTo(DailyQuestData other)
        {
            var claimed = Claimed;
            var otherClaimed = other.Claimed;
            if (claimed && !otherClaimed)
            {
                return 1;
            }
            else if (!claimed && otherClaimed)
            {
                return -1;
            }
            else
            {
                var canClaim = CanClaim;
                var otherCanClaim = other.CanClaim;
                if (canClaim == otherCanClaim)
                {
                    return 0;
                }
                else if (canClaim && !otherCanClaim)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }
    }

    [System.Serializable]
    public class DailyQuestPoint
    {
        public int point;
        public int rewardType;
        public int rewardId;
        public int rewardValue;

        public RewardInfo GetReward()
        {
            return new RewardInfo(rewardType, rewardId, rewardValue);
        }
    }

    public class DailyQuestPointData : DataGroup
    {
        public DailyQuestPoint baseData { get; private set; }
        private BoolData claimed;

        public bool Claimed => claimed.Value;
        public bool CanClaim => GameData.Instance.DailyQuestsGroup.Point >= baseData.point;

        public DailyQuestPointData(int _id, DailyQuestPoint _baseData) : base(_id)
        {
            baseData = _baseData;
            claimed = AddData(new BoolData(0, false));
        }

        //clear for new day
        public void Clear()
        {
            claimed.Value = false;
            EventDispatcher.Raise(new DailyQuestChangeEvent());
        }

        public RewardInfo GetReward()
        {
            return baseData.GetReward();
        }

        public RewardInfo ClaimReward()
        {
            claimed.Value = true;

            EventDispatcher.Raise(new DailyQuestChangeEvent());
            return GetReward();
        }
    }

    [System.Serializable]
    public class DayBonus
    {
        public int day;
        public int rewardType;
        public int rewardId;
        public int rewardValue;

        public RewardInfo GetReward()
        {
            return new RewardInfo(rewardType, rewardId, rewardValue);
        }
    }

    [System.Serializable]
    public class DailyLoginDefinition
    {
        public int day;
        public int rewardType;
        public int rewardId;
        public int rewardValue;
        public int[] vipRewardTypes;
        public int[] vipRewardIds;
        public int[] vipRewardValues;

        public RewardInfo GetFreeReward()
        {
            return new RewardInfo(rewardType, rewardId, rewardValue);
        }

        public List<RewardInfo> GetVipRewards()
        {
            var rewards = new List<RewardInfo>();
            var count = vipRewardTypes.Length;
            for (int i = 0; i < count; i++)
            {
                rewards.Add(new RewardInfo(vipRewardTypes[i], vipRewardIds[i], vipRewardValues[i]));
            }

            return rewards;
        }
    }

    public class DailyLoginData : DataGroup
    {
        public DailyLoginDefinition baseData { get; private set; }
        private BoolData claimedFree;
        private BoolData claimedVip;

        public bool ClaimedFree => claimedFree.Value;
        public bool ClaimedVip => claimedVip.Value;
        public bool CanClaim => GameData.Instance.DailyQuestsGroup.DayCount >= baseData.day;

        public int Day => baseData.day;

        public DailyLoginData(int _id, DailyLoginDefinition _baseData) : base(_id)
        {
            baseData = _baseData;
            claimedFree = AddData(new BoolData(0, false));
            claimedVip = AddData(new BoolData(1, false));
        }

        public RewardInfo GetFreeReward()
        {
            return baseData.GetFreeReward();
        }

        public RewardInfo ClaimFreeReward()
        {
            claimedFree.Value = true;

            EventDispatcher.Raise(new DailyLoginChangeEvent());
            return GetFreeReward();
        }

        public List<RewardInfo> GetVipRewards()
        {
            return baseData.GetVipRewards();
        }

        public List<RewardInfo> ClaimVipRewards()
        {
            claimedVip.Value = true;

            EventDispatcher.Raise(new DailyLoginChangeEvent());
            return GetVipRewards();
        }
    }

    [System.Serializable]
    public class DailyGift
    {
        public int id;
        public int rewardType;
        public int rewardId;
        public int rewardValue;

        public RewardInfo GetReward()
        {
            return new RewardInfo(rewardType, rewardId, rewardValue);
        }
    }

    //==================================

    public class DailyQuestsGroup : DataGroup
    {
        #region Members

        private DataGroup dailyQuestsGroup;
        private DataGroup dailyQuestPointsGroup;
        private IntegerData point;
        private IntegerData dayCount;
        private IntegerData dailyGiftCountInDay;
        private BoolData claimedToday;
        private DataGroup dailyLoginsGroup;
        private BoolData eatNoonToday;
        private BoolData eatDinnerToday;

        private List<DayBonus> daysBonus;
        private List<DailyGift> dailyGifts;

        public int Point => point.Value;
        public int DayCount => dayCount.Value;
        public int DailyGiftCountInDay => dailyGiftCountInDay.Value;
        public bool MaxDailyGiftCountInDay => dailyGiftCountInDay.Value >= 5;
        public bool ClaimedToday => dayCount.Value > 7 || claimedToday.Value; //nếu quá 7 ngày và đã claim
        public bool EatNoonToday => eatNoonToday.Value;
        public bool EatDinnerToday => eatDinnerToday.Value;
        public List<DayBonus> DaysBonus => daysBonus;
        public List<DailyGift> DailyGifts => dailyGifts;

        #endregion

        //=============================================

        #region Public

        public DailyQuestsGroup(int pId) : base(pId)
        {
            dailyQuestsGroup = AddData(new DataGroup(0));
            dailyQuestPointsGroup = AddData(new DataGroup(1));
            point = AddData(new IntegerData(2, 0));
            dayCount = AddData(new IntegerData(3, 0));
            claimedToday = AddData(new BoolData(4, false));
            dailyLoginsGroup = AddData(new DataGroup(5));
            dailyGiftCountInDay = AddData(new IntegerData(6, 0));
            eatNoonToday = AddData(new BoolData(7, false));
            eatDinnerToday = AddData(new BoolData(8, false));
            InitDailyQuests();
            InitDailyQuestPoints();
            Init7DaysBonus();
            InitDailyLogins();
            InitDailyGifts();
        }

        public void NewDay()
        {
            var dailyQuestDatas = GetAllDailyQuestDatas();
            var dailyQuestPointDatas = GetAllDailyQuestPointDatas();
            var count = dailyQuestDatas.Count;
            for (int i = 0; i < count; i++)
            {
                dailyQuestDatas[i].Clear();
            }
            count = dailyQuestPointDatas.Count;
            for (int i = 0; i < count; i++)
            {
                dailyQuestPointDatas[i].Clear();
            }

            point.Value = 0;

            claimedToday.Value = false;
            dayCount.Value++;
            dailyGiftCountInDay.Value = 0;
            eatNoonToday.Value = false;
            eatDinnerToday.Value = false;
        }

        //Daily Quest Point
        public List<DailyQuestPointData> GetAllDailyQuestPointDatas()
        {
            var list = new List<DailyQuestPointData>();
            foreach (DailyQuestPointData item in dailyQuestPointsGroup.Children)
                list.Add(item);
            return list;
        }

        public void AddPoint()
        {
            point.Value++;
        }

        //Daily Quest
        public List<DailyQuestData> GetAllDailyQuestDatas()
        {
            var list = new List<DailyQuestData>();
            foreach (DailyQuestData item in dailyQuestsGroup.Children)
                list.Add(item);
            list.Sort();
            return list;
        }

        private DailyQuestData GetDailyQuestData(int pId)
        {
            return dailyQuestsGroup.GetData<DailyQuestData>(pId);
        }

        public void BuyCoin()
        {
            GetDailyQuestData(IDs.QUEST_BUY_COIN).AddTime();
        }

        public void SpinWheels()
        {
            GetDailyQuestData(IDs.QUEST_SPIN_IN_WHEEL).AddTime();
        }

        public void Discovery()
        {
            GetDailyQuestData(IDs.QUEST_DISCOVERY).AddTime();
        }

        public void DisassembleGear()
        {
            GetDailyQuestData(IDs.QUEST_DISASSEMBLE_GEAR).AddTime();
        }

        public void DisassembleGears(int addValue)
        {
            GetDailyQuestData(IDs.QUEST_DISASSEMBLE_GEAR).AddTime(addValue);
        }

        public void BasicSummon()
        {
            GetDailyQuestData(IDs.QUEST_BASIC_SUMMON).AddTime();
        }

        public void SeniorSummon()
        {
            GetDailyQuestData(IDs.QUEST_SENIOR_SUMMON).AddTime();
        }

        public void FastTravel()
        {
            GetDailyQuestData(IDs.QUEST_FAST_TRAVEL).AddTime();
        }

        public void LevelUpHero()
        {
            GetDailyQuestData(IDs.QUEST_LEVEL_UP_HERO).AddTime();
        }

        public void PlayMission()
        {
            GetDailyQuestData(IDs.QUEST_PLAY_MISSION).AddTime();
        }

        public void BuyAnythingInStore()
        {
            GetDailyQuestData(IDs.QUEST_BUY_ANYTHING_IN_STORE).AddTime();
        }

        public void UpgradeGear()
        {
            GetDailyQuestData(IDs.QUEST_UPGRADE_GEAR).AddTime();
        }

        public void TakeVipBonus()
        {
            GetDailyQuestData(IDs.QUEST_TAKE_VIP_BONUS).AddTime();
        }

        public void Subscrible()
        {
            GetDailyQuestData(IDs.QUEST_TAKE_SUBSCRIBLE_BONUS).AddTime();
        }

        //7 Days Bonus
        public RewardInfo ClaimToDayBonus()
        {
            if (claimedToday.Value)
            {
                return null;
            }
            else
            {
                if (dayCount.Value <= 7)
                {
                    claimedToday.Value = true;
                    EventDispatcher.Raise(new DailyBonusChangeEvent());
                    return daysBonus[dayCount.Value - 1].GetReward();
                }
                else
                {
                    return null;
                }
            }
        }

        //Daily Logins
        public List<DailyLoginData> GetAllDailyLoginDatas()
        {
            var list = new List<DailyLoginData>();
            foreach (DailyLoginData item in dailyLoginsGroup.Children)
                list.Add(item);
            return list;
        }

        //Daily gift
        public void DailyGiftClaim()
        {
            dailyGiftCountInDay.Value++;
            EventDispatcher.Raise(new DailyGiftsChangeEvent());
        }

        //noti
        public bool IsAnyQuestRemain()
        {
            var list = GetAllDailyQuestDatas();
            for (var index = 0; index < list.Count; index++)
            {
                var dailyQuestData = list[index];
                if (!dailyQuestData.Claimed) return true;
            }

            return false;
        }

        private bool IsAnyQuestCanClaimButNotClaimed()
        {
            var list = GetAllDailyQuestDatas();
            for (var index = 0; index < list.Count; index++)
            {
                var dailyQuestData = list[index];
                if (dailyQuestData.CanClaim && !dailyQuestData.Claimed) return true;
            }

            return false;
        }

        private bool IsAnyQuestPointCanClaimButNotClaimed()
        {
            var list = GetAllDailyQuestPointDatas();
            for (var index = 0; index < list.Count; index++)
            {
                var dailyQuestDataPointData = list[index];
                if (dailyQuestDataPointData.CanClaim && !dailyQuestDataPointData.Claimed) return true;
            }

            return false;
        }

        public bool CheckDailyQuestNoti()
        {
            return IsAnyQuestCanClaimButNotClaimed() || IsAnyQuestPointCanClaimButNotClaimed();
        }

        private bool IsAnyDailyLoginCanClaimButNotClaimed()
        {
            var list = GetAllDailyLoginDatas();
            var isVip = /*GameData.Instance.UserGroup.VipLevel > 0 ||*/ !GameData.Instance.StoreGroup.PremiumPass.CanBuy;
            for (var index = 0; index < list.Count; index++)
            {
                var dailyLoginData = list[index];
                if (dailyLoginData.CanClaim && (!dailyLoginData.ClaimedFree || (!dailyLoginData.ClaimedVip && isVip))) return true;
            }

            return false;
        }

        public bool CheckDailyLoginNoti()
        {
            return IsAnyDailyLoginCanClaimButNotClaimed();
        }

        //noon
        public void EatNoon()
        {
            eatNoonToday.Value = true;
        }

        public void EatDinner()
        {
            eatDinnerToday.Value = true;
        }

        #endregion

        //==============================================

        #region Private

        private void InitDailyQuests()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/DailyQuest");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<DailyQuestDefinition>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    var data = new DailyQuestData(item.id, item);
                    dailyQuestsGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        private void InitDailyQuestPoints()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/DailyQuestPoint");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<DailyQuestPoint>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    var data = new DailyQuestPointData(item.point, item);
                    dailyQuestPointsGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        private void Init7DaysBonus()
        {
            var dataContent = GameData.GetTextContent("Data/7DaysBonus");
            daysBonus = JsonHelper.GetJsonList<DayBonus>(dataContent);
        }

        private void InitDailyLogins()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/DailyLogin");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<DailyLoginDefinition>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    var data = new DailyLoginData(item.day, item);
                    dailyLoginsGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        private void InitDailyGifts()
        {
            var dataContent = GameData.GetTextContent("Data/DailyGift");
            dailyGifts = JsonHelper.GetJsonList<DailyGift>(dataContent);
        }

        #endregion
    }
}