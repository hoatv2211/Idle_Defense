using UnityEngine;
using Utilities.Service.RFirebase;
using Utilities.Services;
using System;
using FoodZombie;
using Utilities.Common;
using System.Collections.Generic;
using System.Globalization;

public class Config
{
	public static int MIN_LEVEL_INTERSTITIAL = 1005;
	public static double TIME_INTERSTITIAL_OPENING = 90;
	public static double TIME_INTERSTITIAL_AFTER_RV = 90;
	public static double TIME_INTERSTITIAL_AFTER_INTERSTITIAL = 90;

	public const float TIME_TUTORIAL_TEXT_RUN = 0.11f;
	public const float TIME_TUTORIAL_TEXT_WAIT = 0.65f;

	public static int typeModeInGame = 0;

	public const int TYPE_MODE_NORMAL = 0;
	public const int TYPE_MODE_DISCOVERY = 1;
	//public const int TYPE_MODE_HARD = 2;
	public const int TYPE_MODE_BONUS = 3;
	public const int TYPE_MODE_PvP = 4;
	//public static BlittableBool isHardMode = false;
	public static byte isHardMode = 0;

	//------------Gameplay----------------------
	public const string TAG_HERO = "Hero";
	public const string TAG_ENEMY = "Enemy";

	public const float enemyMoveThreshold = 0.6f;
	public const float xRange = 0.8f;
	public const float MAX_X = 7.5f * xRange; //12 / 2 - 1 => 10, trái phải 7.5 ô
	public const float LOWEST_Y = 9.5f * xRange; //24 / 2 - 2 => 10, 2 ô rồi mới đánh
	public const float HERO_ATTACK_RANGE_Y = 17f * xRange;
	public const float RANGE_MIN = 11f * xRange; //nếu có enemy đi vào vùng min này thì đánh con gần nhất

	public static SceneName backToHomePanel =SceneName.NONE;
	public static bool showAdsTrap = false;
	static PvPConfig _PvpConfig;
	public static PvPConfig PvpConfig
	{
		get
		{
			if (_PvpConfig == null) _PvpConfig = new PvPConfig();
			return _PvpConfig;
		}
	}
	/// <summary>
	/// 0 for unset,1 for win.-1 for lose
	/// </summary>
	public static int LastGameResult = 0;
	public static bool isLoseToday = false;
	//
	// get market link of app
	public static string GetMarketLink()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
        return "market://details?id=" + Application.identifier;
#elif UNITY_IOS && !UNITY_EDITOR
        return "itms-apps://itunes.apple.com/app/id" + Application.identifier;
#else
		return "https://www.google.com.vn/search?q=" + Application.identifier;
#endif
	}

	public static int GetVersionCode()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass contextCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject context = contextCls.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageMngr = context.Call<AndroidJavaObject>("getPackageManager");
        string packageName = context.Call<string>("getPackageName");
        AndroidJavaObject packageInfo = packageMngr.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
        return packageInfo.Get<int>("versionCode");
#elif UNITY_IOS && !UNITY_EDITOR
        return 0;
#else
		return 0;
