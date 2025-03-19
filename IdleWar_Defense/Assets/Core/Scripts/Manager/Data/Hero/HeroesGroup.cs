using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Sirenix.Serialization;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Debug = UnityEngine.Debug;

namespace FoodZombie
{
    [System.Serializable]
    public class HeroDefinition : IComparable<HeroDefinition>
    {
        public int id;
        public string name;
        public bool active;
        public string skin;
        public int[] attackTypes;
        public int rank;
        public int race;
        public int element;
        public int targetToEnemy;
        public float[] skillValues;
        public string passiveSkill1Name;
        public string passiveSkill1;
        public string passiveSkill2Name;
        public string passiveSkill2;
        public string specialSkillName;
        public float cooldown;
        public int lvBase;
        public int starMax;
        public float hp;
        public float hpRegen;
        public float damage;
        public float movement;
        public float armor;
        public float attackSpeed;
        public float accuracy;
        public float knockback;
        public float dodge;
        public float critRate;
        public float critDamage;
        public float attackRange;
        public bool invisible;
        public float bulletSpeed;

        public int CompareTo(HeroDefinition other)
        {
            return id.CompareTo(other.id);
        }

        public Sprite GetIcon()
        {
            return AssetsCollection.instance.heroIcon.GetAsset(id - 1);
        }

        public Sprite GetElementIcon()
        {
            return AssetsCollection.instance.elementIcon.GetAsset(element - 1);
        }

        public Sprite GetRankIcon()
        {
            return AssetsCollection.instance.GetRankIcon(rank);
        }

        public Sprite GetRankElementIcon()
        {
            return AssetsCollection.instance.GetRankElementIcon(rank);
        }

        //sau đổi lại localize
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
    }

    [System.Serializable]
    public class HeroLevelUpStat : IComparable<HeroLevelUpStat>
    {
        public int id;
        public string name;
        public int rank;
        public int lvUp;
        public float hp;
        public float hpRegen;
        public float damage;
        public float movement;
        public float armor;
        public float attackSpeed;
        public float accuracy;
        public float knockback;
        public float dodge;
        public float critRate;
        public float critDamage;
        public float attackRange;
        public float bulletSpeed;
        public int CompareTo(HeroLevelUpStat other)
        {
            return id.CompareTo(other.id);
        }
    }

    [System.Serializable]
    public class HeroLevelUpCost : IComparable<HeroLevelUpCost>
    {
        public int level;
        public int gold;
        public int exp;

        public int CompareTo(HeroLevelUpCost other)
        {
            return level.CompareTo(other.level);
        }
    }

    [System.Serializable]
    public class HeroStarUpStat
    {
        public int rank;
        public float[] valueUps;
    }

    [System.Serializable]
    public class HeroStarUpCost
    {
        public int star;
        public int elementDust;
    }

    [System.Serializable]
    public class HeroDisassemble
    {
        public int rank;
        public int star;
        public int elementDust;
        public int coin;
        public int payBackCoin;
        public int payBackExpHero;
    }

    [System.Serializable]
    public class HeroData : IComparable<HeroData>
    {
        private HeroDefinition baseData;
        private HeroLevelUpStat levelUpStat;
        private float percentStarUpStat;
        private Dictionary<int, GearData> gearDatas;

        public Dictionary<int, GearData> GearDatas => gearDatas;

        private HeroesGroup heroesGroup => GameData.Instance.HeroesGroup;

        public int id;
        public int baseId;
        public int levelUnlocked;//level unlocked
        public int star;

        #region Info

        public string Name => baseData.name;
        public string Skin => baseData.skin;
        public int[] AttackTypes => baseData.attackTypes;
        public int Rank => baseData.rank;
        public int Race => baseData.race;
        public int Element => baseData.element;
        public int TargetToEnemy => baseData.targetToEnemy;
        public string SkillName => Localization.Get("SKILL_MAIN_NAME_" + baseId);
        public string SkillDescription => Localization.Get("SKILL_MAIN_DESCRIPTION_" + baseId);
        public float[] SkillValues => baseData.skillValues;
        public string PassiveSkill1Name => baseData.passiveSkill1Name == null ? "" : Localization.Get("PASSIVE_SKILL_NAME_" + baseData.passiveSkill1Name);
        public string PassiveSkill1Description => baseData.passiveSkill1 == null ? "" : string.Format(Localization.Get("PASSIVE_SKILL_DESCRIPTION_" + baseData.passiveSkill1Name), baseData.passiveSkill1);
        public string PassiveSkill2Name => baseData.passiveSkill2Name == null ? "" : Localization.Get("PASSIVE_SKILL_NAME_" + baseData.passiveSkill2Name);
        public string PassiveSkill2Description => baseData.passiveSkill2 == null ? "" : string.Format(Localization.Get("PASSIVE_SKILL_DESCRIPTION_" + baseData.passiveSkill2Name), baseData.passiveSkill2);
        public string SpecialSkillName => baseData.specialSkillName == null ? "" : Localization.Get("SPECIAL_SKILL_NAME_" + baseData.specialSkillName);
        public string SpecialSkillDescription => baseData.specialSkillName == null ? "" : Localization.Get("SPECIAL_SKILL_DESCRIPTION_" + baseData.specialSkillName);
        public float Cooldown => baseData.cooldown /*/ (1f + percentStarUpStat)*/;
        public bool Invisible => baseData.invisible;

        #endregion

        #region stats

        private float hp;
        private float hpRegen;
        private float damage;
        private float movement;
        private float armor;
        private float attackSpeed;
        private float accuracy;
        private float knockback;
        private float dodge;
        private float critRate;
        private float critDamage;
        private float attackRange;
        private float bulletSpeed;

        public float HP => hp;
        public float HPRegen => hpRegen;
        public float Damage => damage;
        public float Movement => movement;
        public float Armor => armor;
        public float AttackSpeed => attackSpeed;
        public float Accuracy => accuracy;
        public float Knockback => knockback;
        public float Dodge => dodge;
        public float CritRate => critRate;
        public float CritDamage => critDamage;
        public float AttackRange => attackRange;

        #endregion

        #region stats gears

        private float hpGear;
        private float hpRegenGear;
        private float damageGear;
        private float movementGear;
        private float armorGear;
        private float attackSpeedGear;
        private float accuracyGear;
        private float dodgeGear;
        private float critRateGear;
        private float critDamageGear;
        private float attackRangeGear;

        public float HPGear => hpGear;
        public float HPRegenGear => hpRegenGear;
        public float DamageGear => damageGear;
        public float MovementGear => movementGear;
        public float ArmorGear => armorGear;
        public float AttackSpeedGear => attackSpeedGear;
        public float AccuracyGear => accuracyGear;
        public float DodgeGear => dodgeGear;
        public float CritRateGear => critRateGear;
        public float CritDamageGear => critDamageGear;
        public float AttackRangeGear => attackRangeGear;

        #endregion

        #region gameplay

