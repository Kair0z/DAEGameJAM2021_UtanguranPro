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

    

    [Header("Spawn Players")]
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private Transform[] spawns = new Transform[4];
    [SerializeField] Cinemachine.CinemachineTargetGroup playerTargetGroup;

    [Header("Overlays")]
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private GameObject endscreenOverlay;


    private void Start()
    {
        SpawnPlayers();
        SetupOverlays();
    }
    private void SpawnPlayers()
    {
        if (!playerPrefab) return;

        PlayerInputManager inputManager = GetComponent<PlayerInputManager>();
        inputManager.playerPrefab = playerPrefab;

        _currentState = GameState.Intro;

        for (int i = 0; i < PlayerSelectManager.playersJoiningGame.Length; ++i)
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
    private void SetupOverlays()
    {
        if (pauseOverlay) pauseOverlay.SetActive(false);
        if (endscreenOverlay) endscreenOverlay.SetActive(false);
    }

}
