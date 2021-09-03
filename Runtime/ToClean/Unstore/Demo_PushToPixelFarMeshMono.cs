using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Demo_PushToPixelFarMeshMono : MonoBehaviour
{
    public PixelFarMeshMono m_pushToPixel;
    public Transform[] m_pointsToPush;
    public Transform m_cameraRoot;

    public AbstractSquareFarPixelRendererListener [] m_renderers;

    void Start() {
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].InitParams(m_pushToPixel.m_config);
        }
    }
    void Update()
    {
        m_pushToPixel.RefreshCameraPosition(m_cameraRoot.position, m_cameraRoot.rotation);
        m_pushToPixel.RefreshTargetRef(m_pointsToPush);
        m_pushToPixel.ComputeFromSource( out NativeArray< SquareFarPixel>  farPixel);
        
        for (int i = 0; i < m_renderers.Length; i++)
        {
            m_renderers[i].RefreshWith(m_cameraRoot.position, m_cameraRoot.rotation, farPixel);

        }
    }
}
