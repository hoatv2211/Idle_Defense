using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Common;
using DG.Tweening;

using Utilities.Components;
using UnityEngine.Events;

namespace FoodZombie.UI
{
    public class UIEffectItem : MonoBehaviour
    {
        [SerializeField] protected RectTransform mRectTransform;
        [SerializeField] protected Image mImgIcon;
        [SerializeField] protected TextMeshProUGUI mTxtValue;

        public void Init(Sprite icon, int value, RectTransform pFrom, RectTransform pTo, float time, UnityAction<UIEffectItem> Release)
        {
            mImgIcon.color = Color.white;
            mTxtValue.color = Color.white;

            mImgIcon.sprite = icon;
            mTxtValue.text = "" + value;
            mRectTransform.position = pFrom.position;

            Tweener tweener;
            if (pTo != null)
            {
                tweener = mRectTransform.DOMove(pTo.position, time * 0.4f);
            }
            else
            {
                tweener = mRectTransform.DOMoveY(pFrom.position.y + 1f, time * 0.4f);
            }
            tweener.SetDelay(time * 0.5f);

            Color c = new Color(1f, 1f, 1f, 0f);
            tweener = mTxtValue.DOColor(c, time * 0.7f);
            tweener.SetDelay(time * 0.3f);
            tweener = mImgIcon.DOColor(c, time * 0.7f);
            tweener.SetDelay(time * 0.3f);
            tweener.OnComplete(() =>
            {
                Release(this);
            });
        }

        public void InitRandom(Sprite icon, int value, RectTransform pFrom, RectTransform pTo, float time, UnityAction<UIEffectItem> Release)
        {
            mImgIcon.color = Color.white;
            mTxtValue.color = Color.white;

            mImgIcon.sprite = icon;
            mTxtValue.text = "" + value;
            mRectTransform.position = pFrom.position;

            Tweener tweener = mRectTransform.DOMoveX(Mathf.Lerp(pFrom.position.x, pTo.position.x, 0.1f), time * 0.3f);
            tweener.SetEase(Ease.InFlash);
            tweener = mRectTransform.DOMoveY(pFrom.position.y + Random.Range(-1.0f, -0.55f), time * 0.3f);
            tweener.SetEase(Ease.InExpo);
            if (pTo != null)
            {
                tweener = mRectTransform.DOMove(pTo.position, time * 0.65f);
            }
            else
            {
                tweener = mRectTransform.DOMoveY(pFrom.position.y + 1f, time * 0.65f);
            }

            tweener.SetDelay(time * 0.35f);
            tweener.SetEase(Ease.InExpo);

            Color c = new Color(1f, 1f, 1f, 0.5f);
            tweener = mTxtValue.DOColor(c, time * 0.5f);
            tweener.SetDelay(time * 0.5f);
            tweener = mImgIcon.DOColor(c, time * 0.5f);
            tweener.SetDelay(time * 0.5f);
            tweener.OnComplete(() =>
            {
                SimpleLeanFX.instance.Bubble(pTo, 0.25f);
                //foreach (var item in pTo.GetComponentsInChildren<Image>())
                //{
                //    SimpleLeanFX.instance.SingleHightLight(item, 0.25f);
                //}
                Release(this);
            });
        }
    }
}