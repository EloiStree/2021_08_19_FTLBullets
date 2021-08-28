
using Unity.Collections;
using UnityEngine;


public class UpdateTrackLineColliderMono : MonoBehaviour, ICapsuleLine
{


    public Transform m_linkedTransform;
    public float m_radius;

    [Header("Debug")]
    public NCLR_LineRef m_lineRef;


    private void Awake()
    {
        SceneCapsuleLineRegister.m_bulletClaimManager.Claim(out int id);
        SceneCapsuleLineRegister.m_bulletInScene.GetRegisterLink(id, out m_lineRef);
        m_lineRef.SetRadius(ref m_radius);

    }
    public Vector3 m_previousPosition;
    public Vector3 m_currentPosition;

    private void Update()
    {
        m_previousPosition = m_currentPosition;
        m_currentPosition = m_linkedTransform.position;
        m_lineRef.SetLineWith( m_previousPosition, m_currentPosition);
    }

    public void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition)
    {
        startPointPosition = m_previousPosition;
        endPointPosition = m_currentPosition;
    }

    public void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius)
    {
        startPointPosition = m_previousPosition;
        endPointPosition = m_currentPosition;
        radius = m_radius;
    }

    public void GetLineRadius(out float radius)
    {
        radius= m_radius ;
    }

    private void Reset()
    {
        m_linkedTransform = this.transform;
    }

}



//public class UpdateToNativeTrackLineColliderMono : MonoBehaviour, ICapsuleLine
//{


//    public Transform m_linkedTransform;
//    public float m_radius;

//    [Header("Debug")]
//    public Vector3CapsuleLine m_position;


//    private void Awake()
//    {
//        m_position.m_radius = m_radius;

//    }
//    public Vector3 m_previousPosition;
//    public Vector3 m_currentPosition;

//    private void Update()
//    {
//        m_previousPosition = m_currentPosition;
//        m_currentPosition = m_linkedTransform.position;
//        m_position.m_start = m_previousPosition;
//        m_position.m_end = m_currentPosition;
//    }

//    public void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition)
//    {
//        m_position.GetLineAsPoints(out startPointPosition, out endPointPosition);
//    }

//    public void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius)
//    {
//        m_position.GetLineAsPointsWithRadius(out startPointPosition, out endPointPosition, out radius);
//    }

//    public void GetLineRadius(out float radius)
//    {
//        m_position.GetLineRadius(out radius);
//    }

//    private void Reset()
//    {
//        m_linkedTransform = this.transform;
//    }

//}
