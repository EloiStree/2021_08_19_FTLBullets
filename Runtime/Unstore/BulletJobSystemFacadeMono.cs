using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Jobs;

public class BulletJobSystemFacadeMono : MonoBehaviour, IBulletJobSystemFacade
{
    public uint m_numberOfBullets;
    public BulletsPoolMovingComputeJobWrapper m_jobPositioning;

    public NABulletsDataRef m_bulletsInfoInMemoryRef;
    public NABulletsTransformRef m_transformInMemoryRef;

    public JobCompute_MovingBulletsWrapper    m_bulletsJobManager;
    public JobCompute_TransformBulletsWrapper m_transformJobManager;

    public NextBulletAvailableSeeker m_bulletSeeker;
    JobComputeExe_MovingBullets m_bulletMovingExe;
    JobComputeExe_DefaultBulletsToTransform m_bulletTransformExe;
    public TriggeredBulletData m_bulletInit;
    public BulletDataResult m_bulletResult;
    public Transform m_linkedTransform;

    public BulletsRayCastingMono m_raycastManager;
    public BulletsToMeshColliderJobSystem m_bulletsToMesh;
    public JobBulletsFilteringMono m_jobBulletsFilter;

    public UnityEvent m_beforeComputingStart;
    public UnityEvent m_afterPositionComputing;
    public UnityEvent m_afterTransformComputing;


    private bool m_isInit;
    private void Awake()
    {
        m_bulletsInfoInMemoryRef = new NABulletsDataRef(m_numberOfBullets);
        m_transformInMemoryRef = new NABulletsTransformRef(m_numberOfBullets);
        m_bulletsInfoInMemoryRef.GetNativeArrayBulletData(out NativeArray<TriggeredBulletData> bullets);
        m_bulletsInfoInMemoryRef.GetNativeArrayBulletData(out NativeArray<BulletDataResult> bulletsResult);
         m_bulletMovingExe = new JobComputeExe_MovingBullets();
         m_bulletTransformExe = new JobComputeExe_DefaultBulletsToTransform();
        m_bulletsToMesh.SetBulletsMemory(bulletsResult);
        m_bulletMovingExe.SetSharedMemory(bullets, bulletsResult);
        m_bulletTransformExe.SetSharedMemory(bulletsResult);
        m_jobBulletsFilter.SetBulletsNativeArray(bulletsResult);

        m_bulletsJobManager = new JobCompute_MovingBulletsWrapper(m_bulletMovingExe);
        m_transformJobManager = new JobCompute_TransformBulletsWrapper(m_bulletTransformExe);
        m_bulletSeeker = new NextBulletAvailableSeeker();


        m_bulletSeeker.SetMemoryArrayUsed(bullets);
        m_bulletsJobManager.SetSharedMemory(bullets);
        m_bulletsJobManager.SetSharedMemory(bulletsResult);

        m_transformInMemoryRef.GetTransformAccesArray(out TransformAccessArray transformsMemory);
        m_transformJobManager.SetSharedMemory(transformsMemory);
        if(m_raycastManager)
        m_raycastManager.SetMemorySharing(bulletsResult);
        m_isInit = true;
    }

    

    public void GetInfoAboutBullet(IBulletIdTicket id, out TriggeredBulletData initValue, out BulletDataResult result, out Transform linkedTransform)
    {
        if (id != null && m_isInit)
        {
            id.GetId(out int idValue);
            m_bulletsInfoInMemoryRef.GetBullet(idValue, out initValue);
            m_bulletsInfoInMemoryRef.GetBulletResult(idValue, out result);
            //m_transformInMemoryRef.GetTransfromAt(idValue, out linkedTransform);
            linkedTransform = null;
        }
        else
        {
            initValue = new TriggeredBulletData();
            result = new BulletDataResult();
            linkedTransform = null;
        }
    }

    public void SpawnBullet(Vector3 position, Vector3 directionSpeed, Transform proposedView, out IBulletIdTicket ticket)
    {
        float gameTime = Time.time;
        m_bulletSeeker.GetNextAvailableBulletId(out ticket);
        ticket.GetId(out int id);
        m_bulletsInfoInMemoryRef.SetBullet(id,
            new TriggeredBulletData()
            {
                m_isActive = true,
                m_gameTimeWhenTriggerInSeconds = gameTime,
                m_bulletInfo = new BulletDataInitValue()
                {
                    m_startPoint = position,
                    m_directionSpeed= directionSpeed
                }
            }
            );
        m_transformInMemoryRef.SetTransform(id, proposedView);
    }
    internal void ForceSetBullet(int id, Vector3 position, Vector3 directionSpeed, Transform proposedView)
    {
        float gameTime = Time.time;
        m_bulletsInfoInMemoryRef.SetBullet(id,
             new TriggeredBulletData()
             {
                 m_isActive = true,
                 m_gameTimeWhenTriggerInSeconds = gameTime,
                 m_bulletInfo = new BulletDataInitValue()
                 {
                     m_startPoint = position,
                     m_directionSpeed = directionSpeed
                 }
             }
             );
        m_transformInMemoryRef.SetTransform(id, proposedView);
    }

    public void Update()
    {
        m_beforeComputingStart.Invoke(); 
        m_bulletsJobManager.ComputeBulletNewPositions(Time.time);
        m_afterPositionComputing.Invoke(); 
        m_transformJobManager.StartComputingTransformPosition();
    }
    public void LateUpdate()
    {

        m_transformJobManager.ApplyTransformPosition();
        m_afterTransformComputing.Invoke();
        m_bulletsInfoInMemoryRef.GetBullet(0, out m_bulletInit);
        m_bulletsInfoInMemoryRef.GetBulletResult(0, out m_bulletResult);
        m_transformInMemoryRef.GetTransfromAt(0, out m_linkedTransform);
       

    }

}


public interface IBulletJobSystemFacade {

    void SpawnBullet(Vector3 position, Vector3 directionSpeed, Transform proposedView, out IBulletIdTicket ticket);

}

public class NextBulletAvailableSeeker {

    public int m_cyclingIndex;
    public uint m_bulletsCount;
    public NativeArray<TriggeredBulletData> m_bullets;

    public void SetMemoryArrayUsed(NativeArray<TriggeredBulletData> array) {
        m_bullets = array;
        m_bulletsCount = (uint) array.Length;
    }

    public void GetNextAvailableBulletId(out IBulletIdTicket bulletId)
    {
        bulletId = null;
        int antiLoop = 0;
        while (antiLoop < m_bulletsCount)
        {
            if (!m_bullets[m_cyclingIndex].m_isActive)
            {
                bulletId = new BulletTicketId(m_cyclingIndex);
                m_cyclingIndex++;
                return;
            }
            else
            {
                m_cyclingIndex++;
            }
            antiLoop++;
        }
    }
}