using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment_MeshBulletsHit : MonoBehaviour
{

    public Transform m_target;
    public float m_radius=10;
    public LayerMask m_layerMask;
    public float m_drawLineTime = 2;

    public int m_collision;

    void Update()
    {
       Collider [] collisions = Physics.OverlapSphere( m_target.position , m_radius , m_layerMask,QueryTriggerInteraction.Collide);
        m_collision= collisions.Length;
        for (int i = 0; i < collisions.Length; i++)
        {
            Collider c = collisions[i];
            Vector3 closeV= c.ClosestPoint(m_target.position);
            Debug.DrawLine(m_target.position, closeV, Color.cyan, m_drawLineTime);
           Vector3 closeBoundV= c.ClosestPointOnBounds(m_target.position);
            Debug.DrawLine(m_target.position, closeBoundV, Color.yellow, m_drawLineTime);


        }
    }
}
