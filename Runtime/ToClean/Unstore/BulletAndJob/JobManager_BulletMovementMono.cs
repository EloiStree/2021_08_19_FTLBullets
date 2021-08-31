using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;


public interface BulletsPositioningManager {

    void GetMaxIdValue(out uint maxIdValue);
    void SetBulletInitialValue(IBulletIdTicket id, IBulletInitValue initialValue); 
    void SetBulletAsActive(IBulletIdTicket id, bool isActive);

    void GetBulletInfo(IBulletIdTicket id, out IBulletInitValue initialValue);
    void GetBulletInfo(IBulletIdTicket id, out IBulletPositionState positionState);
    void GetBulletInfo(IBulletIdTicket id, out IBulletInitValue initialValue, out IBulletPositionState positionState);

}


public class DragAndDropUnityTransformsMono : MonoBehaviour {

    public uint m_startIndex=0;
    [SerializeField] DragAndDropUnityTransforms m_dragAndDropTransforms;
}

[System.Serializable]
public class DragAndDropUnityTransforms {
    public Transform[] m_groupOfTransforms;

}


//public class JobManager_BulletMovementMono : MonoBehaviour
//{

//    public int m_bulletCount=50000;
//    public BulletsPoolMovingComputeJobWrapper m_bulletsManager ;
//  //  public bullet
//    public Transform m_testBulletPosition;
//    public TriggeredBulletData m_tbi;
//    public BulletDataResult m_tbr;
//    BulletInfoAndBasicAccessRef m_bulletRef;

//    public  TransformAccessArray m_TransformsAccessArray;
//    private void Awake()
//    {
//        m_bulletsManager = new BulletsPoolMovingComputeJobWrapper(m_bulletCount);
//       // m_bulletsTransformManager.SetTranformSource(IBulletVisualInUnity);


//        m_bulletsManager.GetBulletReference(0, out m_bulletRef);
//        m_bulletRef.SetAllInOne(true, Vector3.zero, Vector3.up, 0);
//        m_testBulletPosition.forward = Vector3.up;
//        for (int i = 0; i < m_bulletsManager.m_bulletCount; i++)
//        {
//            m_bulletsManager.m_pool.SetBullet(i, new TriggeredBulletData()
//            {
//                m_isActive = true,
//                m_bulletInfo = new BulletDataInitValue()
//                {
//                    m_startPoint = Vector3.zero,
//                    m_directionSpeed = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value) * UnityEngine.Random.value * 4f
//                }
//            }); 
//        }
//    }

//    private void Update()
//    {
//        m_bulletsManager.m_pool.SetCurrentTime(Time.time);
//        m_bulletsManager.RefreshPosition();
        
//    }
//    private void LateUpdate()
//    {
//        m_bulletsManager.m_pool.GetBullet(0, out m_tbi);
//        m_bulletsManager.m_pool.GetBulletResult(0, out m_tbr);
//        m_bulletRef.GetTajectory(out Vector3 from, out Vector3 to);
//        m_testBulletPosition.position = 
//            new Vector3( (float)to.x, (float)to.y, (float)to.z);
//    }

//    private void OnDestroy()
//    {
//        m_bulletsManager.ForceDispose();
//    }
//    private void OnApplicationQuit()
//    {
//        m_bulletsManager.ForceDispose();
//    }
//}


//public interface BulletClaimerPool {

//    void Claim(out int bulletIndex, out ClaimedBullet bullet);
//    void Unclaim(out int bulletIndex);

//}
//public interface ClaimedBullet {
//    void GetBulletInfo(out ComputedBulletInfo info);
//    void Unclaimed();
//}
public interface ComputedBulletInfo
{

