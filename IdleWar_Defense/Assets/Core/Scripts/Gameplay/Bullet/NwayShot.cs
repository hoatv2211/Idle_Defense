using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities.Common;
using Debug = UnityEngine.Debug;

/// <summary>
/// nWay shot.
/// </summary>
[AddComponentMenu("Bullet/Shot Pattern/nWay Shot")]
public class NwayShot : BaseShot
{
    [Header("===== NwayShot Settings =====")]
    public bool InfinityShot = false;
    public float delayBetweenShot = 0.0f;
    // "Set a number of shot way."
    [FormerlySerializedAs("_WayNum")]
    public int m_wayNum = 5;
    // "Set a center angle of shot. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_CenterAngle")]
    public float m_centerAngle = 180f;
    // "Set a angle between bullet and next bullet. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_BetweenAngle")]
    public float m_betweenAngle = 10f;
    // "Set a delay time between shot and next line shot. (sec)"
    [FormerlySerializedAs("_NextLineDelay")]
    public float m_nextLineDelay = 0.1f;

    public override void Shot(InfoAttacker infoAttacker)
    {
        CoroutineUtil.StartCoroutine(ShotCoroutine(infoAttacker));
    }
    public bool canShot()
    {
        if (m_shooting) return false;
        return true;
    }

    private IEnumerator ShotCoroutine(InfoAttacker infoAttacker)
    {
        if ((m_bulletNum <= 0 && !InfinityShot) || m_bulletSpeed <= 0f || m_wayNum <= 0)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed or WayNum is not set.");
            yield break;
        }
        if (m_shooting)
        {
            yield break;
        }
        m_shooting = true;

        int wayIndex = 0;

        for (int i = 0; i < m_bulletNum; i++)
        {
            if (m_wayNum <= wayIndex)
            {
                wayIndex = 0;

                if (0f < m_nextLineDelay)
                {
                    FiredShot();
                    yield return Util.WaitForSeconds(m_nextLineDelay);
                }
            }

            var bullet = GetBullet(transform.position, transform.rotation, infoAttacker);
            if (bullet == null)
            {
                break;
            }

            var centerAngle = m_centerAngle + transform.rotation.eulerAngles.z;
            float baseAngle = m_wayNum % 2 == 0 ? centerAngle - (m_betweenAngle / 2f) : centerAngle;

            float angle = Util.GetShiftedAngle(wayIndex, baseAngle, m_betweenAngle);

            ShotBullet(bullet, m_bulletSpeed, angle);

            AutoReleaseBullet(bullet);

            wayIndex++;
        }

        FiredShot();

        if (InfinityShot && delayBetweenShot > 0)
            yield return new WaitForSeconds(delayBetweenShot);
        FinishedShot();

        yield break;
    }
}