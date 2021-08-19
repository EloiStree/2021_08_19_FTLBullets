using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class LightSpeedBulletMonoExperiment : MonoBehaviour
{

    public int m_bulletCount=50000;
    public BulletsPoolStructWrapper m_bulletsManager ;
    public Transform m_testBulletPosition;
    public BulletData m_tbi;
    public BulletDataResult m_tbr;
    BulletInfoAndBasicAccessRef m_bulletRef;

    public  TransformAccessArray m_TransformsAccessArray;
    private void Awake()
    {
        m_bulletsManager = new BulletsPoolStructWrapper(m_bulletCount);

        m_bulletsManager.GetBulletReference(0, out m_bulletRef);
        m_bulletRef.SetAllInOne(true, Vector3.zero, Vector3.up, 0);
        m_testBulletPosition.forward = Vector3.up;
        for (int i = 0; i < m_bulletsManager.m_bulletCount; i++)
        {
            m_bulletsManager.m_pool.SetBullet(i, new BulletData()
            {
                m_isUsed = true,
                m_bulletInfo = new LightSpeedBulletData()
                {
                    m_startPoint = Vector3.zero,
                    m_directionSpeed = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value) * UnityEngine.Random.value * 4f
                }
            }); 
        }
    }

    private void Update()
    {
        m_bulletsManager.m_pool.SetCurrentTime(Time.time);
        m_bulletsManager.RefreshPosition();
        
    }
    private void LateUpdate()
    {
        m_bulletsManager.m_pool.GetBullet(0, out m_tbi);
        m_bulletsManager.m_pool.GetBulletResult(0, out m_tbr);
        m_bulletRef.GetTajectory(out DoubleVector3 from, out DoubleVector3 to);
        m_testBulletPosition.position = 
            new Vector3( (float)to.m_x, (float)to.m_y, (float)to.m_z);
    }

    private void OnDestroy()
    {
        m_bulletsManager.ForceDispose();
    }
    private void OnApplicationQuit()
    {
        m_bulletsManager.ForceDispose();
    }
}


public interface BulletClaimerPool {

    void Claim(out int bulletIndex, out ClaimedBullet bullet);
    void Unclaim(out int bulletIndex);

}
public interface ClaimedBullet {
    void GetBulletInfo(out ComputedBulletInfo info);
    void Unclaimed();
}
public interface ComputedBulletInfo
{

    void GetStartPoint(out Vector3 start);
    void GetDirection(out Vector3 direction);
    void GetCurrentPosition(out DoubleVector3 position);
    void GetPreviousPosition(out DoubleVector3 position);
    void GetTajectory(out DoubleVector3 previousPosition, out DoubleVector3 currentPosition);
    void GetLifeTime(out float lifeTimeInSeconds);
}

public interface BulletSetter {

    void SetAsUsing(bool isUsing);
    void SetStartInfo(Vector3 startPoint, Vector3 directionSpeed);
    void SetTimeWhenTriggered(float triggeredTimeInSeconds);
    void SetAllInOne(bool isUsing, Vector3 startPoint, Vector3 directionSPeed, float triggeredTimeInSeconds);

}

public interface BulletInfoAndBasicAccessRef : ComputedBulletInfo, BulletSetter { 

}
public class BulletPoolReference : BulletInfoAndBasicAccessRef
{
    public int m_bulletIndex;
    BulletsPoolStructWrapper m_bulletsPool;

    public BulletPoolReference(int bulletIndex, BulletsPoolStructWrapper bulletsPool)
    {
        m_bulletIndex = bulletIndex;
        m_bulletsPool = bulletsPool;
    }

