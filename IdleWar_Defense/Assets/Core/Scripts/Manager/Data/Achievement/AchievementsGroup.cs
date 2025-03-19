
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Utilities.Service.RFirebase;
using Zirpl.CalcEngine;
using Random = System.Random;

namespace FoodZombie
{
    [System.Serializable]
    public class AchievementDefinition
    {
        public int id;
        public int[] rewardTypes;
        public int[] rewardIds;
        public int[] rewardValues;
        public string description;
        public int[] values;
        public string valueFormula;

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

        public int GetValueMax(int level)
        {
            var count = values.Length;
            if (level + 1 > count) //level bắt đầu từ 0
            {
                var lastValue = values[values.Length - 1];
                var calculator = new CalculationEngine();
                calculator.Variables["level"] = level + 1;
                calculator.Variables["lastValue"] = lastValue;

                return Convert.ToInt32(calculator.Evaluate(valueFormula));
            }
            else
            {
                return values[level];
            }
        }

        public string GetDescription(int level) //Hot fix for CHALLENGE_COMPLETE_MAP
        {
            string desc = Localization.Get("CHALLENGE_DESC_" + id);
            if (id == IDs.CHALLENGE_COMPLETE_MAP)
            {
                //nếu level + 1 bằng map hiện tại thì là chưa xong Challenge
                var temp = level + 1;
                var mapId = GameData.Instance.MissionsGroup.GetCurrentMapInfo().id;
                if(temp == mapId) return string.Format(desc, mapId);
                else return string.Format(desc, temp);
            }
            return string.Format(desc, GetValueMax(level));
        }
    }
    
    public class AchievementData : DataGroup, IComparable<AchievementData>
    {
        public AchievementDefinition baseData { get; private set; }
        private IntegerData level; //level bắt đầu từ 0 vì không hiển thị cho ai thấy cả, tiện thì mình làm
        private IntegerData value; //làm được bao nhiêu mức cửa challenge rồi
        
        public int Value => value.Value;
        public int ValueMax => baseData.GetValueMax(level.Value);
        public bool CanClaim => Value >= ValueMax;
        public string Description => baseData.GetDescription(level.Value);
        
        public AchievementData(int _id, AchievementDefinition _baseData) : base(_id)
        {
            baseData = _baseData;
            level = AddData(new IntegerData(0, 0));
            value = AddData(new IntegerData(1, 0));
        }

        public void AddValue(int addValue = 1)
        {
            value.Value += addValue;
            
            EventDispatcher.Raise(new AchievementChangeEvent());
        }

        //clear for new day
        public void ClaimAndNextLevel()
        {
            // value.Value -= ValueMax;
            level.Value++;
            value.Value = 0;
        }

        public List<RewardInfo> GetRewards()
        {
            return baseData.GetRewards();
        }

        public List<RewardInfo> ClaimRewards()
        {
            ClaimAndNextLevel();
            
            EventDispatcher.Raise(new AchievementChangeEvent());
            return GetRewards();
        }
        
        public int CompareTo(AchievementData other)
        {
            var canClaim = CanClaim;
            var otherCanClaim = other.CanClaim;
            if (canClaim && !otherCanClaim)
            {
                return -1;
            }
            else if (!canClaim && otherCanClaim)
            {
                return 1;
            }
            else
            {
                return -(Value * 1000 / ValueMax).CompareTo(other.Value * 1000 / other.ValueMax);
            }
        }
    }

    //==================================

    public class AchievementsGroup : DataGroup
    {
        #region Members

        private DataGroup achievementsGroup;

        #endregion

        //=============================================

        #region Public

        public AchievementsGroup(int pId) : base(pId)
        {
            achievementsGroup = AddData(new DataGroup(0));
            
            InitAchievements();
        }

        public List<AchievementData> GetAllAchievementDatas()
        {
            var list = new List<AchievementData>();
            foreach (AchievementData item in achievementsGroup.Children)
                list.Add(item);
            list.Sort();
            return list;
        }
        
        private AchievementData GetAchievementData(int pId)
        {
            return achievementsGroup.GetData<AchievementData>(pId);
        }
        
        public void CompleteMap()
        {
            GetAchievementData(IDs.CHALLENGE_COMPLETE_MAP).AddValue();
        }
        
        public void SeniorSummon()
        {
            GetAchievementData(IDs.CHALLENGE_SUMMON_ABC).AddValue();
        }
        
        public void SummonRankS()
        {
            GetAchievementData(IDs.CHALLENGE_SUMMON_S).AddValue();
        }
        
        public void SummonRankSS()
        {
            GetAchievementData(IDs.CHALLENGE_SUMMON_SS).AddValue();
        }
        
        public void KillBoss()
        {
            GetAchievementData(IDs.CHALLENGE_KILL_BOSS).AddValue();
        }
        
        public void HaveGearRankS()
        {
            GetAchievementData(IDs.CHALLENGE_GEAR_S).AddValue();
        }
        
        public void HaveGearRankSS()
        {
            GetAchievementData(IDs.CHALLENGE_GEAR_SS).AddValue();
        }
        
        public void SpinWheels()
        {
            GetAchievementData(IDs.CHALLENGE_WHEELS_COUNT).AddValue();
        }

        //noti
        private bool IsAnyAchievementCanClaim()
        {
            var list = GetAllAchievementDatas();
            for (var index = 0; index < list.Count; index++)
            {
                var achievementData = list[index];
                if (achievementData.CanClaim) return true;
            }

            return false;
        }

        public bool CheckNoti()
        {
            return IsAnyAchievementCanClaim();
        }
        
        #endregion

        //==============================================

        #region Private

        private void InitAchievements()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/Achievement");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<AchievementDefinition>(dataContent);
            if (collection != null)
            {
                // collection.Sort();
                foreach (var item in collection)
                {
                    //Declare unit data, then push it into a data group
                    var data = new AchievementData(item.id, item);
                    achievementsGroup.AddData(data);
                }
            }

            //Debug.Log(collection);
        }

        #endregion
    }
}