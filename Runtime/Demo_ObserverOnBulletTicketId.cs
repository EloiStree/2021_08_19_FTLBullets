using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_ObserverOnBulletTicketId : MonoBehaviour
{
    public BulletTicketId m_id;
    public BulletJobSystemFacadeMono m_bulletsFacade;

    public BulletData m_initValue;
    public BulletDataResult m_result;
    public Transform m_linkedTransform;

    void Update()
    {
        m_bulletsFacade.GetInfoAboutBullet(m_id, out m_initValue, out m_result, out m_linkedTransform);


    }
}
