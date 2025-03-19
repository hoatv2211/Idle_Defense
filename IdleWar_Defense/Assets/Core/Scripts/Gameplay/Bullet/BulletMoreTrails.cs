using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Utilities.Common;

/// <summary>
/// bullet.
/// </summary>
public class BulletMoreTrails : Bullet
{
    public TrailRenderer[] trails;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        var count = trails.Length;
        for (int i = 0; i < count; i++)
        {
            var trail = trails[i];
            trail.Clear();
            trail.SetActive(true);
        }
    }

    protected override void ReleaseBullet()
    {
        var count = trails.Length;
        for (int i = 0; i < count; i++)
        {
            var trail = trails[i];
            trail.Clear();
            trail.SetActive(false);
        }

        base.ReleaseBullet();
    }
}
