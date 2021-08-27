//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Jobs;
//using UnityEngine;

//public class ComputeMovingBulletsShaderMonoWeird : MonoBehaviour
//{
  
//    public ComputeShader m_computeBullets;
//    public ComputeShaderBulletsPiece m_jobExperiment;
//    public int m_bulletCount;
//    public int m_bulletGroupCount;
//    public int m_elementsTotal;

//    void Awake()
//    {
//        TrickyCheater.m_shader = m_computeBullets;
//        TrickyCheater.SetWith(m_bulletGroupCount, m_bulletCount);
//        m_elementsTotal = m_bulletCount * m_bulletGroupCount;
//        m_jobExperiment.InitNumberOfBullet(m_bulletCount, m_bulletGroupCount);

//    }

//     void Update()
//    {
//        JobHandle handler = m_jobExperiment.Schedule(m_elementsTotal, 64);
//        handler.Complete();
//    }


//     void OnDestroy()
//    {
//        m_jobExperiment.DisposeMemoryUse();
//    }






//}


//public static class TrickyCheater {


//    public static ComputeShader m_shader;
//    public static ComputeBuffer[] m_computeBuffer;

//    public static void SetWith(int groups, int elements) {
//        for (int i = 0; i < groups; i++)
//        {
//            TrickyCheater.m_computeBuffer[i] = new ComputeBuffer(elements,
//                Marshal.SizeOf(typeof(ComputableBulletStruct)));
//        }
//    }

//    public static void Dispose() {
//        if (m_computeBuffer != null)
//        {
//            for (int i = 0; i < m_computeBuffer.Length; i++)
//            {
//                m_computeBuffer[i].Dispose();
//            }
//        }
//    }
//}


//[BurstCompile(CompileSynchronously = true)]
//public struct ComputeShaderBulletsPiece : IJobParallelFor
//{
//    public int m_bulletsCountPerGroup;
//    public int m_bulletGroups;
//    public NativeArray<ComputableBulletStruct> m_poolOfBullets;
//    private bool m_disposable;

//    public void InitNumberOfBullet(int bulletsGroup, int bulletsCount  )
//    {
//        if (!m_disposable)
//        {
//            m_bulletsCountPerGroup = bulletsCount;
//            m_bulletGroups = bulletsGroup;
//            m_disposable = true;
//            m_poolOfBullets = new NativeArray<ComputableBulletStruct>(m_bulletGroups * m_bulletsCountPerGroup, Allocator.Persistent);
            
//        }
//    }
//    public void DisposeMemoryUse()
//    {
//        if (m_disposable)
//        {
//            m_disposable = false;
//            if (m_poolOfBullets != null)
//            {
//                m_poolOfBullets.Dispose();
//            }
//        }
//    }
//    public void Execute(int index)
//    {
//        int kernel = TrickyCheater.m_shader.FindKernel("CSMain");
//        TrickyCheater.m_shader.Dispatch(kernel, 8, 1, 1);
//    }
//}

