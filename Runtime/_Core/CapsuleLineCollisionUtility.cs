using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CapsuleLineCollisionUtility
{


    public static void CheckReachabilityOfTwoCapsules(
      Transform startPointA, Transform endPointA, float lineRadiusA,
  Transform startPointB, Transform endPointB, float lineRadiusB,
      out ReachFiltering reachableState, bool useDebugDraw)
    {

        CheckReachabilityOfTwoCapsules(
            startPointA.position, endPointA.position, lineRadiusA,
    startPointB.position, endPointB.position, lineRadiusB, out reachableState, useDebugDraw);
    }

    public static void CheckIfNearEnough(
   ref Vector3 startPointA, ref Vector3 endPointA, ref float lineRadiusA,
ref Vector3 startPointB, ref Vector3 endPointB, ref float lineRadiusB,
   out bool isInRange)
    {
        isInRange = false;


        float radiusAandB = lineRadiusA + lineRadiusB;

        float distanceAWithRadius = (endPointA - startPointA).magnitude+radiusAandB;
        float distanceStart = (startPointB - startPointA).magnitude;
        float distanceEnd = (endPointB - startPointA).magnitude;
        
        float minDistanceRange = distanceStart<distanceEnd?distanceStart:distanceEnd;
       // float maxDistanceRange = distanceStart > distanceEnd ? distanceStart : distanceEnd;

        if (distanceAWithRadius < minDistanceRange)
            isInRange= false;
        else
            isInRange= true;

    }


    public static  void CheckReachabilityOfTwoCapsules(
    Vector3 startPointA, Vector3 endPointA, float lineRadiusA,
Vector3 startPointB, Vector3 endPointB, float lineRadiusB,
    out ReachFiltering reachableState, bool useDebugDraw)
    {
        reachableState = ReachFiltering.MaybeReachable;


        float radiusAandB = lineRadiusA + lineRadiusB;
        float distanceStart = (startPointB - startPointA).magnitude;
        float distanceEnd = (endPointB - startPointA).magnitude;
        float lineMaxRange = (endPointA - startPointA).magnitude + radiusAandB;
        if (lineMaxRange <= distanceStart || lineMaxRange <= distanceEnd)
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


        if (useDebugDraw)
        {
            Debug.DrawLine(startExtremB + Vector3.up * 0.1f, endExtremB + Vector3.up * 0.1f, Color.yellow);
            Debug.DrawLine(shortestVector, upExtreamB, Color.yellow);
            Debug.DrawLine(shortestVector, shortestVector - Vector3.up * radiusAandB, Color.yellow);
            Debug.DrawLine(startPointA, endPointA + (endPointA - startPointA), Color.white);
        }


        float verticalAngleMax = Vector2.Angle(new Vector3(upExtreamB.z, upExtreamB.y), Vector2.right);
        float pointAngleVertical = Vector2.Angle(new Vector3(endPointA.z, endPointA.y), Vector2.right);

        if (pointAngleVertical > verticalAngleMax)
        {
            reachableState = ReachFiltering.LineAOutsideVerticalAngle;
            return;
        }

        float extreStartBAngle = Vector2.Angle(new Vector3(startExtremB.x, startExtremB.z), Vector2.up);
        float extreEndBAngle = Vector2.Angle(new Vector3(endExtremB.x, endExtremB.z), Vector2.up);
        float bigestAngleHorizontal = extreStartBAngle > extreEndBAngle ? extreStartBAngle : extreEndBAngle;

        float pointAngleHorizontal = Vector2.Angle(new Vector3(endPointA.x, endPointA.z), Vector2.up);

        if (pointAngleHorizontal > bigestAngleHorizontal)
        {
            reachableState = ReachFiltering.LineAOutsideHorizontalAngle;
            return;
        }



        //if (debugInfo != null)
        //{
        //    debugInfo.m_angleHorizontalLeft = extreStartBAngle;
        //    debugInfo.m_angleHorizontalRight = extreEndBAngle;
        //    debugInfo.m_angleVertical = verticalAngleMax;
        //    debugInfo.m_bulletAngleHorizontal = pointAngleHorizontal;
        //    debugInfo.m_bulletAngleVertical = pointAngleVertical;
        //}
    }
    [System.Serializable]
    public class CapsuleReachCheckDebugInfo
    {
        public float m_angleHorizontalLeft;
        public float m_angleHorizontalRight;
        public float m_bulletAngleHorizontal;
        public float m_angleVertical;
        public float m_bulletAngleVertical;
    }
    public enum ReachFiltering { LineANotInDistanceRange, LineAOutsideVerticalAngle, LineAOutsideHorizontalAngle, MaybeReachable }



    public struct TwoLinesCollisionInfo
    {
        Vector3 m_lineAShortestPathStart;
        Vector3 m_lineBShortestPathEnd;
        float m_distanceOfPath;
        bool m_areColliding;

    }

    public static bool AreLinesSectionCouldCollide(ICapsuleLine lineA, ICapsuleLine lineB)
    {

        lineA.GetLineAsPointsWithRadius(out Vector3 spA, out Vector3 epA, out float rA);
        lineB.GetLineAsPointsWithRadius(out Vector3 spB, out Vector3 epB, out float rB);
        return AreLinesSectionCouldCollide(spA, epA, rA, spB, epB, rB);
        //
    }
 

    public static void DoesCapsulesCollide(ICapsuleLine lineA, ICapsuleLine lineB,
        ref TwoLinesCollisionInfo collisionInfo)
    {

        lineA.GetLineAsPointsWithRadius(out Vector3 spA, out Vector3 epA, out float rA);
        lineB.GetLineAsPointsWithRadius(out Vector3 spB, out Vector3 epB, out float rB);
        DoesCapsulesCollide(spA, epA, rA, spB, epB, rB, ref collisionInfo);



    }

    public static void DoesCapsulesCollide(
        Vector3 startPointA,
        Vector3 endPointA,
        float lineRadiusA,
         Vector3 startPointB,
        Vector3 endPointB,
        float lineRadiusB, ref TwoLinesCollisionInfo collisionInfo) {


            throw new System.NotImplementedException();



    }

    public static void AreColliding(ref Vector3 startPoint, ref float startPointRadius, ref Vector3 endPoint, ref float endPointRadius, out bool areColliding)
    {
        areColliding = (endPoint - startPoint).magnitude < (startPointRadius + endPointRadius);
    }


    public static bool AreLinesSectionCouldCollide(
      Vector3 startPointA, Vector3 endPointA, float lineRadiusA,
       Vector3 startPointB, Vector3 endPointB, float lineRadiusB)
    {
        bool isInReachPrevious = false;
        bool isInReachCurrent = false;
        bool isInGoodAngle = false;

        float radiusAandB = lineRadiusA + lineRadiusB;
        Vector3 lineADirection = endPointA - startPointA;
        Vector3 lineCenterDirection = (startPointA+endPointA) /2f - startPointA;
        float speed = lineADirection.magnitude+radiusAandB;

        Vector3 startExtremB = startPointB + (startPointB-endPointB).normalized*radiusAandB ;
        Vector3 endExtremB = endPointB + (endPointB - startPointB  ).normalized * radiusAandB; ;


        float angleLeft = Vector3.Angle(lineCenterDirection, startExtremB - startPointA);
        float angleRight = Vector3.Angle(lineCenterDirection, endExtremB - startPointA);
        float angleLineADirection = Vector3.Angle( lineCenterDirection, lineADirection);




        if (!isInReachPrevious && !isInReachCurrent)
            return false;


        if (!isInGoodAngle)
            return false;

        /////////////////COMPUTATION THAT NEED RELOCATION
        Vector3 targetDirectionCenter = ((startPointB + endPointB) / 2f) - startPointA;
        Vector3 up = Vector3.Cross(startPointB - startPointA, endPointB - startPointA);
        Quaternion relocateAngle = Quaternion.Inverse(Quaternion.LookRotation(targetDirectionCenter, up));
        Vector3 a = startPointA;
        startPointA = relocateAngle * (startPointA - a);
        endPointA = relocateAngle * (endPointA - a);
        startPointB = relocateAngle * (startPointB - a);
        endPointB = relocateAngle * (endPointB - a);

        Debug.DrawLine(startPointA, endPointA, Color.red);
        Debug.DrawLine(startPointB, endPointB, Color.red);


        return true;
    }



        public static void GetShortestLineBetweenTwoSections_FirstDraft(
        out Vector3 shortestStartLineA,
        out Vector3 shortestEndLineB,
        Vector3 startPointA,
        Vector3 endPointA
        , Vector3 startPointB,
        Vector3 endPointB, float exageration=1000f
        , bool useDebugDraw=false)

    {



        //COmputing the axes of an imaginary cartesian plane.
        Vector3 targetDirectionCenter = ((startPointB + endPointB) / 2f) - startPointA;
        Vector3 up = Vector3.Cross(startPointB - startPointA, endPointB - startPointA);


        if (useDebugDraw)
        {

            Debug.DrawLine(startPointA, startPointA + up.normalized * 4f, Color.green);
            Debug.DrawLine(startPointA, startPointA + targetDirectionCenter.normalized * 4f, Color.blue);
        }

        // Translate rotation to work on  cartesian plane (0,0,0)
        //
        Quaternion relocateAngle = Quaternion.Inverse(Quaternion.LookRotation(targetDirectionCenter, up));

        if (useDebugDraw)
        {

            Debug.DrawLine(startPointA, startPointB, Color.black);
            Debug.DrawLine(startPointA, endPointB, Color.black);
            Debug.DrawLine(startPointA, startPointB, Color.black);
        }
        GetShortestLineBetweenTwoSectionsLocalComputation_FirstDraft(
            out shortestStartLineA, out shortestEndLineB,
            relocateAngle * (startPointB - startPointA),
            relocateAngle * (endPointB - startPointA),
            relocateAngle * (endPointA - startPointA),
            relocateAngle,
            startPointA, exageration, useDebugDraw);
    }

    private static void GetShortestLineBetweenTwoSectionsLocalComputation_FirstDraft( out Vector3 shortestStartLineA,
        out Vector3 shortestEndLineB,
        Vector3 startPointB,
        Vector3 endPointB,
        Vector3 trackedPoint,
        Quaternion relocateAngleUsed,
        Vector3 originePoint,
        float lineExageration = 5f, bool useDebugDraw = false)
    {
        if (useDebugDraw)
        {

            Debug.DrawLine(Vector3.zero, Vector3.up * 2, Color.green);
            Debug.DrawLine(Vector3.zero, Vector3.left * 2, Color.red);
            Debug.DrawLine(Vector3.zero, Vector3.forward * 2, Color.blue);
        }



        // Exagerate the lenght of compare to lines instead of two segements
        if (lineExageration > 0)
        {
            startPointB = startPointB + (startPointB - endPointB) * lineExageration;
            endPointB = endPointB + (endPointB - startPointB) * lineExageration;
            trackedPoint = trackedPoint * lineExageration;
        }


        if (useDebugDraw)
        {

            Debug.DrawLine(Vector3.zero, startPointB, Color.red);
            Debug.DrawLine(Vector3.zero, endPointB, Color.red);
            Debug.DrawLine(startPointB, endPointB, Color.white);
            Debug.DrawLine(Vector3.zero, trackedPoint, Color.blue);
        }



        //Try to find the up axis on the second line based on the relocated plan
        // I do that by checking the cross of two 2D vectors
        Vector3 lineBDirection = endPointB - startPointB;
        Vector3 trackPointXZ = new Vector3(trackedPoint.x, 0, trackedPoint.z);

        Vector3 upStart;
        LineIntersectionXZ(Vector3.zero, trackPointXZ, startPointB, endPointB, out upStart);

        // Now that we have the up vector of the line we can try to find the perpendicular  line of the rectangle  with the line A
        //So I compute the angle between the projection on the plan and the line A end point
        float alpha = Vector3.Angle(upStart, trackedPoint);
        //float beta = 180f - 90f - alpha;
        //float gamma = 180f - 90f - beta;

        // Now that I have the alpha that can help build the perpendicular line of the line A, I try to find the direction of this perpendicular line in Unity 3D local space
        Quaternion lookAtOrigine = Quaternion.LookRotation(-trackPointXZ, Vector3.up);
        Quaternion angleRotation = Quaternion.Euler(trackedPoint.y > 0 ? (-(90f - alpha)) : (-(-90f + alpha)), 0, 0);
        Vector3 shortestVectorDirection = (lookAtOrigine * angleRotation) * Vector3.forward;
        Vector3 shortestVectorOrigine = upStart;

        // Now that I know where is the perpendiculare line of the lina A from the line B, I try to find the distance between them with Trigono
        float shortestLenght = Mathf.Sin(alpha * Mathf.Deg2Rad) * upStart.magnitude;
        Vector3 shortestVectorEnd = shortestVectorOrigine + shortestVectorDirection.normalized * shortestLenght;

        // Now that we know on our plan what is the the shortest line between A and B, we can relocate it to the line world position
        Quaternion i = Quaternion.Inverse(relocateAngleUsed);
        shortestStartLineA = i * shortestVectorEnd + originePoint;
        shortestEndLineB = i * shortestVectorOrigine + originePoint;

        if (useDebugDraw)
        {

            Debug.DrawLine(shortestVectorOrigine, shortestVectorEnd, Color.yellow+Color.red);
            Debug.DrawLine(shortestStartLineA, shortestEndLineB, Color.yellow + Color.red);
        }





    }



    public static bool LineIntersectionXY(Vector3 sA, Vector3 eA, Vector3 sB, Vector3 eB, out Vector3 intersection)
    {
        sA.z = 0;
        eA.z = 0;
        sB.z = 0;
        eB.z = 0;
        bool hit = LineLineIntersection(out intersection, sA, eA - sA, sB, eB - sB);
        return hit;
    }


    public static bool LineIntersectionXZ(Vector3 sA, Vector3 eA, Vector3 sB, Vector3 eB, out Vector3 intersection)
    {
        sA.y = 0;
        eA.y = 0;
        sB.y = 0;
        eB.y = 0;
        bool hit = LineLineIntersection(out intersection, sA, eA - sA, sB, eB - sB);
        return hit;
    }


    //Not my code, lost the source
    #region Not My code lost the source but work
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        intersection = Vector3.zero;

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //Lines are not coplanar. Take into account rounding errors.
        if ((planarFactor >= 0.00001f) || (planarFactor <= -0.00001f))
        {

            return false;
        }

        //Note: sqrMagnitude does x*x+y*y+z*z on the input vector.
        float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;

        if ((s >= 0.0f) && (s <= 1.0f))
        {

            intersection = linePoint1 + (lineVec1 * s);
            //This is giving you the point at which the lines intersect. It is already returning via the out Vector3 intersection parameter in the method itself.
            return true;
        }

        else
        {
            return false;
        }
    }

    #endregion


    public static void GetShortestLineBetweenTwoSections(
     out Vector3 shortestStartLineAWorldSpace,
     out Vector3 shortestEndLineBWorldSpace,
     Vector3 startPointAWorldSpace,
     Vector3 endPointAWorldSpace
     , Vector3 startPointBWorldSpace,
     Vector3 endPointBWorldSpace
     , bool useDebugDraw = false)
    {
        defaultCollisionCapsule.GetShortestLineBetweenTwoSections(
            out shortestStartLineAWorldSpace,
            out shortestEndLineBWorldSpace,
            startPointAWorldSpace,
            endPointAWorldSpace,
            startPointBWorldSpace,
            endPointBWorldSpace,
            useDebugDraw);
    }
    public readonly static CapsuleLineCollisionComputation defaultCollisionCapsule = new CapsuleLineCollisionComputation();
}