    public void GetCurrentPosition(out DoubleVector3 position)
    {
        m_bulletsPool.m_pool.GetBulletResult(m_bulletIndex, out BulletDataResult result);
        position = result.m_currentPosition;
    }
    public void GetPreviousPosition(out DoubleVector3 position)
    {
        m_bulletsPool.m_pool.GetBulletResult(m_bulletIndex, out BulletDataResult result);
        position = result.m_currentPosition;
    }
    public void GetTajectory(out DoubleVector3 previousPosition, out DoubleVector3 currentPosition)
    {
        m_bulletsPool.m_pool.GetBulletResult(m_bulletIndex, out BulletDataResult result);
        previousPosition = result.m_previousPosition;
        currentPosition = result.m_currentPosition;
    }
    public void GetDirection(out Vector3 direction)
    {
        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out BulletData result);
        direction = result.m_bulletInfo.m_directionSpeed;
    }

    

    public void GetStartPoint(out Vector3 start)
    {
        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out BulletData result);
        start = result.m_bulletInfo.m_startPoint;
    }

    public void SetAsUsing(bool isUsing)
    {
        m_bulletsPool.m_pool.SetBulletAs(m_bulletIndex, isUsing);
    }

    public void SetStartInfo(Vector3 startPoint, Vector3 directionSpeed)
    {
        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out BulletData result);
        result.m_bulletInfo.m_startPoint = startPoint;
        result.m_bulletInfo.m_directionSpeed = directionSpeed;
        m_bulletsPool.m_pool.SetBullet(m_bulletIndex, result);
    }
   

    public void SetTimeWhenTriggered(float triggeredTimeInSeconds)
    {
        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out BulletData result);
        result.m_gameTimeWhenTriggerInSeconds = triggeredTimeInSeconds;
        m_bulletsPool.m_pool.SetBullet(m_bulletIndex, result);
    }

    public void SetAllInOne(bool isUsing, Vector3 startPoint, Vector3 directionSpeed, float triggeredTimeInSeconds)
    {
        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out BulletData result);
        result.m_bulletInfo.m_startPoint = startPoint;
        result.m_bulletInfo.m_directionSpeed = directionSpeed;
        result.m_gameTimeWhenTriggerInSeconds  = triggeredTimeInSeconds;
        result.m_isUsed = isUsing;
        m_bulletsPool.m_pool.SetBullet(m_bulletIndex, result);
    }

    public void GetLifeTime(out float lifeTimeInSeconds)
    {
        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out BulletData result);
        lifeTimeInSeconds = result.m_gameTimeWhenTriggerInSeconds;
    }
}



public class PoolableBulletDataRef {
    public int m_bulletIndex;
}
public class BulletsPoolStructWrapper {
    public BulletsPoolMovingComputeJob m_pool;
    public int m_bulletCount;

    public BulletsPoolStructWrapper(int bulletCount) {
        m_bulletCount = bulletCount;
        m_pool.InitNumberOfBullet(bulletCount);
    }

    public void RefreshPosition() {

        JobHandle handler = m_pool.Schedule(m_bulletCount, 64);
        handler.Complete();

    }

    public void GetBulletReference(int bulletIndex, out BulletInfoAndBasicAccessRef bulletRef)
    {
        bulletRef = new BulletPoolReference(bulletIndex, this);
    }
    public void ForceDispose()
    {
        m_pool.DisposeMemoryUse();
    }
    ~BulletsPoolStructWrapper()
    {
        m_pool.DisposeMemoryUse();
    }
}

[BurstCompile(CompileSynchronously = true)]
public struct BulletsPoolMovingComputeJob : IJobParallelFor {


    public float m_previousGameTimeInSeconds;
    public float m_currentGameTimeInSeconds;
    public NativeArray<BulletData> poolOfBullets;
    public NativeArray<BulletDataResult> poolOfBulletsPosition;
    private bool m_disposable;
    public void InitNumberOfBullet(int bulletsCount)
    {
        if (!m_disposable) { 
            m_disposable = true;
            poolOfBullets = new NativeArray<BulletData>(bulletsCount, Allocator.Persistent);
            poolOfBulletsPosition = new NativeArray<BulletDataResult>(bulletsCount, Allocator.Persistent);
        }
    }

