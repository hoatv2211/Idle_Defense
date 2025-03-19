using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoodZombie;
using UnityEngine;
using Utilities.Common;

//buff một số stats đến chính hero hiện tại
public class HeroSkillBuffStats : HeroSkill
{
    public ParticleMan effectInHeroPrefab;

    //tổng buff cho chính nó và thêm bao nhiêu hero
    public int buffCount;
    private int currentBuffCount;
    
    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        currentBuffCount = buffCount;
        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var skillBuff = new SkillBuff(heroExControl.heroData.baseId, effectInHeroPrefab, skillValues[0]);
        skillBuff.moreAttackSpeedPercent = skillValues[1];
        skillBuff.moreCritRatePercent = skillValues[2];
        skillBuff.moreCritDamagePercent = skillValues[3];
        skillBuff.hpStealByPercentDamage = skillValues[4];
        skillBuff.moreAttackSpeed = skillValues[5];
        skillBuff.moreDamagePercent = skillValues[6];

        if (!heroExControl.IsDead())
        {
            heroExControl.AddBuff(skillBuff);
            currentBuffCount--;
            if (currentBuffCount > 0)
            {
                //tìm hero tiếp theo để buff đến khi đủ count thì thôi
                var heroes = GameplayController.Instance.GetHeroExs();
                var count = heroes.Count;
                var r = Config.EasyRandom(0, count);
                int index;
                for (int i = 0; i < count; i++)
                {
                    index = (i + r) % count;
                    var hero = (HeroExControl) heroes[index];
                    if (!hero.IsDead() && hero != heroExControl)
                    {
                        hero.AddBuff(skillBuff);
                        currentBuffCount--;
                        if(currentBuffCount <= 0) return;
                    }
                }
            }
        }
    }
}
