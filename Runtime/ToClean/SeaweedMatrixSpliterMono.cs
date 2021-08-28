using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class SeaweedMatrixSpliterMono : MonoBehaviour
{

    public float m_bulletMaxSpeedPerFrame=10;
    public Transform m_center;
    public Transform m_root;
    public Transform[] m_demo;
    public Texture2D m_topViewDebug;

    public void GetRandomPositionInMatrix(out Vector3 localPosition)
    {
        float edgeSize = matrixSplite.m_spliter.m_matrixSquareEdgeSize;
        localPosition = new Vector3(UnityEngine.Random.Range(0, edgeSize), UnityEngine.Random.Range(0, edgeSize), UnityEngine.Random.Range(0, edgeSize));
    }

    public Renderer m_renderer;

    public SeaweedMatrixSpliterJobSystem matrixSplite;

    public Transform m_groundDebug;


    public uint m_matrixDim = 64;
    void Start()
    {
        m_squareDimension = m_matrixDim * m_bulletMaxSpeedPerFrame;
        m_root.rotation = m_center.rotation;
        m_root.localPosition = Vector3.zero;

        m_root.Translate(new Vector3(m_squareDimension / 2f, m_squareDimension / 2f, m_squareDimension / 2f)*-1f, Space.Self);

        m_groundDebug.localScale = new Vector3(m_squareDimension, 0.1f, m_squareDimension);
        m_groundDebug.localPosition = new Vector3(m_squareDimension/2f,0, m_squareDimension / 2f);

        Init();
        matrixSplite = new SeaweedMatrixSpliterJobSystem();
        matrixSplite.m_spliter.Init(m_matrixDim,10000, m_squareDimension);

        InvokeRepeating("Debug", 0.1f, 1);
    }

    private void OnDestroy()
    {
        matrixSplite.m_spliter.Dispose();
    }

    private void Init()
    {
        countArray = new int[m_matrixDim * m_matrixDim * m_matrixDim];
        countArray2D = new int[m_matrixDim * m_matrixDim];
        countArray2DColor = new Color[m_matrixDim * m_matrixDim];
        m_topViewDebug = new Texture2D((int)m_matrixDim, (int)m_matrixDim);
        colorPerCells = new Color[m_matrixDim * m_matrixDim * m_matrixDim];
        for (int i = 0; i < colorPerCells.Length; i++)
        {
            float r, g, b;
            r = UnityEngine.Random.Range(0, 3) % 3 == 0 ? UnityEngine.Random.Range(0.3f, 1f) : 0f;
            g = UnityEngine.Random.Range(0, 3) % 3 == 0 ? UnityEngine.Random.Range(0.3f, 1f) : 0f;
            b = UnityEngine.Random.Range(0, 3) % 3 == 0 ? UnityEngine.Random.Range(0.3f, 1f) : 0f;
            colorPerCells[i] =new Color(r,g,b);

        }
        DrawCellColorCouldZone();
        DrawCellPathCouldZone();
    }

    int[] countArray;
    int[] countArray2D;
    Color[] countArray2DColor;


    public Color m_empty = Color.black;
    public Color m_startColor= Color.yellow;
    public Color m_endColor = Color.red;

    public int maximum = 0;
    void Debug()
    {

        DrawDebugZone();

        matrixSplite.m_spliter.PushIn(m_demo,m_root);
        matrixSplite.m_spliter.CompteAllCellIn(ref countArray);
        //LinkedList<int> ids = new LinkedList<int>();
        //matrixSplite.m_spliter.get(0, 0,0, ref ids);
        int i3D = 0;
        int i2D = 0;
        int width =(int) m_matrixDim;
        int[] cellsi3x3 = new int[width];
        LinkedList<int> idsFound = new LinkedList<int>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                idsFound.Clear();
                matrixSplite.m_spliter.GetXZCells(x, width - 1-z, ref cellsi3x3);
                for (int i = 0; i < width; i++)
                {
                    matrixSplite.m_spliter.AppendIdsOfCell(cellsi3x3[i], ref idsFound);
                }
                i2D = (x) * matrixSplite.m_spliter.m_dimensionAsInt + (z);
                countArray2D[i2D] = idsFound.Count;
                if (countArray2D[i2D] > maximum)
                    maximum = countArray2D[i2D];
            }
        }
            for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                //i2D = (width - 1 - x) * matrixSplite.m_spliter.m_dimensionAsInt + (width - 1 - y);
                i2D = ( x) * matrixSplite.m_spliter.m_dimensionAsInt + ( y);


                if (countArray2D[i2D] <= 0)
                    countArray2DColor[i2D] = m_empty;
                else
                    countArray2DColor[i2D] = Color.Lerp(m_startColor, m_endColor,( (float)countArray2D[i2D]) / (float)maximum);
            }
        }
        m_topViewDebug.SetPixels(countArray2DColor);
        m_topViewDebug.Apply();
        if (m_renderer != null)
            m_renderer.material.mainTexture = m_topViewDebug;

        DrawCellColorCouldZone();
        DrawCellPathCouldZone();

        int[] cellsId3x3 = new int[matrixSplite.m_spliter.m_dimensionAsInt];


        matrixSplite.m_spliter.GetXZCells(0, 0, ref cellsId3x3);
        UnityEngine.Debug.Log("TT2 " + string.Join(" ", cellsId3x3));
        DrawColomn(ref cellsId3x3, Color.green, 5);


        matrixSplite.m_spliter.GetXYCells(0, 0, ref cellsId3x3);
        UnityEngine.Debug.Log("TT1 " + string.Join(" ", cellsId3x3));
        DrawColomn(ref cellsId3x3, Color.blue, 5);


        matrixSplite.m_spliter.GetYZCells(0, 0, ref cellsId3x3);
        UnityEngine.Debug.Log("TT3 " + string.Join(" ", cellsId3x3));
        DrawColomn(ref cellsId3x3, Color.red, 5);


    }

    private void DrawColomn(ref int[] cellsId3x3, Color color, float timeInMs)
    {
        float edgeSize = matrixSplite.m_spliter.m_matrixSquareEdgeSize;
        int dim = matrixSplite.m_spliter.m_dimensionAsInt;
        float cellSize = edgeSize / (float)dim;
        float halfCell = cellSize / 2f;

        if (cellsId3x3.Length < 2)
            return;
        Vector3 s, e;
        int si, se;
        int x;  int y;  int z;
        for (int i = 0; i < cellsId3x3.Length - 1; i++)
        {
            si = cellsId3x3[i]; 
            matrixSplite.m_spliter.Get3x3ArrayIndex(si, out  x, out y, out  z);
            s.x = x * cellSize + halfCell;
            s.y = y * cellSize + halfCell;
            s.z = z * cellSize + halfCell;

            s = m_root.TransformPoint(s);

            se = cellsId3x3[i + 1];
            matrixSplite.m_spliter.Get3x3ArrayIndex(se, out  x, out  y, out  z);
            e.x = x*cellSize+halfCell;
            e.y = y * cellSize + halfCell;
            e.z = z * cellSize + halfCell;
            e = m_root.TransformPoint(e);
            //UnityEngine.Debug.Log(string.Format("D {0}->{1}", si, se));
            
            UnityEngine.Debug.DrawLine(s, e, color, timeInMs);
            
        }
    }

    public float m_squareDimension;
    private void DrawDebugZone()
    {
        float i = m_squareDimension ;
        Vector3[] pts = new Vector3[8];

        pts[0] = m_root.TransformPoint(new Vector3(0, 0, 0));
        pts[1] = m_root.TransformPoint(new Vector3(0, 0, i));
        pts[2] = m_root.TransformPoint(new Vector3(0, i, 0));
        pts[3] = m_root.TransformPoint(new Vector3(0, i, i));
        pts[4] = m_root.TransformPoint(new Vector3(i, 0, 0));
        pts[5] = m_root.TransformPoint(new Vector3(i, 0, i));
        pts[6] = m_root.TransformPoint(new Vector3(i, i, 0));
        pts[7] = m_root.TransformPoint(new Vector3(i, i, i));

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                UnityEngine.Debug.DrawLine(pts[y], pts[x], Color.white, 5);
            }
        }




    }
    private void DrawCellColorCouldZone()
    {
        float edgeSize = matrixSplite.m_spliter.m_matrixSquareEdgeSize;
        int dim = matrixSplite.m_spliter.m_dimensionAsInt;
        float cellSize = edgeSize / (float)dim;
        float halfCell = cellSize / 2f;

        Vector3 v;
        int i3x3 = 0;
        for (int x = 0; x < dim; x++)
        {
            for (int y = 0; y < dim; y++)
            {
                for (int z = 0; z < dim; z++)
                {
                    matrixSplite.m_spliter.Get3x3ArrayIndex(x, y, z, out i3x3);
                    bool on = countArray[i3x3] > 0;
                    v = m_root.TransformPoint(new Vector3(x * cellSize + halfCell, y * cellSize + halfCell, z * cellSize + halfCell));
                    UnityEngine.Debug.DrawLine(v - Vector3.up * 0.25f, v + Vector3.up * 0.25f, on ? Color.green : Color.red * 0.2f, 5);
                }
            }
        }
    }

    public enum PathType { Cell, CellAndNearCells}
    public PathType m_pathType;
    private Color[] colorPerCells;
    private void DrawCellPathCouldZone()
    {
        float edgeSize = matrixSplite.m_spliter.m_matrixSquareEdgeSize;
        int dim = matrixSplite.m_spliter.m_dimensionAsInt;
        float cellSize = edgeSize / (float)dim;
        float halfCell = cellSize / 2f;

        Vector3 v;
        int i3x3 = 0;
        LinkedList<int> tmp = new LinkedList<int>();
        for (int x = 0; x < dim; x++)
        {
            for (int y = 0; y < dim; y++)
            {
                for (int z = 0; z < dim; z++)
                {
                    tmp.Clear();
                    matrixSplite.m_spliter.Get3x3ArrayIndex(x, y, z, out i3x3);
                    if (!matrixSplite.m_spliter.IsMatrixCellEmpty(i3x3)) {

                        if (m_pathType == PathType.Cell)
                            matrixSplite.m_spliter.AppendIdsOfCell(i3x3, ref tmp);
                        else { 
                            matrixSplite.m_spliter.AppendIdsOfCellAndAround(i3x3, ref tmp);
                            List<int> l = tmp.Distinct().ToList();
                            tmp.Clear();
                            for (int i = 0; i < l.Count; i++)
                            {
                                tmp.AddLast(l[i]);
                            }

                        }
                        UnityEngine.Debug.Log(string.Format("Not empty to draw {0}:{1}:{2} c{3}",x,y,z, tmp.Count));
                        if (tmp.Count == 1)
                        {

                            bool on = countArray[i3x3] > 0;
                            v = m_root.TransformPoint(new Vector3(x * cellSize + halfCell, y * cellSize + halfCell, z * cellSize + halfCell));
                            UnityEngine.Debug.DrawLine(v - Vector3.up * 0.25f, v + Vector3.up * 0.25f, Color.blue, 5);

                        }
                        else if (tmp.Count > 1) {
                           

                            Vector3 s, e;
                            int si, se;
                            for (int i = 0; i < tmp.Count-1; i++)
                            {
                                si = tmp.ElementAt(i);
                                se = tmp.ElementAt(i + 1);
                                    UnityEngine.Debug.Log(string.Format("Path {0} > {1}", si, se));
                                if (si > -1 && se > -1) {
                                    s = m_demo[si].position;
                                    e = m_demo[se].position;
                                   
                                    UnityEngine.Debug.DrawLine(s, e,
                                        colorPerCells[i3x3], 5);
                                }
                            }

                        }
                    }
                }
            }
        }




    }
}