    void GetStartPoint(out Vector3 start);
    void GetDirection(out Vector3 direction);
    void GetCurrentPosition(out Vector3 position);
    void GetPreviousPosition(out Vector3 position);
    void GetTajectory(out Vector3 previousPosition, out Vector3 currentPosition);
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
//public class BulletPoolReference : BulletInfoAndBasicAccessRef
//{
//    public int m_bulletIndex;
//    BulletsPoolMovingComputeJobWrapper m_bulletsPool;

//    public BulletPoolReference(int bulletIndex, BulletsPoolMovingComputeJobWrapper bulletsPool)
//    {
//        m_bulletIndex = bulletIndex;
//        m_bulletsPool = bulletsPool;
//    }

//    public void GetCurrentPosition(out Vector3 position)
//    {
//        m_bulletsPool.m_pool.GetBulletResult(m_bulletIndex, out BulletDataResult result);
//        position = result.m_currentPosition;
//    }
//    public void GetPreviousPosition(out Vector3 position)
//    {
//        m_bulletsPool.m_pool.GetBulletResult(m_bulletIndex, out BulletDataResult result);
//        position = result.m_currentPosition;
//    }
//    public void GetTajectory(out Vector3 previousPosition, out Vector3 currentPosition)
//    {
//        m_bulletsPool.m_pool.GetBulletResult(m_bulletIndex, out BulletDataResult result);
//        previousPosition = result.m_previousPosition;
//        currentPosition = result.m_currentPosition;
//    }
//    public void GetDirection(out Vector3 direction)
//    {
//        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out TriggeredBulletData result);
//        direction = result.m_bulletInfo.m_directionSpeed;
//    }

    

//    public void GetStartPoint(out Vector3 start)
//    {
//        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out TriggeredBulletData result);
//        start = result.m_bulletInfo.m_startPoint;
//    }

//    public void SetAsUsing(bool isUsing)
//    {
//        m_bulletsPool.m_pool.SetBulletAs(m_bulletIndex, isUsing);
//    }

//    public void SetStartInfo(Vector3 startPoint, Vector3 directionSpeed)
//    {
//        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out TriggeredBulletData result);
//        result.m_bulletInfo.m_startPoint = startPoint;
//        result.m_bulletInfo.m_directionSpeed = directionSpeed;
//        m_bulletsPool.m_pool.SetBullet(m_bulletIndex, result);
//    }
   

//    public void SetTimeWhenTriggered(float triggeredTimeInSeconds)
//    {
//        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out TriggeredBulletData result);
//        result.m_gameTimeWhenTriggerInSeconds = triggeredTimeInSeconds;
//        m_bulletsPool.m_pool.SetBullet(m_bulletIndex, result);
//    }

//    public void SetAllInOne(bool isUsing, Vector3 startPoint, Vector3 directionSpeed, float triggeredTimeInSeconds)
//    {
//        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out TriggeredBulletData result);
//        result.m_bulletInfo.m_startPoint = startPoint;
//        result.m_bulletInfo.m_directionSpeed = directionSpeed;
//        result.m_gameTimeWhenTriggerInSeconds  = triggeredTimeInSeconds;
//        result.m_isActive = isUsing;
//        m_bulletsPool.m_pool.SetBullet(m_bulletIndex, result);
//    }

//    public void GetLifeTime(out float lifeTimeInSeconds)
//    {
//        m_bulletsPool.m_pool.GetBullet(m_bulletIndex, out TriggeredBulletData result);
//        lifeTimeInSeconds = result.m_gameTimeWhenTriggerInSeconds;
//    }
//}


//public class BulletsPoolMovingComputeJobWrapper {
//    public BulletsPoolMovingComputeJob m_pool;
//    public int m_bulletCount;
//    public Transform[] m_transformsByBulletsId;
//    TransformAccessArray targets;
    
//    public uint m_cyclingIndex;
//    public BulletsPoolMovingComputeJobWrapper(int bulletCount)
//    {
//        m_bulletCount = bulletCount;
//        m_transformsByBulletsId = new Transform[bulletCount];
//        targets = new TransformAccessArray(m_transformsByBulletsId);
//        m_pool.InitNumberOfBullet(bulletCount);
//        RefreshTransformTargets();
//    }
//    public BulletsPoolMovingComputeJobWrapper(int bulletCount, NativeArray<BulletDataResult> linkedData)
//    {
//        m_bulletCount = bulletCount;
//        m_transformsByBulletsId = new Transform[bulletCount];
//        targets = new TransformAccessArray(m_transformsByBulletsId);
//        m_pool.InitNumberOfBullet(bulletCount, linkedData);
//        RefreshTransformTargets();
//    }

//    public void RefreshPosition() {
//        JobHandle handler = m_pool.Schedule(m_bulletCount, 64);
//        handler.Complete();
//    }

//    public void GetBulletReference(int bulletIndex, out BulletInfoAndBasicAccessRef bulletRef)
//    {
//        bulletRef = new BulletPoolReference(bulletIndex, this);
//    }
//    public void ForceDispose()
//    {
//        m_pool.DisposeMemoryUse(); targets.Dispose();
//    }
//    public void RefreshTransformTargets()
//    {
//        targets.SetTransforms(m_transformsByBulletsId);
//    }
//    JobHandle transformJobHandler;
//    private bool mtransformJobStarted=false;
//    private bool wasComputedOnce;
//    public void StartRefreshTransformPosition()
//    {
//        transformJobHandler = m_pool.Schedule(targets);
//        mtransformJobStarted = true;
//    }
//    public void StopRefreshTransformPosition()
//    {
//        if (mtransformJobStarted) { 
//             transformJobHandler.Complete();
//            wasComputedOnce = true;
//        }
//    }
//    public bool WasTransformComputedOnce()
//    {
//        return wasComputedOnce;
//    }

//    public void SetTranformView(IBulletIdTicket bulletId, Transform proposedView)
//    {
//        bulletId.GetId(out int id);
//        if (id >= 0 && id < m_transformsByBulletsId.Length)
//        {
//            m_transformsByBulletsId[id] = proposedView;
//            targets[id] = proposedView;
//        }
//    }

//    public void GetNextAvailableBulletId(out IBulletIdTicket bulletId)
//    {
//        bulletId = null;
//        int antiLoop=0;
//        while( antiLoop<m_bulletCount) {
//            if (!m_pool.IsUsed(m_cyclingIndex))
//            {
//                bulletId = new BulletTicketId(m_cyclingIndex);
//                m_cyclingIndex++;
//                return;
//            }
//            else {
//                m_cyclingIndex++;
//            }
//            antiLoop++;
//        }
//        Debug.Log("Yoo "+ antiLoop);
//    }
//    public void GetLinkedTransform(IBulletIdTicket bulletId, out Transform linkedTransform)
//    {
//        bulletId.GetId(out int id);
//        if (id >= 0 && id < m_transformsByBulletsId.Length)
//        {
//            linkedTransform = m_transformsByBulletsId[id];
//        }
//        else
//        {
//            linkedTransform = null;
//        }
//    }

   

//    ~BulletsPoolMovingComputeJobWrapper()
//    {
//        m_pool.DisposeMemoryUse();
//    }
//}

[System.Serializable]
public struct BulletTicketId : IBulletIdTicket
{
    [SerializeField] uint m_id;

