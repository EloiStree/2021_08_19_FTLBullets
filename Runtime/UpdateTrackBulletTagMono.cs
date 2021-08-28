using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UpdateTrackBulletTagMono : AbstractBulletTagMono
{
    public UpdateTrackLineColliderMono m_positionTracked;
    protected void Start()
    {
        BulletInScene.AddBulletInScene(gameObject, m_positionTracked);
    }
    protected void OnDestroy()
    {
        BulletInScene.RemoveBulletInScene(gameObject);
    }

    public override void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition)
    {
        m_positionTracked.GetLineAsPoints(out startPointPosition, out endPointPosition);
    }

    public override void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius)
    {
        m_positionTracked.GetLineAsPointsWithRadius(out startPointPosition, out endPointPosition,out radius);
    }

    public override void GetLineRadius(out float radius)
    {
        m_positionTracked.GetLineRadius(out radius);
    }
}


public interface IBulletTagMono : ICapsuleLine
{

}
public interface IBulletableTagMono : ICapsuleLine
{

}
[Serializable]
public struct Vector3CapsuleLine : ICapsuleLine
{
    public Vector3 m_start;
    public Vector3 m_end;
    public float m_radius;

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


    public void SetWithWorldPositionOf(TransformCapsuleLine source)
    {
        m_start = source.m_start.position;
        m_end = source.m_end.position;
        m_radius = source.m_radius;

    }
}
[Serializable]
public struct TransformCapsuleLine : ICapsuleLine
{
    public Transform m_start;
    public Transform m_end;
    public float m_radius;
    public void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition)
    {
        startPointPosition = m_start.position;
        endPointPosition = m_end.position;
    }

    public void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius)
    {
        startPointPosition = m_start.position;
        endPointPosition = m_end.position;
        radius = m_radius;
    }

    public void GetLineRadius(out float radius)
    {
        radius = m_radius;
    }


}
public abstract class AbstractBulletTagMono : MonoBehaviour, IBulletTagMono
{
    public abstract void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition);
    public abstract void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius);
    public abstract void GetLineRadius(out float radius);
}

public abstract class AbstractBulletableTagMono : MonoBehaviour, IBulletableTagMono {

    public abstract void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition);
    public abstract void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius);
    public abstract void GetLineRadius(out float radius);
}




public abstract class GameObjectCapsuleLineRef : ICapsuleLineWithId
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
        id = "BIS" + m_linkedObject.GetInstanceID();
    }
}

public class BulletableGameObjectCapsuleLineRef : GameObjectCapsuleLineRef
{
    public override void GetUniqueId(out string id)
    {
        id = "BAIS" + m_linkedObject.GetInstanceID();
    }
}


public delegate void GameObjectCapsuleLineRefEvent(GameObjectCapsuleLineRef source);
public delegate void GameObjectCapsuleLineIdRefEvent(string id);

public class BulletInScene {

    public static GameObjectCapsuleLineRefEvent   m_onCreated;
    public static GameObjectCapsuleLineRefEvent   m_onWillBeRemoved;
    public static GameObjectCapsuleLineIdRefEvent m_onIsRemoved;

    public static Dictionary<string, BulletGameObjectCapsuleLineRef> m_bullet = new Dictionary<string, BulletGameObjectCapsuleLineRef>();

    public static void AddBulletInScene(GameObject objectLinked, ICapsuleLine bullet) {

        BulletGameObjectCapsuleLineRef r = new BulletGameObjectCapsuleLineRef();
        r.m_linkedObject = objectLinked;
        r.m_lineInfo = bullet;
        r.GetUniqueId(out string id);
        if (!m_bullet.ContainsKey(id)) { 
            m_bullet.Add(id, r);
            if(m_onCreated!=null)
            m_onCreated(r);
        }
        else
            m_bullet[id]= r;

    }
    public static void Remove(ICapsuleLineWithId line)
    {
        line.GetUniqueId(out string id);
        Remove(id);
    }
    public static void Remove(string id)
    {
        if (m_onWillBeRemoved != null) { 
            m_onWillBeRemoved(m_bullet[id]);
        }
        m_bullet.Remove(id);

        if (m_onIsRemoved != null)
            m_onIsRemoved(id);
    }
   
    public static void RemoveBulletInScene(GameObject objectLinked )
    {
        BulletGameObjectCapsuleLineRef r = new BulletGameObjectCapsuleLineRef();
        r.m_linkedObject = objectLinked;
        r.m_lineInfo = null;
        r.GetUniqueId(out string id);
        Remove(id);
    }

    public static void GetCount(out int bulletsCount)
    {
        bulletsCount = m_bullet.Count;
    }
}
public class BulletableInScene
{

    public static GameObjectCapsuleLineRefEvent m_onCreated;
    public static GameObjectCapsuleLineRefEvent m_onWillBeRemoved;
    public static GameObjectCapsuleLineIdRefEvent m_onIsRemoved;

    public static Dictionary<string, BulletableGameObjectCapsuleLineRef> m_bulletable = new Dictionary<string, BulletableGameObjectCapsuleLineRef>();

    public static void AddBulletInScene(GameObject objectLinked, ICapsuleLine bullet)
    {

        BulletableGameObjectCapsuleLineRef r = new BulletableGameObjectCapsuleLineRef();
        r.m_linkedObject = objectLinked;
        r.m_lineInfo = bullet;
        r.GetUniqueId(out string id);
        if (!m_bulletable.ContainsKey(id)) { 
            m_bulletable.Add(id, r);

            if (m_onCreated != null)
                m_onCreated(r);
        }
        else
            m_bulletable[id] = r;

    }
    public static void Remove(ICapsuleLineWithId line)
    {
        line.GetUniqueId(out string id);
        Remove(id);
    }
    public static void Remove(string id)
    {
        if (m_onWillBeRemoved != null)
        {
            m_onWillBeRemoved(m_bulletable[id]);
        }
        m_bulletable.Remove(id);
        if (m_onIsRemoved != null)
            m_onIsRemoved(id);
    }

    public static void RemoveBulletInScene(GameObject objectLinked)
    {

        BulletableGameObjectCapsuleLineRef r = new BulletableGameObjectCapsuleLineRef();
        r.m_linkedObject = objectLinked;
        r.m_lineInfo = null;
        r.GetUniqueId(out string id);
        Remove(id);

    }

    public static void GetCount(out int bulletsCount)
    {
        bulletsCount = m_bulletable.Count;
    }
}