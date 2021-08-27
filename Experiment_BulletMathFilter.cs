using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment_BulletMathFilter : MonoBehaviour
{

    public Transform m_startPointA;
    public Transform m_endPointA;
    public float m_radiusA = 0.1f;

    public Transform m_startPointB;
    public Transform m_endPointB;
    public float m_radiusB = 0.1f;

    [Header("Debug")]
    public DebugInfo m_debug;
    public ReachFiltering m_reachFilter;
    public bool m_useDebugDraw;
    public enum ReachFiltering { LineANotInDistanceRange, NotEvenIn180GoodDirection, LineAOutsideVerticalAngle, LineAOutsideHorizontalAngle, MaybeReachable }
    private void Update()
    {
        Test(out m_reachFilter, ref  m_debug, m_useDebugDraw);
    }

    private void Test(out ReachFiltering reachableState,ref DebugInfo debugInfo, bool useDebugDraw)
    {
        reachableState = ReachFiltering.MaybeReachable;
        Vector3 startPointA; Vector3 endPointA; float lineRadiusA;
        Vector3 startPointB; Vector3 endPointB; float lineRadiusB;

        startPointA = m_startPointA.position;
        startPointB = m_startPointB.position;
        endPointA = m_endPointA.position;
        endPointB = m_endPointB.position;
        lineRadiusA = m_radiusA;
        lineRadiusB = m_radiusB;


        float radiusAandB = lineRadiusA + lineRadiusB;
        float distanceStart = (startPointB - startPointA).magnitude;
        float distanceEnd = (endPointB - startPointA).magnitude;
        float lineMaxRange = (endPointA - startPointA).magnitude + radiusAandB;
        if (lineMaxRange <= distanceStart && lineMaxRange <= distanceEnd)
        {
            reachableState = ReachFiltering.LineANotInDistanceRange;
            return;
        }


        //TO DO: CHeck that at least the bullet is going in the global 180 directoni of point;



        Vector3 targetDirectionCenter = ((startPointB + endPointB) / 2f) - startPointA;
        Vector3 up = Vector3.Cross(startPointB - startPointA, endPointB - startPointA);
        Quaternion relocateAngle = Quaternion.Inverse(Quaternion.LookRotation(targetDirectionCenter, up));
        Vector3 a = startPointA;
        startPointA = relocateAngle * (startPointA - a);
        endPointA = relocateAngle * (endPointA - a);
        startPointB = relocateAngle * (startPointB - a);
        endPointB = relocateAngle * (endPointB - a);

        if (useDebugDraw)
        {
            Debug.DrawLine(startPointB, endPointB, Color.red);
        }
        Vector3 startExtremB = startPointB + (startPointB - endPointB).normalized * radiusAandB;
        Vector3 endExtremB = endPointB + (endPointB - startPointB).normalized * radiusAandB; ;

        Vector3 startPointADir = startExtremB - startPointA;
        Vector3 endPointBDir = endPointB - startPointA;

        Vector3 shortestVector = startPointADir.magnitude < endPointBDir.magnitude ? startPointADir : endPointBDir;
        Vector3 upExtreamB = shortestVector + Vector3.up * radiusAandB;


        if (useDebugDraw) { 
            Debug.DrawLine(startExtremB + Vector3.up * 0.1f, endExtremB + Vector3.up * 0.1f, Color.yellow);
            Debug.DrawLine(shortestVector, upExtreamB, Color.yellow);
            Debug.DrawLine(shortestVector, shortestVector - Vector3.up * radiusAandB, Color.yellow);
            Debug.DrawLine(startPointA, endPointA + (endPointA - startPointA) , Color.white);
        }


        float verticalAngleMax = Vector2.Angle(new Vector3(upExtreamB.z, upExtreamB.y), Vector2.right);
        float pointAngleVertical = Vector2.Angle(new Vector3(endPointA.z, endPointA.y), Vector2.right);

        if (pointAngleVertical > verticalAngleMax) {
            reachableState = ReachFiltering.LineAOutsideVerticalAngle;
            return;
        }

        float extreStartBAngle = Vector2.Angle(new Vector3(startExtremB.x, startExtremB.z), Vector2.up);
        float extreEndBAngle = Vector2.Angle(new Vector3(endExtremB.x, endExtremB.z), Vector2.up);
        float bigestAngleHorizontal = extreStartBAngle > extreEndBAngle ? extreStartBAngle : extreEndBAngle;

        float pointAngleHorizontal = Vector2.Angle(new Vector3(endPointA.x, endPointA.z), Vector2.up);

        if (pointAngleHorizontal > bigestAngleHorizontal)
        {
            reachableState = ReachFiltering.LineAOutsideVerticalAngle;
            return;
        }



        if (debugInfo != null) {
            debugInfo. m_angleHorizontalLeft = extreStartBAngle;
            debugInfo.m_angleHorizontalRight = extreEndBAngle;
            debugInfo.m_angleVertical = verticalAngleMax;
            debugInfo.m_bulletAngleHorizontal = pointAngleHorizontal;
            debugInfo.m_bulletAngleVertical = pointAngleVertical;
        }
    }
    [System.Serializable]
    public class DebugInfo{
        public float m_angleHorizontalLeft;
        public float m_angleHorizontalRight;
        public float m_bulletAngleHorizontal;
        public float m_angleVertical;
        public float m_bulletAngleVertical;
    }

    //void UpdateArchived()
    //{


    //    m_bullet.GetWorldPoint(out Vector3 previousBullet, out Vector3 currentBullet);

    //    m_bulletable.GetWorldPoint(out Vector3 previousTarget, out Vector3 currentTarget, out float radius);
    //    m_bulletable.GetWorldPoint(out Vector3 previousExtremPoint , out previousTarget, out currentTarget,out Vector3 currentExtremPoint );

    //    Vector3 directionOfBullet = currentBullet - previousBullet;
    //    float speed = directionOfBullet.magnitude;
    //    Vector3 targetDirectionCurrent = (currentTarget - previousBullet);
    //    Vector3 targetDirectionPrevious = (previousTarget - previousBullet);

    //    Vector3 targetDirectionCurrentExtemity = (currentExtremPoint - previousBullet);
    //    Vector3 targetDirectionPreviousExtemity = (previousExtremPoint - previousBullet);
    //    Vector3 targetDirectionCenter = (targetDirectionCurrentExtemity + targetDirectionPreviousExtemity) /2f;
    //    m_isInGoodSide = !(Vector3.Angle(targetDirectionCenter, currentExtremPoint) > 90f && Vector3.Angle(targetDirectionCenter, targetDirectionPreviousExtemity) > 90f);


    //    UnityEngine.Debug.DrawLine(previousBullet, previousBullet + targetDirectionCurrentExtemity * 100f, Color.cyan, 1);
    //    UnityEngine.Debug.DrawLine(previousBullet, previousBullet + targetDirectionPreviousExtemity * 100f, Color.cyan, 1);

    //    m_isInReachCurrent = speed > (targetDirectionCurrent.magnitude - radius);
    //    m_isInReachPrevious = speed > (targetDirectionPrevious.magnitude - radius);
    //    m_isInReach = m_isInReachCurrent || m_isInReachPrevious;



    //    Vector3 forward = targetDirectionCenter;
    //    Vector3 up = Vector3.Cross(targetDirectionPreviousExtemity, targetDirectionCurrentExtemity);
    //    Vector3 right = Vector3.Cross(up, targetDirectionCenter);
    //    Quaternion relocateAngle =Quaternion.Inverse( Quaternion.LookRotation(forward, up) );

    //    UnityEngine.Debug.DrawLine(previousBullet, previousBullet + up * 100f, Color.green, 1);

    //    m_previousRelocated = relocateAngle * targetDirectionPreviousExtemity;
    //    m_currentRelocated = relocateAngle * targetDirectionCurrentExtemity;
    //    m_centerRelocated = relocateAngle * targetDirectionCenter;
    //    m_bulletRelocated = relocateAngle * directionOfBullet;
    //    m_upRelocated = m_centerRelocated + new Vector3(0, radius, 0);
    //    m_downRelocated = m_centerRelocated - new Vector3(0, radius, 0); 

    //    UnityEngine.Debug.DrawLine(Vector3.zero, m_previousRelocated, Color.red, 1);
    //    UnityEngine.Debug.DrawLine(Vector3.zero, m_currentRelocated, Color.red, 1);
    //    UnityEngine.Debug.DrawLine(Vector3.zero, m_bulletRelocated, Color.green , 1);
    //    UnityEngine.Debug.DrawLine(Vector3.zero, m_upRelocated, Color.red, 1);
    //    UnityEngine.Debug.DrawLine(Vector3.zero, m_downRelocated, Color.red, 1);

    //    m_angleVertical = Vector3.Angle(Vector3.forward, m_upRelocated);
    //    m_angleHorizontalLeft = Vector3.Angle(Vector3.forward, m_previousRelocated);
    //    m_angleHorizontalRight = Vector3.Angle(Vector3.forward, m_currentRelocated);

    //    m_bulletAngleVertical = Vector3.Angle(Vector3.forward, new Vector3(0, m_bulletRelocated.y, 0));
    //    if (m_bulletRelocated.x < 0)
    //    {
    //        m_bulletAngleHorizontalLeft = Vector3.Angle(Vector3.forward, new Vector3(m_previousRelocated.x,0, 0)); 
    //        m_bulletAngleHorizontalRight = 0 ;
    //    }
    //    else
    //    {
    //        m_bulletAngleHorizontalLeft = 0 ;
    //        m_bulletAngleHorizontalRight = Vector3.Angle(Vector3.forward, new Vector3( m_currentRelocated.x,0, 0)); 

    //    }


    //    Vector3 bulletDir2D = m_bulletRelocated;
    //    bulletDir2D.y = 0;
    //    Vector3 shortesEdge = m_previousRelocated.magnitude < m_currentRelocated.magnitude ? m_previousRelocated : m_currentRelocated;
    //    shortesEdge.y = 0;
    //    float opposedEdge = shortesEdge.magnitude;

    //    if (bulletDir2D.magnitude > shortesEdge.magnitude)
    //    {
    //        float hitCenterPointDistance;
    //        Vector3 approximateHitPoint;
    //        // if (m_bulletRelocated.magnitude < hypothenus)
    //        {

    //            hitCenterPointDistance = opposedEdge / Mathf.Cos(Vector3.Angle(shortesEdge, bulletDir2D));
    //            approximateHitPoint = (m_bulletRelocated.normalized * hitCenterPointDistance);
    //        }
    //        //else 
    //        {


    //        }

    //        //float hitCenterPointDistance = hypothenus * Mathf.Cos( Vector3.Angle(farestEdge, m_bulletRelocated) );
    //        //Vector3 approximateHitPoint = ( m_bulletRelocated.normalized * hitCenterPointDistance );


    //        UnityEngine.Debug.DrawLine(Vector3.zero, approximateHitPoint, Color.white, 1);
    //    }

    //}






}
