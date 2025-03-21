
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Pattern.UI;
using Utilities.Service.GPGS;
using Utilities.Service.RFirebase;

namespace FoodZombie.UI
{
	public class DebugPanel : MyGamesBasePanel
	{
		#region Members

		[SerializeField] private RectTransform mButtonContainer;
		[SerializeField] private RectTransform mToggleContainer;
		[SerializeField] private Toggle mToggle;
		[SerializeField] private SimpleTMPButton mButton;
		[SerializeField] private TMP_InputField mTxtIdHeroToUnlock;
		[SerializeField] private TMP_InputField mTxtIdGearToUnlock;
		[SerializeField] private TMP_InputField mTxtIdMisisonToUnlock;
		[SerializeField] private SimpleTMPButton mBtnUnlockHero;
		[SerializeField] private SimpleTMPButton mBtnUnlockGear;
		[SerializeField] private SimpleTMPButton mBtnUnlockMission;
		private SimpleTMPButton mBtnGetTime;

		#endregion

		//=============================================

		#region MonoBehaviour

		private void Start()
		{
			mToggle.gameObject.SetActive(false);
			mButton.gameObject.SetActive(false);
#if DEVELOPMENT
			mTxtIdMisisonToUnlock.text = GameData.Instance.MissionsGroup.CurrentMissionId.ToString();
			var gameData = GameData.Instance;
			mBtnUnlockHero.onClick.AddListener(OnBtnUnlockHero_Pressed);
			mBtnUnlockGear.onClick.AddListener(OnBtnUnlockGear_Pressed);
			mBtnUnlockMission.onClick.AddListener(OnBtnUnlockMission_Pressed);
			CreateButton("Save Game", Color.green, () =>
			{
				gameData.SaveGame();
			});
			//============================================================
			// Cheat currencies
			//============================================================
			CreateButton("Add 1000 Gem", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_GEM, 1000, TrackingConstants.VALUE_CHEAT);
			});
			CreateButton("Add 1.000 Coin", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_COIN, 1000, TrackingConstants.VALUE_CHEAT);
			});
			CreateButton("Add 10.000 Coin", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_COIN, 10000, TrackingConstants.VALUE_CHEAT);
			});
			CreateButton("Add 10.000.000 Coin", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_COIN, 10000000, TrackingConstants.VALUE_CHEAT);
			});
			CreateButton("Add 1000 User Exp", Color.blue, () =>
			{
				gameData.UserGroup.AddExp(1000);
			});
			CreateButton("Add 1000 User VIP Exp", Color.blue, () =>
			{
				gameData.UserGroup.AddVipExp(1000);
			});
			CreateButton("Add 100.000 User Exp", Color.blue, () =>
			{
				gameData.UserGroup.AddExp(100000);
			});
			CreateButton("Add 1.000.000 Hero Exp", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_EXP_HERO, 1000000, TrackingConstants.VALUE_CHEAT);
			});
			CreateButton("Add 1000 Scraps", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_MATERIAL, 1000, TrackingConstants.VALUE_CHEAT);
			});
			CreateButton("Add 10 Power Fragments", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_POWER_FRAGMENT, 10, TrackingConstants.VALUE_CHEAT);
			});
			CreateButton("Add 10 Power Crystals", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_POWER_CRYSTAL, 10, TrackingConstants.VALUE_CHEAT);
			});
			CreateButton("Add 10 Devine Crystals", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_DEVINE_CRYSTAL, 10, TrackingConstants.VALUE_CHEAT);
			});
			CreateButton("Add 5 Barrier Item", Color.blue, () =>
			{
				gameData.BaseGroup.AddTrapItem(IDs.ITEM_TRAP_BARRIER, 5);
			});
			CreateButton("Add 5 Call Item", Color.blue, () =>
			{
				gameData.BaseGroup.AddTrapItem(IDs.ITEM_TRAP_CALL, 5);
			});
			CreateButton("Add 5 Mine Item", Color.blue, () =>
			{
				gameData.BaseGroup.AddTrapItem(IDs.ITEM_TRAP_MINE, 5);
			});
			CreateButton("Add 5 Trap Item", Color.blue, () =>
			{
				gameData.BaseGroup.AddTrapItem(IDs.ITEM_TRAP_TRAP, 5);
			});
			CreateButton("Add 5 First Air Kit", Color.blue, () =>
			{
				gameData.BaseGroup.AddTrapItem(IDs.ITEM_TRAP_FIRST_AIR_KIT, 5);
			});
			CreateButton("Add 5 Ice Grenade", Color.blue, () =>
			{
				gameData.BaseGroup.AddTrapItem(IDs.ITEM_TRAP_ICE_GRENADE, 5);
			});
			CreateButton("Add 5 Fire Grenade", Color.blue, () =>
			{
				gameData.BaseGroup.AddTrapItem(IDs.ITEM_TRAP_FIRE_GRENADE, 5);
			});
			CreateButton("Add 5 Electric Grenade", Color.blue, () =>
			{
				gameData.BaseGroup.AddTrapItem(IDs.ITEM_TRAP_ELECTRIC_GRENADE, 5);
			});
			CreateButton("Add 5 blue chip", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_BLUE_CHIP, 5, TrackingConstants.VALUE_CHEAT);
			});
			CreateButton("Add 5 golden chip", Color.blue, () =>
			{
				gameData.CurrenciesGroup.Add(IDs.CURRENCY_GOLDEN_CHIP, 5, TrackingConstants.VALUE_CHEAT);
			});
			//============================================================
			// Cheat data
			//============================================================
			CreateButton("Add 20 Random Hero Fragments", Color.blue, () =>
			{
				gameData.ItemsGroup.AddHeroFragmentItemRandomly(20);
			});
			CreateButton("Add 20 Random item Traps", Color.blue, () =>
			{
				gameData.BaseGroup.AddTrapItemRandomly(20);
			});

			CreateButton("Unlock Mission", Color.blue, () =>
			{
				// gameData.BaseGroup.AddTrapItemRandomly(20);
				gameData.MissionsGroup.GetCurrentMissionData().Win();
				//gameData.MissionsGroup.SetCurrentMission(1010);
			});

			CreateButton("Random Zombie View", Color.cyan, () =>
			{
				List<EnemyData> enemyDatas = GameData.Instance.EnemiesGroup.GetAllEnemyDatas();
				int r = UnityEngine.Random.Range(0, enemyDatas.Count);
				enemyDatas[r].View();
			});
			CreateButton("Unlock all heroes", Color.yellow, () =>
			{
				gameData.HeroesGroup.ClaimAllHeroes();
			});
			//CreateButton("Recruit all units", Color.cyan, () =>
			//{
			//    gameData.UnitsGroup.RecruitAllUnits();
			//});
			CreateButton("Test Crash Popup", Color.black, () =>
			{
				int[] testArray = new int[10];
				testArray[-1] = 10;
			});
			mBtnGetTime = CreateButton("Get Current Time", Color.cyan, () =>
			{
				var time = ServerManager.GetCurrentTime();
				if (time == null)
					mBtnGetTime.labelTMP.text = "Counld not connect";
				else
					mBtnGetTime.labelTMP.text = time.ToString();
			});
