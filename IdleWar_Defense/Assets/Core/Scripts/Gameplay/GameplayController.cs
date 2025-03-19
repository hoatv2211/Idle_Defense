using FoodZombie;
using FoodZombie.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Scripts.UI.Drone_Panel;
using HedgehogTeam.EasyTouch.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Common;
using Debug = UnityEngine.Debug;
using DG.Tweening;

[System.Serializable]
public class HeroControlsPool
{
    public List<HeroControl> list;

    public HeroControlsPool()
    {

    }

    public HeroControlsPool(List<HeroControl> _list)
    {
        list = _list;
    }
}

[System.Serializable]
public class TrapControlsPool
{
    public List<TrapControl> list;

    public TrapControlsPool()
    {

    }

    public TrapControlsPool(List<TrapControl> _list)
    {
        list = _list;
    }
}

//[System.Serializable]
//public class EnemyControlsPool
//{

//    public List<EnemyControl> list;

//    public EnemyControlsPool(EnemyControl _eData)
//    {
//        list = new List<EnemyControl>();
//    }

//    public EnemyControlsPool(List<EnemyControl> _list)
//    {
//        list = _list;
//    }
//}

[System.Serializable]
public class BuiltInPoolingBullet
{
    public Bullet prefab;
    public List<Bullet> builtIn;
}
[System.Serializable]
public class BuiltInPoolingImpact
{
    public Impact prefab;
    public List<Impact> builtIn;
}

public class GameplayController : MonoBehaviour
{
    protected static GameplayController instance;
    public static GameplayController Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
        {
            instance = this;
        }
    }
#if DEVELOPMENT || UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Time.timeScale = Time.timeScale == 1 ? 5 : 1;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
            {
                hubPanel.PvpScoreInGamePanel.score_player = 999999;
                PvPEndMatch();
            }
            else
                ShowWin();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
            {
                hubPanel.PvpScoreInGamePanel.score_player = int.MinValue;
                PvPEndMatch();
            }
            else
                ShowLose();
        }
    }
