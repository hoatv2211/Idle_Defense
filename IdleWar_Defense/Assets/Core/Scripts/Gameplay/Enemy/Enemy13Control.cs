using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using Spine;
using UnityEngine;

public class Enemy13Control : EnemyExControl
{
	public string jump1Animation, jump2Animation, jumpEvent;
	public const int ANIM_JUMP_1_STAGE = 6;
	protected const int ANIM_JUMP_2_STAGE = 7;
	public Vector3 lastPost;
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
						if (!attack.Attacking() && !IsSkilling() && stage != ANIM_JUMP_1_STAGE && stage != ANIM_JUMP_2_STAGE)
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
					case ANIM_JUMP_1_STAGE:
						if (stage != value)
						{
							stage = value;
							Debug.Log("co vao day");
							trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, jump1Animation, false);
							trackEntry.Complete += Anim_Jump1_Complete;

						}
						break;
					case ANIM_JUMP_2_STAGE:
						if (stage != value)
						{
							Debug.Log("vao jump2");
							trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, jump2Animation, false);
							trackEntry.Complete += Anim_Jump2_Complete;
							stage = value;

						}
						break;
				}


			}
		}
	}

	public override void AnimIdle()
	{
		if (STAGE != ANIM_JUMP_1_STAGE && STAGE != ANIM_JUMP_1_STAGE) STAGE = ANIM_IDLE_STAGE;
	}

	public override void AnimRun()
	{
		if (STAGE != ANIM_JUMP_1_STAGE && STAGE != ANIM_JUMP_1_STAGE) STAGE = ANIM_RUN_STAGE;
	}

	protected void Anim_Jump1_Complete(TrackEntry trackEntry)
	{
		Debug.Log("jump1 end");
		STAGE = ANIM_JUMP_2_STAGE;
	}
	protected void Anim_Jump2_Complete(TrackEntry trackEntry)
	{
		Debug.Log("jump2 end");
		STAGE = ANIM_IDLE_STAGE;
	}

	public override void Init(EnemyData _enemyData, int _level, float hpx = 1, float damx = 1)
	{
		base.Init(_enemyData, _level, hpx, damx);
		skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
	}
	private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
	{
		bool eventMatch = (jumpEvent == e.Data.Name); // Performance recommendation: Match cached reference instead of string.
		Debug.Log("trackEntry.Animation.Name = " + trackEntry.Animation.Name);
		if (eventMatch && trackEntry.Animation.Name.Equals(jump2Animation))
		{
			Debug.Log("back to pos = " + trackEntry.Animation.Name);
			autoMove.StopMove(0.4f);
			Debug.Log("last pos = " + lastPost);
			Debug.Log(" transform.position= " + transform.position);

			// jump back to last position
			transform.position = lastPost;
			Debug.Log(" transform.position= " + transform.position);

		}
	}
}