#if ACTIVE_FACEBOOK

			if (!FBManager.Instance.IsLoggedIn)
			{
				CreateButton("Login FB", Color.blue, () =>
				{
					FBManager.Instance.FBLogin();
				});
			}
			else
			{
				CreateButton("Logout FB", Color.red, () =>
				{
					FBManager.Instance.FBLogout();
				});
			}

#endif
			//CreateButton("Log Missing Events", Color.white, () =>
			//{
			//    StartCoroutine(IEPrepareFirebaseEvents());
			//});
			CreateButton("Signout GGPlay", Color.red, () =>
			{
				GameServices.SignOut();
			});
			//             //============================================================
			//             // Ads
			//             //============================================================
			// #if UNITY_ADS
			//             CreateButton("Show Unity Ads", Color.white, () => { AdsHelper.__ShowVideoRewardedAd(null, "", RewardedAdNetwork.UnityAds); });
			// #endif
			// #if ACTIVE_ADMOB
			//             CreateButton("Show Admob Ads", Color.white, () => { AdsHelper.__ShowVideoRewardedAd(null, "", RewardedAdNetwork.AdMob); });
			// #endif
			//============================================================
			// Toggle
			//============================================================
			CreateToggle("Enable Cheat", (isOn) =>
			{
				DevSetting.Instance.enableCheat = isOn;
			}, DevSetting.Instance.enableCheat);
			CreateToggle("Enable Cheat Time", (isOn) =>
			{
				DevSetting.Instance.enableCheatTime = isOn;
			}, DevSetting.Instance.enableCheatTime);
			CreateToggle("Show FPS", (isOn) =>
			{
				DevSetting.Instance.showFPS = isOn;
			}, DevSetting.Instance.showFPS);
			CreateToggle("Enable Log", (isOn) =>
			{
				DevSetting.Instance.enableLog = isOn;
			}, DevSetting.Instance.enableLog);
