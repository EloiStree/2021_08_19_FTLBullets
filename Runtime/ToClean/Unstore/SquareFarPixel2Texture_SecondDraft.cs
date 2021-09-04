using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SquareFarPixel2Texture_SecondDraft : AbstractSquareFarPixelRendererListener
{
    public PixelFarConfig m_currentConfig;
    public Texture2D m_textureTemp;

    public Renderer m_debugObjectRenderer;
    public RawImage m_debugRawImageTexture;
    public Material m_debugMaterial;

    public int m_textureDimensionWidth = 512;
    public int m_textureDimensionHeight = 256;
    public int m_textureDimensionHeightMaxIndex;


    public override void InitParams(PixelFarConfig config)
    {
        m_textureDimensionHeightMaxIndex = m_textureDimensionWidth * m_textureDimensionHeight;
            m_currentConfig = config;
            m_textureTemp = new Texture2D(m_textureDimensionWidth, m_textureDimensionHeight, TextureFormat.ARGB32, true);
            m_textureTemp.Apply();
        

        if (m_debugRawImageTexture != null)
        {
            m_debugRawImageTexture.texture = m_textureTemp;
        }
        if (m_debugObjectRenderer != null)
        {
            m_debugObjectRenderer.material.mainTexture = m_textureTemp;
        }
        if (m_debugMaterial != null)
        {
            m_debugMaterial.mainTexture = m_textureTemp;
        }

        m_flushArray = new Color[m_textureDimensionHeightMaxIndex];
        for (int i = 0; i < m_flushArray.Length; i++)
        {
            m_flushArray[i].a = 0;

        }
    }

     Color [] m_flushArray;
    public Color m_pixelColor = Color.red + Color.green;
    public float m_fullColorUnity = 5;
    Color m_pixelColorPerUnity;

    
    public override void RefreshWith(Vector3 worldPoint, Quaternion worldRotation, NativeArray<SquareFarPixel> farPixel)
    {

        if (!this.enabled)
            return;
        if (m_textureTemp == null)
            return;

        //m_pixelColorPerUnity = m_pixelColor / m_fullColorUnity;
        m_textureTemp.SetPixels(m_flushArray);

        Color[] c = m_textureTemp.GetPixels();
        IntensityBasedOnMinDistance (ref farPixel);
        m_textureTemp.Apply();

    }

    private void IntensityBasedOnMinDistance(ref NativeArray<SquareFarPixel> farPixel)
    {
        int maxIndexFarPixel = m_currentConfig.m_heightInPixel * m_currentConfig.m_widthInPixel;
        double ratio = m_textureDimensionHeightMaxIndex / (double) maxIndexFarPixel;
        float min = m_currentConfig.m_startDistance, max = m_currentConfig.m_endDistance;

        int x; float xp; 
        int y;  float yp;
        int xd ;
        int yd;

        for (int i = 0; i < farPixel.Length; i++)
        {
            // Get from 0 to config width
             x = (int)(farPixel[i].m_horizontalIndex + m_currentConfig.m_widthInPixel * 0.5f);
             y = (int)(farPixel[i].m_verticalIndex + m_currentConfig.m_heightInPixel * 0.5f);
            
            // get from 0 to 1
            xp = ((float)x) / m_currentConfig.m_widthInPixel;
             yp = ((float)y) / m_currentConfig.m_heightInPixel;
             
            // get from 0 to image width
            xd = (int)(xp * m_textureTemp.width);
             yd = (int)(yp * m_textureTemp.height);
            m_textureTemp.SetPixel(xd, yd, Color.yellow);
                //m_pixelColor *  (1f - (farPixel[i].m_minDistance - min) / (max - min)) );

        }
    }
}
