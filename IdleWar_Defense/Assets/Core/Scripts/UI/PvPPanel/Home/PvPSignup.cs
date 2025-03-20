using FoodZombie;
using FoodZombie.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvPSignup : MyGamesBasePanel
{
	public Button Btn_LoginFB, Btn_LoginGuest, Btn_CloseUserName, Btn_OKUserName;
	public InputField IF_UserName;
	public GameObject Panel_UserName;

	public Action OnNext;
	public Button[] Btn_avatars;
	int AvatarID;
	// Start is called before the first frame update
	void Start()
	{
		Btn_LoginFB.onClick.AddListener(OnBtnFBLoginClick);
		Btn_LoginGuest.onClick.AddListener(OnBtnFBGuestClick);
		Btn_CloseUserName.onClick.AddListener(OnBtnCloseUserNameClick);
		Btn_OKUserName.onClick.AddListener(OnBtnOKUserNameClick);
		AvatarID = 0;
		for (int i = 0; i < Btn_avatars.Length; i++)
		{
			Button btn_avatar = Btn_avatars[i];
			btn_avatar.transform.GetChild(0).GetComponent<Image>().sprite = AssetsCollection.instance.heroIcon.GetAsset(i);
			btn_avatar.transform.GetChild(1).gameObject.SetActive(i == AvatarID);
			int index = i;
			btn_avatar.onClick.AddListener(() => OnAvatarSelectClick(index));
		}

	}

	protected override void BeforeShowing()
	{
		base.BeforeShowing();
		Panel_UserName.SetActive(false);
	}
	private void OnAvatarSelectClick(int index)
	{
		AvatarID = index;
		for (int i = 0; i < Btn_avatars.Length; i++)
		{
			Button btn_avatar = Btn_avatars[i];
			btn_avatar.transform.GetChild(1).gameObject.SetActive(i == index);
		}
	}

	private void OnBtnOKUserNameClick()
	{
		if (IF_UserName.text.Trim().Length <= 0)
		{
			MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_28));
			//MainPanel.instance.ShowWarningPopup("Please input your team name");
			return;
		}
		string _inputName = IF_UserName.text.Trim();
		if (HasSpecialChars(_inputName))
		{
			MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_29));
			//MainPanel.instance.ShowWarningPopup("Invalid Name.Your team name CANNOT contain spaces and special characters");
			return;
		}
		if (_inputName.Length >= 20)
		{
			MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_30));
			//MainPanel.instance.ShowWarningPopup("Invalid Name.Your team name is too long");
			return;
		}
		{
			string userName = IF_UserName.text.Trim();
			UserModel user = new UserModel() { UserName = userName, IDAvatar = AvatarID };
			MainPanel.instance.ShowWaitingPanel(true, "signup");
			GameRESTController.Instance.APIUser_signup(user, OnSignupSuccess, OnSignupError);
		}
	}
	bool HasSpecialChars(string yourString)
	{
		foreach (char item in yourString)
		{
			if (!char.IsLetterOrDigit(item)) return true;
		}
		return false;
	}

	private void OnBtnCloseUserNameClick()
	{
		Panel_UserName.SetActive(false);
	}

	private void OnBtnFBGuestClick()
	{
		Panel_UserName.SetActive(true);
	}

	private void OnBtnFBLoginClick()
	{
        GoNext();

  //      if (FBManager.Instance.IsLoggedIn && UserGroup.UserData != null)
		//{
		//	GoNext();
		//}
		//else
		//{
		//	if (!FBManager.Instance.IsLoggedIn)
		//	{
		//		FBManager.Instance.FBLogin((loged) =>
		//		{
		//			if (loged)
		//			{
		//				UserModel user = new UserModel() { IDFacebook = FBManager.Instance.FBID, UserName = FBManager.Instance.FBName };
		//				MainPanel.instance.ShowWaitingPanel(true, "signup");
		//				GameRESTController.Instance.APIUser_signup(user, OnSignupSuccess, OnSignupError);
		//			}
		//			else
		//			{
		//				MainPanel.instance.ShowWarningPopup(Localization.Get(Localization.ID.MESSAGE_31));
		//				//MainPanel.instance.ShowWarningPopup("Can't login Facebook.Please try again");
		//			}
		//		});
		//	}
		//	else
		//	{
		//		//already login fb,let's go
		//		UserModel user = new UserModel() { IDFacebook = FBManager.Instance.FBID, UserName = FBManager.Instance.FBName };
		//		MainPanel.instance.ShowWaitingPanel(true, "signin");
		//		GameRESTController.Instance.APIUser_signup(user, OnSignupSuccess, OnSignupError);
		//	}

		//}
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
		GoNext();
	}


	void GoNext()
	{
		Back();
		if (OnNext != null)
			OnNext();
	}


}
