using FoodZombie;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserModel
{
	public string ID;
	public string UserName;
	public string UserName_Player
	{
		get
		{
			if (FBManager.Instance.IsLoggedIn)
			{
				return FBManager.Instance.FBName;
			}
			return UserName;
		}
	}
	public string UserName_Enemy
	{
		get
		{
			if (IDFacebook != null && IDFacebook.Trim().Length > 0)
			{
				return FBManager.Instance.FBName;
			}
			else
			{
				return UserName;
			}
		}
	}
	//private void OnGetFBAvatarDone(Sprite obj)
	//{
	//	FBAvatarCache = obj;
	//}

	public string IDFacebook;
	public string IDApple;

	public string Formation;
	public int CP;

	public int IDAvatar;
	public int ScorePvPRank;
	public string UpdateTime;
	public string Token;
	public string refreshToken;

	public int SeasonPlaying;
	public int[] RWClaims;
	public bool RWisClaim(int RankID)
	{
		if (RWClaims == null || RWClaims.Length <= 0) return false;
		if (new List<int>(RWClaims).Contains(RankID)) return true;
		return false;
	}
	public void RWAdd(int RankID)
	{
		if (RWClaims == null) RWClaims = new int[] { RankID };
		else
		if (!(new List<int>(RWClaims)).Contains(RankID))
		{
			List<int> _temp = new List<int>(RWClaims);
			_temp.Add(RankID);
			RWClaims = _temp.ToArray();
		}
		this.SaveToGameSave();
	}
	public void RWClear()
	{
		RWClaims = new int[] { };
		this.BestRankID = 9999;
		this.SaveToGameSave();
	}
	public int BestRankID
	{
		get { return GameSave.PvPBestRank; }
		set
		{
			GameSave.PvPBestRank = value;
		}
	}
	public Sprite GetAvatar(bool isMy = true, Action<Sprite> OnGetDone = null)
	{
		if (isMy)
		{
			if (FBManager.Instance.IsLoggedIn)
				return FBManager.Instance.FBAvatar;
			return AssetsCollection.instance.heroIcon.GetAsset(IDAvatar);
		}
		else
		{
			if (IDFacebook != null && IDFacebook.Trim().Length > 0)
			{
				return FBManager.Instance.GetOtherPlayerAvatar(IDFacebook, OnGetDone);

			}
			else
				return AssetsCollection.instance.heroIcon.GetAsset(IDAvatar);
		}
	}
	public UserModel Clone()
	{
		return new UserModel()
		{
			ID = this.ID,
			UserName = this.UserName,
			IDAvatar = this.IDAvatar,
			IDFacebook = this.IDFacebook,
			ScorePvPRank = this.ScorePvPRank,
			Formation = this.Formation,
			CP = this.CP,
			UpdateTime = this.UpdateTime

		};
	}
	public void UpdateBaseData(HttpResultData result)
	{
		this.ID = result._id;
		this.IDFacebook = result.IDFacebook;
		this.IDApple = result.IDApple;
		this.UserName = result.UserName;
		this.ScorePvPRank = result.ScorePvPRank;
		this.IDAvatar = result.IDAvatar;
		this.UpdateTime = result.UpdateTime;

	}
	public void SaveToGameSave()
	{
		GameSave.SetUserModel(this);
	}
	public PvPOneRankData GetRank()
	{
		List<PvPOneRankData> ranks = GameUnityData.instance.PvPRankData.Datas;
		for (int i = ranks.Count - 1; i >= 0; i--)
		{
			PvPOneRankData rank = ranks[i];
			if (this.ScorePvPRank >= rank.RankPointRequest)
				return rank;
		}
		return ranks[ranks.Count - 1];
	}

	public PvPOneRankData GetRankByID(int ID)
	{
		List<PvPOneRankData> ranks = GameUnityData.instance.PvPRankData.Datas;
		for (int i = ranks.Count - 1; i >= 0; i--)
		{
			PvPOneRankData rank = ranks[i];
			if (rank.ID == ID)
				return rank;
		}
		return null;
	}
	public string ToBeautifulString()
	{
		return String.Format("ID:{0}\nUsername:{1}\nIDFacebook:{2}", ID, UserName, IDFacebook);
	}
	public string ToString()
	{
		return JsonUtility.ToJson(this);
	}
}
