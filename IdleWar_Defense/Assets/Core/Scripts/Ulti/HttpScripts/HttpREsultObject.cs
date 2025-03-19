using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class HttpREsultObject
{

	public int Code;
	public string Message;
	public HttpResultData[] Data;
	public override string ToString()
	{
		string dataString = "null";
		if (Data != null && Data.Length > 0)
		{
			dataString = "Data length " + Data.Length;
			for (int i = 0; i < Data.Length; i++)
			{
				dataString += "\n" + Data[i].ToString();
			}
		}
		return "Code:" + Code + ",Message:" + Message + ",Data:" + dataString;
	}
}

[System.Serializable]
public class HttpResultData
{
	//User Infor
	public string _id;
	public string UserName;
	public string IDFacebook;
	public string IDApple;
	public int IDAvatar;
	public string Formation;
	public int CP;
	public int ScorePvPRank;
	public string refreshToken;
	public string Token;
	public string UpdateTime;
	//count
	public int TotalPlayer;
	public int RankCurrentPlay;
	//Time:
	public string TimeNow;
	public string TimeDeadline;
	//record:
	public int[] data;
	public string LevelName;
	//Season:
	public int SeasonTime;
	public override string ToString()
	{
		return JsonUtility.ToJson(this);
	}


}

