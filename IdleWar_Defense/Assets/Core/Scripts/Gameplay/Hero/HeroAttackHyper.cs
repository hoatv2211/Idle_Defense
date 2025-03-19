using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoodZombie;
using UnityEngine;
using Utilities.Common;
using Zirpl.CalcEngine;

public class HeroAttackHyper : HeroAttack
{
    public string[] nameOfAttackMeleeAnimations;
    private bool attackingRange = true;

    public ParticleMan muzzleMelee;
    public string soundStartMeleeAttack;
    public string soundTriggerMeleeAttack;

    public override TrackEntry OnAttack()
    {
        currentPlayAttackTime = atkSpeed;

        string animName;
        //check range
        var posY = transform.position.y;
        var range = heroExControl.Range;

        //nếu có target thì lấy vị trí, không có thì bắn vu vơ lên trên
        var enemyPosY = Config.LOWEST_Y;
        var target = heroExControl.target;
        if (target != null && !target.IsDead()) enemyPosY = heroExControl.target.transform.position.y;
        if ((enemyPosY - posY) <= range) //nhỏ hơn tầm đánh thì đánh melee
        {
            if (!soundStartMeleeAttack.Equals("")) SoundManager.Instance.PlaySFX(soundStartMeleeAttack);

            attackingRange = false;
            animName = nameOfAttackMeleeAnimations[UnityEngine.Random.Range(0, nameOfAttackMeleeAnimations.Length)];
        }
        else
        {
            if (!soundStartAttack.Equals("")) SoundManager.Instance.PlaySFX(soundStartAttack);

            attackingRange = true;
            animName = nameOfAttackAnimations[UnityEngine.Random.Range(0, nameOfAttackAnimations.Length)];
        }

        var trackEntry = heroExControl.skeletonAnimation.AnimationState.SetAnimation(0, animName, false);
        trackEntry.Complete += AnimAttack_Complete;
        var duration = trackEntry.Animation.Duration;
        if (atkSpeed < duration) trackEntry.TimeScale = duration / atkSpeed;
        return trackEntry;
    }

    public override bool CanAttack()
    {
        //vì thằng này đánh cả melee và range nên chỉ cần có enemy là đc
        var enemies = GameplayController.Instance.GetEnemies();
        var posY = transform.position.y;
        for (int i = 0; i < enemies.Count; i++)
        {
            var item = enemies[i];
            var enemyPosY = item.transform.position.y;
            if (enemyPosY <= Config.LOWEST_Y)
            {
                //xCountAttackInAnim - VD: attackSpeed là 4 - 1 giây đánh 4 phát, nhưng trong animation có xCountAttackInAnim = 4
                //tức là 4 event attack trong 1 anim, attackSpeed là 4  -> attackTime là 0.25f và phải nhân thêm 4
                //thì lúc ấy diễn cả anim trong 1 giây và có 4 event = đánh 4 phát
                if (heroExControl != null)
                {
                    atkSpeed = heroExControl.AttackTime * xCountAttackInAnim; //mỗi lần attack cần gán lại vì còn theo boost
                    atkSpeed = atkSpeed * (1 - GameplayController.Instance.BuffHeroObject.ASAddPercent);
                }
                return true;
            }
        }

        return false;
    }

    public override void TriggerAttack()
    {
        if (heroExControl.IsDead()) return;

        if (attackingRange)
        {
            if (!soundTriggerAttack.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerAttack);

            InfoAttacker infoAttacker = new InfoAttacker(true,
                                                            InfoAttacker.TYPE_NORMAL,
                                                            heroExControl,
                                                            null,
                                                            null,
                                                            heroExControl.Damage / maxGun, //coi như duel list đi, bắn tất cả các nòng
                                                            heroExControl.CritRate,
                                                            heroExControl.CritDamage,
                                                            heroExControl.Accuracy,
                                                            heroExControl.Knockback,
                                                            transform.position.y,
                                                            0.1f); //để lúc nào cũng vượt tầm đánh thì luôn là dam range

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
            if (muzzleMelee != null) muzzleMelee.Play();
            if (!soundTriggerMeleeAttack.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerMeleeAttack);

            InfoAttacker infoAttacker = new InfoAttacker(true,
                                                            InfoAttacker.TYPE_NORMAL,
                                                            heroExControl,
                                                            null,
                                                            null,
                                                            heroExControl.Damage,
                                                            heroExControl.CritRate,
                                                            heroExControl.CritDamage,
                                                            heroExControl.Accuracy,
                                                            heroExControl.Knockback); //luôn là dam melee == damage chính

            //đánh melee thì check ray cast
            // var range = heroExControl.Range;
            var range = Config.HERO_ATTACK_RANGE_Y;
            var hits = Physics2D.BoxCastAll((Vector2)transform.position /*+ offsetAttack*/ + new Vector2(0f, range / 2f),
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
