
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Utilities.Service.RFirebase;

using DG.Tweening;
using HedgehogTeam.EasyTouch;
using Spine.Unity;
using Utilities.Service;

namespace FoodZombie.UI
{
	public class HubPanel : MyGamesBasePanel
	{
		private static HubPanel mInstance;
		public static HubPanel Instance
		{
			get
			{
				if (mInstance == null)
					mInstance = FindObjectOfType<HubPanel>();
				return mInstance;
			}
		}

		public Image imgWaveInfo, imgWaveInfoIcon;
		public TextMeshProUGUI txtWaveInfo;

		public TextMeshProUGUI txtCoin;
		public Image imgIconCoin;
		public GameObject[] Panels_OfflineMode;
		public GameObject[] Panels_OnlineMode;
		public PvPScoreInGamePanel PvpScoreInGamePanel;
		public SimpleTMPButton btnPause, btnPauseOnline;
		public SimpleTMPButton btnSkip;
		public SimpleTMPButton btnAutoPlay;
		public SimpleTMPButton btnTimeScale;
		public Transform btnHeroSkillsPanel;
		List<HubHeroSkill> btnHeroSkills;


		public GameObject imgWarningBoss;
		public SimpleTMPButton btnForTutorial;

		[Separator("Trap")]
		//barrier
		public ButtonItemGameplay btnBarrier;
		public TextMeshProUGUI txtBarrierCount;
		public Image imgDragBarrier;
		public Image imgNotDragBarrier;
		//call
		public SimpleTMPButton btnCall;
		public TextMeshProUGUI txtCallCount;
		//mine
		public ButtonItemGameplay btnMine;
		public TextMeshProUGUI txtMineCount;
		public Image imgDragMine;
		public Image imgNotDragMine;
		//trap
		public ButtonItemGameplay btnTrap;
		public TextMeshProUGUI txtTrapCount;
		public Image imgDragTrap;
		public Image imgNotDragTrap;
		//first air kit
		public SimpleTMPButton btnFirstAirKit;
		public TextMeshProUGUI txtFirstAirKitCount;
		//ice grenade
		public ButtonItemGameplay btnIceGrenade;
		public TextMeshProUGUI txtIceGrenadeCount;
		public Image imgDragIceGrenade;
		public Image imgNotDragIceGrenade;
		//fire grenade
		public ButtonItemGameplay btnFireGrenade;
		public TextMeshProUGUI txtFireGrenadeCount;
		public Image imgDragFireGrenade;
		public Image imgNotDragFireGrenade;
		//electric grenade
		public ButtonItemGameplay btnElectricGrenade;
		public TextMeshProUGUI txtElectricGrenadeCount;
		public Image imgDragElectricGrenade;
		public Image imgNotDragElectricGrenade;

		[Separator("Tutorial")]
		public RectTransform imgZoneDragForTutorial;
		public BarrierTutorial barrierTutorial1;
		public BarrierTutorial barrierTutorial2;
		public BarrierTutorial barrierTutorial3;
		public AutoPlayTutorial autoplayTutorial;
		public bool initialized;

		private GameData GameData => GameData.Instance;
		private string missionName = "";

		private Vector2 posDragStart = Vector2.zero;

		private Sequence sequenceCoinFx;
		private Tweener tweenerCoinText;
		private int currentCoin = 0;


		[Header("Skill Cast")]
		public Image imgCastAoe;
		public Image imgNotCastAoe;

		[Space(10)]
		public Image imgWall;
		public Image[] imgCastWall;
		public Image imgNotCastWall;

		[Space(10)]
		public Image imgCastLine;
		public Image imgNotCastLine;

		[Space(10)]
		public GameObject objectBlockRayCast;
		public Image imgCastAim;
		public Image imgNotCastAim;
		
