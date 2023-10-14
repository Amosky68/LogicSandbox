using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public static class LogicMap
{
    public static Dictionary<Vector2Int, dynamic> Map = new Dictionary<Vector2Int, dynamic>();
    public static List<WireNetwork> List = new List<WireNetwork>();


    public static void PlaceObject(Vector2Int position, dynamic LogicObject)
    {
        if (LogicObject != null)
        {
            Map.Add(position, LogicObject);
        }
    }
    public static dynamic GetObject(Vector2Int position) {
        dynamic obj;
        if (Map.TryGetValue(position, out obj)) {
            return obj;
        }
        return null;
    }

}


public class LogicUpdater : MonoBehaviour
{
    public Color activatedMask = Color.white;
    public Color unactivatedMask = Color.black;
    public Tilemap logicTilemap;


    private void Start()
    {
        TickSystem.OnTickUpdate += delegate (object sender, TickSystem.OnTickUpdateArgs ags)
        {
            OnTickUpdate();
        };
    }


    void OnTickUpdate()
    {
        Dictionary<Vector2Int, dynamic> NewMap = new Dictionary<Vector2Int, dynamic>();
        foreach (dynamic logicObject in LogicMap.Map.Values) 
        {

        }
    }


    private void RenderWires()
    {
        if (logicTilemap == null) { return; }
        foreach (dynamic logicObject in LogicMap.Map.Values) {
            if (logicObject is Wire) 
            {
                Color displayMask = logicObject.isActivated ? activatedMask : unactivatedMask;
                logicTilemap.SetTileFlags(logicObject.position, TileFlags.None);
                logicTilemap.SetColor(logicObject.position, displayMask);
            }
        }
    }
}