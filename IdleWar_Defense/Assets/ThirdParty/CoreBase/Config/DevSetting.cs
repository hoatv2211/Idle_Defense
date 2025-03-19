using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Inspector;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(fileName = "DevSetting", menuName = "DevTools/Dev Setting")]
public class DevSetting : ScriptableObject
{
    #region Internal Class

    [System.Serializable]
    public class Profile
    {
        [ReadOnly]
        public string name;

        [Separator("-- COMMON --")]
        public string serverUrl;
        public string crashEmail;
        public bool enableLog;
        public bool enableDraw;
        public bool showFPS;
        public bool enableCheat;
        public bool enableCheatTime;

        [Separator("-- BUILD --"), ReadOnly]
        public List<Directive> defines;

        [Separator("-- TEST --")]
        public string testUserId;
        public string testUserName;
        public string testEmail;
    }

    [System.Serializable]
    public class Directive
    {
        public Directive()
        {
            color = Color.white;
        }
        public Directive(string pName, bool pEnable)
        {
            name = pName;
            enabled = pEnable;
            color = Color.white;
        }
        public string name;
        public Color color;
        public bool enabled = true;
    }

    #endregion

    //==================================

    private static DevSetting mInstance;
    public static DevSetting Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = Resources.Load<DevSetting>("DevSetting");
            return mInstance;
        }
    }

    public Action onSettingsChanged;
    public Profile profile = new Profile();

    public string crashEmail
    {
        get { return profile.crashEmail; }
    }
    public string serverUrl
    {
        get { return profile.serverUrl; }
    }
    public bool enableLog
    {
        get { return profile.enableLog; }
        set
        {
            profile.enableLog = value;
            onSettingsChanged?.Invoke();
        }
    }
    public bool enableDraw
    {
        get { return profile.enableDraw; }
        set
        {
            profile.enableDraw = value;
            onSettingsChanged?.Invoke();
        }
    }
    public bool showFPS
    {
        get { return profile.showFPS; }
        set
        {
            profile.showFPS = value;
            onSettingsChanged?.Invoke();
        }
    }
    public bool enableCheat
    {
        get { return profile.enableCheat; }
        set
        {
            profile.enableCheat = value;
            onSettingsChanged?.Invoke();
        }
    }
    public bool enableCheatTime
    {
        get { return profile.enableCheatTime; }
        set
        {
            profile.enableCheatTime = value;
            onSettingsChanged?.Invoke();
        }
    }
}