using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SquareFarPixel2Debug : AbstractSquareFarPixelRendererListener
{
    public PixelFarConfig m_configuration;
    public SquareFarPixel[] m_farPixels;
    public bool m_useDebugDraw; 

    public override void InitParams(PixelFarConfig config)
    {
        m_configuration = config;
    }
    public override void RefreshWith(Vector3 worldPoint, Quaternion worldRotation,  NativeArray<SquareFarPixel> farPixel)
    {
        if (!this.enabled) return;
        m_farPixels = farPixel.ToArray();
        if (m_useDebugDraw)
        {
            DrawLineMinMax(worldPoint,  worldRotation, Color.green);

            //DrawLineMinMax(worldPoint, worldRotation * Quaternion.Euler(0, -m_configuration.m_horizontalAngle, 0));
            //DrawLineMinMax(worldPoint, worldRotation * Quaternion.Euler(0 , m_configuration.m_horizontalAngle, 0));
            //DrawLineMinMax(worldPoint, worldRotation * Quaternion.Euler(-m_configuration.m_verticalAngle , 0, 0));
            //DrawLineMinMax(worldPoint, worldRotation * Quaternion.Euler(m_configuration.m_verticalAngle , 0, 0));


            DrawLineMinMax(worldPoint, Quaternion.Euler(0, -m_configuration.m_horizontalAngle*0.5f, 0) * worldRotation, Color.red);
            DrawLineMinMax(worldPoint, Quaternion.Euler(0, m_configuration.m_horizontalAngle * 0.5f, 0) * worldRotation, Color.red);
            DrawLineMinMax(worldPoint, Quaternion.Euler(-m_configuration.m_verticalAngle * 0.5f, 0, 0) * worldRotation, Color.red);
            DrawLineMinMax(worldPoint, Quaternion.Euler(m_configuration.m_verticalAngle * 0.5f, 0, 0) * worldRotation, Color.red);

        }

    }

    private void DrawLineMinMax(Vector3 worldPoint, Quaternion worldRotation, Color color)
    {
        Debug.DrawLine(worldPoint + (worldRotation * Vector3.forward) * m_configuration.m_startDistance
                        , worldPoint + (worldRotation * Vector3.forward) * m_configuration.m_endDistance, color);
    }
}