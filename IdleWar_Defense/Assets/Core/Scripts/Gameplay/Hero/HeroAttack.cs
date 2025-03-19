using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoodZombie;
using UnityEngine;
using Utilities.Common;
using Zirpl.CalcEngine;

public class HeroAttack : MonoBehaviour
{
    protected HeroExControl heroExControl;
    protected float atkSpeed = 0.75f;
    protected float currentPlayAttackTime;

    public string[] nameOfAttackAnimations;
    public string eventNameAttack;
    public Spine.EventData eventDataAttack;

    public float sizeAttackX = 0.5f;

    public int xCountAttackInAnim = 1;
    protected int countGun = 0;
    protected int maxGun;
    public BaseShot[] shots;
    public ParticleMan[] muzzles;

    public string soundStartAttack;
    public string soundTriggerAttack;

    public void Init(HeroExControl _heroExControl)
    {
        heroExControl = _heroExControl;
        currentPlayAttackTime = 0f;

        eventDataAttack = heroExControl.skeletonAnimation.Skeleton.Data.FindEvent(eventNameAttack);
        heroExControl.skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;

        maxGun = shots.Length;
        foreach (BaseShot shot in shots)
        {
            shot.m_bulletSpeed = heroExControl.BulletSpeed;
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
            //liên tục kiểm tra có thể attack ko
            if (CanAttack())
            {
                Attack();
            }
        }

        currentPlayAttackTime -= Time.fixedDeltaTime;
        if (currentPlayAttackTime < 0f) currentPlayAttackTime = 0f; //chỉ gán khi < 0 cho nhẹ
    }

    //gọi sang control để đổi stage, đổi stage sẽ gọi vào OnAttack
    public virtual void Attack()
    {
        if (heroExControl != null)
            heroExControl.AnimAttack();
    }

    public bool Attacking()
    {
        return currentPlayAttackTime > 0f;
    }

    public virtual TrackEntry OnAttack()
    {
        if (!soundStartAttack.Equals("")) SoundManager.Instance.PlaySFX(soundStartAttack);

        currentPlayAttackTime = atkSpeed;

        var animName = nameOfAttackAnimations[UnityEngine.Random.Range(0, nameOfAttackAnimations.Length)];
        var trackEntry = heroExControl.skeletonAnimation.AnimationState.SetAnimation(0, animName, false);
        trackEntry.Complete += AnimAttack_Complete;
        var duration = trackEntry.Animation.Duration;
        if (atkSpeed < duration) trackEntry.TimeScale = duration / atkSpeed;
        return trackEntry;
    }

    protected void AnimAttack_Complete(TrackEntry trackEntry)
    {
        heroExControl.AnimIdle();
    }

    public virtual bool CanAttack()
    {
        //nếu melee attack thì phải đợi đúng target mới đánh
        if (maxGun <= 0 || heroExControl.target == null) return false;

        // var enemies = GameplayController.Instance.GetEnemies();
        // for (int i = 0; i < enemies.Count; i++)
        // {
        //     var item = enemies[i];
        //     var enemyPosY = item.transform.position.y;
        //qua mép màn hình trên một tẹo hero mới bắn, vì có thể bắn phát chết mọe enemy thì ko thấy gì chạy ra
        if (heroExControl.target.transform.position.y <= Config.LOWEST_Y)
        {
            //xCountAttackInAnim - VD: attackSpeed là 4 - 1 giây đánh 4 phát, nhưng trong animation có xCountAttackInAnim = 4
            //tức là 4 event attack trong 1 anim, attackSpeed là 4  -> attackTime là 0.25f và phải nhân thêm 4
            //thì lúc ấy diễn cả anim trong 1 giây và có 4 event = đánh 4 phát
            atkSpeed = heroExControl.AttackTime * xCountAttackInAnim; //mỗi lần attack cần gán lại vì còn theo boost
            atkSpeed = atkSpeed * (1 - GameplayController.Instance.BuffHeroObject.ASAddPercent);
            return true;
        }
        // }

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
        if (heroExControl.IsDead()) return;

        if (!soundTriggerAttack.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerAttack);

        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_NORMAL,
                                                        heroExControl,
                                                        null,
                                                        null,
                                                        heroExControl.Damage,
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        heroExControl.Knockback,
                                                        transform.position.y,
                                                        heroExControl.Range);
        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2)transform.position + heroExControl.Offset, 3f * Config.xRange, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyControl>();
                if (enemyControl == heroExControl.target)
                {
                    enemyControl.GetHit(infoAttacker);

                    if (maxGun > 0)
                    {
                        //đánh range thì chỉ gọi muzzles không cần gọi bullet
                        if (muzzles != null && muzzles.Length > 0)
                        {
                            muzzles[countGun].Play();
                        }

                        countGun++;
                        if (countGun >= maxGun) countGun = 0;
                    }

                    //vầy là xong
                    return;
                }
            }
        }

        //
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
            // var range = heroExControl.Range;
            var range = Config.HERO_ATTACK_RANGE_Y;
            hits = Physics2D.BoxCastAll((Vector2)transform.position /*+ offsetAttack*/ + new Vector2(0f, range / 2f),
                                            new Vector2(sizeAttackX, range), 0f, Vector2.zero);

            foreach (var item in hits)
            {
                if (item.collider.CompareTag(Config.TAG_ENEMY))
                {
                    var enemyControl = item.collider.GetComponent<EnemyControl>();
                    if (enemyControl == heroExControl.target)
                    {
                        enemyControl.GetHit(infoAttacker);
                    }
                }
            }
        }
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var dataContent = Resources.Load<TextAsset>("Data/Hero");
        var heroDefinitions = JsonHelper.GetJsonList<HeroDefinition>(dataContent.text);
        Resources.UnloadAsset(dataContent);
        string bulletSpeedString = "bullet speed: ";
        foreach (var item in this.shots)
        {
            if (bulletSpeedString.Length > 0)
                bulletSpeedString += "|";
            bulletSpeedString += item.m_bulletSpeed;
        }

        dataContent = Resources.Load<TextAsset>("Data/HeroLevelUpStat");
        var heroLevelUpStatDefinitions = JsonHelper.GetJsonList<HeroLevelUpStat>(dataContent.text);
        Resources.UnloadAsset(dataContent);
        // Debug.LogError("name length " + name.Length);
        string s = name.Remove(0, 1);
        //string s1 = name[1] + "";
        ////if (name.Length >= 3)
        //string s2 = name[2] + "";
        //string s = s1;
        //int value;
        //if (int.TryParse(s2, out value)) s = s1 + s2;
        int id = -1;//A12
        if (!int.TryParse(s, out id)) return;
        var hero = heroDefinitions[id - 1];
        float range = GetStat(heroDefinitions[id - 1].attackRange, heroLevelUpStatDefinitions[id - 1].attackRange, 1) * Config.xRange;

        UnityEditor.Handles.color = Color.white;
        //   UnityEditor.Handles. = Color.white;
        UnityEditor.Handles.Label(transform.position + new Vector3(0, -1, 0), (id - 1) + "." + hero.name + "\n" + bulletSpeedString, new GUIStyle()
        {
            fontSize = 30
        });
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position /*+ (Vector3) offsetAttack*/ + new Vector3(0f, range / 2f, 0f), new Vector3(sizeAttackX, range, 0f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3)GetComponent<HeroExControl>().Offset, 3f * Config.xRange);
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
