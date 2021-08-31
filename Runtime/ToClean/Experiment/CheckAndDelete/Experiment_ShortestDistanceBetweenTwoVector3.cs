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


    public Transform m_shortestLine;


    [Header("Debug Mesh")]
    public Transform m_lineAStart;
    public Transform m_lineAMiddle;
    public Transform m_lineAMiddleRadius;
    public Transform m_lineAEnd;

    public Transform m_lineBStart;
    public Transform m_lineBMiddle;
    public Transform m_lineBMiddleRadius;
    public Transform m_lineBEnd;

    

    void Update()
    {
        Vector3 shortestStartLineA, shortestEndLineB;
        GetShortestLineBetweenTwoSections (
            out shortestStartLineA,
            out shortestEndLineB,
            m_startPointA.position,
            m_endPointA.position,
            m_startPointB.position,
            m_endPointB.position,
            m_useDebug);
        Vector3 forward = (shortestEndLineB-shortestStartLineA );
     
        Debug.DrawLine(shortestStartLineA, shortestEndLineB, forward.magnitude > (m_radiusA + m_radiusB)?Color.red: Color.green);
        if (m_shortestLine != null && forward!=Vector3.zero)
        {
            m_shortestLine.position = (shortestStartLineA + shortestEndLineB) / 2f;
            m_shortestLine.forward = (forward).normalized;
            m_shortestLine.localScale = new Vector3(0.1f, 0.1f, forward.magnitude);
        }
        else {

            m_shortestLine.position = Vector3.zero;
        m_shortestLine.forward = Vector3.up;
            m_shortestLine.localScale = Vector3.zero;
        }


        Debug.DrawLine(m_startPointA.position, m_endPointA.position, Color.white);
        Debug.DrawLine(m_startPointB.position, m_endPointB.position, Color.white);

        Debug.DrawLine(m_startPointA.position, m_startPointB.position, Color.grey);
        Debug.DrawLine(m_startPointA.position, m_endPointB.position, Color.grey);

        Debug.DrawLine(m_endPointA.position, m_startPointB.position, Color.grey);
        Debug.DrawLine(m_endPointA.position, m_endPointB.position, Color.grey);



    }

  

    private void LateUpdate()
    {
        Vector3 forward;

        forward = (m_endPointA.position - m_startPointA.position);
        m_lineAStart.position = m_startPointA.position;
        m_lineAEnd.position = m_endPointA.position;
        m_lineAStart.localScale = Vector3.one * m_radiusA;
        m_lineAEnd.localScale = Vector3.one * m_radiusA;

        m_lineAMiddle.position = (m_startPointA.position + m_endPointA.position) / 2f;
        m_lineAMiddle.up = (forward).normalized;
        m_lineAMiddle.localScale = new Vector3(m_radiusA, forward.magnitude , m_radiusA);
        m_lineAMiddleRadius.position = m_lineAMiddle.position;
        m_lineAMiddleRadius.up = (forward).normalized;
        m_lineAMiddleRadius.localScale = new Vector3(m_radiusA, forward.magnitude , m_radiusA );


        forward = (m_endPointB.position - m_startPointB.position);
        m_lineBStart.position = m_endPointB.position;
        m_lineBEnd.position = m_startPointB.position;
        m_lineBStart.localScale = Vector3.one * m_radiusB;
        m_lineBEnd.localScale = Vector3.one * m_radiusB;

        m_lineBMiddle.position = (m_lineBStart.position + m_lineBEnd.position) / 2f;
        m_lineBMiddle.up = (forward).normalized;
        m_lineBMiddle.localScale = new Vector3(m_radiusB, forward.magnitude , m_radiusB);
        m_lineBMiddleRadius.position = m_lineBMiddle.position;
        m_lineBMiddleRadius.up = (forward).normalized;
        m_lineBMiddleRadius.localScale = new Vector3(m_radiusB, forward.magnitude, m_radiusB);


    }
    private void GetShortestLineBetweenTwoSections(
        out Vector3 shortestStartLineA,
        out Vector3 shortestEndLineB,
        Vector3 startPointA,
        Vector3 endPointA
        , Vector3 startPointB,
        Vector3 endPointB
        , bool useDebugDraw = false)
    {
        shortestStartLineA = Vector3.zero;
        shortestEndLineB = Vector3.zero;
        Vector3 forward = (endPointB-startPointB) ;
        Vector3 up = Vector3.Cross(startPointA - startPointB, endPointB - startPointB);
        Vector3 right = Vector3.Cross(forward, up);

        if (useDebugDraw)
        {
            Debug.DrawLine(startPointB, startPointB + forward, Color.blue);
            Debug.DrawLine(startPointB, startPointB + up, Color.green);
            Debug.DrawLine(startPointB, startPointB + right, Color.red);
        }

        Quaternion relocateAngle = Quaternion.Inverse(Quaternion.LookRotation(forward, up));

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
            Debug.DrawLine(shortestStartLineA, shortestEndLineB , Color.yellow);
        }
    }

    enum LineAPart { Start,End }
    enum LineAHorizontal { FullLeft, FullRight, BothSide}
    /// not perfect but largely good enough for the momement. contact me if you want to complain and ask me to move my ass to correct it.
    private void GetShortestLineBetweenTwoSectionsRelocated(
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
            Debug.DrawLine(Vector3.zero, forward , Color.white);
            Debug.DrawLine(startPointA, endPointA , Color.white);
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

                if (useDebugDraw) { 
                Debug.DrawLine(planeIntersectionXY+Vector3.one*0.1f, planeIntersectionXZ + Vector3.one * 0.1f, Color.cyan);
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

                float distance = Mathf.Cos(alpha*Mathf.Deg2Rad) * (planeIntersectionXZ - pointForShorest).magnitude;
                Vector3 pt = pointForShorest + (pointHeightOnLine - pointForShorest).normalized * distance;

                if(useDebugDraw)
                Debug.DrawLine(planeIntersectionXZ, pt, Color.blue + Color.red);
                //Debug.Log(string.Format("Angle  {0} - {1}- {2}", rightAngle, alpha, distance));

                shortestEndLineBLocal = planeIntersectionXZ;
                shortestStartLineALocal = pt;


            }
        }

        Debug.DrawLine(shortestStartLineALocal, shortestEndLineBLocal, Color.yellow);
    }



    
}
