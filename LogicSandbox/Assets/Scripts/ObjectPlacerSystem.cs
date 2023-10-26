using System;
using UnityEngine;
using UnityEngine.Tilemaps;



public class ObjectPlacerSystem : MonoBehaviour
{

    [Header("Tiles")]
    [SerializeField] private PlaceableTilesSO _TilePalletSO;
    [SerializeField] private int _SelectedTileIndex = 0;
    [SerializeField] private GameObject _WorldTilePrefab;


    public Tile SelectedTile;
    public dynamic SelectedObject;
    public int SelectedTileRotation = 0;


    [Header("Tilemaps")]
    [Space(10)]
    [SerializeField] private Tilemap _PreviewTilemap;


    // Temporary variables
    private Vector3Int _cellCords;
    private Quaternion TileRotationMatrix;


    void Start()
    {
        TileRotationMatrix = Quaternion.Euler(0f, 0f, SelectedTileRotation * 90f);
        SelectedTile = _TilePalletSO.PlaceableTilesPreview[_SelectedTileIndex];
        UpdateSelectedObject();
    }


    void Update() {
        CalculateCellCord();

        UpdateSelectedRotation();
        UpdateSelectedTile();
        UpdateSelectedObject();


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
            int palletq = _TilePalletSO.PlaceableTilesPreview.Count;
            float change = Input.mouseScrollDelta.y;
            _SelectedTileIndex += (int)change;
            _SelectedTileIndex = (_SelectedTileIndex < 0) ? _SelectedTileIndex + palletq : _SelectedTileIndex % palletq;
        }
        SelectedTile = _TilePalletSO.PlaceableTilesPreview[_SelectedTileIndex];
    }
    private void UpdateSelectedObject()
    {
        SelectedObject = LogicObjectsProperties.Objects[_SelectedTileIndex];
        SelectedObject.orientation = SelectedTileRotation;
        SelectedObject.position = ((Vector2Int)_cellCords);
    }
    private void UpdateSelectedRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SelectedTileRotation = (SelectedTileRotation - 1) % 4;
            TileRotationMatrix = Quaternion.Euler(0f, 0f, SelectedTileRotation * 90f);
        }
    }



    private void UpdatePreviewTilemap() {
        _PreviewTilemap.ClearAllTiles();
        _PreviewTilemap.SetTile(_cellCords, SelectedTile);
        _PreviewTilemap.SetTransformMatrix(_cellCords, Matrix4x4.Rotate(TileRotationMatrix));
    }

    private void PlaceObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TileSprites _objectTextures = _TilePalletSO.TilesSprites[_SelectedTileIndex];

            GameObject _gameObject = Instantiate(_WorldTilePrefab, _cellCords, TileRotationMatrix, this.transform);
            SpriteRenderer _spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = _objectTextures.Active;

            LogicMap.PlaceObject((Vector2Int)_cellCords, SelectedObject, _objectTextures, _gameObject);
        }
    }

    private void DestroyObject() {
        if (Input.GetMouseButton(1)) {
            LogicMap.RemoveObject((Vector2Int)_cellCords);
        }
    }
}
