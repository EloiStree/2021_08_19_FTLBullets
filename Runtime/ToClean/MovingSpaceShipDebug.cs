using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpaceShipDebug : MonoBehaviour
{

    public float m_straffeMultiplicator=2;
    public float m_horizontalStraffe = 1;
    public float m_horizontalStraffeMultiplicator = 10;

    public float m_forwardStraffe = 1;
    public float m_forwardStraffeMultiplicator = 10;

    public float m_upStraffe = 1;
    public float m_upStraffeMultiplicator = 10;


    public float m_zRotation = 90;
    public float m_xRotation = 90;
    public float m_yRotation = 90;


    public Transform m_toAffect;

    void Update()
    {

        if (Input.GetKey(KeyCode.Keypad6))
            m_toAffect.position += Time.deltaTime * (m_horizontalStraffe * m_horizontalStraffeMultiplicator * m_straffeMultiplicator) * m_toAffect.right;
        if (Input.GetKey(KeyCode.Keypad4))
            m_toAffect.position -= Time.deltaTime * (m_horizontalStraffe * m_horizontalStraffeMultiplicator * m_straffeMultiplicator) * m_toAffect.right;
        if (Input.GetKey(KeyCode.Keypad8))
            m_toAffect.position += Time.deltaTime * (m_forwardStraffe * m_forwardStraffeMultiplicator * m_straffeMultiplicator) * m_toAffect.forward;
        if (Input.GetKey(KeyCode.Keypad5))
            m_toAffect.position -= Time.deltaTime * (m_forwardStraffe * m_forwardStraffeMultiplicator * m_straffeMultiplicator) * m_toAffect.forward;
        if (Input.GetKey(KeyCode.KeypadMinus))
            m_toAffect.position += Time.deltaTime * (m_upStraffe * m_upStraffeMultiplicator * m_straffeMultiplicator) * m_toAffect.up;
        if (Input.GetKey(KeyCode.KeypadPlus))
            m_toAffect.position -= Time.deltaTime * (m_upStraffe * m_upStraffeMultiplicator * m_straffeMultiplicator) * m_toAffect.up;


        if (Input.GetKey(KeyCode.Keypad7))
            m_toAffect.Rotate(0, 0, m_zRotation * Time.deltaTime);
        if (Input.GetKey(KeyCode.KeypadDivide))
            m_toAffect.Rotate(0, 0, -m_zRotation * Time.deltaTime);

        if (Input.GetKey(KeyCode.KeypadMultiply))
            m_toAffect.Rotate( -m_xRotation * Time.deltaTime,0,0);
        if (Input.GetKey(KeyCode.Keypad9))
            m_toAffect.Rotate( m_xRotation * Time.deltaTime, 0, 0);

        if (Input.GetKey(KeyCode.Keypad1))
            m_toAffect.Rotate(0, -m_yRotation * Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.Keypad3))
            m_toAffect.Rotate(0, m_yRotation * Time.deltaTime, 0);




    }
}
