using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class LogicMap
{
    public static Dictionary<Vector2Int, dynamic> Map = new Dictionary<Vector2Int, dynamic>();


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



    }
}
