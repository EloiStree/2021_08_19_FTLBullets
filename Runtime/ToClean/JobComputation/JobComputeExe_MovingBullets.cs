using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct NativeBoolLayerMask {
    public int m_size;
    NativeArray<bool> m_booleanLayer;
    public void SetSize(int size, Allocator memoryType = Allocator.Persistent)
    {
        SetSize(size, out NativeArray<bool> l, memoryType);
    }
    public void SetSize(int size, out NativeArray<bool> layerSharedMemory, Allocator memoryType= Allocator.Persistent)
    {
        if (m_isDisposable)
            Dispose();
        bool[] bools = new bool[size];
        m_booleanLayer = new NativeArray<bool>(bools, Allocator.Persistent);
        m_size = size;
        m_isDisposable = true;
        layerSharedMemory = m_booleanLayer;
    }
    public void SetWith(ref NativeArray<bool> layerSharedMemory)
    {
        if (m_isDisposable)
            Dispose();
        m_booleanLayer = layerSharedMemory;
        m_size = layerSharedMemory.Length;
        m_isDisposable = true;
        layerSharedMemory = m_booleanLayer;
    }

    public void GetTrueCount(out int count)
    {
        count = 0;
        for (int i = 0; i < m_size; i++)
        {
            if (m_booleanLayer[i])
                count++;
        }
    }

     bool m_isDisposable;

    public bool IsDefined()
    {
        return m_size > 0;
    }

    public void Dispose() {
        if(m_isDisposable)
        m_booleanLayer.Dispose();
    }

    public bool IsTrue(ref int index)
    {
        return m_booleanLayer[index];
    }
    public bool IsFalse(ref int index)
    {
        return m_booleanLayer[index]==false;
    }

    public void Get(ref int index, out bool value)
    {
        value = m_booleanLayer[index];
    }
    public void GetInRef(ref int index, ref bool value)
    {
        value = m_booleanLayer[index];
    }

    public void SetAs(ref int index, bool isActive)
    {
        m_booleanLayer[index] = isActive;
    }
    public void SetAsFromRef(ref int index, ref bool isActive)
    {
        m_booleanLayer[index] = isActive;
    }

    public void GetIdListOfActive(out Queue<int> inRangeBullets)
    {
        inRangeBullets = new Queue<int>();
        for (int i = 0; i < m_size; i++)
        {
            if (m_booleanLayer[i])
                inRangeBullets.Enqueue(i);
        }
    }
    public void GetIdListOfActive(out List<int> inRangeBullets)
    {
        inRangeBullets = new List<int>();
        for (int i = 0; i < m_size; i++)
        {
            if (m_booleanLayer[i])
                inRangeBullets.Add(i);
        }
    }
}


[BurstCompile(CompileSynchronously = true)]
public struct JobComputeExe_MovingBullets : IJobParallelFor
{


    public float m_previousGameTimeInSeconds;
    public float m_currentGameTimeInSeconds;
    public NativeArray<TriggeredBulletData> m_bulletsInitParams;
    public NativeArray<BulletDataResult> m_bulletsResult;

    public NativeBoolLayerMask m_isCapsuleActive;
    public NativeArray<S_CapsuleLine> m_bulletCapsule;


    public void SetSharedMemory(NativeArray<TriggeredBulletData> bulletInitParamsMemory,
        NativeArray<BulletDataResult> bulletResultParamsMemory, NativeBoolLayerMask isCapsuleActiveParamsMemory, NativeArray<S_CapsuleLine> capsulesParamsMemory) {

        m_bulletsInitParams = bulletInitParamsMemory;
        m_bulletsResult = bulletResultParamsMemory;
        m_isCapsuleActive = isCapsuleActiveParamsMemory;
        m_bulletCapsule = capsulesParamsMemory;
    }

    public void Execute(int index)
    {
        TriggeredBulletData iniInfo = m_bulletsInitParams[index];
        BulletDataResult result = m_bulletsResult[index];
        if (iniInfo.m_isActive)
        {

            float t = m_currentGameTimeInSeconds - iniInfo.m_gameTimeWhenTriggerInSeconds;
            bool isOutOfTime =  t > iniInfo.m_bulletInfo.m_lifeTime;
            // The object is out of time an need to be reset as available
            if (isOutOfTime)
            {
                iniInfo.ResetAsAvailaible();
                m_bulletsInitParams[index] = iniInfo;

                result.ResetAsAvailaible();
                result.m_isUsed = false;
                result.m_lifeTimeInSeconds = t;
                result.m_previousPosition = Vector3.zero;
                result.m_currentPosition = Vector3.zero;
            }
            else
            {
                result.m_isUsed = m_bulletsInitParams[index].m_isActive;
                result.m_previousPosition = result.m_currentPosition;
                result.m_lifeTimeInSeconds = t;

                Vector3 v = m_bulletsResult[index].m_currentPosition;
                iniInfo.m_bulletInfo.GetPositionIn(t, ref v);
                result.m_currentPosition = v;
            }


            m_bulletsResult[index] = result;
        }
        m_isCapsuleActive.SetAs(ref index, iniInfo.m_isActive);
        if (iniInfo.m_isActive)
        {
            S_CapsuleLine cap = m_bulletCapsule[index];
            cap.m_start = result.m_previousPosition;
            cap.m_end = result.m_currentPosition;
            //Should be deal with only once not every time but I don't have time to code it now.
            cap.m_radius = iniInfo.m_bulletInfo.m_radius;

            m_bulletCapsule[index]=cap;
        }
    }


    public void SetCurrentTime(float timeInSeconds)
    {
        m_previousGameTimeInSeconds = m_currentGameTimeInSeconds;
        m_currentGameTimeInSeconds = timeInSeconds;
    }
    public void SetCurrentTime(float previousTimeInSeconds, float newTimeInSeconds)
    {
        m_previousGameTimeInSeconds = previousTimeInSeconds;
        m_currentGameTimeInSeconds = newTimeInSeconds;
    }


}
