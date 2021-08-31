using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;





public  class SeaweedMatrixSpliterMono : MonoBehaviour
{

    public float m_bulletMaxSpeedPerFrame=10;
    public Transform m_center;
    public Transform m_root;
    public DataFetcher m_dataFetcher;
    public Texture2D m_topViewDebug;


    public void SetDataFetcher(DataFetcher dataFetcher) {
        m_dataFetcher = dataFetcher;
    }

    public void GetRandomPositionInMatrix(out Vector3 localPosition)
    {
        float edgeSize = matrixSplite.m_spliter.m_matrixSquareEdgeSize;
        localPosition = new Vector3(UnityEngine.Random.Range(0, edgeSize), UnityEngine.Random.Range(0, edgeSize), UnityEngine.Random.Range(0, edgeSize));
    }

    public Renderer m_renderer;

    public SeaweedMatrixSpliterJobSystem matrixSplite;

    public Transform m_groundDebug;
    public bool m_useDebugDraw;

    //Max theroric=666
    //max tip =300;
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

        InvokeRepeating("Debug", 0.1f, 0.1f);
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
        //colorPerCells = new Color[m_matrixDim * m_matrixDim * m_matrixDim];
        //for (int i = 0; i < colorPerCells.Length; i++)
        //{
        //    float r, g, b;
        //    r = UnityEngine.Random.Range(0, 3) % 3 == 0 ? UnityEngine.Random.Range(0.3f, 1f) : 0f;
        //    g = UnityEngine.Random.Range(0, 3) % 3 == 0 ? UnityEngine.Random.Range(0.3f, 1f) : 0f;
        //    b = UnityEngine.Random.Range(0, 3) % 3 == 0 ? UnityEngine.Random.Range(0.3f, 1f) : 0f;
        //    colorPerCells[i] =new Color(r,g,b);

        //}
        DrawCellColorCouldZone();
        //DrawCellPathCouldZone();
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
        if (!m_useDebugDraw)
            return;
        DrawDebugZone();

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
       // DrawCellPathCouldZone();

        int[] cellsId3x3 = new int[matrixSplite.m_spliter.m_dimensionAsInt];


        matrixSplite.m_spliter.GetXZCells(0, 0, ref cellsId3x3);
       // UnityEngine.Debug.Log("TT2 " + string.Join(" ", cellsId3x3));
        DrawColomn(ref cellsId3x3, Color.green, 5);


        matrixSplite.m_spliter.GetXYCells(0, 0, ref cellsId3x3);
        // UnityEngine.Debug.Log("TT1 " + string.Join(" ", cellsId3x3));
        DrawColomn(ref cellsId3x3, Color.blue, 5);


        matrixSplite.m_spliter.GetYZCells(0, 0, ref cellsId3x3);
        //UnityEngine.Debug.Log("TT3 " + string.Join(" ", cellsId3x3));
        DrawColomn(ref cellsId3x3, Color.red, 5);


    }

    internal void RelocatedFromCenter(Vector3 localPosition, out Vector3 worldPosition)
    {
        worldPosition = m_center.position + m_root.rotation * localPosition;
    }

    internal void RelocatedFromStartCorner(Vector3 localPosition, out Vector3 worldPosition)
    {
        worldPosition = m_root.position + m_root.rotation * localPosition;
    }

   

    public void DrawSquare(int i3x3, Color color, float timeInMs)
    {
        matrixSplite.m_spliter.Get3x3ArrayIndex(i3x3, out int x, out int y, out int z);
        float fullSize = matrixSplite.m_spliter.m_matrixSquareEdgeSize;
        float cellSize = fullSize / (float)matrixSplite.m_spliter.m_dimensionAsInt;
        Vector3 cellStart = new Vector3(
            x * cellSize + cellSize ,
            y * cellSize + cellSize  ,
            z * cellSize + cellSize  
            ); ; 
        Vector3 cellEnd= new Vector3(
            x * cellSize + cellSize * 1f,
            y * cellSize + cellSize * 1f,
            z * cellSize + cellSize * 1f
            ); ;

        UnityEngine.Debug.DrawLine(
            m_root.position + Quaternion.Inverse(m_root.rotation) * cellStart,
            m_root.position + Quaternion.Inverse(m_root.rotation) * cellEnd,
            color, timeInMs
            );


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
            
            UnityEngine.Debug.DrawLine(s, e, color);
            
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
                UnityEngine.Debug.DrawLine(pts[y], pts[x], Color.white);
            }
        }




    }
    private void DrawCellColorCouldZone()
    {
        if (!m_useDebugDraw)
            return;
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
                    UnityEngine.Debug.DrawLine(v - Vector3.up * 0.25f, v + Vector3.up * 0.25f, on ? Color.green : Color.red * 0.2f);
                }
            }
        }
    }

    public enum PathType { Cell, CellAndNearCells}
    public PathType m_pathType;
    //private Color[] colorPerCells;
    private void DrawCellPathCouldZone()
    {

        if (!m_useDebugDraw)
            return;
        if (m_dataFetcher == null)
            return;
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
                            UnityEngine.Debug.DrawLine(v - Vector3.up * 0.25f, v + Vector3.up * 0.25f, Color.blue);

                        }
                        else if (tmp.Count > 1) {


                            int objectId=0;
                            Vector3 s=Vector3.zero, e = Vector3.zero;
                            int si, se;
                            for (int i = 0; i < tmp.Count-1; i++)
                            {
                                si = tmp.ElementAt(i);
                                se = tmp.ElementAt(i + 1);
                                    UnityEngine.Debug.Log(string.Format("Path {0} > {1}", si, se));
                                if (si > -1 && se > -1)
                                {
                                    m_dataFetcher.GetInfoAtId(ref si, ref objectId, ref s);

                                    //UnityEngine.Debug.DrawLine(s, e,
                                    //    colorPerCells[i3x3], 5);
                                    UnityEngine.Debug.DrawLine(s, e,
                                        Color.black*0.5f);
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


public interface DataFetcher
{
    public int GetCollectionCount();
    public void GetInfoAtId(ref int id, ref int objectId, ref Vector3 position);
}