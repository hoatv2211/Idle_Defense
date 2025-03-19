using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoodZombie;
using UnityEngine;
using Utilities.Common;
using Zirpl.CalcEngine;

public class HeroAttackDuelist : HeroAttack
{
    public override void TriggerAttack()
    {
        if (heroExControl.IsDead()) return;

        if (!soundTriggerAttack.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerAttack);

        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_NORMAL, 
                                                        heroExControl,
                                                        null,
                                                        null,
                                                        heroExControl.Damage / maxGun, //chia dam theo số duel list
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        heroExControl.Knockback,
                                                        transform.position.y,
                                                        heroExControl.Range);
        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position + heroExControl.Offset, 3f * Config.xRange, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyControl>();
                if (enemyControl == heroExControl.target)
                {
                    enemyControl.GetHit(infoAttacker);

                    if (maxGun > 0)
                    {
                        //đánh range thì chỉ gọi muzzles không cần gọi bullet
                        if (muzzles != null && muzzles.Length > 0)
                        {
                            var count = muzzles.Length;
                            for (int i = 0; i < count; i++)
                            {
                                muzzles[i].Play();
                            }
                        }
                    }

                    //vầy là xong
                    return;
                }
            }
        }

        //
        if (maxGun > 0)
        {
            int count;
            
            //đánh range thì spawn bullet
            //attack duelist nên bật hết các muzzle
            count = muzzles.Length;
            for (int i = 0; i < count; i++)
            {
                muzzles[i].Play();
            }

            //bắn hết các súng
            for (int i = 0; i < maxGun; i++)
            {
                shots[i].Shot(infoAttacker);
            }
        }
        else
        {
            //đánh melee thì check ray cast
            // var range = heroExControl.Range;
            var range = Config.HERO_ATTACK_RANGE_Y;
            hits = Physics2D.BoxCastAll((Vector2) transform.position /*+ offsetAttack*/ + new Vector2(0f, range / 2f),
                                        new Vector2(sizeAttackX, range), 0f, Vector2.zero);

            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_ENEMY))
                {
                    var enemyControl = item.collider.GetComponent<EnemyControl>();
                    if (enemyControl == heroExControl.target)
                    {
                        enemyControl.GetHit(infoAttacker);
                    }
                }
            }
        }
    }
}
