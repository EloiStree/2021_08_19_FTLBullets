
using System;
using Unity.Collections;

public class  NACapsuleLineDataRef
{

    private uint m_elementMaxCount;
    public NativeBoolLayerMask m_isActiveMask;
    public NativeArray<S_CapsuleLine> m_capsule;

    public NACapsuleLineDataRef(uint elementMaxCount)
    {
        InitWith(elementMaxCount);
    }
    public void InitWith(uint elementCount) {
        m_elementMaxCount = elementCount;
        m_isActiveMask.SetSize((int)elementCount);
        S_CapsuleLine[] dataResult = new S_CapsuleLine[elementCount];
        for (int i = 0; i < dataResult.Length; i++)
        {
            dataResult[i].m_id = i;
        }
        m_capsule = new NativeArray<S_CapsuleLine>(dataResult, Allocator.Persistent);
       
    }

    public void IsCapsuleActive(int index, out bool bullet)
    {
        m_isActiveMask.Get(ref index, out bullet);
    }

    public void GetCapsule(int index,  out S_CapsuleLine bullet)
    {
        bullet = m_capsule[index];
    }
   

    public void Dispose()
    {
        m_capsule.Dispose();
        m_isActiveMask.Dispose();

    }

    ~NACapsuleLineDataRef()
    {
        Dispose();
    }

    public void GetNativeActiveLayer(out NativeBoolLayerMask capsuleLayer)
    {
        capsuleLayer= m_isActiveMask;
    }

    public void GetNativeArrayCapsuleIne(out NativeArray<S_CapsuleLine> capsule)
    {
        capsule = m_capsule;
    }
}