using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Common;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float delayTime = 5f;
    public bool uscaledTime;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(IEDestroy());
    }

    IEnumerator IEDestroy() {
        if (uscaledTime)
        {
            yield return CoroutineUtil.WaitForRealSeconds(delayTime);
        }
        else
        {
            yield return new WaitForSeconds(delayTime);
        }
        gameObject.SetActive(false);
    }
}
