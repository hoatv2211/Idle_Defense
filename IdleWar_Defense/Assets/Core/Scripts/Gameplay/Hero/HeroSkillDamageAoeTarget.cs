using System.Security.Cryptography.X509Certificates;
using FoodZombie;
using UnityEngine;

public class HeroSkillDamageAoeTarget : HeroSkill
{
    public ParticleMan[] effectSkills;
    // hiện tại tạm thời cho chém melee trước mặt

    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (effectSkills != null)
        {
            var count = effectSkills.Length;
            for (int i = 0; i < count; i++)
            {
                effectSkills[i].Play();
            }
        }
        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var skillNerf = new SkillNerf(heroExControl.heroData.baseId, heroExControl, null, 0f);
        skillNerf.dieImmediatePercent = skillValues[1];
        skillNerf.damageAsHealPercent = skillValues[2];
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        heroExControl,
                                                        null,
                                                        skillNerf,
                                                        heroExControl.Damage,
                                                        100f,
                                                        skillValues[0],
                                                        heroExControl.Accuracy);
        
        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position + heroExControl.Offset, 3f * Config.xRange, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyExControl>();
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
            hits = Physics2D.BoxCastAll((Vector2) transform.position + offsetSkill + new Vector2(0f, sizeSkill.y / 2f),
                sizeSkill, 0f, Vector2.zero);

            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_ENEMY))
                {
                    var enemyControl = item.collider.GetComponent<EnemyExControl>();
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

    // public override void SkillEnd()
    // {
    //     base.SkillEnd();
    //     
    //     if (effectSkills != null)
    //     {
    //         var count = effectSkills.Length;
    //         for (int i = 0; i < count; i++)
    //         {
    //             effectSkills[i].Stop();
    //         }
    //     }
    // }
}
