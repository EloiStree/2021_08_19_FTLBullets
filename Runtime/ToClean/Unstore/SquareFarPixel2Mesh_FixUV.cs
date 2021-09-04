using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Unity.Collections;
using System;
using Unity.Jobs;

public class SquareFarPixel2Mesh_FixUV : AbstractSquareFarPixelRendererListener
{

    public Transform m_meshFilterPosition;
    public Mesh m_currentMesh;
    public MeshFilter m_meshFilter;
    public SkinnedMeshRenderer m_meshRenderer;

    public int m_maxSquare=10800;
    public PixelFarConfig m_currentConfig;
    public float m_maxDistanceBounds = 100000;
    public JobExe_FarPixelToLocalSquareMesh m_job;


    private void Awake()
    {
        m_currentMesh = new Mesh();
        m_currentMesh.name = "Far Square as pixels mesh";

        Vector3[] v = new Vector3[m_maxSquare * 4];
        Vector2[] uvv = new Vector2[m_maxSquare * 4];
        int[] t = new int[m_maxSquare * 6];

        m_currentMesh.bounds = new Bounds(Vector3.zero, Vector3.one * m_maxDistanceBounds);

        for (int i = 0; i < m_maxSquare; i++)
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
        }
        m_currentMesh.SetVertices(v);
        m_currentMesh.SetUVs(0, uvv);
        m_currentMesh.SetTriangles(t, 0);
        m_meshFilter.sharedMesh = m_currentMesh;
        if (m_meshRenderer != null) { 
        
            m_meshRenderer.sharedMesh = m_currentMesh;
            m_meshRenderer.localBounds = new Bounds(Vector3.zero, Vector3.one * m_maxDistanceBounds);
        }

        m_job = new JobExe_FarPixelToLocalSquareMesh();
        int[] lp = new int[m_maxSquare];
        m_flushingArray = new Vector3[m_currentMesh.vertices.Length];
        
    }


    public override void InitParams(PixelFarConfig config)
    {
        m_currentConfig = config;

        m_job.SetPixelFarConfiguration(config);
        m_job.SetMaxSquareAllow(m_maxSquare);
    }
    private Vector3[] m_flushingArray;
    public override void RefreshWith(Vector3 worldPoint, Quaternion worldRotation, NativeArray<SquareFarPixel> farPixel)
    {
        if (!this.enabled) return;
        m_meshFilterPosition.position = worldPoint;
        m_meshFilterPosition.rotation = worldRotation;

        m_job.SetFarPixelsInfo(farPixel);
        m_job.FlushPointsInfo(ref m_flushingArray);
        JobHandle handle= m_job.Schedule(farPixel.Length, 64);
        handle.Complete();
        m_job.RecoverAndDispose(ref m_currentMesh);

    }
}

[BurstCompile(CompileSynchronously = true)]
public struct JobExe_FarPixelToLocalSquareMesh : IJobParallelFor {

    public int m_maxSquare;
    public PixelFarConfig m_configuration;
    public NativeArray<SquareFarPixel> m_farPixelsInfo;
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> m_squareMeshVertices;

    public float m_horizontalCellAngle;
    public float m_verticalCellAngle;

    public void SetPixelFarConfiguration(PixelFarConfig configuration) {
        m_configuration = configuration;
        m_horizontalCellAngle = m_configuration.m_horizontalAngle / (float)configuration.m_widthInPixel;
        m_verticalCellAngle = m_configuration.m_verticalAngle / (float)configuration.m_heightInPixel;
        m_pbl = Quaternion.Euler(m_horizontalCellAngle * 0.5f, m_verticalCellAngle * 0.5f, 0);
        m_pbr = Quaternion.Euler(-m_horizontalCellAngle * 0.5f, m_verticalCellAngle * 0.5f, 0);
        m_ptl = Quaternion.Euler(m_horizontalCellAngle * 0.5f, -m_verticalCellAngle * 0.5f, 0);
        m_ptr = Quaternion.Euler(-m_horizontalCellAngle * 0.5f, -m_verticalCellAngle * 0.5f, 0);

    }
    public void FlushPointsInfo(ref Vector3[] dataToFlushWith) {
        m_squareMeshVertices = new NativeArray<Vector3>(dataToFlushWith, Allocator.TempJob);
       
          
    }
    public void RecoverAndDispose(ref Mesh meshToAffect) {
        meshToAffect.SetVertices(m_squareMeshVertices);
        m_squareMeshVertices.Dispose();
    }

    public void Execute(int index)
    {
        SquareFarPixel pixel = m_farPixelsInfo[index];
        SetSquare(index, ref pixel);
    }

    public void SetFarPixelsInfo(NativeArray<SquareFarPixel> farPixel)
    {
        m_farPixelsInfo = farPixel;
    }

    public void SetMaxSquareAllow(int maxSquare)
    {
        m_maxSquare = maxSquare;
    }

    public void SetSquare(int squareIndex, ref SquareFarPixel pixelInfo) {

        int pointIndex = squareIndex * 4;

        Vector3 forward = 
            Quaternion.Euler(
                (-pixelInfo.m_verticalIndex * m_verticalCellAngle) + (pixelInfo.m_verticalIndex > 0f ? m_verticalCellAngle : -m_verticalCellAngle) * 0.5f,
                (pixelInfo.m_horizontalIndex * m_horizontalCellAngle) + (pixelInfo.m_horizontalIndex >0f?-m_horizontalCellAngle: m_horizontalCellAngle)*0.5f,
               
                0) * Vector3.forward;
        m_squareMeshVertices[pointIndex + 0] = (m_pbl * forward) * pixelInfo.m_minDistance;
        m_squareMeshVertices[pointIndex + 1] = (m_pbr * forward) * pixelInfo.m_minDistance;
        m_squareMeshVertices[pointIndex + 2] = (m_ptl * forward) * pixelInfo.m_minDistance;
        m_squareMeshVertices[pointIndex + 3] = (m_ptr * forward) * pixelInfo.m_minDistance;
    }
    Quaternion m_pbl;
    Quaternion m_pbr;
    Quaternion m_ptl;
    Quaternion m_ptr;
}
