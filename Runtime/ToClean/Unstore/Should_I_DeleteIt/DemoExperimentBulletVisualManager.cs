using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class DemoExperimentBulletVisualManager : AbstractBulletsExistanceManager
{

    public int m_maxNumberOfBulletsVisual= int.MaxValue;
    public BulletVisualCollection m_visual;
    public Transform m_parentGrouper;

    private void Awake()
    {
        m_visual = new BulletVisualCollection(m_maxNumberOfBulletsVisual);
    }

    public override void AssociateTransformTo(IBulletIdTicket id, Transform objectTarget)
    {
        m_visual.AssociateTransformTo(id, objectTarget);
    }
    public override void CreateEmptyTransformTo(IBulletIdTicket id, out Transform created, string name = "Bullet", Transform parent = null)
    {
        GameObject objCreated = new GameObject(name);
        objCreated.transform.parent = objCreated.transform;
        if (objCreated.transform.parent == null)
            objCreated.transform.parent = m_parentGrouper;

        created = objCreated.transform;
        m_visual.AssociateTransformTo(id, objCreated.transform);
    }

    public override void CreatePrimitiveTransformTo(IBulletIdTicket id, out Transform created, PrimitiveType primitive, string name = "Bullet", Transform parent = null)
    {
        GameObject objCreated = GameObject.CreatePrimitive(primitive);
        objCreated.transform.parent = objCreated.transform;
        if (objCreated.transform.parent == null)
            objCreated.transform.parent = m_parentGrouper;

        created = objCreated.transform;
        m_visual.AssociateTransformTo(id, objCreated.transform);
    }



    public override void DestroyRequestOf(IBulletIdTicket id)
    {
        m_visual.GetTransformOf(id, out Transform t);
        if (t != null)
            Destroy(t.gameObject);
    }

    public override void GetMaxSizeOfId(out uint maxSize)
    {
        m_visual.GetMaxSizeOfId(out maxSize);
    }

    public override void GetTransformOf(IBulletIdTicket id, out Transform objectLinkedToBullet)
    {
        m_visual.GetTransformOf(id, out objectLinkedToBullet);
    }

    public override bool HasTransformExistance(IBulletIdTicket id)
    {
        return m_visual.HasTransformExistance(id);
    }

    public override void UnlinkTransform(IBulletIdTicket id)
    {
        m_visual.UnlinkTransform(id);
    }

}
