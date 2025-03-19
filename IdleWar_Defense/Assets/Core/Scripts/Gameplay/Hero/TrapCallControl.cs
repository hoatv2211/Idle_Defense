using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using Utilities.Common;

public class TrapCallControl : TrapControl
{
    [SerializeField, Tooltip("Buildin Pool")] private List<ParticleMan> effectExplosions;

    public override void Init(TrapData _trapData)
    {
        base.Init(_trapData);
        effectExplosions.Free();
    }

    public override void TriggerAttack()
    {
        StartCoroutine(IEAttack()); //effect nổ mới hiện dam, ko tự nhiên lại thấy enemies lăn đùng ra chết ngay
    }

    private IEnumerator IEAttack()
    {
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        null,
                                                        null,
                                                        null,
                                                        damage,
                                                        0f, //damage từ trap nên ko có crit
                                                        100f, //hệ số nhân crit giữ nguyên = 1f
                                                        100f); //để thế này cho lớn hơn dodge rồi mới chia 100f

        var count = trapData.Duration / 1f;
        for (int i = 0; i < count; i++)
        {
            //cho no 4 phat
            for (int j = 0; j < 4; j++)
            {
                StartCoroutine(IEShowEffect());
            }

            yield return new WaitForSeconds(1f);

            RaycastHit2D[] hits = CheckHits();
            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_ENEMY))
                {
                    var enemyControl = item.collider.GetComponent<EnemyExControl>();
                    enemyControl.GetHit(infoAttacker);
                }
            }
        }
    }

    private IEnumerator IEShowEffect()
    {
        float r = Random.Range(0, 0.85f);
        yield return new WaitForSeconds(r);

        if (!soundAttack.Equals("")) SoundManager.Instance.PlaySFX(soundAttack);

        var effectExplosion = effectExplosions.Obtain(transform);
        effectExplosion.transform.position = new Vector3(Random.Range(-4.0f, 4.0f), Random.Range(-2f, 8.0f), OderLayerZ.Z_BULLET); //-4.54 tọa độ barrier
        effectExplosion.SetActive(true);
        effectExplosion.Play();

        GameplayController.Instance.ShakeCamera();
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

        //thêm 4s cho hiện hết stop effect
        yield return new WaitForSeconds(4f);

        effectExplosions.Free();
        gameObject.SetActive(false);
    }
}
