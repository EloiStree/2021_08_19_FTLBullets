using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsJobSystemFacadeMono : MonoBehaviour
{

    [Header("In")]
    [Space(10)]
    [Header("Low Distance")]
    [Space(10)]
    public GameObject m_prefabRepresentation;
    public float m_lowDistanceMaxDistance = 1;
    [Header("Medium Distance")]
    [Space(10)]
    public Mesh m_lowMeshToDraw;
    public Material m_lowMeshMaterial;
    public float m_mediumDistanceMaxDistance = 3;
    [Header("Long Distance")]
    [Space(10)]
    public Material m_squadMaterial;
    public float m_longDistanceMaxDistance = 200;

    [Header("Pixel Distance")]
    [Space(10)]
    public Material m_pixelMaterial;
    public float m_pixelDistanceMaxDistance = 20000;


    [Header("Out")]
    [Space(10)]

    public DuoCapsuleLinesEvent m_collisionEvent;
    public DuoCapsuleLineDelegateEvent m_collisionEventDelegate;

    public void OnValidate()
    {
        //Do something.
        // Code not implemented yet.
    }
}
