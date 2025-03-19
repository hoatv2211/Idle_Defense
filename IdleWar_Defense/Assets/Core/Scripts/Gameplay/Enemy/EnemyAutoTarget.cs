using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemyAutoTarget : MonoBehaviour
{
    private EnemyExControl enemyExControl;

    private IEnumerator findTarget = null;

    public void Init(EnemyExControl _enemyExControl)
    {
        enemyExControl = _enemyExControl;

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
        if (enemyExControl != null)
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
        var heroes = GameplayController.Instance.GetHeroes();
        if (heroes != null)
            while (!enemyExControl.IsDead())
            {
                var target = enemyExControl.target;
                if (target == null || target.IsDead()) enemyExControl.SetTarget(GetPrioritizeHero(heroes));

                yield return new WaitForSeconds(1f);
            }
    }

    private HeroControl GetPrioritizeHero(List<HeroControl> _heroes)
    {
        var heroes = new List<HeroControl>(_heroes);
        int lenght = heroes.Count;
        for (int i = lenght - 1; i >= 0; i--)
        {
            if (heroes[i].IsDead()) heroes.RemoveAt(i);
        }

        lenght = heroes.Count;
        if (heroes == null || lenght == 0) return null;
        if (lenght == 1) return heroes[0];

        //xếp hero gần nhất
        HeroControl temp = null;
        float distance1, distance2;
        for (int i = 0; i < lenght - 1; i++)
        {
            distance1 = Vector2.Distance(transform.position, heroes[i].transform.position);
            for (int j = i + 1; j < lenght; j++)
            {
                distance2 = Vector2.Distance(transform.position, heroes[j].transform.position);
                if (distance1 > distance2)
                {
                    temp = heroes[i];
                    heroes[i] = heroes[j];
                    heroes[j] = temp;
                }
            }
        }

        //lấy 3 hero gần nhất
        while (lenght > 3)
        {
            heroes.RemoveAt(lenght - 1);
            lenght = heroes.Count;
        }

        var bestTargetHeroes = new List<HeroControl>();
        var targetToHero = enemyExControl.TargetToHero;
        //Tìm hero có chứa attackTypes = targetToHero
        for (int i = 0; i < lenght; i++)
        {
            var item = heroes[i];
            var attackTypes = item.AttackTypes;
            var atLength = attackTypes.Length;
            for (int j = 0; j < atLength; j++)
            {
                var attackType = attackTypes[j];
                if (attackType.Equals(targetToHero))
                {
                    bestTargetHeroes.Add(item);
                    break;
                }
            }
        }

        if (bestTargetHeroes.Count > 0)
        {
            return bestTargetHeroes[UnityEngine.Random.Range(0, bestTargetHeroes.Count)];
        }
        else
        {
            return heroes[UnityEngine.Random.Range(0, heroes.Count)];
        }
    }
}
