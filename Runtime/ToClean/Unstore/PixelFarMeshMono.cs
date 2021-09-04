using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class PixelFarMeshMono : MonoBehaviour
{

    public PixelFarConfig m_config;

    enum SourceType { Transform, Vector3 }
    SourceType m_sourceType;
    Transform[] m_transformTargets;
    TransformAccessArray m_nativeTransformTargets;
    Vector3[] m_worldLocation;
    NativeArray<Vector3> m_worldLocationTargets;

    public Camera m_useCameraAngle;


    bool m_relcotedIsInit;
    NativeArray<Vector3> m_relocatedPoints;


    PointAsPixelInfo[] m_pixelInfoTemp = new PointAsPixelInfo[0];
    Vector3[] m_relocateTemp = new Vector3[0];
    int[] m_countTemp = new int[0];

    RelocateJobTransform m_jobRelocatedFromTransform;
    RelocateJobWorldPosition m_jobRelocatedFromWorldVector3;
    LocalPositionToPixelCounts m_jobLocalToPixelCountsInfo;
    NativeArray<PointAsPixelInfo> m_pixelInfoCount;
    NativeArray<int> m_pixelElementCount;

    void Awake()
    {

        if (m_useCameraAngle != null)
            SetConfigWithCameraAngle();
        m_jobRelocatedFromTransform = new RelocateJobTransform();
        m_jobRelocatedFromWorldVector3 = new RelocateJobWorldPosition();
        m_jobLocalToPixelCountsInfo = new LocalPositionToPixelCounts();
        m_jobLocalToPixelCountsInfo.SetIniParms(m_config);
    }
     void OnDestroy()
    {
        if (m_nativeTransformTargets.isCreated) {
            m_nativeTransformTargets.Dispose();
        }
        if (m_relocatedPoints.IsCreated)
        {
            m_relocatedPoints.Dispose();
            m_pixelInfoCount.Dispose();
            m_pixelElementCount.Dispose();
        }
    }
    private void SetConfigWithCameraAngle()
    {
        if (m_useCameraAngle != null)
        {
            Vector3 left = Quaternion.Inverse(m_useCameraAngle.transform.rotation) * (m_useCameraAngle.ViewportToWorldPoint(new Vector3(-1f, 0, 10)) - m_useCameraAngle.transform.position);
            Vector3 right = Quaternion.Inverse(m_useCameraAngle.transform.rotation) * (m_useCameraAngle.ViewportToWorldPoint(new Vector3(1f, 0, 10)) - m_useCameraAngle.transform.position);
            Vector3 top = Quaternion.Inverse(m_useCameraAngle.transform.rotation) * (m_useCameraAngle.ViewportToWorldPoint(new Vector3(0, 1f, 10)) - m_useCameraAngle.transform.position);
            Vector3 down = Quaternion.Inverse(m_useCameraAngle.transform.rotation) * (m_useCameraAngle.ViewportToWorldPoint(new Vector3(0, -1f, 10)) - m_useCameraAngle.transform.position);
            m_config.m_horizontalAngle = Vector3.Angle(left, right);
            m_config.m_verticalAngle = Vector3.Angle(top, down);
        }

    }

    private bool transformInit;
    public void RefreshTargetRef(Transform[] pointsToPush)
    {
        m_sourceType = SourceType.Transform;
        if (transformInit) {
            m_nativeTransformTargets.Dispose();
        }
        m_nativeTransformTargets = new TransformAccessArray(pointsToPush.Length, 64);
        transformInit = true;
        m_nativeTransformTargets.SetTransforms(pointsToPush);
        CheckIfRelocationNeedReallocation(pointsToPush.Length);
    }

    private void CheckIfRelocationNeedReallocation(int sizeNeeded)
    {
        if (m_relocatedPoints == null || m_relocatedPoints.Length != sizeNeeded)
        {
            if (m_relcotedIsInit)
            {
                m_relocatedPoints.Dispose();
                m_pixelInfoCount.Dispose();
                m_pixelElementCount.Dispose();
                m_relcotedIsInit = true;
            }

            m_relocateTemp = new Vector3[sizeNeeded];
            m_pixelInfoTemp = new PointAsPixelInfo[sizeNeeded];
            m_countTemp = new int[sizeNeeded];

            m_relocatedPoints = new NativeArray<Vector3>(m_relocateTemp, Allocator.Persistent);
            m_pixelInfoCount = new NativeArray<PointAsPixelInfo>(m_pixelInfoTemp, Allocator.Persistent);
            m_pixelElementCount = new NativeArray<int>(m_countTemp, Allocator.Persistent);

            m_relcotedIsInit = true;
            m_jobLocalToPixelCountsInfo.SetWith(m_relocatedPoints, m_pixelInfoCount);
        }
    }

    public void RefreshTargetWorldPointsRef(NativeArray<Vector3> pointsToPush)
    {
        m_sourceType = SourceType.Vector3;
        m_worldLocationTargets = pointsToPush;
        CheckIfRelocationNeedReallocation(pointsToPush.Length);

    }
    public void RefreshCameraPosition(Vector3 worldPoint, Quaternion worldRotation)
    {
        m_jobRelocatedFromTransform.SetWorldPosition(worldPoint, worldRotation);
        m_jobRelocatedFromWorldVector3.SetWorldPosition(worldPoint, worldRotation);
    }
    public void ComputeFromSource( out NativeArray<SquareFarPixel> farPixels) {
        ComputeFromSource(m_pixelInfoCount, out farPixels);
    }
    public void ComputeFromSource(NativeArray<PointAsPixelInfo> pixelInfoCount, out NativeArray<SquareFarPixel> farPixels) {
        JobHandle job;
        if (m_sourceType == SourceType.Transform)
        {
            m_jobRelocatedFromTransform.SetNativeArrayRef(m_relocatedPoints);
            job = m_jobRelocatedFromTransform.Schedule(m_nativeTransformTargets);
            job.Complete();
        }
        else if (m_sourceType == SourceType.Vector3)
        {
            m_jobRelocatedFromWorldVector3.SetNativeArrayRef(m_worldLocationTargets, m_relocatedPoints);
            job = m_jobRelocatedFromWorldVector3.Schedule(m_worldLocationTargets.Length, 64);
            job.Complete();
        }
        job = m_jobLocalToPixelCountsInfo.Schedule(m_relocatedPoints.Length, 64);
        job.Complete();


        m_idToPixelInfo.Clear();
        for (int i = 0; i < pixelInfoCount.Length; i++)
        {
            if (pixelInfoCount[i].m_wasInZone)
            {
                uint id = pixelInfoCount[i].m_pixelArrayIndex;
                if (!m_idToPixelInfo.ContainsKey(id))
                {
                    m_idToPixelInfo.Add(id, new SquareFarPixel(id, 1, pixelInfoCount[i].m_distanceOfSource,
                        pixelInfoCount[i].m_horizontalAngleIndex,
                        pixelInfoCount[i].m_verticalAngleIndex));
                }
                else
                {
                    SquareFarPixel f = m_idToPixelInfo[id];
                    f.m_count++;
                    if (f.m_minDistance > pixelInfoCount[i].m_distanceOfSource) {
                        f.m_minDistance = pixelInfoCount[i].m_distanceOfSource;
                    }
                    m_idToPixelInfo[id] = f;
                }
            }
        }
        if (m_farPixelComputed.IsCreated)
            m_farPixelComputed.Dispose();
        m_farPixelComputed = new NativeArray<SquareFarPixel>(m_idToPixelInfo.Values.ToArray(), Allocator.TempJob);
        farPixels = m_farPixelComputed;

        // m_debug = m_pixelInfoCount.ToArray();
    }

    private void LateUpdate()
    {

        if (m_farPixelComputed.IsCreated)
            m_farPixelComputed.Dispose();
    }

    public NativeArray<SquareFarPixel> m_farPixelComputed;
    public Dictionary<uint, SquareFarPixel> m_idToPixelInfo = new Dictionary<uint, SquareFarPixel>();

    //public PointAsPixelInfo[] m_debug;


    //#NiceToHave
    //Should be code but when I have time.
    //[Tooltip("Represent pixel where its allowed to compute. To avoid computing zone that are not needed")]
    //public Texture2D m_filteringTexture;


   
}


