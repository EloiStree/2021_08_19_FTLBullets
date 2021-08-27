using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletableTagMono : MonoBehaviour
{

    public CapsuleMonoDebug m_capsuleDebug;


    void Update()
    {
        m_capsuleDebug.Refresh();
       
        
    }



    public void GetWorldPoint(out Vector3 previous, out Vector3 current, out float radius)
    {
        previous = m_capsuleDebug.m_previousPoint.position;
        current = m_capsuleDebug.m_currentPoint.position;
        radius = m_capsuleDebug.m_radius;
    }
    public void GetWorldPoint(out Vector3 previousExtremity , out Vector3 previous, out Vector3 current , out Vector3 currentExtremity)
    {
        GetWorldPoint(out previous, out current, out float radius);
        Vector3 direction = (current-previous).normalized;
        currentExtremity = current + direction * radius;
        previousExtremity = previous + -direction * radius;
    }
}

[System.Serializable]
public class CapsuleMonoDebug {
    public float m_radius;
    public Transform m_previousPoint;
    public Transform m_currentPoint;

    public Transform m_capsuleTransform;
    public CapsuleCollider m_debugCollider;


    public void Refresh()
    {
        if (m_capsuleTransform != null && m_debugCollider != null)
        {
            Vector3 direction = m_currentPoint.position-m_previousPoint.position ;
            m_capsuleTransform.position = (m_previousPoint.position + m_currentPoint.position) / 2f;
            m_capsuleTransform.forward = direction.normalized;
            m_debugCollider.height = direction.magnitude;
            m_debugCollider.radius = m_radius;
        }

    }
}