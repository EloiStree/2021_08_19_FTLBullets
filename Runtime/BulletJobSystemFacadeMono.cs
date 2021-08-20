using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class BulletJobSystemFacadeMono : MonoBehaviour, IBulletJobSystemFacade
{
    public uint m_numberOfBullets;
    public BulletsPoolMovingComputeJobWrapper m_jobPositioning;
    public BulletsMovedBulletsToTransformWrapper m_jobTransformPositioning;

   
    

    public BulletDataResult[] m_bulletResultData;
    public NativeArray<BulletDataResult> m_bulletsCurrentResult;

    private bool isInit;
    private void Awake()
    {
        m_jobPositioning = new BulletsPoolMovingComputeJobWrapper((int)m_numberOfBullets);
        m_jobTransformPositioning = new BulletsMovedBulletsToTransformWrapper((int)m_numberOfBullets);

        m_bulletResultData = new BulletDataResult[m_numberOfBullets];
        m_bulletsCurrentResult = new NativeArray<BulletDataResult>(m_bulletResultData, Allocator.Persistent);
        isInit = true;
    }
    public void GetInfoAboutBullet(IBulletIdTicket id, out BulletData initValue, out BulletDataResult result, out Transform linkedTransform)
    {
        if (id != null && isInit)
        {
            id.GetId(out int idValue);
            m_jobPositioning.m_pool.GetBullet(idValue, out initValue);
            m_jobPositioning.m_pool.GetBulletResult(idValue, out result);
            m_jobTransformPositioning.GetLinkedTransform(id, out linkedTransform);
        }
        else
        {
            initValue = new BulletData();
            result = new BulletDataResult();
            linkedTransform = null;
        }
    }

    public void SpawnBullet(Vector3 position, Vector3 directionSpeed, Transform proposedView)
    {
        float gameTime = Time.time;
        m_jobPositioning.GetNextAvailableBulletId(out IBulletIdTicket bulletId);
        bulletId.GetId(out int id);
        m_jobPositioning.m_pool.SetBullet(id,
            new BulletData()
            {
                m_isUsed = true,
                m_gameTimeWhenTriggerInSeconds = gameTime,
                m_bulletInfo = new LightSpeedBulletData()
                {
                    m_startPoint = position,
                    m_directionSpeed= directionSpeed
                }
            }
            );
        m_jobTransformPositioning.SetTranformView(bulletId, proposedView);
    }


    public void Update()
    {
        m_jobPositioning.m_pool.SetCurrentTime(Time.time);
        m_jobPositioning.RefreshPosition();

        //Not sure needed;
        m_jobPositioning.m_pool.GetCopyOfComputedBulletsPositionResult(ref m_bulletsCurrentResult);
        m_jobTransformPositioning.m_jobsResultToTransform.SetWithComputedResult(ref m_bulletsCurrentResult);
        m_jobTransformPositioning.RefreshTransformTargets();
        m_jobTransformPositioning.RefreshPosition();
    }

}


public interface IBulletJobSystemFacade {

    void SpawnBullet(Vector3 position, Vector3 directionSpeed, Transform proposedView);

}