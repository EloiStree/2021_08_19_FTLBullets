using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NCLR_IdListRef
{

    public List<int> m_ids = new List<int>();
    NativeCapsuleLineRegister m_register;
    public NCLR_IdListRef(NativeCapsuleLineRegister register, params int[] ids)
    {
        m_register = register;
        m_ids.AddRange(ids);
    }
    public NCLR_IdListRef(NativeCapsuleLineRegister register, IEnumerable<int> ids)
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
