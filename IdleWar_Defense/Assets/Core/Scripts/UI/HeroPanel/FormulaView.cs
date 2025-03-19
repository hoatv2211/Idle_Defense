using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Components;

public class FormulaView : MonoBehaviour
{
    public HeroDefinition heroDefinition;
    public RewardView rewardView;
    public UnityAction<FormulaView> showFormula;
    public SimpleTMPButton btnShow;
    
    public GameObject imgHighlight;

    private void Start()
    {
        btnShow.onClick.AddListener(BtnShow_Pressed);
    }

    public void Init(HeroDefinition _heroDefinition, UnityAction<FormulaView> _showFormula)
    {
        heroDefinition = _heroDefinition;
        var rewardInfo = new RewardInfo(IDs.REWARD_TYPE_UNLOCK_CHARACTER, heroDefinition.id, 1);
        rewardView.Init(rewardInfo);
        showFormula = _showFormula;
    }

    private void BtnShow_Pressed()
    {
        showFormula(this);
    }
    
    public void Choice()
    {
        if (imgHighlight != null) imgHighlight.SetActive(true);
    }

    public void UnChoice()
    {
        if (imgHighlight != null) imgHighlight.SetActive(false);
    }
}