[BurstCompile(CompileSynchronously = true)]
public struct RelocateJobTransform : IJobParallelForTransform
{
    public NativeArray<Vector3> m_relocated;
    public Vector3 m_sourcePosition;
    public Quaternion m_sourceRotationInverse;

    public void SetNativeArrayRef(NativeArray<Vector3> relocated)
    {
        m_relocated = relocated;
    }
    public void GetUseNativeArrayRef(out NativeArray<Vector3> relocated)
    {
        relocated = m_relocated;
    }


    public void Execute(int index, TransformAccess transform)
    {
        m_relocated[index] = m_sourceRotationInverse * (transform.position - m_sourcePosition);
    }

    public void SetWorldPosition(Vector3 worldPoint, Quaternion worldRotation)
    {
        m_sourcePosition = worldPoint;
        m_sourceRotationInverse = Quaternion.Inverse(worldRotation);
    }
}

[BurstCompile(CompileSynchronously = true)]
public struct RelocateJobWorldPosition : IJobParallelFor
{
    public NativeArray<Vector3> m_world;
    public NativeArray<Vector3> m_relocated;

    public Vector3 m_sourcePosition;
    public Quaternion m_sourceRotationInverse;

    public void SetNativeArrayRef(NativeArray<Vector3> worldPoints, NativeArray<Vector3> relocated)
    {
        m_world = worldPoints;
        m_relocated = relocated;
    }

