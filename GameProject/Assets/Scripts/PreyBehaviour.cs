using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBehaviour : MonoBehaviour
{
    private enum PreyState
    {
        Wandering,
        Fleeing
    }

    private PreyState _state = PreyState.Wandering;
    private Timer _wanderTimer = new Timer();

    private void Start()
    {
        _state = PreyState.Wandering;
        _wanderTimer.Set(5);
    }

    private void Update()
    {
        _wanderTimer.OnPing(Time.deltaTime, DetermineNewWanderTarget);
    }

    private void DetermineNewWanderTarget()
    {

    }
}
