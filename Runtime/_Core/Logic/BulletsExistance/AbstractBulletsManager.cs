using System;
using System.Collections;
using System.Collections.Generic;




using UnityEngine;

public abstract class AbstractBulletsManager : MonoBehaviour, IBulletsUserFacade, IBulletManagerSpeedLightBulletInfoAccess
{
    public abstract void GetBulletInfo(uint id, ComputedBulletInfo bulletInfo);
    public abstract void RequestBulletFire(Vector3 startPoint, Vector3 direction, float speedInUnityPerSecond, float lifeTimeInSeconds,float radius, out IBulletIdTicket ticket);

    public abstract void NotifyBulletAsNotUsedAnymore(IBulletIdTicket m_bulletTicket);

    public abstract void RequestBulletFire(Vector3 startPoint, Vector3 direction, Quaternion initRotation, float speedInUnityPerSecond, float lifeTimeInSeconds, float radius, out IBulletIdTicket ticket);

}

