using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment_ShortestDistanceBetweenTwoVector3 : MonoBehaviour
{

    public Transform m_startPointA;
    public Transform m_endPointA;
    public float m_radiusA=0.1f;

    public Transform m_startPointB;
    public Transform m_endPointB;
    public float m_radiusB = 0.1f;

    public float m_exageration=0;
    public bool m_useDebug;


    void Update()
    {
        Vector3 shortestStartLineA, shortestEndLineB;
        DrawShortestLine(
            out shortestStartLineA,
            out shortestEndLineB,
            m_startPointA.position,
            m_endPointA.position,
            m_startPointB.position,
            m_endPointB.position, m_exageration,
            m_useDebug);
        Vector3 direction = (shortestEndLineB-shortestStartLineA );
     
        Debug.DrawLine(shortestStartLineA, shortestEndLineB, direction.magnitude > (m_radiusA + m_radiusB)?Color.red: Color.green);
    }

    private void DrawShortestLine(
        out Vector3 shortestStartLineA,
        out Vector3 shortestEndLineB,
        Vector3 startPointA,
        Vector3 endPointA
        , Vector3 startPointB,
        Vector3 endPointB, float exageration
        , bool useDebugDraw)
    {

        //COmputing the axes of an imaginary cartesian plane.
        Vector3 targetDirectionCenter = ((startPointB + endPointB) / 2f)- startPointA;
        Vector3 up = Vector3.Cross(startPointB- startPointA, endPointB- startPointA);


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
        DrawShortestLineFromRelocatedBase(
            out shortestStartLineA, out shortestEndLineB,
            relocateAngle * (startPointB - startPointA),
            relocateAngle * (endPointB  - startPointA), 
            relocateAngle * (endPointA  - startPointA),
            relocateAngle,
            startPointA, exageration, useDebugDraw);
    }

    private void DrawShortestLineFromRelocatedBase(out Vector3 shortestStartLineA,
        out Vector3 shortestEndLineB,
        Vector3 startPointB,
        Vector3 endPointB,
        Vector3 trackedPoint,
        Quaternion relocateAngleUsed,
        Vector3 originePoint, 
        float lineExageration=5f, bool useDebugDraw=false)
    {
        if (useDebugDraw) { 
        
            Debug.DrawLine(Vector3.zero, Vector3.up*2, Color.green);
            Debug.DrawLine(Vector3.zero, Vector3.left * 2, Color.red);
            Debug.DrawLine(Vector3.zero, Vector3.forward * 2, Color.blue);
        }



        // Exagerate the lenght of compare to lines instead of two segements
        if (lineExageration > 0) { 
             startPointB = startPointB + (startPointB- endPointB)* lineExageration;
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
        Quaternion angleRotation =  Quaternion.Euler(trackedPoint.y > 0 ? (- (90f-alpha)): (-(-90f + alpha)), 0,0);
        Vector3 shortestVectorDirection = (lookAtOrigine*angleRotation) * Vector3.forward;
        Vector3 shortestVectorOrigine = upStart;

        // Now that I know where is the perpendiculare line of the lina A from the line B, I try to find the distance between them with Trigono
        float shortestLenght = Mathf.Sin(alpha * Mathf.Deg2Rad) * upStart.magnitude;
        Vector3 shortestVectorEnd = shortestVectorOrigine+shortestVectorDirection.normalized*shortestLenght;

        // Now that we know on our plan what is the the shortest line between A and B, we can relocate it to the line world position
        Quaternion i = Quaternion.Inverse(relocateAngleUsed);
        shortestStartLineA = i * shortestVectorEnd + originePoint;
        shortestEndLineB = i * shortestVectorOrigine + originePoint; 

        if (useDebugDraw)
        {

            Debug.DrawLine(shortestVectorOrigine, shortestVectorEnd, Color.yellow);
            Debug.DrawLine(shortestStartLineA, shortestEndLineB, Color.yellow);
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


    public static bool LineIntersectionXZ(Vector3 sA, Vector3 eA, Vector3 sB, Vector3 eB, out  Vector3 intersection)
    {
        sA.y = 0;
        eA.y = 0;
        sB.y = 0;
        eB.y = 0;
        bool hit = LineLineIntersection(out intersection, sA, eA-sA, sB, eB-sB);
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
    
    public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }
    #endregion
}
