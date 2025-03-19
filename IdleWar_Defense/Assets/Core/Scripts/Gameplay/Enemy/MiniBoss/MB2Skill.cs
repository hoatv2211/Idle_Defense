using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

public class MB2Skill : EnemySkill
{
    public float critPercent;
    public float xDamagePercent;
    public float knockBackPercent;

    public override bool CanSkill()
    {
        animName = nameOfSkillAnimations[UnityEngine.Random.Range(0, nameOfSkillAnimations.Length)];
        var duration = enemyExControl.skeletonAnimation.skeleton.Data.FindAnimation(animName).Duration;
        skillSpeed = duration + enemyExControl.AttackTime / 6f; //mỗi lần attack cần gán lại vì còn theo boost
        var mb2Move = enemyExControl.autoMove as MB2Move;
        return  mb2Move.IsStopMoveStraight() ;
    }

    public override void TriggerSkill()
    {
        // co vao trigger skill k
        Debug.Log("co vao trigger skill");
         if (enemyExControl.IsDead()) return;
        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
        InfoAttacker infoAttacker = new InfoAttacker(false,
                                                        InfoAttacker.TYPE_SKILL,                                             
                                                        null,
                                                        enemyExControl,
                                                        null,
                                                        enemyExControl.Damage,
                                                        critPercent,
                                                        xDamagePercent,
                                                        enemyExControl.Accuracy,
                                                        knockBackPercent);
        //nếu enemies ở gần dưới 3 ô
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
    }

    public override void SkillEnd()
    {
        var mb2Control = enemyExControl as MB2Control;
        mb2Control.moveBeforeSkill = false;
        base.SkillEnd();
    }
}
