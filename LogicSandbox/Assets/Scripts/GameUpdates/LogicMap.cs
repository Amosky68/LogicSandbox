using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;




[System.Serializable]
public class GlobalMap : ICloneable
{
    public Dictionary<Vector2Int, TileSprites> TileTextureMap = new();
    public Dictionary<Vector2Int, GameObject> GameObjectMap = new();
    public Dictionary<Vector2Int, object> Map = new();

    [SerializeField] public List<WireNetwork> WireNetworksMap = new();
    public int MaxNetworkId = 0;


    public object Clone()
    {
        GlobalMap cloneMap = new GlobalMap();
        cloneMap.TileTextureMap = TileTextureMap.ToDictionary(entry => entry.Key, entry => entry.Value);
        cloneMap.GameObjectMap = GameObjectMap.ToDictionary(entry => entry.Key, entry => entry.Value);
        cloneMap.Map = Map.ToDictionary(entry => entry.Key, entry => entry.Value);
        cloneMap.WireNetworksMap = new();

        foreach (WireNetwork wnet in WireNetworksMap) {
            cloneMap.WireNetworksMap.Add( new WireNetwork(false, wnet.Id) );
        }

        cloneMap.MaxNetworkId = MaxNetworkId;
        return cloneMap;
    }
}






[System.Serializable]
public class LogicMap : MonoBehaviour
{
    public GlobalMap GlMap = new GlobalMap();


    private static LogicMap _instance;
    private void Awake()
    {
        if (_instance == null) { _instance = this; }
        DontDestroyOnLoad(gameObject);
    }


    public void PlaceObject(Vector2Int position, object LogicObject, TileSprites ObjectTextures, GameObject gameObject)
    {
        if (LogicObject == null) { return; }
        if (GlMap.Map.ContainsKey(position))
        {
            RemoveObject(position);
        }
        GlMap.Map.Add(position, LogicObject);
        GlMap.TileTextureMap.Add(position, ObjectTextures);
        GlMap.GameObjectMap.Add(position, gameObject);

        if (LogicObject.GetType() ==  typeof(LogicalWire)) { UpdateMapOnWireAdd(position, (LogicalWire)LogicObject); }

    }
    public void RemoveObject(Vector2Int position)
    {
        if (GlMap.Map.ContainsKey(position))
        {
            dynamic wireObj;
            if (GlMap.Map.TryGetValue(position, out wireObj))
            {
                if (wireObj is LogicalWire) { UpdateMapOnWireRemove(position); }
            }

            GlMap.Map.Remove(position);
            GlMap.TileTextureMap.Remove(position);
            GameObject gameObject;
            if (GlMap.GameObjectMap.TryGetValue(position, out gameObject))
            {
                GameObject.Destroy(gameObject);
            }
            GlMap.GameObjectMap.Remove(position);
        }
    }
    public dynamic GetObject(Vector2Int position)
    {
        dynamic obj;
        if (GlMap.Map.TryGetValue(position, out obj))
        {
            return obj;
        }
        return null;
    }


    #region Wires and Networks
    public WireNetwork GetNewWireNetwork(bool active = false)
    {
        WireNetwork newWireNetwork = new WireNetwork(active, GlMap.MaxNetworkId);
        GlMap.MaxNetworkId++;
        GlMap.WireNetworksMap.Add(newWireNetwork);
        return newWireNetwork;
    }
    public void UpdateWireNetworksMap()  // Ne fonctionne pas pour le moment 
    {
        WireNetworkComparer ntwcomparer = new WireNetworkComparer();

        // clone and sort the networks list 
        List<WireNetwork> ToRemove = new();
        GlMap.WireNetworksMap.ForEach((item) => { ToRemove.Add(item);});
        ToRemove.Sort(ntwcomparer); // trie la liste par Id

        // remove all used networks from the remove list
        foreach (KeyValuePair< Vector2Int, object> obj in GlMap.Map) {
            
            if (obj.Value.GetType() == typeof(LogicalWire)) {
                
                LogicalWire wire = (LogicalWire)obj.Value;
                /* 
                int result = ToRemove.BinarySearch(wire.network, ntwcomparer);
                print("result : " + result);
                if (result > 0) { ToRemove.RemoveAt(result); print("removing .."); }
                else { print("wire.network.id : " + wire.network.Id); } 
                */
                try { ToRemove.Remove(wire.network); }
                catch { }
            }


        }

        // remove all unused networks
        foreach (WireNetwork ntw in ToRemove.ToList()) {
            GlMap.WireNetworksMap.Remove(ntw);
        }

    }