public struct SeaweedMatrixSpliterJobSystem {

    public SeaweedMatrixSpliter m_spliter;

}

public struct SeaweedMatrixSpliter
{
    public uint m_dimension;
    public int m_dimensionAsInt;
    public uint m_elementCount;
    NativeArray<int> m_matrixCells;
    NativeArray<int> m_matrixCellsSeaweedHead;
    NativeArray<LinkedId> m_elements;
    public float m_matrixSquareEdgeSize;

    private bool m_isInit;

    private void ResetAll()
    {
        for (int i = 0; i < m_matrixCells.Length; i++)
        {
            m_matrixCells[i] = -1;
            m_matrixCellsSeaweedHead[i] = -1;
        }
        for (int i = 0; i < m_elements.Length; i++)
        {
            m_elements[i] = new LinkedId() { m_nextLinked = -1, m_objectId = -1     };
        }
    }


    public void Init(uint dimensionOfSplit=128, uint idMaxCount=10000, float matrixSquareEdgeSize=256)
    {
        this.m_dimension = dimensionOfSplit;
        m_dimensionAsInt = (int)dimensionOfSplit;
        m_elementCount = dimensionOfSplit * dimensionOfSplit * dimensionOfSplit;
        m_matrixSquareEdgeSize = matrixSquareEdgeSize;

        int[] matrixData = new int[m_elementCount];
        m_matrixCells = new NativeArray<int>(matrixData,Allocator.Persistent);

        matrixData = new int[m_elementCount];
        m_matrixCellsSeaweedHead = new NativeArray<int>(matrixData, Allocator.Persistent);

        LinkedId[] matrixElements = new LinkedId[idMaxCount];
        m_elements = new NativeArray<LinkedId>(matrixElements, Allocator.Persistent);

        for (int i = 0; i < m_elementCount; i++)
        {

            m_matrixCells[i]= m_matrixCellsSeaweedHead[i]=-1;
        }


        ResetAll();
        m_isInit = true;
    }

