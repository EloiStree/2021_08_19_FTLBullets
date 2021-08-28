using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class NCLR_LayerRef
{
   

    public bool[] m_ids;
    public NativeArray<bool> m_nativeArrayIds;
    NativeCapsuleLineRegister m_register;

    public NCLR_LayerRef(NativeCapsuleLineRegister register, int elementCount, bool defaultValue = false)
    {
        m_ids = new bool[elementCount];
        if (defaultValue == true)
        {
            for (int i = 0; i < elementCount; i++)
            {
                m_ids[i] = true;

            }
        }
        m_nativeArrayIds = new NativeArray<bool>(m_ids, Allocator.Persistent);
        m_register = register;
    }

    public void GetLayer(out NativeArray<bool> layer) {
        layer = m_nativeArrayIds;
    }

    ~NCLR_LayerRef()
    {
        m_nativeArrayIds.Dispose();
    }

}
