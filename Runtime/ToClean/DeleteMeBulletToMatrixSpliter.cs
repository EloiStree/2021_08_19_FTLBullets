using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMeBulletToMatrixSpliter : MonoBehaviour
{
    public int count=9999;
    public SeaweedMatrixSpliterMono m_seaweed;
    public Transform m_whereToCreate;
    public Transform[] m_created;
    public bool m_active;
    // Start is called before the first frame update
    void Awake()
    {
        if (m_active) { 
            List<Transform> created = new List<Transform>();
            for (int i = 0; i < count; i++)
            {
                m_seaweed.GetRandomPositionInMatrix(out Vector3 localPosition);
                Vector3 pos = m_seaweed.m_root.TransformPoint(localPosition);
                GameObject g = new GameObject("P"+i);
                g.transform.position = pos;
                g.transform.parent = m_whereToCreate;
                created.Add(g.transform);
            }
            m_created = created.ToArray();
            m_seaweed.matrixSplite.m_spliter.PushIn(m_created, m_whereToCreate);
        }


    }

}
