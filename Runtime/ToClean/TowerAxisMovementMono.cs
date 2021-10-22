using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAxisMovementMono : MonoBehaviour
{


    public Transform m_transformToRoot;
    public Transform m_transformToAffect;
    public TowerDirectAxisInfo m_axisInfo;
    public TowerAxisState m_axisState;
    public TowerWorldPosition m_currentPositionState;


    public void Update()
    {
        m_currentPositionState.m_position = m_transformToRoot.position;
        m_currentPositionState.m_rotation = m_transformToRoot.rotation;

        if (m_axisState.IsRotateRequested())
        {
            Rotate(ref m_axisInfo, ref m_axisState);
            ApplyRotationOnTransform(ref m_axisInfo, ref m_axisState);
        }
    }

    private void ApplyRotationOnTransform(ref TowerDirectAxisInfo axisInfo, ref TowerAxisState axisState)
    {
        m_transformToAffect.rotation = m_transformToRoot.rotation *
            (axisInfo.m_axis==TowerAxisType.Vertical ? Quaternion.Euler(-axisState.m_axisAngle, 0, 0): Quaternion.Euler(0, axisState.m_axisAngle, 0)); 
    }

    public float m_humm;
    private void Rotate(ref TowerDirectAxisInfo axisInfo, ref TowerAxisState axisState)
    {
   
        if (axisState.m_axisRequested != axisState.m_axisAngle) {

            float direction= axisState.m_axisRequested - axisState.m_axisAngle;
            float angle = axisInfo.m_maxSpeedRotationAngle * Math.Sign(direction);
            //if (angle > axisInfo.m_maxSpeedRotationAngle)
            //    angle = axisInfo.m_maxSpeedRotationAngle;
            //if (angle < -axisInfo.m_maxSpeedRotationAngle)
            //    angle = -axisInfo.m_maxSpeedRotationAngle;
            m_humm = angle;

            axisState.m_axisAngle += angle*Time.deltaTime;

            float afterdirection = axisState.m_axisRequested - axisState.m_axisAngle;

            if (axisState.m_axisAngle < -axisInfo.m_maxRotationAngle)
                axisState.m_axisAngle = -axisInfo.m_maxRotationAngle;
            else if (axisState.m_axisAngle > axisInfo.m_maxRotationAngle)
                axisState.m_axisAngle = axisInfo.m_maxRotationAngle;
            else if (direction * afterdirection < 0f)
                axisState.m_axisAngle = axisState.m_axisRequested;// * Math.Sign(direction);
        }

    }


    public void SetRequestRotationFrom(Vector3 lookAt)
    {
        Vector3 location =  Quaternion.Inverse(m_currentPositionState.m_rotation) * (lookAt - m_currentPositionState.m_position);
        relocatedTest = location;
        float angle = 0f;
        if (m_axisInfo.m_axis == TowerAxisType.Horizontal)
        {
            ComputeHorizontalAngle(location, out angle);
            m_h = angle;
        }
        else
        { 
            ComputeVerticalAngle(location, out angle);
            m_v = angle;
        }
        m_axisState.m_axisRequested = angle;
    }
    public void SetRequestRotationFrom(float angle)
    {
        m_axisState.m_axisRequested = angle;

    }

    public void ComputeHorizontalAngle(Vector3 localPosition, out float angle)
    {
        angle = -1;
        localPosition.y = 0;
        if (localPosition == Vector3.zero)
        {
            return;
        }

        if (localPosition.x >= 0 && localPosition.z >= 0)
        {
            angle = Mathf.Atan(localPosition.x / localPosition.z) * Mathf.Rad2Deg;
        }
        else if (localPosition.x > 0 && localPosition.z < 0)
        {
            angle = 90f + Mathf.Atan(-localPosition.z / localPosition.x) * Mathf.Rad2Deg;
        }
        else if (localPosition.x < 0 && localPosition.z >= 0)
        {
            angle = -Mathf.Atan(-localPosition.x / localPosition.z) * Mathf.Rad2Deg;
        }
        else if (localPosition.x < 0f && localPosition.z < 0)
        {
            angle = -90 + -Mathf.Atan(-localPosition.z / -localPosition.x) * Mathf.Rad2Deg;
        }

    }
    public void ComputeVerticalAngle(Vector3 localPosition, out float angle)
    {
        angle = -1;
        localPosition.x = 0;
        if (localPosition == Vector3.zero) {
            return;
        }
        float x = localPosition.y, y = localPosition.z ;

        if (x>= 0 && y >= 0)
        {
            angle = Mathf.Atan(x/ y) * Mathf.Rad2Deg;
        }
        else if (x> 0 && y < 0)
        {
            angle = 90f + Mathf.Atan(-y / x) * Mathf.Rad2Deg;
        }
        else if (x < 0 &&y >= 0)
        {
            angle = -Mathf.Atan(-x / y) * Mathf.Rad2Deg;
        }
        else if (x < 0f &&y < 0)
        {
            angle = -90 + -Mathf.Atan(-y/ -x) * Mathf.Rad2Deg;
        }

    }

    public Vector3 relocatedTest;
    public float m_h, m_v;
}


public enum TowerAxisType{ Horizontal, Vertical }
[System.Serializable]
public struct TowerDirectAxisInfo
{
    public TowerAxisType m_axis;
    public float m_maxRotationAngle;
    public float m_maxSpeedRotationAngle;
}
[System.Serializable]
public struct TowerAcceleratingAxisInfo
{
    public Vector3 m_axis;
    public float m_maxRotationAngle;
    public float m_maxSpeedRotationAngle;
    public float m_accelerationAngle;
}

[System.Serializable]
public struct TowerWorldPosition
{
    public Vector3 m_position;
    public Quaternion m_rotation;
}

[System.Serializable]
public struct TowerAxisState
{
    public float m_axisAngle;
    public float m_axisRequested;

    public bool IsRotateRequested()
    {
        return m_axisAngle != m_axisRequested;
    }

   
}
