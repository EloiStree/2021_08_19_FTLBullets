using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickScript_RandomRotating : MonoBehaviour
{

    public Transform m_targetAnchorToRotate;

    public float m_speedFactor = 1;
    public float m_basicAngle = 90;
    public Vector3 m_euleurDirection;
    public Vector3 m_constance;

    void Update()
    {
        m_euleurDirection.x = UnityEngine.Random.Range(-180,180);
        m_euleurDirection.y = UnityEngine.Random.Range(-180, 180);
        m_euleurDirection.z = UnityEngine.Random.Range(-180, 180);

        m_constance= Vector3.Lerp(m_constance, m_euleurDirection, Time.deltaTime);
        m_targetAnchorToRotate.Rotate(m_constance, m_basicAngle * m_speedFactor * Time.deltaTime);
        
    }
    private void Reset()
    {
        m_targetAnchorToRotate = transform;
    }
}
