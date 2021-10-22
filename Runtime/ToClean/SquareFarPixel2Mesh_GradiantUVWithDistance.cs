
using UnityEngine;

using UnityEngine.Jobs;
using Unity.Burst;
using System;
using Unity.Jobs;
using Unity.Collections;

public class SquareFarPixel2Mesh_GradiantUVWithDistance : AbstractSquareFarPixelRendererListener
{

    public Transform m_meshFilterPosition;
    public Mesh m_currentMesh;
    public MeshFilter m_meshFilter;
    public SkinnedMeshRenderer m_meshRenderer;

    public int m_maxSquare = 10800;
    public PixelFarConfig m_currentConfig;
    public float m_maxDistanceBounds = 100000;
    public JobExe_FarPixelToLocalSquareMeshGradiantUV m_job;
    public enum IntencityType { Distance, Count}
    public IntencityType m_intecityType;

    public int m_maxCountIntensity=5;
    public bool m_inverseGradiant;
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
        if (m_meshRenderer != null)
        {

            m_meshRenderer.sharedMesh = m_currentMesh;
            m_meshRenderer.localBounds = new Bounds(Vector3.zero, Vector3.one * m_maxDistanceBounds);
        }

        m_job = new JobExe_FarPixelToLocalSquareMeshGradiantUV();
        int[] lp = new int[m_maxSquare];
        m_flushingArray = new Vector3[m_currentMesh.vertices.Length];
        m_flushingArrayUV = new Vector2[m_currentMesh.uv.Length];

    }


    public override void InitParams(PixelFarConfig config)
    {
        m_currentConfig = config;

        m_job.SetCountIntensityMax(m_maxCountIntensity);
        m_job.InverseGradiant(m_inverseGradiant);
        m_job.SetType(m_intecityType);
        m_job.SetPixelFarConfiguration(config);
        m_job.SetMaxSquareAllow(m_maxSquare);
    }
    private Vector3[] m_flushingArray;
    private Vector2[] m_flushingArrayUV;
    public SquareFarPixel[] m_humm;
    public override void RefreshWith(Vector3 worldPoint, Quaternion worldRotation, NativeArray<SquareFarPixel> farPixel)
    {
        if (!this.enabled) return;
        m_humm = farPixel.ToArray();
        m_meshFilterPosition.position = worldPoint;
        m_meshFilterPosition.rotation = worldRotation;

 //       Debug.Log("T");
        m_job.SetFarPixelsInfo(farPixel);
        m_job.FlushPointsInfo(ref m_flushingArray, ref m_flushingArrayUV);

        JobHandle handle = m_job.Schedule<JobExe_FarPixelToLocalSquareMeshGradiantUV>(farPixel.Length, 64);
        handle.Complete();
        m_job.RecoverAndDispose(ref m_currentMesh);

    }
}

[BurstCompile(CompileSynchronously = true)]
public struct JobExe_FarPixelToLocalSquareMeshGradiantUV : IJobParallelFor
{

    public int m_maxSquare;
    public PixelFarConfig m_configuration;
    public NativeArray<SquareFarPixel> m_farPixelsInfo;
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> m_squareMeshVertices;
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector2> m_squareMeshUV;

    public float m_horizontalCellAngle;
    public float m_verticalCellAngle;