public interface ICapsuleLineCollisionComputation {

    void GetShortestLineBetweenTwoSections(
     out Vector3 shortestStartLineAWorldSpace,
     out Vector3 shortestEndLineBWorldSpace,
     Vector3 startPointAWorldSpace,
     Vector3 endPointAWorldSpace
     , Vector3 startPointBWorldSpace,
     Vector3 endPointBWorldSpace
     , bool useDebugDraw = false);
    
}

public struct CapsuleLineCollisionComputation : ICapsuleLineCollisionComputation
{

    public  void GetShortestLineBetweenTwoSections(
       out Vector3 shortestStartLineA,
       out Vector3 shortestEndLineB,
       Vector3 startPointA,
       Vector3 endPointA
       , Vector3 startPointB,
       Vector3 endPointB
       , bool useDebugDraw = false)
    {
        //TODO: I should detectif the user gave me a point instead of a line and change code base on that.

        shortestStartLineA = Vector3.zero;
        shortestEndLineB = Vector3.zero;
        Vector3 forward = (endPointB - startPointB);
        Vector3 up = Vector3.Cross(startPointA - startPointB, endPointB - startPointB);
        Vector3 right = Vector3.Cross(forward, up);

        if (useDebugDraw)
        {
            Debug.DrawLine(startPointB, startPointB + forward, Color.blue);
            Debug.DrawLine(startPointB, startPointB + up, Color.green);
            Debug.DrawLine(startPointB, startPointB + right, Color.red);
        }


        Quaternion relocateAngle = Quaternion.identity;
        if (forward != Vector3.zero &&  up != Vector3.zero) {
            relocateAngle = Quaternion.Inverse(Quaternion.LookRotation(forward, up));
        }

        GetShortestLineBetweenTwoSectionsRelocated(
             out Vector3 shortestStartLineALocal,
             out Vector3 shortestEndLineBLocal,
            relocateAngle * (endPointB - startPointB),
            relocateAngle * (startPointA - startPointB),
            relocateAngle * (endPointA - startPointB),
            useDebugDraw

            );
        shortestStartLineA = Quaternion.Inverse(relocateAngle) * shortestStartLineALocal + startPointB;
        shortestEndLineB = Quaternion.Inverse(relocateAngle) * shortestEndLineBLocal + startPointB;
        if (useDebugDraw)
        {
            Debug.DrawLine(shortestStartLineA, shortestEndLineB, Color.yellow);
        }
    }

