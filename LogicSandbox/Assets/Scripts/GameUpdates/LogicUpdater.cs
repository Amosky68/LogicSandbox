using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public static class LogicMap
{
    public static Dictionary<Vector2Int, TileSprites> TileTextureMap = new();
    public static Dictionary<Vector2Int, GameObject> GameObjectMap = new();
    public static Dictionary<Vector2Int, dynamic> Map = new();

    public static List<WireNetwork> WireNetworksMap = new();
    private static int MaxNetworkId = 0;


    public static void PlaceObject(Vector2Int position, dynamic LogicObject, TileSprites ObjectTextures, GameObject gameObject)
    {
        if (LogicObject == null) { return;  }
        if (Map.ContainsKey(position)) {
            RemoveObject(position);
        }
        Map.Add(position, LogicObject);
        TileTextureMap.Add(position, ObjectTextures);
        GameObjectMap.Add(position, gameObject);

        if (LogicObject is Wire) { UpdateMapOnWireAdd(position, LogicObject); }

    }
    public static void RemoveObject(Vector2Int position)
    {
        if (Map.ContainsKey(position))
        {
            dynamic wireObj;
            if (Map.TryGetValue(position, out wireObj)) { 
                if (wireObj is Wire) { UpdateMapOnWireRemove(position); } }

            Map.Remove(position);
            TileTextureMap.Remove(position);
            GameObject gameObject;
            if (GameObjectMap.TryGetValue(position, out gameObject)) {
                GameObject.Destroy(gameObject);
            }
            GameObjectMap.Remove(position);
        }
    }
    public static dynamic GetObject(Vector2Int position) {
        dynamic obj;
        if (Map.TryGetValue(position, out obj)) {
            return obj;
        }
        return null;
    }


    public static WireNetwork GetNewWireNetwork(bool active = false)
    {
        WireNetwork newWireNetwork = new WireNetwork(active, MaxNetworkId);
        MaxNetworkId++;
        WireNetworksMap.Add(newWireNetwork);
        return newWireNetwork;
    }

    // needs to be called before the wire is removed
    public static void UpdateMapOnWireRemove(Vector2Int position)
    {
        // Algorithm description :
        // Create a list of the possible new network (adjacent wire form the removed one)
        // for each of thoses possible network, propagate it to the next wires

        // Get the wire at position "Vector2Int position"
        dynamic objMap;
        Wire InitialWire;
        if (Map.TryGetValue(position, out objMap)) {
            if (objMap is Wire) { InitialWire = objMap;  }
            else { throw new Exception("UpdateMapOnWireRemove error - objMap is not a Wire "); }
        } else { throw new Exception("UpdateMapOnWireRemove error - no LogicObject at " + position); }


        List<List<Wire>> toSearch = new List<List<Wire>>();
        Dictionary<Vector2Int, bool> alreadySearchedTiles = new Dictionary<Vector2Int, bool>();
        WireNetworksMap.Remove(InitialWire.network);


        // add adjescent wires to the search list
        foreach (var nextWire in InitialWire.GetAdjacentWires()) {
            toSearch.Add(new List<Wire> { nextWire });
        }


        // iterates over all possible new networks
        for (int NetworkIndex = 0; NetworkIndex < toSearch.Count; NetworkIndex++) {
            WireNetwork network = GetNewWireNetwork();


            // spreads the network over all wires
            while (toSearch[NetworkIndex].Count > 0) {
                if (alreadySearchedTiles.ContainsKey(toSearch[NetworkIndex][0].position)) { continue; }

                toSearch[NetworkIndex][0].network = network;
                alreadySearchedTiles.Add(toSearch[NetworkIndex][0].position, true);
                toSearch[NetworkIndex].RemoveAt(0);
                Wire currentWire = toSearch[NetworkIndex][0];

                foreach (Wire wire in currentWire.GetAdjacentWires()) {
                    if (!alreadySearchedTiles.ContainsKey(wire.position)) {
                        toSearch[NetworkIndex].Add(wire);
                    }
                }
            }
        } 
    }

    // needs to be called after the wire is placed
    public static void UpdateMapOnWireAdd(Vector2Int position, Wire wire)
    {
        // Algorithm description :
        // check if all of the new adjacent wire's network is the same
        // If it is not, collapse all the networks into one 


        WireNetwork newWireNetwork = GetNewWireNetwork();

        // check if all of the new adjacent wire's network is the same
        List<Wire> adjacentWires = wire.GetAdjacentWires();
        List<WireNetwork> adjacentWiresNetworks = new();
        foreach (Wire adjWire in adjacentWires) { adjacentWiresNetworks.Add(wire.network);  }

        if (adjacentWires.Count == 0) { wire.network = newWireNetwork; return; }        
        if (adjacentWiresNetworks.Any(o => o != adjacentWiresNetworks[0])) { wire.network = adjacentWires[0].network;  }
        // if all the networks are the same 
        

        else // if two networks are different
        {
            foreach (WireNetwork nwk in adjacentWiresNetworks) {   
                if (WireNetworksMap.Contains(nwk)) { WireNetworksMap.Remove(nwk); }
            }

            List<Wire> toSearch = new();
            Dictionary<Vector2Int, bool> alreadySearchedTiles = new Dictionary<Vector2Int, bool>();
            toSearch = adjacentWires;

            while (toSearch.Count > 0) {
                Wire searchWire = toSearch[0];
                toSearch.RemoveAt(0);

                if (alreadySearchedTiles.ContainsKey(searchWire.position)) { continue; }
                searchWire.network = newWireNetwork;
                alreadySearchedTiles.Add(searchWire.position, true);

                foreach (Wire nextWire in searchWire.GetAdjacentWires()) {
                    toSearch.Add(nextWire); 
                }
            }
        }
    }
}





public class LogicUpdater : MonoBehaviour
{

    public List<LogicObjects> lobj = new List<LogicObjects>();



    private void Start()
    {
        TickSystem.OnTickUpdate += delegate (object sender, TickSystem.OnTickUpdateArgs ags)
        {
            OnTickUpdate();
            RenderMap();
        };

    }

    void OnTickUpdate()
    {
        Dictionary<Vector2Int, dynamic> NewMap = new Dictionary<Vector2Int, dynamic>();
        foreach (dynamic logicObject in LogicMap.Map.Values) 
        {
        }
    }

    private void RenderMap()
    {
        Vector2Int _objectPosition;
        foreach (KeyValuePair<Vector2Int, dynamic> logicObject in LogicMap.Map) {
            _objectPosition = logicObject.Key;

            TileSprites _tSprite;
            if (LogicMap.TileTextureMap.TryGetValue(_objectPosition, out _tSprite)) 
            {   Sprite _sprite = logicObject.Value.IsActive() ? _tSprite.Active : _tSprite.Inactive;

                GameObject _gameObject;
                if (LogicMap.GameObjectMap.TryGetValue(_objectPosition, out _gameObject)) {
                    SpriteRenderer spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _sprite;
                }

            }
            else { print(" No value for TileTextureMap at " + _objectPosition + LogicMap.TileTextureMap.Count);  }
        }
    }
}
