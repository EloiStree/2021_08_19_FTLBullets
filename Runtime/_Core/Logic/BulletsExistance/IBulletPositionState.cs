using UnityEngine;

public interface IBulletPositionState {

    void IsInUse(out bool isUse);
    void GetDirection(out Vector3 direction);
    void GetCurrentPosition(out Vector3 position);
    void GetPreviousPosition(out Vector3 position);
    void GetCurrentLifetime(out float time);
    void GetTimeLeft(out float time);

}


