using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// nway lock on shot.
/// </summary>
[AddComponentMenu("Bullet/Shot Pattern/nWay Shot (Lock On)")]
public class NwayLockOnShot : NwayShot
{
    [Header("===== NwayLockOnShot Settings =====")]
    // "Set a target with tag name."
    [FormerlySerializedAs("_SetTargetFromTag")]
    public bool m_setTargetFromTag = true;
    // "Set a unique tag name of target at using SetTargetFromTag."
    [FormerlySerializedAs("_TargetTagName")]
    public string m_targetTagName = "Player";
    // "Transform of lock on target."
    // "It is not necessary if you want to specify target in tag."
    // "Overwrite CenterAngle in direction of target to Transform.position."
    [FormerlySerializedAs("_TargetTransform")]
    public Transform m_targetTransform;
    // "Always aim to target."
    [FormerlySerializedAs("_Aiming")]
    public bool m_aiming;

    public override void Shot(InfoAttacker infoAttacker)
    {
        if (m_shooting)
        {
            return;
        }

        AimTarget();

        if (m_targetTransform == null)
        {
            Debug.LogWarning("Cannot shot because TargetTransform is not set.");
            return;
        }

        base.Shot(infoAttacker);

        if (m_aiming)
        {
            StartCoroutine(AimingCoroutine());
        }
    }

    private void AimTarget()
    {
        if (m_targetTransform == null && m_setTargetFromTag)
        {
            m_targetTransform = Util.GetTransformFromTagName(m_targetTagName);
        }
        if (m_targetTransform != null)
        {
            m_centerAngle = Util.GetAngleFromTwoPosition(transform, m_targetTransform, Util.AXIS.X_AND_Y);
        }
    }

    private IEnumerator AimingCoroutine()
    {
        while (m_aiming)
        {
            if (m_shooting == false)
            {
                yield break;
            }

            AimTarget();

            yield return 0;
        }

        yield break;
    }
}