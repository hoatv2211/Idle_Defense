using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Utilities.Service.Ads
{
#if ACTIVE_ADMOB
    using GoogleMobileAds;
    using GoogleMobileAds.Api;
#endif

    public class AdMobClient : AdClient
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the AdMob (Google Mobile Ads) plugin.";

#if ACTIVE_ADMOB
        private AdMobSettings mAdSettings = null;
        private BannerView mDefaultBanner = null;
        private BannerAdSize mCurrentDefaultBannerSize = new BannerAdSize(-1, -1);
        private InterstitialAd mDefaultInterstitialAd = null;
        private RewardedAd mRewardedAd = null;

        /// <summary>
        /// Check if there is any rewarded video is currently running.
        /// </summary>
        /// Note that we can't have more than 1 rewarded video ad loaded at the same time,
        /// since it's gonna override old one's events.
        private bool mIsRewardedAdPlaying = false;

        /// <summary>
        /// Check if a rewarded ad is completed.
        /// We need this value to check if a rewarded ad was skipped or completed when it is being closed.
        /// </summary>
        private bool mIsRewardedAdCompleted = false;
#endif

        #region AdMob Events

#if ACTIVE_ADMOB

        /// <summary>
        /// Called when a banner ad request has successfully loaded.
        /// </summary>
        public event EventHandler<EventArgs> OnBannerAdLoaded;

        /// <summary>
        /// Called when a banner ad request failed to load.
        /// </summary>
        public event EventHandler<AdFailedToLoadEventArgs> OnBannerAdFailedToLoad;

        /// <summary>
        /// Called when a banner ad is clicked.
        /// </summary>
        public event EventHandler<EventArgs> OnBannerAdOpening;

        /// <summary>
        /// Called when the user returned from the app after a banner ad click.
        /// </summary>
        public event EventHandler<EventArgs> OnBannerAdClosed;

        /// <summary>
        /// Called when a banner ad click caused the user to leave the application.
        /// </summary>
        public event EventHandler<EventArgs> OnBannerAdLeavingApplication;

        /// <summary>
        /// Called when an interstitial ad request has successfully loaded.
        /// </summary>
        public event EventHandler<EventArgs> OnInterstitialAdLoaded;

        /// <summary>
        /// Called when an interstitial ad request failed to load.
        /// </summary>
        public event EventHandler<AdFailedToLoadEventArgs> OnInterstitialAdFailedToLoad;

        /// <summary>
        /// Called when an interstitial ad is shown.
        /// </summary>
        public event EventHandler<EventArgs> OnInterstititalAdOpening;

        /// <summary>
        /// Called when an interstitital ad is closed.
        /// </summary>
        public event EventHandler<EventArgs> OnInterstitialAdClosed;

        /// <summary>
        /// Called when an interstitial ad click caused the user to leave the application.
        /// </summary>
        public event EventHandler<EventArgs> OnInterstitialAdLeavingApplication;

        /// <summary>
        /// Called when a rewarded video ad request has successfully loaded.
        /// </summary>
        public event EventHandler<EventArgs> OnRewardedAdLoaded;

        /// <summary>
        /// Called when a rewarded video ad request failed to load.
        /// </summary>
        public event EventHandler<AdFailedToLoadEventArgs> OnRewardedAdFailedToLoad;

        /// <summary>
        /// Called when a rewarded video ad request failed to show.
        /// </summary>
        public event EventHandler<AdFailedToLoadEventArgs> OnRewardedAdFailedToShow;

        /// <summary>
        /// Called when a rewared video ad is shown.
        /// </summary>
        public event EventHandler<EventArgs> OnRewardedAdOpening;

        /// <summary>
        /// Called when a rewarded video ad starts to play.
        /// </summary>
        public event EventHandler<EventArgs> OnRewardedAdStarted;

        /// <summary>
        /// Called when the user should be rewarded for watching a video.
        /// </summary>
        public event EventHandler<Reward> OnRewardedAdRewarded;

        /// <summary>
        /// Called when a rewarded video ad is closed.
        /// </summary>
        public event EventHandler<EventArgs> OnRewardedAdClosed;

#endif

        #endregion  // AdMob Events

        #region Singleton

        private static AdMobClient sInstance;

        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static AdMobClient CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new AdMobClient();
            }
            return sInstance;
        }

        #endregion  // Singleton

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.AdMob; } }

        public override bool IsBannerAdSupported { get { return true; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        {
            get
            {
#if ACTIVE_ADMOB
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValid(AdType type)
        {
#if ACTIVE_ADMOB
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

        public override void Init(AdSettings pAdMobSettings)
        {
#if ACTIVE_ADMOB
            if (mIsInitialized)
                return;

            mIsInitialized = true;
            mAdSettings = pAdMobSettings as AdMobSettings;
            MobileAds.Initialize(null);
            MobileAds.SetiOSAppPauseOnBackground(true);
            MobileAds.SetApplicationMuted(true);

            Debug.Log("AdMob client has been initialized.");
#endif
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        protected override void InternalShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
#if ACTIVE_ADMOB
            string id = mAdSettings.defaultBannerAdId.Id;

            if (string.IsNullOrEmpty(id))
            {
                Debug.Log("Attempting to show AdMob banner ad with an undefined ID");
                return;
            }

            // If the requested banner (default or custom) doesn't exist or player request a banner with different size, create a new one and show it.
            // Otherwise just show the existing banner (which might be hidden before).

            if (mDefaultBanner == null || mCurrentDefaultBannerSize != size)
            {
                mDefaultBanner = CreateNewBanner(position, size, id);
                mCurrentDefaultBannerSize = size;
                Debug.Log("Creating new default banner...");
            }

            mDefaultBanner.SetPosition(ToAdMobAdPosition(position));
            mDefaultBanner.Show();
#endif
        }

        protected override void InternalHideBannerAd()
        {
#if ACTIVE_ADMOB
            if (mDefaultBanner != null)
                mDefaultBanner.Hide();
#endif
        }

        protected override void InternalDestroyBannerAd()
        {
#if ACTIVE_ADMOB
            if (mDefaultBanner != null)
            {
                mDefaultBanner.Destroy();
                mDefaultBanner = null;
            }
#endif
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        protected override void InternalLoadInterstitialAd()
        {
#if ACTIVE_ADMOB
            string id = mAdSettings.defaultInterstitialAdId.Id;

            if (string.IsNullOrEmpty(id))
            {
                Debug.Log("Attempting to load AdMob interstitial ad with an undefined ID");
                return;
            }

            // Note: On iOS, InterstitialAd objects are one time use objects. 
            // That means once an interstitial is shown, the InterstitialAd object can't be used to load another ad. 
            // To request another interstitial, you'll need to create a new InterstitialAd object.
            if (mDefaultInterstitialAd == null)
                mDefaultInterstitialAd = CreateNewInterstitialAd(id);

            mDefaultInterstitialAd.LoadAd(CreateAdMobAdRequest());
#endif
        }

        protected override bool InternalIsInterstitialAdReady()
        {
#if ACTIVE_ADMOB
            return mDefaultInterstitialAd != null && mDefaultInterstitialAd.IsLoaded();
#else
            return false;
#endif
        }

        protected override void InternalShowInterstitialAd(string id)
        {
#if ACTIVE_ADMOB
            if (mDefaultInterstitialAd != null)
                mDefaultInterstitialAd.Show();
#endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        /// <summary>
        /// Instructs the underlaying SDK to load a rewarded ad. Only invoked if the client is initialized.
        /// AdMob doesn't really support loading multiple rewarded ads at the same time, so we restrict that
        /// only one ad at any placement can be loaded at a time. The user must consume that ad, or it fails
        /// to load, before another rewarded ad can be loaded.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected override void InternalLoadRewardedAd()
        {
#if ACTIVE_ADMOB
            // Loading a new rewarded ad seems to disable all events of the currently playing ad,
            // so we shouldn't perform rewarded ad loading while playing another one.
            if (mIsRewardedAdPlaying)
                return;

            string id = mAdSettings.defaultRewardedAdId.Id;

            if (string.IsNullOrEmpty(id))
            {
                Debug.Log("Attempting to load AdMob rewarded ad with an undefined ID");
                return;
            }

            if (mRewardedAd == null)
                mRewardedAd = CreateNewRewardedAd(id);

            mRewardedAd.LoadAd(CreateAdMobAdRequest());
#endif
        }

        protected override bool InternalIsRewardedAdReady()
        {
#if ACTIVE_ADMOB
            return mRewardedAd != null && mRewardedAd.IsLoaded();
#else
            return false;
#endif
        }

        protected override void InternalShowRewardedAd(string id)
        {
#if ACTIVE_ADMOB
            mIsRewardedAdPlaying = true;
            mRewardedAd.Show();
#endif
        }

        #endregion  // AdClient Overrides

        #region Private Methods

#if ACTIVE_ADMOB

        private AdSize ToAdMobAdSize(BannerAdSize adSize)
        {
            return adSize.IsSmartBanner ? AdSize.SmartBanner : new AdSize(adSize.Width, adSize.Height);
        }

        private AdPosition ToAdMobAdPosition(BannerAdPosition pos)
        {
            switch (pos)
            {
                case BannerAdPosition.Top:
                    return AdPosition.Top;
                case BannerAdPosition.Bottom:
                    return AdPosition.Bottom;
                case BannerAdPosition.TopLeft:
                    return AdPosition.TopLeft;
                case BannerAdPosition.TopRight:
                    return AdPosition.TopRight;
                case BannerAdPosition.BottomLeft:
                    return AdPosition.BottomLeft;
                case BannerAdPosition.BottomRight:
                    return AdPosition.BottomRight;
                default:
                    return AdPosition.Top;
            }
        }

        private AdRequest CreateAdMobAdRequest()
        {
            AdRequest.Builder adBuilder = new AdRequest.Builder();

            // Test mode.
            if (mAdSettings.enableTestMode)
            {
                // Add all emulators
                adBuilder.AddTestDevice(AdRequest.TestDeviceSimulator);

                // Add user-specified test devices
                for (int i = 0; i < mAdSettings.testDeviceIds.Length; i++)
                    if (!string.IsNullOrEmpty(mAdSettings.testDeviceIds[i]))
                    {
                        adBuilder.AddTestDevice(mAdSettings.testDeviceIds[i].Trim());
                        Debug.Log("Admob add test devide: " + mAdSettings.testDeviceIds[i]);
                    }

                adBuilder.AddTestDevice(SystemInfo.deviceUniqueIdentifier.ToUpper());
            }

            return adBuilder.Build();
        }

        /// <summary>
        /// Create new banner, register all the events and load it automatically.
        /// </summary>
        /// <param name="position">The new banner will be placed at this position.</param>
        /// <param name="size">Size of the new banner.</param>
        /// <param name="bannerId">Id to request new banner.</param>
        private BannerView CreateNewBanner(BannerAdPosition position, BannerAdSize size, string bannerId)
        {
            BannerView newBanner = new BannerView(
                                       bannerId,
                                       ToAdMobAdSize(size),
                                       ToAdMobAdPosition(position)
                                   );

            /// Register for banner ad events.
            newBanner.OnAdLoaded += HandleAdMobBannerAdLoaded;
            newBanner.OnAdFailedToLoad += HandleAdMobBannerAdFailedToLoad;
            newBanner.OnAdOpening += HandleAdMobBannerAdOpening;
            newBanner.OnAdClosed += HandleAdMobBannerAdClosed;
            newBanner.OnAdLeavingApplication += HandleAdMobBannerAdLeftApplication;

            newBanner.LoadAd(CreateAdMobAdRequest());

            return newBanner;
        }

        /// <summary>
        /// Create new interstitial ad and register all the events.
        /// </summary>
        /// <param name="interstitialAdId">Id to request new interstitial ad.</param>
        /// <param name="placement">Used when invoking events.</param>
        private InterstitialAd CreateNewInterstitialAd(string interstitialAdId)
        {
            // Create new interstitial object.
            InterstitialAd defaultInterstitialAd = new InterstitialAd(interstitialAdId);

            // Register for interstitial ad events.
            defaultInterstitialAd.OnAdLoaded += HandleAdMobInterstitialLoaded;
            defaultInterstitialAd.OnAdFailedToLoad += HandleAdMobInterstitialFailedToLoad;
            defaultInterstitialAd.OnAdOpening += HandleAdMobInterstitialOpening;
            defaultInterstitialAd.OnAdClosed += (sender, param) => HandleAdMobInterstitialClosed(sender, param);
            defaultInterstitialAd.OnAdLeavingApplication += HandleAdMobInterstitialLeftApplication;

            return defaultInterstitialAd;
        }

        /// <summary>
        /// Create new rewarded video ad and register all the events.
        /// </summary>
        private RewardedAd CreateNewRewardedAd(string interstitialAdId)
        {
            RewardedAd newRewardedAd = new RewardedAd(interstitialAdId);

            // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
            newRewardedAd.OnAdLoaded += HandleAdMobRewardBasedVideoLoaded;
            newRewardedAd.OnAdFailedToLoad += HandleAdMobRewardBasedVideoFailedToLoad;
            newRewardedAd.OnAdFailedToShow += HandleAdMobRewardBasedVideoFailedToShow;
            newRewardedAd.OnAdOpening += HandleAdMobRewardBasedVideoOpening;
            newRewardedAd.OnUserEarnedReward += HandleAdMobRewardBasedVideoRewarded;
            newRewardedAd.OnAdClosed += HandleAdMobRewardBasedVideoClosed;

            return newRewardedAd;
        }

        /// <summary>
        /// Destroy an interstitial ad and invoke the InterstitialAdCompleted event.
        /// </summary>
        /// Called in HandleAdMobInterstitialClosed event handler.
        private void CloseInterstititlaAd()
        {
            if (mDefaultInterstitialAd != null)
            {
                mDefaultInterstitialAd.Destroy();
                mDefaultInterstitialAd = null;
            }

            OnInterstitialAdCompleted();
        }

        /// <summary>
        /// Get the right action to invoke when a rewarded ad is skipped.
        /// </summary>
        private Action GetRewardedAdSkippedAction()
        {
            return () => { OnRewardedAdSkipped(); };
        }

        /// <summary>
        /// Get the right action to invoke when a rewarded ad is completed.
        /// </summary>
        private Action GetRewardedAdCompletedAction()
        {
            return () => { OnRewardedAdCompleted(); };
        }

#endif

        #endregion // Private Methods

        #region Ad Event Handlers

#if ACTIVE_ADMOB

        //------------------------------------------------------------
        // Banner Ads Callbacks.
        //------------------------------------------------------------

        private void HandleAdMobBannerAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob banner ad has been loaded successfully.");

            if (OnBannerAdLoaded != null)
                OnBannerAdLoaded.Invoke(sender, args);
        }

        private void HandleAdMobBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("AdMob banner ad failed to load. Error: " + args.Message);

            if (OnBannerAdFailedToLoad != null)
                OnBannerAdFailedToLoad.Invoke(sender, args);
        }

        private void HandleAdMobBannerAdOpening(object sender, EventArgs args)
        {
            if (OnBannerAdOpening != null)
                OnBannerAdOpening.Invoke(sender, args);
        }

        private void HandleAdMobBannerAdClosed(object sender, EventArgs args)
        {
            if (OnBannerAdClosed != null)
                OnBannerAdClosed.Invoke(sender, args);
        }

        private void HandleAdMobBannerAdLeftApplication(object sender, EventArgs args)
        {
            if (OnBannerAdLeavingApplication != null)
                OnBannerAdLeavingApplication.Invoke(sender, args);
        }

        //------------------------------------------------------------
        // Interstitial Ads Callbacks.
        //------------------------------------------------------------

        private void HandleAdMobInterstitialLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob interstitial ad has been loaded successfully.");

            if (OnInterstitialAdLoaded != null)
                OnInterstitialAdLoaded.Invoke(sender, args);
        }

        private void HandleAdMobInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("AdMob interstitial ad failed to load. Error: " + args.Message);

            if (OnInterstitialAdFailedToLoad != null)
                OnInterstitialAdFailedToLoad.Invoke(sender, args);
        }

        private void HandleAdMobInterstitialOpening(object sender, EventArgs args)
        {
            if (OnInterstititalAdOpening != null)
                OnInterstititalAdOpening.Invoke(sender, args);
        }

        private void HandleAdMobInterstitialClosed(object sender, EventArgs args)
        {
            CloseInterstititlaAd();

            if (OnInterstitialAdClosed != null)
                OnInterstitialAdClosed.Invoke(sender, args);
        }

        private void HandleAdMobInterstitialLeftApplication(object sender, EventArgs args)
        {
            if (OnInterstitialAdLeavingApplication != null)
                OnInterstitialAdLeavingApplication.Invoke(sender, args);
        }

        //------------------------------------------------------------
        // Rewarded Ads Callbacks.
        //------------------------------------------------------------

        private void HandleAdMobRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob rewarded video ad has been loaded successfully.");

            if (OnRewardedAdLoaded != null)
                OnRewardedAdLoaded.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoFailedToLoad(object sender, AdErrorEventArgs args)
        {
            Debug.Log("AdMob rewarded video ad failed to load. Message: " + args.Message);

            if (OnRewardedAdFailedToLoad != null)
                OnRewardedAdFailedToLoad.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoFailedToShow(object sender, AdErrorEventArgs args)
        {
            Debug.Log("AdMob rewarded video ad failed to show. Message: " + args.Message);

            if (OnRewardedAdFailedToShow != null)
                OnRewardedAdFailedToShow.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoRewarded(object sender, Reward args)
        {
            mIsRewardedAdCompleted = true;

            if (OnRewardedAdRewarded != null)
                OnRewardedAdRewarded.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoOpening(object sender, EventArgs args)
        {
            if (OnRewardedAdOpening != null)
                OnRewardedAdOpening.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoClosed(object sender, EventArgs args)
        {
            // Make sure this method always be called after the OnAdRewarded event.
            RuntimeHelper.RunOnMainThread(() =>
                RuntimeHelper.RunCoroutine(RewardedBasedVideoClosedDelayCoroutine(sender, args)));
        }

        private IEnumerator RewardedBasedVideoClosedDelayCoroutine(object sender, EventArgs args)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            // Ad is not playing anymore.
            mIsRewardedAdPlaying = false;

            // If the ad was completed, the "rewarded" event should be fired previously,
            // setting the completed bool to true. Otherwise the ad was skipped.
            // Events are raised on main thread.
            Action callback = mIsRewardedAdCompleted ? GetRewardedAdCompletedAction() : GetRewardedAdSkippedAction();
            callback();

            // Reset the completed flag.
            mIsRewardedAdCompleted = false;
            mRewardedAd = null;

            if (OnRewardedAdClosed != null)
                OnRewardedAdClosed.Invoke(sender, args);

            LoadRewardedAd();
        }
#endif

        #endregion // Ad Event Handlers
    }

    //===========================================================================

    [Serializable]
    public class AdMobSettings : AdSettings
    {
        public AdId appId;
        public AdId defaultBannerAdId;
        public AdId defaultInterstitialAdId;
        public AdId defaultRewardedAdId;
        public bool enableTestMode;
        public string[] testDeviceIds;
        [Range(0, 10)]
        public int weight;

        public int CurInterstitialAdWeight
        {
            get { return PlayerPrefs.GetInt("AdMob_InterstitialAdWeight", weight); }
            set { PlayerPrefs.SetInt("Admob_InterstitialAdWeight", value); }
        }
        public int CurRewardedAdWeight
        {
            get { return PlayerPrefs.GetInt("AdMob_RewardedAdWeight", weight); }
            set { PlayerPrefs.SetInt("Admob_RewardedAdWeight", value); }
        }
        public void ResetCurWeight()
        {
            CurInterstitialAdWeight = weight;
            CurRewardedAdWeight = weight;
        }
    }
}