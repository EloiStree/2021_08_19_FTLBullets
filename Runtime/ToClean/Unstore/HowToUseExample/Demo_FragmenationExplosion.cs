using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_FragmenationExplosion : MonoBehaviour
{

    public Transform m_sourceOfExplosion;
    public Transform[] m_existingTransformInScene;
    public BulletJobSystemFacadeMono m_bulletJobSystem;
    public float m_minSpeed=0.5f;
    public float m_maxSpeed=3f;


    public uint m_numberToUse=15000;
    public GameObject m_objectToUse;

    void Start()
    {
        m_existingTransformInScene = new Transform[m_numberToUse];
        if(m_objectToUse!=null)
        {
           
            for (int i = 0; i < m_numberToUse; i++)
            {
                GameObject a =
                    GameObject.Instantiate(m_objectToUse);
                a.SetActive(true);
                m_existingTransformInScene[i] = a.transform;
                a.transform.parent = m_sourceOfExplosion;
            }
        }

        Invoke("Explosion",  2);
        InvokeRepeating("ReExplosion", 10, 10);
    }
    public void Explosion()
    {
        Vector3 start = m_sourceOfExplosion.position;
        for (int i = 0; i < m_existingTransformInScene.Length; i++)
        {
            Vector3 direction = GetRandomDirection(m_minSpeed, m_maxSpeed);
            if (m_existingTransformInScene[i] != null) { 
                    m_existingTransformInScene[i].forward = direction;
            }
            m_bulletJobSystem.SpawnBullet(start, direction, m_existingTransformInScene[i], out IBulletIdTicket ticket);
        }
    }
    public void ReExplosion()
    {
        Vector3 start = m_sourceOfExplosion.position;
        for (int i = 0; i < m_existingTransformInScene.Length; i++)
        {
            Vector3 direction = GetRandomDirection(m_minSpeed, m_maxSpeed);
            if (m_existingTransformInScene[i] != null)
            {
                    m_existingTransformInScene[i].forward = direction;
            }
            m_bulletJobSystem.ForceSetBullet(i, start, direction, m_existingTransformInScene[i]);
        }
    }
    private Vector3 GetRandomDirection(float minSpeed,float maxSpeed)
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) *
            UnityEngine.Random.Range(minSpeed, maxSpeed);
    }

}
