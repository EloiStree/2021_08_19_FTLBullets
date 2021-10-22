using UnityEngine;

public interface IBulletsUserFacade {

    void RequestBulletFire(Vector3 startPoint, Vector3 direction, float speedInUnityPerSecond, float lifeTimeInSeconds, float radius, out IBulletIdTicket ticket);
    void RequestBulletFire(Vector3 startPoint, Vector3 direction,Quaternion initRotation, float speedInUnityPerSecond, float lifeTimeInSeconds, float radius, out IBulletIdTicket ticket);
    void NotifyBulletAsNotUsedAnymore(IBulletIdTicket m_bulletTicket);
}
