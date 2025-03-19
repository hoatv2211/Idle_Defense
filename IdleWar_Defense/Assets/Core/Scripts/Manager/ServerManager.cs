
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.AntiCheat;
using Utilities.Common;

namespace FoodZombie
{
    [Serializable]
    public class EventDate : IComparable<EventDate>
    {
        public int eventId;
        public DateTime start;
        public DateTime end;

        public int CompareTo(EventDate other)
        {
            return start.CompareTo(other.start);
        }
    }

    //===============================================

    [Serializable]
    public class ConfigDefinition
    {
        public string key;
        public string value1;
        public string value2;
        public string valueType;
    }

    //===============================================

    [Serializable]
    public class PlayerDefinition
    {
        public int idPlayer;
        public string email;
        public string displayName;
        public string platform;
        /// <summary>
        /// Google Play Service ID or Game Center ID
        /// </summary>
        public string serviceUserId;
    }

    //===============================================

    public class ServerManager
    {
        #region Constants

        private static readonly string URL_INDEX = string.Format("{0}/{1}", DevSetting.Instance.serverUrl, "index");
        private static readonly string URL_GET_DATE = string.Format("{0}/{1}", DevSetting.Instance.serverUrl, "index/GetDate");
        private static readonly string URL_GET_GMDATE = string.Format("{0}/{1}", DevSetting.Instance.serverUrl, "index/GetGMDate");
        private static readonly string URL_PLAYER_LOGIN = string.Format("{0}/{1}", DevSetting.Instance.serverUrl, "player/Login");
        private static readonly string URL_PLAYER_CLAIM_GIFT_CODE = string.Format("{0}/{1}", DevSetting.Instance.serverUrl, "player/ClaimGiftCode");
        private static readonly string URL_CONFIG = string.Format("{0}/{1}", DevSetting.Instance.serverUrl, "index/GetConfigs");
        private static bool HasServerUrl => !string.IsNullOrEmpty(DevSetting.Instance.serverUrl);

        #endregion

        //===========================================

        #region Members

        public static Action onInitialized;
        public static PlayerDefinition player;
        public static ConfigDefinition[] configs;

        private static List<string> mWaitingRequests = new List<string>();
        private static DateTime? mStartServerTime;
        private static float mStartUnscaledTime;

        //private static readonly GameEncryption encryption = new GameEncryption(new byte[] { 1, 2, 3, 4, 5, 6, 7, 9 });
        private static readonly Encryption encryption = new Encryption();
        private static readonly WebRequest mWebRequest = new WebRequest(encryption);
        private static EventDate mHighlightEvent;
        private static List<EventDate> mEventDates;

        public static bool Authorized { get { return player != null; } }
        public static bool SyncedConfiguration { get; private set; }
        public static bool SyncedTime { get; private set; }
        public static bool SyncedData { get; private set; }
        public static bool Synced { get { return SyncedConfiguration && SyncedTime; } }

        #endregion

        //===========================================

        #region Public

        public static void Init()
        {
            if (!SyncedTime) GetDate(null);
            if (!SyncedConfiguration) GetConfigurations(null);
        }

        public static void CheckConnection(Action<bool> pOnFinished)
        {
            string url = URL_INDEX;
            if (mWaitingRequests.Contains(url))
                return;

            mWaitingRequests.Add(url);
            mWebRequest.Post(url, (res) =>
            {
                mWaitingRequests.Remove(url);
                pOnFinished.Raise(!res.error);
            });
        }

