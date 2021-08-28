
using UnityEngine;
public interface ICapsuleLine
{

    void GetLineAsPointsWithRadius(out Vector3 startPointPosition, out Vector3 endPointPosition, out float radius);
    void GetLineAsPoints(out Vector3 startPointPosition, out Vector3 endPointPosition);
    void GetLineRadius(out float radius);
}