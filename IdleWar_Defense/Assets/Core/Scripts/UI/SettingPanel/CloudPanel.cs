using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.Common;
using Utilities.Components;
using Utilities.Service.GPGS;

namespace FoodZombie.UI
{
    public class CloudPanel : MyGamesBasePanel
    {
        [SerializeField] private TextMeshProUGUI mTxtUserId;
        [SerializeField] private TextMeshProUGUI mTxtUserName;
        [SerializeField] private TextMeshProUGUI mTxtLastBackup;
        [SerializeField] private TextMeshProUGUI mTxtMissionCleared;
        [SerializeField] private TextMeshProUGUI mTxtTotalGold;
        [SerializeField] private SimpleTMPButton mBtnBackup;
        [SerializeField] private SimpleTMPButton mBtnRestore;
        [SerializeField] private GameObject mMessageBox;
        [SerializeField] private SimpleTMPButton mBtnLogin;
        [SerializeField] private GameObject mFunctionButtons;

        private GameData GameData => GameData.Instance;
        private GameInitializer Initializer => GameInitializer.Instance;
        private DateTime mLastTimeRestore;

        private void Start()
        {
            mBtnBackup.onClick.AddListener(OnBtnBackup_Pressed);
            mBtnRestore.onClick.AddListener(OnBtnRestore_Pressed);
            mBtnLogin.onClick.AddListener(OntBtnLogin_Pressed);
        }

        private void OntBtnLogin_Pressed()
        {
            GameServices.Init();
        }

        private void OnEnable()
        {
            Refresh();

            GameInitializer.onDownloadedData += OnDownloadedData;
            GameInitializer.onSignedIn += OnSignedIn;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            GameInitializer.onDownloadedData -= OnDownloadedData;
            GameInitializer.onSignedIn -= OnSignedIn;
        }

        private void OnSignedIn()
        {
            Refresh();
        }

        private void OnDownloadedData()
        {
            Refresh();
        }

        private void Refresh()
        {
            mFunctionButtons.SetActive(!string.IsNullOrEmpty(GameInitializer.Instance.LoggedUserId));
            mMessageBox.SetActive(string.IsNullOrEmpty(GameInitializer.Instance.LoggedUserId));

            mBtnBackup.SetEnable(Initializer.CanBackup());
            mBtnRestore.SetEnable(Initializer.CanRestore() && CheckLastTimeRestore());

            var downloadData = Initializer.DownloadedData;
            string missionCleared = downloadData != null ? downloadData.comparisonData.missionId.ToString() : "No Info";
            string lastBackup = downloadData != null && !string.IsNullOrEmpty(downloadData.updatedDate) ? downloadData.updatedDate : "No Info";
            string userName = downloadData != null && !string.IsNullOrEmpty(downloadData.userName) ? downloadData.userName : "No Info";
            string userId = downloadData != null && !string.IsNullOrEmpty(downloadData.userId) ? downloadData.userId : "No Info";
            string gold = downloadData != null && downloadData.comparisonData != null ? downloadData.comparisonData.gold.ToString() : "No Info";
            mTxtLastBackup.text = lastBackup;
            mTxtMissionCleared.text = missionCleared;
            mTxtUserId.text = userId;
            mTxtUserName.text = userName;
            mTxtTotalGold.text = gold;
        }

        private void OnBtnRestore_Pressed()
        {
            MainPanel.instance.ShowMessagePopup(new MessagesPopup.Message()
            {
                content = "Your current data will be overwritten by Cloud Data, Do you want to do it?",
                yesActionLabel = "RESTORE",
                noActionLabel = "CANCEL",
                yesAction = () =>
                {
                    var loggedUserId = Initializer.LoggedUserId;
                    var loggedUserName = Initializer.LoggedUserName;
                    var downloadData = Initializer.DownloadedData;
                    GameData.ImportData(downloadData.gameData);
                    GameData.GameConfigGroup.SetStorageAccount(loggedUserId, loggedUserName);
                    GameData.GameConfigGroup.LastTimeRestore = DateTime.Now;
                    mLastTimeRestore = DateTime.Now;
                    SceneLoader.LoadScene("Home",false,true, null, null);
                },
                noAction = () => { },
                title = "RESTORE DATA",
                popupSize = new Vector2(900, 500),
            });
        }

        private void OnBtnBackup_Pressed()
        {
            CanvasGroup.interactable = false;
            Initializer.BackupGameData((success) =>
            {
                CanvasGroup.interactable = true;

                if (success)
                {
                    Refresh();
                }
            });
        }

        private bool CheckLastTimeRestore()
        {
            mLastTimeRestore = GameData.GameConfigGroup.LastTimeRestore;
            return (DateTime.Now - mLastTimeRestore).TotalSeconds > 240;
        }
    }
}