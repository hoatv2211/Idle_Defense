using System;
using UnityEngine;

public class TrackingConstants
{
	public const string USER_PROPERTY_MAX_LEVEL = "max_level";
	public const string USER_PROPERTY_MAX_MISSION = "max_mission";
	public const string USER_PROPERTY_MAX_GAME_STEP = "max_gamestep";
	public const string USER_PROPERTY_LAST_FEATURE = "last_feature";

	public const string CLICK_SKIP_INTRO = "click_skip_intro";
	public const string CLICK_USERSETTING = "click_usersetting";
	public const string CLICK_BUY_COIN = "click_buy_coin";
	public const string CLICK_BUY_GEM = "click_buy_gem";
	public const string CLICK_QUEST = "click_quest";
	public const string CLICK_ACHIEVEMENT = "click_achievement";
	public const string CLICK_DAILY_LOGIN = "click_daily_login";
	public const string CLICK_DAILY_GIFT = "click_daily_gift";
	public const string CLICK_DISCOVERY = "click_discovery";
	public const string CLICK_CAMPAIGN = "click_campaign";
	public const string CLICK_FACTORY = "click_factory";
	public const string CLICK_HERO_LAB = "click_hero_lab";
	public const string CLICK_EVOLUTION = "click_evolution";
	public const string CLICK_FORMULA = "click_formula";
	public const string CLICK_SUMMON = "click_summon";
	public const string CLICK_STORE = "click_store";
	public const string CLICK_PACKAGE = "click_package";
	public const string CLICK_WHEEL = "click_wheel";
	public const string CLICK_BATTLE = "click_battle";
	public const string CLICK_BASE = "click_base";
	public const string CLICK_FORMATION = "click_formation";
	public const string CLICK_MAINMENU = "click_mainmenu";
	public const string CLICK_INVENTORY = "click_inventory";
	public const string CLICK_HEROES = "click_heroes";
	public const string CLICK_AUTO_BATTLE_REWARD = "click_auto_battle_reward";
	public const string CLICK_FAST = "click_fast";
	public const string CLICK_BUY_FAST_TICKET = "click_buy_fast_ticket";
	public const string CLICK_HEROES_LEVELUP = "click_heroes_levelup";
	public const string CLICK_HEROES_LEVELUP_PASS = "click_heroes_levelup_pass";
	public const string CLICK_HEROES_STARUP = "click_heroes_starup";
	public const string CLICK_HEROES_STARUP_PASS = "click_heroes_starup_pass";
	public const string CLICK_HEROES_GEAR = "click_heroes_gear";
	public const string CLICK_HEROES_GEAR_ONE_CLICK_EQUIP = "click_heroes_gear_one_click_equip";
	public const string CLICK_HEROES_GEAR_UNEQUIP_ALL = "click_heroes_gear_unequip_all";
	public const string CLICK_INVENTORY_GEAR = "click_inventory_gear";
	public const string CLICK_INVENTORY_ITEM = "click_inventory_item";
	public const string CLICK_INVENTORY_PART = "click_inventory_part";
	public const string CLICK_BASE_LEVELUP = "click_base_levelup";
	public const string CLICK_BASE_LEVELUP_PASS = "click_base_levelup_pass";

	public const string CLICK_BATTLE_DETAIL_OPENFORMATION = "click_battle_detail_openFormation";
	public const string CLICK_BATTLE_DETAIL_HEROBUTTON_SELECT = "click_battle_detail_herobutton_select";
	public const string CLICK_BATTLE_DETAIL_PLAY = "click_battle_detail_play";
	public const string CLICK_BATTLE_DETAIL_PLAY_WITH_TRAP = "click_battle_detail_play";

	public const string CLICK_PVP = "click_pvp";
	public const string CLICK_PVP_FORMATION = "click_pvp_formation";
	public const string CLICK_PVP_INFOR = "click_pvp_infor";
	public const string CLICK_PVP_BATTLE = "click_pvp_battle";
	public const string CLICK_PVP_RESTART = "click_pvp_restart";
	public const string CLICK_PVP_FINDMATCH_SUCCESS = "click_pvp_findMatch_success";
	public const string CLICK_PVP_FINDMATCH_FAILD = "click_pvp_findMatch_faild";
	public const string CLICK_PVP_MATCH_START = "click_pvp_match_start";
	public const string CLICK_PVP_MATCH_END_FROMPAUSE = "click_pvp_match_endFromPause";


	public const string CLICK_BASE_X_LEVELUP = "click_base_{0}_levelup";
	public const string CLICK_BASE_X_LEVELUP_PASS = "click_base_{0}_levelup_pass";

	public const string IAP_EVENT = "iap_event";

	public const string CLICK_IAP_X = "click_iap_{0}";
	public const string CLICK_IAP_X_PASS = "click_iap_{0}_pass";
	public const string CLICK_IAP_X_FAIL = "click_iap_{0}_fail";

	public const string CLICK_ERROR_QUIT = "click_error_quit";
	public const string CLICK_RATEGAME = "click_rategame";

	public const string MISSION_PLAY_COUNT = "mission_play_count";
	public const string MISSION_WIN_COUNT = "mission_win_count";
	public const string MISSION_LOSE_COUNT = "mission_lose_count";
	public const string MISSION_MAX_DAY_COUNT = "mission_max_day_count";
	public const string MISSION_PLAY_DAY_COUNT = "mission_play_day_count";

