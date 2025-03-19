using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoodZombie;
using Spine.Unity;
using UnityEngine;
using Utilities.Common;

public class HeroSkillThrowArrow : HeroSkill
{
    [SerializeField] public Transform baseLocation;

    public Vector2 topLeft;
    public Vector2 bottomRight;
    public BulletDamageDelay bombPrefab;

    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL, 
                                                        heroExControl, 
                                                        null,
                                                        null,
                                                        heroExControl.Damage * skillValues[2] / 100f,
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        1f);

        Vector3[] curvePath;
        var start = new Vector2(baseLocation.transform.localPosition.x,baseLocation.transform.localPosition.y);
        for (var i = 0; i < skillValues[1]; i++)
        {
            var bomb = Instantiate(bombPrefab,baseLocation);
            bomb.Init(infoAttacker);
            bomb.delayDuration = skillValues[0];
            bomb.gameObject.SetActive(true);
            BulletManager.instance.AddBullet(bomb);
            bomb.transform.localPosition = Vector3.zero;
            bomb.shooting = true;
            
            var destination = new Vector2(Random.Range(topLeft.x,bottomRight.x),Random.Range(bottomRight.y,topLeft.y));
            curvePath = new Vector3[]
            {
                start,
                //(start+destination)/2 + new Vector2((start.x-destination.x)/2,0f),
                destination
            };
            bomb.gameObject.transform.position = destination;
            bomb.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "animation", false);
            bomb.StartCountDown();
            //bomb.gameObject.transform.DOMove(destination, 0.75f).SetEase(Ease.OutCirc).OnComplete(() => { bomb.shooting = true;});
            // bomb.gameObject.transform.DOPath(curvePath, 0.75f, PathType.CatmullRom, PathMode.TopDown2D)
            //     .SetEase(Ease.OutCirc).OnComplete(() => { bomb.shooting = true;});

        }
    }
}
