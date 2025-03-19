using System;
using System.Collections.Generic;
using System.Security.Claims;
using Sirenix.Serialization;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Debug = UnityEngine.Debug;

namespace FoodZombie
{
    [System.Serializable]
    public class BaseDefinition : IComparable<BaseDefinition>
    {
        public int level;
        public int hp;
        public int slotLimit;
        public int trapUnlock;
        public int trapCount;
        public int levelUpCost;
        public int userLevelRequest;

        public int CompareTo(BaseDefinition other)
        {
            return level.CompareTo(other.level);
        }

        // public Sprite GetIcon()
        // {
        //     return AssetsCollection.instance.gearIcon.GetAsset(id - 1);
        // }
    }

    [System.Serializable]
    public class TrapDefinition
    {
        public int id;
        public string name;
        public string logName;
        public string AI;
        public float hp;
        public float damage;
        public float duration;
        public float hpRegen;

    }

    [System.Serializable]
    public class TrapLevelUpStat
    {
        public int id;
        public float hp;
        public float damage;
        public float duration;
        public float hpRegen;
        public int levelUpCoin;
        public int levelUpGem;
    }

    [System.Serializable]
    public class TrapData : ConsumableItemData
    {
        public TrapDefinition baseData { get; private set; }
        public TrapLevelUpStat levelUpStat;
        private BoolData unlocked;
        private IntegerData level;

        public bool Unlocked => unlocked.Value;
        public int Level => level.Value;

        #region Info

        public string Name => baseData.name;
        public string NameLocal=> Localization.Get("ITEM_TRAP_" + baseData.id);
        public string LogName => baseData.logName;
        public string Description => string.Format(Localization.Get("TRAP_DESC_"+baseData.id)/*baseData.AI*/, HP.ToString("0"), Damage.ToString("0"), Duration.ToString("0"), HPRegen.ToString("0"));
        public int LevelUpCoin => (int)(levelUpStat.levelUpCoin * Mathf.Pow(1.1f, level.Value - 1));
        public int LevelUpGem => levelUpStat.levelUpGem;
        public int RequireLevelBase => GameData.Instance.BaseGroup.GetBaseByTrapId(Id).level;

        //Fix HP upgrade trap barrier 
        float GetStatHP(int _lv) { return 3000 + 7000 * (Mathf.Pow(1.05f, _lv - 1) - 1); }
        public float HP => GetStatHP(level.Value); /*ConfigStats.GetStat(baseData.hp, levelUpStat.hp, level.Value);*/
        public float HPnextLV => GetStatHP(level.Value+1);/*ConfigStats.GetStat(baseData.hp, levelUpStat.hp, level.Value + 1);*/
        public float HPRegen => ConfigStats.GetStat(baseData.hpRegen, levelUpStat.hpRegen, level.Value);

        public float HPRegenNext => ConfigStats.GetStat(baseData.hpRegen, levelUpStat.hpRegen, level.Value + 1);
        public float Damage => ConfigStats.GetStat(baseData.damage, levelUpStat.damage, level.Value);
        public float DamagenextLV => ConfigStats.GetStat(baseData.damage, levelUpStat.damage, level.Value + 1);
        public float Duration => ConfigStats.GetStat(baseData.duration, levelUpStat.duration, level.Value);

        #endregion

        public TrapData(int _id, TrapDefinition _baseData, TrapLevelUpStat _levelUpStat, int _stockNumber = 0) : base(_id, _stockNumber)
        {
            baseData = _baseData;
            levelUpStat = _levelUpStat;
            unlocked = AddData(new BoolData(2, false));
            level = AddData(new IntegerData(3, 1));
        }

        public void Unlock()
        {
            unlocked.Value = true;
        }

        public void LevelUp()
        {
            level.Value++;
            //Daily Quest và Achievement
            GameData.Instance.CampaignsGroup.AddUpgradeTrapCount();
        }

        public bool CanLevelUp()
        {
            if (!Unlocked) return false;

            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            return currenciesGroup.CanPay(IDs.CURRENCY_COIN, LevelUpCoin);
        }

        public void SetLevel(int _level)
        {
            level.Value = _level;
        }

        public Sprite GetIcon()
        {
            return AssetsCollection.instance.traps.GetAsset(Id - 1);
        }

        public bool CheckNoti()
        {
            return CanLevelUp();
        }

        public int GetLevelUpCoinByLevel(int level)
        {
            return (int)(levelUpStat.levelUpCoin * Mathf.Pow(1.1f, level - 1));
        }
    }

    public class BaseGroup : DataGroup
    {
        #region Members

        private List<BaseDefinition> baseDefinitions;
        private DataGroup trapsGroup;
        private IntegerData level;

        public int Level => level.Value;
        public List<BaseDefinition> BaseDefinitions => baseDefinitions;

        #endregion

        //=============================================

        #region Public

        public BaseGroup(int pId) : base(pId)
        {
            //Declare sub groups which contain units data
            trapsGroup = AddData(new DataGroup(0));
            level = AddData(new IntegerData(1, 1));

            InitTrapsGroup();
        }

        public override void PostLoad()
        {
            base.PostLoad();

            CheckUnlockTraps();
        }

        public BaseDefinition GetCurrentBase()
        {
            return baseDefinitions[level.Value - 1];
        }

        public BaseDefinition GetNextBase()
        {
            return baseDefinitions[level.Value];
        }

        public BaseDefinition GetBaseByTrapId(int trapId)
        {
            var count = baseDefinitions.Count;
            for (int i = 0; i < count; i++)
            {
                var baseDefinition = baseDefinitions[i];
                if (baseDefinition.trapUnlock == trapId) return baseDefinition;
            }
            return null;
        }

