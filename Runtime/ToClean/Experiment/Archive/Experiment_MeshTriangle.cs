using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment_MeshTriangle : MonoBehaviour
{
    public Mesh meshObject;
    public List<MeshLine> lines;

    [SerializeField]
    private MeshRenderer m_renderer;
    [SerializeField]
    private MeshFilter m_filter;
    [SerializeField]
    private MeshCollider m_collider;

    public int m_numberBulletToCreate=4;

    public float m_basePourcent=0.01f;

    private void Reset()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_filter = GetComponent<MeshFilter>();
        m_collider = GetComponent<MeshCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_numberBulletToCreate; i++)
        {
            lines.Add(GetRandomLine());
        }

        meshObject = new Mesh();
        GenerateLineColliderMesh();
        m_filter.sharedMesh = meshObject;
        m_collider.sharedMesh = meshObject;

        
    }

    //private void Update()
    //{
    //    for (int i = 0; i < lines.Count; i++)
    //    {
    //        MeshLine ml = lines[i];
    //        RandomizeMeshLine(ref ml);
    //    }
    //    GenerateLineColliderMesh();
    //}

    private MeshLine GetRandomLine()
    {
        MeshLine l = new MeshLine();

        l.m_startPosition = new Vector3(GetRandom(100), GetRandom(100), GetRandom(100));
        l.m_endPosition = l.m_startPosition + new Vector3(GetRandom(20), GetRandom(20), GetRandom(20));

        return l;
    }
    private void RandomizeMeshLine( ref MeshLine meshLine)
    {
        meshLine.m_startPosition.x = GetRandom(100);
        meshLine.m_startPosition.y = GetRandom(100);
        meshLine.m_startPosition.z = GetRandom(100);
        meshLine.m_endPosition.x = GetRandom(5);
        meshLine.m_endPosition.y = GetRandom(5);
        meshLine.m_endPosition.z = GetRandom(5);

    }
    public float GetRandom(float value) {
        return UnityEngine.Random.Range(-value, value);
    }

    public void GenerateLineColliderMesh()
    {
        int i1, i2, i3;
        int t1, t2, t3, t4,t5,t6;
        float triangleBasePourcent = m_basePourcent;

        Vector3[] vertices = new Vector3[lines.Count * 3];
        int[] triangles = new int[lines.Count * 6];

        Quaternion q1 = Quaternion.Euler(0, 90, 0);
        Quaternion q2 = Quaternion.Euler(0, -90, 0);
        Vector3 direction;
        Vector3 start;
        for (int i = 0; i < lines.Count; i++)
        {
            start = lines[i].m_startPosition;
            direction = lines[i].m_endPosition - lines[i].m_startPosition;

            i1 = i * 3;
            i2 = i * 3 + 1;
            i3 = i * 3 + 2;
            vertices[i1] = lines[i].m_endPosition;
            vertices[i2] = start+ (q1*(direction * triangleBasePourcent));
            vertices[i3] = start + (q2 * (direction* triangleBasePourcent));

            t1 = i * 6 + 0;
            t2 = i * 6 + 1;
            t3 = i * 6 + 2;
            t4 = i * 6 + 3;
            t5 = i * 6 + 4;
            t6 = i * 6 + 5;

            triangles[t1]= i1;
            triangles[t2] = i2;
            triangles[t3] = i3;
            triangles[t4] = i1;
            triangles[t5] = i3;
            triangles[t6] = i2;

            //Vector2[] uvs = new Vector2[0];
            //uvs[0] = new Vector2(0, 0);
            //uvs[1] = new Vector2(1, 0);
            //uvs[2] = new Vector2(0, 1);
            //uvs[3] = new Vector2(1, 1);


        }

        meshObject.SetVertices(vertices);
        meshObject.SetTriangles(triangles, 0);
       // meshObject.SetUVs(0, uvs);
        //meshObject.vertices = vertices;
        meshObject.name = "Tiangle Bullets";
     //  meshObject.RecalculateNormals();

      
    }
}

[Serializable]
public class MeshLine
{
    public Vector3 m_startPosition;
    public Vector3 m_endPosition;
}
