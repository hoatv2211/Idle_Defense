using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoodZombie;
using UnityEngine;
using Utilities.Common;

//hiện tại là buff shield to all heroes
public class HeroSkillBuffAll : HeroSkill
{
    public ParticleMan effectInHeroPrefab;
    
    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var skillBuff = new SkillBuff(heroExControl.heroData.baseId, effectInHeroPrefab);
        skillBuff.shield = skillBuff.shieldMax = skillValues[0] * heroExControl.HP_MAX / 100f;
        skillBuff.moreRegenHPPercent = skillValues[1];

        var heroes = GameplayController.Instance.GetHeroExs();
        var count = heroes.Count;
        for (int i = 0; i < count; i++)
        {
            var hero = (HeroExControl) heroes[i];
            if(!hero.IsDead()) hero.AddBuff(skillBuff);
        }
    }
}
