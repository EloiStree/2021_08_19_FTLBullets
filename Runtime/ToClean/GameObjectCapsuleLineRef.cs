using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class AbstractBulletTagMono : MonoBehaviour, ICapsuleLine
{
    public abstract void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition);
    public abstract void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius);
    public abstract void GetLineRadius(out float radius);
}

public abstract class AbstractBulletableTagMono : MonoBehaviour, ICapsuleLine
{

    public abstract void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition);
    public abstract void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius);
    public abstract void GetLineRadius(out float radius);
}




public abstract class GameObjectCapsuleLineRef : ICapsuleLineWithStringId
{
    public GameObject m_linkedObject;
    public ICapsuleLine m_lineInfo;

    public void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition)
    {
        m_lineInfo.GetLineAsPoints(out startPointPosition, out endPointPosition);
    }

    public void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius)
    {
        m_lineInfo.GetLineAsPointsWithRadius(out startPointPosition, out endPointPosition, out radius);
    }

    public void GetLineRadius(out float radius)
    {
        m_lineInfo.GetLineRadius(out radius);
    }

    public abstract void GetUniqueId(out string id);
}

public class BulletGameObjectCapsuleLineRef : GameObjectCapsuleLineRef
{
    public override void GetUniqueId(out string id)
    {
        id = "BCL" + m_linkedObject.GetInstanceID();
    }
}

public class BulletableGameObjectCapsuleLineRef : GameObjectCapsuleLineRef
{
    public override void GetUniqueId(out string id)
    {
        id = "BACL" + m_linkedObject.GetInstanceID();
    }
}


public delegate void GameObjectCapsuleLineRefEvent(GameObjectCapsuleLineRef source);
public delegate void GameObjectCapsuleLineIdRefEvent(string id);

//public class BulletInScene {

//    public static GameObjectCapsuleLineRefEvent   m_onCreated;
//    public static GameObjectCapsuleLineRefEvent   m_onWillBeRemoved;
//    public static GameObjectCapsuleLineIdRefEvent m_onIsRemoved;

//    public static Dictionary<string, BulletGameObjectCapsuleLineRef> m_bullet = new Dictionary<string, BulletGameObjectCapsuleLineRef>();

//    public static void AddBulletInScene(GameObject objectLinked, ICapsuleLine bullet) {

//        BulletGameObjectCapsuleLineRef r = new BulletGameObjectCapsuleLineRef();
//        r.m_linkedObject = objectLinked;
//        r.m_lineInfo = bullet;
//        r.GetUniqueId(out string id);
//        if (!m_bullet.ContainsKey(id)) { 
//            m_bullet.Add(id, r);
//            if(m_onCreated!=null)
//            m_onCreated(r);
//        }
//        else
//            m_bullet[id]= r;

//    }
//    public static void Remove(ICapsuleLineWithStringId line)
//    {
//        line.GetUniqueId(out string id);
//        Remove(id);
//    }
//    public static void Remove(string id)
//    {
//        if (m_onWillBeRemoved != null) { 
//            m_onWillBeRemoved(m_bullet[id]);
//        }
//        m_bullet.Remove(id);

//        if (m_onIsRemoved != null)
//            m_onIsRemoved(id);
//    }
   
//    public static void RemoveBulletInScene(GameObject objectLinked )
//    {
//        BulletGameObjectCapsuleLineRef r = new BulletGameObjectCapsuleLineRef();
//        r.m_linkedObject = objectLinked;
//        r.m_lineInfo = null;
//        r.GetUniqueId(out string id);
//        Remove(id);
//    }

//    public static void GetCount(out int bulletsCount)
//    {
//        bulletsCount = m_bullet.Count;
//    }
//}
//public class BulletableInScene
//{

//    public static GameObjectCapsuleLineRefEvent m_onCreated;
//    public static GameObjectCapsuleLineRefEvent m_onWillBeRemoved;
//    public static GameObjectCapsuleLineIdRefEvent m_onIsRemoved;

//    public static Dictionary<string, BulletableGameObjectCapsuleLineRef> m_bulletable = new Dictionary<string, BulletableGameObjectCapsuleLineRef>();

//    public static void AddBulletInScene(GameObject objectLinked, ICapsuleLine bullet)
//    {

//        BulletableGameObjectCapsuleLineRef r = new BulletableGameObjectCapsuleLineRef();
//        r.m_linkedObject = objectLinked;
//        r.m_lineInfo = bullet;
//        r.GetUniqueId(out string id);
//        if (!m_bulletable.ContainsKey(id)) { 
//            m_bulletable.Add(id, r);

//            if (m_onCreated != null)
//                m_onCreated(r);
//        }
//        else
//            m_bulletable[id] = r;

//    }
//    public static void Remove(ICapsuleLineWithStringId line)
//    {
//        line.GetUniqueId(out string id);
//        Remove(id);
//    }
//    public static void Remove(string id)
//    {
//        if (m_onWillBeRemoved != null)
//        {
//            m_onWillBeRemoved(m_bulletable[id]);
//        }
//        m_bulletable.Remove(id);
//        if (m_onIsRemoved != null)
//            m_onIsRemoved(id);
//    }

//    public static void RemoveBulletInScene(GameObject objectLinked)
//    {

//        BulletableGameObjectCapsuleLineRef r = new BulletableGameObjectCapsuleLineRef();
//        r.m_linkedObject = objectLinked;
//        r.m_lineInfo = null;
//        r.GetUniqueId(out string id);
//        Remove(id);

//    }

//    public static void GetCount(out int bulletsCount)
//    {
//        bulletsCount = m_bulletable.Count;
//    }
//}