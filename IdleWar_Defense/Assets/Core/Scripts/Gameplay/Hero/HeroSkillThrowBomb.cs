
using FoodZombie;
using Spine;
using UnityEngine;
using Utilities.Inspector;
using FoodZombie.UI;


public class HeroSkillThrowBomb : HeroSkill
{
    public ParticleMan effectInEnemyPrefab;
    public Transform baseLocation;
    public bool targetToCenter = false;
    public bool followData = true;
    
    public Vector2 topLeft;
    public Vector2 bottomRight;
    
    public BulletDamageDelay bombPrefab;
    public float delayDuration = 1.633f/4;

    private Vector3 vectorCast;
    public override void AoeCastSkill(Vector3 _pos)
    {
        vectorCast = _pos;
        isCast = true;
    }

    public override void TriggerSkill()
    {

        if (heroExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);
        var skillValues = heroExControl.SkillValues;
        var timePlay = skillValues[0];
        SkillNerf skillNerf = null;
        if (timePlay > 0f)
        {
            skillNerf = new SkillNerf(heroExControl.heroData.baseId, heroExControl, effectInEnemyPrefab, skillValues[0]);
            skillNerf.stun = true;
        }
        InfoAttacker infoAttacker = new InfoAttacker(true,
                                                        InfoAttacker.TYPE_SKILL,
                                                        heroExControl,
                                                        null,
                                                        skillNerf,
                                                        heroExControl.Damage * skillValues[1] / 100f,
                                                        heroExControl.CritRate,
                                                        heroExControl.CritDamage,
                                                        heroExControl.Accuracy,
                                                        heroExControl.Knockback);

        Vector3[] curvePath;
        var start = new Vector2(baseLocation.transform.localPosition.x, baseLocation.transform.localPosition.y);
        var count = 1f;
        if (followData)
        {
            count = skillValues[2];
        }
        for (var i = 0; i < count; i++)
        {
            var bomb = Instantiate(bombPrefab, baseLocation);
            bomb.Init(infoAttacker);
            BulletManager.instance.AddBullet(bomb);
            bomb.delayDuration = delayDuration;
            bomb.shooting = true;
            bomb.gameObject.SetActive(true);

            if (isCast&&!GameplayController.Instance.autoPlay)
            {
                bomb.transform.position = vectorCast;
                bomb.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-45f, 45f));
                bomb.StartCountDown();
                return;
            }
           
            if (targetToCenter)
            {
                var enemies = GameplayController.Instance.GetEnemies();
                var countEnemies = enemies.Count;
                var destination = Vector3.zero;
                for (int j = 0; j < countEnemies; j++)
                {
                    var enemy = enemies[j];
                    destination += enemy.transform.position + (Vector3)enemy.Offset;
                }
                destination = destination / countEnemies;
                bomb.transform.position = destination;
                // bomb.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-45f, 45f));
                bomb.StartCountDown();
            }
            else
            {
                var destination = new Vector2(Random.Range(topLeft.x, bottomRight.x),
                    Random.Range(bottomRight.y, topLeft.y));
                bomb.transform.position = destination;
                bomb.transform.localEulerAngles = new Vector3(0, 0, Random.Range(-45f, 45f));
                bomb.StartCountDown();
                //bomb.GetComponent<BulletSkeletonAnimation>().OnComplete(()=>{ bomb.shooting = true;});
                curvePath = new Vector3[]
                {
                    start,
                    //(start+destination)/2 + new Vector2((start.x-destination.x)/2,0f),
                    destination
                };
                //bomb.gameObject.transform.DOMove(destination, 0.75f).SetEase(Ease.OutCirc).OnComplete(() => { bomb.shooting = true;});
                // bomb.gameObject.transform.DOPath(curvePath, 0.75f, PathType.CatmullRom, PathMode.TopDown2D)
                //     .SetEase(Ease.OutCirc).OnComplete(() => { bomb.shooting = true;});
            }
        }
    }
}
