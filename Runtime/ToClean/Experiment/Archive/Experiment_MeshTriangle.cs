//using System;
//using System.Collections.Generic;
//using System.Linq;

//[System.Serializable]
//public struct STRUCT_ItemObjectDistance : IComparable<STRUCT_ItemObjectDistance>, IOrderedEnumerable<STRUCT_ItemObjectDistance>
//{
//    public int CompareTo(STRUCT_ItemObjectDistance other)
//    {
//        // Compare the distances
//        int distanceComparison = m_distance.CompareTo(other.m_distance);

//        return distanceComparison;
//    }

//    public int m_index;
//    public float m_distance;

//    public IOrderedEnumerable<STRUCT_ItemObjectDistance> CreateOrderedEnumerable<TKey>(Func<STRUCT_ItemObjectDistance, TKey> keySelector, IComparer<TKey> comparer, bool descending)
//    {
//        return descending ? this.OrderByDescending(keySelector, comparer) : this.OrderBy(keySelector, comparer);
//    }
//}
