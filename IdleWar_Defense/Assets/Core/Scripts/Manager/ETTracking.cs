using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracking engagement time user
/// </summary>
public class ETTracking : MonoBehaviour
{
    private float timeOnline = 0;

    private const string ABBase = "abt_base";
    private const string ABChange = "abt_change";

    private void Start()
    {
        timeOnline = 0;
    }

    private void Update()
    {
        timeOnline += Time.deltaTime;
    }

    private void PushTime()
    {
        if (GameUnityData.instance.GameRemoteConfig.active_ABLevel)
        {
            Config.LogEvent(ABChange,"time","timeplay", TrackingConstants.PARAM_AMOUNT, (int)timeOnline);
        }
        else
        {
            Config.LogEvent(ABBase,"time", "timeplay", TrackingConstants.PARAM_AMOUNT, (int)timeOnline);
        }

        Debug.LogError("Time Online: " + (int)timeOnline);
        timeOnline = 0;
    }

    private void OnApplicationFocus(bool pFocus)
    {
        if (!pFocus)
        {
            //GetPlayingTime();
            PushTime();
        }
    }

    //private void OnApplicationQuit()
    //{
    //    PushTime();
    //}

}
