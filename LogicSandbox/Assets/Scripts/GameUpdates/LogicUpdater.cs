using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;



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


    void OnDrawGizmos()
    {
        Handles.Label(new Vector3(0,0,10), "Text test");

        dynamic obj;
        foreach (KeyValuePair<Vector2Int, dynamic> logicObject in LogicMap.Map)
        {
            obj = logicObject.Value;

            if (obj is Wire)
            {
                Handles.Label((Vector3Int)logicObject.Key, obj.network.Id.ToString());
            }
            else { print("OnDrawGizmos : No object at at " + logicObject.Key); }
        }
        
    }
}
