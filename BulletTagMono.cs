using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTagMono : MonoBehaviour
{

    public CapsuleMonoDebug m_capsuleDebug;



    public void GetWorldPoint(out Vector3 previous, out Vector3 current)
    {
        previous = m_capsuleDebug.m_previousPoint.position;
        current = m_capsuleDebug.m_currentPoint.position;
    }
}
