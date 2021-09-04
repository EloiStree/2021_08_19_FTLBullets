using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using UnityEngine;

public class Demo_PushToPixelFarMeshMono : MonoBehaviour
{
    public PixelFarMeshMono m_pushToPixel;
    public Transform[] m_pointsToPush;
    public Transform m_viewPosition;
    public Transform m_viewRotation;

    public AbstractSquareFarPixelRendererListener [] m_renderers;

    void Start() {
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].InitParams(m_pushToPixel.m_config);
        }
    }
    void Update()
    {
        m_pushToPixel.RefreshCameraPosition(m_viewPosition.position, m_viewRotation.rotation);
        m_pushToPixel.RefreshTargetRef(m_pointsToPush);
        m_pushToPixel.ComputeFromSource( out NativeArray< SquareFarPixel>  farPixel);
        
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].RefreshWith(m_viewPosition.position, m_viewRotation.rotation, farPixel);

        }
    }


    


}
