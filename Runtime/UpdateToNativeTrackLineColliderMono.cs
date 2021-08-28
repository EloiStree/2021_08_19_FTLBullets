
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public class ClaimIndexRegister {


    public Queue<int> m_unclaimedId = new Queue<int>();

    public int m_idCursor = 0;
    public int m_elementCount = -1;

    public ClaimIndexRegister(int elementCount)
    {
        SetMaxSize(elementCount);
    }

    public void SetMaxSize(int elementCount)
    {
        m_elementCount = elementCount;
    }
    public void ResetCursorToZero() {
        m_unclaimedId.Clear();
        m_idCursor = 0;
    }


    public void Claim(out int id) {
        SeekAvailaibleId(out id);
    }
    private void SeekAvailaibleId(out int id)
    {
        if (m_unclaimedId.Count > 0) { 
            id = m_unclaimedId.Dequeue();
        }
        else
        {
            id = m_idCursor;
            m_idCursor++;
            if (m_idCursor >= m_elementCount)
                throw new System.IndexOutOfRangeException("Cursor out of range");
        }
    }

    public void Unclaim(int id) {
        if (id <= m_idCursor)
            m_unclaimedId.Enqueue(id);

    }
}

public class NativeCapsuleLineRegister
{



    public static ClaimIndexRegister ClaimRegister = new ClaimIndexRegister(200000);
    public static NativeCapsuleLineRegister SharedMemory = new NativeCapsuleLineRegister(200000);
    public static void OverrideMemory(int elements) {

        SharedMemory = new NativeCapsuleLineRegister(elements);
        ClaimRegister = new ClaimIndexRegister(elements);
    }


    public int m_memoryElementCount;
    public NativeArray<S_CapsuleLine> m_capsuleMemory;
    public NativeCapsuleLineRegister(int memoryElementCount)
    {
        m_memoryElementCount = memoryElementCount;
        S_CapsuleLine [] cl = new S_CapsuleLine[memoryElementCount];
        m_capsuleMemory =new NativeArray<S_CapsuleLine>(cl, Allocator.Persistent);

    }

    public void GetRegisterLink(int index, out LineRef reference) {
        reference = new LineRef(index,this) ;
    }


    private bool m_wasDispose;
    public void Dispose() {
        if (m_wasDispose) { 
            m_wasDispose = true;
            m_capsuleMemory.Dispose();
        }
    }
    ~NativeCapsuleLineRegister() {
        Dispose();
    }

    public void GetCount(int count) {
        count = m_memoryElementCount;
    }


    public void SetRadius(ref int id, ref float radius)
    {
        S_CapsuleLine c = m_capsuleMemory[id];
        c.m_radius = radius;
        m_capsuleMemory[id]=c;
    }

    public void SetLineWith(ref int id, ref Vector3 startPoint, ref Vector3 endPoint)
    {
        S_CapsuleLine c = m_capsuleMemory[id];
        c.m_start = startPoint;
        c.m_end = endPoint;
        m_capsuleMemory[id] = c;
    }

    #region Reference to register
    public class LineRef
    {
        int m_id;
        NativeCapsuleLineRegister m_register;

        public LineRef(int id, NativeCapsuleLineRegister register)
        {
            m_register = register;
            m_id = id;
        }
        public void SetLineWith( Vector3 startPoint, Vector3 endPoint)
        {
            m_register.SetLineWith(ref m_id, ref startPoint, ref endPoint);
        }
        public void SetLineWith(ref Vector3 startPoint, ref Vector3 endPoint)
        {
            m_register.SetLineWith(ref m_id, ref startPoint, ref endPoint);
        }
        public void SetRadius(ref float radius)
        {
            m_register.SetRadius(ref m_id, ref radius);
        }
    }

    public class ListRef {

        public List<int> m_ids = new List<int>();
        NativeCapsuleLineRegister m_register;
        public ListRef(NativeCapsuleLineRegister register, params int[] ids)
        {
            m_register = register;
            m_ids.AddRange(ids);
        }
        public ListRef(NativeCapsuleLineRegister register, IEnumerable<int> ids)
        {
            m_register = register;
            m_ids.AddRange(ids);
        }

        public void Remove(params int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                m_ids.Remove(ids[i]);
            }
        }
        public void Add(params int[] ids)
        {
            m_ids.AddRange(ids);
        }
        public void ResetWith(params int[] ids)
        {
            m_ids.Clear();
            m_ids.AddRange(ids);
        }
    }

    public class NativeLayerRef {

        public bool[] m_ids;
        public NativeArray<bool> m_nativeArrayIds;
        NativeCapsuleLineRegister m_register;

        public NativeLayerRef(NativeCapsuleLineRegister register,int elementCount, bool defaultValue=false)
        {
            m_ids = new bool[elementCount];
            if (defaultValue == true) { 
                for (int i = 0; i < elementCount; i++)
                {
                    m_ids[i] = true;

                }
            }
            m_nativeArrayIds = new NativeArray<bool>(m_ids, Allocator.Persistent);
            m_register = register;
        }
        ~NativeLayerRef() {
            m_nativeArrayIds.Dispose();
        }
    }
    #endregion
}


