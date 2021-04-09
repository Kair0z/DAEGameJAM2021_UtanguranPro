using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer
{
    public void Set(float maxTime)
    {
        _timer = maxTime;
        _maxTime = maxTime;
    }

    public void OnPing(float deltaTime, Action todo, bool reset = true)
    {
        if (_maxTime < 0.0f) return; // Invalid timer not set

        _timer -= deltaTime;
        if (_timer <= 0.0f)
        {
            todo();
            if (reset) Set(_maxTime);
        }
    }

    private float _timer = 0.0f;
    private float _maxTime = -1.0f;
}
