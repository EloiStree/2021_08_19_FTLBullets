using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile(CompileSynchronously = true)]
public struct JobComputeExe_MovingBullets : IJobParallelFor
{


    public float m_previousGameTimeInSeconds;
    public float m_currentGameTimeInSeconds;
    public NativeArray<TriggeredBulletData> m_bulletsInitParams;
    public NativeArray<BulletDataResult> m_bulletsResult;


    public void SetSharedMemory(NativeArray<TriggeredBulletData> bulletInitParamsMemory,
        NativeArray<BulletDataResult> bulletResultParamsMemory) {

        m_bulletsInitParams = bulletInitParamsMemory;
        m_bulletsResult = bulletResultParamsMemory;
    }

    public void Execute(int index)
    {
        if (m_bulletsInitParams[index].m_isActive)
        {

            float t = m_currentGameTimeInSeconds - m_bulletsInitParams[index].m_gameTimeWhenTriggerInSeconds;
            BulletDataResult result = m_bulletsResult[index];
            result.m_isUsed = m_bulletsInitParams[index].m_isActive;
            result.m_previousPosition = result.m_currentPosition;
            result.m_lifeTimeInSeconds = t;

            Vector3 v = m_bulletsResult[index].m_currentPosition;
            m_bulletsInitParams[index].m_bulletInfo.GetPositionIn(t, ref v);
            result.m_currentPosition = v;
            m_bulletsResult[index] = result;


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
