using UnityEngine;
using System;
using System.Collections.Generic;

namespace Utilities.Service.Ads
{
    public interface IAdClient
    {
        #region General Info

        /// <summary>
        /// Gets the associated ad network of this client.
        /// </summary>
        /// <value>The network.</value>
        AdNetwork Network { get; }

        /// <summary>
        /// Whether banner ads are supported.
        /// </summary>
        /// <value><c>true</c> if banner ads are supported; otherwise, <c>false</c>.</value>
        bool IsBannerAdSupported { get; }

        /// <summary>
        /// Whether interstitial ads are supported.
        /// </summary>
        /// <value><c>true</c> if interstitial ads are supported; otherwise, <c>false</c>.</value>
        bool IsInterstitialAdSupported { get; }

        /// <summary>
        /// Whether rewarded ads are supported.
        /// </summary>
        /// <value><c>true</c> if rewarded ads are supported; otherwise, <c>false</c>.</value>
        bool IsRewardedAdSupported { get; }

        /// <summary>
        /// Whether the required SDK is available.
        /// </summary>
        /// <value><c>true</c> if avail; otherwise, <c>false</c>.</value>
        bool IsSdkAvail { get; }

        #endregion

        #region Initialization

        /// <summary>
        /// Gets a value indicating whether this client is initialized.
        /// </summary>
        /// <value><c>true</c> if this client is initialized; otherwise, <c>false</c>.</value>
        bool IsInitialized { get; }

        /// <summary>
        /// Check if placement is valid.
        /// </summary>
        /// <param name="placement">Placement.</param>
        bool IsValid(AdType type);

        #endregion

        #region Banner Ads

        /// <summary>
        /// Hides the banner ad at the default placement.
        /// </summary>
        void HideBannerAd();

        /// <summary>
        /// Destroys the banner ad at the default placement.
        /// </summary>
        void DestroyBannerAd();

        /// <summary>
        /// Shows the banner ad.
        /// </summary>
        /// <param name="placement">Placement.</param>
        /// <param name="position">Position.</param>
        /// <param name="size">Size.</param>
        void ShowBannerAd(BannerAdPosition position, BannerAdSize size);

        #endregion

        #region Interstitial Ads

        /// <summary>
        /// Occurs when an interstitial ad completed.
        /// This event is always raised on main thread.
        /// </summary>
        event Action<IAdClient> InterstitialAdCompleted;

        /// <summary>
        /// Loads the interstitial ad at the default placement.
        /// </summary>
        void LoadInterstitialAd();

        /// <summary>
        /// Determines whether the interstitial ad at the default placement is loaded.
        /// </summary>
        /// <returns><c>true</c> if the ad is loaded; otherwise, <c>false</c>.</returns>
        bool IsInterstitialAdReady();

        /// <summary>
        /// Shows the interstitial ad.
        /// </summary>
        void ShowInterstitialAd(string id);

        #endregion

        #region Rewarded Ads

        /// <summary>
        /// Occurs when a rewarded ad is skipped. This event is always raised on main thread.
        /// </summary>
        event Action<IAdClient> RewardedAdSkipped;

        /// <summary>
        /// Occurs when a rewarded ad completed. This event is always raised on main thread.
        /// </summary>
        event Action<IAdClient> RewardedAdCompleted;

        /// <summary>
        /// Loads the rewarded ad at the default placement.
        /// </summary>
        void LoadRewardedAd();

        /// <summary>
        /// Determines whether the rewarded ad ready at the default placement is loaded.
        /// </summary>
        /// <returns><c>true</c> if the ad is loaded; otherwise, <c>false</c>.</returns>
        bool IsRewardedAdReady();

        /// <summary>
        /// Shows the rewarded ad at the default placement.
        /// </summary>
        void ShowRewardedAd(string id);

        #endregion
    }
}