using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;
using System.IO;
using FoodZombie;
using UnityEditor;
using UnityEngine.EventSystems;
using Utilities.Inspector;

public class IntroDesignTool : MonoBehaviour
{
    private MissionInfo currentMissionInfo;

    public List<EnemyData> enemyDatas;

    [Separator("Enemies List")]
    public SimpleTMPButton btnEnemyPrefab;
    public GameObject btnEnemyHighLightPrefab;
    public Transform transformBtnEnemiesPool;
    public List<GameObject> listBtnEnemyHighLight;
    public List<SimpleTMPButton> listBtnEnemy;
    private int currentEnemyIndex = 0;
    public SimpleTMPButton btnAddEnemy;

    [Separator("Mission setting")]
    public TMP_InputField inputMissionWaveNumber;
    //first reward
    public TMP_InputField inputMissionCoinBonus;
    public TMP_InputField inputMissionGemBonus;
    public TMP_InputField inputMissionUserEXPBonus;
    public TMP_InputField inputMissionHeroEXPBonus;
    public TMP_Dropdown dropdownGearBonus;
    //afk reward
    public TMP_InputField inputMissionCoinAFK;
    public TMP_InputField inputMissionGemAFK;
    public TMP_InputField inputMissionUserEXPAFK;
    public TMP_InputField inputMissionHeroEXPAFK;
    public TMP_Dropdown dropdownGearAFK;
    public TMP_InputField inputRateGearAFK;
    //boss reward
    public GameObject groupMissionBossReward;
    public TMP_InputField inputMissionCoinBoss;
    public TMP_InputField inputMissionGemBoss;
    public TMP_InputField inputMissionUserEXPBoss;
    public TMP_InputField inputMissionHeroEXPBoss;
    public TMP_InputField inputMissionPWFragmentBoss;
    public TMP_InputField inputMissionPWCrystalBoss;
    public TMP_InputField inputMissionPWDevineBoss;
    public TMP_Dropdown dropdownGearBoss;

    [Separator("Wave List")]
    public SimpleTMPButton btnWavePrefab;
    public GameObject btnWaveHighLightPrefab;
    public Transform transformBtnWavesPool;
    public List<GameObject> listBtnWaveHighLight;
    public List<SimpleTMPButton> listBtnWave;
    public TMP_InputField inputWaveTime;
    private int currentWaveIndex = 0;
    private WaveInfo currentWaveInfo;

    [Separator("Block List")]
    public BlockEnemy btnBlockPrefab;
    public Transform transformBtnBlocksPool;
    public List<BlockEnemy> listBtnBlock;

    [Separator("Mission Enemies List")]
    public EnemyInfoTool btnMissionEnemyPrefab;
    public Transform transformBtnMissionEnemiesPool;
    public List<EnemyInfoTool> listBtnMissionEnemy;
    private int currentMissionEnemyIndex = 0;
    private EnemyInfo currentMissionEnemyInfo;
    public SimpleTMPButton btnRemoveEnemy;

    [Separator("Wave Statistic")]
    public EnemyStatisticTool enemyStatisticToolPrefab;
    public Transform transformEnemyStatisticToolsPool;
    public List<EnemyStatisticTool> listEnemyStatisticTool;
    public TextMeshProUGUI txtTotalPower;
    public TextMeshProUGUI txtTotalCoinDrop;


    public SimpleTMPButton btnSave;

    private int currentMapIndex = 0;

