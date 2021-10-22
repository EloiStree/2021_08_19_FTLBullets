using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class InSceneTransformBulletPoolingMono : AbstractTransformRepresentationOfBullets
{

    public Transform[] m_bulletsAsTransform;
    public TransformAccessArray m_accesArray;

    public Transform m_hidingSpotInGame;
    public bool m_useZeroScaleHiding;
    public override void GetMaxiumAllow(out int maxEntity)
    {
        maxEntity=  m_bulletsAsTransform.Length;
    }

    public override void GetNativeTransformArrayRef(out TransformAccessArray nativeArrayRef)
    {
        nativeArrayRef = m_accesArray;
    }

    public override void GetTransformFor(int index, out Transform transform)
    {
        if (index < 0 || index >= m_bulletsAsTransform.Length)
            transform = null;
        else 
            transform = m_bulletsAsTransform[index];
    }

    void Awake()
    {
        SetTheNativeArray();
    }
    private void SetTheNativeArray()
    {
        if (m_accesArray.isCreated)
            m_accesArray.Dispose();

        m_accesArray = new TransformAccessArray(m_bulletsAsTransform, 64);
    }


    private void OnDestroy()
    {
        if (m_accesArray.isCreated)
            m_accesArray.Dispose();
    }

    public override void SetAsUsing(int index, bool usingTransform)
    {
        if (index > -1 && index < m_bulletsAsTransform.Length && m_bulletsAsTransform[index] != null)
        {
            m_bulletsAsTransform[index].gameObject.SetActive(usingTransform);
            if (m_hidingSpotInGame) {
                m_bulletsAsTransform[index].position = m_hidingSpotInGame.position;

            }
            if (m_useZeroScaleHiding) {
                m_bulletsAsTransform[index].localPosition = Vector3.zero;
            }
        }
    }
}
