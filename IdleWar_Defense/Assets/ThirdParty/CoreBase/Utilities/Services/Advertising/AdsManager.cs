#pragma warning disable 0649
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utilities.Service.Ads
{
	public class AdsManager : MonoBehaviour
	{
		private static AdsManager mInstance;
		public static AdsManager Instance
		{
			get
			{
				if (mInstance == null)
				{
					mInstance = FindObjectOfType<AdsManager>();
					if (mInstance == null)
					{
						var obj = new GameObject("Advertising");
						mInstance = obj.AddComponent<AdsManager>();
						DontDestroyOnLoad(obj);
					}
				}
				return mInstance;
			}
		}

		[SerializeField] private bool mAutoInit = true;

		[Header("Android Default Network")]
		[SerializeField] private BannerAdNetwork m_AndroidDefaultBannerAdNetwork = BannerAdNetwork.None;
		[SerializeField] private InterstitialAdNetwork m_AndroidDefaultInterstitialAdNetwork = InterstitialAdNetwork.UnityAds;
		[SerializeField] private RewardedAdNetwork m_AndroidDefaultRewardedAdNetwork = RewardedAdNetwork.UnityAds;

		[Header("IOS Default Network")]
		[SerializeField] private BannerAdNetwork m_IOSDefaultBannerAdNetwork = BannerAdNetwork.None;
		[SerializeField] private InterstitialAdNetwork m_IOSDefaultInterstitialAdNetwork = InterstitialAdNetwork.UnityAds;
		[SerializeField] private RewardedAdNetwork m_IOSDefaultRewardedAdNetwork = RewardedAdNetwork.UnityAds;

#if !ACTIVE_ADMOB
		[HideInInspector]
#endif
		[Header("Admob"), SerializeField] private AdMobSettings m_AdMobSettings;
#if !UNITY_MONETIZATION
		[HideInInspector]
#endif
		[Header("Unity Ads"), SerializeField] private UnityAdsSettings m_UnityAdSettings;
#if !ACTIVE_FBAN
		[HideInInspector]
#endif
		[Header("FAN"), SerializeField] private AudienceNetworkSettings m_AudienceNetworkSettings;
#if !ACTIVE_TAPJOY
		[HideInInspector]
#endif
		[Header("TapJoy"), SerializeField] private TapjoySettings m_TapJoySettings;
#if !ACTIVE_IRONSOURCE
        [HideInInspector]
#endif
		[Header("IronSource"), SerializeField] private IronSourceSettings m_IronSourceSettings;
		[Space(10)]
		[SerializeField, Tooltip("Hours"), Range(1, 24)] private int m_ResetAdsWeightAfter = 12;

		// Supported ad clients.
		private AdMobClient m_AdMobClient;
		private AudienceNetworkClient m_AudienceNetworkClient;
		private TapjoyClient m_TapjoyClient;
		private UnityAdsClient m_UnityAdsClient;
		private IronSourceClient m_IronSourceClient;

		// Default ad clients for each ad types.
		private AdClient m_BannerAdClient;
		private AdClient m_InterstitialAdClient;
		private AdClient m_RewardedAdClient;

		#region Ad Events

		public event Action<InterstitialAdNetwork> onInterstitialAdCompleted;
		public event Action<RewardedAdNetwork> onRewardedAdSkipped;
		public event Action<RewardedAdNetwork> onRewardedAdCompleted;

		#endregion

		#region Ad Clients

		public AdMobClient AdMobClient
		{
			get
			{
				if (m_AdMobClient == null)
				{
					m_AdMobClient = CreateAdClient(AdNetwork.AdMob) as AdMobClient;
					m_AdMobClient.Init(Instance.m_AdMobSettings);
				}
				return m_AdMobClient;
			}
		}
		public AudienceNetworkClient AudienceNetworkClient
		{
			get
			{
				if (m_AudienceNetworkClient == null)
				{
					m_AudienceNetworkClient = CreateAdClient(AdNetwork.AudienceNetwork) as AudienceNetworkClient;
					m_AudienceNetworkClient.Init(Instance.m_AudienceNetworkSettings);
				}
				return m_AudienceNetworkClient;
			}
		}
		public TapjoyClient TapjoyClient
		{
			get
			{
				if (m_TapjoyClient == null)
				{
					m_TapjoyClient = CreateAdClient(AdNetwork.TapJoy) as TapjoyClient;
					m_TapjoyClient.Init(Instance.m_TapJoySettings);
				}
				return m_TapjoyClient;
			}
		}
		public UnityAdsClient UnityAdsClient
		{
			get
			{
				if (m_UnityAdsClient == null)
				{
					m_UnityAdsClient = CreateAdClient(AdNetwork.UnityAds) as UnityAdsClient;
					m_UnityAdsClient.Init(Instance.m_UnityAdSettings);
				}
				return m_UnityAdsClient;
			}
		}
		public IronSourceClient IronSourceClient
		{
			get
			{
				if (m_IronSourceClient == null)
				{
					m_IronSourceClient = CreateAdClient(AdNetwork.IronSource) as IronSourceClient;
					m_IronSourceClient.Init(Instance.m_IronSourceSettings);
				}
				return m_IronSourceClient;
			}
		}

		private AdClient DefaultBannerAdClient
		{
			get
			{
				if (m_BannerAdClient == null)
				{
					switch (Application.platform)
					{
						case RuntimePlatform.Android:
							m_BannerAdClient = GetWorkableAdClient((AdNetwork)Instance.m_AndroidDefaultBannerAdNetwork);
							break;
						case RuntimePlatform.IPhonePlayer:
							m_BannerAdClient = GetWorkableAdClient((AdNetwork)Instance.m_IOSDefaultBannerAdNetwork);
							break;
						default:
							m_BannerAdClient = GetWorkableAdClient(AdNetwork.None);
							break;
					}
				}
				return m_BannerAdClient;
			}
		}
		private AdClient DefaultInterstitialAdClient
		{
			get
			{
				if (m_InterstitialAdClient == null)
				{
					switch (Application.platform)
					{
						case RuntimePlatform.Android:
							m_InterstitialAdClient = GetWorkableAdClient((AdNetwork)Instance.m_AndroidDefaultInterstitialAdNetwork);
							break;
						case RuntimePlatform.IPhonePlayer:
							m_InterstitialAdClient = GetWorkableAdClient((AdNetwork)Instance.m_IOSDefaultInterstitialAdNetwork);
							break;
						case RuntimePlatform.WindowsEditor:
						case RuntimePlatform.OSXEditor:
							m_InterstitialAdClient = GetWorkableAdClient(AdNetwork.UnityAds);
							break;
						default:
							m_InterstitialAdClient = GetWorkableAdClient(AdNetwork.None);
							break;
					}
				}
				return m_InterstitialAdClient;
			}
		}
		private AdClient DefaultRewardedAdClient
		{
			get
			{
				if (m_RewardedAdClient == null)
				{
					switch (Application.platform)
					{
						case RuntimePlatform.Android:
							m_RewardedAdClient = GetWorkableAdClient((AdNetwork)Instance.m_AndroidDefaultRewardedAdNetwork);
							break;
						case RuntimePlatform.IPhonePlayer:
							m_RewardedAdClient = GetWorkableAdClient((AdNetwork)Instance.m_IOSDefaultRewardedAdNetwork);
							break;
						case RuntimePlatform.WindowsEditor:
						case RuntimePlatform.OSXEditor:
							m_RewardedAdClient = GetWorkableAdClient(AdNetwork.UnityAds);
							break;
						default:
							m_RewardedAdClient = GetWorkableAdClient(AdNetwork.None);
							break;
					}
				}
				return m_RewardedAdClient;
			}
		}

		#endregion

		#region MonoBehaviour Events

		private void Awake()
		{
			if (mInstance == null)
			{
				mInstance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (mInstance != this)
				Destroy(this);
		}

		private void Start()
		{
			if (mAutoInit)
				Init();
		}

		#endregion

		#region Ads API

		//------------------------------------------------------------
		// Banner Ads.
		//------------------------------------------------------------

		public void Init()
		{
			// Set the flag as true
#if UNITY_IOS
            AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
#endif

#if ACTIVE_ADMOB
            m_AdMobClient = CreateAdClient(AdNetwork.AdMob) as AdMobClient;
            m_AdMobClient.Init(Instance.m_AdMobSettings);
#endif
#if ACTIVE_FBAN && !UNITY_EDITOR
            m_AudienceNetworkClient = CreateAdClient(AdNetwork.AudienceNetwork) as AudienceNetworkClient;
            m_AudienceNetworkClient.Init(Instance.m_AudienceNetworkSettings);
#endif
#if ACTIVE_TAPJOY
            m_TapjoyClient = CreateAdClient(AdNetwork.TapJoy) as TapjoyClient;
            m_TapjoyClient.Init(Instance.m_TapJoySettings);
#endif
#if UNITY_MONETIZATION
            m_UnityAdsClient = CreateAdClient(AdNetwork.UnityAds) as UnityAdsClient;
            m_UnityAdsClient.Init(Instance.m_UnityAdSettings);
#endif
#if ACTIVE_IRONSOURCE
			m_IronSourceClient = CreateAdClient(AdNetwork.IronSource) as IronSourceClient;
			m_IronSourceClient.Init(Instance.m_IronSourceSettings);
#endif
			RuntimeHelper.Init();
			CheckToResetAdsWeight();
			StartCoroutine(IERAutoLoadAllAds(0f, 20f));
		}

		public void SetUnityAdsSettings(UnityAdsSettings pSettings)
		{
			m_UnityAdSettings = pSettings;
		}

		public void SetAdMobSettings(AdMobSettings pSettings)
		{
			m_AdMobSettings = pSettings;
		}

		public void SetTapJoySettings(TapjoySettings pSettings)
		{
			m_TapJoySettings = pSettings;
		}

		public void SetAudienceNetworkSettings(AudienceNetworkSettings pSettings)
		{
			m_AudienceNetworkSettings = pSettings;
		}

		public void SetIronSourceSettings(IronSourceSettings pSettings)
		{
			m_IronSourceSettings = pSettings;
		}

		/// <summary>
		/// Shows the default banner ad at the specified position.
		/// </summary>
		public void ShowBannerAd(BannerAdPosition position)
		{
			ShowBannerAd(DefaultBannerAdClient, position, BannerAdSize.SmartBanner);
		}

		/// <summary>
		/// Shows a banner ad of the default banner ad network
		/// at the specified position and size.
		/// </summary>
		public void ShowBannerAd(BannerAdPosition position, BannerAdSize size)
		{
			ShowBannerAd(DefaultBannerAdClient, position, size);
		}

		/// <summary>
		/// Shows a banner ad of the specified ad network at the specified position and size.
		/// </summary>
		public void ShowBannerAd(BannerAdNetwork adNetwork, BannerAdPosition position, BannerAdSize size)
		{
			ShowBannerAd(GetWorkableAdClient((AdNetwork)adNetwork), position, size);
		}

		/// <summary>
		/// Hides the default banner ad.
		/// </summary>
		public void HideBannerAd()
		{
			HideBannerAd(DefaultBannerAdClient);
		}

		/// <summary>
		/// Hides the banner ad of the specified ad network .
		/// </summary>
		public void HideBannerAd(BannerAdNetwork adNetwork)
		{
			HideBannerAd(GetWorkableAdClient((AdNetwork)adNetwork));
		}

		public void ClearInterAdsCallback()
		{
			onInterstitialAdCompleted = null;
		}

		/// <summary>
		/// Destroys the default banner ad.
		/// </summary>
		public void DestroyBannerAd()
		{
			DestroyBannerAd(DefaultBannerAdClient);
		}

		/// <summary>
		/// Destroys the banner ad of the specified ad network .
		/// </summary>
		public void DestroyBannerAd(BannerAdNetwork adNetwork)
		{
			DestroyBannerAd(GetWorkableAdClient((AdNetwork)adNetwork));
		}

		//------------------------------------------------------------
		// Interstitial Ads.
		//------------------------------------------------------------

		/// <summary>
		/// Loads the default interstitial ad.
		/// </summary>
		public void LoadInterstitialAd()
		{
			LoadInterstitialAd(DefaultInterstitialAdClient);
		}

		/// <summary>
		/// Loads the interstitial ad of the specified ad network .
		/// </summary>
		public void LoadInterstitialAd(InterstitialAdNetwork adNetwork)
		{
			LoadInterstitialAd(GetWorkableAdClient((AdNetwork)adNetwork));
		}

		/// <summary>
		/// Determines whether the default interstitial ad is ready to show.
		/// </summary>
		public bool IsInterstitialAdReady()
		{
			return IsInterstitialAdReady(DefaultInterstitialAdClient);
		}

		/// <summary>
		/// Determines whether the interstitial ad of the specified ad network 
		///  is ready to show.
		/// </summary>
		public bool IsInterstitialAdReady(InterstitialAdNetwork adNetwork)
		{
			return IsInterstitialAdReady(GetWorkableAdClient((AdNetwork)adNetwork));
		}

		public bool IsAnyInterstitialAdReady()
		{
			if (m_AdMobClient != null && m_AdMobClient.IsInterstitialAdReady())
				return true;
			if (m_AudienceNetworkClient != null && m_AudienceNetworkClient.IsInterstitialAdReady())
				return true;
			if (m_IronSourceClient != null && m_IronSourceClient.IsInterstitialAdReady())
				return true;
			if (m_TapjoyClient != null && m_TapjoyClient.IsInterstitialAdReady())
				return true;
			if (m_UnityAdsClient != null && m_UnityAdsClient.IsInterstitialAdReady())
				return true;
			return false;
		}

		/// <summary>
		/// Shows the default interstitial ad.
		/// </summary>
		public void ShowInterstitialAd(string id)
		{
			ShowInterstitialAd(id, DefaultInterstitialAdClient);
		}

		/// <summary>
		/// Shows the interstitial ad of the specified ad network.
		/// </summary>
		public void ShowInterstitialAd(string id, InterstitialAdNetwork adNetwork)
		{
			ShowInterstitialAd(id, GetWorkableAdClient((AdNetwork)adNetwork));
		}

		/// <summary>
		/// Show random interstitial ad, depend on the weight number of each network type
		/// </summary>
		public InterstitialAdNetwork ShowInterstitialAdRandomly(string id)
		{
			var networkChances = new List<int>();
			var networks = new List<InterstitialAdNetwork>();
			if (m_AdMobClient != null && m_AdMobClient.IsInterstitialAdReady())
			{
				int weight = m_AdMobSettings.CurInterstitialAdWeight * 100;
				weight = weight <= 0 ? 10 : weight;
				networkChances.Add(weight);
				networks.Add(InterstitialAdNetwork.AdMob);
			}
			if (m_AudienceNetworkClient != null && m_AudienceNetworkClient.IsInterstitialAdReady())
			{
				int weight = m_AudienceNetworkSettings.CurInterstitialAdWeight * 100;
				weight = weight <= 0 ? 10 : weight;
				networkChances.Add(weight);
				networks.Add(InterstitialAdNetwork.AudienceNetwork);
			}
			if (m_IronSourceClient != null && m_IronSourceClient.IsInterstitialAdReady())
			{
				int weight = m_IronSourceSettings.CurInterstitialAdWeight * 100;
				weight = weight <= 0 ? 10 : weight;
				networkChances.Add(weight);
				networks.Add(InterstitialAdNetwork.IronSource);
			}
			if (m_TapjoyClient != null && m_TapjoyClient.IsInterstitialAdReady())
			{
				int weight = m_TapJoySettings.CurInterstitialAdWeight * 100;
				weight = weight <= 0 ? 10 : weight;
				networkChances.Add(weight);
				networks.Add(InterstitialAdNetwork.TapJoy);
			}
			if (m_UnityAdsClient != null && m_UnityAdsClient.IsInterstitialAdReady())
			{
				int weight = m_UnityAdSettings.CurInterstitialAdWeight * 100;
				weight = weight <= 0 ? 10 : weight;
				networkChances.Add(weight);
				networks.Add(InterstitialAdNetwork.UnityAds);
			}
			if (networkChances.Count == 0)
				return InterstitialAdNetwork.None;

			int totalRatio = 0;
			for (int i = 0; i < networkChances.Count; i++)
				totalRatio += networkChances[i];

			int selected = -1;
			int random = Random.Range(0, totalRatio);
			int temp = 0;
			for (int i = 0; i < networkChances.Count; i++)
			{
				temp += networkChances[i];
				if (temp > random)
				{
					selected = i;
					break;
				}
			}
			var network = InterstitialAdNetwork.None;
			if (selected > -1)
				network = networks[selected];

			ShowInterstitialAd(id, network);
			return network;
		}

		//------------------------------------------------------------
		// Rewarded Ads.
		//------------------------------------------------------------

		/// <summary>
		/// Loads the default rewarded ad.
		/// </summary>
		public void LoadRewardedAd()
		{
			LoadRewardedAd(DefaultRewardedAdClient);
		}

		/// <summary>
		/// Loads the rewarded ad of the specified ad network.
		/// </summary>
		public void LoadRewardedAd(RewardedAdNetwork adNetwork)
		{
			LoadRewardedAd(GetWorkableAdClient((AdNetwork)adNetwork));
		}

		/// <summary>
		/// Determines whether the default rewarded ad is ready to show.
		/// </summary>
		public bool IsRewardedAdReady()
		{
			return IsRewardedAdReady(DefaultRewardedAdClient);
		}

		public bool IsRewardedAdReady(RewardedAdNetwork adNetwork)
		{
			return IsRewardedAdReady(GetWorkableAdClient((AdNetwork)adNetwork));
		}

		public bool IsAnyRewardedAdReady()
		{
			if (m_AdMobClient != null && m_AdMobClient.IsRewardedAdReady())
				return true;
			if (m_AudienceNetworkClient != null && m_AudienceNetworkClient.IsRewardedAdReady())
				return true;
			if (m_IronSourceClient != null && m_IronSourceClient.IsRewardedAdReady())
				return true;
			if (m_TapjoyClient != null && m_TapjoyClient.IsRewardedAdReady())
				return true;
			if (m_UnityAdsClient != null && m_UnityAdsClient.IsRewardedAdReady())
				return true;
			return false;
		}

		/// <summary>
		/// Shows the default rewarded ad.
		/// </summary>
		public void ShowRewardedAd(string id)
		{
			ShowRewardedAd(id, DefaultRewardedAdClient);
		}

		/// <summary>
		/// Shows the rewarded ad of the specified ad network .
		/// </summary>
		public void ShowRewardedAd(string id, RewardedAdNetwork adNetwork)
		{
			ShowRewardedAd(id, GetWorkableAdClient((AdNetwork)adNetwork));
		}

		/// <summary>
		/// Show random interstitial ad, depend on the weight number of each network type
		/// </summary>
		public RewardedAdNetwork ShowRewardedAdRandomly(string id)
		{
			var networkChances = new List<int>();
			var networks = new List<RewardedAdNetwork>();
			if (m_AdMobClient != null && m_AdMobClient.IsRewardedAdReady())
			{
				int weight = m_AdMobSettings.CurRewardedAdWeight * 100;
				weight = weight <= 0 ? 10 : weight;
				networkChances.Add(weight);
				networks.Add(RewardedAdNetwork.AdMob);
			}
			if (m_AudienceNetworkClient != null && m_AudienceNetworkClient.IsRewardedAdReady())
			{
				int weight = m_AudienceNetworkSettings.CurRewardedAdWeight * 100;
				weight = weight <= 0 ? 10 : weight;
				networkChances.Add(weight);
				networks.Add(RewardedAdNetwork.AudienceNetwork);
			}
			if (m_IronSourceClient != null && m_IronSourceClient.IsRewardedAdReady())
			{
				int weight = m_IronSourceSettings.CurRewardedAdWeight * 100;
				weight = weight <= 0 ? 10 : weight;
				networkChances.Add(weight);
				networks.Add(RewardedAdNetwork.IronSource);
			}
			if (m_TapjoyClient != null && m_TapjoyClient.IsRewardedAdReady())
			{
				int weight = m_TapJoySettings.CurRewardedAdWeight * 100;
				weight = weight <= 0 ? 10 : weight;
				networkChances.Add(weight);
				networks.Add(RewardedAdNetwork.TapJoy);
			}
			if (m_UnityAdsClient != null && m_UnityAdsClient.IsRewardedAdReady())
			{
				int weight = m_UnityAdSettings.CurRewardedAdWeight * 100;
				weight = weight <= 0 ? 10 : weight;
				networkChances.Add(weight);
				networks.Add(RewardedAdNetwork.UnityAds);
			}
			if (networkChances.Count == 0)
				return RewardedAdNetwork.None;

			int totalRatio = 0;
			for (int i = 0; i < networkChances.Count; i++)
				totalRatio += networkChances[i];

			int selected = -1;
			int random = Random.Range(0, totalRatio);
			int temp = 0;
			for (int i = 0; i < networkChances.Count; i++)
			{
				temp += networkChances[i];
				if (temp > random)
				{
					selected = i;
					break;
				}
			}
			var network = RewardedAdNetwork.None;
			if (selected > -1)
				network = networks[selected];

			ShowRewardedAd(id, network);
			return network;
		}

		#endregion // Public API

		#region Internal Stuff

		private void CheckToResetAdsWeight()
		{
			string lastDateString = PlayerPrefs.GetString("lastDateOfAds");
			DateTime lateDate = DateTime.MinValue;
			DateTime.TryParse(lastDateString, out lateDate);

			if ((DateTime.Now - lateDate).TotalHours > m_ResetAdsWeightAfter)
			{
				m_AdMobSettings.ResetCurWeight();
				m_AudienceNetworkSettings.ResetCurWeight();
				m_IronSourceSettings.ResetCurWeight();
				m_TapJoySettings.ResetCurWeight();
				m_UnityAdSettings.ResetCurWeight();
				PlayerPrefs.SetString("lastDateOfAds", DateTime.Now.ToString());
			}
		}

		private void ShowBannerAd(IAdClient client, BannerAdPosition position, BannerAdSize size)
		{
			client.ShowBannerAd(position, size);
		}

		private void HideBannerAd(IAdClient client)
		{
			client.HideBannerAd();
		}

		private void DestroyBannerAd(IAdClient client)
		{
			client.DestroyBannerAd();
		}

		private void LoadInterstitialAd(IAdClient client)
		{
			client.LoadInterstitialAd();
		}

		private bool IsInterstitialAdReady(IAdClient client)
		{
			return client.IsInterstitialAdReady();
		}

		private void ShowInterstitialAd(string id, IAdClient client)
		{
			client.ShowInterstitialAd(id);
		}

		// Note that rewarded ads should still be available after ads removal.
		// which is why we don't check if ads were removed in the following methods.

		private void LoadRewardedAd(IAdClient client)
		{
			client.LoadRewardedAd();
		}

		private bool IsRewardedAdReady(IAdClient client)
		{
			return client.IsRewardedAdReady();
		}

		private void ShowRewardedAd(string id, IAdClient client)
		{
			Config.LogEvent(id);
			client.ShowRewardedAd(id);
		}

		/// <summary>
		/// Grabs the singleton ad client for the specified network and performs
		/// necessary setup for it, including initializing it and subscribing to its events.
		/// </summary>
		private AdClient CreateAdClient(AdNetwork network)
		{
			AdClient client = null;

			switch (network)
			{
				case AdNetwork.AdMob:
					client = AdMobClient.CreateClient(); break;
				case AdNetwork.AudienceNetwork:
					client = AudienceNetworkClient.CreateClient(); break;
				case AdNetwork.TapJoy:
					client = TapjoyClient.CreateClient(); break;
				case AdNetwork.UnityAds:
					client = UnityAdsClient.CreateClient(); break;
				case AdNetwork.None:
					client = NoOpClient.CreateClient(); break;
				case AdNetwork.IronSource:
					client = IronSourceClient.CreateClient(); break;
				default:
					throw new NotImplementedException("No client implemented for the network:" + network.ToString());
			}

			if (client != null && client.Network != AdNetwork.None)
				SubscribeAdClientEvents(client);

			return client;
		}

		/// <summary>
		/// Grabs the ready to work (done initialization, setup, etc.)
		/// ad client for the specified network.
		/// </summary>
		private AdClient GetWorkableAdClient(AdNetwork network)
		{
			switch (network)
			{
				case AdNetwork.AdMob:
					return AdMobClient;
				case AdNetwork.AudienceNetwork:
					return AudienceNetworkClient;
				case AdNetwork.UnityAds:
					return UnityAdsClient;
				case AdNetwork.TapJoy:
					return TapjoyClient;
				case AdNetwork.IronSource:
					return IronSourceClient;
				case AdNetwork.None:
					return NoOpClient.CreateClient();
				default:
					throw new NotImplementedException("No client found for the network:" + network.ToString());
			}
		}

		private void SubscribeAdClientEvents(IAdClient client)
		{
			if (client == null)
				return;

			client.InterstitialAdCompleted += OnInternalInterstitialAdCompleted;
			client.RewardedAdSkipped += OnInternalRewardedAdSkipped;
			client.RewardedAdCompleted += OnInternalRewardedAdCompleted;
		}

		private void OnInternalInterstitialAdCompleted(IAdClient client)
		{
			if (onInterstitialAdCompleted != null)
				onInterstitialAdCompleted((InterstitialAdNetwork)client.Network);

			switch ((InterstitialAdNetwork)client.Network)
			{
				case InterstitialAdNetwork.AdMob:
					m_AdMobSettings.CurInterstitialAdWeight -= 1;
					break;
				case InterstitialAdNetwork.AudienceNetwork:
					m_AudienceNetworkSettings.CurInterstitialAdWeight -= 1;
					break;
				case InterstitialAdNetwork.IronSource:
					m_IronSourceSettings.CurInterstitialAdWeight -= 1;
					break;
				case InterstitialAdNetwork.TapJoy:
					m_TapJoySettings.CurInterstitialAdWeight -= 1;
					break;
				case InterstitialAdNetwork.UnityAds:
					m_UnityAdSettings.CurInterstitialAdWeight -= 1;
					break;
			}
		}

		private void OnInternalRewardedAdSkipped(IAdClient client)
		{
			if (onRewardedAdSkipped != null)
				onRewardedAdSkipped((RewardedAdNetwork)client.Network);
		}

		private void OnInternalRewardedAdCompleted(IAdClient client)
		{
			if (onRewardedAdCompleted != null)
				onRewardedAdCompleted((RewardedAdNetwork)client.Network);

			switch ((RewardedAdNetwork)client.Network)
			{
				case RewardedAdNetwork.AdMob:
					m_AdMobSettings.CurRewardedAdWeight -= 1;
					break;
				case RewardedAdNetwork.AudienceNetwork:
					m_AudienceNetworkSettings.CurRewardedAdWeight -= 1;
					break;
				case RewardedAdNetwork.IronSource:
					m_IronSourceSettings.CurRewardedAdWeight -= 1;
					break;
				case RewardedAdNetwork.TapJoy:
					m_TapJoySettings.CurRewardedAdWeight -= 1;
					break;
				case RewardedAdNetwork.UnityAds:
					m_UnityAdSettings.CurRewardedAdWeight -= 1;
					break;
			}
		}

		/// <summary>
		/// This now just load all available ads (default and custom) of all imported networks.
		/// </summary>
		public void ForceLoadAllAds()
		{
			List<IAdClient> availableInterstitialNetworks = GetAvailableNetworks<InterstitialAdNetwork>();
			List<IAdClient> availableRewardedNetworks = GetAvailableNetworks<RewardedAdNetwork>();

			LoadAllInterstitialAds(availableInterstitialNetworks);
			LoadAllRewardedAds(availableRewardedNetworks);
		}

		/// <summary>
		/// This coroutine load all available ads (default and custom) of all imported networks.
		/// </summary>
		private IEnumerator IERAutoLoadAllAds(float delay = 0, float interval = 20)
		{
			if (delay > 0)
				yield return new WaitForSeconds(delay);
			else
				yield return null;

			List<IAdClient> availableInterstitialNetworks = GetAvailableNetworks<InterstitialAdNetwork>();
			List<IAdClient> availableRewardedNetworks = GetAvailableNetworks<RewardedAdNetwork>();

			while (true)
			{
				LoadAllInterstitialAds(availableInterstitialNetworks);
				LoadAllRewardedAds(availableRewardedNetworks);
				yield return new WaitForSeconds(interval);
			}
		}

		/// <summary>
		/// Load all available interstitial ads of specific clients.
		/// </summary>
		private void LoadAllInterstitialAds(List<IAdClient> clients)
		{
			foreach (var client in clients)
			{
				if (!client.IsValid(AdType.Interstitial))
					continue;

				string tempIndex = client.Network.ToString();

				if (IsInterstitialAdReady(client))
					continue;

				LoadInterstitialAd(client);
			}
		}

		/// <summary>
		/// Load all available rewarded ads of specific clients. 
		/// </summary>
		private void LoadAllRewardedAds(List<IAdClient> clients)
		{
			foreach (var client in clients)
			{
				if (!client.IsValid(AdType.Rewarded))
					continue;

				string tempIndex = client.Network.ToString();

				if (IsRewardedAdReady(client))
					continue;

				LoadRewardedAd(client);
			}
		}

		/// <summary>
		/// Returns all imported ads networks.
		/// </summary>
		private List<IAdClient> GetAvailableNetworks<T>()
		{
			List<IAdClient> availableNetworks = new List<IAdClient>();
			foreach (T network in Enum.GetValues(typeof(T)))
			{
				AdClient client = GetAdClient((AdNetwork)(Enum.Parse(typeof(T), network.ToString())));
				if (client.IsSdkAvail)
				{
					var workableClient = GetWorkableAdClient((AdNetwork)(Enum.Parse(typeof(T), network.ToString())));
					availableNetworks.Add(workableClient);
				}
			}
			return availableNetworks;
		}

		/// <summary>
		/// Gets the singleton ad client of the specified network.
		/// This may or may not be initialized.
		/// </summary>
		private AdClient GetAdClient(AdNetwork network)
		{
			switch (network)
			{
				case AdNetwork.AdMob:
					return AdMobClient;
				case AdNetwork.AudienceNetwork:
					return AudienceNetworkClient;
				case AdNetwork.TapJoy:
					return TapjoyClient;
				case AdNetwork.UnityAds:
					return UnityAdsClient;
				case AdNetwork.IronSource:
					return IronSourceClient;
				case AdNetwork.None:
					return NoOpClient.CreateClient();
				default:
					throw new NotImplementedException("No client implemented for the network:" + network.ToString());
			}
		}

		#endregion

#if UNITY_EDITOR

		#region EDitor

		[CustomEditor(typeof(AdsManager))]
		public class AdvertisingEditor : Editor
		{
			private void RemoveDirective(string pSymbol)
			{
				var taget = EditorUserBuildSettings.selectedBuildTargetGroup;
				string directives = PlayerSettings.GetScriptingDefineSymbolsForGroup(taget);
				directives = directives.Replace(pSymbol, "");
				if (directives.Length > 0 && directives[directives.Length - 1] == ';')
					directives = directives.Remove(directives.Length - 1, 1);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(taget, directives);
			}

			private void AddDirective(string pSymbol)
			{
				var taget = EditorUserBuildSettings.selectedBuildTargetGroup;
				string directives = PlayerSettings.GetScriptingDefineSymbolsForGroup(taget);
				if (string.IsNullOrEmpty(directives))
					directives += pSymbol;
				else
					directives += ";" + pSymbol;
				PlayerSettings.SetScriptingDefineSymbolsForGroup(taget, directives);
			}

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				Color defaultColor = GUI.backgroundColor;

				GUILayout.BeginVertical("box");

				if (GUILayout.Button("Download Admob Plugin")) Application.OpenURL("https://github.com/googleads/googleads-mobile-unity/releases");
#if ACTIVE_ADMOB
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Disable Admob")) RemoveDirective("ACTIVE_ADMOB");
#else
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Enable Admob")) AddDirective("ACTIVE_ADMOB");
#endif
				GUI.backgroundColor = defaultColor;
				GUILayout.Space(5);
				if (GUILayout.Button("Download TapJoy Plugin")) Application.OpenURL("https://ltv.tapjoy.com/d/sdks");
#if ACTIVE_TAPJOY
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Disable Tapjoy")) RemoveDirective("ACTIVE_TAPJOY");
#else
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Enable Tapjoy")) AddDirective("ACTIVE_TAPJOY");
#endif
				GUI.backgroundColor = defaultColor;
				GUILayout.Space(5);
				if (GUILayout.Button("Download Audience Network Plugin")) Application.OpenURL("https://developers.facebook.com/docs/audience-network/download#unity");
#if ACTIVE_FBAN
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Disable Audience Network")) RemoveDirective("ACTIVE_FBAN");
#else
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Enable Audience Network")) AddDirective("ACTIVE_FBAN");
#endif
				GUI.backgroundColor = defaultColor;
				GUILayout.Space(5);
				if (GUILayout.Button("Download IronSource Plugin")) Application.OpenURL("https://developers.ironsrc.com/ironsource-mobile/unity/unity-plugin/#step-1");
#if ACTIVE_IRONSOURCE
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Disable IronSource")) RemoveDirective("ACTIVE_IRONSOURCE");
#else
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Enable IronSource")) AddDirective("ACTIVE_IRONSOURCE");
#endif
				GUI.backgroundColor = defaultColor;
				GUILayout.Space(5);
				var style = new GUIStyle(EditorStyles.boldLabel);
				style.alignment = TextAnchor.MiddleCenter;
#if UNITY_MONETIZATION
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Disable Unity Ads")) RemoveDirective("UNITY_MONETIZATION");
#else
				EditorGUILayout.HelpBox("Install Unity Ads from Package Manager", MessageType.Info);
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Enable Unity Ads")) AddDirective("UNITY_MONETIZATION");
#endif
				GUI.backgroundColor = defaultColor;
				GUILayout.EndVertical();

			}
		}
		#endregion

#endif
	}
}