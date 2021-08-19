using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ComputeMovingBulletsShaderMono : MonoBehaviour
{
    public int m_numberOfBulletsBufferWidth = 1024;
    public int m_numberOfBullets = 1024;
    public ComputeShader m_computeBullets;


    ComputableBulletStruct [] m_bulletsData;
    ComputeBuffer m_movingBulletBuffer;
    public ComputableBulletStruct m_test0;
    bool m_wasInit;

    public Transform m_toMoveForDebug;

    void Awake()
    {
        InitBuffer();
    }

    void InitBuffer() {
        m_numberOfBullets = m_numberOfBulletsBufferWidth * m_numberOfBulletsBufferWidth;
        m_bulletsData = new ComputableBulletStruct[m_numberOfBullets];
        for (int i = 0; i < m_numberOfBullets; i++)
        {
            //UseRandom
            m_bulletsData[i] = new ComputableBulletStruct()
            {
                m_xStartPoint = 0,
                m_yStartPoint = 0,
                m_zStartPoint = 0,

                m_xDirectionSpeed = UnityEngine.Random.value * 5,
                m_yDirectionSpeed = UnityEngine.Random.value * 5,
                m_zDirectionSpeed = UnityEngine.Random.value * 5,

               m_isUsed = 1
            };
            //Just Create
            //m_bulletsData[i] = new ComputableBulletStruct()
            //{
            //    m_startPoint = new Vector3(0, 0, 0),
            //    m_directionSpeed = new Vector3(0, 0, 0),
            //    m_isUsed =true
            //};

        }
        m_movingBulletBuffer = new ComputeBuffer( (int) m_numberOfBullets,
            Marshal.SizeOf(typeof(ComputableBulletStruct)), ComputeBufferType.Default);
        m_movingBulletBuffer.SetData(m_bulletsData);

        m_toMoveForDebug.transform.forward = m_bulletsData[0].GetDirection();

    }

     void Update()
    {
        ProcessMovement();
        m_test0 = m_bulletsData[0];
        m_toMoveForDebug.transform.position = m_test0.GetCurrentPosition();
    }

    void ProcessMovement() {

        int kernel  = m_computeBullets.FindKernel("CSMain");
        m_computeBullets.SetInt("m_bulletCount", m_numberOfBullets);
        m_computeBullets.SetInt("m_width", m_numberOfBulletsBufferWidth);
        m_computeBullets.SetFloat("m_timeOfGameInSeconds", Time.time);
        m_computeBullets.SetBuffer(kernel, "m_toProcess", m_movingBulletBuffer);
        m_computeBullets.Dispatch(kernel, m_numberOfBulletsBufferWidth / 32, m_numberOfBulletsBufferWidth / 32, 1);
        m_movingBulletBuffer.GetData(m_bulletsData);
    }


}


[StructLayout(LayoutKind.Sequential)]
[System.Serializable]
public struct ComputableBulletStruct
{
    //Origine
    public int m_isUsed;
    public float m_gameTimeWhenTriggerInSeconds;

    public float m_xStartPoint;
    public float m_yStartPoint;
    public float m_zStartPoint;

    public float m_xDirectionSpeed;
    public float m_yDirectionSpeed;
    public float m_zDirectionSpeed;


    //Result
    public double m_xCurrent;
    public double m_yCurrent;
    public double m_zCurrent;

    public double m_xPrevious;
    public double m_yPrevious;
    public double m_zPrevious;

    public float m_lifeTimeInSeconds;

    public Vector3 GetCurrentPosition()
    {
        return new Vector3((float)m_xCurrent, (float)m_yCurrent,(float) m_zCurrent);
    }
    public Vector3 GetDirection()
    {
        return new Vector3(m_xDirectionSpeed, m_yDirectionSpeed, m_zDirectionSpeed);
    }
    public Vector3 GetStartPoint()
    {
        return new Vector3(m_xStartPoint, m_yStartPoint, m_zStartPoint);
    }
}