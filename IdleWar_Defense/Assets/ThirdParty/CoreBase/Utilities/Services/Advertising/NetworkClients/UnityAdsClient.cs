using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Utilities.Service.Ads
{
#if UNITY_MONETIZATION
    using UnityEngine.Advertisements;
#endif

    public class UnityAdsClient : AdClient
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please enable UnityAds service.";
        private const string BANNER_UNSUPPORTED_MESSAGE = "UnityAds does not support banner ad format.";

        #region Members

#if UNITY_MONETIZATION
        private UnityAdsSettings mAdSettings;
        public event Action<ShowResult> InterstitialAdCallback;
        public event Action<ShowResult> RewardedAdCallback;
        public event Action BannerAdShownCallback;
        public event Action BannerAdHiddenCallback;
#endif

        #endregion

        //===========================================================

        #region Singleton

        private static UnityAdsClient mInstance;

        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static UnityAdsClient CreateClient()
        {
            if (mInstance == null)
                mInstance = new UnityAdsClient();
            return mInstance;
        }

        #endregion

        //===========================================================

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.UnityAds; } }

        public override bool IsBannerAdSupported
        {
            get
            {
#if UNITY_MONETIZATION
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsInitialized
        {
            get
            {
#if UNITY_MONETIZATION
                return mIsInitialized && Advertisement.isInitialized;
#else
                return mIsInitialized;
#endif
            }
        }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        public override bool IsSdkAvail
        {
            get
            {
#if UNITY_MONETIZATION
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValid(AdType type)
        {
#if UNITY_MONETIZATION
            if (mAdSettings == null)
                return false;

            string id;
            switch (type)
            {
                case AdType.Rewarded:
                    id = mAdSettings.defaultRewardedAdId.Id;
                    break;
                case AdType.Interstitial:
                    id = mAdSettings.defaultInterstitialAdId.Id;
                    break;
                default:
                    return false;
            }

            if (string.IsNullOrEmpty(id))
                return false;
            else
                return true;
#else
            return false;
#endif
        }

        public override void Init(AdSettings mUnityAdSettings)
        {
#if UNITY_MONETIZATION
            if (mIsInitialized)
                return;

            mIsInitialized = true;
            mAdSettings = mUnityAdSettings as UnityAdsSettings;

            string appId = mAdSettings.appId.Id;
            var platform = Application.platform;

            Advertisement.AddListener(new UnityAdsListener(this));

            if (string.IsNullOrEmpty(appId))
                Debug.LogWarning("Attempting to initialize UnityAds with an empty App ID.");

            Advertisement.Initialize(appId, mAdSettings.enableTestMode);

            Debug.Log("UnityAds client has been initialized.");

            //The rest of the initialization is done automatically by Unity if using buildin UnityAds.
#endif
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        protected override void InternalShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
#if UNITY_MONETIZATION
            string id = mAdSettings.defaultBannerAdId.Id;

            if (string.IsNullOrEmpty(id))
            {
                Debug.Log("Attempting to show UnityAds banner ad with an undefined ID");
                return;
            }

            if (!Advertisement.Banner.isLoaded)
            {
                BannerLoadOptions options = new BannerLoadOptions();

                options.errorCallback += ((string message) =>
                {
                    Debug.Log("Error loading Unity banner ad: " + message);
                });

                options.loadCallback += (() =>
                {
                    DoShowBannerAd(id);
                });

                Advertisement.Banner.SetPosition(ToUnityAdsBannerPosition(position));
                Advertisement.Banner.Load(id, options);
            }
            else
            {
                DoShowBannerAd(id);
            }

#else
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
#endif
        }

        private void InternalShowBannerAdCallback()
        {
#if UNITY_MONETIZATION
            if (BannerAdShownCallback != null)
                BannerAdShownCallback();
#endif
        }

        protected override void InternalHideBannerAd()
        {
#if UNITY_MONETIZATION
            var hideOptions = new BannerOptions
            {
                hideCallback = () =>
                {
                    InternalHideBannerAdCallback();
                }
            };
            Advertisement.Banner.Hide(false);
#else
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
#endif
        }

        private void InternalHideBannerAdCallback()
        {
#if UNITY_MONETIZATION
            if (BannerAdHiddenCallback != null)
                BannerAdHiddenCallback();
#endif
        }

        protected override void InternalDestroyBannerAd()
        {
#if UNITY_MONETIZATION
            var hideOptions = new BannerOptions
            {
                hideCallback = () =>
                {
                    InternalHideBannerAdCallback();
                }
            };
            Advertisement.Banner.Hide(true);
#else
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
#endif
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        protected override void InternalLoadInterstitialAd()
        {
            // Unity Ads handles loading automatically.
        }

        protected override bool InternalIsInterstitialAdReady()
        {
#if UNITY_MONETIZATION
            string Id = mAdSettings.defaultInterstitialAdId.Id;

            if (Id == string.Empty)
                return false;

            return Advertisement.IsReady(Id);
#else
            return false;
#endif
        }

        protected override void InternalShowInterstitialAd(string id)
        {
#if UNITY_MONETIZATION
            string id = mAdSettings.defaultInterstitialAdId.Id;

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogFormat("Attempting to show {0} interstitial ad with an undefined ID",
                    Network.ToString());
                return;
            }

#if !UNITY_MONETIZATION
            var showOptions = new ShowOptions
            {
                resultCallback = (result) =>
                {
                    InternalInterstitialAdCallback(result);
                }
            };
            Advertisement.Show(id, showOptions);
#else
            Advertisement.Show(id);
#endif
#endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        protected override void InternalLoadRewardedAd()
        {
            // Unity Ads handles loading automatically.
        }

        protected override bool InternalIsRewardedAdReady()
        {
#if UNITY_MONETIZATION
            string Id = mAdSettings.defaultRewardedAdId.Id;

            if (Id == string.Empty)
                return false;

            return Advertisement.IsReady(Id);
#else
            return false;
#endif
        }

        protected override void InternalShowRewardedAd(string id)
        {
#if UNITY_MONETIZATION
            string id = mAdSettings.defaultRewardedAdId.Id;

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogFormat("Attempting to show {0} rewarded ad with an undefined ID",
                    Network.ToString());
                return;
            }
#if !UNITY_MONETIZATION
            var showOptions = new ShowOptions
            {
                resultCallback = (result) =>
                {
                    InternalRewardedAdCallback(result);
                }
            };
            Advertisement.Show(id, showOptions);
#else
            Advertisement.Show(id);
#endif
#endif
        }

        #endregion

        //===========================================================

        #region Ad Event Handlers

#if UNITY_MONETIZATION
        private void InternalInterstitialAdCallback(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    OnInterstitialAdCompleted();
                    break;
                case ShowResult.Skipped:
                    OnInterstitialAdCompleted();
                    break;
                case ShowResult.Failed:
                    break;
            }

            if (InterstitialAdCallback != null)
                InterstitialAdCallback(result);
        }

        private void InternalRewardedAdCallback(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    OnRewardedAdCompleted();
                    break;
                case ShowResult.Skipped:
                    OnRewardedAdSkipped();
                    break;
                case ShowResult.Failed:
                    break;
            }

            if (RewardedAdCallback != null)
                RewardedAdCallback(result);
        }
#endif

#if UNITY_MONETIZATION
        private class UnityAdsListener : IUnityAdsListener
        {
            private UnityAdsClient mClient;

            public UnityAdsListener(UnityAdsClient client)
            {
                mClient = client;
            }

            public void OnUnityAdsDidError(string message)
            {
                Debug.Log("OnUnityAdsDidError: " + message);
            }

            public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
            {
                Debug.Log("OnUnityAdsDidFinish: " + placementId);

                if (placementId == mClient.mAdSettings.defaultInterstitialAdId.Id)
                {
                    mClient.InternalInterstitialAdCallback(showResult);
                }
                else if (placementId == mClient.mAdSettings.defaultRewardedAdId.Id)
                {
                    mClient.InternalRewardedAdCallback(showResult);
                }
            }

            public void OnUnityAdsDidStart(string placementId)
            {
                Debug.Log("OnUnityAdsDidStart: " + placementId);
            }

            public void OnUnityAdsReady(string placementId)
            {
                Debug.Log("OnUnityAdsReady: " + placementId);
            }
        }

        private void DoShowBannerAd(string id)
        {
            var showOptions = new BannerOptions
            {
                showCallback = () =>
                {
                    InternalShowBannerAdCallback();
                }
            };
            Advertisement.Banner.Show(id, showOptions);
        }

        private BannerPosition ToUnityAdsBannerPosition(BannerAdPosition position)
        {
            switch (position)
            {
                case BannerAdPosition.Bottom:
                    return BannerPosition.BOTTOM_CENTER;
                case BannerAdPosition.BottomLeft:
                    return BannerPosition.BOTTOM_LEFT;
                case BannerAdPosition.BottomRight:
                    return BannerPosition.BOTTOM_RIGHT;
                case BannerAdPosition.Top:
                    return BannerPosition.TOP_CENTER;
                case BannerAdPosition.TopLeft:
                    return BannerPosition.TOP_LEFT;
                case BannerAdPosition.TopRight:
                    return BannerPosition.TOP_RIGHT;
                default:
                    return BannerPosition.CENTER;
            }
        }
#endif

        #endregion
    }

    //================================================================

    [Serializable]
    public class UnityAdsSettings : AdSettings
    {
        public const string DEFAULT_VIDEO_ZONE_ID = "video";
        public const string DEFAULT_REWARDED_ZONE_ID = "rewardedVideo";

        /// <summary>
        /// Android app Id for manually initialization
        /// If using buildin UnityAds, you can let this empty
        /// </summary>
        public AdId appId;
        public AdId defaultInterstitialAdId = new AdId(DEFAULT_VIDEO_ZONE_ID, DEFAULT_VIDEO_ZONE_ID);
        public AdId defaultRewardedAdId = new AdId(DEFAULT_REWARDED_ZONE_ID, DEFAULT_REWARDED_ZONE_ID);
        public AdId defaultBannerAdId;
        public bool enableTestMode;
        [Range(0, 10)]
        public int weight;

        public int CurInterstitialAdWeight
        {
            get { return PlayerPrefs.GetInt("Unity_InterstitialAdWeight", weight); }
            set { PlayerPrefs.SetInt("Unity_InterstitialAdWeight", value); }
        }
        public int CurRewardedAdWeight
        {
            get { return PlayerPrefs.GetInt("Unity_RewardedAdWeight", weight); }
            set { PlayerPrefs.SetInt("Unity_RewardedAdWeight", value); }
        }
        public void ResetCurWeight()
        {
            CurInterstitialAdWeight = weight;
            CurRewardedAdWeight = weight;
        }
    }
}