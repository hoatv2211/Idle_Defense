
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
    public class Campaign
    {
        public int id;
        public int type;
        public int[] infoValues;
        public int rewardType;
        public int rewardId;
        public int rewardValue;

        public RewardInfo GetReward()
        {
            return new RewardInfo(rewardType, rewardId, rewardValue);
        }

        public string GetDescription()
        {
            var countValue = infoValues.Length;
            string descType = Localization.Get("CAMPAIGN_" + type);
            
            if (type == IDs.CAMPAIGN_TYPE_WIN_MISSION_X)
            {
                var value0 = GetValue0();
                var mission = GameData.Instance.MissionsGroup.GetMissionData(value0);
                if(mission != null) return string.Format(descType/*GameData.Instance.CampaignsGroup.GetDescription(type)*/, mission.GetName());
                else return string.Format(GameData.Instance.CampaignsGroup.GetDescription(type), (value0 / 1000) + "-" + (value0 % 1000));
            }
            else {
                if (countValue == 1) return string.Format(descType/*GameData.Instance.CampaignsGroup.GetDescription(type)*/, GetValue0());
                else if (countValue == 2) return string.Format(descType/*GameData.Instance.CampaignsGroup.GetDescription(type)*/, GetValue0(), GetValue1());
                else return descType/*GameData.Instance.CampaignsGroup.GetDescription(type)*/;
            }
        }

        public int GetMaxCount()
        {
            switch (type)
            {
                case IDs.CAMPAIGN_TYPE_WIN_MISSION_X:
                case IDs.CAMPAIGN_TYPE_SUMMON_BASIC_1_TIME:
                case IDs.CAMPAIGN_TYPE_SUMMON_SENIOR_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_HAVE_1_HERO_LV_Y:
                case IDs.CAMPAIGN_TYPE_EQUIP_GEAR_FOR_1_HERO:
                case IDs.CAMPAIGN_TYPE_UPGRADE_TRAP_1_TIME:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_1_GEAR:
                case IDs.CAMPAIGN_TYPE_UPGRADE_GEAR_1_TIME:
                case IDs.CAMPAIGN_TYPE_EVOLUTION_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_STAR_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_DISCOVERY_1_TIME:
                case IDs.CAMPAIGN_TYPE_FORMATION_HAVE_1_HERO:
                case IDs.CAMPAIGN_TYPE_UPGRADE_BASE_LV_X:
                case IDs.CAMPAIGN_TYPE_USER_LV_X:
                    return 1;
                
                case IDs.CAMPAIGN_TYPE_SUMMON_BASIC_X_TIME:
                case IDs.CAMPAIGN_TYPE_SUMMON_SENIOR_X_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_HERO_X_TIME:
                case IDs.CAMPAIGN_TYPE_HAVE_X_HERO_LV_Y:
                case IDs.CAMPAIGN_TYPE_EQUIP_GEAR_FOR_X_HERO:
                case IDs.CAMPAIGN_TYPE_UPGRADE_TRAP_X_TIME:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_X_GEAR:
                case IDs.CAMPAIGN_TYPE_UPGRADE_GEAR_X_TIME:
                case IDs.CAMPAIGN_TYPE_EVOLUTION_HERO_X_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_STAR_HERO_X_TIME:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_X_TIME:
                case IDs.CAMPAIGN_TYPE_DISCOVERY_X_TIME:
                case IDs.CAMPAIGN_TYPE_FORMATION_HAVE_X_HERO:
                    return GetValue0();
            }

            return 1;
        }
        
        public int GetValue0()
        {
            //bắt đầu từ 1, kết thúc ở 89
            int value = infoValues[0];
            
            //tìm phần dư level
            var index = GameData.Instance.CampaignsGroup.CampaignLevel - 1;
            var xCount = 0;
            if (index >= 89)
            {
                //Từ quest 90 trở đi sẽ loop lại quest 77 đến 89 , mỗi lần yêu cầu quest tăng lên 5 đơn vị. Reward giữ nguyên
                xCount = ((index - 89) / 13) + 1;
            }

            switch (type)
            {
                case IDs.CAMPAIGN_TYPE_WIN_MISSION_X:
                    value += xCount * 1000;
                    break;
                case IDs.CAMPAIGN_TYPE_SUMMON_BASIC_1_TIME:
                case IDs.CAMPAIGN_TYPE_SUMMON_SENIOR_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_HAVE_1_HERO_LV_Y:
                case IDs.CAMPAIGN_TYPE_EQUIP_GEAR_FOR_1_HERO:
                case IDs.CAMPAIGN_TYPE_UPGRADE_TRAP_1_TIME:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_1_GEAR:
                case IDs.CAMPAIGN_TYPE_UPGRADE_GEAR_1_TIME:
                case IDs.CAMPAIGN_TYPE_EVOLUTION_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_STAR_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_DISCOVERY_1_TIME:
                case IDs.CAMPAIGN_TYPE_FORMATION_HAVE_1_HERO:
                    return 1;
                
                case IDs.CAMPAIGN_TYPE_SUMMON_BASIC_X_TIME:
                case IDs.CAMPAIGN_TYPE_SUMMON_SENIOR_X_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_HERO_X_TIME:
                case IDs.CAMPAIGN_TYPE_EQUIP_GEAR_FOR_X_HERO:
                case IDs.CAMPAIGN_TYPE_UPGRADE_TRAP_X_TIME:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_X_GEAR:
                case IDs.CAMPAIGN_TYPE_UPGRADE_GEAR_X_TIME:
                case IDs.CAMPAIGN_TYPE_EVOLUTION_HERO_X_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_STAR_HERO_X_TIME:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_X_TIME:
                case IDs.CAMPAIGN_TYPE_DISCOVERY_X_TIME:
                case IDs.CAMPAIGN_TYPE_FORMATION_HAVE_X_HERO:
                case IDs.CAMPAIGN_TYPE_UPGRADE_BASE_LV_X:
                case IDs.CAMPAIGN_TYPE_USER_LV_X:
                    value += xCount * 5;
                    break;
                
                case IDs.CAMPAIGN_TYPE_HAVE_X_HERO_LV_Y:
                    value += xCount * 5;
                    if (value > 6) value = 6;
                    break;
            }

            return value;
        }

        public int GetValue1()
        {
            //bắt đầu từ 1, kết thúc ở 89
            var index = GameData.Instance.CampaignsGroup.CampaignLevel - 1;
            if (index >= 89)
            {
                //Từ quest 90 trở đi sẽ loop lại quest 77 đến 89 , mỗi lần yêu cầu quest tăng lên 5 đơn vị. Reward giữ nguyên
                var count = ((index - 89) / 13) + 1;
                return infoValues[1] + count * 5;
            }
            else
            {
                return infoValues[1];
            }
        }
    }

    [System.Serializable]
    public class CampaignInfo
    {
        public int type;
        public string info;
    }

    //==================================

    public class CampaignsGroup : DataGroup
    {
        #region Members

        private List<Campaign> campaigns;
        private Dictionary<int, CampaignInfo> campaignInfos;

        private IntegerData campaignCount;//campaign hiện tại đã thực hiện bao nhiêu lần
        private IntegerData campaignLevel;

        //count
        private IntegerData summonBasicCount;
        private IntegerData summonSeniorCount;
        private IntegerData upgradeHeroCount;
        private IntegerData upgradeTrapCount;
        private IntegerData disassembleGearCount;
        private IntegerData upgradeGearCount;
        private IntegerData evolutionHeroCount;
        private IntegerData disassembleCount;
        private IntegerData upgradeStarHeroCount;
        private IntegerData discoveryCount;
        
        public int CampaignCount => campaignCount.Value;//campaign hiện tại đã thực hiện bao nhiêu lần
        public int CampaignLevel => campaignLevel.Value;//campaign hiện tại đã thực hiện bao nhiêu lần

        public Campaign CurrentCampaign
        {
            get
            {
                //bắt đầu từ 1, kết thúc ở 89
                var index = campaignLevel.Value - 1;
                if (index >= 89)
                {
                    //Từ quest 90 trở đi sẽ loop lại quest 77 đến 89 , mỗi lần yêu cầu quest tăng lên 5 đơn vị. Reward giữ nguyên
                    index = (index - 89) % 13 + 76;
                    return campaigns[index];
                }
                else
                {
                    return campaigns[index];
                }
            }
        }
        
        #endregion

        //=============================================

        #region Public

        public CampaignsGroup(int pId) : base(pId)
        {
            campaignLevel = AddData(new IntegerData(0, 1));
            campaignCount = AddData(new IntegerData(1, 0));

            //count
            summonBasicCount = AddData(new IntegerData(2, 0));
            summonSeniorCount = AddData(new IntegerData(3, 0));
            upgradeHeroCount = AddData(new IntegerData(4, 0));
            upgradeTrapCount = AddData(new IntegerData(5, 0));
            disassembleGearCount = AddData(new IntegerData(6, 0));
            upgradeGearCount = AddData(new IntegerData(7, 0));
            evolutionHeroCount = AddData(new IntegerData(8, 0));
            disassembleCount = AddData(new IntegerData(9, 0));
            upgradeStarHeroCount = AddData(new IntegerData(10, 0));
            discoveryCount = AddData(new IntegerData(11, 0));
        
            InitCampaigns();
        }

        public override void PostLoad()
        {
            base.PostLoad();
            
            CheckCurrentCampaign();
        }

        public string GetDescription(int type)
        {
            return campaignInfos[type].info;
        }

        // add
        // CAMPAIGN_TYPE_SUMMON_BASIC_1_TIME
        // CAMPAIGN_TYPE_SUMMON_BASIC_X_TIME
        public void AddSummonBasicCount()
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_SUMMON_BASIC_1_TIME
            //     || type == IDs.CAMPAIGN_TYPE_SUMMON_BASIC_X_TIME)
            // {
                summonBasicCount.Value++;
            // }
        }
        // CAMPAIGN_TYPE_SUMMON_SENIOR_1_TIME
        // CAMPAIGN_TYPE_SUMMON_SENIOR_X_TIME
        public void AddSummonSeniorCount()
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_SUMMON_SENIOR_1_TIME
            //     || type == IDs.CAMPAIGN_TYPE_SUMMON_SENIOR_X_TIME)
            // {
                summonSeniorCount.Value++;
            // }
        }
        // CAMPAIGN_TYPE_UPGRADE_HERO_1_TIME
        // CAMPAIGN_TYPE_UPGRADE_HERO_X_TIME
        public void AddUpgradeHeroCount()
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_UPGRADE_HERO_1_TIME
            //     || type == IDs.CAMPAIGN_TYPE_UPGRADE_HERO_X_TIME)
            // {
                upgradeHeroCount.Value++;
            // }
        }
        // CAMPAIGN_TYPE_UPGRADE_TRAP_1_TIME
        // CAMPAIGN_TYPE_UPGRADE_TRAP_X_TIME
        public void AddUpgradeTrapCount()
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_UPGRADE_TRAP_1_TIME
            //     || type == IDs.CAMPAIGN_TYPE_UPGRADE_TRAP_X_TIME)
            // {
                upgradeTrapCount.Value++;
            // }
        }
        // CAMPAIGN_TYPE_DISASSEMBLE_1_GEAR
        // CAMPAIGN_TYPE_DISASSEMBLE_X_GEAR
        public void AddDisassembleGearCount()
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_DISASSEMBLE_1_GEAR
            //     || type == IDs.CAMPAIGN_TYPE_DISASSEMBLE_X_GEAR)
            // {
                disassembleGearCount.Value++;
            // }
        }
        public void AddDisassembleGearCount(int count)
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_DISASSEMBLE_1_GEAR
            //     || type == IDs.CAMPAIGN_TYPE_DISASSEMBLE_X_GEAR)
            // {
                disassembleGearCount.Value += count;
            // }
        }
        // CAMPAIGN_TYPE_UPGRADE_GEAR_1_TIME
        // CAMPAIGN_TYPE_UPGRADE_GEAR_X_TIME
        public void AddUpgradeGearCount()
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_UPGRADE_GEAR_1_TIME
            //     || type == IDs.CAMPAIGN_TYPE_UPGRADE_GEAR_X_TIME)
            // {
                upgradeGearCount.Value++;
            // }
        }
        // CAMPAIGN_TYPE_EVOLUTION_HERO_1_TIME
        // CAMPAIGN_TYPE_EVOLUTION_HERO_X_TIME
        public void AddEvolutionHeroCount()
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_EVOLUTION_HERO_1_TIME
            //     || type == IDs.CAMPAIGN_TYPE_EVOLUTION_HERO_X_TIME)
            // {
                evolutionHeroCount.Value++;
            // }
        }
        // CAMPAIGN_TYPE_DISASSEMBLE_HERO_1_TIME
        // CAMPAIGN_TYPE_DISASSEMBLE_HERO_X_TIME
        public void AddDisassembleCount()
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_1_TIME
            //     || type == IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_X_TIME)
            // {
                disassembleCount.Value++;
            // }
        }
        public void AddDisassembleCount(int count)
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_1_TIME
            //     || type == IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_X_TIME)
            // {
                disassembleCount.Value += count;
            // }
        }
        // CAMPAIGN_TYPE_UPGRADE_STAR_HERO_1_TIME
        // CAMPAIGN_TYPE_UPGRADE_STAR_HERO_X_TIME
        public void AddUpgradeStarHeroCount()
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_UPGRADE_STAR_HERO_1_TIME
            //     || type == IDs.CAMPAIGN_TYPE_UPGRADE_STAR_HERO_X_TIME)
            // {
                upgradeStarHeroCount.Value++;
            // }
        }
        // CAMPAIGN_TYPE_DISCOVERY_1_TIME
        // CAMPAIGN_TYPE_DISCOVERY_X_TIME
        public void AddDiscoveryCount()
        {
            // var currentCampaign = CurrentCampaign;
            // var type = currentCampaign.type;
            //
            // if (type == IDs.CAMPAIGN_TYPE_DISCOVERY_1_TIME
            //     || type == IDs.CAMPAIGN_TYPE_DISCOVERY_X_TIME)
            // {
                discoveryCount.Value++;
            // }
        }

        //check
        public RewardInfo ClaimAndNextCampaign()
        {
            var currentCampaign = CurrentCampaign;
            var reward = currentCampaign.GetReward();
            campaignLevel.Value++;
            campaignCount.Value = 0;

            CheckCurrentCampaign();

            return reward;
        }
        
        #if DEVELOPMENT
        public RewardInfo ClaimAndNextCampaign(int toLevel)
        {
            var currentCampaign = CurrentCampaign;
            var reward = currentCampaign.GetReward();
            campaignLevel.Value = toLevel;
            campaignCount.Value = 0;

            CheckCurrentCampaign();

            return reward;
        }
        #endif

        public void CheckCurrentCampaign()
        {
            var currentCampaign = CurrentCampaign;
            var type = currentCampaign.type;

            //next campaign
            switch (type)
            {
                case IDs.CAMPAIGN_TYPE_WIN_MISSION_X:
                    CheckWinMission();
                    break;
                case IDs.CAMPAIGN_TYPE_HAVE_1_HERO_LV_Y:
                case IDs.CAMPAIGN_TYPE_HAVE_X_HERO_LV_Y:
                    CheckHasXHeroLevelY();
                    break;
                case IDs.CAMPAIGN_TYPE_EQUIP_GEAR_FOR_1_HERO:
                case IDs.CAMPAIGN_TYPE_EQUIP_GEAR_FOR_X_HERO:
                    CheckEquipGearForXHero();
                    break;
                case IDs.CAMPAIGN_TYPE_FORMATION_HAVE_1_HERO:
                case IDs.CAMPAIGN_TYPE_FORMATION_HAVE_X_HERO:
                    CheckFormationHaveXHero();
                    break;
                case IDs.CAMPAIGN_TYPE_UPGRADE_BASE_LV_X:
                    CheckBaseLevel();
                    break;
                case IDs.CAMPAIGN_TYPE_USER_LV_X:
                    CheckUserLevel();
                    break;

                case IDs.CAMPAIGN_TYPE_SUMMON_BASIC_1_TIME:
                case IDs.CAMPAIGN_TYPE_SUMMON_BASIC_X_TIME:
                    campaignCount.Value = summonBasicCount.Value;
                    break;
                case IDs.CAMPAIGN_TYPE_SUMMON_SENIOR_1_TIME:
                case IDs.CAMPAIGN_TYPE_SUMMON_SENIOR_X_TIME:
                    campaignCount.Value = summonSeniorCount.Value;
                    break;
                case IDs.CAMPAIGN_TYPE_UPGRADE_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_HERO_X_TIME:
                    campaignCount.Value = upgradeHeroCount.Value;
                    break;
                case IDs.CAMPAIGN_TYPE_UPGRADE_TRAP_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_TRAP_X_TIME:
                    campaignCount.Value = upgradeTrapCount.Value;
                    break;
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_1_GEAR:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_X_GEAR:
                    campaignCount.Value = disassembleGearCount.Value;
                    break;
                case IDs.CAMPAIGN_TYPE_UPGRADE_GEAR_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_GEAR_X_TIME:
                    campaignCount.Value = upgradeGearCount.Value;
                    break;
                case IDs.CAMPAIGN_TYPE_EVOLUTION_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_EVOLUTION_HERO_X_TIME:
                    campaignCount.Value = evolutionHeroCount.Value;
                    break;
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_X_TIME:
                    campaignCount.Value = disassembleCount.Value;
                    break;
                case IDs.CAMPAIGN_TYPE_UPGRADE_STAR_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_STAR_HERO_X_TIME:
                    campaignCount.Value = upgradeStarHeroCount.Value;
                    break;
                case IDs.CAMPAIGN_TYPE_DISCOVERY_1_TIME:
                case IDs.CAMPAIGN_TYPE_DISCOVERY_X_TIME:
                    campaignCount.Value = discoveryCount.Value;
                    break;
            }
        }

        private void CheckWinMission()
        {
            var currentCampaign = CurrentCampaign;
            var value = currentCampaign.GetValue0();
            
            var mission = GameData.Instance.MissionsGroup.GetMissionData(value);
            if (mission != null && mission.IsWin)
            {
                campaignCount.Value = 1;
            }
        }

        private void CheckHasXHeroLevelY()
        {
            var currentCampaign = CurrentCampaign;
            var values = currentCampaign.infoValues;
            var x = currentCampaign.GetValue0();
            var y = currentCampaign.GetValue1();

            int counHero = 0;
            var heroDatas = GameData.Instance.HeroesGroup.GetAllHeroDatas();
            var count = heroDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var heroData = heroDatas[i];
                if (heroData.levelUnlocked >= y) counHero++;
            }
            
            campaignCount.Value = counHero;
        }
        
        private void CheckEquipGearForXHero()
        {
            int counHero = 0;
            var heroDatas = GameData.Instance.HeroesGroup.GetAllHeroDatas();
            var count = heroDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var heroData = heroDatas[i];
                if (heroData.GearDatas.Count > 0) counHero++;
            }
            
            campaignCount.Value = counHero;
        }
        
        private void CheckFormationHaveXHero()
        {
            //max chỉ có 6 slot trong formation nên mission này ko loop
            int counHero = 0;
            var heroDatas = GameData.Instance.HeroesGroup.GetEquippedHeroes();
            var count = heroDatas.Length;
            for (int i = 0; i < count; i++)
            {
                var heroData = heroDatas[i];
                if (heroData != null) counHero++;
            }

            campaignCount.Value = counHero;
        }
        
        private void CheckBaseLevel()
        {
            //max chỉ có lv.10 nên mission này ko loop
            var currentCampaign = CurrentCampaign;
            var value = currentCampaign.GetValue0();

            var baseLevel = GameData.Instance.BaseGroup.Level;
            if(baseLevel >= value) campaignCount.Value = 1;
            else campaignCount.Value = 0;
        }
        
        private void CheckUserLevel()
        {
            var currentCampaign = CurrentCampaign;
            var value = currentCampaign.GetValue0();

            var baseLevel = GameData.Instance.UserGroup.Level;
            if(baseLevel >= value) campaignCount.Value = 1;
            else campaignCount.Value = 0;
        }

        #endregion

        //==============================================

        #region Private

        private void InitCampaigns()
        {
            //Read json data from resouce file
            var dataContent = GameData.GetTextContent("Data/Campaign");
            //Parse json data to list objects
            campaigns = JsonHelper.GetJsonList<Campaign>(dataContent);

            //Debug.Log(collection);
            dataContent = GameData.GetTextContent("Data/CampaignInfo");
            //Parse json data to list objects
            var collection = JsonHelper.GetJsonList<CampaignInfo>(dataContent);
            campaignInfos = new Dictionary<int, CampaignInfo>();
            foreach (var item in collection)
            {
                //Declare unit data, then push it into a data group
                campaignInfos.Add(item.type, item);
            }
        }
        
        #endregion
    }
}