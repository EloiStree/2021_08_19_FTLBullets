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


    void Start()
    {
        Invoke("Explosion", 2);
    }
    public void Explosion() {
        Vector3 start = m_sourceOfExplosion.position;
        for (int i = 0; i < m_existingTransformInScene.Length; i++)
        {
            Vector3 direction = GetRandomDirection(m_minSpeed, m_maxSpeed);
            m_existingTransformInScene[i].forward = direction;
            m_bulletJobSystem.SpawnBullet(start, direction, m_existingTransformInScene[i]);
        }
    }
    private Vector3 GetRandomDirection(float minSpeed,float maxSpeed)
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) *
            UnityEngine.Random.Range(minSpeed, maxSpeed);
    }

}
