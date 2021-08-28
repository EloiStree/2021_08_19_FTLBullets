using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTrackBulletableTagMono : AbstractBulletableTagMono
{
    public UpdateTrackLineColliderMono m_positionTracked;
    protected void Start()
    {
        BulletableInScene.AddBulletInScene(gameObject, m_positionTracked);
    }
    protected void OnDestroy()
    {
        BulletableInScene.RemoveBulletInScene(gameObject);
    }

    public override void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition)
    {
        m_positionTracked.GetLineAsPoints(out startPointPosition, out endPointPosition);
    }

    public override void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius)
    {
        m_positionTracked.GetLineAsPointsWithRadius(out startPointPosition, out endPointPosition, out radius);
    }

    public override void GetLineRadius(out float radius)
    {
        m_positionTracked.GetLineRadius(out radius);
    }
}
