using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBulletsManager : MonoBehaviour, IBulletsUserFacade, IBulletManagerSpeedLightBulletInfoAccess
{
    public abstract void GetSpeedLightBulletsInfo(uint id, ComputedBulletInfo bulletInfo);
    public abstract void RequestBulletFire(Vector3 startPoint, Vector3 direction, float speedInUnityPerSecond, float lifeTimeInSeconds, out IBulletIdTicket ticket);
}

public interface IBulletIdTicket
{
    void GetId(out int id);
}

public interface IBulletsUserFacade {

    void RequestBulletFire(Vector3 startPoint, Vector3 direction, float speedInUnityPerSecond, float lifeTimeInSeconds, out IBulletIdTicket ticket);
    
}



public interface IBulletManagerSpeedLightBulletInfoAccess {

    public void GetSpeedLightBulletsInfo(uint id, ComputedBulletInfo bulletInfo);
}


public interface IBulletPositionState {

    void IsInUse(out bool isUse);
    void GetDirection(out Vector3 direction);
    void GetCurrentPosition(out Vector3 position);
    void GetPreviousPosition(out Vector3 position);
    void GetCurrentLifetime(out float time);
    void GetTimeLeft(out float time);

}

public interface IBulletInitValue
{
    void IsActive(out bool value);
    void GetStartPoint(out Vector3 startPoint);
    void GetDirectionPoint(out Vector3 directionPoint);
    void WhenBulletWasFired(out float giveTimeInSecondsWhenTriggered);
}


public interface IBulletsVisualManager {

    void SpawnVisualDisplay(IBulletIdTicket ticket, out IBulletVisualInUnity visual);
    void AccessVisualDisplay(IBulletIdTicket ticket, out IBulletVisualInUnity visual);
    void UnspwanVisualDisplay(IBulletIdTicket ticket, out IBulletVisualInUnity visual);
}

public interface IBulletVisualInUnity {

    void GetLinkedBulletTicket(out IBulletIdTicket ticket);
    void GetLinkedTransformRoot(out Transform root);
    void GetAllTransformAsArray(out Transform[] asArray);
}