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
        CapsuleLineCollisionUtility.
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
 
    
}
