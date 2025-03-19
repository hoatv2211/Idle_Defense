using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using Spine;
using UnityEngine;

public class HeroSkillAttackNearest : HeroSkill
{
    public ParticleMan effectInHeroPrefab;
    private EnemyControl target;

    public override TrackEntry OnSkill()
    {
        if (effectInHeroPrefab != null) effectInHeroPrefab.Play();
        target = GetNearestEnemy();
        if (target != null && !target.IsDead())
        {
            if (heroExControl != null)
                heroExControl.SetTarget(target);
        }
        else
            return null;
        return base.OnSkill();
    }


    public override void TriggerSkill()
    {
        if (heroExControl == null || heroExControl.IsDead()) return;
        if (target == null || target.IsDead()) return;
        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        heroExControl,
                                                        null,
                                                        null,
                                                        heroExControl.Damage * heroExControl.SkillValues[0] / 100f,
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        0f);


        RaycastHit2D[] hits =
            Physics2D.BoxCastAll((Vector2)transform.position /*+ offsetAttack*/ + new Vector2(0f, 1.35f),
                new Vector2(2f, 2.7f), 0f, Vector2.zero);
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
            hits = Physics2D.BoxCastAll((Vector2)transform.position + offsetSkill + new Vector2(0f, sizeSkill.y / 2f),
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
    private EnemyControl GetNearestEnemy()
    {
        var list = GameplayController.Instance.GetEnemies();
        EnemyControl target = null;
        float range = 1000000;
        foreach (var enemyControl in list)
        {
            if (Vector2.Distance(new Vector2(heroExControl.transform.position.x, heroExControl.transform.position.y),
                new Vector2(enemyControl.transform.position.x, enemyControl.transform.position.y)) < range)
            {
                target = enemyControl;
                range = Vector2.Distance(
                    new Vector2(heroExControl.transform.position.x, heroExControl.transform.position.y),
                    new Vector2(enemyControl.transform.position.x, enemyControl.transform.position.y));
            }
        }
        return target;
    }
}