        public float HPTotal => hp + hpGear;
        public float HPRegenTotal => hpRegen + hpRegenGear;
        public float DamageTotal => damage + damageGear;
        public float MovementTotal => movement + movementGear;
        public float ArmorTotal => armor + armorGear;
        public float AttackSpeedTotal => attackSpeed + attackSpeedGear;
        public float AccuracyTotal => accuracy + accuracyGear;
        public float DodgeTotal => dodge + dodgeGear;
        public float CritRateTotal => critRate + critRateGear;
        public float CritDamageTotal => critDamage + critDamageGear;
        public float AttackRangeTotal => attackRange + attackRangeGear;

        public float BulletSpeed => bulletSpeed;

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

        public float HPTotalNextStar
        {
            get
            {
                baseData = heroesGroup.GetHeroDefinition(baseId);
                levelUpStat = heroesGroup.GetHeroLevelUpStat(baseId);
                percentStarUpStat = heroesGroup.GetPercentHeroStarUpStat(baseData.rank, star + 1);
                var scaleStarUpStat = Mathf.Pow(1f + percentStarUpStat, star);

                var hpNextStar = ConfigStats.GetStat(baseData.hp, levelUpStat.hp, levelUnlocked) * scaleStarUpStat;

                return hpNextStar + hpGear;
            }
        }
        public float DamageTotalNextStar
        {
            get
            {
                baseData = heroesGroup.GetHeroDefinition(baseId);
                levelUpStat = heroesGroup.GetHeroLevelUpStat(baseId);
                percentStarUpStat = heroesGroup.GetPercentHeroStarUpStat(baseData.rank, star + 1);
                var scaleStarUpStat = Mathf.Pow(1f + percentStarUpStat, star);

                var damageNextStar = ConfigStats.GetStat(baseData.damage, levelUpStat.damage, levelUnlocked) * scaleStarUpStat;

                return damageNextStar + damageGear;
            }
        }
        public float ArmorTotalNextStar
        {
            get
            {
                baseData = heroesGroup.GetHeroDefinition(baseId);
                levelUpStat = heroesGroup.GetHeroLevelUpStat(baseId);
                percentStarUpStat = heroesGroup.GetPercentHeroStarUpStat(baseData.rank, star + 1);
                var scaleStarUpStat = Mathf.Pow(1f + percentStarUpStat, star);

                var armorNextStar = ConfigStats.GetStat(baseData.armor, levelUpStat.armor, levelUnlocked) * scaleStarUpStat;

                return armorNextStar + armorGear;
            }
        }

        #endregion

        public HeroData(int _id, int _baseId, int _levelUnlocked = 1)
        {
            id = _id;
            baseId = _baseId;
            levelUnlocked = _levelUnlocked;
            star = 1;

            Init();
        }

        public void Init()
        {
            baseData = heroesGroup.GetHeroDefinition(baseId);
            if (baseData == null) return;
            levelUpStat = heroesGroup.GetHeroLevelUpStat(baseId);
            percentStarUpStat = heroesGroup.GetPercentHeroStarUpStat(baseData.rank, star);
            var scaleStarUpStat = Mathf.Pow(1f + percentStarUpStat, star - 1);

            hp = ConfigStats.GetStat(baseData.hp, levelUpStat.hp, levelUnlocked) * scaleStarUpStat;
            hpRegen = ConfigStats.GetStat(baseData.hpRegen, levelUpStat.hpRegen, levelUnlocked) /** scaleStarUpStat*/;
            damage = ConfigStats.GetStat(baseData.damage, levelUpStat.damage, levelUnlocked) * scaleStarUpStat;
            movement = ConfigStats.GetStat(baseData.movement, levelUpStat.movement, levelUnlocked) /** scaleStarUpStat*/ * ConfigStats.GetXSpeed();
            armor = ConfigStats.GetStat(baseData.armor, levelUpStat.armor, levelUnlocked) * scaleStarUpStat;
            attackSpeed = ConfigStats.GetStat(baseData.attackSpeed, levelUpStat.attackSpeed, levelUnlocked) /** scaleStarUpStat*/;
            accuracy = ConfigStats.GetStat(baseData.accuracy, levelUpStat.accuracy, levelUnlocked) /** scaleStarUpStat*/;
            knockback = ConfigStats.GetStat(baseData.knockback, levelUpStat.knockback, levelUnlocked) /** scaleStarUpStat*/;
            dodge = ConfigStats.GetStat(baseData.dodge, levelUpStat.dodge, levelUnlocked) /** scaleStarUpStat*/;
            critRate = ConfigStats.GetStat(baseData.critRate, levelUpStat.critRate, levelUnlocked) /** scaleStarUpStat*/;
            critDamage = ConfigStats.GetStat(baseData.critDamage, levelUpStat.critDamage, levelUnlocked) /** scaleStarUpStat*/;
            attackRange = ConfigStats.GetStat(baseData.attackRange, levelUpStat.attackRange, levelUnlocked);
            bulletSpeed = ConfigStats.GetStat(baseData.bulletSpeed, levelUpStat.bulletSpeed, levelUnlocked);

            hpGear = 0f;
            hpRegenGear = 0f;
            damageGear = 0f;
            movementGear = 0f;
            armorGear = 0f;
            attackSpeedGear = 0f;
            accuracyGear = 0f;
            dodgeGear = 0f;
            critRateGear = 0f;
            critDamageGear = 0f;
            attackRangeGear = 0f;

            //cache
            if (gearDatas == null)
            {
                gearDatas = GameData.Instance.GearsGroup.GetAllGearDatasByHero(id);
            }

            foreach (var item in gearDatas)
            {
                var gearData = item.Value;
                hpGear += gearData.HP + gearData.BonusHp;
                hpRegenGear += gearData.BonusHpRegen;
                damageGear += gearData.Damage + gearData.BonusDamage;
                movementGear += gearData.BonusMovement;
                armorGear += gearData.Armor + gearData.BonusArmor;
                attackSpeedGear += gearData.BonusAttackSpeed;
                accuracyGear += gearData.Accuracy + gearData.BonusAccuracy;
                dodgeGear += gearData.BonusDodge;
                critRateGear += gearData.CritRate + gearData.BonusCritRate;
                critDamageGear += gearData.BonusCritDamage;
                attackRangeGear += gearData.BonusAttackRange;
            }

            power = ConfigStats.GetPower(HPTotal,
                        DamageTotal,
                        ArmorTotal,
                        AttackSpeedTotal,
                        CritRateTotal,
                        AccuracyTotal,
                        DodgeTotal,
                        CritDamageTotal) * 10;
        }

        public bool IsMaxLevelUp()
        {
            return heroesGroup.IsMaxHeroLevelUp(levelUnlocked);
        }

        public void LevelUp()
        {
            levelUnlocked += 1;

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.LevelUpHero();
            GameData.Instance.CampaignsGroup.AddUpgradeHeroCount();

            Init();
            heroesGroup.UpdateHeroData(this);

            EventDispatcher.Raise(new HeroLevelUpEvent());
        }

        public int GetMaxStars()
        {
            return heroesGroup.GetMaxHeroStarUp(baseData.rank);
        }

        public bool IsMaxStarUp()
        {
            return star >= GetMaxStars();
        }

