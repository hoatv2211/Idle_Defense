#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FoodZombie;

namespace Utilities.Service
{
    public class ServicesMenuTools : Editor
    {
        [MenuItem("DevTools/Services/Add Firebase Manager")]
        private static void AddFirebaseManager()
        {
            var manager = FindObjectOfType<RFirebase.RFirebaseManager>();
            if (manager == null)
            {
                var obj = new GameObject("RFirebaseManager");
                obj.AddComponent<RFirebase.RFirebaseManager>();
            }
        }

        [MenuItem("DevTools/Services/Add Game Services")]
        private static void AddGameServices()
        {
            var manager = FindObjectOfType<GPGS.GameServices>();
            if (manager == null)
            {
                var obj = new GameObject("GameServices");
                obj.AddComponent<GPGS.GameServices>();
            }
        }


        [MenuItem("DevTools/Services/Add IAP Helper")]
        private static void AddIAPHelper()
        {
            var manager = FindObjectOfType<PaymentHelper>();
            if (manager == null)
            {
                var obj = new GameObject("IAPHelper");
                obj.AddComponent<PaymentHelper>();
            }
        }

        [MenuItem("DevTools/Services/Add Local Notification Helper")]
        private static void AddLocalNotificationHelper()
        {
            // var manager = FindObjectOfType<Notification.LocalNotificationHelper>();
            // if (manager == null)
            // {
            //     var obj = new GameObject("LocalNotificationHelper");
            //     obj.AddComponent<Notification.LocalNotificationHelper>();
            // }
        }
    }
}
#endif