using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class ListGearsPanel : MyGamesBasePanel
    {
        public SimpleTMPButton btnChoice;
        
        public Transform transformGearsPool;
        [SerializeField, Tooltip("Buildin Pool")] private List<GearView> gearViewsPool;

        private GearData choiceGearData;
        private UnityAction<GearData> choiceAction;
        
        private void Start()
        {
            btnChoice.onClick.AddListener(BtnChoice_Pressed);
        }

        public void Init(List<GearData> gearDatas, UnityAction<GearData> _choiceAction)
        {
            choiceAction = _choiceAction;
            
            gearViewsPool.Free();
            var count = gearDatas.Count;
            for (int i = 0; i < count; i++)
            {
                var unEquipGear = gearViewsPool.Obtain(transformGearsPool);
                unEquipGear.Init(gearDatas[i], ChoiceGear);
                unEquipGear.SetActive(true);
                unEquipGear.UnChoice();
            }
            
            btnChoice.SetEnable(false);
        }
        
        private void ChoiceGear(GearView gearView)
        {
            var count = gearViewsPool.Count;
            for (int i = 0; i < count; i++)
            {
                gearViewsPool[i].UnChoice();
            }
            
            gearView.Choice();
            choiceGearData = gearView.gearData;
            btnChoice.SetEnable(true);
        }
        
        private void BtnChoice_Pressed()
        {
            Back();

            choiceAction(choiceGearData);
        }
    }
}
