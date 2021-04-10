using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    enum GameState
    {
        Intro,
        Game,
        GameLoss,
        GameWin
    }

    GameState _currentState;

    private void Start()
    {
        _currentState = GameState.Intro;
    }
}
