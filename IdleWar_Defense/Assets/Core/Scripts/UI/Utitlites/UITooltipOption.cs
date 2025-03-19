using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Components;

namespace FoodZombie.UI
{
    public class UITooltipOption : OptimizedScrollItem
    {
        public UITooltips uiTooltips;
        public SimpleTMPButton btnSelect;
        public TextMeshProUGUI txtDescription;
        public Image imgFragmentIcon;

        public override void UpdateContent(int pIndex)
        {
            if (mIndex == pIndex)
                return;

            base.UpdateContent(pIndex);

            var option = uiTooltips.GetOption(mIndex);
            btnSelect.onClick.RemoveAllListeners();
            btnSelect.onClick.AddListener(() =>
            {
                option.onSelect.Raise();
                uiTooltips.HideOptionsToolTip();
            });
            if (option.value >= 0) btnSelect.labelTMP.text = "x" + option.value;
            else btnSelect.labelTMP.text = "";
            btnSelect.img.sprite = option.imgIcon;
            txtDescription.text = option.content;
            txtDescription.enableWordWrapping = option.enableWordWrapping;

            //hot fix tạm
            if (option.content.Contains("Fragment"))
                imgFragmentIcon.SetActive(true);
            else
                imgFragmentIcon.SetActive(false);

            if (mIndex % 2 == 0)
            {
                btnSelect.targetGraphic.color = new Color(0.05098f, 0.18039f, 0.30980f, 1);
            }
            else
            {
                btnSelect.targetGraphic.color = new Color(0.03529f, 0.13333f, 0.23137f, 1);
            }
        }
    }
}