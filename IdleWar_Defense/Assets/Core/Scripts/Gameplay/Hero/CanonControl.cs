using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Inspector;

public class CanonControl : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;

    public CanonAttack attack;
    public CanonAutoTarget autoTarget;

    [Separator("Aim")]
    [SpineBone] public string[] boneAimNames;
    private Spine.Bone[] boneAims;
    public float[] offsetRotationAims;
    private Vector3 posAim;

    [HideInInspector]
    public EnemyControl target;
    private float random = 1f;

    private const int ANIM_IDLE_STAGE = 1;
    private const int ANIM_ATTACK_STAGE = 3;

    [SerializeField] protected string[] nameOfIdleAnimations;

    #region info

    private float damage;
    public float Damage
    {
        get
        {
            return damage;
        }
    }

    private float attackTime;
    public float AttackTime
    {
        get
        {
            return attackTime;
        }
    }

    #endregion

    [HideInInspector]
    public int stage = -1;
    public int STAGE
    {
        get
        {
            return stage;
        }
        set
        {
            TrackEntry trackEntry;

            switch (value)
            {
                case ANIM_IDLE_STAGE:
                    if (stage != value && !attack.Attacking())
                    {
                        trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true);
                        if (stage != ANIM_ATTACK_STAGE) skeletonAnimation.Update(Mathf.Abs(random) / 2f);
                        trackEntry.TimeScale = 1f + random / 4f;
                        stage = value;
                    }

                    break;
                case ANIM_ATTACK_STAGE:
                    if (!attack.Attacking())
                    {
                        attack.OnAttack();
                        skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
                        stage = value;
                    }

                    break;

            }

        }
    }

    public void Init(float _damage)
    {
        damage = _damage;
        attackTime = 0.17f; //để attack 4 phát một giây
        attackTime -= attackTime * GameplayController.Instance.BuffHeroObject.ASAddPercent;
        skeletonAnimation.Initialize(false);
        skeletonAnimation.skeleton.A = 1f;
        random = UnityEngine.Random.Range(-1f, 1f);

        target = null;
        if (attack != null) attack.Init(this);
        if (autoTarget != null) autoTarget.Init(this);

        //look at
        var count = boneAimNames.Length;
        boneAims = new Bone[count];
        for (int i = 0; i < count; i++)
        {
            boneAims[i] = skeletonAnimation.Skeleton.FindBone(boneAimNames[i]);
        }
        posAim = transform.up * 5f;
        //lúc đầu mọi hero nhìn thẳng lên
    }

    private void Start()
    {
        StartAim();
    }

    private void OnDestroy()
    {
        EndAim();
    }

    private void SkeletonAnimation_UpdateLocal(ISkeletonAnimation animated)
    {
        var rotZ = Util.GetAngleFromTwoPosition(transform.position, posAim, Util.AXIS.X_AND_Y);
        var rotZrad = rotZ * Mathf.Deg2Rad;
        var cosRot = Mathf.Cos(rotZrad);
        var cosMinR = Mathf.Cos(80 * Mathf.Deg2Rad);

        if ((cosMinR > 0 && (cosRot < cosMinR))
            || (cosMinR < 0 && (cosRot > cosMinR)))
        {
            if (Mathf.Sin(rotZrad) > 0f) rotZ = 80f;
            else rotZ = -80f;
        }
        var count = boneAims.Length;
        for (int i = 0; i < count; i++)
        {
            boneAims[i].Rotation = rotZ + offsetRotationAims[i];
        }
    }

    private void FixedUpdate()
    {
        if (target != null && !target.IsDead() &&
            (!GameplayController.Instance.holdingTap
             || GameplayController.Instance.autoPlay))
        {
            LookAt(target.transform.position + (Vector3)target.Offset);
        }
    }

    public void StartAim()
    {
        skeletonAnimation.UpdateLocal += SkeletonAnimation_UpdateLocal;
    }

    public void EndAim()
    {
        skeletonAnimation.UpdateLocal -= SkeletonAnimation_UpdateLocal;

        var count = boneAims.Length;
        for (int i = 0; i < count; i++)
        {
            boneAims[i].Rotation = 0f + offsetRotationAims[i];
        }
    }

    public void AnimIdle()
    {
        STAGE = ANIM_IDLE_STAGE;
    }

    public void AnimAttack()
    {
        STAGE = ANIM_ATTACK_STAGE;
    }

    public void SetTarget(EnemyControl _target)
    {
        target = _target;
    }

    public void LookAt(Vector3 _posAim)
    {
        posAim = _posAim;
    }
}