#endif
	}

	public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
	{
		// Unix timestamp is seconds past epoch
		DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
		return dtDateTime;
	}

	//------Tracking------
	public static void LogScene(string SceneName)
	{

//#if DEVELOPMENT
//		Debug.LogError("<color=green>SCENE: </color>" + SceneName);
//#else
//if (RFirebaseManager.initialized)
//			FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventScreenView,
//			new Firebase.Analytics.Parameter(FirebaseAnalytics.ParameterScreenName, SceneName));
//#endif

	}
	public static void LogPurchaseEvent(float logPurchase, string currency)
	{
//#if !DEVELOPMENT
//        // FirebaseAnalytics.LogEvent(eventName);
//        if (RFirebaseManager.initialized) FBServices.LogPurchase(logPurchase, currency);

//        //AppsFlyer.trackRichEvent(pEventName, null);
//#endif

		UnityEngine.Debug.Log("LogPurchaseEvent: " + logPurchase + " " + currency);
	}

	public static void LogEvent(string eventName)
	{
//#if !DEVELOPMENT
//        if (RFirebaseManager.initialized) FirebaseAnalytics.LogEvent(eventName);
//        if (eventName.Contains("click_"))
//        {
//            if (RFirebaseManager.initialized) FirebaseAnalytics.SetUserProperty(TrackingConstants.USER_PROPERTY_LAST_FEATURE, eventName);
//        }
//        //FBServices.LogEvent(pEventName);

//        //AppsFlyer.trackRichEvent(pEventName, null);
//#endif

//		UnityEngine.Debug.Log("LogEvent: " + eventName);
	}

	public static void LogEvent(string eventName, string paramName, string paramValue)
	{
//#if !DEVELOPMENT
//        if (RFirebaseManager.initialized) FirebaseAnalytics.LogEvent(eventName, paramName, paramValue);
//        //FBServices.LogEvent(pEventName, pParamName, pParamValue);

//        //Dictionary<string, string> paramEvent = new Dictionary<string, string>();
//        //paramEvent.Add(pParamName, pParamValue);
//        //AppsFlyer.trackRichEvent(pEventName, paramEvent);
//#endif

//		UnityEngine.Debug.Log("LogEvent: " + eventName + "\nParam: " + paramName + " | " + paramValue);
	}

	public static void LogEvent(string eventName, string paramName, int paramValue)
	{
//		FirebaseAnalytics.LogEvent(eventName, paramName, paramValue);
//#if !DEVELOPMENT
//        if (RFirebaseManager.initialized) FirebaseAnalytics.LogEvent(eventName, paramName, paramValue);
//        //FBServices.LogEvent(pEventName, pParamName, pParamValue);

//        //Dictionary<string, string> paramEvent = new Dictionary<string, string>();
//        //paramEvent.Add(pParamName, pParamValue);
//        //AppsFlyer.trackRichEvent(pEventName, paramEvent);
//#endif

//		UnityEngine.Debug.Log("LogEvent: " + eventName + "\nParam: " + paramName + " | " + paramValue);
	}

	public static void LogEvent(string eventName, string paramName1, string paramValue1, string paramName2, int paramValue2)
	{
//#if !DEVELOPMENT
//        Parameter[] param =
//        {
//            new Parameter(paramName1, paramValue1),
//            new Parameter(paramName2, paramValue2)
//        };
//        if (RFirebaseManager.initialized) FirebaseAnalytics.LogEvent(eventName, param);
//        //FBServices.LogEvent(pEventName, pParamName, pParamValue);

//        //Dictionary<string, string> paramEvent = new Dictionary<string, string>();
//        //paramEvent.Add(pParamName, pParamValue);
//        //AppsFlyer.trackRichEvent(pEventName, paramEvent);
//#endif

//		UnityEngine.Debug.Log("LogEvent: " + eventName
//							  + "\nParam: " + paramName1 + " | " + paramValue1
//							  + "\nParam: " + paramName2 + " | " + paramValue2);
	}

	//-------------------

	private static System.Random mRandom = new System.Random();
	public static int EasyRandom(int range)
	{
		return mRandom.Next(range);
	}

	public static int EasyRandom(int min, int max)//không bao gồm max
	{
		return mRandom.Next(min, max);
	}

	public static float EasyRandom(float min, float max)
	{
		return UnityEngine.Random.RandomRange(min, max);
	}

	//code trên mạng
	public static float GetZangleFromTwoPosition(Vector3 fromPos, Vector3 toPos)
	{
		var xDistance = toPos.x - fromPos.x;
		var yDistance = toPos.y - fromPos.y;
		var angle = Mathf.Atan2(xDistance, yDistance) * Mathf.Rad2Deg;
		angle = -Get360Angle(angle);

		return angle + 90;
	}

	public static float Get360Angle(float angle)
	{
		while (angle < 0f)
		{
			angle += 360f;
		}
		while (360f < angle)
		{
			angle -= 360f;
		}
		return angle;
	}

	public static string CurrencyAndCostToString(int currency, int cost)
	{
		//cái mình đang có trên cái cần
		if (currency >= cost)
		{
			return "<color=#00FF00>" + currency + "</color>/" + cost;
		}
		else
		{
			return "<color=#FF0000>" + currency + "</color>/" + cost;
		}
	}

	public static string CurrencyAndCostToKKKString(int currency, int cost)
	{
		var stringCurrency = BigNumberAlpha.Create(currency).GetKKKString();
		var stringCost = BigNumberAlpha.Create(cost).GetKKKString();

		//cái mình đang có trên cái cần
		if (currency >= cost)
		{
			return "<color=#00FF00>" + stringCurrency + "</color>/" + stringCost;
		}
		else
		{
			return "<color=#FF0000>" + stringCurrency + "</color>/" + stringCost;
		}
	}
}

public enum DIRECTION
{
	Left = -1,
	Right = 1,
	Up = 2,
	Down = -2,
};


