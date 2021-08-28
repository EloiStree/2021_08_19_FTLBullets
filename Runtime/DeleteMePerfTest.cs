using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMePerfTest : MonoBehaviour
{

    public UpdateTrackLineColliderMono m_trackedBullet;

    public int m_interation = 50000;

    void Start()
    {
        
    }


    void Update()
    {
        S_CapsuleLine capsule= new S_CapsuleLine();
        ICapsuleLine target = m_trackedBullet;
        for (int i = 0; i < m_interation; i++)
        {
            capsule.SetWith(target);
        }
    }
}
