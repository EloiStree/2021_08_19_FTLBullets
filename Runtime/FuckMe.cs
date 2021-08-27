using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuckMe : MonoBehaviour
{
    public Transform m_direction;
    public Vector3 m_euleurRotation;
    public Vector3 m_positionLocal;

    void Update()
    {
        DrawLineAndSQuare();
    }

    private void DrawLineAndSQuare()
    {
        Quaternion botLeft = Quaternion.LookRotation(new Vector3(-1, -1, 0), Vector3.up);
        Quaternion botRight = Quaternion.LookRotation(new Vector3(1, -1, 0), Vector3.up);
        Quaternion topleft = Quaternion.LookRotation(new Vector3(-1, 1, 0), Vector3.up);
        Quaternion topRight = Quaternion.LookRotation(new Vector3(1, 1, 0), Vector3.up);

        Quaternion dirAsQuad = Quaternion.LookRotation(m_direction.forward, Vector3.up);


        Debug.DrawLine(m_direction.position, m_direction.position +  m_direction.forward, Color.green,1);
        Debug.DrawLine(m_direction.position, m_direction.position + ((dirAsQuad*topleft) * Vector3.forward), Color.red, 1);
        Debug.DrawLine(m_direction.position, m_direction.position + ((dirAsQuad * topRight) * Vector3.forward), Color.red, 1);
        Debug.DrawLine(m_direction.position, m_direction.position + ((dirAsQuad * botRight) * Vector3.forward), Color.red, 1);
        Debug.DrawLine(m_direction.position, m_direction.position + ((dirAsQuad * botLeft) * Vector3.forward), Color.red, 1);


        Debug.DrawLine(m_direction.position, m_direction.position + ((topleft * dirAsQuad) * Vector3.forward), Color.cyan, 1);
        Debug.DrawLine(m_direction.position, m_direction.position + ((topRight * dirAsQuad) * Vector3.forward), Color.cyan, 1);
        Debug.DrawLine(m_direction.position, m_direction.position + ((botRight * dirAsQuad) * Vector3.forward), Color.cyan, 1);
        Debug.DrawLine(m_direction.position, m_direction.position + ((botLeft * dirAsQuad) * Vector3.forward), Color.cyan, 1);
    }

    private void OnValidate()
    {
        //DrawLineAndSQuare();

    }
}
