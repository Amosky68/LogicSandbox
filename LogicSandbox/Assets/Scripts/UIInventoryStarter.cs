using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UIInventoryStarter : MonoBehaviour
{

    [SerializeField] PlaceableTilesSO _placeableTiles;
    [SerializeField] GameObject _itemsHolder;
    [SerializeField] GameObject _itemPrefab;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _placeableTiles.PlaceableTilesPreview.Count; i++) {
            Tile tile = _placeableTiles.PlaceableTilesPreview[i];

            GameObject obj = Instantiate(_itemPrefab, _itemsHolder.transform);
            GameObject textureChild = obj.transform.Find("Texture").gameObject;
            textureChild.GetComponent<Image>().sprite = tile.sprite;
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }


}
