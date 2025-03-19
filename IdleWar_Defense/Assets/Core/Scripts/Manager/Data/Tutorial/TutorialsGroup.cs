
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;
using Utilities.Pattern.Data;
using Utilities.Service.RFirebase;

namespace FoodZombie
{
	public class TutorialData : DataGroup
	{
		private BoolData mCompleted;
		private ListData<int> mFinishedSteps;

		public bool Completed { get { return mCompleted.Value; } }

		public TutorialData(int pId) : base(pId)
		{
			//mFinishedSteps = AddData(new ListData<int>(0, 0));
			mFinishedSteps = AddData(new ListData<int>(0, new List<int>()));
			mCompleted = AddData(new BoolData(1));
			//  TutDonePOW = AddData(new BoolData(2, false));
		}

		public void SaveStep(int pId)
		{
			if (!mFinishedSteps.Contain(pId))
				mFinishedSteps.Add(pId);
			GameData.Instance.SaveGame();
		}

		public bool IsStepFinished(int pId)
		{
			return mFinishedSteps.Contain(pId);
		}

		public void SetComplete(bool pValue)
		{
			mCompleted.Value = (pValue);

			//RFirebaseManager.LogEvent(TrackingConstants.EVENT_COMPLETE_TUTORIAL, TrackingConstants.PARAM_NAME, GetTrackingTutorialName());
			//RFirebaseManager.LogEvent(string.Format(TrackingConstants.EVENT_COMPLETE_TUTORIAL_, GetTrackingTutorialName()));
		}

		private string GetTrackingTutorialName()
		{
			switch (Id)
			{
				case TutorialsGroup.MISSION_INTRO:
					return "mision intro";
				case TutorialsGroup.SUMMON_HERO:
					return "summon hero";
				case TutorialsGroup.SUMMON_HERO_SENIOR_GAMEPLAY:
					return "summon hero senior gameplay";
				case TutorialsGroup.SUMMON_HERO_SENIOR_HOME:
					return "summon hero senior home";
				case TutorialsGroup.LEVELUP_HERO:
					return "level up hero";
				// case TutorialsGroup.EQUIP_GEAR:
				//     return "equip gear";
				case TutorialsGroup.SUMMON_HERO_X10_HOME:
					return "summon hero x10";
				case TutorialsGroup.SUMMON_HERO_X10_HOME_FORMATION:
					return "summon hero x10 formation";
				case TutorialsGroup.SUMMON_AUTO_SKILL_GAMEPLAY:
					return "summon auto skill gameplay";
				// case TutorialsGroup.SUMMON_X2_SPEED_GAMEPLAY:
				//     return "summon x2 speed gameplay";
				case TutorialsGroup.SUMMON_HERO_X10_GAMEPLAY:
					return "summon hero x10 gameplay";
				case TutorialsGroup.AUTO_BATTLE_REWARD:
					return "auto battle reward";
				case TutorialsGroup.DISCOVERY:
					return "discovery";
				case TutorialsGroup.DISSAMBLE_GEAR:
					return "dissamble gear";
				case TutorialsGroup.DISSAMBLE_HERO:
					return "dissamble hero";
			}
			return "";
		}
	}

	public class TutorialsGroup : DataGroup
	{
		//max =24
		//NOTE: These below flows attually like Tooltips Chain than Tutorials
		//Player can mostly ignore them, but they will not disapear util some certain conditions are completed
		//And most importantly, multi Tooltips Chains can be active at the same time

		//Unignorable tutorial
		public const int WELLCOME_NOTUT = -1;
		public const int WELLCOME_1 = 22;

		public const int PLAY_ON_GAMEPLAY = 24;

		public const int MISSION_INTRO = 0;
		public const int SUMMON_HERO = 1;
		public const int SUMMON_HERO_SENIOR_GAMEPLAY = 2;
		public const int SUMMON_HERO_SENIOR_HOME = 3;

		public const int LEVELUP_HERO_BACKHOME = 14;
		public const int LEVELUP_HERO = 4;
		// public const int EQUIP_GEAR = 5;
		public const int SUMMON_HERO_X10_HOME = 6;
		public const int SUMMON_HERO_X10_HOME_FORMATION = 23;


		public const int QUEST_GAMEPLAY = 20;
		public const int QUEST_HOME = 15;

