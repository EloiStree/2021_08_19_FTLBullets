using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_CollisionAction : MonoBehaviour
{

    public Color m_shortestPath = Color.blue;
    public Color m_bullet = Color.green*0.5f;
    public Color m_bulletable = Color.green;
    public float m_timeDisplay = 5;

    public void DrawLineOfCollision(IBulletTargetLinesLink collision) {

        if (collision == null)
            return;
        Vector3 start, end;
        if (collision is BulletTargetLinesLinkWithComputeData)
        {

            BulletTargetLinesLinkWithComputeData cd = (BulletTargetLinesLinkWithComputeData)collision;
            start = cd.m_shortestLine.lineAPoint;
            end = cd.m_shortestLine.lineBPoint;
            Debug.DrawLine(start, end, m_shortestPath, m_timeDisplay);
        }
        
        collision.GetBulletAndTarget(out ICapsuleLine bullet, out ICapsuleLine bulletable);
        BulletDrawing.DrawLine(bullet, m_bullet, bulletable, m_bulletable, m_timeDisplay);

        // Add Raycast demo 
    }

}

public class BulletDrawing {

    public static void DrawLine(ICapsuleLine a, Color aColor, ICapsuleLine b, Color bColor, float timeInSeconds)
    {
        DrawLine(a, aColor, timeInSeconds);
        DrawLine(b, bColor, timeInSeconds);
    }

    private static void DrawLine(ICapsuleLine a, Color aColor, float timeInSeconds)
    {
        a.GetLineAsPoints(out Vector3 s, out Vector3 e);
        Debug.DrawLine(s, e, aColor, timeInSeconds);
    }
}



public interface IDuoCapsuleLines {
     void GetTwoLines(out ICapsuleLine a, out ICapsuleLine b);
}
public interface IBulletTargetLinesLink:IDuoCapsuleLines {

    void GetBulletAndTarget(out ICapsuleLine bullet, out ICapsuleLine bulletable);
    void SetBullet(ICapsuleLine bullet);
    void SetTarget(ICapsuleLine target);
}

public class BulletTargetLinesLinkWithComputeData : IBulletTargetLinesLink
{
    public IBulletTargetLinesLink m_bulletTargetLink;
    public ShortestLine m_shortestLine;

    public BulletTargetLinesLinkWithComputeData(IBulletTargetLinesLink bulletTargetLink, ShortestLine shortestLine)
    {
        m_bulletTargetLink = bulletTargetLink;
        m_shortestLine = shortestLine;
    }

    public void GetBulletAndTarget(out ICapsuleLine bullet, out ICapsuleLine bulletable)
    {
        m_bulletTargetLink.GetBulletAndTarget(out bullet, out bulletable);
    }

    public void GetTwoLines(out ICapsuleLine a, out ICapsuleLine b)
    {
        m_bulletTargetLink.GetTwoLines(out a , out b);
    }

    public void SetBullet(ICapsuleLine bullet)
    {
        m_bulletTargetLink.SetBullet(bullet);
    }

    public void SetTarget(ICapsuleLine target)
    {
        m_bulletTargetLink.SetTarget(target);
    }
}

[System.Serializable]
public struct ShortestLine {
   public  Vector3 lineAPoint;
    public Vector3 lineBPoint;
}

public class BulletTargetLinesLink : IBulletTargetLinesLink
{
    public ICapsuleLine m_bullet;
    public ICapsuleLine m_bulletable;
    public void GetTwoLines(out ICapsuleLine a, out ICapsuleLine b)
    {
        a = m_bullet; b = m_bulletable;
    }
    public void GetBulletAndTarget(out ICapsuleLine a, out ICapsuleLine b)
    {
        a = m_bullet; b = m_bulletable;
    }

    public void SetBullet(ICapsuleLine bullet)
    {
        m_bullet = bullet;
    }

    public void SetTarget(ICapsuleLine target)
    {
        m_bulletable = target;
    }
}

public interface I_ToProcessCollisions {
    void GetBullets(out IEnumerable<ICapsuleLineWithStringId> bulletsAsCapsuleLine);
    void AppendBullets(IEnumerable<ICapsuleLineWithStringId> bullets);
    void RemoveBullet(ICapsuleLineWithStringId bullet);

    void GetBulletable(out ICapsuleLineWithStringId bulletableTargetAsCapsuleLine);
    void SetBulletable(ICapsuleLineWithStringId bulletable);
}

public class ToProcessBulletableCollisionsWithDico: I_ToProcessCollisions
{
    LinkedList<ICapsuleLineWithStringId> m_ListIn = new LinkedList<ICapsuleLineWithStringId>();
    Dictionary<string, ICapsuleLineWithStringId> m_bullets = new Dictionary<string, ICapsuleLineWithStringId>();
    ICapsuleLineWithStringId m_target;

    public void AppendBullets(IEnumerable<ICapsuleLineWithStringId> bullets)
    {
        foreach (ICapsuleLineWithStringId line in bullets)
        {
            line.GetUniqueId(out string id);
            if (m_bullets.ContainsKey(id))
            {
                m_bullets[id] = line;
                m_ListIn.AddLast(line);
            }
            else { 
                m_bullets.Add(id, line);
            
            }
        }
    }
    public void RemoveBullet(ICapsuleLineWithStringId bullet)
    {
        bullet.GetUniqueId(out string id);
        m_bullets.Remove(id);
        m_ListIn.Remove(bullet);
    }
    public void GetBulletable(out ICapsuleLineWithStringId bulletableTargetAsCapsuleLine)
    {
        bulletableTargetAsCapsuleLine = m_target;
    }
    public void GetBullets(out IEnumerable<ICapsuleLineWithStringId> bulletsAsCapsuleLine)
    {
        bulletsAsCapsuleLine= m_ListIn;
    }
    public void SetBulletable(ICapsuleLineWithStringId bulletable)
    {
        m_target = bulletable;
    }
}
