using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Utilities.Common;
using Utilities.Service.RFirebase;
using Utilities.Services;
using Random = UnityEngine.Random;

namespace FoodZombie
{
	public class AdsHelper : MonoBehaviour
	{
		#region Members

		private static AdsHelper mInstance;
		public static AdsHelper Instance
		{
			get
			{
				if (mInstance == null)
					mInstance = FindObjectOfType<AdsHelper>();
				return mInstance;
			}
		}

		//private Advertising Advertising { get { return Advertising.Instance; } }

		private Action<bool> mOnRewardedAdCompleted;

		#endregion
		public DateTime m_TimeShowInterstitial = DateTime.Now;

		//=============================================

		#region MonoBehaviour

		private void Awake()
		{
			if (mInstance == null)
				mInstance = this;
			else if (mInstance != this)
				Destroy(gameObject);
		}

		//-----show ads itner------
		private void Start()
		{
			m_TimeShowInterstitial = DateTime.Now;
			m_TimeShowInterstitial = m_TimeShowInterstitial.AddSeconds(Config.TIME_INTERSTITIAL_OPENING);
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				//if (!AdsManager.Instance.TapjoyClient.IsOfferwallReady())
				//	AdsManager.Instance.TapjoyClient.LoadOfferwall();
			}
		}

		#endregion

		//=============================================

		#region Methods

		//---------------------------------------------
		// Rewarded Ads
		//---------------------------------------------

		public static bool __IsVideoRewardedAdReady()
		{
			return true;
		}


		public static bool __ShowVideoRewardedAd(string id, Action<bool> pOnCompleted)
		{
			bool showed = AdsManager.Instance.ShowAd_Reward(() => { pOnCompleted(true); });

			return showed;
		}

		private bool ShowVideoRewardedAd(string id, Action<bool> pOnCompleted)
        {

            return false;
        }

		//Interstitial
		private bool IsInterstitialAdReady()
		{

			return true;
		}

		static float interAds_lastShow_time = 0;
		static int interAds_lastShow_level = 0;
		public bool CheckAndShowAdsEndGame(string id, Action OnStart = null, Action<bool> OnDone = null)
		{
			GameRemoteConfig config = GameUnityData.instance.GameRemoteConfig;
			int currentMission = GameData.Instance.MissionsGroup.CurrentMissionId;
			if (!config.ads_inter_active)
			{
				if (OnDone != null) OnDone(false);
				return false;
			}

			if (GameData.Instance.UserGroup.isIAP())
			{
				if (OnDone != null) OnDone(false);
				return false;
			}
			if (!IsInterstitialAdReady())
			{
				if (OnDone != null) OnDone(false);
				return false;
			}

			if (currentMission < config.ads_inter_showAfterMission)
			{
				if (OnDone != null) OnDone(false);
				return false;
			}
			if (interAds_lastShow_level != 0 && currentMission - interAds_lastShow_level < config.ads_inter_showPerMission)
			{
				if (OnDone != null) OnDone(false);
				return false;
			}
			if (interAds_lastShow_time != 0 && Time.realtimeSinceStartup - interAds_lastShow_time < config.ads_inter_showAfterTime)
			{
				if (OnDone != null) OnDone(false);
				return false;
			}


			return Instance.ShowInterstitialAd(id, OnStart, OnDone);
		}



		private bool ShowInterstitialAd(string id,Action OnStart = null, Action<bool> OnDone = null)
		{
			if (IsInterstitialAdReady())
			{
				interAds_lastShow_time = Time.realtimeSinceStartup;
				interAds_lastShow_level = GameData.Instance.MissionsGroup.CurrentMissionId;

				m_TimeShowInterstitial = DateTime.Now;
				m_TimeShowInterstitial = m_TimeShowInterstitial.AddSeconds(Config.TIME_INTERSTITIAL_AFTER_INTERSTITIAL);

				AdsManager.Instance.ShowAd_Inter();

				return true;
			}
			else if (OnDone != null) OnDone(false);

			//RFirebaseManager.LogEvent(TrackingConstants.EVENT_FALL_ADS_INTERSTITIAL);
			return false;
		}

		public bool CanShowInterstitial()
		{
			if (GameData.Instance.GameConfigGroup.NoAds) return false;

			if (GameData.Instance.MissionsGroup.CurrentMissionId > Config.MIN_LEVEL_INTERSTITIAL)
			{
				DateTime now = DateTime.Now;
				if (DateTime.Compare(now, m_TimeShowInterstitial) > 0)
				{
					return true;
				}
			}
			return false;
		}

		#endregion

	}
}