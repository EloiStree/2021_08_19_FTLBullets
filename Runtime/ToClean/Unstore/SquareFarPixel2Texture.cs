using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SquareFarPixel2Texture : AbstractSquareFarPixelRendererListener
{

    public Camera m_camera;
    public PixelFarConfig m_currentConfig;
    public RenderTexture m_texture;
    public Texture2D m_textureTemp;
    public ComputeShader m_blackFlusher;

    public Transform m_debugObject;
    public Renderer m_debugObjectRenderer;

    public RawImage m_debugRawImageTexture;


    public override void InitParams(PixelFarConfig config)
    {
        m_currentConfig = config;
        
            m_texture = new RenderTexture(m_currentConfig.m_widthInPixel, m_currentConfig.m_heightInPixel, 0, RenderTextureFormat.ARGB32);
            m_texture.wrapMode = TextureWrapMode.Clamp;
            m_texture.enableRandomWrite = true;
            m_texture.Create();
        
       
            m_textureTemp = new Texture2D(m_currentConfig.m_widthInPixel, m_currentConfig.m_heightInPixel, TextureFormat.ARGB32, false);
            m_textureTemp.Apply();
        

        if (m_debugRawImageTexture != null)
        {
            m_debugRawImageTexture.texture = m_textureTemp;
        }
        if (m_debugObjectRenderer != null)
        {
            //m_debugObject.position = 
            m_debugObjectRenderer.material.mainTexture = m_textureTemp;
        }
        if (m_debugObject != null)
        {
            //TODO MOve the object with the renderer
        }
        m_flushArray = new Color[config.m_widthInPixel * config.m_heightInPixel];
        for (int i = 0; i < m_flushArray.Length; i++)
        {
            m_flushArray[i].a = 0;

        }
    }

    public Color [] m_flushArray;
    public Color m_pixelColor = Color.red + Color.green;
    public float m_fullColorUnity = 5;
    Color m_pixelColorPerUnity;

    


    private void FlushToBlackTransparent()
    {
        if (m_blackFlusher != null && m_texture != null)
        {
            if (m_camera != null)
            {
                Vector3 left = Quaternion.Inverse(m_camera.transform.rotation)*(m_camera.ViewportToWorldPoint(new Vector3(-1f, 0, 10))- m_camera.transform.position);
                Vector3 right = Quaternion.Inverse(m_camera.transform.rotation) * (m_camera.ViewportToWorldPoint(new Vector3(1f, 0, 10)) - m_camera.transform.position);
                Vector3 top = Quaternion.Inverse(m_camera.transform.rotation) * (m_camera.ViewportToWorldPoint(new Vector3(0, 1f, 10)) - m_camera.transform.position);
                Vector3 down = Quaternion.Inverse(m_camera.transform.rotation) * (m_camera.ViewportToWorldPoint(new Vector3(0, -1f, 10)) - m_camera.transform.position);
                m_currentConfig.m_horizontalAngle = Vector3.Angle(left, right);
                m_currentConfig.m_verticalAngle = Vector3.Angle(top, down);

            }

            //m_texture.Create();
            //Should be done with shader
            //int i = m_blackFlusher.FindKernel("CSMain");
            //m_blackFlusher.SetBool("m_useTransparence", true);
            //m_blackFlusher.SetTexture(i, "m_texture", m_texture);
            //m_blackFlusher.Dispatch(i, 8, 8, 1);
        }
    }
    public override void RefreshWith(Vector3 worldPoint, Quaternion worldRotation, NativeArray<SquareFarPixel> farPixel)
    {

        if (!this.enabled)
            return;
        if (m_texture == null || m_textureTemp == null)
            return;

        m_pixelColorPerUnity = m_pixelColor / m_fullColorUnity;
        m_texture.Create();
        //FlushToBlackTransparent();
        RenderTexture.active = m_texture;
        m_textureTemp.ReadPixels(new Rect(0, 0, m_texture.width, m_texture.height), 0, 0);
        

        float min = m_currentConfig.m_startDistance, max= m_currentConfig.m_endDistance;
        
        Color[] c = m_textureTemp.GetPixels();
        for (int i = 0; i < farPixel.Length; i++)
        {
           
                c[(int)farPixel[i].m_index] = m_pixelColor *
                (1f-(farPixel[i].m_minDistance-min)/(max- min));
            
        }
        m_textureTemp.SetPixels(c);

        m_textureTemp.Apply();
        RenderTexture.active = null;

    }

}
