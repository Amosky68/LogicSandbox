using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;



/*  Tick Update Steps :
 *  
 *  - Call all Update Methods on all objects of the map
 *      | put the result in the "afterupdate" variable
 *    
 *  - Reset all Networks
 *    
 *  - after update all objects of the map 
 *      | Set the object activation from afterupdate
 * 
 *  - Render all objects with the right texture
 * 
 */





public class LogicUpdater : MonoBehaviour
{
    LogicMap _LogicMap;
    public List<LogicObjects> lobj = new List<LogicObjects>();


    private void Awake()
    {
        _LogicMap = GameObject.Find("LogicMap").GetComponent<LogicMap>();
    }
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
        // Update all objects 
        foreach (dynamic logicObject in _LogicMap.GlMap.Map.Values)  {
            logicObject.OnObjectUpdate(_LogicMap.GlMap);
        }
        // Reset all the networks
        foreach (WireNetwork network in _LogicMap.GlMap.WireNetworksMap) {
            network.Deactivate();
        }
        // After update all the objects
        foreach (dynamic logicObject in _LogicMap.GlMap.Map.Values) {
            logicObject.AfterUpdate();
        }
    }

    private void RenderMap()
    {
        Vector2Int _objectPosition;
        foreach (KeyValuePair<Vector2Int, dynamic> logicObject in _LogicMap.GlMap.Map) {
            _objectPosition = logicObject.Key;

            TileSprites _tSprite;
            if (_LogicMap.GlMap.TileTextureMap.TryGetValue(_objectPosition, out _tSprite)) 
            {   Sprite _sprite = logicObject.Value.IsActive() ? _tSprite.Active : _tSprite.Inactive;

                GameObject _gameObject;
                if (_LogicMap.GlMap.GameObjectMap.TryGetValue(_objectPosition, out _gameObject)) {
                    SpriteRenderer spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = _sprite;
                }

            }
            else { print(" No value for TileTextureMap at " + _objectPosition + _LogicMap.GlMap.TileTextureMap.Count);  }
        }
    }


    // Debug Functions -----------
    public static void drawString(string text, Vector3 worldPos, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();
        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
    }

    void OnDrawGizmos()
    {
        if (_LogicMap == null) { return; }

        foreach (KeyValuePair<Vector2Int, object> logicObject in _LogicMap.GlMap.Map)
        {
            if (logicObject.Value.GetType() == typeof(LogicalWire))
            {
                LogicalWire obj = (LogicalWire)logicObject.Value;
                drawString(obj.position.ToString(), (Vector3Int)logicObject.Key + new Vector3(0.4f, 0.2f), Color.red);
                drawString(obj.network.Id.ToString(), (Vector3Int)logicObject.Key + new Vector3(0.4f, 0.5f), Color.cyan);
            }
            else { print("OnDrawGizmos : No object at at " + logicObject.Key); }
        }
        
    }
}
