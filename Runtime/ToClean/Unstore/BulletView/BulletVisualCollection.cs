
using UnityEngine;

public class BulletVisualCollection : IBulletsExistanceCollection
{
    public uint m_size;
    private Transform [] m_visualTransform;

    public BulletVisualCollection(int size)
    {
        m_size = (uint)size;
        m_visualTransform = new Transform[size];
    }

    public void AssociateTransformTo(IBulletIdTicket id, Transform objectTarget)
    {
        id.GetId(out int ticketId);
        if (ticketId < 0 || ticketId >= m_size) 
            return;
        m_visualTransform[ticketId] = objectTarget;
    }

    public void GetMaxSizeOfId(out uint maxSize)
    {
        maxSize = m_size;
    }

    public void GetTransformOf(IBulletIdTicket id, out Transform objectLinkedToBullet)
    {
        id.GetId(out int ticketId);
        if (ticketId < 0 || ticketId >= m_size) { 
            objectLinkedToBullet=  null;
            return;
        }
        objectLinkedToBullet = m_visualTransform[ticketId];
    }

    public bool HasTransformExistance(IBulletIdTicket id)
    {
        id.GetId(out int ticketId);
        if (ticketId < 0 || ticketId >= m_size)
            return false;
       return  m_visualTransform[ticketId] != null;
    }

    public void UnlinkTransform(IBulletIdTicket id)
    {
        id.GetId(out int ticketId);
        if (ticketId < 0 || ticketId >= m_size)
            return ;
         m_visualTransform[ticketId] = null;
    }
}
