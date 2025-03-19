using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager of Bullet
/// </summary>
public class BulletManager : UbhSingletonMonoBehavior<BulletManager>
{
    private List<Bullet> m_bulletList = new List<Bullet>(2048);

    protected override void Awake()
    {
        base.Awake();

        // Create Timer
        if (BulletTimer.instance == null) { }
    }

    /// <summary>
    /// Update Bullets Move
    /// </summary>
    public void UpdateBullets()
    {
        for (int i = 0; i < m_bulletList.Count; i++)
        {
            if (m_bulletList[i] == null)
            {
                continue;
            }
            m_bulletList[i].UpdateMove();
        }
    }

    /// <summary>
    /// Add bullet
    /// </summary>
    public void AddBullet(Bullet bullet)
    {
        if (m_bulletList.Contains(bullet))
        {
            Debug.LogWarning("This bullet is already added in m_bulletList.");
            return;
        }

        for (int i = 0; i < m_bulletList.Count; i++)
        {
            if (m_bulletList[i] == null)
            {
                m_bulletList[i] = bullet;
                return;
            }
        }

        m_bulletList.Add(bullet);
    }

    /// <summary>
    /// Remove bullet
    /// </summary>
    public void RemoveBullet(Bullet bullet)
    {
        int index = m_bulletList.IndexOf(bullet);
        if (index < 0)
        {
            Debug.LogWarning("This bullet is not found in m_bulletList.");
            return;
        }
        m_bulletList[index] = null;
    }
}