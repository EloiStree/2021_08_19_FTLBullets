using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_HowToGunBullets : MonoBehaviour
{

    public Transform m_gunSpawnPoint;
    public AbstractBulletsManager m_bulletManager;
    public float m_timeBetweenFireSeconds = 0.2f;
    public float m_bulletLifeTimeWanted=20;
    public float m_bulletSpeed=500;
    
    
    IEnumerator Start()
    {

        while (true) {

            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(m_timeBetweenFireSeconds);
            Fire();
        }

    }

    private void Fire()
    {

        m_bulletManager.RequestBulletFire(m_gunSpawnPoint.position, m_gunSpawnPoint.forward, m_bulletSpeed, m_bulletLifeTimeWanted,
            out IBulletIdTicket bulletTicket);
    }

  
}
