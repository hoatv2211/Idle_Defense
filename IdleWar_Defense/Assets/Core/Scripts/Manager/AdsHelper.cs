using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Service.Ads;
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
				if (!AdsManager.Instance.TapjoyClient.IsOfferwallReady())
					AdsManager.Instance.TapjoyClient.LoadOfferwall();
			}
		}

		#endregion

		//=============================================

		#region Methods

		//---------------------------------------------
		// TapJoy
		//---------------------------------------------

		public static void __ShowTapJoyOfferwall()
		{
			Instance.ShowTapjoyOfferWall();
		}

		public void ShowTapjoyOfferWall()
		{
			AdsManager.Instance.TapjoyClient.ShowOfferWall();
		}

		public static int __GetTapJoyBallance()
		{
			return Instance.GetTapJoyBallance();
		}

		public int GetTapJoyBallance()
		{
			return AdsManager.Instance.TapjoyClient.currentBallance;
		}

		public static bool __IsTapjoyOfferwallReady()
		{
			return Instance.IsTapjoyOfferwallReady();
		}

		public bool IsTapjoyOfferwallReady()
		{
			return AdsManager.Instance.TapjoyClient.IsOfferwallReady();
		}

		public static void __UpdateTapJoyBallance(Action<bool> pOnResponse)
		{
			Instance.UpdateTapJoyBallance(pOnResponse);
		}

		public void UpdateTapJoyBallance(Action<bool> onSuccess)
		{
			AdsManager.Instance.TapjoyClient.UpdateCurrencyBallance(onSuccess);
		}

		public static void __SpentCurrencyBallance(Action<bool> pOnResponse)
		{
			Instance.SpentCurrencyBallance(pOnResponse);
		}

		public void SpentCurrencyBallance(Action<bool> pOnResponse)
		{
			int all = AdsManager.Instance.TapjoyClient.currentBallance;
			AdsManager.Instance.TapjoyClient.SpendCurrencyBallance(all, pOnResponse);
		}

		public static void __AwardTapJoyCurrency(int pAmount, Action<bool> pOnResponse)
		{
			Instance.AwardTapJoyCurrency(pAmount, pOnResponse);
		}

		public void AwardTapJoyCurrency(int pAmount, Action<bool> pOnResponse)
		{
			AdsManager.Instance.TapjoyClient.AwardCurrency(pAmount, pOnResponse);
		}

		//---------------------------------------------
		// Rewarded Ads
		//---------------------------------------------

		public static bool __IsVideoRewardedAdReady(RewardedAdNetwork pNetwork = RewardedAdNetwork.None)
		{
			return Instance.IsVideoRewardedAdReady(pNetwork);
		}

		private bool IsVideoRewardedAdReady(RewardedAdNetwork pNetwork = RewardedAdNetwork.None)
		{
#if UNITY_EDITOR
			return true;
#else
            if (pNetwork == RewardedAdNetwork.None)
            {
                if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.AdMob))
                    return true;
                else if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.UnityAds))
                    return true;
                else if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.AudienceNetwork))
                    return true;
                else if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.IronSource))
                    return true;
            }
            else if (pNetwork == RewardedAdNetwork.UnityAds)
            {
                if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.UnityAds))
                    return true;
            }
            else if (pNetwork == RewardedAdNetwork.AdMob)
            {
                if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.AdMob))
                    return true;
            }
            else if (pNetwork == RewardedAdNetwork.AudienceNetwork)
            {
                if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.AudienceNetwork))
                    return true;
            }
            else if (pNetwork == RewardedAdNetwork.IronSource)
            {
                if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.IronSource))
                    return true;
            }
            return false;
