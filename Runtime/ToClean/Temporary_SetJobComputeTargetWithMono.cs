using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Temporary_SetJobComputeTargetWithMono : MonoBehaviour
{
    public Experiment_JobComputeBulletCapsuleLine m_jobCompute;
    public AbstractCapsuleLineMono[] m_targets;

    public NativeBoolLayerMask m_activeTargets;
    public NativeArray<S_CapsuleLine> m_targetsAsCapsule;

    private void Awake()
    {
        m_activeTargets.SetSize(m_targets.Length);
        S_CapsuleLine[] cl = new S_CapsuleLine[m_targets.Length];
        m_targetsAsCapsule = new NativeArray<S_CapsuleLine>(cl, Allocator.Persistent);

        m_jobCompute.SetBulletableRefWith(m_activeTargets, m_targetsAsCapsule);
        S_CapsuleLine c;
        for (int i = 0; i < m_targets.Length; i++)
        {
            m_activeTargets.SetAs(ref i, m_targets[i].isActiveAndEnabled);
            if (m_targets[i].isActiveAndEnabled)
            {

                c = m_targetsAsCapsule[i];
                m_targets[i].GetLineRadius(out float r);
                c.m_radius = r;
                m_targetsAsCapsule[i] = c;
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < m_targets.Length; i++)
        {
            m_activeTargets.SetAs(ref i, m_targets[i].isActiveAndEnabled);
            if (m_targets[i].isActiveAndEnabled) {

                m_targets[i].GetLineAsPoints(out Vector3 s, out Vector3 end);
                S_CapsuleLine cl = m_targetsAsCapsule[i];
                cl.m_start = s;
                cl.m_end = end;
                m_targetsAsCapsule[i]= cl;
            }
        }
    }
}
