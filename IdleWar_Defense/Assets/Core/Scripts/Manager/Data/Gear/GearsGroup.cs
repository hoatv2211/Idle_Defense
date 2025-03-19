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
    public class GearDefinition : IComparable<GearDefinition>
    {
        public int id;
        public string name;
        public int slot;
        public int rank;
        public string AI;
        public int starBase;
        public int starMax;
        public float damage;
        public float hp;
        public float armor;
        public float critRate;
        public float accuracy;
        public float bonusHp;
        public float bonusHpRegen;
        public float bonusDamage;
        public float bonusMovement;
        public float bonusArmor;
        public float bonusAttackSpeed;
        public float bonusAccuracy;
        public float bonusDodge;
        public float bonusCritRate;
        public float bonusCritDamage;
        public float bonusAttackRange;
        public int coinCost;
        public int gemCost;

        public int CompareTo(GearDefinition other)
        {
            return id.CompareTo(other.id);
        }

        public Sprite GetIcon()
        {
            return AssetsCollection.instance.gearIcon.GetAsset(id - 1);
        }

        public Sprite GetRankIcon()
        {
            return AssetsCollection.instance.GetRankIcon(rank);
        }

        public string GetRankName()
        {
            switch (rank)
            {
                case IDs.RANK_C:
                    return "C";
                case IDs.RANK_B:
                    return "B";
                case IDs.RANK_A:
                    return "A";
                case IDs.RANK_S:
                    return "S";
                case IDs.RANK_SS:
                    return "SS";
            }

            return "";
        }

        public string GetSlotName()
        {
            switch (slot)
            {
                case IDs.SLOT_ARMOR:
                    return "Armor";
                case IDs.SLOT_HELMET:
                    return "Helmet";
                case IDs.SLOT_WEAPON:
                    return "Weapon";
                case IDs.SLOT_ACCESSORY:
                    return "Accessory";
            }

            return "";
        }

        public string GetDescription()
        {
            string s = "";
            switch (rank)
            {
                case IDs.RANK_C:
                    s += "<color=#6277A8>";
                    break;
                case IDs.RANK_B:
                    s += "<color=#3EA43B>";
                    break;
                case IDs.RANK_A:
                    s += "<color=#1B6DB3>";
                    break;
                case IDs.RANK_S:
                    s += "<color=#8543D0>";
                    break;
                case IDs.RANK_SS:
                    s += "<color=#E4BE2D>";
                    break;
            }

            return s + AI + "</color>";
        }
    }

    [System.Serializable]
    public class GearStarUpStat
    {
        public int id;
        public int damage;
        public int hp;
        public int armor;
        public int critRate;
        public int accuracy;
        public int bonusHp;
        public int bonusHpRegen;
        public int bonusDamage;
        public int bonusMovement;
        public int bonusArmor;
        public int bonusAttackSpeed;
        public int bonusAccuracy;
        public int bonusDodge;
        public int bonusCritRate;
        public int bonusCritDamage;
        public int bonusAttackRange;
    }

    [System.Serializable]
    public class GearStarUpCost
    {
        public int rank;
        public int star;
        public int material;
        public int coin;
    }

    [System.Serializable]
    public class GearDisassemble
    {
        public int rank;
        public int star;
        public int material;
        public int coin;
    }

    [System.Serializable]
    public class GearData : IComparable<GearData>
    {
        private GearDefinition baseData;
        private GearStarUpStat starUpStat;

        private GearsGroup gearsGroup => GameData.Instance.GearsGroup;

        public int id;
        public int baseId;
        public int star;
        public int heroId = -1;

        #region Info

        public string Name => baseData.name;
        public string NameLocal => Localization.Get("ITEM_NAME_" + baseData.id);
        public int Rank => baseData.rank;
        public int Slot => baseData.slot;
        // public int StarMax => baseData.starMax;
        public float Damage => ConfigStats.GetStat(baseData.damage, starUpStat.damage, star);
        public float HP => ConfigStats.GetStat(baseData.hp, starUpStat.hp, star);
        public float Armor => ConfigStats.GetStat(baseData.armor, starUpStat.armor, star);
        public float CritRate => ConfigStats.GetStat(baseData.critRate, starUpStat.critRate, star);
        public float Accuracy => ConfigStats.GetStat(baseData.accuracy, starUpStat.accuracy, star);

        public float BonusHp => ConfigStats.GetStat(baseData.bonusHp, starUpStat.bonusHp, star);
        public float BonusHpRegen => ConfigStats.GetStat(baseData.bonusHpRegen, starUpStat.bonusHpRegen, star);
        public float BonusDamage => ConfigStats.GetStat(baseData.bonusDamage, starUpStat.bonusDamage, star);
        public float BonusMovement => ConfigStats.GetStat(baseData.bonusMovement, starUpStat.bonusMovement, star);
        public float BonusArmor => ConfigStats.GetStat(baseData.bonusArmor, starUpStat.bonusArmor, star);
        public float BonusAttackSpeed => ConfigStats.GetStat(baseData.bonusAttackSpeed, starUpStat.bonusAttackSpeed, star);
        public float BonusAccuracy => ConfigStats.GetStat(baseData.bonusAccuracy, starUpStat.bonusAccuracy, star);
        public float BonusDodge => ConfigStats.GetStat(baseData.bonusDodge, starUpStat.bonusDodge, star);
        public float BonusCritRate => ConfigStats.GetStat(baseData.bonusCritRate, starUpStat.bonusCritRate, star);
        public float BonusCritDamage => ConfigStats.GetStat(baseData.bonusCritDamage, starUpStat.bonusCritDamage, star);
        public float BonusAttackRange => ConfigStats.GetStat(baseData.bonusAttackRange, starUpStat.bonusAttackRange, star);

        private int power = -1;
        public int Power
        {
            get
            {
                return power;
            }
        }

        #endregion

        #region Next Star

        public float DamageNextStar => ConfigStats.GetStat(baseData.damage, starUpStat.damage, star + 1);
        public float HPNextStar => ConfigStats.GetStat(baseData.hp, starUpStat.hp, star + 1);
        public float ArmorNextStar => ConfigStats.GetStat(baseData.armor, starUpStat.armor, star + 1);
        public float CritRateNextStar => ConfigStats.GetStat(baseData.critRate, starUpStat.critRate, star + 1);
        public float AccuracyNextStar => ConfigStats.GetStat(baseData.accuracy, starUpStat.accuracy, star + 1);
        public float BonusHpNextStar => ConfigStats.GetStat(baseData.bonusHp, starUpStat.bonusHp, star + 1);
        public float BonusHpRegenNextStar => ConfigStats.GetStat(baseData.bonusHpRegen, starUpStat.bonusHpRegen, star + 1);
        public float BonusDamageNextStar => ConfigStats.GetStat(baseData.bonusDamage, starUpStat.bonusDamage, star + 1);
        public float BonusMovementNextStar => ConfigStats.GetStat(baseData.bonusMovement, starUpStat.bonusMovement, star + 1);
        public float BonusArmorNextStar => ConfigStats.GetStat(baseData.bonusArmor, starUpStat.bonusArmor, star + 1);
        public float BonusAttackSpeedNextStar => ConfigStats.GetStat(baseData.bonusAttackSpeed, starUpStat.bonusAttackSpeed, star + 1);
        public float BonusAccuracyNextStar => ConfigStats.GetStat(baseData.bonusAccuracy, starUpStat.bonusAccuracy, star + 1);
        public float BonusDodgeNextStar => ConfigStats.GetStat(baseData.bonusDodge, starUpStat.bonusDodge, star + 1);
        public float BonusCritRateNextStar => ConfigStats.GetStat(baseData.bonusCritRate, starUpStat.bonusCritRate, star + 1);
        public float BonusCritDamageNextStar => ConfigStats.GetStat(baseData.bonusCritDamage, starUpStat.bonusCritDamage, star + 1);
        public float BonusAttackRangeNextStar => ConfigStats.GetStat(baseData.bonusAttackRange, starUpStat.bonusAttackRange, star + 1);

        #endregion

        public int CompareTo(GearData other)
        {
            // return -Power.CompareTo(other.Power);
            return -Rank.CompareTo(other.Rank);
        }

        public GearData(int _id, int _baseId)
        {
            id = _id;
            baseId = _baseId;
            star = 1;
            heroId = -1;//unequip

            Init();
        }

        public void Init()
        {
            baseData = gearsGroup.GetGearDefinition(baseId);
            starUpStat = gearsGroup.GetGearStarUpStat(baseId);

            power = ConfigStats.GetPowerGear(HP + BonusHp,
                                            Damage + BonusDamage,
                                            Armor + BonusArmor,
                                            BonusAttackSpeed,
                                            CritRate + BonusCritRate,
                                            Accuracy + BonusAccuracy,
                                            BonusDodge,
                                            BonusCritDamage);
        }

        public int GetMaxStars()
        {
            return gearsGroup.GetMaxGearStarUp(baseData.rank);
        }

        public bool IsMaxStarUp()
        {
            return star >= GetMaxStars();
        }

        public void StarUp()
        {
            star += 1;
            power = ConfigStats.GetPowerGear(HP + BonusHp,
                                        Damage + BonusDamage,
                                        Armor + BonusArmor,
                                        BonusAttackSpeed,
                                        CritRate + BonusCritRate,
                                        Accuracy + BonusAccuracy,
                                        BonusDodge,
                                        BonusCritDamage);

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.UpgradeGear();
            GameData.Instance.CampaignsGroup.AddUpgradeGearCount();

            gearsGroup.UpdateGearData(this);
        }

        public Sprite GetIcon()
        {
            return baseData.GetIcon();
        }

        public Sprite GetRankIcon()
        {
            return baseData.GetRankIcon();
        }

        //sau đổi lại localize
        public string GetRankName()
        {
            return baseData.GetRankName();
        }

        public string GetSlotName()
        {
            return baseData.GetSlotName();
        }

        public string GetDescription()
        {
            return baseData.GetDescription();
        }

        public bool IsEquipped()
        {
            return heroId != -1;
        }

        public void Equip(int _heroId)
        {
            heroId = _heroId;

            gearsGroup.UpdateGearData(this);
        }

        public void UnEquip()
        {
            heroId = -1;

            gearsGroup.UpdateGearData(this);
        }

        public GearStarUpCost GetGearStarUpCost()
        {
            return gearsGroup.GetGearStarUpCost(baseData.rank, star);
        }

        public List<RewardInfo> GetRewardFromDisassembleGear()
        {
            var gearDisassemble = gearsGroup.GetGearDisassemble(baseData.rank, star);

            var rewardInfos = new List<RewardInfo>();
            //coin reward
            rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, gearDisassemble.coin));
            //material reward
            rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_MATERIAL, gearDisassemble.material));

            return rewardInfos;
        }

        public string ToSortString()
        {
            var s = "";
            if (Damage > 0f) s += "Damage " + Damage + " ";
            if (HP > 0f) s += "HP " + HP + " ";
            if (Armor > 0f) s += "Armor " + Armor + " ";
            if (CritRate > 0f) s += "Critical Rate " + CritRate + " ";
            if (Accuracy > 0f) s += "Accuracy " + Accuracy;

            return s;
        }

        public override string ToString()
        {
            var s = "";
            if (Damage > 0f) s += "Damage " + Damage + "\n";
            if (HP > 0f) s += "HP " + HP + "\n";
            if (Armor > 0f) s += "Armor " + Armor + "\n";
            if (CritRate > 0f) s += "CritRate " + CritRate + "\n";
            if (Accuracy > 0f) s += "Accuracy " + Accuracy + "\n";
            if (BonusHp > 0f) s += "Bonus Hp " + BonusHp + "\n";
            if (BonusHpRegen > 0f) s += "BonusHp Regen " + BonusHpRegen + "\n";
            if (BonusDamage > 0f) s += "Bonus Damage " + BonusDamage + "\n";
            if (BonusMovement > 0f) s += "Bonus Movement " + BonusMovement + "\n";
            if (BonusArmor > 0f) s += "Bonus Armor " + BonusArmor + "\n";
            if (BonusAttackSpeed > 0f) s += "Bonus Attack Speed " + BonusAttackSpeed + "\n";
            if (BonusAccuracy > 0f) s += "Bonus Accuracy " + BonusAccuracy + "\n";
            if (BonusDodge > 0f) s += "Bonus Dodge " + BonusDodge + "\n";
            if (BonusCritRate > 0f) s += "Bonus Critical Rate " + BonusCritRate + "\n";
            if (BonusCritDamage > 0f) s += "Bonus Critical Damage " + BonusCritDamage + "\n";
            if (BonusAttackRange > 0f) s += "Bonus Attack Range " + BonusAttackRange + "\n";

            return s;
        }
    }

    public class GearsGroup : DataGroup
    {
        #region Members

        private Dictionary<int, GearDefinition> gearDefinitions;
        private Dictionary<int, GearStarUpStat> gearStarUpStats;
        private Dictionary<int, List<GearStarUpCost>> gearStarUpCosts;
        private Dictionary<int, List<GearDisassemble>> gearDisassembles;

        private ListData<string> gearsGroup;
        private IntegerData countClaimGear;

        public Dictionary<int, GearDefinition> GearDefinitions => gearDefinitions;

        private List<GearData> cacheGearDatas;

        #endregion

        //=============================================

        #region Public

        public GearsGroup(int _Id) : base(_Id)
        {
            //Declare sub groups which contain units data
            gearsGroup = AddData(new ListData<string>(0, new List<string>()));
            countClaimGear = AddData(new IntegerData(1, 0));

            InitGearsGroup();
            InitGrearStarUpStats();
            InitGearStarUpCosts();
            InitGearDisassembles();
        }

        public GearData GetGearData(int _gearId)
        {
            if (_gearId != -1)
            {
                var currentGearDatas = GetAllGearDatasCache(); //get cache
                var count = currentGearDatas.Count;
                for (int i = 0; i < count; i++)
                {
                    var gearData = currentGearDatas[i];
                    if (gearData.id == _gearId)
                    {
                        return gearData;
                    }
                }
            }

            return null;
        }

        public List<GearData> GetAllGearDatas()
        {
            return new List<GearData>(GetAllGearDatasCache());
        }

        private List<GearData> GetAllGearDatasCache()
        {
            if (cacheGearDatas != null) return cacheGearDatas;
            else
            {
                cacheGearDatas = new List<GearData>();
                for (int i = 0; i < gearsGroup.Count; i++)
                {
                    var gearData = JsonUtility.FromJson<GearData>(gearsGroup[i]);
                    gearData.Init();
                    cacheGearDatas.Add(gearData);
                }

                return cacheGearDatas;
            }
        }

        public List<GearData> GetAllGearDatasEquip()
        {
            var list = new List<GearData>();
            var currentGearDatas = GetAllGearDatasCache(); //get cache
            var count = currentGearDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var gearData = currentGearDatas[i];
                if (gearData.IsEquipped())
                {
                    list.Add(gearData);
                }
            }

            return list;
        }

        public List<GearData> GetAllGearDatasUnEquip()
        {
            var list = new List<GearData>();
            var currentGearDatas = GetAllGearDatasCache(); //get cache
            var count = currentGearDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var gearData = currentGearDatas[i];
                if (!gearData.IsEquipped())
                {
                    list.Add(gearData);
                }
            }

            return list;
        }

        public List<GearData> GetAllGearDatasUnEquip(int slotId)
        {
            var list = new List<GearData>();
            var currentGearDatas = GetAllGearDatasCache(); //get cache
            var count = currentGearDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var gearData = currentGearDatas[i];
                if (!gearData.IsEquipped() && gearData.Slot == slotId)
                {
                    list.Add(gearData);
                }
            }

            return list;
        }

        public Dictionary<int, GearData> GetAllGearDatasByHero(int heroId)
        {
            var list = new Dictionary<int, GearData>();
            var currentGearDatas = GetAllGearDatasCache(); //get cache
            var count = currentGearDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var gearData = currentGearDatas[i];
                if (gearData.heroId == heroId)
                {
                    list.Add(gearData.Slot, gearData);
                }
            }

            return list;
        }

        public void ClaimGear(int _baseId)
        {
            int count = countClaimGear.Value;

            var gearData = new GearData(count, _baseId); //với mỗi heroData mới tạo id ++
            gearsGroup.Add(JsonUtility.ToJson(gearData));
            var currentGearDatas = GetAllGearDatasCache(); //get cache
            currentGearDatas.Add(gearData); //update cache

            countClaimGear.Value++;

            //Daily Quest và Achievement
            if (gearData.Rank == IDs.RANK_S) GameData.Instance.AchievementsGroup.HaveGearRankS();
            else if (gearData.Rank == IDs.RANK_SS) GameData.Instance.AchievementsGroup.HaveGearRankSS();

            EventDispatcher.Raise(new ChangeGearEvent());
        }

        public void ClaimAllGears()
        {
            for (int i = 0; i < gearDefinitions.Count; i++)
            {
                ClaimGear(gearDefinitions[i].id);
            }
        }

        public void UpdateGearData(GearData _gearData)
        {
            var currentGearDatas = GetAllGearDatasCache(); //get cache
            var count = currentGearDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var gearData = currentGearDatas[i];
                if (gearData.id == _gearData.id)
                {
                    //data & cache
                    gearsGroup[i] = JsonUtility.ToJson(_gearData);
                    currentGearDatas[i] = _gearData;
                    return;
                }
            }
        }

        public List<RewardInfo> DisassembleGear(GearData _gearData)
        {
            var rewardInfos = _gearData.GetRewardFromDisassembleGear();
            RemoveGearData(_gearData);

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.DisassembleGear();
            GameData.Instance.CampaignsGroup.AddDisassembleGearCount();

            return rewardInfos;
        }

        public List<RewardInfo> GetRewardFromDisassembleGears(List<GearData> _gearDatas)
        {
            var count = _gearDatas.Count;
            var totalRewardInfos = new List<RewardInfo>();
            bool isNewReward = true;
            for (int i = 0; i < count; i++)
            {
                var _gearData = _gearDatas[i];
                var rewardInfos = _gearData.GetRewardFromDisassembleGear();
                var countTotalReward = totalRewardInfos.Count;
                var countReward = rewardInfos.Count;
                for (int j = 0; j < countReward; j++)
                {
                    isNewReward = true;
                    //kiểm tra một vòng các reward mới dã ra, cái nào có thì cộng vào, còn một vòng rồi mà ko có thì add mới
                    var rewardInfo = rewardInfos[j];
                    for (int k = 0; k < countTotalReward; k++)
                    {
                        var totalRewardInfo = totalRewardInfos[k];
                        if (totalRewardInfo.Type == rewardInfo.Type && totalRewardInfo.Id == rewardInfo.Id)
                        {
                            totalRewardInfos[k] = new RewardInfo(totalRewardInfo.Type, totalRewardInfo.Id, totalRewardInfo.Value + rewardInfo.Value);
                            isNewReward = false;
                            break;
                        }
                    }
                    if (isNewReward) totalRewardInfos.Add(rewardInfo);
                }
            }

            return totalRewardInfos;
        }

        public List<RewardInfo> DisassembleGears(List<GearData> _gearDatas)
        {
            RemoveGearDatas(_gearDatas);

            var count = _gearDatas.Count;
            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.DisassembleGears(count);
            GameData.Instance.CampaignsGroup.AddDisassembleGearCount(count);

            return GetRewardFromDisassembleGears(_gearDatas);
        }

        private void RemoveGearData(GearData _gearData)
        {
            var currentGearDatas = GetAllGearDatasCache(); //get cache
            var count = currentGearDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var gearData = currentGearDatas[i];
                if (gearData.id == _gearData.id)
                {
                    //data & cache
                    gearsGroup.Remove(gearsGroup[i]);
                    currentGearDatas.RemoveAt(i);
                    return;
                }
            }
        }

        private void RemoveGearDatas(List<GearData> _gearDatas)
        {
            var gearDatas = new List<GearData>(_gearDatas); //để nó còn check cho hàm GetRewardFromDisassembleGears

            var currentGearDatas = GetAllGearDatasCache(); //get cache
            var count = currentGearDatas.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                //tìm trong list gear cái cần remove và tìm được cái nào thì bỏ đi lần sau lặp lại cái chưa tìm
                var gearData = currentGearDatas[i];
                var countGearsRemove = gearDatas.Count;
                for (int j = countGearsRemove - 1; j >= 0; j--)
                {
                    var _gearDataRemove = gearDatas[j];
                    if (gearData.id == _gearDataRemove.id)
                    {
                        //data & cache
                        gearsGroup.Remove(gearsGroup[i]);
                        currentGearDatas.RemoveAt(i);

                        gearDatas.RemoveAt(j);
                        break;
                    }
                }

                if (countGearsRemove == 0) return;
            }
        }

        public GearDefinition GetGearDefinition(int _gearBaseId)
        {
            return gearDefinitions[_gearBaseId];
        }

        //star up
        public GearStarUpStat GetGearStarUpStat(int _gearBaseId)
        {
            return gearStarUpStats[_gearBaseId];
        }

        public int GetMaxGearStarUp(int rank)
        {
            return gearStarUpCosts[rank].Count;
        }

        public GearStarUpCost GetGearStarUpCost(int rank, int star)
        {
            if (rank > gearStarUpCosts.Count) return null;
            if (star >= gearStarUpCosts[rank].Count) return null;
            return gearStarUpCosts[rank][star];// ko - 1 vì lấy giá trị next
        }

        public GearDisassemble GetGearDisassemble(int rank, int star)
        {
            return gearDisassembles[rank][star - 1];
        }

        #endregion

        //==============================================

        #region Private

        private void InitGearsGroup()
        {
            var dataContent = GameData.GetTextContent("Data/Gear");
            var listGearDefinition = JsonHelper.GetJsonList<GearDefinition>(dataContent);
            gearDefinitions = new Dictionary<int, GearDefinition>();
            foreach (var item in listGearDefinition)
            {
                gearDefinitions.Add(item.id, item);
            }
        }

        private void InitGrearStarUpStats()
        {
            var dataContent = GameData.GetTextContent("Data/GearStarUpStat");
            var listGearStarUpStatDefinition = JsonHelper.GetJsonList<GearStarUpStat>(dataContent);
            gearStarUpStats = new Dictionary<int, GearStarUpStat>();
            foreach (var item in listGearStarUpStatDefinition)
            {
                gearStarUpStats.Add(item.id, item);
            }
        }

        private void InitGearStarUpCosts()
        {
            var dataContent = GameData.GetTextContent("Data/GearStarUpCost");
            var listGearStarUpCosts = JsonHelper.GetJsonList<GearStarUpCost>(dataContent);

            //nhóm các GearStarUpCost cùng rank vào một list chung theo thứ tự
            //lúc sau mình chỉ cần get theo rank và theo thứ tự là star - 1
            var lastRank = -1;
            gearStarUpCosts = new Dictionary<int, List<GearStarUpCost>>();
            List<GearStarUpCost> subGearStarUpCosts = null;
            foreach (var item in listGearStarUpCosts)
            {
                if (lastRank != item.rank)
                {
                    subGearStarUpCosts = new List<GearStarUpCost>();
                    gearStarUpCosts.Add(item.rank, subGearStarUpCosts);
                    lastRank = item.rank;
                }
                subGearStarUpCosts.Add(item);
            }
        }

        private void InitGearDisassembles()
        {
            var dataContent = GameData.GetTextContent("Data/GearDisassemble");
            var listGearDisassembles = JsonHelper.GetJsonList<GearDisassemble>(dataContent);

            //nhóm các GearDisassemble cùng rank vào một list chung theo thứ tự
            //lúc sau mình chỉ cần get theo rank và theo thứ tự là star - 1
            var lastRank = -1;
            gearDisassembles = new Dictionary<int, List<GearDisassemble>>();
            List<GearDisassemble> subGearDisassembles = null;
            foreach (var item in listGearDisassembles)
            {
                if (lastRank != item.rank)
                {
                    subGearDisassembles = new List<GearDisassemble>();
                    gearDisassembles.Add(item.rank, subGearDisassembles);
                    lastRank = item.rank;
                }
                subGearDisassembles.Add(item);
            }
        }

        #endregion
    }
}
