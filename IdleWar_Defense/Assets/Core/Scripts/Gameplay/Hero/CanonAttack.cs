using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoodZombie;
using UnityEngine;
using Utilities.Common;
using Zirpl.CalcEngine;

public class CanonAttack : MonoBehaviour
{
    protected CanonControl canonControl;
    protected float atkSpeed = 0.75f; //tạm thôi ko có giá trị 

    public string[] nameOfAttackAnimations;
    public string eventNameAttack;
    public Spine.EventData eventDataAttack;
    
    protected float currentPlayAttackTime;

    public int xCountAttackInAnim = 1;
    protected int countGun = 0;
    protected int maxGun;
    public BaseShot[] shots;
    public ParticleMan[] muzzles;
    
    public string soundStartAttack;
    public string soundTriggerAttack;
    
    public void Init(CanonControl _canonControl)
    {
        canonControl = _canonControl;
        currentPlayAttackTime = 0f;

        eventDataAttack = canonControl.skeletonAnimation.Skeleton.Data.FindEvent(eventNameAttack);
        canonControl.skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
        
        maxGun = shots.Length;
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
        canonControl.AnimAttack();
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
        var trackEntry = canonControl.skeletonAnimation.AnimationState.SetAnimation(0, animName, false);
        trackEntry.Complete += AnimAttack_Complete;
        var duration = trackEntry.Animation.Duration;
        if (atkSpeed < duration) trackEntry.TimeScale = duration / atkSpeed;
        return trackEntry;
    }

    protected void AnimAttack_Complete(TrackEntry trackEntry)
    {
        canonControl.AnimIdle();
    }

    public virtual bool CanAttack()
    {
        //nếu melee attack thì phải đợi đúng target mới đánh
        if (maxGun <= 0 && canonControl.target == null) return false;

        var enemies = GameplayController.Instance.GetEnemies();
        var posY = transform.position.y;
        for (int i = 0; i < enemies.Count; i++)
        {
            var item = enemies[i];
            var enemyPosY = item.transform.position.y;
            if (enemyPosY <= Config.LOWEST_Y)
            {
                atkSpeed = canonControl.AttackTime * xCountAttackInAnim;; //mỗi lần attack cần gán lại vì còn theo boost
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
        if (!soundTriggerAttack.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerAttack);

        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_NORMAL,
                                                        null,
                                                        null,
                                                        null,
                                                        canonControl.Damage * 0.17f,
                                                        0f, //damage từ trap nên ko có crit
                                                        100f, //hệ số nhân crit giữ nguyên = 1f
                                                        100f); //để thế này cho lớn hơn dodge rồi mới chia 100f

        //nếu enemies ở gần dưới 3 ô
        RaycastHit2D[] hits = Physics2D.CircleCastAll((Vector2) transform.position + new Vector2(0f, 2.0f), 3f * Config.xRange, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyControl>();
                if (enemyControl == canonControl.target)
                {
                    enemyControl.GetHit(infoAttacker);

                    if (maxGun > 0)
                    {
                        //đánh range thì spawn bullet
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

        //đánh range thì spawn bullet
        if (muzzles != null && muzzles.Length > 0)
        {
            muzzles[countGun].Play();
        }

        shots[countGun].Shot(infoAttacker);

        countGun++;
        if (countGun >= maxGun) countGun = 0;
    }

    void OnDrawGizmosSelected()
    {        
        var dataContent = Resources.Load<TextAsset>("Data/Hero");
        var heroDefinitions = JsonHelper.GetJsonList<HeroDefinition>(dataContent.text);
        Resources.UnloadAsset(dataContent);

        dataContent = Resources.Load<TextAsset>("Data/HeroLevelUpStat");
        var heroLevelUpStatDefinitions = JsonHelper.GetJsonList<HeroLevelUpStat>(dataContent.text);
        Resources.UnloadAsset(dataContent);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, 2.0f, 0f), 3f * Config.xRange);
    }
}
