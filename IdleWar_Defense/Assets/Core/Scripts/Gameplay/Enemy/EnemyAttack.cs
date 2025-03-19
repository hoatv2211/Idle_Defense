using Spine;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using UnityModule.Utility;
using Utilities.Common;
using JsonHelper = Utilities.Common.JsonHelper;
using Zirpl.CalcEngine;
using System;
using System.Linq;
using UnityEngine.Events;

public class EnemyAttack : MonoBehaviour
{
    protected EnemyExControl enemyExControl;
    protected float atkSpeed = 0.75f;
    private float currentPlayAttackTime;
    protected float threshold;

    public string[] nameOfAttackAnimations;
    public string eventNameAttack;
    public Spine.EventData eventDataAttack;

    public Vector2 offsetAttack;
    public float sizeAttackX = 0.5f;

    public int xCountAttackInAnim = 1;
    protected int countGun = 0;
    protected int maxGun;
    public BaseShot[] shots;
    public ParticleMan[] muzzles;
    public string soundStartAttack;
    public string soundTriggerAttack;

    public virtual void Init(EnemyExControl _enemyExControl)
    {
        enemyExControl = _enemyExControl;
        currentPlayAttackTime = 0f;
        threshold = enemyExControl.random * Config.enemyMoveThreshold;

        eventDataAttack = enemyExControl.skeletonAnimation.Skeleton.Data.FindEvent(eventNameAttack);
        enemyExControl.skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;

        maxGun = shots.Length;
        if (enemyExControl.BulletSpeed.Length == maxGun)
        {
            for (int i = 0; i < maxGun; i++)
            {
                shots[i].m_bulletSpeed = enemyExControl.BulletSpeed[i];
            }
        }
        else
        {
            if (enemyExControl.BulletSpeed.Length > 0)
                for (int i = 0; i < maxGun; i++)
                {
                    shots[i].m_bulletSpeed = enemyExControl.BulletSpeed[0];
                }
        }


        enabled = true;
    }

    public void End()
    {
        enabled = false;
    }

    private void FixedUpdate()
    {
        if (currentPlayAttackTime <= 0f)
        {
            if (CanAttack())
            {
                Attack();
            }
        }

        currentPlayAttackTime -= Time.fixedDeltaTime;
        if (currentPlayAttackTime < 0f) currentPlayAttackTime = 0f; //chỉ gán khi < 0 cho nhẹ
    }

    public virtual void Attack()
    {
        enemyExControl.StopMove(atkSpeed);
        enemyExControl.AnimAttack();
    }

    public bool Attacking()
    {
        return currentPlayAttackTime > 0f;
    }

    public virtual TrackEntry OnAttack()
    {
        enemyExControl.SetInvisible(false);

        if (!soundStartAttack.Equals("")) SoundManager.Instance.PlaySFX(soundStartAttack);

        currentPlayAttackTime = atkSpeed;
        // Debug.LogError("Atk Speed " + atkSpeed+"_"+ enemyExControl.AttackTime);
        var animName = nameOfAttackAnimations[UnityEngine.Random.Range(0, nameOfAttackAnimations.Length)];
        var trackEntry = enemyExControl.skeletonAnimation.AnimationState.SetAnimation(0, animName, false);
        trackEntry.Complete += AnimAttack_Complete;
        var duration = trackEntry.Animation.Duration;
        if (atkSpeed < duration) trackEntry.TimeScale = duration / atkSpeed;
        return trackEntry;
    }

    protected virtual void AnimAttack_Complete(TrackEntry trackEntry)
    {
        enemyExControl.AnimIdle();
    }

    public virtual RaycastHit2D[] CheckHits()
    {
        var range = enemyExControl.Range;
        return Physics2D.BoxCastAll(transform.position + (Vector3)offsetAttack + new Vector3(0f, -range / 2f, 0f),
                                    new Vector3(sizeAttackX + threshold / 2f, range + threshold, 0f), transform.localEulerAngles.z, Vector2.zero);
    }