        public void StarUp()
        {
            star += 1;
            Init();
            heroesGroup.UpdateHeroData(this);

            EventDispatcher.Raise(new HeroStarUpEvent());
            //Daily Quest và Achievement
            GameData.Instance.CampaignsGroup.AddUpgradeStarHeroCount();
        }

        public Sprite GetIcon()
        {
            return baseData.GetIcon();
        }

        public Sprite GetElementIcon()
        {
            return baseData.GetElementIcon();
        }

        public Sprite GetRankIcon()
        {
            return baseData.GetRankIcon();
        }

        public Sprite GetRankElementIcon()
        {
            return baseData.GetRankElementIcon();
        }

        public string GetRankName()
        {
            return baseData.GetRankName();
        }

        public bool IsEquipped(int formationIndex)
        {
            return heroesGroup.HasEquippedHero(formationIndex, id);
        }

        public bool IsEquipped()
        {
            for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
            {
                if (heroesGroup.HasEquippedHero(i, id))
                {
                    return true;
                }
            }

            return false;
        }

        public HeroLevelUpCost GetHeroLevelUpCost()
        {
            return heroesGroup.GetHeroLevelUpCost(levelUnlocked);
        }

        public int GetHeroStarUpCost()
        {
            return heroesGroup.GetHeroStarUpCost(star);
        }