		private void Start()
		{
			btnPause.onClick.AddListener(BtnPause_Pressed);
			btnPauseOnline.onClick.AddListener(BtnPause_Pressed);
			btnSkip.onClick.AddListener(BtnSkip_Pressed);
			btnAutoPlay.onClick.AddListener(BtnAutoPlay_Pressed);
			btnTimeScale.onClick.AddListener(BtnTimeScale_Pressed);
			//trap
			btnBarrier.onDragStart.AddListener(BtnBarrier_OnDragStart);
			btnBarrier.onDrag.AddListener(BtnBarrier_OnDrag);
			btnBarrier.onDragEnd.AddListener(BtnBarrier_OnDragEnd);
			btnCall.onClick.AddListener(BtnCall_Pressed);
			btnMine.onDragStart.AddListener(BtnMine_OnDragStart);
			btnMine.onDrag.AddListener(BtnMine_OnDrag);
			btnMine.onDragEnd.AddListener(BtnMine_OnDragEnd);
			btnTrap.onDragStart.AddListener(BtnTrap_OnDragStart);
			btnTrap.onDrag.AddListener(BtnTrap_OnDrag);
			btnTrap.onDragEnd.AddListener(BtnTrap_OnDragEnd);
			btnFirstAirKit.onClick.AddListener(BtnFirstAirKit_Pressed);
			btnIceGrenade.onDragStart.AddListener(BtnIceGrenade_OnDragStart);
			btnIceGrenade.onDrag.AddListener(BtnIceGrenade_OnDrag);
			btnIceGrenade.onDragEnd.AddListener(BtnIceGrenade_OnDragEnd);
			btnFireGrenade.onDragStart.AddListener(BtnFireGrenade_OnDragStart);
			btnFireGrenade.onDrag.AddListener(BtnFireGrenade_OnDrag);
			btnFireGrenade.onDragEnd.AddListener(BtnFireGrenade_OnDragEnd);
			btnElectricGrenade.onDragStart.AddListener(BtnElectricGrenade_OnDragStart);
			btnElectricGrenade.onDrag.AddListener(BtnElectricGrenade_OnDrag);
			btnElectricGrenade.onDragEnd.AddListener(BtnElectricGrenade_OnDragEnd);
			btnForTutorial.onClick.AddListener(BtnForTutorial_Pressed);
		}

		internal override void Init()
		{
			MissionData missionData;
			foreach (GameObject item in Panels_OfflineMode)
			{
				item.SetActive(Config.typeModeInGame != Config.TYPE_MODE_PvP);
			}
			foreach (GameObject item in Panels_OnlineMode)
			{
				item.SetActive(Config.typeModeInGame == Config.TYPE_MODE_PvP);
			}

			if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
			{
				Debug.LogError("Edit Here=>Get PvP Data");
				PvpScoreInGamePanel.CallStart();
				missionData = GameData.Instance.MissionsGroup.GetCurrentMissionData();
			}
			else
			{
				if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
				{
					missionData = GameData.Instance.MissionsGroup.GetCurrentMissionData();
				}
				else
				{
					missionData = GameData.Instance.DiscoveriesGroup.GetCurrentMissionData();
				}
			}

			var waveCount = missionData.baseData.waveInfos.Count;
			txtWaveInfo.text = missionName + "     "+Localization.Get(Localization.WAVE)+ " 0/" + waveCount;
			imgWaveInfo.fillAmount = 0f;
			imgWaveInfoIcon.GetComponent<RectTransform>().localPosition = new Vector3(-355, 0, 0);

			//tut
			//Lv0:
			//Bỏ các Tut còn lại (Auto, xSpeed)
			//Bỏ tất cả các Boost Item
			//Trước khi hướng dẫn Player sử dụng skill Hero (tut 3) thì thanh nộ khí chưa chạy. Gần đến khi có tut thì mới unlock chức năng nộ khí.
			//Tut 3 (dùng skill) để cho quái xuống gần hơn chút nữa, để cho 1 số quái bắt đầu tấn công Player thì mới yêu cầu bấm skill (để thấy rõ hiệu quả của Skill hơn)
			//Thêm 2 lần Tut 3 cho 2 Hero có skill đẹp kế tiếp

			//Lv1:
			//Thêm tut Auto Skill ngay khi có ít nhất 1 Hero đủ nộ khí (thêm điều kiện thời gian hiện tut sớm nhất từ giây thứ 10)
			//Tăng độ khó để quái có thể xuống gần hơn biến hình và tấn công Player (mục đích tăng cao trào và thể hiện mechanic hay của game)
			//Thay đổi text = Auto Hero Skills

			//lv2:
			//Tăng độ khó để quái có thể xuống gần hơn biến hình và tấn công Player (mục đích tăng cao trào và thể hiện mechanic hay của game)
			//Thêm loại quái mới có vẻ nguy hiểm hơn (chạy nhanh) để đe dọa Player
			//Khi user bị nguy hiểm unlock chức năng item, hiện tut hướng dẫn Player dùng item chặn đường quái. Cho user dùng hẳn 3 item tường chắn quái để thể hiện hiệu quả của item.

			if (!GameData.Instance.MissionsGroup.IsWinIntroMission())
			{
				missionName = missionData.GetName();
				btnPause.SetActive(false);
				btnSkip.SetActive(true);
				btnAutoPlay.SetActive(false);
				btnTimeScale.SetActive(false);
			}
			else if (GameData.Instance.MissionsGroup.CurrentMissionId == 1001)
			{
				missionName = Localization.Get(Localization.ID.MISSION) + " " + missionData.GetName();
				btnPause.SetActive(false);
				btnSkip.SetActive(false);
				btnAutoPlay.SetActive(false);
				btnTimeScale.SetActive(true);
			}
			else if (GameData.Instance.MissionsGroup.CurrentMissionId == 1002)
			{
				missionName = Localization.Get(Localization.ID.MISSION)+" " + missionData.GetName();
				btnPause.SetActive(false);
				btnSkip.SetActive(false);
				btnAutoPlay.SetActive(false);
				btnTimeScale.SetActive(true);
			}
			else
			{
				if (Config.typeModeInGame == Config.TYPE_MODE_NORMAL)
				{
					missionName = Localization.Get(Localization.ID.MISSION) + " " + missionData.GetName();
				}
				else
				{
					missionName = missionData.GetName();
				}

				if (!GameUnityData.instance.GameRemoteConfig.Function_Autoplay_Active)
				{
					btnAutoPlay.SetActive(false);
					GameplayController.Instance.SetAutoPlay(false);
				}
				else
				{
					int _levelActiveAutoplay = GameUnityData.instance.GameRemoteConfig.Function_Autoplay_ActiveLevel;
					if (_levelActiveAutoplay <= 0)
						btnAutoPlay.SetActive(true);
					else
						btnAutoPlay.SetActive(GameData.Instance.MissionsGroup.CurrentMissionId >= _levelActiveAutoplay);
				}


				btnPause.SetActive(true);
				btnSkip.SetActive(false);
				if (Config.typeModeInGame == Config.TYPE_MODE_PvP)
				{
					if (GameUnityData.instance.GameRemoteConfig.Function_Autoplay_Active)
						btnAutoPlay.SetActive(true);
					else
					{
						btnAutoPlay.SetActive(false);
						GameplayController.Instance.SetAutoPlay(false);
					}
					btnTimeScale.SetActive(false);
				}
			}

			initialized = true;
		}

