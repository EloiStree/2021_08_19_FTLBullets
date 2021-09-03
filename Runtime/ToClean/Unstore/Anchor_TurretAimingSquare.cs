using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor_TurretAimingSquare : MonoBehaviour
{

    public Transform m_cannonOrientation;
    public Transform m_cannonAnchor;
    public Transform m_cannonBulletStart;

    public float m_rotationSpeedHorizontalAngle = 40;
    public float m_maxHorizontalAngle = 80;

    public float m_rotationSpeedVerticalAngle = 10;
    public float m_maxVerticalAngle = 20;



    public Transform m_target;

    private Quaternion m_wantedRotation;
    public Vector3 m_euleur;

    void Update()
    {

        Vector3 direction = m_target.position - m_cannonAnchor.position;
        Quaternion rotationForward = Quaternion.LookRotation(m_cannonOrientation.forward, m_cannonOrientation.up);
        Quaternion rotationTarget = Quaternion.LookRotation(direction, m_cannonOrientation.up);
        Quaternion localRotation = rotationForward * Quaternion.Inverse(rotationTarget);
        m_euleur = localRotation.eulerAngles;


        //m_cannonAnchor.rotation = Quaternion.Lerp(m_cannonAnchor.rotation , m_wantedRotation, Time.deltaTime);
    }

    void ClampCameraAngle(ref Quaternion targetRotation, Quaternion initialRotation)
    {
        float rotPosX = Mathf.Clamp(targetRotation.eulerAngles.x, initialRotation.x - m_maxHorizontalAngle, initialRotation.x + m_maxHorizontalAngle);
        float rotPosY = Mathf.Clamp(targetRotation.eulerAngles.y, initialRotation.y - m_maxVerticalAngle, initialRotation.y + m_maxVerticalAngle);
        float rotPosZ = Mathf.Clamp(targetRotation.eulerAngles.z, initialRotation.z - 0, initialRotation.z + 0);

        targetRotation = Quaternion.Euler(new Vector3(rotPosX, rotPosY, rotPosZ));
    }
}
