using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
	public class TutorialLevelUpHero : TutorialController
	{
		private JustButton btnPlay;
		private JustButton btnOneClickEquip;
		private bool pressedBtn;

		public TutorialLevelUpHero(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
		{
		}

		public override void Start()
		{
			base.Start();
		}

		public override void Pause()
		{
		}

		public override void Resume()
		{
		}

		protected override void OnAnyChildHide(MyGamesBasePanel pPanel)
		{
		}

		protected override void OnAnyChildShow(MyGamesBasePanel pPanel)
		{

		}

		public override IEnumerator IEProcess()
		{
			yield return new WaitUntil(() => (MainPanel != null && MainPanel.MainMenuPanel != null
																&& (MainPanel.TopPanel is MainMenuPanel)));
			string s = "";
			MessageWithPointer board;
			//if (MainPanel.TopPanel is FormationPanel)
			//{
			//	var btnBack = MainPanel.FormationPanel.btnHero.rectTransform();
			//	TutorialsManager.InputMasker.Lock();
			//	//s = "Okay, you've completed your first x10 summon. Now we have quite a few heroes.";
			//	s = Localization.Get(Localization.ID.TUTORIAL_73);
			//	board = MainPanel.ShowNotificationBoard(btnBack, s, Alignment.Top, new Vector2(144f, -315f), false);
			//	board.transform.SetParent(TutorialsManager.transform);
			//	yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
			//	TutorialsManager.InputMasker.FocusToTargetImmediately(btnBack);
			//	board = MainPanel.ShowNotificationBoard(btnBack, "", Alignment.Top, new Vector2(144f, -315f));
			//	board.transform.SetParent(TutorialsManager.transform);
			//}

			while (!(MainPanel.TopPanel is MainMenuPanel))
			{
				MainPanel.Back();
			}

			//=== step 1:
			//Focus to hero button
			var btnHero = MainPanel.MainMenuPanel.btnHero.rectTransform;

			TutorialsManager.InputMasker.Lock();
			//s = "Click the heroes tab to open the Hero screen.";
			//s = Localization.Get(Localization.ID.TUTORIAL_88);
			//board = MainPanel.ShowNotificationBoard(btnHero, s, Alignment.Bot, new Vector2(144f, -515f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			TutorialsManager.InputMasker.FocusToTargetImmediately(btnHero);
			board = MainPanel.ShowNotificationBoard(btnHero, "", Alignment.Top, new Vector2(144f, -515f));
			board.transform.SetParent(TutorialsManager.transform);

			yield return new WaitUntil(() => MainPanel.HeroPanel.IsActiveAndEnabled());
			//đợi cho đến khi popup hero hiện lên thì khóa popup
			TutorialsManager.LockPanel(MainPanel.HeroPanel);

			//=== step 1:
			//Focus to material cost
			var txtLevelUpExpHeroCost = MainPanel.HeroPanel.txtLevelUpExpHeroCost.transform.parent.rectTransform();
			TutorialsManager.InputMasker.FocusToTargetImmediately(txtLevelUpExpHeroCost);
			//string s = "Each time you win the game, you will receive Hero Exp...";
			//s = Localization.Get(Localization.ID.TUTORIAL_39);
			//board = MainPanel.ShowNotificationBoard(txtLevelUpExpHeroCost, s, Alignment.Left, new Vector2(144f, 335f));
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT + 0.35f);

			//=== step 2:
			//Focus to coin cost
			//var txtLevelUpCoinCost = MainPanel.HeroPanel.txtLevelUpCoinCost.transform.parent.rectTransform();
			//TutorialsManager.InputMasker.FocusToTargetImmediately(txtLevelUpCoinCost);
			////s = "...and Coin";
			//s = Localization.Get(Localization.ID.TUTORIAL_40);
			//board = MainPanel.ShowNotificationBoard(txtLevelUpCoinCost, s, Alignment.Left, new Vector2(144f, 335f));
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT + 0.45f);

			////=== step 3:
			var btnLevelUp = MainPanel.HeroPanel.btnLevelUp.rectTransform;
			//TutorialsManager.InputMasker.Lock();
			////s = "Use them to upgrade your Hero.";
			//s = Localization.Get(Localization.ID.TUTORIAL_41);
			//board = MainPanel.ShowNotificationBoard(btnLevelUp, s, Alignment.Left, new Vector2(144f, 335f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			TutorialsManager.InputMasker.Lock();
			//s = "Now try to upgrade your Hero to level 5. You can tap or hold button";
			//s = Localization.Get(Localization.ID.TUTORIAL_42);
			//board = MainPanel.ShowNotificationBoard(btnLevelUp, s, Alignment.Left, new Vector2(144f, 335f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			//Focus to level up button
			TutorialsManager.InputMasker.FocusToTargetImmediately(btnLevelUp);
			board = MainPanel.ShowNotificationBoard(btnLevelUp, "", Alignment.Left, new Vector2(144f, 335f));
			board.transform.SetParent(TutorialsManager.transform);

			var heroView = MainPanel.HeroPanel.heroDataView;

			//chờ khi không nâng cấp đc, hoặc khi levelUnlock >= 5 và đến khi thả ngón tay (đang hold nút levelup)
			yield return new WaitUntil(() => MainPanel.MessagesPopup.IsActiveAndEnabled()
											 || (heroView.levelUnlocked >= 5));
			GameData.Instance.TutorialsGroup.Finish(id);
			MainPanel.HeroPanel.btnLevelUp.SetEnable(false);
			MainPanel.HeroPanel.btnLevelUpHold = false;
			yield return null;

			var togGears = MainPanel.HeroPanel.togGears.rectTransform();
			TutorialsManager.InputMasker.Lock();
			//s = "Very well, in addition, you can also help hero become stronger by equipping them with Gears";
			//s = Localization.Get(Localization.ID.TUTORIAL_43);
			//board = MainPanel.ShowNotificationBoard(togGears, s, Alignment.Left, new Vector2(144f, 335f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			//TutorialsManager.InputMasker.Lock();
			////s = "Click the Gear tab";
			//s = Localization.Get(Localization.ID.TUTORIAL_44);
			//board = MainPanel.ShowNotificationBoard(togGears, s, Alignment.Left, new Vector2(144f, 335f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			//focus gear tab
			TutorialsManager.InputMasker.FocusToTargetImmediately(togGears);
			board = MainPanel.ShowNotificationBoard(togGears, "", Alignment.Left, new Vector2(144f, 335f));
			board.transform.SetParent(TutorialsManager.transform);
			yield return new WaitUntil(() => MainPanel.HeroPanel.tabGears.activeSelf);

			//=== step 2:
			btnOneClickEquip = MainPanel.HeroPanel.btnOneClickEquip;
			TutorialsManager.InputMasker.Lock();
			//s = "Then choose One Click Equip, the best equipment will be used on your Hero";
			//s = Localization.Get(Localization.ID.TUTORIAL_45);
			//board = MainPanel.ShowNotificationBoard(btnOneClickEquip.rectTransform, s, Alignment.Left, new Vector2(144f, 335f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			//Focus to one key equip all
			TutorialsManager.InputMasker.FocusToTargetImmediately(btnOneClickEquip.rectTransform);
			board = MainPanel.ShowNotificationBoard(btnOneClickEquip.rectTransform, "", Alignment.Top, new Vector2(144f, 335f));
			board.transform.SetParent(TutorialsManager.transform);

			btnOneClickEquip.onClick.RemoveListener(OnBtnOneClickEquip_Pressed);
			btnOneClickEquip.onClick.AddListener(OnBtnOneClickEquip_Pressed);
			yield return new WaitUntil(() => pressedBtn);

			//=== step 3:
			var btnMain = MainPanel.HeroPanel.btnMain.rectTransform;
			TutorialsManager.InputMasker.Lock();
			//s = "Now try to fight with new powers.";
			//s = Localization.Get(Localization.ID.TUTORIAL_18);
			//board = MainPanel.ShowNotificationBoard(btnMain, s, Alignment.Bot, new Vector2(144f, 335f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSeconds(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			////close hero popup
			TutorialsManager.InputMasker.FocusToTargetImmediately(btnMain);
			board = MainPanel.ShowNotificationBoard(btnMain, "", Alignment.Top, new Vector2(144f, 335f), true);
			board.transform.SetParent(TutorialsManager.transform);

			TutorialsManager.UnlockPanel(MainPanel.HeroPanel);
			yield return new WaitUntil(() => !MainPanel.HeroPanel.IsActiveAndEnabled());



			if (GameUnityData.instance.GameRemoteConfig.Function_ShowMap)
			{
				////=== step 8:
				////open map popup
				var btnFight = MainPanel.MainMenuPanel.btnFight;
				TutorialsManager.InputMasker.FocusToTargetImmediately(btnFight.rectTransform);
				board = MainPanel.ShowNotificationBoard(btnFight.rectTransform, "", Alignment.Top, new Vector2(144f, 335f), true, false);
				board.transform.SetParent(TutorialsManager.transform);

				yield return new WaitUntil(() => MainPanel.MapPanel.IsActiveAndEnabled());
				TutorialsManager.LockPanel(MainPanel.MapPanel);

				////=== step 9:
				//open mission detail popup
				JustButton btnMissionPlay = null;
				var btnMissions = MainPanel.MapPanel.btnMissions;
				var count = btnMissions.Length;
				for (int i = 0; i < count; i++)
				{
					var btnMission = btnMissions[i];
					if (btnMission.btnPlay.Enabled())
					{
						btnMissionPlay = btnMission.btnPlay;
						break;
					}
				}
				TutorialsManager.InputMasker.FocusToTargetImmediately(btnMissionPlay.rectTransform);
				board = MainPanel.ShowNotificationBoard(btnMissionPlay.rectTransform, "", Alignment.Bot, new Vector2(144f, -215f), true, false);
				board.transform.SetParent(TutorialsManager.transform);

				yield return new WaitUntil(() => MainPanel.MissionDetailPanel.IsActiveAndEnabled());
				TutorialsManager.LockPanel(MainPanel.MissionDetailPanel);

				// //=== step 10:
				// //choice formation
				// var btnChange = MainPanel.MissionDetailPanel.btnChange;
				// TutorialsManager.InputMasker.FocusToTargetImmediately(btnChange.rectTransform);
				// board = MainPanel.ShowNotificationBoard(btnChange.rectTransform, "", Alignment.Top, new Vector2(480, 150));
				// board.transform.SetParent(TutorialsManager.transform);
				//
				// yield return new WaitUntil(() => MainPanel.MissionDetailPanel.groupFormation.activeSelf);
				//
				// //=== step 10:
				// //choice formation toggle
				// pressedBtn = false;
				// btnChoiceFormation = MainPanel.MissionDetailPanel.btnChoiceFormations[0];
				// TutorialsManager.InputMasker.FocusToTargetImmediately(btnChoiceFormation.rectTransform);
				// board = MainPanel.ShowNotificationBoard(btnChoiceFormation.rectTransform, "", Alignment.BotRight, new Vector2(480, 150));
				// board.transform.SetParent(TutorialsManager.transform);
				//
				// btnChoiceFormation.onClick.RemoveListener(OnBtnChoiceFormation_Pressed);
				// btnChoiceFormation.onClick.AddListener(OnBtnChoiceFormation_Pressed);
				// yield return new WaitUntil(() => pressedBtn);
				// yield return null;
				//
				// //=== step 11:
				// //choice formation save
				// var btnSaveFormation = MainPanel.MissionDetailPanel.btnSaveFormation;
				// TutorialsManager.InputMasker.FocusToTargetImmediately(btnSaveFormation.rectTransform);
				// board = MainPanel.ShowNotificationBoard(btnSaveFormation.rectTransform, "", Alignment.Bot, new Vector2(480, 150));
				// board.transform.SetParent(TutorialsManager.transform);
				//
				// yield return new WaitUntil(() => !MainPanel.MissionDetailPanel.groupFormation.activeSelf);

				//=== step 11:
				//PLAY MISSIONNNNNNNNNNNNNNNN
				pressedBtn = false;
				btnPlay = MainPanel.MissionDetailPanel.btnPlay;
				TutorialsManager.InputMasker.FocusToTargetImmediately(btnPlay.rectTransform);
				board = MainPanel.ShowNotificationBoard(btnPlay.rectTransform, "", Alignment.Bot, new Vector2(144f, -215f), true, false);
				board.transform.SetParent(TutorialsManager.transform);

				btnPlay.onClick.RemoveListener(OnBtnPlay_Pressed);
				btnPlay.onClick.AddListener(OnBtnPlay_Pressed);

				//yield return new WaitUntil(() => !MainPanel.MissionDetailPanel.groupFormation.activeSelf);
				//Khong PlayMission thi End luon vay
				//End(true);
			}
			else
			{
				TutorialsManager.ShowPlayOnGameplay = true;
				pressedBtn = false;
				var btnFight = MainPanel.MainMenuPanel.btnFight;
				TutorialsManager.InputMasker.FocusToTargetImmediately(btnFight.rectTransform);
				board = MainPanel.ShowNotificationBoard(btnFight.rectTransform, "", Alignment.Top, new Vector2(144f, 335f), true, false);
				board.transform.SetParent(TutorialsManager.transform);
				btnFight.onClick.AddListener(OnBtnBattle_Pressed);
				yield return new WaitUntil(() => pressedBtn);
				End(true);
			}

		}

		public override void End(bool pFinished)
		{
			base.End(pFinished);

			MainPanel.UITooltips.LockOptionsGrid(false);

			//Unlock input
			TutorialsManager.InputMasker.Active(false);

			//Hide notification board
			MainPanel.HideNotificationBoard(0);

			//Unlock panel
			TutorialsManager.UnlockPanel(MainPanel.HeroPanel);
		}

		private void OnBtnOneClickEquip_Pressed()
		{
			pressedBtn = true;
			btnOneClickEquip.onClick.RemoveListener(OnBtnOneClickEquip_Pressed);
		}

		private void OnBtnPlay_Pressed()
		{
			End(true);
			btnOneClickEquip.onClick.RemoveListener(OnBtnPlay_Pressed);
		}
		private void OnBtnBattle_Pressed()
		{
			pressedBtn = true;
			//	End(true);
			//btnOneClickEquip.onClick.RemoveListener(OnBtnPlay_Pressed);
		}
	}
}
