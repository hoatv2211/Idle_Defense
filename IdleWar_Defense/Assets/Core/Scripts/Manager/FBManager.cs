using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using FoodZombie;
using Utilities.Common;
using Utilities.Pattern.Data;
using System;
using FoodZombie.UI;

#if ACTIVE_FACEBOOK
using Facebook.Unity;
using Facebook.MiniJSON;
#endif

public class FBManager : SerializedMonoBehaviour
{
#if ACTIVE_FACEBOOK

	#region Singleton
	public static FBManager Instance => mInstance;

	private static FBManager mInstance;
	private void Awake()
	{
		if (mInstance == null)
			mInstance = this;
	}
	#endregion

	public UserModel mUserModel;

	public string FBID;
	public string FBName
	{
		get { return GameSave.FBname; }
		set { GameSave.FBname = value; }
	}
	public Sprite FBAvatar;
	public Texture2D profilePic;

	public bool IsLoggedIn => FB.IsLoggedIn;
	private void Start()
	{
		//Cache
		var _texture = GameSave.ReadTextureFromPlayerPrefs("FBAvatar");
		if (_texture != null)
		{
			profilePic = _texture;
			FBAvatar = Sprite.Create(profilePic, new Rect(0, 0, profilePic.width, profilePic.height), new Vector2(0, 0));
		}


		if (!FB.IsInitialized)
		{
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);

		}
		else
		{
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();

			DealWithFBMenus(FB.IsLoggedIn);
		}

	}

	private void InitCallback()
	{
		if (FB.IsInitialized)
		{
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...

			DealWithFBMenus(FB.IsLoggedIn);
		}
		else
		{
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown)
		{
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		}
		else
		{
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	#region Login & Logout
	[Button("FBLogin", ButtonSizes.Small)]
	public void FBLogin(Action<bool> OnDone = null)
	{
		List<string> perms = new List<string>() {/* "user_friends",*/ "public_profile", "email" };
		FB.LogInWithReadPermissions(perms, LoginReadCallback);
		this.OnLoginDone = OnDone;
	}


	Action<bool> OnLoginDone;
	private void LoginReadCallback(IResult result)
	{
		if (result.Error != null)
		{
			Debug.Log(result.Error);
			if (OnLoginDone != null) OnLoginDone(false);
		}
		else
		{
			if (FB.IsLoggedIn)
			{
				Debug.Log("FB is logged in");
				DealWithFBMenus(FB.IsLoggedIn);
			}
			else
			{
				Debug.Log("FB is not logged in");
				if (OnLoginDone != null) OnLoginDone(false);
			}

		}
	}
	void CheckLoginDone()
	{
		if (FB.IsLoggedIn && GetUserNameDone && GetAvatarDone)
			if (this.OnLoginDone != null) OnLoginDone(true);
	}

	bool GetUserNameDone = false;
	bool GetAvatarDone = false;
	private void DealWithFBMenus(bool isLoggedIn)
	{
		if (isLoggedIn)
		{
			//login success
			FB.API("/me?fields=first_name", HttpMethod.GET, GetUserName);
			FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, GetProfileAvatar);

			FBID = AccessToken.CurrentAccessToken.UserId;
			PlayerPrefs.SetString("IF_userIDFB", FBID.ToString());
			mUserModel.UserName = FBName;
			mUserModel.UserName = FBID;
		}
		else
		{
			//login fail

		}
	}
	Dictionary<string, Sprite> AvatarCache = new Dictionary<string, Sprite>();
	List<string> GetAvatarHistory = new List<string>();
	public Sprite GetOtherPlayerAvatar(string user_id, Action<Sprite> OnDone)
	{
		//	GetPictureURL(user_id, OnDone);
		if (GetAvatarHistory.Contains(user_id))
			GetAvatarHistory.Remove(user_id);
		GetAvatarHistory.Add(user_id);
		if (GetAvatarHistory.Count > 50)
		{
			string delID = GetAvatarHistory[0];
			GetAvatarHistory.RemoveAt(0);
			if (AvatarCache.ContainsKey(delID))
				AvatarCache.Remove(delID);
		}
		////FB.Mobile.CurrentAuthenticationToken;
		Sprite Sprite_output;
		if (!AvatarCache.TryGetValue(user_id, out Sprite_output))
			GetPictureURL(user_id, OnDone);
		else
		{
			//	PvPMainPanel.Instance.Img_test.sprite = Sprite_output;
			OnDone(Sprite_output);
			return Sprite_output;
		}
		return AssetsCollection.instance.heroIcon.GetAsset(0);
	}

	private void GetOtherAvatar(IGraphResult result, string id, Action<Sprite> OnDone)
	{
		if (result.Texture != null)
		{
			Texture2D profilePic = result.Texture as Texture2D;
			Sprite FBAvatar = Sprite.Create(profilePic, new Rect(0, 0, profilePic.width, profilePic.height), new Vector2(0, 0));
			if (!AvatarCache.ContainsKey(id))
				AvatarCache.Add(id, FBAvatar);
			if (OnDone != null) OnDone(FBAvatar);
		}
	}

	[Button("FBLogout", ButtonSizes.Small)]
	public void FBLogout()
	{
		FB.LogOut();
	}

	#endregion


	#region Actions

	private void GetUserName(IResult result)
	{
		if (result.Error == null)
		{
			FBName = result.ResultDictionary["first_name"].ToString();
			Debug.Log("FB User Name: " + FBName);
			GetUserNameDone = true;
			CheckLoginDone();
		}
		else
		{
			Debug.Log(result.Error);
			if (this.OnLoginDone != null) OnLoginDone(false);

		}
	}

	public Texture2D texture2D;
	private void GetProfileAvatar(IGraphResult result)
	{

		if (result.Texture != null)
		{
			profilePic = result.Texture as Texture2D;
			FBAvatar = Sprite.Create(profilePic, new Rect(0, 0, profilePic.width, profilePic.height), new Vector2(0, 0));
			GameSave.WriteTextureToPlayerPrefs("FBAvatar", profilePic);
			EventDispatcher.Raise(new FBChangeEvent());
			//var Bytes = profilePic.EncodeToPNG();
			//System.IO.File.WriteAllBytes(Application.dataPath + "/Core/Resources/imgAvatar.png", Bytes);
			//texture2D = GetPictureURL(FBID);
			GetAvatarDone = true;
			CheckLoginDone();
		}
		else
		{
			if (this.OnLoginDone != null) OnLoginDone(false);
		}
	}

	//Dùng hàm này lấy ảnh user theo id
	public void GetPictureURL(string _IdFacebook, Action<Sprite> OnDone)
	{
		FB.API(GetURL(_IdFacebook), HttpMethod.GET, (result) =>
		{

			var texture = result.Texture;

			if (OnDone != null)
			{
				texture2D = texture;// TEST
				Sprite spr = AssetsCollection.instance.heroIcon.GetAsset(0);
				if (texture2D != null)
				{
					spr = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
					if (!AvatarCache.ContainsKey(_IdFacebook))
						AvatarCache.Add(_IdFacebook, spr);
				}
				OnDone(spr);
			}

		});

	}

	public Sprite test;
	public string idtest;
	[Button("TEST")]
	public void TEST()
	{
		GetPictureURL(idtest, (spr) => { test = spr; });
	}

	public string GetURL(string facebookID, int? width = null, int? height = null, string type = null)
	{
		string url = string.Format("/{0}/picture", facebookID);
		string query = width != null ? "&width=" + width.ToString() : "";
		query += height != null ? "&height=" + height.ToString() : "";
		query += type != null ? "&type=" + type : "";
		if (query != "") url += ("?g" + query);
		return url;
	}

	public void ShareWithFriends()
	{
		string url = "https://play.google.com/store/apps/details?id=com.beemob.mergedefend";

#if UNITY_IOS
url = "https://apps.apple.com/us/app/cyber-war-idle-tower-defense/id1563690159";
#endif
		FB.FeedShare("", new Uri(url), "Name", "Cyber War", "Let's play together", null, "", null);
	}

	public void InviteFriends()
	{
		FB.AppRequest(
			message: "This game is awesome, join me. now.",
			title: "Invite your friends to join you"
			);
	}



	#endregion


	#region Services
	public void LogEvent(string pEventName)
	{
		if (!FB.IsInitialized) return;
		FB.LogAppEvent(pEventName);
	}

	public void LogEvent(string pEventName, float pValue)
	{
		if (!FB.IsInitialized) return;
		FB.LogAppEvent(pEventName, pValue);
	}

	public void LogEvent(string pEventName, string pParamName, string pParamValue)
	{
		if (!FB.IsInitialized) return;
		FB.LogAppEvent(pEventName, null, new Dictionary<string, object>()
			{
				{ pEventName, pParamValue }
			});
	}

	public void LogEvent(string pEventName, string pParamName, long pParamValue)
	{
		if (!FB.IsInitialized) return;
		FB.LogAppEvent(pEventName, null, new Dictionary<string, object>()
			{
				{ pEventName, pParamValue }
			});
	}

	public void LogEvent(string pEventName, string pParamName, double pParamValue)
	{
		if (!FB.IsInitialized) return;
		FB.LogAppEvent(pEventName, null, new Dictionary<string, object>()
			{
				{ pEventName, pParamValue }
			});
	}

	public void LogEvent(string pEventName, string[] pParamNames, int[] pParamValues)
	{
		if (!FB.IsInitialized) return;
		Dictionary<string, object> parameters = new Dictionary<string, object>();
		for (int i = 0; i < pParamNames.Length; i++)
		{
			parameters.Add(pParamNames[i], pParamValues[i]);
		}
		FB.LogAppEvent(pEventName, null, parameters);
	}

	public void LogEvent(string pEventName, string[] pParamNames, string[] pParamValues)
	{
		if (!FB.IsInitialized) return;
		Dictionary<string, object> parameters = new Dictionary<string, object>();
		for (int i = 0; i < pParamNames.Length; i++)
		{
			parameters.Add(pParamNames[i], pParamValues[i]);
		}
		FB.LogAppEvent(pEventName, null, parameters);
	}

	public void LogPurchase(float pLogPurchase, string pCurrency)
	{
		if (!FB.IsInitialized) return;
		FB.LogPurchase(pLogPurchase, pCurrency);
	}

	#region Example
	public void LogPurchasedEvent(int numItems, string contentType, string contentId, string currency, double price)
	{
		var parameters = new Dictionary<string, object>();
		parameters[AppEventParameterName.NumItems] = numItems;
		parameters[AppEventParameterName.ContentType] = contentType;
		parameters[AppEventParameterName.ContentID] = contentId;
		parameters[AppEventParameterName.Currency] = currency;
		FB.LogPurchase(
			(float)price,
			currency,
			parameters
		);
		Debug.Log("LogPurchasedEvent");
	}

	public void LogCancerPurchaseEvent(string contentType, float valToSum)
	{
		var parameters = new Dictionary<string, object>();
		parameters[AppEventParameterName.ContentType] = contentType;
		FB.LogAppEvent("Cancer Buy Purchase", valToSum, parameters);
	}

	public void LogFinishBuyPurchased(string name, float valToSum)
	{
		FB.LogAppEvent(name, valToSum);
	}

	public void LogAddedToCartEvent(int numItems, string contentId, string contentType, string currency, double price)
	{
		var parameters = new Dictionary<string, object>();
		parameters[AppEventParameterName.NumItems] = numItems;
		parameters[AppEventParameterName.ContentID] = contentId;
		parameters[AppEventParameterName.ContentType] = contentType;
		parameters[AppEventParameterName.Currency] = currency;
		FB.LogAppEvent(
			AppEventName.AddedToCart,
			(float)price,
			parameters
		);
		Debug.Log("LogAddedToCartEvent");
	}

	public void LogAds_VideoRewarded(string contentType, float valToSum)
	{
		var parameters = new Dictionary<string, object>();
		parameters[AppEventParameterName.ContentType] = contentType;
		FB.LogAppEvent("Ads_Video_Rewarded", valToSum, parameters);
	}

	public void LogAds_VideoFullAds(string contentType, float valToSum)
	{
		var parameters = new Dictionary<string, object>();
		parameters[AppEventParameterName.ContentType] = contentType;
		FB.LogAppEvent("Ads_Full_Video", valToSum, parameters);
	}

	public void LogEventWithName(string nameEvent, float valTosum)
	{
		FB.LogAppEvent(nameEvent, valTosum);
	}

	#endregion

	#endregion


#endif
}
