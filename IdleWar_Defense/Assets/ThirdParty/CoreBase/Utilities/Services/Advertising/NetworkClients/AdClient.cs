using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Utilities.Service.Ads
{
    public enum AdType
    {
        Banner,
        Interstitial,
        Rewarded
    }

    public enum BannerAdPosition
    {
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class AdSettings { }

    public abstract class AdClient : IAdClient
    {
        #region IAdClient Implementation

        protected bool mIsInitialized = false;

        /// <summary>
        /// Whether the required SDK is available.
        /// </summary>
        /// <value><c>true</c> if avail; otherwise, <c>false</c>.</value>
        public abstract bool IsSdkAvail { get; }

        /// <summary>
        /// Checks if the placement is valid, i.e. it has non-empty
        /// associated IDs if such placement require dedicated IDs.
        /// </summary>

        public abstract bool IsValid(AdType type);

        /// <summary>
        /// The message to print if the required SDK is not available.
        /// </summary>
        /// <value>The no sdk message.</value>
        protected abstract string NoSdkMessage { get; }

        /// <summary>
        /// Instructs the underlaying SDK to show a banner ad. Only invoked if the client is initialized.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="size">Size.</param>
        protected abstract void InternalShowBannerAd(BannerAdPosition position, BannerAdSize size);

        /// <summary>
        /// Instructs the underlaying SDK to hide a banner ad. Only invoked if the client is initialized.
        /// </summary>
        protected abstract void InternalHideBannerAd();

        /// <summary>
        /// Instructs the underlaying SDK to destroy a banner ad. Only invoked if the client is initialized.
        /// </summary>
        protected abstract void InternalDestroyBannerAd();

        /// <summary>
        /// Instructs the underlaying SDK to load an interstitial ad. Only invoked if the client is initialized.
        /// </summary>
        protected abstract void InternalLoadInterstitialAd();

        /// <summary>
        /// Checks with the underlaying SDK to see if an interstitial ad is loaded. Only invoked if the client is initialized.
        /// </summary>
        /// <returns><c>true</c>, if is interstitial ad ready was internaled, <c>false</c> otherwise.</returns>
        protected abstract bool InternalIsInterstitialAdReady();

        /// <summary>
        /// Instructs the underlaying SDK to show an interstitial ad. Only invoked if the client is initialized.
        /// </summary>
        protected abstract void InternalShowInterstitialAd(string id);

        /// <summary>
        /// Instructs the underlaying SDK to load a rewarded ad. Only invoked if the client is initialized.
        /// </summary>
        protected abstract void InternalLoadRewardedAd();

        /// <summary>
        /// Checks with the underlaying SDK to see if a rewarded ad is loaded. Only invoked if the client is initialized.
        /// </summary>
        /// <returns><c>true</c>, if is rewarded ad ready was internaled, <c>false</c> otherwise.</returns>
        protected abstract bool InternalIsRewardedAdReady();

        /// <summary>
        /// Instructs the underlaying SDK to show a rewarded ad. Only invoked if the client is initialized.
        /// </summary>
        protected abstract void InternalShowRewardedAd(string id);

        /// <summary>
        /// Occurs when an interstitial ad completed.
        /// </summary>
        public event Action<IAdClient> InterstitialAdCompleted;

        /// <summary>
        /// Occurs when a rewarded ad is skipped.
        /// </summary>
        public event Action<IAdClient> RewardedAdSkipped;

        /// <summary>
        /// Occurs when a rewarded ad completed.
        /// </summary>
        public event Action<IAdClient> RewardedAdCompleted;

        /// <summary>
        /// Gets the associated ad network of this client.
        /// </summary>
        /// <value>The network.</value>
        public abstract AdNetwork Network { get; }

        /// <summary>
        /// Whether banner ads are supported.
        /// </summary>
        /// <value><c>true</c> if banner ads are supported; otherwise, <c>false</c>.</value>
        public abstract bool IsBannerAdSupported { get; }

        /// <summary>
        /// Whether interstitial ads are supported.
        /// </summary>
        /// <value><c>true</c> if interstitial ads are supported; otherwise, <c>false</c>.</value>
        public abstract bool IsInterstitialAdSupported { get; }

        /// <summary>
        /// Whether rewarded ads are supported.
        /// </summary>
        /// <value><c>true</c> if rewarded ads are supported; otherwise, <c>false</c>.</value>
        public abstract bool IsRewardedAdSupported { get; }

        /// <summary>
        /// Gets a value indicating whether this client is initialized.
        /// </summary>
        /// <value>true</value>
        /// <c>false</c>
        public virtual bool IsInitialized
        {
            get { return mIsInitialized; }
        }

        public abstract void Init(AdSettings pSettings);

        /// <summary>
        /// Shows the banner ad, position and size.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="size">Size.</param>
        public virtual void ShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
            if (IsSdkAvail)
            {
                if (size == null)
                {
                    Debug.LogFormat("Cannot show {0} banner ad with ad size: null", Network.ToString());
                    return;
                }

                if (CheckInitialize())
                    InternalShowBannerAd(position, size);
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Hides the banner ad.
        /// </summary>
        public virtual void HideBannerAd()
        {
            if (CheckInitialize())
                InternalHideBannerAd();
        }

        /// <summary>
        /// Destroys the banner ad.
        /// </summary>
        public virtual void DestroyBannerAd()
        {
            if (CheckInitialize())
                InternalDestroyBannerAd();
        }

        /// <summary>
        /// Loads the interstitial ad.
        /// </summary>
        public virtual void LoadInterstitialAd()
        {
            if (IsSdkAvail)
            {
                if (!CheckInitialize())
                    return;

                // Not reloading a loaded ad.
                if (!IsInterstitialAdReady())
                    InternalLoadInterstitialAd();
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Determines whether the interstitial ad is loaded.
        /// </summary>
        /// <returns><c>true</c> if the ad is loaded; otherwise, <c>false</c>.</returns>
        public virtual bool IsInterstitialAdReady()
        {
            if (CheckInitialize(false))
                return InternalIsInterstitialAdReady();
            else
                return false;
        }

        /// <summary>
        /// Shows the interstitial ad.
        /// </summary>
        public virtual void ShowInterstitialAd(string id)
        {
            if (IsSdkAvail)
            {
                if (!CheckInitialize())
                    return;

                if (!IsInterstitialAdReady())
                {
                    Debug.LogFormat("Cannot show {0} interstitial ad: ad is not loaded.",
                        Network.ToString());
                    return;
                }

                InternalShowInterstitialAd(id);
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Loads the rewarded ad.
        /// </summary>
        public virtual void LoadRewardedAd()
        {
            if (IsSdkAvail)
            {
                if (!CheckInitialize())
                    return;

                // Not reloading a loaded ad.
                if (!IsRewardedAdReady())
                    InternalLoadRewardedAd();
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Determines whether the rewarded ad is loaded.
        /// </summary>
        /// <returns><c>true</c> if the ad is loaded; otherwise, <c>false</c>.</returns>
        public virtual bool IsRewardedAdReady()
        {
            if (CheckInitialize(false))
                return InternalIsRewardedAdReady();
            else
                return false;
        }

        /// <summary>
        /// Shows the rewarded ad.
        /// </summary>
        public virtual void ShowRewardedAd(string id)
        {
            if (IsSdkAvail)
            {
                if (!CheckInitialize())
                    return;

                if (!IsRewardedAdReady())
                {
                    Debug.LogFormat("Cannot show {0} rewarded ad: ad is not loaded.",
                        Network.ToString());
                    return;
                }

                InternalShowRewardedAd(id);
            }
            else
            {
                Debug.Log(NoSdkMessage);
            }
        }

        /// <summary>
        /// Raises the <see cref="InterstitialAdCompleted"/> event.
        /// </summary>
        protected virtual void OnInterstitialAdCompleted()
        {
            RuntimeHelper.RunOnMainThread(() =>
            {
                if (InterstitialAdCompleted != null)
                    InterstitialAdCompleted(this);
            });
        }

        /// <summary>
        /// Raises the <see cref="RewardedAdSkipped"/> event.
        /// </summary>
        protected virtual void OnRewardedAdSkipped()
        {
            RuntimeHelper.RunOnMainThread(() =>
            {
                if (RewardedAdSkipped != null)
                    RewardedAdSkipped(this);
            });
        }

        /// <summary>
        /// Raises the <see cref="RewardedAdCompleted"/> event.
        /// </summary>
        protected virtual void OnRewardedAdCompleted()
        {
            RuntimeHelper.RunOnMainThread(() =>
            {
                if (RewardedAdCompleted != null)
                    RewardedAdCompleted(this);
            });
        }

        /// <summary>
        /// Checks if the client is initialized and print a warning message if not.
        /// </summary>
        /// <returns><c>true</c>, if initialize was checked, <c>false</c> otherwise.</returns>
        protected virtual bool CheckInitialize(bool logMessage = true)
        {
            if (Network == AdNetwork.None)
                return false;

            bool isInit = IsInitialized;

            if (!isInit && logMessage)
                Debug.Log("Please initialize the " + Network.ToString() + " client first.");

            return isInit;
        }

        #endregion
    }
}