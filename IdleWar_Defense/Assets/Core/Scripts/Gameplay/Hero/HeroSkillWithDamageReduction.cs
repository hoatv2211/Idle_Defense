using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using Sirenix.OdinInspector;
using Spine;
using UnityEngine;

public class HeroSkillWithDamageReduction : HeroSkill
{
    public ParticleMan effectInHeroPrefab;

    private float[] skillValues;
    
    public override TrackEntry OnSkill()
    {
        skillValues = heroExControl.SkillValues;
        SkillBuff skillBuff = new SkillBuff(heroExControl.heroData.baseId, effectInHeroPrefab, skillValues[0]);
        skillBuff.damageReduction = skillValues[1];
        skillBuff.hpStealByPercentDamage = skillValues[3];
        if(!heroExControl.IsDead()) heroExControl.AddBuff(skillBuff);
        heroExControl.EndAim();
        return base.OnSkill();
    }

    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;
        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL, 
                                                        heroExControl, 
                                                        null,
                                                        null,
                                                        heroExControl.Damage * skillValues[2] / 100f,
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        heroExControl.Knockback);
        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position + heroExControl.Offset, 3f * Config.xRange, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyControl>();
                enemyControl.GetHit(infoAttacker);
            }
        }

        //
        if (shots != null && shots.Length > 0)
        {
            //đánh range thì spawn bullet
            shots[countGun].Shot(infoAttacker);
        }
        else
        {
            //đánh melee thì check ray cast
            hits = Physics2D.BoxCastAll((Vector2) transform.position + offsetSkill + new Vector2(0f, sizeSkill.y / 2f),
                sizeSkill, 0f, Vector2.zero);

            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_ENEMY))
                {
                    var enemyControl = item.collider.GetComponent<EnemyControl>();
                    enemyControl.GetHit(infoAttacker);
                }
            }
        }

        if (shots != null && shots.Length > 0)
        {
            //đánh range thì spawn bullet
            if (muzzles != null && muzzles.Length > 0)
            {
                muzzles[countGun].Play();
            }

            countGun++;
            if (countGun >= shots.Length) countGun = 0;
        }
    }

    public override void SkillEnd()
    {
        base.SkillEnd();
        heroExControl.StartAim();
    }
}
