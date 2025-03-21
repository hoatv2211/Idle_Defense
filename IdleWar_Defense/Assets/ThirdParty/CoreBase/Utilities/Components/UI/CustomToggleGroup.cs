﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if USE_DOTWEEN
using DG.Tweening;
#endif

namespace Utilities.Components
{
    public class CustomToggleGroup : ToggleGroup
    {
        public RectTransform dynamicBackground;

        public void SetTarget(RectTransform pTarget, float pTweenDuration = 0)
        {
            var oldPos = dynamicBackground.anchoredPosition;
            var oldSize = dynamicBackground.sizeDelta;

            if (!Application.isPlaying || pTweenDuration == 0)
            {
                dynamicBackground.anchoredPosition = pTarget.anchoredPosition;
                dynamicBackground.sizeDelta = pTarget.sizeDelta;
                return;
            }

#if USE_DOTWEEN
            float lerp = 0;
            DOTween.Kill(GetInstanceID());
            DOTween.To(() => lerp, x => lerp = x, 1f, pTweenDuration)
                .OnUpdate(() =>
                {
                    Vector2 pos = Vector2.Lerp(oldPos, pTarget.anchoredPosition, lerp);
                    dynamicBackground.anchoredPosition = pos;

                    Vector2 size = Vector2.Lerp(oldSize, pTarget.sizeDelta, lerp);
                    dynamicBackground.sizeDelta = size;
                })
                .OnComplete(() =>
                {
                    dynamicBackground.anchoredPosition = pTarget.anchoredPosition;
                    dynamicBackground.sizeDelta = pTarget.sizeDelta;
                })
                .SetEase(Ease.OutCubic)
                .SetId(GetInstanceID());
#else
            dynamicBackground.anchoredPosition = pTarget.anchoredPosition;
            dynamicBackground.sizeDelta = pTarget.sizeDelta;
#endif
        }
    }
}