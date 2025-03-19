using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using AnimationState = Spine.AnimationState;

public class SkeletonAnimationController : MonoBehaviour
{
    [SerializeField] protected SkeletonAnimation skeletonAnim;
    public List<AnimationInfo> animations;
    private AnimationState animationState;

    private void Awake()
    {
        animationState = skeletonAnim.AnimationState;
        RegisterEvent();
    }

    public void PlayAnimationWithName(int trackIndex, string nameAnim, bool isLoop)
    {
        skeletonAnim.AnimationState.SetAnimation(trackIndex, nameAnim, isLoop);
    }

    protected void RegisterEvent()
    {
        if (null != animationState)
        {
            animationState.Start += OnSpineAnimationStart;
            animationState.Interrupt += OnSpineAnimationInterrupt;
            animationState.End += OnSpineAnimationEnd;
            animationState.Dispose += OnSpineAnimationDispose;
            animationState.Complete += OnSpineAnimationComplete;
            animationState.Event += HandleEvent;
        }
    }

    protected void UnRegisterEvent()
    {
        if (null != animationState)
        {
            animationState.Event -= HandleEvent;
        }
    }

    public void OnSpineAnimationStart(TrackEntry trackEntry)
    {
        if (animations.Exists(anim => anim.animation.Equals(trackEntry.Animation.Name)))
        {
            var index = animations.FindIndex(anim => anim.animation.Equals(trackEntry.Animation.Name));
            animations[index].OnAnimationStart.Invoke();
        }
    }

    public void OnSpineAnimationInterrupt(TrackEntry trackEntry)
    {
        // Add your implementation code here to react to interrupt events
    }

    public void OnSpineAnimationEnd(TrackEntry trackEntry)
    {
        if (animations.Exists(anim => anim.animation.Equals(trackEntry.Animation.Name)))
        {
            var index = animations.FindIndex(anim => anim.animation.Equals(trackEntry.Animation.Name));
            animations[index].OnAnimationEnd.Invoke();
        }
    }

    public void OnSpineAnimationDispose(TrackEntry trackEntry)
    {
    }

    public void OnSpineAnimationComplete(TrackEntry trackEntry)
    {
        if (animations.Exists(anim => anim.animation.Equals(trackEntry.Animation.Name)))
        {
            var index = animations.FindIndex(anim => anim.animation.Equals(trackEntry.Animation.Name));
            animations[index].OnAnimationComplete.Invoke();
        }
    }

    public void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (animations.Exists(anim => anim.animation.Equals(trackEntry.Animation.Name)))
        {
            var index = animations.FindIndex(anim => anim.animation.Equals(trackEntry.Animation.Name));
            var animation = animations[index];
            if (animation.events.Exists(eventInfo => eventInfo.eventName.Equals(e.Data.Name)))
            {
                var eventIndex = animation.events.FindIndex(eventInfo => eventInfo.eventName.Equals(e.Data.Name));
                animation.events[eventIndex].OnEventTrigger.Invoke();
            }
        }
    }

    public float DurationAnimation(string name)
    {
        var anim = skeletonAnim.Skeleton.Data.FindAnimation(name);
        if (anim != null)
        {
            return anim.Duration;
        }

        return 0;
    }

    [Button]
    private void InitAnimation()
    {
        //skeletonAnim = GetComponent<SkeletonAnimation>();
        if (skeletonAnim.skeletonDataAsset != null)
        {
            var _length = skeletonAnim.skeletonDataAsset.GetSkeletonData(true).Animations.Items.Length;

            animations = new List<AnimationInfo>();
            for (var i = 0; i < _length; i++)
            {
                animations.Add(new AnimationInfo(skeletonAnim.skeletonDataAsset.GetSkeletonData(true).Animations
                    .Items[i].Name));
            }
        }
    }
}

[Serializable]
public class AnimationInfo
{
    public string animation;
    public List<AnimationEventInfo> events;
    public UnityEvent OnAnimationStart;
    public UnityEvent OnAnimationComplete;
    public UnityEvent OnAnimationEnd;

    public AnimationInfo(string animation)
    {
        this.animation = animation;
    }
}

[Serializable]
public class AnimationEventInfo
{
    public string eventName;
    public UnityEvent OnEventTrigger;
}