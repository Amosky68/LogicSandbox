using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;



public static class LogicObjectsProperties 
{
    static WireNetwork wNetwork = new WireNetwork(false);
    static Vector2Int po = Vector2Int.zero;

    public static List<object> Objects = new List<object>()
    {
        new LogicalWire(po, 0 , new List<int> {0,1,2,3} , wNetwork) ,       // 4 connected wire
        new LogicalWire(po, 0 , new List<int> {1,2,3} , wNetwork) ,         // 3 connected wire
        new LogicalWire(po, 0 , new List<int> {0,2} , wNetwork) ,           // 2 connected wire
        new LogicalWire(po, 0 , new List<int> {1,2} , wNetwork) ,           // 2 connected wire with right angle
        new LogicalInverter(po, 0 , new List<int> {3}, new List<int> {1}),  // Basic LogicalInverter
        new LogicalBridger(po) ,                                            // Basic Bridger (1 tick delay)
    };
        
        
}
