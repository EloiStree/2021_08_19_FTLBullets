using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoExperimentalBulletManager : AbstractBulletsManager
{

    public int m_bulletId;

    public override void GetSpeedLightBulletsInfo(uint id, ComputedBulletInfo bulletInfo)
    {
        throw new System.NotImplementedException();
    }

    public override void RequestBulletFire(Vector3 startPoint,
        Vector3 direction,
        float speedInUnityPerSecond, 
        float lifeTimeInSeconds,
        out IBulletIdTicket ticket)
    {
        ticket = GetTicket();

    }

    private IBulletIdTicket GetTicket()
    {
        IBulletIdTicket ticket = new DefaultBulletTicket(m_bulletId);
        m_bulletId++;
        return ticket;
    }
}


public class DefaultBulletTicket: IBulletIdTicket {
    private int m_id;

    public DefaultBulletTicket(int id)
    {
        m_id = id;
    }
    public void GetId(out int id)
    {
        id = m_id;
    }
}
