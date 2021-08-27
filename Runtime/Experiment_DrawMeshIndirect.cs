using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SOURCE/ https://www.youtube.com/watch?v=cwbyvbtJ9UY
public class Experiment_DrawMeshIndirect : MonoBehaviour
{

    public Transform m_playerPointOfView;
    public Material m_materialToUse;
    public Mesh m_mesh;

    public uint m_xCount=1000;
    public uint m_zCount=1000;
    public float m_objectsScale=0.1f;
    public List<Position> m_positions = new List<Position>();
    public List<List<Matrix4x4>> m_matrix4x4 = new List< List<Matrix4x4>>();

    public uint m_totalCount;
    void Start()
    {
        List<Matrix4x4> batch = new List<Matrix4x4>();
        bool createdOtherBatch=false;
        int c=0;
        for (int i = 0; i < m_xCount; i++)
        {
            for (int j = 0; j < m_zCount; j++)
            {
                Position p = new Position(i, 0, j);
                m_positions.Add(  p);
                batch.Add(p.GetM4x4());
                c++;
                if (c >= 1023) {
                    createdOtherBatch = true;
                    c = 0;
                    m_matrix4x4.Add(batch);
                    batch = new List<Matrix4x4>();
                }
            }
        }
        if(createdOtherBatch)
            m_matrix4x4.Add(batch);

        m_totalCount = m_xCount * m_zCount;

    }

    public LayerMask m_mask;
    MaterialPropertyBlock mpb;
    private void LateUpdate()
    {
        Bounds b = new Bounds(Vector3.zero, Vector3.one * float.MaxValue * 0.8f);
        for (int i = 0; i < m_matrix4x4.Count; i++)
        {
            Matrix4x4[] m4= m_matrix4x4[i].ToArray();
            Graphics.DrawMeshInstanced(m_mesh, 0, m_materialToUse, m4, m4.Length,
               mpb, UnityEngine.Rendering.ShadowCastingMode.Off, false, m_mask,
               null, UnityEngine.Rendering.LightProbeUsage.Off, null);

        }
    }

    public struct Position
    {
        public Vector3 m_position;
        public Vector3 m_scale;
        public Quaternion m_rotation;
        public Matrix4x4 m_matrix4x4;
        public Position(float x, float y, float z, float scale=0.1f) : this()
        {
            m_position.x = x;
            m_position.y = y;
            m_position.z = z;
            m_scale = Vector3.one* scale;
            m_rotation = Quaternion.identity;
        }

        public Matrix4x4 GetM4x4() {
            return Matrix4x4.TRS(m_position, m_rotation, m_scale); 
        }
    }
}
