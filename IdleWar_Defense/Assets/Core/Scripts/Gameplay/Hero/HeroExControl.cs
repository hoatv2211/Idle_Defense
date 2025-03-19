using FoodZombie;
using HedgehogTeam.EasyTouch;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Common;
using Utilities.Inspector;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class BuiltInPoolingParticleMan
{
    public ParticleMan prefab;
    public List<ParticleMan> builtIn;
}

public enum TypeSkill
{
    Click,
    CastLineOne,
    CastAoe,
    CastTarget,
    CastWall
}

public class HeroExControl : HeroControl
{
    //public float knockBack;


    protected float cooldown;
    protected float cooldown_MAX;

    private const int ANIM_ATTACK_STAGE = 3;
    private const int ANIM_SKILL_STAGE = 4;

    public Transform body;
    public SkeletonAnimation skeletonAnimation;
    public HeroAttack attack;
    public HeroSkill skill;
    public HeroPassiveSkill[] passiveSkills;
    public HeroAutoTarget autoTarget;

    public CooldownBar cooldownBar;

    public TypeSkill typeSkill = TypeSkill.Click;

    [Separator("Aim")]
    [SpineBone] public string[] boneAimNames;
    private Spine.Bone[] boneAims;
    public float[] offsetRotationAims;
    private Vector3 posAim;

    //nerf
    private Dictionary<int, SkillNerf> skillNerfs;
    //buff
    private Dictionary<int, SkillBuff> skillBuffs;
    private PoolsContainer<ParticleMan> particleManPools;
    [SerializeField] private List<BuiltInPoolingParticleMan> builtInParticleMan;

    private IEnumerator showRedColor = null;
    private IEnumerator showSkillNerfColor = null;

    private bool isShowedFullCooldown = false;


    #region info

    public bool Immortal
    {
        get
        {
            var count = skillBuffs.Count;
            bool immortal = false;
            for (int i = 0; i < count; i++)
            {
                var item = skillBuffs.ElementAt(i);
                // var itemKey = item.Key;
                var itemValue = item.Value;
                if (itemValue.immortal)
                {
                    immortal = true;
                    return immortal;
                }
            }
            return immortal;
        }
    }

    public bool Stun
    {
        get
        {
            var count = skillNerfs.Count;
            bool stun = false;
            for (int i = 0; i < count; i++)
            {
                var item = skillNerfs.ElementAt(i);
                // var itemKey = item.Key;
                var itemValue = item.Value;
                if (itemValue.stun)
                {
                    stun = true;
                    return stun;
                }
            }
            return stun;
        }
    }

    public float DamageReduction
    {
        get
        {
            var count = skillBuffs.Count;
            float totalDamageReduction = 0;
            for (int i = 0; i < count; i++)
            {
                var item = skillBuffs.ElementAt(i);
                // var itemKey = item.Key;
                var itemValue = item.Value;
                totalDamageReduction += itemValue.damageReduction;
            }
            return totalDamageReduction;
        }
    }

    public float Shield
    {
        get
        {
            int count = 0;
            if (skillBuffs != null)
                count = skillBuffs.Count;
            float totalShield = 0;
            for (int i = 0; i < count; i++)
            {
                var item = skillBuffs.ElementAt(i);
                // var itemKey = item.Key;
                var itemValue = item.Value;
                totalShield += itemValue.shield;
            }
            return totalShield;
        }
    }

    public float ShieldMax
    {
        get
        {
            int count = 0;
            if (skillBuffs != null) count = skillBuffs.Count;
            float totalShieldMax = 0;
            for (int i = 0; i < count; i++)
            {
                var item = skillBuffs.ElementAt(i);
                // var itemKey = item.Key;
                var itemValue = item.Value;
                totalShieldMax += itemValue.shieldMax;
            }
            return totalShieldMax;
        }
    }

