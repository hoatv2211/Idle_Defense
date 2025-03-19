using FoodZombie;

public class HeroSkillBarrierShield : HeroSkill
{
    public ParticleMan effectInHeroPrefab;
    public bool buffAll;
    
    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var skillBuff = new SkillBuff(heroExControl.heroData.baseId, effectInHeroPrefab, skillValues[0]);
        skillBuff.shield = skillBuff.shieldMax = heroExControl.HP_MAX * skillValues[1] / 100f;
        skillBuff.damagedTarget = heroExControl;
        
        if (buffAll)
        {
            var heroes = GameplayController.Instance.GetHeroExs();
            var count = heroes.Count;
            for (int i = 0; i < count; i++)
            {
                var hero = heroes[i];
                if (!hero.IsDead()) hero.AddBuff(skillBuff);
            }
        }
        else
        {
            if (!heroExControl.IsDead()) heroExControl.AddBuff(skillBuff);
        }
    }
}
