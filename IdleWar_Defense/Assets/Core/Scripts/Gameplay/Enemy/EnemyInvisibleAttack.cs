using Spine;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using UnityModule.Utility;
using Utilities.Common;
using JsonHelper = Utilities.Common.JsonHelper;
using Zirpl.CalcEngine;
using System;
using System.Linq;

public class EnemyInvisibleAttack : EnemyAttack
{
    public float xDamagePercent = 300f;
    private bool onAttackInvisible = false;
    
    public override TrackEntry OnAttack()
    {
        onAttackInvisible = enemyExControl.InInvisible;

        return base.OnAttack();
    }

    public override void TriggerAttack()
    {
        if (enemyExControl.IsDead()) return;

        if (!soundTriggerAttack.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerAttack);

        var damage = enemyExControl.Damage;
        if (onAttackInvisible) damage *= (xDamagePercent / 100f); 
        InfoAttacker infoAttacker = new InfoAttacker(false, 
                                                    InfoAttacker.TYPE_NORMAL, 
                                                    null, 
                                                    enemyExControl, 
                                                    null, 
                                                    damage,
                                                    enemyExControl.CritRate,
                                                    enemyExControl.CritDamage, 
                                                    enemyExControl.Accuracy);
        
        if (maxGun > 0)
        {
            //đánh range thì spawn bullet
            if (muzzles != null && muzzles.Length > 0)
            {
                muzzles[countGun].Play();
            }
            
            shots[countGun].Shot(infoAttacker);

            countGun++;
            if (countGun >= maxGun) countGun = 0;
        }
        else
        {
            //đánh melee thì check ray cast
            RaycastHit2D[] hits = CheckHits();
            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_HERO))
                {
                    var heroControl = item.collider.GetComponent<HeroControl>();
                    // if (heroControl == enemyExControl.target)
                    // {
                    heroControl.GetHit(infoAttacker);
                    return;
                    // }
                }
            }
        }
    }
}
