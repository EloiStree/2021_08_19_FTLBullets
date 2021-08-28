using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public struct TransformCapsuleLine : ICapsuleLine
{
    public Transform m_start;
    public Transform m_end;
    public float m_radius;
    public void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition)
    {
        startPointPosition = m_start.position;
        endPointPosition = m_end.position;
    }

    public void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius)
    {
        startPointPosition = m_start.position;
        endPointPosition = m_end.position;
        radius = m_radius;
    }

    public void GetLineRadius(out float radius)
    {
        radius = m_radius;
    }


}