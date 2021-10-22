using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class DrawMeshPoolingToRenderBulletsDefault : AbstractBulletsToRenderListener
{


    public int m_maxMesh = 200;
    public Mesh m_meshToUse;
    public Material m_materialToUse;
    public int m_layer =0;

    public int[] m_currentBulletId;


    public DrawMeshJob m_job;

    public bool m_useAdditionalRotation;
    public Vector3 m_eulerRotation;
    public bool m_useAdditionalScaling;
    public float m_scaleMultiplication;


    public override void ApplyComputeRendering(ref FilteredBulletsId bulletsToProcess)
    {
        m_currentBulletId = bulletsToProcess.m_mediumIds.ToArray();

        if (m_currentBulletId.Length == 0)
            return;

        m_job.SetAdjustement(m_useAdditionalRotation, m_eulerRotation, m_useAdditionalScaling, m_scaleMultiplication);
        m_job.SetTemp(m_currentBulletId);
        JobHandle jobHandle = m_job.Schedule<DrawMeshJob>(m_currentBulletId.Length,64);
        jobHandle.Complete();
        Matrix4x4[] m = m_job.m_matrixes.ToArray();
        Graphics.DrawMeshInstanced(m_meshToUse, m_layer, m_materialToUse, m, m.Length);

        m_job.Dispose();


    }
   
 
    public override void InitWithCount(int count)
    {
        m_job = new DrawMeshJob();
        m_job.Set(m_bulletsRenderingRef);
        m_job.Set(m_bulletsRef);
        m_job.Set(m_bulletInitRef);
    }

}

[BurstCompile(CompileSynchronously =true)]
public struct DrawMeshJob : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    [ReadOnly]
    public NativeArray<BulletRendering> m_bulletWantedRenderingInfo;
    [NativeDisableParallelForRestriction]
    [ReadOnly]
    public NativeArray<BulletDataResult> m_bulletStateResultInfo;
    [NativeDisableParallelForRestriction]
    [ReadOnly]
    public NativeArray<TriggeredBulletData> m_bulletStateInitInfo;
   
    [WriteOnly]
    public NativeArray<Matrix4x4> m_matrixes;
    [ReadOnly]
    public NativeArray<int> m_bulletsId;

    internal void Set(NativeArray<BulletDataResult> bulletDataResult)
    {
        m_bulletStateResultInfo = bulletDataResult;
    }
    internal void Set(NativeArray<TriggeredBulletData> bulletInitData)
    {
        m_bulletStateInitInfo = bulletInitData;
    }
    public void Set(NativeArray<BulletRendering> bulletRenderingInfo)
    {
        m_bulletWantedRenderingInfo = bulletRenderingInfo;
    }
    public void SetTemp(int[] ids)
    {
        m_bulletsId = new NativeArray<int>(ids, Allocator.TempJob);
        m_matrixes = new NativeArray<Matrix4x4>( new Matrix4x4[ids.Length], Allocator.TempJob);
    }
    public void Dispose() {
        if (m_bulletsId.IsCreated)
            m_bulletsId.Dispose();
        if (m_matrixes.IsCreated)
            m_matrixes.Dispose();
    }


    public void Execute(int index)
    {
        int bulletId = m_bulletsId[index];
        if (bulletId < 0)
            return; 
        if (!m_bulletStateResultInfo[bulletId].m_isUsed)
            return;

        float radius = m_bulletStateInitInfo[bulletId].m_bulletInfo.m_radius * 2f;

        Vector3 scale = new Vector3(radius, radius, radius);
        if (m_useAdditionalScalingRotation)
            scale *= m_scaleMultiplication;
        Quaternion rotation =
            Quaternion.LookRotation(
                m_bulletStateResultInfo[bulletId].m_currentPosition - m_bulletStateResultInfo[bulletId].m_previousPosition,
                Vector3.up);
        if (m_useAdditionalRotation)
            rotation *= m_eulerRotation;


        m_matrixes[index]= Matrix4x4.TRS(
            m_bulletStateResultInfo[bulletId].m_currentPosition,rotation, scale      );
    }

    internal void SetAdjustement(bool useAdditionalRotation, Vector3 eulerRotation, bool useAdditionalScaling, float scaleMultiplication)
    {
        m_useAdditionalRotation = useAdditionalRotation;
        m_eulerRotation = Quaternion.Euler(eulerRotation);
        m_useAdditionalScalingRotation = useAdditionalScaling;
        m_scaleMultiplication = scaleMultiplication;
    }
    public bool m_useAdditionalRotation;
    public Quaternion m_eulerRotation;
    public bool m_useAdditionalScalingRotation;
    public float m_scaleMultiplication;
}