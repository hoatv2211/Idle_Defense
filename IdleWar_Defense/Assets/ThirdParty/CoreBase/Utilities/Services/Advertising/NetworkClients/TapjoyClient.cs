using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Service.Ads
{
    using System;
#if ACTIVE_TAPJOY
    using TapjoyUnity;
#endif

    public class TapjoyClient : AdClient
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the TapJoy plugin.";
        private const string BANNER_UNSUPPORTED_MESSAGE = "TapJoy does not support banner ad format";

#if ACTIVE_TAPJOY
        private TapjoySettings mAdSettings = null;

        private TJPlacement mDefaultInterstitialPlacement;
        private TJPlacement mDefaultRewaredVideoPlacement;
        private TJPlacement mDefaultOfferWallPlacement;

        private IEnumerator mAutoReconnectCoroutine;
        private bool mIsAutoReconnectCoroutineRunning;

        public Action<int> onUpdatedBallance;
        private Action<bool> mOnGetBallanceResponsed;
        private Action<bool> mOnSpentBallanceResponsed;
        private Action<bool> mOnAwardCurrencyResponsed;
#endif

        public int currentBallance { get; private set; }

        #region Singleton

        private static TapjoyClient sInstance = null;

        public static TapjoyClient CreateClient()
        {
            if (sInstance == null)
                sInstance = new TapjoyClient();

            return sInstance;
        }

        #endregion

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.TapJoy; } }

        public override bool IsBannerAdSupported { get { return false; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        {
            get
            {
#if ACTIVE_TAPJOY
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValid(AdType type)
        {
#if ACTIVE_TAPJOY
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

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }


        public override void Init(AdSettings pSettings)
        {
#if ACTIVE_TAPJOY

            mIsInitialized = true;
            mAdSettings = pSettings as TapjoySettings;

            // TapJoy's placements can only be created after the game is successfully connnected to server.
            // If it is already connected when we're initializing this client, create all of them normally...
            if (Tapjoy.IsConnected)
            {
                CreateDefaultAdPlacements(mAdSettings);
            }
            // ...otherwise, we need to wait until it has been connected.
            else
            {
                Tapjoy.OnConnectSuccess += () =>
                {
                    CreateDefaultAdPlacements(mAdSettings);
                };
            }

            // Subscribe to events.
            SetupTapJoyEventCallbacks();
            SetupTapJoyPlacementEventCallbacks();

            if (mAdSettings.enableLog)
                Debug.Log("Tapjoy client has been initialized.");
#endif
        }

        protected override void InternalShowBannerAd(BannerAdPosition __, BannerAdSize ___)
        {
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
        }

        protected override void InternalHideBannerAd()
        {
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
        }

        protected override void InternalDestroyBannerAd()
        {
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
        }

        protected override bool InternalIsInterstitialAdReady()
        {
#if ACTIVE_TAPJOY
            return IsPlacementAvailable(mDefaultInterstitialPlacement);
#else
            return false;
#endif
        }

        protected override void InternalLoadInterstitialAd()
        {
#if ACTIVE_TAPJOY
            LoadPlacement(mDefaultInterstitialPlacement);
#endif
        }

        protected override void InternalShowInterstitialAd(string id)
        {
#if ACTIVE_TAPJOY
            ShowPlacement(mDefaultInterstitialPlacement);
#endif
        }

        protected override bool InternalIsRewardedAdReady()
        {
#if ACTIVE_TAPJOY
            return IsPlacementAvailable(mDefaultRewaredVideoPlacement);
#else
            return false;
#endif
        }

        protected override void InternalLoadRewardedAd()
        {
#if ACTIVE_TAPJOY
            LoadPlacement(mDefaultRewaredVideoPlacement);
#endif
        }

        protected override void InternalShowRewardedAd(string id)
        {
#if ACTIVE_TAPJOY
            ShowPlacement(mDefaultRewaredVideoPlacement);
#endif
        }

        #endregion

        #region Tap Joy Extented

        public bool IsOfferwallReady()
        {
#if ACTIVE_TAPJOY
            return IsPlacementAvailable(mDefaultOfferWallPlacement);
#else
            return false;
#endif
        }

        public void LoadOfferwall()
        {
#if ACTIVE_TAPJOY
            LoadPlacement(mDefaultOfferWallPlacement);
#endif
        }

        public void AwardCurrency(int pAmount, Action<bool> pOnResponsed)
        {
#if ACTIVE_TAPJOY
            mOnAwardCurrencyResponsed = pOnResponsed;
            Tapjoy.AwardCurrency(pAmount);
#endif
        }

        public void ShowOfferWall()
        {
#if ACTIVE_TAPJOY
            ShowPlacement(mDefaultOfferWallPlacement);
#endif
        }

        public override void LoadRewardedAd()
        {
            if (IsSdkAvail)
            {
                if (!CheckInitialize())
                    return;

                // Not reloading a loaded ad.
                if (!IsRewardedAdReady())
                    InternalLoadRewardedAd();

                if (!IsOfferwallReady())
                    LoadOfferwall();
            }
            else
            {
                Debug.LogWarning(NoSdkMessage);
            }
        }

        public void UpdateCurrencyBallance(Action<bool> pOnResponsed)
        {
#if ACTIVE_TAPJOY
            mOnGetBallanceResponsed = pOnResponsed;
            Tapjoy.GetCurrencyBalance();
#endif
        }

        public void SpendCurrencyBallance(int pAmount, Action<bool> pOnResponsed)
        {
#if ACTIVE_TAPJOY
            mOnSpentBallanceResponsed = pOnResponsed;
            Tapjoy.SpendCurrency(pAmount);
#endif
        }

        #endregion

#if ACTIVE_TAPJOY

        #region Protected methods

        protected IEnumerator IEAutoReconnect(float interval)
        {
            if (interval < 0)
                yield break;

            var wait = new WaitForSeconds(interval);

            while (true)
            {
                Tapjoy.Connect();

                if (mAdSettings.enableLog)
                    Debug.Log("Connecting to Tapjoy server...");

                yield return wait;
            }
        }

        /// <summary>
        /// Creates default InterstitialPlacement and RewaredVideoPlacement.
        /// </summary>
        protected void CreateDefaultAdPlacements(TapjoySettings adSettings)
        {
            mDefaultInterstitialPlacement = TJPlacement.CreatePlacement(adSettings.defaultInterstitialAdId.Id);
            mDefaultRewaredVideoPlacement = TJPlacement.CreatePlacement(adSettings.defaultRewardedAdId.Id);
            mDefaultOfferWallPlacement = TJPlacement.CreatePlacement(adSettings.defaultOfferWall.Id);

            Tapjoy.GetCurrencyBalance();
        }

        /// <summary>
        /// Registers all TapJoy's events into right handlers.
        /// </summary>
        protected virtual void SetupTapJoyEventCallbacks()
        {
            Tapjoy.OnConnectSuccess += HandleOnConnectSuccess;
            Tapjoy.OnConnectFailure += HandleOnConnectFailure;

            Tapjoy.OnAwardCurrencyResponse += HandleOnAwardCurrencyResponse;
            Tapjoy.OnAwardCurrencyResponseFailure += HandleOnAwardCurrencyResponseFailure;
            Tapjoy.OnEarnedCurrency += HandleOnEarnedCurrency;
            Tapjoy.OnGetCurrencyBalanceResponse += HandleGetCurrencyBalanceResponse;
            Tapjoy.OnGetCurrencyBalanceResponseFailure += HandleGetCurrencyBalanceResponseFailure;
            Tapjoy.OnSpendCurrencyResponse += HandleSpendCurrencyResponse;
            Tapjoy.OnSpendCurrencyResponseFailure += HandleSpendCurrencyResponseFailure;

            Tapjoy.OnVideoComplete += HandleTapJoyOnVideoComplete;
            Tapjoy.OnVideoError += HandleTapJoyOnVideoError;
            Tapjoy.OnVideoStart += HandleTapJoyOnVideoStart;
        }

        /// <summary>
        /// Registers all TJPlacement's events into right handlers.
        /// </summary>
        protected virtual void SetupTapJoyPlacementEventCallbacks()
        {
            TJPlacement.OnContentDismiss += HandleOnContentDismiss;
            TJPlacement.OnContentReady += HandleOnContentReady;
            TJPlacement.OnContentShow += HandleOnContentShow;
            TJPlacement.OnPurchaseRequest += HandleOnPurchaseRequest;
            TJPlacement.OnRequestFailure += HandleOnRequestFailure;
            TJPlacement.OnRequestSuccess += HandleOnRequestSuccess;
            TJPlacement.OnRewardRequest += HandleOnRewardRequest;

            TJPlacement.OnVideoComplete += HandleTJPlacementOnVideoComplete;
            TJPlacement.OnVideoError += HandleTJPlacementOnVideoError;
            TJPlacement.OnVideoStart += HandleTJPlacementOnVideoStart;
        }

        /// <summary>
        /// Checks if a specific TapJoy placement is available or not.
        /// </summary>
        protected virtual bool IsPlacementAvailable(TJPlacement placement)
        {
            return placement == null ? false : placement.IsContentReady();
        }

        /// <summary>
        /// Loads a specific TapJoy placement if it's not null.
        /// </summary>
        /// <param name="nullMessage">This message will be logged to the console if the placement is null.</param>
        protected virtual void LoadPlacement(TJPlacement placement, string nullMessage = "Attempting to load a null Tapjoy placement.")
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("The TapJoy network is not ready...");
                return;
            }

            if (placement == null)
            {
                Debug.LogWarning(nullMessage);
                return;
            }

            placement.RequestContent();
        }

        /// <summary>
        /// Shows a specific TapJoy placement if it's not null.
        /// </summary>
        /// <param name="nullMessage">This message will be logged to the console if the placement is null.</param>
        protected virtual void ShowPlacement(TJPlacement placement, string nullMessage = "Attempting to show a null Tapjoy placement.")
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("The TapJoy network is not ready...");
                return;
            }

            if (placement == null)
            {
                Debug.LogWarning(nullMessage);
                return;
            }

            placement.ShowContent();
        }

        /// <summary>
        /// Checks the placement name and invoke right completed event.
        /// </summary>
        /// Called in HandleTJPlacementOnVideoComplete event handler.
        protected virtual void InvokePlacementCompleteEvent(TJPlacement tjPlacement)
        {
            if (mAdSettings == null || tjPlacement == null)
            {
                Debug.LogWarning("Null value(s)!!!");
                return;
            }

            string targetName = tjPlacement.GetName();

            /// Check if the tjPlacement is the default interstitial placement.
            if (mDefaultInterstitialPlacement != null && mDefaultInterstitialPlacement.GetName().Equals(targetName))
            {
                OnInterstitialAdCompleted();
                return;
            }

            /// Check if the tjPlacement is the default rewarded video placement.
            if (mDefaultRewaredVideoPlacement != null && mDefaultRewaredVideoPlacement.GetName().Equals(targetName))
            {
                OnRewardedAdCompleted();
                return;
            }

            /// Check if the tjPlacement is the default rewarded video placement.
            if (mDefaultOfferWallPlacement != null && mDefaultOfferWallPlacement.GetName().Equals(targetName))
            {
                return;
            }

            /// Otherwise
            Debug.LogWarning("Tried to invoke completed event of an unexpected custom placement. Name: " + targetName);
        }

        #endregion

        #region Ad Event Handlers

        /// TapJoy's events.

        private void SetBallance(int pBallance)
        {
            if (currentBallance == pBallance)
                return;

            currentBallance = pBallance;
            if (onUpdatedBallance != null) onUpdatedBallance(pBallance);
        }

        private void HandleOnConnectSuccess()
        {
            if (mAdSettings.enableLog)
                Debug.Log("Connect to TapJoy server successfully");

            /// When we connected to Tapjoy server,
            /// stop the reconnect progress if it's running.
            if (mIsAutoReconnectCoroutineRunning)
            {
                mIsAutoReconnectCoroutineRunning = false;
                RuntimeHelper.EndCoroutine(mAutoReconnectCoroutine);
            }

            if (!IsOfferwallReady())
                LoadOfferwall();
        }

        private void HandleOnConnectFailure()
        {
            if (mAdSettings.enableLog)
                Debug.Log("Failed to connect to TapJoy server");

            /// At default, Tapjoy only connect to the server once when the app is opened,
            /// so we have to start a coroutine to reconnect automatically.
            /// Otherwise players will never be able to connect to the server
            /// if they open the app when their device is offline.
            if (mAdSettings.enableAutoReconnect && !mIsAutoReconnectCoroutineRunning)
            {
                mAutoReconnectCoroutine = IEAutoReconnect(mAdSettings.autoReconnectInterval);
                RuntimeHelper.RunCoroutine(mAutoReconnectCoroutine);

                mIsAutoReconnectCoroutineRunning = true;
            }
        }

        private void HandleOnAwardCurrencyResponse(string currencyName, int balance)
        {
            SetBallance(balance);
            if (mOnAwardCurrencyResponsed != null) mOnAwardCurrencyResponsed(true);
            if (mAdSettings.enableLog)
                Debug.Log(string.Format("TapJoyClient::HandleOnAwardCurrencyResponse: {0}/{1}", currencyName, balance));
        }

        private void HandleOnAwardCurrencyResponseFailure(string errorMessage)
        {
            if (mOnAwardCurrencyResponsed != null) mOnAwardCurrencyResponsed(false);
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleOnAwardCurrencyResponceFailure: " + errorMessage);
        }

        private void HandleGetCurrencyBalanceResponse(string currencyName, int balance)
        {
            SetBallance(balance);
            if (mOnGetBallanceResponsed != null) mOnGetBallanceResponsed(true);
            if (mAdSettings.enableLog)
                Debug.Log(string.Format("TapJoyClient::HandleGetCurrencyBalanceResponse: {0}/{1}", currencyName, balance));
        }

        private void HandleGetCurrencyBalanceResponseFailure(string errorMessage)
        {
            if (mOnGetBallanceResponsed != null) mOnGetBallanceResponsed(false);
            if (mAdSettings.enableLog)
                Debug.Log(string.Format("TapJoyClient::HandleGetGetCurrencyBalanceResponseFailure: {0}", errorMessage));
        }

        private void HandleOnEarnedCurrency(string currencyName, int amount)
        {
            currentBallance += amount;
            if (mAdSettings.enableLog)
                Debug.Log(string.Format("TapJoyClient::HandleOnEarnedCurrency: {0}/{1}", currencyName, amount));
        }

        private void HandleSpendCurrencyResponse(string currencyName, int balance)
        {
            SetBallance(balance);
            if (mOnSpentBallanceResponsed != null) mOnSpentBallanceResponsed(true);
            if (mAdSettings.enableLog)
                Debug.Log(string.Format("TapJoyClient::HandleSpendCurrencyResponse: {0}/{1}", currencyName, balance));
        }

        private void HandleSpendCurrencyResponseFailure(string errorMessage)
        {
            if (mOnSpentBallanceResponsed != null) mOnSpentBallanceResponsed(false);
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleTapJoyOnVideoError: " + errorMessage);
        }

        private void HandleTapJoyOnVideoComplete()
        {
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleTapJoyOnVideoComplete");
        }

        private void HandleTapJoyOnVideoError(string errorMessage)
        {
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleTapJoyOnVideoError: " + errorMessage);
        }

        private void HandleTapJoyOnVideoStart()
        {
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleTapJoyOnVideoStart");
        }

        /// TJPlacement's events

        private void HandleOnContentDismiss(TJPlacement placement)
        {
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleOnContentDismiss: " + placement.GetName());
        }

        private void HandleOnContentReady(TJPlacement placement)
        {
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleOnContentReady: " + placement.GetName());
        }

        private void HandleOnContentShow(TJPlacement placement)
        {
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleOnContentShow: " + placement.GetName());
        }

        private void HandleOnPurchaseRequest(TJPlacement placement, TJActionRequest request, string productId)
        {
            if (mAdSettings.enableLog)
                Debug.Log(string.Format("TapJoyClient::HandleOnPurchaseRequest: {0}/{1}", placement.GetName(), productId));
        }

        private void HandleOnRequestFailure(TJPlacement placement, string error)
        {
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleOnRequestFailure: " + placement.GetName());
        }

        private void HandleOnRequestSuccess(TJPlacement placement)
        {
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleOnRequestSuccess: " + placement.GetName());
        }

        private void HandleOnRewardRequest(TJPlacement placement, TJActionRequest request, string itemId, int quantity)
        {
            if (mAdSettings.enableLog)
                Debug.Log(string.Format("TapJoyClient::HandleOnRewardRequest: {0}/{1}/{2}", placement.GetName(), itemId, quantity));
        }

        private void HandleTJPlacementOnVideoComplete(TJPlacement placement)
        {
            InvokePlacementCompleteEvent(placement);
        }

        private void HandleTJPlacementOnVideoError(TJPlacement placement, string errorMessage)
        {
            if (mAdSettings.enableLog)
                Debug.Log(string.Format("TapJoyClient::HandleTJPlacementOnVideoError: {0}/{1}", placement.GetName(), errorMessage));
        }

        private void HandleTJPlacementOnVideoStart(TJPlacement placement)
        {
            if (mAdSettings.enableLog)
                Debug.Log("TapJoyClient::HandleTJPlacementOnVideoStart: " + placement.GetName());
        }

        #endregion
#endif
    }

    //================================================================

    [Serializable]
    public class TapjoySettings : AdSettings
    {
        public bool enableLog = true;

        /// <summary>
        /// Enables or disables auto-reconnect coroutine.
        /// </summary>
        public bool enableAutoReconnect = true;
        public float autoReconnectInterval = 10f;
        public AdId defaultInterstitialAdId;
        public AdId defaultRewardedAdId;
        public AdId defaultOfferWall;
        [Range(0, 10)]
        public int weight;

        public int CurInterstitialAdWeight
        {
            get { return PlayerPrefs.GetInt("TapJoy_InterstitialAdWeight", weight); }
            set { PlayerPrefs.SetInt("TapJoy_InterstitialAdWeight", value); }
        }
        public int CurRewardedAdWeight
        {
            get { return PlayerPrefs.GetInt("TapJoy_RewardedAdWeight", weight); }
            set { PlayerPrefs.SetInt("TapJoy_RewardedAdWeight", value); }
        }
        public void ResetCurWeight()
        {
            CurInterstitialAdWeight = weight;
            CurRewardedAdWeight = weight;
        }
    }
}
