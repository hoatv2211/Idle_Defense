using System;
using System.Collections.Generic;
using FoodZombie;
using NotificationSamples;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;


namespace Core.Scripts.Mobile_Notification
{
    public enum NotificationChannel
    {
        AutoBattleReward,
        DailyQuest,
        DailyNoon,
        DailyReward,
        DailyDinner
    }

    [RequireComponent(typeof(GameNotificationsManager))]
    public class MobileNotificationManager : MonoBehaviour
    {

        [SerializeField] private GameNotificationsManager _manager;
        private const string APP_ICON_SMALL = "icon_small";
        private const string APP_ICON_LARGE = "truck";
        private List<IGameNotification> _listNotifications = new List<IGameNotification>();

        private static MobileNotificationManager mInstance;
        public static MobileNotificationManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = FindObjectOfType<MobileNotificationManager>();
                return mInstance;
            }
        }

        private void Awake()
        {
            if (mInstance == null)
                mInstance = this;
            else if (mInstance != this)
                Destroy(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (!_manager.Initialized)
                InitChannels();
        }

        private void InitChannels()
        {


            var autoBattle = new GameNotificationChannel(NotificationChannel.AutoBattleReward.ToString(),
                "Auto Battle Rewards", "Auto battle rewards notifications");
            var dailyQuest = new GameNotificationChannel(
                NotificationChannel.DailyQuest.ToString(), "Daily Quests",
                "Daily quests notifications");
            var DailyDinner = new GameNotificationChannel(NotificationChannel.DailyDinner.ToString(),
               "Daily Dinner Rewards", "Daily Dinner rewards notifications");
            var DailyNoon = new GameNotificationChannel(NotificationChannel.DailyNoon.ToString(),
               "Daily Noon Rewards", "Daily Noon rewards notifications");
            var DailyReward = new GameNotificationChannel(NotificationChannel.DailyReward.ToString(),
          "Daily Rewards", "Daily rewards notifications");

            _manager.Initialize(autoBattle, dailyQuest, DailyDinner, DailyNoon, DailyReward);
            //  _manager.CancelAllNotifications();
            //  _manager.DismissAllNotifications();
            RegisterNotification();
        }


        public void SendNotification(string title, string body, DateTime deliveryTime, int channelId, int? badgeNumber = null,
            bool reschedule = false,
            string smallIcon = APP_ICON_SMALL, string largeIcon = APP_ICON_LARGE)
        {
            var notification = _manager.CreateNotification();

            if (notification == null)
            {
                return;
            }

            notification.Title = title;
            notification.Body = body;
            notification.DeliveryTime = deliveryTime;
            notification.SmallIcon = smallIcon;
            notification.LargeIcon = largeIcon;
            notification.Id = channelId;
            notification.RepeatInterval = new TimeSpan(1, 0, 0, 0);
            notification.Group = ((NotificationChannel)channelId).ToString();
            if (badgeNumber != null)
            {
                notification.BadgeNumber = badgeNumber;
            }
            var match = _listNotifications.Find((noti) => noti.Id == notification.Id);
            if (match != null) return;
            _listNotifications.Add(notification);

            var notificationToDisplay = _manager.ScheduleNotification(notification);
            notificationToDisplay.Reschedule = reschedule;
            //  notificationToDisplay.
            //var notificationtest = new AndroidNotification();
            //notificationtest.Title = "Your Title";
            //notificationtest.Text = "Your Text";
            //notificationtest.FireTime = System.DateTime.Now.AddSeconds(10);
            //AndroidNotificationCenter.SendNotification(notificationtest, ((NotificationChannel)channelId).ToString());
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                if (_manager == null)
                    return;
                if (_manager.Initialized)
                {
                    //    _manager.CancelAllNotifications();
                    //     _manager.DismissAllNotifications();
                }
                else
                    InitChannels();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Debug.Log("RegisterNotification");
                RegisterNotification();
            }
            else if (_manager.Initialized)
            {
                Debug.Log("CancelAllNotifications");
                // _manager.CancelAllNotifications();
                // _manager.DismissAllNotifications();
            }
        }

        //   void Create
        private void RegisterNotification()
        {
            // _tempTime.
            //   _manager.DismissAllNotifications();
            //   _manager.CancelAllNotifications();
            int currentSecondInDay = (int)(UnbiasedTime.Instance.Now() - UnbiasedTime.Instance.Today()).TotalSeconds;
            //int currentSecondInDay = (int)(DateTime.Now - DateTime.Today).TotalSeconds;

            if (currentSecondInDay > 9 * 60 * 60)
            {
                DateTime tomorrow9h = UnbiasedTime.Instance.Now().AddDays(1).Date.Add(new TimeSpan(0, 9, 0, 0));
                //DateTime tomorrow9h = DateTime.Now.AddDays(1).Date.Add(new TimeSpan(0, 9, 0, 0));
                SendNotification("🔥🔥Daily Quest Available!🔥🔥",
                    "🔥Daily Quest is available now! Login to complete all quests!🔥",
                    tomorrow9h, reschedule: true, channelId: (int)NotificationChannel.DailyQuest);
            }
            else
            {
                DateTime tomorrow9h = UnbiasedTime.Instance.Today().Add(new TimeSpan(0, 9, 0, 0));
                //DateTime tomorrow9h = DateTime.Today.Add(new TimeSpan(0, 9, 0, 0));
                SendNotification("🔥🔥Daily Quest Available!🔥🔥",
                    "🔥Daily Quest is available now! Login to complete all quests!🔥",
                    tomorrow9h, reschedule: true, channelId: (int)NotificationChannel.DailyQuest);
            }

            // Add 1 notif lúc 12h bắn cho user kèm quà tặng 100k gold , 50 gem , 300 scrap với nội dung : Comeback and take free gem ! We need you now !
            if (currentSecondInDay > 12 * 60 * 60)
            {
                DateTime tomorrow12h = UnbiasedTime.Instance.Now().AddDays(1).Date.Add(new TimeSpan(0, 12, 0, 0));
                //DateTime tomorrow12h = DateTime.Now.AddDays(1).Date.Add(new TimeSpan(0, 12, 0, 0));
                SendNotification("🔥🔥Comeback and take free treasure! We need you🔥🔥",
                    "🔥Comeback and take free gem! We need you now!🔥",
                    tomorrow12h, reschedule: true, channelId: (int)NotificationChannel.DailyNoon);
            }
            else
            {
                DateTime tomorrow12h = UnbiasedTime.Instance.Today().Add(new TimeSpan(0, 12, 0, 0));
                //DateTime tomorrow12h = DateTime.Today.Add(new TimeSpan(0, 12, 0, 0));
                //DateTime tomorrow12h = DateTime.Today.AddSeconds(20);
                SendNotification("🔥🔥Comeback and take free treasure! We need you🔥🔥",
                    "🔥Comeback and take free gem! We need you now!🔥",
                    tomorrow12h, reschedule: true, channelId: (int)NotificationChannel.DailyNoon);
            }

            // Add 1 notif lúc 8h tối bắn cho user với nội dung : Comeback and take diner bonus 100k gold and 100 gem
            if (currentSecondInDay > 20 * 60 * 60)
            {
                DateTime tomorrow20h = UnbiasedTime.Instance.Now().AddDays(1).Date.Add(new TimeSpan(0, 20, 0, 0));
                //DateTime tomorrow20h = DateTime.Now.AddDays(1).Date.Add(new TimeSpan(0, 20, 0, 0));
                SendNotification("🔥🔥Comeback and take diner! We need you🔥🔥",
                    "🔥Comeback and take 100k gold and 100 gem🔥",
                    tomorrow20h, reschedule: true, channelId: (int)NotificationChannel.DailyDinner);
            }
            else
            {
                DateTime tomorrow20h = UnbiasedTime.Instance.Today().Add(new TimeSpan(0, 20, 0, 0));
                //DateTime tomorrow20h = DateTime.Today.Add(new TimeSpan(0, 20, 0, 0));
                //DateTime tomorrow12h = DateTime.Today.AddSeconds(20);
                SendNotification("🔥🔥Comeback and take diner! We need you🔥🔥",
                    "🔥Comeback and take 100k gold and 100 gem🔥",
                    tomorrow20h, reschedule: true, channelId: (int)NotificationChannel.DailyDinner);
            }

            // Add 1 notif lúc 7h bắn cho user daily Reward
            if (currentSecondInDay > 7 * 60 * 60)
            {
                DateTime tomorrow7h = UnbiasedTime.Instance.Now().AddDays(1).Date.Add(new TimeSpan(0, 7, 0, 0));
                //DateTime tomorrow7h = DateTime.Now.AddDays(1).Date.Add(new TimeSpan(0, 7, 0, 0));
                SendNotification("🔥🔥All Daily Reward reset !🔥🔥",
                    "🔥Comeback and collect resource now !🔥",
                    tomorrow7h, reschedule: true, channelId: (int)NotificationChannel.DailyReward);
            }
            else
            {
                DateTime tomorrow7h = UnbiasedTime.Instance.Today().Add(new TimeSpan(0, 7, 0, 0));
                //DateTime tomorrow7h = DateTime.Today.Add(new TimeSpan(0, 7, 0, 0));
                //DateTime tomorrow12h = DateTime.Today.AddSeconds(20);
                SendNotification("🔥🔥All Daily Reward reset !🔥🔥",
                    "🔥Comeback and collect resource now !🔥",
                    tomorrow7h, reschedule: true, channelId: (int)NotificationChannel.DailyReward);
            }

            // register afk reward
            var remainSecond = GameData.Instance.MissionsGroup.GetRemainTimeAFK();
            var notifyTime = UnbiasedTime.Instance.Now().AddSeconds(remainSecond);
            //var notifyTime = DateTime.Now.AddSeconds(remainSecond);
            SendNotification("🔥🔥 All resource from Afk is full!🔥🔥", "🔥Comeback and take it now Captain !🔥",
                notifyTime, reschedule: true, channelId: (int)NotificationChannel.AutoBattleReward);


        }
    }
}