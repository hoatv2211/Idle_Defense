﻿using UnityEngine;
using UnityEngine.Serialization;

public class UbhSetting : UbhMonoBehaviour
{
    [Range(0, 2), FormerlySerializedAs("_VsyncCount")]
    public int m_vsyncCount = 1;
    [Range(0, 60), FormerlySerializedAs("_FrameRate")]
    public int m_frameRate = 60;

    private void Start()
    {
        SetValue();
    }

    private void OnValidate()
    {
        SetValue();
    }

    private void Update()
    {
        if (UbhUtil.IsMobilePlatform() && Input.GetKey(KeyCode.Escape))
        {
            GameExtension.QuitGame();
        }
    }

    private void SetValue()
    {
        m_vsyncCount = Mathf.Clamp(m_vsyncCount, 0, 2);
        // QualitySettings.vSyncCount = m_vsyncCount;

        m_frameRate = Mathf.Clamp(m_frameRate, 1, 120);
        // Application.targetFrameRate = m_frameRate;
    }
}