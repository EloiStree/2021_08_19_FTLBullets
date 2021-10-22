using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeMissileMono : MonoBehaviour
{

    public AccelerationMissileInfo m_missileInfo;
    public Transform m_missileToAffect;
    public Transform m_missileTarget;
    public Vector3 m_lastTargetPosition;
    public MissileMovingStateInfo m_stateInfo;

    private void Awake()
    {
        m_stateInfo.m_currentSpeed = m_missileInfo.m_initialSpeed;
    }
    public float m_angle;
    public float m_maxAngle;
    public float m_pourcent;
    public void Update()
    {
        if (m_missileTarget!=null) {

            Quaternion forward = m_missileToAffect.rotation;
            Quaternion target = Quaternion.LookRotation(m_missileTarget.position - m_missileToAffect.position, Vector3.up);

            // Quaternion rot = target * Quaternion.Inverse(forward);
            float td = Time.deltaTime;
            float angle = Quaternion.Angle(target, forward) *td;
            float maxAngle =  m_missileInfo.m_maxAngleRotation * td ;
            float pct = 0;
            if (angle > maxAngle) {
                pct = maxAngle / angle;
                m_missileToAffect.rotation = Quaternion.Lerp(forward, target, pct);
            }
            else {
                m_missileToAffect.rotation = target;
            }

            m_angle = angle;
            m_maxAngle = maxAngle;
            m_pourcent = pct;



        }


        if (m_missileInfo.m_acceleration != 0f) {
            if (m_missileInfo.m_acceleration > 0f && m_stateInfo.m_currentSpeed < m_missileInfo.m_maxSpeed)
            {
                m_stateInfo.m_currentSpeed += m_missileInfo.m_acceleration * Time.deltaTime ;
                if (m_stateInfo.m_currentSpeed > m_missileInfo.m_maxSpeed)
                {
                    m_stateInfo.m_currentSpeed = m_missileInfo.m_maxSpeed;
                }
            }
            else if (m_missileInfo.m_acceleration < 0f && m_stateInfo.m_currentSpeed > m_missileInfo.m_minSpeed) {
                m_stateInfo.m_currentSpeed += m_missileInfo.m_acceleration * Time.deltaTime;
                if (m_stateInfo.m_currentSpeed < m_missileInfo.m_minSpeed)
                {
                    m_stateInfo.m_currentSpeed = m_missileInfo.m_minSpeed;
                }
            }

           
        }
        m_missileToAffect.position += m_missileToAffect.forward * (m_stateInfo.m_currentSpeed * Time.deltaTime);

    }

}

[System.Serializable]
public struct MissileMovingStateInfo {

    public Vector3 m_currentDirection;
    public Vector3 m_currentPosition;
    public float m_currentSpeed;
}


[System.Serializable]
public struct AccelerationMissileInfo {

    public float m_initialSpeed;
    public float m_acceleration;
    public float m_minSpeed;
    public float m_maxSpeed;

    public float m_maxAngleRotation;
}
