using System;
using DG.Tweening;
using UnityEngine;
using Utilities.Common;

/// <summary>
/// bullet.
/// </summary>
public class BulletSkill : Bullet
{
    public float radiusY = 1f;

    protected override void CheckHits()
    {
        var timeScale = Time.timeScale;
        float alpha = 1f;
        if (timeScale > 1f)
        {
            alpha = 1f + (timeScale - 1f) / 2f; //1.5f -> 1.25f    2.5f -> 1.75f
            Debug.Log(alpha);
        }
        RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2) transform.position, new Vector2(radius * alpha, radiusY * alpha), 0f , Vector2.zero);

        foreach (var item in hits)
        {
            var other = item.collider;

            if (infoAttacker.fromHero) //dùng biến này đơn thuần vì hiệu năng
            {
                if (other.CompareTag(Config.TAG_ENEMY))
                {
                    var enemyControl = other.GetComponent<EnemyControl>();
                    if (!enemies.Contains(enemyControl)) //mỗi viên đạn chỉ xuyên một con một lần
                    {
                        enemies.Add(enemyControl);
                        countPierce++;

                        if (countPierceMax > 0 && countPierce <= countPierceMax)
                        {
                            SpawnImpact(enemyControl);
                            enemyControl.GetHit(infoAttacker);

                            var r = UnityEngine.Random.Range(0f, 1f);
                            if (r <= (infoAttacker.knockback / 100f))
                            {
                                AddForce(enemyControl.transform);
                            }

                            //break loop
                            if(countPierce == countPierceMax) ReleaseBullet();
                        }
                        else if (countPierceMax == -1)
                        {
                            SpawnImpact(enemyControl);
                            enemyControl.GetHit(infoAttacker);

                            var r = UnityEngine.Random.Range(0f, 1f);
                            if (r <= (infoAttacker.knockback / 100f))
                            {
                                AddForce(enemyControl.transform);
                            }
                        }
                    }
                }
            }
            else
            {
                if (other.CompareTag(Config.TAG_HERO))
                {
                    var heroControl = other.GetComponent<HeroControl>();
                    if (!heroes.Contains(heroControl)) //mỗi viên đạn chỉ xuyên một con một lần
                    {
                        heroes.Add(heroControl);
                        countPierce++;

                        if (trail != null)
                        {
                            trail.Clear();
                            trail.SetActive(false);
                        }

                        if (countPierceMax > 0 && countPierce <= countPierceMax)
                        {
                            SpawnImpact(heroControl);
                            heroControl.GetHit(infoAttacker);

                            //break loop
                            if(countPierce == countPierceMax) ReleaseBullet();
                        }
                        else if (countPierceMax == -1)
                        {
                            SpawnImpact(heroControl);
                            heroControl.GetHit(infoAttacker);
                        }
                    }
                }
            }
        }

        //break loop
        var pos = transform.position;
        if (pos.x > 9f || pos.x < -9f || pos.y < -12f || pos.y > 12f)
        {
            ReleaseBullet();
        }
    }
    
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(radius, radiusY, 0f));
    }
}