    enum LineAPart { Start, End }
    enum LineAHorizontal { FullLeft, FullRight, BothSide }
    /// not perfect but largely good enough for the momement. contact me if you want to complain and ask me to move my ass to correct it.
    public void GetShortestLineBetweenTwoSectionsRelocated(
      out Vector3 shortestStartLineALocal,
      out Vector3 shortestEndLineBLocal,
      Vector3 forward,
      Vector3 startPointA,
      Vector3 endPointA,
      bool useDebugDraw = false)
    {
        shortestStartLineALocal = Vector3.zero;
        shortestEndLineBLocal = Vector3.zero;

        if (useDebugDraw)
        {
            Debug.DrawLine(Vector3.zero, forward, Color.white);
            Debug.DrawLine(startPointA, endPointA, Color.white);
        }

        float axeDistanceOfStart = new Vector3(startPointA.x, startPointA.y, 0).magnitude;
        float axeDistanceOfEnd = new Vector3(endPointA.x, endPointA.y, 0).magnitude;
        LineAPart nearestOfZAxis = axeDistanceOfStart < axeDistanceOfEnd ? LineAPart.Start : LineAPart.End;
        LineAHorizontal linaAPosition = LineAHorizontal.BothSide;
        if (startPointA.x < 0 && endPointA.x < 0)
            linaAPosition = LineAHorizontal.FullLeft;
        if (startPointA.x >= 0 && endPointA.x >= 0)
            linaAPosition = LineAHorizontal.FullRight;

        // If line is left or right
        if (linaAPosition != LineAHorizontal.BothSide)
        {

            shortestStartLineALocal = nearestOfZAxis == LineAPart.Start ? startPointA : endPointA;
            // if down  the line B
            if (shortestStartLineALocal.z < 0)
                shortestEndLineBLocal = Vector3.zero;
            // if up the line B
            else if (shortestStartLineALocal.z > forward.z)
                shortestEndLineBLocal = forward;
            // if between start of line B and end of line B
            else
                shortestEndLineBLocal = new Vector3(0, 0, shortestStartLineALocal.z);
        }
        else if (linaAPosition == LineAHorizontal.BothSide)
        {

            Vector3 nearestPoint = nearestOfZAxis == LineAPart.Start ? startPointA : endPointA;
            Vector3 farestPoint = nearestOfZAxis == LineAPart.Start ? endPointA : startPointA;

            // if down  the line B
            if (nearestPoint.z < 0)
            {
                shortestStartLineALocal = nearestPoint;
                // I AM SUPPOSING THIS IS TRUE BUT IF THERE IS BUG IT IS HERE
                shortestEndLineBLocal = Vector3.zero;
            }
            // if up the line B
            else if (nearestPoint.z > forward.z)
            {
                shortestStartLineALocal = nearestPoint;
                // I AM SUPPOSING THIS IS TRUE BUT IF THERE IS BUG IT IS HERE
                shortestEndLineBLocal = forward;
            }
            // if between start of line B and end of line B
            else
            {

                CapsuleLineCollisionUtility.LineIntersectionXZ(
                    nearestPoint, farestPoint, Vector3.zero, Vector3.forward * 999999f,
                    out Vector3 planeIntersectionXZ);

                CapsuleLineCollisionUtility.LineIntersectionXY(
                    nearestPoint, farestPoint, Vector3.zero, Vector3.up * 999999f,
                    out Vector3 planeIntersectionXY);


                Vector3 pointHeightOnLine = new Vector3(0, planeIntersectionXY.y, planeIntersectionXZ.z);

                if (useDebugDraw)
                {
                    Debug.DrawLine(planeIntersectionXY + Vector3.one * 0.1f, planeIntersectionXZ + Vector3.one * 0.1f, Color.cyan);
                    Debug.DrawLine(planeIntersectionXY + Vector3.one * 0.1f, pointHeightOnLine + Vector3.one * 0.1f, Color.cyan);
                    Debug.DrawLine(pointHeightOnLine + Vector3.one * 0.1f, planeIntersectionXZ + Vector3.one * 0.1f, Color.cyan);
                    Debug.DrawLine(nearestPoint + Vector3.one * 0.1f, planeIntersectionXZ + Vector3.one * 0.1f, Color.cyan);

                }

                //TEST 
                shortestStartLineALocal = planeIntersectionXZ;
                shortestEndLineBLocal = pointHeightOnLine;

                //Debug.Log(string.Format("Hello NF {0} - {1}", nearestPoint, farestPoint));
                //Debug.Log(string.Format("Hello Trio {0} - {1} -{2}", planeIntersectionXY, planeIntersectionXZ, pointHeightOnLine));


                Vector3 leftPoint = startPointA.x < endPointA.x ? startPointA : endPointA;
                Vector3 rightPoint = startPointA.x >= endPointA.x ? startPointA : endPointA;

                float rightAngle = Vector3.Angle(planeIntersectionXZ - pointHeightOnLine, rightPoint - pointHeightOnLine);
                bool isRightPointOptuseAngle = rightAngle > 90f;
                Vector3 pointForShorest = isRightPointOptuseAngle ? leftPoint : rightPoint;
                float alpha = Vector3.Angle(planeIntersectionXZ - pointForShorest, pointHeightOnLine - pointForShorest);

                float distance = Mathf.Cos(alpha * Mathf.Deg2Rad) * (planeIntersectionXZ - pointForShorest).magnitude;
                Vector3 pt = pointForShorest + (pointHeightOnLine - pointForShorest).normalized * distance;

                if (useDebugDraw)
                    Debug.DrawLine(planeIntersectionXZ, pt, Color.blue + Color.red);
                //Debug.Log(string.Format("Angle  {0} - {1}- {2}", rightAngle, alpha, distance));
                Vector3 clampedShortestB = planeIntersectionXZ;
                if (clampedShortestB.z > forward.z)
                    clampedShortestB.z = forward.z;
                if (clampedShortestB.z < 0)
                    clampedShortestB.z = 0;

                shortestEndLineBLocal = clampedShortestB;
                shortestStartLineALocal = pt;



            }
        }

        if (useDebugDraw)
            Debug.DrawLine(shortestStartLineALocal, shortestEndLineBLocal, Color.yellow);
    }

}