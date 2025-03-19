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
using UnityEngine.UI;

public class LevelDesignTool : MonoBehaviour
{
	public List<EnemyData> enemyDatas;

	[Separator("List Maps")]
	public SimpleTMPButton btnMapPrefab;
	public Transform transformBtnMapsPool;
	private MapInfo currentMapInfo;

	[Separator("Map Settings")]
	public TMP_InputField inputMapName;
	public TMP_InputField inputMissionNumber;
	public TMP_InputField inputCoinStartBonus;
	public TMP_InputField inputCoinNextMissionAdd;
	public TMP_InputField inputUserEXPStartBonus;
	public TMP_InputField inputUserEXPNextMissionAdd;
	public TMP_InputField inputHeroEXPStartBonus;
	public TMP_InputField inputHeroEXPNextMissionAdd;
	public TMP_InputField inputGemBonus;

	public SimpleTMPButton btnBuildMap;
	public SimpleTMPButton btnUpdateStats;

	[Separator("Enemies List")]
	public SimpleTMPButton btnEnemyPrefab;
	public GameObject btnEnemyHighLightPrefab;
	public Transform transformBtnEnemiesPool;
	public List<GameObject> listBtnEnemyHighLight;
	public List<SimpleTMPButton> listBtnEnemy;
	private int currentEnemyIndex = 0;
	public SimpleTMPButton btnAddEnemy;

	[Separator("Mission List")]
	public SimpleTMPButton btnMissionPrefab;
	public GameObject btnMissionHighLightPrefab;
	public TextMeshProUGUI txtMissionPowerPrefab;
	public Transform transformBtnMissionsPool;
	public List<GameObject> listBtnMissionHighLight;
	public List<SimpleTMPButton> listBtnMission;
	private int currentMissionIndex = 0;
	private MissionInfo currentMissionInfo;

