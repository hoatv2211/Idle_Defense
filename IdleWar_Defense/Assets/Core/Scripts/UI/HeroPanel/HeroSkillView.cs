using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie.UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class HeroSkillView : MonoBehaviour
{
    public const int SKILL_TYPE_ACTIVE = 1;
    public const int SKILL_TYPE_PASSIVE = 2;
    
    public Image imgSkill;
    public SimpleTMPButton btnSkill;

    private int skillType;
    private string skillName, skillInfo, cooldown;
    private void Start()
    {
        btnSkill.onClick.AddListener(BtnSkill_Pressed);
    }

    public void Init()
    {
        imgSkill.SetActive(false);
        btnSkill.SetEnable(false);
    }

    public void Init(int _skillType, Sprite sprite, string _skillName, string _skillInfo, string _cooldown = "")
    {
        skillType = _skillType;
        imgSkill.SetActive(true);
        btnSkill.SetEnable(true);
        imgSkill.sprite = sprite;
        skillName = _skillName;
        skillInfo = _skillInfo;
        cooldown = _cooldown;
    }

    private void BtnSkill_Pressed()
    {
        MainPanel.instance.ShowSkillDetailPanel(skillType, imgSkill.sprite, skillName, skillInfo, cooldown);
    }
}
