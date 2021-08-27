using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment_RaycastPerfTest : MonoBehaviour
{
    public uint     m_raycastCountPerFrame=5000;
    public LayerMask m_mask;
    public Vector3  m_start;
    public Vector3  m_direction;
    public float    m_distance=10;
    public QueryTriggerInteraction m_raycastTrigger;

    public DateTime m_tStart;
    public DateTime m_tStop;
    public double m_millisecondsPast;

    void Update()
    {
        m_tStart = DateTime.Now;

        for (int i = 0; i < m_raycastCountPerFrame; i++)
        {
            //Physics.Raycast(m_start, m_direction, m_distance, m_mask, m_raycastTrigger);
            Physics.RaycastAll(m_start, m_direction, m_distance, m_mask, m_raycastTrigger);

        }

        m_tStop = DateTime.Now;
        m_millisecondsPast = (m_tStop - m_tStart).Milliseconds;

    }
}
