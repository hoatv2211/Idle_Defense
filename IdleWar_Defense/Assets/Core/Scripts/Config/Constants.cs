using FoodZombie;
using System;
using UnityEngine;
using Utilities.Service.RFirebase;

public class Constants
{
    public const string PREMIUM_DRONE_GEM_SKU = "mergedefend.drone.pack.premium";
    public const int PREMIUM_DRONE_GEM_COST = 550;
    public const string PREMIUM_DRONE_USD_COST = "1.99$";
    public const int MAX_HEROES_CAN_EQUIP = 6;
    //public const int SUMMON_HERO_ID_1 = IDs.HA5;
    public const int SUMMON_HERO_ID_1 = IDs.HA13;
    public const int SUMMON_HERO_ID_2 = IDs.HA1;
    // public const int SUMMON_HERO_ID_3 {
    public const int MIN_STAGE_RD = 10;
    public const int MAX_STAGE_RD = 90;
    public const float CHARACTER_SCALE = 0.8f;
    public const string GraphApiVersion = "V1.0";
    #region UnlockContent
    //0 for UNLOCK by UserLevel,1 for UNLOCK by MissionLevel
    public static int UNLOCK_CONTENT_TYPE
    {
        get
        {
            int _output = 1;
            try
            {
                _output = (int)RFirebaseRemote.Instance.GetNumberValue("unlock_content_type", 1);
            }
            catch (Exception ex)
            {

                return 1;
            }

            return _output;
        }
    }
    public static LevelConfigFirebaseObject LevelDataFromFirebase
    {
        get
        {
            if (!GameUnityData.instance.GameRemoteConfig.active_LevelConfigData) return null;
            LevelConfigFirebaseObject levelConfigObject = null;
            try
            {
                string _data = RFirebaseRemote.Instance.GetStringValue("level_data", "");
                Debug.LogError("Level Data From Firebase " + _data);
                if (_data != null && _data.Trim().Length > 0)
                    levelConfigObject = JsonUtility.FromJson<LevelConfigFirebaseObject>(_data);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                return null;
            }

            return levelConfigObject;
        }
    }
    public static int UNLOCK_SUMMON = 1001;
    public static int UNLOCK_FORMATION = 1;
    public static int UNLOCK_BASE = 2001;
    //public static int UNLOCK_PVP = 1004;
    public static int UNLOCK_PVP
    {
        get
        {
            int _output = 2015;
            try
            {
                _output = (int)RFirebaseRemote.Instance.GetNumberValue("PvPUnlock", 2015);
            }
            catch (Exception ex)
            {

                return 2015;
            }

            return _output;
        }
    }
    public static int UNLOCK_UPGRADE_HERO
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 1001;
            return 3;
        }
    }
    public static int UNLOCK_GEAR_EQUIP
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 1001;
            return 3;
        }
    }
    public static int UNLOCK_DAILY_LOGIN
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 1001;
            return 5;
        }
    }
    public static int UNLOCK_7_DAYS_BONUS
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 1001;
            return 5;
        }
    }
    public static int UNLOCK_QUEST
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 1001;
            return 7;
        }
    }
    public static int UNLOCK_ACHIEVEMENT
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 2001;
            return 7;
        }
    }
    public static int UNLOCK_AFK_REWARD
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 1001;
            return 10;
        }
    }
    public static int UNLOCK_STAR_UP_HERO
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 2015;
            return 14;
        }
    }
    public static int UNLOCK_DISCOVERY
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 3005;
            return 15;
        }
    }
    public static int UNLOCK_WHEELS
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 2005;
            return 18;
        }
    }
    public static int UNLOCK_WHEELS_ROYALE
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 4015;
            return 40;
        }
    }
    //Workshop
    public static int UNLOCK_FACTORY
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 2010;
            return 20;
        }
    }
    //Hero Lab
    public static int UNLOCK_DISASSEMBLE_HERO
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 4005;
            return 25;
        }
    }
    public static int UNLOCK_HERO_EVOLUTION
    {
        get
        {
            if (UNLOCK_CONTENT_TYPE == 1)
                return 3015;
            return 28;
        }
    }
    public static int FORMATION_SLOT_NUMBER
    {
        get
        {
            int _slotNumber = 1;
            int _maxLevel = GameData.Instance.MissionsGroup.CurrentMissionId;
            for (int i = 2; i <= 6; i++)
            {
                if (_maxLevel - 1 >= FORMATION_SLOTUNLOCK(i))
                    _slotNumber++;
            }
            return _slotNumber;
        }
    }
    public static int FORMATION_SLOTUNLOCK(int SlotNumber)
    {
        switch (SlotNumber)
        {
            case 2: return 1001; break;
            case 3: return 1005; break;
            case 4: return 2008; break;
            case 5: return 3012; break;
            case 6: return 4012; break;
            default:
                return 0;
                break;
        }
    }
    #endregion
    #region TUT
    public static int TUT_ACTIVE_GAMEPLAY_LEVELUP
    {
        get
        {
            int _output = 0;
            try
            {
                _output = (int)RFirebaseRemote.Instance.GetNumberValue("TUT_ACTIVE_GAMEPLAY_LEVELUP", 0);
            }
            catch (Exception ex)
            {

                return 0;
            }

            return _output;
        }
    }
    #endregion
    public static int PVP_VERSION_ONBOARD
    {
        get
        {
            int _output = DongNHEditor.PVP_VERSION;
            try
            {
                _output = (int)RFirebaseRemote.Instance.GetNumberValue("PVP_VERSION", DongNHEditor.PVP_VERSION);
            }
            catch (Exception ex)
            {

                return DongNHEditor.PVP_VERSION;
            }

            return _output;
        }
    }

    public const int GEM_TO_COIN_SHOP = 20;
    public const int COIN_BY_GEM_SHOP = 50000;
    public const int GEM_TO_TICKET_SHOP = 100;
    public const int GEM_TO_TICKET_PVP_SHOP = 10;
    public const int TICKET_BY_GEM_SHOP = 1;
    public const int COST_BLUE_HERO_FRAGMENT = 30;
    public const int COST_EPIC_HERO_FRAGMENT = 50;
    public const int BUY_COIN_LIMIT = 5;
    public const int BUY_FAST_LOOT_LIMIT = 5;
    public const int DISCOVERY_LIMIT = 2;
    public const int BUY_DISCOVERY_TIME = 1;
    public const float MISSION_COIN_X = 1.5f;
    public const float MISSION_HERO_EXP_X = 1.5f;
    public const float MISSION_USER_EXP_X = 2f;


    public class ConstantsLayer
    {
        public const string LAYER_CHARACTER_NAME = "Character";
    }

}