    public void Dispose()
    {
        if (m_isInit) { 
            m_matrixCells.Dispose();
            m_matrixCellsSeaweedHead.Dispose();
            m_elements.Dispose();
        }
    }

    public void SetIdArrayValue(int elementId, int value) {
        LinkedId linkedId = m_elements[elementId];
        linkedId.m_objectId = value;
        m_elements[elementId] = linkedId;


    }

    public void AddIdToSeaweedList(int matrix3x3Id, int elementIndex) {

        if (IsMatrixCellEmpty(matrix3x3Id))
        {
            m_matrixCells[matrix3x3Id] = elementIndex;
            m_matrixCellsSeaweedHead[matrix3x3Id] = elementIndex;

            Debug.Log(string.Format("create {0}->{1}", matrix3x3Id, elementIndex));
        }
        else
        {
            int previousId = m_matrixCellsSeaweedHead[matrix3x3Id];
            LinkedId element = m_elements[previousId];
            element.m_nextLinked = elementIndex;
            m_elements[previousId] = element;
            m_matrixCellsSeaweedHead[matrix3x3Id] = elementIndex;
            Debug.Log(string.Format("add m {0}  p{2} ->n {1}", matrix3x3Id, elementIndex, previousId));
        }
    }

    public bool IsMatrixCellEmpty(int matrix3x3Id)
    {
        return m_matrixCells[matrix3x3Id] < 0;
    }



