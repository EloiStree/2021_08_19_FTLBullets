using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

 
public class Experiment_BulletableMirrorEntityData : ComponentSystem
{

    protected override void OnUpdate()
    {
        Entities.WithAll<FTLBulletCollider>().ForEach((Entity entity, ref FTLBulletCollider collider) =>
        {
            //position.Value = new float4x4(githu
            //    float3x3.LookRotation(collider.m_localStartPosition - collider.m_localEndTailPosition,
            //    ,new float3(0, 1, 0));
            Debug.DrawLine(collider.m_localCurrentPosition, collider.m_localPreviousPosition, Color.green);
        });
        Entities.WithAll<FTLBulletableZoneCollider>().ForEach((Entity entity, ref FTLBulletableZoneCollider collider) =>
        {
            //position.Value = new float4x4(
            //    float3x3.LookRotation(collider.m_localStartPosition - collider.m_localEndTailPosition,
            //    ,new float3(0, 1, 0));
            Debug.DrawLine(collider.m_localStartPosition, collider.m_localEndTailPosition, Color.red);
        });
    }

  
}
