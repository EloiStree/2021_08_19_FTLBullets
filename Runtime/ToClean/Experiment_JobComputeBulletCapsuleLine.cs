using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class Experiment_JobComputeBulletCapsuleLine : MonoBehaviour
{

    public delegate void CollisionListernWithShortenPath(S_CapsulesCollision collisionEmitted);
    public delegate void CollisionListernLight(int bulletId, int bulletableId);

    public CollisionListernLight m_listenToCollisionId;
    public CollisionListernWithShortenPath m_listenToCollisionShortenPath;

    public NativeBoolLayerMask m_activeBullets;
    public NativeArray<S_CapsuleLine> m_capsuleBullet;
    public NativeBoolLayerMask m_activeBulletables;
    public NativeArray<S_CapsuleLine> m_capsuleBulletable;
    public NativeArray<SeaweedMatrixSpliter.ThreadComputedPosition> m_threadComputation;
    public NativeBoolLayerMask m_matrixHandled;
    public JobExe_NativeCapsuleToSeaweed m_job;
    public JobExe_AllLessLayer_IsItWorthComputingThenCompute m_jobReach;
    public JobExe_IsItCapsulesCollidingWith m_jobColliding;
    public int m_maxCapsuleSetCount;

    public int m_bulletsActiveCount;
    public int m_bulletablesActiveCount;
    public long m_nonOptimimalMaxIteration;
    public long m_nonOptimimalRealTimeMaxIteration;


    public int m_inMatrixZoneCount;

    public SeaweedMatrixSpliterMono m_matrixSpliter;
    private bool m_threadComputeInit;
    private void Awake()
    {
        //m_job = new JobExe_NativeCapsuleToSeaweed();
        //m_jobReach = new JobExe_AllLessLayer_IsItWorthComputingThenCompute();
    }
    private void OnDestroy()
    {
       if( m_threadComputeInit)
        m_threadComputation.Dispose();
    }
    public void SetBulletsRefWith(NativeBoolLayerMask activeCapsules, NativeArray<S_CapsuleLine> capsule)
    {
        m_activeBullets = activeCapsules;
        m_capsuleBullet = capsule;
        RefreshMaxIteration();
        m_maxCapsuleSetCount = capsule.Length;
        SeaweedMatrixSpliter.ThreadComputedPosition[] tc = new SeaweedMatrixSpliter.ThreadComputedPosition[capsule.Length];
        m_threadComputation = new NativeArray<SeaweedMatrixSpliter.ThreadComputedPosition>(tc, Allocator.Persistent);
        m_matrixHandled = new NativeBoolLayerMask();
        m_matrixHandled.SetSize(m_maxCapsuleSetCount);

        m_job = new JobExe_NativeCapsuleToSeaweed();
        m_jobReach = new JobExe_AllLessLayer_IsItWorthComputingThenCompute();
        m_jobColliding = new JobExe_IsItCapsulesCollidingWith();
        m_job.SetWith(m_matrixSpliter.matrixSplite.m_spliter, capsule, activeCapsules, m_threadComputation, m_matrixHandled);
        m_jobReach.SetBulletsRef(capsule, activeCapsules);
        m_jobColliding.SetBulletsRef(capsule);

        m_threadComputeInit = true;

    }


    public void SetBulletableRefWith(NativeBoolLayerMask activeCapsules, NativeArray<S_CapsuleLine> capsule)
    {

        m_activeBulletables = activeCapsules;
        m_capsuleBulletable = capsule;
        RefreshMaxIteration();
    }
    private void RefreshMaxIteration()
    {
        m_nonOptimimalMaxIteration = m_capsuleBullet.Length * m_capsuleBulletable.Length;
    }

    //public float m_timeBetweenPush=0.5f;
    public void Start()
    {
        //InvokeRepeating("PushDataInMatrix", 1, m_timeBetweenPush);
    }


    private bool m_dirtyFlagSizeInit;
    public void PushDataInMatrix (){
        //TODO
        if (m_threadComputeInit) {
            if (!m_dirtyFlagSizeInit) {

                m_matrixSpliter.matrixSplite.m_spliter.RedefineSizeOfElementCount(m_maxCapsuleSetCount);
                m_dirtyFlagSizeInit = true;
            }
            m_job.SetSeaweedMatrix(m_matrixSpliter.matrixSplite.m_spliter);
            m_matrixSpliter.SetDataFetcher(m_job);
           JobHandle handle= m_job.Schedule(m_capsuleBullet.Length, 64);
            handle.Complete();

            //Debug.Log(string.Format(Time.time+" T: ", m_job.m_computePosition.Select(k=>k.m_matrix3x3Id)));
             m_job.m_handleMask.GetTrueCount(out m_inMatrixZoneCount);
            m_job.m_seaweed.PushInPrecompute(m_job.m_handleMask, m_job.m_computePosition);
            m_matrixSpliter.matrixSplite.m_spliter = m_job.m_seaweed;
        }
    
    }

    public void Update()
    {
        PushDataInMatrix();

        if (m_activeBullets.IsDefined())
            m_activeBullets.GetTrueCount(out m_bulletsActiveCount);
        if (m_activeBulletables.IsDefined())
            m_activeBulletables.GetTrueCount(out m_bulletablesActiveCount);

        m_nonOptimimalRealTimeMaxIteration = m_bulletablesActiveCount * m_bulletsActiveCount;

        //TO DO
        //Fetch bullet in target zone;



        /* //USE MATRICE To PREfilter bullets 
        LinkedList<int> aroundTheCell = new LinkedList<int>();
        for (int i = 0; i < m_capsuleBulletable.Length; i++)
        {
            S_CapsuleLine bulletable = m_capsuleBulletable[i];
            int i3x3 = -1;
            bool isInZone = false;
            m_matrixSpliter.matrixSplite.m_spliter.ComputePositionOf(ref bulletable.m_end, ref i3x3, ref isInZone);
            if (i == 0)
            {
                m_isInZone = isInZone;
                m_position0 = i3x3;
                m_position = bulletable.m_end;
            }
            if (isInZone)
            {
                aroundTheCell.Clear();
                m_matrixSpliter.matrixSplite.m_spliter.AppendIdsOfCellAndAround(i3x3, ref aroundTheCell);

                // bullets vs bulletable
                if (aroundTheCell.Count > 0)
                {


                    //Job to check what of those are active and worthy for the bulletable.
                    //Job to compute the collision of the those who worth it.
                    //CheckForCollisionWithBulletable(i, aroundTheCell); 

                }


                if (i == 0)
                {

                    m_matrixSpliter.RelocatedFromStartCorner(Vector3.zero, out Vector3 reloZero);
                    Debug.DrawLine(reloZero, m_position, Color.black, .5f);
                    m_matrixSpliter.RelocatedFromCenter(Vector3.zero, out reloZero);
                    Debug.DrawLine(reloZero, m_position, Color.white, .5f);
                    m_matrixSpliter.DrawSquare(i3x3, Color.red, .5f);

                    // DrawBulletsPath(aroundTheCell.ToArray(), Color.green);
                }


            }

        }
        //*/
        //TEST WITH ALL on the first bulletable
        m_activeBullets.GetIdListOfActive(out List<int> allBulletsActive);
        for (int i = 0; i < m_capsuleBulletable.Length; i++)
            CheckForCollisionWithBulletable(i, allBulletsActive); 
    }

    private void CheckForCollisionWithBulletable( int bulletableId, IEnumerable<int> bulletsId )
    {
        bool wantDebugCollisions = true;

        a = b = c = false;
        if (m_threadComputeInit && m_capsuleBulletable.Length > 0)
        {
            //Should be in seaweed filter but for the moment I will try all bullets on first bulletable;
            m_target = m_capsuleBulletable[bulletableId];
            //DOING WITH ALL BULLET FOR TESTING...
          
            ComputeListOfBulletsWithOneBulletable(ref m_target, bulletsId, out Queue<int> inZoneBullets);
            if (inZoneBullets != null)
            {
                ComputeListOfBulletsCollidingWithOneBulletable(ref m_target,
                    inZoneBullets,
                    out List<int> collidingBullets,
                    wantDebugCollisions || m_listenToCollisionShortenPath!=null,
                    out S_CapsulesCollision[] collisionsDetails);

                if ( collisionsDetails != null)
                {


                    if (m_listenToCollisionId != null)
                    {
                        for (int i = 0; i < collidingBullets.Count; i++)
                        {
                            m_listenToCollisionId(collidingBullets[i], bulletableId);

                        }
                    }
                    if (m_listenToCollisionShortenPath != null)
                    {
                        for (int i = 0; i < collisionsDetails.Length; i++)
                        {
                            m_listenToCollisionShortenPath(collisionsDetails[i]);

                        }
                    }

                }


            }

            for (int i = 0; i < computedTest.Length; i++)
            {

                Debug.DrawLine(computedTest[i].m_shortestPath.m_bulletShortestStart,
                    computedTest[i].m_shortestPath.m_bulletableShorestEnd,
                    Color.red, Time.deltaTime);
            }
        }
    }

    public bool a;
    public bool b;
    public bool c;


    private void DrawBulletsPath( int[] bulletIds, Color color)
    {
       
        for (int j = 0; j < bulletIds.Length ; j++)
        {
            
            Debug.DrawLine(m_capsuleBullet[bulletIds[j]].m_start, m_capsuleBullet[bulletIds[j]].m_end, color * 0.5f);
        }
    }
    public S_CapsuleLine m_target;
    public int m_bulletCount;
    public int m_bulletActive;
    public int m_bulletReach;
    private void ComputeListOfBulletsWithOneBulletable(ref S_CapsuleLine bulletable, NativeBoolLayerMask bulletsToInteractWith ,out Queue<int> bulletInZone)
    {
        m_bulletCount = bulletsToInteractWith.m_size;

        bulletsToInteractWith.GetIdListOfActive(out Queue<int> activeList);
        m_bulletActive = activeList.Count;
        ComputeListOfBulletsWithOneBulletable(ref bulletable, activeList, out bulletInZone);
    }
    private void ComputeListOfBulletsWithOneBulletable(ref S_CapsuleLine bulletable, IEnumerable<int> bulletsToInteractWith, out Queue<int> bulletInZone)
    {
        bulletInZone = null;
        if (bulletsToInteractWith.Count() < 1)
        {
            m_bulletOneCollisionPossible = new List<int>();
        }
        else
        {
            TempNativeList list = new TempNativeList();
            list.Set(bulletsToInteractWith);
            NativeBoolLayerMask inRangeResult = new NativeBoolLayerMask();
            inRangeResult.SetSize(list.m_size, Allocator.TempJob);

            m_jobReach.SetBulletsRef(m_capsuleBullet, m_activeBullets);
            m_jobReach.SetData(list, m_capsuleBulletable[0], inRangeResult);
            JobHandle handle = m_jobReach.Schedule(list.m_size, 64);
            handle.Complete();

            inRangeResult.GetIdListOfActive(out bulletInZone);
            m_bulletOneCollisionPossible = bulletInZone.ToList();
            DrawBulletsPath(m_bulletOneCollisionPossible.ToArray(), m_debugNow);
            list.Dispose();
            inRangeResult.Dispose();
            m_bulletReach = m_bulletOneCollisionPossible.Count;
        }
    }

    //m_jobColliding
    private void ComputeListOfBulletsCollidingWithOneBulletable(
        ref S_CapsuleLine bulletable,
        IEnumerable<int> bulletsToInteractWith,
        out List<int> bulletInCollision
        ,bool withCollisionDetails,
        out  S_CapsulesCollision [] collisionDetailsOfCollidingBullets)
    {
        collisionDetailsOfCollidingBullets = null;
        bulletInCollision = null;
        if (bulletsToInteractWith.Count() < 1)
        {
            m_bulletOneColliding = new List<int>();
        }
        else
        {
            bulletInCollision = new List<int>();
            int[] idsIn = bulletsToInteractWith.ToArray();
            TempNativeList list = new TempNativeList();
            list.Set(bulletsToInteractWith);
            NativeBoolLayerMask collidingResultBool = new NativeBoolLayerMask();
            collidingResultBool.SetSize(list.m_size, Allocator.TempJob);

            S_CapsulesCollision[] collisionArray = new S_CapsulesCollision[list.m_size];
            NativeArray<S_CapsulesCollision> collidingResultDetails = new NativeArray<S_CapsulesCollision>(collisionArray, Allocator.TempJob);
            

            m_jobColliding.SetBulletsRef(m_capsuleBullet );
            m_jobColliding.SetData(list, m_capsuleBulletable[0], collidingResultBool, collidingResultDetails);
            JobHandle handle = m_jobColliding.Schedule(list.m_size, 64);
            handle.Complete(); 
            //computedTest = new S_CapsulesCollision[collidingResultDetails.Length];
            //collidingResultDetails.CopyTo(computedTest);

            collidingResultBool.GetIdListOfActive(out List<int>valideIdOFTempListColliding);
            for (int i = 0; i < valideIdOFTempListColliding.Count; i++)
            {
                bulletInCollision.Add(idsIn[valideIdOFTempListColliding[i]]);
               

            }
            m_bulletOneColliding = bulletInCollision;
            
            //not sure of me
            if (withCollisionDetails) {
                collisionDetailsOfCollidingBullets = new S_CapsulesCollision[bulletInCollision.Count];
 
                for (int i = 0; i < collisionDetailsOfCollidingBullets.Length; i++)
                {
                    collisionDetailsOfCollidingBullets[i] = collidingResultDetails[valideIdOFTempListColliding[i]];
                }
            }
            test = collisionDetailsOfCollidingBullets;
            DrawBulletsPath(m_bulletOneColliding.ToArray(), Color.red);
            list.Dispose();
            collidingResultBool.Dispose();
            collidingResultDetails.Dispose();

        }
    }



    public Color m_debugNow = Color.green;
    public List<int> m_bulletOneCollisionPossible;
    public List<int> m_bulletOneColliding;
    public S_CapsulesCollision[] computedTest;
    public S_CapsulesCollision[] test;

    public bool m_isInZone;
    public int m_position0;
    public Vector3 m_position;
    public int[] m_idsAroundTarget0Debug;
}