    private float HpRegen
    {
        // Đồng thời +100% Hp regen cho mọi đồng đội trong 5s.
        // nếu có hai con surmy thì chỉ tính buff 1 con theo baseId trong buff
        get
        {
            int count = 0;
            if (skillBuffs != null)
                count = skillBuffs.Count;
            float totalMoreRegenHPPercent = 0;
            float totalMoreRegenHP = 0;

            for (int i = 0; i < count; i++)
            {
                var item = skillBuffs.ElementAt(i);
                // var itemKey = item.Key;
                var itemValue = item.Value;
                totalMoreRegenHPPercent += itemValue.moreRegenHPPercent;
                totalMoreRegenHP += itemValue.moreRegenHP;
            }
            return hpRegen * (1f + totalMoreRegenHPPercent / 100f) + totalMoreRegenHP;
        }
    }

    private float damage;
    public float Damage
    {
        get
        {
            var count = skillBuffs.Count;
            float moreDamagePercent = 0;
            for (int i = 0; i < count; i++)
            { // cộng thêm từ skill buff
                var item = skillBuffs.ElementAt(i);
                // var itemKey = item.Key;
                var itemValue = item.Value;
                if (itemValue.moreDamagePercent > 0)
                    moreDamagePercent += itemValue.moreDamagePercent - 100; //buff 150% damage tức là + 50% damage

            }

            if (passiveSkills != null) // cộng thêm từ passive
            {
                count = passiveSkills.Length;
                for (int i = 0; i < count; i++)
                {
                    if (passiveSkills[i].moreDamagePercent > 0)
                        moreDamagePercent += passiveSkills[i].moreDamagePercent;
                }
            }

            float output = damage * (1 + moreDamagePercent / 100f);
            float damGamePlay = GameplayController.Instance.BuffHeroObject.DamAddPercent;
            if (damGamePlay <= 0)
                return output;
            else
                return output * (1 + damGamePlay);
        }
    }
    private float bulletSpeed;
    public float BulletSpeed
    {
        get
        {
            return bulletSpeed;
        }
    }

    private float attackTime;
    public float AttackTime
    {
        get
        {
            var count = skillBuffs.Count;
            float totalMoreAttackSpeedPercent = 0;
            float totalAddedAttackSpeed = 0;
            for (int i = 0; i < count; i++)
            {
                var item = skillBuffs.ElementAt(i);
                // var itemKey = item.Key;
                var itemValue = item.Value;
                totalMoreAttackSpeedPercent += itemValue.moreAttackSpeedPercent;
                totalAddedAttackSpeed += itemValue.moreAttackSpeed;
            }

            if (passiveSkills != null) // cộng thêm từ passive
            {
                count = passiveSkills.Length;
                for (int i = 0; i < count; i++)
                {
                    totalMoreAttackSpeedPercent += passiveSkills[i].moreAttackSpeedPercent;
                }
            }
            return attackTime / (1f + totalMoreAttackSpeedPercent / 100f + totalAddedAttackSpeed);
        }
    }

    private float armor;
    public float Armor
    {
        get
        {
            return armor;
        }
    }

    private int element;
    public int Element
    {
        get
        {
            return element;
        }
    }

    private float critRate;
    public float CritRate
    {
        get
        {
            var count = skillBuffs.Count;
            float totalMoreCritRatePercent = 0;
            for (int i = 0; i < count; i++)
            {
                var item = skillBuffs.ElementAt(i);
                // var itemKey = item.Key;
                var itemValue = item.Value;
                totalMoreCritRatePercent += itemValue.moreCritRatePercent;
            }
            return critRate + totalMoreCritRatePercent;
        }
    }

    private float critDamage;
    public float CritDamage
    {
        get
        {
            var count = skillBuffs.Count;
            float totalMoreCritDamagePercent = 0;
            for (int i = 0; i < count; i++)
            {
                var item = skillBuffs.ElementAt(i);
                // var itemKey = item.Key;
                var itemValue = item.Value;
                totalMoreCritDamagePercent += itemValue.moreCritDamagePercent;
            }
            return critDamage + totalMoreCritDamagePercent;
        }
    }

    private float accuracy;
    public float Accuracy
    {
        get
        {
            return accuracy;
        }
    }

    private float dodge;
    public float Dodge
    {
        get
        {
            var totalMoreDodgePercent = 0f;
            if (passiveSkills != null) // cộng thêm từ passive
            {
                var count = passiveSkills.Length;
                for (int i = 0; i < count; i++)
                {
                    totalMoreDodgePercent += passiveSkills[i].moreDodgePercent;
                }
            }
            return dodge * (1 + totalMoreDodgePercent / 100f);
        }
    }

