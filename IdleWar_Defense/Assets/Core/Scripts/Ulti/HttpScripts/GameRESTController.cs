using FoodZombie;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRESTController : MonoBehaviour
{
	public string TOKEN { get { return HttpRESTController.TOKEN; } set { HttpRESTController.TOKEN = value; } }
	public string REFESH_TOKEN { get { return HttpRESTController.REFESH_TOKEN; } set { HttpRESTController.REFESH_TOKEN = value; } }
	//public static GameRESTController Instance;

	HttpRESTController httpRESTController;
	public static GameRESTController Instance;

	private void Awake()
	{
		Instance = this;
		httpRESTController = new HttpRESTController();
	}
	//public GameRESTController()
	//{
	//	httpRESTController = new HttpRESTController();
	//}
	public void APIUser_signup(UserModel user, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//string _json = "{\"UserData\":" + JsonUtility.ToJson(user) + "}";
		httpRESTController.POST(this, "signup", true, JsonUtility.ToJson(user), null, null, onSuccess, onError);
	}

	public void APIUser_login(UserModel user, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//string _json = "{\"UserData\":" + JsonUtility.ToJson(user) + "}";
		httpRESTController.POST(this, "login", true, JsonUtility.ToJson(user), null, null, onSuccess, onError);
	}

	public void APIUser_RefreshTOKEN(Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//string _json = "{\"refreshToken\":" + this.REFESH_TOKEN + "}";
		string _json = new System.Object[] { "refreshToken", this.REFESH_TOKEN }.toJSON();
		httpRESTController.POST(this, "refresh", true, _json, null, null, (data) =>
		{
			try
			{
				HttpResultData result = data.Data[0];
				this.TOKEN = result.Token;
				Debug.LogError("Update TOKEN done");
				if (onSuccess != null) onSuccess(data);
			}
			catch (Exception ex)
			{
				Debug.LogError("Update TOKEN error " + ex.ToString());
				if (onError != null) onError(ex.ToString());
			}

		}, (err) =>
		{
			Debug.LogError(err);
			if (onError != null) onError(err);
		});
	}


	public void APIUser_getCurrentUserInfor(Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//string _json = "{\"UserData\":" + JsonUtility.ToJson(user) + "}";
		httpRESTController.GET(this, "actionGetUserInfor", null, onSuccess, onError);
	}


	/// <summary>
	/// Add Score to CurrentUser
	/// </summary>
	/// <param name="AddScore">Score to add</param>
	/// <param name="onSuccess"></param>
	/// <param name="onError"></param>
	public void APIUser_AddScore(int AddScore, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//string _json = "{\"UserData\":" + JsonUtility.ToJson(user) + "}";
		UserModel user = UserGroup.UserData.Clone();
		//user.refreshToken = this.REFESH_TOKEN;
		user.ScorePvPRank = AddScore;
		httpRESTController.POST(this, "PvP/addPvPScore", true, JsonUtility.ToJson(user), null, null, onSuccess, onError);
	}

	public void APIUser_UpdateInfor(string Formation, int CP, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//string _json = "{\"UserData\":" + JsonUtility.ToJson(user) + "}";
		//UserModel user = UserGroup.UserData;
		//user.refreshToken = this.REFESH_TOKEN;
		//user.ScorePvPRank = AddScore;
		string data = new string[] { "Formation", Formation, "CP", CP.ToString() }.toJSON();
		httpRESTController.POST(this, "actionUserUpdate", true, data, null, null, onSuccess, onError);
	}

	/// <summary>
	/// Get Enemies for matching
	/// </summary>
	/// <param name="onSuccess"></param>
	/// <param name="onError"></param>
	public void APIUser_GetEnemies(int ScoreMax, int ScoreMin, string[] IDNot, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		string JSON = "";
		if (ScoreMax >= 0)
		{
			JSON = new object[] { "ScoreMax", ScoreMax, "ScoreMin", ScoreMin, "IDNot", IDNot }.toJSON();
		}
		else
			JSON = new object[] { "ScoreMin", ScoreMin, "IDNot", IDNot }.toJSON();
		httpRESTController.POST(this, "PvP/getEnemies", true, JSON, null, null, onSuccess, onError);
		//httpRESTController.POST(this, "PvP/getEnemies", JSON, onSuccess, onError);
	}
	public void APIUser_GetRank(Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//string _json = "{\"UserData\":" + JsonUtility.ToJson(user) + "}";
		//UserModel user = UserGroup.UserData;
		//user.refreshToken = this.REFESH_TOKEN;
		//httpRESTController.POST(this.Mono, "PvP/getEnemies", true, JsonUtility.ToJson(user), null, null, onSuccess, onError);
		httpRESTController.GET(this, "PvP/getRank", null, onSuccess, onError);
	}
	public void APIUser_GetLeaderboard(int limit, int skip, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//string _json = "{\"UserData\":" + JsonUtility.ToJson(user) + "}";
		//UserModel user = UserGroup.UserData;
		//user.refreshToken = this.REFESH_TOKEN;
		//httpRESTController.POST(this.Mono, "PvP/getEnemies", true, JsonUtility.ToJson(user), null, null, onSuccess, onError);
		httpRESTController.GET(this, "PvP/getLeaderboard", new string[] { "limit", limit.ToString(), "skip", skip.ToString() }, onSuccess, onError);
	}

	public void APIRecord_AddRecord(PvPPlayRecordModel recordModel, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//string _json = "{\"UserData\":" + JsonUtility.ToJson(user) + "}";
		httpRESTController.POST(this, "PvP/addPvPRecord", true, JsonUtility.ToJson(recordModel), null, null, onSuccess, onError);
	}
	public void APIRecord_GetRecord(int rankGroupID, string[] LevelNames, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//string _json = "{\"UserData\":" + JsonUtility.ToJson(user) + "}";
		string json = (new System.Object[] { "RankGroupID", rankGroupID, "LevelNames", LevelNames }).toJSON();
		httpRESTController.POST(this, "PvP/getPvPRecord", true, json, null, null, onSuccess, onError);
	}

	public void APIPVPInfor_GetInfor(Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		httpRESTController.GET(this, "PvP/infor", null, onSuccess, onError);
	}
	public void API_GetTime(Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		httpRESTController.GET(this, "time", null, onSuccess, onError);
	}

	#region Example
	//public void APICustomerLogin(string sCustID, Action<HttpREsultObject> onSuccess, Action<string> onError)
	//{
	//	httpRESTController.GET(this, "Billing/CustomerLogin", new string[] { "sCustID", sCustID }, onSuccess, onError);
	//}

	//	public void APIPostReadingBilling(string sMngObjCod, string sCardID, string sPeriod, string sYear,
	//string iReadNumPrev, string sReadStsCod, string dReadDate, string iReadNum, string sReadChange, string iAddConsum, string iOldConsum, string sXbox, string sYbox, string dReadTime, string ReadDesc, string sUserName, string TotalConsum, Action<HttpREsultObject> onSuccess, Action<string> onError)
	//	{

	//		string _jsonOne = new string[] { "MngObjCod", sMngObjCod,
	// "CardID", sCardID,"Period",sPeriod,"Year",sYear,"ReadNumPrev",iReadNumPrev,"ReadStsCod",sReadStsCod,"ReadDate",dReadDate,
	//"ReadNum",iReadNum,"ReadChange",sReadChange,"AddConsum",iAddConsum,"OldConsum",iOldConsum,"Xbox",sXbox ,"Ybox",sYbox,"ReadTime",dReadTime,"ReadDesc",ReadDesc,"UserName",sUserName,"TotalConsum",TotalConsum}.toJSON();
	//		_jsonOne = string.Format("[{0}]", _jsonOne);
	//		httpRESTController.POST(this.Mono, "Reading/PostReadingBilling", true, _jsonOne, null, null, onSuccess, onError);
	//	}

	#endregion
}
