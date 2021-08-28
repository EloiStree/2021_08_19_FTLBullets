using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDrawingMono : MonoBehaviour
{

    public AbstractBulletTagMono m_bulletTag;


    public Color m_bulletPath = Color.green;
    public Color m_bulletNext = Color.yellow;
    public Color m_bulletWillBe= Color.red;
    public float m_willBeDistance=20f;
    void Update()
    {
        Vector3 p, c;
        m_bulletTag.GetLineAsPoints(out p, out c);
        Vector3 direction = c-p ;
        Debug.DrawLine(c, p + direction * m_willBeDistance, m_bulletWillBe);
        Debug.DrawLine(p,p + direction + Vector3.up*0.01f, m_bulletNext);
        Debug.DrawLine(p, c + Vector3.up * 0.02f, m_bulletPath);
    }
}
