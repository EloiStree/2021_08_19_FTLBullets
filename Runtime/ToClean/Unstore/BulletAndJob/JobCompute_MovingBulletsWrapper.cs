using System;
using Unity.Collections;
using Unity.Jobs;

public class JobCompute_MovingBulletsWrapper
{
    private JobComputeExe_MovingBullets m_jobExe;

    public JobCompute_MovingBulletsWrapper(JobComputeExe_MovingBullets executor)
    {
        this.m_jobExe = executor;
    }

    public void SetSharedMemory(NativeArray<TriggeredBulletData> bullets)
    {
        m_jobExe.m_bulletsInitParams = bullets;
    }

    public void SetSharedMemory(NativeArray<BulletDataResult> bulletsResult)
    {
        m_jobExe.m_bulletsResult = bulletsResult;
    }

    public void ComputeBulletNewPositions(float gameReferenceTimeInSeconds)
    {
      m_jobExe.SetCurrentTime(gameReferenceTimeInSeconds);
      JobHandle handler=  m_jobExe.Schedule(m_jobExe.m_bulletsInitParams.Length, 64);
      handler.Complete();
    }

}