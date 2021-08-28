using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class JobBulletsFilteringMono : MonoBehaviour
{
    JobComputeExe_FilterBulletsByDistance m_jobsFiltering;
    public float m_maxRenderingAngle = 110;
    public float m_closeDistance = 10;
    public float m_mediumDistance = 150;
    public float m_farDistance = 5000;

    public Transform m_playerPointOfView;


    [Header("Debug")]
    public int m_closeCount;
    public int m_mediumCount;
    public int m_farCount;
    public int m_renderCount;
    public FilteredBulletsId m_filteredBulletsId = new FilteredBulletsId();
    [Range(0,1)]
    public float m_pourcentRender;

    public int m_bulletIndex;
    public BulletDataResult m_bulletResultInfo;
    public BulletRendering m_bulletRenderingInfo;

    

    public AbstractBulletsToRenderListener[] m_listener;

 

    public void SetBulletsNativeArray(NativeArray<BulletDataResult> bullets) {
        m_jobsFiltering = new JobComputeExe_FilterBulletsByDistance();
        m_jobsFiltering.SetFilterPreferences(m_closeDistance,
           m_mediumDistance,
           m_farDistance,
           m_maxRenderingAngle);
        m_jobsFiltering.SetNativeArrayOfBullets(bullets);
        for (int i = 0; i < m_listener.Length; i++)
        {
            m_listener[i].SetBulletsInformatoinRef(bullets, m_jobsFiltering.m_bulletRenderData);
        }
    }
    public void GetBulletsRenderingResult(out NativeArray<BulletRendering> bulletRenderData) {
        bulletRenderData = m_jobsFiltering.m_bulletRenderData;
    }
    public void ComputeRendering()
    {
        //SHOULD BE DEFINE BY USER OF THE SCRIPT
        // HERE JUST FOR TEST
        Vector3 direction = m_playerPointOfView.forward;
        Vector3 position = m_playerPointOfView.position;
        m_jobsFiltering.SetCameraLocalPosition(direction, position);
        ComputeRendering(direction, position);
        for (int i = 0; i < m_listener.Length; i++)
        {
            m_listener[i].ApplyComputeRendering( ref m_filteredBulletsId);
        }
    }
    public void ComputeRendering(Vector3 cameraDirection, Vector3 cameraPosition)
    {
        JobHandle job = m_jobsFiltering.Schedule(m_jobsFiltering.GetLenght(),64);
        job.Complete();
        m_filteredBulletsId.Clear();
        NativeArray<BulletRendering> bulletsRendering = m_jobsFiltering.m_bulletRenderData;
        m_renderCount = m_closeCount = m_mediumCount = m_farCount = 0;
        m_pourcentRender = 0;
        for (int i = 0; i < bulletsRendering.Length; i++)
        {
            if (bulletsRendering[i].m_canBeRender)
                m_renderCount++;
            BulletDistanceType dType = bulletsRendering[i].m_distanceType;
            if (dType == BulletDistanceType.Close)
            {
                m_filteredBulletsId.m_closeIds.Add(i);
                m_closeCount++;
            }
            else if (dType == BulletDistanceType.Medium)
            {

                m_filteredBulletsId.m_mediumIds.Add(i);
                m_mediumCount++;
            }
            else if (dType == BulletDistanceType.Far) {

                m_filteredBulletsId.m_farIds.Add(i);
                m_farCount++;
            }
        }
        m_pourcentRender = (float)m_renderCount / (float)bulletsRendering.Length;
        m_bulletResultInfo = m_jobsFiltering.m_bulletsRef[m_bulletIndex];
        m_bulletRenderingInfo = m_jobsFiltering.m_bulletRenderData[m_bulletIndex];
    }

    private void OnDestroy()
    {
        m_jobsFiltering.Dispose();
    }
}

public class FilteredBulletsId
{

    public List<int> m_closeIds = new List<int>();
    public List<int> m_mediumIds = new List<int>();
    public List<int> m_farIds = new List<int>();

