using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletTargetLineCapsuleCollisionListener : MonoBehaviour
{

    public DuoCapsuleLinesEvent m_collisionEmittedUnityEvent;
    public DuoCapsuleLineDelegateEvent m_collisionEmittedDelegEvent;
    public Queue<IBulletTargetLinesLink> m_frameCollisions= new Queue<IBulletTargetLinesLink>();
    public void NotifyNewCollision(IBulletTargetLinesLink collision) {

        if (m_collisionEmittedDelegEvent!=null)
            m_collisionEmittedDelegEvent(collision);
        m_collisionEmittedUnityEvent.Invoke(collision);
    }

    public void GetFrameCollisionDetectedYet(out Queue<IBulletTargetLinesLink> registeredCollisions) {
        registeredCollisions = m_frameCollisions;
    }

    private void LateUpdate()
    {
        m_frameCollisions.Clear();
    }

}

public delegate void DuoCapsuleLineDelegateEvent(IBulletTargetLinesLink collision);
[System.Serializable]
public class DuoCapsuleLinesEvent : UnityEvent<IBulletTargetLinesLink> { }
