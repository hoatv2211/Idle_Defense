using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;

namespace FoodZombie
{
    [System.Serializable]
    public class EnemyDefinition : IComparable<EnemyDefinition>
    {
        public int id;
        public string name;
        public int[] attackTypes;
        public int race;
        public int element;
        public int targetToHero;
        public string AI;
        public float cooldown;
        public float hp;
        public float hpRegen;
        public float damage;
        public float movement;
        public float armor;
        public float attackSpeed;
        public float accuracy;
        public float dodge;
        public float critRate;
        public float critDamage;
        public float attackRange;
        public bool invisible;
        public int coinDrop;
        public int gemDrop;
        public float[] bulletSpeed;

        public int CompareTo(EnemyDefinition other)
        {
            return id.CompareTo(other.id);
        }
    }

    [System.Serializable]
    public class EnemyLevelUpStat : IComparable<EnemyLevelUpStat>
    {
        public int id;
        public string name;
        public int lvUp;
        public float hp;
        public float hpRegen;
        public float damage;
        public float movement;
        public float armor;
        public float attackSpeed;
        public float accuracy;
        public float dodge;
        public float critRate;
        public float critDamage;
        public float attackRange;
        public int coinDrop;
        public int gemDrop;
        public float[] bulletSpeed;
        public int CompareTo(EnemyLevelUpStat other)
        {
            return id.CompareTo(other.id);
        }
    }

    public class EnemyData : DataGroup
    {
        public EnemyDefinition baseData { get; private set; }
        public EnemyLevelUpStat levelUpStat;
        private BoolData unlocked;
        private BoolData viewed;
        private IntegerData killsCount;

        public bool Unlocked => unlocked.Value;
        public bool Viewed => viewed.Value;
        public int KillsCount => killsCount.Value;

        #region Info

        public string Name => baseData.name;
        public int[] AttackTypes => baseData.attackTypes;
        public int Race => baseData.race;
        public int Element => baseData.element;
        public int TargetToHero => baseData.targetToHero;
        public float Cooldown => baseData.cooldown;

        public float GetHP(int level)
        {
            return ConfigStats.GetStat(baseData.hp, levelUpStat.hp, level);
        }
        public float GetHPRegen(int level)
        {
            return ConfigStats.GetStat(baseData.hpRegen, levelUpStat.hpRegen, level);
        }
        public float GetDamage(int level)
        {
            return ConfigStats.GetStat(baseData.damage, levelUpStat.damage, level);
        }
        public float GetMovement(int level)
        {
            return ConfigStats.GetStat(baseData.movement, levelUpStat.movement, level) * ConfigStats.GetXSpeed();
        }
        public float GetArmor(int level)
        {
            return ConfigStats.GetStat(baseData.armor, levelUpStat.armor, level);
        }
        public float GetAttackSpeed(int level)
        {
            return ConfigStats.GetStat(baseData.attackSpeed, levelUpStat.attackSpeed, level);
        }
        public float[] GetBulletSpeed(int level)
        {
            return ConfigStats.GetStat(baseData.bulletSpeed, levelUpStat.bulletSpeed, level);
        }
        public float GetAccuracy(int level)
        {
            return ConfigStats.GetStat(baseData.accuracy, levelUpStat.accuracy, level);
        }
        public float GetDodge(int level)
        {
            return ConfigStats.GetStat(baseData.dodge, levelUpStat.dodge, level);
        }
        public float GetCritRate(int level)
        {
            return ConfigStats.GetStat(baseData.critRate, levelUpStat.critRate, level);
        }
        public float GetCritDamage(int level)
        {
            return ConfigStats.GetStat(baseData.critDamage, levelUpStat.critDamage, level);
        }
        public float GetAttackRange(int level)
        {
            return ConfigStats.GetStat(baseData.attackRange, levelUpStat.attackRange, level);
        }
        public int GetCoinDrop(int level)
        {
            return ConfigStats.GetStat(baseData.coinDrop, levelUpStat.coinDrop, level);
        }
        public float GetGemDrop(int level)
        {
            return ConfigStats.GetStat(baseData.gemDrop, levelUpStat.gemDrop, level);
        }

        #endregion

        public bool Invisible => baseData.invisible;

        public EnemyData(int _id, EnemyDefinition _baseData, EnemyLevelUpStat _levelUpStat) : base(_id)
        {
            baseData = _baseData;
            levelUpStat = _levelUpStat;
            unlocked = AddData(new BoolData(0, false));
            viewed = AddData(new BoolData(1, false));
            killsCount = AddData(new IntegerData(2));
        }

        public void Unlock()
        {
            unlocked.Value = true;
        }

        public void View()
        {
            viewed.Value = true;
        }

        public Spine.Unity.SkeletonDataAsset GetSkeletonData()
        {
            return AssetsCollection.instance.enemyAnimations.GetAsset(Id - 1);
        }

        public string GetSkinName()
        {
            //return baseData.skin;
            return "skin1";
        }

        public Sprite GetIcon()
        {
            return AssetsCollection.instance.enemyIcon.GetAsset(Id - 1);
        }

        public Sprite GetElementIcon()
        {
            return AssetsCollection.instance.elementIcon.GetAsset(baseData.element - 1);
        }

        public void AddKillsCount(int pValue)
        {
            killsCount.Value += pValue;
        }
    }

    public class EnemiesGroup : DataGroup
    {
        #region Members

        private DataGroup enemiesGroup;
        private DataGroup enemiesTutorialGroup;
        private IntegerData lastZombieViewedId;

        public int LastZombieViewedId { get { return lastZombieViewedId.Value; } }
        #endregion

        //=============================================

        #region Public

