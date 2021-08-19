using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LightSpeedBulletPooling : MonoBehaviour
{
    public int m_bulletCount = 1000;
    public int m_poolIndex = 0;

    private NativeArray<BulletData> poolOfBullets = new NativeArray<BulletData>();



    public void GetNextBullet() { 
    
    
    }

}
