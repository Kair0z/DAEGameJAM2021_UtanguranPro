using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        foreach (PlayerInput player in PlayerSelectManager.playersJoiningGame)
        {
            Debug.Log("Player Joins game:" + player.gameObject.name);
        }
    }


}