		public const int SUMMON_AUTO_SKILL_GAMEPLAY = 8;
		// public const int SUMMON_X2_SPEED_GAMEPLAY = 9;
		public const int SUMMON_HERO_X10_GAMEPLAY = 7;

		public const int AUTO_BATTLE_REWARD_GAMEPLAY = 16;
		public const int AUTO_BATTLE_REWARD = 10;

		public const int USE_BASE_GAMEPLAY = 17;
		public const int USE_BASE_HOME = 18;
		public const int USE_BASEPLAY_GAMEPLAY = 19;

		public const int DISCOVERY = 11;
		public const int DISSAMBLE_GEAR = 12;
		public const int DISSAMBLE_HERO = 13;

		public const int PLAYWITHTRAP = 21;

		private DataGroup mGroup;
		private BoolData TutDonePOW;
		public bool TutDone_POW
		{
			get
			{
				return TutDonePOW.Value;
			}
			set
			{
				TutDonePOW.Value = value;
			}
		}
		public TutorialsGroup(int pId) : base(pId)
		{

			mGroup = AddData(new DataGroup(0));
			TutDonePOW = AddData(new BoolData(1, false));
			//
			mGroup.AddData(new TutorialData(WELLCOME_NOTUT));
			mGroup.AddData(new TutorialData(WELLCOME_1));
			mGroup.AddData(new TutorialData(PLAY_ON_GAMEPLAY));
			mGroup.AddData(new TutorialData(MISSION_INTRO));
			mGroup.AddData(new TutorialData(SUMMON_HERO));
			mGroup.AddData(new TutorialData(SUMMON_HERO_SENIOR_GAMEPLAY));
			mGroup.AddData(new TutorialData(SUMMON_HERO_SENIOR_HOME));
			mGroup.AddData(new TutorialData(LEVELUP_HERO_BACKHOME));
			mGroup.AddData(new TutorialData(LEVELUP_HERO));
			// mGroup.AddData(new TutorialData(EQUIP_GEAR));
			mGroup.AddData(new TutorialData(SUMMON_HERO_X10_HOME));
			mGroup.AddData(new TutorialData(SUMMON_HERO_X10_HOME_FORMATION));
			mGroup.AddData(new TutorialData(SUMMON_AUTO_SKILL_GAMEPLAY));
			// mGroup.AddData(new TutorialData(SUMMON_X2_SPEED_GAMEPLAY));
			mGroup.AddData(new TutorialData(SUMMON_HERO_X10_GAMEPLAY));

			mGroup.AddData(new TutorialData(QUEST_HOME));
			mGroup.AddData(new TutorialData(QUEST_GAMEPLAY));

			mGroup.AddData(new TutorialData(AUTO_BATTLE_REWARD_GAMEPLAY));
			mGroup.AddData(new TutorialData(AUTO_BATTLE_REWARD));

			mGroup.AddData(new TutorialData(USE_BASE_HOME));
			mGroup.AddData(new TutorialData(USE_BASE_GAMEPLAY));
			mGroup.AddData(new TutorialData(USE_BASEPLAY_GAMEPLAY));

			mGroup.AddData(new TutorialData(DISCOVERY));
			mGroup.AddData(new TutorialData(DISSAMBLE_GEAR));
			mGroup.AddData(new TutorialData(DISSAMBLE_HERO));
			//       mGroup.AddData(new TutorialData(PLAYWITHTRAP));


		}

		public void Finish(int pTutId, bool isFinish = true)
		{
			foreach (TutorialData t in mGroup.Children)
				if (t.Id == pTutId)
				{
					t.SetComplete(isFinish);
					EventDispatcher.Raise(new TutorialFinishedEvent(pTutId));
				}
		}

		public void Reset(int pTutId)
		{
			foreach (TutorialData t in mGroup.Children)
				if (t.Id == pTutId)
					t.Reset();
		}

		public bool IsFinished(int pTutId)
		{
			foreach (TutorialData t in mGroup.Children)
				if (t.Id == pTutId)
					return t.Completed;
			return true;
		}

		public List<TutorialData> GetTutorialsCompleted()
		{
			var list = new List<TutorialData>();
			foreach (TutorialData t in mGroup.Children)
			{
				if (t.Completed)
					list.Add(t);
			}
			return list;
		}

		public TutorialData GetTutorial(int pTutId)
		{
			foreach (TutorialData t in mGroup.Children)
				if (t.Id == pTutId)
					return t;
			return null;
		}
	}
}