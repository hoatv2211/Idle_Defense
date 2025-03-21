﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Utilities.Common;
using Debug = UnityEngine.Debug;

/// <summary>
/// base shot.
/// Each shot pattern classes inherit this class.
/// </summary>
public abstract class BaseShot : MonoBehaviour
{
    [Header("===== Common Settings =====")]
    // "Set a bullet prefab for the shot. (ex. sprite or model)"
    [FormerlySerializedAs("_BulletPrefab")]
    public Bullet m_bulletPrefab;
    // "Set a bullet number of shot."
    [FormerlySerializedAs("_BulletNum")]
    public int m_bulletNum = 10;
    // "Set a bullet base speed of shot."
    [FormerlySerializedAs("_BulletSpeed")]
    public float m_bulletSpeed = 2f;
    // "Set an acceleration of bullet speed."
    [FormerlySerializedAs("_AccelerationSpeed")]
    public float m_accelerationSpeed = 0f;
    // "Set an acceleration of bullet turning."
    [FormerlySerializedAs("_AccelerationTurn")]
    public float m_accelerationTurn = 0f;
    // "This flag is pause and resume bullet at specified time."
    [FormerlySerializedAs("_UsePauseAndResume")]
    public bool m_usePauseAndResume = false;
    // "Set a time to pause bullet."
    [FormerlySerializedAs("_PauseTime")]
    public float m_pauseTime = 0f;
    // "Set a time to resume bullet."
    [FormerlySerializedAs("_ResumeTime")]
    public float m_resumeTime = 0f;
    // "This flag is automatically release the bullet GameObject at the specified time."
    [FormerlySerializedAs("_UseAutoRelease")]
    public bool m_useAutoRelease = false;
    // "Set a time to automatically release after the shot at using UseAutoRelease. (sec)"
    [FormerlySerializedAs("_AutoReleaseTime")]
    public float m_autoReleaseTime = 10f;

    [Space(10)]

    // "Set a callback method fired shot."
    [SerializeField]
    private UnityEvent m_shotFiredCallbackEvents = new UnityEvent();
    // "Set a callback method after shot."
    [SerializeField]
    private UnityEvent m_shotFinishedCallbackEvents = new UnityEvent();

    protected bool m_shooting;

    /// <summary>
    /// is shooting flag.
    /// </summary>
    public bool shooting { get { return m_shooting; } }

    /// <summary>
    /// Call from override OnDisable method in inheriting classes.
    /// Example : protected override void OnDisable () { base.OnDisable (); }
    /// </summary>
    protected virtual void OnDisable()
    {
        m_shooting = false;
    }

    /// <summary>
    /// Abstract shot method.
    /// </summary>
    public abstract void Shot(InfoAttacker infoAttacker);
    
    /// <summary>
    /// Fired shot.
    /// </summary>
    protected void FiredShot()
    {
        m_shotFiredCallbackEvents.Invoke();
    }

    /// <summary>
    /// Finished shot.
    /// </summary>
    protected void FinishedShot()
    {
        m_shotFinishedCallbackEvents.Invoke();
        m_shooting = false;
    }

    /// <summary>
    /// Get Bullet object from object pool.
    /// </summary>
    protected Bullet GetBullet(Vector3 position, Quaternion rotation, InfoAttacker infoAttacker)
    {
        if (m_bulletPrefab == null)
        {
            Debug.LogWarning("Cannot generate a bullet because BulletPrefab is not set.");
            return null;
        }

        // get Bullet from ObjectPool
        Bullet bullet = GameplayController.Instance.SpawnBullet(m_bulletPrefab, position, rotation, infoAttacker);
        if (bullet == null)
        {
            return null;
        }

        return bullet;
    }

    /// <summary>
    /// Shot Bullet object.
    /// </summary>
    protected void ShotBullet(Bullet bullet, float speed, float angle,
                               bool homing = false, Transform homingTarget = null, float homingAngleSpeed = 0f,
                               bool wave = false, float waveSpeed = 0f, float waveRangeSize = 0f)
    {
        if (bullet == null)
        {
            return;
        }
        bullet.Shot(speed, angle, m_accelerationSpeed, m_accelerationTurn,
                    homing, homingTarget, homingAngleSpeed,
                    wave, waveSpeed, waveRangeSize,
                    m_usePauseAndResume, m_pauseTime, m_resumeTime, Util.AXIS.X_AND_Y);
    }

    /// <summary>
    /// Auto release bullet GameObject.
    /// </summary>
    protected void AutoReleaseBullet(Bullet bullet)
    {
        if (m_useAutoRelease == false || m_autoReleaseTime < 0f)
        {
            return;
        }
        CoroutineUtil.StartCoroutine(AutoReleaseBulletCoroutine(bullet));
    }
    
    private IEnumerator AutoReleaseBulletCoroutine(Bullet bullet)
    {
        float countUpTime = 0f;

        while (true)
        {
            if (bullet == null || bullet.gameObject == null || bullet.gameObject.activeSelf == false)
            {
                yield break;
            }

            if (m_autoReleaseTime <= countUpTime)
            {
                break;
            }

            yield return 0;

            countUpTime += BulletTimer.instance.deltaTime;
        }

        GameplayController.Instance.ReleaseBullet(bullet);
    }
}