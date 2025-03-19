using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities.Service.RFirebase;

[CreateAssetMenu(fileName = "GameUnityData", menuName = "Assets/Scriptable Objects/GameUnityData")]
public class GameUnityData : ScriptableObject
{

    #region Constants
    #endregion
    //========================================================
    #region Members
    private static GameUnityData mInstance;
    public static GameUnityData instance
    {
        get
        {
            if (mInstance == null)
                mInstance = Resources.Load<GameUnityData>("Collection/GameUnityData");
            return mInstance;
        }
    }
    public List<HeroControl> HeroDatas;
    public List<EnemyControl> EnemyDatas;
    public BoosterPOW BoosterPOW;
    public HpBar HpBarEnemyPrefab;
    public AutoDespawnObject Text_ingame;

    public ConstantsConfig ConstantsConfig;
    public ContentUnlockInfor ContentUnlockInfor;


    [SerializeField]
    GameRemoteConfig gameRemoteConfig;

    //public bool active_Hard_LevelConfigData;
    public GameRemoteConfig GameRemoteConfig
    {
        get
        {
#if UNITY_EDITOR
            return gameRemoteConfig;
#endif

            try
            {
                string _json = RFirebaseRemote.Instance.GetStringValue("GameRemoteConfigData", "");
                GameRemoteConfig temp = JsonUtility.FromJson<GameRemoteConfig>(_json);
                if (temp == null) return gameRemoteConfig;
                return temp;
            }
            catch (System.Exception ex)
            {

                Debug.LogError(ex.ToString());
                return gameRemoteConfig;
            }
            return gameRemoteConfig;

        }
    }

    [Button("ShowRemoteConfig")]
    public void ShowReoteConfigData()
    {
        Debug.LogError("COPY " + JsonUtility.ToJson(this.gameRemoteConfig));
        GUIUtility.systemCopyBuffer = JsonUtility.ToJson(this.gameRemoteConfig);
    }




    [SerializeField]
    PvPRankData pvpRankData;
    [SerializeField]
    PvPLevelsData pvpLevelsData;
    public PvPLevelsData PvPLevelData
    {
        get
        {
            try
            {
                PvPLevelsData _temp = null;
                string _data = RFirebaseRemote.Instance.GetStringValue("PvPLevel", "");
                Debug.LogError("PvpLevel Data From Firebase " + _data);
                if (_data != null && _data.Trim().Length > 0)
                {
                    _temp = JsonUtility.FromJson<PvPLevelsData>(_data);
                    if (_temp != null)
                    {
                        pvpLevelsData.Datas = _temp.Datas;
                        return _temp;
                    }
                    else
                        return pvpLevelsData;
                }
                else
                    return pvpLevelsData;
            }
            catch (Exception ex)
            {
                Debug.LogError("cant get Firebase PvPLevelData");
                return pvpLevelsData;
            }
            return pvpLevelsData;
        }
        set
        {
            pvpLevelsData = value;
        }
    }
    public PvPRankData PvPRankData
    {
        get
        {
            try
            {
                PvPRankData _temp = null;
                string _data = RFirebaseRemote.Instance.GetStringValue("PvPRank", "");
                Debug.LogError("PvPRank Data From Firebase " + _data);
                if (_data != null && _data.Trim().Length > 0)
                {
                    _temp = JsonUtility.FromJson<PvPRankData>(_data);
                    if (_temp != null)
                    {
                        pvpRankData.Datas = _temp.Datas;
                        return _temp;
                    }
                    else
                        return pvpRankData;
                }
                else
                    return pvpRankData;
            }
            catch (Exception ex)
            {
                //	Debug.LogError("cant get Firebase PvPRankData");
                return pvpRankData;
            }
            return pvpRankData;
        }

        set
        {
            pvpRankData = value;
        }
    }
    #endregion
    //========================================================
    #region Unity Funtions
    #endregion
    //========================================================
    #region Private
    #endregion
    //========================================================
    #region Public

    #endregion




}


[System.Serializable]
public class GameRemoteConfig
{
    public float Speed_ingamex1;
    public float Speed_ingamex2;
    public float Speed_PvP;
    public bool active_ABLevel;
    public bool active_LevelConfigData;


    public bool ads_inter_active;
    [Tooltip("Tần suất hiển thị mỗi x (x cấu hình, x mặc định = 1) ván 1 lần")]
    public int ads_inter_showPerMission;
    [Tooltip("Điều kiện hiển thị: Sau lvl x")]
    public int ads_inter_showAfterMission;
    [Tooltip("Thời gian tối thiểu giữa 2 lần show x s")]
    public float ads_inter_showAfterTime;


    public bool Function_ShowMap;
    public bool Function_ChangeFormationInMissionDetail;
    public bool Function_TapToShot_Active;
    public bool Function_TapToShot_CanHold;
    public bool Function_PvP_Active;
    public bool Function_Autoplay_Active;
    public int Function_Autoplay_ActiveLevel;
}
