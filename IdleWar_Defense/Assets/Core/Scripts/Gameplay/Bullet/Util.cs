﻿using System.Collections;
using UnityEngine;

/// <summary>
/// util.
/// </summary>
public static class Util
{
    /// <summary>
    /// Move axis types.
    /// </summary>
    public enum AXIS
    {
        X_AND_Y,
        X_AND_Z,
    }

    /// <summary>
    /// Determines if is mobile platform.
    /// </summary>
    public static bool IsMobilePlatform()
    {
#if UNITY_IOS || UNITY_ANDROID
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// Wait for seconds coroutine for UniBulletHell.
    /// </summary>
    public static IEnumerator WaitForSeconds(float waitTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            elapsedTime += BulletTimer.instance.deltaTime;
            yield return 0;
        }
    }

    /// <summary>
    /// Get Transform from tag name.
    /// </summary>
    public static Transform GetTransformFromTagName(string tagName)
    {
        if (string.IsNullOrEmpty(tagName))
        {
            return null;
        }
        GameObject goTarget = GameObject.FindWithTag(tagName);
        if (goTarget == null)
        {
            return null;
        }
        return goTarget.transform;
    }

    /// <summary>
    /// Get shifted angle.
    /// </summary>
    public static float GetShiftedAngle(int wayIndex, float baseAngle, float betweenAngle)
    {
        float angle = wayIndex % 2 == 0 ?
                      baseAngle - (betweenAngle * (float)wayIndex / 2f) :
                      baseAngle + (betweenAngle * Mathf.Ceil((float)wayIndex / 2f));
        return angle;
    }

    /// <summary>
    /// Get 0 to 360 angle.
    /// </summary>
    public static float Get360Angle(float angle)
    {
        while (angle < 0f)
        {
            angle += 360f;
        }
        while (360f < angle)
        {
            angle -= 360f;
        }
        return angle;
    }

    /// <summary>
    /// Get angle from two transforms position.
    /// </summary>
    public static float GetAngleFromTwoPosition(Transform fromTrans, Transform toTrans, Util.AXIS axisMove)
    {
        switch (axisMove)
        {
            case AXIS.X_AND_Y:
                return GetZangleFromTwoPosition(fromTrans, toTrans);
            case AXIS.X_AND_Z:
                return GetYangleFromTwoPosition(fromTrans, toTrans);
            default:
                return 0f;
        }
    }

    /// <summary>
    /// Get Z angle from two transforms position.
    /// </summary>
    private static float GetZangleFromTwoPosition(Transform fromTrans, Transform toTrans)
    {
        if (fromTrans == null || toTrans == null)
        {
            return 0f;
        }
        var xDistance = toTrans.position.x - fromTrans.position.x;
        var yDistance = toTrans.position.y - fromTrans.position.y;
        var angle = Mathf.Atan2(xDistance, yDistance) * Mathf.Rad2Deg;
        angle = -Get360Angle(angle);

        return angle;
    }

    /// <summary>
    /// Get Y angle from two transforms position.
    /// </summary>
    private static float GetYangleFromTwoPosition(Transform fromTrans, Transform toTrans)
    {
        if (fromTrans == null || toTrans == null)
        {
            return 0f;
        }
        var xDistance = toTrans.position.x - fromTrans.position.x;
        var zDistance = toTrans.position.z - fromTrans.position.z;
        var angle = Mathf.Atan2(xDistance, zDistance) * Mathf.Rad2Deg;
        angle = -Get360Angle(angle);

        return angle;
    }
    
    /// <summary>
    /// Get angle from two vector3 position.
    /// </summary>
    public static float GetAngleFromTwoPosition(Vector3 fromPos, Vector3 toPos, Util.AXIS axisMove)
    {
        switch (axisMove)
        {
            case AXIS.X_AND_Y:
                return GetZangleFromTwoPosition(fromPos, toPos);
            case AXIS.X_AND_Z:
                return GetYangleFromTwoPosition(fromPos, toPos);
            default:
                return 0f;
        }
    }
    
    /// <summary>
    /// Get Z angle from two transforms position.
    /// </summary>
    private static float GetZangleFromTwoPosition(Vector3 fromPos, Vector3 toPos)
    {
        if (fromPos == null || toPos == null)
        {
            return 0f;
        }
        var xDistance = toPos.x - fromPos.x;
        var yDistance = toPos.y - fromPos.y;
        var angle = Mathf.Atan2(xDistance, yDistance) * Mathf.Rad2Deg;
        angle = -Get360Angle(angle);

        return angle;
    }

    /// <summary>
    /// Get Y angle from two transforms position.
    /// </summary>
    private static float GetYangleFromTwoPosition(Vector3 fromPos, Vector3 toPos)
    {
        if (fromPos == null || toPos == null)
        {
            return 0f;
        }
        var xDistance = toPos.x - fromPos.x;
        var zDistance = toPos.z - fromPos.z;
        var angle = Mathf.Atan2(xDistance, zDistance) * Mathf.Rad2Deg;
        angle = -Get360Angle(angle);

        return angle;
    }
}