public struct TempNativeList
{
    public int m_size;
    public NativeArray<int> m_idsToHangle;

    public void Set(int size)
    {
        m_size = size;
        m_idsToHangle = new NativeArray<int>(new int[m_size], Allocator.TempJob);
    }
    public void Set(IEnumerable<int> elements)
    {
        int[] t = elements.ToArray();
        m_size = t.Length;
        m_idsToHangle = new NativeArray<int>(t, Allocator.TempJob);
    }
    public void Dispose() {
        m_idsToHangle.Dispose();
    }

    public int Get(ref int index)
    {
        return m_idsToHangle[index];
    }
}


/// <summary>
/// The idea is to divide large list of around 10.000 in 1024 package where -1 id means return and do nothing
/// Allowing to resuse the native array instead of allocated and destroy all time. Is it worth it? I don't know yet.
/// </summary>
public struct NativeList1024
{
    //TO COMPLETE


    public NativeArray<int> m_idsToHangle;

    public void Init() {
        m_idsToHangle = new NativeArray<int>(new int[1024], Allocator.Persistent);
    }

    public void Dispose() {
        m_idsToHangle.Dispose();
    }
}

[BurstCompile]
public struct JobExe_AllLessLayer_IsItWorthComputingThenCompute : IJobParallelFor
{