        public List<RewardInfo> GetRewardFromDisassembleHero()
        {
            var heroDisassemble = heroesGroup.GetHeroDisassemble(baseData.rank, star);

            var rewardInfos = new List<RewardInfo>();
            //tính tổng coin và exp hero đã dùng để level up - từng level từ đầu đến giờ
            var coin = 0;
            var expHero = 0;
            for (int i = 1; i < levelUnlocked; i++)
            {
                var heroLevelUpCost = heroesGroup.GetHeroLevelUpCost(i);
                coin += heroLevelUpCost.gold;
                expHero += heroLevelUpCost.exp;
            }
            //coin reward
            rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_COIN, heroDisassemble.coin + (int)(coin * heroDisassemble.payBackCoin / 100f)));
            if (expHero > 0) rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_EXP_HERO, (int)(expHero * heroDisassemble.payBackExpHero / 100f)));
            //dust reward
            rewardInfos.Add(new RewardInfo(IDs.REWARD_TYPE_CURRENCY, CurrenciesGroup.GetCurrencyIDFromElementID(baseData.element), heroDisassemble.elementDust));

            return rewardInfos;
        }

        public GearData GetEquippedGear(int slotId)
        {
            if (gearDatas != null && gearDatas.ContainsKey(slotId)) return gearDatas[slotId];
            return null;
        }

        //gear
        public void EquipGear(int slotId, int gearId)
        {
            var gearData = GetEquippedGear(slotId);
            if (gearData != null) gearData.UnEquip();

            var gearsGroup = GameData.Instance.GearsGroup;
            var newGearData = gearsGroup.GetGearData(gearId);
            newGearData.Equip(id);
            //data & cache
            if (gearData != null) gearDatas[slotId] = newGearData;
            else gearDatas.Add(slotId, newGearData);

            Init();
            EventDispatcher.Raise(new ChangeGearEvent());
        }

        public void UnEquipAllGear()
        {
            foreach (var gearData in gearDatas)
            {
                gearData.Value.UnEquip();
            }
            //data & cache
            gearDatas.Clear();

            Init();
            EventDispatcher.Raise(new ChangeGearEvent());
        }

        public void UnEquipGear(int slotId)
        {
            var gearData = GetEquippedGear(slotId);
            if (gearData != null)
            {
                gearData.UnEquip();
                //data & cache
                gearDatas.Remove(slotId);

                Init();
                EventDispatcher.Raise(new ChangeGearEvent());
            }
        }

        //noti
        public bool CanLevelUp()
        {
            if (IsMaxLevelUp()) return false;

            var currenciesGroup = GameData.Instance.CurrenciesGroup;

            //level-up
            var heroLevelUpCost = GetHeroLevelUpCost();
            var coin = heroLevelUpCost.gold;
            var heroExp = heroLevelUpCost.exp;

            return (currenciesGroup.CanPay(IDs.CURRENCY_COIN, coin) &&
                    currenciesGroup.CanPay(IDs.CURRENCY_EXP_HERO, heroExp));
        }

        public bool CanStarUp()
        {
            if (IsMaxStarUp()) return false;

            var currenciesGroup = GameData.Instance.CurrenciesGroup;

            //star-up
            var elementCost = GetHeroStarUpCost();
            var currencyId = CurrenciesGroup.GetCurrencyIDFromElementID(Element);

            return (currenciesGroup.CanPay(currencyId, elementCost));
        }

        public bool CanOneClickEquip()
        {
            var gearsGroup = GameData.Instance.GearsGroup;

            for (int i = 0; i < 4; i++)
            {
                //lấy ra gear theo thứ tự slot
                var gearData = GetEquippedGear(i + 1);
                var gearDatasUnEquip = gearsGroup.GetAllGearDatasUnEquip(i + 1);
                var countUnEquip = gearDatasUnEquip.Count;
                if (gearData == null && countUnEquip > 0) return true;

                for (int j = 0; j < countUnEquip; j++)
                {
                    if (gearData.Power < gearDatasUnEquip[j].Power) return true;
                    //if (gearData.Rank < gearDatasUnEquip[j].Rank) return true;
                }
            }

            return false;
        }

        public bool CanOneClickEquip(int slotId)
        {
            var gearsGroup = GameData.Instance.GearsGroup;

            var gearData = GetEquippedGear(slotId);
            var gearDatasUnEquip = gearsGroup.GetAllGearDatasUnEquip(slotId);
            var countUnEquip = gearDatasUnEquip.Count;
            if (gearData == null && countUnEquip > 0) return true;

            for (int j = 0; j < countUnEquip; j++)
            {
                if (gearData.Power < gearDatasUnEquip[j].Power)
                    return true;
                //if (gearData.Rank < gearDatasUnEquip[j].Rank) return true;
            }


            return false;
        }

        public int CompareTo(HeroData other)
        {
            var equipped = IsEquipped();
            var otherIsEquipped = other.IsEquipped();
            if (equipped == otherIsEquipped)
            {
                return other.Power.CompareTo(Power);
            }
            else if (equipped && !otherIsEquipped)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    [System.Serializable]
    public class SummonInfo
    {
        public int id;
        public float dropRate;
        public float dropChange;
        public float dropMax;
    }

    [System.Serializable]
    public class HeroEvolutionInfo
    {
        public int heroId;
        public int idMaterial_1;
        public int idMaterial_2;
        public int idMaterial_3;
        public int rankMaterial_4; //Random Rank S same element Hero
        public int rankMaterial_5;
        public int coinCost;
    }

    public class HeroesGroup : DataGroup
    {
        #region Members

        public const int MAX_FORMATION = 3;

        private Dictionary<int, HeroDefinition> heroDefinitions;
        private Dictionary<int, HeroLevelUpStat> heroLevelUpStats;
        private List<HeroLevelUpCost> heroLevelUpCosts;
        private Dictionary<int, HeroStarUpStat> heroStarUpStats;
        private List<HeroStarUpCost> heroStarUpCosts;
        private Dictionary<int, List<HeroDisassemble>> heroDisassembles;

        private List<SummonInfo> powerFragmentSummonInfos;
        private List<SummonInfo> powerCrystalSummonInfos;
        private List<SummonInfo> devineCrystalSummonInfos;

        private List<HeroEvolutionInfo> heroEvolutionInfos;

        private ListData<string> heroesGroup;
        private ListData<string>[] formations;
        private IntegerData countClaimHero;
        private IntegerData countClaimHerox10;
        private IntegerData currentFormation;

        private IntegerData countPowerFragmentSummon;
        private IntegerData countPowerFragmentRollToRankA;
        private IntegerData countPowerFragmentRollToRankS;
        private IntegerData countPowerCrystalSummon;
        private IntegerData countPowerCrystalRollToRankA;
        private IntegerData countPowerCrystalRollToRankS;
        private IntegerData countDevineCrystalSummon;
        private IntegerData countDevineCrystalRollToRankS;

        private IntegerData countPowerCrystalSummonByGem;
        private BoolData freePowerFragmentSummon;
        private BoolData freePowerCrystalSummon;

        public int CountClaimHero => countClaimHero.Value;
        public int CurrentFormation => currentFormation.Value;

        public bool FreePowerFragmentSummon => freePowerFragmentSummon.Value;
        public bool FreePowerCrystalSummon => freePowerCrystalSummon.Value;

        private List<HeroData> cacheHeroDatas;

        #endregion

        //=============================================

        #region Public

        // public override void PostLoad()
        // {
        //     base.PostLoad();
        //
        //     var heroes = GetAllHeroDatas();
        //     int count = heroes.Count;
        //     if (count <= 0)
        //     {
        //         ClaimHero(Constants.DEFAULT_HERO_ID);
        //     }
        // }

        public HeroesGroup(int _Id) : base(_Id)
        {
            //Declare sub groups which contain units data
            heroesGroup = AddData(new ListData<string>(0, new List<string>()));
            countClaimHero = AddData(new IntegerData(1, 0));
            currentFormation = AddData(new IntegerData(2, 0));

            //summon
            countPowerFragmentSummon = AddData(new IntegerData(3, 0));
            countPowerFragmentRollToRankA = AddData(new IntegerData(4, 0));
            countPowerFragmentRollToRankS = AddData(new IntegerData(5, 0));
            countPowerCrystalSummon = AddData(new IntegerData(6, 0));
            countPowerCrystalRollToRankA = AddData(new IntegerData(7, 0));
            countPowerCrystalRollToRankS = AddData(new IntegerData(8, 0));
            countDevineCrystalSummon = AddData(new IntegerData(9, 0));
            countDevineCrystalRollToRankS = AddData(new IntegerData(10, 0));

            countPowerCrystalSummonByGem = AddData(new IntegerData(11, 0));
            freePowerFragmentSummon = AddData(new BoolData(12, true));
            freePowerCrystalSummon = AddData(new BoolData(13, true));

            countClaimHerox10 = AddData(new IntegerData(14, 0));
            //formation start from 100
            formations = new ListData<string>[MAX_FORMATION];
            for (int i = 0; i < MAX_FORMATION; i++)
            {
                formations[i] = AddData(new ListData<string>(100 + i, new List<string>() { "-1", "-1", "-1", "-1", "-1", "-1" }));
            }

            InitHeroesGroup();
            InitHeroLevelUpCosts();
            InitHeroStarUpStats();
            InitHeroStarUpCosts();
            InitHeroDisassembles();
            InitSummonInfos();
            InitHeroEvolutions();
        }

        public override void PostLoad()
        {
            base.PostLoad();

            //kiểm tra trong có ít nhất một hero trong một đội hình hay chưa
            var hasHero = false;
            var count = formations.Length;
            for (int i = 0; i < count; i++)
            {
                var formation = formations[i];
                var fcount = formation.Count;
                for (int j = 0; j < fcount; j++)
                {
                    var item = formation[j];
                    if (!item.Equals("-1"))
                    {
                        hasHero = true;
                        break;
                        break;
                    }
                }
            }

            if (!hasHero)
            {
                //nếu không có hero và đã từng claim hero thì tặng nó thằng cùi John
                if (countClaimHero.Value > 0 && heroesGroup.Count <= 0)
                {
                    ClaimHero(Constants.SUMMON_HERO_ID_1);
                }

                //tạo formation mới và add hero power cao nhất vào
                var newFormation = new List<string>();
                var formation = formations[currentFormation.Value];
                var fCount = formation.Count;

                //get list sort heroData
                var heroDatas = GetAllHeroDatas();
                count = heroDatas.Count;
                // var min = GameData.Instance.BaseGroup.GetCurrentBase().slotLimit;
                var min = Constants.FORMATION_SLOT_NUMBER;
                if (count < min) min = count;
                for (int i = 0; i < min; i++)
                {
                    newFormation.Add(heroDatas[i].id + "");
                }
                for (int i = min; i < fCount; i++)
                {
                    newFormation.Add("-1");
                }
                SaveEquippedHeros(newFormation, currentFormation.Value);
            }
        }

        public void NewDay()
        {
            freePowerFragmentSummon.Value = true;
            freePowerCrystalSummon.Value = true;
        }

        public HeroData GetHeroData(int _heroId)
        {
            if (_heroId != -1)
            {
                var currentHeroDatas = GetAllHeroDatasCache(); //get cache
                var count = currentHeroDatas.Count;
                for (int i = 0; i < count; i++)
                {
                    var heroData = currentHeroDatas[i];
                    if (heroData.id == _heroId)
                    {
                        return heroData;
                    }
                }
            }

            return null;
        }
        public HeroData GetHeroDataAll(int _heroId)
        {
            if (_heroId != -1)
            {
                var currentHeroDatas = GetAllHeroDatasCache(); //get cache
                var count = currentHeroDatas.Count;
                for (int i = 0; i < count; i++)
                {
                    var heroData = currentHeroDatas[i];
                    if (heroData.id == _heroId)
                    {
                        return heroData;
                    }
                }
            }

            return null;
        }


        public List<HeroData> GetAllHeroDatas()
        {
            var heroDatas = new List<HeroData>(GetAllHeroDatasCache());
            heroDatas.Sort();
            return heroDatas;
        }

        public List<HeroData> GetAllHeroDatasCache()
        {
            if (cacheHeroDatas != null) return cacheHeroDatas;
            else
            {
                cacheHeroDatas = new List<HeroData>();
                for (int i = 0; i < heroesGroup.Count; i++)
                {
                    var heroData = JsonUtility.FromJson<HeroData>(heroesGroup[i]);
                    heroData.Init();
                    cacheHeroDatas.Add(heroData);
                }

                return cacheHeroDatas;
            }
        }

        public List<HeroData> GetAllHeroDatasUnEquip()
        {
            var list = new List<HeroData>();
            var equipped = false;

            var currentHeroDatas = GetAllHeroDatasCache(); //get cache
            var count = currentHeroDatas.Count;
            for (int i = 0; i < count; i++)
            {
                equipped = false;
                var heroData = currentHeroDatas[i];

                if (GameUnityData.instance.GameRemoteConfig.Function_PvP_Active)
                {
                    for (int j = 0; j < MAX_FORMATION; j++)
                    {
                        if (heroData.IsEquipped(j))
                        {
                            equipped = true;
                            break;
                        }
                    }
                }
                else
                {
                    //for (int j = 0; j < MAX_FORMATION; j++)
                    //{
                    if (heroData.IsEquipped(0))
                    {
                        equipped = true;
                        //break;
                    }
                    //}
                }



                if (!equipped)
                {
                    list.Add(heroData);
                }
            }
            list.Sort();
            return list;
        }

        public void ClaimHero(int _baseId)
        {
            int count = countClaimHero.Value;

            var heroData = new HeroData(count, _baseId); //với mỗi heroData mới tạo id ++
            heroesGroup.Add(JsonUtility.ToJson(heroData));
            var currentHeroDatas = GetAllHeroDatasCache(); //get cache
            currentHeroDatas.Add(heroData); //update cache

            countClaimHero.Value++;

            //Daily Quest và Achievement
            if (heroData.Rank == IDs.RANK_SS) GameData.Instance.AchievementsGroup.SummonRankSS();
            else if (heroData.Rank == IDs.RANK_S) GameData.Instance.AchievementsGroup.SummonRankS();

            EventDispatcher.Raise(new FormationChangeEvent());
        }

        public void ClaimAllHeroes()
        {
            foreach (var heroDefinition in heroDefinitions)
            {
                ClaimHero(heroDefinition.Value.id);
            }
        }

        public void UpdateHeroData(HeroData _heroData)
        {
            var currentHeroDatas = GetAllHeroDatasCache(); //get cache
            var count = currentHeroDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var heroData = currentHeroDatas[i];
                if (heroData.id == _heroData.id)
                {
                    //data & cache
                    heroesGroup[i] = JsonUtility.ToJson(_heroData);
                    currentHeroDatas[i] = _heroData;
                    return;
                }
            }
        }

        public bool HasEquippedHero(int _formationIndex, int _Id)
        {
            return formations[_formationIndex].Contain(_Id + "");
        }

        public HeroData[] GetEquippedHeroes(int _formationIndex) //có thể chứa null
        {
            var formation = formations[_formationIndex];
            int count = formation.Count;
            var heroes = new HeroData[count];
            for (int i = 0; i < count; i++)
            {
                int id = int.Parse(formation[i]);
                if (id != -1) heroes[i] = GetHeroData(id);
                else heroes[i] = null;
            }
            return heroes;
        }

        //
        public void SetCurrentFormation(int index)
        {
            currentFormation.Value = index;
        }

        /// <summary>
        /// Remove Hero on Lock Position:
        /// </summary>
        public void CheckFormation()
        {
            HeroData[][] formationsDatas = new HeroData[HeroesGroup.MAX_FORMATION][];
            int limit = Constants.FORMATION_SLOT_NUMBER;
            for (int i = 0; i < HeroesGroup.MAX_FORMATION; i++)
            {
                //if (i < limit)
                HeroData[] datas = this.GetEquippedHeroes(i);
                formationsDatas[i] = new HeroData[datas.Length];
                for (int j = 0; j < datas.Length; j++)
                {
                    if (j < limit)
                        formationsDatas[i][j] = datas[j];
                    else
                        formationsDatas[i][j] = null;
                }
                //formationsDatas[i] = this.GetEquippedHeroes(i);
                //	else
                //		formationsDatas[i] = null;
            }
            var count = formationsDatas.Length;
            var newFormations = new List<List<string>>();
            for (int i = 0; i < count; i++)
            {
                var newFormation = new List<string>();
                var formation = formationsDatas[i];
                var fCount = formation.Length;
                for (int j = 0; j < fCount; j++)
                {
                    var heroData = formation[j];
                    if (heroData == null) newFormation.Add("-1");
                    else newFormation.Add(formation[j].id + "");
                }

                newFormations.Add(newFormation);
            }
            this.SaveEquippedHeros(newFormations);
        }
        public HeroData[] GetEquippedHeroes() //có thể chứa null
        {
            var formation = formations[currentFormation.Value];
            int count = formation.Count;
            var heroes = new HeroData[count];
            for (int i = 0; i < count; i++)
            {
                int id = int.Parse(formation[i]);
                if (id != -1) heroes[i] = GetHeroData(id);
                else heroes[i] = null;
            }
            return heroes;
        }
        public string GetEquippedHeroesString() //có thể chứa null
        {
            string output = "";
            HeroData[] formationData = this.GetEquippedHeroes();
            int count = formationData.Length;
            for (int i = 0; i < count; i++)
            {
                HeroData item = formationData[i];
                if (item == null)
                {
                    output += -1 + ",";
                }
                else
                {
                    output += item.baseId + ",";
                }
            }
            return output;
        }

        public int GetEquippedHeroesCP() //có thể chứa null
        {
            int cp = 0;
            HeroData[] listHeroDatas = GameData.Instance.HeroesGroup.GetEquippedHeroes();
            int lenght = listHeroDatas.Length;
            for (int i = 0; i < lenght; i++)
            {
                var heroData = listHeroDatas[i];
                if (heroData != null)
                {
                    cp += heroData.Power;
                }
            }
            return cp;
        }

        public bool IsMaxEquippedHeros(int _formationIndex)
        {
            var formation = formations[_formationIndex];
            int count = formation.Count;
            int countEquipped = 0;
            for (int i = 0; i < count; i++)
            {
                string id = formation[i];
                if (!id.Equals("-1")) countEquipped++;
            }

            return countEquipped >= Constants.MAX_HEROES_CAN_EQUIP;
        }

        public bool EquipHero(int _formationIndex, int _Id)
        {
            if (IsMaxEquippedHeros(_formationIndex)) return false;

            var heroData = GetHeroData(_Id);
            if (heroData.IsEquipped(_formationIndex)) return false;

            var formation = formations[_formationIndex];
            int count = formation.Count;
            for (int i = 0; i < count; i++)
            {
                if (formation[i].Equals("-1"))
                {
                    formation[i] = _Id + "";
                    break;
                }
            }

            EventDispatcher.Raise(new FormationChangeEvent());

            return true;
        }

        public void SaveEquippedHeros(List<List<string>> _formations)
        {
            var count = _formations.Count;
            for (int i = 0; i < count; i++)
            {
                var formation = formations[i];
                formation.Values = _formations[i];
            }

            EventDispatcher.Raise(new FormationChangeEvent());
        }

        public void SaveEquippedHeros(List<string> _formation, int _formationIndex)
        {
            var formation = formations[_formationIndex];
            formation.Values = _formation;

            EventDispatcher.Raise(new FormationChangeEvent());
        }

        public List<RewardInfo> DisassembleHero(HeroData _heroData)
        {
            var rewardInfos = _heroData.GetRewardFromDisassembleHero();
            RemoveHeroData(_heroData);

            //Daily Quest và Achievement
            GameData.Instance.CampaignsGroup.AddDisassembleCount();
            return rewardInfos;
        }

        public List<RewardInfo> GetRewardFromDisassembleHeroes(List<HeroData> _heroDatas)
        {
            var count = _heroDatas.Count;
            var totalRewardInfos = new List<RewardInfo>();
            bool isNewReward = true;
            for (int i = 0; i < count; i++)
            {
                var _heroData = _heroDatas[i];
                var rewardInfos = _heroData.GetRewardFromDisassembleHero();
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

        public List<RewardInfo> DisassembleHeroes(List<HeroData> _heroDatas)
        {
            RemoveHeroDatas(_heroDatas);

            //Daily Quest và Achievement
            GameData.Instance.CampaignsGroup.AddDisassembleCount(_heroDatas.Count);
            return GetRewardFromDisassembleHeroes(_heroDatas);
        }

        private void RemoveHeroData(HeroData _heroData)
        {
            var currentHeroDatas = GetAllHeroDatasCache(); //get cache
            var count = currentHeroDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var heroData = currentHeroDatas[i];
                if (heroData.id == _heroData.id)
                {
                    //cởi gear trước khi remove
                    if (_heroData.GearDatas.Count > 0) _heroData.UnEquipAllGear();

                    //data & cache
                    heroesGroup.Remove(heroesGroup[i]);
                    currentHeroDatas.RemoveAt(i);

                    EventDispatcher.Raise(new FormationChangeEvent());

                    return;
                }
            }
        }

        private void RemoveHeroDatas(List<HeroData> _heroDatas)
        {
            var gearsGroup = GameData.Instance.GearsGroup;

            var heroDatas = new List<HeroData>(_heroDatas); //để nó còn check cho hàm GetRewardFromDisassembleHeroes

            var currentHeroDatas = GetAllHeroDatasCache(); //get cache
            var count = currentHeroDatas.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                //tìm trong list Hero cái cần remove và tìm được cái nào thì bỏ đi lần sau lặp lại cái chưa tìm
                var heroData = currentHeroDatas[i];
                var countHeroesRemove = heroDatas.Count;
                for (int j = countHeroesRemove - 1; j >= 0; j--)
                {
                    var _heroDataRemove = heroDatas[j];
                    if (heroData.id == _heroDataRemove.id)
                    {
                        //cởi gear trước khi remove
                        if (_heroDataRemove.GearDatas.Count > 0) _heroDataRemove.UnEquipAllGear();

                        //data & cache
                        heroesGroup.Remove(heroesGroup[i]);
                        currentHeroDatas.RemoveAt(i);

                        heroDatas.RemoveAt(j);
                        break;
                    }
                }

                if (countHeroesRemove == 0)
                {
                    EventDispatcher.Raise(new FormationChangeEvent());

                    return;
                }
            }
        }

        public HeroDefinition GetHeroDefinition(int _heroBaseId)
        {
            try
            {
                return heroDefinitions[_heroBaseId];
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        //level up
        public bool IsMaxHeroLevelUp(int level)
        {
            return level >= heroLevelUpCosts.Count;
        }

        public HeroLevelUpStat GetHeroLevelUpStat(int _heroBaseId)
        {
            return heroLevelUpStats[_heroBaseId];
        }

        public HeroLevelUpCost GetHeroLevelUpCost(int level)
        {
            //	Debug.LogError("Max level " + heroLevelUpCosts.Count);
            if (level >= heroLevelUpCosts.Count)
            {
                return heroLevelUpCosts[heroLevelUpCosts.Count - 1];
            }
            return heroLevelUpCosts[level];
        }

        //star up
        public int GetMaxHeroStarUp(int rank)
        {
            return heroStarUpStats[rank].valueUps.Length;
        }

        public float GetPercentHeroStarUpStat(int rank, int star)
        {
            return heroStarUpStats[rank].valueUps[star - 1] / 100f;// - 1 vì lấy giá trị hiện tại
        }

        public int GetHeroStarUpCost(int star)
        {
            return heroStarUpCosts[star].elementDust;// ko - 1 vì lấy giá trị next
        }

        public HeroDisassemble GetHeroDisassemble(int rank, int star)
        {
            return heroDisassembles[rank][star - 1];
        }

        //summon hero
        public RewardInfo SummonByBlueHeroFragment()
        {
            var countSummon = countPowerFragmentSummon.Value;
            var count = powerFragmentSummonInfos.Count;
            var ids = new int[count];
            var chances = new float[count];
            for (int i = 0; i < count; i++)
            {
                var summonInfo = powerFragmentSummonInfos[i];
                ids[i] = summonInfo.id;
                chances[i] = summonInfo.dropRate + summonInfo.dropChange * countSummon;
            }

            int idChoice = 0;
            //nếu là lần đầu tiên thì fix sẵn ra hero mặc định
            if (countClaimHero.Value <= 0)
            {
                idChoice = Constants.SUMMON_HERO_ID_1;
            }
            else
            {
                HeroDefinition heroDefinition = null;
                while (heroDefinition == null || !heroDefinition.active)
                {
                    int rewardChoice = LogicAPI.CalcRandomWithChances(chances);
                    idChoice = ids[rewardChoice];
                    heroDefinition = GetHeroDefinition(idChoice);
                }

                //Các tỉ lệ sẽ reset về ban đầu khi roll ra 5 Hero cấp A hoặc 1 hero S
                if (heroDefinition.rank == IDs.RANK_A) countPowerFragmentRollToRankA.Value++;
                else if (heroDefinition.rank == IDs.RANK_S) countPowerFragmentRollToRankS.Value++;
                if (countPowerFragmentRollToRankA.Value >= 5 || countPowerFragmentRollToRankS.Value >= 1)
                {
                    countPowerFragmentSummon.Value = 0;
                    countPowerFragmentRollToRankA.Value = 0;
                    countPowerFragmentRollToRankS.Value = 0;
                }
                else
                {
                    if (countSummon < 20) countPowerFragmentSummon.Value++;
                }
            }

            return new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, idChoice, 1);
        }

        public RewardInfo SummonByEpicHeroFragment()
        {
            var countSummon = countPowerCrystalSummon.Value;
            var count = powerCrystalSummonInfos.Count;
            var ids = new int[count];
            var chances = new float[count];
            for (int i = 0; i < count; i++)
            {
                var summonInfo = powerCrystalSummonInfos[i];
                ids[i] = summonInfo.id;
                chances[i] = summonInfo.dropRate + summonInfo.dropChange * countSummon;
            }

            int idChoice = 0;
            //nếu là lần đầu tiên thì fix sẵn ra hero mặc định
            if (countClaimHero.Value <= 1)
            {
                idChoice = Constants.SUMMON_HERO_ID_2;
            }
            else
            {
                HeroDefinition heroDefinition = null;
                while (heroDefinition == null || !heroDefinition.active)
                {
                    int rewardChoice = LogicAPI.CalcRandomWithChances(chances);
                    idChoice = ids[rewardChoice];
                    heroDefinition = GetHeroDefinition(idChoice);
                }

                //Các tỉ lệ sẽ reset về ban đầu khi roll ra 5 Hero cấp A hoặc 1 hero S
                heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(idChoice);
                if (heroDefinition.rank == IDs.RANK_A) countPowerCrystalRollToRankA.Value++;
                else if (heroDefinition.rank == IDs.RANK_S) countPowerCrystalRollToRankS.Value++;
                if (countPowerCrystalRollToRankA.Value >= 5 || countPowerCrystalRollToRankS.Value >= 1)
                {
                    countPowerCrystalSummon.Value = 0;
                    countPowerCrystalRollToRankA.Value = 0;
                    countPowerCrystalRollToRankS.Value = 0;
                }
                else
                {
                    if (countSummon < 20) countPowerCrystalSummon.Value++;
                }
            }

            return new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, idChoice, 1);
        }

        public RewardInfo SummonByPowerFragment()
        {
            var countSummon = countPowerFragmentSummon.Value;
            var count = powerFragmentSummonInfos.Count;
            var ids = new int[count];
            var chances = new float[count];
            for (int i = 0; i < count; i++)
            {
                var summonInfo = powerFragmentSummonInfos[i];
                ids[i] = summonInfo.id;
                chances[i] = summonInfo.dropRate + summonInfo.dropChange * countSummon;
            }

            int idChoice = 0;
            //nếu là lần đầu tiên thì fix sẵn ra hero mặc định
            if (countClaimHero.Value <= 0)
            {
                idChoice = Constants.SUMMON_HERO_ID_1;
            }
            else
            {
                HeroDefinition heroDefinition = null;
                while (heroDefinition == null || !heroDefinition.active)
                {
                    int rewardChoice = LogicAPI.CalcRandomWithChances(chances);
                    idChoice = ids[rewardChoice];
                    heroDefinition = GetHeroDefinition(idChoice);
                }

                //Các tỉ lệ sẽ reset về ban đầu khi roll ra 5 Hero cấp A hoặc 1 hero S
                if (heroDefinition.rank == IDs.RANK_A) countPowerFragmentRollToRankA.Value++;
                else if (heroDefinition.rank == IDs.RANK_S) countPowerFragmentRollToRankS.Value++;
                if (countPowerFragmentRollToRankA.Value >= 5 || countPowerFragmentRollToRankS.Value >= 1)
                {
                    countPowerFragmentSummon.Value = 0;
                    countPowerFragmentRollToRankA.Value = 0;
                    countPowerFragmentRollToRankS.Value = 0;
                }
                else
                {
                    if (countSummon < 20) countPowerFragmentSummon.Value++;
                }
            }

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.BasicSummon();
            GameData.Instance.CampaignsGroup.AddSummonBasicCount();

            return new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, idChoice, 1);
        }

        public void ClaimFreePowerFragmentSummon()
        {
            freePowerFragmentSummon.Value = false;
        }

        public void UpdataClaimHeroValues()
        {
            countClaimHero.Value++;
            //  countClaimHerox10.Value++;
        }
        public RewardInfo SummonByPowerCrytal(int source = 0)
        {
            var countSummon = countPowerCrystalSummon.Value;
            var count = powerCrystalSummonInfos.Count;
            var ids = new int[count];
            var chances = new float[count];
            for (int i = 0; i < count; i++)
            {
                var summonInfo = powerCrystalSummonInfos[i];
                ids[i] = summonInfo.id;
                chances[i] = summonInfo.dropRate + summonInfo.dropChange * countSummon;
            }

            int idChoice = 0;
            //nếu là lần đầu tiên thì fix sẵn ra hero mặc định
            if (countClaimHero.Value <= 1)
            {
                // if (countClaimHero.Value <= 1)
                idChoice = Constants.SUMMON_HERO_ID_2;
                //    idChoice = IDs.HA19;
                //   else
                //        idChoice = UnityEngine.Random.Range(IDs.HS1, IDs.HS20);
            }
            else
            if (source == 1 && countClaimHerox10.Value < 1)
            {
                //Fake x10 first time
                //    idChoice = UnityEngine.Random.Range(IDs.HS1, IDs.HS20);
                idChoice = IDs.HS1;
                countClaimHerox10.Value++;
            }
            else
            {
                HeroDefinition heroDefinition = null;
                while (heroDefinition == null || !heroDefinition.active)
                {
                    int rewardChoice = LogicAPI.CalcRandomWithChances(chances);
                    idChoice = ids[rewardChoice];
                    heroDefinition = GetHeroDefinition(idChoice);
                }

                //Các tỉ lệ sẽ reset về ban đầu khi roll ra 5 Hero cấp A hoặc 1 hero S
                heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(idChoice);
                if (heroDefinition.rank == IDs.RANK_A) countPowerCrystalRollToRankA.Value++;
                else if (heroDefinition.rank == IDs.RANK_S) countPowerCrystalRollToRankS.Value++;
                if (countPowerCrystalRollToRankA.Value >= 5 || countPowerCrystalRollToRankS.Value >= 1)
                {
                    countPowerCrystalSummon.Value = 0;
                    countPowerCrystalRollToRankA.Value = 0;
                    countPowerCrystalRollToRankS.Value = 0;
                }
                else
                {
                    if (countSummon < 20) countPowerCrystalSummon.Value++;
                }
            }

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.SeniorSummon();
            GameData.Instance.AchievementsGroup.SeniorSummon();
            GameData.Instance.CampaignsGroup.AddSummonSeniorCount();

            return new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, idChoice, 1);
        }

        public void ClaimFreePowerCrystalSummon()
        {
            freePowerCrystalSummon.Value = false;
        }

        public RewardInfo AddCountPowerCrystalSummon()
        {
            countPowerCrystalSummonByGem.Value++;

            if (countPowerCrystalSummonByGem.Value >= 15)
            {
                countPowerCrystalSummonByGem.Value = 0;
                return new RewardInfo(IDs.REWARD_TYPE_CURRENCY, IDs.CURRENCY_DEVINE_CRYSTAL, 1);
            }
            else
            {
                return null;
            }
        }

        public RewardInfo SummonByDevineCrystal()
        {
            var countSummon = countDevineCrystalSummon.Value;
            var count = devineCrystalSummonInfos.Count;
            var ids = new int[count];
            var chances = new float[count];
            for (int i = 0; i < count; i++)
            {
                var summonInfo = devineCrystalSummonInfos[i];
                ids[i] = summonInfo.id;
                chances[i] = summonInfo.dropRate + summonInfo.dropChange * countSummon;
            }

            int idChoice = 0;
            HeroDefinition heroDefinition = null;
            while (heroDefinition == null || !heroDefinition.active)
            {
                int rewardChoice = LogicAPI.CalcRandomWithChances(chances);
                idChoice = ids[rewardChoice];
                heroDefinition = GetHeroDefinition(idChoice);
            }

            //Các tỉ lệ sẽ reset về ban đầu khi roll ra 5 Hero cấp A hoặc 1 hero S
            heroDefinition = GameData.Instance.HeroesGroup.GetHeroDefinition(idChoice);
            if (heroDefinition.rank == IDs.RANK_S) countDevineCrystalRollToRankS.Value++;
            if (countDevineCrystalRollToRankS.Value >= 1)
            {
                countDevineCrystalSummon.Value = 0;
                countDevineCrystalRollToRankS.Value = 0;
            }
            else
            {
                if (countSummon < 20) countDevineCrystalSummon.Value++;
            }

            //Daily Quest và Achievement
            GameData.Instance.DailyQuestsGroup.SeniorSummon();
            GameData.Instance.AchievementsGroup.SeniorSummon();
            GameData.Instance.CampaignsGroup.AddSummonSeniorCount();

            return new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, idChoice, 1);
        }

        //hero evolution
        public List<HeroEvolutionInfo> GetAllHeroEvolutionInfos()
        {
            return heroEvolutionInfos;
        }

        public HeroEvolutionInfo GetEvolutionInfoFromHeroBaseId(int heroBaseId)
        {
            var count = heroEvolutionInfos.Count;
            for (int i = 0; i < count; i++)
            {
                var heroEvolutionInfo = heroEvolutionInfos[i];
                if (heroEvolutionInfo.idMaterial_1 == heroBaseId)
                {
                    return heroEvolutionInfo;
                }
            }

            return null;
        }

        public HeroData Evolution(HeroEvolutionInfo heroEvolutionInfo, HeroData mainHeroEvolution, HeroData heroData2, HeroData heroData3, HeroData heroData4, HeroData heroData5)
        {
            var newHero = new HeroData(mainHeroEvolution.id, heroEvolutionInfo.heroId);
            newHero.levelUnlocked = mainHeroEvolution.levelUnlocked;
            newHero.star = mainHeroEvolution.star;
            mainHeroEvolution = newHero; //gán này vào chơi, sau có dùng tiếp biến mainHeroEvolution thôi, chứ lưu trữ ở hàm UpdateHeroData nó làm hết
            mainHeroEvolution.Init();
            UpdateHeroData(mainHeroEvolution);

            RemoveHeroData(heroData2);
            RemoveHeroData(heroData3);
            RemoveHeroData(heroData4);
            RemoveHeroData(heroData5);

            EventDispatcher.Raise(new HeroEvolutionEvent());
            //Daily Quest và Achievement
            if (mainHeroEvolution.Rank == IDs.RANK_SS) GameData.Instance.AchievementsGroup.SummonRankSS();
            GameData.Instance.CampaignsGroup.AddEvolutionHeroCount();
            return mainHeroEvolution;
        }

        //noti
        public bool CheckHeroUpgradeNoti()
        {
            var currentHeroDatas = GetAllHeroDatasCache(); //get cache
            var count = currentHeroDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var heroData = currentHeroDatas[i];
                if (heroData.IsEquipped() && (heroData.CanLevelUp() || heroData.CanOneClickEquip()))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckHeroStarUpNoti()
        {
            var currentHeroDatas = GetAllHeroDatasCache(); //get cache
            var count = currentHeroDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var heroData = currentHeroDatas[i];
                if (heroData.IsEquipped() && heroData.CanStarUp())
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckFormationNoti()
        {
            int slotLimit = Constants.FORMATION_SLOT_NUMBER;
            int heroesCount = heroesGroup.Count;

            var formation = formations[0];
            int countSlotUnEquip = 0;
            int countSlotEquip = 0;
            for (int i = 0; i < slotLimit; i++)
            {
                int id = int.Parse(formation[i]);
                if (id == -1) countSlotUnEquip++;
                else countSlotEquip++;
            }
            //nếu số slot trống != 0 và dư hero thì noti
            if (countSlotUnEquip != 0 && heroesCount > countSlotEquip)
            {
                return true;
            }

            return false;
        }

        #endregion

        //==============================================

        #region Private

        private void InitHeroesGroup()
        {
            var dataContent = GameData.GetTextContent("Data/Hero");
            var listHeroDefinition = JsonHelper.GetJsonList<HeroDefinition>(dataContent);
            heroDefinitions = new Dictionary<int, HeroDefinition>();
            foreach (var item in listHeroDefinition)
            {
                heroDefinitions.Add(item.id, item);
            }
            dataContent = GameData.GetTextContent("Data/HeroLevelUpStat");
            var listHeroLevelUpStatDefinition = JsonHelper.GetJsonList<HeroLevelUpStat>(dataContent);
            heroLevelUpStats = new Dictionary<int, HeroLevelUpStat>();
            foreach (var item in listHeroLevelUpStatDefinition)
            {
                heroLevelUpStats.Add(item.id, item);
            }
        }

        private void InitHeroLevelUpCosts()
        {
            //cái này theo level tăng dần nên chỉ cần dùng List
            var dataContent = GameData.GetTextContent("Data/HeroLevelUpCost");
            heroLevelUpCosts = JsonHelper.GetJsonList<HeroLevelUpCost>(dataContent);
            heroLevelUpCosts.Sort();
        }

        private void InitHeroStarUpStats()
        {
            var dataContent = GameData.GetTextContent("Data/HeroStarUpStat");
            var listHeroStarUpStats = JsonHelper.GetJsonList<HeroStarUpStat>(dataContent);
            heroStarUpStats = new Dictionary<int, HeroStarUpStat>();
            foreach (var item in listHeroStarUpStats)
            {
                heroStarUpStats.Add(item.rank, item);
            }
        }

        private void InitHeroStarUpCosts()
        {
            //cái này theo star tăng dần nên chỉ cần dùng List
            var dataContent = GameData.GetTextContent("Data/HeroStarUpCost");
            heroStarUpCosts = JsonHelper.GetJsonList<HeroStarUpCost>(dataContent);
        }

        private void InitHeroDisassembles()
        {
            var dataContent = GameData.GetTextContent("Data/HeroDisassemble");
            var listHeroDisassembles = JsonHelper.GetJsonList<HeroDisassemble>(dataContent);

            //nhóm các HeroDisassemble cùng rank vào một list chung theo thứ tự
            //lúc sau mình chỉ cần get theo rank và theo thứ tự là star - 1
            var lastRank = -1;
            heroDisassembles = new Dictionary<int, List<HeroDisassemble>>();
            List<HeroDisassemble> subHeroDisassembles = null;
            foreach (var item in listHeroDisassembles)
            {
                if (lastRank != item.rank)
                {
                    subHeroDisassembles = new List<HeroDisassemble>();
                    heroDisassembles.Add(item.rank, subHeroDisassembles);
                    lastRank = item.rank;
                }
                subHeroDisassembles.Add(item);
            }
        }

        private void InitSummonInfos()
        {
            var dataContent = GameData.GetTextContent("Data/PowerFragmentSummon");
            powerFragmentSummonInfos = JsonHelper.GetJsonList<SummonInfo>(dataContent);

            dataContent = GameData.GetTextContent("Data/PowerCrystalSummon");
            powerCrystalSummonInfos = JsonHelper.GetJsonList<SummonInfo>(dataContent);

            dataContent = GameData.GetTextContent("Data/DevineCrystalSummon");
            devineCrystalSummonInfos = JsonHelper.GetJsonList<SummonInfo>(dataContent);
        }

        private void InitHeroEvolutions()
        {
            var dataContent = GameData.GetTextContent("Data/HeroEvolution");
            heroEvolutionInfos = JsonHelper.GetJsonList<HeroEvolutionInfo>(dataContent);
        }

        #endregion
    }
}
