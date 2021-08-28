using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBasicDestroyLifeTime : MonoBehaviour
{

    public GameObject m_objectToDestroy;
    public float m_lifeTime = 9;
    public float m_countingDown=9;
    void Start()
    {
        m_countingDown = m_lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        m_countingDown -= Time.deltaTime;
        if (m_countingDown < 0) {
            
            Destroy(this.m_objectToDestroy);
        }


    }
    private void Reset()
    {
        m_objectToDestroy = this.gameObject;
    }
}
