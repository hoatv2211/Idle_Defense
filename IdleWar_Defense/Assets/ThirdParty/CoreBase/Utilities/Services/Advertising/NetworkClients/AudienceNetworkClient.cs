using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Service.Ads
{
#if ACTIVE_FBAN
    using AudienceNetwork;
#endif

    public class AudienceNetworkClient : AdClient
    {
        #region Inner classes

#if ACTIVE_FBAN

        /// <summary>
        /// Hold data of a banner ad.
        /// </summary>
        protected class BannerAd
        {
            private bool isLoaded;

            public AdView Ad { get; set; }

            public AdSize CurrentSize { get; private set; }

            public bool IsLoaded
            {
                get { return Ad != null && isLoaded; }
                set { isLoaded = value; }
            }

            public BannerAd(AdView adView, AdSize adSize)
            {
                Ad = adView;
                CurrentSize = adSize;

                Ad.AdViewDidLoad += () => isLoaded = true;
            }
        }

        /// <summary>
        /// Hold data of an interstitial ad.
        /// </summary>
        public class Interstitial
        {
            private bool isReady;

            public InterstitialAd Ad { get; set; }

            public bool IsReady
            {
                get { return Ad != null && isReady; }
                set { isReady = value; }
            }

            public bool IsLoading { get; set; }

            public Interstitial(InterstitialAd interstitialAd)
            {
                Ad = interstitialAd;

                /// It seems like the IsValid() method in Intersitital ads can't be used to check if an ad is loaded or not (???).
                /// So we have to detect the DidLoad events (invoked when those ads are loaded) and raise a bool manually like this.
                Ad.InterstitialAdDidLoad += () =>
                {
                    IsLoading = false;
                    isReady = true;
                };
            }
        }

        /// <summary>
        /// Hold data of a rewarded video ad.
        /// </summary>
        public class RewardedVideo
        {
            private bool isReady;

            public RewardedVideoAd Ad { get; set; }

            public bool IsReady
            {
                get { return Ad != null && isReady; }
                set { isReady = value; }
            }

            public RewardedVideo(RewardedVideoAd rewardedVideo)
            {
                Ad = rewardedVideo;

                /// It seems like the IsValid() method in Rewarded ads can't be used to check if an ad is loaded or not (???).
                /// So we have to detect the DidLoad events (invoked when those ads are loaded) and raise a bool manually like this.
                Ad.RewardedVideoAdDidLoad += () => isReady = true;
            }
        }

#endif

        #endregion  // Inner Classes

        #region FB Audience Events

#if ACTIVE_FBAN

        public event FBAdViewBridgeCallback AdViewDidLoad;
        public event FBAdViewBridgeCallback AdViewWillLogImpression;
        public event FBAdViewBridgeErrorCallback AdViewDidFailWithError;
        public event FBAdViewBridgeCallback AdViewDidClick;
        public event FBAdViewBridgeCallback AdViewDidFinishClick;

        public event FBInterstitialAdBridgeCallback InterstitialAdDidLoad;
        public event FBInterstitialAdBridgeCallback InterstitialAdWillLogImpression;
        public event FBInterstitialAdBridgeErrorCallback InterstitialAdDidFailWithError;
        public event FBInterstitialAdBridgeCallback InterstitialAdDidClick;
        public event FBInterstitialAdBridgeCallback InterstitialAdWillClose;
        public event FBInterstitialAdBridgeCallback InterstitialAdDidClose;
#if UNITY_ANDROID
        /// <summary>
        /// Only relevant to Android.
        /// This event will only occur if the Interstitial activity has
        /// been destroyed without being properly closed. This can happen if an
        /// app with launchMode:singleTask (such as a Unity game) goes to
        /// background and is then relaunched by tapping the icon.
        /// </summary>
        public event FBInterstitialAdBridgeCallback InterstitialAdActivityDestroyed;
#endif

        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdDidLoad;
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdWillLogImpression;
        public event FBRewardedVideoAdBridgeErrorCallback RewardedVideoAdDidFailWithError;
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdDidClick;
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdWillClose;
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdDidClose;
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdComplete;
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdDidSucceed;
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdDidFail;
#if UNITY_ANDROID
        /// <summary>
        /// Only relevant to Android.
        /// This event will only occur if the Rewarded Video activity
        /// has been destroyed without being properly closed.This can happen if
        /// an app with launchMode:singleTask(such as a Unity game) goes to
        /// background and is then relaunched by tapping the icon.
        /// </summary>
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdActivityDestroyed;
#endif
#endif

        #endregion  // FBAN-Specific events

        private const string NO_SDK_MESSAGE = "SDK missing. Please import the FB Audience Network plugin.";

        /// <summary>
        /// Name of the Unity's GameObject that will be used to register this network (required by FaceBook Audience Network).
        /// </summary>
        private const string AD_HANDLER_GO_NAME = "FBAN_Handler";

#if ACTIVE_FBAN

        /// <summary>
        /// The banner ad will be moved to this position when hiding.
        /// </summary>
        /// Since Facebook Audience doesn't has a method to hide banner ad,
        /// we will move it far away from the camera instead.
        private readonly Vector2 BANNER_HIDE_POSITION = new Vector2(9999, 9999);

        private AudienceNetworkSettings mAdSettings;
        private GameObject mAdHandlerObject;

        private BannerAd mDefaultBanner;
        private Interstitial mDefaultInterstitial;
        private RewardedVideo mDefaultRewardedVideo;
#endif

        #region Singleton

        private static AudienceNetworkClient sInstance;

        public static AudienceNetworkClient CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new AudienceNetworkClient();
            }
            return sInstance;
        }

        #endregion

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.AudienceNetwork; } }

        public override bool IsBannerAdSupported { get { return true; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        {
            get
            {
#if ACTIVE_FBAN
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValid(AdType type)
        {
#if ACTIVE_FBAN
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
                    id = mAdSettings.defaultBannerAdId.Id;
                    break;
            }

            if (string.IsNullOrEmpty(id))
                return false;
            else
                return true;
#else
            return false;
#endif
        }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        public override void Init(AdSettings pAudienceNetworkSettings)
        {
#if ACTIVE_FBAN
            mIsInitialized = true;

            mAdSettings = pAudienceNetworkSettings as AudienceNetworkSettings;

            if (mAdSettings.enableTestMode)
                SetupTestMode(mAdSettings);

            // The FB Audience Network ads need to be registered to a gameObject,
            // so we just create one here and register them to it.
            mAdHandlerObject = new GameObject(AD_HANDLER_GO_NAME);

            // This game object should persist across scenes.
            UnityEngine.Object.DontDestroyOnLoad(mAdHandlerObject);

            mDefaultBanner = CreateNewBannerAd(mAdSettings.defaultBannerAdId.Id, ToAdSize(mAdSettings.bannerAdSize));
            mDefaultInterstitial = CreateNewInterstitialAd(mAdSettings.defaultInterstitialAdId.Id);
            mDefaultRewardedVideo = CreateNewRewardedVideoAd(mAdSettings.defaultRewardedAdId.Id);

            Debug.Log("Audience Network client has been initialized.");
#endif
        }

        protected override void InternalShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
#if ACTIVE_FBAN
            string id = mAdSettings.defaultBannerAdId.Id;

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogFormat("Attempting to show {0} banner ad with an undefined ID", Network.ToString());
                return;
            }

            mDefaultBanner = ShowBannerAd(mDefaultBanner, id, position, size);
#endif
        }

        protected override void InternalHideBannerAd()
        {
#if ACTIVE_FBAN
            HideBannerAd(mDefaultBanner);
#endif
        }

        protected override void InternalDestroyBannerAd()
        {
#if ACTIVE_FBAN
            DestroyBannerAd(mDefaultBanner);
#endif
        }

        protected override bool InternalIsInterstitialAdReady()
        {
#if ACTIVE_FBAN
            return mDefaultInterstitial != null && mDefaultInterstitial.IsReady;
#else
            return false;
#endif
        }

        protected override void InternalLoadInterstitialAd()
        {
#if ACTIVE_FBAN
            if (mDefaultInterstitial == null)
                mDefaultInterstitial = CreateNewInterstitialAd(mAdSettings.defaultInterstitialAdId.Id);

            if (!mDefaultInterstitial.IsLoading && !mDefaultInterstitial.IsReady)
            {
                mDefaultInterstitial.IsLoading = true;
                mDefaultInterstitial.Ad.LoadAd();
            }
#endif
        }

        protected override void InternalShowInterstitialAd(string id)
        {
#if ACTIVE_FBAN
            if (mDefaultInterstitial == null)
                return;

            mDefaultInterstitial.Ad.Show();
            mDefaultInterstitial.IsReady = false;
#endif
        }

        protected override bool InternalIsRewardedAdReady()
        {
#if ACTIVE_FBAN
            return mDefaultRewardedVideo != null && mDefaultRewardedVideo.IsReady;
#else
            return false;
#endif
        }

        protected override void InternalLoadRewardedAd()
        {
#if ACTIVE_FBAN
            if (mDefaultRewardedVideo == null)
                mDefaultRewardedVideo = CreateNewRewardedVideoAd(mAdSettings.defaultRewardedAdId.Id);

            mDefaultRewardedVideo.Ad.LoadAd();
#endif
        }

        protected override void InternalShowRewardedAd(string id)
        {
#if ACTIVE_FBAN
            if (mDefaultRewardedVideo == null)
                return;

            mDefaultRewardedVideo.Ad.Show();
            mDefaultRewardedVideo.IsReady = false;
#endif
        }

        #endregion

        #region  Ad Event Handlers

#if ACTIVE_FBAN

        private void OnBannerAdFailedWithError(string error)
        {
            Debug.Log("AudienceNetwork::OnBannerAdFailedWithError. Error: " + error);

            if (AdViewDidFailWithError != null)
                AdViewDidFailWithError(error);
        }

        private void OnBannerAdLoaded()
        {
            Debug.Log("AudienceNetwork::OnBannerAdLoaded");

            if (AdViewDidLoad != null)
                AdViewDidLoad();
        }

        private void OnBannerAdClicked()
        {
            Debug.Log("AudienceNetwork::OnBannerAdClicked");

            if (AdViewDidClick != null)
                AdViewDidClick();
        }

        private void OnBannerAdFinishClicked()
        {
            Debug.Log("AudienceNetwork::OnBannerAdFinishClicked");

            if (AdViewDidFinishClick != null)
                AdViewDidFinishClick();
        }

        private void OnBannerAdWillLogImpression()
        {
            Debug.Log("AudienceNetwork::OnBannerAdWillLogImpression");

            if (AdViewWillLogImpression != null)
                AdViewWillLogImpression();
        }

        private void OnInterstitialAdClosed()
        {
            Debug.Log("AudienceNetwork::OnInterstitialAdClosed");
            // Dispose ad.
            if (mDefaultInterstitial != null && mDefaultInterstitial.Ad != null)
            {
                mDefaultInterstitial.Ad.Dispose();
                mDefaultInterstitial = null;
            }

            OnInterstitialAdCompleted();

            if (InterstitialAdDidClose != null)
                InterstitialAdDidClose();
        }

        private void OnInterstitialAdClicked()
        {
            Debug.Log("AudienceNetwork::OnInterstitialAdClicked");

            if (InterstitialAdDidClick != null)
                InterstitialAdDidClick();
        }

        private void OnInterstitialAdFailedWithError(string error)
        {
            Debug.Log("AudienceNetwork::OnInterstitialAdFailedWithError. Error: " + error);

            if (InterstitialAdDidFailWithError != null)
                InterstitialAdDidFailWithError(error);
        }

        private void OnInterstitialAdLoaded()
        {
            Debug.Log("AudienceNetwork::OnInterstitialAdLoaded");

            if (InterstitialAdDidLoad != null)
                InterstitialAdDidLoad();
        }

        private void OnInterstitialWillClose()
        {
            Debug.Log("AudienceNetwork::OnInterstitialWillClose");

            if (InterstitialAdWillClose != null)
                InterstitialAdWillClose();
        }

        private void OnInterstitialWillLogImpression()
        {
            Debug.Log("AudienceNetwork::OnInterstitialWillLogImpression");

            if (InterstitialAdWillLogImpression != null)
                InterstitialAdWillLogImpression();
        }

        private void OnRewardVideoAdComplete()
        {
            Debug.Log("AudienceNetwork::OnRewardVideoAdComplete");

            OnRewardedAdCompleted();

            if (RewardedVideoAdComplete != null)
                RewardedVideoAdComplete();
        }

        private void OnRewardedVideoAdClicked()
        {
            Debug.Log("AudienceNetwork::OnRewardedVideoAdClicked");

            if (RewardedVideoAdDidClick != null)
                RewardedVideoAdDidClick();
        }

        private void OnRewaredVideoAdClosed()
        {
            Debug.Log("AudienceNetwork::OnRewaredVideoAdClosed");

            if (mDefaultRewardedVideo != null && mDefaultRewardedVideo.Ad != null)
            {
                mDefaultRewardedVideo.Ad.Dispose();
                mDefaultRewardedVideo = null;
            }

            if (RewardedVideoAdDidClose != null)
                RewardedVideoAdDidClose();
        }

        private void OnRewardVideoAdFailed()
        {
            Debug.Log("AudienceNetwork::OnRewardVideoAdFailed. Rewarded video ad not validated, or no response from server.");

            if (RewardedVideoAdDidFail != null)
                RewardedVideoAdDidFail();
        }

        private void OnRewardedVideoAdFailedWithError(string error)
        {
            Debug.Log("AudienceNetwork::OnRewardedVideoAdFailedWithError. RewardedVideo ad failed to load with error: " + error);

            if (RewardedVideoAdDidFailWithError != null)
                RewardedVideoAdDidFailWithError(error);
        }

        private void OnRewardVideoAdLoaded()
        {
            Debug.Log("AudienceNetwork::OnRewardVideoAdLoaded");

            if (RewardedVideoAdDidLoad != null)
                RewardedVideoAdDidLoad();
        }

        private void OnRewardedVideoAdSucceeded()
        {
            Debug.Log("AudienceNetwork::OnRewardedVideoAdSucceeded. Rewarded video ad validated by server.");

            if (RewardedVideoAdDidSucceed != null)
                RewardedVideoAdDidSucceed();
        }

        private void OnRewardedVideoAdWillClose()
        {
            Debug.Log("AudienceNetwork::OnRewardedVideoAdWillClose");

            if (RewardedVideoAdWillClose != null)
                RewardedVideoAdWillClose();
        }

        private void OnRewardedVideoAdWillLogImpression()
        {
            Debug.Log("AudienceNetwork::OnRewardedVideoAdWillLogImpression");

            if (RewardedVideoAdWillLogImpression != null)
                RewardedVideoAdWillLogImpression();
        }

#endif

        #endregion

        #region Create & Setup Events methods

#if ACTIVE_FBAN

        /// <summary>
        /// Create new banner ad.
        /// </summary>
        protected virtual BannerAd CreateNewBannerAd(string bannerId, AdSize adSize)
        {
            if (string.IsNullOrEmpty(bannerId))
            {
                return null;
            }

            AdView newBanner = new AdView(bannerId, adSize);
            newBanner.Register(mAdHandlerObject);
            SetupBannerAdEvents(newBanner);

            return new BannerAd(newBanner, adSize);
        }

        protected virtual void SetupBannerAdEvents(AdView bannerAd)
        {
            if (bannerAd != null)
            {
                bannerAd.AdViewDidFailWithError += OnBannerAdFailedWithError;
                bannerAd.AdViewDidLoad += OnBannerAdLoaded;
                bannerAd.AdViewDidClick += OnBannerAdClicked;
                bannerAd.AdViewDidFinishClick += OnBannerAdFinishClicked;
                bannerAd.AdViewWillLogImpression += OnBannerAdWillLogImpression;
            }
        }

        /// <summary>
        /// Create new interstitial with specific id.
        /// </summary>
        protected virtual Interstitial CreateNewInterstitialAd(string interstitialId)
        {
            if (string.IsNullOrEmpty(interstitialId))
            {
                return null;
            }

            InterstitialAd newInterstitial = new InterstitialAd(interstitialId);
            SetupInterstitialAdEvents(newInterstitial);

            newInterstitial.Register(mAdHandlerObject);

            return new Interstitial(newInterstitial);
        }

        protected virtual void SetupInterstitialAdEvents(InterstitialAd interstitialAd)
        {
            if (interstitialAd != null)
            {
                interstitialAd.InterstitialAdDidClose += OnInterstitialAdClosed;
                interstitialAd.InterstitialAdDidClick += OnInterstitialAdClicked;
                interstitialAd.InterstitialAdDidFailWithError += OnInterstitialAdFailedWithError;
                interstitialAd.InterstitialAdDidLoad += OnInterstitialAdLoaded;
                interstitialAd.InterstitialAdWillClose += OnInterstitialWillClose;
                interstitialAd.InterstitialAdWillLogImpression += OnInterstitialWillLogImpression;
#if UNITY_ANDROID
                interstitialAd.InterstitialAdActivityDestroyed += OnInterstitialAdActivityDestroyed;
#endif
            }
        }

        private void OnInterstitialAdActivityDestroyed()
        {
#if UNITY_ANDROID
            mDefaultInterstitial = null;

            if (InterstitialAdActivityDestroyed != null)
                InterstitialAdActivityDestroyed();
#endif
        }

        /// <summary>
        /// Create new rewarded video ad with specific id.
        /// </summary>
        protected virtual RewardedVideo CreateNewRewardedVideoAd(string rewardedId)
        {
            if (string.IsNullOrEmpty(rewardedId))
            {
                return null;
            }

            RewardedVideoAd newRewardedVideoAd = new RewardedVideoAd(rewardedId);
            SetupRewardedVideoEvents(newRewardedVideoAd);

            newRewardedVideoAd.Register(mAdHandlerObject);

            return new RewardedVideo(newRewardedVideoAd);
        }

        protected void SetupRewardedVideoEvents(RewardedVideoAd rewardedVideoAd)
        {
            if (rewardedVideoAd != null)
            {
                rewardedVideoAd.RewardedVideoAdComplete += OnRewardVideoAdComplete;
                rewardedVideoAd.RewardedVideoAdDidClick += OnRewardedVideoAdClicked;
                rewardedVideoAd.RewardedVideoAdDidClose += OnRewaredVideoAdClosed;
                rewardedVideoAd.RewardedVideoAdDidFailWithError += OnRewardedVideoAdFailedWithError;
                rewardedVideoAd.RewardedVideoAdDidLoad += OnRewardVideoAdLoaded;
                rewardedVideoAd.RewardedVideoAdDidFail += OnRewardVideoAdFailed;
                rewardedVideoAd.RewardedVideoAdDidSucceed += OnRewardedVideoAdSucceeded;
                rewardedVideoAd.RewardedVideoAdWillClose += OnRewardedVideoAdWillClose;
                rewardedVideoAd.RewardedVideoAdWillLogImpression += OnRewardedVideoAdWillLogImpression;
#if UNITY_ANDROID
                rewardedVideoAd.RewardedVideoAdActivityDestroyed += OnRewardedVideoAdActivityDestroyed;
#endif
            }
        }

        private void OnRewardedVideoAdActivityDestroyed()
        {
            mDefaultRewardedVideo = null;

            if (RewardedVideoAdActivityDestroyed != null)
                RewardedVideoAdActivityDestroyed();
        }

#endif

        #endregion

        #region Test Mode related methods

        protected virtual void SetupTestMode(AudienceNetworkSettings adSettings)
        {
            SetupTestDevices(adSettings.testDevices);
        }

        protected virtual void SetupTestDevices(string[] ids)
        {
#if ACTIVE_FBAN
            if (ids == null)
                return;

            foreach (string id in ids)
            {
                if (id == null)
                    continue;

                AudienceNetwork.AdSettings.AddTestDevice(id);
                Debug.Log("AudienceNetwork::SetupTestDevices. AddTestDevice " + id);
            }
#else
            Debug.Log(NO_SDK_MESSAGE);
#endif
        }

        #endregion

        #region Other stuff

#if ACTIVE_FBAN

        protected virtual BannerAd ShowBannerAd(BannerAd banner, string bannerID, BannerAdPosition position, BannerAdSize size)
        {
            /// If the default banner is null or user request a new banner with different size with the created one,
            /// create a new banner (since we can only set fb's banner ad size when creating it).
            if (banner == null || banner.Ad == null || banner.CurrentSize != ToFBAdSize(size))
            {
                /// Destroy old banner.
                DestroyBannerAd();

                /// Create new one.
                banner = CreateNewBannerAd(bannerID, ToFBAdSize(size));
            }

            /// Load the banner if it hasn't been loaded yet.
            if (!banner.IsLoaded)
                banner.Ad.LoadAd();

            banner.Ad.Show(ToFBAudienceAdPosition(position));

            return banner;
        }

        protected virtual void HideBannerAd(BannerAd banner)
        {
            if (banner == null || banner.Ad == null)
                return;

            // The FB Audience's banner ad doesn't have Hide method,
            // so we just move it far away from the camera instead.
            banner.Ad.Show(BANNER_HIDE_POSITION.x, BANNER_HIDE_POSITION.y);
        }

        protected virtual void DestroyBannerAd(BannerAd banner)
        {
            if (banner != null)
            {
                HideBannerAd(banner);
                banner.Ad = null;
                banner.IsLoaded = false;
            }
        }

        protected virtual AdSize ToFBAdSize(BannerAdSize adSize)
        {
            return adSize.IsSmartBanner ? AdSize.BANNER_HEIGHT_50 : ToFBNearestSize(adSize);
        }

        protected virtual AdSize ToFBNearestSize(BannerAdSize adSize)
        {
            if (adSize.Height < 75)
                return AdSize.BANNER_HEIGHT_50;

            if (adSize.Height < 150)
                return AdSize.BANNER_HEIGHT_90;

            return AdSize.RECTANGLE_HEIGHT_250;
        }

        protected virtual AdSize ToAdSize(AudienceNetworkSettings.FBAudienceBannerAdSize bannerAdSize)
        {
            switch (bannerAdSize)
            {
                case AudienceNetworkSettings.FBAudienceBannerAdSize._50:
                    return AdSize.BANNER_HEIGHT_50;

                case AudienceNetworkSettings.FBAudienceBannerAdSize._90:
                    return AdSize.BANNER_HEIGHT_90;

                case AudienceNetworkSettings.FBAudienceBannerAdSize._250:
                    return AdSize.RECTANGLE_HEIGHT_250;

                default:
                    return AdSize.BANNER_HEIGHT_50;
            }
        }

        protected virtual AdPosition ToFBAudienceAdPosition(BannerAdPosition pos)
        {
            switch (pos)
            {
                case BannerAdPosition.Bottom:
                case BannerAdPosition.BottomLeft:
                case BannerAdPosition.BottomRight:
                    return AdPosition.BOTTOM;

                case BannerAdPosition.Top:
                case BannerAdPosition.TopLeft:
                case BannerAdPosition.TopRight:
                default:
                    return AdPosition.TOP;
            }
        }

#endif

        #endregion

    }

    //============================================================================

    [Serializable]
    public class AudienceNetworkSettings : AdSettings
    {
        public FBAudienceBannerAdSize bannerAdSize;
        public bool enableTestMode;
        public string[] testDevices;
        public AdId defaultBannerAdId;
        public AdId defaultInterstitialAdId;
        public AdId defaultRewardedAdId;
        [Range(0, 10)]
        public int weight;

        public enum FBAudienceBannerAdSize
        {
            _50,
            _90,
            _250,
        }

        public int CurInterstitialAdWeight
        {
            get { return PlayerPrefs.GetInt("FAN_InterstitialAdWeight", weight); }
            set { PlayerPrefs.SetInt("FAN_InterstitialAdWeight", value); }
        }
        public int CurRewardedAdWeight
        {
            get { return PlayerPrefs.GetInt("FAN_RewardedAdWeight", weight); }
            set { PlayerPrefs.SetInt("FAN_RewardedAdWeight", value); }
        }
        public void ResetCurWeight()
        {
            CurInterstitialAdWeight = weight;
            CurRewardedAdWeight = weight;
        }
    }
}
