using FoodZombie;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPOW : MonoBehaviour
{
    public float Time_del = 15;
    public float Time_Active = 5;
    [SerializeField]
    SpriteRenderer sr;
    [SerializeField]
    GameObject hintObject;
    private float _tempTimeDel = 0;

    // Update is called once per frame
    void Update()
    {
        if (_tempTimeDel > 0)
        {
            _tempTimeDel -= Time.unscaledDeltaTime;
            if (_tempTimeDel <= 3)
                sr.enabled = ((int)(_tempTimeDel * 10)) % 2 == 0;
            if (_tempTimeDel <= 0)
            {
                SimplePool.Despawn(gameObject);
            }
        }
    }

    private void OnEnable()
    {
        sr.enabled = true;
        _tempTimeDel = Time_del;
        hintObject.SetActive(!GameData.Instance.TutorialsGroup.TutDone_POW);
    }

    private void OnMouseDown()
    {
        GameData.Instance.TutorialsGroup.TutDone_POW = true;
        SimplePool.Despawn(gameObject);
        GameplayController.Instance.UseBooster(Time_Active);
    }
}
