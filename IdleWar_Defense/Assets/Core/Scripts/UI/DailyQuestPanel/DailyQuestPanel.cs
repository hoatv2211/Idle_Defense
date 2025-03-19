using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Common;
using Utilities.Components;
using Utilities.Inspector;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;
using DG.Tweening;
using UnityEngine.UI;
using EventDispatcher = Utilities.Common.EventDispatcher;

namespace FoodZombie.UI
{
	public class DailyQuestPanel : MyGamesBasePanel
	{
		public CurrencyView coinView;
		public CurrencyView gemView;
		public CurrencyView expHeroView;

		public GameObject groupDailyQuest;
		public GameObject groupAchievement;

		public ScrollRect scrollDailyQuest;
		public ScrollRect scrollAchievement;

		public Toggle togDailyQuest;
		public Toggle togAchievement;
		public GameObject imgQuestNoti;
		public GameObject imgAchievementNoti;

		public TextMeshProUGUI txtDailyQuestProcess;
		public Image imgDailyQuestProcess;

		public List<DailyQuestPointView> dailyQuestPointViews;
		public Transform transformPool;
		[SerializeField, Tooltip("Buildin Pool")] public List<DailyQuestView> dailyQuestViewsPool;

		public Transform transformAchievementPool;
		[SerializeField, Tooltip("Buildin Pool")] private List<AchievementView> achievementViewsPool;

		private DailyQuestsGroup DailyQuestsGroup => GameData.Instance.DailyQuestsGroup;
		private AchievementsGroup AchievementsGroup => GameData.Instance.AchievementsGroup;

		private int lastHighlights;
		public bool isInitDone = false;
		void Start()
		{
			togDailyQuest.onValueChanged.AddListener(TogUpgrade_Changed);
			togAchievement.onValueChanged.AddListener(TogDisassemble_Changed);

			// btnGeneralRefresh.onClick.AddListener(BtnGeneralRefresh_Pressed);

			EventDispatcher.AddListener<DailyQuestChangeEvent>(OnDailyQuestChange);
			EventDispatcher.AddListener<AchievementChangeEvent>(OnAchievementChange);
		}

		private void OnDestroy()
		{
			EventDispatcher.RemoveListener<DailyQuestChangeEvent>(OnDailyQuestChange);
			EventDispatcher.RemoveListener<AchievementChangeEvent>(OnAchievementChange);
		}

		internal override void Init()
		{
			coinView.Init(IDs.CURRENCY_COIN);
			gemView.Init(IDs.CURRENCY_GEM);
			expHeroView.Init(IDs.CURRENCY_EXP_HERO);

			RefreshDailyQuestTab();
			RefreshAchievementTab();
			if (togDailyQuest.isOn)
			{
				groupDailyQuest.SetActive(true);
				groupAchievement.SetActive(false);
			}
			else
			{
				groupDailyQuest.SetActive(false);
				groupAchievement.SetActive(true);
			}

			ShowNoti();
			isInitDone = true;
		}

		public void ShowDailyQuest()
		{
			togDailyQuest.isOn = true;
			//if (!TutorialsManager.Instance.IsCompleted(TutorialsGroup.QUEST_HOME))
			//{
			//	EventDispatcher.Raise(new TutorialTriggeredEvent(TutorialsGroup.QUEST_HOME));
			//}
		}

		public void ShowAchievement()
		{
			togAchievement.isOn = true;
		}

		private void TogUpgrade_Changed(bool value)
		{
			if (value)
			{
				groupDailyQuest.SetActive(true);
				groupAchievement.SetActive(false);
			}

			ShowNoti();
		}

		private void TogDisassemble_Changed(bool value)
		{
			if (value)
			{
				groupDailyQuest.SetActive(false);
				groupAchievement.SetActive(true);
			}

			ShowNoti();
		}

		private void RefreshDailyQuestTab()
		{
			//Daily Quest Point
			var dailyQuestPointDatas = DailyQuestsGroup.GetAllDailyQuestPointDatas();
			var count = dailyQuestPointDatas.Count;
			for (int i = 0; i < count; i++)
			{
				dailyQuestPointViews[i].Init(dailyQuestPointDatas[i]);
			}

			var point = DailyQuestsGroup.Point;
			if (point > 10) point = 10;
			txtDailyQuestProcess.text = point + "/10";
			var process = ((float)point) / 10f;
			if (process > 1f) process = 1f;
			imgDailyQuestProcess.fillAmount = process;

			//Daily Quest
			var dailyQuestDatas = DailyQuestsGroup.GetAllDailyQuestDatas();
			count = dailyQuestDatas.Count;
			dailyQuestViewsPool.Free();
			for (int i = 0; i < count; i++)
			{
				var dailyQuestView = dailyQuestViewsPool.Obtain(transformPool);
				dailyQuestView.Init(dailyQuestDatas[i], RefreshDailyQuestTab);
				dailyQuestView.SetActive(true);
			}

			scrollDailyQuest.DOVerticalNormalizedPos(1f, 0f);
		}

		private void RefreshAchievementTab()
		{
			//Daily Quest
			var achievementDatas = AchievementsGroup.GetAllAchievementDatas();
			var count = achievementDatas.Count;
			achievementViewsPool.Free();
			for (int i = 0; i < count; i++)
			{
				var achievementView = achievementViewsPool.Obtain(transformAchievementPool);
				achievementView.Init(achievementDatas[i], RefreshAchievementTab);
				achievementView.SetActive(true);
			}

			scrollAchievement.DOVerticalNormalizedPos(1f, 0f);
		}

		// private void OnHasFreeGeneralRefresh(HasFreeGeneralRefresh e)
		// {
		//     btnGeneralRefresh.SetActive(false);
		//     btnFreeGeneralRefresh.SetActive(true);
		//     txtGeneralTime.SetActive(false);
		// }
		//
		// private void OnHasFreeRoyaleRefresh(HasFreeRoyaleRefresh e)
		// {
		//     btnRoyaleRefresh.SetActive(false);
		//     btnFreeRoyaleRefresh.SetActive(true);
		//     txtRoyaleTime.SetActive(false);
		// }

		//noti
		private void ShowNoti()
		{
			ShowDailyQuestNoti();
			ShowAchievementNoti();
		}

		private void OnDailyQuestChange(DailyQuestChangeEvent e)
		{
			ShowDailyQuestNoti();
		}

		private void OnAchievementChange(AchievementChangeEvent e)
		{
			ShowAchievementNoti();
		}

		private void ShowDailyQuestNoti()
		{
			imgQuestNoti.SetActive(!togDailyQuest.isOn && GameData.Instance.DailyQuestsGroup.CheckDailyQuestNoti());
		}

		private void ShowAchievementNoti()
		{
			imgAchievementNoti.SetActive(!togAchievement.isOn && GameData.Instance.AchievementsGroup.CheckNoti());
		}
	}
}