    public void SetResourceInfo(Vector3 worldPosition, Quaternion worldRotation)
    {
        m_sourcePosition = worldPosition;
        m_sourceRotationInverse = Quaternion.Inverse(worldRotation);
    }

    public void Execute(int index)
    {
        m_relocated[index] = m_sourceRotationInverse * (m_world[index] - m_sourcePosition);
    }

    public void SetWorldPosition(Vector3 worldPoint, Quaternion worldRotation)
    {
        m_sourcePosition = worldPoint;
        m_sourceRotationInverse = Quaternion.Inverse(worldRotation);
    }
}

[BurstCompile(CompileSynchronously = true)]
public struct LocalPositionToPixelCounts : IJobParallelFor
{
    public PixelFarConfig m_config;

    public NativeArray<Vector3> m_relocatedPoints;
    public NativeArray<PointAsPixelInfo> m_relocatedPixelIndex;


        public float m_cellHorizontalAngle;
        public float m_cellVerticalAngle;
        public int m_halfWidthInPixel;
        public int m_halfHeightInPixel;
        public float m_halfHorizontalAngle;
        public float m_halfVerticalAngle;
        public int m_maxIndex;

        public void SetWith(NativeArray<Vector3> relocatedPoints, NativeArray<PointAsPixelInfo> pixelInfoCount)
        {
            m_relocatedPoints = relocatedPoints;
            m_relocatedPixelIndex = pixelInfoCount;
        }

        public void SetIniParms(PixelFarConfig configuration) {
            m_config = configuration;
            m_cellHorizontalAngle = m_config.m_horizontalAngle / (float) m_config.m_widthInPixel;
            m_cellVerticalAngle = m_config.m_verticalAngle / (float) m_config.m_heightInPixel;
            m_halfWidthInPixel = (int)(m_config.m_widthInPixel / 2);
            m_halfHeightInPixel = (int)(m_config.m_heightInPixel / 2);
            m_halfHorizontalAngle = m_config.m_horizontalAngle / 2f;
            m_halfVerticalAngle = m_config.m_verticalAngle / 2f;
            m_maxIndex = m_config.m_widthInPixel * m_config.m_heightInPixel;

        }