    public TempNativeList m_bulletToCompute;
    public NativeBoolLayerMask m_bulletToComputeInRange;

    public NativeBoolLayerMask m_bulletExistanceLayer;

    [NativeDisableParallelForRestriction]
    public NativeArray<S_CapsuleLine> m_bullets;

    public S_CapsuleLine m_bulletable;

    public void SetBulletsRef(NativeArray<S_CapsuleLine> capsules, NativeBoolLayerMask activeCapsulesLayer)
    {
        m_bulletExistanceLayer = activeCapsulesLayer;
        m_bullets = capsules;
    }

    public void SetData(   TempNativeList bulletsId,
        S_CapsuleLine bulletable,
        NativeBoolLayerMask bulletInRange
        ) {
        m_bulletToCompute = bulletsId;
            m_bulletable= bulletable;
        m_bulletToComputeInRange= bulletInRange;
    }

    public void Execute(int index)
    {
        if (m_bulletExistanceLayer.IsFalse(ref index))
            return;

        int bulletId = m_bulletToCompute.Get(ref index);
        bool inRange = false;
        
      
        
            S_CapsuleLine bullet = m_bullets[bulletId];
        CapsuleLineCollisionUtility.CheckIfNearEnough(
           ref bullet.m_start, ref bullet.m_end, ref bullet.m_radius, ref m_bulletable.m_start, ref m_bulletable.m_end, ref m_bulletable.m_radius
           , out inRange);

        
        m_bulletToComputeInRange.SetAs(ref index, inRange);
    }

    
}


