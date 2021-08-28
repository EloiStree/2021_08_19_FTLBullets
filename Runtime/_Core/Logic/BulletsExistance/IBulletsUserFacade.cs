using UnityEngine;

public interface IBulletsUserFacade {

    void RequestBulletFire(Vector3 startPoint, Vector3 direction, float speedInUnityPerSecond, float lifeTimeInSeconds, out IBulletIdTicket ticket);
    
}