        public static void GetDate(Action<bool> pOnFinished)
        {
            if (!HasServerUrl)
                return;

            string url = URL_GET_DATE;
            if (mWaitingRequests.Contains(url))
                return;

            mWaitingRequests.Add(url);
            mWebRequest.Post(url, (res) =>
            {
                mWaitingRequests.Remove(url);
                if (!res.error)
                {
                    DateTime date = DateTime.MinValue;
                    SyncedTime = false;
                    string text = res.text;
                    if (TimeHelper.TryParse(text, out date))
                    {
                        mStartServerTime = date;
                        mStartUnscaledTime = Time.unscaledTime;
                        SyncedTime = true;
                        if (SyncedConfiguration)
                            onInitialized.Raise();
                        Debug.Log(mStartServerTime + " " + mStartUnscaledTime);
                    }
                    pOnFinished.Raise(SyncedTime);
                }
                else
                {
                    pOnFinished.Raise(false);
                    //Try again
                    WaitUtil.Start(7f, (s) => { GetDate(pOnFinished); });
                }
            });
        }

        public static DateTime? GetCurrentTime()
        {
            if (!HasServerUrl)
                return UnbiasedTime.Instance.Now();

            if (mStartServerTime != null)
            {
                var time = mStartServerTime.Value.AddSeconds(Time.unscaledTime - mStartUnscaledTime);
                return time;
            }
            else
            {
                GetDate(null);
            }
            return null;
        }

        public static void Login(string pGameServiceId, string pEmail, string pDisplayName, Action<bool> pOnFinished)
        {
            if (!HasServerUrl)
                return;

            string url = URL_PLAYER_LOGIN;
            if (mWaitingRequests.Contains(url))
                return;

            string platform = Application.platform.ToString();
            var keyValues = new List<KeyValue>();
            keyValues.Add(new KeyValue("gameServiceId", pGameServiceId));
            keyValues.Add(new KeyValue("platform", platform));
            keyValues.Add(new KeyValue("email", pEmail));
            keyValues.Add(new KeyValue("displayName", pDisplayName));

            mWaitingRequests.Add(url);
            mWebRequest.Post(url, keyValues, (res) =>
            {
                mWaitingRequests.Remove(url);
                if (!res.error)
                {
                    string jsonData = res.text;
                    try
                    {
                        var nodes = JSONNode.Parse(jsonData);
                        if (nodes[0].GetString("result") == "error")
                        {
                            player = null;
                        }
                        else
                        {
                            var array = JsonHelper.GetJsonArray<PlayerDefinition>(jsonData);
                            player = array[0];
                            //Debug.Log(player);
                        }
                    }
                    catch
                    {
                        player = null;
                    }
                    pOnFinished.Raise(player != null);
                }
                else
                {
                    pOnFinished.Raise(false);
                    //Try again
                    WaitUtil.Start(7f, (s) => { Login(pGameServiceId, pEmail, pDisplayName, pOnFinished); });
                }
            });
        }

        public static void ClaimGiftCode(string pCode, Action<RewardInfo, string> pOnFinished)
        {
            string url = URL_PLAYER_CLAIM_GIFT_CODE;
            if (mWaitingRequests.Contains(url))
                return;

            var keyValues = new List<KeyValue>();
            keyValues.Add(new KeyValue("giftCode", pCode));
            keyValues.Add(new KeyValue("playerId", player.idPlayer.ToString()));

            mWaitingRequests.Add(url);
            mWebRequest.Post(url, keyValues, (res) =>
            {
                mWaitingRequests.Remove(url);
                if (!res.error)
                {
                    string jsonData = res.text;
                    try
                    {
                        JSONNode nodes = JSON.Parse(jsonData);
                        if (nodes != null)
                        {
                            string message = nodes[0].GetString("result");
                            if (message == "used" || message == "not_found" || message == "expired")
                            {
                                pOnFinished.Raise(null, message);
                            }
                            else
                            {
                                int rewardType = nodes[0].GetInt("rewardType", -1);
                                int rewardId = nodes[0].GetInt("rewardId", -1);
                                int rewardAmount = nodes[0].GetInt("rewardAmount", -1);
                                //int rewardRarity = nodes[0].GetInt("rewardRarity", -1);
                                var rewardInfo = new RewardInfo(rewardType, rewardId, rewardAmount);
                                if (rewardType != -1)
                                    pOnFinished.Raise(rewardInfo.PrePickReward(), null);
                                else
                                    pOnFinished.Raise(null, null);
                            }
                        }
                    }
                    catch
                    {
                        pOnFinished.Raise(null, null);
                    }
                }
                else
                {
                    pOnFinished.Raise(null, null);
                }
            });
        }

