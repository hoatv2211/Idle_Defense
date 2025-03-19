using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

public class ElectricGrenadeControl : TrapControl
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

        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        null,
                                                        null,
                                                        null,
                                                        damage,
                                                        0f, //damage từ trap nên ko có crit
                                                        100f, //hệ số nhân crit giữ nguyên = 100f
                                                        100f); //để thế này cho lớn hơn dodge rồi mới chia 100f

        var enemies = new List<EnemyExControl>();
        RaycastHit2D[] hits = CheckHits();
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyExControl>();
                enemies.Add(enemyControl);
                enemyControl.GetHit(infoAttacker);
            }
        }

        //Khi sử dụng nó sẽ nổ và gây choáng cho kẻ địch 0.5s sau mỗi 1s. Hiệu ứng choáng kéo dài 4s.
        var skillNerf = new SkillNerf(SkillNerf.ID_TRAP_ELECTRIC, null,effectInEnemyPrefab, 0.5f);
        skillNerf.stun = true;
        skillNerf.color = Color.yellow;

        var count = trapData.Duration / 1f;
        var countEnemy = enemies.Count;
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < countEnemy; j++)
            {
                var enemy = enemies[j];
                enemy.AddNerf(skillNerf);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    protected override void OnDead()
    {
        if (!soundDie.Equals("")) SoundManager.Instance.PlaySFX(soundDie);

        disapear = IEDisapear();
        StartCoroutine(disapear);
    }

    //vì cái nerf này nên cho quả lựu đạn disapear lâu hơn, để đoạn code nerf còn chạy
    private IEnumerator IEDisapear()
    {
        float timePlay = 1f;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            var color = imgBody.color;
            color.a = timePlay;
            imgBody.color = color;
        }

        yield return new WaitForSeconds(trapData.Duration);

        gameObject.SetActive(false);
    }
}
