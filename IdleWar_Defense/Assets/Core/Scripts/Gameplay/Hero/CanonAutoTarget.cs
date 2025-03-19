using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonAutoTarget : MonoBehaviour
{
    private CanonControl canonControl;

    private IEnumerator findTarget = null;

    public void Init(CanonControl _canonControl)
    {
        canonControl = _canonControl;

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
        if (canonControl != null)
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
        while (true)
        {
            hasEnemyNearBy = false;
            RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position, Config.RANGE_MIN, Vector2.zero);
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
                canonControl.SetTarget(GetPrioritizeEnemy(enemies, 1)); //random lenght = 1 thì chỉ có con gần nhất
            }
            else
            {
                var target = canonControl.target;
                if (target == null || target.IsDead())
                    canonControl.SetTarget(GetPrioritizeEnemy(enemies)); //random 4 con gần nhât
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private EnemyControl GetPrioritizeEnemy(List<EnemyControl> _enemies, int randomLenght = 4)
    {
        if (_enemies == null || _enemies.Count == 0) return null;
        if (_enemies.Count == 1) return _enemies[0];
        
        var enemies = new List<EnemyControl>(_enemies);
        int lenght = enemies.Count;
        for (int i = lenght - 1; i >= 0; i--)
        {
            if (enemies[i].IsDead()) enemies.RemoveAt(i);
        }

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

        if (enemies.Count == 1) return enemies[0];

        //lấy 4 enemy gần nhất
        lenght = enemies.Count;
        while (lenght > randomLenght)
        {
            enemies.RemoveAt(lenght - 1);
            lenght = enemies.Count;
        }

        // var bestTargets = new List<EnemyControl>();
        // var targetToEnemy = canonControl.TargetToEnemy;
        // //Tìm hero có chứa attackTypes = targetToHero
        // for (int i = 0; i < lenght; i++)
        // {
        //     var item = enemies[i];
        //     var attackTypes = item.AttackTypes;
        //     var atLength = attackTypes.Length;
        //     for (int j = 0; j < atLength; j++)
        //     {
        //         var attackType = attackTypes[j];
        //         if (attackType.Equals(targetToEnemy))
        //         {
        //             bestTargets.Add(item);
        //             break;
        //         }
        //     }
        // }

        // if (bestTargets.Count > 0)
        // {
        //     return bestTargets[UnityEngine.Random.Range(0, bestTargets.Count)];
        // }
        // else
        // {
            return enemies[UnityEngine.Random.Range(0, enemies.Count)];
        // }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Config.RANGE_MIN);
    }
}
