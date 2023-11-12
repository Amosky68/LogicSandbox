using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;


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



public class WireNetworkComparer : IComparer<WireNetwork>
{
    public int Compare(WireNetwork x, WireNetwork y)
    {
        return x.Id > y.Id ? 1 : -1;
    }

    public static IComparer GetWireNetworkComparer()
    {
        return (IComparer)new WireNetworkComparer();
    }
}





public class LogicObjects
{

    public int orientation = 0;
    public List<int> inputsSides = new List<int>();
    

}


[System.Serializable]
public class WireNetwork  
{
    public Dictionary<Vector2Int, Wire> Wires = new Dictionary<Vector2Int, Wire>();
    public bool isActivated = false;
    public int Id = 0;

    public WireNetwork(bool isActivated, int id = 0)
    {
        this.isActivated = isActivated;
        Id = id;
    }


    public void Activate()
    {
        isActivated = true;
    }
    public void Deactivate()
    {
        isActivated = false;
    }

    public void AddWire(Wire wire)
    {
        if (wire == null) { return; }

        Wires.Add(wire.position, wire);
    }
}


public class Wire : LogicObjects
{
    public WireNetwork network;
    public Vector2Int position = Vector2Int.zero;


    public Wire(int _orientation, List<int> _connectedSides, WireNetwork _network = null)
    {
        network = _network;
        orientation = _orientation;
        inputsSides = _connectedSides;
    }

    public bool IsActive()
    {
        return network.isActivated;
    }


    public List<Vector2Int> GetConnections()
    {
        Vector2Int[] pos = new Vector2Int[4]
        {
            position + new Vector2Int(0, 1),
            position + new Vector2Int(1, 0),
            position + new Vector2Int(0, -1),
            position + new Vector2Int(-1, 0)
        };
        List<Vector2Int> connections = new();

        foreach (int side in inputsSides)
        { 
            int id = (side - orientation) % 4;   // (-) because the orientation sides is reversed
            Vector2Int connectedPosition = pos[id];
            connections.Add(connectedPosition);
        }

        return connections;
    }

    public List<Wire> GetAdjacentWires(LogicMap _LogicMap)
    { 
        


        List<Wire> outlist = new List<Wire>(); // List of output

        foreach (Vector2Int connectedPosition in GetConnections())  { // look at all adjacent tiles
            if (_LogicMap.Map.TryGetValue(connectedPosition, out object adjWire)) {
                if (adjWire.GetType() == typeof(Wire)) {
                    Wire nextwire = (Wire) adjWire;
                    
                    if (nextwire.GetConnections().Contains(position)) { outlist.Add(nextwire); }
                }     
            }
        }

        return outlist;
    }
}



public class Delayer : LogicObjects
{
        public Vector2Int position = Vector2Int.zero;

}
