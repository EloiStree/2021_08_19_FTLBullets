using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletableTagMono : MonoBehaviour
{
    public static List<BulletableTagMono> m_bulletablesInScene = new List<BulletableTagMono>();

    private void Awake()
    {
        m_bulletablesInScene.Add(this);
    }
    private void OnDestroy()
    {
        m_bulletablesInScene.Remove(this);
    }

}

