using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;



public static class LogicObjectsProperties 
{
    static WireNetwork wNetwork = new WireNetwork(false);

    public static List<object> Objects = new List<object>()
    {
        new Wire(0 , new List<int> {0,1,2,3} , wNetwork) , // 4 connected wire
        new Wire(0 , new List<int> {1,2,3} , wNetwork) , // 3 connected wire
        new Wire(0 , new List<int> {0,2} , wNetwork) , // 2 connected wire
        new Wire(0 , new List<int> {1,2} , wNetwork) , // 2 connected wire with right angle
    };
        
        
}
