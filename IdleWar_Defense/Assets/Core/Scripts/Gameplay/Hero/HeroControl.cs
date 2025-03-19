using FoodZombie;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroControl : SerializedMonoBehaviour
{
	protected int[] attackTypes;
	public int[] AttackTypes => attackTypes;

	//public float knockBack;
	protected float HP;
	public float GetHP => HP;
	public float GetHPPercent => (float)HP / (float)HP_MAX;
	public float HP_MAX;
	protected float hpRegen;
	protected float timeHpRegen = 1f;

	protected const int ANIM_IDLE_STAGE = 1;
	protected const int ANIM_DEAD_STAGE = 0;

	public HpBar hpBar;
	[SerializeField]
	protected BoxCollider2D collider2D;
	public Vector2 Offset => collider2D.offset;
	public Vector2 Size => collider2D.size;

	public GameObject[] relativeObjects;

	public string soundDie;
	public string soundBeHit;



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
			stage = value;
		}
	}

	private void Awake()
	{
		this.gameObject.layer = LayerMask.NameToLayer(Constants.ConstantsLayer.LAYER_CHARACTER_NAME);
	}
	public virtual void Init(float _HP, float _HPRegen)
	{
		attackTypes = new[] { IDs.ATTACK_TYPE_BASE };

		//máu của barrier, cái này kế thừa ra 1 cái riêng sau
		HP = _HP;
		HP_MAX = HP;
		hpRegen = _HPRegen;

		Refresh();
	}

	public virtual void Init(HeroData _heroData)
	{
		attackTypes = _heroData.AttackTypes;

		HP_MAX = _heroData.HPTotal;
		HP = HP_MAX;

		Refresh();
	}

	public virtual void Refresh()
	{
		if (hpBar != null)
		{
			hpBar.Init();
			hpBar.ShowHP(HP, HP_MAX);
		}

		var count = relativeObjects.Length;
		for (int i = 0; i < count; i++)
		{
			relativeObjects[i].SetActive(true);
		}
		collider2D.enabled = true;
		GameplayController.Instance.AddHero(this);

		stage = -1;
		STAGE = ANIM_IDLE_STAGE;
	}

	private void FixedUpdate()
	{
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
					var pos = hpBar.ShowHP(HP, HP_MAX);
				}
			}
			else
			{
				timeHpRegen -= Time.fixedDeltaTime;
			}
		}
	}

	public void AnimIdle()
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
		GameplayController.Instance.RemoveHero(this);
	}
	public System.Action<float> OnGetHit;
	public virtual void GetHit(InfoAttacker infoAttacker)
	{
		if (IsDead()) return;
		var damageAttacker = infoAttacker.damageAttacker;
		float realDam = damageAttacker;
		if (GameplayController.isMisstionIntro) realDam /= 1000f; //nếu trong tutorial intro thì dam chỉ bằng 1/10 và hero ko chết
		if (realDam >= HP) realDam = HP;
		HP -= realDam;
		if (HP <= 0)
		{
			HP = 0;
			AnimDead();
		}
		else
		{
			if (!soundBeHit.Equals("")) SoundManager.Instance.PlaySFX(soundBeHit);
		}
		if (OnGetHit != null) OnGetHit(realDam);

		if (hpBar != null)
		{
			var pos = hpBar.ShowHP(HP, HP_MAX);
			GameplayController.Instance.SpawnHp(pos + new Vector3(0f, 0f, -1f), damageAttacker, false, TextHp.TEXT_DAMAGE_HP);
		}
	}

	public virtual void LookAt(Vector3 targetPos)
	{

	}

	public virtual void BtnSkill_Pressed()
	{

	}


}

