using System;
using System.Collections;
using System.Collections.Generic;
using FoodZombie;
using FoodZombie.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

public class SummonNextPopup : MonoBehaviour
{
    public SimpleTMPButton btnPowerFragment;
    public SimpleTMPButton btnPowerFragmentX10;
    public SimpleTMPButton btnPowerCrytal;
    public SimpleTMPButton btnPowerCrytalX10;
    public SimpleTMPButton btnDevineCrytal;
    public SimpleTMPButton btnDevineCrytalX10;

    //free / 100 Gem / 1000 Gem
    public Image imgPowerFragment;
    public TextMeshProUGUI txtPowerFragment;
    public GameObject txtFreePowerFragment;

    public Image imgPowerCrytal;
    public TextMeshProUGUI txtPowerCrytal;
    public GameObject imgGem;
    public GameObject txtGem;
    public GameObject txtFreePowerCrytal;

    public Image imgPowerCrytalX10;
    public TextMeshProUGUI txtPowerCrytalX10;
    public GameObject imgGemX10;
    public GameObject txtGemX10;
    //end

    private void Start()
    {
        btnPowerFragment.SetUpEvent(BtnPowerFragment_Pressed);
        btnPowerFragmentX10.SetUpEvent(BtnPowerFragmentX10_Pressed);
        btnPowerCrytal.SetUpEvent(BtnPowerCrytal_Pressed);
        btnPowerCrytalX10.SetUpEvent(BtnPowerCrytalX10_Pressed);
        btnDevineCrytal.SetUpEvent(BtnDevineCrytal_Pressed);
        btnDevineCrytalX10.SetUpEvent(BtnDevineCrytalX10_Pressed);
    }

    public void CallStart(int _typeButton=1)
    {
        btnPowerFragment    .gameObject.SetActive(_typeButton==1);
        btnPowerFragmentX10 .gameObject.SetActive(_typeButton==1);
        btnPowerCrytal      .gameObject.SetActive(_typeButton == 2);
        btnPowerCrytalX10   .gameObject.SetActive(_typeButton == 2);
        btnDevineCrytal     .gameObject.SetActive(_typeButton == 3);
        btnDevineCrytalX10  .gameObject.SetActive(_typeButton == 3);

        CheckPowerCrytal();
        CheckPowerFrament();
        
    }

    private void BtnPowerFragment_Pressed()
    {
        MainPanel.instance.RewardsPopup.Back();
        MainPanel.instance.SummonGatePanel.BtnPowerFragment_Pressed();
    }

    private void BtnPowerFragmentX10_Pressed()
    {
        MainPanel.instance.RewardsPopup.Back();
        MainPanel.instance.SummonGatePanel.BtnPowerFragmentX10_Pressed();
    }

    private void BtnPowerCrytal_Pressed()
    {
        MainPanel.instance.RewardsPopup.Back();
        MainPanel.instance.SummonGatePanel.BtnPowerCrytal_Pressed();
    }

    private void BtnPowerCrytalX10_Pressed()
    {
        MainPanel.instance.RewardsPopup.Back();
        MainPanel.instance.SummonGatePanel.BtnPowerCrytalX10_Pressed();
    }

    private void BtnDevineCrytal_Pressed()
    {
        MainPanel.instance.RewardsPopup.Back();
        MainPanel.instance.SummonGatePanel.BtnDevineCrytal_Pressed();
    }

    private void BtnDevineCrytalX10_Pressed()
    {
        MainPanel.instance.RewardsPopup.Back();
        MainPanel.instance.SummonGatePanel.BtnDevineCrytalX10_Pressed();
    }


    private void CheckPowerFrament()
    {
        if (GameData.Instance.HeroesGroup.FreePowerFragmentSummon)
        {
            imgPowerFragment.SetActive(false);
            txtPowerFragment.SetActive(false);
            txtFreePowerFragment.SetActive(true);
        }
        else
        {
            imgPowerFragment.SetActive(true);
            txtPowerFragment.SetActive(true);
            txtFreePowerFragment.SetActive(false);
        }
    }

    private void CheckPowerCrytal()
    {
        if (GameData.Instance.HeroesGroup.FreePowerCrystalSummon)
        {
            imgPowerCrytal.SetActive(false);
            txtPowerCrytal.SetActive(false);
            txtFreePowerCrytal.SetActive(true);
        }
        else
        {
            if (GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL) <= 0)
            {
                imgPowerCrytal.SetActive(false);
                txtPowerCrytal.SetActive(false);
                imgGem.SetActive(true);
                txtGem.SetActive(true);
                txtFreePowerCrytal.SetActive(false);
            }
            else
            {
                imgPowerCrytal.SetActive(true);
                txtPowerCrytal.SetActive(true);
                imgGem.SetActive(false);
                txtGem.SetActive(false);
                txtFreePowerCrytal.SetActive(false);
            }
        }

        if (GameData.Instance.CurrenciesGroup.GetValue(IDs.CURRENCY_POWER_CRYSTAL) < 10)
        {
            imgPowerCrytalX10.SetActive(false);
            txtPowerCrytalX10.SetActive(false);
            imgGemX10.SetActive(true);
            txtGemX10.SetActive(true);
        }
        else
        {
            imgPowerCrytalX10.SetActive(true);
            txtPowerCrytalX10.SetActive(true);
            imgGemX10.SetActive(false);
            txtGemX10.SetActive(false);
        }
    }
}
