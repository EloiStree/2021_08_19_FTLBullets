using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBulletAndTargetInSceneMono : MonoBehaviour
{

    public int m_bulletsCount;
    public int m_bulletablesCount;

    void Update()
    {
        BulletInScene.GetCount(out m_bulletsCount);
        BulletableInScene.GetCount(out m_bulletablesCount);
    }
}
