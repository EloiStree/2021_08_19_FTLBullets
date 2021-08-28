using UnityEngine;



public interface IBulletsExistanceCollection
{
    void AssociateTransformTo(IBulletIdTicket id, Transform objectTarget);
    void UnlinkTransform(IBulletIdTicket id);
    void GetTransformOf(IBulletIdTicket id, out Transform objectLinkedToBullet);
    bool HasTransformExistance(IBulletIdTicket id);
    void GetMaxSizeOfId(out uint maxSize);

}
public interface IBulletsExistanceManager: IBulletsExistanceCollection
{

    void CreateEmptyTransformTo(IBulletIdTicket id, out Transform created, string name="Bullet", Transform parent=null);
    void CreatePrimitiveTransformTo(IBulletIdTicket id, out Transform created, PrimitiveType primitive, string name = "Bullet", Transform parent = null);
    void DestroyRequestOf(IBulletIdTicket id);

}
public abstract class AbstractBulletsExistanceManager  : MonoBehaviour, IBulletsExistanceManager
{
    public abstract void AssociateTransformTo(IBulletIdTicket id, Transform objectTarget);
    public abstract void DestroyRequestOf(IBulletIdTicket id);
    public abstract void GetTransformOf(IBulletIdTicket id, out Transform objectLinkedToBullet);
    public abstract bool HasTransformExistance(IBulletIdTicket id);
    public abstract void GetMaxSizeOfId(out uint maxSize);
    public abstract void UnlinkTransform(IBulletIdTicket id);
    public abstract void CreateEmptyTransformTo(IBulletIdTicket id, out Transform created, string name = "Bullet", Transform parent = null);
    public abstract void CreatePrimitiveTransformTo(IBulletIdTicket id, out Transform created, PrimitiveType primitive, string name = "Bullet", Transform parent = null);
}
