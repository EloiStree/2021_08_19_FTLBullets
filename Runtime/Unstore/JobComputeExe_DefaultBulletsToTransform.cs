using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

//[BurstCompile(CompileSynchronously = true)]
[BurstCompile(CompileSynchronously = false)]
public struct JobComputeExe_DefaultBulletsToTransform: IJobParallelForTransform
{
    public NativeArray<BulletDataResult> m_bulletsResult;
    public bool m_useLocalPosition;
    public bool m_takeInChargeRotation;

    public void SetSharedMemory( NativeArray<BulletDataResult> bulletResultParamsMemory)
    {
        m_bulletsResult = bulletResultParamsMemory;
    }

    public void Execute(int index, TransformAccess transform)
    {
        if (!transform.isValid || !m_bulletsResult[index].m_isUsed)
            return;
        Vector3 c = m_useLocalPosition ? transform.localPosition : transform.position;

        c.x = m_bulletsResult[index].m_currentPosition.x;
        c.y = m_bulletsResult[index].m_currentPosition.y;
        c.z = m_bulletsResult[index].m_currentPosition.z;

        if (m_takeInChargeRotation)
        {

            Vector3 previous = m_useLocalPosition ? transform.localPosition : transform.position;
            Vector3 dir = c - previous;
            Quaternion orientation = Quaternion.LookRotation(dir, Vector3.up);

            if (m_useLocalPosition)
                transform.localRotation = orientation;
            else
                transform.rotation = orientation;
        }

        if (m_useLocalPosition)
            transform.localPosition = c;
        else
            transform.position = c;
    }
}