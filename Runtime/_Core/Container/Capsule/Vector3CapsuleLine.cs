using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Vector3CapsuleLine : ICapsuleLine
{
    public Vector3 m_start;
    public Vector3 m_end;
    public float m_radius;

    public void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition)
    {
        startPointPosition = m_start;
        endPointPosition = m_end;
    }

    public void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius)
    {
        startPointPosition = m_start;
        endPointPosition = m_end;
        radius = m_radius;
    }

    public void GetLineRadius(out float radius)
    {
        radius = m_radius;
    }


    public void SetWithWorldPositionOf(TransformCapsuleLine source)
    {
        m_start = source.m_start.position;
        m_end = source.m_end.position;
        m_radius = source.m_radius;

    }
}