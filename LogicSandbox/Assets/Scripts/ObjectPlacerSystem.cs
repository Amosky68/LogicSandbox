using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectPlacerSystem : MonoBehaviour
{

    [Header("Tiles")]
    [SerializeField] private PlaceableTilesSO _TilePalletSO;
    public Tile SelectedTile;
    [SerializeField] private int _SelectedTileIndex = 0;

    [Header("Tilemaps")]
    [Space(10)]
    [SerializeField] private Tilemap _PreviewTilemap;
    [SerializeField] private Tilemap _LogicTilemap;


    // Temporary variables
    private Vector3Int _cellCords;


    void Start()
    {
        UpdateSelectedTile();
    }


    void Update() {
        CalculateCellCord();

        UpdateSelectedTile();
        UpdatePreviewTilemap();

        PlaceObject();
    }


    private void CalculateCellCord()
    {
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellCords = _PreviewTilemap.WorldToCell(mouseWorldPoint);
        _cellCords = cellCords;
    }

    private void UpdateSelectedTile()
    {   
        if (Input.GetKey(KeyCode.LeftControl)) {
            float change = Input.mouseScrollDelta.y;
            _SelectedTileIndex += (int)change;
        }
        SelectedTile = _TilePalletSO.PlaceableTiles[Mathf.Abs(_SelectedTileIndex) % _TilePalletSO.PlaceableTiles.Count];
    }



    private void UpdatePreviewTilemap() {
        _PreviewTilemap.ClearAllTiles();
        _PreviewTilemap.SetTile(_cellCords, SelectedTile);
    }

    private void PlaceObject() {
        if (Input.GetMouseButton(0)) {
            _LogicTilemap.SetTile(_cellCords, SelectedTile);
        }
    }
}
