using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class BulletsToMeshColliderJobSystem : MonoBehaviour
{

    public JobCompteExe_BulletResultToMesh m_jobMesh;
    public Mesh m_mesh;
    public MeshFilter m_meshFilter;
    public SkinnedMeshRenderer m_skinMesh;
    public GameObject m_meshColliderObj;
    public MeshCollider m_meshCollider;


    [Header("Debug")]
    public int m_bulletsCount;
    public int m_verticesCount;
    public int m_triangleIntCount;
    public bool m_wasInitialized;

    public void SetBulletsMemory(NativeArray<BulletDataResult> bulletsMemory, NativeArray<TriggeredBulletData> initBulletsMemory ) {
        m_wasInitialized = true;
        m_bulletsCount = bulletsMemory.Length;

        m_jobMesh.Init(bulletsMemory, initBulletsMemory);
        m_verticesCount = m_jobMesh.m_vertices.Length;
    }

    void Start()
    {
        m_mesh = new Mesh();
        m_skinMesh.sharedMesh = m_mesh;
        if (m_meshFilter != null)
            m_meshFilter.sharedMesh = m_mesh;
        if (m_meshCollider != null)
            m_meshCollider.sharedMesh = m_mesh;

        Vector3[] vertices = new Vector3[m_bulletsCount * 3];
        int[] triangles = new int[m_bulletsCount * 6];
        Vector2[] uv = new Vector2[m_bulletsCount * 3];
        //Vector3 [] normals = new Vector3[]
        m_mesh.name = "Tiangle Bullets";

        for (int i = 0; i < m_bulletsCount; i++)
        {
            int indexTri = i * 6;
            int indexVert = i * 3;
            int indexUV = i * 3;
            triangles[indexTri] = indexVert + 0;
            triangles[indexTri + 1] = indexVert + 1;
            triangles[indexTri + 2] = indexVert + 2;
            triangles[indexTri + 3] = indexVert + 0;
            triangles[indexTri + 4] = indexVert + 2;
            triangles[indexTri + 5] = indexVert + 1;
            vertices[indexVert] = Vector3.up * 0.0001f;
            vertices[indexVert+1] = Vector3.left * 0.0001f;
            vertices[indexVert+2] = Vector3.right * 0.0001f;
            //uv[indexUV + 0] = new Vector2(1, 0.5f);
            //uv[indexUV + 1] = new Vector2(0, 1);
            //uv[indexUV + 2] = new Vector2(1, 1);

        }
        m_mesh.SetVertices(vertices);
        m_mesh.SetTriangles(triangles, 0);
        m_mesh.SetUVs(0,uv);
        //m_mesh.SetNormals(normals);
        m_mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100000);
        if (m_meshCollider != null) {

            m_meshCollider.sharedMesh = null;
            m_meshCollider.sharedMesh = m_mesh;
        }

    }

    public void RefreshMesh()
    {
        if (!m_wasInitialized)
            return;
        JobHandle job=  m_jobMesh.Schedule(m_jobMesh.m_bulletsCount, 64);
        job.Complete();
        m_jobMesh.ApplyToMesh(ref m_mesh);

       

    }
    private void OnDestroy()
    {

        m_jobMesh.Dispose();
    }
}


[BurstCompile]
public struct JobCompteExe_BulletResultToMesh : IJobParallelFor
{
    public bool m_isInit;
    public int m_bulletsCount;
    public NativeArray<BulletDataResult> m_bulletResult;
    public NativeArray<TriggeredBulletData> m_initInfo;
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> m_vertices;
    Quaternion q1;
    Quaternion q2;
    public void Execute(int index)
    {


        int verticesIndex = index * 3;
        if (m_bulletResult[index].m_isUsed)
        {
            //VERSION WITH TWO TRIANGLES
            int triangleIndex = index * 6;
            int i1 = verticesIndex;
            int i2 = verticesIndex + 1;
            int i3 = verticesIndex + 2;

            Vector3 pevious = m_bulletResult[index].m_previousPosition.magnitude < 0.0001f
                ?
                m_bulletResult[index].m_currentPosition : 
                m_bulletResult[index].m_previousPosition;

            Vector3 start = m_bulletResult[index].m_currentPosition; ;
            Vector3 direction = (m_bulletResult[index].m_currentPosition - pevious).normalized;
            m_vertices[verticesIndex] = pevious;
            m_vertices[verticesIndex + 1] = start + (q1 * (direction * m_initInfo[index].m_bulletInfo.m_radius));
            m_vertices[verticesIndex + 2] = start + (q2 * (direction * m_initInfo[index].m_bulletInfo.m_radius));
        }
        else
        {
            m_vertices[verticesIndex]    = Vector3.zero;
            m_vertices[verticesIndex + 1] = Vector3.zero;
            m_vertices[verticesIndex + 2] = Vector3.zero;

        }



        //VERSTION WITH 3 TRIANGLES ???
        //TO TRY

    }



    public void Init(NativeArray<BulletDataResult> bulletsSharedMemory, NativeArray<TriggeredBulletData> initInfo)
    {
        m_isInit = true;
        m_bulletResult = bulletsSharedMemory;
        m_bulletsCount = bulletsSharedMemory.Length;
        m_initInfo = initInfo;
        Vector3[] vertices = new Vector3[m_bulletsCount * 3];
        m_vertices = new NativeArray<Vector3>(vertices, Allocator.Persistent);

       



        q1 = Quaternion.Euler(0, 90, 0);
       q2 = Quaternion.Euler(0, -90, 0);


    }

    public void ApplyToMesh(ref Mesh mesh) {

        mesh.SetVertices(m_vertices);
        //mesh.RecalculateNormals();
       
        //mesh.RecalculateNormals();
    }


    public void Dispose()
    {
        if(m_vertices.IsCreated)
        m_vertices.Dispose();
    }
}