    public void SetCurrentTime(float timeInSeconds)
    {
        m_previousGameTimeInSeconds = m_currentGameTimeInSeconds;
        m_currentGameTimeInSeconds = timeInSeconds;
    }
    public void SetCurrentTime(float previousTimeInSeconds,float newTimeInSeconds)
    {
        m_previousGameTimeInSeconds = previousTimeInSeconds;
        m_currentGameTimeInSeconds = newTimeInSeconds;
    }

    public void SetBulletAs(int index, bool isUsed) {
        BulletData bullet = poolOfBullets[index];
        bullet.m_isUsed = isUsed;
        poolOfBullets[index] = bullet;
    }

    public void SetBullet(int index, BulletData bullet)
    {
        poolOfBullets[index] = bullet;
       

    }
    public void GetBullet(int index, out BulletData bullet)
    {
        bullet = poolOfBullets[index];
    }
    public void GetBulletResult(int index, out BulletDataResult bullet)
    {
        bullet = poolOfBulletsPosition[index];
    }

    public void DisposeMemoryUse() {
        if (m_disposable) {
            m_disposable = false;
            if (poolOfBullets != null)
            {
                poolOfBullets.Dispose();
            }
            if (poolOfBulletsPosition != null)
            {
                poolOfBulletsPosition.Dispose();
            }
        }
    }


    public void Execute(int index)
    {
        if (poolOfBullets[index].m_isUsed) {

            float t = m_currentGameTimeInSeconds - poolOfBullets[index].m_gameTimeWhenTriggerInSeconds;
            BulletDataResult result = poolOfBulletsPosition[index];
            result.m_isUsed = poolOfBullets[index].m_isUsed;
            result.m_previousPosition = result.m_currentPosition;
            result.m_lifeTimeInSeconds = t;

            DoubleVector3 v = poolOfBulletsPosition[index].m_currentPosition;
            poolOfBullets[index].m_bulletInfo.GetPositionIn(t, ref v);
            result.m_currentPosition = v;
            poolOfBulletsPosition[index] = result;

        }
    }
}

[System.Serializable]
public struct BulletDataResult {
    public bool m_isUsed;
    public DoubleVector3 m_previousPosition;
    public DoubleVector3 m_currentPosition;
    public float m_lifeTimeInSeconds;
}

[System.Serializable]
public struct BulletData {

    public bool m_isUsed;
    public float m_gameTimeWhenTriggerInSeconds;
    public LightSpeedBulletData m_bulletInfo;
}

[System.Serializable]
public struct LightSpeedBulletData {

    public Vector3 m_startPoint;
    public Vector3 m_directionSpeed;

    public void GetPositionIn(double timeSinceTriggeredInSeconds, ref DoubleVector3 newPosition)
    {

        newPosition.m_x = m_startPoint.x + m_directionSpeed.x * timeSinceTriggeredInSeconds;
        newPosition.m_y = m_startPoint.y + m_directionSpeed.y * timeSinceTriggeredInSeconds;
        newPosition.m_z = m_startPoint.z + m_directionSpeed.z * timeSinceTriggeredInSeconds;
    }

    public void GetPosition(double timeSinceTriggeredInSeconds, out DoubleVector3 newPosition) {

        newPosition = new DoubleVector3(m_startPoint.x + m_directionSpeed.x * timeSinceTriggeredInSeconds,
            m_startPoint.y + m_directionSpeed.y * timeSinceTriggeredInSeconds,
            m_startPoint.z + m_directionSpeed.z * timeSinceTriggeredInSeconds); 
    }
    
}

[System.Serializable]
public struct DoubleVector3
{
    public double m_x;
    public double m_y;
    public double m_z;

    public DoubleVector3(double x, double y, double z)
    {
        m_x = x;
        m_y = y;
        m_z = z;
    }
}