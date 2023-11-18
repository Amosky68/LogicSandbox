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

[System.Serializable]
public class WireNetwork
{
    public Dictionary<Vector2Int, LogicalWire> Wires = new Dictionary<Vector2Int, LogicalWire>();
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

    public void AddWire(LogicalWire wire)
    {
        if (wire == null) { return; }

        Wires.Add(wire.position, wire);
    }
}


// LogicObjects

interface LogicObjectsInterface
{
    void OnObjectUpdate(GlobalMap Map);
    public bool IsActive();
    public void SetAfterUpdateActivation(bool activation);
    public void AfterUpdate();

}

public class LogicObjects
{
    public bool afterUpdateActivation = false;

    public int orientation = 0;
    public List<int> inputsSides = new List<int>();
    public List<int> outputSides = new List<int>();
    public Vector2Int position = Vector2Int.zero;
    public List<Vector2Int> inputConnections = new();
    public List<Vector2Int> outputConnections = new();

    public LogicObjects()
    {
    }

    public void CalculateConnections()
    {
        inputConnections.Clear();
        outputConnections.Clear();

        Vector2Int[] pos = new Vector2Int[4]
        {
            position + new Vector2Int(0, 1),
            position + new Vector2Int(1, 0),
            position + new Vector2Int(0, -1),
            position + new Vector2Int(-1, 0)
        };

        foreach (int side in inputsSides)
        {
            int id = (side - orientation) % 4;   // (-) because the orientation sides is reversed
            Vector2Int connectedPosition = pos[id];
            inputConnections.Add(connectedPosition);
        }

        foreach (int side in outputSides)
        {
            int id = (side - orientation) % 4;   // (-) because the orientation sides is reversed
            Vector2Int connectedPosition = pos[id];
            outputConnections.Add(connectedPosition);
        }
    }
}


# region Objects Class

public class LogicalWire : LogicObjects, LogicObjectsInterface
{
    public WireNetwork network;
    public LogicalWire(Vector2Int _position, int _orientation, List<int> _connectedSides, WireNetwork _network = null)
    {
        network = _network;
        orientation = _orientation;
        inputsSides = _connectedSides;
        position = _position;
        CalculateConnections();
    }


    public bool IsActive()
    {
        return network.isActivated;
    }
    public void SetAfterUpdateActivation(bool activation) {
        afterUpdateActivation = activation;
    }
    public void SetNetworkActive(bool active)
    {
        network.isActivated |= active;
    }

    public void OnObjectUpdate(GlobalMap Map) { } // wires don't updates
    public void AfterUpdate() {
        SetNetworkActive(afterUpdateActivation);
        afterUpdateActivation = false;
    }

    public List<LogicalWire> GetAdjacentWires(LogicMap _LogicMap)
    { 
        List<LogicalWire> outlist = new List<LogicalWire>(); // List of output

        foreach (Vector2Int connectedPosition in inputConnections)  { // look at all adjacent tiles
            if (_LogicMap.GlMap.Map.TryGetValue(connectedPosition, out object adjWire)) {
                if (adjWire.GetType() == typeof(LogicalWire)) {
                    LogicalWire nextwire = (LogicalWire) adjWire;
                    
                    if (nextwire.inputConnections.Contains(position)) { outlist.Add(nextwire); }
                }     
            }
        }
        return outlist;
    }
}

// Not implemented
public class LogicalClock : LogicObjects , LogicObjectsInterface
{
    void LogicObjectsInterface.OnObjectUpdate(GlobalMap Map) 
    {
        
    }

    public void SetAfterUpdateActivation(bool activation)
    {
        afterUpdateActivation = activation;
        afterUpdateActivation = false;
    }

    public void AfterUpdate() { }
    bool LogicObjectsInterface.IsActive() { return true; }

}


public class LogicalInverter : LogicObjects, LogicObjectsInterface
{
    public bool isActive = false;

    public LogicalInverter(Vector2Int _position, int _orientation, List<int> _inputSides, List<int> _outputSides)
    {
        orientation = _orientation;
        inputsSides = _inputSides;
        outputSides = _outputSides;
        position = _position;
        CalculateConnections();
    }

    public void OnObjectUpdate( GlobalMap Map)
    {
        if (Map.Map.TryGetValue(inputConnections[0], out object inputWire)) {
            if (inputWire.GetType() == typeof(LogicalWire)) {
                LogicalWire wire = (LogicalWire)inputWire;
                isActive = wire.IsActive();
            }
        }

        if (Map.Map.TryGetValue(outputConnections[0], out object outputWire))
        {
            if (outputWire.GetType() == typeof(LogicalWire))
            {
                LogicalWire wire = (LogicalWire)outputWire;
                wire.SetAfterUpdateActivation(!isActive);
            }
        }
    }
    public bool IsActive()
    {
        return isActive;
    }
    public void SetAfterUpdateActivation(bool activation)
    {
        afterUpdateActivation = activation;
    }
    public void AfterUpdate() {
        isActive |= afterUpdateActivation;
        afterUpdateActivation = false;
    }

}


// Not implemented
public class LogicalBridger : LogicObjects, LogicObjectsInterface
{
    void LogicObjectsInterface.OnObjectUpdate(GlobalMap Map)
    {

    }

    public void SetAfterUpdateActivation(bool activation)
    {
        afterUpdateActivation = activation;
        afterUpdateActivation = false;
    }

    public void AfterUpdate() { }

    bool LogicObjectsInterface.IsActive() { return true; }

}


// Not implemented
public class LogicalAnd : LogicObjects, LogicObjectsInterface
{
    void LogicObjectsInterface.OnObjectUpdate(GlobalMap Map)
    {

    }

    public void SetAfterUpdateActivation(bool activation)
    {
        afterUpdateActivation = activation;
        afterUpdateActivation = false;
    }

    public void AfterUpdate() { }

    bool LogicObjectsInterface.IsActive() { return true; }

}


#endregion