    public void CompteAllCellIn(ref int[] array) 
    {
        if (array.Length != m_elementCount)
            throw new ArgumentOutOfRangeException("You should provide an array with the same size of the matrix spliter (dimension):" +m_dimension);

        LinkedList<int> ids= new LinkedList<int>();
        for (int i = 0; i < array.Length; i++)
        {
            int startCount = ids.Count;
            AppendIdsOfCell(i, ref ids);
            array[i] = ids.Count - startCount;
            if(array[i] > 0)
            Debug.Log(string.Format("count m {0}  -> {1}", i, array[i]));
        }
        ids.Clear();
    }
    public int GetCount(int matrix3x3Id) {
        LinkedList<int> ids = new LinkedList<int>();
        AppendIdsOfCell(matrix3x3Id, ref ids);
        return ids.Count;
    }


    public void AppendIdsOfCell(int matrix3x3Id, ref LinkedList<int> ids) {
        if (ids ==null)
            ids = new LinkedList<int>();
        int i = m_matrixCells[matrix3x3Id];
        if (i < 0)
            return;
        Debug.Log(string.Format("C> m{0} i{1}", matrix3x3Id,i));

        int antiloop=0;

        do
        {
            Debug.Log(string.Format("N> pi{0} ni{1}", i, m_elements[i].m_nextLinked));
            ids.AddLast(m_elements[i].m_objectId);
            i = m_elements[i].m_nextLinked;
            antiloop++;
            if (antiloop > (300))
                throw new Exception("Loop present in your code");
        } while ( i > -1 );


    }
    public void PushIn(Transform[] pointsAsTransform, Transform compareTo)
    {
        IdToRef[] idToRef = new IdToRef[pointsAsTransform.Length];
        Vector3 position;
        for (int i = 0; i < idToRef.Length; i++)
        {
            if (compareTo == null)
            {

                position = pointsAsTransform[i].position;

            }
            else
            {
                position = compareTo.InverseTransformPoint(pointsAsTransform[i].position);
            }
            idToRef[i] = new IdToRef(i, position);
        }
        PushIn(idToRef);

    }
    public void PushIn(IdToRef[] worldPositionV3 )
    {
        ResetAll();
        int elementIndex=0;

        int x, y, z;
        for (int i = 0; i < worldPositionV3.Length ; i++)
        {

            Vector3 v3 = worldPositionV3[i].m_localPosition;
            
            if (Mathf.Abs(v3.x) < m_matrixSquareEdgeSize
                && Mathf.Abs(v3.y) < m_matrixSquareEdgeSize
                && Mathf.Abs(v3.z) < m_matrixSquareEdgeSize 
                && Mathf.Abs(v3.x) >= 0f
                && Mathf.Abs(v3.y) >= 0f
                && Mathf.Abs(v3.z) >= 0f)
            {
                v3 /= m_matrixSquareEdgeSize;

                x = (int)((v3.x ) * m_dimension);
                y = (int)((v3.y ) * m_dimension);
                z = (int)((v3.z ) * m_dimension);
                Get3x3ArrayIndex(x, y, z, out int i3x3);
                SetIdArrayValue(i, worldPositionV3[i].m_objectId);
                AddIdToSeaweedList(i3x3,i);


                elementIndex++;
                if (elementIndex >= m_elementCount)
                    break;
            }
        }
    }



