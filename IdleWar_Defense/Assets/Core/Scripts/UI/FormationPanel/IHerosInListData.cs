using FoodZombie;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FoodZombie.UI
{
	public interface IHerosInListData
	{
		List<HeroData> allHeroes { get; set; }
		List<HeroData> allHeroesSelect { get; set; }
		List<HeroData> allHeroesToView { get; }

		int MinPOWonSelect { get; }
		bool HaveOtherStrongerNotSelect(HeroData data);

		void ChoiceHeroData(HeroData herodata);
	}
}
