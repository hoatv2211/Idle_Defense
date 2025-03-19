using System;
using UnityEngine;

namespace Utilities.Service.Ads
{
    /// <summary>
    /// Generic cross-platform identifier for ad resources.
    /// </summary>
    [Serializable]
    public class AdId
    {
        public AdId(string pIosID, string pAndroidId)
        {
            iosId = pIosID;
            androidId = pAndroidId;
        }
        public string Id
        {
            get
            {
#if UNITY_ANDROID
                return androidId;
#elif UNITY_IOS
                return iosId;
#else
                return string.Empty;
#endif
            }
        }
        /// <summary>
        /// The ad ID for iOS platform.
        /// </summary>
        public string iosId;
        /// <summary>
        /// The ad ID for Android platform.
        /// </summary>
        public string androidId;
    }
}