using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTagMono : MonoBehaviour
{
    public static List<BulletTagMono> m_bulletsInScene = new List<BulletTagMono>();

    private void Awake()
    {
        m_bulletsInScene.Add(this);
    }
    private void OnDestroy()
    {
        m_bulletsInScene.Remove(this);
    }

}
