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



    private void UpdatePreviewTilemap() {
        _PreviewTilemap.ClearAllTiles();
        _PreviewTilemap.SetTile(_cellCords, SelectedTile);
    }

    private void PlaceObject() {


        if (Input.GetMouseButton(0)) {
            _LogicTilemap.SetTile(_cellCords, SelectedTile);
        }



    }

    private void DestroyObject() {
        if (Input.GetMouseButton(1)) {
            _LogicTilemap.SetTile(_cellCords, null);
        }
    }
}