#endif

    public static bool isMisstionIntro = false;
    public HubPanel hubPanel;

    public Camera camera;
    public ZCameraShake cameraShake;

    BuffObject buffHeroObject;
    public BuffObject BuffHeroObject
    {
        get
        {
            if (buffHeroObject == null)
                buffHeroObject = new BuffObject() { ASAddPercent = 0, DamAddPercent = 0 };
            return buffHeroObject;
        }

        set
        {
            buffHeroObject = value;
        }
    }

    public bool canStartGame = false;
    private bool isSpawnEnd = false;
    private bool isShowedEndPanel = false;

    //pool
    public Transform[] heroSpawnPoses;
    public Transform transformPool;
    //  [SerializeField, Tooltip("Buildin Pool")] private List<HeroControlsPool> heroControlsPool;
    // [SerializeField, Tooltip("Buildin Pool")] private List<EnemyControlsPool> enemyControlsPool;
    [SerializeField, Tooltip("Buildin Pool")] private List<HeroControlsPool> barriersPool;
    [SerializeField, Tooltip("Buildin Pool")] private List<TrapControlsPool> trapsPool;
    [SerializeField, Tooltip("Buildin Pool")] private List<TextHp> textHpsPool;
    [SerializeField, Tooltip("Buildin Pool")] private List<ParticleGold> coinDropsPool;

    [SerializeField] protected PoolsContainer<Bullet> bulletPools;
    [SerializeField] protected List<BuiltInPoolingBullet> builtInBullet;
    [SerializeField] protected PoolsContainer<Impact> impactPools;
    [SerializeField] protected List<BuiltInPoolingImpact> builtInImpact;

    public ParticleMan effectBuffFirstAirKit;

    private HeroData[] listHeroDatas;
    private List<EnemyData> listEnemyDatas;

    protected List<HeroControl> heroes;
    protected List<HeroExControl> heroExs;
    public List<CanonControl> canons;

    protected List<EnemyControl> enemies;

    private Dictionary<int, int> totalKills;

    public bool autoPlay = false;
    public bool holdingTap = false;//sửa lại là thả tay thì vẫn auto
    public TapToTarget tapToTarget;

    public float currentTimeScale = 1f;

    public bool CanAutoSkill { get; set; }

    //fake data for intro mission
    private TrapData trapDataBarrier, trapDataCall, trapDataMine, trapDataSpike, trapDataFirstAirKit, trapDataIceGrenade, trapDataFireGrenade, trapDataElectricGrenade, trapDataCanon;

    private bool isWinIntroMission;

    // private int currencyCoin;
    private int enemiesCoin;

    private MissionsGroup MissionsGroup => GameData.Instance.MissionsGroup;
    private DiscoveriesGroup DiscoveriesGroup => GameData.Instance.DiscoveriesGroup;

    protected virtual void Start()
    {
        Config.LastGameResult = 0;
        Debug.Log("Mission Data: Mode: " + Config.typeModeInGame.ToString() + "_level ID_" + GameData.Instance.MissionsGroup.CurrentMissionId);
        IEAutoSpawnEnemy = DoAutoSpawnEnemy();
        StartCoroutine(IEAutoSpawnEnemy);
        GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.START_MISSION);
        if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
        {
            EventDispatcher.AddListener<WaveEndEvent>(OnWaveEndEvent);
            Config.LogScene(TrackingConstants.SceneName.Screen_Play_PvP);
        }
        if (Config.typeModeInGame == Config.TYPE_MODE_DISCOVERY) Config.LogScene(TrackingConstants.SceneName.Screen_Play_Discovery);
        if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL) Config.LogScene(TrackingConstants.SceneName.Screen_Play_Mission);
    }
    private void OnDestroy()
    {
        if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
            EventDispatcher.RemoveListener<WaveEndEvent>(OnWaveEndEvent);
    }

    public void Init(HubPanel _hubPanel)
    {
        canStartGame = false;
        if (!GameData.Instance.MissionsGroup.IsWinIntroMission()) isMisstionIntro = true;
        else isMisstionIntro = false;

        hubPanel = _hubPanel;

        //foreach (var item in heroControlsPool)
        //{
        //    item.list.Free();
        //}
        //foreach (var item in enemyControlsPool)
        //{
        //    item.list.Free();
        //}
        foreach (var item in barriersPool)
        {
            item.list.Free();
        }
        foreach (var item in trapsPool)
        {
            item.list.Free();
        }
        textHpsPool.Free();
        coinDropsPool.Free();

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

        //  if (isMisstionIntro) listEnemyDatas = GameData.Instance.EnemiesGroup.GetAllEnemyDatasTutorial();
        //  else listEnemyDatas = GameData.Instance.EnemiesGroup.GetAllEnemyDatas();
        listEnemyDatas = GameData.Instance.EnemiesGroup.GetAllEnemyDatas();

        heroes = new List<HeroControl>();
        heroExs = new List<HeroExControl>();
        enemies = new List<EnemyControl>();

        totalKills = new Dictionary<int, int>();

        //trap items
        trapDataBarrier = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_BARRIER);
        trapDataCall = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_CALL);
        trapDataMine = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_MINE);
        trapDataSpike = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_TRAP);
        trapDataFirstAirKit = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_FIRST_AIR_KIT);
        trapDataIceGrenade = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_ICE_GRENADE);
        trapDataFireGrenade = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_FIRE_GRENADE);
        trapDataElectricGrenade = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_ELECTRIC_GRENADE);
        trapDataCanon = GameData.Instance.BaseGroup.GetTrapData(IDs.ITEM_TRAP_CANON);

        isWinIntroMission = MissionsGroup.IsWinIntroMission();
        //load team
        if (isWinIntroMission)
        {
            listHeroDatas = GameData.Instance.HeroesGroup.GetEquippedHeroes();

            var currentMissionId = GameData.Instance.MissionsGroup.CurrentMissionId;
            if (currentMissionId != 1001 && currentMissionId != 1002)
            {
                ShowTrapDatas();
            }

            //trap canon
            var damage = trapDataCanon.Damage;
            var baseLevel = GameData.Instance.BaseGroup.GetCurrentBase().level;
            if (baseLevel >= 9)
            {
                canons[0].Init(damage);
                canons[0].SetActive(true);
            }
            if (baseLevel >= 10)
            {
                canons[1].Init(damage);
                canons[1].SetActive(true);
            }
        }
        else
        {
            //fake formation for intro mission
            listHeroDatas = new HeroData[6];
            listHeroDatas[0] = new HeroData(1, IDs.HS15, 25);
            listHeroDatas[1] = new HeroData(2, IDs.HSS6, 25);
            listHeroDatas[2] = new HeroData(3, IDs.HSS4, 25);
            listHeroDatas[3] = new HeroData(4, IDs.HSS2, 25);
            listHeroDatas[4] = new HeroData(5, IDs.HSS1, 25);
            listHeroDatas[5] = new HeroData(6, IDs.HSS3, 25);

            //fake trap items
            trapDataBarrier = new TrapData(trapDataBarrier.Id, trapDataBarrier.baseData, trapDataBarrier.levelUpStat);
            trapDataBarrier.SetStock(2);
            trapDataBarrier.Unlock();
            trapDataBarrier.SetLevel(200);
            trapDataCall = new TrapData(trapDataCall.Id, trapDataCall.baseData, trapDataCall.levelUpStat);
            trapDataCall.SetStock(1);
            trapDataCall.Unlock();
            trapDataCall.SetLevel(50);
            trapDataMine = new TrapData(trapDataMine.Id, trapDataMine.baseData, trapDataMine.levelUpStat);
            trapDataMine.SetStock(1);
            trapDataMine.Unlock();
            trapDataMine.SetLevel(50);
            trapDataSpike = new TrapData(trapDataSpike.Id, trapDataSpike.baseData, trapDataSpike.levelUpStat);
            trapDataSpike.SetStock(2);
            trapDataSpike.Unlock();
            trapDataSpike.SetLevel(50);
            trapDataFirstAirKit = new TrapData(trapDataFirstAirKit.Id, trapDataFirstAirKit.baseData, trapDataFirstAirKit.levelUpStat);
            trapDataFirstAirKit.SetStock(1);
            trapDataFirstAirKit.Unlock();
            trapDataFirstAirKit.SetLevel(50);
            trapDataIceGrenade = new TrapData(trapDataIceGrenade.Id, trapDataIceGrenade.baseData, trapDataIceGrenade.levelUpStat);
            trapDataIceGrenade.SetStock(1);
            trapDataIceGrenade.Unlock();
            trapDataIceGrenade.SetLevel(50);
            trapDataFireGrenade = new TrapData(trapDataFireGrenade.Id, trapDataFireGrenade.baseData, trapDataFireGrenade.levelUpStat);
            trapDataFireGrenade.SetStock(1);
            trapDataFireGrenade.Unlock();
            trapDataFireGrenade.SetLevel(50);
            trapDataElectricGrenade = new TrapData(trapDataElectricGrenade.Id, trapDataElectricGrenade.baseData, trapDataElectricGrenade.levelUpStat);
            trapDataElectricGrenade.SetStock(1);
            trapDataElectricGrenade.Unlock();
            trapDataElectricGrenade.SetLevel(50);
            trapDataCanon = new TrapData(trapDataCanon.Id, trapDataCanon.baseData, trapDataCanon.levelUpStat);
            trapDataCanon.Unlock();
            trapDataCanon.SetLevel(50);

            var damage = trapDataCanon.Damage;
            canons[0].Init(damage);
            canons[0].SetActive(true);
            canons[1].Init(damage);
            canons[1].SetActive(true);
        }
        //var lenght = listHeroDatas.Length;
        //for (int i = 0; i < lenght; i++)
        //{
        //	var heroData = listHeroDatas[i];
        //	if (heroData != null)
        //	{
        //		SpawnHero(heroData, heroSpawnPoses[i].position);
        //	}
        //}

        AddBarrierBase(new Vector2(-4.295f, -4.89f));
        AddBarrierBase(new Vector2(-1.43f, -4.89f));
        AddBarrierBase(new Vector2(1.43f, -4.89f));
        AddBarrierBase(new Vector2(4.295f, -4.89f));
        PreloadEnemies();
        //hubPanel
        // currencyCoin = GameData.Instance.CurrenciesGroup.GetCoin();
        enemiesCoin = 0;
        hubPanel.ShowHero();
        // hubPanel.ShowCoin(currencyCoin);
        hubPanel.ShowCoin(0);
        LoadAutoPlay();
        LoadTimeScale();
        //Time.timeScale = 0;

        if (GameUnityData.instance.GameRemoteConfig.Function_ShowMap && Config.typeModeInGame != Config.TYPE_MODE_PvP&&!Config.showAdsTrap)
        {
            canStartGame = true;
            ReloadFormation();
            hubPanel.ShowHero();
            //Load Trap in case what Video:
            ShowTrapDatas();

        }
        else
        {
            MainGamePanel.instance.ShowMissionDetailPanel(() =>
            {
                canStartGame = true;
                ReloadFormation();
                hubPanel.ShowHero();
                //Load Trap in case what Video:
                ShowTrapDatas();
            });
            //	canStartGame = true;

        }

    }
    void ReloadFormation()
    {
        listHeroDatas = GameData.Instance.HeroesGroup.GetEquippedHeroes();
        var lenght = listHeroDatas.Length;
        for (int i = 0; i < lenght; i++)
        {
            var heroData = listHeroDatas[i];
            if (heroData != null)
            {
                SpawnHero(heroData, heroSpawnPoses[i].position);
            }
        }
    }
    public void ShowTrapDatas()
    {
        hubPanel.ShowBarrierCount(trapDataBarrier);
        hubPanel.ShowCallCount(trapDataCall);
        hubPanel.ShowMineCount(trapDataMine);
        hubPanel.ShowTrapCount(trapDataSpike);
        hubPanel.ShowFirstAirKitCount(trapDataFirstAirKit);
        hubPanel.ShowIceGrenadeCount(trapDataIceGrenade);
        hubPanel.ShowFireGrenadeCount(trapDataFireGrenade);
        hubPanel.ShowElectricGrenadeCount(trapDataElectricGrenade);
    }
    public void PreloadEnemies()
    {
        Debug.Log("PreloadEnemies");
        Dictionary<int, int> _eDic = new Dictionary<int, int>();
        List<WaveInfo> waveInfos;
        if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
        {
            Debug.Log("Edit Here=>PreloadEnemies:PvP Mode");
            Config.PvpConfig.LevelsToPlay_CurrentIndex = 0;
            return;
            //	waveInfos = MissionsGroup.GetCurrentMissionData().baseData.waveInfos;
        }
        else
            if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
        {
            waveInfos = MissionsGroup.GetCurrentMissionData().baseData.waveInfos;
        }
        else
        {
            waveInfos = DiscoveriesGroup.GetCurrentMissionData().baseData.waveInfos;
        }
        for (int i = 0; i < waveInfos.Count; i++)
        {
            WaveInfo _currentWave = waveInfos[i];
            Dictionary<int, int> _currentEDic = new Dictionary<int, int>();
            foreach (EnemyInfo _enemy in _currentWave.enemyInfos)
            {
                int _currentEID = _enemy.id;
                if (_currentEID <= 0) continue;
                int _currentENumber = 0;
                if (_currentEDic.TryGetValue(_currentEID, out _currentENumber))
                {
                    _currentEDic[_currentEID] = _currentENumber + 1;
                }
                else
                {
                    _currentEDic.Add(_currentEID, 1);
                }
            }
            foreach (var eID in _currentEDic.Keys)
            {
                int _currentMaxENumber = 0;
                if (_eDic.TryGetValue(eID, out _currentMaxENumber))
                {
                    if (_currentEDic[eID] > _currentMaxENumber)
                        _eDic[eID] = _currentEDic[eID];
                }
                else
                {
                    _eDic.Add(eID, _currentEDic[eID]);
                }
            }
        }

        foreach (var eID in _eDic.Keys)
        {
            Debug.Log("<color=yellow>Prespawn " + (eID - 1) + "_number =" + _eDic[eID] + "</color>");

            SimplePool.Preload(GameUnityData.instance.EnemyDatas[eID - 1].gameObject, _eDic[eID]);
            //   EnemyControlsPool _ePool = new EnemyControlsPool();
        }
    }

    public void SpawnHero(HeroData heroData, Vector3 heroSpawnPos)
    {
        var idHero = heroData.baseId;

        HeroControl heroControl = GameObject.Instantiate(GameUnityData.instance.HeroDatas[idHero - 1]);
        heroControl.transform.parent = transformPool;
        if (Constants.CHARACTER_SCALE != 1)
            heroControl.transform.localScale = Constants.CHARACTER_SCALE * Vector3.one;
        //   var heroControl = heroControlsPool[idHero - 1].list.Obtain(transformPool);
        var pos = heroSpawnPos;
        heroControl.transform.position = pos;
        heroControl.Init(heroData);
        heroControl.SetActive(true);
        // hubPanel.LinkHubInfoHero((HeroExControl) heroControl);
    }

    private EnemyControl SpawnEnemy(EnemyData enemyData, int level, float posX, float posY = 10.5f, float hpx = 1.0f, float damx = 1.0f)
    {
        // Debug.LogError("Spawn E");
        var idEnemy = enemyData.Id;
        //  var enemyControl = enemyControlsPool[idEnemy - 1].list.Obtain(transformPool);
        EnemyControl enemyControl = SimplePool.Spawn(GameUnityData.instance.EnemyDatas[idEnemy - 1], Vector3.zero, Quaternion.identity);
        enemyControl.transform.parent = transformPool;
        if (Constants.CHARACTER_SCALE != 1)
            enemyControl.transform.localScale = Constants.CHARACTER_SCALE * Vector3.one;
        var pos = new Vector3(posX, posY, OderLayerZ.PIVOT_POINT);
        enemyControl.transform.position = pos;
        enemyControl.Init(enemyData, level, hpx, damx);
        enemyControl.SetActive(true);

        if (enemyControl.isBoss) hubPanel.WarningBoss();

        return enemyControl;
        //mMaxHpWave += enemyControl.MaxHp;
    }

    private float hpx = 1.0f, damx = 1.0f;
    private int indexPowSpawn = -1;
    List<WaveIngameInfor> waveIngameInfors = new List<WaveIngameInfor>();
    IEnumerator IEAutoSpawnEnemy;
    private IEnumerator DoAutoSpawnEnemy()
    {
        while (!canStartGame)
        {
            yield return new WaitForSeconds(1f);
        }
        if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
            hubPanel.PvpScoreInGamePanel.StartGame();
        int numberEnemyInLine = 11;
        float space = 0.8f;
        CanAutoSkill = false;
        while (!isSpawnEnd)
        {
            MissionData currentMisisonData;
            List<WaveInfo> waveInfos;
            #region InitMissionData
            if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL || Config.typeModeInGame == Config.TYPE_MODE_PvP)
            {
                if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
                {
                    int currentPvPLevelToPlay = Config.PvpConfig.GetCurrentLevelToPlay();
                    Config.LogEvent(TrackingConstants.PVP_PLAY_COUNT);

                    hubPanel.PvpScoreInGamePanel.Record_InitLevel(currentPvPLevelToPlay);
                    //	int misisonID = MissionsGroup.IntToMissionID(currentPvPLevelToPlay);
                    Debug.Log("<color=green>PVP Start Level :</color>" + currentPvPLevelToPlay);
                    currentMisisonData = MissionsGroup.GetMissionData(currentPvPLevelToPlay);
                }
                else
                    currentMisisonData = MissionsGroup.GetCurrentMissionData();
                hpx = currentMisisonData.baseData.hpx;
                damx = currentMisisonData.baseData.damx;

                LevelConfigFirebaseObject levelFromFirebase = Constants.LevelDataFromFirebase;
                if (levelFromFirebase != null)
                {
                    for (int i = 0; i < levelFromFirebase.LevelConfigDatas.Length; i++)
                    {
                        LevelConfigObject levelConfigObject = levelFromFirebase.LevelConfigDatas[i];
                        if (levelConfigObject.levelToChange == GameData.Instance.MissionsGroup.CurrentMissionId)
                        {
                            Debug.Log("Set FirebaseData to current Level " + levelConfigObject.ToString());
                            if (levelConfigObject.hpx > 0)
                                hpx = (float)levelConfigObject.hpx;
                            if (levelConfigObject.damx > 0)
                                damx = (float)levelConfigObject.damx;
                        }
                    }
                }
                if (hpx <= 0) hpx = 1;
                if (damx <= 0) damx = 1;
                waveInfos = currentMisisonData.baseData.waveInfos;
                //Debug.LogError("Edit=>waveInfos PvP");
            }
            else
            {
                currentMisisonData = DiscoveriesGroup.GetCurrentMissionData();
                hpx = currentMisisonData.baseData.hpx;
                damx = currentMisisonData.baseData.damx;
                if (hpx <= 0) hpx = 1;
                if (damx <= 0) damx = 1;
                waveInfos = currentMisisonData.baseData.waveInfos;
            }
            #endregion
            EnemyCounter.instance.Init(waveInfos);
            bool hasEnemyInLine = false;
            int countHasEnemyInLine = 1;
            var countWave = waveInfos.Count;

            System.Action<int> droneAction = null;
            if (Config.typeModeInGame != Config.TYPE_MODE_PvP)
                droneAction = DroneManager.instance.Init(countWave);
            bool enemyAppear = false;
            #region SpawnWaveByWave

            for (int i = 0; i < countWave; i++)
            {
                WaveIngameInfor currentWaveInfor = new WaveIngameInfor();
                waveIngameInfors.Add(currentWaveInfor);
                hubPanel.ShowWaveInfo(i + 1, countWave);
                countHasEnemyInLine = 1;
                if (Config.typeModeInGame != Config.TYPE_MODE_PvP)
                    droneAction?.Invoke(i + 1);
                WaveInfo waveInfo = waveInfos[i];
                EnemyInfo[] enemyInfos = waveInfo.enemyInfos;
                indexPowSpawn = -1;
                if (waveInfo.powActive)
                {
                    indexPowSpawn = Random.Range(1, waveInfo.powValue + 1);
                }
                // Debug.LogError("Index Spawn " + indexPowSpawn);
                var count = enemyInfos.Length / numberEnemyInLine;
                for (int j = count - 1; j >= 0; j--)
                {
                    hasEnemyInLine = false;

                    for (int k = 0; k < numberEnemyInLine; k++)
                    {
                        var enemyInfo = enemyInfos[j * 11 + k];
                        var id = enemyInfo.id;
                        if (id != 0)
                        {
                            var level = enemyInfo.level;

                            //   SpawnEnemy(listEnemyDatas[id - 1], level, (-((float)numberEnemyInLine / 2f - 0.5f) + k) * space, 12, hpx, damx);

                            hasEnemyInLine = true; break;
                        }
                    }
                    //nếu có enemy trong một line thì reset biến đếm về 0
                    //nếu line có enemy thì đợi thêm 1 giây, nếu không có thì cộng tổng lại rồi đợi một thể
                    //chỉ là line cuối == 0 thì không phải đợi
                    if (hasEnemyInLine)
                    {
                        if (j != 0)
                        {
                            if (enemyAppear)
                            {
                                //   Debug.LogError("Have Enemy in line,wait " + countHasEnemyInLine);
                                yield return new WaitForSeconds(1f * countHasEnemyInLine);
                            }
                        }
                        countHasEnemyInLine = 1;
                        for (int k = 0; k < numberEnemyInLine; k++)
                        {
                            var enemyInfo = enemyInfos[j * 11 + k];
                            var id = enemyInfo.id;
                            if (id != 0)
                            {
                                var level = enemyInfo.level;
                                EnemyControl e = SpawnEnemy(listEnemyDatas[id - 1], level, (-((float)numberEnemyInLine / 2f - 0.5f) + k) * space, 10.5f, hpx, damx);
                                currentWaveInfor.AddEnemy(e);
                                enemyAppear = true;
                                //   hasEnemyInLine = true;
                            }
                        }
                    }
                    else
                    {
                        //   Debug.LogError("Have no E, countHasEnemyInLine++ " + countHasEnemyInLine);
                        countHasEnemyInLine++;
                    }
                }
                if (i != waveInfos.Count - 1)
                {
                    //nếu nhỏ hơn 10 giây để play wave tiếp, hoặc còn enemies
                    var lastCheckTime = Time.time;
                    while (Time.time < lastCheckTime + waveInfo.time && enemies.Count > 0)
                    {
                        yield return null;
                    }
                    // Debug.LogError("Wait 0.5f");
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    //  Debug.LogError("Wait 0.25f");
                    yield return new WaitForSeconds(0.25f);
                }
                currentWaveInfor.SetSpawnAll();
            }
            #endregion
            if (Config.typeModeInGame != Config.TYPE_MODE_PvP)
            {
                //if not PvP:we dont need Loop=>Break and  Check end game
                break;
            }
            else
            {
                //PvP:
                //Wait for no enemies and continue loop
                while (enemies.Count > 0)
                {
                    yield return new WaitForSeconds(1);
                }
                if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
                    hubPanel.PvpScoreInGamePanel.Record_PackCurrentRecord();
            }
        }
        #region EndSpawn
        if (Config.typeModeInGame != Config.TYPE_MODE_PvP)
        {
            isSpawnEnd = true;

            if (isSpawnEnd && !isShowedEndPanel && enemies.Count <= 0)
            {
                yield return StartCoroutine(IEShowWin());
            }
        }
        #endregion
    }
    #region Win
    private IEnumerator IEShowWin()
    {
        isShowedEndPanel = true;
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(3f));
        ShowWin();
    }

    private void ShowWin(bool fromLoseIntroMission = false)
    {
        if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
        {
            var currentMissionId = GameData.Instance.MissionsGroup.CurrentMissionId;
            //if (currentMissionId == 1005) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.LEVELUP_HERO_BACKHOME));
            // if (currentMissionId == 1008) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.QUEST_GAMEPLAY));
            // if (currentMissionId == 1010) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.AUTO_BATTLE_REWARD_GAMEPLAY));
            //if (currentMissionId == 2001) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.USE_BASE_GAMEPLAY));
            //if (currentMissionId == 2002) EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.USE_BASEPLAY_GAMEPLAY));

            int lastEnemiesCoin = 0;
            //nếu chưa thắng intro thì lần này là thắng intro và không phải force win từ lose intro
            if (!isWinIntroMission && !fromLoseIntroMission)
            {
                Config.LogEvent(TrackingConstants.MISSION_WIN_COUNT, TrackingConstants.PARAM_MISSION, TrackingConstants.VALUE_INTRO_MISSION);
            }
            else
            {
                //cộng tiền bắn quái từ đầu battle đến giờ
                lastEnemiesCoin = enemiesCoin;
            }

            List<RewardInfo> mainRewardInfos = null;
            List<RewardInfo> bonusRewardInfos = null;
            MissionsGroup.Cleared(ref mainRewardInfos, ref bonusRewardInfos, totalKills, lastEnemiesCoin);
            MainGamePanel.instance.ShowWinPanel(mainRewardInfos, bonusRewardInfos);
            Debug.Log("Endgame Win");
            GameData.Instance.UserGroup.SetGameStep(UserGroup.UserGameStep.END_MISSION);
        }
        else if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
        {
            //Set Reward MODE_PvP Here
            List<RewardInfo> mainRewardInfos = null;
            List<RewardInfo> bonusRewardInfos = null;
            MainGamePanel.instance.ShowWinPanel(mainRewardInfos, bonusRewardInfos);
        }
        else
        {
            List<RewardInfo> mainRewardInfos = null;
            List<RewardInfo> bonusRewardInfos = null;
            DiscoveriesGroup.Cleared(ref mainRewardInfos, ref bonusRewardInfos, totalKills);
            MainGamePanel.instance.ShowWinPanel(mainRewardInfos, bonusRewardInfos);
        }
    }
    #endregion
    #region Lose
    private IEnumerator IEShowLose(float delay = 3)
    {
        isShowedEndPanel = true;
        if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
        {
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(delay));
            hubPanel.PvpScoreInGamePanel.SetUserDied();
            this.PvPEndMatch();

        }
        else
        {
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(delay));
            ShowLose();
        }
    }

    private void ShowLose()
    {
        if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
        {
            //đề phòng đánh mission intro cũng thua
            if (!isWinIntroMission)
            {
                Config.LogEvent(TrackingConstants.MISSION_LOSE_COUNT, TrackingConstants.PARAM_MISSION,
                    TrackingConstants.VALUE_INTRO_MISSION);

                ShowWin(true);
            }
            else
            {
                // //cộng tiền bắn quái từ đầu battle đến giờ
                // var currentMissionId = MissionsGroup.CurrentMissionId;
                // GameData.Instance.CurrenciesGroup.Add(IDs.CURRENCY_COIN, enemiesCoin, string.Format(TrackingConstants.VALUE_MISSION, currentMissionId)); 
                var rewardInfo = MissionsGroup.Lose();
                MainGamePanel.instance.ShowLosePanel(rewardInfo);

                var currentMissionId = GameData.Instance.MissionsGroup.CurrentMissionId;
                //if (Constants.TUT_ACTIVE_GAMEPLAY_LEVELUP == 1)
                //{
                //    if (currentMissionId >= 1005)
                //        if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.LEVELUP_HERO_BACKHOME))
                //          EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.LEVELUP_HERO_BACKHOME));
                //}
            }
        }
        else
        {
            var rewardInfo = DiscoveriesGroup.Lose();
            MainGamePanel.instance.ShowLosePanel(rewardInfo);
        }
    }
    #endregion


    #region GetSet
    public void SetTapToShotActive(bool isActive)
    {
        this.tapToTarget.SetActive(isActive);
    }
    public EnemyControl GetPrioritizeEnemy(HeroExControl heroExControl, int randomLenght = 4)
    {
        var enemies = new List<EnemyControl>(GameplayController.Instance.GetEnemies());
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
        if (heroExControl != null)
        {
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

    public float GetBonusDamageFormationByElement(int elementType)
    {
        if (listHeroDatas == null) return 0;
        var count = listHeroDatas.Length;
        int countHero = 0;
        for (int i = 0; i < count; i++)
        {
            var heroData = listHeroDatas[i];
            if (heroData != null && heroData.Element == elementType) countHero++;
        }

        if (countHero >= 2) return ConfigStats.GetBonusDamageFormationByElement();
        else return 0f;
    }
    public virtual List<HeroControl> GetHeroes()
    {
        return heroes;
    }

    public List<HeroExControl> GetHeroExs()
    {
        return heroExs;
    }
    public void AddHero(HeroControl hero)
    {
        heroes.Add(hero);
    }

    public void AddHeroEx(HeroExControl hero)
    {
        heroExs.Add(hero);
    }

    public void RemoveHero(HeroControl hero)
    {
        heroes.Remove(hero);
    }

    public void RemoveHeroEx(HeroExControl hero)
    {
        heroExs.Remove(hero);

        if (heroExs.Count <= 0)
        {
            CallShowLose();
        }
    }
    public void CallShowLose(float delay = 3)
    {
        if (!isShowedEndPanel)
            StartCoroutine(IEShowLose(delay));
    }
    public virtual List<EnemyControl> GetEnemies()
    {
        return enemies;
    }

    public void AddEnemy(EnemyControl enemy)
    {
        enemies.Add(enemy);
        if (enemies.Count == 1)
        {
            CanAutoSkill = false;
            if (IEAutoSkill != null)
                StopCoroutine(IEAutoSkill);
            IEAutoSkill = DoActiveAutoSkill();
            StartCoroutine(IEAutoSkill);
            //   Debug.LogError("Only ONE==> CHECK ING              SKIL L L L L");
        }
    }
    public virtual void RemoveEnemy(EnemyControl enemy)
    {
        //=========POW============
        if (indexPowSpawn > 0)
        {
            indexPowSpawn--;
            if (indexPowSpawn <= 0)
            {
                Vector3 _spawnPos = enemy.transform.position;
                _spawnPos.z = 0;
                SimplePool.Spawn(GameUnityData.instance.BoosterPOW, _spawnPos, Quaternion.identity);
            }
        }
        //================================
        enemies.Remove(enemy);
        if (waveIngameInfors != null && waveIngameInfors.Count > 0)
        {
            foreach (WaveIngameInfor item in waveIngameInfors)
            {
                if (item.RemoveEnemy(enemy))
                {
                    if (item.isDone && waveIngameInfors.Contains(item)) waveIngameInfors.Remove(item);
                    break;
                }
            }
        }
        if (enemies.Count <= 0)
        {
            CanAutoSkill = false;
            if (IEAutoSkill != null)
            {
                StopCoroutine(IEAutoSkill); IEAutoSkill = null;
            }
        }
        var enemyEx = (EnemyExControl)enemy;
        var id = enemyEx.enemyData.Id;
        //nếu ko phải map intro thì bắn đc enemy nào thì cộng coin enemy đó

        if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
        {
            hubPanel.PvpScoreInGamePanel.SetScore(0, enemy.Score);
        }
        else
        {
            int coin = enemyEx.CoinDrop;
            enemiesCoin += coin;
            hubPanel.ShowCoin(enemiesCoin);
        }
        EnemyCounter.instance.OnEnemyDie();



        if (!totalKills.ContainsKey(id))
        {
            var count = 1;
            totalKills.Add(id, count);
        }
        else
        {
            var count = totalKills[id]++;
            totalKills[id] = count;
        }

        if (isSpawnEnd && !isShowedEndPanel && enemies.Count <= 0)
        {
            StartCoroutine(IEShowWin());
        }
    }
    #endregion
    #region Spawn/Destroy
    public void SpawnHp(Vector3 pos, float hp, bool crit = false, int typeAttack = TextHp.TEXT_DAMAGE_HP)
    {
        if (!crit) return;
        var textHp = textHpsPool.Obtain(transformPool);
        textHp.transform.position = pos;
        textHp.SetInfo(hp, crit, typeAttack);
        textHp.SetActive(true);
    }
    public Bullet SpawnBullet(Bullet bullet, Vector3 pos, Quaternion rot, InfoAttacker infoAttacker)
    {
        var pool = bulletPools.Get(bullet);
        var obj = pool.Spawn(new Vector3(pos.x, pos.y, OderLayerZ.Z_BULLET), true);
        obj.transform.rotation = rot;
        obj.Init(infoAttacker);

        BulletManager.instance.AddBullet(obj);
        return obj;
    }

    public void ReleaseBullet(Bullet bullet)
    {
        BulletManager.instance.RemoveBullet(bullet);
        bulletPools.Release(bullet);
        if (bullet.gameObject.activeSelf)
            bullet.gameObject.SetActive(false);
    }

    public Impact SpawnImpact(Impact impact, Vector3 pos, Quaternion rot)
    {
        var pool = impactPools.Get(impact);
        var obj = pool.Spawn(new Vector3(pos.x, pos.y, OderLayerZ.Z_BULLET), true);
        obj.transform.rotation = rot;

        return obj;
    }

    public void ReleaseImpact(Impact impact)
    {
        impactPools.Release(impact);
    }

    public virtual void SpawnCoinDrop(Vector3 pos, float delay)
    {
        if (Config.typeModeInGame == Config.TYPE_MODE_PvP) return;
        var coinDrop = coinDropsPool.Obtain(transformPool);
        coinDrop.transform.position = new Vector3(pos.x, pos.y, OderLayerZ.Z_BULLET);
        coinDrop.SetActive(true);
        coinDrop.Play(delay);
    }
    public EnemyControl SpawnEnemyWithId(int id, int level, float posX, float posY)
    {
        return SpawnEnemy(listEnemyDatas[id - 1], level, posX, posY, this.hpx, this.damx);
    }

    public void SpawnTextInGame(string text, int color, Vector3 position)
    {
        if (position.y >= 4)
            position.y = 4;
        AutoDespawnObject textObject = SimplePool.Spawn(GameUnityData.instance.Text_ingame, position, Quaternion.identity);
        textObject.TextShow(text, color, 2);
        textObject.DOKill();
        textObject.transform.DOMoveY(0.3f, 1.0f).SetRelative(true);
    }

    private void OnWaveEndEvent(WaveEndEvent e)
    {
        if (e.timePlay <= 10)
        {
            SpawnTextInGame("Astonishing\n+2000", 0, e.position);
            hubPanel.PvpScoreInGamePanel.SetScore(0, 2000);
        }
        else
        if (e.timePlay <= 15)
        {
            SpawnTextInGame("Great\n+1500", 1, e.position);
            hubPanel.PvpScoreInGamePanel.SetScore(0, 1500);
        }
        else
        if (e.timePlay <= 20)
        {
            SpawnTextInGame("Excellent\n+1000", 2, e.position);
            hubPanel.PvpScoreInGamePanel.SetScore(0, 1000);
        }
    }

    #endregion
    IEnumerator IEAutoSkill;
    IEnumerator DoActiveAutoSkill()
    {
        yield return new WaitForSeconds(2);
        CanAutoSkill = true;
        IEAutoSkill = null;
    }
    public void SetHeroesLookAt(Vector3 targetPos)
    {
        if (!canStartGame) return;
        holdingTap = true;
        var count = heroes.Count;
        for (int i = 0; i < count; i++)
        {
            heroes[i].LookAt(targetPos);
        }

        count = canons.Count;
        for (int i = 0; i < count; i++)
        {
            canons[i].LookAt(targetPos);
        }
    }

    public void SetHeroesLookAt()
    {
        holdingTap = false;
    }
    #region GameActionControl
    public void Skip()
    {
        TutorialsManager.Instance.SkipFixedTutotiral();
        int lastEnemiesCoin = 0;
        //nếu chưa thắng intro thì lần này là thắng intro và không phải force win từ lose intro
        if (!isWinIntroMission)
        {
            Config.LogEvent(TrackingConstants.MISSION_WIN_COUNT, TrackingConstants.PARAM_MISSION, TrackingConstants.VALUE_INTRO_MISSION);
        }
        else
        {
            //cộng tiền bắn quái từ đầu battle đến giờ
            lastEnemiesCoin = enemiesCoin;
        }

        List<RewardInfo> mainRewardInfos = null;
        List<RewardInfo> bonusRewardInfos = null;
        MissionsGroup.Cleared(ref mainRewardInfos, ref bonusRewardInfos, totalKills, lastEnemiesCoin);
        var lastWinMissionId = GameData.Instance.MissionsGroup.LastWinMissionId;
        int count = mainRewardInfos.Count;
        for (int i = 0; i < count; i++)
        {
            var item = mainRewardInfos[i];
            LogicAPI.ClaimReward(item, string.Format(TrackingConstants.VALUE_MISSION, lastWinMissionId), false);
        }

        count = bonusRewardInfos.Count;
        for (int i = 0; i < count; i++)
        {
            var item = bonusRewardInfos[i];
            LogicAPI.ClaimReward(item, string.Format(TrackingConstants.VALUE_MISSION, lastWinMissionId), false);
        }

        BackToHome();
    }
    public void BreakGame()
    {
        StopAllCoroutines();
        //foreach (var item in this.enemies)
        //{
        //    item.gameObject.SetActive(false);
        //}
        foreach (var item in this.enemies)
        {
            item.gameObject.SetActive(false);
        }

    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = currentTimeScale;
    }

    public void BackToHome()
    {
        Time.timeScale = 1f;
        SceneLoader.LoadScene("Home", false);
        LoadGameWithTrap(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneLoader.LoadScene("Gameplay", false);
        LoadGameWithTrap();
    }

    public void NextGame()
    {
        Time.timeScale = 1f;
        SceneLoader.LoadScene("Gameplay", false);
        LoadGameWithTrap();
    }

    public void LoadGameWithTrap(bool _showAdsTrap=true)
    {
        Config.showAdsTrap = _showAdsTrap;
    }

    public void SetAutoPlay(bool isAuto)
    {
        autoPlay = !isAuto;
        ChangeAutoPlay();
    }

    public void ChangeAutoPlay()
    {
        autoPlay = !autoPlay;
        //save config
        GameData.Instance.GameConfigGroup.SetAutoTarget(autoPlay);
        LoadAutoPlay();
    }

    public void LoadAutoPlay()
    {
        //load config
        autoPlay = GameData.Instance.GameConfigGroup.EnableAutoTarget;
        tapToTarget.SetActive(true);
        //	tapToTarget.SetActive(!autoPlay);
        hubPanel.ShowTextAutoPlay();
    }

    public void ChangeTimeScale()
    {
        var enableX2Speed = GameData.Instance.GameConfigGroup.EnableX2Speed;
        var enableX3Speed = GameData.Instance.GameConfigGroup.EnableX3Speed;
        if (!enableX2Speed && !enableX3Speed)
        {
            GameData.Instance.GameConfigGroup.SetX2Speed(true);
            GameData.Instance.GameConfigGroup.SetX3Speed(false);
        }
        // else if (enableX2Speed && !enableX3Speed)
        // {
        //     GameData.Instance.GameConfigGroup.SetX2Speed(false);
        //     GameData.Instance.GameConfigGroup.SetX3Speed(true);
        // }
        else
        {
            GameData.Instance.GameConfigGroup.SetX2Speed(false);
            GameData.Instance.GameConfigGroup.SetX3Speed(false);
        }

        LoadTimeScale();
    }

    public void LoadTimeScale()
    {
        //load config
        // var enableX3Speed = GameData.Instance.GameConfigGroup.EnableX3Speed;
        // if (enableX3Speed)
        // {
        //     currentTimeScale = 2f;
        // }
        // else
        // {
        var enableX2Speed = GameData.Instance.GameConfigGroup.EnableX2Speed;
        if (enableX2Speed)
        {
            currentTimeScale = GameUnityData.instance.GameRemoteConfig.Speed_ingamex2;
        }
        else
        {
            currentTimeScale = GameUnityData.instance.GameRemoteConfig.Speed_ingamex1;
        }
        if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
            currentTimeScale = GameUnityData.instance.GameRemoteConfig.Speed_PvP;
        // }
        Time.timeScale = currentTimeScale;
        hubPanel.btnTimeScale.GetComponent<ButtonHelper>().ChangeValue(enableX2Speed);
    }
    public void PvPEndMatch()
    {
        if (IEAutoSpawnEnemy != null)
        {
            StopCoroutine(IEAutoSpawnEnemy);
            IEAutoSpawnEnemy = null;
        }
        hubPanel.PvpScoreInGamePanel.UpdateBarriers(this.barriersPool);
        hubPanel.PvpScoreInGamePanel.Record_FinishRecord();
        StartCoroutine(DoPVPEndGame());
    }


    IEnumerator DoPVPEndGame()
    {
        yield return Yielders.Get(1);
        MainGamePanel.instance.ShowPvPEndgamePanel(hubPanel.PvpScoreInGamePanel);
    }
    #endregion
    #region Trap
    public void AddBarrierBase(Vector2 pos)
    {
        HeroControl barrier = barriersPool[1].list.Obtain(transformPool);
        barrier.Init(trapDataBarrier.HP, trapDataBarrier.HPRegen);
        barrier.transform.position = new Vector3(pos.x, pos.y, OderLayerZ.PIVOT_POINT);
        barrier.SetActive(true);
        if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
        {
            barrier.OnGetHit = (dam) =>
            {
                hubPanel.PvpScoreInGamePanel.Record_LogBarrierTakeDamage(dam);
            };
        }
    }

    public void AddBarrier(Vector2 pos)
    {
        trapDataBarrier.Use();
        hubPanel.ShowBarrierCount(trapDataBarrier);

        var barrier = barriersPool[1].list.Obtain(transformPool);
        barrier.Init(trapDataBarrier.HP, trapDataBarrier.HPRegen);
        barrier.transform.position = new Vector3(pos.x, pos.y, OderLayerZ.PIVOT_POINT);
        barrier.SetActive(true);
    }

    public void Call()
    {
        trapDataCall.Use();
        hubPanel.ShowCallCount(trapDataCall);

        var trap = trapsPool[0].list.Obtain(transformPool);
        trap.Init(trapDataCall);
        trap.transform.position = Vector3.zero;
        trap.SetActive(true);
        trap.Attack();
    }

    public void AddMine(Vector2 pos)
    {
        trapDataMine.Use();
        hubPanel.ShowMineCount(trapDataMine);

        var mine = trapsPool[1].list.Obtain(transformPool);
        mine.Init(trapDataMine);
        mine.transform.position = new Vector3(pos.x, pos.y, OderLayerZ.PIVOT_POINT);
        mine.SetActive(true);
    }

    public void AddTrap(Vector2 pos)
    {
        trapDataSpike.Use();
        hubPanel.ShowTrapCount(trapDataSpike);

        var trap = trapsPool[2].list.Obtain(transformPool);
        trap.Init(trapDataSpike);
        trap.transform.position = new Vector3(pos.x, pos.y, OderLayerZ.PIVOT_POINT);
        trap.SetActive(true);
    }

    public void FirstAirKit()
    {
        //skill buff từ trap nên dùng ID_TRAP
        var skillBuff = new SkillBuff(SkillBuff.ID_TRAP_FIRST_AIR_KIT, effectBuffFirstAirKit, trapDataFirstAirKit.Duration);

        trapDataFirstAirKit.Use();
        hubPanel.ShowFirstAirKitCount(trapDataFirstAirKit);
        skillBuff.healHP = trapDataFirstAirKit.Damage;
        skillBuff.moreRegenHP = trapDataFirstAirKit.HPRegen;

        var count = heroExs.Count;
        for (int i = 0; i < count; i++)
        {
            var hero = heroExs[i];
            if (!hero.IsDead()) hero.AddBuff(skillBuff);
        }
    }

    public void AddIceGrenade(Vector2 pos)
    {
        trapDataIceGrenade.Use();
        hubPanel.ShowIceGrenadeCount(trapDataIceGrenade);

        var trap = trapsPool[3].list.Obtain(transformPool);
        trap.Init(trapDataIceGrenade);
        trap.transform.position = new Vector3(pos.x, pos.y, OderLayerZ.PIVOT_POINT);
        trap.SetActive(true);
        trap.Attack();
    }

    public void AddFireGrenade(Vector2 pos)
    {
        trapDataFireGrenade.Use();
        hubPanel.ShowFireGrenadeCount(trapDataFireGrenade);

        var trap = trapsPool[4].list.Obtain(transformPool);
        trap.Init(trapDataFireGrenade);
        trap.transform.position = new Vector3(pos.x, pos.y, OderLayerZ.PIVOT_POINT);
        trap.SetActive(true);
        trap.Attack();
    }

    public void AddElectricGrenade(Vector2 pos)
    {
        trapDataElectricGrenade.Use();
        hubPanel.ShowElectricGrenadeCount(trapDataElectricGrenade);

        var trap = trapsPool[5].list.Obtain(transformPool);
        trap.Init(trapDataElectricGrenade);
        trap.transform.position = new Vector3(pos.x, pos.y, OderLayerZ.PIVOT_POINT);
        trap.SetActive(true);
        trap.Attack();
    }
    #endregion
    #region Booster
    public void UseBooster(float timeActive)
    {
        if (IEDoBooster != null)
            StopCoroutine(IEDoBooster);
        IEDoBooster = DoUseBooster(timeActive);
        StartCoroutine(IEDoBooster);
        if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
        {
            hubPanel.PvpScoreInGamePanel.SetScore(0, 500);
        }
    }
    IEnumerator IEDoBooster;
    IEnumerator DoUseBooster(float timeActive)
    {
        this.BuffHeroObject.ASAddPercent = .5f;
        this.BuffHeroObject.DamAddPercent = .5f;

        var damage = trapDataCanon.Damage;
        var baseLevel = GameData.Instance.BaseGroup.GetCurrentBase().level;

        canons[0].Init(damage * (1 + this.BuffHeroObject.DamAddPercent));
        canons[0].SetActive(true);
        canons[1].Init(damage * (1 + this.BuffHeroObject.DamAddPercent));
        canons[1].SetActive(true);



        yield return new WaitForSecondsRealtime(timeActive);
        this.BuffHeroObject.ASAddPercent = 0;
        this.BuffHeroObject.DamAddPercent = 0;
        IEDoBooster = null;

        // var baseLevel = GameData.Instance.BaseGroup.GetCurrentBase().level;
        canons[0].Init(damage);
        canons[1].Init(damage);
        if (baseLevel < 9)
        {

            canons[0].SetActive(false);
        }
        if (baseLevel < 10)
        {

            canons[1].SetActive(false);
        }

    }
    #endregion
    public void ShakeCamera()
    {
        cameraShake.SetShakeCamera(0.5f, 0.2f);
    }

    public void EndShakeCamera()
    {
        cameraShake.EndShakeCamera();
    }


}


public class BuffObject
{
    public float DamAddPercent;
    public float ASAddPercent;
}

public class WaveIngameInfor
{
    public List<EnemyControl> enemies;
    public float timeStart;
    bool isSpawnAll = false;
    Vector3 positionLastDead = Vector3.zero;

    public bool isDone { get; set; }
    public WaveIngameInfor()
    {
        timeStart = Time.time;
        enemies = new List<EnemyControl>();
        isSpawnAll = false;
        isDone = false;
    }
    public void AddEnemy(EnemyControl e)
    {
        if (!enemies.Contains(e))
            enemies.Add(e);
    }
    public bool RemoveEnemy(EnemyControl e)
    {
        if (enemies.Contains(e))
        {
            positionLastDead = e.transform.position;
            enemies.Remove(e);
            if (isSpawnAll && enemies.Count == 0)
                EndWave();
            return true;
        }
        return false;
    }
    public void SetSpawnAll()
    {
        isSpawnAll = true;
        if (enemies.Count == 0)
            EndWave();
    }
    void EndWave()
    {
        float timePlay = Time.time - timeStart;
        //Debug.LogError("end wave on " + timePlay);
        EventDispatcher.Raise(new WaveEndEvent() { timePlay = timePlay, position = positionLastDead });
        isDone = true;
    }

}