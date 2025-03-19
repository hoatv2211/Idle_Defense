using UnityEngine;
using System.Collections.Generic;
#if ACTIVE_FACEBOOK
using Facebook.Unity;
#endif
namespace Utilities.Services
{
    public class FBServices
    {
#if ACTIVE_FACEBOOK
        public static void LogEvent(string pEventName)
        {
            if (!FB.IsInitialized) return;
            FB.LogAppEvent(pEventName);
        }

        public static void LogEvent(string pEventName, float pValue)
        {
            if (!FB.IsInitialized) return;
            FB.LogAppEvent(pEventName, pValue);
        }

        public static void LogEvent(string pEventName, string pParamName, string pParamValue)
        {
            if (!FB.IsInitialized) return;
            FB.LogAppEvent(pEventName, null, new Dictionary<string, object>()
            {
                { pEventName, pParamValue }
            });
        }

        public static void LogEvent(string pEventName, string pParamName, long pParamValue)
        {
            if (!FB.IsInitialized) return;
            FB.LogAppEvent(pEventName, null, new Dictionary<string, object>()
            {
                { pEventName, pParamValue }
            });
        }

        public static void LogEvent(string pEventName, string pParamName, double pParamValue)
        {
            if (!FB.IsInitialized) return;
            FB.LogAppEvent(pEventName, null, new Dictionary<string, object>()
            {
                { pEventName, pParamValue }
            });
        }

        public static void LogEvent(string pEventName, string[] pParamNames, int[] pParamValues)
        {
            if (!FB.IsInitialized) return;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            for (int i = 0; i < pParamNames.Length; i++)
            {
                parameters.Add(pParamNames[i], pParamValues[i]);
            }
            FB.LogAppEvent(pEventName, null, parameters);
        }

        public static void LogEvent(string pEventName, string[] pParamNames, string[] pParamValues)
        {
            if (!FB.IsInitialized) return;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            for (int i = 0; i < pParamNames.Length; i++)
            {
                parameters.Add(pParamNames[i], pParamValues[i]);
            }
            FB.LogAppEvent(pEventName, null, parameters);
        }
        
        public static void LogPurchase(float pLogPurchase, string pCurrency)
        {
            if (!FB.IsInitialized) return;
            FB.LogPurchase(pLogPurchase, pCurrency);
        }
#else
        public static void LogEvent(string pEventName) { }
        public static void LogEvent(string pEventName, float pValue) { }
        public static void LogEvent(string pEventName, string pParamName, string pParamValue) { }
        public static void LogEvent(string pEventName, string pParamName, long pParamValue) { }
        public static void LogEvent(string pEventName, string pParamName, double pParamValue) { }
        public static void LogEvent(string pEventName, string[] pParamNames, int[] pParamValues) { }
        public static void LogEvent(string pEventName, string[] pParamNames, string[] pParamValues) { }
        public static void LogEvent(float pLogPurchase, string pCurrency) { }
#endif
    }
}