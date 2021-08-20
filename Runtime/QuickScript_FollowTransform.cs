using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickScript_FollowTransform : MonoBehaviour
{
    public Transform m_target;
    public Transform m_anchorPoint;

    void Update()
    {
        if(m_anchorPoint!=null && m_target!=null)
            m_anchorPoint.up = m_target.position-m_anchorPoint.position;
    }
}
