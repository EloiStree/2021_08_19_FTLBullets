using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class DebugBulletRegisterClaimIDMono : MonoBehaviour
{

    public CapsuleLineRegisterClaimIDMono m_mergePointOfDataToProcess;
    // Start is called before the first frame update
    void Start()
    {
        BulletInScene.m_onCreated += AddBulletToRegister;
        BulletInScene.m_onIsRemoved += RemoveBulletToRegister;
    }

    private void RemoveBulletToRegister(string idName)
    {
       // m_mergePointOfDataToProcess.Unclaim(idName);
    }

    private void AddBulletToRegister(GameObjectCapsuleLineRef source)
    {
        source.GetUniqueId(out string idName);
        //m_mergePointOfDataToProcess.Claim(idName, out int id);
    }

   
}

public class CapsuleLineRegisterClaimIDMono {


    public ClaimIndexRegister m_claimRegister;
    public Dictionary<int, string> m_idToNameIdJoin = new Dictionary<int, string>();
    public Dictionary<string,int> m_nameToIdJoin = new Dictionary<string,int >();


    public NativeArray<S_CapsuleLine> m_bullets = new NativeArray<S_CapsuleLine>();

    public int elementCount = -1;
    public void SetMaxSize(int elementCount) {
        m_claimRegister = new ClaimIndexRegister(elementCount);
        if (elementCount >-1 )
            m_bullets.Dispose();
        S_CapsuleLine [] array = new S_CapsuleLine[elementCount];
        m_bullets = new NativeArray<S_CapsuleLine>(array, Allocator.Persistent);
        m_claimRegister.SetMaxSize(elementCount);
    }

    public void ClaimOverride(int id, string idName) {
        if (!m_idToNameIdJoin.ContainsKey(id))
        {
            m_idToNameIdJoin.Add(id, idName);
        }
        else m_idToNameIdJoin[id] = idName;
        if (!m_nameToIdJoin.ContainsKey(idName))
        {
            m_nameToIdJoin.Add(idName, id);
        }
        else m_nameToIdJoin[idName] = id;
    }

    public void ClaimNewOrFreeId(string idName, out int id)
    {
        m_claimRegister.Claim(out id);

        if (!m_idToNameIdJoin.ContainsKey(id))
        {
            m_idToNameIdJoin.Add(id, idName);
        }
        else m_idToNameIdJoin[id]= idName;
        if (!m_nameToIdJoin.ContainsKey(idName))
        {
            m_nameToIdJoin.Add(idName, id);
        }
        else m_nameToIdJoin[idName] = id;

    }


    public void Unclaim(int id)
    {
        if (m_idToNameIdJoin.ContainsKey(id))
        {
            string name = m_idToNameIdJoin[id];
            m_nameToIdJoin.Remove(name);
        }
        m_idToNameIdJoin.Remove(id);

    }
}

public interface StructCapsuleLine : ICapsuleLine{

    void GetIntId(out int id);
}

public struct S_CapsuleLine : StructCapsuleLine
{

    public int m_id;
    public Vector3 m_start;
    public Vector3 m_end;
    public float m_radius;

    public void GetIntId(out int id)
    {
        id= m_id;
    }

    public void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition)
    {
        startPointPosition = m_start;
        endPointPosition = m_end;
    }

    public void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius)
    {
        startPointPosition = m_start;
        endPointPosition = m_end;
        radius = m_radius;
    }

    public void GetLineRadius(out float radius)
    {
        radius = m_radius;
    }

    public void SetWith(ICapsuleLine target)
    {
        target.GetLineAsPointsWithRadius(out m_start, out m_end, out m_radius);
    }
}

