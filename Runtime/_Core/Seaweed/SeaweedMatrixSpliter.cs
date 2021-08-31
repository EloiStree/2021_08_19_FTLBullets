using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public struct SeaweedMatrixSpliter
{
    public uint m_dimension;
    public int m_dimensionAsInt;
    public uint m_elementCount;
    NativeArray<int> m_matrixCells;
    NativeArray<int> m_matrixCellsSeaweedHead;
    NativeArray<LinkedId> m_elements;
    public float m_matrixSquareEdgeSize;
    public float m_matrixHalfSquareEdgeSize;

    private bool m_isInit;

    public void ResetAll()
    {
        for (int i = 0; i < m_matrixCells.Length; i++)
        {
            m_matrixCells[i] = -1;
            m_matrixCellsSeaweedHead[i] = -1;
        }
        for (int i = 0; i < m_elements.Length; i++)
        {
            m_elements[i] = new LinkedId() { m_nextLinked = -1, m_objectId = -1 };
        }
    }


    public void Init(uint dimensionOfSplit = 128, uint idMaxCount = 10000, float matrixSquareEdgeSize = 256)
    {
        this.m_dimension = dimensionOfSplit;
        m_dimensionAsInt = (int)dimensionOfSplit;
        m_elementCount = dimensionOfSplit * dimensionOfSplit * dimensionOfSplit;
        m_matrixSquareEdgeSize = matrixSquareEdgeSize;
        m_matrixHalfSquareEdgeSize = matrixSquareEdgeSize * 0.5f;

        int[] matrixData = new int[m_elementCount];
        m_matrixCells = new NativeArray<int>(matrixData, Allocator.Persistent);

        matrixData = new int[m_elementCount];
        m_matrixCellsSeaweedHead = new NativeArray<int>(matrixData, Allocator.Persistent);

        RedefineSizeOfElementCount((int)idMaxCount);

       


        ResetAll();
        m_isInit = true;
    }

    private bool m_sizeWasDefined;
    public void RedefineSizeOfElementCount(int sizeMaxElements) {

        if (m_sizeWasDefined)
            m_elements.Dispose();

        LinkedId[] matrixElements = new LinkedId[sizeMaxElements];
        m_elements = new NativeArray<LinkedId>(matrixElements, Allocator.Persistent);
        m_sizeWasDefined = true;

        for (int i = 0; i < m_elementCount; i++)
        {

            m_matrixCells[i] = m_matrixCellsSeaweedHead[i] = -1;
        }
    }


    public void Dispose()
    {
        if (m_isInit)
        {
            m_matrixCells.Dispose();
            m_matrixCellsSeaweedHead.Dispose();
            m_elements.Dispose();
        }
    }

    public void SetIdArrayValue(int elementId, int value)
    {
        LinkedId linkedId = m_elements[elementId];
        linkedId.m_objectId = value;
        m_elements[elementId] = linkedId;


    }

    public void AddIdToSeaweedList(int matrix3x3Id, int elementIndex)
    {

        if (IsMatrixCellEmpty(matrix3x3Id))
        {
            m_matrixCells[matrix3x3Id] = elementIndex;
            m_matrixCellsSeaweedHead[matrix3x3Id] = elementIndex;

            //Debug.Log(string.Format("create {0}->{1}", matrix3x3Id, elementIndex));
        }
        else
        {
            int previousId = m_matrixCellsSeaweedHead[matrix3x3Id];
            LinkedId element = m_elements[previousId];
            element.m_nextLinked = elementIndex;
            m_elements[previousId] = element;
            m_matrixCellsSeaweedHead[matrix3x3Id] = elementIndex;
            //Debug.Log(string.Format("add m {0}  p{2} ->n {1}", matrix3x3Id, elementIndex, previousId));
        }
    }

    public bool IsMatrixCellEmpty(int matrix3x3Id)
    {
        return m_matrixCells[matrix3x3Id] < 0;
    }



    public void CompteAllCellIn(ref int[] array)
    {
        if (array.Length != m_elementCount)
            throw new ArgumentOutOfRangeException("You should provide an array with the same size of the matrix spliter (dimension):" + m_dimension);

        LinkedList<int> ids = new LinkedList<int>();
        for (int i = 0; i < array.Length; i++)
        {
            int startCount = ids.Count;
            AppendIdsOfCell(i, ref ids);
            array[i] = ids.Count - startCount;
            //if (array[i] > 0)
            //    Debug.Log(string.Format("count m {0}  -> {1}", i, array[i]));
        }
        ids.Clear();
    }
    public int GetCount(int matrix3x3Id)
    {
        LinkedList<int> ids = new LinkedList<int>();
        AppendIdsOfCell(matrix3x3Id, ref ids);
        return ids.Count;
    }


    public void AppendIdsOfCell(int matrix3x3Id, ref LinkedList<int> ids)
    {
        if (ids == null)
            ids = new LinkedList<int>();
        int i = m_matrixCells[matrix3x3Id];
        if (i < 0)
            return;
        //Debug.Log(string.Format("C> m{0} i{1}", matrix3x3Id, i));

        int antiloop = 0;

        do
        {
            // Debug.Log(string.Format("N> pi{0} ni{1}", i, m_elements[i].m_nextLinked));
            ids.AddLast(m_elements[i].m_objectId);
            i = m_elements[i].m_nextLinked;
            antiloop++;
            if (antiloop > (m_matrixCells.Length)) {
                Debug.LogWarning("Loop present in your code:" +
                    string.Join( ">", ids ));
                //throw new Exception("Loop present in your code");
                break;
            }
        } while (i > -1);


    }

    internal void DrawSquare(int i3x3)
    {
        throw new NotImplementedException();
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

    public void PushInPrecompute(NativeBoolLayerMask layer, NativeArray<ThreadComputedPosition> matrixData)
    {
        ResetAll();
        for (int i = 0; i < layer.m_size; i++)
        {
            if (layer.IsTrue(ref i))
            {
                ThreadComputedPosition data = matrixData[i];
                PushInPrecompute(ref data.m_arrayPosition, ref data.m_matrix3x3Id, ref data.m_objectId);
            }
        }
    }

    public void PushInPrecompute(ref int arrayPosition, ref int matrix3x3Id, ref int objectId)
    {

        SetIdArrayValue(arrayPosition, objectId);
        AddIdToSeaweedList(matrix3x3Id, arrayPosition);
    }


    public struct ThreadComputedPosition
    {
        public int m_arrayPosition;
        public int m_matrix3x3Id;
        public int m_objectId;
    }
    public void ComputePositionOf(ref Vector3 position, ref int matrix3x3Id, ref bool isInZone)
    {

        int x, y, z;
        isInZone = (Mathf.Abs(position.x) <= m_matrixSquareEdgeSize
            && Mathf.Abs(position.y) <= m_matrixSquareEdgeSize
            && Mathf.Abs(position.z) <= m_matrixSquareEdgeSize
            && Mathf.Abs(position.x) >= 0f
            && Mathf.Abs(position.y) >= 0f
            && Mathf.Abs(position.z) >= 0f);
        if (isInZone)
        {
            position /= m_matrixSquareEdgeSize;

            x = (int)((position.x * 0.5f + 0.499999f) * m_dimension);
            y = (int)((position.y * 0.5f + 0.499999f) * m_dimension);
            z = (int)((position.z * 0.5f + 0.499999f) * m_dimension);
            Get3x3ArrayIndex(x, y, z, out matrix3x3Id);

        }
        else matrix3x3Id = -1;
    }

    public void PushIn(IdToRef[] worldPositionV3)
    {
        ResetAll();
        //int elementIndex=0;

        int x, y, z;
        Vector3 v3 = Vector3.zero;
        bool isInZone = false;
        int i3x3 = -1;
        int objectId;
        for (int i = 0; i < worldPositionV3.Length; i++)
        {
            objectId = worldPositionV3[i].m_objectId;
            v3 = worldPositionV3[i].m_localPosition;
            ComputePositionOf(ref v3, ref i3x3, ref isInZone);
            if (isInZone)
                PushInPrecompute(ref i, ref i3x3, ref objectId);


            //Old version
            //if (Mathf.Abs(v3.x) < m_matrixSquareEdgeSize
            //    && Mathf.Abs(v3.y) < m_matrixSquareEdgeSize
            //    && Mathf.Abs(v3.z) < m_matrixSquareEdgeSize 
            //    && Mathf.Abs(v3.x) >= 0f
            //    && Mathf.Abs(v3.y) >= 0f
            //    && Mathf.Abs(v3.z) >= 0f)
            //{
            //    v3 /= m_matrixSquareEdgeSize;

            //    x = (int)((v3.x ) * m_dimension);
            //    y = (int)((v3.y ) * m_dimension);
            //    z = (int)((v3.z ) * m_dimension);
            //    Get3x3ArrayIndex(x, y, z, out int i3x3);


            //    SetIdArrayValue(i, worldPositionV3[i].m_objectId);
            //    AddIdToSeaweedList(i3x3,i);


            //    elementIndex++;
            //    if (elementIndex >= m_elementCount)
            //        break;
        }

    }





    public void Get3x3ArrayIndex(int x, int y, int z, out int index3x3)
    {
        index3x3 = x * (m_dimensionAsInt * m_dimensionAsInt) + y * (m_dimensionAsInt) + z;

    }
    public void Get3x3ArrayIndex(int index3x3, out int x, out int y, out int z)
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

    private void AppendIdsOfCellsProtected(int x, int y, int z, ref LinkedList<int> index3x3s)
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