        public TrapData GetTrapData(int pId)
        {
            return trapsGroup.GetData<TrapData>(pId);
        }

        public TrapData GetRandomTrap()
        {
            return trapsGroup.GetRandomData<TrapData>();
        }

        public List<TrapData> GetAllTrapDatas()
        {
            var list = new List<TrapData>();
            foreach (TrapData item in trapsGroup.Children)
                list.Add(item);
            return list;
        }

        private void CheckUnlockTraps()
        {
            for (int i = 0; i < level.Value; i++)
            {
                var trapData = GetTrapData(baseDefinitions[i].trapUnlock);
                trapData.Unlock();
            }
        }

        public void UnlockTrap(int pId)
        {
            var unit = GetTrapData(pId);
            if (unit != null)
                unit.Unlock();
        }

        public void UnlockAllTraps()
        {
            foreach (EnemyData unit in trapsGroup.Children)
            {
                unit.Unlock();
            }
        }

        public bool IsMaxLevel()
        {
            //level từ 1 -> 10 và count = 10
            return level.Value >= baseDefinitions.Count;
        }

        public int GetLevelUpCost()
        {
            if (!IsMaxLevel()) return baseDefinitions[level.Value].levelUpCost;
            else return 0;
        }

        public int GetUserLevelRequest()
        {
            if (!IsMaxLevel()) return baseDefinitions[level.Value].userLevelRequest;
            else return 0;
        }

        public bool CanLevelUp()
        {
            if (IsMaxLevel()) return false;

            var userLevel = GameData.Instance.UserGroup.Level;
            var userLevelRequest = GetUserLevelRequest();
            var currenciesGroup = GameData.Instance.CurrenciesGroup;
            return userLevel >= userLevelRequest && currenciesGroup.CanPay(IDs.CURRENCY_COIN, GetLevelUpCost());
        }

        public void LevelUp()
        {
            if (!IsMaxLevel())
            {
                //  var oldSlotLimit = GetCurrentBase().slotLimit;
                level.Value++;

                // if (oldSlotLimit != GetCurrentBase().slotLimit)
                //  {
                //      EventDispatcher.Raise(new FormationChangeEvent());
                // }
            }
            CheckUnlockTraps();

            EventDispatcher.Raise(new BaseLevelUpEvent());
        }

        public TrapData AddTrapItem(int _id, int _value)
        {
            var trap = GetTrapData(_id);
            if (trap != null)
                trap.AddToStock(_value);
            return trap;
        }

        public TrapData AddTrapItemRandomly(int pValue)
        {
            var trap = GetRandomTrap();
            if (trap != null)
            {
                trap.AddToStock(pValue);
                return trap;
            }
            return trap;
        }

        //noti
        public bool CheckNoti()
        {
            return CanLevelUp();
        }

        #endregion

        //==============================================

        #region Private

        private void InitTrapsGroup()
        {
            //base
            var dataContent = GameData.GetTextContent("Data/Base");
            baseDefinitions = JsonHelper.GetJsonList<BaseDefinition>(dataContent);
            //trap
            dataContent = GameData.GetTextContent("Data/Trap");
            var trapDefinitions = JsonHelper.GetJsonList<TrapDefinition>(dataContent);
            dataContent = GameData.GetTextContent("Data/TrapLevelUpStat");
            var trapLevelUpStatDefinitions = JsonHelper.GetJsonList<TrapLevelUpStat>(dataContent);
            if (trapDefinitions != null && trapLevelUpStatDefinitions != null)
            {
                var count = baseDefinitions.Count;
                for (int i = 0; i < count; i++)
                {
                    var baseDefinition = baseDefinitions[i];

                    //vì mỗi base unlock các trap khác nhau không theo thứ tự, nên phải for list trap và trap level up để tìm
                    TrapDefinition trapDefinition = null;
                    var countTD = trapDefinitions.Count;
                    for (int j = 0; j < countTD; j++)
                    {
                        var temp = trapDefinitions[j];
                        if (temp.id == baseDefinition.trapUnlock)
                        {
                            trapDefinition = temp;
                            break;
                        }
                    }
                    TrapLevelUpStat trapLevelUpStatDefinition = null;
                    var countLU = trapLevelUpStatDefinitions.Count;
                    for (int j = 0; j < countLU; j++)
                    {
                        var temp = trapLevelUpStatDefinitions[j];
                        if (temp.id == baseDefinition.trapUnlock)
                        {
                            trapLevelUpStatDefinition = temp;
                            break;
                        }
                    }
                    //vì trong bảng base có 2 cái unlock canon nên phải kiểm tra có data T9 hay chưa
                    var id = trapDefinition.id;
                    if (trapsGroup.GetData<TrapData>(id) == null)
                    {
                        var stockNumber = 0;
                        if (id == IDs.ITEM_TRAP_BARRIER) stockNumber = 5;
                        else if (id == IDs.ITEM_TRAP_MINE) stockNumber = 3;
                        else if (id == IDs.ITEM_TRAP_CALL) stockNumber = 1;
                        else if (id == IDs.ITEM_TRAP_TRAP) stockNumber = 2;
                        else if (id == IDs.ITEM_TRAP_FIRST_AIR_KIT) stockNumber = 1;
                        var data = new TrapData(id, trapDefinition, trapLevelUpStatDefinition, stockNumber);
                        trapsGroup.AddData(data);
                    }
                }
            }
        }

        #endregion
    }
}