    public virtual bool CanAttack()
    {
        if (enemyExControl.IsSleep()) return false;
        //qua mép màn hình trên một tẹo rồi mới attack, để người chơi còn biết con gì đang bắn
        if (transform.position.y > Config.LOWEST_Y) return false;
        //nếu melee attack thì phải đợi đúng target mới đánh
        // if (maxGun <= 0 || enemyExControl.target == null) return false;

        RaycastHit2D[] hits = CheckHits();
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_HERO))
            {
                atkSpeed = enemyExControl.AttackTime * xCountAttackInAnim; //mỗi lần attack cần gán lại vì còn theo boost
                return true;
            }
        }

        return false;
    }

    //-------------active event-----
    private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        CheckEvent(e);
    }

    public void CheckEvent(Spine.Event e)
    {
        bool eventMatch = (eventDataAttack == e.Data);
        if (eventMatch)
        {
            TriggerAttack();
        }
    }

    public virtual void TriggerAttack()
    {
        if (enemyExControl.IsDead()) return;

        if (!soundTriggerAttack.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerAttack);

        InfoAttacker infoAttacker = new InfoAttacker(false,
                                                    InfoAttacker.TYPE_NORMAL,
                                                    null,
                                                    enemyExControl,
                                                    null,
                                                    enemyExControl.Damage,
                                                    enemyExControl.CritRate,
                                                    enemyExControl.CritDamage,
                                                    enemyExControl.Accuracy);

        if (maxGun > 0)
        {
            //đánh range thì spawn bullet
            if (muzzles != null && muzzles.Length > 0)
            {
                muzzles[countGun].Play();
            }

            shots[countGun].Shot(infoAttacker);

            countGun++;
            if (countGun >= maxGun) countGun = 0;
        }
        else
        {
            //đánh melee thì check ray cast
            RaycastHit2D[] hits = CheckHits();
            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_HERO))
                {
                    var heroControl = item.collider.GetComponent<HeroControl>();
                    // if (heroControl == enemyExControl.target)
                    // {
                    heroControl.GetHit(infoAttacker);
                    return;
                    // }
                }
            }
        }
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var dataContent = Resources.Load<TextAsset>("Data/Enemy");
        var enemyDefinitions = JsonHelper.GetJsonList<EnemyDefinition>(dataContent.text);
        Resources.UnloadAsset(dataContent);

        dataContent = Resources.Load<TextAsset>("Data/EnemyLevelUpStat");
        var enemyLevelUpStatDefinitions = JsonHelper.GetJsonList<EnemyLevelUpStat>(dataContent.text);
        Resources.UnloadAsset(dataContent);

        dataContent = Resources.Load<TextAsset>("Data/Boss");
        enemyDefinitions.AddRange(JsonHelper.GetJsonList<EnemyDefinition>(dataContent.text));
        Resources.UnloadAsset(dataContent);

        dataContent = Resources.Load<TextAsset>("Data/BossLevelUpStat");
        enemyLevelUpStatDefinitions.AddRange(JsonHelper.GetJsonList<EnemyLevelUpStat>(dataContent.text));
        Resources.UnloadAsset(dataContent);

        string s1 = name[1] + "";
        int id;
        if (name.Length > 2)
        {
            string s2 = name[2] + "";
            string s = s1;
            int value;
            if (int.TryParse(s2, out value)) s = s1 + s2;
            id = int.Parse(s);//A12
        }
        else
        {
            id = int.Parse(s1); //A1
        }
        float range = GetStat(enemyDefinitions[id - 1].attackRange, enemyLevelUpStatDefinitions[id - 1].attackRange, 1) * Config.xRange;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)offsetAttack + new Vector3(0f, -range / 2f, 0f), new Vector3(sizeAttackX, range, 0f));
        var e = enemyDefinitions[id - 1];
        string bulletSpeedString = "Bullet speed ";
        foreach (var item in this.shots)
        {
            if (bulletSpeedString.Length > 0)
                bulletSpeedString += "|";
            bulletSpeedString += item.m_bulletSpeed;
        }
        UnityEditor.Handles.color = Color.white;
        //   UnityEditor.Handles. = Color.white;
        UnityEditor.Handles.Label(transform.position + new Vector3(0, -0.5f, 0), bulletSpeedString + "\n" + (id - 1) + "." + e.name, new GUIStyle()
        {
            fontSize = 30
        });
    }
#endif
    public static float GetStat(float statBase, float statAdd, int level)
    {
        var dataContent = GameData.GetTextContent("Data/Formula");
        var formulaDefinitions = JsonHelper.GetJsonList<FormulaDefinition>(dataContent);

        var formulaDefinition = formulaDefinitions.Where(
                e => e.stat == "stat"
                ).FirstOrDefault().formula;

        var calculator = new CalculationEngine();
        calculator.Variables["statBase"] = statBase;
        calculator.Variables["statAdd"] = statAdd;
        calculator.Variables["level"] = level;

        return Convert.ToSingle(calculator.Evaluate(formulaDefinition));
    }
}