        public static void GetConfigurations(Action<bool> pOnFinished)
        {
            if (!HasServerUrl)
                return;

            string url = URL_CONFIG;
            if (mWaitingRequests.Contains(url))
                return;

            mWaitingRequests.Add(url);
            mWebRequest.Post(url, (res) =>
            {
                mWaitingRequests.Remove(url);
                if (!res.error)
                {
                    string jsonData = res.text;
                    try
                    {
                        configs = JsonHelper.GetJsonArray<ConfigDefinition>(jsonData);
                        InitConfigs();
                    }
                    catch
                    {
                        configs = null;
                    }
                    pOnFinished.Raise(configs != null);
                }
                else
                {
                    pOnFinished.Raise(false);
                    //Try again
                    WaitUtil.Start(15f, (s) => { GetConfigurations(pOnFinished); });
                }
            });
        }

        public static bool HasEvent()
        {
            return IsChristmas() || IsNewYear() || IsValentine() || IsHalloween() || IsEaster();
        }

        public static int GetCurrentEvent()
        {
            if (IsChristmas())
                return IDs.EVENT_CHRISTMAS;
            else if (IsNewYear())
                return IDs.EVENT_NEW_YEAR;
            else if (IsValentine())
                return IDs.EVENT_VALENTINE;
            else if (IsHalloween())
                return IDs.EVENT_HALLOWEEN;
            else if (IsEaster())
                return IDs.EVENT_EASTER;
            return 0;
        }

        public static bool HasEvent(int pEventId)
        {
            switch (pEventId)
            {
                case IDs.EVENT_NEW_YEAR:
                    return IsNewYear();
                case IDs.EVENT_VALENTINE:
                    return IsValentine();
                case IDs.EVENT_EASTER:
                    return IsEaster();
                case IDs.EVENT_HALLOWEEN:
                    return IsHalloween();
                case IDs.EVENT_CHRISTMAS:
                    return IsChristmas();
            }
            return false;
        }

        public static double GetCountdownOfEvent(int pEvent)
        {
            var now = GetCurrentTime();
            if (now == null || mHighlightEvent == null)
                return 0;

            if (mHighlightEvent.eventId == pEvent)
                return (mHighlightEvent.end - now.Value).TotalSeconds;
            //This below will return future event, we basically don't need it
            foreach (var e in mEventDates)
            {
                if (e.eventId == pEvent)
                    if (e.start > now.Value)
                        return (e.end - now.Value).TotalSeconds;
            }
            return 0;
        }

        public static double GetCountdownToNextEvent(int pEvent)
        {
            var now = GetCurrentTime();
            if (now == null || mHighlightEvent == null)
                return 300000;

            if (mHighlightEvent.eventId == pEvent)
                return (mHighlightEvent.start - now.Value).TotalSeconds;

            foreach (var e in mEventDates)
            {
                if (e.eventId == pEvent)
                    if (e.start > now.Value)
                        return (e.start - now.Value).TotalSeconds;
            }
            return 300000;
        }

        #endregion

        //===========================================

        #region Private

