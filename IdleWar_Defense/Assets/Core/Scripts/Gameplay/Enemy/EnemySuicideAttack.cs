using Spine;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using UnityModule.Utility;
using Utilities.Common;
using JsonHelper = Utilities.Common.JsonHelper;
using Zirpl.CalcEngine;
using System;
using System.Linq;

public class EnemySuicideAttack : EnemyAttack
{
    public ParticleMan effectSuicide;
    public float xDamagePercent = 50f;
    //kể cả quân địch cũng dính damage. Damage khi bị bắn chết phát nổ = 1/2 damage cơ bản. 

    public override RaycastHit2D[] CheckHits()
    {
        //vì là attack kiểu nổ nên tính vùng attack là từ tâm con enemy, chứ không phải trước mặt như bọn enemy thông thường
        var range = enemyExControl.Range;
        var size = enemyExControl.Size + new Vector2(range - threshold / 2f, range - threshold / 2f); //cho vào gần tý nhìn cho đẹp
        var offset = enemyExControl.Offset;
        return Physics2D.BoxCastAll((Vector2)transform.position + offset, size, 0f, Vector2.zero);
    }

    public override bool CanAttack()
    {
        if (enemyExControl == null || enemyExControl.IsSleep()) return false;
        RaycastHit2D[] hits = CheckHits();
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_HERO))
            {
                atkSpeed = enemyExControl.AttackTime * xCountAttackInAnim;//mỗi lần attack cần gán lại vì còn theo boost
                return true;
            }
        }

        return false;
    }

    public override void TriggerAttack()
    {
        if (!soundTriggerAttack.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerAttack);
        if (effectSuicide != null) effectSuicide.Play();

        var damage = enemyExControl.Damage;
        if (enemyExControl.HP <= 0f) damage *= (xDamagePercent / 100f);
        InfoAttacker infoAttacker = new InfoAttacker(false,
                                                        InfoAttacker.TYPE_NORMAL,
                                                        null,
                                                        enemyExControl,
                                                        null,
                                                        damage,
                                                        enemyExControl.CritRate,
                                                        enemyExControl.CritDamage,
                                                        enemyExControl.Accuracy);

        RaycastHit2D[] hits = CheckHits();
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_HERO))
            {
                var heroControl = item.collider.GetComponent<HeroControl>();
                heroControl.GetHit(infoAttacker);
            }

            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyControl>();
                enemyControl.GetHit(infoAttacker);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        var dataContent = Resources.Load<TextAsset>("Data/Enemy");
        var enemyDefinitions = JsonHelper.GetJsonList<EnemyDefinition>(dataContent.text);
        Resources.UnloadAsset(dataContent);

        dataContent = Resources.Load<TextAsset>("Data/EnemyLevelUpStat");
        var enemyLevelUpStatDefinitions = JsonHelper.GetJsonList<EnemyLevelUpStat>(dataContent.text);
        Resources.UnloadAsset(dataContent);

        dataContent = Resources.Load<TextAsset>("Data/Boss");
        enemyDefinitions.AddRange(JsonHelper.GetJsonList<EnemyDefinition>(dataContent.text));
        Resources.UnloadAsset(dataContent);

        dataContent = Resources.Load<TextAsset>("Data/BossLevelUpStat");
        enemyLevelUpStatDefinitions.AddRange(JsonHelper.GetJsonList<EnemyLevelUpStat>(dataContent.text));
        Resources.UnloadAsset(dataContent);

        int id = int.Parse(name.Substring(0, 3).Remove(0, 1));//E
        float range = GetStat(enemyDefinitions[id - 1].attackRange, enemyLevelUpStatDefinitions[id - 1].attackRange, 1) * Config.xRange;

        Gizmos.color = Color.red;
        //vì là attack kiểu nổ nên tính vùng attack là từ tâm con enemy, chứ không phải trước mặt như bọn enemy thông thường
        var size = GetComponent<BoxCollider2D>().size + new Vector2(range, range);
        var offset = GetComponent<BoxCollider2D>().offset;
        Gizmos.DrawWireCube(transform.position + (Vector3)offset, (Vector3)size);
    }
}
