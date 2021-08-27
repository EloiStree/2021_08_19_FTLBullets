using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class BulletsRendering_OrientedSquareWithJob : AbstractBulletsToRenderListener
{


    public int m_maxQuad=20000;
    public MeshFilter m_meshFilter;
    public MeshRenderer m_MeshRenderer;
    public SkinnedMeshRenderer m_skinMeshRenderer;
    public float m_maxDistanceBounds=100000;
    public float m_sizeOfBullets=0.5f;

    [Header("Debug")]
    public Mesh m_currentMesh;
    public List<int> m_bulletsIndex;
    public ProcessBulletsForParallels m_job;

   
    public override void ApplyComputeRendering(ref FilteredBulletsId bulletsToProcess)
    {
        m_bulletsIndex = bulletsToProcess.m_farIds;
        m_job.SetIdToProcessList(bulletsToProcess.m_farIds);
        m_job.m_squareSize = m_sizeOfBullets;
        JobHandle jh= m_job.Schedule(m_maxQuad, 64);
        jh.Complete();


        m_currentMesh.SetVertices(m_job.m_positions);

//        m_currentMesh.RecalculateNormals();

        //for (int i = 0; i < 100; i++)
        //{
        //    int iv = i * 4;
        //    Debug.DrawLine(m_job.m_positions[iv + 0], m_job.m_positions[iv + 1], Color.red);
        //    Debug.DrawLine(m_job.m_positions[iv + 0], m_job.m_positions[iv + 2], Color.red);
        //    Debug.DrawLine(m_job.m_positions[iv + 2], m_job.m_positions[iv + 3], Color.red);
        //    Debug.DrawLine(m_job.m_positions[iv + 3], m_job.m_positions[iv + 1], Color.red);

        //}
    }

    public override void InitWithCount(int count)
    {
        m_currentMesh = new Mesh();
        m_currentMesh.name = "Bullet as square at far distance";
        Vector3 [] v=new Vector3[m_maxQuad * 4];
        Vector2 []uvv = new Vector2[m_maxQuad * 4];
        int[] t=new int[m_maxQuad * 6];

        m_currentMesh.bounds = new Bounds(Vector3.zero, Vector3.one * m_maxDistanceBounds);

        for (int i = 0; i < m_maxQuad; i++)
        {
            int iuv = i * 4;
            int itv = i * 6;
            uvv[iuv + 0] = new Vector2(0, 0);
            uvv[iuv + 1] = new Vector2(1, 0);
            uvv[iuv + 2] = new Vector2(0, 1);
            uvv[iuv + 3] = new Vector2(1, 1);
            t[itv + 0] = iuv + 0;
            t[itv + 1] = iuv + 2;
            t[itv + 2] = iuv + 1;
            t[itv + 3] = iuv + 2;
            t[itv + 4] = iuv + 3;
            t[itv + 5] = iuv + 1;

            //t[itv + 0] = iuv + 2;
            //t[itv + 1] = iuv + 1;
            //t[itv + 2] = iuv + 0;
            //t[itv + 3] = iuv + 3;
            //t[itv + 4] = iuv + 1;
            //t[itv + 5] = iuv + 2;
        }
        m_currentMesh.SetVertices(v);
        m_currentMesh.SetUVs(0,uvv);
        m_currentMesh.SetTriangles(t,0);
        m_meshFilter.sharedMesh = m_currentMesh;
        if(m_skinMeshRenderer!=null)
        m_skinMeshRenderer.sharedMesh = m_currentMesh;
        

        m_job = new ProcessBulletsForParallels();
        int[] lp = new int[m_maxQuad];
        Vector3[] v3 = new Vector3[m_currentMesh.vertices.Length];
        m_job.Init();
        m_job.Set(m_bulletsRenderingRef);
        m_job.Set(m_bulletsRef);
        m_job.Set(new NativeArray<Vector3>(v3, Allocator.Persistent));
        m_job.Set(new NativeArray<int>(lp, Allocator.Persistent));
        m_job.SetMeshInfo(m_maxQuad, m_maxQuad * 4);
    }

    public void Dispose() {
        m_job.Dispose();
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct ProcessBulletsForParallels : IJobParallelFor
    {
        public int m_maxQuad;
        public int m_maxQuadVertices;

        public void SetMeshInfo(int quadCount, int quadVertices) {
            m_maxQuad = quadCount;
            m_maxQuadVertices = quadVertices;
        }

        [NativeDisableParallelForRestriction]
        public NativeArray<BulletRendering> m_bulletWantedRenderingInfo;
        [NativeDisableParallelForRestriction]
        public NativeArray<BulletDataResult> m_bulletStateResultInfo;

        [NativeDisableParallelForRestriction]
        public NativeArray<Vector3> m_positions;
        public int m_positionsCount;

        public NativeArray<int> m_ids;
        public int m_idsCount;

        internal void Set(NativeArray<BulletDataResult> bulletDataResult)
        {
            m_bulletStateResultInfo = bulletDataResult;
        }
        public void Set(NativeArray<BulletRendering> bulletRenderingInfo)
        {
            m_bulletWantedRenderingInfo = bulletRenderingInfo;
        }
        public void Set(NativeArray<int> ids)
        {
            m_ids = ids;
            m_idsCount = ids.Length;
        }
        public void Set(NativeArray<Vector3> position) {
            m_positions = position;
            m_positionsCount = position.Length;
        }

        public void SetIdToProcessList(List<int> idsToProcess) {

           
            for (int i = 0; i < m_idsCount; i++)
            {
                if (i < idsToProcess.Count)
                {
                    m_ids[i] = idsToProcess[i];
                }
                else { 
                    m_ids[i] = -1;
                }
            }
        }
        Quaternion m_pbl;
        Quaternion m_pbr;
        Quaternion m_ptl;
        Quaternion m_ptr;

        public float m_squareSize;
        public void SetRotations() {
            //p1 = Quaternion.Euler(0, 90, 0) * Quaternion.Euler(0, 0, 45);
            //p2 = Quaternion.Euler(0, 90, 0) * Quaternion.Euler(0, 0, -45);
            //p3 = Quaternion.Euler(0, -90, 0) * Quaternion.Euler(0, 0, 45);
            //p4 = Quaternion.Euler(0, -90, 0) * Quaternion.Euler(0, 0, -45);
            m_pbl = Quaternion.Euler(-135, 90, 0);
            m_pbr = Quaternion.Euler(-45, 90, 0);
            m_ptl = Quaternion.Euler(135, 90, 0);
            m_ptr = Quaternion.Euler(45, 90, 0);
            m_squareSize = 1f;
        }

        public void Execute(int index)
        {
            if (index >= m_maxQuad)
                return;
                
          
            int idBullet = m_ids[index];
            int vertexIndexPosition = index * 4;
            if ( idBullet < 0 )
            {
                m_positions[vertexIndexPosition + 0] = Vector3.zero;
                m_positions[vertexIndexPosition + 1] = Vector3.zero;
                m_positions[vertexIndexPosition + 2] = Vector3.zero;
                m_positions[vertexIndexPosition + 3] = Vector3.zero;
            }
            else {

                Vector3 cameraDirection = m_bulletWantedRenderingInfo[idBullet].m_cameraDirection.normalized;
                Quaternion cQ = Quaternion.LookRotation(cameraDirection, Vector3.up);
                Vector3 position = m_bulletStateResultInfo[idBullet].m_currentPosition;
                //Debug.DrawLine(position, position+ cameraDirection *10);

                //m_positions[vertexIndexPosition + 0] = (position + (m_pbl * cameraDirection));
                //m_positions[vertexIndexPosition + 1] = (position + (m_pbr * cameraDirection));
                //m_positions[vertexIndexPosition + 2] = (position + (m_ptl * cameraDirection));
                //m_positions[vertexIndexPosition + 3] = (position + (m_ptr * cameraDirection));

                m_positions[vertexIndexPosition + 0] = position + ((cQ * m_pbl) * Vector3.forward * m_squareSize) ;
                m_positions[vertexIndexPosition + 1] = position + ((cQ * m_pbr) * Vector3.forward * m_squareSize);
                m_positions[vertexIndexPosition + 2] = position + ((cQ * m_ptl) * Vector3.forward * m_squareSize);
                m_positions[vertexIndexPosition + 3] = position + ((cQ * m_ptr) * Vector3.forward * m_squareSize);


                // Q * V3Direction

            }
        }

        public void Init()
        {
            SetRotations();
        }

        public void Dispose()
        {
            m_positions.Dispose();
            m_ids.Dispose();
        }

        internal void Set(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            // Quaternion.AngleAxis(-45, Vector3.up) * vector;
            //this.m_pbl = Quaternion.Euler(p1);
            //this.m_pbr = Quaternion.Euler(p2);
            //this.m_ptl = Quaternion.Euler(p3);
            //this.m_ptr = Quaternion.Euler(p4);

            this.m_pbl = Quaternion.LookRotation(new Vector3(1,-1,0), Vector3.up);
            this.m_pbr = Quaternion.LookRotation(new Vector3(-1, -1, 0), Vector3.up);
            this.m_ptl = Quaternion.LookRotation(new Vector3(1, 1, 0), Vector3.up);
            this.m_ptr = Quaternion.LookRotation(new Vector3(-1, 1, 0), Vector3.up);
        }
        public static Vector3 Rotated( Vector3 vector, Quaternion rotation, Vector3 pivot = default(Vector3))
        {
            return rotation * (vector - pivot) + pivot;
        }

        public static Vector3 Rotated( Vector3 vector, Vector3 rotation, Vector3 pivot = default(Vector3))
        {
            return Rotated(vector, Quaternion.Euler(rotation), pivot);
        }

        public static Vector3 Rotated( Vector3 vector, float x, float y, float z, Vector3 pivot = default(Vector3))
        {
            return Rotated(vector, Quaternion.Euler(x, y, z), pivot);
        }

       
    }
}
