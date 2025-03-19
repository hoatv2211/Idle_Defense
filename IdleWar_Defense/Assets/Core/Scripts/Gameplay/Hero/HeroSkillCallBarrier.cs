using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoodZombie;
using UnityEngine;
using Utilities.Common;

//tất cả các thể loại tường barrier
public class HeroSkillCallBarrier : HeroSkill
{
    [SerializeField, Tooltip("Buildin Pool")] private List<HeroControl> barriersPool;
    public float sizeBarrier = 3.2f;
    

    public override void Init(HeroExControl _heroExControl)
    {
        base.Init(_heroExControl);
        
        barriersPool.Free();
    }

    protected Vector3 vtPos;
    public override void AoeCastSkill(Vector3 _pos)
    {
        vtPos = _pos;
        isCast = true;
    }

    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var count = skillValues[1];
        for (int i = 0; i < count; i++) 
        {
            var barrier = barriersPool.Obtain(GameplayController.Instance.transformPool);
            barrier.Init(skillValues[0] * heroExControl.HP_MAX / 100f, 0f);

            var pos = Vector3.zero;
           
            if (isCast& !GameplayController.Instance.autoPlay)
            {
                pos = vtPos;
            }
            else
            {
                pos = transform.position + new Vector3(Config.EasyRandom(-2f, 2f), Config.EasyRandom(3.5f, 6f) * Config.xRange, 0f);
                var target = heroExControl.target;
                if (target != null && !target.IsDead())
                {
                    pos = transform.position * 0.6f + (target.transform.position + (Vector3)target.Offset) * 0.4f;
                }
            }
           
            //trường hợp 2 barrie
            pos += new Vector3(-(count - 1) * sizeBarrier / 2f + i * sizeBarrier, 0f, 0f);
        
            barrier.transform.position = new Vector3(pos.x, pos.y, OderLayerZ.PIVOT_POINT);
            barrier.SetActive(true);
        }
    }
}
