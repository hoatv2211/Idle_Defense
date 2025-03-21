using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
	public class InfoUserPopup : MyGamesBasePanel
	{
		public UserModel mUserModel;
		[SerializeField] private Image imtAvatar;
		[SerializeField] private TextMeshProUGUI txtName;
		[SerializeField] private TextMeshProUGUI txtID;
		[SerializeField] private TextMeshProUGUI txtVip;
		[SerializeField] private TextMeshProUGUI txtLevel;

		[SerializeField] private SimpleTMPButton btnFBLogin;
		[SerializeField] private SimpleTMPButton btnFBLogout;
		[SerializeField] private CustomToggleSlider mTogMusic;
		[SerializeField] private CustomToggleSlider mTogSFX;
		[SerializeField] private CustomToggleSlider mTogVibration;
		[SerializeField] private CustomToggleSlider mTogNotification;

		private GameData GameData { get { return GameData.Instance; } }
		private void Start()
		{

			//btnFBLogin.SetUpEvent(OnBtnFBLoginClick);
			//btnFBLogout.SetUpEvent(FBManager.Instance.FBLogout);

			mTogMusic.onValueChanged.AddListener(OnTogMusic_Changed);
			mTogSFX.onValueChanged.AddListener(OnTogSFX_Changed);

		}

		private void OnEnable()
		{
			Refresh();
			//EventDispatcher.AddListener<FBChangeEvent>(OnFBChange);
		}
		protected override void OnDisable()
		{
			base.OnDisable();
			//EventDispatcher.RemoveListener<FBChangeEvent>(OnFBChange);
		}
		private void Refresh()
		{
			UserModel user = UserGroup.UserData;
			//btnFBLogin.SetEnable(!FBManager.Instance.IsLoggedIn);
			//   btnFBLogin.interactable=(!FBManager.Instance.IsLoggedIn);
			// btnFBLogout.SetActive(FBManager.Instance.IsLoggedIn);
			mTogMusic.isOn = GameData.GameConfigGroup.EnableMusic;
			mTogSFX.isOn = GameData.GameConfigGroup.EnableSFX;

			if (user != null)
			{
				imtAvatar.sprite = UserGroup.UserData.GetAvatar();
				txtName.text = user.UserName_Player;
				txtID.text = "#" + user.ID;
			}
			else
			{
				txtName.text = "PLAYER";
			}


			txtLevel.text = "" + GameData.Instance.UserGroup.Level;
			txtVip.text = "VIP " + GameData.Instance.UserGroup.VipLevel;
		}




		#region Private
		private void OnBtnFBLoginClick()
		{
            //if (!FBManager.Instance.IsLoggedIn)
            //{
            //	FBManager.Instance.FBLogin((loged) =>
            //	{
            //		if (loged)
            //		{
            //			UserModel user = new UserModel() { IDFacebook = FBManager.Instance.FBID, UserName = FBManager.Instance.FBName };

            //			UserModel userDataLocal = UserGroup.UserData;
            //			if (userDataLocal != null)
            //			{
            //				//already have UserData for PvP
            //				user.ScorePvPRank = userDataLocal.ScorePvPRank;
            //			}


            //			MainPanel.instance.ShowWaitingPanel(true, "signup");
            //			GameRESTController.Instance.APIUser_signup(user, OnSignupSuccess, OnSignupError);
            //			Refresh();
            //		}
            //		else
            //		{
            //			MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_27));
            //			//MainPanel.instance.ShowWarningPopup("Can't login Facebook");
            //		}
            //	});
            //}
            MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_27));
        }
		private void OnSignupError(string obj)
		{
			MainPanel.instance.ShowWaitingPanel(false, "signup");
			MainPanel.instance.ShowWarningPopup(obj.ToString());
		}

		private void OnSignupSuccess(HttpREsultObject o)
		{
			MainPanel.instance.ShowWaitingPanel(false, "signup");
			HttpResultData result = o.Data[0];
			UserModel user = new UserModel();
			user.ID = result._id;
			user.UserName = result.UserName;
			user.IDFacebook = result.IDFacebook;
			user.IDApple = result.IDApple;
			user.ScorePvPRank = result.ScorePvPRank;
			user.IDAvatar = result.IDAvatar;
			GameRESTController.Instance.TOKEN = result.Token;
			GameRESTController.Instance.REFESH_TOKEN = result.refreshToken;
			UserGroup.UserData = user;
			UserGroup.UserData.SaveToGameSave();

		}

		private void OnFBChange(FBChangeEvent e)
		{
			Refresh();
		}

		private void OnTogSFX_Changed(bool pIsOn)
		{
			GameData.GameConfigGroup.SetEnableSFX(pIsOn);
		}

		private void OnTogMusic_Changed(bool pIsOn)
		{
			GameData.GameConfigGroup.SetEnableMusic(pIsOn);
		}

		private void OnBtnPolicy_Pressed()
		{
			Application.OpenURL("");
		}

		#endregion
	}
}