#endif
		}

		public static bool __ShowVideoRewardedAd(string id, Action<bool> pOnCompleted, RewardedAdNetwork pNetwork = RewardedAdNetwork.None)
		{
			bool showed = Instance.ShowVideoRewardedAd(id, pOnCompleted, pNetwork);

			return showed;
		}

		private bool ShowVideoRewardedAd(string id, Action<bool> pOnCompleted, RewardedAdNetwork pNetwork = RewardedAdNetwork.None)
		{

#if UNITY_EDITOR
			mOnRewardedAdCompleted = pOnCompleted;
			OnRewardedAdCompleted(RewardedAdNetwork.None);
			return true;
#else

            AdsManager.Instance.onRewardedAdCompleted -= OnRewardedAdCompleted;
            AdsManager.Instance.onRewardedAdSkipped -= OnRewardedAdSkipped;
            AdsManager.Instance.onRewardedAdCompleted += OnRewardedAdCompleted;
            AdsManager.Instance.onRewardedAdSkipped += OnRewardedAdSkipped;

            mOnRewardedAdCompleted = pOnCompleted;
            var network = RewardedAdNetwork.None;
            if (pNetwork == RewardedAdNetwork.None)
            {
                network = AdsManager.Instance.ShowRewardedAdRandomly(id);
            }
            else
            {
                network = pNetwork;
                AdsManager.Instance.ShowRewardedAd(id, network);
            }

            if (network != RewardedAdNetwork.None)
            {
                return true;
            }
            return false;
#endif
		}

		private void OnRewardedAdSkipped(RewardedAdNetwork pNetwork)
		{
			AdsManager.Instance.onRewardedAdSkipped -= OnRewardedAdSkipped;
			mOnRewardedAdCompleted.Raise(false);
			mOnRewardedAdCompleted = null;

			StartCoroutine(ForceLoadAllAds());
		}

		private void OnRewardedAdCompleted(RewardedAdNetwork pNetwork)
		{
			m_TimeShowInterstitial = DateTime.Now;
			m_TimeShowInterstitial = m_TimeShowInterstitial.AddSeconds(Config.TIME_INTERSTITIAL_AFTER_RV);

			AdsManager.Instance.onRewardedAdCompleted -= OnRewardedAdCompleted;
			mOnRewardedAdCompleted.Raise(true);
			mOnRewardedAdCompleted = null;

			StartCoroutine(ForceLoadAllAds());
		}

		//Interstitial
		private bool IsInterstitialAdReady(InterstitialAdNetwork pNetwork = InterstitialAdNetwork.None)
		{
			// Config.LogEvent(TrackingConstants.EVENT_REQUEST_INTERSTITIAL_AD);
#if UNITY_EDITOR
			return false;
#endif
			if (pNetwork == InterstitialAdNetwork.None)
			{
				if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.AdMob))
					return true;
				else if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.UnityAds))
					return true;
				else if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.AudienceNetwork))
					return true;
				else if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.IronSource))
					return true;
			}
			else if (pNetwork == InterstitialAdNetwork.UnityAds)
			{
				if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.UnityAds))
					return true;
			}
			else if (pNetwork == InterstitialAdNetwork.AdMob)
			{
				if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.AdMob))
					return true;
			}
			else if (pNetwork == InterstitialAdNetwork.AudienceNetwork)
			{
				if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.AudienceNetwork))
					return true;
			}
			else if (pNetwork == InterstitialAdNetwork.IronSource)
			{
				if (AdsManager.Instance.IsRewardedAdReady(RewardedAdNetwork.IronSource))
					return true;
			}
			return false;
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


			return Instance.ShowInterstitialAd(id, InterstitialAdNetwork.None, null, OnStart, OnDone);
		}





		public static bool __ShowInterstitialAd(string id, InterstitialAdNetwork pNetwork = InterstitialAdNetwork.None)
		{
			return Instance.ShowInterstitialAd(id, pNetwork);
		}

		private bool ShowInterstitialAd(string id, InterstitialAdNetwork pNetwork = InterstitialAdNetwork.None, Action<InterstitialAdNetwork> onComplete = null, Action OnStart = null, Action<bool> OnDone = null)
		{
			if (IsInterstitialAdReady())
			{
				AdsManager.Instance.ClearInterAdsCallback();
				if (OnDone != null)
					AdsManager.Instance.onInterstitialAdCompleted += ID => { if (OnDone != null) OnDone(true); };
				if (onComplete != null)
					AdsManager.Instance.onInterstitialAdCompleted += onComplete;

				var network = InterstitialAdNetwork.None;
				if (pNetwork == InterstitialAdNetwork.None)
				{
#if UNITY_EDITOR
					network = InterstitialAdNetwork.UnityAds;
					AdsManager.Instance.ShowInterstitialAd(id, network);
#else
                    network =  AdsManager.Instance.ShowInterstitialAdRandomly(id);
#endif
				}
				else
				{
					network = pNetwork;
					AdsManager.Instance.ShowInterstitialAd(id, network);
				}
				if (OnStart != null) OnStart();

				// Config.LogEvent(TrackingConstants.EVENT_SHOW_INTERSTITIAL_AD, TrackingConstants.PARAM_NETWORK, network.ToString());
				// Config.LogEvent(TrackingConstants.EVENT_COUNT_AD_IMPRESSION);

				interAds_lastShow_time = Time.realtimeSinceStartup;
				interAds_lastShow_level = GameData.Instance.MissionsGroup.CurrentMissionId;

				m_TimeShowInterstitial = DateTime.Now;
				m_TimeShowInterstitial = m_TimeShowInterstitial.AddSeconds(Config.TIME_INTERSTITIAL_AFTER_INTERSTITIAL);



				StartCoroutine(ForceLoadAllAds());
				return true;
			}
			else
if (OnDone != null) OnDone(false);

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

		private IEnumerator ForceLoadAllAds()
		{
			yield return new WaitForSeconds(1f);
			UnityEngine.Debug.Log("ForceLoadAllAds");
			AdsManager.Instance.ForceLoadAllAds();
		}
	}
}