[System.Serializable]
public struct S_CapsulesCollision {
    public int m_bulletCapsuleId;
    public int m_bulletableCapsuleId;
    public S_ShortestPath m_shortestPath;
}
[System.Serializable]
public struct S_ShortestPath{
    public Vector3 m_bulletShortestStart;
    public Vector3 m_bulletableShorestEnd;
}


/// <summary>
///  It is supposed that you check if the elements exists and if it worth the computation before you inject them in this job.
///  So I don't check it before computing. Please take that in consideration.
/// </summary>
[BurstCompile]
public struct JobExe_IsItCapsulesCollidingWith : IJobParallelFor
{

    public S_CapsuleLine m_bulletable;

    public TempNativeList m_bulletsToCompute;

    [NativeDisableParallelForRestriction]
    public NativeArray<S_CapsuleLine> m_bullets;

    public NativeBoolLayerMask m_bulletToComputeInCollision;
    public NativeArray<S_CapsulesCollision> m_capsuleCollisions;

    public CapsuleLineCollisionComputation m_collisionUtility;
    public void SetBulletsRef(NativeArray<S_CapsuleLine> capsules)
    {
        m_bullets = capsules;
        m_collisionUtility = new CapsuleLineCollisionComputation();
    }

    public void SetData(TempNativeList listOfBulletsToCheck,
        S_CapsuleLine bulletable,
        NativeBoolLayerMask bulletCollisionResultBool,
        NativeArray<S_CapsulesCollision> bulletCollisionResultDetails
        )
    {
        m_bulletsToCompute = listOfBulletsToCheck;
        m_bulletable = bulletable;
        m_bulletToComputeInCollision = bulletCollisionResultBool;
        m_capsuleCollisions = bulletCollisionResultDetails;
    }