public class PvPConfig
{
	public PvPConfig()
	{
		LevelsToPlay_CurrentIndex = 0;
		LevelsToPlay = new int[] { };
		EnemyIDs_future = new List<UserModel>();
		EnemyIDs_history = new List<UserModel>();
	}
	public void AddCallendars()
	{
		new System.Globalization.GregorianCalendar();
		new System.Globalization.PersianCalendar();
		new System.Globalization.UmAlQuraCalendar();
		new System.Globalization.ThaiBuddhistCalendar();
	}

	public UserModel UserEnemy;
	public int LevelsToPlay_CurrentIndex;
	int[] LevelsToPlay;
	public int[] getLevesToPlay => LevelsToPlay;
	public void InitLevelToPlay(int[] levelToPlay)
	{
		this.LevelsToPlay = levelToPlay;
		this.LevelsToPlay_CurrentIndex = 0;
	}
	public int GetCurrentLevelToPlay()
	{
		if (LevelsToPlay_CurrentIndex >= LevelsToPlay.Length)
		{
			LevelsToPlay_CurrentIndex = 0;
		}
		int _temp = LevelsToPlay[LevelsToPlay_CurrentIndex];
		LevelsToPlay_CurrentIndex++;
		return _temp;
	}
	public List<UserModel> EnemyIDs_future;
	public List<UserModel> EnemyIDs_history;
	public List<PvPPlayRecordNote> Records;
	public PvPPlayRecordNote GetRecordsByLevel(int level)
	{
		if (Records == null) return null;
		foreach (PvPPlayRecordNote item in Records)
		{
			if (item.level == level) return item;
		}
		return null;
	}
	public void UpdateTime()
	{
		GameRESTController.Instance.API_GetTime(OnUpdatetimeDone, OnUpdatetimeError);
	}

	private void OnUpdatetimeError(string obj)
	{
		Debug.LogError(obj);
	}
	public int SeasonTime;
	public DateTime TimeServerNow;
	public DateTime TimeServerDeadline;
	public DateTime TimeLocalDeadline;
	private void OnUpdatetimeDone(HttpREsultObject obj)
	{
		HttpResultData result = obj.Data[0];
		string timeNow = result.TimeNow;
		TimeServerNow = DateTime.ParseExact(timeNow, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		string timeToDeadline = result.TimeDeadline;
		TimeServerDeadline = DateTime.ParseExact(timeToDeadline, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		TimeLocalDeadline = UnbiasedTime.Instance.Now().Add(TimeServerDeadline - TimeServerNow);
		Config.PvpConfig.SaveCache();
		EventDispatcher.Raise(new PvPUpdateTime());
		//HttpResultData result = obj.Data[0];
		//long timeToDeadline = result.TimeToDeadline;
		//DateTime timeToDeadlineDate = new DateTime(timeToDeadline);
	}

	public int Pay_Type;
	public int Pay_Value;
	public void PayToPlay()
	{
		GameData.Instance.CurrenciesGroup.Pay(Pay_Type, Pay_Value);
	}
	public void SaveCache()
	{
		PvPConfigCache cache = new PvPConfigCache()
		{
			TimeLocalDeadline = this.TimeLocalDeadline.ToMyString(),
			SeasonTime = this.SeasonTime
		};
		GameSave.SetPvPConfigCache(cache);
	}
	public bool LoadCache()
	{
		PvPConfigCache cache = GameSave.GetPvpConfigCache();
		if (cache != null)
		{
			try
			{
				if (cache.TimeLocalDeadline != null && cache.TimeLocalDeadline.Trim().Length > 0)
					this.TimeLocalDeadline = cache.TimeLocalDeadline.ToMyDate();
				this.SeasonTime = cache.SeasonTime;
				return true;
			}
			catch (Exception)
			{

				return false;
			}
		}
		return false;
	}

	public string DeadlineTimeString()
	{
		if (Config.PvpConfig.TimeLocalDeadline > UnbiasedTime.Instance.Now())
		{
			TimeSpan diff = Config.PvpConfig.TimeLocalDeadline - UnbiasedTime.Instance.Now();

			return diff.ToReadableSimpleString();
		}
		else
			return "NEW SEASON";
	}
}

[System.Serializable]
public class PvPConfigCache
{
	public string TimeLocalDeadline;
	public int SeasonTime;

}

public enum SceneName
{
	NONE, HeroPanel, FactoryPanel, SummonGatePanel,PvPMainPanel,MapPanel
}