    private float knockback;
    public float Knockback
    {
        get
        {
            return knockback;
        }
    }

    protected float range;
    public float Range
    {
        get
        {
            return range;
        }
    }

    public int TargetToEnemy => heroData.TargetToEnemy;
    public float[] SkillValues => heroData.SkillValues;

    #endregion

    [HideInInspector]
    public HeroData heroData;
    [HideInInspector]
    public HubHeroSkill heroHubSkill;
    [HideInInspector]
    public EnemyControl target;
    private float random = 1f;

    [SerializeField] protected string[] nameOfIdleAnimations;
    [SerializeField] protected string[] nameOfDeadAnimations;

    public override int STAGE
    {
        get
        {
            return stage;
        }
        set
        {
            TrackEntry trackEntry;
            if (stage != ANIM_DEAD_STAGE)
            {
                switch (value)
                {
                    case ANIM_DEAD_STAGE:
                        if (stage != value)
                        {
                            //  skeletonAnimation.AnimationState.TimeScale = 1.0f;
                            trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, nameOfDeadAnimations[UnityEngine.Random.Range(0, nameOfDeadAnimations.Length)], false);
                            trackEntry.Complete += AnimDead_Complete;
                            stage = value;
                        }
                        break;
                    case ANIM_IDLE_STAGE:
                        if (stage != value && !attack.Attacking() && !skill.Skilling())
                        {
                            //  skeletonAnimation.AnimationState.TimeScale = 1.0f;
                            trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true);
                            if (stage != ANIM_ATTACK_STAGE) skeletonAnimation.Update(Mathf.Abs(random) / 2f);
                            trackEntry.TimeScale = 1f + random / 4f;
                            stage = value;
                        }
                        break;
                    case ANIM_ATTACK_STAGE:
                        if (!attack.Attacking() && !skill.Skilling())
                        {
                            //skeletonAnimation.AnimationState.TimeScale = 1.0f;
                            attack.OnAttack();
                            skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
                            stage = value;
                        }
                        break;
                    case ANIM_SKILL_STAGE:
                        if (!skill.Skilling())
                        {
                            skill.OnSkill();
                            //skeletonAnimation.AnimationState.TimeScale = 0.25f;
                            skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
                            stage = value;
                        }
                        break;
                 
                }
            }
        }
    }

    public override void Init(HeroData _heroData)
    {
        base.Init(_heroData);
        int countPassive = passiveSkills.Length;
        float totalMoreHPPercent = 0;
        if (passiveSkills != null) // cộng thêm từ passive
        {
            for (int i = 0; i < countPassive; i++)
            {
                totalMoreHPPercent += passiveSkills[i].moreHpPercent;
            }
        }

        HP_MAX *= (1 + totalMoreHPPercent / 100f);
        HP = HP_MAX;
        heroData = _heroData;
        hpRegen = heroData.HPRegen;
        damage = heroData.DamageTotal;
        //  attackTime = 1f / (heroData.AttackSpeedTotal * (1f + random / 10f));
        attackTime = 1f / heroData.AttackSpeedTotal;
        armor = heroData.ArmorTotal;
        element = heroData.Element;
        critRate = heroData.CritRateTotal;
        critDamage = heroData.CritDamageTotal;
        accuracy = heroData.AccuracyTotal;
        dodge = heroData.DodgeTotal;
        knockback = heroData.Knockback;
        range = heroData.AttackRangeTotal * Config.xRange * (1f + random / 8f);
        bulletSpeed = heroData.BulletSpeed;
        //Khi build team nếu có từ 2 character cùng nguyên tố sẽ được bonus 20% damage
        damage += damage * GameplayController.Instance.GetBonusDamageFormationByElement(element);

        cooldown_MAX = heroData.Cooldown / 2f;
        cooldown = cooldown_MAX;
        if (cooldownBar != null)
        {
            cooldownBar.SetActive(true);
            cooldownBar.Init();
            cooldownBar.ShowCooldown(cooldown, cooldown_MAX);
        }

        target = null;
        if (attack != null) attack.Init(this);
        skill.Init(this);
        autoTarget.Init(this);

        //look at
        var count = boneAimNames.Length;
        boneAims = new Bone[count];
        for (int i = 0; i < count; i++)
        {
            boneAims[i] = skeletonAnimation.Skeleton.FindBone(boneAimNames[i]);
        }
        posAim = transform.up * 5f;
        //lúc đầu mọi hero nhìn thẳng lên

        //get hit show red color
        if (showRedColor != null)
        {
            StopCoroutine(showRedColor);
            showRedColor = null;
        }
        //get freeze show blue color
        if (showSkillNerfColor != null)
        {
            StopCoroutine(showSkillNerfColor);
            showSkillNerfColor = null;
        }

        //buff
        particleManPools = new PoolsContainer<ParticleMan>("PoolsParticleMan", 0, transform);
        //for (int i = 0; i < builtInParticleMan.Count; i++)
        //{
        //    var prefab = builtInParticleMan[i].prefab;
        //    var pool = particleManPools.Get(prefab);
        //    pool.AddOutsiders(builtInParticleMan[i].builtIn);
        //}
    }

    private void SkeletonAnimation_UpdateLocal(ISkeletonAnimation animated)
    {
        var rotZ = Util.GetAngleFromTwoPosition(transform.position, posAim, Util.AXIS.X_AND_Y);
        var rotZrad = rotZ * Mathf.Deg2Rad;
        var cosRot = Mathf.Cos(rotZrad);
        var cosMinR = Mathf.Cos(60f * Mathf.Deg2Rad);

        if ((cosMinR > 0 && (cosRot < cosMinR))
            || (cosMinR < 0 && (cosRot > cosMinR)))
        {
            if (Mathf.Sin(rotZrad) > 0f) rotZ = 60f;
            else rotZ = -60f;
        }
        if (boneAims != null)
        {
            var count = boneAims.Length;
            for (int i = 0; i < count; i++)
            {
                boneAims[i].Rotation = rotZ + offsetRotationAims[i];
            }
        }
    }

    public void StartAim()
    {
        skeletonAnimation.UpdateLocal += SkeletonAnimation_UpdateLocal;
    }

    public void EndAim()
    {
        skeletonAnimation.UpdateLocal -= SkeletonAnimation_UpdateLocal;
        if (boneAims != null)
        {
            var count = boneAims.Length;
            for (int i = 0; i < count; i++)
            {
                boneAims[i].Rotation = 0f + offsetRotationAims[i];
            }
        }
    }

    public override void Refresh()
    {
        skeletonAnimation.Initialize(false);
        skeletonAnimation.skeleton.A = 1f;

        base.Refresh();
        GameplayController.Instance.AddHeroEx(this);

        if (skillNerfs != null) skillNerfs.Clear();
        else skillNerfs = new Dictionary<int, SkillNerf>();
        if (skillBuffs != null) skillBuffs.Clear();
        else skillBuffs = new Dictionary<int, SkillBuff>();
        random = UnityEngine.Random.Range(-1f, 1f);

        if (hpBar != null)
        {
            hpBar.SetActive(true);
        }

        skeletonAnimation.Update(0f);
    }

    private void Start()
    {
        StartAim();
    }

    private void OnDestroy()
    {
        EndAim();
    }

    //TEST
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            cooldown = 0;
        }
    }

    public bool isSkill = false;

    private void FixedUpdate()
    {
        if (!GameplayController.Instance.canStartGame) return;

        var fixedDeltaTime = Time.fixedDeltaTime;

        if (!GameplayController.isMisstionIntro || isShowedFullCooldown)
        {
            cooldown -= fixedDeltaTime;
            if (heroHubSkill != null)
                heroHubSkill.SetPercent(cooldown / cooldown_MAX);
            if (cooldownBar != null) cooldownBar.ShowCooldown(cooldown, cooldown_MAX);
        }

        if (target != null && !target.IsDead()&&!isSkill)
        {
            if (!GameplayController.Instance.holdingTap
                || GameplayController.Instance.autoPlay)
            {
                LookAt(target.transform.position + (Vector3)target.Offset);
            }
        }

        if (GameplayController.Instance.CanAutoSkill)
        {
            if (GameplayController.Instance.autoPlay)
            {
                if (cooldown <= 0f)
                {
                    bool canActiveSkill = false;
                    List<EnemyControl> es = GameplayController.Instance.GetEnemies();
                    foreach (EnemyControl e in es)
                    {
                        if (e != null && e.IsActive() && !e.IsDead() && e.transform.position.y <= Config.LOWEST_Y)
                        {
                            canActiveSkill = true;
                            break;
                        }
                    }
                    if (canActiveSkill)
                        BtnSkill_Pressed();
                }
            }
        }

        //regen HP
        if (!IsDead())
        {
            if (timeHpRegen < 0f)
            {
                timeHpRegen = 1f;
                HP += HpRegen;
                if (HP > HP_MAX)
                {
                    HP = HP_MAX;
                }

                if (hpBar != null)
                {
                    var pos = hpBar.ShowHP(HP, HP_MAX, Shield, ShieldMax);
                }
            }
            else
            {
                timeHpRegen -= fixedDeltaTime;
            }
        }

        int count = 0;
        //nerd time to remove
        if (skillNerfs != null)
        {
            count = skillNerfs.Count;
            for (int i = count - 1; i >= 0; i--) //vì có remove buff
            {
                var skillNerf = skillNerfs.ElementAt(i);
                if (skillNerf.Value.timePlay > 0f) //với các buff mà ko giới hạn time như buff shield thì từ đầu đã ko chạy vào đây vì timePlay = -1
                {
                    skillNerf.Value.timePlay -= fixedDeltaTime;
                    var timePlay = skillNerf.Value.timePlay;
                    if (timePlay <= 0f)
                    {
                        RemoveNerf(skillNerf.Value);
                    }
                }
            }
        }
        //buff time to remove
        if (skillBuffs != null)
        {
            count = skillBuffs.Count;
            for (int i = count - 1; i >= 0; i--) //vì có remove buff
            {
                var skillBuff = skillBuffs.ElementAt(i);
                if (skillBuff.Value.timePlay > 0f) //với các buff mà ko giới hạn time như buff shield thì từ đầu đã ko chạy vào đây vì timePlay = -1
                {
                    skillBuff.Value.timePlay -= fixedDeltaTime;
                    var timePlay = skillBuff.Value.timePlay;
                    if (timePlay <= 0f)
                    {
                        RemoveBuff(skillBuff.Value);
                    }
                }
            }
        }
    }

    // public void LinkHubInfoHero(HubInfoHero _hubInfoHero)
    // {
    //     hubInfoHero = _hubInfoHero;
    //     hubInfoHero.Init(heroData.GetIcon());
    // }

    public void AnimAttack()
    {
        STAGE = ANIM_ATTACK_STAGE;
    }

    public void AnimSkill()
    {
        STAGE = ANIM_SKILL_STAGE;
    }

    public override void OnDead()
    {
        base.OnDead();
        heroHubSkill.HeroDeath();
        GameplayController.Instance.RemoveHeroEx(this);
        

        if (hpBar != null)
        {
            hpBar.SetActive(false);
        }
        if (cooldownBar != null)
        {
            cooldownBar.SetActive(false);
        }

        if (attack != null) attack.End();
        skill.End();
        autoTarget.End();

        StopAllCoroutines();

        particleManPools.ReleaseAll();
    }

    protected void AnimDead_Complete(TrackEntry trackEntry)
    {
        gameObject.SetActive(false);
    }

    public override void GetHit(InfoAttacker infoAttacker)
    {
        if (IsDead()) return;

        //nerf
        if (infoAttacker.skillNerf != null) AddNerf(infoAttacker.skillNerf);

        var count = skillBuffs.Count;
        // Nếu có 1 thằng khác nhận damage hộ thì gây damage thằng đó trước.
        for (int i = count - 1; i >= 0; i--)
        {
            var skillBuff = skillBuffs.ElementAt(i);
            if (skillBuff.Value.damagedTarget != null && !skillBuff.Value.damagedTarget.IsDead() && skillBuff.Value.damagedTarget != this)
            {
                skillBuff.Value.damagedTarget.GetHit(infoAttacker);
                return;
            }
        }
        bool isCrit = false;
        var damInBattle = ConfigStats.GetDamInBattle(ref isCrit, infoAttacker.damageAttacker,
                                                            Armor,
                                                            infoAttacker.critRate,
                                                            infoAttacker.critDamage,
                                                            infoAttacker.enemyExControl.Element, element,
                                                            infoAttacker.accuracyAttacker, Dodge);
        damInBattle = damInBattle * (1f - DamageReduction / 100f);
        //nếu có shield thì trừ vào shield trước nếu trừ quá thì trừ thêm vào HP
        float realDam = damInBattle;
        if (GameplayController.isMisstionIntro) realDam /= 1000f; //nếu trong tutorial intro thì dam chỉ bằng 1/10 và hero ko chết
        var lastShield = -realDam;
        for (int i = count - 1; i >= 0; i--) //vì có remove buff
        {
            //kiểu shield đầu trừ dam mà vẫn âm thì để shield sau gánh tiếp
            if (lastShield < 0f)
            {
                var skillBuff = skillBuffs.ElementAt(i);
                if (skillBuff.Value.shield > 0f)
                {
                    skillBuff.Value.shield += lastShield;
                    lastShield = skillBuff.Value.shield;
                    if (lastShield < 0f)
                    {
                        RemoveBuff(skillBuff.Value);
                    }
                }

                if (skillBuff.Value.shieldBarrier > 0f && skillBuff.Value.damagedTarget == this)// đoạn này tính cho shield đặc biệt, đỡ ddamaage cho toàn bộ hero
                {
                    skillBuff.Value.shieldBarrier += lastShield;
                    lastShield = skillBuff.Value.shieldBarrier;
                    if (lastShield < 0f)
                    {
                        RemoveBuff(skillBuff.Value);
                        // remove thêm toàn bộ hero có target này

                        var heroes = GameplayController.Instance.GetHeroExs();
                        var countHero = heroes.Count;
                        for (int j = 0; j < countHero; j++)
                        {
                            var hero = heroes[j];
                            if (!hero.IsDead()) hero.RemoveBarrierDamageTarget(this);
                        }
                    }
                }

            }
            else //đến lúc dương thì thôi
            {
                break;
            }
        }

        if (lastShield < 0f)
        {
            HP += lastShield;
        }

        if (HP <= 0f)
        {
            if (Immortal || GameplayController.isMisstionIntro)
            {
                HP = 1f;
            }
            else
            {
                HP = 0f;
                AnimDead();
            }
        }
        else
        {
            int r = UnityEngine.Random.Range(0, 5);
            if (isCrit || r == 0)
            {
                if (!soundBeHit.Equals("")) SoundManager.Instance.PlaySFX(soundBeHit);
            }
        }
        ShowRedColor();

        if (hpBar != null)
        {
            var pos = hpBar.ShowHP(HP, HP_MAX, Shield, ShieldMax);
            GameplayController.Instance.SpawnHp(pos + new Vector3(0f, 0f, -1f), damInBattle, isCrit, TextHp.TEXT_DAMAGE_HP);
        }
    }

    public void SetTarget(EnemyControl _target)
    {
        target = _target;
    }

    public override void LookAt(Vector3 _posAim)
    {
        posAim = _posAim;
    }

    public override void BtnSkill_Pressed()
    {
        base.BtnSkill_Pressed();

        if (cooldown <= 0f && !IsDead())
        {
            skill.Skill();
            cooldown_MAX = heroData.Cooldown;
            cooldown = cooldown_MAX;
            if (cooldownBar != null) cooldownBar.ShowCooldown(cooldown, cooldown_MAX);
        }
    }

    public bool isCanSkill => cooldown <= 0;
    public void SetCountDown()
    {
        if (cooldown <= 0f && !IsDead())
        {
            cooldown_MAX = heroData.Cooldown;
            cooldown = cooldown_MAX;
            if (cooldownBar != null) cooldownBar.ShowCooldown(cooldown, cooldown_MAX);
        }
    }


    private void ShowRedColor()
    {
        if (showRedColor != null)
        {
            StopCoroutine(showRedColor);
        }
        if (showSkillNerfColor != null)
        {
            StopCoroutine(showSkillNerfColor);
        }

        showRedColor = IEShowRedColor();
        StartCoroutine(showRedColor);
    }

    protected virtual IEnumerator IEShowRedColor()
    {
        float timeMax = 0.5f;
        float timePlay = timeMax;
        float a;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            a = skeletonAnimation.skeleton.A;
            var c = Color.Lerp(Color.white, Color.red, (timePlay / timeMax));
            skeletonAnimation.skeleton.SetColor(c);
            skeletonAnimation.skeleton.A = a;
        }
    }

    //nerd
    private void ShowSkillNerfColor(Color color, float timeMax)
    {
        if (showRedColor != null)
        {
            StopCoroutine(showRedColor);
        }
        if (showSkillNerfColor != null)
        {
            StopCoroutine(showSkillNerfColor);
        }

        showSkillNerfColor = IEShowSkillNerfColor(color, timeMax);
        StartCoroutine(showSkillNerfColor);
    }

    protected virtual IEnumerator IEShowSkillNerfColor(Color color, float timeMax)
    {
        float timePlay = timeMax;
        float a;
        while (timePlay >= 0f)
        {
            yield return null;
            timePlay -= Time.deltaTime;
            a = skeletonAnimation.skeleton.A;
            var c = Color.Lerp(Color.white, color, (timePlay / timeMax));
            skeletonAnimation.skeleton.SetColor(c);
            skeletonAnimation.skeleton.A = a;
        }
    }

    //nerf
    public void AddNerf(SkillNerf _skillNerf)
    {
        if (IsDead()) return;

        //đoạn này mình clone ra một object SkillNerf mới để gán các effectBuff vào, sau tiện cho việc release effectBuff trong pool
        var skillNerf = new SkillNerf(_skillNerf);

        var stun = skillNerf.stun;
        if (stun)
        {
            ShowSkillNerfColor(skillNerf.color, skillNerf.timePlay);
            skeletonAnimation.timeScale = 0f;
        }

        //kiểm tra đã có buff từ hero này chưa
        bool hasBuff = false;
        if (skillNerfs.ContainsKey(skillNerf.baseId))
        {
            hasBuff = true;

            //nếu đã có buff thì add lại effectBuff đã spawn gắn với skillBuff
            skillNerf.effectNerf = skillNerfs[skillNerf.baseId].effectNerf;
            //rồi reset chỉ số buff
            skillNerfs[skillNerf.baseId] = skillNerf;
        }

        if (!hasBuff)
        {
            skillNerfs.Add(skillNerf.baseId, skillNerf);

            //phần này xử lý effect chung cho tất cả các buff
            var effectNeftPrefab = skillNerf.effectNerf;
            if (effectNeftPrefab != null)
            {
                skillNerf.effectNerf = SpawnParticleMan(effectNeftPrefab, transform.position + new Vector3(Offset.x, Offset.y, -1f), Quaternion.identity);
                //gán lại effectBuff để sau mình remove buff thì relese luôn effectBuff trong pool theo cái buff đó luôn
                
                var effectBuff = skillNerf.effectNerf;
                effectBuff.SetActive(true);
                effectBuff.Play();
            }
        }
    }

    private void RemoveNerf(SkillNerf skillNerf)
    {
        if (skillNerfs.ContainsKey(skillNerf.baseId))
        {
            skillNerfs.Remove(skillNerf.baseId);

            if (skillNerf.effectNerf != null)
            {
                var effectNerf = skillNerf.effectNerf;
                effectNerf.Stop();
                ReleaseParticleMan(effectNerf);
            }
        }

        if (!Stun)
        {
            skeletonAnimation.timeScale = 1f;
        }
    }

    //buff
    public void AddBuff(SkillBuff _skillBuff)
    {
        //đoạn này mình clone ra một object skillBuff mới để gán các effectBuff vào, sau tiện cho việc release effectBuff trong pool
        var skillBuff = new SkillBuff(_skillBuff);

        //---HEAL HP
        //trường hợp hồi máu, mỗi lần buff là heal luôn
        if (skillBuff.healHP > 0f)
        {
            HP += skillBuff.healHP;
            if (HP > HP_MAX) HP = HP_MAX;
            hpBar.ShowHP(HP, HP_MAX, Shield, ShieldMax);
        }

        //kiểm tra đã có buff từ hero này chưa
        bool hasBuff = false;
        if (skillBuffs.ContainsKey(skillBuff.baseId))
        {
            hasBuff = true;

            //nếu đã có buff thì add lại effectBuff đã spawn gắn với skillBuff
            skillBuff.effectBuff = skillBuffs[skillBuff.baseId].effectBuff;
            //rồi reset chỉ số buff
            skillBuffs[skillBuff.baseId] = skillBuff;
        }

        if (!hasBuff)
        {
            skillBuffs.Add(skillBuff.baseId, skillBuff);

            //phần này xử lý effect chung cho tất cả các buff
            var effectBuffPrefab = skillBuff.effectBuff;
            if (effectBuffPrefab != null)
            {
                skillBuff.effectBuff = SpawnParticleMan(effectBuffPrefab, transform.position + new Vector3(0f, 0f, -1f), Quaternion.identity);
                //gán lại effectBuff để sau mình remove buff thì relese luôn effectBuff trong pool theo cái buff đó luôn
                var effectBuff = skillBuff.effectBuff;
                effectBuff.SetActive(true);
                effectBuff.Play();
            }
        }
    }

    public void RemoveBuff(SkillBuff skillBuff)
    {
        if (skillBuffs.ContainsKey(skillBuff.baseId))
        {
            skillBuffs.Remove(skillBuff.baseId);

            if (skillBuff.effectBuff != null)
            {
                var effectBuff = skillBuff.effectBuff;
                effectBuff.Stop();
                ReleaseParticleMan(effectBuff);
            }
        }
    }

    private ParticleMan SpawnParticleMan(ParticleMan particleMan, Vector3 pos, Quaternion rot)
    {
        var obj = SimplePool.Spawn(particleMan,pos,rot);
        //var pool = particleManPools.Get(particleMan);
        //var obj = pool.Spawn(pos, true);
        obj.transform.SetParent(particleManPools.container);
        obj.transform.rotation = rot;

        return obj;
    }

    private void ReleaseParticleMan(ParticleMan particleMan)
    {
        //thêm 2s cho hiện hết stop effect
        StartCoroutine(IEReleaseParticleMan(particleMan, 2f));
    }

    private IEnumerator IEReleaseParticleMan(ParticleMan particleMan, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SimplePool.Despawn(particleMan.gameObject);
        particleManPools.Release(particleMan);
    }

    //steal HP
    public void GetHPBack(float damage)
    {
        if (IsDead()) return;

        var count = skillBuffs.Count;
        float totalHpStealByPercentDamage = 0;
        for (int i = 0; i < count; i++)
        {
            var item = skillBuffs.ElementAt(i);
            var itemValue = item.Value;
            totalHpStealByPercentDamage += itemValue.hpStealByPercentDamage;
        }

        HP += damage * totalHpStealByPercentDamage / 100f;
        if (HP > HP_MAX)
        {
            HP = HP_MAX;
        }

        if (hpBar != null)
        {
            var pos = hpBar.ShowHP(HP, HP_MAX, Shield, ShieldMax);
        }
    }
    public void GetHPBack(float damage, float percent)
    {
        if (IsDead()) return;
        HP += damage * percent / 100f;
        if (HP > HP_MAX)
        {
            HP = HP_MAX;
        }

        if (hpBar != null)
        {
            var pos = hpBar.ShowHP(HP, HP_MAX, Shield, ShieldMax);
        }
    }

    public void RemoveBarrierDamageTarget(HeroExControl heroExControl)
    {
        var count = skillBuffs.Count;
        for (int i = 0; i < count; i++)
        {
            var item = skillBuffs.ElementAt(i);
            var itemValue = item.Value;
            if (itemValue.damagedTarget == heroExControl)
            {
                RemoveBuff(itemValue);
                return;
            }
        }
    }

    public void FullCooldown()
    {
        cooldown_MAX = heroData.Cooldown;
        cooldown = 0;
        if (cooldownBar != null) cooldownBar.ShowCooldown(cooldown, cooldown_MAX);
        isShowedFullCooldown = true;
    }
}