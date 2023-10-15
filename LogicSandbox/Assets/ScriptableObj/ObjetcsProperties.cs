using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;



public static class LogicObjectsProperties 
{

    public static List<dynamic> Objects = new List<dynamic>()
    {
        new Wire(0 , new List<int> {0,1,2,3} ) , // 4 connected wire
        new Wire(0 , new List<int> {1,2,3} ) , // 3 connected wire
        new Wire(0 , new List<int> {0,2} ) , // 2 connected wire
        new Wire(0 , new List<int> {2,3} ) , // 2 connected wire with right angle
    };
        
        
}
