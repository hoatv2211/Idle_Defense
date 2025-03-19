using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utilities.Common;

/// <summary>
/// bullet.
/// </summary>
public class Bullet : MonoBehaviour
{
	public float radius = 1f;

	public Impact impactPrefab;
	public TrailRenderer trail;
	public int countPierceMax = 1;

	private float m_speed;
	private float m_angle;
	private float m_accelSpeed;
	private float m_accelTurn;
	private bool m_homing;
	private Transform m_homingTarget;
	private float m_homingAngleSpeed;
	private bool m_wave;
	private float m_waveSpeed;
	private float m_waveRangeSize;
	private bool m_pauseAndResume;
	private float m_pauseTime;
	private float m_resumeTime;
	private Util.AXIS m_axisMove;

	private float m_selfFrameCnt;
	private float m_selfTimeCount;

	private TentacleBullet m_tentacleBullet;

	//dongdd
	protected InfoAttacker infoAttacker;
	protected int countPierce = 0;

	protected List<HeroControl> heroes = new List<HeroControl>();
	protected List<EnemyControl> enemies = new List<EnemyControl>();

	public bool shooting { get; set; }

	private void Awake()
	{
		m_tentacleBullet = GetComponent<TentacleBullet>();
	}

	protected virtual void OnEnable()
	{
		if (trail != null)
		{
			trail.Clear();
			trail.SetActive(true);
		}
		lastPos = transform.position;
	}

	private void OnDisable()
	{
		transform.ResetPosition();
		transform.ResetRotation();

		shooting = false;
	}

	public virtual void Init(InfoAttacker _infoAttacker)
	{
		countPierce = 0;
		heroes.Clear();
		enemies.Clear();
		infoAttacker = _infoAttacker;
		lastPos = transform.position;
	}

	/// <summary>
	/// Bullet Shot
	/// </summary>
	public void Shot(float speed, float angle, float accelSpeed, float accelTurn,
		bool homing, Transform homingTarget, float homingAngleSpeed,
		bool wave, float waveSpeed, float waveRangeSize,
		bool pauseAndResume, float pauseTime, float resumeTime, Util.AXIS axisMove)
	{
		if (shooting)
		{
			return;
		}

		shooting = true;

		m_speed = speed;
		m_angle = angle;
		m_accelSpeed = accelSpeed;
		m_accelTurn = accelTurn;
		m_homing = homing;
		m_homingTarget = homingTarget;
		m_homingAngleSpeed = homingAngleSpeed;
		m_wave = wave;
		m_waveSpeed = waveSpeed;
		m_waveRangeSize = waveRangeSize;
		m_pauseAndResume = pauseAndResume;
		m_pauseTime = pauseTime;
		m_resumeTime = resumeTime;
		m_axisMove = axisMove;

		if (axisMove == Util.AXIS.X_AND_Z)
		{
			// X and Z axis
			transform.SetEulerAnglesY(-angle);
		}
		else
		{
			// X and Y axis
			transform.SetEulerAnglesZ(angle);
		}

		m_selfFrameCnt = 0f;
		m_selfTimeCount = 0f;
		lastPos = transform.position;
	}

