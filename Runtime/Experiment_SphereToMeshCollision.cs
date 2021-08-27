using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment_SphereToMeshCollision : MonoBehaviour
{
    public GameObject m_touchObject;
    public Vector3 m_contactPoints;

    public GameObject m_rayHit;
    public int m_triangleHit;
    public LayerMask m_bulletVirtualZone;
    private void OnCollisionStay(Collision collision)
    {
        m_touchObject = collision.gameObject;
        m_contactPoints = collision.contacts[0].point;
        if(Physics.Raycast(collision.contacts[0].point- collision.contacts[0].normal*0.01f, collision.contacts[0].normal,out RaycastHit hit, 0.2f, m_bulletVirtualZone) ){

            m_rayHit = hit.collider.gameObject;
            m_triangleHit= hit.triangleIndex;

        }
        

        //https://docs.unity3d.com/ScriptReference/RaycastHit-triangleIndex.html
    }
}
