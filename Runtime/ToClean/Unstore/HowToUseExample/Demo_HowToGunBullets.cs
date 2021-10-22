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
    public float m_radiusOrBullet;
    public float m_randomnessAtFiringAngle = 0.1f;
    
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

        Quaternion randomnes = Quaternion.Euler(
            UnityEngine.Random.Range(-m_randomnessAtFiringAngle, m_randomnessAtFiringAngle),
            UnityEngine.Random.Range(-m_randomnessAtFiringAngle, m_randomnessAtFiringAngle),
            0);
        if(m_bulletManager!=null)
            m_bulletManager.RequestBulletFire(m_gunSpawnPoint.position,
                randomnes*m_gunSpawnPoint.forward, m_gunSpawnPoint.rotation,
                m_bulletSpeed, m_bulletLifeTimeWanted, m_radiusOrBullet,
            out IBulletIdTicket bulletTicket);
    }

  
}
