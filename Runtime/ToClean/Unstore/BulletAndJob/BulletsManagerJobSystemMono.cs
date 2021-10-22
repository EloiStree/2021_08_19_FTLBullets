using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Jobs;

public class BulletsManagerJobSystemMono : AbstractBulletsManager, IBulletJobSystemFacade
{


    [Header("Setup")]
    [Space(20)]


    public uint m_numberOfBullets;
    

    public NABulletsDataRef m_bulletsInfoInMemoryRef;
    public NACapsuleLineDataRef m_capsuleLineInMemoryRef;
    public NABulletsTransformRef m_transformInMemoryRef;

    public JobCompute_MovingBulletsWrapper    m_bulletsJobManager;
    public JobCompute_TransformBulletsWrapper m_transformJobManager;

    public NextBulletAvailableSeeker m_bulletSeeker;
    JobComputeExe_MovingBullets m_bulletMovingExe;
    JobComputeExe_DefaultBulletsToTransform m_bulletTransformExe;
    public TriggeredBulletData m_bulletInit;
    public BulletDataResult m_bulletResult;
    public Transform m_linkedTransform;

    public bool m_useBulletToMesh;
    public BulletsToMeshColliderJobSystem m_bulletsToMesh;
    public JobBulletsFilteringMono m_jobBulletsFilter;
    public Experiment_JobComputeBulletCapsuleLine m_jobCapsuleComputing;
    public Experiment_BulletsListToPixelFarMeshMono m_bulletsFarMeshRendering;

    public UnityEvent m_beforeComputingStart;
    public UnityEvent m_afterPositionComputing;
    public UnityEvent m_afterTransformComputing;


    private bool m_isInit;
    private void Start()
    {
        m_bulletsInfoInMemoryRef = new NABulletsDataRef(m_numberOfBullets);
        m_transformInMemoryRef = new NABulletsTransformRef(m_numberOfBullets);
        m_capsuleLineInMemoryRef = new NACapsuleLineDataRef(m_numberOfBullets);
        m_bulletsInfoInMemoryRef.GetNativeArrayBulletData(out NativeArray<TriggeredBulletData> bullets);
        m_bulletsInfoInMemoryRef.GetNativeArrayBulletData(out NativeArray<BulletDataResult> bulletsResult);
        m_capsuleLineInMemoryRef.GetNativeActiveLayer(out NativeBoolLayerMask capsuleLayer);
        m_capsuleLineInMemoryRef.GetNativeArrayCapsuleIne(out NativeArray<S_CapsuleLine> capsule);


        m_bulletMovingExe = new JobComputeExe_MovingBullets();
         m_bulletTransformExe = new JobComputeExe_DefaultBulletsToTransform();
        if(m_useBulletToMesh)
            m_bulletsToMesh.SetBulletsMemory(bulletsResult, bullets);
        m_bulletMovingExe.SetSharedMemory(bullets, bulletsResult, capsuleLayer, capsule);
        m_bulletTransformExe.SetSharedMemory(bulletsResult);
        m_jobBulletsFilter.SetBulletsNativeArray(bullets,bulletsResult);
        m_jobCapsuleComputing.SetBulletsRefWith(capsuleLayer, capsule);
        m_bulletsFarMeshRendering.SetBulletsRef(bulletsResult);

        m_bulletsJobManager = new JobCompute_MovingBulletsWrapper(m_bulletMovingExe);
        m_transformJobManager = new JobCompute_TransformBulletsWrapper(m_bulletTransformExe);
        m_bulletSeeker = new NextBulletAvailableSeeker();


        m_bulletSeeker.SetMemoryArrayUsed(bullets);
        m_bulletsJobManager.SetSharedMemory(bullets);
        m_bulletsJobManager.SetSharedMemory(bulletsResult);

        m_transformInMemoryRef.GetTransformAccesArray(out TransformAccessArray transformsMemory);
        m_transformJobManager.SetSharedMemory(transformsMemory);
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

    public int m_lastIdUsed;
    public int m_idInQueue;
    public void SpawnBullet(Vector3 position, Vector3 directionSpeed, float lifeTimeInSeconds, float radius, Transform customTransform, out IBulletIdTicket ticket)
    {
        Quaternion q = Quaternion.LookRotation(directionSpeed, Vector3.up);
        SpawnBullet(position, directionSpeed, q, lifeTimeInSeconds, radius, customTransform, out ticket);


    }
    public void SpawnBullet(Vector3 position, Vector3 directionSpeed, Quaternion initRotation, float lifeTimeInSeconds, float radius, Transform customTransform, out IBulletIdTicket ticket)
    {
        if (!m_isInit) {
            ticket = null;
            return ;
        }
        float gameTime = Time.time;
        m_bulletSeeker.GetNextAvailableBulletId(out ticket);
        ticket.GetId(out int id);
        m_lastIdUsed = id;

        m_idInQueue = m_bulletSeeker.m_shouldBeAvailaible.Count;

        m_bulletsInfoInMemoryRef.SetBullet(id,
            new TriggeredBulletData()
            {
                m_isActive = true,
                m_gameTimeWhenTriggerInSeconds = gameTime,


                m_bulletInfo = new BulletDataInitValue()
                {

                    m_startPoint = position,
                    m_directionSpeed = directionSpeed,
                    m_rotation = initRotation,
                    m_lifeTime = lifeTimeInSeconds,
                    m_radius = radius
                }
            }
            );


        if (customTransform != null)
            m_transformInMemoryRef.SetTransform(id, customTransform);
    }

    internal void ForceSetBullet(int id, Vector3 position, Vector3 directionSpeed, float lifeTimeInSeconds, float radius, Transform proposedView)
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
                     ,
                     m_lifeTime = lifeTimeInSeconds,
                     m_radius = radius
                 }
             }
             );
        m_transformInMemoryRef.SetTransform(id, proposedView);
    }
    public void DeactivateBullet(int id) {

        m_bulletsInfoInMemoryRef.SetBulletAs(id, false);
        m_bulletSeeker.EnqueuAvailaibleId(id);
    }


    public void Update()
    {
        if (!m_isInit)
            return;
        m_beforeComputingStart.Invoke(); 
        m_bulletsJobManager.ComputeBulletNewPositions(Time.time);
        m_afterPositionComputing.Invoke(); 
        m_transformJobManager.StartComputingTransformPosition();
    }
    public void LateUpdate()
    {
        if (!m_isInit)
            return;

        m_transformJobManager.ApplyTransformPosition();
        m_afterTransformComputing.Invoke();
        m_bulletsInfoInMemoryRef.GetBullet(0, out m_bulletInit);
        m_bulletsInfoInMemoryRef.GetBulletResult(0, out m_bulletResult);
        m_transformInMemoryRef.GetTransfromAt(0, out m_linkedTransform);
       

    }

    public override void GetBulletInfo(uint id, ComputedBulletInfo bulletInfo)
    {
        throw new NotImplementedException();
    }

    public override void RequestBulletFire(Vector3 startPoint, Vector3 direction, float speedInUnityPerSecond, float lifeTimeInSeconds, float radius, out IBulletIdTicket ticket)
    {
        SpawnBullet(startPoint, direction * speedInUnityPerSecond, lifeTimeInSeconds, radius, null, out ticket);
    }

    public override void NotifyBulletAsNotUsedAnymore(IBulletIdTicket bulletTicket)
    {
        bulletTicket.GetId(out int id);
        DeactivateBullet(id);

    }

    public override void RequestBulletFire(Vector3 startPoint, Vector3 direction, Quaternion initRotation, float speedInUnityPerSecond, float lifeTimeInSeconds, float radius, out IBulletIdTicket ticket)
    {
        SpawnBullet(startPoint, direction * speedInUnityPerSecond, initRotation, lifeTimeInSeconds, radius, null, out ticket);

    }
}


