using System;
using UnityEngine;
using UnityEngine.Tilemaps;



public class ObjectPlacerSystem : MonoBehaviour
{

    [Header("Tiles")]
    [SerializeField] private PlaceableTilesSO _TilePalletSO;
    [SerializeField] private int _SelectedTileIndex = 0;

    public Tile SelectedTile;
    public dynamic SelectedObject;
    public int SelectedTileRotation;


    [Header("Tilemaps")]
    [Space(10)]
    [SerializeField] private Tilemap _PreviewTilemap;
    [SerializeField] private Tilemap _LogicTilemap;


    // Temporary variables
    private Vector3Int _cellCords;
    private Matrix4x4 TileRotationMatrix;


    void Start()
    {
        UpdateSelectedTile();
        UpdateSelectedObject();
        UpdateSelectedRotation();
    }


    void Update() {
        CalculateCellCord();

        UpdateSelectedTile();
        UpdateSelectedObject();
        UpdateSelectedRotation();

        UpdatePreviewTilemap();

        PlaceObject();
        DestroyObject();
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
            int palletq = _TilePalletSO.PlaceableTiles.Count;
            float change = Input.mouseScrollDelta.y;
            _SelectedTileIndex += (int)change;
            _SelectedTileIndex = (_SelectedTileIndex < 0) ? _SelectedTileIndex + palletq : _SelectedTileIndex % palletq;
        }
        SelectedTile = _TilePalletSO.PlaceableTiles[_SelectedTileIndex];
    }
    private void UpdateSelectedObject()
    {
        //SelectedObject = LogicObjectsProperties.Objects[_SelectedTileIndex];
    }
    private void UpdateSelectedRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            print("rotating");
            SelectedTileRotation = (SelectedTileRotation + 1) % 4;
            TileRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, SelectedTileRotation * 90f));
        }
    }



    private void UpdatePreviewTilemap() {
        _PreviewTilemap.ClearAllTiles();
        _PreviewTilemap.SetTile(_cellCords, SelectedTile);
        _PreviewTilemap.SetTransformMatrix(_cellCords, TileRotationMatrix);
    }

    private void PlaceObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _LogicTilemap.SetTile(_cellCords, SelectedTile);
            _LogicTilemap.SetTransformMatrix(_cellCords, TileRotationMatrix);
            LogicMap.Map.Add((Vector2Int)_cellCords, SelectedObject);

        }
    }

    private void DestroyObject() {
        if (Input.GetMouseButton(1)) {
            _LogicTilemap.SetTile(_cellCords, null);
        }
    }
}
