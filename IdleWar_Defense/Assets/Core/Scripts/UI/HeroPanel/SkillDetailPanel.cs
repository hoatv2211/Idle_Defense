using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class SkillDetailPanel : MyGamesBasePanel
    {
        public Image imgSkillActive;
        public Image imgSkillPassive;
        public GameObject skillActive;
        public GameObject skillPassive;
        public TextMeshProUGUI txtName;
        public TextMeshProUGUI txtCooldown;
        public TextMeshProUGUI txtInfo;

        public void Init(int skillType, Sprite sprite, string _skillName, string _skillInfo, string _cooldown)
        {
            if (skillType == HeroSkillView.SKILL_TYPE_ACTIVE)
            {
                imgSkillActive.sprite = sprite;
                skillActive.SetActive(true);
                skillPassive.SetActive(false);
                txtCooldown.text = _cooldown;
            }
            else
            {
                imgSkillPassive.sprite = sprite;
                skillActive.SetActive(false);
                skillPassive.SetActive(true);
                txtCooldown.text = "";
            }
            txtName.text = _skillName;
            txtInfo.text = _skillInfo;
        }
    }
}
