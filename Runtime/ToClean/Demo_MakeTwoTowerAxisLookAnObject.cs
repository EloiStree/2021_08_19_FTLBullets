using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_MakeTwoTowerAxisLookAnObject : MonoBehaviour
{
    public TowerAxisMovementMono m_horizontal;
    public TowerAxisMovementMono m_vertical;
    public Transform m_target;
     Transform m_targetPrevious;

    public float m_notTargetHorizontal;
    public float m_notTargetVertical;

 
    void Update()
    {
        if (m_target != m_targetPrevious) {
            if (m_target == null)
            {
                m_horizontal.SetRequestRotationFrom(m_notTargetHorizontal);
                m_vertical.SetRequestRotationFrom(m_notTargetVertical);
            }
        }
        if (m_target != null)
        {

            m_horizontal.SetRequestRotationFrom(m_target.position);
            m_vertical.SetRequestRotationFrom(m_target.position);
        }
        m_targetPrevious = m_target;
    }
}
