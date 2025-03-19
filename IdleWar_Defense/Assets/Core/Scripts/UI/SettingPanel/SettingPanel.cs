using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using UnityEngine.UI;
using TMPro;
using System;
using Utilities.Pattern.UI;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace FoodZombie.UI
{
	public class SettingPanel : MyGamesBasePanel
	{
		#region Members

		//[SerializeField] private TMP_InputField mInFiDisplayName;
		[SerializeField] private CustomToggleSlider mTogMusic;
		[SerializeField] private CustomToggleSlider mTogSFX;
		[SerializeField] private CustomToggleSlider mTogVibration;
		//[SerializeField] private CustomToggleSlider mTogHints;
		[SerializeField] private CustomToggleSlider mTogNotification;
		[SerializeField] private Button mBtnFB;
		[SerializeField] private Text mBtnTextFB;
		//[SerializeField] private SimpleTMPButton mBtnAchievement;
		//[SerializeField] private SimpleTMPButton mBtnCredit;
		//[SerializeField] private SimpleTMPButton mBtnLeaderboard;
		//[SerializeField] private SimpleTMPButton mBtnLoginFB;
		//[SerializeField] private SimpleTMPButton mBtnLanguage;
		//[SerializeField] private SimpleTMPButton mBtnFeedback;
		//[SerializeField] private SimpleTMPButton mBtnCloudCode;
		//[SerializeField] private SimpleTMPButton mBtnGiftCode;
		[SerializeField] private TMP_Dropdown mDropdownLanguage;
		[SerializeField] private SimpleTMPButton mBtnTerms;
		[SerializeField] private SimpleTMPButton mBtnPolicy;
		//[SerializeField] private GiftCodePanel mGfitCodePanel;
		//[SerializeField] private CreditPanel mCreditPanel;
		//[SerializeField] private Button mBtnDarkLayer;
		//[SerializeField] private CloudPanel mCloudPanel;

		[Separator("Cheats Gate")]
		[SerializeField] private JustButton mBtnDebugPanel;
		[SerializeField] private DebugPanel mDebugPanel;

		private string[] languages = new[] { "english", "vietnamease" };
		private GameData GameData { get { return GameData.Instance; } }

		public static bool loadingNewLanguage = false;

		#endregion

		//=============================================

		#region MonoBehaviour

		private void Start()
		{
#if DEVELOPMENT
			mBtnDebugPanel.SetActive(true);
			mBtnDebugPanel.onClick.AddListener(() =>
			{
				PushPanelToTop(ref mDebugPanel);
			});
#else
            mBtnDebugPanel.SetActive(false);
#endif

			//mInFiDisplayName.onValueChanged.AddListener(OnDisplayName_Changed);
			//mInFiDisplayName.characterLimit = 20;

			mTogMusic.onValueChanged.AddListener(OnTogMusic_Changed);
			mTogSFX.onValueChanged.AddListener(OnTogSFX_Changed);
			mTogVibration.onValueChanged.AddListener(OnTogVibration_Changed);
			//mTogHints.onValueChanged.AddListener(OnTogHints_Changed);
			mTogNotification.onValueChanged.AddListener(OnTogNotification_Changed);
			mDropdownLanguage.onValueChanged.AddListener(OnDropdownLanguage_Changed);
			mBtnFB.onClick.AddListener(OnBtnFacebook_Pressed);
			CheckFBButton();
			//mBtnAchievement.onClick.AddListener(OnBtnAchievement_Pressed);
			//mBtnCredit.onClick.AddListener(OnBtnCredit_Pressed);
			//mBtnLoginFB.onClick.AddListener(OnBtnLoginFB_Pressed);
			//mBtnLanguage.onClick.AddListener(OnBtnLanguage_Pressed);
			//mBtnLeaderboard.onClick.AddListener(OnBtnLeaderboard_Pressed);
			//mBtnCloudCode.onClick.AddListener(OnBtnCloudData_Pressed);
			//mBtnGiftCode.onClick.AddListener(OnBtnGiftCode_Pressed);
			//mBtnDarkLayer.onClick.AddListener(OnBtnDarkLayer_Pressed);
			//mBtnFeedback.onClick.AddListener(OnBtnFeedback_Pressed);
			mBtnTerms.SetEnable(false);
			mBtnPolicy.onClick.AddListener(OnBtnPolicy_Pressed);

			//mBtnAchievement.SetEnable(false);
			//mBtnCloudCode.SetEnable(GameInitializer.ENABLE_CLOUD);
			//mBtnLanguage.SetEnable(false);
			//mBtnLeaderboard.SetEnable(false);
			//mBtnLoginFB.SetEnable(false);
		}

		private void OnEnable()
		{
			Refresh();
		}

		#endregion

		//=============================================

		#region Public

		#endregion

		//==============================================

		#region Private

		private void Refresh()
		{
			//mTogHints.isOn = GameData.GameConfigGroup.EnableHint;
			mTogMusic.isOn = GameData.GameConfigGroup.EnableMusic;
			mTogNotification.isOn = GameData.GameConfigGroup.EnableNotification;
			mTogSFX.isOn = GameData.GameConfigGroup.EnableSFX;
			mTogVibration.isOn = GameData.GameConfigGroup.EnableVibration;
			var count = languages.Length;
			var currentLanguage = Localization.currentLanguage;
			for (int i = 0; i < count; i++)
			{
				if (currentLanguage.Equals(languages[i]))
				{
					mDropdownLanguage.value = i;
					break;
				}
			}

			CheckFBButton();
			//if (string.IsNullOrEmpty(GameData.GameConfigGroup.DisplayName))
			//    GameData.GameConfigGroup.SetDisplayName(GameInitializer.Instance.LoggedUserName);

			//mInFiDisplayName.text = GameData.GameConfigGroup.DisplayName;
		}

		//private void OnBtnLanguage_Pressed()
		//{
		//    if (Localization.currentLanguage.Equals("english"))
		//    {
		//        Localization.currentLanguage = "vietnamease";
		//    }
		//    else
		//    {
		//        Localization.currentLanguage = "english";
		//    }

		//    Back();
		//    MainPanel.instance.SetActive(false);
		//    MainPanel.instance.SetActive(true);
		//}

		//private void OnBtnLoginFB_Pressed()
		//{
		//    Application.OpenURL("fb://page/102327184460018/");
		//}

		//private void OnBtnCredit_Pressed()
		//{
		//    PushPanelToTop(ref mCreditPanel);
		//}

		//private void OnBtnAchievement_Pressed()
		//{
		//}
		private void OnBtnFacebook_Pressed()
		{
			if (!FBManager.Instance.IsLoggedIn)
				FBManager.Instance.FBLogin(OnFacebookLoginDone);
			else
				FBManager.Instance.ShareWithFriends();
		}
		void OnFacebookLoginDone(bool isDone)
		{
			CheckFBButton();
		}
		void CheckFBButton()
		{
			if (FBManager.Instance.IsLoggedIn)
			{
				this.mBtnTextFB.text = "FACEBOOK SHARE";
			}
			else
			{
				this.mBtnTextFB.text = "FACEBOOK LOGIN";
			}
		}
		private void OnTogNotification_Changed(bool pIsOn)
		{
			GameData.GameConfigGroup.SetEnableNotification(pIsOn);
		}

		private void OnDropdownLanguage_Changed(int index)
		{
			Localization.currentLanguage = languages[index];

			//reset text
			loadingNewLanguage = true;
			SceneLoader.LoadScene("Home", false);
		}

		//private void OnTogHints_Changed(bool pIsOn)
		//{
		//    GameData.GameConfigGroup.SetEnableHint(pIsOn);
		//}

		private void OnTogSFX_Changed(bool pIsOn)
		{
			GameData.GameConfigGroup.SetEnableSFX(pIsOn);
		}

		private void OnTogMusic_Changed(bool pIsOn)
		{
			GameData.GameConfigGroup.SetEnableMusic(pIsOn);
		}

		private void OnTogVibration_Changed(bool pIsOn)
		{
			GameData.GameConfigGroup.SetEnableVibration(pIsOn);
		}

		//private void OnDisplayName_Changed(string pValue)
		//{
		//    GameData.GameConfigGroup.SetDisplayName(pValue);
		//}

		//private void OnBtnGiftCode_Pressed()
		//{
		//    PushPanel(ref mGfitCodePanel, false);
		//}

		//private void OnBtnCloudData_Pressed()
		//{
		//    PushPanel(ref mCloudPanel, false);
		//}

		//private void OnBtnLeaderboard_Pressed()
		//{
		//}

		//protected override void OnAnyChildHide(PanelController pLastTop)
		//{
		//    base.OnAnyChildHide(pLastTop);

		//    if (TopPanel != null)
		//        mBtnDarkLayer.transform.SetSiblingIndex(TopPanel.transform.GetSiblingIndex() - 1);
		//    mBtnDarkLayer.SetActive(TopPanel != null);
		//}

		//protected override void OnAnyChildShow(PanelController pPanel)
		//{
		//    base.OnAnyChildShow(pPanel);

		//    if (TopPanel != null)
		//        mBtnDarkLayer.transform.SetSiblingIndex(TopPanel.transform.GetSiblingIndex() - 1);
		//    mBtnDarkLayer.SetActive(TopPanel != null);
		//}

		//private void OnBtnDarkLayer_Pressed()
		//{
		//    TopPanel.Back();
		//}

		//private void OnBtnFeedback_Pressed()
		//{
		//    #if UNITY_ANDROID
		//    Application.OpenURL("market://details?id=" + Application.identifier);
		//    #elif UNITY_IOS
		//    Device.RequestStoreReview();
		//    #endif
		//}

		private void OnBtnPolicy_Pressed()
		{
			Application.OpenURL("https://www.beemob.vn/terms-of-use/");
		}

		#endregion
	}
}