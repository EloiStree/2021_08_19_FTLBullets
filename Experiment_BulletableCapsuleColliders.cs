using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Rendering;

public class Experiment_BulletableCapsuleColliders : MonoBehaviour
{
    public float m_bulletDefaultSize = 0.5f;
    public float m_targetableDefaultSize = 2f;
    public int m_stressTestCount=200;
    static EntityManager eManager;
    public List<Entity> m_bulletsColliderEntity;
    void Start()
    {
        eManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        for (int i = 0; i < m_stressTestCount; i++)
        {
            SpawnBulletCollider(i);
            SpawnBulletableCollider(i);
        }

    }

    private void SpawnBulletCollider(int i)
    {
        float3 position = new float3(0, i, i);
        Entity entity = eManager.CreateEntity(typeof(LocalToWorld), typeof(RenderMesh),  typeof(FTLBulletCollider));
        eManager.SetComponentData(entity, new FTLBulletCollider()
        {
            m_capsuleRadius = m_targetableDefaultSize,
            m_localCurrentPosition = position + new float3(-1, 0, 0),
            m_localPreviousPosition = position + new float3(1, 0, 0)
        });
        eManager.SetName(entity, "Bullet: " + i);



    }
    private void SpawnBulletableCollider(int i)
    {
        float3 position = new float3(0, i*2, i*2);
        Entity entity = eManager.CreateEntity(typeof(LocalToWorld), typeof(RenderMesh), typeof(FTLBulletableZoneCollider));
        eManager.SetComponentData(entity, new FTLBulletableZoneCollider()
        {
            m_capsuleRadius = m_bulletDefaultSize,
            m_localStartPosition = position + new float3(-1, 0, 0),
            m_localEndTailPosition = position + new float3(1, 0, 0)
        });
        eManager.SetName(entity, "Bulletable: " + i);
    }

}


public struct FTLBulletCollider : IComponentData
{

    public float3 m_localCurrentPosition;
    public float3 m_localPreviousPosition;
    public float m_capsuleRadius;
}
public struct FTLBulletableZoneCollider : IComponentData
{
    public float3 m_localStartPosition;
    public float3 m_localEndTailPosition;
    public float m_capsuleRadius;
}