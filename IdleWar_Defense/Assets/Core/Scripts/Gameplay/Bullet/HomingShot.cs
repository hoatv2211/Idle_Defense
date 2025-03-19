using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities.Common;
using Debug = UnityEngine.Debug;

public class HomingShot : BaseShot
{
    [Header("===== HomingShot Settings =====")]
    // "Set a delay time between bullet and next bullet. (sec)"
    [FormerlySerializedAs("_BetweenDelay")]
    public float m_betweenDelay = 0.1f;
    // "Set a speed of homing angle."
    [FormerlySerializedAs("_HomingAngleSpeed")]
    public float m_homingAngleSpeed = 20f;
    // "Set a target with tag name."
    [FormerlySerializedAs("_SetTargetFromTag")]
    public bool m_setTargetFromTag = true;
    // "Set a unique tag name of target at using SetTargetFromTag."
    [FormerlySerializedAs("_TargetTagName")]
    public string m_targetTagName = "Player";
    // "Transform of lock on target."
    // "It is not necessary if you want to specify target in tag."
    [FormerlySerializedAs("_TargetTransform")]
    public Transform m_targetTransform;
    public override void Shot(InfoAttacker infoAttacker)
    {
        if (m_setTargetFromTag && GetTransformFromTagName(m_targetTagName) == null) return;
        CoroutineUtil.StartCoroutine(ShotCoroutine(infoAttacker));
    }
    private IEnumerator ShotCoroutine(InfoAttacker infoAttacker)
    {
        if (m_bulletNum <= 0 || m_bulletSpeed <= 0f)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed is not set.");
            yield break;
        }
        if (m_shooting)
        {
            yield break;
        }
        m_shooting = true;

        for (int i = 0; i < m_bulletNum; i++)
        {
            if (0 < i && 0f < m_betweenDelay)
            {
                FiredShot();
                yield return UbhUtil.WaitForSeconds(m_betweenDelay);
            }

            var bullet = GetBullet(transform.position, transform.rotation, infoAttacker);
            if (bullet == null)
            {
                break;
            }

            if (m_targetTransform == null || IsTargetDead(m_targetTagName) && m_setTargetFromTag)
            {
                m_targetTransform = GetTransformFromTagName(m_targetTagName);
            }

            float angle = UbhUtil.GetAngleFromTwoPosition(transform, m_targetTransform, UbhUtil.AXIS.X_AND_Y);

            ShotBullet(bullet, m_bulletSpeed, angle, true, m_targetTransform, m_homingAngleSpeed);

            AutoReleaseBullet(bullet);
        }

        FiredShot();

        FinishedShot();

        yield break;
    }
    public static Transform GetTransformFromTagName(string tagName)
    {
        if (string.IsNullOrEmpty(tagName))
        {
            return null;
        }

        if (tagName.Equals("Hero"))
        {
            var heroesList = GameplayController.Instance.GetHeroes();
            if (heroesList != null && heroesList.Exists(a => !a.IsDead()))
            {
                var hero = heroesList.Find(a => !a.IsDead());
                return hero.transform;
            }
        }

        return null;
    }

    public bool IsTargetDead(string tagName)
    {
        if (tagName.Equals("Hero"))
        {
            return m_targetTransform.GetComponent<HeroControl>().IsDead();
        }

        return true;
    }
}