	/// <summary>
	/// Update Move
	/// </summary>
	public virtual void UpdateMove()
	{
		if (shooting == false)
		{
			return;
		}

		m_selfTimeCount += BulletTimer.instance.deltaTime;

		// pause and resume.
		if (m_pauseAndResume && m_pauseTime >= 0f && m_resumeTime > m_pauseTime)
		{
			if (m_pauseTime <= m_selfTimeCount && m_selfTimeCount < m_resumeTime)
			{
				return;
			}
		}

		if (m_homing)
		{
			// homing target.
			if (m_homingTarget != null && 0f < m_homingAngleSpeed)
			{
				float rotAngle = Util.GetAngleFromTwoPosition(transform, m_homingTarget, m_axisMove);
				float myAngle = 0f;
				if (m_axisMove == Util.AXIS.X_AND_Z)
				{
					// X and Z axis
					myAngle = -transform.eulerAngles.y;
				}
				else
				{
					// X and Y axis
					myAngle = transform.eulerAngles.z;
				}

				float toAngle =
					Mathf.MoveTowardsAngle(myAngle, rotAngle, BulletTimer.instance.deltaTime * m_homingAngleSpeed);

				if (m_axisMove == Util.AXIS.X_AND_Z)
				{
					// X and Z axis
					transform.SetEulerAnglesY(-toAngle);
				}
				else
				{
					// X and Y axis
					transform.SetEulerAnglesZ(toAngle);
				}
			}

		}
		else if (m_wave)
		{
			// acceleration turning.
			m_angle += (m_accelTurn * BulletTimer.instance.deltaTime);
			// wave.
			if (0f < m_waveSpeed && 0f < m_waveRangeSize)
			{
				float waveAngle = m_angle + (m_waveRangeSize / 2f * Mathf.Sin(m_selfFrameCnt * m_waveSpeed / 100f));
				if (m_axisMove == Util.AXIS.X_AND_Z)
				{
					// X and Z axis
					transform.SetEulerAnglesY(-waveAngle);
				}
				else
				{
					// X and Y axis
					transform.SetEulerAnglesZ(waveAngle);
				}
			}

			m_selfFrameCnt += BulletTimer.instance.deltaFrameCount;
		}
		else
		{
			// acceleration turning.
			float addAngle = m_accelTurn * BulletTimer.instance.deltaTime;
			if (m_axisMove == Util.AXIS.X_AND_Z)
			{
				// X and Z axis
				transform.AddEulerAnglesY(-addAngle);
			}
			else
			{
				// X and Y axis
				transform.AddEulerAnglesZ(addAngle);
			}
		}

		// acceleration speed.
		m_speed += (m_accelSpeed * BulletTimer.instance.deltaTime);

		//nếu tăng tốc độ game thì phải giới hạn tốc độ bullet lại
		var timeScale = Time.timeScale;
		if (timeScale >= 1.5f && m_speed * timeScale >= 50f) m_speed = 50f / timeScale;

		// move.
		if (m_axisMove == Util.AXIS.X_AND_Z)
		{
			// X and Z axis
			transform.localPosition += transform.forward.normalized * m_speed * BulletTimer.instance.deltaTime;
		}
		else
		{
			// X and Y axis
			transform.localPosition += transform.up.normalized * m_speed * BulletTimer.instance.deltaTime;
		}

		if (m_tentacleBullet != null)
		{
			// Update tentacles
			m_tentacleBullet.UpdateRotate();
		}

		CheckHits();
	}
	Vector2 lastPos;
	protected virtual void CheckHits()
	{
		if (!gameObject.activeSelf) return;
		var timeScale = Time.timeScale;
		float alpha = 1f;
		//if (timeScale > 1f)
		//{
		//    alpha = 1f + (timeScale - 1f) / 2f; //1.5f -> 1.25f    2.5f -> 1.75f
		//}
		alpha = timeScale;
		LayerMask mask = LayerMask.GetMask(Constants.ConstantsLayer.LAYER_CHARACTER_NAME);

		RaycastHit2D[] hits = Physics2D.LinecastAll((Vector2)transform.position, lastPos, mask);
		if (hits == null || hits.Length <= 0)
		{
			hits = Physics2D.CircleCastAll((Vector2)transform.position, radius * alpha, Vector2.zero, 0, mask);
		}
		lastPos = transform.position;
		foreach (var item in hits)
		{
			var other = item.collider;

			if (infoAttacker.fromHero) //dùng biến này đơn thuần vì hiệu năng
			{
				if (other.CompareTag(Config.TAG_ENEMY))
				{
					var enemyControl = other.GetComponent<EnemyControl>();
					if (!enemyControl.IsDead()
						&& (!enemyControl.InInvisible || (enemyControl.InInvisible && infoAttacker.type == InfoAttacker.TYPE_SKILL))
						&& !enemies.Contains(enemyControl)) //mỗi viên đạn chỉ xuyên một con một lần
					{
						enemies.Add(enemyControl);
						countPierce++;

						if (countPierceMax > 0 && countPierce <= countPierceMax)
						{
							SpawnImpact(enemyControl);
							enemyControl.GetHit(infoAttacker);

							var r = UnityEngine.Random.Range(0f, 1f);
							if (r <= (infoAttacker.knockback / 100f))
							{
								AddForce(enemyControl.transform);
							}

							//break loop
							if (countPierce == countPierceMax) ReleaseBullet();
						}
						else if (countPierceMax == -1)
						{
							SpawnImpact(enemyControl);
							enemyControl.GetHit(infoAttacker);

							var r = UnityEngine.Random.Range(0f, 1f);
							if (r <= (infoAttacker.knockback / 100f))
							{
								AddForce(enemyControl.transform);
							}
						}
					}
				}
			}
			else
			{
				if (other.CompareTag(Config.TAG_HERO))
				{
					var heroControl = other.GetComponent<HeroControl>();
					if (!heroes.Contains(heroControl)) //mỗi viên đạn chỉ xuyên một con một lần
					{
						heroes.Add(heroControl);
						countPierce++;

						if (countPierceMax > 0 && countPierce <= countPierceMax)
						{
							SpawnImpact(heroControl);
							heroControl.GetHit(infoAttacker);

							//break loop
							if (countPierce == countPierceMax) ReleaseBullet();
						}
						else if (countPierceMax == -1)
						{
							SpawnImpact(heroControl);
							heroControl.GetHit(infoAttacker);
						}
					}
				}
			}
		}

		//break loop
		var pos = transform.position;
		if (pos.x > 9f || pos.x < -9f || pos.y < -12f || pos.y > 12f)
		{
			ReleaseBullet();
		}
	}

