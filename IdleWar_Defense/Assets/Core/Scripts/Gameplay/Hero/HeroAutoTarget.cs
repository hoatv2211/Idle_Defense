using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAutoTarget : MonoBehaviour
{
    private HeroExControl heroExControl;

    private IEnumerator findTarget = null;

    public void Init(HeroExControl _heroExControl)
    {
        heroExControl = _heroExControl;

        if (findTarget != null)
        {
            StopCoroutine(findTarget);
            findTarget = null;
        }

        enabled = true;
    }

    public void End()
    {
        enabled = false;
    }

    private void OnEnable()
    {
        if (heroExControl != null)
        {
            findTarget = IEFindTarget();
            StartCoroutine(findTarget);
        }
    }

    private void Start()
    {
        if (findTarget == null)
        {
            findTarget = IEFindTarget();
            StartCoroutine(findTarget);
        }
    }

    // Start is called before the first frame update
    IEnumerator IEFindTarget()
    {
        bool hasEnemyNearBy = false;
        var enemies = GameplayController.Instance.GetEnemies();
        while (heroExControl != null && !heroExControl.IsDead())
        {
            if (!heroExControl.skill.Skilling())
            {
                hasEnemyNearBy = false;
                RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2)transform.position, Config.RANGE_MIN, Vector2.zero);
                //nếu enemy đến gần quá (trong phạm vi RANGE_MIN) thì target vào con gần nhât
                foreach (var item in hits)
                {
                    var other = item.collider;
                    if (other.CompareTag(Config.TAG_ENEMY))
                    {
                        var enemyControl = other.GetComponent<EnemyControl>();
                        if (!enemyControl.IsDead())
                        {
                            hasEnemyNearBy = true;
                            break;
                        }
                    }
                }

                if (hasEnemyNearBy)
                {
                    heroExControl.SetTarget(GetPrioritizeEnemy(enemies, 1)); //random lenght = 1 thì chỉ có con gần nhất
                }
                else
                {
                    var target = heroExControl.target;
                    if (target == null || target.IsDead())
                        heroExControl.SetTarget(GetPrioritizeEnemy(enemies)); //random 4 con gần nhât
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private EnemyControl GetPrioritizeEnemy(List<EnemyControl> _enemies, int randomLenght = 4)
    {
        var enemies = new List<EnemyControl>(_enemies);
        int lenght = enemies.Count;
        for (int i = lenght - 1; i >= 0; i--)
        {
            var enemie = enemies[i];
            if (enemie.IsDead() || enemie.InInvisible || enemie.transform.position.y > Config.LOWEST_Y) enemies.RemoveAt(i);
        }

        lenght = enemies.Count;
        if (enemies == null || lenght == 0) return null;
        if (lenght == 1) return enemies[0];

        //xếp hero gần nhất
        EnemyControl temp = null;
        float distance1, distance2;
        for (int i = 0; i < lenght - 1; i++)
        {
            distance1 = Vector2.Distance(transform.position, enemies[i].transform.position);
            for (int j = i + 1; j < lenght; j++)
            {
                distance2 = Vector2.Distance(transform.position, enemies[j].transform.position);
                if (distance1 > distance2)
                {
                    temp = enemies[i];
                    enemies[i] = enemies[j];
                    enemies[j] = temp;
                }
            }
        }

        //lấy 4 enemy gần nhất
        while (lenght > randomLenght)
        {
            enemies.RemoveAt(lenght - 1);
            lenght = enemies.Count;
        }

        var bestTargets = new List<EnemyControl>();
        var targetToEnemy = heroExControl.TargetToEnemy;
        //Tìm hero có chứa attackTypes = targetToHero
        for (int i = 0; i < lenght; i++)
        {
            var item = enemies[i];
            var attackTypes = item.AttackTypes;
            var atLength = attackTypes.Length;
            for (int j = 0; j < atLength; j++)
            {
                var attackType = attackTypes[j];
                if (attackType.Equals(targetToEnemy))
                {
                    bestTargets.Add(item);
                    break;
                }
            }
        }

        if (bestTargets.Count > 0)
        {
            return bestTargets[UnityEngine.Random.Range(0, bestTargets.Count)];
        }
        else
        {
            return enemies[UnityEngine.Random.Range(0, enemies.Count)];
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Config.RANGE_MIN);
    }


}
