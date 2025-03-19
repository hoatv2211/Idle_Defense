
using Firebase.Crashlytics;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.Common;
using Utilities.Components;
using Utilities.Pattern.UI;

namespace FoodZombie.UI
{
	public class SystemPanel : PanelController
	{
		#region Members

		private static SystemPanel mInstance;
		public static SystemPanel Instance
		{
			get
			{
				if (mInstance == null)
					mInstance = FindObjectOfType<SystemPanel>();
				return mInstance;
			}
		}

		[SerializeField] private MessagesPopup mMessagePopup;
		[SerializeField] private DisplayFPS mDisplayFPS;

		private Queue<MessagesPopup.Message> mMessageQueue = new Queue<MessagesPopup.Message>();

		#endregion

		//=============================================

		#region MonoBehaviour

		protected override void Awake()
		{
			base.Awake();

			if (mInstance == null)
				mInstance = this;
			else if (mInstance != this)
				Destroy(gameObject);

			enabled = true;
		}

		private void Start()
		{
			Application.logMessageReceived += OnLogMessageReceived;
			//DevSetting.onShowFPS += OnShowFPS;
			DevSetting.Instance.onSettingsChanged += OnShowFPS;
			GameInitializer.onDownloadedData += OnDownloadedData;

			mDisplayFPS.gameObject.SetActive(DevSetting.Instance.showFPS);
		}

		private void OnDestroy()
		{
			Application.logMessageReceived -= OnLogMessageReceived;
			//DevSetting.onShowFPS -= OnShowFPS;
			DevSetting.Instance.onSettingsChanged -= OnShowFPS;
			GameInitializer.onDownloadedData -= OnDownloadedData;
		}

		private void Update()
		{
			if (!mDisplayFPS.gameObject.activeSelf)
				enabled = false;
		}

		#endregion

		//=============================================

		#region Public

		public void ShowMessagePopup(MessagesPopup.Message pMessage)
		{
			if (mMessageQueue.Contains(pMessage))
				return;
			mMessageQueue.Enqueue(pMessage);
			if (mMessagePopup.IsShowing)
				return;
			ShowMessagePopup();
		}

		#endregion

		//==============================================

		#region Private

		private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
		{
			if (type != LogType.Exception)
				return;


			//Debug.LogError("stackTRace: " + stackTrace);
			//Debug.LogError("condition: " + condition);
			try
			{
				string crashReport = string.Format("LOG ENTRY: {0}\n{1}", condition, stackTrace);
				Crashlytics.Log(crashReport);
				Crashlytics.LogException(new Exception("OnLogMessageReceived:" + condition));
			}
			catch (Exception ex)
			{

				Debug.LogError(ex.ToString());
			}


			//ShowMessagePopup(new MessagesPopup.Message()
			//{
			//    title = "FATAL ERROR",
			//    content = crashReport,
			//    yesAction = () =>
			//    {
			//        Config.LogEvent(TrackingConstants.CLICK_ERROR_QUIT);
			//        GameExtension.QuitGame();
			//    },
			//    noAction = null,
			//    additionalAction = () =>
			//    {
			//        string userId = GameInitializer.Instance.LoggedUserId;
			//        string subject = EmailHelper.CreateEmailReportSubject();
			//        string content = EmailHelper.CreateEmailReportContent(stackTrace, userId);
			//        EmailHelper.SendEmailByDefaultApp(subject, content, DevSetting.Instance.crashEmail, null);
			//    },
			//    yesActionLabel = "Quit",
			//    noActionLabel = "Ignore",
			//    additionalActionLabel = "Email us",
			//    allowIgnore = false,
			//    lockPopup = true,
			//    popupSize = new Vector2(1080, 960),
			//    contentAignment = TextAlignmentOptions.TopLeft
			//});
		}

		private void ShowMessagePopup()
		{
			PushPanelToTop(ref mMessagePopup);
			mMessagePopup.InitMessage(mMessageQueue.Peek());
			mMessagePopup.onDidHide += OnMessagePopupHidden;
		}

