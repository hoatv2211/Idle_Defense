using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

public class B4Skill_2 : EnemySkill
{
    public float xDamagePercent;
     public override void TriggerSkill()
    {
        if (enemyExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
        InfoAttacker infoAttacker = new InfoAttacker(false,
                                                        InfoAttacker.TYPE_SKILL,
                                                        null,
                                                        enemyExControl,
                                                        null,
                                                        enemyExControl.Damage,
                                                        100,
                                                        xDamagePercent,
                                                        enemyExControl.Accuracy);
            
       

        //
        var listHero = GameplayController.Instance.GetHeroes();
        if (shots != null && shots.Length > 0)
        {
            //đánh range thì spawn bullet
            for (var i = 0; i < shots.Length; i++)
            {
                shots[i].Shot(infoAttacker);
            }
            if (muzzles != null && muzzles.Length > 0)
            {
                for (var i = 0; i < muzzles.Length; i++)
                {
                    muzzles[i].Play();
                }
            }

        }
        
    }
}
