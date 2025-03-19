using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

namespace FoodZombie.UI
{
    public class NotificationCenter : MonoBehaviour
    {
        // AndroidNotificationChannel channel;
        // private void Start()
        // {
        //     channel = new AndroidNotificationChannel()
        //     {
        //         Id = "Zombie-Defense2",
        //         Name = "Zombie Defense 2",
        //         Importance = Importance.High,
        //         Description = "Notification",
        //     };
        //     AndroidNotificationCenter.RegisterNotificationChannel(channel);
        // }
        //
        // private void AddNotification(string title, string content, DateTime time)
        // {
        //     var notification = new AndroidNotification();
        //     notification.Title = title;
        //     notification.Text = content;
        //     notification.FireTime = time;
        //     AndroidNotificationCenter.SendNotification(notification, channel.Id);
        // }
        //
        // private void RegisterNotification()
        // {
        //     if (!GameData.Instance.GameConfigGroup.EnableNotification) return;
        //     AddNotification("New Day!", "New day begins. Join us to fight the zombies!", DateTime.Now.AddSeconds(24 * 60 * 60 - DateTime.Now.Second - DateTime.Now.Minute * 60 - DateTime.Now.Hour * 60 * 60));
        //     if (GameData.Instance.WheelData.IsRunning) AddNotification("Spin The Wheel!", "Free spin! Don't miss out!", DateTime.Now.AddSeconds(2 * 60 * 60));
        //     AddNotification("They Miss You!", "The zombies are too crowded and aggressive. Help me kill them!", DateTime.Now.AddSeconds(8 * 60 * 60));
        // }
        //
        // private void OnApplicationPause(bool pause)
        // {
        //     if (pause)
        //     {
        //         RegisterNotification();
        //     }
        //     else
        //     {
        //         AndroidNotificationCenter.CancelAllNotifications();
        //     }
        // }
        //
        // private void OnApplicationQuit()
        // {
        //     AndroidNotificationCenter.CancelAllNotifications();
        //     RegisterNotification();
        // }
    }
}