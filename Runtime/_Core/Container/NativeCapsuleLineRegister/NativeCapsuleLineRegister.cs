
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class NativeCapsuleLineRegister
{
    public int m_memoryElementCount;
    public NativeArray<S_CapsuleLine> m_capsuleMemory;
    public NativeCapsuleLineRegister(int memoryElementCount)
    {
        m_memoryElementCount = memoryElementCount;
        S_CapsuleLine[] cl = new S_CapsuleLine[memoryElementCount];
        m_capsuleMemory = new NativeArray<S_CapsuleLine>(cl, Allocator.Persistent);

    }

    public void GetRegisterLink(int index, out NCLR_LineRef reference)
    {
        reference = new NCLR_LineRef(index, this);
    }


    private bool m_wasDispose;
    public void Dispose()
    {
        if (m_wasDispose)
        {
            m_wasDispose = true;
            m_capsuleMemory.Dispose();
        }
    }
    ~NativeCapsuleLineRegister()
    {
        Dispose();
    }

    public void GetCount(int count)
    {
        count = m_memoryElementCount;
    }


    public void SetRadius(ref int id, ref float radius)
    {
        S_CapsuleLine c = m_capsuleMemory[id];
        c.m_radius = radius;
        m_capsuleMemory[id] = c;
    }

    public void SetLineWith(ref int id, ref Vector3 startPoint, ref Vector3 endPoint)
    {
        S_CapsuleLine c = m_capsuleMemory[id];
        c.m_start = startPoint;
        c.m_end = endPoint;
        m_capsuleMemory[id] = c;
    }

}