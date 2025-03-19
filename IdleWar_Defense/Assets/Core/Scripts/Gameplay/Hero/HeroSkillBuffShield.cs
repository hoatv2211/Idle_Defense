using FoodZombie;

public class HeroSkillBuffShield : HeroSkill
{
    public ParticleMan effectInHeroPrefab;
    public bool buffAll;
    public override void TriggerSkill()
    {
        if (heroExControl.IsDead()) return;

        if (!soundTriggerSkill.Equals("")) SoundManager.Instance.PlaySFX(soundTriggerSkill);

        var skillValues = heroExControl.SkillValues;
        var skillBuff = new SkillBuff(heroExControl.heroData.baseId, effectInHeroPrefab, skillValues[0]);
        skillBuff.damageReduction = skillValues[1];
        if (buffAll)
        {
            var heroes = GameplayController.Instance.GetHeroExs();
            var count = heroes.Count;
            for (int i = 0; i < count; i++)
            {
                var hero = (HeroExControl) heroes[i];
                if(!hero.IsDead()) hero.AddBuff(skillBuff);
            }
        }
        else
        {
            heroExControl.AddBuff(skillBuff);
        }
    }
}
