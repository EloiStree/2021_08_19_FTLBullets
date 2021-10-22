using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Experiment_BulletsListToPixelFarMeshMono : AbstractBulletsToRenderListener
{
    public PixelFarMeshMono m_pushToPixel;

    public Transform m_viewPosition;
    public Transform m_viewRotation;

    public bool m_bulletsInit;
    public NativeArray<Vector3> m_relocations;
    public int m_countClamping = 10000;


    public RelocateJobBullets m_jobReloction=new RelocateJobBullets();
    public List<int> m_debugBulletsIds;

   
    public void SetBulletsRef(NativeArray<BulletDataResult> bulletsResultRef)
    {
        m_bulletsInit = true;
        m_jobReloction.SetNativeArrayRef( bulletsResultRef);
    }
    public override void InitWithCount(int count)
    {
        if (count < m_countClamping)
            m_countClamping = count;
        ClampMemoryZoneUsed(m_countClamping);
        m_jobReloction.SetNativeArrayRefAndSoMaxCount(m_relocations);
    }
    private void ClampMemoryZoneUsed(int count)
    {
        if (m_relocations.IsCreated)
            m_relocations.Dispose();

        Vector3[] m_flusherSpace;
        m_flusherSpace = new Vector3[m_countClamping];
        m_relocations = new NativeArray<Vector3>(m_flusherSpace, Allocator.Persistent);
    }
    private void OnDestroy()
    {
        if(m_relocations.IsCreated)
        m_relocations.Dispose();

        m_jobReloction.Dispose();


    }

    public override void ApplyComputeRendering(ref FilteredBulletsId bulletsToProcess)
    {
        if (m_bulletsInit==false)
            return;

        m_debugBulletsIds = bulletsToProcess.m_pixelFarIds;
        m_jobReloction.SetWithNewList (bulletsToProcess.m_pixelFarIds);

        JobHandle jobHandle= m_jobReloction.Schedule<RelocateJobBullets>(m_debugBulletsIds.Count,64);
        jobHandle.Complete();

        m_pushToPixel.RefreshCameraPosition(m_viewPosition.position, m_viewRotation.rotation);
        m_pushToPixel.RefreshTargetWithWorldPointsRef(m_relocations);
        m_pushToPixel.ComputeFromSource(out NativeArray<SquareFarPixel> farPixel); 

        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].RefreshWith(m_viewPosition.position, m_viewRotation.rotation, farPixel);

        }
    }

  
    public AbstractSquareFarPixelRendererListener[] m_renderers;

    void Start()
    {
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].InitParams(m_pushToPixel.m_config);
        }
    }

   

    [BurstCompile(CompileSynchronously = true)]
    public struct RelocateJobBullets : IJobParallelFor
    {


        public Vector3 m_sourcePosition;
        public Quaternion m_sourceRotationInverse;

        [NativeDisableParallelForRestriction]
        public NativeArray<BulletDataResult> m_bullets;

        public NativeArray<Vector3> m_relocated;
        public NativeArray<int> m_selectedToBeRender;

        public int m_maxCount;
        public int m_fillCount;

        public void SetWithNewList(List<int> selection) {
            if (m_selectedToBeRender == null || m_selectedToBeRender.Length < 1)
                return;
            m_fillCount = selection.Count;
            if (m_fillCount > m_maxCount)
                m_fillCount = m_maxCount;
            for (int i = 0; i < m_fillCount ; i++)
            {
                m_selectedToBeRender[i] = selection[i];
            }
        }
      
        public void SetNativeArrayRef(NativeArray<BulletDataResult> bullets)
        {
            m_bullets = bullets;
        }
        public void SetNativeArrayRefAndSoMaxCount(NativeArray<Vector3> relocated)
        {
            m_maxCount = relocated.Length;
            int [] i = new int[m_maxCount];
            m_selectedToBeRender = new NativeArray<int>(i, Allocator.Persistent);
            m_relocated = relocated;
        }
   
        public void Execute(int index)
        {
            if (index >= m_fillCount)
                return;
            if (m_selectedToBeRender[index] < 0)
                return;
            int bulletId = m_selectedToBeRender[index];
            m_relocated[index] = m_sourceRotationInverse * (m_bullets[bulletId].m_currentPosition - m_sourcePosition);
        }

        public void SetWorldPosition(Vector3 worldPoint, Quaternion worldRotation)
        {
            m_sourcePosition = worldPoint;
            m_sourceRotationInverse = Quaternion.Inverse(worldRotation);
        }

        public void Dispose() {
            if(m_selectedToBeRender.IsCreated)
            m_selectedToBeRender.Dispose();
        }
        
    }
}

