using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class TickSystem : MonoBehaviour
{

    public bool IsPaused = true;
    public float TickDelay = 0.1f;
    public int tickCount { get; private set; }


    [ReadOnly] float _currentDelay = 0f;


    void Start()
    {
        tickCount = 0;
    }



    void Update()
    {
        _currentDelay -= Time.deltaTime;
        if (_currentDelay >= TickDelay) {
            _currentDelay = 0;
            tickCount++;

            OnTickUpdate();
        }
    }

    public void OnTickUpdate()
    {

    }
}