    // needs to be called before the wire is removed
    public void UpdateMapOnWireRemove(Vector2Int position)
    {
        // Algorithm description :
        // Create a list of the possible new network (adjacent wire form the removed one)
        // for each of thoses possible network, propagate it to the next wires

        // Get the wire at position "Vector2Int position"
        dynamic objMap;
        LogicalWire InitialWire;
        if (GlMap.Map.TryGetValue(position, out objMap))
        {
            if (objMap is LogicalWire) { InitialWire = objMap; }
            else { throw new Exception("UpdateMapOnWireRemove error - objMap is not a LogicalWire "); }
        }
        else { throw new Exception("UpdateMapOnWireRemove error - no LogicObject at " + position); }


        List<List<LogicalWire>> toSearch = new List<List<LogicalWire>>();
        Dictionary<Vector2Int, bool> alreadySearchedTiles = new Dictionary<Vector2Int, bool>();
        alreadySearchedTiles.Add(position, true);
        GlMap.WireNetworksMap.Remove(InitialWire.network);


        // add adjescent wires to the search list
        foreach (var nextWire in InitialWire.GetAdjacentWires(_instance)) {
            toSearch.Add(new List<LogicalWire> { nextWire });
        }

        //Debug.Log("------------- network collapse ------------- ");
        //Debug.Log("toSearch.Count on remove : " + toSearch.Count);
        // iterates over all possible new networks
        for (int NetworkIndex = 0; NetworkIndex < toSearch.Count; NetworkIndex++)
        {
            WireNetwork network = GetNewWireNetwork();

            // spreads the network over all wires
            while (toSearch[NetworkIndex].Count > 0)
            {
                LogicalWire currentWire = toSearch[NetworkIndex][0];
                toSearch[NetworkIndex].RemoveAt(0);            
                if (alreadySearchedTiles.ContainsKey(currentWire.position)) { continue; }
                currentWire.network = network;
                alreadySearchedTiles.Add(currentWire.position, true);
                //Debug.Log("Searched wire position : " + NetworkIndex + " | " + currentWire.position);

                foreach (LogicalWire wire in currentWire.GetAdjacentWires(_instance))
                {
                    if (!alreadySearchedTiles.ContainsKey(wire.position))
                    {
                        toSearch[NetworkIndex].Add(wire);
                    }
                }
            }
        }
        UpdateWireNetworksMap();
    }

    // needs to be called after the wire is placed
    public void UpdateMapOnWireAdd(Vector2Int position, LogicalWire wire)
    {
        // Algorithm description :
        // check if all of the new adjacent wire's network is the same
        // If it is not, collapse all the networks into one 

        print("UpdateMapOnWireAdd");

        
        // check if all of the new adjacent wire's network is the same
        List<LogicalWire> adjacentWires = wire.GetAdjacentWires(_instance);
        List<WireNetwork> adjacentWiresNetworks = new();
        foreach (LogicalWire adjWire in adjacentWires) { adjacentWiresNetworks.Add(wire.network); }

        // if the wire has no neighbours
        if (adjacentWires.Count == 0) { wire.network = GetNewWireNetwork(); return; }
        // if all the networks are the same (if any networks aren't different than thz first one) 
        if (adjacentWiresNetworks.Any(o => o != adjacentWiresNetworks[0])) { 
            wire.network = adjacentWires[0].network;
            return;
        }
        

        // if two networks are different
        //Debug.Log("------------- network collapse ------------- ");
        foreach (WireNetwork nwk in adjacentWiresNetworks) {
            if (GlMap.WireNetworksMap.Contains(nwk)) { GlMap.WireNetworksMap.Remove(nwk); print("removed " + nwk.Id); }
        }
        WireNetwork newWireNetwork = GetNewWireNetwork();


        List < LogicalWire> toSearch = new();
        Dictionary<Vector2Int, bool> alreadySearchedTiles = new Dictionary<Vector2Int, bool>();
        toSearch = adjacentWires;

        while (toSearch.Count > 0)
        {
            LogicalWire searchWire = toSearch[0];
            toSearch.RemoveAt(0);

            if (alreadySearchedTiles.ContainsKey(searchWire.position)) { continue; }
            //Debug.Log("Searched wire position : " + searchWire.position);
            searchWire.network = newWireNetwork;
            alreadySearchedTiles.Add(searchWire.position, true);

            foreach (LogicalWire nextWire in searchWire.GetAdjacentWires(_instance))
            {
                toSearch.Add(nextWire);
            }
        }

        UpdateWireNetworksMap();
    }

    #endregion
    #region Map work

    public GlobalMap GetMapCopy()
    {
        return (GlobalMap)GlMap.Clone();
    }

    #endregion
}
