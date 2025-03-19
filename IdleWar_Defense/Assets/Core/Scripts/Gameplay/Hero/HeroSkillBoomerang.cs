using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoodZombie;
using UnityEngine;

public class HeroSkillBoomerang : HeroSkill
{
    public Bullet boomerang;
    public Transform baseLocation;
    public ParticleMan effectInEnemyPrefab;
    public Vector3[] curvePath;
     public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var skillNerf = new SkillNerf(heroExControl.heroData.baseId, heroExControl, effectInEnemyPrefab);
        skillNerf.armorReducePercent = skillValues[1];
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL, 
                                                        heroExControl, 
                                                        null,
                                                        skillNerf,
                                                        heroExControl.Damage * skillValues[0] / 100f,
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        100f);

        var start = new Vector2(baseLocation.transform.position.x,baseLocation.transform.position.y);
        boomerang.Init(infoAttacker);
        boomerang.shooting = true;
        boomerang.gameObject.SetActive(true);
        BulletManager.instance.AddBullet(boomerang);
        //bombPrefab[i].Shot(0,0,0,0,false,null,0,false,0,0,false,0,0,Util.AXIS.X_AND_Y);
        boomerang.transform.localPosition = Vector3.zero;
        boomerang.GetComponent<DOTweenPath>().DOPlay();
        // var tween = boomerang.gameObject.transform.DOPath(curvePath, 0.75f, PathType.CatmullRom, PathMode.TopDown2D)
        //      .SetEase(Ease.OutCirc).OnComplete(() => { boomerang.shooting = true;});
        
    }
}
