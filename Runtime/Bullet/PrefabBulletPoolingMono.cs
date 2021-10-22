using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class PrefabBulletPoolingMono : AbstractTransformRepresentationOfBullets
{

    public int m_maxPrefab = 200;
    public Transform m_prefabParent;
    public GameObject m_prefabToUse;

    public Transform[] m_prefabCreated;
    public TransformAccessArray m_prefabArray;


    public override void GetMaxiumAllow(out int maxEntity)
    {
        maxEntity = m_prefabCreated.Length;
    }

    public override void GetNativeTransformArrayRef(out TransformAccessArray nativeArrayRef)
    {
        nativeArrayRef = m_prefabArray;
    }

    public override void GetTransformFor(int index, out Transform transform)
    {
        if (index < 0 || index >= m_prefabCreated.Length)
            transform = null;
        else
            transform = m_prefabCreated[index];
    }

    void Start()
    {

        m_prefabCreated = new Transform[m_maxPrefab]; 
        for (int i = 0; i < m_maxPrefab; i++)
        {
            m_prefabToUse.SetActive(false);
            GameObject created = GameObject.Instantiate(m_prefabToUse, Vector3.zero, Quaternion.identity, m_prefabParent);
            created.SetActive(false);
            m_prefabCreated[i] = created.transform;
        }
        m_prefabArray = new TransformAccessArray(m_prefabCreated);
    }

    private void OnDestroy()
    {
        if (m_prefabArray.isCreated)
            m_prefabArray.Dispose();
    }

    public override void SetAsUsing(int index, bool usingTransform)
    {
        if (index > -1 && index < m_prefabCreated.Length && m_prefabCreated[index]!=null) { 
           m_prefabCreated[index].gameObject.SetActive(usingTransform);
        }
    }
}
