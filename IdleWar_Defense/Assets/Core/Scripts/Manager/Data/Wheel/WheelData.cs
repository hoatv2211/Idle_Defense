
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
    public class SlotRewardDefinition
    {
        public int no;
        public int group;
        public int rewardType;
        public int rewardId;
        public int rewardValue;

        public RewardInfo GetReward()
        {
            return new RewardInfo(rewardType, rewardId, rewardValue);
        }
    }

    [System.Serializable]
    public class SlotDefinition
    {
        public int slot;
        public int rank;
        public int group;
        public float dropRate;
        public float dropChange;
        public float dropMax;
    }
    
    public class SlotRewardData : DataGroup
    {
        private IntegerData rewardType;
        private IntegerData rewardId;
        private IntegerData rewardValue;

        private BoolData firstClaimed;
        private int rank;

        public bool CanClaim
        {
            get
            {
                //nếu đã từng quay vào slot tặng đồ vàng trong phiên thì next
                if (rank == IDs.SLOT_TYPE_LEGENDARY)
                {
                    return !firstClaimed.Value;
                }
                else
                {
                    return true;
                }
            }
        }

        public SlotRewardData(int _id, int _rank) : base(_id)
        {
            rewardType = AddData(new IntegerData(0));
            rewardId = AddData(new IntegerData(1));
            rewardValue = AddData(new IntegerData(2));
            firstClaimed = AddData(new BoolData(3, false));
            rank = _rank;
        }

        public void Init(RewardInfo _rewardInfo)
        {
            rewardType.Value = _rewardInfo.Type;
            rewardId.Value = _rewardInfo.Id;
            rewardValue.Value = _rewardInfo.Value;
            firstClaimed.Value = false;
        }

        public RewardInfo GetReward()
        {
            return new RewardInfo(rewardType.Value, rewardId.Value, rewardValue.Value);
        }

        public RewardInfo ClaimReward()
        {
            firstClaimed.Value = true;
            return GetReward();
        }
    }

    //==================================

    public class WheelData : DataGroup
    {
        #region Members
        
        private const int TIME_GENERAL_REFRESH = 12 * 60 * 60; //12 hours
        private const int TIME_ROYALE_REFRESH = 12 * 60 * 60; //12 hours

        private List<SlotDefinition> generalSlots;
        private List<SlotDefinition> royaleSlots;
        private List<List<SlotRewardDefinition>> slotRewardDefinitions;
        
        private IntegerData spinCount;
        private TimerTask timerGeneralRefresh;
        private TimerTask timerRoyaleRefresh;
        private IntegerData spinCountByAdsInDay;

        //lưu tạm bằng cách tù này vậy
        // private ListData<string> generalRewardInfos;
        // private ListData<string> royaleRewardInfos;
        private IntegerData countGeneralDraw;
        private IntegerData countRoyaleDraw;
        // private BoolData hasLegendaryGeneralDraw;
        // private BoolData hasLegendaryRoyaleDraw;
        private BoolData freeGeneralRefresh;
        private BoolData freeRoyaleRefresh;
        private DataGroup generalSlotRewardsGroup;
        private DataGroup royaleSlotRewardsGroup;

        public int SpinCount => spinCount.Value;
        public bool FreeGeneralRefresh => freeGeneralRefresh.Value;
        public bool FreeRoyaleRefresh => freeRoyaleRefresh.Value;
        public int SpinCountByAdsInDay => spinCountByAdsInDay.Value;

        public List<SlotDefinition> GeneralSlots => generalSlots;
        public List<SlotDefinition> RoyaleSlots => royaleSlots;
        
        #endregion

        //=============================================

        #region Public

        public WheelData(int pId) : base(pId)
        {
            spinCount = AddData(new IntegerData(0, -1));
            timerGeneralRefresh = AddData(new TimerTask(1, false));
            timerRoyaleRefresh = AddData(new TimerTask(2, false));
            // generalRewardInfos = AddData(new ListData<string>(3, new List<string>()));
            // royaleRewardInfos = AddData(new ListData<string>(4, new List<string>()));
            countGeneralDraw = AddData(new IntegerData(5, 0));
            countRoyaleDraw = AddData(new IntegerData(6, 0));
            // hasLegendaryGeneralDraw = AddData(new BoolData(7, false));
            // hasLegendaryRoyaleDraw = AddData(new BoolData(8, false));
            freeGeneralRefresh = AddData(new BoolData(9, false));
            freeRoyaleRefresh = AddData(new BoolData(10, false));
            generalSlotRewardsGroup = AddData(new DataGroup(11));
            royaleSlotRewardsGroup = AddData(new DataGroup(12));
            spinCountByAdsInDay = AddData(new IntegerData(13, 0));
            
            InitGeneralSlots();
            InitRoyaleSlots();
            InitRankRewards();
            timerGeneralRefresh.SetOnComplete(TimeGeneralRefreshEnd);
            timerRoyaleRefresh.SetOnComplete(TimeRoyaleRefreshEnd);
        }

        public override void PostLoad()
        {
            base.PostLoad();

            //load xong data mà ko có list reward sẵn thì random ra
            if (spinCount.Value < 0)
            {
                SetRewards(generalSlotRewardsGroup, generalSlots);
                SetRewards(royaleSlotRewardsGroup, royaleSlots);
                spinCount.Value = 0;
            }
        }
        
        public void NewDay()
        {
            spinCountByAdsInDay.Value = 0;
        }

        private void TimeGeneralRefreshEnd(TimerTask timer, long remain)
        {
            if (remain <= 0)
            {
                freeGeneralRefresh.Value = true;
                EventDispatcher.Raise(new HasFreeGeneralRefresh());
            }
        }
        
        private void TimeRoyaleRefreshEnd(TimerTask timer, long remain)
        {
            if (remain <= 0)
            {
                freeRoyaleRefresh.Value = true;
                EventDispatcher.Raise(new HasFreeRoyaleRefresh());
            }
        }
        
        public void CheckRefreshTime()
        {
            if (!timerGeneralRefresh.IsRunning && !freeGeneralRefresh.Value)
            {
                RefreshGeneralReward();
            }
            
            if (!timerRoyaleRefresh.IsRunning && !freeRoyaleRefresh.Value)
            {
                RefreshRoyaleReward();
            }
        }

        public void RefreshGeneralReward()
        {
            //reset count
            countGeneralDraw.Value = 0;
            // hasLegendaryGeneralDraw.Value = false;
            freeGeneralRefresh.Value = false;
            
            SetRewards(generalSlotRewardsGroup, generalSlots);
            //trường hợp trả coin refresh nên phải kiểm tra
            if (!timerGeneralRefresh.IsRunning) timerGeneralRefresh.Start(TIME_GENERAL_REFRESH);
        }
        
        public void RefreshRoyaleReward()
        {
            //reset count
            countRoyaleDraw.Value = 0;
            // hasLegendaryRoyaleDraw.Value = false;
            freeRoyaleRefresh.Value = false;
            
            SetRewards(royaleSlotRewardsGroup, royaleSlots);
            //trường hợp trả coin refresh nên phải kiểm tra
            if (!timerRoyaleRefresh.IsRunning) timerRoyaleRefresh.Start(TIME_ROYALE_REFRESH);
        }

        private void SetRewards(DataGroup slotRewardsGroup, List<SlotDefinition> slots)
        {
            //clone lại list data để remove tránh 2 slot giống nhau
            var slotRewardDefinitionsClone = new List<List<SlotRewardDefinition>>();
            var count = slotRewardDefinitions.Count;
            for (int i = 0; i < count; i++)
            {
                slotRewardDefinitionsClone.Add(new List<SlotRewardDefinition>(slotRewardDefinitions[i]));
            }
            
            count = slots.Count;
            RewardInfo rewardInfo;
            //list all slotDefinition
            int countSetGem = 0;
            for (int i = 0; i < count; i++)
            {
                //nếu chỉ là i9 tặng gem
                var slotDefinition = slots[i];
                if (slotDefinition.rank == IDs.SLOT_TYPE_GEM)
                {
                    if(countSetGem == 0) rewardInfo = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_GEM, 200);
                    else rewardInfo = new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_GEM, 50);
                    countSetGem++;
                }
                else
                {
                    //không thì chọn ra list SlotRewardDefinition tương ứng
                    var rankRewards = slotRewardDefinitionsClone[slotDefinition.rank - 1];
                    SlotRewardDefinition slotReward;
                    //group == 0 thì ko phải lọc kiểu đồ nào
                    if (slotDefinition.group == 0)
                    {
                        slotReward = rankRewards[Config.EasyRandom(0, rankRewards.Count)];
                    }
                    else
                    {
                        var slotRewards = new List<SlotRewardDefinition>();
                        var countReward = rankRewards.Count;
                        for (int j = 0; j < countReward; j++)
                        {
                            slotReward = rankRewards[j];
                            if (slotDefinition.group == slotReward.group) slotRewards.Add(slotReward);
                        }

                        slotReward = slotRewards[Config.EasyRandom(0, slotRewards.Count)];
                    }
                    rewardInfo = slotReward.GetReward();
                    rankRewards.Remove(slotReward);//remove khỏi list clone
                }

                var slotRewardData = slotRewardsGroup.GetData<SlotRewardData>(slotDefinition.slot);
                slotRewardData.Init(rewardInfo);
            }
        }

        public string GetTimeGeneralRefresh()
        {
            var s = timerGeneralRefresh.RemainSeconds;
            return TimeHelper.FormatHHMMss(s, true);
        }
        
        public string GetTimeRoyaleRefresh()
        {
            var s = timerRoyaleRefresh.RemainSeconds;
            return TimeHelper.FormatHHMMss(s, true);
        }

        public List<SlotRewardData> GetGeneralSlotRewards()
        {
            var list = new List<SlotRewardData>();
            foreach (SlotRewardData item in generalSlotRewardsGroup.Children)
                list.Add(item);
            return list;
        }
        
        private SlotRewardData GetGeneralSlotReward(int pId)
        {
            return generalSlotRewardsGroup.GetData<SlotRewardData>(pId);
        }
        
        public List<SlotRewardData> GetRoyaleSlotRewards()
        {
            var list = new List<SlotRewardData>();
            foreach (SlotRewardData item in royaleSlotRewardsGroup.Children)
                list.Add(item);
            return list;
        }
        
        private SlotRewardData GetRoyaleSlotReward(int pId)
        {
            return royaleSlotRewardsGroup.GetData<SlotRewardData>(pId);
        }
        
        public void GeneralDraw(ref int index, ref RewardInfo rewardInfo)
        {
            //index vị trí slot quay vào
            var countDraw = countGeneralDraw.Value;
            var count = generalSlots.Count;

            var slots = new int[count];
            var chances = new float[count];
            for (int i = 0; i < count; i++)
            {
                var generalSlot = generalSlots[i];
                slots[i] = generalSlot.slot;
                var chance = generalSlot.dropRate + generalSlot.dropChange * countDraw;
                if (GetGeneralSlotReward(generalSlot.slot).CanClaim)
                {
                    if (generalSlot.dropChange < 0f)
                    {
                        if (chance < generalSlot.dropMax) chance = generalSlot.dropMax;
                    }
                    else
                    {
                        if (chance > generalSlot.dropMax) chance = generalSlot.dropMax;
                    }
                }
                else
                {
                    chance = 0f;
                }
                chances[i] = chance;
            }
            
            int choice = LogicAPI.CalcRandomWithChances(chances);
            var slot = slots[choice];
            index = choice;
            var slotRewardData = GetGeneralSlotReward(slot);
            rewardInfo = slotRewardData.ClaimReward();

            spinCount.Value++;
            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.SpinWheels();
            GameData.Instance.AchievementsGroup.SpinWheels();
        }
        
        public void RoyaleDraw(ref int index, ref RewardInfo rewardInfo)
        {
            //index vị trí slot quay vào
            var countDraw = countRoyaleDraw.Value;
            var count = royaleSlots.Count;

            var slots = new int[count];
            var chances = new float[count];
            for (int i = 0; i < count; i++)
            {
                var royaleSlot = royaleSlots[i];
                slots[i] = royaleSlot.slot;
                var chance = royaleSlot.dropRate + royaleSlot.dropChange * countDraw;
                if (GetRoyaleSlotReward(royaleSlot.slot).CanClaim)
                {
                    if (royaleSlot.dropChange < 0f)
                    {
                        if (chance < royaleSlot.dropMax) chance = royaleSlot.dropMax;
                    }
                    else
                    {
                        if (chance > royaleSlot.dropMax) chance = royaleSlot.dropMax;
                    }
                }
                else
                {
                    chance = 0;
                }
                chances[i] = chance;
            }
            
            int choice = LogicAPI.CalcRandomWithChances(chances);
            var slot = slots[choice];
            index = choice;
            var slotRewardData = GetRoyaleSlotReward(slot);
            rewardInfo = slotRewardData.ClaimReward();
            
            spinCount.Value++;
            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.SpinWheels();
            GameData.Instance.AchievementsGroup.SpinWheels();
        }

        public void AddSpinCountByAdsInDay()
        {
            spinCountByAdsInDay.Value++;
        }

        #endregion

        //==============================================

        #region Private

        private void InitGeneralSlots()
        {
            var dataContent = GameData.GetTextContent("Data/GeneralSlot");
            var slots = JsonHelper.GetJsonList<SlotDefinition>(dataContent);

            //trộn data để sau hiển thị nó sole nhau
            generalSlots = new List<SlotDefinition>();
            var count = slots.Count / 2;
            for (int i = 0; i < count; i++)
            {
                generalSlots.Add(slots[i]);
                generalSlots.Add(slots[i + count]);
            }

            count = generalSlots.Count;
            for (int i = 0; i < count; i++)
            {
                var item = generalSlots[i];
                var data = new SlotRewardData(item.slot, item.rank);
                generalSlotRewardsGroup.AddData(data);
            }
        }

        private void InitRoyaleSlots()
        {
            var dataContent = GameData.GetTextContent("Data/RoyaleSlot");
            var slots = JsonHelper.GetJsonList<SlotDefinition>(dataContent);

            //trộn data để sau hiển thị nó sole nhau
            royaleSlots = new List<SlotDefinition>();
            var count = slots.Count / 2;
            for (int i = 0; i < count; i++)
            {
                royaleSlots.Add(slots[i]);
                royaleSlots.Add(slots[i + count]);
            }
            
            count = royaleSlots.Count;
            for (int i = 0; i < count; i++)
            {
                var item = royaleSlots[i];
                var data = new SlotRewardData(item.slot, item.rank);
                royaleSlotRewardsGroup.AddData(data);
            }
        }
        
        private void InitRankRewards()
        {
            //list các list reward từ common đến legendary, sau dùng để generate ra mấy slot
            slotRewardDefinitions = new List<List<SlotRewardDefinition>>();
            var dataContent = GameData.GetTextContent("Data/WheelsCommonAndUncommon");
            slotRewardDefinitions.Add(JsonHelper.GetJsonList<SlotRewardDefinition>(dataContent));
            
            dataContent = GameData.GetTextContent("Data/WheelsRare");
            slotRewardDefinitions.Add(JsonHelper.GetJsonList<SlotRewardDefinition>(dataContent));
            
            dataContent = GameData.GetTextContent("Data/WheelsMythic");
            slotRewardDefinitions.Add(JsonHelper.GetJsonList<SlotRewardDefinition>(dataContent));

            dataContent = GameData.GetTextContent("Data/WheelsLegendary");
            slotRewardDefinitions.Add(JsonHelper.GetJsonList<SlotRewardDefinition>(dataContent));
        }

        #endregion
    }
}