using FoodZombie;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;

public class GameplayTestController : GameplayController
{
    List<HeroExControl> heroViews;
    public EnemyControlTestMouse EMouse;
    public GameData gameData;
    List<EnemyExControl> enemyExControles;
    protected override void Awake()
    {
        base.Awake();

        // gameData.Init();

        //     GameData.Instance.Init();
        //foreach (HeroDefinition hero in heroDefinitions)
        //{
        //    //  heroViews[0].Init();
        //}

        this.enemies = new List<EnemyControl>();
        enemyExControles = new List<EnemyExControl>(FindObjectsOfType<EnemyExControl>());
        List<EnemyData> listEnemyDatas = GameData.Instance.EnemiesGroup.GetAllEnemyDatas();
        int i = 0;
        if (enemyExControles != null && enemyExControles.Count > 0)
        {
            foreach (var item in enemyExControles)
            {
                // item.STAGE=
                if (item.gameObject.activeSelf)
                    item.Init(listEnemyDatas[i], 1, 1, 1);
                i++;
            }

            //    StartCoroutine(DoEATK());
        }
        this.heroes = new List<HeroControl>();
        this.heroExs = new List<HeroExControl>();
        enemies.Add(EMouse);


    }
    protected override void Start()
    {
        int heroID = 0;
        heroViews = new List<HeroExControl>(FindObjectsOfType<HeroExControl>());
        foreach (var item in heroViews)
        {
            string heroName = item.gameObject.name;
            heroName = heroName.Remove(0, 1);
            heroID = int.Parse(heroName);
            Debug.Log("init " + heroName + "_id_" + heroID + "_");
            HeroData heroData = new HeroData(1, heroID, 25);

            item.Init(heroData);
            item.SetActive(false);
            item.SetActive(true);
        }

        bulletPools = new PoolsContainer<Bullet>("PoolsBullet", 10, transformPool);
        for (int i = 0; i < builtInBullet.Count; i++)
        {
            var prefab = builtInBullet[i].prefab;
            var pool = bulletPools.Get(prefab);
            pool.AddOutsiders(builtInBullet[i].builtIn);
        }
        impactPools = new PoolsContainer<Impact>("PoolsImpact", 10, transformPool);
        for (int i = 0; i < builtInImpact.Count; i++)
        {
            var prefab = builtInImpact[i].prefab;
            var pool = impactPools.Get(prefab);
            pool.AddOutsiders(builtInImpact[i].builtIn);
        }

    }
    public override List<EnemyControl> GetEnemies()
    {
        // Debug.LogError("get Es " + enemies);
        if (enemies == null)
        {
            enemies = new List<EnemyControl>();
            enemies.Add(EMouse);
        }
        return enemies;
    }
    public override List<HeroControl> GetHeroes()
    {
        return null;
    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator DoEATK()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            foreach (var item in enemyExControles)
            {
                if (item.gameObject.activeSelf)
                    item.AnimAttack();
            }
        }
    }

    public override void RemoveEnemy(EnemyControl enemy)
    {

    }

    public override void SpawnCoinDrop(Vector3 pos, float delay)
    {
        //   base.SpawnCoinDrop(pos, delay);
    }
}
