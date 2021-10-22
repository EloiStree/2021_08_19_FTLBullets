using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeLaserMono : MonoBehaviour
{
    public LaserInfo m_laserInfo;

    public Transform m_debugObject;
    public Transform m_startPoint;

    public Vector3 m_start;
    public Vector3 m_end;

    public LayerMask m_physicLayerMaskOfLaser;



    public bool m_display;

    private void Update()
    {
        if (m_display)
        {

            m_start = m_startPoint.position;
            m_end = m_startPoint.position + m_startPoint.forward;

            Vector3 direction = (m_end - m_start);
            RaycastHit hit;
            if (Physics.Raycast(m_start, direction, out hit, m_laserInfo.m_maxRange, m_physicLayerMaskOfLaser)) 
            {
                m_end = hit.point; 
                direction = (m_end - m_start);
            }
            m_debugObject.position = (m_start + m_end) * 0.5f;
            m_debugObject.forward = direction;
            Transform p = m_debugObject.transform.parent;
            m_debugObject.transform.parent = null;
            m_debugObject.localScale = new Vector3(m_laserInfo.m_radius, m_laserInfo.m_radius, direction.magnitude*0.5f);
            m_debugObject.transform.parent = p;
            m_debugObject.gameObject.SetActive(true);
        }
        else {
            m_debugObject.gameObject.SetActive(false);
        }
    }

}

[System.Serializable]
public struct LaserInfo {
    public float m_radius;
    public float m_maxRange;
}