	public const string PVP_PLAY_COUNT = "pvp_play_count";
	public const string PVP_WIN_COUNT = "pvp_win_count";
	public const string PVP_LOSE_COUNT = "pvp_lose_count";
	//value
	public const string PARAM_DAY = "day";
	public const string PARAM_MISSION = "mission";
	public const int VALUE_INTRO_MISSION = 0;

	public const string COLLECT_COIN = "collect_coin";
	public const string COLLECT_GEM = "collect_gem";
	public const string USE_COIN = "use_coin";
	public const string USE_GEM = "use_gem";

	//source use
	//coin
	public const string VALUE_LEVEL_UP_HERO = "level_up_hero";
	public const string VALUE_LEVEL_UP_GEAR = "level_up_gear";
	public const string VALUE_LEVEL_UP_BASE = "level_up_base";
	public const string VALUE_LEVEL_UP_TRAP_X = "level_up_trap_{0}";
	public const string VALUE_HERO_EVOLUTION = "hero_evolution";
	public const string VALUE_STORE = "store";
	//gem
	public const string VALUE_SUMMON = "summon";
	public const string VALUE_BUY_COIN = "buy_coin";
	public const string VALUE_BUY_TICKET = "buy_ticket";
	public const string VALUE_WHEEL_F5_GENERAL = "wheel_f5_general";
	public const string VALUE_WHEEL_F5_ROYALE = "wheel_f5_royale";
	public const string VALUE_FLASH_SALE_F5 = "flash_sale_f5";
	public const string VALUE_MARKET_F5 = "market_f5";
	public const string VALUE_BUY_DRONE = "buy_drone";

	//source collect
	public const string VALUE_CHEAT = "cheat";
	public const string VALUE_PVP = "PvP";
	//coin
	public const string VALUE_LOSE = "lose";
	// public const string VALUE_MISSION_X = "mission_{0}";
	public const string VALUE_MISSION = "mission";
	public const string VALUE_AUTO_BATTLE = "auto_battle";
	public const string VALUE_FAST_COLLECT = "fast_collect";
	public const string VALUE_7DAYS_BONUS = "7days_bonus";
	public const string VALUE_WHEEL = "wheel";
	public const string VALUE_DISASSEMBLE_GEAR = "disassemble_gear";
	public const string VALUE_DISASSEMBLE_HERO = "disassemble_hero";
	public const string VALUE_DAILY_GIFT = "daily_gift";
	public const string VALUE_NOTIFI_12H = "notifi_12h";
	public const string VALUE_NOTIFI_20H = "notifi_20h";
	public const string VALUE_TUTORIAL = "tutorial";
	//gem
	public const string VALUE_MAP_X = "map_{0}";
	public const string VALUE_FIRST_PURCHASE = "first_purchase";
	public const string VALUE_DAILY_LOGIN_FREE = "daily_login_free";
	public const string VALUE_DAILY_LOGIN_VIP = "daily_login_vip";
	public const string VALUE_DAILY_QUEST = "daily_quest";
	public const string VALUE_DAILY_QUEST_POINT = "daily_quest_point";
	public const string VALUE_ACHIEVEMENT = "achievement";
	public const string VALUE_SUB = "sub";
	public const string VALUE_PREMIUM_PASS = "premium_pass";
	public const string VALUE_LEVEL_UP = "level_up";
	public const string VALUE_DISCOVERY = "discovery";

	//empty
	public const string VALUE_EMPTY = "xxxx";

	public const string PARAM_SOURCE = "source";
	public const string PARAM_AMOUNT = "amount";


	//Ads
	public const string ADS_VIEW_USER = "ads_view_user";

	public const string ADS_REWARD_WIN_X = "ads_reward_win_x";
	public const string ADS_REWARD_START_WITH_TRAP = "ads_reward_start_with_trap";
	public const string ADS_REWARD_TRY_TRAP = "ads_reward_try_trap";
	public const string ADS_REWARD_AUTO_BATTLE_X = "ads_reward_auto_battle_x";
	public const string ADS_REWARD_FAST_TICKET = "ads_reward_fast_ticket";
	public const string ADS_REWARD_FREE_WHEEL = "ads_reward_free_wheel";
	public const string ADS_REWARD_DRONE_IN_GAME = "ads_reward_drone";
	public const string ADS_REWARD_DAILY_GIFT = "ads_reward_daily_gift";

	public const string ADS_INTER_END_GAME = "ads_inter_end_game";

	public const string ERROR_CATCHING = "error_Catching";

	public static class SceneName
	{
		public const string Screen_Home_MainMenu = "Screen_Home_MainMenu";
		public const string Screen_Home_Base = "Screen_Home_Base";
		public const string Screen_Home_Formation = "Screen_Home_Formation";
		public const string Screen_Home_Inventory = "Screen_Home_Inventory";
		public const string Screen_Home_Hero = "Screen_Home_Hero";
		public const string Screen_Home_PvP = "Screen_Home_PvP";

		public const string Screen_Home_Discovery = "Screen_Home_Discovery";
		public const string Screen_Home_WorkShop = "Screen_Home_WorkShop";
		public const string Screen_Home_Store = "Screen_Home_Store";
		public const string Screen_Home_LuckyWheel = "Screen_Home_LuckyWheel";
		public const string Screen_Home_HeroLab = "Screen_Home_HeroLab";

		public const string Screen_Play_Mission = "Screen_Play_Mission";
		public const string Screen_Play_Discovery = "Screen_Play_Discovery";
		public const string Screen_Play_PvP = "Screen_Play_PvP";
	}
}