		private void OnMessagePopupHidden()
		{
			mMessagePopup.onDidHide -= OnMessagePopupHidden;
			mMessageQueue.Dequeue();
			if (mMessageQueue.Count > 0)
				ShowMessagePopup();
		}

		private void OnShowFPS()
		{
			mDisplayFPS.gameObject.SetActive(DevSetting.Instance.showFPS);
			enabled = true;
		}

		private void OnDownloadedData()
		{
			CompareClouDataToLocalData();
		}

		private float mPreScaleTime;
		private void CompareClouDataToLocalData()
		{
			if (!GameInitializer.ENABLE_CLOUD)
				return;

			var gameData = GameData.Instance;

			if (GameInitializer.Instance.Downloading)
			{
				WaitUtil.Start(new WaitUtil.ConditionEvent()
				{
					id = gameObject.GetInstanceID() + 13,
					onTrigger = CompareClouDataToLocalData,
					triggerCondition = () => !GameInitializer.Instance.Downloading
				});
			}
			else
			{
				var downloadedData = GameInitializer.Instance.DownloadedData;
				if (downloadedData == null)
					return;

				string loggedUserId = GameInitializer.Instance.LoggedUserId;
				string loggedUserName = GameInitializer.Instance.LoggedUserName;
				string currentUserId = gameData.GameConfigGroup.StorageUserId; //Previous user who has backed up data in this device
																			   //If different user
				if (loggedUserId != currentUserId)
				{
					//If previous user is different
					if (!string.IsNullOrEmpty(currentUserId))
					{
						string message = "";
						//User with total new data
						if (downloadedData.comparisonData.missionId == 0)
							message = Localization.Get(Localization.CLOUD_CHANGE_ACCOUNT);
						//User with existed data
						else
							message = Localization.Get(Localization.CLOUD_DATA_IS_READY);

						mPreScaleTime = Time.timeScale;
						Time.timeScale = 0;
						ShowMessagePopup(new MessagesPopup.Message()
						{
							content = message,
							allowIgnore = false,
							lockPopup = true,
							noAction = () =>
							{
								Time.timeScale = mPreScaleTime;
							},
							yesAction = () =>
							{
								Time.timeScale = mPreScaleTime;
								gameData.ImportData(downloadedData.gameData);
								gameData.GameConfigGroup.SetStorageAccount(loggedUserId, loggedUserName);

								if (SceneManager.GetActiveScene().name == "Home")
									SceneLoader.LoadScene("Home", false, true, null, null);
							},
							popupSize = new Vector2(1000, 600),
							contentAignment = TextAlignmentOptions.Center,
							title = Localization.Get(Localization.NOTIFICATION),
							yesActionLabel = Localization.Get(Localization.LOAD),
							noActionLabel = Localization.Get(Localization.IGNORE)
						});
					}
					//If previous user id not existed (user did not login in previous session)
					else
					{
						gameData.GameConfigGroup.SetStorageAccount(loggedUserId, loggedUserName);
					}
				}
				else
				{
					var currentData = gameData.BuildStorageData();
					if (downloadedData.IsNewerThan(currentData))
					{
						string message = Localization.Get(Localization.CLOUD_DATA_IS_NEWER);
						mPreScaleTime = Time.timeScale;
						Time.timeScale = 0;
						ShowMessagePopup(new MessagesPopup.Message()
						{
							content = message,
							allowIgnore = false,
							lockPopup = true,
							noAction = () =>
							{
								Time.timeScale = mPreScaleTime;
							},
							yesAction = () =>
							{
								Time.timeScale = mPreScaleTime;
								gameData.ImportData(downloadedData.gameData);
								gameData.GameConfigGroup.SetStorageAccount(loggedUserId, loggedUserName);

								if (SceneManager.GetActiveScene().name == "Home")
									SceneLoader.LoadScene("Home", false, true, null, null);
							},
							popupSize = new Vector2(1000, 600),
							contentAignment = TextAlignmentOptions.Center,
							title = Localization.Get(Localization.NOTIFICATION),
							yesActionLabel = Localization.Get(Localization.LOAD),
							noActionLabel = Localization.Get(Localization.IGNORE)
						});
					}
				}
			}
		}

		#endregion
	}
}