using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct S_CapsuleLine : IStructCapsuleLine
{

    public int m_id;
    public Vector3 m_start;
    public Vector3 m_end;
    public float m_radius;

    public void GetIntId(out int id)
    {
        id = m_id;
    }

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

    public void SetWith(ICapsuleLine target)
    {
        target.GetLineAsPointsWithRadius(out m_start, out m_end, out m_radius);
    }
}

