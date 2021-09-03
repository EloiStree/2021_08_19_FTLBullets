using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet2BulletableCollisionListener : MonoBehaviour
{
    public AbstractBulletsManager m_bulletJobSystem;
    public Experiment_JobComputeBulletCapsuleLine m_listenTo;
    public Temporary_SetJobComputeTargetWithMono m_bulletable;


    public List<S_CapsulesCollision> m_collisions =new List<S_CapsulesCollision>();

    public  CapsulesCollisionUnityEvent m_collisionEvent;
    public CapsulesCollisionWithSourceUnityEvent m_collisionWithInfo;

    public int m_receivedCount;

    public Queue<S_CapsulesCollision> m_collisionStack= new Queue<S_CapsulesCollision>();
    void Awake()
    {

        m_listenTo.m_listenToCollisionShortenPath += CatchCollision;
    }
    private void OnDestroy()
    {
        m_listenTo.m_listenToCollisionShortenPath -= CatchCollision;
    }
    public void ReleaseCollisionCatched() {

        while (m_collisionStack.Count>0) 
        {
            ManageCollision(m_collisionStack.Dequeue());
        }
    }

  
    private void CatchCollision(S_CapsulesCollision collisionEmitted)
    {
        m_collisionStack.Enqueue(collisionEmitted);
        m_receivedCount++;
      
    }
    private void ManageCollision(S_CapsulesCollision collisionEmitted)
    {
        m_collisionEvent.Invoke(collisionEmitted);

        //VERY BAD CODE BUT I WANT TO SEE A KIND OF A CODE WORKs.
        CollisionInfo info = new CollisionInfo();
        info.m_collisionRaw = collisionEmitted;
        info.m_bulletManager = m_bulletJobSystem;
        info.m_bulletableMono = m_bulletable.Get(collisionEmitted.m_bulletCapsuleId);
        info.m_bulletTicket = new DefaultBulletTicket(collisionEmitted.m_bulletableCapsuleId);
        m_collisionWithInfo.Invoke(info);
    }
}

[System.Serializable]
public class CapsulesCollisionUnityEvent : UnityEvent<S_CapsulesCollision>
{

}
[System.Serializable]
public class CapsulesCollisionWithSourceUnityEvent : UnityEvent<CollisionInfo>
{

}

public class CollisionInfo {

    public S_CapsulesCollision m_collisionRaw;
    public AbstractCapsuleLineMono m_bulletableMono;
    public IBulletIdTicket m_bulletTicket;
    public AbstractBulletsManager m_bulletManager;
    //public IBulletableTicket m_bulletable;
}
