using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class GearDetailPanel : MyGamesBasePanel
    {
        public GearView gearView;
        public TextMeshProUGUI txtName;
        public TextMeshProUGUI txtMainStat;
        public TextMeshProUGUI txtRank;
        public TextMeshProUGUI txtInfo;
        public SimpleTMPButton btnDisassemble;
        public SimpleTMPButton btnUnEquip;
        public SimpleTMPButton btnChange;

        //inventory panel
        private GearData gearData;
        private UnityAction refreshAction;

        //hero panel
        private HeroGearSlot heroGearSlot;
        private UnityAction<HeroGearSlot> unEquipAction;
        private UnityAction<HeroGearSlot> changeAction;

        private void Start()
        {
            btnDisassemble.onClick.AddListener(BtnDisassemble_Pressed);
            btnUnEquip.onClick.AddListener(BtnUnEquip_Pressed);
            btnChange.onClick.AddListener(BtnChange_Pressed);
        }

        public void Init(GearData _gearData, UnityAction _refreshAction, bool showbtnDisassemble = true)
        {
            gearData = _gearData;
            refreshAction = _refreshAction;

            gearView.Init(gearData);
            //txtName.text = gearData.Name;
            txtName.text = gearData.NameLocal;
            txtRank.text = "Rank: " + gearData.GetRankName();
            txtMainStat.text = gearData.ToSortString();
            txtInfo.text = gearData.ToString() + gearData.GetDescription();

            var equipped = gearData.IsEquipped();
            btnDisassemble.SetActive(showbtnDisassemble);
            //    btnDisassemble.SetActive(true);
            if (equipped)
            {
                btnDisassemble.SetEnable(false);
                btnDisassemble.labelTMP.text = /*"Equipped";*/ Localization.Get(Localization.ID.EQUIPED);
            }
            else
            {
                btnDisassemble.SetEnable(true);
                btnDisassemble.labelTMP.text =/* "Disassemble"*/Localization.Get(Localization.ID.DISASSEMBLE);
            }

            btnUnEquip.SetActive(false);
            btnChange.SetActive(false);
        }

        public void Init(HeroGearSlot _heroGearSlot, UnityAction<HeroGearSlot> _unEquipAction, UnityAction<HeroGearSlot> _changeAction)
        {
            heroGearSlot = _heroGearSlot;
            gearData = heroGearSlot.gearData;
            unEquipAction = _unEquipAction;
            changeAction = _changeAction;

            gearView.Init(gearData);
            //txtName.text = gearData.Name;
            txtName.text = gearData.NameLocal;
            txtRank.text = "Rank: " + gearData.GetRankName();
            txtMainStat.text = gearData.ToSortString();
            txtInfo.text = gearData.ToString() + gearData.GetDescription();

            btnDisassemble.SetActive(false);
            btnUnEquip.SetActive(true);
            btnChange.SetActive(true);
        }

        private void BtnDisassemble_Pressed()
        {
            Back();

            var rewardInfo = GameData.Instance.GearsGroup.DisassembleGear(gearData);
            LogicAPI.ClaimRewards(rewardInfo, TrackingConstants.VALUE_DISASSEMBLE_GEAR);

            if (refreshAction != null) refreshAction();
        }

        private void BtnUnEquip_Pressed()
        {
            Back();

            if (unEquipAction != null) unEquipAction(heroGearSlot);
        }

        private void BtnChange_Pressed()
        {
            Back();

            if (changeAction != null) changeAction(heroGearSlot);
        }
    }
}