		public void ShowHero()
		{
			var heroControls = GameplayController.Instance.GetHeroExs();
			var count = heroControls.Count;

			if (btnHeroSkills == null || btnHeroSkills.Count == 0)
			{
				btnHeroSkills = new List<HubHeroSkill>();
				foreach (Transform item in btnHeroSkillsPanel)
				{
					HubHeroSkill heroButton = item.GetComponent<HubHeroSkill>();
					btnHeroSkills.Add(heroButton);
				}
			}
			for (int i = 0; i < btnHeroSkills.Count; i++)
			{
				if (i >= count)
				{
					btnHeroSkills[i].SetActive(false);
					continue;
				}
				int index = i;
				var btnHeroSkill = btnHeroSkills[index];

				var heroControl = heroControls[index];
				heroControl.heroHubSkill = btnHeroSkill;
				btnHeroSkill.SetRankColor(heroControl.heroData.Rank);
				btnHeroSkill.Image_HeroAvatar.sprite = heroControl.heroData.GetIcon();
				//btnHeroSkill.SetTypeSkill(heroControl.typeSkill,()=> BtnHeroSkill_Pressed(heroControl));
				btnHeroSkill.SetTypeSkill(heroControl.typeSkill,heroControl);

				//btnHeroSkill.Button.onClick.AddListener(() =>
				//{
				//	BtnHeroSkill_Pressed(heroControl);
				//});
				
				var heroPos = heroControl.transform.position;
				btnHeroSkill.transform.position = new Vector3(heroPos.x, heroPos.y + 0.25f, 0f);
				btnHeroSkill.SetActive(true);
			}
		}

		private void BtnPause_Pressed()
		{
			MainGamePanel.instance.ShowPausePanel();
		}

		private void BtnSkip_Pressed()
		{
			GameplayController.Instance.Skip();
			Config.LogEvent(TrackingConstants.CLICK_SKIP_INTRO);
		}

		private void BtnAutoPlay_Pressed()
		{
			GameplayController.Instance.ChangeAutoPlay();
		}

