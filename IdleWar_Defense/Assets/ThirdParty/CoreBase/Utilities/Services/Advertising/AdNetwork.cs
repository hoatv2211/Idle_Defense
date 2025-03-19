using UnityEngine;
using System.Collections;

namespace Utilities.Service.Ads
{
    /// <summary>
    /// List of all supported ad networks
    /// </summary>
    public enum AdNetwork
    {
        None,
        AdMob,
        AudienceNetwork,
        TapJoy,
        UnityAds,
        IronSource,
    }

    public enum BannerAdNetwork
    {
        None = AdNetwork.None,
        AdMob = AdNetwork.AdMob,
        AudienceNetwork = AdNetwork.AudienceNetwork,
        IronSource = AdNetwork.IronSource,
    }

    public enum InterstitialAdNetwork
    {
        None = AdNetwork.None,
        AdMob = AdNetwork.AdMob,
        AudienceNetwork = AdNetwork.AudienceNetwork,
        TapJoy = AdNetwork.TapJoy,
        UnityAds = AdNetwork.UnityAds,
        IronSource = AdNetwork.IronSource,
    }

    public enum RewardedAdNetwork
    {
        None = AdNetwork.None,
        AdMob = AdNetwork.AdMob,
        AudienceNetwork = AdNetwork.AudienceNetwork,
        TapJoy = AdNetwork.TapJoy,
        UnityAds = AdNetwork.UnityAds,
        IronSource = AdNetwork.IronSource,
    }
}