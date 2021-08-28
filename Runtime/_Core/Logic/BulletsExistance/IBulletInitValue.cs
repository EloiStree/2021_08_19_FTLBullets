using UnityEngine;

public interface IBulletInitValue
{
    void IsActive(out bool value);
    void GetStartPoint(out Vector3 startPoint);
    void GetDirectionPoint(out Vector3 directionPoint);
    void WhenBulletWasFired(out float giveTimeInSecondsWhenTriggered);
}
