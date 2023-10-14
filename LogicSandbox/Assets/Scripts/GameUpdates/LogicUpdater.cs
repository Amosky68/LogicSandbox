using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class LogicMap
{
    public static Dictionary<Vector2Int, dynamic> Map = new Dictionary<Vector2Int, dynamic>();


    public static void PlaceObject(int x, int y , dynamic LogicObject)
    {
        if (LogicObject != null)
        {
            Map.Add((x, y), LogicObject);
        }
    }
    public static object GetObject(int x , int y) {
        return Map[(x, y)];
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
