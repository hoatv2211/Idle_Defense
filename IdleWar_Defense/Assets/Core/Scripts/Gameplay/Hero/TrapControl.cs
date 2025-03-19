using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using Utilities.Common;

public class TrapControl : MonoBehaviour
{
    public SpriteRenderer imgBody;

    public Vector2 offset;
    public Vector2 range = new Vector2(2f * Config.xRange, 2f * Config.xRange); //Đặt trên map, quân địch dẫm phải sẽ gây sát thương lớn trong khoảng 1 ô.

    protected const int ANIM_ACTIVE_STAGE = 1;
    protected const int ANIM_ATTACK_STAGE = 3;
    protected const int ANIM_DEAD_STAGE = 0;
    
    public ParticleMan effectAttack;

    public string soundActive;
    public string soundAttack;
    public string soundDie;

    [HideInInspector]
    public TrapData trapData;

    #region Info
    
    protected float damage;
    // protected float duration;
    
    #endregion
    
    protected IEnumerator disapear = null;

    [HideInInspector]
    public int stage = -1;
    public virtual int STAGE
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
    
    public virtual void Init(TrapData _trapData)
    {
        trapData = _trapData;
        
        damage = trapData.Damage;
        // duration = trapData.Duration;

        Refresh();
    }

    public virtual void Refresh()
    {
        stage = -1;
        STAGE = ANIM_ACTIVE_STAGE;
        
        if (disapear != null)
        {
            StopCoroutine(disapear);
            disapear = null;
        }
        var color = imgBody.color;
        color.a = 1f;
        imgBody.color = color;
    }
    
    protected virtual void FixedUpdate()
    {
        if (!IsDead())
        {
            if (CanAttack())
            {
                Attack();
            }
        }
    }
    
    public void AnimActive()
    {
        STAGE = ANIM_ACTIVE_STAGE;
    }
    
    public void AnimDead()
    {
        STAGE = ANIM_DEAD_STAGE;
    }
    
    public void Attack()
    {
        STAGE = ANIM_ATTACK_STAGE;
    }

    public bool IsDead()
    {
        return STAGE.Equals(ANIM_DEAD_STAGE);
    }

    protected virtual void OnActive()
    {
        if (!soundActive.Equals("")) SoundManager.Instance.PlaySFX(soundActive);
    }

    public virtual void OnAttack()
    {
        TriggerAttack();
    }

    public virtual void AnimAttack_Complete()
    {
        AnimDead();
    }
    
    public virtual RaycastHit2D[] CheckHits()
    {
        return Physics2D.BoxCastAll((Vector2) transform.position + offset, range, 0f, Vector2.zero);
    }

    public virtual bool CanAttack()
    {
        RaycastHit2D[] hits = CheckHits();
        foreach (var item in hits)
        {
            if (item.collider.CompareTag(Config.TAG_ENEMY))
            {
                return true;
            }
        }

        return false;
    }
    
    public virtual void TriggerAttack()
    {
        if (!soundAttack.Equals("")) SoundManager.Instance.PlaySFX(soundAttack);
        if (effectAttack != null) effectAttack.Play();

        StartCoroutine(IEAttack()); //effect nổ mới hiện dam, ko tự nhiên lại thấy enemies lăn đùng ra chết ngay
    }

    private IEnumerator IEAttack()
    {
        yield return new WaitForSeconds(0.2f);
        
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
            }
        }
    }
    
    protected virtual void OnDead()
    {
        if (!soundDie.Equals("")) SoundManager.Instance.PlaySFX(soundDie);

        disapear = IEDisapear();
        StartCoroutine(disapear);
    }

    private IEnumerator IEDisapear()
    {
        float timePlay = 1f;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            var color = imgBody.color;
            color.a = timePlay;
            imgBody.color = color;
        }

        gameObject.SetActive(false);
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3) offset, range);
    }
}
