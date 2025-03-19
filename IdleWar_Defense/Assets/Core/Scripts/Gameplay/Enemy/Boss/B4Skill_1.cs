using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using MagicArsenal;
using Spine;
using UnityEngine;

public class B4Skill_1 : EnemySkill
{
    public List<MagicBeamStatic> beamList;
    public float xDamagePercent;

    public override TrackEntry OnSkill()
    {
        var heroes = GameplayController.Instance.GetHeroExs();
        for (var i = 0; i < beamList.Count; i++)
        {
            if (i < heroes.Count)
            {
                beamList[i].gameObject.SetActive(true);
                beamList[i].transform.LookAt(heroes[i].transform);
            }
            else
            {
                beamList[i].gameObject.SetActive(false);
            }
        }
        return base.OnSkill();
    }

    public override void TriggerSkill()
    {
        if (enemyExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
       
        Invoke(nameof(DamageAllHero),0.1f);    
    }

    public void DamageAllHero()
    {
        
        InfoAttacker infoAttacker = new InfoAttacker(false,
            InfoAttacker.TYPE_SKILL,
            null,
            enemyExControl,
            null,
            enemyExControl.Damage,
            100,
            xDamagePercent,
            enemyExControl.Accuracy);
        var heroes = GameplayController.Instance.GetHeroes();
        for (var i = 0; i < heroes.Count; i++)
        {
            var hero = heroes[i];
            if(hero==null) continue;
            hero.GetHit(infoAttacker);
        }

        for (var i = 0; i < beamList.Count; i++)
        {
            beamList[i].gameObject.SetActive(false);
        }
    }
}