		private void BtnTimeScale_Pressed()
		{
			GameplayController.Instance.ChangeTimeScale();
		}

		//Trap
		private void BtnBarrier_OnDragStart(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			posDragStart = btnBarrier.transform.position;
			imgDragBarrier.rectTransform.anchoredPosition = pos * 100f;
			imgDragBarrier.SetActive(true);
			imgDragBarrier.color = Color.white;
		}

		private void BtnBarrier_OnDrag(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			if (pos.y > 3f || pos.y < -4.6f || Vector2.Distance(pos, posDragStart) <= 0.45f)
			{
				imgNotDragBarrier.SetActive(true);
				var c = Color.red;
				c.a = 0.5f;
				imgDragBarrier.color = c;
			}
			else
			{
				imgNotDragBarrier.SetActive(false);
				imgDragBarrier.color = Color.white;
			}
			imgDragBarrier.rectTransform.anchoredPosition = pos * 100f;
		}

		private void BtnBarrier_OnDragEnd(Gesture gesture)
		{
			imgDragBarrier.SetActive(false);
			if (imgNotDragBarrier.gameObject.activeSelf) return;

			if (Vector2.Distance(gesture.position, gesture.startPosition) < 100.0f)
				return;
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			GameplayController.Instance.AddBarrier(pos);
		}

		private void BtnCall_Pressed()
		{
			GameplayController.Instance.Call();
		}

		private void BtnMine_OnDragStart(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			posDragStart = btnMine.transform.position;
			imgDragMine.rectTransform.anchoredPosition = pos * 100f;
			imgDragMine.SetActive(true);
			imgDragMine.color = Color.white;
		}

		private void BtnMine_OnDrag(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			if (pos.y < -4.6f || Vector2.Distance(pos, posDragStart) <= 0.45f)
			{
				imgNotDragMine.SetActive(true);
				var c = Color.red;
				c.a = 0.5f;
				imgDragMine.color = c;
			}
			else
			{
				imgNotDragMine.SetActive(false);
				imgDragMine.color = Color.white;
			}
			imgDragMine.rectTransform.anchoredPosition = pos * 100f;
		}

		private void BtnMine_OnDragEnd(Gesture gesture)
		{
			imgDragMine.SetActive(false);
			if (imgNotDragMine.gameObject.activeSelf) return;

			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			GameplayController.Instance.AddMine(pos);
		}

		private void BtnTrap_OnDragStart(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			posDragStart = btnTrap.transform.position;
			imgDragTrap.rectTransform.anchoredPosition = pos * 100f;
			imgDragTrap.SetActive(true);
			imgDragTrap.color = Color.white;
		}

		private void BtnTrap_OnDrag(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			if (pos.y < -4.6f || Vector2.Distance(pos, posDragStart) <= 0.45f)
			{
				imgNotDragTrap.SetActive(true);
				var c = Color.red;
				c.a = 0.5f;
				imgDragTrap.color = c;
			}
			else
			{
				imgNotDragTrap.SetActive(false);
				imgDragTrap.color = Color.white;
			}
			imgDragTrap.rectTransform.anchoredPosition = pos * 100f;
		}

		private void BtnTrap_OnDragEnd(Gesture gesture)
		{
			imgDragTrap.SetActive(false);
			if (imgNotDragTrap.gameObject.activeSelf) return;

			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			GameplayController.Instance.AddTrap(pos);
		}

		private void BtnIceGrenade_OnDragStart(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			posDragStart = btnIceGrenade.transform.position;
			imgDragIceGrenade.rectTransform.anchoredPosition = pos * 100f;
			imgDragIceGrenade.SetActive(true);
			imgDragIceGrenade.color = Color.white;
		}

		private void BtnIceGrenade_OnDrag(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			if (pos.y < -4.6f || Vector2.Distance(pos, posDragStart) <= 0.45f)
			{
				imgNotDragIceGrenade.SetActive(true);
				var c = Color.red;
				c.a = 0.5f;
				imgDragIceGrenade.color = c;
			}
			else
			{
				imgNotDragIceGrenade.SetActive(false);
				imgDragIceGrenade.color = Color.white;
			}
			imgDragIceGrenade.rectTransform.anchoredPosition = pos * 100f;
		}

		private void BtnIceGrenade_OnDragEnd(Gesture gesture)
		{
			imgDragIceGrenade.SetActive(false);
			if (imgNotDragIceGrenade.gameObject.activeSelf) return;

			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			GameplayController.Instance.AddIceGrenade(pos);
		}

