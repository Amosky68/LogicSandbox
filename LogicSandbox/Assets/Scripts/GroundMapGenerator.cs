using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundMapGenerator : MonoBehaviour
{

    [SerializeField] private List<Tile> _TilesList;
    [SerializeField] private Tilemap _GroundTileMap;
    public int generatedSize = 20;


    // Start is called before the first frame update
    void Start()
    {
        GenerateTilemap();   
    }


    private void GenerateTilemap()
    {
        for (int x = -generatedSize; x <= generatedSize; x++) {
            for (int y = -generatedSize; y <= generatedSize; y++) {

                _GroundTileMap.SetTile(new Vector3Int(x, y), _TilesList[0]);
            }
        }
    }

}