        public EnemiesGroup(int pId) : base(pId)
        {
            //Declare sub groups which contain units data
            enemiesGroup = AddData(new DataGroup(0));
            enemiesTutorialGroup = AddData(new DataGroup(2));
            lastZombieViewedId = AddData(new IntegerData(1, 0));

            InitEnemiesGroup();
            InitEnemiesTutorialGroup();
        }

        public EnemyData GetEnemyData(int pId)
        {
            return enemiesGroup.GetData<EnemyData>(pId);
        }

        public EnemyData GetRandomEnemy()
        {
            return enemiesGroup.GetRandomData<EnemyData>();
        }

        public List<EnemyData> GetAllEnemyDatas()
        {
            var list = new List<EnemyData>();
            foreach (EnemyData item in enemiesGroup.Children)
                list.Add(item);
            return list;
        }

        public List<EnemyData> GetAllEnemyDatasTutorial()
        {
            var list = new List<EnemyData>();
            foreach (EnemyData item in enemiesTutorialGroup.Children)
                list.Add(item);
            return list;
        }

        public void UnlockEnemy(int pId)
        {
            var unit = GetEnemyData(pId);
            if (unit != null)
                unit.Unlock();
        }

        public void UnlockAllEnemies()
        {
            foreach (EnemyData unit in enemiesGroup.Children)
            {
                unit.Unlock();
            }
        }

        public void SetLastZombieViewed(int pLastZombieViewedId)
        {
            lastZombieViewedId.Value = (pLastZombieViewedId);
        }

        #endregion

        //==============================================

        #region Private

        private void InitEnemiesGroup()
        {
            //enemy
            var dataContent = GameData.GetTextContent("Data/Enemy");
            var enemyDefinitions = JsonHelper.GetJsonList<EnemyDefinition>(dataContent);
            dataContent = GameData.GetTextContent("Data/EnemyLevelUpStat");
            var enemyLevelUpStatDefinitions = JsonHelper.GetJsonList<EnemyLevelUpStat>(dataContent);
            if (enemyDefinitions != null && enemyLevelUpStatDefinitions != null)
            {
                enemyDefinitions.Sort();
                enemyLevelUpStatDefinitions.Sort();
                for (int i = 0; i < enemyDefinitions.Count; i++)
                {
                    var enemyDefinition = enemyDefinitions[i];
                    var enemyLevelUpStatDefinition = enemyLevelUpStatDefinitions[i];
                    //Declare unit data, then push it into a data group
                    var data = new EnemyData(enemyDefinition.id, enemyDefinition, enemyLevelUpStatDefinition);
                    enemiesGroup.AddData(data);
                }
            }

            //boss
            dataContent = GameData.GetTextContent("Data/Boss");
            enemyDefinitions = JsonHelper.GetJsonList<EnemyDefinition>(dataContent);
            dataContent = GameData.GetTextContent("Data/BossLevelUpStat");
            enemyLevelUpStatDefinitions = JsonHelper.GetJsonList<EnemyLevelUpStat>(dataContent);
            if (enemyDefinitions != null && enemyLevelUpStatDefinitions != null)
            {
                enemyDefinitions.Sort();
                enemyLevelUpStatDefinitions.Sort();
                for (int i = 0; i < enemyDefinitions.Count; i++)
                {
                    var enemyDefinition = enemyDefinitions[i];
                    var enemyLevelUpStatDefinition = enemyLevelUpStatDefinitions[i];
                    //Declare unit data, then push it into a data group
                    var data = new EnemyData(enemyDefinition.id, enemyDefinition, enemyLevelUpStatDefinition);
                    enemiesGroup.AddData(data);
                }
            }
        }

        private void InitEnemiesTutorialGroup()
        {
            //enemy
            var dataContent = GameData.GetTextContent("Data/EnemyTutorial");
            var enemyDefinitions = JsonHelper.GetJsonList<EnemyDefinition>(dataContent);
            dataContent = GameData.GetTextContent("Data/EnemyLevelUpStatTutorial");
            var enemyLevelUpStatDefinitions = JsonHelper.GetJsonList<EnemyLevelUpStat>(dataContent);
            if (enemyDefinitions != null && enemyLevelUpStatDefinitions != null)
            {
                enemyDefinitions.Sort();
                enemyLevelUpStatDefinitions.Sort();
                for (int i = 0; i < enemyDefinitions.Count; i++)
                {
                    var enemyDefinition = enemyDefinitions[i];
                    var enemyLevelUpStatDefinition = enemyLevelUpStatDefinitions[i];
                    //Declare unit data, then push it into a data group
                    var data = new EnemyData(enemyDefinition.id, enemyDefinition, enemyLevelUpStatDefinition);
                    enemiesTutorialGroup.AddData(data);
                }
            }

            //boss
            dataContent = GameData.GetTextContent("Data/BossTutorial");
            enemyDefinitions = JsonHelper.GetJsonList<EnemyDefinition>(dataContent);
            dataContent = GameData.GetTextContent("Data/BossLevelUpStatTutorial");
            enemyLevelUpStatDefinitions = JsonHelper.GetJsonList<EnemyLevelUpStat>(dataContent);
            if (enemyDefinitions != null && enemyLevelUpStatDefinitions != null)
            {
                enemyDefinitions.Sort();
                enemyLevelUpStatDefinitions.Sort();
                for (int i = 0; i < enemyDefinitions.Count; i++)
                {
                    var enemyDefinition = enemyDefinitions[i];
                    var enemyLevelUpStatDefinition = enemyLevelUpStatDefinitions[i];
                    //Declare unit data, then push it into a data group
                    var data = new EnemyData(enemyDefinition.id, enemyDefinition, enemyLevelUpStatDefinition);
                    enemiesTutorialGroup.AddData(data);
                }
            }
        }

        #endregion
    }
}
