using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;
using DG.Tweening;
using UnityEngine.UI;
using EventDispatcher = Utilities.Common.EventDispatcher;

namespace FoodZombie.UI
{
    public class CampaignPanel : MyGamesBasePanel
    {
        //level
        public TextMeshProUGUI txtQuestLevel;
        public TextMeshProUGUI txtCampaignInfo;
        public TextMeshProUGUI txtCampaignCount;

        public RewardView rewardView;
        public SimpleTMPButton btnCollect;
        public SimpleTMPButton btnGo;

        public TMP_InputField inputLevelDev;
        public SimpleTMPButton btnCollectDev;

        private CampaignsGroup CampaignsGroup => GameData.Instance.CampaignsGroup;
        
        void Start()
        {
            btnCollect.onClick.AddListener(BtnCollect_Pressed);
            btnGo.onClick.AddListener(BtnGo_Pressed);
            
            #if DEVELOPMENT
            btnCollectDev.onClick.AddListener(BtnCollectDev_Pressed);
            #endif
        }
        
        internal override void Init()
        {
            txtQuestLevel.text = "Quest " + CampaignsGroup.CampaignLevel;
            
            var currentCampaign = CampaignsGroup.CurrentCampaign;
            var campaignCount = CampaignsGroup.CampaignCount;
            var maxCount = currentCampaign.GetMaxCount();
            if (campaignCount < maxCount)
            {
                txtCampaignCount.text = campaignCount + "/" + maxCount;
                txtCampaignCount.color = Color.red;
                txtCampaignInfo.color = Color.white;
                btnCollect.SetActive(false);
                btnGo.SetActive(true);
            }
            else
            {
                txtCampaignCount.text = maxCount + "/" + maxCount;
                txtCampaignCount.color = Color.green;
                txtCampaignInfo.color = Color.green;
                btnCollect.SetActive(true);
                btnGo.SetActive(false);
            }
            txtCampaignInfo.text = currentCampaign.GetDescription();
            
            rewardView.Init(currentCampaign.GetReward());
            
            #if DEVELOPMENT
            inputLevelDev.text = (CampaignsGroup.CampaignLevel + 1) + "";
            inputLevelDev.SetActive(true);
            btnCollectDev.SetActive(true);
            #endif
        }

        private void BtnCollect_Pressed()
        {
            var rewardInfo = CampaignsGroup.ClaimAndNextCampaign();
            //Quest hoàn thành thì button Go chuyển thành button Collect.
            //Ấn vào Collect thì dẫn sang quest tiếp theo.
            //Nếu Quest tiếp theo đã được hoàn thành thì user có thể ấn liên tục vào Collect cho tới khi tới 1 quest chưa hoàn thành sẽ chuyển thành button Go.
            Init();
            
            LogicAPI.ClaimReward(rewardInfo);
        }
        
        #if DEVELOPMENT
        private void BtnCollectDev_Pressed()
        {
            var rewardInfo = CampaignsGroup.ClaimAndNextCampaign(int.Parse(inputLevelDev.text));
            Init();
            
            LogicAPI.ClaimReward(rewardInfo);
        }
        #endif

        private void BtnGo_Pressed()
        {
            var currentCampaign = CampaignsGroup.CurrentCampaign;
            var type = currentCampaign.type;
            switch (type)
            {
                case IDs.CAMPAIGN_TYPE_WIN_MISSION_X:
                case IDs.CAMPAIGN_TYPE_USER_LV_X:
                    Back();
                    MainPanel.instance.ShowMapPanel();
                    break;
                case IDs.CAMPAIGN_TYPE_SUMMON_BASIC_1_TIME:
                case IDs.CAMPAIGN_TYPE_SUMMON_SENIOR_1_TIME:
                case IDs.CAMPAIGN_TYPE_SUMMON_BASIC_X_TIME:
                case IDs.CAMPAIGN_TYPE_SUMMON_SENIOR_X_TIME:
                    Back();
                    MainPanel.instance.ShowSummonGatePanel();
                    break;
                case IDs.CAMPAIGN_TYPE_UPGRADE_STAR_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_STAR_HERO_X_TIME:
                    Back();
                    MainPanel.instance.ShowHeroStarUpPanel();
                    break;
                case IDs.CAMPAIGN_TYPE_UPGRADE_BASE_LV_X:
                case IDs.CAMPAIGN_TYPE_UPGRADE_TRAP_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_TRAP_X_TIME:
                    Back();
                    MainPanel.instance.ShowBasePanel();
                    break;
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_1_GEAR:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_X_GEAR:
                    Back();
                    MainPanel.instance.ShowDisassembleGearPanel();
                    break;
                case IDs.CAMPAIGN_TYPE_UPGRADE_GEAR_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_GEAR_X_TIME:
                    Back();
                    MainPanel.instance.ShowUpgradeGearPanel();
                    break;
                case IDs.CAMPAIGN_TYPE_UPGRADE_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_UPGRADE_HERO_X_TIME:
                case IDs.CAMPAIGN_TYPE_EQUIP_GEAR_FOR_1_HERO:
                case IDs.CAMPAIGN_TYPE_EQUIP_GEAR_FOR_X_HERO:
                case IDs.CAMPAIGN_TYPE_HAVE_1_HERO_LV_Y:
                case IDs.CAMPAIGN_TYPE_HAVE_X_HERO_LV_Y:
                case IDs.CAMPAIGN_TYPE_EVOLUTION_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_EVOLUTION_HERO_X_TIME:
                    Back();
                    MainPanel.instance.ShowHeroPanel();
                    break;
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_1_TIME:
                case IDs.CAMPAIGN_TYPE_DISASSEMBLE_HERO_X_TIME:
                    Back();
                    MainPanel.instance.ShowDisassembleHeroPanel();
                    break;
                case IDs.CAMPAIGN_TYPE_DISCOVERY_1_TIME:
                case IDs.CAMPAIGN_TYPE_DISCOVERY_X_TIME:
                    Back();
                    MainPanel.instance.ShowDiscoveryPanel();
                    break;
                case IDs.CAMPAIGN_TYPE_FORMATION_HAVE_1_HERO:
                case IDs.CAMPAIGN_TYPE_FORMATION_HAVE_X_HERO:
                    Back();
                    MainPanel.instance.ShowFormationPanel();
                    break;
            }
        }
    }
}