    public void SetPixelFarConfiguration(PixelFarConfig configuration)
    {
        m_configuration = configuration;
        m_horizontalCellAngle = m_configuration.m_horizontalAngle / (float) (configuration.m_widthInPixel+1);
        m_verticalCellAngle = m_configuration.m_verticalAngle / (float) (configuration.m_heightInPixel+1);
        //m_pbl = Quaternion.Euler(m_horizontalCellAngle * 0.5f, m_verticalCellAngle * 0.5f, 0);
        //m_pbr = Quaternion.Euler(-m_horizontalCellAngle * 0.5f, m_verticalCellAngle * 0.5f, 0);
        //m_ptl = Quaternion.Euler(m_horizontalCellAngle * 0.5f, -m_verticalCellAngle * 0.5f, 0);
        //m_ptr = Quaternion.Euler(-m_horizontalCellAngle * 0.5f, -m_verticalCellAngle * 0.5f, 0);
        m_pbl = Quaternion.Euler(m_verticalCellAngle * 0.5f, m_horizontalCellAngle   * 0.5f, 0);
        m_pbr = Quaternion.Euler(m_verticalCellAngle  * 0.5f,-m_horizontalCellAngle  * 0.5f, 0);
        m_ptl = Quaternion.Euler(-m_verticalCellAngle* 0.5f, m_horizontalCellAngle    * 0.5f, 0);
        m_ptr = Quaternion.Euler(-m_verticalCellAngle * 0.5f, -m_horizontalCellAngle  * 0.5f, 0);
        //m_pbl = Quaternion.Euler(m_horizontalCellAngle , m_verticalCellAngle , 0);
        //m_pbr = Quaternion.Euler(-m_horizontalCellAngle , m_verticalCellAngle , 0);
        //m_ptl = Quaternion.Euler(m_horizontalCellAngle , -m_verticalCellAngle , 0);
        //m_ptr = Quaternion.Euler(-m_horizontalCellAngle , -m_verticalCellAngle , 0);

    }
    public void FlushPointsInfo(ref Vector3[] dataToFlushWith, ref Vector2[] dataToFlushWithUV)
    {
        m_squareMeshVertices = new NativeArray<Vector3>(dataToFlushWith, Allocator.TempJob);
        m_squareMeshUV = new NativeArray<Vector2>(dataToFlushWithUV, Allocator.TempJob);


    }
    public void RecoverAndDispose(ref Mesh meshToAffect)
    {
        meshToAffect.SetVertices(m_squareMeshVertices);
        meshToAffect.SetUVs(0,m_squareMeshUV);
        m_squareMeshVertices.Dispose();
        m_squareMeshUV.Dispose();
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

    public void SetSquare(int squareIndex, ref SquareFarPixel pixelInfo)
    {

        int pointIndex = squareIndex * 4;

        float hAdd = 0;
        float vAdd = 0;
        float v=0;
        if (pixelInfo.m_verticalIndex < 0)
            vAdd += m_verticalCellAngle * v;

        if (pixelInfo.m_verticalIndex > 0)
            vAdd -= m_verticalCellAngle * v;

        if (pixelInfo.m_horizontalIndex < 0)
            hAdd -= m_horizontalCellAngle * v;

        if (pixelInfo.m_horizontalIndex > 0)
            hAdd += m_horizontalCellAngle* v;

        Vector3 forward =
            Quaternion.Euler(
                (-pixelInfo.m_verticalIndex * m_verticalCellAngle) + vAdd
                //+(pixelInfo.m_verticalIndex > 0f ? -m_verticalCellAngle : m_verticalCellAngle) * 0.5f
                ,
                (pixelInfo.m_horizontalIndex * m_horizontalCellAngle) + hAdd
                //+(pixelInfo.m_horizontalIndex > 0f ? -m_horizontalCellAngle : m_horizontalCellAngle) * 0.5f
                ,

                0) * Vector3.forward;
        m_squareMeshVertices[pointIndex + 0] = (m_pbl * forward * pixelInfo.m_minDistance);
        m_squareMeshVertices[pointIndex + 1] = (m_pbr * forward * pixelInfo.m_minDistance);
        m_squareMeshVertices[pointIndex + 2] = (m_ptl * forward * pixelInfo.m_minDistance);
        m_squareMeshVertices[pointIndex + 3] = (m_ptr * forward * pixelInfo.m_minDistance);

        float pourcent = 0;
        if (m_type == SquareFarPixel2Mesh_GradiantUVWithDistance.IntencityType.Distance)
        {
            pourcent=(pixelInfo.m_minDistance - m_configuration.m_startDistance)
            / (m_configuration.m_endDistance - m_configuration.m_startDistance);
            if (m_inverseGradiant)
                pourcent = 1f - pourcent;
        }
        else if (m_type == SquareFarPixel2Mesh_GradiantUVWithDistance.IntencityType.Count)
        {
            // pourcent = ( pixelInfo.m_count / (float)m_maxCountIntensity);

            pourcent = (pixelInfo.m_count / (float) m_maxCountIntensity);
            if (m_inverseGradiant)
                pourcent = 1f - pourcent;
            if (pourcent < 0.01f)
                pourcent = 0.01f;
            else if (pourcent > 0.99f)
                pourcent = 0.99f;
        }
        m_squareMeshUV[pointIndex + 0] = new Vector2(0, pourcent);
        m_squareMeshUV[pointIndex + 1] = new Vector2(1, pourcent);
        m_squareMeshUV[pointIndex + 2] = new Vector2(0, pourcent );
        m_squareMeshUV[pointIndex + 3] = new Vector2(1, pourcent );


    }
    public float m_maxCountIntensity;
    public void SetCountIntensityMax(int max) {
        m_maxCountIntensity = max;
    }
    public SquareFarPixel2Mesh_GradiantUVWithDistance.IntencityType m_type;
    internal void SetType(SquareFarPixel2Mesh_GradiantUVWithDistance.IntencityType m_intecityType)
    {
        m_type = m_intecityType;
    }

    public bool m_inverseGradiant;
    internal void InverseGradiant(bool inverseGradiant)
    {
        m_inverseGradiant = inverseGradiant;
    }

    Quaternion m_pbl;
    Quaternion m_pbr;
    Quaternion m_ptl;
    Quaternion m_ptr;
}