	protected virtual void ReleaseBullet()
	{
		if (trail != null)
		{
			trail.Clear();
			trail.SetActive(false);
		}
		GameplayController.Instance.ReleaseBullet(this);
	}

	protected void SpawnImpact(EnemyControl enemyControl)
	{
		if (impactPrefab != null)
		{
			Vector2 offset = enemyControl.Offset;
			Vector2 size = enemyControl.Size;
			float deltaX = size.x / 2f;
			float deltaY = size.y / 2f;
			Vector3 spawnPos = enemyControl.transform.position + new Vector3(UnityEngine.Random.Range(-deltaX, deltaX) + offset.x, UnityEngine.Random.Range(-deltaY, deltaY) + offset.y, 0f);
			GameplayController.Instance.SpawnImpact(impactPrefab, spawnPos, Quaternion.identity);
		}
	}

	protected void SpawnImpact(HeroControl heroControl)
	{
		if (impactPrefab != null)
		{
			Vector2 offset = heroControl.Offset;
			Vector2 size = heroControl.Size;
			float deltaX = size.x / 2f;
			float deltaY = size.y / 2f;
			Vector3 spawnPos = heroControl.transform.position + new Vector3(UnityEngine.Random.Range(-deltaX, deltaX) + offset.x, UnityEngine.Random.Range(-deltaY, deltaY) + offset.y, 0f);
			GameplayController.Instance.SpawnImpact(impactPrefab, spawnPos, Quaternion.identity);
		}
	}

	protected void AddForce(Transform target)
	{
		target.DOBlendableMoveBy(transform.up.normalized * infoAttacker.rangeKnockBack, 0.4f).SetEase(Ease.OutQuad);
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}

[Serializable]
public class InfoAttacker
{
	public const int TYPE_NORMAL = 1;
	public const int TYPE_SKILL = 2;

	//lúc đầu định bỏ cái nerf này đi nhưng vẫn có những skill active khi bullet chạm enemy
	//chứ không phải active ngay lập tức được
	public InfoAttacker(bool _fromHero,
						int _type,
						HeroExControl _heroExControl,
						EnemyExControl _enemyExControl,
						SkillNerf _skillNerf,
						float _damageAttacker,
						float _critRate,
						float _critDamage,
						float _accuracyAttacker,
						float _knockback = 0f,
						float _startY = -1f,
						float _rangeY = -1f)
	{
		fromHero = _fromHero; //dùng biến này đơn thuần vì hiệu năng
		type = _type;
		heroExControl = _heroExControl;
		enemyExControl = _enemyExControl;
		skillNerf = _skillNerf;
		damageAttacker = _damageAttacker;
		critRate = _critRate;
		critDamage = _critDamage;
		accuracyAttacker = _accuracyAttacker;
		knockback = _knockback;
		startY = _startY;
		rangeY = _rangeY;
		rangeKnockBack = 0.5f;
	}

	public bool fromHero;
	public int type;
	public HeroExControl heroExControl;
	public EnemyExControl enemyExControl;
	public SkillNerf skillNerf;
	public float damageAttacker;
	public float critRate;
	public float critDamage;
	public float accuracyAttacker;
	public float knockback;
	public float startY;
	public float rangeY;
	// public float dps;
	// public float dpsDuration;
	public float rangeKnockBack;

}

[Serializable]
public class SkillBuff
{
	public const int ID_TRAP_FIRST_AIR_KIT = -1000;

	public SkillBuff(int _baseId,
						ParticleMan _effectBuff,
						float _timePlay = -1)
	{
		baseId = _baseId;
		effectBuff = _effectBuff;
		timePlay = _timePlay;
	}

	// //ông nội Trường đừng add thêm constructor nữa, gán thuộc tính đi :))))
	// public SkillBuff(int _baseId, ParticleMan _effectBuff, float _timePlay, float _healHP, float _moreRegenHPPercent)
	// {
	//     baseId = _baseId;
	//     effectBuff = _effectBuff;
	//     timePlay = _timePlay;
	//     healHP = _healHP;
	//     moreRegenHPPercent = _moreRegenHPPercent;
	// }
	//
	// //ông nội Trường đừng add thêm constructor nữa, gán thuộc tính đi :))))
	// public SkillBuff(int _baseId, ParticleMan _effectBuff, float _timePlay, float _damageReduction)
	// {
	//     baseId = _baseId;
	//     effectBuff = _effectBuff;
	//     timePlay = _timePlay;
	//     damageReduction = _damageReduction;
	// }

