using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBasicMoveForwardMono : MonoBehaviour
{
    public Transform m_linkedTransform;
    public float m_speed=10;

    void Update()
    {
        m_linkedTransform.position += m_linkedTransform.forward * m_speed * Time.deltaTime;
    }
}
