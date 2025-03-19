using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZCameraShake : MonoBehaviour
{
    // How long the object should shake for.
    private float SHAKE_MaxDuration = 0;
    private float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    private float SHAKE_MaxAmount = 0f;
    private float shakeAmount = 0f;
    private float decreaseFactor = 1.0f;

    Vector3 originalPos = Vector3.zero;
    public bool unscaledDeltaTime;
    public void Update()
    {        
        var dt = unscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
        if (shakeDuration > 0 && (Time.timeScale != 0f || unscaledDeltaTime))
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= dt * decreaseFactor;
            shakeAmount = SHAKE_MaxAmount * shakeDuration / SHAKE_MaxDuration;
        }
        else
        {
            shakeDuration = 0f;
            SHAKE_MaxDuration = 0f;
            shakeAmount = 0f;
            SHAKE_MaxAmount = 0f;
            transform.localPosition = originalPos;
        }
    }

    public void SetShakeCamera(float _shakeDuration, float _shakeAmount)
    {
        if(_shakeAmount < SHAKE_MaxAmount) //ranh lol
        {
            return;
        }

//		originalPos = transform.localPosition;
        SHAKE_MaxDuration = _shakeDuration;
        SHAKE_MaxAmount = _shakeAmount;

        shakeDuration = SHAKE_MaxDuration;
        shakeAmount = SHAKE_MaxAmount;
    }

    public void ShakeCameraLarge() {
        SetShakeCamera(0.5f, 0.3f);
    }
    public void ShakeCameraNormal()
    {
        SetShakeCamera(0.4f, 0.2f);
    }
    public void ShakeCameraSmall()
    {
        SetShakeCamera(0.3f, 0.1f);
    }

    public void EndShakeCamera()
    {
        shakeDuration = 0f;
        SHAKE_MaxDuration = 0f;
        shakeAmount = 0f;
        SHAKE_MaxAmount = 0f;
        transform.localPosition = originalPos;
    }
}
