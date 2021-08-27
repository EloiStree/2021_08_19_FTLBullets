using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LightSpeedBulletPooling : MonoBehaviour
{
    public int m_bulletCount = 1000;
    public int m_poolIndex = 0;

    private NativeArray<TriggeredBulletData> poolOfBullets = new NativeArray<TriggeredBulletData>();



    public void GetNextBullet() { 
    
    
    }

}
