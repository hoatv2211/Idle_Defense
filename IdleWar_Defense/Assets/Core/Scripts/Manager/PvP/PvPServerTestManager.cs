using FoodZombie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
public class PvPServerTestManager : MonoBehaviour
{
	public GameRESTController RestController;

	public InputField IF_userID;
	public InputField IF_userIDFB;
	public InputField IF_username_signup;
	public InputField IF_userIDFB_signup;
	public Text Text_CurrentUSer;

	// Start is called before the first frame update
	void Start()
	{
		LoadUI();
		IF_userID.onEndEdit.AddListener((s) => SaveUI());
		IF_userIDFB.onEndEdit.AddListener((s) => SaveUI());
		IF_userIDFB_signup.onEndEdit.AddListener((s) => SaveUI());
		IF_username_signup.onEndEdit.AddListener((s) => SaveUI());
		RestController.API_GetTime(OnGetTimeDone, null);

	}

	private void OnGetTimeDone(HttpREsultObject obj)
	{
		HttpResultData result = obj.Data[0];
		string timeNow = result.TimeNow;
		DateTime timeNowDate = DateTime.ParseExact(timeNow, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		string timeToDeadline = result.TimeDeadline;
		DateTime timeDLDate = DateTime.ParseExact(timeToDeadline, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
		TimeSpan diff = timeDLDate - timeNowDate;
		Debug.LogError(timeNowDate.ToString() + " " + timeDLDate.ToString() + " " + diff.ToString(@"hh\:mm"));

	}

	void SaveUI()
	{
		PlayerPrefs.SetString("IF_userID", IF_userID.text);
		PlayerPrefs.SetString("IF_userIDFB", IF_userIDFB.text);
		PlayerPrefs.SetString("IF_userIDFB_signup", IF_userIDFB_signup.text);
		PlayerPrefs.SetString("IF_username_signup", IF_username_signup.text);
		PlayerPrefs.Save();
	}

	void LoadUI()
	{
		IF_userID.text = PlayerPrefs.GetString("IF_userID");
		IF_userIDFB.text = PlayerPrefs.GetString("IF_userIDFB");
		IF_userIDFB_signup.text = PlayerPrefs.GetString("IF_userIDFB_signup");
		IF_username_signup.text = PlayerPrefs.GetString("IF_username_signup");
	}


	#region Action
	public void Action_User_signup()
	{
		string userName = "";
		if (IF_username_signup.text.Length > 0)
			userName = IF_username_signup.text;
		else userName = UIExtension.GenerateName(5);
		string facebookID = IF_userIDFB_signup.text;
		UserModel user = new UserModel() { UserName = userName, IDApple = "", IDFacebook = facebookID, IDAvatar = 0, ScorePvPRank = 0 };
		RestController.APIUser_signup(user, (o) =>
		{
			Debug.Log("Done: " + o.ToString());
			HttpResultData result = o.Data[0];
			user.UpdateBaseData(result);
			this.RestController.TOKEN = result.Token;
			this.RestController.REFESH_TOKEN = result.refreshToken;
			Text_CurrentUSer.text = user.ToBeautifulString();
			user.Token = "";
			user.refreshToken = "";
			UserGroup.UserData = user;
			Debug.Log("Done: " + user.ToString());

		},
		(s) => { Debug.Log("Error " + s); });
	}
	public void Action_User_signin()
	{
		UserModel user = new UserModel() { ID = IF_userID.text, IDFacebook = IF_userIDFB.text };
		RestController.APIUser_login(user, (o) =>
		{

			HttpResultData result = o.Data[0];
			user.ID = result._id;
			user.UserName = result.UserName;
			user.IDFacebook = result.IDFacebook;
			user.IDApple = result.IDApple;
			user.ScorePvPRank = result.ScorePvPRank;
			user.IDAvatar = result.IDAvatar;
			user.UpdateTime = result.UpdateTime;
			this.RestController.TOKEN = result.Token;
			this.RestController.REFESH_TOKEN = result.refreshToken;
			Debug.Log("Done: " + user.ToString());
			Debug.Log("ID: " + user.ID);
			Text_CurrentUSer.text = user.ToBeautifulString();
			user.Token = "";
			user.refreshToken = "";
			UserGroup.UserData = user;


		},
		(s) => { Debug.Log("Error " + s); });
	}

	public void Action_User_AddScore()
	{
		UserModel user = UserGroup.UserData.Clone();
		user.ScorePvPRank = 10;
		RestController.APIUser_AddScore(10, (o) =>
		{
			Debug.Log("Done: " + o.ToString());

		},
		(s) => { Debug.Log("Error " + s); });
	}

	public void Action_User_GetREcord()
	{
		RestController.APIRecord_GetRecord(5, new string[] { "1001", "1002", "1003", "1004" }, (o) =>
			 {
				 Debug.Log("Done: " + o.ToString());

			 },
		(s) => { Debug.Log("Error " + s); });
	}
	public void Action_User_GetEnemies()
	{
		RestController.APIUser_GetEnemies(0, 100, null, (o) =>
		   {
			   Debug.Log("Done: " + o.ToString());

		   },
		(s) => { Debug.Log("Error " + s); });
	}
	public void Action_User_GetRank()
	{
		RestController.APIUser_GetRank((o) =>
		{
			if (o.Data != null)
				Debug.Log("Done: " + o.Data[0].RankCurrentPlay + "/" + o.Data[0].TotalPlayer);

		},
		(s) => { Debug.Log("Error " + s); });
	}
	public void Action_User_GetLeaderboard()
	{
		RestController.APIUser_GetLeaderboard(10, 0, (o) =>
		  {
			  if (o.Data != null)
				  Debug.Log("Done 10-0: " + o.Data.Length);

		  },
		(s) => { Debug.Log("Error " + s); });
		RestController.APIUser_GetLeaderboard(10, 10, (o) =>
		{
			if (o.Data != null)
				Debug.Log("Done 10-10: " + o.Data.Length);

		},
			(s) => { Debug.Log("Error " + s); });
	}

	public void Action_User_RefeshTOKEN()
	{
		RestController.APIUser_RefreshTOKEN(null, null);
	}
	#endregion


}
