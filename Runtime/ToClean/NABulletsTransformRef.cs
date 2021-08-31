

using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

public class NABulletsTransformRef {


    private uint m_elementMaxCount;

    public Transform[] m_transforms;
    public TransformAccessArray m_transformAccess;

    //private bool m_isCurrentlyUseBySomeone;
    //public void SetAsUsedBySomeThread(bool isProccessing) {
    //    m_isCurrentlyUseBySomeone = isProccessing;
    //}
    //public bool IsUsedBySomeThread() { 
    //    return m_isCurrentlyUseBySomeone; 
    //}

    public NABulletsTransformRef(uint elementMaxCount)
    {
        InitWithEmptyTransforms(elementMaxCount);
    }
    public void InitWithEmptyTransforms(uint elementMaxCount)
    {
        m_elementMaxCount = elementMaxCount;
        m_transforms = new Transform[elementMaxCount];
        m_transformAccess = new TransformAccessArray(m_transforms);

    }
    public void InitWithEmptyTransforms(params Transform [] transforms)
    {
        m_elementMaxCount = (uint)transforms.Length;
        m_transforms = transforms;
        m_transformAccess = new TransformAccessArray(m_transforms);

    }

    public void GetMaxSize(out uint maxSize) { maxSize = m_elementMaxCount; }
    public void SetTransform(int index, Transform transformToUse) {
        
        m_transformAccess[index] = transformToUse;
    }
    public void GetTransfromAt(int index, out Transform linkedTransform)
    {
        linkedTransform = m_transformAccess[index];
    }

    public void GetTransformAccesArray(out TransformAccessArray memory) {
        memory = m_transformAccess;
    }

    public void Dispose() {
        m_transformAccess.Dispose();

    }

    ~NABulletsTransformRef() {
        Dispose();
    }

}