		private void BtnFireGrenade_OnDragStart(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			posDragStart = btnFireGrenade.transform.position;
			imgDragFireGrenade.rectTransform.anchoredPosition = pos * 100f;
			imgDragFireGrenade.SetActive(true);
			imgDragFireGrenade.color = Color.white;
		}

		private void BtnFireGrenade_OnDrag(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			if (pos.y < -4.6f || Vector2.Distance(pos, posDragStart) <= 0.45f)
			{
				imgNotDragFireGrenade.SetActive(true);
				var c = Color.red;
				c.a = 0.5f;
				imgDragFireGrenade.color = c;
			}
			else
			{
				imgNotDragFireGrenade.SetActive(false);
				imgDragFireGrenade.color = Color.white;
			}
			imgDragFireGrenade.rectTransform.anchoredPosition = pos * 100f;
		}

		private void BtnFireGrenade_OnDragEnd(Gesture gesture)
		{
			imgDragFireGrenade.SetActive(false);
			if (imgNotDragFireGrenade.gameObject.activeSelf) return;

			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			GameplayController.Instance.AddFireGrenade(pos);
		}

		private void BtnElectricGrenade_OnDragStart(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			posDragStart = btnElectricGrenade.transform.position;
			imgDragElectricGrenade.rectTransform.anchoredPosition = pos * 100f;
			imgDragElectricGrenade.SetActive(true);
			imgDragElectricGrenade.color = Color.white;
		}

		private void BtnElectricGrenade_OnDrag(Gesture gesture)
		{
			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			if (pos.y < -4.6f || Vector2.Distance(pos, posDragStart) <= 0.45f)
			{
				imgNotDragElectricGrenade.SetActive(true);
				var c = Color.red;
				c.a = 0.5f;
				imgDragElectricGrenade.color = c;
			}
			else
			{
				imgNotDragElectricGrenade.SetActive(false);
				imgDragElectricGrenade.color = Color.white;
			}
			imgDragElectricGrenade.rectTransform.anchoredPosition = pos * 100f;
		}

		private void BtnElectricGrenade_OnDragEnd(Gesture gesture)
		{
			imgDragElectricGrenade.SetActive(false);
			if (imgNotDragElectricGrenade.gameObject.activeSelf) return;

			var pos = gesture.GetTouchToWorldPoint(gesture.position);
			GameplayController.Instance.AddElectricGrenade(pos);
		}

		private void BtnFirstAirKit_Pressed()
		{
			GameplayController.Instance.FirstAirKit();
		}

		public void ShowTextAutoPlay()
		{
			// if (GameplayController.Instance.autoPlay) txtAutoPlay.text = "Control";
			// else txtAutoPlay.text = "Auto";
			btnAutoPlay.GetComponent<ButtonHelper>().ChangeValue(GameplayController.Instance.autoPlay);
		}

		private void BtnHeroSkill_Pressed(HeroControl heroControl)
		{
			heroControl.BtnSkill_Pressed();
		}

		public void ShowWaveInfo(int index, int count)
		{
			txtWaveInfo.text = missionName + "     "+Localization.Get(Localization.WAVE)+" " + index + "/" + count;
		}

		public void UpdateWaveInfo(int current, int total)
		{
			//đoạn này chỉnh tí cho đẹp
			var percent = (current + 1) * 1f / total;
			if (percent < 0.02055f) percent = 0.02055f;
			else if (percent > 1f) percent = 1f;

			imgWaveInfo.fillAmount = percent * 0.915f; //(710 - 60) / 710
			var delta = (imgWaveInfo.rectTransform.sizeDelta.x - 60f) * percent;
			if (delta < 0f) delta = 0f;
			imgWaveInfoIcon.GetComponent<RectTransform>().localPosition = new Vector3(-355f + delta, 0f, 0f);
		}
		private void BtnForTutorial_Pressed()
		{
			GameplayController.Instance.tapToTarget.SetTarget(btnForTutorial.transform.position);
			btnForTutorial.SetActive(false);
		}
		public SimpleTMPButton GetBtnForTutorial()
		{
			btnForTutorial.SetActive(true);
			var enemies = GameplayController.Instance.GetEnemies();
			var count = enemies.Count;
			var pos = new Vector3(0f, 0f, OderLayerZ.PIVOT_POINT);
			for (int i = 0; i < count; i++)
			{
				var enemy = enemies[i];
				if (!enemy.IsDead())
				{
					pos = enemy.transform.position + (Vector3)enemy.Offset;
					if (pos.y <= 5f)
					{
						btnForTutorial.transform.position = pos;
						return btnForTutorial;
					}
				}
			}

			return null;
		}