#endif
		}

		private void OnBtnUnlockHero_Pressed()
		{
			if (!mTxtIdHeroToUnlock.text.Equals(""))
			{
				var reward = new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, int.Parse(mTxtIdHeroToUnlock.text), 1);
				LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_CHEAT);
			}
		}

		private void OnBtnUnlockGear_Pressed()
		{
			if (!mTxtIdGearToUnlock.text.Equals(""))
			{
				var reward = new RewardInfo(IDs.REWARD_TYPE_GEAR, int.Parse(mTxtIdGearToUnlock.text), 1);
				LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_CHEAT);
			}
		}
		private void OnBtnUnlockMission_Pressed()
		{
			if (!mTxtIdMisisonToUnlock.text.Equals(""))
			{
				// var reward = new RewardInfo(IDs.REWARD_TYPE_GEAR, int.Parse(mTxtIdGearToUnlock.text), 1);
				//   LogicAPI.ClaimReward(reward, TrackingConstants.VALUE_CHEAT);
				int _missionIndex = -1;
				if (int.TryParse(mTxtIdMisisonToUnlock.text, out _missionIndex))
				{
					GameData.Instance.MissionsGroup.SetCurrentMission(_missionIndex);
					SceneLoader.LoadScene("Splash", false, true, null, null);
				}
			}
		}
		#endregion

		//=============================================

		#region Public

		#endregion

		//==============================================

		#region Private

		private string EventsSent
		{
			set { PlayerPrefs.SetString("Events_Sent", value); }
			get { return PlayerPrefs.GetString("Events_Sent"); }
		}

		//private IEnumerator IEPrepareFirebaseEvents()
		//{
		//    var shopItemsGroup = GameData.Instance.ShopItemsGroup;
		//    var goldPackDefinitions = shopItemsGroup.GoldPackDefinitions;
		//    for (int i = 0; i < goldPackDefinitions.Count; i++)
		//    {
		//        if (!EventsSent.Contains(goldPackDefinitions[i].sku))
		//        {
		//            RFirebaseManager.LogEvent(TrackingConstants.EVENT_BUY_IAP, goldPackDefinitions[i].sku.Replace(".", "_"), 0);
		//            Utilities.Services.FBServices.LogEvent(TrackingConstants.EVENT_BUY_IAP, goldPackDefinitions[i].sku.Replace(".", "_"), 0);
		//            EventsSent = $"{EventsSent},{goldPackDefinitions[i].sku}";
		//            yield return null;
		//        }
		//    }

		//    var specialPacks = shopItemsGroup.GetSpecialPacks();
		//    for (int i = 0; i < specialPacks.Count; i++)
		//    {
		//        if (!EventsSent.Contains(specialPacks[i].baseData.sku))
		//        {
		//            RFirebaseManager.LogEvent(TrackingConstants.EVENT_BUY_IAP, specialPacks[i].baseData.sku.Replace(".", "_"), 0);
		//            Utilities.Services.FBServices.LogEvent(TrackingConstants.EVENT_BUY_IAP, specialPacks[i].baseData.sku.Replace(".", "_"), 0);
		//            EventsSent = $"{EventsSent},{specialPacks[i].baseData.sku}";
		//            yield return null;
		//        }
		//    }

		//    var lootBoxes = shopItemsGroup.LootBoxPackDefinitions;
		//    for (int i = 0; i < lootBoxes.Count; i++)
		//    {
		//        if (!EventsSent.Contains(lootBoxes[i].sku))
		//        {
		//            RFirebaseManager.LogEvent(TrackingConstants.EVENT_BUY_IAP, lootBoxes[i].sku.Replace(".", "_"), 0);
		//            Utilities.Services.FBServices.LogEvent(TrackingConstants.EVENT_BUY_IAP, lootBoxes[i].sku.Replace(".", "_"), 0);
		//            EventsSent = $"{EventsSent},{lootBoxes[i].sku}";
		//            yield return null;
		//        }
		//    }
		//}

		private SimpleTMPButton CreateButton(string name, Color pButtonColor, UnityAction pDoSomething)
		{
			SimpleTMPButton btn = Instantiate(mButton, mButtonContainer);
			btn.gameObject.SetActive(true);
			btn.onClick.AddListener(pDoSomething);
			btn.name = name;
			btn.image.color = pButtonColor;
			btn.labelTMP.text = name;
			btn.labelTMP.color = pButtonColor.Invert();
			return btn;
		}

		private Toggle CreateToggle(string name, UnityAction<bool> pDoSomething, bool pDefault = false)
		{
			Toggle tgl = Instantiate(mToggle, mToggleContainer);
			tgl.gameObject.SetActive(true);
			tgl.onValueChanged.AddListener(pDoSomething);
			tgl.name = name;
			tgl.GetComponentInChildren<TextMeshProUGUI>().text = name;
			tgl.isOn = pDefault;
			return tgl;
		}

		#endregion

	}
}