    public void Get3x3ArrayIndex(int x, int y, int z, out int index3x3)
    {
        index3x3 = x * (m_dimensionAsInt * m_dimensionAsInt) + y * (m_dimensionAsInt) + z;

    }
    public void Get3x3ArrayIndex( int index3x3, out int x, out int y, out int z )
    {
       
              z = index3x3 / (m_dimensionAsInt * m_dimensionAsInt);
              index3x3 -= (z * m_dimensionAsInt * m_dimensionAsInt);
              y = index3x3 / m_dimensionAsInt;
              x = index3x3 % m_dimensionAsInt;
        
    }

    internal void GetXYCells(int x, int y, ref int[] cellsId3x3)
    {
        if (cellsId3x3.Length != m_dimensionAsInt)
            throw new Exception("Dimension not matching");
        int i3x3;
        for (int i = 0; i < m_dimensionAsInt; i++)
        {
            Get3x3ArrayIndex(x, y, i, out i3x3);
            cellsId3x3[i] = i3x3;

        }
    }

    internal void GetXZCells(int x, int z, ref int[] cellsId3x3)
    {
        if (cellsId3x3.Length != m_dimensionAsInt)
            throw new Exception("Dimension not matching");
        int id;
        for (int i = 0; i < m_dimensionAsInt; i++)
        {
            Get3x3ArrayIndex(x, i, z, out id);
            cellsId3x3[i] = id;

        }
    }

    internal void GetYZCells(int y, int z, ref int[] cellsId3x3)
    {
        if (cellsId3x3.Length != m_dimensionAsInt)
            throw new Exception("Dimension not matching");
        int id;
        for (int i = 0; i < m_dimensionAsInt; i++)
        {
            Get3x3ArrayIndex(i, y, z, out id);
            cellsId3x3[i] = id;

        }
    }

    internal void AppendIdsOfCellAndAround(int x, int y, int z, ref LinkedList<int> index3x3s)
    {
        int tx = x, ty = y, tz = z;

        for (int xx = -1; xx < 1; xx++)
        {
            for (int yy = -1; yy < 1; yy++)
            {
                for (int zz = -1; zz < 1; zz++)
                {

                    AppendIdsOfCellsProtected(x + xx, y + yy, z + zz, ref index3x3s);
                }
            }
        }
    



    }

    private void AppendIdsOfCellsProtected( int x, int y, int z, ref LinkedList<int> index3x3s)
    {
        if (x >= 0 && x < m_dimensionAsInt
                    && y >= 0 && y < m_dimensionAsInt
                    && z >= 0 && z < m_dimensionAsInt)
        {
            Get3x3ArrayIndex(x, y, z, out int id);
            AppendIdsOfCell(id, ref index3x3s);
        }

    }

    internal void AppendIdsOfCellAndAround(int i3x3, ref LinkedList<int> tmp)
    {
        Get3x3ArrayIndex(i3x3, out int x, out int y, out int z);
        AppendIdsOfCellAndAround(x, y, z, ref tmp);
        
    }



    //public void GetIndex(Vector3 position, out int x, int y, int z) { 

    //}

    public class IdToRef
    {
        public int m_objectId;
        public Vector3 m_localPosition;


        public IdToRef(int objectId, Vector3 position)
        {
            m_objectId = objectId;
            m_localPosition = position;
        }
        public void Reset()
        {
            m_objectId = -1;
            m_localPosition = Vector3.zero;
        }
    }
}


public struct LinkedId
{
    public int m_objectId;
    public int m_nextLinked;

    public bool IsDefine() { return m_objectId>-1; }
    public bool HasNextLink() { return m_nextLinked>-1; }

    public void Reset()
    {
        m_objectId = -1;
        m_nextLinked = -1;
    }
}


