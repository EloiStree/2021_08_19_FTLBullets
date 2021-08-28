using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NCLR_LineRef
{
    int m_id;
    NativeCapsuleLineRegister m_register;

    public NCLR_LineRef(int id, NativeCapsuleLineRegister register)
    {
        m_register = register;
        m_id = id;
    }
    public void SetLineWith(Vector3 startPoint, Vector3 endPoint)
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