using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BarrierTutorial : MonoBehaviour
{
    public Image imgBarrier;
    public Vector2 targetPos;
    
    // Start is called before the first frame update
    void Start()
    {
        imgBarrier.DOFade(0.8f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            
        var sequence = DOTween.Sequence();
        sequence.SetUpdate(true);
        sequence.SetDelay(0.5f);
        sequence.Append(transform.DOLocalMove(new Vector3(targetPos.x, targetPos.y, 0f), 1f));
        //wait 0.5f
        sequence.Append(transform.DOLocalMove(new Vector3(targetPos.x, targetPos.y, 0f), 0.5f));
        sequence.SetLoops(-1, LoopType.Restart);
        sequence.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
