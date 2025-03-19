using FoodZombie;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Service.Ads
{
	public sealed class IronSourceAppStateHandler : MonoBehaviour
	{
#if ACTIVE_IRONSOURCE
		private void Start()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void OnApplicationPause(bool pause)
		{
			IronSource.Agent.onApplicationPause(pause);
		}
#endif
	}

	//==============================================================

	public class IronSourceClient : AdClient
	{
		private const string NO_SDK_MESSAGE = "SDK missing. Please import the ironSource plugin.";

#if ACTIVE_IRONSOURCE

		protected IronSourceSettings mAdSettings;
		protected bool mIsBannerAdLoaded = false;
		protected IronSourceBannerSize mCurrentBannerAdSize = IronSourceBannerSize.SMART;
		protected IronSourceBannerPosition mCurrentBannerAdPos = IronSourceBannerPosition.BOTTOM;

		protected bool mRewardedVideoIsCompleted = false;
#endif

		#region IronSource Events

		#endregion  // ironSource-Specific Events

		#region Singleton

		private static IronSourceClient mInstance;

		public static IronSourceClient CreateClient()
		{
			if (mInstance == null)
				mInstance = new IronSourceClient();
			return mInstance;
		}

		#endregion

		#region AdClient Overrides

		public override AdNetwork Network { get { return AdNetwork.IronSource; } }

		public override bool IsBannerAdSupported { get { return true; } }

		public override bool IsInterstitialAdSupported { get { return true; } }

		public override bool IsRewardedAdSupported { get { return true; } }

		public override bool IsSdkAvail
		{
			get
			{
#if ACTIVE_IRONSOURCE
				return true;
#else
                return false;
#endif
			}
		}

		public override bool IsValid(AdType type)
		{
#if ACTIVE_IRONSOURCE
			return true;
#else
            return false;
#endif
		}

		protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

		public override void Init(AdSettings pSettings)
		{
#if ACTIVE_IRONSOURCE

			mIsInitialized = true;
			mAdSettings = pSettings as IronSourceSettings;

			// Advanced settings.
			if (mAdSettings.UseAdvancedSetting)
				SetupAdvancedSetting(mAdSettings);

			// IronSource requires a gameObject to pass the OnApplicationPause event to it agent.
			GameObject appStateHandler = new GameObject("IronSourceAppStateHandler");
			appStateHandler.hideFlags = HideFlags.HideAndDontSave;
			appStateHandler.AddComponent<IronSourceAppStateHandler>();

			IronSource.Agent.init(mAdSettings.appId.Id);


			/// Add event callbacks.
			IronSourceEvents.onBannerAdClickedEvent += OnBannerAdClicked;
			IronSourceEvents.onBannerAdLeftApplicationEvent += OnBannerAdLeftApplication;
			IronSourceEvents.onBannerAdLoadedEvent += OnBannerAdLoaded;
			IronSourceEvents.onBannerAdLoadFailedEvent += OnBannerAdLoadFailed;

			IronSourceEvents.onInterstitialAdClickedEvent += OnInterstitialAdClicked;
			IronSourceEvents.onInterstitialAdClosedEvent += OnInterstititalAdClosed;
			IronSourceEvents.onInterstitialAdLoadFailedEvent += OnInterstitialAdLoadFailed;
			IronSourceEvents.onInterstitialAdOpenedEvent += OnInterstitialAdOpened;
			IronSourceEvents.onInterstitialAdReadyEvent += OnInterstitialAdReady;
			IronSourceEvents.onInterstitialAdShowSucceededEvent += OnInterstitialAdShowSucceeded;
			IronSourceEvents.onInterstitialAdShowFailedEvent += OnInterstitialAdShowFailed;

			IronSourceEvents.onRewardedVideoAdClickedEvent += OnRewardedVideoAdClicked;
			IronSourceEvents.onRewardedVideoAdClosedEvent += OnRewardedVideoClosed;
			IronSourceEvents.onRewardedVideoAdEndedEvent += OnRewardedVideoAdEnded;
			IronSourceEvents.onRewardedVideoAdOpenedEvent += OnRewardedVideoAdOpened;
			IronSourceEvents.onRewardedVideoAdRewardedEvent += OnRewardedVideoAdRewarded;
			IronSourceEvents.onRewardedVideoAdShowFailedEvent += OnRewardedVideoAdShowFailed;
			IronSourceEvents.onRewardedVideoAdStartedEvent += OnRewardedVideoAdStarted;
			IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += OnRewardedVideoAvailabilityChanged;

			IronSourceEvents.onSegmentReceivedEvent += OnSegmentReceived;
			IronSourceEvents.onImpressionSuccessEvent += ImpressionSuccessEvent;
#if DEVELOPMENT
			IronSource.Agent.validateIntegration();

#endif
			Debug.Log("ironSource client has been initialized.");
#endif
		}

		protected override void InternalShowBannerAd(BannerAdPosition position, BannerAdSize size)
		{
#if ACTIVE_IRONSOURCE
			// If player requests a banner with different position or size,
			// we have to load a new banner.
			var newPos = ToIronSourceBannerPosition(position);
			var newSize = ToIronSourceBannerSize(size);

			if (mCurrentBannerAdPos != newPos)
			{
				mCurrentBannerAdPos = newPos;
				mIsBannerAdLoaded = false;
			}

			if (mCurrentBannerAdSize != newSize)
			{
				mCurrentBannerAdSize = newSize;
				mIsBannerAdLoaded = false;
			}

			if (!mIsBannerAdLoaded)
			{
				if (string.IsNullOrEmpty(mAdSettings.defaultBannerPlacement.Id))
					IronSource.Agent.loadBanner(mCurrentBannerAdSize, mCurrentBannerAdPos);
				else
					IronSource.Agent.loadBanner(mCurrentBannerAdSize, mCurrentBannerAdPos, mAdSettings.defaultBannerPlacement.Id);
			}

			IronSource.Agent.displayBanner();
#endif
		}

		protected override void InternalHideBannerAd()
		{
#if ACTIVE_IRONSOURCE
			IronSource.Agent.hideBanner();
#endif
		}

		protected override void InternalDestroyBannerAd()
		{
#if ACTIVE_IRONSOURCE
			IronSource.Agent.destroyBanner();
			mIsBannerAdLoaded = false;
#endif
		}

		protected override bool InternalIsInterstitialAdReady()
		{
#if ACTIVE_IRONSOURCE
			return IronSource.Agent.isInterstitialReady();
#else
            return false;
#endif
		}

		protected override void InternalLoadInterstitialAd()
		{
#if ACTIVE_IRONSOURCE
			IronSource.Agent.loadInterstitial();
#endif
		}

		protected override void InternalShowInterstitialAd(string id)
		{
#if ACTIVE_IRONSOURCE
			if (string.IsNullOrEmpty(mAdSettings.defaultInterstitialPlacement.Id))
				IronSource.Agent.showInterstitial(id);
			else
				IronSource.Agent.showInterstitial(mAdSettings.defaultInterstitialPlacement.Id);
#endif
		}

		protected override bool InternalIsRewardedAdReady()
		{
#if ACTIVE_IRONSOURCE
			return IronSource.Agent.isRewardedVideoAvailable();
#else
            return false;
#endif
		}

		protected override void InternalLoadRewardedAd()
		{
			// IronSource loads rewarded video ads in the background automatically,
			// so we don't need to do anything here.
		}

		protected override void InternalShowRewardedAd(string id)
		{
#if ACTIVE_IRONSOURCE
			if (string.IsNullOrEmpty(mAdSettings.defaultRewardedPlacement.Id))
				IronSource.Agent.showRewardedVideo(id);
			else
				IronSource.Agent.showRewardedVideo(mAdSettings.defaultRewardedPlacement.Id);
#endif
		}

		#endregion

		#region Helper Methods

#if ACTIVE_IRONSOURCE
		protected IronSourceBannerSize ToIronSourceBannerSize(IronSourceSettings.IronSourceBannerType bannerType)
		{
			switch (bannerType)
			{
				case IronSourceSettings.IronSourceBannerType.Banner:
					return IronSourceBannerSize.BANNER;

				case IronSourceSettings.IronSourceBannerType.LargeBanner:
					return IronSourceBannerSize.LARGE;

				case IronSourceSettings.IronSourceBannerType.RectangleBanner:
					return IronSourceBannerSize.RECTANGLE;

				case IronSourceSettings.IronSourceBannerType.SmartBanner:
					return IronSourceBannerSize.SMART;

				default:
					return IronSourceBannerSize.BANNER;
			}
		}

		protected IronSourceBannerPosition ToIronSourceBannerPosition(BannerAdPosition pos)
		{
			switch (pos)
			{
				case BannerAdPosition.Top:
				case BannerAdPosition.TopLeft:
				case BannerAdPosition.TopRight:
					return IronSourceBannerPosition.TOP;

				case BannerAdPosition.Bottom:
				case BannerAdPosition.BottomLeft:
				case BannerAdPosition.BottomRight:
				default:
					return IronSourceBannerPosition.BOTTOM;
			}
		}

		protected IronSourceBannerSize ToIronSourceBannerSize(BannerAdSize adSize)
		{
			return adSize.IsSmartBanner ? IronSourceBannerSize.SMART : ToIronSourceNearestSize(adSize);
		}

		protected virtual IronSourceBannerSize ToIronSourceNearestSize(BannerAdSize adSize)
		{
			if (adSize.Height < 70) // (50+90)/2
				return IronSourceBannerSize.BANNER; // screen width x 50
			else if (adSize.Height < 170)   // (90 + 250)/2
				return IronSourceBannerSize.LARGE;   // screen width x 90
			else
				return IronSourceBannerSize.RECTANGLE;   // screen width x 250
		}
#endif

		#endregion

		#region Advanced Settings

		protected virtual void SetupAdvancedSetting(IronSourceSettings adSettings)
		{
#if ACTIVE_IRONSOURCE
			SetupSegment(adSettings.Segments);
#endif
		}

		protected virtual void SetupSegment(IronSourceSettings.SegmentSettings segmentSettings)
		{
#if ACTIVE_IRONSOURCE
			if (segmentSettings == null)
			{
				Debug.LogError("SengmentSettings is null!!!");
				return;
			}

			IronSourceSegment newSegment = segmentSettings.ToIronSourceSegment();
			if (newSegment == null)
			{
				Debug.LogError("Segment is null!!!");
				return;
			}

			IronSource.Agent.setSegment(newSegment);

#endif
		}

		#endregion

		#region Ad Event Handlers

#if ACTIVE_IRONSOURCE

		private void OnBannerAdClicked()
		{

		}

		private void OnBannerAdLeftApplication()
		{

		}

		private void OnBannerAdLoaded()
		{
			mIsBannerAdLoaded = true;
			Debug.Log("Banner ad is loaded.");
		}

		private void OnBannerAdLoadFailed(IronSourceError error)
		{
			mIsBannerAdLoaded = false;
			Debug.Log("Failed to load banner ad. Error: " + error);
		}

		private void OnInterstitialAdClicked()
		{
			//AdjustManager.LogAdClick();
		}

		private void OnInterstititalAdClosed()
		{
			OnInterstitialAdCompleted();
		}

		private void OnInterstitialAdLoadFailed(IronSourceError error)
		{
			Debug.Log("Failed to load interstitial ad. Error: " + error);
		}

		private void OnInterstitialAdOpened()
		{
			//AdjustManager.LogAdView();
		}

		private void OnInterstitialAdReady()
		{

		}

		private void OnInterstitialAdShowSucceeded()
		{
		}

		private void OnInterstitialAdShowFailed(IronSourceError error)
		{
			Debug.Log("Failed to show interstitial ad. Error: " + error);
		}

		private void OnRewardedVideoAdClicked(IronSourcePlacement obj)
		{
			//AdjustManager.LogAdClick();
		}

		private void OnRewardedVideoClosed()
		{
			if (!mRewardedVideoIsCompleted)
				OnRewardedAdSkipped();
			else
				OnRewardedAdCompleted();
			GameData.Instance.GameConfigGroup.UnPauseGame();
			mRewardedVideoIsCompleted = false;
		}

		private void OnRewardedVideoAdEnded()
		{

		}

		private void OnRewardedVideoAdOpened()
		{
			Debug.LogError("ironsource ADOPEN");
			mRewardedVideoIsCompleted = false;
			GameData.Instance.GameConfigGroup.PauseGame();
		}

		private void OnRewardedVideoAdRewarded(IronSourcePlacement placement)
		{
			if (placement == null)
				return;
			//   if (AppsFlyerObjectScript.Instance != null)
			{
				// AppsFlyerObjectScript.LogViewAd();
				// AppsFlyerObjectScript.LogViewRewardVideo();
				//AdjustManager.LogAdView();
			}
			mRewardedVideoIsCompleted = true;
		}

		private void OnRewardedVideoAdShowFailed(IronSourceError error)
		{
			Debug.Log("Failed to show rewarded video ad. Error: " + error);
			mRewardedVideoIsCompleted = false;
		}

		private void OnRewardedVideoAdStarted()
		{
			Debug.LogError("ironsource AD Start");
			mRewardedVideoIsCompleted = false;
		}

		private void OnRewardedVideoAvailabilityChanged(bool obj)
		{

		}

		private void OnSegmentReceived(string segment)
		{
			Debug.Log("Received segment: " + segment);
		}

		void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
		{
			if (impressionData != null)
			{


				Firebase.Analytics.Parameter[] AdParameters = {
		  new Firebase.Analytics.Parameter("ad_platform", impressionData.adNetwork),
		  new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),
		  new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),
		  new Firebase.Analytics.Parameter("ad_format", impressionData.instanceName),
		  new Firebase.Analytics.Parameter("currency","USD"),
		  new Firebase.Analytics.Parameter("value",impressionData.revenue.ToString())
		};
				Debug.LogError("IronSource Log Event ImpressionSuccessEvent " + impressionData.adNetwork);
				//Firebase.Analytics.FirebaseAnalytics.ParamA
				// Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
				Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression_manual", AdParameters);

				//   var appFlyer = ServiceLocator.Instance.Resolve<AppsFlyerObjectScript>();
				//     if (AppsFlyerObjectScript.Instance != null)
				//AppsFlyerObjectScript.LogAdImpression("ironSource", impressionData.adNetwork, impressionData.adUnit, impressionData.instanceName, "USD", impressionData.revenue);

				//  AppsFlyerSDK.AppsFlyer.sendEvent():
			}
			else
			{
				Debug.LogError("IronSource Log Event ImpressionSuccessEvent:ALL NULL ");
			}
		}

