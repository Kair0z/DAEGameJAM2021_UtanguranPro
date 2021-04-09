using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBehavior : MonoBehaviour
{
    public enum PreyState
    {
        Wandering,
        Fleeing
    }

    private PreyState _state = PreyState.Wandering;

    // Start is called before the first frame update
    void Start()
    {
        _state = PreyState.Wandering;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
