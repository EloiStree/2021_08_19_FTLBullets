
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class PrefabPoolingToRenderBulletsDefault : AbstractBulletsToRenderListener
{


    public AbstractTransformRepresentationOfBullets m_bulletTransformProvider;
    public NativeArray<TransformClaim> m_transformClaimRegistered;
   
    public Dictionary<int, int> m_bulletToClaimHolderTransform = new Dictionary<int, int>();
    public int[] m_previousBulletId;
    public int[] m_currentBulletId;
    public int[] m_newId;
    public int[] m_lostId;


    public RefreshTransformNearPrefab m_job;


    public override void ApplyComputeRendering(ref FilteredBulletsId bulletsToProcess)
    {
        m_previousBulletId = m_currentBulletId;
        m_currentBulletId = bulletsToProcess.m_closeIds.ToArray();
        m_lostId = m_previousBulletId.Except(m_currentBulletId).ToArray();
        m_newId = m_currentBulletId.Except(m_previousBulletId).ToArray();


        for (int i = 0; i < m_lostId.Length; i++)
        {
            int bulletId = m_lostId[i];
            if (m_bulletToClaimHolderTransform.ContainsKey(bulletId))
            {
                int claimId = m_bulletToClaimHolderTransform[bulletId];
                //Send interface message to disable object;
                m_transformClaimRegistered[claimId] = new TransformClaim() { m_bulletId = -1, m_transformArrayIndex = claimId };
                DisableGameObject(claimId);
            }
            else
            {
              //  throw new Exception("Can happend if limited exceed and so an had not been add");
            }
        }
        for (int i = 0; i < m_newId.Length; i++)
        {
            int bulletId = m_newId[i];
            if (m_bulletToClaimHolderTransform.ContainsKey(bulletId))
            {
                throw new Exception("Should not happend as it is a new id and I remove the lost id");
            }
            else {
                if (GetUnclaimedId(out int claimerId)) {
                    m_bulletToClaimHolderTransform.Add(bulletId, claimerId);
                    m_transformClaimRegistered[claimerId]= new TransformClaim() { m_bulletId= bulletId, m_transformArrayIndex= claimerId };
                    EnableGameObject(claimerId);
                }
            }
        }

        m_bulletTransformProvider.GetNativeTransformArrayRef(out TransformAccessArray naRef);
        JobHandle jobHandle =  m_job.Schedule<RefreshTransformNearPrefab>(naRef);
        jobHandle.Complete();


    }
    private void EnableGameObject(int claimerId)
    {
         m_bulletTransformProvider.SetAsUsing(claimerId, true);
    }

    private void DisableGameObject(int claimerId)
    {
        m_bulletTransformProvider.SetAsUsing(claimerId, false);
    }

    private bool GetUnclaimedId(out int claimerId)
    {
        claimerId = -1;
        for (int i = 0; i < m_transformClaimRegistered.Length; i++)
        {
            if (!m_transformClaimRegistered[i].IsClaiming()) { 
                claimerId = i;
                return true; 
            }
        }
        return false;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public override void InitWithCount(int count)
    {
        m_bulletTransformProvider.GetMaxiumAllow(out int maxPrefab);
        TransformClaim[] t = new TransformClaim[maxPrefab]; 
        for (int i = 0; i < maxPrefab; i++)
        {
            t[i].SetClaimIndex(i);
            t[i].SetBulletIdClaimed(-1);
        }
        m_transformClaimRegistered = new NativeArray<TransformClaim>(t, Allocator.Persistent);
       
        m_job = new RefreshTransformNearPrefab();
        m_job.Set(m_bulletsRenderingRef);
        m_job.Set(m_bulletsRef);
        m_job.Set(m_bulletInitRef);
        m_job.Set(m_transformClaimRegistered);
    }

    public void Dispose()
    {
        if (m_transformClaimRegistered.IsCreated)
            m_transformClaimRegistered.Dispose();
    }


   
}
[BurstCompile]
public struct RefreshTransformNearPrefab : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<BulletRendering> m_bulletWantedRenderingInfo;
    [NativeDisableParallelForRestriction]
    public NativeArray<BulletDataResult> m_bulletStateResultInfo;
    [NativeDisableParallelForRestriction]
    public NativeArray<TriggeredBulletData> m_bulletStateInitInfo;
    [NativeDisableParallelForRestriction]
    public NativeArray<TransformClaim> m_transformClaimed;

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


    public void Execute(int index, TransformAccess transform)
    {
        TransformClaim c = m_transformClaimed[index];
        int bulletId = c.m_bulletId;
        if (bulletId < 0)
            return;

        if (!m_bulletStateResultInfo[bulletId].m_isUsed)
            return;

        transform.position = m_bulletStateResultInfo[bulletId].m_currentPosition;
        float radius = m_bulletStateInitInfo[bulletId].m_bulletInfo.m_radius*2f;
        transform.localScale = new Vector3(radius, radius, radius);
        transform.rotation = Quaternion.LookRotation(m_bulletStateResultInfo[bulletId].m_currentPosition - m_bulletStateResultInfo[bulletId].m_previousPosition, Vector3.up);

    }

    public void Set(NativeArray<TransformClaim> transformClaimRegistered)
    {
        m_transformClaimed = transformClaimRegistered;
    }
}
public struct TransformClaim
{
    public int m_transformArrayIndex;
    public int m_bulletId;
  

    public void SetClaimIndex(int index)
    {
        m_transformArrayIndex = index;
    }
    public void SetUnclaimed()
    {
        m_bulletId = -1;
    }
    public void SetBulletIdClaimed(int bulletId)
    {
        m_bulletId = bulletId;
    }
    public bool IsClaiming() { return m_bulletId > -1; }

}


public interface ITransformRepresentationOfBullets {

     void GetMaxiumAllow(out int maxEntity);
    void GetTransformFor(int index, out Transform transform);
    void GetNativeTransformArrayRef(out TransformAccessArray nativeArrayRef);
}

public abstract class AbstractTransformRepresentationOfBullets : MonoBehaviour, ITransformRepresentationOfBullets
{
    public abstract void GetMaxiumAllow(out int maxEntity);
    public abstract void GetNativeTransformArrayRef(out TransformAccessArray nativeArrayRef);
    public abstract void GetTransformFor(int index, out Transform transform);
    public abstract void SetAsUsing(int index, bool usingTransform);
}