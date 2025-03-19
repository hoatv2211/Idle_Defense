using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;
using FoodZombie;

public class Form_LeadBoardItem : EnhancedScrollerCellView
{

	public HttpResultData Info;

	[SerializeField] private GameObject[] SttRank;
	[SerializeField] private Image imgBackground;
	[SerializeField] private Image imgAvatar;
	[SerializeField] private Text txtRankSTT;
	[SerializeField] private Text txtName;
	[SerializeField] private Text txtScore;
	[SerializeField] private Text txtRankType;
	[SerializeField] private Text txtHighScore;
	UserModel userCurrent;

	bool isPlayer;
	public void SetData(HttpResultData _info, bool _isUser, int _stt)
	{
		Info = _info;
		userCurrent = new UserModel();
		userCurrent.UpdateBaseData(_info);

		isPlayer = _isUser;
		imgBackground.gameObject.SetActive(_isUser);

		SetSTTRank(_stt);

		txtRankType.text = userCurrent.GetRank().RankName;
		//txtRankSTT.text     = Info.RankCurrentPlay.ToString("");
		//if (_isUser)
		//	txtName.text = userCurrent.UserName_Player;
		//else
		txtName.text = userCurrent.UserName;
		txtScore.text = Info.ScorePvPRank.ToString("");
		txtHighScore.text = Info.ScorePvPRank.ToString("");
		imgAvatar.sprite = userCurrent.GetAvatar(isPlayer, OnGetAvatarDone);


	}

	public void SetSTTRank(int _rank)
	{
        try
        {
			SttRank[0].SetActive(_rank == 0);
			SttRank[1].SetActive(_rank == 1);
			SttRank[2].SetActive(_rank == 2);
			SttRank[3].SetActive(_rank > 2);

			txtRankSTT.text = (_rank + 1).ToString("");
		}
        catch (System.Exception ex)
        {

			Debug.LogError(ex.ToString());
        }
		
	}
	private void OnGetAvatarDone(Sprite obj)
	{
		imgAvatar.sprite = obj;
	}

	public void UpdateUserData()
	{
		isPlayer = true;
		imgBackground.gameObject.SetActive(true);
		try
        {
		
			UserModel user = UserGroup.UserData;
			txtName.text = user.UserName_Player;
			txtScore.text = user.ScorePvPRank.ToString("");
			txtHighScore.text = user.ScorePvPRank.ToString("");
			imgAvatar.sprite = user.GetAvatar();
			txtRankType.text = user.GetRank().RankName;
		}
        catch (System.Exception ex)
        {

			Debug.LogError(ex.ToString());
        }
	
	}
	public void SetRank(int _stt)
	{
		txtRankSTT.text = (_stt + 1).ToString("");
	}

	private string ConvertTypeRank(int _type)
	{
		var str = "";
		switch (_type)
		{
			case 1: str = "Elite 1"; break;
			case 2: str = "Elite 2"; break;
			case 3: str = "Hero 1"; break;
			case 4: str = "Hero 2"; break;
			case 5: str = "Hero 3"; break;
			case 6: str = "Diamond 1"; break;
			case 7: str = "Diamond 2"; break;
			case 8: str = "Diamond 3"; break;
			case 9: str = "Gold 1"; break;
			case 10: str = "Gold 2"; break;
			case 11: str = "Gold 3"; break;
			case 12: str = "Silver 1"; break;
			case 13: str = "Silver 2"; break;
			case 14: str = "Silver 3"; break;
			case 15: str = "Bronze 1"; break;
			case 16: str = "Bronze 2"; break;
			case 17: str = "Bronze 3"; break;
			case 18: str = "Iron 1"; break;
			case 19: str = "Iron 2"; break;
			case 20: str = "Iron 3"; break;
		}

		return str;
	}


}
