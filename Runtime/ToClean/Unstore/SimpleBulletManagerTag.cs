using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBulletManagerTag : MonoBehaviour
{
    private static AbstractBulletsManager m_bulletManagerInScene;
    public static AbstractBulletsManager Instance { get { return m_bulletManagerInScene; } }
    public AbstractBulletsManager m_bulletManagerToUse;
    void Awake()
    {
        m_bulletManagerInScene = m_bulletManagerToUse;
    }

}
