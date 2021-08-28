using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment_BulletMathFilter : MonoBehaviour
{

    public Transform m_startPointA;
    public Transform m_endPointA;
    public float m_radiusA = 0.1f;

    public Transform m_startPointB;
    public Transform m_endPointB;
    public float m_radiusB = 0.1f;

    [Header("Debug")]
    public CapsuleLineCollisionUtility.CapsuleReachCheckDebugInfo m_debug;
    public CapsuleLineCollisionUtility.ReachFiltering m_reachFilter;
    public bool m_useDebugDraw;
    private void Update()
    {
        CapsuleLineCollisionUtility.CheckReachabilityOfTwoCapsules(
                 m_startPointA, m_endPointA, m_radiusA,
                m_startPointB, m_endPointB, m_radiusB,
            out m_reachFilter, ref  m_debug, m_useDebugDraw);
    }
}
