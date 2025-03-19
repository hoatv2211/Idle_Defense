using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

// This class is intended to be used the the AppsFlyerObject.prefab

public class AppsFlyerObjectScript : MonoBehaviour, IAppsFlyerConversionData
{

	public static AppsFlyerObjectScript Instance;
	// These fields are set from the editor so do not modify!
	//******************************//
	public string devKey;
	public string appID;
	public bool isDebug;
	public bool getConversionData;
	//******************************//
	private static bool isInit = false;
	void Start()
	{
		// These fields are set from the editor so do not modify!
		//******************************//
		AppsFlyer.setIsDebug(isDebug);
		AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);
		//******************************//
		// AppsFlyer.getConversionData("");
		AppsFlyer.startSDK();
		isInit = true;
		Instance = this;
		LogOpenApp();
	}



	// Mark AppsFlyer CallBacks
	public void onConversionDataSuccess(string conversionData)
	{
		AppsFlyer.AFLog("didReceiveConversionData", conversionData);
		Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
		// add deferred deeplink logic here
	}

	public void onConversionDataFail(string error)
	{
		AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
	}

	public void onAppOpenAttribution(string attributionData)
	{
		AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
		Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
		// add direct deeplink logic here
	}

	public void onAppOpenAttributionFailure(string error)
	{
		AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
	}

	public static void LogPurchase(string currency, string currencyValue, string skuId)
	{
		if (!isInit) return;
		var purchaseEvent = new Dictionary<string, string>();
		Debug.Log("IAP " + currency + "_" + currencyValue);
		purchaseEvent.Add(AFInAppEvents.CURRENCY, currency);
		purchaseEvent.Add(AFInAppEvents.REVENUE, currencyValue);
		purchaseEvent.Add(AFInAppEvents.QUANTITY, "1");
		purchaseEvent.Add(AFInAppEvents.CONTENT_ID, skuId);
		AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, purchaseEvent);

		//	AppsFlyer.ValidateAndLogIAP();
	}
	public void validateAndSendInAppPurchase_IOS(string productIdentifier, string price, string currency, string tranactionId, Dictionary<string, string> additionalParameters)
	{
		if (!isInit) return;
#if DEVELOPMENT
		Debug.Log("validateAndSendInAppPurchase_IOS " + productIdentifier + "_" + price + "_" + price + "_" + currency); return;
#endif
#if UNITY_IOS && !UNITY_EDITOR
       AppsFlyeriOS.validateAndSendInAppPurchase(
	productIdentifier,
		price,
		currency,
		tranactionId,
		additionalParameters,
		Instance);
#endif
	}
	public void validateAndSendInAppPurchase_Android(string publicKey, string signature, string purchaseData, string price, string currency, Dictionary<string, string> additionalParameters)
	{
		if (!isInit) return;
#if DEVELOPMENT
		Debug.Log("validateAndSendInAppPurchase_Android " + signature + "_" + purchaseData + "_" + price + "_" + currency); return;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
		AppsFlyerAndroid.validateAndSendInAppPurchase(
		  publicKey,
		  signature,
		 purchaseData,
		  price,
		  currency,
		  additionalParameters,
		  Instance);
#endif
	}
	public static void LogViewAd()
	{
		if (!isInit) return;
		var purchaseEvent = new Dictionary<string, string>();
		AppsFlyer.sendEvent("af_view_ad", purchaseEvent);
	}
	public static void LogViewInterstitial()
	{
		if (!isInit) return;
		var purchaseEvent = new Dictionary<string, string>();
		AppsFlyer.sendEvent("af_view_in", purchaseEvent);
	}

	public static void LogViewRewardVideo()
	{
		if (!isInit) return;
		var purchaseEvent = new Dictionary<string, string>();
		AppsFlyer.sendEvent("af_view_rw", purchaseEvent);
	}
	public static void LogStartViewRewardVideo()
	{
		if (!isInit) return;
		var purchaseEvent = new Dictionary<string, string>();
		AppsFlyer.sendEvent("af_start_view_rw", purchaseEvent);
	}

	public static void LogAdImpression(string platform, string adSource, string adUnit, string adFormat, string currency, double? value)
	{
		if (!isInit) return;
		var purchaseEvent = new Dictionary<string, string>();
		purchaseEvent.Add("platform", platform);
		purchaseEvent.Add("adSource", adSource);
		purchaseEvent.Add("adUnit", adUnit);
		purchaseEvent.Add("adFormat", adFormat);
		purchaseEvent.Add(AFInAppEvents.CURRENCY, currency);
		//	purchaseEvent.Add(AFInAppEvents.REVENUE, value.ToString());
		AppsFlyer.sendEvent("af_ad_impression", purchaseEvent);
	}

	public static void LogFirstOpenApp()
	{
		if (!isInit) return;
		var purchaseEvent = new Dictionary<string, string>();
		AppsFlyer.sendEvent("af_first_open_app", purchaseEvent);
	}
	public static void LogOpenApp()
	{
		if (!isInit) return;
		var purchaseEvent = new Dictionary<string, string>();
		AppsFlyer.sendEvent("af_open_app", purchaseEvent);
	}

	public static void LogLevelAchieved(int level)
	{
		if (!isInit) return;
		var purchaseEvent = new Dictionary<string, string>();
		purchaseEvent.Add("level", level.ToString());
		AppsFlyer.sendEvent("af_level_achieved", purchaseEvent);
	}
	public static void LogLevelClear(int level)
	{
		if (!isInit) return;
		var purchaseEvent = new Dictionary<string, string>();
		if (level == 1001 || level == 1003 || level == 1005 || level == 1008 || level == 1010 || level == 2001 || level == 3001 || level == 4001)
			AppsFlyer.sendEvent("cyber_level_" + level, purchaseEvent);
		var levelEvent = new Dictionary<string, string>();
		levelEvent.Add("level", level.ToString());
		AppsFlyer.sendEvent("af_level_clear", levelEvent);
	}

	public static void LogVisitShop()
	{
		if (!isInit) return;
		AppsFlyer.sendEvent("cyber_visit_shop", new Dictionary<string, string>());
	}
}
