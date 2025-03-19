using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

//buff heal và regen cho all hero
public class HeroSkillBuffHealHp : HeroSkill
{
    public ParticleMan effectInHeroPrefab;
    public bool isImmortal = false;
    
    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;
        
        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var skillBuff = new SkillBuff(heroExControl.heroData.baseId, effectInHeroPrefab, skillValues[0]);
        skillBuff.healHP = skillValues[1] * heroExControl.Damage / 100f;
        skillBuff.moreRegenHPPercent = skillValues[2];

        var heroes = GameplayController.Instance.GetHeroExs();
        var count = heroes.Count;
        for (int i = 0; i < count; i++)
        {
            var hero = heroes[i];
            if (!hero.IsDead() && hero != heroExControl) hero.AddBuff(skillBuff);
        }
        
        //Bất tử trong thời gian regen
        if (isImmortal)
        {
            skillBuff.immortal = isImmortal;
        }
        
        heroExControl.AddBuff(skillBuff);
    }
}
