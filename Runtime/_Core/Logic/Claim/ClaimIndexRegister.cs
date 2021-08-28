
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



