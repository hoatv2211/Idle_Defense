using FoodZombie;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSave
{
	public static string FBname
	{
		get
		{
			return PlayerPrefsUtility.GetEncryptedString(GameSaveKey.KEY_FB_NAME);
		}
		set
		{
			PlayerPrefsUtility.SetEncryptedString(GameSaveKey.KEY_FB_NAME, value);
			PlayerPrefs.Save();
		}
	}
	public static int PvPBestRank
	{
		get
		{
			return PlayerPrefsUtility.GetEncryptedInt(GameSaveKey.KEY_PVP_BEST_RANKID, 9999);
		}
		set
		{
			PlayerPrefsUtility.SetEncryptedInt(GameSaveKey.KEY_PVP_BEST_RANKID, value);
			PlayerPrefs.Save();
		}
	}

	#region UserModel
	public static void SetUserModel(UserModel usermodel)
	{
		PlayerPrefsUtility.SetEncryptedString(GameSaveKey.KEY_USER_MODEL, JsonUtility.ToJson(usermodel));
		PlayerPrefs.Save();
	}
	public static UserModel GetUserModel()
	{
		try
		{
			string data = PlayerPrefsUtility.GetEncryptedString(GameSaveKey.KEY_USER_MODEL);
			if (data == null || data.Length <= 0) return null;
			UserModel user = JsonUtility.FromJson<UserModel>(data);
			return user;
		}
		catch (System.Exception ex)
		{
			return null;
		}
	}
	#endregion
	#region PvpConfigCache
	public static void SetPvPConfigCache(PvPConfigCache usermodel)
	{
		PlayerPrefsUtility.SetEncryptedString(GameSaveKey.KEY_PVPConfig_Cache, JsonUtility.ToJson(usermodel));
		PlayerPrefs.Save();
	}
	public static PvPConfigCache GetPvpConfigCache()
	{
		try
		{
			string data = PlayerPrefsUtility.GetEncryptedString(GameSaveKey.KEY_PVPConfig_Cache);
			if (data == null || data.Length <= 0) return null;
			PvPConfigCache user = JsonUtility.FromJson<PvPConfigCache>(data);
			return user;
		}
		catch (System.Exception ex)
		{
			return null;
		}
	}
	#endregion
	#region Trap
	public static int Trap_TryTime_Get(TrapData trapData)
	{
		return PlayerPrefs.GetInt(string.Format(GameSaveKey.KEY_TRY_TRAP, trapData.Id), 0);
	}
	public static void Trap_TryTime_Set(TrapData trapData, int value)
	{
		PlayerPrefs.SetInt(string.Format(GameSaveKey.KEY_TRY_TRAP, trapData.Id), value);
		PlayerPrefs.Save();
	}
	#endregion

	public static void WriteTextureToPlayerPrefs(string tag, Texture2D tex)
	{
		// if texture is png otherwise you can use tex.EncodeToJPG().
		byte[] texByte = tex.EncodeToPNG();

		// convert byte array to base64 string
		string base64Tex = System.Convert.ToBase64String(texByte);

		// write string to playerpref
		PlayerPrefs.SetString(tag, base64Tex);
		PlayerPrefs.Save();
	}

	public static Texture2D ReadTextureFromPlayerPrefs(string tag)
	{
		// load string from playerpref
		string base64Tex = PlayerPrefs.GetString(tag, null);

		if (!string.IsNullOrEmpty(base64Tex))
		{
			// convert it to byte array
			byte[] texByte = System.Convert.FromBase64String(base64Tex);
			Texture2D tex = new Texture2D(2, 2);

			//load texture from byte array
			if (tex.LoadImage(texByte))
			{
				return tex;
			}
		}

		return null;
	}
}

public class GameSaveKey
{
	public const string KEY_FB_NAME = "KEY_FB_NAME";
	public const string KEY_USER_MODEL = "KEY_USER_MODEL";
	public const string KEY_PVP_BEST_RANKID = "KEY_PVP_BEST_RANKID";
	public const string KEY_PVPConfig_Cache = "KEY_PVPConfig_Cache";
	public const string KEY_TRY_TRAP = "KEY_TRY_TRAP_{0}";
}
