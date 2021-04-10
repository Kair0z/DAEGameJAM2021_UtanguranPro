using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(PlayerInputManager))]
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

    [SerializeField] private GameObject playerPrefab = null;

    [Header("Spawns")]
    [SerializeField] private Transform[] spawns = new Transform[4];


    [SerializeField] Cinemachine.CinemachineTargetGroup playerTargetGroup;

    private void Start()
    {
        if (!playerPrefab) return;

        PlayerInputManager inputManager = GetComponent<PlayerInputManager>();
        inputManager.playerPrefab = playerPrefab;

        _currentState = GameState.Intro;

        for (int i= 0; i < PlayerSelectManager.playersJoiningGame.Length; ++i)
        {
            bool playerJoin = PlayerSelectManager.playersJoiningGame[i];
            if (playerJoin)
            {
                PlayerInput newPlayer = inputManager.JoinPlayer(i);
                newPlayer.transform.position = spawns[i].position;
                if (playerTargetGroup) playerTargetGroup.AddMember(newPlayer.transform, 1.0f, 1.0f);
            }
        }
    }


}
