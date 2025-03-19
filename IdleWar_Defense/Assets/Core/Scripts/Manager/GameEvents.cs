using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;

namespace FoodZombie
{
	public class CurrencyChangedEvent : BaseEvent
	{
		public int id;
		public int additionalValue;
		public CurrencyChangedEvent(int pId, int pAdditionalValue)
		{
			id = pId;
			additionalValue = pAdditionalValue;
		}
	}

	// public class UserExpChangedEvent : BaseEvent
	// {
	//     public int additionalValue;
	//     public UserExpChangedEvent(int pAdditionalValue)
	//     {
	//         additionalValue = pAdditionalValue;
	//     }
	// }
	public class UserTryTrapEvent : BaseEvent { }
	public class UserExpChangeEvent : BaseEvent { }
	public class UserLevelUpEvent : BaseEvent { }
	public class VipLevelUpEvent : BaseEvent { }
	public class HasFreeGeneralRefresh : BaseEvent { }
	public class HasFreeRoyaleRefresh : BaseEvent { }

	public class TutorialFinishedEvent : BaseEvent
	{
		public int tutorial;
		public TutorialFinishedEvent(int pId)
		{
			tutorial = pId;
		}
	}

	public class TutorialTriggeredEvent : BaseEvent
	{
		public int tutorial;
		public TutorialTriggeredEvent(int pId)
		{
			tutorial = pId;
		}
	}

	public class ResetTimeBossEvent : BaseEvent { }

	public class VehicleUpgradeEvent : BaseEvent { }

	public class FoodGuyUpgradeEvent : BaseEvent { }

	public class SafeChangeValueEvent : BaseEvent
	{
		public int id;
		public bool value;
		public SafeChangeValueEvent(int pId, bool pValue)
		{
			id = pId;
			value = pValue;
		}
	}

	public class BuyCountChangeEvent : BaseEvent { }
	public class IAPByGemEvent : BaseEvent { }

	public class UIChangeEvent : BaseEvent { }
	public class FlashSaleRefreshEvent : BaseEvent { }
	public class DailyQuestChangeEvent : BaseEvent { }
	public class DailyLoginChangeEvent : BaseEvent { }
	public class AchievementChangeEvent : BaseEvent { }
	public class HeroLevelUpEvent : BaseEvent { }
	public class HeroStarUpEvent : BaseEvent { }
	public class HeroEvolutionEvent : BaseEvent { }
	public class ChangeGearEvent : BaseEvent { }
	public class FormationChangeEvent : BaseEvent { }
	public class DailyBonusChangeEvent : BaseEvent { }
	public class DailyGiftsChangeEvent : BaseEvent { }
	public class SpecialPackBoughtEvent : BaseEvent { }
	public class RefreshFlashSaleEvent : BaseEvent { }
	public class HeroFragmentChangeEvent : BaseEvent { }
	public class WaveEndEvent : BaseEvent { public float timePlay; public Vector3 position; }
	public class BaseLevelUpEvent : BaseEvent { }
	public class GamePaymentInitializedEvent : BaseEvent { }
	public class UserModelChange : BaseEvent { }
	public class FBChangeEvent : BaseEvent { }
	public class PvPUpdateTime : BaseEvent { }
}
