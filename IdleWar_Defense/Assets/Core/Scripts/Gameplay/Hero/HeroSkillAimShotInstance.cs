using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using Spine;
using UnityEngine;

public class HeroSkillAimShotInstance : HeroSkill
{
    public ParticleMan effectInHeroPrefab;
    public ParticleMan effectInEnemyBeforeShotPrefab;
    public ParticleMan effectInEnemyWhenHitPrefab;
    private EnemyControl target;

    public override void AimCastSkill(EnemyControl _enemy)
    {
        target = _enemy;
    
        heroExControl.BtnSkill_Pressed();
    }

    public override TrackEntry OnSkill()
    {
        if (effectInHeroPrefab != null) effectInHeroPrefab.Play();
        if (GameplayController.Instance.autoPlay)
            target = GetRandomEnemy();

        if (target != null)
        {
            if (effectInEnemyBeforeShotPrefab != null)
            {
                effectInEnemyBeforeShotPrefab.transform.position = target.transform.position;
                effectInEnemyBeforeShotPrefab.gameObject.SetActive(true);
                effectInEnemyBeforeShotPrefab.Play();
            }

        }
        return base.OnSkill();
    }

    private EnemyControl GetRandomEnemy()
    {
        var list = GameplayController.Instance.GetEnemies();
        var listAlive = list.FindAll(a => !a.IsDead());
        if (listAlive?.Count == 0) return null;
        var target = listAlive[Random.Range(0, listAlive.Count)];
        return target;
    }

    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;
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
        if (target != null) target.GetHit(infoAttacker);
        if (effectInEnemyWhenHitPrefab != null)
        {
            effectInEnemyWhenHitPrefab.transform.position = target.transform.position;
            effectInEnemyWhenHitPrefab.gameObject.SetActive(true);
            effectInEnemyWhenHitPrefab.Play();
        }

    }
}