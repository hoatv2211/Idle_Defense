using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HttpRESTController
{
	public static string TOKEN = "";
	public static string REFESH_TOKEN = "";

	public static string API_URL
	{
		get
		{
			switch (GameUnityData.instance.ConstantsConfig.SeverHost)
			{
				case ConstantsConfig.ServerHostType.Local:
					return "http://localhost:5000/";
					break;
				case ConstantsConfig.ServerHostType.GG_Clound:
					return "http://34.84.194.180:5000/";
					break;
				default:
					return "http://localhost:5000/";
					break;
			}

		}
	}
	public static bool isDebug => GameUnityData.instance.ConstantsConfig.RESTDebug;
	//#if DEVELOPMENT
	//public static string API_URL = "http://localhost:5000/";
	//#else
	//	public static string API_URL = "http://34.84.194.180:5000/";
	//#endif
	//public static string API_URL
	//{
	//	get
	//	{
	//		return AppCache.Login_APIURL + "/api/";
	//	}
	//}
	public void GET(MonoBehaviour mono, string API_AddLink, string[] urlAdd, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
#if !DEVELOPMENT
		Config.LogEvent("API_GET_" + API_AddLink.RemoveSpecialCharacters());
#endif
		mono.StartCoroutine(DoGET(mono, API_AddLink, urlAdd, onSuccess, onError));
	}
	public void POST(MonoBehaviour mono, string API_AddLink, bool userJSONRAW, string JSONRAW, string[] DataJSON, string[] urlAdd, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
#if !DEVELOPMENT
		Config.LogEvent("API_POST_" + API_AddLink.RemoveSpecialCharacters());
#endif
		mono.StartCoroutine(DoPOST(mono, API_AddLink, userJSONRAW, JSONRAW, DataJSON, urlAdd, onSuccess, onError));
	}

	IEnumerator DoGET(MonoBehaviour mono, string API_AddLink, string[] urlAdd, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//  DialogWaiting.Instance.Show(true);
		string stringAdd = null;
		if (urlAdd != null && urlAdd.Length > 0)
		{
			stringAdd = "?";
			bool _isFirst = true;
			for (int i = 0; i < urlAdd.Length; i = i + 2)
			{
				string _addValue = urlAdd[i + 1];
				if (_addValue != null && _addValue.Trim().Length > 0)
				{
					if (urlAdd[i + 1] != null)
					{
						if (!_isFirst) stringAdd += "&";
						_isFirst = false;
						stringAdd += urlAdd[i] + "=" + urlAdd[i + 1];
					}
				}
			}
		}
		string URL_GET = stringAdd != null ? API_URL + API_AddLink + stringAdd : API_URL + API_AddLink;
		if (isDebug)
			Debug.Log("get Data from :" + URL_GET);
		try
		{
			Uri u = new Uri(URL_GET);
		}
		catch (Exception e)
		{
			//	Debug.LogError(e.ToString());
			onError(e.Message.ToString());
			yield break;
		}
		using (UnityWebRequest req = UnityWebRequest.Get(URL_GET))
		{
			req.SetRequestHeader("x_authorization", TOKEN);
			req.SetRequestHeader("Accept", "application/json");
			yield return req.Send();
			while (!req.isDone)
				yield return null;
			if (req.isNetworkError || req.isHttpError)
			{

				onError("Can't connect server.Check your connection");
				//   DialogWaiting.Instance.Show(false);
			}
			else
			{
				byte[] result = req.downloadHandler.data;
				string dataJSON = System.Text.Encoding.Default.GetString(result);
				if (isDebug)
					Debug.LogError("get data " + dataJSON);
				HttpREsultObject info = null;
				try
				{
					info = JsonUtility.FromJson<HttpREsultObject>(dataJSON);
					if (isDebug)
						Debug.Log(dataJSON);
				}
				catch (Exception ex)
				{
				}

				if (info.Code == 0)
				{
					onError(info.Message);
				}
				else
				if (info.Code == 2)
				{
					if (isDebug)
						Debug.LogError("Token het han");
					GameRESTController.Instance.APIUser_RefreshTOKEN((data) =>
					{
						mono.StartCoroutine(DoGET(mono, API_AddLink, urlAdd, onSuccess, onError));
					}, (err) => { });
					//TOKEN het han
				}
				else
					onSuccess(info);
				//   DialogWaiting.Instance.Show(false);
			}
		}
	}

	IEnumerator DoPOST(MonoBehaviour mono, string API_AddLink, bool useJSONRAW, string JSONRAW, string[] DataJSON, string[] urlAdd, Action<HttpREsultObject> onSuccess, Action<string> onError)
	{
		//List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
		//formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
		//formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
		//WWWForm form = new WWWForm();
		//form.AddField("username", "admin");
		//form.AddField("password", "8bYJZ58ghiG4T+czbh0hXA==");
		//    DialogWaiting.Instance.Show(true);
		byte[] postDATA = null;
		string jsonData = null;

		if (!useJSONRAW)
		{
			if (DataJSON != null)
			{

				jsonData = DataJSON.toJSON();

			}

			if (DataJSON != null)
				postDATA = System.Text.UTF8Encoding.UTF8.GetBytes(jsonData);
		}
		else
		{
			jsonData = JSONRAW;
			if (jsonData != null)
				postDATA = System.Text.UTF8Encoding.UTF8.GetBytes(jsonData);
		}


		string stringAdd = null;
		if (urlAdd != null && urlAdd.Length > 0)
		{
			stringAdd = "?";
			bool _isFirst = true;
			for (int i = 0; i < urlAdd.Length; i = i + 2)
			{
				string _addValue = urlAdd[i + 1];
				if (_addValue != null && _addValue.Trim().Length > 0)
				{
					if (urlAdd[i + 1] != null)
					{
						if (!_isFirst) stringAdd += "&";
						_isFirst = false;
						stringAdd += urlAdd[i] + "=" + urlAdd[i + 1];
					}
				}
			}
		}
		string URL_FINAL = stringAdd != null ? API_URL + API_AddLink + stringAdd : API_URL + API_AddLink;


		UnityWebRequest www = null;
		try
		{
			Debug.Log(URL_FINAL);
			if (jsonData != null && isDebug)
				Debug.Log("POST DATA:" + jsonData);
			www = new UnityWebRequest(URL_FINAL);

		}
		catch (Exception e)
		{
			//Debug.Log("eror ne " + e.Message));
			onError(e.Message.ToString());
			yield break;
		}
		//Debug.LogError(form.ToString());
		www.method = "POST";
		//if (Data != null)
		if (postDATA != null)
			www.uploadHandler = new UploadHandlerRaw(postDATA);
		//www.uploadHandler = new UploadHandlerRaw(form.data);
		//	Debug.LogError("Token " + TOKEN);
		if (TOKEN != null && TOKEN.Trim().Length > 0)
			www.SetRequestHeader("x_authorization", TOKEN);
		www.downloadHandler = new DownloadHandlerBuffer();
		www.SetRequestHeader("Content-Type", "application/json");
		www.SetRequestHeader("Accept", "application/json");
		//yield return www.Send();
		yield return www.SendWebRequest();
		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(www.error);
			onError("Can't connect server.Check your connection");
			//    DialogWaiting.Instance.Show(false);
		}
		else
		{
			//Debug.Log("Form upload complete!");
			byte[] result = www.downloadHandler.data;
			string dataJSON = System.Text.Encoding.Default.GetString(result);
			if (isDebug)
				Debug.Log("dataJSON from POST: " + API_AddLink + "_data:" + dataJSON);
			HttpREsultObject info = JsonUtility.FromJson<HttpREsultObject>(dataJSON);
			if (info == null)
			{
				if (onError != null)
					onError("Not get Response");
			}
			else
			{
				if (info.Code == 0)
				{
					if (onError != null)
						onError(info.Message);
				}
				else
							if (info.Code == 2)
				{
					//TOKEN het han
					if (isDebug)
						Debug.LogError("Token het han");
					GameRESTController.Instance.APIUser_RefreshTOKEN((data) =>
					{
						mono.StartCoroutine(DoPOST(mono, API_AddLink, useJSONRAW, JSONRAW, DataJSON, urlAdd, onSuccess, onError));
					}, (err) => { });

				}
				else
						 if (onSuccess != null)
					onSuccess(info);

			}

			//    DialogWaiting.Instance.Show(false);
		}
	}


	//	public void GetTOKEN(MonoBehaviour mono, Action<HttpREsultObject> onSuccess, Action<string> onError)
	//	{
	//		var tokenData = new string[] { "username", "", "password", "" };
	//		mono.StartCoroutine(DoPOST(mono, "Security/Token", false, null, tokenData, null,
	// (s) =>
	// {
	//	 Debug.Log("TOKEN :" + s.Message);
	//	 HttpRESTController.TOKEN = s.Message;
	//	 if (onSuccess != null)
	//		 onSuccess(s);
	// },
	//(err) =>
	//{
	//	if (onError != null) onError(err);
	//}));
	//	}



	//[System.Serializable]
	//public class TOKENClass
	//{
	//	public string username;
	//	public string password;
	//}
}

