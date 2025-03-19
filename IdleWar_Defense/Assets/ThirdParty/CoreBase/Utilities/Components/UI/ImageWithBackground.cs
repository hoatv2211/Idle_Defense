#pragma warning disable 0649

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using Utilities.Inspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utilities.Components
{
    [AddComponentMenu("Utitlies/UI/ImageWithBackground")]
    public class ImageWithBackground : ImageWithText
    {
        [SerializeField] private Image mImgBackground;

#if UNITY_EDITOR
        protected override void Validate()
        {
            if (mTxt == null)
                mTxt = GetComponentInChildren<TextMeshProUGUI>();
            if (mImg == null || mImgBackground == null)
            {
                var images = GetComponentsInChildren<Image>();
                mImgBackground = images.Length > 0 ? images[0] : null;
                mImg = images.Length > 1 ? images[1] : images[0];
            }
        }
#endif
    }
}
