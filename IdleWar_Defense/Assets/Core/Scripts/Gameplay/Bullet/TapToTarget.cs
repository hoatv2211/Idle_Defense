using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities.Common;

public class TapToTarget : MonoBehaviour
{
    public Transform objectTap;
    public Transform target;
    public ParticleSystem muzzle;
    public NwayShot ShotController;
    public float ShotDam = 10;
    bool function_isActive;
    bool function_canHold;
    //	private float timeHold = 0f;

    private void OnEnable()
    {
        //	timeHold = 0f;
        target.SetActive(true);
    }
    InfoAttacker infoAttacker;
    // Start is called before the first frame update
    void Start()
    {
        if (!GameplayController.Instance.autoPlay) GameplayController.Instance.SetHeroesLookAt(target.position);
        infoAttacker = new InfoAttacker(true,
                      InfoAttacker.TYPE_NORMAL,
                      null,
                      null,
                      null,
                      ShotDam,
                      0,
                      0,
                      0,
                      0f);
        function_isActive = GameUnityData.instance.GameRemoteConfig.Function_TapToShot_Active;
        function_canHold = GameUnityData.instance.GameRemoteConfig.Function_TapToShot_CanHold;
        GameplayController.Instance.SetHeroesLookAt();
        if (!function_isActive)
        {
            target.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            target.gameObject.SetActive(true);
            //if (function_canHold)
            //{ }
            //else { }
        }
    }
    void LookAt(Vector3 targetPos)
    {
        Vector3 diff = targetPos - target.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        target.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }
    public void SetTarget(Vector3 position)
    {
        if (!ShotController.canShot()) return;
        target.position = position;

        //GameplayController.Instance.SetHeroesLookAt(target.position);
        //	if (ShotController.m_targetTransform == null || !ShotController.m_targetTransform.gameObject.activeSelf)
        {
            EnemyControl targetEnemy = GameplayController.Instance.GetPrioritizeEnemy(null, 1);
            if (targetEnemy != null)
            {
                //ShotController.m_targetTransform = targetEnemy.transform;
                LookAt(targetEnemy.transform.position + (Vector3)targetEnemy.Offset);
                //transform.LookAt2D(targetEnemy.transform);

                ShotController.Shot(infoAttacker);
                muzzle.Play(true);

            }
            else
            {
                LookAt(Vector3.up * 5.0f);
                {
                    ShotController.Shot(infoAttacker);
                    muzzle.Play(true);
                }
            }
        }
        //this.transform.position = position;


    }

    bool _isActive = true;
    public void SetActive(bool isActive)
    {
        this._isActive = isActive;
    }
    Touch _cache = new Touch();
    bool IsTouch(Touch item)
    {
        // Input.touches
        if (!function_isActive) return false;
#if UNITY_EDITOR
        if (function_canHold)
            return (Input.GetMouseButton(0));
        else
            return (Input.GetMouseButtonDown(0));
#else
  if (function_canHold)
            return (item.phase == TouchPhase.Began || item.phase == TouchPhase.Moved || item.phase == TouchPhase.Stationary);
        else
            return (item.phase == TouchPhase.Began);
#endif
        return false;
    }
    // Update is called once per frame
    private void Update()
    {
        if (!function_isActive) return;
        if (Time.timeScale == 0) return;
        if (!this._isActive) return;
#if UNITY_EDITOR
        if (EventSystem.current == null || EventSystem.current.IsPointerOverGameObject(-1))    // is the touch on the GUI
        {
            // GUI Action
            return;
        }

        if (IsTouch(_cache))
        {
            Ray ray = GameplayController.Instance.camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == objectTap)
                {
                    var pos = GameplayController.Instance.camera.ScreenToWorldPoint(Input.mousePosition);
                    //target.position = new Vector3(pos.x, pos.y, target.position.z);
                    SetTarget(new Vector3(pos.x, pos.y, target.position.z));
                    //timeHold = 1.5f;
                    //if (!target.gameObject.activeSelf) target.SetActive(true);
                }
            }
        }
#else
        if (EventSystem.current == null || EventSystem.current.IsPointerOverGameObject(0))    // is the touch on the GUI
        {
            // GUI Action
            return;
        }

        if (Input.touchCount > 0)
        {
            foreach (var item in Input.touches)
            {
                if (IsTouch(item)) {
                    Ray ray = GameplayController.Instance.camera.ScreenPointToRay(item.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform == objectTap)
                        {
                            var pos = GameplayController.Instance.camera.ScreenToWorldPoint(item.position);
                         //   target.position = new Vector3(pos.x, pos.y, target.position.z);
SetTarget(new Vector3(pos.x, pos.y, target.position.z));
                         //   timeHold = 1.5f;
                          //  if(!target.gameObject.activeSelf) target.SetActive(true);
                            break;
                        }
                    }
                }
            }
        }
#endif
        //sửa lại là alway auto
        //GameplayController.Instance.SetHeroesLookAt();

        //timeHold -= Time.deltaTime;
        //if (timeHold <= 0f)
        //{
        //	timeHold = 0f;
        //	if (target.gameObject.activeSelf) target.SetActive(false);
        //GameplayController.Instance.SetHeroesLookAt();
        //}
        //else
        //{
        //	if (!target.gameObject.activeSelf) target.SetActive(true);
        //	GameplayController.Instance.SetHeroesLookAt(target.position);
        //}
    }
}
