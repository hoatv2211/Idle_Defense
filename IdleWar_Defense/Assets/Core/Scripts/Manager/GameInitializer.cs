using FoodZombie.UI;
using System;
using System.Collections.Generic;

using UnityEngine;
using Utilities.Common;
using Utilities.Service;
using Utilities.Service.GPGS;
using Utilities.Service.RFirebase;
using Utilities.Service.RFirebase.Storage;

namespace FoodZombie
{
    public class GameInitializer : MonoBehaviour
    {
        #region Members

#if DEVELOPMENT
        public static readonly bool ENABLE_CLOUD = true;
#else
        public static readonly bool ENABLE_CLOUD = false;
#endif
        public static Action onDownloadedData;
        public static Action onSignedIn;

        private static GameInitializer mInstance;
        public static GameInitializer Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = FindObjectOfType<GameInitializer>();
                return mInstance;
            }
        }

        private bool mInitialized;
        private bool mDownloading;
        private StorageGameData mDownloadedData;
        private string mLoggedUserId;
        private string mLoggedUserName;
        private bool mBackingUp;

        public StorageGameData DownloadedData => mDownloadedData;
        public bool Downloading => mDownloading;
        public string LoggedUserId => mLoggedUserId;
        public string LoggedUserName => mLoggedUserName;

        private float mLoadingTime;
        private bool mSentLoadingTime;

        #endregion

        //=============================================

        #region MonoBehaviour

        private void Awake()
        {
            if (mInstance == null)
                mInstance = this;
            else if (mInstance != this)
                Destroy(gameObject);
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
        }

        private void OnApplicationPause(bool pause)
        {
            if (!mInitialized)
                return;

            GameData.Instance.OnApplicationPause(pause);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!mInitialized)
                return;

            GameData.Instance.OnApplicationFocus(focus);
        }

        private void OnApplicationQuit()
        {
            GameData.Instance.OnApplicationQuit();
        }

        private void OnDestroy()
        {
            GameServices.onUserLoginSucceeded -= OnLoggedGPGS;
        }

        #endregion

        //=============================================

        #region Public

        public void Init()
        {
            if (mInitialized)
                return;

            mInitialized = true;
            mLoadingTime = Time.unscaledTime;

            //Init Server
            ServerManager.Init();

            //Init Game Core
            //EventDispatcher.Instance.Init();
            //GameData.CreateInstance().Init();

            //Others
            Localization.Init();
            AssetsCollection.instance.Init();

            //Init Game Services
            InitGameServices();

#if !ENABLE_GPGS
            ServerManager.Login(SystemInfo.deviceUniqueIdentifier, "", SystemInfo.deviceName, null);
#endif
        }

        /// <summary>
        /// Download data of logged in account
        /// </summary>
        public void DownloadCloudGameData(Action<bool> pOnFinished)
        {
            if (!ENABLE_CLOUD || !RFirebaseManager.storage.Initialized || string.IsNullOrEmpty(mLoggedUserId))
            {
                pOnFinished.Raise(false);
                return;
            }

            if (mDownloading)
                return;

            mDownloading = true;
            RFirebaseManager.CheckConnection((connected) =>
            {
                if (connected)
                {
                    RFirebaseManager.DownloadFile(BuildStorageFilePath(mLoggedUserId, mLoggedUserName), (content) =>
                    {
                        mDownloading = false;

                        mDownloadedData = JsonUtility.FromJson<StorageGameData>(content);
                        if (mDownloadedData == null)
                            mDownloadedData = new StorageGameData();
                        pOnFinished.Raise(true);
                        onDownloadedData.Raise();

                        Debug.Log("Found file in cloud");
                    }, () =>
                    {
                        mDownloading = false;

                        mDownloadedData = new StorageGameData();
                        pOnFinished.Raise(true);
                        onDownloadedData.Raise();

                        Debug.Log("File is not existed in cloud, that means this is new user");
                    }, () =>
                    {
                        mDownloading = false;
                        pOnFinished.Raise(false);

                        Debug.Log("Cannot connect to firebase");
                    });
                }
                else
                    mDownloading = false;
            });
        }

        /// <summary>
        /// Upload data of logged in account
        /// </summary>
        public void BackupGameData(Action<bool> pOnFinished)
        {
            var configGroup = GameData.Instance.GameConfigGroup;
            var localData = GameData.Instance.BuildStorageData();
            string curUserId = configGroup.StorageUserId;
            if (!ENABLE_CLOUD
                || !RFirebaseManager.storage.Initialized
                || string.IsNullOrEmpty(mLoggedUserId)
                || (!string.IsNullOrEmpty(curUserId) && curUserId != mLoggedUserId)
                || !localData.IsNewerThan(mDownloadedData))
            {
                pOnFinished.Raise(false);
                return;
            }

            if (mBackingUp)
                return;

            mBackingUp = true;
            var content = JsonUtility.ToJson(localData);
            RFirebaseManager.storage.UploadBytes(content, (success) =>
            {
                mBackingUp = false;
                if (success)
                {
                    mDownloadedData = localData;
                    configGroup.SetStorageAccount(mLoggedUserId, mLoggedUserName);
                }
                pOnFinished.Raise(success);
            }, BuildStorageFilePath(mLoggedUserId, mLoggedUserName));
        }

        public void SendLoadingTime()
        {
            if (mSentLoadingTime)
                return;
            mSentLoadingTime = true;

            mLoadingTime = Time.unscaledTime - mLoadingTime;
        }

        public bool CanBackup()
        {
            var configGroup = GameData.Instance.GameConfigGroup;
            string curUserId = configGroup.StorageUserId;
            if (!ENABLE_CLOUD
                || !RFirebaseManager.storage.Initialized
                || string.IsNullOrEmpty(mLoggedUserId)
                || (!string.IsNullOrEmpty(curUserId) && curUserId != mLoggedUserId)
                || mBackingUp)
            {
                return false;
            }
            var localData = GameData.Instance.BuildStorageData();
            return localData.IsNewerThan(mDownloadedData);
        }

        public bool CanRestore()
        {
            return mDownloadedData != null;
        }

        #endregion

        //==============================================

        #region Private

        private void InitGameServices()
        {
            var storeGroup = GameData.Instance.StoreGroup;
            //Payment
            PaymentHelper.Instance.InitProducts(storeGroup.GetAllConsumableSkus(), storeGroup.GetAllSubscriptionSkus(), null);
            GameServices.onUserLoginSucceeded += OnLoggedGPGS;

            //Init GPGS
            GameServices.Init();

            #if ACTIVE_FACEBOOK
            //Init FB
            Facebook.Unity.FB.Init();
            #endif

            //Init Firebase
            RFirebaseManager.Init((success) =>
            {
                //Login Firebase
                if (success)
                    RFirebaseManager.SigninAnonymously((authenticated) =>
                    {
                        if (authenticated)
                        {
#if FAKE_ACCOUNT
                            mLoggedUserId = DevSetting.instance.testUserId;
                            mLoggedUserName = DevSetting.instance.testUserName;
#else
                            if (GameServices.LocalUser != null)
                            {
                                mLoggedUserId = GameServices.LocalUser.id;
                                mLoggedUserName = GameServices.LocalUser.userName;
                            }
#endif
                            if (!string.IsNullOrEmpty(mLoggedUserId))
                                DownloadCloudGameData(null);
                        }
                    });
            });
        }

        private void OnLoggedGPGS()
        {
#if FAKE_ACCOUNT
            mLoggedUserId = DevSetting.instance.testUserId;
            mLoggedUserName = DevSetting.instance.testUserName;
#else
            if (GameServices.LocalUser != null)
            {
                mLoggedUserId = GameServices.LocalUser.id;
                mLoggedUserName = GameServices.LocalUser.userName;
            }
#endif
            if (!string.IsNullOrEmpty(mLoggedUserId))
                DownloadCloudGameData(null);

            onSignedIn.Raise();

            ServerManager.Login(mLoggedUserId, "", mLoggedUserName, null);
        }

        private SavedFileDefinition BuildStorageFilePath(string pStorageUserId, string pStorageUserName)
        {
#if UNITY_EDITOR
            string folder = "editor_users";
#elif UNITY_ANDROID
            string folder = "android_users";
#elif UNITY_IOS
            string folder = "ios_users";
#endif
            return new SavedFileDefinition(folder, pStorageUserId + ".json", new Dictionary<string, string>()
            {
                { "userId", pStorageUserId },
                { "userName", pStorageUserName }
            });
        }

#endregion
    }
}