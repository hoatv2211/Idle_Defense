using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoodZombie;
using UnityEngine;

public class HeroSkillAoeDamageWithSkillNerf : HeroSkill
{
    public bool setEffectPos = false;
    public Vector2 effectPos;

    public ParticleMan effectSkill;
    public ParticleMan effectInEnemyPrefab;
    public bool isSleep = false;
    public float effectTime = 2f;

    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (effectSkill != null)
        {
            if(setEffectPos)
            {
                var pos = effectSkill.transform.position;
                effectSkill.transform.position = new Vector3(effectPos.x, effectPos.y, pos.z);
            }
            effectSkill.Play();
        }
        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var duration = skillValues[0];
        SkillNerf skillNerf = null;
        if (duration > 0f)
        {
            skillNerf = new SkillNerf(heroExControl.heroData.baseId, heroExControl, effectInEnemyPrefab, duration);
            skillNerf.isSleep = isSleep;
        }
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        heroExControl,
                                                        null,
                                                        skillNerf,
                                                        heroExControl.Damage * skillValues[1] / 100f,
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        heroExControl.Knockback);

        infoAttacker.rangeKnockBack = skillValues[2] * Config.xRange;
       
        var enemies = GameplayController.Instance.GetEnemies();
        var count = enemies.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];
            enemy.GetHit(infoAttacker);
            AddForce(enemy.transform);
        }
        
        StartCoroutine((IEEndEffect()));
    }

    private IEnumerator IEEndEffect()
    {
        yield return new WaitForSeconds(effectTime);
        
        if (effectSkill != null) effectSkill.Stop();
    }

    private void AddForce(Transform target)
    {
        var maxRange = heroExControl.SkillValues[2] * Config.xRange;
        if (Config.LOWEST_Y - transform.position.y < maxRange)
        {
            maxRange = Config.LOWEST_Y - transform.position.y;
        }

        if (maxRange < 0) maxRange = 0;
        target.DOBlendableMoveBy(Vector3.up.normalized * maxRange, 0.4f).SetEase(Ease.OutQuad);
    }
}
