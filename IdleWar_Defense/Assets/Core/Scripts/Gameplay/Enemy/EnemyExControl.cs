using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using Spine.Unity;
using Spine;
using System.Collections.Generic;
using System;
using System.Linq;
using FoodZombie;
using HedgehogTeam.EasyTouch.UI;
using Utilities.Common;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class EnemyExControl : EnemyControl
{
	protected float cooldown;
	protected float cooldown_MAX;

	protected const int ANIM_RUN_STAGE = 2;
	protected const int ANIM_ATTACK_STAGE = 3;
	protected const int ANIM_GET_HIT_STAGE = 4;
	protected const int ANIM_SKILL_STAGE = 5;
	public SkeletonAnimation skeletonAnimation;
	public EnemyMove autoMove;
	public EnemyAttack attack;
	public List<EnemySkill> skills;
	public EnemyAutoTarget autoTarget;

	public string soundMove;
	public ParticleMan effectDie;
	public float delayTimeEffectDie = 0.6f;

	private float timeHpRegen = 1f; //sử dụng cho cả chu kỳ trừ HP per second 

	//nerf
	private Dictionary<int, SkillNerf> skillNerfs = new Dictionary<int, SkillNerf>();
	private Dictionary<int, SkillBuff> skillBuffs = new Dictionary<int, SkillBuff>();
	private PoolsContainer<ParticleMan> particleManPools;
	[SerializeField] private List<BuiltInPoolingParticleMan> builtInParticleMan;

	//skills
	public int lastIndexSkill = -1;

	#region Info

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
	private float hpRegen;
	protected float damage;
	public float Damage
	{
		get
		{
			return damage;
		}
	}
	protected float[] bulletSpeed;
	public float[] BulletSpeed
	{
		get
		{

			return bulletSpeed;
		}
	}
	protected float attackTime;
	public float AttackTime
	{
		get
		{

			return attackTime;
		}
	}

	public float DamagePerSec
	{
		get
		{
			var count = skillNerfs.Count;
			float damagePerSec = 0f;
			float temp;
			for (int i = 0; i < count; i++)
			{
				var item = skillNerfs.ElementAt(i);
				// var itemKey = item.Key;
				var itemValue = item.Value;
				temp = 0f;
				temp += itemValue.damagePerSec;
				temp += itemValue.damagePercentHpTargetPerSec * HP_MAX / 100f;
				damagePerSec += temp;

				if (itemValue.heroExControl != null)
				{
					DamageTracker.instance.RecordDamageInBattle(itemValue.heroExControl.heroData.id, temp);
				}
			}

			return damagePerSec;
		}
	}

	protected float armor;
	public float Armor
	{
		get
		{
			var count = skillNerfs.Count;
			float armorReducePercent = 0f;
			for (int i = 0; i < count; i++)
			{
				var item = skillNerfs.ElementAt(i);
				// var itemKey = item.Key;
				var itemValue = item.Value;
				armorReducePercent += itemValue.armorReducePercent;
			}
			var valueArmor = armor * (1f - armorReducePercent / 100f);
			if (valueArmor < 0f) return 0f;

			return valueArmor;
		}
	}

	protected float movement;
	public float moveSpeedMultiple;
	public float Movement
	{
		get
		{
			float alphaTutorial = 1f;
			if (GameplayController.isMisstionIntro) alphaTutorial = 1.55f;

			if (IsSleep()) return 0;
			var count = skillNerfs.Count;
			float totalMovementPercent = 100f;
			for (int i = 0; i < count; i++)
			{
				var item = skillNerfs.ElementAt(i);
				// var itemKey = item.Key;
				var itemValue = item.Value;
				totalMovementPercent -= itemValue.downMovementPercent;
			}
			if (totalMovementPercent <= 0f) totalMovementPercent = 0f;
			if (moveSpeedMultiple > 0) return alphaTutorial * movement * totalMovementPercent / 100f * moveSpeedMultiple;
			return alphaTutorial * movement * totalMovementPercent / 100f;
		}
	}

	protected int element;
	public int Element
	{
		get
		{
			return element;
		}
	}

	protected float critRate;
	public float CritRate
	{
		get
		{
			return critRate;
		}
	}

	protected float critDamage;
	public float CritDamage
	{
		get
		{
			return critDamage;
		}
	}

	protected float accuracy;
	public float Accuracy
	{
		get
		{
			return accuracy;
		}
	}

	protected float dodge;
	public float Dodge
	{
		get
		{
			return dodge;
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

	protected int coinDrop;
	public int CoinDrop
	{
		get
		{
			return coinDrop;
		}
	}

	public bool IsSleep()
	{
		var count = skillNerfs.Count;
		for (int i = 0; i < count; i++)
		{
			var item = skillNerfs.ElementAt(i);
			// var itemKey = item.Key;
			var itemValue = item.Value;
			if (itemValue.isSleep) return true;
		}

		return false;
	}

	public int TargetToHero => enemyData.TargetToHero;

	#endregion

	public EnemyData enemyData;

	[HideInInspector]
	public HeroControl target;
	public float random = 1f;

	[SerializeField] protected string[] nameOfIdleAnimations;
	[SerializeField] protected string[] nameOfRunAnimations;
	[SerializeField] protected string[] nameOfDeadAnimations;
	[SerializeField] protected string[] nameOfGetHitAnimations;
	private IEnumerator showRedColor = null;
	private IEnumerator showSkillNerfColor = null;

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
							trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, nameOfDeadAnimations[UnityEngine.Random.Range(0, nameOfDeadAnimations.Length)], false);
							trackEntry.Complete += AnimDead_Complete;
							stage = value;
						}
						break;
					case ANIM_IDLE_STAGE:
						if (stage != value && !attack.Attacking() && !IsSkilling())
						{
							if (stage == ANIM_GET_HIT_STAGE)
							{
								skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
							}
							else
							{
								trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true);
								if (stage != ANIM_ATTACK_STAGE) skeletonAnimation.Update(Mathf.Abs(random) / 1.8f);
								trackEntry.TimeScale = 1f + random / 4f;
							}
							stage = value;
						}
						break;
					case ANIM_RUN_STAGE:
						if (stage != value && !attack.Attacking() && !IsSkilling())
						{
							OnRun();
							if (stage == ANIM_GET_HIT_STAGE)
							{
								skeletonAnimation.AnimationState.AddAnimation(0, nameOfRunAnimations[UnityEngine.Random.Range(0, nameOfRunAnimations.Length)], true, 0f);
							}
							else
							{
								trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, nameOfRunAnimations[UnityEngine.Random.Range(0, nameOfRunAnimations.Length)], true);
								if (stage != ANIM_ATTACK_STAGE) skeletonAnimation.Update(Mathf.Abs(random) / 1.8f);
								trackEntry.TimeScale = 1f + random / 4f;

								// trackEntry.Complete += AnimRun_Complete; //hot fix cho GD
							}
							stage = value;
						}
						break;
					case ANIM_ATTACK_STAGE:
						if (!attack.Attacking() && !IsSkilling())
						{
							attack.OnAttack();
							skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
							stage = value;
						}
						break;
					case ANIM_GET_HIT_STAGE:
						if (stage != value /*&& !attack.Attacking()*/)
						{
							skeletonAnimation.AnimationState.SetAnimation(0, nameOfGetHitAnimations[UnityEngine.Random.Range(0, nameOfGetHitAnimations.Length)], false);
							stage = value;
						}
						break;
					case ANIM_SKILL_STAGE:
						if (!IsSkilling())
						{
							skills[lastIndexSkill].OnSkill();
							skeletonAnimation.AnimationState.AddAnimation(0, nameOfIdleAnimations[UnityEngine.Random.Range(0, nameOfIdleAnimations.Length)], true, 0f);
							stage = value;
						}
						break;
				}
			}
		}
	}

	public void AnimSkill()
	{
		STAGE = ANIM_SKILL_STAGE;
	}
	public override void Init(EnemyData _enemyData, int _level, float hpx = 1, float damx = 1)
	{
		base.Init(_enemyData, _level, hpx, damx);
		level = _level;

		enemyData = _enemyData;
		hpRegen = enemyData.GetHPRegen(level);
		damage = enemyData.GetDamage(level);
		damage = damage * damx;
		var attackSpeed = (enemyData.GetAttackSpeed(level) * (1f + random / 10f));
		if (attackSpeed <= 0f) attackSpeed = 1f;
		attackTime = 1f / attackSpeed;
		bulletSpeed = enemyData.GetBulletSpeed(level);
		movement = enemyData.GetMovement(level);
		element = enemyData.Element;
		critRate = enemyData.GetCritRate(level);
		critDamage = enemyData.GetCritDamage(level);
		critDamage = critDamage * damx;
		accuracy = enemyData.GetAccuracy(level);
		dodge = enemyData.GetDodge(level);
		range = enemyData.GetAttackRange(level) * Config.xRange * (1f + random / 8f);
		coinDrop = enemyData.GetCoinDrop(level);

		cooldown_MAX = enemyData.Cooldown;
		cooldown = cooldown_MAX;
		target = null;
		autoMove.Init(this);
		attack.Init(this);
		SkillsInit();
		autoTarget.Init(this);

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

		//nerf
		particleManPools = new PoolsContainer<ParticleMan>("PoolsParticleMan", 1, transform);
		for (int i = 0; i < builtInParticleMan.Count; i++)
		{
			var prefab = builtInParticleMan[i].prefab;
			var pool = particleManPools.Get(prefab);
			pool.AddOutsiders(builtInParticleMan[i].builtIn);
		}

		//get POWER:
		GetPOW();
	}
	void GetPOW()
	{
		var HP = enemyData.GetHP(level);
		var damage = enemyData.GetDamage(level);
		var armor = enemyData.GetArmor(level);
		var attackSpeed = enemyData.GetAttackSpeed(level);
		var critRate = enemyData.GetCritRate(level);
		var accuracy = enemyData.GetAccuracy(level);
		var dodge = enemyData.GetDodge(level);
		var critDamage = enemyData.GetCritDamage(level);

		this.POWER = ConfigStats.GetPower(HP, damage, armor, attackSpeed, critRate, accuracy, dodge, critDamage);
		//  totalPower += power;
	}
	public override void Refresh()
	{
		skeletonAnimation.Initialize(false);
		skeletonAnimation.skeleton.A = 1f;
		skeletonAnimation.skeleton.SetColor(Color.white);

		base.Refresh();

		if (skillNerfs != null) skillNerfs.Clear();
		else skillNerfs = new Dictionary<int, SkillNerf>();
		if (skillBuffs != null) skillBuffs.Clear();
		else skillBuffs = new Dictionary<int, SkillBuff>();
		random = UnityEngine.Random.Range(-1f, 1f);

		skeletonAnimation.timeScale = 1f;
		skeletonAnimation.Update(0f);
	}

	private void OnEnable()
	{
		//invisible
		if (enemyData != null && enemyData.Invisible)
		{
			StartCoroutine(IESetInvisible());
		}
	}

	private void Start()
	{
		//invisible
		if (enemyData != null && enemyData.Invisible)
		{
			StartCoroutine(IESetInvisible());
		}
	}

	private void FixedUpdate()
	{
		var fixedDeltaTime = Time.fixedDeltaTime;
		if (skills.Count > 0)
		{
			cooldown -= fixedDeltaTime;

			if (target != null && !target.IsDead())
			{
				if (cooldown <= 0f) CheckSkills();
			}
		}

		//regen HP
		if (!IsDead())
		{
			if (timeHpRegen < 0f)
			{
				timeHpRegen = 1f;
				HP += hpRegen;
				if (HP > HP_MAX)
				{
					HP = HP_MAX;
				}

				if (hpBar != null)
				{
					hpBar.ShowHP(HP, HP_MAX);
				}

				var damagePerSec = DamagePerSec;
				if (damagePerSec > 0f)
				{
					GetHitBySec(damagePerSec);
				}
			}
			else
			{
				timeHpRegen -= fixedDeltaTime;
			}
		}

		//nerd time to remove
		var count = skillNerfs.Count;
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

		var pos = transform.position;
		if (pos.y > Config.LOWEST_Y || pos.x <= -Config.MAX_X || pos.x >= Config.MAX_X)
		{
			if (Stun)
			{
				RemoveAllNerf();
			}
		}
	}

	public void StopMove(float _length)
	{
		autoMove.StopMove(_length);
	}

	public void AnimAttack()
	{
		STAGE = ANIM_ATTACK_STAGE;
	}

	public virtual void AnimRun()
	{
		STAGE = ANIM_RUN_STAGE;
	}

	public void AnimGetHit()
	{
		STAGE = ANIM_GET_HIT_STAGE;
	}

	protected virtual void OnRun()
	{
		if (!soundMove.Equals("")) SoundManager.Instance.PlaySFX(soundMove);
	}

	protected virtual void AnimRun_Complete(TrackEntry trackEntry)
	{
		if (!soundMove.Equals("")) SoundManager.Instance.PlaySFX(soundMove);
	}

	public override void OnDead()
	{
		base.OnDead();

		SetInvisible(false);
		autoMove.End();
		attack.End();
		SkillsEnd();
		autoTarget.End();

		skeletonAnimation.timeScale = 1f;
		particleManPools.ReleaseAll();

		if (effectDie != null)
		{
			SimplePool.Spawn(effectDie, transform.position, Quaternion.identity).Play(delayTimeEffectDie, true);
		}
		GameplayController.Instance.SpawnCoinDrop(transform.position + (Vector3)Offset, delayTimeEffectDie);
	}

	protected void AnimDead_Complete(TrackEntry trackEntry)
	{
		StartCoroutine(IEDisapear());
	}

	protected virtual IEnumerator IEDisapear()
	{
		float timePlay = 1.0f;
		while (timePlay >= 0f)
		{
			yield return null;
			timePlay -= Time.deltaTime;
			skeletonAnimation.skeleton.A = timePlay;
		}

		gameObject.SetActive(false);
	}

	private void GetHitBySec(float damagePerSec)
	{
		HP -= damagePerSec;
		if (HP <= 0)
		{
			HP = 0;
			AnimDead();
		}
		ShowRedColor();

		var pos = hpBar.ShowHP(HP, HP_MAX);
		GameplayController.Instance.SpawnHp(pos + new Vector3(0f, 0f, -1f), damagePerSec, false, TextHp.TEXT_DAMAGE_HP);
	}

	public override void GetHit(InfoAttacker infoAttacker)
	{
		if (IsDead()) return;

		var pos = transform.position;
		if (pos.y > Config.LOWEST_Y || pos.x <= -Config.MAX_X || pos.x >= Config.MAX_X)
		{
			return;
		}

		float damInBattle = infoAttacker.damageAttacker;

		//invisible
		if (infoAttacker.type == InfoAttacker.TYPE_SKILL) SetInvisible(false);
		//nerf
		if (infoAttacker.skillNerf != null) AddNerf(infoAttacker.skillNerf);

		bool isCrit = false;
		if (infoAttacker.heroExControl != null)
		{
			//hit
			damInBattle = ConfigStats.GetDamInBattle(ref isCrit, infoAttacker.damageAttacker,
																Armor,
																infoAttacker.critRate,
																infoAttacker.critDamage,
																infoAttacker.heroExControl.Element, element,
																infoAttacker.accuracyAttacker, Dodge);
			damInBattle = damInBattle * (1f - DamageReduction / 100f);

			var rangeY = infoAttacker.rangeY;
			if (rangeY > 0f)
			{
				var distance = transform.position.y - infoAttacker.startY;
				if (distance >= rangeY) damInBattle = ConfigStats.GetPercentOutRangeDamage() * damInBattle;
			}

			if (infoAttacker.skillNerf != null && infoAttacker.skillNerf.dieImmediatePercent > 0 && !isBoss)
			{
				var isDie = Random.Range(0f, 100f) < infoAttacker.skillNerf.dieImmediatePercent;
				if (isDie)
				{
					DieImmediately(infoAttacker.heroExControl, infoAttacker.skillNerf.damageAsHealPercent);
				}
				else
				{
					infoAttacker.heroExControl.GetHPBack(damInBattle, infoAttacker.skillNerf.damageAsHealPercent);
				}
			}
			//hồi máu cho hero attack
			infoAttacker.heroExControl.GetHPBack(damInBattle);
		}

		float realDam = damInBattle;
		if (GameplayController.isMisstionIntro
			&& infoAttacker.type == InfoAttacker.TYPE_SKILL
			&& !isBoss) realDam = HP; //nếu trong tutorial intro thì dam skill chết luôn enemy, ngoại trừ boss
		HP -= realDam;

		if (HP <= 0)
		{
			HP = 0;
			AnimDead();
		}
		else
		{
			// autoMove.StopMove(0.3f);
			if (infoAttacker.knockback >= 100f)
			{
				// AnimGetHit();
				SoundManager.Instance.PlaySFX(soundBeHit);
			}
			else
			{
				int r = Random.Range(0, 5);
				if (isCrit || r == 0)
				{
					if (!soundBeHit.Equals("")) SoundManager.Instance.PlaySFX(soundBeHit);
				}
			}

			var count = skillNerfs.Count;
			bool receiveDamageSkill = false;
			for (int i = count - 1; i >= 0; i--)
			{
				var item = skillNerfs.ElementAt(i);
				// var itemKey = item.Key;
				var itemValue = item.Value;
				if (itemValue.isSleep)
				{
					if (itemValue.isFirstDamage)
					{
						receiveDamageSkill = true;
					}
					else
					{
						itemValue.isFirstDamage = true;
					}

				}
			}
			for (int i = count - 1; i >= 0; i--)
			{
				var item = skillNerfs.ElementAt(i);
				// var itemKey = item.Key;
				var itemValue = item.Value;
				if (itemValue.isSleep && receiveDamageSkill)
				{
					RemoveNerf(itemValue);
				}
			}
		}
		ShowRedColor();

		var posHP = hpBar.ShowHP(HP, HP_MAX);
		if (infoAttacker.heroExControl != null)
		{
			DamageTracker.instance.RecordDamageInBattle(infoAttacker.heroExControl.heroData.id, damInBattle);
		}

		GameplayController.Instance.SpawnHp(posHP + new Vector3(0f, 0f, -1f), damInBattle, isCrit, TextHp.TEXT_DAMAGE_HP);
	}

	private void DieImmediately(HeroExControl heroExControl, float percent)
	{
		float damInBattle = HP;

		HP = 0;
		heroExControl.GetHPBack(damInBattle, percent);
		DamageTracker.instance.RecordDamageInBattle(heroExControl.heroData.id, damInBattle);
		AnimDead();
		ShowRedColor();
		var pos = hpBar.ShowHP(HP, HP_MAX);
		GameplayController.Instance.SpawnHp(pos + new Vector3(0f, 0f, -1f), damInBattle, false, TextHp.TEXT_DAMAGE_HP);
	}

	public void SetTarget(HeroControl _target)
	{
		target = _target;
	}

	//Invisible
	private IEnumerator IESetInvisible()
	{
		yield return new WaitForSeconds(1f);
		SetInvisible(true);
	}

	public override void StartInvisible()
	{
		base.StartInvisible();

		StartCoroutine(IEStartInvisible());
	}

	protected virtual IEnumerator IEStartInvisible()
	{
		float timeMax = 1f;
		float timePlay = timeMax;
		while (timePlay >= 0f)
		{
			yield return null;
			timePlay -= Time.deltaTime;
			var a = 0.4f + (timePlay / timeMax) * 0.6f;
			if (a < 0.4f) a = 0.4f;
			skeletonAnimation.skeleton.A = a; // chỉ fade một nửa
		}
	}

	public override void EndInvisible()
	{
		base.EndInvisible();

		StartCoroutine(IEEndInvisible());
	}

	protected virtual IEnumerator IEEndInvisible()
	{
		float timeMax = 0.45f;
		float timePlay = timeMax;
		while (timePlay >= 0f)
		{
			yield return null;
			timePlay -= Time.deltaTime;
			var a = 0.4f + (1f - (timePlay / timeMax)) * 0.6f;
			if (a > 1f) a = 1f;
			skeletonAnimation.skeleton.A = a; // chỉ fade một nửa
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

	//buff
	public void AddNerf(SkillNerf _skillNerf)
	{
		if (IsDead()) return;

		var pos = transform.position;
		if (pos.y > Config.LOWEST_Y || pos.x <= -Config.MAX_X || pos.x >= Config.MAX_X)
		{
			return;
		}

		//đoạn này mình clone ra một object SkillNerf mới để gán các effectBuff vào, sau tiện cho việc release effectBuff trong pool
		var skillNerf = new SkillNerf(_skillNerf);

		//hot fix Những enemy dính đạn sẽ có 40% khả năng bị đóng băng vĩnh viễn ( không tác dụng với boss và mini boss )
		var stunPercent = skillNerf.stunPercent;
		if (stunPercent > 0f)
		{
			if (Config.EasyRandom(0f, 100f) <= stunPercent && !isBoss)
			{
				skillNerf.stun = true;
				skillNerf.timePlay = -1f;
			}
		}

		var stun = skillNerf.stun;
		if (stun && !isBoss)
		{
			ShowSkillNerfColor(skillNerf.color, skillNerf.timePlay);
			skeletonAnimation.timeScale = 0f;
			var timePlay = skillNerf.timePlay;
			if (timePlay > 0f) autoMove.StopMove(skillNerf.timePlay);
			else autoMove.StopMove(9999f);
		}

		//kiểm tra đã có buff từ hero này chưa
		bool hasNerf = false;
		if (skillNerfs.ContainsKey(skillNerf.baseId))
		{
			hasNerf = true;

			//nếu đã có buff thì add lại effectBuff đã spawn gắn với skillBuff
			skillNerf.effectNerf = skillNerfs[skillNerf.baseId].effectNerf;
			//rồi reset chỉ số buff
			skillNerfs[skillNerf.baseId] = skillNerf;
		}

		if (!hasNerf)
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

	private void RemoveAllNerf()
	{
		foreach (var skillNerf in skillNerfs)
		{
			if (skillNerf.Value.effectNerf != null)
			{
				var effectNerf = skillNerf.Value.effectNerf;
				effectNerf.Stop();
				ReleaseParticleMan(effectNerf);
			}
		}
		skillNerfs.Clear();
		skeletonAnimation.timeScale = 1f;
		autoMove.Init(this);
	}

	private ParticleMan SpawnParticleMan(ParticleMan particleMan, Vector3 pos, Quaternion rot)
	{
		var pool = particleManPools.Get(particleMan);
		var obj = pool.Spawn(pos, true);
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
		particleManPools.Release(particleMan);
	}

	public float GetHPPercent()
	{
		return HP * 100 / HP_MAX;
	}
	public void AddBuff(SkillBuff _skillBuff)
	{
		//đoạn này mình clone ra một object skillBuff mới để gán các effectBuff vào, sau tiện cho việc release effectBuff trong pool
		var skillBuff = new SkillBuff(_skillBuff);

		//---HEAL HP
		//trường hợp hồi máu, mỗi lần buff là heal luôn
		// if (skillBuff.healHP > 0f)
		// {
		//     HP += skillBuff.healHP;
		//     if (HP > HP_MAX) HP = HP_MAX;
		//     hpBar.ShowHP(HP, HP_MAX, Shield, ShieldMax);
		// }

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

	//list skills cho mấy con boss nhiều skills
	private void SkillsInit()
	{
		lastIndexSkill = -1;
		var count = skills.Count;
		for (int i = 0; i < count; i++)
		{
			var skill = skills[i];
			if (skill != null) skill.Init(this);
		}
	}

	protected bool IsSkilling()
	{
		var count = skills.Count;
		for (int i = 0; i < count; i++)
		{
			if (skills[i].Skilling())
			{
				return true;
			}
		}

		return false;
	}

	public virtual void CheckSkills()
	{
		var count = skills.Count;
		for (int i = 0; i < count; i++)
		{
			//check lần lượt skill, cái nào có thể chạy thì break vòng lặp ko thì next
			lastIndexSkill++;
			if (lastIndexSkill >= count) lastIndexSkill = 0;
			var skill = skills[lastIndexSkill];

			if (skill.CanSkill())
			{
				skill.Skill(); //cast
				cooldown_MAX = enemyData.Cooldown;
				cooldown = cooldown_MAX;
				return;
			}
		}
	}

	private void SkillsEnd()
	{
		var count = skills.Count;
		for (int i = 0; i < count; i++)
		{
			var skill = skills[i];
			skill.End();
		}
	}
}