        private static void InitConfigs()
        {
            mEventDates = new List<EventDate>();
            foreach (var c in configs)
            {
                if (c.valueType == "event_datetime")
                {
                    var enUS = new System.Globalization.CultureInfo("en-US");

                    DateTime from = DateTime.MinValue;
                    DateTime to = DateTime.MinValue;
                    if (c.key.Contains("event_new_year"))
                    {
                        //dd/MM/YYYY HH:mm:ss
                        TimeHelper.TryParse(c.value1, out from);
                        TimeHelper.TryParse(c.value2, out to);
                        mEventDates.Add(new EventDate()
                        {
                            eventId = IDs.EVENT_NEW_YEAR,
                            start = from,
                            end = to,
                        });
                    }
                    else if (c.key.Contains("event_valentine"))
                    {
                        //dd/MM/YYYY HH:mm:ss
                        TimeHelper.TryParse(c.value1, out from);
                        TimeHelper.TryParse(c.value2, out to);
                        mEventDates.Add(new EventDate()
                        {
                            eventId = IDs.EVENT_VALENTINE,
                            start = from,
                            end = to,
                        });
                    }
                    else if (c.key.Contains("event_easter"))
                    {
                        //dd/MM/YYYY HH:mm:ss
                        TimeHelper.TryParse(c.value1, out from);
                        TimeHelper.TryParse(c.value2, out to);
                        mEventDates.Add(new EventDate()
                        {
                            eventId = IDs.EVENT_EASTER,
                            start = from,
                            end = to,
                        });
                    }
                    else if (c.key.Contains("event_halloween"))
                    {
                        //dd/MM/YYYY HH:mm:ss
                        TimeHelper.TryParse(c.value1, out from);
                        TimeHelper.TryParse(c.value2, out to);
                        mEventDates.Add(new EventDate()
                        {
                            eventId = IDs.EVENT_HALLOWEEN,
                            start = from,
                            end = to,
                        });
                    }
                    else if (c.key.Contains("event_christmas"))
                    {
                        //dd/MM/YYYY HH:mm:ss
                        TimeHelper.TryParse(c.value1, out from);
                        TimeHelper.TryParse(c.value2, out to);
                        mEventDates.Add(new EventDate()
                        {
                            eventId = IDs.EVENT_CHRISTMAS,
                            start = from,
                            end = to,
                        });
                    }
                }
            }
            mEventDates.Sort();
            WaitUtil.Start(new WaitUtil.ConditionEvent()
            {
                triggerCondition = () => GetCurrentTime() != null,
                onTrigger = () =>
                {
                    //Find closest event in event or future
                    DateTime now = GetCurrentTime().Value;
                    foreach (EventDate e in mEventDates)
                    {
                        if (e.start < now && now < e.end)
                        {
                            mHighlightEvent = e;
                            break;
                        }
                        else if (e.start > now) //Go to next event in future
                        {
                            mHighlightEvent = e;
                            break;
                        }
                    }

                    SyncedConfiguration = true;
                    if (SyncedTime)
                        onInitialized.Raise();

                    Debug.Log(mEventDates);
                }
            });
        }

        private static bool IsChristmas()
        {
            if (mHighlightEvent != null && mHighlightEvent.eventId == IDs.EVENT_CHRISTMAS)
            {
                var now = GetCurrentTime();
                return now.Value > mHighlightEvent.start && now <= mHighlightEvent.end;
            }
            return false;
        }

        private static bool IsHalloween()
        {
            if (mHighlightEvent != null && mHighlightEvent.eventId == IDs.EVENT_HALLOWEEN)
            {
                var now = GetCurrentTime();
                return now.Value > mHighlightEvent.start && now <= mHighlightEvent.end;
            }
            return false;
        }

        private static bool IsEaster()
        {
            if (mHighlightEvent != null && mHighlightEvent.eventId == IDs.EVENT_EASTER)
            {
                var now = GetCurrentTime();
                return now.Value > mHighlightEvent.start && now <= mHighlightEvent.end;
            }
            return false;
        }

        private static bool IsNewYear()
        {
            if (mHighlightEvent != null && mHighlightEvent.eventId == IDs.EVENT_NEW_YEAR)
            {
                var now = GetCurrentTime();
                return now.Value > mHighlightEvent.start && now <= mHighlightEvent.end;
            }
            return false;
        }

        private static bool IsValentine()
        {
            if (mHighlightEvent != null && mHighlightEvent.eventId == IDs.EVENT_VALENTINE)
            {
                var now = GetCurrentTime();
                return now.Value > mHighlightEvent.start && now <= mHighlightEvent.end;
            }
            return false;
        }

        #endregion
    }
}