#endif
		#endregion



	}

	//==========================================================================================

	[Serializable]
	public class IronSourceSettings : AdSettings
	{
		public int age;
		public string gender = null;
		public AdId appId;
		public AdId defaultBannerPlacement;
		public AdId defaultInterstitialPlacement;
		public AdId defaultRewardedPlacement;
		public bool UseAdvancedSetting;
		public SegmentSettings Segments;
		[Range(0, 10)]
		public int weight;

		[Serializable]
		public class SegmentSettings
		{
			public int age;
			public string gender = null;
			public int level;
			public bool isPaying;
			public long userCreationDate;
			public double iapt;
			public string segmentName = null;
			public Dictionary<string, string> customParams;

#if ACTIVE_IRONSOURCE
			public IronSourceSegment ToIronSourceSegment()
			{
				IronSourceSegment segment = new IronSourceSegment
				{
					age = this.age,
					gender = this.gender,
					level = this.level,
					isPaying = isPaying ? 1 : 0,
					userCreationDate = this.userCreationDate,
					iapt = this.iapt,
					segmentName = this.segmentName,
				};

				if (customParams != null)
				{
					foreach (var param in customParams)
					{
						segment.setCustom(param.Key, param.Value);
					}
				}

				return segment;
			}
#endif
		}

		public enum IronSourceBannerType
		{
			/// <summary>
			/// 50 X screen width.
			/// Supports: Admob, AppLovin, Facebook, InMobi.
			/// </summary>
			Banner,

			/// <summary>
			/// 90 X screen width.
			/// Supports: Admob, Facebook.
			/// </summary>
			LargeBanner,

			/// <summary>
			/// 250 X screen width.
			/// Supports: Admob, AppLovin, Facebook, InMobi.
			/// </summary>
			RectangleBanner,

			/// <summary>
			/// 50 (screen height ≤ 720) X screen width, 90 (screen height > 720) X screen width.
			/// Supports: Admob, AppLovin, Facebook, InMobi.
			/// </summary>
			SmartBanner,
		}

		public int CurInterstitialAdWeight
		{
			get { return PlayerPrefs.GetInt("IS_InterstitialAdWeight", weight); }
			set { PlayerPrefs.SetInt("IS_InterstitialAdWeight", value); }
		}
		public int CurRewardedAdWeight
		{
			get { return PlayerPrefs.GetInt("IS_RewardedAdWeight", weight); }
			set { PlayerPrefs.SetInt("IS_RewardedAdWeight", value); }
		}
		public void ResetCurWeight()
		{
			CurInterstitialAdWeight = weight;
			CurRewardedAdWeight = weight;
		}
	}
}