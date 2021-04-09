using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamBehaviour : MonoBehaviour
{
    public enum RamState
    {
        Idle,
        Wandering,
        Rage
    }

    private RamState _state = RamState.Idle;
    private Timer _testTimer = new Timer();

    private void Start()
    {
        _state = RamState.Wandering;
    }
}
