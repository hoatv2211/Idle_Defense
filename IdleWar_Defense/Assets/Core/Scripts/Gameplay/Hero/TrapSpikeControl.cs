using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using Utilities.Common;
using Debug = UnityEngine.Debug;

public class TrapSpikeControl : TrapControl
{
    public GameObject imgSpike;
    
    protected const int ANIM_IDLE_STAGE = 5;

    private const float DURATION_TIME = 1f;
    protected float currentPlayAttackTime = DURATION_TIME;
    
    public override int STAGE
    {
        get
        {
            return stage;
        }
        set
        {
            if (stage != ANIM_DEAD_STAGE)
            {
                switch (value)
                {
                    case ANIM_DEAD_STAGE:
                        if (stage != value)
                        {
                            OnDead();
                            stage = value;
                        }
                        break;
                    case ANIM_IDLE_STAGE:
                        if (stage != value)
                        {
                            stage = value;
                        }
                        break;
                    case ANIM_ATTACK_STAGE:
                        if (stage != value)
                        {
                            OnAttack();
                            stage = value;
                            AnimAttack_Complete();
                        }
                        break;
                    case ANIM_ACTIVE_STAGE:
                        if (stage != value)
                        {
                            OnActive();
                            stage = value;
                        }
                        break;
                }
            }
        }
    }
    
    protected override void FixedUpdate()
    {
        if (!IsDead())
        {
            if (currentPlayAttackTime <= 0f)
            {
                Attack();
                currentPlayAttackTime = DURATION_TIME;
                
                imgSpike.SetActive(true);
            }

            if (currentPlayAttackTime <= DURATION_TIME / 2f)
            {
                imgSpike.SetActive(false);
            }
            
            currentPlayAttackTime -= Time.fixedDeltaTime;
        }
    }
    
    public void AnimIdle()
    {
        STAGE = ANIM_IDLE_STAGE;
    }
    
    public override void OnAttack()
    {
        TriggerAttack();
    }

    public override void AnimAttack_Complete()
    {
        AnimIdle();
    }

    public virtual void TriggerAttack()
    {
        var hasEnemy = false;
        
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        null,
                                                        null,
                                                        null,
                                                        damage,
                                                        0f, //damage từ trap nên ko có crit
                                                        100f, //hệ số nhân crit giữ nguyên = 100f
                                                        100f); //để thế này cho lớn hơn dodge rồi mới chia 100f
        
        RaycastHit2D[] hits = CheckHits();
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                var enemyControl = item.collider.GetComponent<EnemyControl>();
                enemyControl.GetHit(infoAttacker);

                hasEnemy = true;
            }
        }

        if (hasEnemy)
        {
            if (!soundAttack.Equals("")) SoundManager.Instance.PlaySFX(soundAttack);
            if (effectAttack != null) effectAttack.Play();
        }
    }
}