    public BulletTicketId(uint id)
    {
        this.m_id = id;
    }
    public BulletTicketId(int id)
    {
        this.m_id =(uint) id;
    }
    public uint GetIdAsUInt()
    {
        return m_id;
    }
    public int GetIdAsInt()
    {
        return (int)m_id;
    }
    public void GetId(out int id)
    {
        id = (int) m_id;
    }
    public void GetId(out uint id)
    {
        id = m_id;
    }
}




//[BurstCompile]
[BurstCompile(CompileSynchronously = true)]
public struct BulletsPoolMovingComputeJob : IJobParallelFor , IJobParallelForTransform{


    public float m_previousGameTimeInSeconds;
    public float m_currentGameTimeInSeconds;
    public NativeArray<TriggeredBulletData> m_poolOfBullets;
    public NativeArray<BulletDataResult> m_poolOfBulletsPosition;
    private bool m_disposable;
    public void InitNumberOfBullet(int bulletsCount) 
    {
        InitNumberOfBullet(bulletsCount, new NativeArray<BulletDataResult>(bulletsCount, Allocator.Persistent));
    }
    public void InitNumberOfBullet(int bulletsCount, NativeArray<BulletDataResult> poolOfBulletsPosition)
    {
        if (!m_disposable)
        {
            m_disposable = true;
            m_poolOfBullets = new NativeArray<TriggeredBulletData>(bulletsCount, Allocator.Persistent);
            m_poolOfBulletsPosition = poolOfBulletsPosition;
        }
    }

    public void RaycastBullet(int id, LayerMask mask, out RaycastHit [] hits) {
        BulletDataResult d = m_poolOfBulletsPosition[id];
        d.GetDirection(out Vector3 origine, out Vector3 direction, out float distance);
        hits = Physics.RaycastAll(origine, direction, distance, mask, QueryTriggerInteraction.Ignore);
    }


    public void GetCopyOfComputedBulletsPositionResult( ref NativeArray<BulletDataResult> whereToCopy) {
        m_poolOfBulletsPosition.CopyTo(whereToCopy) ;
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
        TriggeredBulletData bullet = m_poolOfBullets[index];
        bullet.m_isActive = isUsed;
        m_poolOfBullets[index] = bullet;
    }

    public void SetBullet(int index, TriggeredBulletData bullet)
    {
        m_poolOfBullets[index] = bullet;
       

    }
    public void GetBullet(int index, out TriggeredBulletData bullet)
    {
        bullet = m_poolOfBullets[index];
    }
    public void GetBulletResult(int index, out BulletDataResult bullet)
    {
        bullet = m_poolOfBulletsPosition[index];
    }

