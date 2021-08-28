using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public class IdClaimedToStringIdRegister {


    public Dictionary<int, string> m_idToNameIdJoin = new Dictionary<int, string>();
    public Dictionary<string,int> m_nameToIdJoin = new Dictionary<string,int >();

  

    //public void ClaimOverride(int id, string idName) {
    //    if (!m_idToNameIdJoin.ContainsKey(id))
    //    {
    //        m_idToNameIdJoin.Add(id, idName);
    //    }
    //    else m_idToNameIdJoin[id] = idName;
    //    if (!m_nameToIdJoin.ContainsKey(idName))
    //    {
    //        m_nameToIdJoin.Add(idName, id);
    //    }
    //    else m_nameToIdJoin[idName] = id;
    //}

    //public void ClaimNewOrFreeId(string idName, out int id)
    //{
        

    //    if (!m_idToNameIdJoin.ContainsKey(id))
    //    {
    //        m_idToNameIdJoin.Add(id, idName);
    //    }
    //    else m_idToNameIdJoin[id]= idName;
    //    if (!m_nameToIdJoin.ContainsKey(idName))
    //    {
    //        m_nameToIdJoin.Add(idName, id);
    //    }
    //    else m_nameToIdJoin[idName] = id;

    //}


    //public void Unclaim(int id)
    //{
    //    if (m_idToNameIdJoin.ContainsKey(id))
    //    {
    //        string name = m_idToNameIdJoin[id];
    //        m_nameToIdJoin.Remove(name);
    //    }
    //    m_idToNameIdJoin.Remove(id);

    //}
}