    private EnemiesGroup EnemiesGroup => GameData.Instance.EnemiesGroup;
    private GearsGroup GearsGroup => GameData.Instance.GearsGroup;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameData.Instance != null);

        listBtnEnemy = new List<SimpleTMPButton>();
        listBtnEnemyHighLight = new List<GameObject>();

        btnSave.onClick.AddListener(BtnSave_Pressed);
        inputMissionWaveNumber.onEndEdit.AddListener(InputMissionWaveNumber_Changed);
        inputWaveTime.onEndEdit.AddListener(InputWaveTime_Changed);

        btnAddEnemy.onClick.AddListener(BtnAddEnemy_Pressed);
        btnRemoveEnemy.onClick.AddListener(BtnRemoveEnemy_Pressed);

        LoadAllEnemy();
        LoadIntroMission();
    }

    private void Clear()
    {
        inputMissionWaveNumber.text = "";
        //first reward
        inputMissionCoinBonus.text = "";
        inputMissionGemBonus.text = "";
        inputMissionUserEXPBonus.text = "";
        inputMissionHeroEXPBonus.text = "";
        var listGears = GearsGroup.GearDefinitions;
        var options = new List<TMP_Dropdown.OptionData>();
        options.Add(new TMP_Dropdown.OptionData("None"));
        for (int i = 0; i < listGears.Count; i++)
        {
            var gear = listGears[i + 1];
            options.Add(new TMP_Dropdown.OptionData(gear.GetSlotName() + " " + gear.GetRankName() + " " + gear.name));
        }
        dropdownGearBonus.options = options;
        dropdownGearBonus.value = 0;
        //afk reward
        inputMissionCoinAFK.text = "";
        inputMissionGemAFK.text = "";
        inputMissionUserEXPAFK.text = "";
        inputMissionHeroEXPAFK.text = "";
        listGears = GearsGroup.GearDefinitions;
        options = new List<TMP_Dropdown.OptionData>();
        options.Add(new TMP_Dropdown.OptionData("None"));
        for (int i = 0; i < listGears.Count; i++)
        {
            var gear = listGears[i + 1];
            options.Add(new TMP_Dropdown.OptionData(gear.GetSlotName() + " " + gear.GetRankName() + " " + gear.name));
        }
        dropdownGearAFK.options = options;
        dropdownGearAFK.value = 0;
        inputRateGearAFK.text = "";

        for (int i = listBtnWave.Count - 1; i >= 0; i--)
        {
            Destroy(listBtnWave[i].gameObject);
        }
        listBtnWave = new List<SimpleTMPButton>();
        listBtnWaveHighLight = new List<GameObject>();

        for (int i = listBtnBlock.Count - 1; i >= 0; i--)
        {
            Destroy(listBtnBlock[i].gameObject);
        }
        listBtnBlock = new List<BlockEnemy>();

        for (int i = listBtnMissionEnemy.Count - 1; i >= 0; i--)
        {
            Destroy(listBtnMissionEnemy[i].gameObject);
        }
        listBtnMissionEnemy = new List<EnemyInfoTool>();
        inputWaveTime.text = "";
    }

    private string GetFolderMap()
    {
        return Application.dataPath + "/Core/Resources/LevelDesign";
    }

    private void LoadAllEnemy()
    {
        for (int i = listBtnEnemy.Count - 1; i >= 0; i--)
        {
            Destroy(listBtnEnemy[i].gameObject);
        }
        listBtnEnemy = new List<SimpleTMPButton>();
        listBtnEnemyHighLight = new List<GameObject>();

        enemyDatas = GameData.Instance.EnemiesGroup.GetAllEnemyDatas();
        var lenght = enemyDatas.Count;
        for (int i = 0; i < lenght; i++)
        {
            int index = i;
            var btnEnemy = GameObject.Instantiate(btnEnemyPrefab, transformBtnEnemiesPool);
            btnEnemy.onClick.AddListener(() =>
            {
                ChoiceEnemy(index);
            });
            btnEnemy.img.sprite = AssetsCollection.instance.enemyIcon.GetAsset((int)enemyDatas[i].Id - 1);
            btnEnemy.SetActive(true);
            var btnEnemyHighLight = GameObject.Instantiate(btnEnemyHighLightPrefab, btnEnemy.transform);
            btnEnemyHighLight.transform.localPosition = Vector3.zero;
            btnEnemyHighLight.SetActive(false);
            listBtnEnemyHighLight.Add(btnEnemyHighLight);
            listBtnEnemy.Add(btnEnemy);
        }

        currentEnemyIndex = 0;
        ChoiceEnemy();
    }

    private void ChoiceEnemy()
    {
        for (int i = 0; i < listBtnEnemyHighLight.Count; i++)
        {
            listBtnEnemyHighLight[i].SetActive(false);
        }
        listBtnEnemyHighLight[currentEnemyIndex].SetActive(true);
    }

    private void ChoiceEnemy(int index)
    {
        currentEnemyIndex = index;
        ChoiceEnemy();
    }

    private void LoadIntroMission()
    {
        if (File.Exists(GetFolderMap() + "/mission_Intro.json"))
        {
            var missionInfo = JsonUtility.FromJson<MissionInfo>(ReadString(GetFolderMap() + "/mission_Intro.json"));
            if (missionInfo != null)
            {
                currentMissionInfo = missionInfo;
            }
            inputMissionWaveNumber.text = currentMissionInfo.waveNumber + "";

            //first reward
            inputMissionCoinBonus.text = currentMissionInfo.coinBonus + "";
            inputMissionGemBonus.text = currentMissionInfo.gemBonus + "";
            inputMissionUserEXPBonus.text = currentMissionInfo.userEXPBonus + "";
            inputMissionHeroEXPBonus.text = currentMissionInfo.heroEXPBonus + "";
            var listGears = GearsGroup.GearDefinitions;
            var options = new List<TMP_Dropdown.OptionData>();
            options.Add(new TMP_Dropdown.OptionData("None"));
            for (int i = 0; i < listGears.Count; i++)
            {
                var gear = listGears[i + 1];
                options.Add(new TMP_Dropdown.OptionData(gear.GetSlotName() + " " + gear.GetRankName() + " " + gear.name));
            }
            dropdownGearBonus.options = options;
            dropdownGearBonus.value = currentMissionInfo.gearBonus;

            //afk reward
            inputMissionCoinAFK.text = currentMissionInfo.coinAFK + "";
            inputMissionGemAFK.text = currentMissionInfo.gemAFK + "";
            inputMissionUserEXPAFK.text = currentMissionInfo.userEXPAFK + "";
            inputMissionHeroEXPAFK.text = currentMissionInfo.heroEXPAFK + "";
            listGears = GearsGroup.GearDefinitions;
            options = new List<TMP_Dropdown.OptionData>();
            options.Add(new TMP_Dropdown.OptionData("None"));
            for (int i = 0; i < listGears.Count; i++)
            {
                var gear = listGears[i + 1];
                options.Add(new TMP_Dropdown.OptionData(gear.GetSlotName() + " " + gear.GetRankName() + " " + gear.name));
            }
            dropdownGearAFK.options = options;
            dropdownGearAFK.value = currentMissionInfo.gearAFK;
            inputRateGearAFK.text = currentMissionInfo.rateGearAFK + "";

            //boss reward
            inputMissionCoinBoss.text = currentMissionInfo.coinBoss + "";
            inputMissionGemBoss.text = currentMissionInfo.gemBoss + "";
            inputMissionUserEXPBoss.text = currentMissionInfo.userEXPBoss + "";
            inputMissionHeroEXPBoss.text = currentMissionInfo.heroEXPBoss + "";

            inputMissionPWFragmentBoss.text = currentMissionInfo.powerFragmentBoss + "";
            inputMissionPWCrystalBoss.text = currentMissionInfo.powerCrystalBoss + "";
            inputMissionPWDevineBoss.text = currentMissionInfo.powerDevineBoss + "";

            listGears = GearsGroup.GearDefinitions;
            options = new List<TMP_Dropdown.OptionData>();
            options.Add(new TMP_Dropdown.OptionData("None"));
            for (int i = 0; i < listGears.Count; i++)
            {
                var gear = listGears[i + 1];
                options.Add(
                    new TMP_Dropdown.OptionData(gear.GetSlotName() + " " + gear.GetRankName() + " " + gear.name));
            }

            dropdownGearBoss.options = options;
            dropdownGearBoss.value = currentMissionInfo.gearBoss;
            // if (currentMissionInfo.id % 5 == 0)
            // {
            //     groupMissionBossReward.SetActive(true);
            // }
            // else
            // {
            //     groupMissionBossReward.SetActive(false);
            // }

            LoadAllWave();
            LoadAllMissionEnemy();
        }
    }

    private void BtnSave_Pressed()
    {
        currentMissionInfo.id = 0;
        currentMissionInfo.waveNumber = int.Parse(inputMissionWaveNumber.text);
        //first reward
        currentMissionInfo.coinBonus = int.Parse(inputMissionCoinBonus.text);
        currentMissionInfo.userEXPBonus = int.Parse(inputMissionUserEXPBonus.text);
        currentMissionInfo.heroEXPBonus = int.Parse(inputMissionHeroEXPBonus.text);
        currentMissionInfo.gemBonus = int.Parse(inputMissionGemBonus.text);
        currentMissionInfo.gearBonus = dropdownGearBonus.value;
        //afk reward
        currentMissionInfo.coinAFK = int.Parse(inputMissionCoinAFK.text);
        currentMissionInfo.userEXPAFK = int.Parse(inputMissionUserEXPAFK.text);
        currentMissionInfo.heroEXPAFK = int.Parse(inputMissionHeroEXPAFK.text);
        currentMissionInfo.gemAFK = int.Parse(inputMissionGemAFK.text);
        currentMissionInfo.gearAFK = dropdownGearAFK.value;
        currentMissionInfo.rateGearAFK = int.Parse(inputRateGearAFK.text);
        //boss reward
        currentMissionInfo.coinBoss = int.Parse(inputMissionCoinBoss.text);
        currentMissionInfo.userEXPBoss = int.Parse(inputMissionUserEXPBoss.text);
        currentMissionInfo.heroEXPBoss = int.Parse(inputMissionHeroEXPBoss.text);
        currentMissionInfo.gemBoss = int.Parse(inputMissionGemBoss.text);

        currentMissionInfo.powerFragmentBoss = int.Parse(inputMissionPWFragmentBoss.text);
        currentMissionInfo.powerCrystalBoss = int.Parse(inputMissionPWCrystalBoss.text);
        currentMissionInfo.powerDevineBoss = int.Parse(inputMissionPWDevineBoss.text);

        currentMissionInfo.gearBoss = dropdownGearBoss.value;

        var json = JsonUtility.ToJson(currentMissionInfo);

        WriteString(GetFolderMap() + "/mission_Intro.json", json);

        //f5 power mission
        int totalPower = 0;
        for (int j = 0; j < currentMissionInfo.waveInfos.Count; j++)
        {
            var waveInfo = currentMissionInfo.waveInfos[j];

            for (int k = 0; k < waveInfo.enemyInfos.Length; k++)
            {
                var enemyInfo = waveInfo.enemyInfos[k];
                if (enemyInfo.id != 0)
                {
                    var enemyData = EnemiesGroup.GetEnemyData(enemyInfo.id);
                    var HP = enemyData.GetHP(enemyInfo.level);
                    var damage = enemyData.GetDamage(enemyInfo.level);
                    var armor = enemyData.GetArmor(enemyInfo.level);
                    var attackSpeed = enemyData.GetAttackSpeed(enemyInfo.level);
                    var critRate = enemyData.GetCritRate(enemyInfo.level);
                    var accuracy = enemyData.GetAccuracy(enemyInfo.level);
                    var dodge = enemyData.GetDodge(enemyInfo.level);
                    var critDamage = enemyData.GetCritDamage(enemyInfo.level);

                    var power = ConfigStats.GetPower(HP, damage, armor, attackSpeed, critRate, accuracy, dodge,
                        critDamage);
                    totalPower += power;
                }
            }
        }

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    private void InputMissionWaveNumber_Changed(string s)
    {
        if (!s.Equals(""))
        {
            currentMissionInfo.waveNumber = int.Parse(inputMissionWaveNumber.text);
            LoadAllWave();
        }
    }

    private void LoadAllWave()
    {
        for (int i = listBtnWave.Count - 1; i >= 0; i--)
        {
            Destroy(listBtnWave[i].gameObject);
        }
        listBtnWave = new List<SimpleTMPButton>();
        listBtnWaveHighLight = new List<GameObject>();

        var lenght = currentMissionInfo.waveNumber;
        var wayInfos = currentMissionInfo.waveInfos;
        while (wayInfos.Count > lenght)
        {
            wayInfos.RemoveAt(currentMissionInfo.waveInfos.Count - 1);
        }
        while (currentMissionInfo.waveInfos.Count < lenght)
        {
            var enemiesInfos = new EnemyInfo[11 * 13];
            for (int j = 0; j < enemiesInfos.Length; j++)
            {
                enemiesInfos[j] = new EnemyInfo();
            }
            wayInfos.Add(new WaveInfo(currentMissionInfo.waveInfos.Count + 1, 15f, enemiesInfos));
        }
        for (int i = 0; i < lenght; i++)
        {
            int index = i;
            var btnWave = GameObject.Instantiate(btnWavePrefab, transformBtnWavesPool);
            btnWave.onClick.AddListener(() =>
            {
                ChoiceWave(index);
            });
            btnWave.SetActive(true);
            btnWave.labelTMP.text = "Wave " + (index + 1);
            var btnWaveHighLight = GameObject.Instantiate(btnWaveHighLightPrefab, btnWave.transform);
            btnWaveHighLight.transform.localPosition = Vector3.zero;
            btnWaveHighLight.SetActive(false);
            listBtnWaveHighLight.Add(btnWaveHighLight);
            listBtnWave.Add(btnWave);
        }

        currentWaveIndex = 0;
        ChoiceWave();
    }

    private void ChoiceWave()
    {
        currentWaveInfo = currentMissionInfo.waveInfos[currentWaveIndex];

        for (int i = 0; i < listBtnWaveHighLight.Count; i++)
        {
            listBtnWaveHighLight[i].SetActive(false);
        }
        listBtnWaveHighLight[currentWaveIndex].SetActive(true);

        LoadAllBlock();

        inputWaveTime.text = currentWaveInfo.time + "";
    }

    private void ChoiceWave(int index)
    {
        currentWaveIndex = index;
        ChoiceWave();
    }

    private void LoadAllBlock()
    {
        for (int i = listBtnBlock.Count - 1; i >= 0; i--)
        {
            Destroy(listBtnBlock[i].gameObject);
        }
        listBtnBlock = new List<BlockEnemy>();

        var length = currentWaveInfo.enemyInfos.Length;
        for (int i = 0; i < length; i++)
        {
            var enemyInfo = currentWaveInfo.enemyInfos[i];
            var btnBlock = GameObject.Instantiate(btnBlockPrefab, transformBtnBlocksPool);
            btnBlock.Init(i, enemyInfo, AddEnemy, RemoveEnemy);
            btnBlock.SetActive(true);
            listBtnBlock.Add(btnBlock);
        }

        ShowWaveStatistic();
    }

    private void AddEnemy(int index)
    {
        currentWaveInfo.enemyInfos[index] = currentMissionEnemyInfo;

        LoadAllBlock();
    }

    private void RemoveEnemy(int index)
    {
        currentWaveInfo.enemyInfos[index] = new EnemyInfo(0, 0);

        LoadAllBlock();
    }

    private void LoadAllMissionEnemy()
    {
        for (int i = listBtnMissionEnemy.Count - 1; i >= 0; i--)
        {
            Destroy(listBtnMissionEnemy[i].gameObject);
        }
        listBtnMissionEnemy = new List<EnemyInfoTool>();

        var length = currentMissionInfo.enemyInfos.Count;
        for (int i = 0; i < length; i++)
        {
            int index = i;
            var enemyInfo = currentMissionInfo.enemyInfos[index];
            var btnMissionEnemy = GameObject.Instantiate(btnMissionEnemyPrefab, transformBtnMissionEnemiesPool);
            btnMissionEnemy.Init(index, enemyInfo, ChoiceMissionEnemy, ChangeMissionEnemy);
            btnMissionEnemy.SetActive(true);
            listBtnMissionEnemy.Add(btnMissionEnemy);
        }

        currentMissionEnemyIndex = 0;
        ChoiceMissionEnemy();
    }

    private void ChoiceMissionEnemy()
    {
        if (currentMissionInfo.enemyInfos.Count > 0)
        {
            currentMissionEnemyInfo = currentMissionInfo.enemyInfos[currentMissionEnemyIndex];

            for (int i = 0; i < listBtnMissionEnemy.Count; i++)
            {
                listBtnMissionEnemy[i].UnChoice();
            }
            listBtnMissionEnemy[currentMissionEnemyIndex].Choice();

        }

        LoadAllBlock();
    }

    private void ChoiceMissionEnemy(int index)
    {
        currentMissionEnemyIndex = index;
        ChoiceMissionEnemy();
    }

    private void ChangeMissionEnemy(int index, int level)
    {
        var idOld = currentMissionInfo.enemyInfos[index].id;
        var levelOld = currentMissionInfo.enemyInfos[index].level;
        currentMissionInfo.enemyInfos[index].level = level;
        for (int i = 0; i < currentMissionInfo.waveNumber; i++)
        {
            var enemyInfos = currentMissionInfo.waveInfos[i].enemyInfos;
            for (int j = 0; j < enemyInfos.Length; j++)
            {
                if (enemyInfos[j].id == idOld && enemyInfos[j].level == levelOld)
                {
                    enemyInfos[j].level = level;
                }
            }
        }
        LoadAllBlock();
    }

    private void BtnAddEnemy_Pressed()
    {
        var enemyId = enemyDatas[currentEnemyIndex].Id;
        currentMissionInfo.enemyInfos.Add(new EnemyInfo(enemyId, 1));

        LoadAllMissionEnemy();
    }

    private void BtnRemoveEnemy_Pressed()
    {
        var idOld = currentMissionEnemyInfo.id;
        var levelOld = currentMissionEnemyInfo.level;
        for (int i = 0; i < currentMissionInfo.waveNumber; i++)
        {
            var enemyInfos = currentMissionInfo.waveInfos[i].enemyInfos;
            for (int j = 0; j < enemyInfos.Length; j++)
            {
                if (enemyInfos[j].id == idOld && enemyInfos[j].level == levelOld)
                {
                    enemyInfos[j].id = 0;
                    enemyInfos[j].level = 0;
                }
            }
        }

        currentMissionInfo.enemyInfos.Remove(currentMissionEnemyInfo);

        LoadAllBlock();
        LoadAllMissionEnemy();
    }

    private void InputWaveTime_Changed(string s)
    {
        if (!s.Equals(""))
        {
            currentWaveInfo.time = int.Parse(inputWaveTime.text);
        }
    }

    private void ShowWaveStatistic()
    {
        for (int i = listEnemyStatisticTool.Count - 1; i >= 0; i--)
        {
            Destroy(listEnemyStatisticTool[i].gameObject);
        }
        listEnemyStatisticTool = new List<EnemyStatisticTool>();

        var length = currentMissionInfo.enemyInfos.Count;
        int totalPower = 0;
        int totalCoin = 0;
        for (int i = 0; i < length; i++)
        {
            int index = i;
            int count = 0;
            var enemyInfo = currentMissionInfo.enemyInfos[index];
            for (int j = 0; j < currentWaveInfo.enemyInfos.Length; j++)
            {
                var enemyInfoInWave = currentWaveInfo.enemyInfos[j];
                if (enemyInfoInWave.id == enemyInfo.id && enemyInfoInWave.level == enemyInfo.level)
                {
                    count++;
                }
            }

            if (count > 0)
            {
                var enemyStatisticTool = GameObject.Instantiate(enemyStatisticToolPrefab, transformEnemyStatisticToolsPool);

                var enemyData = EnemiesGroup.GetEnemyData(enemyInfo.id);
                var HP = enemyData.GetHP(enemyInfo.level);
                var damage = enemyData.GetDamage(enemyInfo.level);
                var armor = enemyData.GetArmor(enemyInfo.level);
                var attackSpeed = enemyData.GetAttackSpeed(enemyInfo.level);
                var critRate = enemyData.GetCritRate(enemyInfo.level);
                var accuracy = enemyData.GetAccuracy(enemyInfo.level);
                var dodge = enemyData.GetDodge(enemyInfo.level);
                var critDamage = enemyData.GetCritDamage(enemyInfo.level);
                var coinDrop = enemyData.GetCoinDrop(enemyInfo.level);

                var power = ConfigStats.GetPower(HP, damage, armor, attackSpeed, critRate, accuracy, dodge, critDamage);

                enemyStatisticTool.Init(enemyInfo, count, power, coinDrop);
                totalPower += count * power;
                totalCoin += count * coinDrop;
                enemyStatisticTool.SetActive(true);
                listEnemyStatisticTool.Add(enemyStatisticTool);
            }
        }

        txtTotalPower.text = "P " + totalPower;
        txtTotalCoinDrop.text = "C " + totalCoin;
    }

    //===========================================
    private void WriteString(string path, string text)
    {
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(text);
        writer.Close();
    }

    private string ReadString(string path)
    {
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        var text = reader.ReadToEnd();
        reader.Close();

        return text;
    }
}
