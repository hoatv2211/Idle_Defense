using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

public class IceGrenadeControl : TrapControl
{
    public ParticleMan effectInEnemyPrefab;
    
    public override void TriggerAttack()
    {
        if (!soundAttack.Equals("")) SoundManager.Instance.PlaySFX(soundAttack);
        if (effectAttack != null) effectAttack.Play();

        StartCoroutine(IEAttack()); //effect nổ mới hiện dam, ko tự nhiên lại thấy enemies lăn đùng ra chết ngay
    }

    private IEnumerator IEAttack()
    {
        yield return new WaitForSeconds(0.2f);
        var skillNerf = new SkillNerf(SkillNerf.ID_TRAP_ICE, null, effectInEnemyPrefab, trapData.Duration);
        skillNerf.stun = true;
        skillNerf.color = Color.blue;

        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        null,
                                                        null,
                                                        null,
                                                        damage,
                                                        0f, //damage từ trap nên ko có crit
                                                        100f, //hệ số nhân crit giữ nguyên = 1f
                                                        100f); //để thế này cho lớn hơn dodge rồi mới chia 100f

        RaycastHit2D[] hits = CheckHits();
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyExControl>();
                enemyControl.GetHit(infoAttacker);
                enemyControl.AddNerf(skillNerf);
            }
        }
    }
}
