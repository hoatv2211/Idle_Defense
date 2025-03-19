using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class BulletSkeletonAnimation : MonoBehaviour
{
    public SkeletonAnimation ske;
    public string animation;
    public bool loop;
    public void OnComplete(Action action)
    {
        var trackEntry = ske.state.SetAnimation(0, animation, loop);
        trackEntry.Complete += entry => { action?.Invoke();};
    }
}
