using System;
using FoodZombie;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Scripts.UI.Drone_Panel
{
	public class DroneManager : MonoBehaviour
	{
		private static DroneManager mInstance;
		public static DroneManager instance
		{
			get
			{
				if (mInstance == null)
					mInstance = FindObjectOfType<DroneManager>();
				return mInstance;
			}
		}
		private int _waveSpawn = -1;
		public DroneController drone;
		public SkeletonDataAsset[] skeletonDataAssets;
		public int minSpawn = 1;
		public int maxSpawn = 3;
		private static int playedGame;
		private static int targetGame;
		private static bool isEnable = false;

		DroneController.DroneType DroneIndex;
		bool isShow = false;
		public Action<int> Init(int totalWave)
		{

			isShow = false;
			var currentMissionId = GameData.Instance.MissionsGroup.CurrentMissionId;
			if (!GameData.Instance.MissionsGroup.IsWinIntroMission()
				|| currentMissionId <= 2001) return null;

			var rd = Random.Range(0f, 100f);
			DroneIndex = DroneController.DroneType.Ads;
			if (rd <= 25f) DroneIndex = DroneController.DroneType.IAP;

			playedGame++;

			if (playedGame >= targetGame)
			{
				playedGame = 0;
				targetGame = Random.Range(minSpawn, maxSpawn + 1);
				isEnable = true;
			}
			else
			{
				isEnable = false;
				return null;
			}

			var maxWaveSpawnDrone = totalWave > 4 ? 4 : totalWave;
			if (maxWaveSpawnDrone < 2) return null;
			_waveSpawn = Random.Range(2, maxWaveSpawnDrone);
			return CallRandomDrone;
		}

		public void CallRandomDrone(int wave)
		{
			if (!isEnable) return;
			bool canShow = false;
			switch (DroneIndex)
			{
				case DroneController.DroneType.Ads:
					if (wave % 3 == 0)
					{
						if (Random.Range(0, 100) <= 30)
						{
							canShow = true;
						}
					}
					break;
				case DroneController.DroneType.IAP:
					if (_waveSpawn != wave) return;
					canShow = true;
					break;
				default:
					if (_waveSpawn != wave) return;
					canShow = true;
					break;
			}
			if (canShow && !isShow && !drone.isClaimed)
			{
				drone.Init(skeletonDataAssets[(int)DroneIndex], DroneIndex);
				if (DroneIndex == DroneController.DroneType.IAP)
				{
					//IAP show 1 lan
					isShow = true;
				}
			}
		}

#if UNITY_EDITOR
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.O))
			{
				var index = DroneController.DroneType.Ads;
				drone.Init(skeletonDataAssets[(int)index], index);
			}
			if (Input.GetKeyDown(KeyCode.P))
			{
				var index = DroneController.DroneType.IAP;
				drone.Init(skeletonDataAssets[(int)index], index);
			}
		}
#endif
	}
}