        public void Execute(int index)
        {
            Vector3 pt = m_relocatedPoints[index];
            PointAsPixelInfo ptResult = m_relocatedPixelIndex[index];
            ptResult.m_distanceOfSource = pt.magnitude;
            ptResult.m_wasInZone = true;
           
            if (pt.magnitude < m_config.m_startDistance || pt.z < m_config.m_startDistance || pt.magnitude > m_config.m_endDistance)
            {
                WasNotInZone(index);
                return;
            }

            float forwardLenght = pt.z;
            Vector3 pth = new Vector3(pt.x, 0, pt.z);
            Vector3 ptv = new Vector3(0 , pt.y, pt.z);

            float horizontal = (float) Math.Acos((float)forwardLenght /(float)pth.magnitude) * Mathf.Rad2Deg;
            if ( horizontal > m_halfHorizontalAngle)
            {
                WasNotInZone(index);
                return;
            }
            horizontal *= Math.Sign((float)pth.x);


            float vertical =  (float)Math.Acos((float)forwardLenght / (float)ptv.magnitude) * Mathf.Rad2Deg;
            if (vertical > m_halfVerticalAngle)
            {
                WasNotInZone(index);
                return;
            }
            vertical *= Math.Sign((float)ptv.y);

        //ptResult.m_horizontalAngle = horizontal;
        //ptResult.m_verticalAngle = vertical;
        float vp = vertical / m_cellVerticalAngle;
        float hp = horizontal / m_cellHorizontalAngle;
        //if (vp >= -1f && vp < 0f)
        //    vp = -1f;
        //else if (vp <= 1f && vp > 0f)
        //    vp = 1f;
        //if (hp >= -1f && hp <= 0f)
        //    hp = -1f;
        //else if (hp <= 1f && hp >= 0f)
        //    hp = 1f;

        ptResult.m_horizontalAngleIndex = (int) Math.Round(hp);
            ptResult.m_verticalAngleIndex = (int) Math.Round(vp);
            ptResult.m_horizontalAngleIndexR = (m_halfWidthInPixel + ptResult.m_horizontalAngleIndex);
            ptResult.m_verticalAngleIndexR = (m_halfHeightInPixel + ptResult.m_verticalAngleIndex);
           
            if (ptResult.m_horizontalAngleIndexR >= m_config.m_widthInPixel)
                ptResult.m_horizontalAngleIndexR = m_config.m_widthInPixel - 1;
            if (ptResult.m_verticalAngleIndexR >= m_config.m_heightInPixel)
                ptResult.m_verticalAngleIndexR = m_config.m_heightInPixel - 1;

            uint ii =
              (uint)(
              (ptResult.m_verticalAngleIndexR
               * ((int)m_config.m_widthInPixel)
              + ptResult.m_horizontalAngleIndexR
              ));

            if (ii >= m_maxIndex)
                ii = (uint)(m_maxIndex - 1);


            ptResult.m_pixelArrayIndex = ii;
            m_relocatedPixelIndex[index] = ptResult;
        }

       
        private void WasNotInZone(int index)
        {
            m_relocatedPixelIndex[index] = new PointAsPixelInfo() { m_wasInZone = false };
        }
       
    }

    [System.Serializable]
    public struct PointAsPixelInfo {
        public bool m_wasInZone;
        public uint m_pixelArrayIndex;
        //public float m_horizontalAngle;
        //public float m_verticalAngle;
        public int m_horizontalAngleIndex;
        public int m_verticalAngleIndex;
        public int m_horizontalAngleIndexR;
        public int m_verticalAngleIndexR;
        public float m_distanceOfSource;

    }

[System.Serializable]
public struct SquareFarPixel
{
    public uint m_index;
    public int m_count;
    public float m_minDistance;
    public int m_horizontalIndex;
    public int m_verticalIndex;

    public SquareFarPixel(uint index, int count, float minDistance, int horizontalIndex, int verticalIndex)
    {
        m_index = index;
        m_count = count;
        m_minDistance = minDistance;
        m_horizontalIndex = horizontalIndex;
        m_verticalIndex = verticalIndex;
    }
}
[System.Serializable]
public struct PixelFarConfig
{

    public float m_horizontalAngle;
    public float m_verticalAngle;
    public float m_startDistance;
    public float m_endDistance;
    public int m_widthInPixel;
    public int m_heightInPixel;

    internal void GetAngleOf(int squareIndex, out float startLeftAngle, out float endRightAngle, out float startBotAngle, out float endTopAngle)
    {
        throw new NotImplementedException();
    }
}


public abstract class AbstractSquareFarPixelRendererListener: MonoBehaviour
{
    public abstract void InitParams(PixelFarConfig config);
    public abstract void RefreshWith(Vector3 worldPoint, Quaternion worldRotation,  NativeArray<SquareFarPixel> farPixel);

}
