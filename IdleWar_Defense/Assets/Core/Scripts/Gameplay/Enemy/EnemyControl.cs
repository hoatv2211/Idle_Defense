using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using Spine.Unity;
using Spine;
using System.Collections.Generic;
using System;
using FoodZombie;

public class EnemyControl : MonoBehaviour
{
	public int level;
	public int Score
	{
		get
		{
			return (int)(0.1 * POWER);
		}
	}
	public int POWER;
	protected int[] attackTypes;
	public int[] AttackTypes => attackTypes;

	//public float knockBack = 1f;
	public float HP;
	protected float HP_MAX;
	public float hpx { get; set; }
	public float damx { get; set; }
	public bool isBoss;

	protected const int ANIM_IDLE_STAGE = 1;
	protected const int ANIM_DEAD_STAGE = 0;
	public GameObject[] relativeObjects;
	public Vector3 PositionHpBar;
	HpBar _hpBar;
	public HpBar hpBar
	{
		get
		{
			if (_hpBar == null)
			{
				_hpBar = SimplePool.Spawn(GameUnityData.instance.HpBarEnemyPrefab, transform.position, Quaternion.identity);
				_hpBar.transform.position = PositionHpBar + transform.position + new Vector3(0, 0, -1);
				_hpBar.transform.parent = this.transform;

			}
			return _hpBar;
		}
		set
		{
			_hpBar = value;
		}
	}
	[SerializeField] protected BoxCollider2D collider2D;
	public Vector2 Offset => collider2D.offset;
	public Vector2 Size => collider2D.size;

	public string soundDie;
	public string soundBeHit;
	public string soundInvisible;

	[HideInInspector] public int stage = -1;

	[HideInInspector] protected bool inInvisible = false;
	public bool InInvisible => inInvisible;
	//  public bool SpawnItemOnDead { get; set; }
	public virtual int STAGE
	{
		get { return stage; }
		set { stage = value; }
	}
	private void Awake()
	{
		this.gameObject.layer = LayerMask.NameToLayer(Constants.ConstantsLayer.LAYER_CHARACTER_NAME);

	}
	public virtual void Init()
	{
		attackTypes = new[] { IDs.ATTACK_TYPE_RANGE };

		//máu của barrier, cái này kế thừa ra 1 cái riêng sau
		HP = 1000;
		HP_MAX = HP;

		Refresh();
		GameplayController.Instance.AddEnemy(this);
	}

	public virtual void Init(EnemyData _enemyData, int level, float hpx = 1, float damx = 1)
	{
		this.level = level;
		attackTypes = _enemyData.AttackTypes;
		this.hpx = hpx;
		this.damx = damx;
		HP = _enemyData.GetHP(level) * hpx;
		HP_MAX = HP;

		Refresh();
	}

	public virtual void Refresh()
	{
		hpBar.Init();
		hpBar.ShowHP(HP, HP_MAX);
		collider2D.enabled = true;
		GameplayController.Instance.AddEnemy(this);
		var count = relativeObjects.Length;
		for (int i = 0; i < count; i++)
		{
			relativeObjects[i].SetActive(false);
		}
		inInvisible = false;

		stage = -1;
		STAGE = ANIM_IDLE_STAGE;
	}

	public virtual void AnimIdle()
	{
		STAGE = ANIM_IDLE_STAGE;
	}

	public void AnimDead()
	{
		STAGE = ANIM_DEAD_STAGE;
		OnDead();
	}

	public bool IsDead()
	{
		return STAGE.Equals(ANIM_DEAD_STAGE);
	}

	public virtual void OnDead()
	{
		if (!soundDie.Equals("")) SoundManager.Instance.PlaySFX(soundDie);
		var count = relativeObjects.Length;
		for (int i = 0; i < count; i++)
		{
			relativeObjects[i].SetActive(false);
		}
		collider2D.enabled = false;
		GameplayController.Instance.RemoveEnemy(this);
		if (hpBar != null)
		{
			hpBar.transform.parent = GameplayController.Instance.transform;
			SimplePool.Despawn(hpBar.gameObject);
			hpBar = null;
		}

	}

	public virtual void GetHit(InfoAttacker infoAttacker)
	{
		if (IsDead()) return;

		var pos = transform.position;
		if (pos.y > Config.LOWEST_Y || pos.x <= -Config.MAX_X || pos.x >= Config.MAX_X)
		{
			return;
		}

		float damageAttacker = infoAttacker.damageAttacker;
		if (infoAttacker.heroExControl != null)
		{
			var rangeY = infoAttacker.rangeY;
			if (rangeY > 0f)
			{
				var distance = transform.position.y - infoAttacker.startY;
				if (distance >= rangeY) damageAttacker = ConfigStats.GetPercentOutRangeDamage() * damageAttacker;
			}

			//hồi máu cho hero attack
			infoAttacker.heroExControl.GetHPBack(damageAttacker);
		}

		HP -= damageAttacker;
		if (HP <= 0)
		{
			HP = 0;
			AnimDead();
		}
		else
		{
			if (!soundBeHit.Equals("")) SoundManager.Instance.PlaySFX(soundBeHit);
		}

		var posHP = hpBar.ShowHP(HP, HP_MAX);
		GameplayController.Instance.SpawnHp(posHP + new Vector3(0f, 0f, -1f), damageAttacker, false, TextHp.TEXT_DAMAGE_HP);

		// if (infoAttacker.dps > 0 && infoAttacker.dpsDuration > 0)
		// {
		//     StartCoroutine(GotDamagedPerSecond(infoAttacker.dps,infoAttacker.dpsDuration));
		// }
	}

	public virtual void SetInvisible(bool value)
	{
		var oldInInvisible = inInvisible;
		inInvisible = value;

		if (inInvisible && oldInInvisible != inInvisible) StartInvisible();
		else if (!inInvisible && oldInInvisible != inInvisible) EndInvisible();
	}

	public virtual void StartInvisible()
	{
	}

	public virtual void EndInvisible()
	{
		if (!soundInvisible.Equals("")) SoundManager.Instance.PlaySFX(soundInvisible);
	}

	// private IEnumerator GotDamagedPerSecond(float dps,float duration)
	// {
	//     var countTime = duration;
	//     while (!IsDead() && countTime>0)
	//     {
	//         HP -= dps;
	//         if (HP <= 0)
	//         {
	//             HP = 0;
	//             AnimDead();
	//         }
	//         var pos = hpBar.ShowHP(HP, HP_MAX);
	//         GameplayController.Instance.SpawnHp(pos + new Vector3(0f, 0f, -1f), dps, false, TextHp.TEXT_DAMAGE_HP);
	//         yield return new WaitForSeconds(1);
	//         countTime -= 1f;
	//     }
	// }
#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position + PositionHpBar, .2f);
	}

#endif
}