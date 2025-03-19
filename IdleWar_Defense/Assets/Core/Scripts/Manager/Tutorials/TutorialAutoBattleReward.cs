using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

namespace FoodZombie.UI
{
	public class TutorialAutoBattleReward : TutorialController
	{
		public TutorialAutoBattleReward(int pId, bool pIsToolTips) : base(pId, pIsToolTips)
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
																		&& MainPanel.TopPanel is MainMenuPanel));

			while (!(MainPanel.TopPanel is MainMenuPanel))
			{
				MainPanel.Back();
			}

			//
			var btnTimeCollect = MainPanel.MainMenuPanel.btnTimeCollect.rectTransform;
			////Focus to summon button
			TutorialsManager.InputMasker.Lock();
			//string s = "The Auto Battle Reward feature is unlocked.";
			//string s = Localization.Get(Localization.ID.TUTORIAL_89);
			//var board = MainPanel.ShowNotificationBoard(btnTimeCollect, s, Alignment.Bot, new Vector2(144f, 630f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			//TutorialsManager.InputMasker.Lock();
			//s = "Here you can obtain a large amount of resources even if you are not online.";
			//board = MainPanel.ShowNotificationBoard(btnTimeCollect, s, Alignment.Bot, new Vector2(144f, 630f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			//TutorialsManager.InputMasker.Lock();
			//s = "You will be collecting resources every minute and will be full within 8 hours.";
			//board = MainPanel.ShowNotificationBoard(btnTimeCollect, s, Alignment.Bot, new Vector2(144f, 630f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			TutorialsManager.InputMasker.Lock();
			//s = "Click the Box button right above the Battle button to check the amount of collected resources.";
			//s = Localization.Get(Localization.ID.TUTORIAL_87);
			//board = MainPanel.ShowNotificationBoard(btnTimeCollect, s, Alignment.Bot, new Vector2(144f, 630f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			TutorialsManager.InputMasker.FocusToTargetImmediately(btnTimeCollect);
			var board = MainPanel.ShowNotificationBoard(btnTimeCollect, "", Alignment.Bot, new Vector2(144f, 630f));
			board.transform.SetParent(TutorialsManager.transform);

			yield return new WaitUntil(() => MainPanel.AFKRewardPanel.IsActiveAndEnabled());
			//đợi cho đến khi popup summon hiện lên thì khóa popup
			TutorialsManager.LockPanel(MainPanel.AFKRewardPanel);

			//
			//	string s;
			//	MessageWithPointer board;

			// {
			var btnCollect = MainPanel.AFKRewardPanel.btnCollect.rectTransform;

			TutorialsManager.InputMasker.Lock();
			yield return new WaitForEndOfFrame();
			//s = "Wellcome to the Auto Battle Reward.Here you can obtain a large amount of resources even if you are not online";
			//s = Localization.Get(Localization.ID.TUTORIAL_1);
			//board = MainPanel.ShowNotificationBoard(btnCollect, s, Alignment.Bot, new Vector2(144f, 630f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
			//yield return new WaitForSecondsRealtime(1);
			//TutorialsManager.InputMasker.Lock();

			////s = "You will be collecting resources every minute and will be full within 8 hours";
			//s = Localization.Get(Localization.ID.TUTORIAL_2);
			//board = MainPanel.ShowNotificationBoard(btnCollect, s, Alignment.Bot, new Vector2(144f, 630f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
			//yield return new WaitForSecondsRealtime(1);
			//TutorialsManager.InputMasker.Lock();

			////s = "The amount of collected resources will be displayed here. Press collect to collect all.";
			//s = Localization.Get(Localization.ID.TUTORIAL_3);
			//board = MainPanel.ShowNotificationBoard(btnCollect, s, Alignment.Bot, new Vector2(144f, 630f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
			//yield return new WaitForSecondsRealtime(1);
			//TutorialsManager.InputMasker.Lock();

			////s = "Don't forget to watch the promotional video because it will help you get 150% more resources.";
			//s = Localization.Get(Localization.ID.TUTORIAL_4);
			//board = MainPanel.ShowNotificationBoard(btnCollect, s, Alignment.Bot, new Vector2(144f, 630f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);
			//yield return new WaitForSecondsRealtime(1);

			TutorialsManager.UnlockPanel(MainPanel.AFKRewardPanel);

			TutorialsManager.InputMasker.FocusToTargetImmediately(btnCollect);
			board = MainPanel.ShowNotificationBoard(btnCollect, "", Alignment.Top, new Vector2(144f, 630f));
			board.transform.SetParent(TutorialsManager.transform);


			List<RewardInfo> rewards = MainPanel.AFKRewardPanel.rewardsInfos;
			if (rewards.Count > 0)
			{
				yield return new WaitUntil(() => MainPanel.RewardsPopup.IsActiveAndEnabled());
				End(false);
				yield return null;
				yield return new WaitUntil(() => !MainPanel.RewardsPopup.IsActiveAndEnabled());
			}
			else
			{
				yield return new WaitUntil(() => !MainPanel.AFKRewardPanel.IsActiveAndEnabled());
				End(false);
				yield return null;
			}

			// }
			//
			if (!GameData.Instance.MissionsGroup.FreeTicket)
			{
				GameData.Instance.MissionsGroup.SetFreeTicket();
			}

			var btnTimeBuff = MainPanel.MainMenuPanel.btnTimeBuff.rectTransform;

			TutorialsManager.InputMasker.Lock();
			//s = "Okay, what if the times when you need the resources urgently that afk collect are not enough?";
			//s = Localization.Get(Localization.ID.TUTORIAL_5);
			//board = MainPanel.ShowNotificationBoard(btnTimeBuff, s, Alignment.Bot, new Vector2(144f, -65f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			//TutorialsManager.InputMasker.Lock();
			////s = "Use the Fast Collect feature. Now I will instruct you to use it.";
			//s = Localization.Get(Localization.ID.TUTORIAL_6);
			//board = MainPanel.ShowNotificationBoard(btnTimeBuff, s, Alignment.Bot, new Vector2(144f, -65f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			TutorialsManager.InputMasker.FocusToTargetImmediately(btnTimeBuff);
			board = MainPanel.ShowNotificationBoard(btnTimeBuff, "", Alignment.Top, new Vector2(144f, -65f));
			board.transform.SetParent(TutorialsManager.transform);

			yield return new WaitUntil(() => MainPanel.FastCollectPanel.IsActiveAndEnabled());
			//đợi cho đến khi popup summon hiện lên thì khóa popup
			TutorialsManager.LockPanel(MainPanel.FastCollectPanel);

			//
			var btnUse = MainPanel.FastCollectPanel.btnUse.rectTransform;

			TutorialsManager.InputMasker.Lock();
			//s = "Fast collect can provide you with afk resources for 2 hours instantly.";
			//s = Localization.Get(Localization.ID.TUTORIAL_7);
			//board = MainPanel.ShowNotificationBoard(btnUse, s, Alignment.Bot, new Vector2(144f, 555f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			TutorialsManager.InputMasker.Lock();
			//s = "Using fast collect requires a ticket. You can earn tickets through quests, stores, and event rewards.";
			//s = Localization.Get(Localization.ID.TUTORIAL_8);
			//board = MainPanel.ShowNotificationBoard(btnUse, s, Alignment.Bot, new Vector2(144f, 555f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			//TutorialsManager.InputMasker.Lock();
			////s = "Here, this time is FREE";
			//s = Localization.Get(Localization.ID.TUTORIAL_9);
			//board = MainPanel.ShowNotificationBoard(btnUse, s, Alignment.Bot, new Vector2(144f, 555f), false);
			//board.transform.SetParent(TutorialsManager.transform);
			//yield return new WaitForSecondsRealtime(s.Length * Config.TIME_TUTORIAL_TEXT_RUN / 8f + Config.TIME_TUTORIAL_TEXT_WAIT);

			TutorialsManager.UnlockPanel(MainPanel.FastCollectPanel);

			TutorialsManager.InputMasker.FocusToTargetImmediately(btnUse);
			board = MainPanel.ShowNotificationBoard(btnUse, "", Alignment.Top, new Vector2(144f, 555f));
			board.transform.SetParent(TutorialsManager.transform);

			yield return new WaitUntil(() => MainPanel.RewardsPopup.IsActiveAndEnabled());
			End(false);
			GameData.Instance.TutorialsGroup.Finish(id);
			yield return null;
			yield return new WaitUntil(() => !MainPanel.RewardsPopup.IsActiveAndEnabled());
	

			yield return BackAndBattle(MainPanel.FormationPanel.btnMain, MainPanel.AFKRewardPanel, false);
			//End(true);
			MainPanel.MainMenuPanel.timeCollectNotif.SetActive(false);
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
			TutorialsManager.UnlockPanel(MainPanel.AFKRewardPanel);
			TutorialsManager.UnlockPanel(MainPanel.FastCollectPanel);
		}
	}
}
