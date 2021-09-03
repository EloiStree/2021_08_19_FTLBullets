using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DemoExample_ListeningTobulletCollision : MonoBehaviour
{


    public GameObject m_particuleExplosion;
    public float m_particuleLifeTime = 3;
    public Color m_colorShortenLine = Color.green+Color.yellow;
    public int m_drawCount;
    public int m_hitCount;
    public int m_hitCountWithAntiSpam;
    public void DrawShortestLineCollision(S_CapsulesCollision data)
    {
        m_drawCount++;
        Debug.DrawLine(
            data.m_shortestPath.m_bulletShortestStart + Vector3.up,
            data.m_shortestPath.m_bulletableShorestEnd + Vector3.up,
            m_colorShortenLine

            );

    }

    Queue<int> remove = new Queue<int>();
    private void Update()
    {
        if (m_antiSpam.Count <= 0)
            return;
        float t = Time.deltaTime;
        remove.Clear();
        List<int> keys = m_antiSpam.Keys.ToList();
        foreach (int item in keys)
        {
         
                float td = m_antiSpam[item] - t;
                m_antiSpam[item] =td;
                if (td < 0f)
                    remove.Enqueue(item);
            
        }

        while (remove.Count > 0){
            int id = remove.Dequeue();
            m_antiSpam.Remove(id);

        }
    }

    public float m_antiSpamTime = 3f;
    public Dictionary<int, float> m_antiSpam= new Dictionary<int, float>();
    public void ManageBulletHit(CollisionInfo data) {


        m_hitCount++;
        try
        {
            int id = data.m_collisionRaw.m_bulletCapsuleId;
            if (!m_antiSpam.ContainsKey(id)) {
                m_hitCountWithAntiSpam++;
                //data.m_bulletManager.NotifyBulletAsNotUsedAnymore(data.m_bulletTicket);
                GameObject g= GameObject.Instantiate(m_particuleExplosion);
                S_ShortestPath s = data.m_collisionRaw.m_shortestPath;
                g.transform.position = (s.m_bulletShortestStart + s.m_bulletableShorestEnd)*0.5f;
                Destroy(g, m_particuleLifeTime);
                m_antiSpam.Add(id, m_antiSpamTime);
            }
        }
        catch (Exception e) {
            Debug.LogWarning("HUMMM "+e.StackTrace);
        }
    }

}