	public SkillBuff(SkillBuff skillBuff)
	{
		baseId = skillBuff.baseId;
		effectBuff = skillBuff.effectBuff;
		timePlay = skillBuff.timePlay;
		moreAttackSpeedPercent = skillBuff.moreAttackSpeedPercent;
		moreCritRatePercent = skillBuff.moreCritRatePercent;
		moreCritDamagePercent = skillBuff.moreCritDamagePercent;
		hpStealByPercentDamage = skillBuff.hpStealByPercentDamage;
		shield = skillBuff.shield;
		shieldMax = skillBuff.shieldMax;
		healHP = skillBuff.healHP;
		moreRegenHP = skillBuff.moreRegenHP;
		moreRegenHPPercent = skillBuff.moreRegenHPPercent;
		damageReduction = skillBuff.damageReduction;
		moreAttackSpeed = skillBuff.moreAttackSpeed;
		moreDamagePercent = skillBuff.moreDamagePercent;
		damagedTarget = skillBuff.damagedTarget;
		shieldBarrier = skillBuff.shieldBarrier;
		immortal = skillBuff.immortal;
	}

	//nếu không có cái gì tức là theo value
	//tên biến cái nào theo phần trăm thì thêm percent vào cuối
	//nếu phần trăm theo damage thì ByPercentDamage
	//more tức là add thêm
	public int baseId;
	public ParticleMan effectBuff;
	public float timePlay;
	public float moreAttackSpeedPercent;
	public float moreCritRatePercent;
	public float moreCritDamagePercent;
	public float hpStealByPercentDamage; //đồng thời mỗi phát đánh hồi lại lượng HP = 20% sát thương gây ra. 
	public float shield; //Bật 1 lớp Shield cho toàn bộ đồng đội . Lớp Shield có thể chịu được lượng damage bằng 50% HP của East
	public float shieldMax;
	public float healHP; // giá trị hồi máu tức thì
	public float moreRegenHP; // buff tăng regen HP
	public float moreRegenHPPercent; // giá trị hồi máu được buff thêm. VD: regenPercent = 120 => hồi máu = 120% giá trị có sẵn
	public float damageReduction; // giảm bao nhiêu % damage nhận vào
	public float moreAttackSpeed;
	public float moreDamagePercent;
	public HeroExControl damagedTarget; //target gánh damage cho đồng đội
	public float shieldBarrier; // shield đặc biệt, đỡ đạn cho toàn bộ team
	public bool immortal;
}

[Serializable]
public class SkillNerf
{
	public const int ID_TRAP_ICE = -1001;
	public const int ID_TRAP_ELECTRIC = -1001;

	public SkillNerf(int _baseId, HeroExControl _heroExControl,
						ParticleMan _effectNerf,
						float _timePlay = -1)
	{
		baseId = _baseId;
		heroExControl = _heroExControl;
		effectNerf = _effectNerf;
		timePlay = _timePlay;
		color = Color.red;
	}

	public SkillNerf(SkillNerf skillNerf)
	{
		baseId = skillNerf.baseId;
		effectNerf = skillNerf.effectNerf;
		timePlay = skillNerf.timePlay;
		downMovementPercent = skillNerf.downMovementPercent;
		damagePerSec = skillNerf.damagePerSec;
		damagePercentHpTargetPerSec = skillNerf.damagePercentHpTargetPerSec;
		stun = skillNerf.stun;
		color = skillNerf.color;
		armorReducePercent = skillNerf.armorReducePercent;
		dieImmediatePercent = skillNerf.dieImmediatePercent;
		damageAsHealPercent = skillNerf.damageAsHealPercent;
		isSleep = skillNerf.isSleep;   // dùng cho skill ngủ của virus
		isFirstDamage = skillNerf.isFirstDamage; // dùng cho skill ngủ của virus, có cả damage của skill
		stunPercent = skillNerf.stunPercent;

		heroExControl = skillNerf.heroExControl;
	}

	public int baseId;
	public ParticleMan effectNerf;
	public float timePlay;
	public float downMovementPercent;
	public float damagePerSec;
	public float damagePercentHpTargetPerSec;
	public bool stun; //stun freeze trói
	public Color color;
	public float armorReducePercent;
	public float dieImmediatePercent;
	public float damageAsHealPercent;
	public bool isSleep;
	public bool isFirstDamage;
	public float stunPercent; //hot fix Những enemy dính đạn sẽ có 40% khả năng bị đóng băng vĩnh viễn ( không tác dụng với boss và mini boss )
	public HeroExControl heroExControl;

}
