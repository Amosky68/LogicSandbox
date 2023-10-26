using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;


// Basic Orientations :
//
//            side 0
//        +-------------+
//        |             |
//        |             |
// side 3 |    Tile     |  side 1
//        |             |
//        |             |
//        |             |
//        +-------------+
//            side 2






public class LogicObjects
{

    public int orientation = 0;
    public List<int> inputsSides = new List<int>();
    

}


public class WireNetwork
{
    public Dictionary<Vector2Int, Wire> Wires = new Dictionary<Vector2Int, Wire>();
    public bool isActivated = false;


    public void Activate()
    {
        if (isActivated) { return; }
        isActivated = true;
        foreach (Wire wire in Wires.Values)
        {
            wire.isActivated = true;
        }
    }
    public void Deactivate()
    {
        if (!isActivated) { return; }
        isActivated = false;
        foreach (Wire wire in Wires.Values)
        {
            wire.isActivated = false;
        }
    }

    public void AddWire(Wire wire)
    {
        if (wire == null) { return; }

        Wires.Add(wire.position, wire);
        wire.isActivated = isActivated;
    }


    public void UpdateNetwork(Wire InitialWire)
    {
        Wires.Clear();
        isActivated = false;
        Dictionary<Vector2Int, bool> alreadySearchedTiles = new Dictionary<Vector2Int, bool>();
        List<Wire> toSearch = new List<Wire>() { InitialWire };


        while (toSearch.Count > 0) {
            alreadySearchedTiles.Add(toSearch[0].position, true);
            Wire currentWire = toSearch[0];;
            toSearch.RemoveAt(0);

            foreach (Wire wire in currentWire.GetAdjacentWires()) {
                if (!alreadySearchedTiles.ContainsKey(wire.position)) {
                    toSearch.Add(wire);
                }
            }

            Wires.Add(currentWire.position, currentWire);
            isActivated |= currentWire.isActivated; 

        }
    }
}


public class Wire : LogicObjects
{
    public int networkID = 0;
    public bool isActivated = false;

    public Vector2Int position = Vector2Int.zero;


    public Wire(int _orientation, List<int> connectedSides, int wireNetworkID = 0)
    {
        networkID = wireNetworkID;
        orientation = _orientation;
        inputsSides = connectedSides;
    }

    public bool IsActive()
    {
        return isActivated;
    }

    // obselete !
    public List<Wire> GetAdjacentWires()
    { 
        Vector2Int[] pos = new Vector2Int[4]
        {
            position + new Vector2Int(1, 0),
            position + new Vector2Int(0, 1),
            position + new Vector2Int(-1, 0),
            position + new Vector2Int(0, -1)
        };


        List<Wire> outlist = new List<Wire>(); // List of output

        dynamic wire;
        foreach (Vector2Int positions in pos) { // look at all adjacent tiles
            if (LogicMap.Map.TryGetValue(positions, out wire)) { 
            if (wire is Wire) { outlist.Add(wire); } // if it contains a wire
            }
        }

        return outlist;
    }
}



public class Delayer : LogicObjects
{
        public Vector2Int position = Vector2Int.zero;

}