		public SimpleTMPButton GetBtnHeroSkill(int index)
		{
			var enemies = GameplayController.Instance.GetEnemies();
			var count = enemies.Count;
			Vector3 pos;
			for (int i = 0; i < count; i++)
			{
				var enemy = enemies[i];
				if (!enemy.IsDead())
				{
					pos = enemy.transform.position /*+ (Vector3) enemy.Offset*/;
					if (pos.y <= -2.75f)
					{
						return btnHeroSkills[index].Button;
					}
				}
			}

			return null;
		}

		public SimpleTMPButton GetBtnHeroSkillHard(int index)
		{
			return btnHeroSkills[index].Button;
		}

		public bool HaveHeroSkillFull()
		{
			foreach (HubHeroSkill item in btnHeroSkills)
			{
				if (item.gameObject.activeSelf && item.isFull()) return true;
			}
			return false;
		}

		private void ShowTrapData(TrapData trapData, TextMeshProUGUI txtCount, GameObject btn)
		{
			//if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_BASE || !trapData.Unlocked)
			if (GameData.Instance.LevelUnlockContent < Constants.UNLOCK_BASE)
			{
				btn.SetActive(false); return;
			}
			var trapCount = trapData.StockNumber;
			txtCount.text = "" + trapCount;
			if (trapCount <= 0) btn.SetActive(false);
			else btn.SetActive(true);
		}

		public void ShowBarrierCount(TrapData trapData)
		{
			ShowTrapData(trapData, txtBarrierCount, btnBarrier.gameObject);
		}

		public void ShowCallCount(TrapData trapData)
		{
			ShowTrapData(trapData, txtCallCount, btnCall.gameObject);
		}

		public void ShowMineCount(TrapData trapData)
		{
			ShowTrapData(trapData, txtMineCount, btnMine.gameObject);
		}

		public void ShowTrapCount(TrapData trapData)
		{
			ShowTrapData(trapData, txtTrapCount, btnTrap.gameObject);
		}

		public void ShowFirstAirKitCount(TrapData trapData)
		{
			ShowTrapData(trapData, txtFirstAirKitCount, btnFirstAirKit.gameObject);
		}

		public void ShowIceGrenadeCount(TrapData trapData)
		{
			ShowTrapData(trapData, txtIceGrenadeCount, btnIceGrenade.gameObject);
		}

		public void ShowFireGrenadeCount(TrapData trapData)
		{
			ShowTrapData(trapData, txtFireGrenadeCount, btnFireGrenade.gameObject);
		}

		public void ShowElectricGrenadeCount(TrapData trapData)
		{
			ShowTrapData(trapData, txtElectricGrenadeCount, btnElectricGrenade.gameObject);
		}

		public void ShowCoin(int coin)
		{
			if (coin == 0) return;

			if (sequenceCoinFx != null) sequenceCoinFx.Kill();
			txtCoin.transform.localScale = Vector3.one;

			sequenceCoinFx = DOTween.Sequence();
			sequenceCoinFx.SetDelay(0.24f);
			var groupCoin = txtCoin.transform.parent;
			var tweener = groupCoin.DOScale(1.3f, 0.2f).SetEase(Ease.InBounce);
			sequenceCoinFx.Append(tweener);
			tweener = groupCoin.DOScale(1f, 0.2f);
			sequenceCoinFx.Append(tweener);
			sequenceCoinFx.Play();

			if (tweenerCoinText != null) tweenerCoinText.Kill();

			int oldCoin = currentCoin;
			currentCoin = coin;
			float alpha = 0f;
			tweenerCoinText = DOTween.To(tweenVal => alpha = tweenVal, oldCoin, currentCoin, 0.38f)
				.SetDelay(0.24f)
				.OnUpdate(() =>
				{
					txtCoin.text = alpha.ToString("0");
				})
				.OnComplete(() =>
				{
					txtCoin.text = currentCoin + "";
				});
		}

		public Vector3 GetCoinIconPos()
		{
			return imgIconCoin.transform.position;
		}

		public void WarningBoss()
		{
			imgWarningBoss.SetActive(true);
		}
	}
}