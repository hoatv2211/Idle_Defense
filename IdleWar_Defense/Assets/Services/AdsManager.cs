using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AdsManager : Singleton<AdsManager>
{
    [SerializeField] private string _adBanner = "ca-app-pub-1367611331592257/7816143392";
    [SerializeField] private string _adInter = "ca-app-pub-1367611331592257/6746604300";
    [SerializeField] private string _adReward = "ca-app-pub-1367611331592257/5612683700";


    private void Start()
    {
        //LoadAd_Banner();
        LoadAd_Inter();
        LoadAd_Reward();

        //DOVirtual.DelayedCall(15,ShowAd_Banner).SetLoops(20);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowAd_Inter();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowAd_Reward(() => { Debug.LogError("Done"); });
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ShowAd_Banner();
        }
    }

    #region Banner
    /// <summary>
    /// UI element activated when an ad is ready to show.
    /// </summary>
    //public GameObject AdLoadedStatus_Banner;

    private BannerView _bannerView;

    /// <summary>
    /// Creates a 320x50 banner at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view.");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyAd_Banner();
        }

        // Create a 320x50 banner at top of the screen.
        _bannerView = new BannerView(_adBanner, AdSize.Banner, AdPosition.Bottom);

        // Listen to events the banner may raise.
        ListenToAdEvents_Banner();

        Debug.Log("Banner view created.");


        LoadAd_Banner();
    }

    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadAd_Banner()
    {
        // Create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowAd_Banner()
    {
        if (_bannerView != null)
        {
            Debug.Log("Showing banner view.");
            _bannerView.Show();
        }
        else
        {
            LoadAd_Banner();
        }
    }

    /// <summary>
    /// Hides the ad.
    /// </summary>
    public void HideAd_Banner()
    {
        if (_bannerView != null)
        {
            Debug.Log("Hiding banner view.");
            _bannerView.Hide();
        }
    }

    /// <summary>
    /// Destroys the ad.
    /// When you are finished with a BannerView, make sure to call
    /// the Destroy() method before dropping your reference to it.
    /// </summary>
    public void DestroyAd_Banner()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }

        // Inform the UI that the ad is not ready.
        //AdLoadedStatus_Banner?.SetActive(false);
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo_Banner()
    {
        if (_bannerView != null)
        {
            var responseInfo = _bannerView.GetResponseInfo();
            if (responseInfo != null)
            {
                UnityEngine.Debug.Log(responseInfo);
            }
        }
    }

    /// <summary>
    /// Listen to events the banner may raise.
    /// </summary>
    private void ListenToAdEvents_Banner()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
            ShowAd_Banner();
            LoadAd_Inter();
            LoadAd_Reward();
            // Inform the UI that the ad is ready.
            //AdLoadedStatus_Banner?.SetActive(true);
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    #endregion

    #region InterstitialAd
    private InterstitialAd _interstitialAd;

    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadAd_Inter()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            DestroyAd_Inter();
        }

        Debug.Log("Loading interstitial ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        InterstitialAd.Load(_adInter, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                return;
            }

            // The operation completed successfully.
            Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
            _interstitialAd = ad;

            // Register to ad events to extend functionality.
            RegisterEventHandlers_Inter(ad);

            // Inform the UI that the ad is ready.
            //AdLoadedStatus_Banner?.SetActive(true);
        });
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowAd_Inter()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
            LoadAd_Inter();
        }

        // Inform the UI that the ad is not ready.
        //AdLoadedStatus_Banner?.SetActive(false);
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd_Inter()
    {
        if (_interstitialAd != null)
        {
            Debug.Log("Destroying interstitial ad.");
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        // Inform the UI that the ad is not ready.
        //AdLoadedStatus_Banner?.SetActive(false);
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo_Inter()
    {
        if (_interstitialAd != null)
        {
            var responseInfo = _interstitialAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    private void RegisterEventHandlers_Inter(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            LoadAd_Inter();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error : "
                + error);
        };
    }

    #endregion

    #region Reward 
    private RewardedAd _rewardedAd;

    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadAd_Reward()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            DestroyAd_Reward();
        }

        Debug.Log("Loading rewarded ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        RewardedAd.Load(_adReward, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                return;
            }

            // The operation completed successfully.
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            _rewardedAd = ad;

            // Register to ad events to extend functionality.
            RegisterEventHandlers(ad);

            // Inform the UI that the ad is ready.
            //AdLoadedStatus?.SetActive(true);
        });
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public bool ShowAd_Reward(Action _Action = null)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            Debug.Log("Showing rewarded ad.");
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log(String.Format("Rewarded ad granted a reward: {0} {1}",
                                        reward.Amount,
                                        reward.Type));

                _Action?.Invoke();
            });

            return true;
        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");
            LoadAd_Reward();

            return false;
        }

        // Inform the UI that the ad is not ready.
        //AdLoadedStatus?.SetActive(false);
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd_Reward()
    {
        if (_rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        // Inform the UI that the ad is not ready.
        //AdLoadedStatus?.SetActive(false);
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo()
    {
        if (_rewardedAd != null)
        {
            var responseInfo = _rewardedAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            LoadAd_Reward();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content with error : "
                + error);
        };
    }


    #endregion

}
