using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastSkillAim : MonoBehaviour
{
    protected HeroSkill heroSkill;

    public GameObject[] objectBlockRayCast;

    public void Aim(Transform _trAim)
    {
        gameObject.SetActive(true);
        transform.position = _trAim.position + new Vector3(0, 1, 0);
    }

    public void Init(HeroSkill _heroSkill,EnemyControl _default=null)
    {
        gameObject.SetActive(true);
        heroSkill = _heroSkill;
        timeAim = 1;      

        if (_default != null)
        {
            objAim = _default.transform;
            transform.position = _default.transform.position + new Vector3(0, 1, 0);
        }
           

        for (int i = 0; i < objectBlockRayCast.Length; i++)
            objectBlockRayCast[i].SetActive(false);
    }

    public void EndAim()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < objectBlockRayCast.Length; i++)
            objectBlockRayCast[i].SetActive(true);
    }

    private Transform objAim;
    private float timeAim = 3;

    private void FixedUpdate()
    {

        if (objAim != null)
        {
            transform.position = objAim.position + new Vector3(0, 1, 0);
        }
        else
        {
            objAim = GetRandomEnemy().transform;
        }

        timeAim -= Time.deltaTime;
        if(timeAim<=0)
        {
            heroSkill.AimCastSkill(objAim.GetComponent<EnemyControl>());
            EndAim();
        }


        if (Input.GetMouseButtonDown(0))
        {
            Vector2 cubeRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D cubeHit = Physics2D.Raycast(cubeRay, Vector2.zero);

            if (cubeHit)
            {
                Debug.Log(cubeHit.transform.name);
                if (cubeHit.transform.tag == Config.TAG_ENEMY)
                {
                    objAim = cubeHit.transform;
                    transform.position = cubeHit.transform.position + new Vector3(0, 1, 0);
                }
 
            }
        }
    }

    private EnemyControl GetRandomEnemy()
    {
        var list = GameplayController.Instance.GetEnemies();
        var listAlive = list.FindAll(a => !a.IsDead());
        if (listAlive?.Count == 0) return null;
        var target = listAlive[Random.Range(0, listAlive.Count)];
        return target;
    }
}
