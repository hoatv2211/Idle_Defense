using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class AutoPlayTutorial : MonoBehaviour
{
    public RectTransform autoplayFake, autoplayReal;
    public float doTime;
    // Start is called before the first frame update

    private void OnEnable()
    {
        autoplayReal.gameObject.SetActive(false);
        autoplayFake.DOMove(autoplayReal.position, doTime).OnComplete(() =>
        {
            autoplayReal.gameObject.SetActive(true);
        }).SetUpdate(true).SetDelay(1);
        autoplayFake.transform.localScale = 2 * Vector3.one;
        autoplayFake.transform.DOScale(Vector3.one, doTime).SetUpdate(true).SetDelay(1);
    }

}