public interface IBulletJobSystemFacade {

    void SpawnBullet(Vector3 position, Vector3 directionSpeed, float timeInSeconds, float radius, Transform proposedView, out IBulletIdTicket ticket);
    void SpawnBullet(Vector3 position, Vector3 directionSpeed, Quaternion initRotation, float timeInSeconds, float radius, Transform proposedView, out IBulletIdTicket ticket);

}

public class NextBulletAvailableSeeker {

    public int m_cyclingIndex;
    public uint m_bulletsCount;
    public NativeArray<TriggeredBulletData> m_bullets;


    public void SetMemoryArrayUsed(NativeArray<TriggeredBulletData> array) {
        m_bullets = array;
        m_bulletsCount = (uint)array.Length;
    }
   

    public void GetNextAvailableBulletId(out IBulletIdTicket bulletId)
    {
        bulletId = null;
        int idShouldBeOk;
        
        while (m_shouldBeAvailaible.Count>0)
        {
            idShouldBeOk = m_shouldBeAvailaible.Dequeue();
            if (!m_bullets[idShouldBeOk].m_isActive)
            {
                bulletId = new BulletTicketId(idShouldBeOk);
                return;
            }
        }

        int antiLoop = 0;
        while (antiLoop < m_bulletsCount)
        {
            if (m_cyclingIndex >= m_bulletsCount)
                m_cyclingIndex = 0;

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
        throw new OutOfAmmoExeception(this, "You are apparently our to ammo in your pool. Tweak your level design or give more bullets to the player at game start.");
    }
    public Queue<int> m_shouldBeAvailaible = new Queue<int>();
    public void EnqueuAvailaibleId(int id)
    {
        m_shouldBeAvailaible.Enqueue(id);
    }

    public class OutOfAmmoExeception : Exception
    {
        public NextBulletAvailableSeeker m_bulletsSeekerState;
        public OutOfAmmoExeception(NextBulletAvailableSeeker seeker,string message) : base(message)
        {
            m_bulletsSeekerState = seeker;
        }
    }
}