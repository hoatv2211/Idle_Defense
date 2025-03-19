using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoodZombie;
using UnityEngine;
using Utilities.Common;

public class HeroSkillAoeDamage : HeroSkill
{
    public Vector2 effectPos;
    public ParticleMan effectAoe;
    public Impact impactPrefab;

    public float timePlay = 2f;
    public bool hasStun = false;
    public ParticleMan effectInEnemyPrefab;

    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
        if (effectAoe != null)
        {
            var pos = effectAoe.transform.position;
            effectAoe.transform.position = new Vector3(effectPos.x, effectPos.y, pos.z);
            effectAoe.Play();
        }

        InfoAttacker infoAttacker = new InfoAttacker(true, 
	                                                    InfoAttacker.TYPE_SKILL,                                             
                                                        heroExControl,
                                                        null,
                                                        null,
                                                        heroExControl.Damage * heroExControl.SkillValues[0] / 100f,
                                                        0f,//aoe skill nên dam xDamagePercent luôn
                                                        100f,
                                                        heroExControl.Accuracy,
                                                        100f);

        SkillNerf skillNerf = null;
        if (hasStun)
        {
            //Tất cả những nhân vật dính bão sét sẽ bị stun trong 1.5s
            skillNerf = new SkillNerf(heroExControl.heroData.baseId, heroExControl, effectInEnemyPrefab, heroExControl.SkillValues[1]);
            skillNerf.stun = true;
            skillNerf.color = Color.yellow;
        }

        var enemies = GameplayController.Instance.GetEnemies();
        var count = enemies.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            var enemy = enemies[i] as EnemyExControl;
            if(impactPrefab != null) CoroutineUtil.StartCoroutine(IESpawnImpact(enemy));
            // AddForce(enemy.transform);
            
            enemy.GetHit(infoAttacker);
            if (hasStun)
            {
                enemy.AddNerf(skillNerf);
            }
        }

        CoroutineUtil.StartCoroutine(IEEndPlay());
    }
    
    private void AddForce(Transform target)
    {
        target.DOBlendableMoveBy(Vector3.up.normalized * 0.3f, 0.4f).SetEase(Ease.OutQuad);
    }

    private IEnumerator IESpawnImpact(EnemyControl enemyControl)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.35f));

        if(enemyControl == null) yield break;
        
        Vector2 offset = enemyControl.Offset;
        Vector2 size = enemyControl.Size;
        float deltaX = size.x / 2f;
        float deltaY = size.y / 2f;
        Vector3 spawnPos = enemyControl.transform.position + new Vector3(
            UnityEngine.Random.Range(-deltaX, deltaX) + offset.x,
            UnityEngine.Random.Range(-deltaY, deltaY) + offset.y, 0f);
        GameplayController.Instance.SpawnImpact(impactPrefab, spawnPos, Quaternion.identity);
    }

    private IEnumerator IEEndPlay()
    {
        yield return new WaitForSeconds(timePlay);
        
        if (effectAoe != null) effectAoe.Stop();
    }
}