    public void Execute(int index)
    {
        

        int bulletId = m_bulletsToCompute.Get(ref index);
        bool areColliding = false;
        Vector3 s= m_bulletable.m_start, e= m_bulletable.m_end;

       // bullet.m_radius m_bulletable.m_radius
        S_CapsuleLine bullet = m_bullets[bulletId];
        S_CapsulesCollision collision = m_capsuleCollisions[index];

        if(s==  e)
        {
            //s = e + Vector3.one * 1f;
            //Give an axis if the algo detection of collision can't detect capsule not moving
            //s = e + Vector3.one * 0.001f;
        }

        m_collisionUtility.GetShortestLineBetweenTwoSections( 
            out collision.m_shortestPath.m_bulletShortestStart, 
            out collision.m_shortestPath.m_bulletableShorestEnd, 
            bullet.m_start,  bullet.m_end, s,e
           , false);

        if (collision.m_shortestPath.m_bulletShortestStart == Vector3.zero && collision.m_shortestPath.m_bulletableShorestEnd == Vector3.zero)
        {
            areColliding = false;
        }
        else {

            AreColliding(
               ref collision.m_shortestPath.m_bulletShortestStart,
               ref bullet.m_radius,
               ref collision.m_shortestPath.m_bulletableShorestEnd,
               ref m_bulletable.m_radius,
               out areColliding);
        }


        m_bulletToComputeInCollision.SetAs(ref index, areColliding);
        //if (areColliding)
        {
            collision.m_bulletCapsuleId = bullet.m_id;
            collision.m_bulletableCapsuleId = m_bulletable.m_id;
            m_capsuleCollisions[index]= collision;
        }
    }

    public  void AreColliding(ref Vector3 startPoint, ref float startPointRadius, ref Vector3 endPoint, ref float endPointRadius, out bool areColliding)
    {
        areColliding = (endPoint - startPoint).magnitude < (startPointRadius + endPointRadius);
    }
}





[BurstCompile]
//[BurstCompile(CompileSynchronously = true)]
public struct JobExe_NativeCapsuleToSeaweed : IJobParallelFor, DataFetcher
{
    public SeaweedMatrixSpliter m_seaweed;
    public NativeArray<S_CapsuleLine> m_capsule;
    public NativeBoolLayerMask m_activeMask;
    public NativeArray<SeaweedMatrixSpliter.ThreadComputedPosition> m_computePosition;
    public NativeBoolLayerMask m_handleMask;


    public void SetWith(SeaweedMatrixSpliter spliter,
        NativeArray<S_CapsuleLine> capsule,
        NativeBoolLayerMask activeMask,
        NativeArray<SeaweedMatrixSpliter.ThreadComputedPosition> computedPosition, NativeBoolLayerMask handleMask)
    {
        m_computePosition = computedPosition;
        m_capsule = capsule;
        m_activeMask = activeMask;
        m_seaweed = spliter;
        m_handleMask = handleMask;
    }
    public void SetSeaweedMatrix(SeaweedMatrixSpliter spliter)
    {

        m_seaweed = spliter;
    }
    public void Execute(int index)
    {
        if (m_activeMask.IsFalse(ref index)) {
            m_handleMask.SetAs(ref index, false);
            return;
        }
        S_CapsuleLine c = m_capsule[index];
        bool isInZone = false;
        int i3x3 = -1;
        m_seaweed.ComputePositionOf(ref c.m_end,ref i3x3, ref isInZone);


        m_handleMask.SetAs(ref index, isInZone);
        if (isInZone) {

            SeaweedMatrixSpliter.ThreadComputedPosition computation = m_computePosition[index];
            computation.m_arrayPosition = index;
            computation.m_matrix3x3Id = i3x3;
            computation.m_objectId = c.m_id;
            m_computePosition[index] = computation;
        }
    }

    public int GetCollectionCount()
    {
        return m_capsule.Length;
    }

    public void GetInfoAtId(ref int id, ref int objectId, ref Vector3 position)
    {
        S_CapsuleLine c = m_capsule[id];
        objectId = c.m_id;
        position = c.m_end;


    }

   
}