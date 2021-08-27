using System;
using Unity.Jobs;
using UnityEngine.Jobs;

public class JobCompute_TransformBulletsWrapper
{
    private JobComputeExe_DefaultBulletsToTransform m_jobExe;
    private TransformAccessArray m_transformsMemory;

    public JobCompute_TransformBulletsWrapper(JobComputeExe_DefaultBulletsToTransform executor)
    {
        this.m_jobExe = executor;
    }

    public void SetSharedMemory(TransformAccessArray transformsMemory)
    {
        m_transformsMemory = transformsMemory;
    }
    private JobHandle m_jobHandler;
    public void StartComputingTransformPosition()
    {
        m_jobHandler= m_jobExe.Schedule(m_transformsMemory);
        //m_jobHandler.Complete();
    }

    public void ApplyTransformPosition()
    {
        m_jobHandler.Complete();
    }
}