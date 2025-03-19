using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FoodZombie;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.Components;

public class DiscoveryView : MonoBehaviour
{
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtStatus;
    public TextMeshProUGUI txtInfo;
    public SkeletonGraphic model;
    public DOTweenAnimation[] tweenAnimations;

    public SimpleTMPButton btnOpen;
    public GameObject imgShadow;
    
    private DiscoveryData discoveryData;
    private UnityAction<DiscoveryData> refreshDiscoveryLevel;

    private void Start()
    {
        btnOpen.onClick.AddListener(BtnOpen_Pressed);
    }

    public void Init(DiscoveryData _discoveryData, UnityAction<DiscoveryData> _refreshDiscoveryLevel, int dayOfWeek)
    {
        discoveryData = _discoveryData;
        refreshDiscoveryLevel = _refreshDiscoveryLevel;
        txtName.text = discoveryData.NameLocal;

        var daysOpen = discoveryData.DaysOpen;
        if (daysOpen.Contains(dayOfWeek))
        {
            txtStatus.text = Localization.Get(Localization.ID.OPEN);
            txtInfo.text = /*Localization.Get(Localization.ID.TIME)+" " + */(discoveryData.LimitInDay - discoveryData.CountClaimInDay) + "/" + discoveryData.LimitInDay;
            
            btnOpen.enabled = true;
            imgShadow.SetActive(false);
            if(model != null) model.timeScale = 1f;
            for (int i = 0; i < tweenAnimations.Length; i++)
            {
                tweenAnimations[i].DOPlay();
            }
        }
        else
        {
            txtStatus.text = Localization.Get(Localization.ID.CLOSE);
            var s = Localization.Get(Localization.ID.OPEN_ON) + " ";
            var count = daysOpen.Length;
            for (int i = 0; i < count; i++)
            {
                var day = daysOpen[i];
                switch (day)
                {
                    case 2: s += Localization.Get(Localization.ID.MON); break;
                    case 3: s += Localization.Get(Localization.ID.TUE); break;
                    case 4: s += Localization.Get(Localization.ID.WED); break;
                    case 5: s += Localization.Get(Localization.ID.THU); break;
                    case 6: s += Localization.Get(Localization.ID.FRI); break;
                    case 7: s += Localization.Get(Localization.ID.SAT); break;
                    default: s += Localization.Get(Localization.ID.SUN); break;
                }

                if (i < count - 1) s += ", ";
            }
            txtInfo.text = s;
            
            btnOpen.enabled = false;
            imgShadow.SetActive(true);
            if (model != null)
            {
                model.Update(1f);
                model.timeScale = 0f;
            }
            for (int i = 0; i < tweenAnimations.Length; i++)
            {
                tweenAnimations[i].DOPause();
            }
        }
    }

    private void BtnOpen_Pressed()
    {
        if(refreshDiscoveryLevel != null) refreshDiscoveryLevel(discoveryData);
    }
}
