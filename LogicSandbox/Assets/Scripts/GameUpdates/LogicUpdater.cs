using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public static class LogicMap
{
    public static Dictionary<Vector2Int, TileSprites> TileTextureMap = new();
    public static Dictionary<Vector2Int, GameObject> GameObjectMap = new();
    public static Dictionary<Vector2Int, dynamic> Map = new();

    public static List<WireNetwork> WireNetworksMap= new();


    public static void PlaceObject(Vector2Int position, dynamic LogicObject, TileSprites ObjectTextures, GameObject gameObject)
    {
        if (LogicObject == null) { return;  }
        if (Map.ContainsKey(position)) {
            RemoveObject(position);
        }
        Map.Add(position, LogicObject);
        TileTextureMap.Add(position, ObjectTextures);
        GameObjectMap.Add(position, gameObject);
        
    }

    public static void RemoveObject(Vector2Int position)
    {
        if (Map.ContainsKey(position))
        {
            Map.Remove(position);
            TileTextureMap.Remove(position);
            GameObject gameObject;
            if (GameObjectMap.TryGetValue(position, out gameObject)) {
                GameObject.Destroy(gameObject);
            }
            GameObjectMap.Remove(position);
        }
    }




    public static dynamic GetObject(Vector2Int position) {
        dynamic obj;
        if (Map.TryGetValue(position, out obj)) {
            return obj;
        }
        return null;
    }

}



public class LogicUpdater : MonoBehaviour
{

    public List<LogicObjects> lobj = new List<LogicObjects>();



    private void Start()
    {
        TickSystem.OnTickUpdate += delegate (object sender, TickSystem.OnTickUpdateArgs ags)
        {
            OnTickUpdate();
            RenderMap();
        };

    }


    void OnTickUpdate()
    {
        Dictionary<Vector2Int, dynamic> NewMap = new Dictionary<Vector2Int, dynamic>();
        foreach (dynamic logicObject in LogicMap.Map.Values) 
        {
        }
    }


    private void RenderMap()
    {
        Vector2Int _objectPosition;
        foreach (KeyValuePair<Vector2Int, dynamic> logicObject in LogicMap.Map) {
            _objectPosition = logicObject.Key;

            TileSprites _tSprite;
            if (LogicMap.TileTextureMap.TryGetValue(_objectPosition, out _tSprite)) 
            {   Sprite _sprite = logicObject.Value.IsActive() ? _tSprite.Active : _tSprite.Inactive;

                GameObject _gameObject;
                if (LogicMap.GameObjectMap.TryGetValue(_objectPosition, out _gameObject)) {
                    SpriteRenderer spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _sprite;
                }

            }
            else { print(" No value for TileTextureMap at " + _objectPosition + LogicMap.TileTextureMap.Count);  }
        }
    }
}
