using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Components;
using DG.Tweening;
using System.Collections;
using System;
using System.Collections.Generic;

namespace FoodZombie.UI
{
	public class ResultFindMatchPanel : MonoBehaviour
	{
		[Header("User")]
		[SerializeField] private Text txtUserName;
		[SerializeField] private Image Image_UserView;
		[SerializeField] private TextMeshProUGUI txtUserCP;

		public HeroView[] currentFormation;
		public SimpleButton[] btnSlots;
		public Text[] btnSlotsTextes;

		[Header("Enemy")]
		[SerializeField] private Text txtEnemyName;
		[SerializeField] private Image Image_EnemyView;
		[SerializeField] private TextMeshProUGUI txtEnemyCP;
		public HeroView[] EnemyFormation;

		public Text Text_StartTime;

		private PvPMainPanel pvPMainPanel;

		public SimpleTMPButton btnFight;

		public void CallStart(PvPMainPanel _pvp)
		{
			pvPMainPanel = _pvp;
			gameObject.SetActive(true);
			ShowCurrentFormation();
			//for (int i = 0; i < btnSlots.Length; i++)
			//{
			//	int index = i;
			//	btnSlots[i].onClick.AddListener(() => pvPMainPanel.BtnSlot_Click(index));
			//	currentFormation[i].btnChoice.onClick.AddListener(() => pvPMainPanel.BtnSlot_Click(index));

			//}

			btnFight.SetUpEvent(Action_BtnFight);
			StartCoroutine(IEDoStartGame());

		}
		IEnumerator IEDoStartGame()
		{
			UserModel user = UserGroup.UserData;


			UserModel enemy = Config.PvpConfig.UserEnemy;
			txtUserName.text = user.UserName;
			Image_UserView.sprite = user.GetAvatar(true);
			txtUserCP.text = GameData.Instance.HeroesGroup.GetEquippedHeroesCP().ToString();

			if (enemy.UserName == null || enemy.UserName.Trim().Length == 0)
				enemy.UserName = UIExtension.GenerateName(UnityEngine.Random.Range(5, 10));
			txtEnemyName.text = enemy.UserName;
			if (enemy.CP == 0)
				enemy.CP = UnityEngine.Random.Range(user.CP - (user.CP / 2), user.CP + (user.CP / 2));
			if (enemy.CP <= 0) enemy.CP = UnityEngine.Random.Range((user.CP / 2), (user.CP * 2)); ;
			txtEnemyCP.text = enemy.CP.ToString();
			Image_EnemyView.sprite = enemy.GetAvatar(false, (sprite) =>
			{
				Image_EnemyView.sprite = sprite;
			});
			ShowEnemyFormation(enemy.Formation);

			float timeToShow = 5;
			Text_StartTime.text = timeToShow.ToString();
			bool show = true;
			float _temp = 0.5f;
			while (timeToShow > 0)
			{
				Text_StartTime.text = Mathf.FloorToInt(timeToShow).ToString();
				Text_StartTime.gameObject.SetActive(show);
				yield return new WaitForSeconds(_temp);
				show = !show;
				timeToShow -= .2f;
				if (timeToShow <= 2) _temp = 0.1f;
			}
			Action_BtnFight();
		}



		void ShowEnemyFormation(string formation)
		{
			int[] heroIDToShow = new int[6];
			if (formation != null)
			{
				string[] datas = formation.Split(',');
				for (int i = 0; i < datas.Length; i++)
				{
					string data = datas[i];
					if (data == null || data.Trim().Length == 0) continue;
					if (i >= EnemyFormation.Length) break;
					//	HeroView view = EnemyFormation[i];
					int heroID = -1;
					if (int.TryParse(data, out heroID))
					{
						heroIDToShow[i] = heroID;
					}
					else
					{
						heroIDToShow[i] = -1;
					}
				}
			}
			else
			{
				HeroData[] _playerFormation = GameData.Instance.HeroesGroup.GetEquippedHeroes();
				for (int j = 0; j < _playerFormation.Length; j++)
				{
					if (_playerFormation[j] != null)
						heroIDToShow[j] = UnityEngine.Random.Range(0, _playerFormation[j].baseId + 1);
					else
					if (UnityEngine.Random.Range(0, 3) == 0)
						heroIDToShow[j] = UnityEngine.Random.Range(31, 60);
					else heroIDToShow[j] = -1;
				}
			}
			for (int i = 0; i < heroIDToShow.Length; i++)
			{
				HeroView view = EnemyFormation[i];
				view.ShowLevel(false);
				view.ActiveArrow(false);
				int heroID = heroIDToShow[i];
				if (heroID != -1 && heroID != 0)
				{
					HeroData _hero = new HeroData(i, heroID);
					//Debug.LogError("HEro init " + heroID);
					try
					{
						if (_hero != null)
							view.Init(_hero, pvPMainPanel);
						else
							view.SetEmpty();
					}
					catch (Exception ex)
					{
						view.SetEmpty();
					}

				}
				else
					view.SetEmpty();
			}
		}
		public void ShowCurrentFormation()
		{
			pvPMainPanel.choiceFormation = GameData.Instance.HeroesGroup.CurrentFormation;
			HeroData[] formationData = GameData.Instance.HeroesGroup.GetEquippedHeroes();
			int count = formationData.Length;
			for (int i = 0; i < count; i++)
			{
				int _maxLevel = GameData.Instance.MissionsGroup.CurrentMissionId;
				int levelToUnlock = Constants.FORMATION_SLOTUNLOCK(i + 1);
				HeroData item = formationData[i];
				HeroView heroView = currentFormation[i];
				SimpleButton btnSlot = btnSlots[i];
				if (_maxLevel - 1 >= levelToUnlock)
				{
					//Unlocked:
					btnSlotsTextes[i].gameObject.SetActive(false);
					btnSlot.image.color = Color.white;
					if (item == null)
					{
						heroView.gameObject.SetActive(false);
						btnSlot.gameObject.SetActive(true);
						btnSlot.enabled = true;
						btnSlot.transform.DOKill();
						btnSlot.transform.localScale = Vector3.one;
						//	btnSlot.image.color = Color.black;
						//btnSlot.rectTransform.DOScale(1.1f * Vector3.one, 1).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
					}
					else
					{
						heroView.gameObject.SetActive(true);
						heroView.isTOPHero = !pvPMainPanel.HaveOtherStrongerNotSelect(item);
						btnSlot.gameObject.SetActive(false);
						btnSlot.transform.DOKill();
						btnSlot.transform.localScale = Vector3.one;
						heroView.Init(item, pvPMainPanel);
					}
				}
				else
				{
					//Lock
					btnSlotsTextes[i].gameObject.SetActive(true);
					btnSlot.gameObject.SetActive(true);
					btnSlot.enabled = false;
					btnSlot.transform.DOKill();
					btnSlot.transform.localScale = Vector3.one;
					btnSlot.image.color = Color.gray;
					btnSlotsTextes[i].text = string.Format("UNLOCK AT\n{0}", GameData.Instance.MissionLevelToString(levelToUnlock));
					heroView.gameObject.SetActive(false);
				}
			}
		}

		public void RefreshFormation()
		{
			if (gameObject.activeInHierarchy)
				ShowCurrentFormation();

		}

		private void Action_BtnFight()
		{
			Config.PvpConfig.PayToPlay();
			StopAllCoroutines();
			gameObject.SetActive(false);
			pvPMainPanel.Back();
			Config.LogEvent(TrackingConstants.CLICK_PVP_MATCH_START);
		}

	}

}
