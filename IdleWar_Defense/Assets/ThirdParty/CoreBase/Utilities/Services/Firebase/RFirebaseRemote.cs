using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Utilities.Common;
#if ACTIVE_FIREBASE_REMOTE
using Firebase;
using Firebase.RemoteConfig;
#endif
using System;

namespace Utilities.Service.RFirebase
{
    public class RFirebaseRemote
    {
        private static RFirebaseRemote mInstance;
        public static RFirebaseRemote Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new RFirebaseRemote();
                return mInstance;
            }
        }

        public void Initialize(Dictionary<string, object> pDefaultData, bool pcheckDependency)
        {
#if ACTIVE_FIREBASE_REMOTE

            Firebase.Installations.FirebaseInstallations.DefaultInstance.GetTokenAsync(false).ContinueWith(
                task =>
                {
                    if (!(task.IsCanceled || task.IsFaulted) && task.IsCompleted)
                    {
                        UnityEngine.Debug.Log(System.String.Format(" Installationstoken {0}", task.Result));
                    }
                });
          //  FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(pDefaultData);
            if (pcheckDependency)
            {
                var task = FirebaseApp.CheckAndFixDependenciesAsync();
                WaitUtil.WaitTask(task, () =>
                {
                    bool success = !task.IsCanceled && !task.IsFaulted;
                    if (success)
                    {
                        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(pDefaultData);
                        FetchDataAsync();
                    }
                });
            }
            else
            {
                FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(pDefaultData);
                FetchDataAsync();
            }
#endif
        }

        /// <summary>
        /// Get the currently loaded data. If fetch has been called, this will be the data fetched from the server. Otherwise, it will be the defaults.
        /// Note: Firebase will cache this between sessions, so even if you haven't called fetch yet, if it was called on a previous run of the program, you will still have data from the last time it was run.
        /// </summary>
        public double GetNumberValue(string pKey, double pDefault = 0)
        {
#if ACTIVE_FIREBASE_REMOTE
            try
            {
                return FirebaseRemoteConfig.DefaultInstance.GetValue(pKey).DoubleValue;
            }
            catch (Exception ex)
            {

                return pDefault;
            }
           
#else
            return pDefault;
#endif
        }

        public string GetStringValue(string pKey, string pDefault = "")
        {
#if ACTIVE_FIREBASE_REMOTE
            try
            {
                return FirebaseRemoteConfig.DefaultInstance.GetValue(pKey).StringValue;
            }
            catch (Exception ex)
            {

                return pDefault;
            }
           
#else
            return pDefault;
#endif
        }

        public bool GetBoolValue(string pKey, bool pDefault = false)
        {
#if ACTIVE_FIREBASE_REMOTE
            try
            {
                return FirebaseRemoteConfig.DefaultInstance.GetValue(pKey).BooleanValue;
            }
            catch (Exception ex)
            {

                return pDefault;
            }
            
#else
            return pDefault;
#endif
        }

        /// <summary>
        /// Fetch new data if the current data is older than the provided timespan. 
        /// Otherwise it assumes the data is "recent enough", and does nothing.
        /// By default the timespan is 12 hours, and for production apps, this is a good number. 
        /// For this example though, it's set to a timespan of zero, so that
        /// changes in the console will always show up immediately.
        /// </summary>
        public void FetchDataAsync()
        {
#if ACTIVE_FIREBASE_REMOTE
            var fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
            WaitUtil.WaitTask(fetchTask, () =>
            {
                if (fetchTask.IsCanceled)
                    Debug.Log("Fetch canceled.");
                else if (fetchTask.IsFaulted)
                    Debug.Log("Fetch encountered an error.");
                else if (fetchTask.IsCompleted)
                    Debug.Log("Fetch completed successfully!");

                var info = FirebaseRemoteConfig.DefaultInstance.Info;
                switch (info.LastFetchStatus)
                {
                    case LastFetchStatus.Success:
                        FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
#if UNITY_EDITOR
                        Debug.Log($"Remote data loaded and ready (last fetch time {info.FetchTime}).");
#endif
                        break;
                    case LastFetchStatus.Failure:
                        switch (info.LastFetchFailureReason)
                        {
                            case FetchFailureReason.Error:
                                Debug.Log("Fetch failed for unknown reason");
                                break;
                            case FetchFailureReason.Throttled:
#if UNITY_EDITOR
                                Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
#endif
                                break;
                        }
                        break;
                    case LastFetchStatus.Pending:
                        Debug.Log("Latest Fetch call still pending.");
                        break;
                }
            });
#endif
        }

        public void LogFetchedData()
        {
#if ACTIVE_FIREBASE_REMOTE
            string log = "";
            var result = new Dictionary<string, ConfigValue>();
            var keys = FirebaseRemoteConfig.DefaultInstance.Keys;
            foreach (string key in keys)
            {
                var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
                result.Add(key, value);
                log += $"Key:{key} Value:{value}\n";
            }
            Debug.Log(log);
#endif
        }
    }
}