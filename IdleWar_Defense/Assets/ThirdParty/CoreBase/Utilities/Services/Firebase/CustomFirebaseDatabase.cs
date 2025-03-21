﻿#if ACTIVE_FIREBASE_DATABASE
using Firebase;
using Firebase.Database;
#if UNITY_EDITOR
using Firebase.Unity.Editor;
#endif
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.Common;

namespace Utilities.Service.RFirebase.Database
{
    public class CustomFirebaseDatabase
    {
        //-- UserData
        //---- UserProfile

        private static CustomFirebaseDatabase mInstance;
        public static CustomFirebaseDatabase Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new CustomFirebaseDatabase();
                return mInstance;
            }
        }

#if ACTIVE_FIREBASE_DATABASE
        public RDatabaseReference userProfile;

        public CustomFirebaseDatabase(DatabaseReference pParent)
        {
            var userData = pParent.Child("UserData");
            userProfile = new RDatabaseReference("UserProfile", userData);
        }
        public CustomFirebaseDatabase()
        {

            var userData = FirebaseDatabase.DefaultInstance.GetReference("UserData");
            userProfile = new RDatabaseReference("UserProfile", userData);
        }

        public void RegisterNewuser(string pId, string pUserJsonData, Action<bool> pOnFinished)
        {
            userProfile.SetJsonData(pId, pUserJsonData, pOnFinished);
        }
        public void SaveUserDataToCloud(string pId, string pUserJsonData, Action<bool> pOnFinished)
        {
            userProfile.SetJsonDataPriority(pId, pUserJsonData, pOnFinished);
        }
        public void GetUserData(string pId, Action<string, bool> pOnFinished)
        {
            userProfile.GetData(pId, pOnFinished);
        }
        public void RemoveUserData(string pId, Action<bool> pOnFinished)
        {
            userProfile.RemoveData(pId, pOnFinished);
        }

        /// <summary>
        /// Used to get a dictionary of uses, that fit the condition
        /// Most used for PVP feature or Leaderboard
        /// </summary>
        /// <param name="limit">Limitation of dictionary</param>
        /// <param name="orderBy">Condition to search for eg. power/level/etc... </param>
        /// <param name="endAt">Stop query when reaching this value</param>
        /// <param name="pOnFinished">Callback with value is a json list (This list can be parsed to a Dictionary)</param>
        public void GetListUsersData(int limit, string orderBy, int endAt, Action<bool, string> pOnFinished)
        {
            var task = userProfile.reference
                .OrderByChild(orderBy)
                .EndAt(endAt)
                .LimitToLast(limit)
                .GetValueAsync();
            WaitUtil.WaitTask(task, () =>
            {
                bool success = !task.IsFaulted && !task.IsCanceled;
                string jsonData = "";
                if (success)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot != null)
                    {
                        if (snapshot.Exists)
                            jsonData = snapshot.GetRawJsonValue();
                    }
                }
                if (pOnFinished != null)
                    pOnFinished(success, jsonData);
            });
        }
#else
        public CustomFirebaseDatabase(object pParent) { }
        public CustomFirebaseDatabase() { }
        public void RegisterNewuser(string pId, string pUserJsonData, Action<bool> pOnFinished) { }
        public void SaveUserDataToCloud(string pId, string pUserJsonData, Action<bool> pOnFinished) { }
        public void GetUserData(string pId, Action<string, bool> pOnFinished) { }
        public void RemoveUserData(string pId, Action<bool> pOnFinished) { }
        public void GetListUsersData(int limit, string orderBy, int endAt, Action<bool, string> pOnFinished) { }
#endif
    }
}