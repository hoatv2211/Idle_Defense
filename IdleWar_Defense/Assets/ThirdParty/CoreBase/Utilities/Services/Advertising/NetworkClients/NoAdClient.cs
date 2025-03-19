using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Utilities.Service.Ads
{
    internal class NoOpClient : AdClient
    {
        // Singleton.
        private static NoOpClient sInstance;

        private NoOpClient()
        {
        }

        /// <summary>
        /// Creates and initializes the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static NoOpClient CreateClient()
        {
            if (sInstance == null)
                sInstance = new NoOpClient();
            return sInstance;
        }

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.None; } }

        public override bool IsBannerAdSupported { get { return false; } }

        public override bool IsInterstitialAdSupported { get { return false; } }

        public override bool IsRewardedAdSupported { get { return false; } }

        public override bool IsSdkAvail { get { return true; } }

        protected override string NoSdkMessage { get { return string.Empty; } }

        public override bool IsValid(AdType type)
        {
            return false;
        }

        protected override void InternalShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
        }

        protected override void InternalHideBannerAd()
        {
        }

        protected override void InternalDestroyBannerAd()
        {
        }

        protected override void InternalLoadInterstitialAd()
        {
        }

        protected override bool InternalIsInterstitialAdReady()
        {
            return false;
        }

        protected override void InternalShowInterstitialAd(string id)
        {
        }

        protected override void InternalLoadRewardedAd()
        {
        }

        protected override bool InternalIsRewardedAdReady()
        {
            return false;
        }

        protected override void InternalShowRewardedAd(string id)
        {
        }

        public override void Init(AdSettings pSettings)
        {
        }

        #endregion
    }
}