    public void DisposeMemoryUse() {
        if (m_disposable) {
            m_disposable = false;
            if (m_poolOfBullets != null)
            {
                m_poolOfBullets.Dispose();
            }
            if (m_poolOfBulletsPosition != null)
            {
                m_poolOfBulletsPosition.Dispose();
            }
        }
    }


    public void Execute(int index)
    {
        if (m_poolOfBullets[index].m_isActive) {

            
            float t = m_currentGameTimeInSeconds - m_poolOfBullets[index].m_gameTimeWhenTriggerInSeconds;
            if (t > m_poolOfBullets[index].m_bulletInfo.m_lifeTime)
            {
                TriggeredBulletData iniInfo= m_poolOfBullets[index];
                iniInfo.m_isActive = false;
                m_poolOfBullets[index]= iniInfo;

                BulletDataResult result = m_poolOfBulletsPosition[index];
                result.m_isUsed = false;
                result.m_lifeTimeInSeconds = t;
                result.m_previousPosition =Vector3.zero;
                result.m_currentPosition = Vector3.zero;
                m_poolOfBulletsPosition[index] = result;
            }
            else {
                BulletDataResult result = m_poolOfBulletsPosition[index];
                result.m_isUsed = m_poolOfBullets[index].m_isActive;
                result.m_previousPosition = result.m_currentPosition;
                result.m_lifeTimeInSeconds = t;

                Vector3 v = m_poolOfBulletsPosition[index].m_currentPosition;
                m_poolOfBullets[index].m_bulletInfo.GetPositionIn(t, ref v);
                result.m_currentPosition = v;
                m_poolOfBulletsPosition[index] = result;
            }
        


        }
    }

    public bool IsUsed(uint index)
    {
        return m_poolOfBullets[(int)index].m_isActive;
    }
    public bool IsUsed(int index)
    {
        return m_poolOfBullets[index].m_isActive;
    }


    public bool m_takeInChargeRotation;
    public bool m_useLocalPosition;

   
  
    public void Execute(int index, TransformAccess transform)
    {
        if (!transform.isValid || !m_poolOfBulletsPosition[index].m_isUsed)
            return;
        Vector3 c = m_useLocalPosition ? transform.localPosition : transform.position;

        c.x = m_poolOfBulletsPosition[index].m_currentPosition.x;
        c.y = m_poolOfBulletsPosition[index].m_currentPosition.y;
        c.z = m_poolOfBulletsPosition[index].m_currentPosition.z;

        if (m_takeInChargeRotation)
        {

            Vector3 previous = m_useLocalPosition ? transform.localPosition : transform.position;
            Vector3 dir = c - previous;
            Quaternion orientation = Quaternion.LookRotation(dir, Vector3.up);

            if (m_useLocalPosition)
                transform.localRotation = orientation;
            else
                transform.rotation = orientation;
        }

        if (m_useLocalPosition)
            transform.localPosition = c;
        else
            transform.position = c;
    }

    
}

[System.Serializable]
public struct BulletDataResult {
    public bool m_isUsed;
    public Vector3 m_previousPosition;
    public Vector3 m_currentPosition;
    public float m_lifeTimeInSeconds;

    public void GetDirection(out Vector3 origine, out Vector3 direction, out float distance)
    {
        origine = m_previousPosition;
        direction = m_currentPosition - m_previousPosition;
        distance = direction.magnitude;
    }

    internal void ResetAsAvailaible()
    {

         m_isUsed = false ;
         m_previousPosition = Vector3.zero;
         m_currentPosition = Vector3.zero; 
         m_lifeTimeInSeconds=0;


    }
}

[System.Serializable]
public struct TriggeredBulletData {

    public bool m_isActive;
    public float m_gameTimeWhenTriggerInSeconds;
    public BulletDataInitValue m_bulletInfo;

    internal void ResetAsAvailaible()
    {
        m_isActive = false;
        m_gameTimeWhenTriggerInSeconds = 0;
    }
}

[System.Serializable]
public struct BulletDataInitValue {

    public Vector3 m_startPoint;
    public Vector3 m_directionSpeed;
    public float m_lifeTime;
    public float m_radius;

    public void GetPositionIn(float timeSinceTriggeredInSeconds, ref Vector3 newPosition)
    {

        newPosition.x = m_startPoint.x + m_directionSpeed.x * timeSinceTriggeredInSeconds;
        newPosition.y = m_startPoint.y + m_directionSpeed.y * timeSinceTriggeredInSeconds;
        newPosition.z = m_startPoint.z + m_directionSpeed.z * timeSinceTriggeredInSeconds;
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