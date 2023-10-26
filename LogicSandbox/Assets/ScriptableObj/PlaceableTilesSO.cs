using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;


[System.Serializable]
public class TileSprites
{
    public Sprite Active;
    public Sprite Inactive;
}


[CreateAssetMenu(fileName = "PlaceableTilesSO", menuName = "ScriptableObjects/PlaceableTilesSO", order = 1)]
public class PlaceableTilesSO : ScriptableObject
{


    public List<Tile> PlaceableTilesPreview;
    public List<TileSprites> TilesSprites;
    
}