	[Separator("Mission setting")]
	public TMP_InputField inputMissionWaveNumber;
	public TMP_InputField inputHPx, inputDamx;
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
	public Toggle powToggle;
	public InputField powInputField;
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
		System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Application.dataPath + "/Core/Resources/LevelDesign/");
		int countMap = dir.GetDirectories().Length - 1; //trừ folder Discovery
		for (int i = 0; i < countMap; i++)
		{
			int index = i;
			var btnMap = GameObject.Instantiate(btnMapPrefab, transformBtnMapsPool);
			btnMap.onClick.AddListener(() =>
			{
				BtnMap_Pressed(index);
			});
			btnMap.labelTMP.text = "Map " + (index + 1);
			btnMap.SetActive(true);
		}
		dir = new System.IO.DirectoryInfo(Application.dataPath + "/Core/Resources/LevelDesign/Discovery/");
		var file = dir.GetFiles();
		var countDiscovery = file.Length / 2;
		var totalInfo = new TotalInfo(countMap, countDiscovery);
		var json = JsonUtility.ToJson(totalInfo);
		WriteString(Application.dataPath + "/Core/Resources/LevelDesign/totalInfo.json", json);

		yield return new WaitUntil(() => GameData.Instance != null);

		listBtnEnemy = new List<SimpleTMPButton>();
		listBtnEnemyHighLight = new List<GameObject>();
		listBtnMission = new List<SimpleTMPButton>();
		listBtnMissionHighLight = new List<GameObject>();

		btnBuildMap.onClick.AddListener(BtnBuildMap_Pressed);
		btnUpdateStats.onClick.AddListener(BtnUpdateStats_Pressed);
		btnSave.onClick.AddListener(BtnSave_Pressed);
		inputMissionWaveNumber.onEndEdit.AddListener(InputMissionWaveNumber_Changed);
		inputHPx.onEndEdit.AddListener(InputMissionHPX_Changed);
		inputDamx.onEndEdit.AddListener(InputMissionDamX_Changed);
		inputWaveTime.onEndEdit.AddListener(InputWaveTime_Changed);
		powInputField.onEndEdit.AddListener(InputWavePOWchange);
		powToggle.onValueChanged.AddListener((s) => { InputWavePOWchange(""); });
		btnAddEnemy.onClick.AddListener(BtnAddEnemy_Pressed);
		btnRemoveEnemy.onClick.AddListener(BtnRemoveEnemy_Pressed);

		LoadMap();
		LoadAllEnemy();
		LoadAllMission();
	}

	private void Clear()
	{
		inputMapName.text = "";
		inputMissionNumber.text = "";
		inputCoinStartBonus.text = "";
		inputCoinNextMissionAdd.text = "";
		inputUserEXPStartBonus.text = "";
		inputUserEXPNextMissionAdd.text = "";
		inputHeroEXPStartBonus.text = "";
		inputHeroEXPNextMissionAdd.text = "";
		inputGemBonus.text = "";
		// for (int i = listBtnEnemy.Count - 1; i >= 0; i--)
		// {
		//     Destroy(listBtnEnemy[i].gameObject);
		// }
		// listBtnEnemy = new List<SimpleTMPButton>();
		// listBtnEnemyHighLight = new List<GameObject>();

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

		for (int i = listBtnMission.Count - 1; i >= 0; i--)
		{
			Destroy(listBtnMission[i].gameObject);
		}
		listBtnMission = new List<SimpleTMPButton>();
		listBtnMissionHighLight = new List<GameObject>();

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
		powInputField.text = "";
		this.powToggle.isOn = false;
		powInputField.gameObject.SetActive(this.powToggle.isOn);
	}

	private string GetFolderMap()
	{
		return Application.dataPath + "/Core/Resources/LevelDesign/Map" + (currentMapIndex + 1);
	}

	private void BtnMap_Pressed(int index)
	{
		currentMapIndex = index;
		if (File.Exists(GetFolderMap() + "/mapInfo.json"))
		{
			LoadMap();
			LoadAllMission();
		}
		else
		{
			Clear();
		}
	}

	private void BtnBuildMap_Pressed()
	{
		currentMapInfo = new MapInfo(currentMapIndex + 1,
										inputMapName.text,
										int.Parse(inputMissionNumber.text),
										int.Parse(inputCoinStartBonus.text),
										int.Parse(inputCoinNextMissionAdd.text),
										int.Parse(inputUserEXPStartBonus.text),
										int.Parse(inputUserEXPNextMissionAdd.text),
										int.Parse(inputHeroEXPStartBonus.text),
										int.Parse(inputHeroEXPNextMissionAdd.text),
										int.Parse(inputGemBonus.text)
		);
		var json = JsonUtility.ToJson(currentMapInfo);

		WriteString(GetFolderMap() + "/mapInfo.json", json);

		var misionNumber = currentMapInfo.missionNumber;
		for (int i = 0; i < misionNumber; i++)
		{
			var path = GetFolderMap() + "/mission_" + (i + 1) + ".json";
			if (!File.Exists(path))
			{
				WriteString(path, "");
			}
		}

		LoadAllMission();
	}

	private void BtnUpdateStats_Pressed()
	{
		currentMapInfo = new MapInfo(currentMapIndex + 1,
			inputMapName.text,
			int.Parse(inputMissionNumber.text),
			int.Parse(inputCoinStartBonus.text),
			int.Parse(inputCoinNextMissionAdd.text),
			int.Parse(inputUserEXPStartBonus.text),
			int.Parse(inputUserEXPNextMissionAdd.text),
			int.Parse(inputHeroEXPStartBonus.text),
			int.Parse(inputHeroEXPNextMissionAdd.text),
			int.Parse(inputGemBonus.text)
		);
		var json = JsonUtility.ToJson(currentMapInfo);

		WriteString(GetFolderMap() + "/mapInfo.json", json);

		var misionNumber = currentMapInfo.missionNumber;
		for (int i = 0; i < misionNumber; i++)
		{
			var path = GetFolderMap() + "/mission_" + (i + 1) + ".json";
			if (File.Exists(path))
			{
				var missionInfo = JsonUtility.FromJson<MissionInfo>(ReadString(path));
				if (missionInfo != null)
				{
					missionInfo.coinBonus = currentMapInfo.coinStartBonus + currentMapInfo.coinNextMissionAdd * i;
					missionInfo.userEXPBonus = currentMapInfo.userEXPStartBonus + currentMapInfo.userEXPNextMissionAdd * i;
					missionInfo.heroEXPBonus = currentMapInfo.heroEXPStartBonus + currentMapInfo.heroEXPNextMissionAdd * i;
					missionInfo.gemBonus = currentMapInfo.gemBonus;

					json = JsonUtility.ToJson(missionInfo);
					WriteString(path, json);
				}
			}
			else
			{
				WriteString(path, "");
			}
		}

		LoadAllMission();
	}

	private void LoadMap()
	{
		if (File.Exists(GetFolderMap() + "/mapInfo.json"))
		{
			currentMapInfo = JsonUtility.FromJson<MapInfo>(ReadString(GetFolderMap() + "/mapInfo.json"));

			inputMapName.text = currentMapInfo.mapName;
			inputMissionNumber.text = currentMapInfo.missionNumber + "";
			inputCoinStartBonus.text = currentMapInfo.coinStartBonus + "";
			inputCoinNextMissionAdd.text = currentMapInfo.coinNextMissionAdd + "";
			inputUserEXPStartBonus.text = currentMapInfo.userEXPStartBonus + "";
			inputUserEXPNextMissionAdd.text = currentMapInfo.userEXPNextMissionAdd + "";
			inputHeroEXPStartBonus.text = currentMapInfo.heroEXPStartBonus + "";
			inputHeroEXPNextMissionAdd.text = currentMapInfo.heroEXPNextMissionAdd + "";
			inputGemBonus.text = currentMapInfo.gemBonus + "";
		}
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

	private void LoadAllMission()
	{
		if (currentMapInfo != null)
		{
			for (int i = listBtnMission.Count - 1; i >= 0; i--)
			{
				Destroy(listBtnMission[i].gameObject);
			}
			listBtnMission = new List<SimpleTMPButton>();
			listBtnMissionHighLight = new List<GameObject>();

			var lenght = currentMapInfo.missionNumber;
			for (int i = 0; i < lenght; i++)
			{
				int index = i;
				var btnMission = GameObject.Instantiate(btnMissionPrefab, transformBtnMissionsPool);
				btnMission.onClick.AddListener(() =>
				{
					ChoiceMission(index);
				});
				btnMission.SetActive(true);
				btnMission.labelTMP.text = "Mission " + (index + 1);
				var btnMissionHighLight = GameObject.Instantiate(btnMissionHighLightPrefab, btnMission.transform);
				btnMissionHighLight.transform.localPosition = Vector3.zero;
				btnMissionHighLight.SetActive(false);
				var txtMissionPower = GameObject.Instantiate(txtMissionPowerPrefab, btnMission.transform);
				txtMissionPower.transform.localPosition = new Vector3(80f, 0f, 0f);
				txtMissionPower.SetActive(true);

				var missionInfo = JsonUtility.FromJson<MissionInfo>(ReadString(GetFolderMap() + "/mission_" + (index + 1) + ".json"));
				int totalPower = 0;
				if (missionInfo != null)
				{
					for (int j = 0; j < missionInfo.waveInfos.Count; j++)
					{
						var waveInfo = missionInfo.waveInfos[j];

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

								var power = ConfigStats.GetPower(HP, damage, armor, attackSpeed, critRate, accuracy, dodge, critDamage);
								totalPower += power;
							}
						}
					}
				}
				txtMissionPower.text = "P " + totalPower;

				listBtnMissionHighLight.Add(btnMissionHighLight);
				listBtnMission.Add(btnMission);
			}

			currentMissionIndex = 0;
			ChoiceMission();
		}
	}

	private void ChoiceMission()
	{
		for (int i = 0; i < listBtnMissionHighLight.Count; i++)
		{
			listBtnMissionHighLight[i].SetActive(false);
		}
		listBtnMissionHighLight[currentMissionIndex].SetActive(true);

		if (File.Exists(GetFolderMap() + "/mission_" + (currentMissionIndex + 1) + ".json"))
		{
			var missionInfo = JsonUtility.FromJson<MissionInfo>(ReadString(GetFolderMap() + "/mission_" + (currentMissionIndex + 1) + ".json"));
			if (missionInfo != null)
			{
				currentMissionInfo = missionInfo;
			}
			else
			{
				var wayInfos = new List<WaveInfo>();
				for (int i = 0; i < 10; i++)
				{
					var enemiesInfos = new EnemyInfo[11 * 13];
					for (int j = 0; j < enemiesInfos.Length; j++)
					{
						enemiesInfos[j] = new EnemyInfo();
					}
					wayInfos.Add(new WaveInfo(i + 1, 10f, enemiesInfos));
				}

				currentMissionInfo = new MissionInfo((currentMapIndex + 1) * 1000 + currentMissionIndex + 1, 10,
					1, 1,
					currentMapInfo.coinStartBonus + currentMapInfo.coinNextMissionAdd * currentMissionIndex,
					currentMapInfo.userEXPStartBonus + currentMapInfo.userEXPNextMissionAdd * currentMissionIndex,
					currentMapInfo.heroEXPStartBonus + currentMapInfo.heroEXPNextMissionAdd * currentMissionIndex,
					currentMapInfo.gemBonus,
					wayInfos,
					new List<EnemyInfo>()
					);
			}
			inputMissionWaveNumber.text = currentMissionInfo.waveNumber + "";
			inputHPx.text = currentMissionInfo.hpx + "";
			inputDamx.text = currentMissionInfo.damx + "";
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

	private void ChoiceMission(int index)
	{
		currentMissionIndex = index;
		ChoiceMission();
	}

	private void BtnSave_Pressed()
	{
		currentMissionInfo.id = (currentMapIndex + 1) * 1000 + currentMissionIndex + 1;
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

		WriteString(GetFolderMap() + "/mission_" + (currentMissionIndex + 1) + ".json", json);

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

		listBtnMission[currentMissionIndex].transform.FindInChildren("txtPower(Clone)").GetComponent<TextMeshProUGUI>().text = "P " + totalPower;

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
	private void InputMissionHPX_Changed(string s)
	{
		if (!s.Equals(""))
		{
			currentMissionInfo.hpx = int.Parse(inputHPx.text);
			LoadAllWave();
		}
	}
	private void InputMissionDamX_Changed(string s)
	{
		if (!s.Equals(""))
		{
			currentMissionInfo.damx = int.Parse(inputDamx.text);
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
		powToggle.isOn = currentWaveInfo.powActive;
		powInputField.gameObject.SetActive(this.powToggle.isOn);
		powInputField.text = currentWaveInfo.powValue + "";
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
	private void InputWavePOWchange(string s)
	{
		currentWaveInfo.powActive = this.powToggle.isOn;
		// Debug.Log("");
		if (!powInputField.text.Equals(""))
			currentWaveInfo.powValue = int.Parse(powInputField.text);

		if (currentWaveInfo.powActive)
		{
			if (currentWaveInfo.powValue <= 0) currentWaveInfo.powValue = 3;
		}
		powInputField.text = currentWaveInfo.powValue + "";
		powInputField.gameObject.SetActive(this.powToggle.isOn);
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
