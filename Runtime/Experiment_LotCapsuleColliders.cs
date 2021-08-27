using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment_LotCapsuleColliders : MonoBehaviour
{
    public Transform m_root;
    public Vector3 m_rotateAngle;
    public float m_rotateSpeed;
    public Transform m_origine;
    public int m_count;
    public bool m_areTrigger;

    void Start()
    {

        for (int i = 0; i < m_count; i++)
        {
            GameObject g = new GameObject("C");
            g.transform.parent = m_origine;
            g.transform.localPosition = new Vector3(0, i, 0);
            CapsuleCollider script= (CapsuleCollider) g.AddComponent(typeof(CapsuleCollider));
            script.radius = 0.1f;
            script.height = 0.5f;
            script.isTrigger = m_areTrigger;
        }

    }

    private void Update()
    {
        m_root.Rotate(m_rotateAngle, m_rotateSpeed*Time.deltaTime);
    }


}
