using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoodZombie;
using UnityEngine;
using Utilities.Common;

public class HeroSkillMeleeDamage : HeroSkill
{
    public ParticleMan effectCharge;
    public ParticleMan effectInEnemyPrefab;
    
    public bool aim = false;
    
    public override void End()
    {
        base.End();
        
        if (effectCharge != null) effectCharge.Stop();
    }

    public override TrackEntry OnSkill()
    {
        if(!aim) heroExControl.EndAim();
        
        return base.OnSkill();
    }

    public override void SkillEnd()
    {
        base.SkillEnd();
        
        if(!aim) heroExControl.StartAim();
    }

    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (effectCharge != null) effectCharge.Play();
        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var skillNerf = new SkillNerf(heroExControl.heroData.baseId, heroExControl, effectInEnemyPrefab, skillValues[0]);
        skillNerf.damagePercentHpTargetPerSec = skillValues[2];
        skillNerf.armorReducePercent = skillValues[3];
        InfoAttacker infoAttacker = new InfoAttacker(true, 
                                                        InfoAttacker.TYPE_SKILL,
                                                        heroExControl,
                                                        null,
                                                        skillNerf,
                                                        heroExControl.Damage * skillValues[1] / 100f,
                                                        0f,//aoe skill nên dam xDamagePercent luôn
                                                        100f,
                                                        heroExControl.Accuracy);

        //đánh melee thì check ray cast
        //Chém 1 phát thương bao quát toàn bộ melee range với 300% damage. Các Enemy dính đòn tiếp tục bị thiêu đốt với 100% sát thương mỗi giây trong vòng 3 giây.
        var range = heroExControl.Range;
        RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2) transform.position /*+ offsetAttack*/ + new Vector2(0f, range / 2f),
            new Vector2(heroExControl.attack.sizeAttackX, range), 0f, Vector2.zero);

        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyExControl>();
                enemyControl.GetHit(infoAttacker);
            }
        }
    }
}