    public void Clear()
    {
        m_closeIds.Clear();
        m_mediumIds.Clear();
        m_farIds.Clear();
    }


}

[BurstCompile(CompileSynchronously = true)]
public struct JobComputeExe_FilterBulletsByDistance : IJobParallelFor{

    public Vector3 m_cameraDirection;
    public Vector3 m_cameraPosition;

    public float m_maxRenderingAngle;
    public float m_maxHaflRenderingAngle;
    public float m_closeDistance ;
    public float m_mediumDistance ;
    public float m_farDistance ;

    public NativeArray<BulletDataResult> m_bulletsRef;
    public NativeArray<BulletRendering> m_bulletRenderData;

    public void SetCameraLocalPosition(Vector3 direction, Vector3 position) {
        m_cameraDirection = direction;
        m_cameraPosition = position;
    }
    public void SetFilterPreferences(float closeDistance, float mediumDistance, float farDistance, float cameraRenderingAngle) {

        m_closeDistance = closeDistance;
        m_mediumDistance = mediumDistance;
        m_farDistance = farDistance;
        m_maxRenderingAngle = cameraRenderingAngle;
        m_maxHaflRenderingAngle = m_maxRenderingAngle / 2f;
    }

    public void SetNativeArrayOfBullets(NativeArray<BulletDataResult> bullets)
    {
        m_bulletsRef = bullets;
        BulletRendering [] bulletsSpace = new BulletRendering[bullets.Length];
        m_bulletRenderData = new NativeArray<BulletRendering>(bulletsSpace, Allocator.Persistent);
    }

    public void Execute(int index)
    {
        BulletDataResult bd= m_bulletsRef[index];
        BulletRendering br = m_bulletRenderData[index];
        br.Reset();
        if (!bd.m_isUsed) {
            br.m_canBeRender = false;
            m_bulletRenderData[index] = br;
            return;
        }
        Vector3 directionFromCamera = bd.m_currentPosition - m_cameraPosition;
        Vector3 directionToCamera =  m_cameraPosition - bd.m_currentPosition;

        br.m_bulletId = index;
        br.m_distance = directionFromCamera.magnitude;
        if (br.m_distance > m_farDistance)
        {
            br.m_distanceType = BulletDistanceType.TooFar;
            br.m_canBeRender = false;
            m_bulletRenderData[index] = br;
            return;
        }
        br.m_outOfAngle = Vector3.Angle(m_cameraDirection, directionFromCamera) > m_maxHaflRenderingAngle;
        if (br.m_outOfAngle)
        {
            br.m_canBeRender = false;
            m_bulletRenderData[index] = br;
            return;
        }
        

        if (br.m_distance > m_mediumDistance)
        {
            br.m_distanceType = BulletDistanceType.Far;
        }
        else if (br.m_distance > m_closeDistance)
        {
            br.m_distanceType = BulletDistanceType.Medium;
        }
        else { 
            br.m_distanceType = BulletDistanceType.Close;
        }
        br.m_cameraDirection = directionToCamera;


        br.m_canBeRender = true;
        m_bulletRenderData[index]= br;
    }

    public int GetLenght()
    {
        return m_bulletsRef.Length;
    }

    internal void Dispose()
    {
        m_bulletRenderData.Dispose();
    }
}

[System.Serializable]
public struct BulletRendering
{
    public bool m_canBeRender;
    public BulletDistanceType m_distanceType;
    public int m_bulletId;
    public float m_distance;
    public Vector3 m_cameraDirection;
    public bool m_outOfAngle;

    public void Reset() {
        m_canBeRender = false ;
         m_distanceType=BulletDistanceType.TooFar;
         m_bulletId=-1;
         m_distance=0;
         m_cameraDirection=Vector3.forward;
         m_outOfAngle=false;
    }
}
public enum BulletDistanceType { Close, Medium, Far, TooFar }