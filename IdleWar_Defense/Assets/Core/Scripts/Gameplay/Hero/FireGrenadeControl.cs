using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;

public class FireGrenadeControl : TrapControl
{
    public ParticleMan effectFlame;

    public override void TriggerAttack()
    {
        if (!soundAttack.Equals("")) SoundManager.Instance.PlaySFX(soundAttack);
        if (effectAttack != null) effectAttack.Play();

        StartCoroutine(IEAttack()); //effect nổ mới hiện dam, ko tự nhiên lại thấy enemies lăn đùng ra chết ngay
    }

    private IEnumerator IEAttack()
    {
        yield return new WaitForSeconds(0.1f);
        if (effectFlame != null) effectFlame.Play();

        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        null,
                                                        null,
                                                        null,
                                                        damage,
                                                        0f, //damage từ trap nên ko có crit
                                                        100f, //hệ số nhân crit giữ nguyên = 1f
                                                        100f); //để thế này cho lớn hơn dodge rồi mới chia 100f

        //Khi sử dụng nó sẽ nổ và gây sát thương đồng thời đốt cháy 1 vùng rộng 2 ô xung quanh nó. Quân địch đi qua vùng đất cháy này sẽ bị dính damage.
        //1s check hit một lần rồi chuyển infoAttacker vào enemy
        var count = trapData.Duration / 1f;
        for (int i = 0; i < count; i++)
        {
            RaycastHit2D[] hits = CheckHits();
            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_ENEMY))
                {
                    var enemyControl = item.collider.GetComponent<EnemyExControl>();
                    enemyControl.GetHit(infoAttacker);
                }
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

        if (effectFlame != null) effectFlame.Stop();

        //thêm 2s cho hiện hết stop effect
        yield return new WaitForSeconds(2f);

        gameObject.SetActive(false);
    }
}
