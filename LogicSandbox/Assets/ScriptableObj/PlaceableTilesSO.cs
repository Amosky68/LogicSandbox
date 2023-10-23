using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class TilesTextures
{
    public Tile Active;
    public Tile Inactive;
}


[CreateAssetMenu(fileName = "PlaceableTilesSO", menuName = "ScriptableObjects/PlaceableTilesSO", order = 1)]
public class PlaceableTilesSO : ScriptableObject
{


    public List<Tile> PlaceableTiles;
    
    public List<TilesTextures> TilesTexture;
    
}
